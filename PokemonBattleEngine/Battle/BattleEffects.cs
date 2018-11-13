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
        bool bUsedMove; // If true, moveused and ppreduced were already announced and should not be announced again (when targetting multiple pokemon)
        double bDamageMultiplier;
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
            PTeam team = teams[pkmn.Local ? 0 : 1];
            // Major statuses
            switch (pkmn.Status1)
            {
                case PStatus1.Burned:
                    BroadcastStatus1(pkmn, PStatus1.Burned, PStatusAction.Damage);
                    DealDamage(pkmn, (ushort)(pkmn.MaxHP / PConstants.BurnDamageDenominator), PEffectiveness.Normal, true);
                    FaintCheck(pkmn);
                    break;
                case PStatus1.Poisoned:
                    BroadcastStatus1(pkmn, PStatus1.Poisoned, PStatusAction.Damage);
                    DealDamage(pkmn, (ushort)(pkmn.MaxHP / PConstants.PoisonDamageDenominator), PEffectiveness.Normal, true);
                    FaintCheck(pkmn);
                    break;
                case PStatus1.BadlyPoisoned:
                    BroadcastStatus1(pkmn, PStatus1.BadlyPoisoned, PStatusAction.Damage);
                    DealDamage(pkmn, (ushort)(pkmn.MaxHP * pkmn.Status1Counter / PConstants.ToxicDamageDenominator), PEffectiveness.Normal, true);
                    if (FaintCheck(pkmn))
                        pkmn.Status1Counter = 0;
                    else
                        pkmn.Status1Counter++;
                    break;
            }

            // Items
            switch (pkmn.Item)
            {
                case PItem.Leftovers:
                    if (HealDamage(pkmn, (ushort)(pkmn.MaxHP / PConstants.LeftoversDenominator)))
                        BroadcastItemUsed(pkmn, PItem.Leftovers);
                    break;
            }

            // Abilities
            Ab_LimberCure(pkmn);
        }

        void UseMove(PPokemon attacker)
        {
            bUsedMove = false;
            PTeam attackerTeam = teams[attacker.Local ? 0 : 1]; // Attacker's team
            PTeam opposingTeam = teams[attacker.Local ? 1 : 0]; // Other team

            #region Targets

            PTarget selectedTarget = attacker.SelectedAction.Targets;
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
            if (attacker.Status1 == PStatus1.Asleep)
                attacker.Status1Counter++;

            bAttacker = attacker;
            bMove = attacker.SelectedAction.Move; // bMoveType gets set in BattleDamage.cs->TypeCheck()
            if (MoveCancelCheck())
                return;

            int aliveTargets = targets.Count(t => t != null);
            if (aliveTargets == 0)
            {
                BroadcastMoveUsed();
                PPReduce(attacker, bMove);
                BroadcastFail(PFailReason.Default);
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
            bDamageMultiplier = initialDamageMultiplier;
            bLandedCrit = false;

            PMoveData mData = PMoveData.Data[bMove];
            switch (mData.Effect)
            {
                case PMoveEffect.BrickBreak:
                    Ef_BrickBreak();
                    break;
                case PMoveEffect.Burn:
                    TryForceStatus1(PStatus1.Burned);
                    break;
                case PMoveEffect.ChangeTarget_ACC:
                    ChangeTargetStat(PStat.Accuracy, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_ATK:
                    ChangeTargetStat(PStat.Attack, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_DEF:
                    ChangeTargetStat(PStat.Defense, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_EVA:
                    ChangeTargetStat(PStat.Evasion, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_SPDEF:
                    ChangeTargetStat(PStat.SpDefense, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeTarget_SPE:
                    ChangeTargetStat(PStat.Speed, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_ATK:
                    ChangeUserStat(PStat.Attack, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_DEF:
                    ChangeUserStat(PStat.Defense, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_EVA:
                    ChangeUserStat(PStat.Evasion, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_SPATK:
                    ChangeUserStat(PStat.SpAttack, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_SPDEF:
                    ChangeUserStat(PStat.SpDefense, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.ChangeUser_SPE:
                    ChangeUserStat(PStat.Speed, (sbyte)mData.EffectParam);
                    break;
                case PMoveEffect.Confuse:
                    TryForceStatus2(PStatus2.Confused);
                    break;
                case PMoveEffect.Dive:
                    Ef_Dive();
                    break;
                case PMoveEffect.Fail:
                case PMoveEffect.Transform: // TODO
                    if (!bUsedMove)
                    {
                        bUsedMove = true;
                        BroadcastMoveUsed();
                        PPReduce(bAttacker, bMove);
                        BroadcastFail(PFailReason.Default);
                    }
                    break;
                case PMoveEffect.Growth:
                    Ef_Growth();
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
                    {
                        ChangeUserStat(PStat.SpDefense, -1);
                    }
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_SPATK_By2:
                    HitAndMaybeChangeUserStat(PStat.SpAttack, -2, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_SPE_By1:
                    HitAndMaybeChangeUserStat(PStat.Speed, -1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1:
                    if (HitAndMaybeChangeUserStat(PStat.Speed, -1, mData.EffectParam))
                    {
                        ChangeUserStat(PStat.Defense, -1);
                        ChangeUserStat(PStat.SpDefense, -1);
                    }
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
                case PMoveEffect.LightScreen:
                    Ef_LightScreen();
                    break;
                case PMoveEffect.LowerTarget_ATK_DEF_By1:
                    if (ChangeTargetStat(PStat.Attack, -1))
                    {
                        ChangeTargetStat(PStat.Defense, -1);
                    }
                    break;
                case PMoveEffect.LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2:
                    ChangeUserStat(PStat.Defense, -1);
                    ChangeUserStat(PStat.SpDefense, -1);
                    ChangeUserStat(PStat.Attack, +2);
                    ChangeUserStat(PStat.SpAttack, +2);
                    ChangeUserStat(PStat.Speed, +2);
                    break;
                case PMoveEffect.Moonlight:
                    Ef_Moonlight();
                    break;
                case PMoveEffect.Paralyze:
                    TryForceStatus1(PStatus1.Paralyzed);
                    break;
                case PMoveEffect.Poison:
                    TryForceStatus1(PStatus1.Poisoned);
                    break;
                case PMoveEffect.Protect:
                    Ef_Protect();
                    break;
                case PMoveEffect.RainDance:
                    Ef_RainDance();
                    break;
                case PMoveEffect.RaiseUser_ATK_ACC_By1:
                    ChangeUserStat(PStat.Attack, +1);
                    ChangeUserStat(PStat.Accuracy, +1);
                    break;
                case PMoveEffect.RaiseUser_ATK_DEF_By1:
                    ChangeUserStat(PStat.Attack, +1);
                    ChangeUserStat(PStat.Defense, +1);
                    break;
                case PMoveEffect.RaiseUser_ATK_DEF_ACC_By1:
                    ChangeUserStat(PStat.Attack, +1);
                    ChangeUserStat(PStat.Defense, +1);
                    ChangeUserStat(PStat.Accuracy, +1);
                    break;
                case PMoveEffect.RaiseUser_ATK_SPATK_By1:
                    ChangeUserStat(PStat.Attack, +1);
                    ChangeUserStat(PStat.SpAttack, +1);
                    break;
                case PMoveEffect.RaiseUser_ATK_SPE_By1:
                    ChangeUserStat(PStat.Attack, +1);
                    ChangeUserStat(PStat.Speed, +1);
                    break;
                case PMoveEffect.RaiseUser_DEF_SPDEF_By1:
                    ChangeUserStat(PStat.Defense, +1);
                    ChangeUserStat(PStat.SpDefense, +1);
                    break;
                case PMoveEffect.RaiseUser_SPATK_SPDEF_By1:
                    ChangeUserStat(PStat.SpAttack, +1);
                    ChangeUserStat(PStat.SpDefense, +1);
                    break;
                case PMoveEffect.RaiseUser_SPATK_SPDEF_SPE_By1:
                    ChangeUserStat(PStat.SpAttack, +1);
                    ChangeUserStat(PStat.SpDefense, +1);
                    ChangeUserStat(PStat.Speed, +1);
                    break;
                case PMoveEffect.RaiseUser_SPE_By2_ATK_By1:
                    ChangeUserStat(PStat.Speed, +2);
                    ChangeUserStat(PStat.Attack, +1);
                    break;
                case PMoveEffect.Reflect:
                    Ef_Reflect();
                    break;
                case PMoveEffect.Sleep:
                    TryForceStatus1(PStatus1.Asleep);
                    break;
                case PMoveEffect.Substitute:
                    TryForceStatus2(PStatus2.Substitute);
                    break;
                case PMoveEffect.SunnyDay:
                    Ef_SunnyDay();
                    break;
                case PMoveEffect.Toxic:
                    TryForceStatus1(PStatus1.BadlyPoisoned);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mData.Effect), $"Invalid move effect: {mData.Effect}");
            }
        }

        // Returns true if an attack gets cancelled
        // Broadcasts status ending events & status causing immobility events
        bool MoveCancelCheck()
        {
            PMoveData mData = PMoveData.Data[bMove];

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
                    if (bAttacker.Status1Counter > bAttacker.SleepTurns)
                    {
                        bAttacker.Status1 = PStatus1.None;
                        bAttacker.Status1Counter = bAttacker.SleepTurns = 0;
                        BroadcastStatus1(bAttacker, PStatus1.Asleep, PStatusAction.Ended);
                    }
                    else
                    {
                        BroadcastStatus1(bAttacker, PStatus1.Asleep, PStatusAction.Activated);
                        return true;
                    }
                    break;
                case PStatus1.Frozen:
                    // Some moves always defrost the user, but if they don't, there is a 20% chance to thaw out
                    if (mData.Flags.HasFlag(PMoveFlag.DefrostsUser) || PUtils.ApplyChance(20, 100))
                    {
                        bAttacker.Status1 = PStatus1.None;
                        BroadcastStatus1(bAttacker, PStatus1.Frozen, PStatusAction.Ended);
                    }
                    else
                    {
                        BroadcastStatus1(bAttacker, PStatus1.Frozen, PStatusAction.Activated);
                        return true;
                    }
                    break;
                case PStatus1.Paralyzed:
                    // 25% chance to be unable to move
                    if (PUtils.ApplyChance(25, 100))
                    {
                        BroadcastStatus1(bAttacker, PStatus1.Paralyzed, PStatusAction.Activated);
                        return true;
                    }
                    break;
            }

            // Minor statuses
            if (bAttacker.Status2.HasFlag(PStatus2.Confused))
            {
                // Check if we snap out of confusion
                if (bAttacker.ConfusionCounter > bAttacker.ConfusionTurns)
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
                        DealDamage(bAttacker, CalculateDamage(bAttacker, bAttacker, 40, PMoveCategory.Physical, true), PEffectiveness.Normal, true);
                        BroadcastStatus2(bAttacker, PStatus2.Confused, PStatusAction.Damage);
                        FaintCheck(bAttacker);
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

            // Hitting underwater opponents
            if (bDefender.Status2.HasFlag(PStatus2.Underwater))
            {
                if (mData.Flags.HasFlag(PMoveFlag.HitsUnderwater))
                    bDamageMultiplier *= 2;
                else
                    goto miss;
            }

            if (mData.Accuracy == 0 // Always-hit moves
                || PUtils.ApplyChance(mData.Accuracy, 100) // Got lucky and landed a hit
                )
                return false;

            miss:
            BroadcastMiss();
            return true;
        }

        // Returns false if no damage was done (ineffective)
        // Broadcasts the hp changing, effectiveness, substitute
        bool DealDamage(PPokemon pkmn, ushort hp, PEffectiveness effectiveness, bool ignoreSubstitute)
        {
            // TODO: Return how much damage was done so hp drain moves can know
            if (effectiveness == PEffectiveness.Ineffective)
            {
                BroadcastEffectiveness(pkmn, effectiveness);
                return false;
            }

            if (!ignoreSubstitute && pkmn.Status2.HasFlag(PStatus2.Substitute))
            {
                pkmn.SubstituteHP = (ushort)Math.Max(0, pkmn.SubstituteHP - Math.Max((ushort)1, hp)); // Always lose at least 1 HP
                BroadcastStatus2(pkmn, PStatus2.Substitute, PStatusAction.Damage);
                BroadcastEffectiveness(pkmn, effectiveness);
                if (pkmn.SubstituteHP == 0)
                {
                    pkmn.Status2 &= ~PStatus2.Substitute;
                    BroadcastStatus2(pkmn, PStatus2.Substitute, PStatusAction.Ended);
                }
            }
            else
            {
                ushort oldHP = pkmn.HP;
                pkmn.HP = (ushort)Math.Max(0, pkmn.HP - Math.Max((ushort)1, hp)); // Always lose at least 1 HP
                int damageAmt = oldHP - pkmn.HP;
                BroadcastHPChanged(pkmn, -damageAmt);
                BroadcastEffectiveness(pkmn, effectiveness);
            }
            return true;
        }

        // Returns true if it healed
        // Broadcasts the event if it healed
        bool HealDamage(PPokemon pkmn, ushort hp)
        {
            ushort oldHP = pkmn.HP;
            pkmn.HP = (ushort)Math.Min(pkmn.MaxHP, pkmn.HP + Math.Max((ushort)1, hp)); // Always try to heal at least 1 HP
            int healAmt = pkmn.HP - oldHP;
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
                BroadcastStatus1(pkmn, PStatus1.Paralyzed, PStatusAction.Cured);
            }
        }

        // Broadcasts the event
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
        bool FaintCheck(PPokemon pkmn)
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
            // Cannot change status of a target behind a substitute
            if (bDefender.Status1 != PStatus1.None
                || bDefender.Status2.HasFlag(PStatus2.Substitute))
            {
                if (tryingToForce)
                    BroadcastFail(PFailReason.Default);
                return false;
            }

            PPokemonData pData = PPokemonData.Data[bDefender.Shell.Species];

            // A Pokémon with Limber cannot be paralyzed unless the attacker has mold breaker
            if (status == PStatus1.Paralyzed && bDefender.Ability == PAbility.Limber)
            {
                if (tryingToForce)
                {
                    BroadcastLimber(bDefender, true);
                    BroadcastEffectiveness(bDefender, PEffectiveness.Ineffective);
                }
                return false;
            }

            // An Ice type pokemon cannot be Frozen
            if (status == PStatus1.Frozen && pData.HasType(PType.Ice))
            {
                if (tryingToForce)
                    BroadcastEffectiveness(bDefender, PEffectiveness.Ineffective);
                return false;
            }
            // A Fire type pokemon cannot be burned
            if (status == PStatus1.Burned && pData.HasType(PType.Fire))
            {
                if (tryingToForce)
                    BroadcastEffectiveness(bDefender, PEffectiveness.Ineffective);
                return false;
            }
            // A Poison or Steel type pokemon cannot be poisoned or badly poisoned
            if ((status == PStatus1.BadlyPoisoned || status == PStatus1.Poisoned) && (pData.HasType(PType.Poison) || pData.HasType(PType.Steel)))
            {
                if (tryingToForce)
                    BroadcastEffectiveness(bDefender, PEffectiveness.Ineffective);
                return false;
            }


            bDefender.Status1 = status;
            // Start toxic counter
            if (status == PStatus1.BadlyPoisoned)
                bDefender.Status1Counter = 1;
            // Set sleep length
            if (status == PStatus1.Asleep)
                bDefender.SleepTurns = (byte)PUtils.RNG.Next(PConstants.SleepMinTurns, PConstants.SleepMaxTurns + 1);

            BroadcastStatus1(bDefender, status, PStatusAction.Added);

            return true;
        }
        // Returns true if the status was applied
        // Broadcasts the change if applied and required
        // "tryingToForce" being true will broadcast failing
        bool ApplyStatus2IfPossible(PStatus2 status, bool tryingToForce)
        {
            switch (status)
            {
                case PStatus2.Confused:
                    if (!bDefender.Status2.HasFlag(PStatus2.Confused)
                        && !bDefender.Status2.HasFlag(PStatus2.Substitute))
                    {
                        bDefender.Status2 |= PStatus2.Confused;
                        bDefender.ConfusionTurns = (byte)PUtils.RNG.Next(PConstants.ConfusionMinTurns, PConstants.ConfusionMaxTurns + 1);
                        BroadcastStatus2(bDefender, PStatus2.Confused, PStatusAction.Added);
                        return true;
                    }
                    break;
                case PStatus2.Flinching:
                    if (!bDefender.Status2.HasFlag(PStatus2.Substitute))
                    {
                        bDefender.Status2 |= status;
                        return true;
                    }
                    break;
                case PStatus2.Substitute:
                    {
                        ushort hpRequired = (ushort)(bAttacker.MaxHP / 4);
                        if (!bAttacker.Status2.HasFlag(PStatus2.Substitute) && hpRequired > 0 && bAttacker.HP > hpRequired)
                        {
                            DealDamage(bAttacker, hpRequired, PEffectiveness.Normal, true);
                            bAttacker.Status2 |= PStatus2.Substitute;
                            bAttacker.SubstituteHP = hpRequired;
                            BroadcastStatus2(bAttacker, PStatus2.Substitute, PStatusAction.Added);
                            return true;
                        }
                    }
                    break;
            }
            if (tryingToForce)
                BroadcastFail(PFailReason.Default);
            return false;
        }

        // Returns false if the attack failed to hit or the defender fainted
        bool Ef_Hit()
        {
            if (!bUsedMove)
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bAttacker, bMove);
            }
            if (AccuracyCheck())
                return false;
            // CritCheck();
            PEffectiveness effectiveness = TypeCheck(bAttacker, bDefender);
            if (!DealDamage(bDefender, (ushort)(CalculateDamage() * bDamageMultiplier), effectiveness, false))
                return false;
            if (bLandedCrit)
                BroadcastCrit();
            if (FaintCheck(bDefender))
                return false;
            return true;
        }
        bool Ef_Hit__MaybeConfuse(int chance)
        {
            bool behindSubstitute = bDefender.Status2.HasFlag(PStatus2.Substitute);
            if (!Ef_Hit())
                return false;
            if (behindSubstitute || !PUtils.ApplyChance(chance, 100))
                return false;
            if (!ApplyStatus2IfPossible(PStatus2.Confused, false))
                return false;
            return true;
        }
        bool Ef_Hit__MaybeFlinch(int chance)
        {
            bool behindSubstitute = bDefender.Status2.HasFlag(PStatus2.Substitute);
            if (!Ef_Hit())
                return false;
            if (behindSubstitute || !PUtils.ApplyChance(chance, 100))
                return false;
            if (!ApplyStatus2IfPossible(PStatus2.Flinching, false))
                return false;
            return true;
        }

        bool TryForceStatus1(PStatus1 status)
        {
            if (!bUsedMove)
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bAttacker, bMove);
            }
            if (AccuracyCheck())
                return false;
            PEffectiveness effectiveness = TypeCheck(bAttacker, bDefender);
            if (effectiveness == PEffectiveness.Ineffective) // Paralysis, Normalize
            {
                BroadcastEffectiveness(bDefender, effectiveness);
                return false;
            }
            if (!ApplyStatus1IfPossible(status, true))
                return false;
            return true;
        }
        bool TryForceStatus2(PStatus2 status)
        {
            if (!bUsedMove)
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bAttacker, bMove);
            }
            if (AccuracyCheck())
                return false;
            if (!ApplyStatus2IfPossible(status, true))
                return false;
            return true;
        }
        bool HitAndMaybeApplyStatus1(PStatus1 status, int chance)
        {
            bool behindSubstitute = bDefender.Status2.HasFlag(PStatus2.Substitute);
            if (!Ef_Hit())
                return false;
            if (behindSubstitute || !PUtils.ApplyChance(chance, 100))
                return false;
            if (!ApplyStatus1IfPossible(status, false))
                return false;
            return true;
        }

        bool ChangeTargetStat(PStat stat, sbyte change)
        {
            if (!bUsedMove)
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bAttacker, bMove);
            }
            if (bDefender.Status2.HasFlag(PStatus2.Substitute))
            {
                BroadcastFail(PFailReason.Default);
                return false;
            }
            else
            {
                ApplyStatChange(bDefender, stat, change);
                return true;
            }
        }
        bool ChangeUserStat(PStat stat, sbyte change)
        {
            if (!bUsedMove)
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bAttacker, bMove);
            }
            ApplyStatChange(bAttacker, stat, change);
            return true;
        }
        bool HitAndMaybeChangeTargetStat(PStat stat, sbyte change, int chance)
        {
            bool behindSubstitute = bDefender.Status2.HasFlag(PStatus2.Substitute);
            if (!Ef_Hit())
                return false;
            if (behindSubstitute || !PUtils.ApplyChance(chance, 100))
                return false;
            ApplyStatChange(bDefender, stat, change);
            return true;
        }
        bool HitAndMaybeChangeUserStat(PStat stat, sbyte change, int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance, 100))
                return false;
            ApplyStatChange(bAttacker, stat, change);
            return true;
        }

        bool Ef_Protect()
        {
            BroadcastMoveUsed();
            PPReduce(bAttacker, bMove);
            // TODO: If the user goes last, fail
            ushort chance = ushort.MaxValue;
            for (int i = 0; i < bAttacker.ProtectCounter; i++)
                chance /= 2;
            if (!PUtils.ApplyChance(chance, ushort.MaxValue))
            {
                bAttacker.ProtectCounter = 0;
                BroadcastFail(PFailReason.Default);
                return false;
            }
            bAttacker.ProtectCounter++;
            bAttacker.Status2 |= PStatus2.Protected;
            BroadcastStatus2(bAttacker, PStatus2.Protected, PStatusAction.Added);
            return true;
        }
        bool Ef_Reflect()
        {
            BroadcastMoveUsed();
            PPReduce(bAttacker, bMove);
            PTeam team = teams[bAttacker.Local ? 0 : 1];
            if (team.ReflectCount > 0)
            {
                BroadcastFail(PFailReason.Default);
                return false;
            }
            team.ReflectCount = (byte)(PConstants.ReflectLightScreenTurns + (bAttacker.Item == PItem.LightClay ? PConstants.LightClayTurnExtension : 0));
            BroadcastReflectLightScreen(team.Local, true, PReflectLightScreenAction.Added);
            return true;
        }
        bool Ef_LightScreen()
        {
            BroadcastMoveUsed();
            PPReduce(bAttacker, bMove);
            PTeam team = teams[bAttacker.Local ? 0 : 1];
            if (team.LightScreenCount > 0)
            {
                BroadcastFail(PFailReason.Default);
                return false;
            }
            team.LightScreenCount = (byte)(PConstants.ReflectLightScreenTurns + (bAttacker.Item == PItem.LightClay ? PConstants.LightClayTurnExtension : 0));
            BroadcastReflectLightScreen(team.Local, false, PReflectLightScreenAction.Added);
            return true;
        }
        bool Ef_BrickBreak()
        {
            BroadcastMoveUsed();
            PPReduce(bAttacker, bMove);
            if (AccuracyCheck())
                return false;
            // CritCheck();
            PEffectiveness effectiveness = TypeCheck(bAttacker, bDefender);
            if (effectiveness == PEffectiveness.Ineffective)
            {
                BroadcastEffectiveness(bDefender, effectiveness);
                return false;
            }
            PTeam team = teams[bDefender.Local ? 0 : 1];
            if (team.ReflectCount > 0)
            {
                team.ReflectCount = 0;
                BroadcastReflectLightScreen(team.Local, true, PReflectLightScreenAction.Broke);
            }
            if (team.LightScreenCount > 0)
            {
                team.LightScreenCount = 0;
                BroadcastReflectLightScreen(team.Local, false, PReflectLightScreenAction.Broke);
            }
            DealDamage(bDefender, (ushort)(CalculateDamage() * bDamageMultiplier), effectiveness, false);
            if (bLandedCrit)
                BroadcastCrit();
            if (FaintCheck(bDefender))
                return false;
            return true;
        }
        bool Ef_Dive()
        {
            top:
            if (bAttacker.Status2.HasFlag(PStatus2.Underwater))
            {
                if (!bUsedMove)
                {
                    bUsedMove = true;
                    BroadcastMoveUsed();
                }
                bAttacker.LockedAction.Decision = PDecision.None;
                bAttacker.Status2 &= ~PStatus2.Underwater;
                BroadcastStatus2(bAttacker, PStatus2.Underwater, PStatusAction.Ended);
                return Ef_Hit();
            }
            else
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bAttacker, bMove);
                bAttacker.LockedAction = bAttacker.SelectedAction;
                bAttacker.Status2 |= PStatus2.Underwater;
                BroadcastStatus2(bAttacker, PStatus2.Underwater, PStatusAction.Added);
                if (bAttacker.Item == PItem.PowerHerb)
                {
                    bAttacker.Item = PItem.None;
                    BroadcastItemUsed(bAttacker, PItem.PowerHerb);
                    goto top;
                }
                return true;
            }
        }
        bool Ef_RainDance()
        {
            BroadcastMoveUsed();
            PPReduce(bAttacker, bMove);
            if (Weather == PWeather.Raining)
            {
                BroadcastFail(PFailReason.Default);
                return false;
            }
            Weather = PWeather.Raining;
            WeatherCounter = (byte)(PConstants.RainTurns + (bAttacker.Item == PItem.DampRock ? PConstants.DampRockTurnExtension : 0));
            BroadcastWeather(Weather, PWeatherAction.Added);
            return true;
        }
        bool Ef_SunnyDay()
        {
            BroadcastMoveUsed();
            PPReduce(bAttacker, bMove);
            if (Weather == PWeather.Sunny)
            {
                BroadcastFail(PFailReason.Default);
                return false;
            }
            Weather = PWeather.Sunny;
            WeatherCounter = (byte)(PConstants.SunTurns + (bAttacker.Item == PItem.HeatRock ? PConstants.HeatRockTurnExtension : 0));
            BroadcastWeather(Weather, PWeatherAction.Added);
            return true;
        }
        bool Ef_Growth()
        {
            sbyte change = (sbyte)(Weather == PWeather.Sunny ? +2 : +1);
            ChangeUserStat(PStat.Attack, change);
            ChangeUserStat(PStat.SpAttack, change);
            return true;
        }
        bool Ef_Moonlight()
        {
            BroadcastMoveUsed();
            PPReduce(bAttacker, bMove);
            double percentage;
            switch (Weather)
            {
                case PWeather.None: percentage = 0.5; break;
                case PWeather.Sunny: percentage = 0.66; break;
                default: percentage = 0.25; break;
            }
            ushort hp = (ushort)(bAttacker.MaxHP * percentage);
            if (!HealDamage(bAttacker, hp))
            {
                BroadcastFail(PFailReason.HPFull);
                return false;
            }            
            return true;
        }
    }
}
