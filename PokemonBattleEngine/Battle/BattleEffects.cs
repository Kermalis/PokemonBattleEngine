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
        PPokemon bAttacker, bDefender;
        PMove bMove; PType bMoveType;
        double bEffectiveness, bDamageMultiplier;
        bool bLandedCrit;

        void DoSwitchInEffects(PPokemon pkmn)
        {
            // Abilities
            Ab_LimberCure(pkmn);
        }
        void DoPreMoveEffects(PPokemon pkmn)
        {
            // Abilities
            Ab_LimberCure(pkmn);
        }
        void DoTurnEndedEffects(PPokemon pkmn)
        {
            // Major statuses
            switch (pkmn.Status1)
            {
                case PStatus1.Asleep:
                    pkmn.Status1Counter++; // TODO: Does this go after using a move, or does it go here?
                    break;
                case PStatus1.Burned:
                    BroadcastStatus1(pkmn, PStatusAction.CausedDamage);
                    DealDamage(pkmn, (ushort)(pkmn.MaxHP / PConstants.BurnDamageDenominator));
                    CheckFaint(pkmn);
                    break;
                case PStatus1.Poisoned:
                    BroadcastStatus1(pkmn, PStatusAction.CausedDamage);
                    DealDamage(pkmn, (ushort)(pkmn.MaxHP / PConstants.PoisonDamageDenominator));
                    CheckFaint(pkmn);
                    break;
                case PStatus1.BadlyPoisoned:
                    BroadcastStatus1(pkmn, PStatusAction.CausedDamage);
                    DealDamage(pkmn, (ushort)(pkmn.MaxHP * pkmn.Status1Counter / PConstants.ToxicDamageDenominator));
                    if (CheckFaint(pkmn))
                        pkmn.Status1Counter = 0;
                    else
                        pkmn.Status1Counter++;
                    break;
            }

            // Items
            switch (pkmn.Shell.Item)
            {
                case PItem.Leftovers:
                    if (HealDamage(pkmn, (ushort)(pkmn.MaxHP / PConstants.LeftoversDenominator)))
                        BroadcastItemUsed(pkmn);
                    break;
            }

            // Abilities
            Ab_LimberCure(pkmn);
        }

        void UseMove(PPokemon attacker)
        {
            PTeam attackerTeam = teams[attacker.Local ? 0 : 1]; // Attacker's team
            PTeam opposingTeam = teams[attacker.Local ? 1 : 0]; // Other team

            #region Targets

            PTarget selectedTarget = attacker.Action.Targets;
            var targets = new List<PPokemon>();
            if (selectedTarget.HasFlag(PTarget.AllyLeft))
            {
                PPokemon b = attackerTeam.BattlerAtPosition(PFieldPosition.Left);
                targets.Add(b);
            }
            if (selectedTarget.HasFlag(PTarget.AllyCenter))
            {
                PPokemon b = attackerTeam.BattlerAtPosition(PFieldPosition.Center);
                targets.Add(b);
            }
            if (selectedTarget.HasFlag(PTarget.AllyRight))
            {
                PPokemon b = attackerTeam.BattlerAtPosition(PFieldPosition.Right);
                targets.Add(b);
            }
            if (selectedTarget.HasFlag(PTarget.FoeLeft))
            {
                PPokemon b = opposingTeam.BattlerAtPosition(PFieldPosition.Left);
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
                PPokemon b = opposingTeam.BattlerAtPosition(PFieldPosition.Center);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleStyle == PBattleStyle.Triple)
                    {
                        if (attacker.FieldPosition == PFieldPosition.Left)
                        {
                            b = opposingTeam.BattlerAtPosition(PFieldPosition.Left);
                        }
                        else if (attacker.FieldPosition == PFieldPosition.Right)
                        {
                            b = opposingTeam.BattlerAtPosition(PFieldPosition.Right);
                        }
                        else // Center
                        {
                            // If left fainted but not right, choose right, and vice versa
                            PPokemon oppLeft = opposingTeam.BattlerAtPosition(PFieldPosition.Left),
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
                PPokemon b = opposingTeam.BattlerAtPosition(PFieldPosition.Right);
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

            // TODO: Find a better place for this..?
            if (attacker.Status2.HasFlag(PStatus2.Confused))
                attacker.ConfusionCounter++;

            bAttacker = attacker;
            bMove = attacker.Action.Move; // bMoveType gets set in BattleDamage.cs->TypeCheck()
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

            foreach (PPokemon target in targets)
            {
                // For example, we use growl which attacks two surrounding opponents, but one fainted
                if (target == null || target.HP < 1)
                    continue;
                bDefender = target;
                UseMoveOnDefender(initialDamageMultiplier);
            }
        }
        void UseMoveOnDefender(double initialDamageMultiplier)
        {
            bEffectiveness = 1; bDamageMultiplier = initialDamageMultiplier;
            bLandedCrit = false;

            PMoveData mData = PMoveData.Data[bMove];
            switch (mData.Effect)
            {
                case PMoveEffect.ChangeTarget_ACC:
                    ApplyStatChange(bDefender, PStat.Accuracy, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_ATK:
                    ApplyStatChange(bDefender, PStat.Attack, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_DEF:
                    ApplyStatChange(bDefender, PStat.Defense, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_EVA:
                    ApplyStatChange(bDefender, PStat.Evasion, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_SPDEF:
                    ApplyStatChange(bDefender, PStat.SpDefense, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_SPE:
                    ApplyStatChange(bDefender, PStat.Speed, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_ATK:
                    ApplyStatChange(bAttacker, PStat.Attack, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_DEF:
                    ApplyStatChange(bAttacker, PStat.Defense, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_SPATK:
                    ApplyStatChange(bAttacker, PStat.SpAttack, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_SPDEF:
                    ApplyStatChange(bAttacker, PStat.SpDefense, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_SPE:
                    ApplyStatChange(bAttacker, PStat.Speed, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.Hit:
                    Ef_Hit();
                    break;
                case PMoveEffect.Hit__MaybeBurn:
                    HitAndMaybeApplyStatus1(PStatus1.Burned, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeConfuse:
                    Ef_Hit__MaybeConfuse(mData.EffectParam);
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
                        ApplyStatChange(bAttacker, PStat.SpDefense, -1);
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
                case PMoveEffect.Hit__MaybeRaiseUser_SPE_By1:
                    HitAndMaybeChangeUserStat(PStat.Speed, +1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeToxic:
                    HitAndMaybeApplyStatus1(PStatus1.BadlyPoisoned, mData.EffectParam);
                    break;
                case PMoveEffect.LowerTarget_ATK_DEF_By1:
                    ApplyStatChange(bDefender, PStat.Attack, -1);
                    ApplyStatChange(bDefender, PStat.Defense, -1);
                    break;
                case PMoveEffect.LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2:
                    ApplyStatChange(bAttacker, PStat.Defense, -1);
                    ApplyStatChange(bAttacker, PStat.SpDefense, -1);
                    ApplyStatChange(bAttacker, PStat.Attack, +2);
                    ApplyStatChange(bAttacker, PStat.SpAttack, +2);
                    ApplyStatChange(bAttacker, PStat.Speed, +2);
                    break;
                case PMoveEffect.Protect:
                    Ef_Protect();
                    break;
                case PMoveEffect.RaiseUser_ATK_ACC_By1:
                    ApplyStatChange(bAttacker, PStat.Attack, +1);
                    ApplyStatChange(bAttacker, PStat.Accuracy, +1);
                    break;
                case PMoveEffect.RaiseUser_ATK_DEF_By1:
                    ApplyStatChange(bAttacker, PStat.Attack, +1);
                    ApplyStatChange(bAttacker, PStat.Defense, +1);
                    break;
                case PMoveEffect.RaiseUser_ATK_DEF_ACC_By1:
                    ApplyStatChange(bAttacker, PStat.Attack, +1);
                    ApplyStatChange(bAttacker, PStat.Defense, +1);
                    ApplyStatChange(bAttacker, PStat.Accuracy, +1);
                    break;
                case PMoveEffect.RaiseUser_ATK_SPATK_By1:
                    ApplyStatChange(bAttacker, PStat.Attack, +1);
                    ApplyStatChange(bAttacker, PStat.SpAttack, +1);
                    break;
                case PMoveEffect.RaiseUser_ATK_SPE_By1:
                    ApplyStatChange(bAttacker, PStat.Attack, +1);
                    ApplyStatChange(bAttacker, PStat.Speed, +1);
                    break;
                case PMoveEffect.RaiseUser_DEF_SPDEF_By1:
                    ApplyStatChange(bAttacker, PStat.Defense, +1);
                    ApplyStatChange(bAttacker, PStat.SpDefense, +1);
                    break;
                case PMoveEffect.RaiseUser_SPATK_SPDEF_By1:
                    ApplyStatChange(bAttacker, PStat.SpAttack, +1);
                    ApplyStatChange(bAttacker, PStat.SpDefense, +1);
                    break;
                case PMoveEffect.RaiseUser_SPATK_SPDEF_SPE_By1:
                    ApplyStatChange(bAttacker, PStat.SpAttack, +1);
                    ApplyStatChange(bAttacker, PStat.SpDefense, +1);
                    ApplyStatChange(bAttacker, PStat.Speed, +1);
                    break;
                case PMoveEffect.RaiseUser_SPE_By2_ATK_By1:
                    ApplyStatChange(bAttacker, PStat.Speed, +2);
                    ApplyStatChange(bAttacker, PStat.Attack, +1);
                    break;
                case PMoveEffect.Toxic:
                    TryForceStatus1(PStatus1.BadlyPoisoned);
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
            if (bAttacker.Status2.HasFlag(PStatus2.Flinching))
            {
                BroadcastStatus2(bAttacker, PStatus2.Flinching, PStatusAction.Activated);
                return true;
            }

            // Major statuses
            switch (bAttacker.Status1)
            {
                case PStatus1.Asleep:
                    // Check if we can wake up
                    if (bAttacker.Status1Counter >= bAttacker.SleepTurns)
                    {
                        bAttacker.Status1 = PStatus1.None;
                        bAttacker.Status1Counter = bAttacker.SleepTurns = 0;
                        BroadcastStatus1(bAttacker, PStatusAction.Ended);
                    }
                    else
                    {
                        BroadcastStatus1(bAttacker, PStatusAction.Activated);
                        return true;
                    }
                    break;
                case PStatus1.Frozen:
                    // 20% chance to thaw out
                    if (PUtils.ApplyChance(20, 100))
                    {
                        bAttacker.Status1 = PStatus1.None;
                        BroadcastStatus1(bAttacker, PStatusAction.Ended);
                    }
                    else
                    {
                        BroadcastStatus1(bAttacker, PStatusAction.Activated);
                        return true;
                    }
                    break;
                case PStatus1.Paralyzed:
                    // 25% chance to be unable to move
                    if (PUtils.ApplyChance(25, 100))
                    {
                        BroadcastStatus1(bAttacker, PStatusAction.Activated);
                        return true;
                    }
                    break;
            }

            // Minor statuses
            if (bAttacker.Status2.HasFlag(PStatus2.Confused))
            {
                // Check if we snap out of confusion
                if (bAttacker.ConfusionCounter >= bAttacker.ConfusionTurns)
                {
                    bAttacker.Status2 &= ~PStatus2.Confused;
                    bAttacker.ConfusionCounter = bAttacker.ConfusionTurns = 0;
                    BroadcastStatus2(bAttacker, PStatus2.Confused, PStatusAction.Ended);
                }
                else
                {
                    BroadcastStatus2(bAttacker, PStatus2.Confused, PStatusAction.Activated);
                    // 50% chance to hit itself
                    if (PUtils.ApplyChance(50, 100))
                    {
                        DealDamage(bAttacker, CalculateDamage(bAttacker, bDefender, 40, PMoveCategory.Physical));
                        BroadcastStatus2(bAttacker, PStatus2.Confused, PStatusAction.CausedDamage);
                        CheckFaint(bAttacker);
                        return true;
                    }
                }
            }

            return false;
        }

        // Returns true if an attack fails to hit
        // Broadcasts the event if it missed
        // Broadcasts protect if protect activated
        bool AccuracyCheck()
        {
            PMoveData mData = PMoveData.Data[bMove];

            if (bDefender.Status2.HasFlag(PStatus2.Protected) && mData.Flags.HasFlag(PMoveFlag.AffectedByProtect))
            {
                BroadcastStatus2(bDefender, PStatus2.Protected, PStatusAction.Activated);
                return true;
            }

            // No Guard always hits
            if (bAttacker.Ability == PAbility.NoGuard || bDefender.Ability == PAbility.NoGuard)
                return false;

            if (mData.Accuracy == 0 // Always-hit moves
                || PUtils.ApplyChance(mData.Accuracy, 100) // Got lucky and landed a hit
                )
                return false;
            BroadcastMiss();
            return true;
        }

        // Broadcasts the event
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

        void Ab_LimberCure(PPokemon pkmn)
        {
            if (pkmn.Ability == PAbility.Limber && pkmn.Status1 == PStatus1.Paralyzed)
            {
                bAttacker.Status1 = PStatus1.None;
                BroadcastLimber(pkmn, false);
                BroadcastStatus1(pkmn, PStatusAction.Cured);
            }
        }

        // Broadcasts the event
        void PPReduce()
            => PPReduce(bAttacker, bMove);
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

        // Returns true if the pokemon fainted & removes it from activeBattlers
        // Broadcasts the event if it did
        bool CheckFaint(PPokemon pkmn)
        {
            if (pkmn.HP < 1)
            {
                activeBattlers.Remove(pkmn);
                pkmn.FieldPosition = PFieldPosition.None;
                BroadcastFaint(pkmn);
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
        // "tryingToForce" being true will broadcast events such as failing or ineffective types
        bool ApplyStatus1IfPossible(PStatus1 status, bool tryingToForce)
        {
            // Cannot change status if already afflicted
            if (bDefender.Status1 != PStatus1.None)
            {
                if (tryingToForce)
                    BroadcastFail();
                return false;
            }

            PPokemonData pData = PPokemonData.Data[bDefender.Shell.Species];

            // A Pokémon with Limber cannot be paralyzed unless the attacker has mold breaker
            if (status == PStatus1.Paralyzed && bDefender.Ability == PAbility.Limber)
            {
                if (tryingToForce)
                {
                    BroadcastLimber(bDefender, true);
                    BroadcastEffectiveness(0);
                }
                return false;
            }

            // An Ice type pokemon cannot be Frozen
            if (status == PStatus1.Frozen && pData.HasType(PType.Ice))
            {
                if (tryingToForce)
                    BroadcastEffectiveness(0);
                return false;
            }
            // A Fire type pokemon cannot be burned
            if (status == PStatus1.Burned && pData.HasType(PType.Fire))
            {
                if (tryingToForce)
                    BroadcastEffectiveness(0);
                return false;
            }
            // A Poison or Steel type pokemon cannot be poisoned or badly poisoned
            if ((status == PStatus1.BadlyPoisoned || status == PStatus1.Poisoned) && (pData.HasType(PType.Poison) || pData.HasType(PType.Steel)))
            {
                if (tryingToForce)
                    BroadcastEffectiveness(0);
                return false;
            }


            bDefender.Status1 = status;
            // Start toxic counter
            if (status == PStatus1.BadlyPoisoned)
                bDefender.Status1Counter = 1;
            // Set sleep length
            if (status == PStatus1.Asleep)
                bDefender.SleepTurns = (byte)(PUtils.RNG.Next(PConstants.SleepMinTurns, PConstants.SleepMaxTurns + 1) + 1);

            BroadcastStatus1(bDefender, PStatusAction.Added);

            return true;
        }

        // Returns false if the attack failed to hit or the defender fainted
        bool Ef_Hit()
        {
            if (AccuracyCheck())
                return false;
            // CritCheck();
            if (!TypeCheck(bAttacker, bDefender))
                return false;
            DealDamage(bDefender, (ushort)(CalculateDamage() * bEffectiveness * bDamageMultiplier));
            BroadcastEffectiveness(bEffectiveness);
            BroadcastCrit();
            if (CheckFaint(bDefender))
                return false;
            return true;
        }
        bool Ef_Hit__MaybeConfuse(int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance, 100))
                return true;
            if (!bDefender.Status2.HasFlag(PStatus2.Confused))
            {
                bDefender.Status2 |= PStatus2.Confused;
                bDefender.ConfusionTurns = (byte)(PUtils.RNG.Next(PConstants.ConfusionMinTurns, PConstants.ConfusionMaxTurns + 1) + 1);
                BroadcastStatus2(bDefender, PStatus2.Confused, PStatusAction.Added);
            }
            return true;
        }
        bool Ef_Hit__MaybeFlinch(int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance, 100))
                return true;
            bDefender.Status2 |= PStatus2.Flinching;
            return true;
        }

        bool TryForceStatus1(PStatus1 status)
        {
            if (AccuracyCheck())
                return false;
            if (!ApplyStatus1IfPossible(status, true))
                return false;
            return true;
        }
        bool HitAndMaybeApplyStatus1(PStatus1 status, int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance, 100))
                return true;
            if (!ApplyStatus1IfPossible(status, false))
                return true;
            return true;
        }

        bool HitAndMaybeChangeTargetStat(PStat stat, sbyte change, int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance, 100))
                return true;
            ApplyStatChange(bDefender, stat, change);
            return true;
        }
        bool HitAndMaybeChangeUserStat(PStat stat, sbyte change, int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance, 100))
                return true;
            ApplyStatChange(bAttacker, stat, change);
            return true;
        }

        bool Ef_Protect()
        {
            // TODO: If the user goes last, fail
            ushort chance = ushort.MaxValue;
            for (int i = 0; i < bAttacker.ProtectCounter; i++)
                chance /= 2;
            if (!PUtils.ApplyChance(chance, ushort.MaxValue))
            {
                bAttacker.ProtectCounter = 0;
                BroadcastFail();
                return false;
            }
            bAttacker.ProtectCounter++;
            bAttacker.Status2 |= PStatus2.Protected;
            BroadcastStatus2(bAttacker, PStatus2.Protected, PStatusAction.Added);
            return true;
        }
    }
}
