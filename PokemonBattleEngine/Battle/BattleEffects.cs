using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        PBattlePokemon bAttacker, bDefender;
        PMove bMove; PType bMoveType;
        ushort bDamage;
        double bEffectiveness, bDamageMultiplier;
        bool bLandedCrit;

        void DoTurnEndedEffects(PBattlePokemon battler)
        {
            // TODO: Limber

            // Major statuses
            switch (battler.Mon.Status1)
            {
                case PStatus1.Burned:
                    BroadcastStatus1CausedDamage(battler.Mon);
                    DealDamage(battler.Mon, (ushort)(battler.Mon.MaxHP / PConstants.BurnDamageDenominator));
                    TryFaint(battler);
                    break;
                case PStatus1.Poisoned:
                    BroadcastStatus1CausedDamage(battler.Mon);
                    DealDamage(battler.Mon, (ushort)(battler.Mon.MaxHP / PConstants.PoisonDamageDenominator));
                    TryFaint(battler);
                    break;
                case PStatus1.BadlyPoisoned:
                    BroadcastStatus1CausedDamage(battler.Mon);
                    DealDamage(battler.Mon, (ushort)(battler.Mon.MaxHP * battler.Status1Counter / PConstants.ToxicDamageDenominator));
                    if (!TryFaint(battler))
                        battler.Status1Counter++;
                    break;
            }

            // Items
            switch (battler.Mon.Shell.Item)
            {
                case PItem.Leftovers:
                    if (HealDamage(battler.Mon, (ushort)(battler.Mon.MaxHP / PConstants.LeftoversDenominator)))
                        BroadcastItemUsed(battler.Mon);
                    break;
            }
        }

        void UseMove(PBattlePokemon attacker)
        {
            PTeam attackerTeam = teams[attacker.Mon.LocallyOwned ? 0 : 1]; // Attacker's team
            PTeam opposingTeam = teams[attacker.Mon.LocallyOwned ? 1 : 0]; // Other team

            #region Targets

            PTarget selectedTarget = attacker.SelectedAction.Targets;
            var targets = new List<PBattlePokemon>();
            if (selectedTarget.HasFlag(PTarget.AllyLeft))
            {
                PBattlePokemon b = attackerTeam.BattlerAtPosition(PFieldPosition.Left);
                targets.Add(b);
            }
            if (selectedTarget.HasFlag(PTarget.AllyCenter))
            {
                PBattlePokemon b = attackerTeam.BattlerAtPosition(PFieldPosition.Center);
                targets.Add(b);
            }
            if (selectedTarget.HasFlag(PTarget.AllyRight))
            {
                PBattlePokemon b = attackerTeam.BattlerAtPosition(PFieldPosition.Right);
                targets.Add(b);
            }
            if (selectedTarget.HasFlag(PTarget.FoeLeft))
            {
                PBattlePokemon b = opposingTeam.BattlerAtPosition(PFieldPosition.Left);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleStyle == PBattleStyle.Double)
                    {
                        b = opposingTeam.BattlerAtPosition(PFieldPosition.Right);
                    }
                    else if (BattleStyle == PBattleStyle.Triple)
                    {
                        // TODO: Center fainted as well but move can hit anyone, who gets picked?
                        b = opposingTeam.BattlerAtPosition(PFieldPosition.Center);
                    }
                }
                targets.Add(b);
            }
            if (selectedTarget.HasFlag(PTarget.FoeCenter))
            {
                PBattlePokemon b = opposingTeam.BattlerAtPosition(PFieldPosition.Center);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleStyle == PBattleStyle.Triple)
                    {
                        if (attacker.Mon.FieldPosition == PFieldPosition.Left)
                        {
                            b = opposingTeam.BattlerAtPosition(PFieldPosition.Left);
                        }
                        else if (attacker.Mon.FieldPosition == PFieldPosition.Right)
                        {
                            b = opposingTeam.BattlerAtPosition(PFieldPosition.Right);
                        }
                        else // Center
                        {
                            // If left fainted but not right, choose right, and vice versa
                            PBattlePokemon oppLeft = opposingTeam.BattlerAtPosition(PFieldPosition.Left),
                                oppRight = opposingTeam.BattlerAtPosition(PFieldPosition.Right);
                            if (oppLeft == null && oppRight != null)
                            {
                                b = oppRight;
                            }
                            else if (oppLeft != null && oppRight == null)
                            {
                                b = oppLeft;
                            }
                            // TODO: Find out if it should be random:
                            else // Both alive; randomly select left or right
                            {
                                b = PUtils.RNG.NextDouble() >= 0.5 ? oppLeft : oppRight;
                            }
                        }
                    }
                }
                targets.Add(b);
            }
            if (selectedTarget.HasFlag(PTarget.FoeRight))
            {
                PBattlePokemon b = opposingTeam.BattlerAtPosition(PFieldPosition.Right);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleStyle == PBattleStyle.Double)
                    {
                        b = opposingTeam.BattlerAtPosition(PFieldPosition.Left);
                    }
                    else if (BattleStyle == PBattleStyle.Triple)
                    {
                        // TODO: Center fainted as well but move can hit anyone, so pick far corner
                        b = opposingTeam.BattlerAtPosition(PFieldPosition.Center);
                    }
                }
                targets.Add(b);
            }
            targets = targets.Distinct().ToList(); // Remove duplicate targets

            #endregion

            bAttacker = attacker;
            bMove = bAttacker.SelectedAction.Move; // bMoveType gets set in BattleDamage.cs->TypeCheck()
            if (AttackCancelCheck())
                return;
            BroadcastMoveUsed();
            PPReduce();

            int aliveTargets = targets.Count(t => t != null);
            if (aliveTargets == 0)
            {
                BroadcastFail();
                return;
            }
            // Reduced damage if targetting multiple pokemon
            double initialDamageMultiplier = aliveTargets > 1 ? 0.75 : 1;

            foreach (PBattlePokemon target in targets)
            {
                // For example, we use growl which attacks two surrounding opponents, but one fainted
                if (target == null || target.Mon.HP < 1)
                    continue;
                bDefender = target;
                UseMoveOnDefender(initialDamageMultiplier);
            }
        }
        void UseMoveOnDefender(double initialDamageMultiplier)
        {
            bDamage = 0;
            bEffectiveness = 1; bDamageMultiplier = initialDamageMultiplier;
            bLandedCrit = false;

            PMoveData mData = PMoveData.Data[bMove];
            switch (mData.Effect)
            {
                case PMoveEffect.ChangeTarget_ACC:
                    ApplyStatChange(bDefender.Mon, PStat.Accuracy, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_ATK:
                    ApplyStatChange(bDefender.Mon, PStat.Attack, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_DEF:
                    ApplyStatChange(bDefender.Mon, PStat.Defense, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_EVA:
                    ApplyStatChange(bDefender.Mon, PStat.Evasion, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_SPDEF:
                    ApplyStatChange(bDefender.Mon, PStat.SpDefense, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_SPE:
                    ApplyStatChange(bDefender.Mon, PStat.Speed, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_ATK:
                    ApplyStatChange(bAttacker.Mon, PStat.Attack, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_DEF:
                    ApplyStatChange(bAttacker.Mon, PStat.Defense, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_SPATK:
                    ApplyStatChange(bAttacker.Mon, PStat.SpAttack, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_SPDEF:
                    ApplyStatChange(bAttacker.Mon, PStat.SpDefense, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_SPE:
                    ApplyStatChange(bAttacker.Mon, PStat.Speed, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.Hit:
                    Ef_Hit();
                    break;
                case PMoveEffect.Hit__MaybeBurn:
                    HitAndMaybeApplyStatus1(PStatus1.Burned, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeFlinch:
                    Ef_Hit__MaybeFlinch(mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeFreeze:
                    HitAndMaybeApplyStatus1(PStatus1.Frozen, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_ACC_By1:
                    HitAndMaybeChangeTargetStat(PStat.Accuracy, -1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_ATK_By1:
                    HitAndMaybeChangeTargetStat(PStat.Attack, -1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_DEF_By1:
                    HitAndMaybeChangeTargetStat(PStat.Defense, -1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_SPATK_By1:
                    HitAndMaybeChangeTargetStat(PStat.SpAttack, -1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1:
                    HitAndMaybeChangeTargetStat(PStat.SpDefense, -1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2:
                    HitAndMaybeChangeTargetStat(PStat.SpDefense, -2, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_SPE_By1:
                    HitAndMaybeChangeTargetStat(PStat.Speed, -1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeParalyze:
                    HitAndMaybeApplyStatus1(PStatus1.Paralyzed, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybePoison:
                    HitAndMaybeApplyStatus1(PStatus1.Poisoned, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1:
                    if (HitAndMaybeChangeUserStat(PStat.Defense, -1, mData.EffectParam))
                        ApplyStatChange(bAttacker.Mon, PStat.SpDefense, -1);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_SPATK_By2:
                    HitAndMaybeChangeUserStat(PStat.SpAttack, -2, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_SPE_By1:
                    HitAndMaybeChangeUserStat(PStat.Speed, -1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeRaiseUser_ATK_By1:
                    HitAndMaybeChangeUserStat(PStat.Attack, +1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeRaiseUser_DEF_By1:
                    HitAndMaybeChangeUserStat(PStat.Defense, +1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeRaiseUser_SPATK_By1:
                    HitAndMaybeChangeUserStat(PStat.SpAttack, +1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeToxic:
                    HitAndMaybeApplyStatus1(PStatus1.BadlyPoisoned, mData.EffectParam);
                    break;
                case PMoveEffect.LowerTarget_ATK_DEF_By1:
                    ApplyStatChange(bDefender.Mon, PStat.Attack, -1);
                    ApplyStatChange(bDefender.Mon, PStat.Defense, -1);
                    break;
                case PMoveEffect.LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2:
                    ApplyStatChange(bAttacker.Mon, PStat.Defense, -1);
                    ApplyStatChange(bAttacker.Mon, PStat.SpDefense, -1);
                    ApplyStatChange(bAttacker.Mon, PStat.Attack, +2);
                    ApplyStatChange(bAttacker.Mon, PStat.SpAttack, +2);
                    ApplyStatChange(bAttacker.Mon, PStat.Speed, +2);
                    break;
                case PMoveEffect.RaiseUser_ATK_DEF_By1:
                    ApplyStatChange(bAttacker.Mon, PStat.Attack, +1);
                    ApplyStatChange(bAttacker.Mon, PStat.Defense, +1);
                    break;
                case PMoveEffect.RaiseUser_ATK_SPE_By1:
                    ApplyStatChange(bAttacker.Mon, PStat.Attack, +1);
                    ApplyStatChange(bAttacker.Mon, PStat.Speed, +1);
                    break;
                case PMoveEffect.RaiseUser_DEF_SPDEF_By1:
                    ApplyStatChange(bAttacker.Mon, PStat.Defense, +1);
                    ApplyStatChange(bAttacker.Mon, PStat.SpDefense, +1);
                    break;
                case PMoveEffect.RaiseUser_SPATK_SPDEF_By1:
                    ApplyStatChange(bAttacker.Mon, PStat.SpAttack, +1);
                    ApplyStatChange(bAttacker.Mon, PStat.SpDefense, +1);
                    break;
                case PMoveEffect.Toxic:
                    Ef_Toxic();
                    break;
                case PMoveEffect.Fail:
                case PMoveEffect.Transform: // TODO
                case PMoveEffect.Moonlight: // TODO
                    BroadcastFail();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mData.Effect), $"Invalid move effect: {mData.Effect}");
            }
        }

        // Returns true if an attack gets cancelled
        // Broadcasts status ending events & status causing immobility events
        bool AttackCancelCheck()
        {
            // Flinch first
            if (bAttacker.Mon.Status2.HasFlag(PStatus2.Flinching))
            {
                BroadcastFlinch();
                return true;
            }

            // Major statuses
            switch (bAttacker.Mon.Status1)
            {
                case PStatus1.Frozen:
                    // 20% chance to thaw out
                    if (PUtils.ApplyChance(20))
                    {
                        BroadcastStatus1Ended(bAttacker.Mon);
                        bAttacker.Mon.Status1 = PStatus1.None;
                        return false;
                    }
                    // Didn't thaw out
                    BroadcastStatus1CausedImmobility(bAttacker.Mon);
                    return true;
                case PStatus1.Paralyzed:
                    // 25% chance to be unable to move
                    if (PUtils.ApplyChance(25))
                    {
                        BroadcastStatus1CausedImmobility(bAttacker.Mon);
                        return true;
                    }
                    break;
            }

            return false;
        }

        // Returns true if an attack misses
        // Broadcasts the event if it missed
        bool AccuracyCheck()
        {
            // No Guard always hits
            if (bAttacker.Mon.Shell.Ability == PAbility.NoGuard || bDefender.Mon.Shell.Ability == PAbility.NoGuard)
                return true;

            PMoveData mData = PMoveData.Data[bMove];
            if (mData.Accuracy == 0 // Always-hit moves
                || PUtils.ApplyChance(mData.Accuracy) // Got lucky and landed a hit
                )
                return false;
            BroadcastMiss();
            return true;
        }

        // Broadcasts the event
        void DealDamage()
            => DealDamage(bDefender.Mon, (ushort)(bDamage * bEffectiveness * bDamageMultiplier));
        void DealDamage(PPokemon pkmn, ushort hp)
        {
            var oldHP = pkmn.HP;
            pkmn.HP = (ushort)Math.Max(0, pkmn.HP - Math.Max((ushort)1, hp)); // Always try to lose at least 1 HP
            var damageAmt = oldHP - pkmn.HP;
            BroadcastHPChanged(pkmn, -damageAmt);
        }

        // Returns true if it healed
        // Broadcasts the event if it healed
        bool HealDamage(PPokemon pkmn, ushort hp)
        {
            var oldHP = pkmn.HP;
            pkmn.HP = (ushort)Math.Min(pkmn.MaxHP, pkmn.HP + Math.Max((ushort)1, hp)); // Always try to heal at least 1 HP
            var healAmt = pkmn.HP - oldHP;
            if (healAmt < 1)
                return false;
            BroadcastHPChanged(pkmn, healAmt);
            return true;
        }

        // Broadcasts the event
        void PPReduce()
            => PPReduce(bAttacker.Mon, bMove);
        void PPReduce(PPokemon pkmn, PMove move)
        {
            var moveIndex = Array.IndexOf(pkmn.Shell.Moves, move);
            int amtToReduce = 1;
            // TODO: If target is not self and has pressure
            var oldPP = pkmn.PP[moveIndex];
            pkmn.PP[moveIndex] = (byte)Math.Max(0, pkmn.PP[moveIndex] - amtToReduce);
            var reduceAmt = oldPP - pkmn.PP[moveIndex];
            BroadcastPPChanged(pkmn, move, -reduceAmt);
        }

        // Returns true if the pokemon fainted
        // Broadcasts the event if it did
        bool TryFaint()
            => TryFaint(bDefender);
        bool TryFaint(PBattlePokemon pkmn)
        {
            if (pkmn.Mon.HP < 1)
            {
                activeBattlers.Remove(pkmn);
                pkmn.Mon.FieldPosition = PFieldPosition.None;
                BroadcastFaint(pkmn.Mon);
                return true;
            }
            return false;
        }

        // Does not broadcast the event
        public static void ApplyStatChange(PPkmnStatChangedPacket packet)
            => ApplyStatChange(PKnownInfo.Instance.Pokemon(packet.PokemonId), packet.Stat, packet.Change, null);
        // Broadcasts the event
        void ApplyStatChange(PPokemon pkmn, PStat stat, sbyte change)
            => ApplyStatChange(pkmn, stat, change, this);
        // Broadcasts the event if "battle" is not null
        static unsafe void ApplyStatChange(PPokemon pkmn, PStat stat, sbyte change, PBattle battle)
        {
            bool isTooMuch = false;
            fixed (sbyte* ptr = &pkmn.AttackChange)
            {
                sbyte* scPtr = ptr + (stat - PStat.Attack); // Points to the proper stat change sbyte
                if (*scPtr < -PConstants.MaxStatChange || *scPtr > PConstants.MaxStatChange)
                    isTooMuch = true;
                else
                    *scPtr = (sbyte)PUtils.Clamp(*scPtr + change, -PConstants.MaxStatChange, PConstants.MaxStatChange);
            }
            battle?.BroadcastStatChange(pkmn, stat, change, isTooMuch);
        }

        // Returns true if the status was applied
        // Broadcasts the change if applied
        bool ApplyStatus1IfPossible(PBattlePokemon pkmn, PStatus1 status)
        {
            if (pkmn.Mon.Status1 != PStatus1.None)
                return false;

            PPokemonData pData = PPokemonData.Data[pkmn.Mon.Shell.Species];

            // TODO: Limber

            // An Ice type pokemon cannot be Frozen
            if (status == PStatus1.Frozen && pData.HasType(PType.Ice))
                return false;
            // A Fire type pokemon cannot be burned
            if (status == PStatus1.Burned && pData.HasType(PType.Fire))
                return false;
            // A Poison or Steel type pokemon cannot be poisoned or badly poisoned
            if ((status == PStatus1.BadlyPoisoned || status == PStatus1.Poisoned) && (pData.HasType(PType.Poison) || pData.HasType(PType.Steel)))
                return false;


            pkmn.Mon.Status1 = status;
            // Start toxic counter
            if (status == PStatus1.BadlyPoisoned)
                pkmn.Status1Counter = 1;
            BroadcastStatus1Change(pkmn.Mon);

            return true;
        }

        bool Ef_Hit()
        {
            if (AccuracyCheck())
                return false;
            // CritCheck();
            bDamage = CalculateDamage();
            if (!TypeCheck())
                return false;
            DealDamage();
            BroadcastEffectiveness();
            BroadcastCrit();
            if (TryFaint())
                return false;
            return true;
        }
        bool Ef_Hit__MaybeFlinch(int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance))
                return false;
            bDefender.Mon.Status2 |= PStatus2.Flinching;
            return true;
        }

        bool HitAndMaybeApplyStatus1(PStatus1 status, int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance))
                return false;
            if (!ApplyStatus1IfPossible(bDefender, status))
                return false;
            return true;
        }

        bool HitAndMaybeChangeTargetStat(PStat stat, sbyte change, int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance))
                return false;
            ApplyStatChange(bDefender.Mon, stat, change);
            return true;
        }
        bool HitAndMaybeChangeUserStat(PStat stat, sbyte change, int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance))
                return false;
            ApplyStatChange(bAttacker.Mon, stat, change);
            return true;
        }

        bool Ef_Toxic()
        {
            if (AccuracyCheck())
                return false;
            if (!ApplyStatus1IfPossible(bDefender, PStatus1.BadlyPoisoned))
            {
                BroadcastFail();
                return false;
            }
            return true;
        }
    }
}
