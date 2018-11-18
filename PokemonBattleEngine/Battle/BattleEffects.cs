using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        PPokemon bUser, bTarget;
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

            // Items
            switch (pkmn.Item)
            {
                case PItem.Leftovers:
                    if (HealDamage(pkmn, (ushort)(pkmn.MaxHP / PSettings.LeftoversDenominator)))
                        BroadcastItemUsed(pkmn, PItem.Leftovers);
                    break;
            }

            // Major statuses
            switch (pkmn.Status1)
            {
                case PStatus1.Burned:
                    int damage = pkmn.MaxHP / PSettings.BurnDamageDenominator;
                    // Pokémon with the Heatproof ability take half as much damage from burns
                    if (pkmn.Ability == PAbility.Heatproof)
                        damage /= 2;
                    DealDamage(pkmn, (ushort)damage, PEffectiveness.Normal, true);
                    BroadcastStatus1(pkmn, PStatus1.Burned, PStatusAction.Damage);
                    if (FaintCheck(pkmn))
                        return;
                    break;
                case PStatus1.Poisoned:
                    DealDamage(pkmn, (ushort)(pkmn.MaxHP / PSettings.PoisonDamageDenominator), PEffectiveness.Normal, true);
                    BroadcastStatus1(pkmn, PStatus1.Poisoned, PStatusAction.Damage);
                    FaintCheck(pkmn);
                    break;
                case PStatus1.BadlyPoisoned:
                    DealDamage(pkmn, (ushort)(pkmn.MaxHP * pkmn.Status1Counter / PSettings.ToxicDamageDenominator), PEffectiveness.Normal, true);
                    BroadcastStatus1(pkmn, PStatus1.BadlyPoisoned, PStatusAction.Damage);
                    if (FaintCheck(pkmn))
                    {
                        pkmn.Status1Counter = 0;
                        return;
                    }
                    else
                    {
                        pkmn.Status1Counter++;
                        break;
                    }
            }

            // Abilities
            Ab_LimberCure(pkmn);
        }

        void UseMove(PPokemon user)
        {
            bUsedMove = false;
            PTeam attackerTeam = teams[user.Local ? 0 : 1]; // Attacker's team
            PTeam opposingTeam = teams[user.Local ? 1 : 0]; // Other team

            bUser = user;
            bMove = user.SelectedAction.FightMove; // bMoveType gets set in BattleDamage.cs->TypeCheck()
            PMoveData mData = PMoveData.Data[bMove];

            #region Targets

            PTarget selectedTarget = user.SelectedAction.FightTargets;
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
                        b = opposingTeam.BattlerAtPosition(PFieldPosition.Center);
                        // Center fainted as well and user can reach far right
                        if (b == null && (user.FieldPosition != PFieldPosition.Right || mData.Targets == PMoveTarget.SingleNotSelf))
                        {
                            b = opposingTeam.BattlerAtPosition(PFieldPosition.Right);
                        }
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
                        if (user.FieldPosition == PFieldPosition.Left)
                        {
                            b = opposingTeam.BattlerAtPosition(PFieldPosition.Right);
                            // Right fainted as well and user can reach far left
                            if (b == null && (user.FieldPosition != PFieldPosition.Left || mData.Targets == PMoveTarget.SingleNotSelf))
                            {
                                b = opposingTeam.BattlerAtPosition(PFieldPosition.Left);
                            }
                        }
                        else if (user.FieldPosition == PFieldPosition.Right)
                        {
                            b = opposingTeam.BattlerAtPosition(PFieldPosition.Left);
                            // Left fainted as well and user can reach far right
                            if (b == null && (user.FieldPosition != PFieldPosition.Right || mData.Targets == PMoveTarget.SingleNotSelf))
                            {
                                b = opposingTeam.BattlerAtPosition(PFieldPosition.Right);
                            }
                        }
                        else // Center
                        {
                            PPokemon oppLeft = opposingTeam.BattlerAtPosition(PFieldPosition.Left),
                                oppRight = opposingTeam.BattlerAtPosition(PFieldPosition.Right);
                            // Left is dead but not right
                            if (oppLeft == null && oppRight != null)
                            {
                                b = oppRight;
                            }
                            // Right is dead but not left
                            else if (oppLeft != null && oppRight == null)
                            {
                                b = oppLeft;
                            }
                            // Randomly select left or right
                            else
                            {
                                b = PUtils.RNG.NextBoolean() ? oppLeft : oppRight;
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
                        b = opposingTeam.BattlerAtPosition(PFieldPosition.Center);
                        // Center fainted as well and user can reach far left
                        if (b == null && (user.FieldPosition != PFieldPosition.Left || mData.Targets == PMoveTarget.SingleNotSelf))
                        {
                            b = opposingTeam.BattlerAtPosition(PFieldPosition.Left);
                        }
                    }
                }
                targets.Add(b);
            }
            targets = targets.Distinct().ToList(); // Remove duplicate targets

            #endregion

            if (MoveCancelCheck())
                return;

            int aliveTargets = targets.Count(t => t != null);
            if (aliveTargets == 0)
            {
                BroadcastMoveUsed();
                PPReduce(user, bMove);
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
                bTarget = target;
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
                    ChangeTargetStats(new PStat[] { PStat.Accuracy }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeTarget_ATK:
                    ChangeTargetStats(new PStat[] { PStat.Attack }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeTarget_DEF:
                    ChangeTargetStats(new PStat[] { PStat.Defense }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeTarget_EVA:
                    ChangeTargetStats(new PStat[] { PStat.Evasion }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeTarget_SPDEF:
                    ChangeTargetStats(new PStat[] { PStat.SpDefense }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeTarget_SPE:
                    ChangeTargetStats(new PStat[] { PStat.Speed }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeUser_ATK:
                    ChangeUserStats(new PStat[] { PStat.Attack }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeUser_DEF:
                    ChangeUserStats(new PStat[] { PStat.Defense }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeUser_EVA:
                    ChangeUserStats(new PStat[] { PStat.Evasion }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeUser_SPATK:
                    ChangeUserStats(new PStat[] { PStat.SpAttack }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeUser_SPDEF:
                    ChangeUserStats(new PStat[] { PStat.SpDefense }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeUser_SPE:
                    ChangeUserStats(new PStat[] { PStat.Speed }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.Confuse:
                    TryForceStatus2(PStatus2.Confused);
                    break;
                case PMoveEffect.Dive:
                    Ef_Dive();
                    break;
                case PMoveEffect.Fail:
                    Ef_Fail();
                    break;
                case PMoveEffect.FocusEnergy:
                    TryForceStatus2(PStatus2.Pumped);
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
                    HitAndMaybeChangeTargetStats(new PStat[] { PStat.Accuracy }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_ATK_By1:
                    HitAndMaybeChangeTargetStats(new PStat[] { PStat.Attack }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_DEF_By1:
                    HitAndMaybeChangeTargetStats(new PStat[] { PStat.Defense }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_SPATK_By1:
                    HitAndMaybeChangeTargetStats(new PStat[] { PStat.SpAttack }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1:
                    HitAndMaybeChangeTargetStats(new PStat[] { PStat.SpDefense }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2:
                    HitAndMaybeChangeTargetStats(new PStat[] { PStat.SpDefense }, new sbyte[] { -2 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_SPE_By1:
                    HitAndMaybeChangeTargetStats(new PStat[] { PStat.Speed }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeParalyze:
                    HitAndMaybeApplyStatus1(PStatus1.Paralyzed, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybePoison:
                    HitAndMaybeApplyStatus1(PStatus1.Poisoned, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1:
                    HitAndMaybeChangeUserStats(new PStat[] { PStat.Defense, PStat.SpDefense }, new sbyte[] { -1, -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_SPATK_By2:
                    HitAndMaybeChangeUserStats(new PStat[] { PStat.SpAttack }, new sbyte[] { -2 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_SPE_By1:
                    HitAndMaybeChangeUserStats(new PStat[] { PStat.Speed }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1:
                    HitAndMaybeChangeUserStats(new PStat[] { PStat.Speed, PStat.Defense, PStat.SpDefense }, new sbyte[] { -1, -1, -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeRaiseUser_ATK_By1:
                    HitAndMaybeChangeUserStats(new PStat[] { PStat.Attack }, new sbyte[] { +1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeRaiseUser_DEF_By1:
                    HitAndMaybeChangeUserStats(new PStat[] { PStat.Defense }, new sbyte[] { +1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeRaiseUser_SPATK_By1:
                    HitAndMaybeChangeUserStats(new PStat[] { PStat.SpAttack }, new sbyte[] { +1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeRaiseUser_SPE_By1:
                    HitAndMaybeChangeUserStats(new PStat[] { PStat.Speed }, new sbyte[] { +1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeToxic:
                    HitAndMaybeApplyStatus1(PStatus1.BadlyPoisoned, mData.EffectParam);
                    break;
                case PMoveEffect.LightScreen:
                    Ef_LightScreen();
                    break;
                case PMoveEffect.LowerTarget_ATK_DEF_By1:
                    ChangeTargetStats(new PStat[] { PStat.Attack, PStat.Defense }, new sbyte[] { -1, -1 });
                    break;
                case PMoveEffect.LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2:
                    ChangeUserStats(new PStat[] { PStat.Defense, PStat.SpDefense, PStat.Attack, PStat.SpAttack, PStat.Speed }, new sbyte[] { -1, -1, +2, +2, +2 });
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
                    TryForceStatus2(PStatus2.Protected);
                    break;
                case PMoveEffect.RainDance:
                    Ef_RainDance();
                    break;
                case PMoveEffect.RaiseUser_ATK_ACC_By1:
                    ChangeUserStats(new PStat[] { PStat.Attack, PStat.Accuracy }, new sbyte[] { +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_ATK_DEF_By1:
                    ChangeUserStats(new PStat[] { PStat.Attack, PStat.Defense }, new sbyte[] { +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_ATK_DEF_ACC_By1:
                    ChangeUserStats(new PStat[] { PStat.Attack, PStat.Defense, PStat.Accuracy }, new sbyte[] { +1, +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_ATK_SPATK_By1:
                    ChangeUserStats(new PStat[] { PStat.Attack, PStat.SpAttack }, new sbyte[] { +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_ATK_SPE_By1:
                    ChangeUserStats(new PStat[] { PStat.Attack, PStat.Speed }, new sbyte[] { +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_DEF_SPDEF_By1:
                    ChangeUserStats(new PStat[] { PStat.Defense, PStat.SpDefense }, new sbyte[] { +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_SPATK_SPDEF_By1:
                    ChangeUserStats(new PStat[] { PStat.SpAttack, PStat.SpDefense }, new sbyte[] { +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_SPATK_SPDEF_SPE_By1:
                    ChangeUserStats(new PStat[] { PStat.SpAttack, PStat.SpDefense, PStat.Speed }, new sbyte[] { +1, +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_SPE_By2_ATK_By1:
                    ChangeUserStats(new PStat[] { PStat.Speed, PStat.Attack }, new sbyte[] { +2, +1 });
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
                case PMoveEffect.Transform:
                    Ef_Transform();
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

            // Increment counters first
            if (bUser.Status2.HasFlag(PStatus2.Confused))
                bUser.ConfusionCounter++;
            if (bUser.Status1 == PStatus1.Asleep)
                bUser.Status1Counter++;

            // Flinch happens before statuses
            if (bUser.Status2.HasFlag(PStatus2.Flinching))
            {
                BroadcastStatus2(bUser, PStatus2.Flinching, PStatusAction.Activated);
                return true;
            }

            // Major statuses
            switch (bUser.Status1)
            {
                case PStatus1.Asleep:
                    // Check if we can wake up
                    if (bUser.Status1Counter > bUser.SleepTurns)
                    {
                        bUser.Status1 = PStatus1.None;
                        bUser.Status1Counter = bUser.SleepTurns = 0;
                        BroadcastStatus1(bUser, PStatus1.Asleep, PStatusAction.Ended);
                    }
                    else
                    {
                        BroadcastStatus1(bUser, PStatus1.Asleep, PStatusAction.Activated);
                        return true;
                    }
                    break;
                case PStatus1.Frozen:
                    // Some moves always defrost the user, but if they don't, there is a 20% chance to thaw out
                    if (mData.Flags.HasFlag(PMoveFlag.DefrostsUser) || PUtils.ApplyChance(20, 100))
                    {
                        bUser.Status1 = PStatus1.None;
                        BroadcastStatus1(bUser, PStatus1.Frozen, PStatusAction.Ended);
                    }
                    else
                    {
                        BroadcastStatus1(bUser, PStatus1.Frozen, PStatusAction.Activated);
                        return true;
                    }
                    break;
                case PStatus1.Paralyzed:
                    // 25% chance to be unable to move
                    if (PUtils.ApplyChance(25, 100))
                    {
                        BroadcastStatus1(bUser, PStatus1.Paralyzed, PStatusAction.Activated);
                        return true;
                    }
                    break;
            }

            // Minor statuses
            if (bUser.Status2.HasFlag(PStatus2.Confused))
            {
                // Check if we snap out of confusion
                if (bUser.ConfusionCounter > bUser.ConfusionTurns)
                {
                    bUser.Status2 &= ~PStatus2.Confused;
                    bUser.ConfusionCounter = bUser.ConfusionTurns = 0;
                    BroadcastStatus2(bUser, PStatus2.Confused, PStatusAction.Ended);
                }
                else
                {
                    BroadcastStatus2(bUser, PStatus2.Confused, PStatusAction.Activated);
                    // 50% chance to hit itself
                    if (PUtils.ApplyChance(50, 100))
                    {
                        DealDamage(bUser, CalculateDamage(bUser, bUser, 40, PMoveCategory.Physical, true, true), PEffectiveness.Normal, true);
                        BroadcastStatus2(bUser, PStatus2.Confused, PStatusAction.Damage);
                        FaintCheck(bUser);
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

            if (bTarget.Status2.HasFlag(PStatus2.Protected) && mData.Flags.HasFlag(PMoveFlag.AffectedByProtect))
            {
                BroadcastStatus2(bTarget, PStatus2.Protected, PStatusAction.Activated);
                return true;
            }

            // No Guard always hits
            if (bUser.Ability == PAbility.NoGuard || bTarget.Ability == PAbility.NoGuard)
                return false;

            // Hitting underwater opponents
            if (bTarget.Status2.HasFlag(PStatus2.Underwater))
            {
                if (mData.Flags.HasFlag(PMoveFlag.HitsUnderwater))
                    bDamageMultiplier *= 2;
                else
                    goto miss;
            }

            // Moves that always hit
            if (mData.Accuracy == 0)
                return false;

            double chance = mData.Accuracy;
            chance *= GetStatMultiplier(bUser.AccuracyChange, true) / GetStatMultiplier(bTarget.EvasionChange, true); // Accuracy & Evasion changes
            // Pokémon with the Compoundeyes ability get a 30% accuracy boost
            if (bUser.Ability == PAbility.Compoundeyes)
                chance *= 1.3;
            // Pokémon with the Hustle ability get a 20% accuracy reduction for physical moves
            if (bUser.Ability == PAbility.Hustle && mData.Category == PMoveCategory.Physical)
                chance *= 0.8;
            // Pokémon holding BrightPowder or Lax Incense get a 10% evasion boost
            if (bTarget.Item == PItem.BrightPowder)
                chance *= 0.9;
            // Pokémon holding Wide Lens get a 10% accuracy boost
            if (bUser.Item == PItem.WideLens)
                chance *= 1.1;
            // Try to hit
            if (PUtils.ApplyChance((int)chance, 100))
                return false;

            miss:
            BroadcastMiss();
            return true;
        }

        void CritCheck()
        {
            // If critical hits cannot be landed, return
            if (bTarget.Ability == PAbility.BattleArmor
                || bTarget.Ability == PAbility.ShellArmor
                )
                return;

            PMoveData mData = PMoveData.Data[bMove];
            byte stage = 0;

            if (mData.Flags.HasFlag(PMoveFlag.HighCritChance))
                stage += 1;
            if (bUser.Ability == PAbility.SuperLuck)
                stage += 1;
            if (bUser.Item == PItem.RazorClaw || bUser.Item == PItem.ScopeLens)
                stage += 1;
            if (bUser.Status2.HasFlag(PStatus2.Pumped))
                stage += 2;

            double chance;
            switch (stage)
            {
                case 0: chance = 6.25; break;
                case 1: chance = 12.5; break;
                case 2: chance = 25; break;
                case 3: chance = 33.3; break;
                default: chance = 50; break;
            }

            // Try to score a critical hit
            if (mData.Flags.HasFlag(PMoveFlag.AlwaysCrit)
                || PUtils.ApplyChance((int)(chance * 100), 100 * 100)
                )
            {
                bLandedCrit = true;
                bDamageMultiplier *= PSettings.CritMultiplier;
                if (bUser.Ability == PAbility.Sniper)
                    bDamageMultiplier *= 1.5;
            }
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
                bUser.Status1 = PStatus1.None;
                BroadcastLimber(pkmn, false);
                BroadcastStatus1(pkmn, PStatus1.Paralyzed, PStatusAction.Cured);
            }
        }

        // Broadcasts the event
        void PPReduce(PPokemon pkmn, PMove move)
        {
            var moveIndex = Array.IndexOf(pkmn.Moves, move);
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
            => ApplyStatChange(PKnownInfo.Instance.Pokemon(packet.PokemonId), packet.Stat, packet.Change, true, null);
        // Broadcasts the event
        void ApplyStatChange(PPokemon pkmn, PStat stat, sbyte change)
            => ApplyStatChange(pkmn, stat, change, false, this);
        // Broadcasts the event if "battle" is not null
        static unsafe void ApplyStatChange(PPokemon pkmn, PStat stat, sbyte change, bool ignoreSimple, PBattle battle)
        {
            if (!ignoreSimple && pkmn.Ability == PAbility.Simple)
                change *= 2;
            bool isTooMuch = false;
            fixed (sbyte* ptr = &pkmn.AttackChange)
            {
                sbyte* scPtr = ptr + (stat - PStat.Attack); // Points to the proper stat change sbyte
                if (*scPtr <= -PSettings.MaxStatChange || *scPtr >= PSettings.MaxStatChange)
                    isTooMuch = true;
                else
                    *scPtr = (sbyte)PUtils.Clamp(*scPtr + change, -PSettings.MaxStatChange, PSettings.MaxStatChange);
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
            if (bTarget.Status1 != PStatus1.None
                || bTarget.Status2.HasFlag(PStatus2.Substitute))
            {
                if (tryingToForce)
                    BroadcastFail(PFailReason.Default);
                return false;
            }

            PPokemonData pData = PPokemonData.Data[bTarget.Species];

            // A Pokémon with Limber cannot be paralyzed unless the attacker has mold breaker
            if (status == PStatus1.Paralyzed && bTarget.Ability == PAbility.Limber)
            {
                if (tryingToForce)
                {
                    BroadcastLimber(bTarget, true);
                    BroadcastEffectiveness(bTarget, PEffectiveness.Ineffective);
                }
                return false;
            }

            // An Ice type pokemon cannot be Frozen
            if (status == PStatus1.Frozen && pData.HasType(PType.Ice))
            {
                if (tryingToForce)
                    BroadcastEffectiveness(bTarget, PEffectiveness.Ineffective);
                return false;
            }
            // A Fire type pokemon cannot be burned
            if (status == PStatus1.Burned && pData.HasType(PType.Fire))
            {
                if (tryingToForce)
                    BroadcastEffectiveness(bTarget, PEffectiveness.Ineffective);
                return false;
            }
            // A Poison or Steel type pokemon cannot be poisoned or badly poisoned
            if ((status == PStatus1.BadlyPoisoned || status == PStatus1.Poisoned) && (pData.HasType(PType.Poison) || pData.HasType(PType.Steel)))
            {
                if (tryingToForce)
                    BroadcastEffectiveness(bTarget, PEffectiveness.Ineffective);
                return false;
            }


            bTarget.Status1 = status;
            // Start toxic counter
            if (status == PStatus1.BadlyPoisoned)
                bTarget.Status1Counter = 1;
            // Set sleep length
            if (status == PStatus1.Asleep)
                bTarget.SleepTurns = (byte)PUtils.RNG.Next(PSettings.SleepMinTurns, PSettings.SleepMaxTurns + 1);

            BroadcastStatus1(bTarget, status, PStatusAction.Added);

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
                    if (!bTarget.Status2.HasFlag(PStatus2.Confused)
                        && !bTarget.Status2.HasFlag(PStatus2.Substitute))
                    {
                        bTarget.Status2 |= PStatus2.Confused;
                        bTarget.ConfusionTurns = (byte)PUtils.RNG.Next(PSettings.ConfusionMinTurns, PSettings.ConfusionMaxTurns + 1);
                        BroadcastStatus2(bTarget, PStatus2.Confused, PStatusAction.Added);
                        return true;
                    }
                    break;
                case PStatus2.Flinching:
                    if (!bTarget.Status2.HasFlag(PStatus2.Substitute))
                    {
                        bTarget.Status2 |= status;
                        return true;
                    }
                    break;
                case PStatus2.Protected:
                    {
                        // TODO: If the user goes last, fail
                        ushort chance = ushort.MaxValue;
                        for (int i = 0; i < bUser.ProtectCounter; i++)
                            chance /= 2;
                        if (PUtils.ApplyChance(chance, ushort.MaxValue))
                        {
                            bUser.Status2 |= PStatus2.Protected;
                            bUser.ProtectCounter++;
                            BroadcastStatus2(bUser, PStatus2.Protected, PStatusAction.Added);
                            return true;
                        }
                        bUser.ProtectCounter = 0;
                    }
                    break;
                case PStatus2.Pumped:
                    if (!bUser.Status2.HasFlag(PStatus2.Pumped))
                    {
                        bUser.Status2 |= status;
                        BroadcastStatus2(bUser, PStatus2.Pumped, PStatusAction.Added);
                        return true;
                    }
                    break;
                case PStatus2.Substitute:
                    {
                        ushort hpRequired = (ushort)(bUser.MaxHP / 4);
                        if (!bUser.Status2.HasFlag(PStatus2.Substitute) && hpRequired > 0 && bUser.HP > hpRequired)
                        {
                            DealDamage(bUser, hpRequired, PEffectiveness.Normal, true);
                            bUser.Status2 |= PStatus2.Substitute;
                            bUser.SubstituteHP = hpRequired;
                            BroadcastStatus2(bUser, PStatus2.Substitute, PStatusAction.Added);
                            return true;
                        }
                    }
                    break;
            }
            if (tryingToForce)
                BroadcastFail(PFailReason.Default);
            return false;
        }

        // Returns false if the attack failed to hit or the target fainted
        bool Ef_Hit()
        {
            if (!bUsedMove)
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bUser, bMove);
            }
            if (AccuracyCheck())
                return false;
            CritCheck();
            PEffectiveness effectiveness = TypeCheck(bUser, bTarget);
            if (!DealDamage(bTarget, (ushort)(CalculateDamage() * bDamageMultiplier), effectiveness, false))
                return false;
            if (bLandedCrit)
                BroadcastCrit();
            if (FaintCheck(bTarget))
                return false;
            return true;
        }
        void Ef_Hit__MaybeConfuse(int chance)
        {
            bool behindSubstitute = bTarget.Status2.HasFlag(PStatus2.Substitute);
            if (!Ef_Hit())
                return;
            if (!behindSubstitute && PUtils.ApplyChance(chance, 100))
            {
                ApplyStatus2IfPossible(PStatus2.Confused, false);
            }
        }
        void Ef_Hit__MaybeFlinch(int chance)
        {
            bool behindSubstitute = bTarget.Status2.HasFlag(PStatus2.Substitute);
            if (!Ef_Hit())
                return;
            if (!behindSubstitute && PUtils.ApplyChance(chance, 100))
            {
                ApplyStatus2IfPossible(PStatus2.Flinching, false);
            }
        }

        void TryForceStatus1(PStatus1 status)
        {
            if (!bUsedMove)
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bUser, bMove);
            }
            if (AccuracyCheck())
                return;
            PEffectiveness effectiveness = TypeCheck(bUser, bTarget);
            if (effectiveness == PEffectiveness.Ineffective) // Paralysis, Normalize
            {
                BroadcastEffectiveness(bTarget, effectiveness);
            }
            else
            {
                ApplyStatus1IfPossible(status, true);
            }
        }
        void TryForceStatus2(PStatus2 status)
        {
            if (!bUsedMove)
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bUser, bMove);
            }
            if (AccuracyCheck())
                return;
            ApplyStatus2IfPossible(status, true);
        }
        void HitAndMaybeApplyStatus1(PStatus1 status, int chance)
        {
            bool behindSubstitute = bTarget.Status2.HasFlag(PStatus2.Substitute);
            if (!Ef_Hit())
                return;
            if (!behindSubstitute && PUtils.ApplyChance(chance, 100))
            {
                ApplyStatus1IfPossible(status, false);
            }
        }

        void ChangeTargetStats(PStat[] stats, sbyte[] changes)
        {
            if (!bUsedMove)
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bUser, bMove);
            }
            if (bTarget.Status2.HasFlag(PStatus2.Substitute))
            {
                BroadcastFail(PFailReason.Default);
            }
            else
            {
                for (int i = 0; i < stats.Length; i++)
                    ApplyStatChange(bTarget, stats[i], changes[i]);
            }
        }
        void ChangeUserStats(PStat[] stats, sbyte[] changes)
        {
            if (!bUsedMove)
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bUser, bMove);
            }
            for (int i = 0; i < stats.Length; i++)
            {
                ApplyStatChange(bUser, stats[i], changes[i]);
            }
        }
        void HitAndMaybeChangeTargetStats(PStat[] stats, sbyte[] changes, int chance)
        {
            bool behindSubstitute = bTarget.Status2.HasFlag(PStatus2.Substitute);
            if (!Ef_Hit())
                return;
            if (!behindSubstitute && PUtils.ApplyChance(chance, 100))
            {
                for (int i = 0; i < stats.Length; i++)
                {
                    ApplyStatChange(bTarget, stats[i], changes[i]);
                }
            }
        }
        void HitAndMaybeChangeUserStats(PStat[] stats, sbyte[] changes, int chance)
        {
            if (!bUsedMove)
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bUser, bMove);
            }
            if (AccuracyCheck())
                return;
            CritCheck();
            PEffectiveness effectiveness = TypeCheck(bUser, bTarget);
            if (!DealDamage(bTarget, (ushort)(CalculateDamage() * bDamageMultiplier), effectiveness, false))
                return;
            if (bLandedCrit)
                BroadcastCrit();
            FaintCheck(bTarget);
            if (PUtils.ApplyChance(chance, 100))
            {
                for (int i = 0; i < stats.Length; i++)
                {
                    ApplyStatChange(bUser, stats[i], changes[i]);
                }
            }
        }

        void Ef_Fail()
        {
            if (!bUsedMove)
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bUser, bMove);
                BroadcastFail(PFailReason.Default);
            }
        }
        void Ef_Reflect()
        {
            BroadcastMoveUsed();
            PPReduce(bUser, bMove);
            PTeam team = teams[bUser.Local ? 0 : 1];
            if (team.ReflectCount > 0)
            {
                BroadcastFail(PFailReason.Default);
            }
            else
            {
                team.ReflectCount = (byte)(PSettings.ReflectLightScreenTurns + (bUser.Item == PItem.LightClay ? PSettings.LightClayTurnExtension : 0));
                BroadcastReflectLightScreen(team.Local, true, PReflectLightScreenAction.Added);
            }
        }
        void Ef_LightScreen()
        {
            BroadcastMoveUsed();
            PPReduce(bUser, bMove);
            PTeam team = teams[bUser.Local ? 0 : 1];
            if (team.LightScreenCount > 0)
            {
                BroadcastFail(PFailReason.Default);
            }
            else
            {
                team.LightScreenCount = (byte)(PSettings.ReflectLightScreenTurns + (bUser.Item == PItem.LightClay ? PSettings.LightClayTurnExtension : 0));
                BroadcastReflectLightScreen(team.Local, false, PReflectLightScreenAction.Added);
            }
        }
        void Ef_BrickBreak()
        {
            BroadcastMoveUsed();
            PPReduce(bUser, bMove);
            if (AccuracyCheck())
                return;
            CritCheck();
            PEffectiveness effectiveness = TypeCheck(bUser, bTarget);
            if (effectiveness == PEffectiveness.Ineffective)
            {
                BroadcastEffectiveness(bTarget, effectiveness);
            }
            else
            {
                PTeam team = teams[bTarget.Local ? 0 : 1];
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
                DealDamage(bTarget, (ushort)(CalculateDamage() * bDamageMultiplier), effectiveness, false);
                if (bLandedCrit)
                    BroadcastCrit();
                FaintCheck(bTarget);
            }
        }
        void Ef_Dive()
        {
            top:
            if (bUser.Status2.HasFlag(PStatus2.Underwater))
            {
                if (!bUsedMove)
                {
                    bUsedMove = true;
                    BroadcastMoveUsed();
                }
                bUser.LockedAction.Decision = PDecision.None;
                bUser.Status2 &= ~PStatus2.Underwater;
                BroadcastStatus2(bUser, PStatus2.Underwater, PStatusAction.Ended);
                Ef_Hit();
            }
            else
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bUser, bMove);
                bUser.LockedAction = bUser.SelectedAction;
                bUser.Status2 |= PStatus2.Underwater;
                BroadcastStatus2(bUser, PStatus2.Underwater, PStatusAction.Added);
                if (bUser.Item == PItem.PowerHerb)
                {
                    bUser.Item = PItem.None;
                    BroadcastItemUsed(bUser, PItem.PowerHerb);
                    goto top;
                }
            }
        }
        void Ef_RainDance()
        {
            BroadcastMoveUsed();
            PPReduce(bUser, bMove);
            if (Weather == PWeather.Raining)
            {
                BroadcastFail(PFailReason.Default);
            }
            else
            {
                Weather = PWeather.Raining;
                WeatherCounter = (byte)(PSettings.RainTurns + (bUser.Item == PItem.DampRock ? PSettings.DampRockTurnExtension : 0));
                BroadcastWeather(Weather, PWeatherAction.Added);
            }
        }
        void Ef_SunnyDay()
        {
            BroadcastMoveUsed();
            PPReduce(bUser, bMove);
            if (Weather == PWeather.Sunny)
            {
                BroadcastFail(PFailReason.Default);
            }
            else
            {
                Weather = PWeather.Sunny;
                WeatherCounter = (byte)(PSettings.SunTurns + (bUser.Item == PItem.HeatRock ? PSettings.HeatRockTurnExtension : 0));
                BroadcastWeather(Weather, PWeatherAction.Added);
            }
        }
        void Ef_Growth()
        {
            sbyte change = (sbyte)(Weather == PWeather.Sunny ? +2 : +1);
            ChangeUserStats(new PStat[] { PStat.Attack, PStat.SpAttack }, new sbyte[] { change, change });
        }
        void Ef_Moonlight()
        {
            BroadcastMoveUsed();
            PPReduce(bUser, bMove);
            double percentage;
            switch (Weather)
            {
                case PWeather.None: percentage = 0.5; break;
                case PWeather.Sunny: percentage = 0.66; break;
                default: percentage = 0.25; break;
            }
            ushort hp = (ushort)(bUser.MaxHP * percentage);
            if (!HealDamage(bUser, hp))
            {
                BroadcastFail(PFailReason.HPFull);
            }
        }
        void Ef_Transform()
        {
            BroadcastMoveUsed();
            PPReduce(bUser, bMove);
            if (bUser.Status2.HasFlag(PStatus2.Transformed)
                || bTarget.Status2.HasFlag(PStatus2.Transformed)
                || bTarget.Status2.HasFlag(PStatus2.Substitute))
            {
                BroadcastFail(PFailReason.Default);
                return;
            }
            if (AccuracyCheck())
                return;
            bUser.Transform(bTarget, bTarget.Attack, bTarget.Defense, bTarget.SpAttack, bTarget.SpDefense, bTarget.Speed, bTarget.Ability, bTarget.Moves);
            BroadcastTransform();
        }
    }
}
