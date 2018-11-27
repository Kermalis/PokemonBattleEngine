using Kermalis.PokemonBattleEngine.Data;
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
            PPokemonData pData = PPokemonData.Data[pkmn.Species]; // TODO: this is only needed for types, so when types are a part of PPokemon, use them instead
            PTeam team = Teams[pkmn.Local ? 0 : 1];

            // Entry Hazards
            if (team.Status.HasFlag(PTeamStatus.Spikes) && !pData.HasType(PType.Flying) && pkmn.Ability != PAbility.Levitate)
            {
                double denominator = 10.0 - (2 * team.SpikeCount);
                ushort damage = (ushort)(pkmn.MaxHP / denominator);
                DealDamage(pkmn, pkmn, damage, PEffectiveness.Normal, true);
                BroadcastTeamStatus(team.Local, PTeamStatus.Spikes, PTeamStatusAction.Damage, pkmn.Id);
                if (FaintCheck(pkmn))
                    return;
            }
            if (team.Status.HasFlag(PTeamStatus.StealthRock))
            {
                double effectiveness = 0.125;
                effectiveness *= PPokemonData.TypeEffectiveness[(int)PType.Rock, (int)pData.Type1];
                effectiveness *= PPokemonData.TypeEffectiveness[(int)PType.Rock, (int)pData.Type2];
                ushort damage = (ushort)(pkmn.MaxHP * effectiveness);
                DealDamage(pkmn, pkmn, damage, PEffectiveness.Normal, true);
                BroadcastTeamStatus(team.Local, PTeamStatus.StealthRock, PTeamStatusAction.Damage, pkmn.Id);
                if (FaintCheck(pkmn))
                    return;
            }
            if (team.Status.HasFlag(PTeamStatus.ToxicSpikes))
            {
                // Grounded Poison types remove the toxic spikes
                if (pData.HasType(PType.Poison) && pkmn.Ability != PAbility.Levitate && !pData.HasType(PType.Flying))
                {
                    BroadcastTeamStatus(team.Local, PTeamStatus.ToxicSpikes, PTeamStatusAction.Cleared);
                }
                // Steel types and floating Pokémon don't get poisoned
                else if (pkmn.Status1 == PStatus1.None && !pData.HasType(PType.Steel) && !pData.HasType(PType.Flying) && pkmn.Ability != PAbility.Levitate)
                {
                    PStatus1 status = team.ToxicSpikeCount == 1 ? PStatus1.Poisoned : PStatus1.BadlyPoisoned;
                    pkmn.Status1 = status;
                    if (status == PStatus1.BadlyPoisoned)
                        pkmn.Status1Counter = 1;
                    BroadcastStatus1(pkmn, pkmn, status, PStatusAction.Added);
                }
            }

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
            PTeam userTeam = Teams[pkmn.Local ? 0 : 1];
            PTeam opposingTeam = Teams[pkmn.Local ? 1 : 0];

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
                    DealDamage(pkmn, pkmn, (ushort)damage, PEffectiveness.Normal, true);
                    BroadcastStatus1(pkmn, pkmn, PStatus1.Burned, PStatusAction.Damage);
                    if (FaintCheck(pkmn))
                        return;
                    break;
                case PStatus1.Poisoned:
                    DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / PSettings.PoisonDamageDenominator), PEffectiveness.Normal, true);
                    BroadcastStatus1(pkmn, pkmn, PStatus1.Poisoned, PStatusAction.Damage);
                    FaintCheck(pkmn);
                    break;
                case PStatus1.BadlyPoisoned:
                    DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP * pkmn.Status1Counter / PSettings.ToxicDamageDenominator), PEffectiveness.Normal, true);
                    BroadcastStatus1(pkmn, pkmn, PStatus1.BadlyPoisoned, PStatusAction.Damage);
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

            // Minor statuses
            if (pkmn.Status2.HasFlag(PStatus2.LeechSeed))
            {
                PPokemon seeder = opposingTeam.PokemonAtPosition(pkmn.SeededPosition);
                if (seeder != null)
                {
                    ushort hp = (ushort)(pkmn.MaxHP / PSettings.LeechSeedDenominator);
                    ushort amtDealt = DealDamage(seeder, pkmn, hp, PEffectiveness.Normal, true);
                    HealDamage(seeder, amtDealt);
                    BroadcastStatus2(seeder, pkmn, PStatus2.LeechSeed, PStatusAction.Damage);
                    if (FaintCheck(pkmn))
                        return;
                }
            }
            if (pkmn.Status2.HasFlag(PStatus2.Cursed))
            {
                DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / PSettings.CurseDenominator), PEffectiveness.Normal, true);
                BroadcastStatus2(pkmn, pkmn, PStatus2.Cursed, PStatusAction.Damage);
                if (FaintCheck(pkmn))
                    return;
            }

            // Abilities
            Ab_LimberCure(pkmn);
        }

        void UseMove(PPokemon user)
        {
            bUsedMove = false;
            bUser = user;
            bMove = user.SelectedAction.FightMove; // bMoveType gets set in BattleDamage.cs->TypeCheck()

            if (MoveCancelCheck())
                return;

            PPokemon[] targets = GetRuntimeTargets(user.SelectedAction.FightTargets, user, PMoveData.Data[bMove].Targets == PMoveTarget.SingleNotSelf);
            int aliveTargets = targets.Count(t => t != null);
            if (aliveTargets == 0)
            {
                BroadcastMoveUsed();
                PPReduce(user, bMove);
                BroadcastFail(PFailReason.NoTarget);
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
                case PMoveEffect.Curse:
                    Ef_Curse();
                    break;
                case PMoveEffect.Dig:
                    Ef_Dig();
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
                case PMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1:
                    HitAndMaybeChangeUserStats(new PStat[] { PStat.Attack, PStat.Defense, PStat.SpAttack, PStat.SpDefense, PStat.Speed }, new sbyte[] { +1, +1, +1, +1, +1 }, mData.EffectParam);
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
                case PMoveEffect.LeechSeed:
                    TryForceStatus2(PStatus2.LeechSeed);
                    break;
                case PMoveEffect.LightScreen:
                    TryForceTeamStatus(PTeamStatus.LightScreen);
                    break;
                case PMoveEffect.LowerTarget_ATK_DEF_By1:
                    ChangeTargetStats(new PStat[] { PStat.Attack, PStat.Defense }, new sbyte[] { -1, -1 });
                    break;
                case PMoveEffect.LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2:
                    ChangeUserStats(new PStat[] { PStat.Defense, PStat.SpDefense, PStat.Attack, PStat.SpAttack, PStat.Speed }, new sbyte[] { -1, -1, +2, +2, +2 });
                    break;
                case PMoveEffect.Minimize:
                    TryForceStatus2(PStatus2.Minimized);
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
                    TryForceTeamStatus(PTeamStatus.Reflect);
                    break;
                case PMoveEffect.Sleep:
                    TryForceStatus1(PStatus1.Asleep);
                    break;
                case PMoveEffect.Spikes:
                    TryForceTeamStatus(PTeamStatus.Spikes);
                    break;
                case PMoveEffect.StealthRock:
                    TryForceTeamStatus(PTeamStatus.StealthRock);
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
                case PMoveEffect.ToxicSpikes:
                    TryForceTeamStatus(PTeamStatus.ToxicSpikes);
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
                BroadcastStatus2(bUser, bUser, PStatus2.Flinching, PStatusAction.Activated);
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
                        BroadcastStatus1(bUser, bUser, PStatus1.Asleep, PStatusAction.Ended);
                    }
                    else
                    {
                        BroadcastStatus1(bUser, bUser, PStatus1.Asleep, PStatusAction.Activated);
                        return true;
                    }
                    break;
                case PStatus1.Frozen:
                    // Some moves always defrost the user, but if they don't, there is a 20% chance to thaw out
                    if (mData.Flags.HasFlag(PMoveFlag.DefrostsUser) || PUtils.ApplyChance(20, 100))
                    {
                        bUser.Status1 = PStatus1.None;
                        BroadcastStatus1(bUser, bUser, PStatus1.Frozen, PStatusAction.Ended);
                    }
                    else
                    {
                        BroadcastStatus1(bUser, bUser, PStatus1.Frozen, PStatusAction.Activated);
                        return true;
                    }
                    break;
                case PStatus1.Paralyzed:
                    // 25% chance to be unable to move
                    if (PUtils.ApplyChance(25, 100))
                    {
                        BroadcastStatus1(bUser, bUser, PStatus1.Paralyzed, PStatusAction.Activated);
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
                    BroadcastStatus2(bUser, bUser, PStatus2.Confused, PStatusAction.Ended);
                }
                else
                {
                    BroadcastStatus2(bUser, bUser, PStatus2.Confused, PStatusAction.Activated);
                    // 50% chance to hit itself
                    if (PUtils.ApplyChance(50, 100))
                    {
                        DealDamage(bUser, bUser, CalculateDamage(bUser, bUser, 40, PMoveCategory.Physical, true, true), PEffectiveness.Normal, true);
                        BroadcastStatus2(bUser, bUser, PStatus2.Confused, PStatusAction.Damage);
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
                BroadcastStatus2(bUser, bTarget, PStatus2.Protected, PStatusAction.Activated);
                return true;
            }

            // No Guard always hits
            if (bUser.Ability == PAbility.NoGuard || bTarget.Ability == PAbility.NoGuard)
                return false;

            // Hitting underground opponents
            if (bTarget.Status2.HasFlag(PStatus2.Underground) && !mData.Flags.HasFlag(PMoveFlag.HitsUnderground))
                goto miss;
            // Hitting underwater opponents
            if (bTarget.Status2.HasFlag(PStatus2.Underwater) && !mData.Flags.HasFlag(PMoveFlag.HitsUnderwater))
                goto miss;

            // Moves that always hit
            if (mData.Accuracy == 0)
                return false;

            double chance = mData.Accuracy;
            chance *= GetStatMultiplier(bUser.AccuracyChange, true) / GetStatMultiplier(bTarget.EvasionChange, true); // Accuracy & Evasion changes
            // Pokémon with Compoundeyes get a 30% Accuracy boost
            if (bUser.Ability == PAbility.Compoundeyes)
                chance *= 1.3;
            // Pokémon with Hustle get a 20% Accuracy reduction for Physical moves
            if (bUser.Ability == PAbility.Hustle && mData.Category == PMoveCategory.Physical)
                chance *= 0.8;
            // Pokémon holding a BrightPowder or Lax Incense get a 10% Evasion boost
            if (bTarget.Item == PItem.BrightPowder)
                chance *= 0.9;
            // Pokémon holding a Wide Lens get a 10% Accuracy boost
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

        // Returns amount of damage done
        // Broadcasts the hp changing, effectiveness, substitute
        ushort DealDamage(PPokemon culprit, PPokemon victim, ushort hp, PEffectiveness effectiveness, bool ignoreSubstitute)
        {
            if (effectiveness == PEffectiveness.Ineffective)
            {
                BroadcastEffectiveness(victim, effectiveness);
                return 0;
            }

            if (!ignoreSubstitute && victim.Status2.HasFlag(PStatus2.Substitute))
            {
                ushort oldHP = victim.SubstituteHP;
                victim.SubstituteHP = (ushort)Math.Max(0, victim.SubstituteHP - Math.Max((ushort)1, hp)); // Always lose at least 1 HP
                ushort damageAmt = (ushort)(oldHP - victim.SubstituteHP);
                BroadcastStatus2(culprit, victim, PStatus2.Substitute, PStatusAction.Damage);
                BroadcastEffectiveness(victim, effectiveness);
                if (victim.SubstituteHP == 0)
                {
                    victim.Status2 &= ~PStatus2.Substitute;
                    BroadcastStatus2(culprit, victim, PStatus2.Substitute, PStatusAction.Ended);
                }
                return damageAmt;
            }
            else
            {
                ushort oldHP = victim.HP;
                victim.HP = (ushort)Math.Max(0, victim.HP - Math.Max((ushort)1, hp)); // Always lose at least 1 HP
                ushort damageAmt = (ushort)(oldHP - victim.HP);
                BroadcastHPChanged(victim, -damageAmt);
                BroadcastEffectiveness(victim, effectiveness);
                return damageAmt;
            }
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
                BroadcastStatus1(pkmn, pkmn, PStatus1.Paralyzed, PStatusAction.Cured);
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

        // Returns true if the Pokémon fainted & removes it from activeBattlers
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

        // Broadcasts the event
        void ApplyStatChange(PPokemon pkmn, PStat stat, sbyte change)
            => ApplyStatChange(pkmn, stat, change, false, this);
        // Pass in the battle to broadcast the event. If you do not want to broadcast it, just pass in null
        public static unsafe void ApplyStatChange(PPokemon pkmn, PStat stat, sbyte change, bool ignoreSimple, PBattle battle)
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

            BroadcastStatus1(bUser, bTarget, status, PStatusAction.Added);

            return true;
        }
        // Returns true if the status was applied
        // Broadcasts the change if applied and required
        // "tryingToForce" being true will broadcast failing
        bool ApplyStatus2IfPossible(PStatus2 status, bool tryingToForce)
        {
            PPokemonData pData = PPokemonData.Data[bTarget.Species];

            switch (status)
            {
                case PStatus2.Confused:
                    if (!bTarget.Status2.HasFlag(PStatus2.Confused)
                        && !bTarget.Status2.HasFlag(PStatus2.Substitute))
                    {
                        bTarget.Status2 |= PStatus2.Confused;
                        bTarget.ConfusionTurns = (byte)PUtils.RNG.Next(PSettings.ConfusionMinTurns, PSettings.ConfusionMaxTurns + 1);
                        BroadcastStatus2(bUser, bTarget, PStatus2.Confused, PStatusAction.Added);
                        return true;
                    }
                    break;
                case PStatus2.Cursed:
                    if (!bTarget.Status2.HasFlag(PStatus2.Cursed))
                    {
                        bTarget.Status2 |= PStatus2.Cursed;
                        BroadcastStatus2(bUser, bTarget, PStatus2.Cursed, PStatusAction.Added);
                        DealDamage(bUser, bUser, (ushort)(bUser.MaxHP / 2), PEffectiveness.Normal, true);
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
                case PStatus2.LeechSeed:
                    if (!bTarget.Status2.HasFlag(PStatus2.LeechSeed)
                        && !bTarget.Status2.HasFlag(PStatus2.Substitute)
                        && !pData.HasType(PType.Grass))
                    {
                        bTarget.Status2 |= PStatus2.LeechSeed;
                        bTarget.SeededPosition = bUser.FieldPosition;
                        BroadcastStatus2(bUser, bTarget, PStatus2.LeechSeed, PStatusAction.Added);
                        return true;
                    }
                    break;
                case PStatus2.Minimized:
                    bUser.Status2 |= PStatus2.Minimized;
                    BroadcastStatus2(bUser, bUser, PStatus2.Minimized, PStatusAction.Added);
                    ApplyStatChange(bUser, PStat.Evasion, +2);
                    return true;
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
                            BroadcastStatus2(bUser, bUser, PStatus2.Protected, PStatusAction.Added);
                            return true;
                        }
                        bUser.ProtectCounter = 0;
                    }
                    break;
                case PStatus2.Pumped:
                    if (!bUser.Status2.HasFlag(PStatus2.Pumped))
                    {
                        bUser.Status2 |= status;
                        BroadcastStatus2(bUser, bUser, PStatus2.Pumped, PStatusAction.Added);
                        return true;
                    }
                    break;
                case PStatus2.Substitute:
                    {
                        ushort hpRequired = (ushort)(bUser.MaxHP / 4);
                        if (!bUser.Status2.HasFlag(PStatus2.Substitute) && hpRequired > 0 && bUser.HP > hpRequired)
                        {
                            DealDamage(bUser, bUser, hpRequired, PEffectiveness.Normal, true);
                            bUser.Status2 |= PStatus2.Substitute;
                            bUser.SubstituteHP = hpRequired;
                            BroadcastStatus2(bUser, bUser, PStatus2.Substitute, PStatusAction.Added);
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
            if (DealDamage(bUser, bTarget, (ushort)(CalculateDamage() * bDamageMultiplier), effectiveness, false) == 0)
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
        void TryForceTeamStatus(PTeamStatus status)
        {
            BroadcastMoveUsed();
            PPReduce(bUser, bMove);
            PTeam userTeam = Teams[bUser.Local ? 0 : 1];
            PTeam opposingTeam = Teams[bUser.Local ? 1 : 0];
            switch (status)
            {
                case PTeamStatus.LightScreen:
                    if (!userTeam.Status.HasFlag(PTeamStatus.LightScreen))
                    {
                        userTeam.Status |= PTeamStatus.LightScreen;
                        userTeam.LightScreenCount = (byte)(PSettings.ReflectLightScreenTurns + (bUser.Item == PItem.LightClay ? PSettings.LightClayTurnExtension : 0));
                        BroadcastTeamStatus(userTeam.Local, PTeamStatus.LightScreen, PTeamStatusAction.Added);
                        return;
                    }
                    break;
                case PTeamStatus.Reflect:
                    if (!userTeam.Status.HasFlag(PTeamStatus.Reflect))
                    {
                        userTeam.Status |= PTeamStatus.Reflect;
                        userTeam.ReflectCount = (byte)(PSettings.ReflectLightScreenTurns + (bUser.Item == PItem.LightClay ? PSettings.LightClayTurnExtension : 0));
                        BroadcastTeamStatus(userTeam.Local, PTeamStatus.Reflect, PTeamStatusAction.Added);
                        return;
                    }
                    break;
                case PTeamStatus.Spikes:
                    if (opposingTeam.SpikeCount < 3)
                    {
                        opposingTeam.Status |= PTeamStatus.Spikes;
                        opposingTeam.SpikeCount++;
                        BroadcastTeamStatus(opposingTeam.Local, PTeamStatus.Spikes, PTeamStatusAction.Added);
                        return;
                    }
                    break;
                case PTeamStatus.StealthRock:
                    if (!opposingTeam.Status.HasFlag(PTeamStatus.StealthRock))
                    {
                        opposingTeam.Status |= PTeamStatus.StealthRock;
                        BroadcastTeamStatus(opposingTeam.Local, PTeamStatus.StealthRock, PTeamStatusAction.Added);
                        return;
                    }
                    break;
                case PTeamStatus.ToxicSpikes:
                    if (opposingTeam.ToxicSpikeCount < 2)
                    {
                        opposingTeam.Status |= PTeamStatus.ToxicSpikes;
                        opposingTeam.ToxicSpikeCount++;
                        BroadcastTeamStatus(opposingTeam.Local, PTeamStatus.ToxicSpikes, PTeamStatusAction.Added);
                        return;
                    }
                    break;
            }
            BroadcastFail(PFailReason.Default);
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
            if (DealDamage(bUser, bTarget, (ushort)(CalculateDamage() * bDamageMultiplier), effectiveness, false) == 0)
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
                PTeam team = Teams[bTarget.Local ? 0 : 1];
                if (team.Status.HasFlag(PTeamStatus.Reflect))
                {
                    team.Status &= ~PTeamStatus.Reflect;
                    team.ReflectCount = 0;
                    BroadcastTeamStatus(team.Local, PTeamStatus.Reflect, PTeamStatusAction.Cleared);
                }
                if (team.Status.HasFlag(PTeamStatus.LightScreen))
                {
                    team.Status &= ~PTeamStatus.LightScreen;
                    team.LightScreenCount = 0;
                    BroadcastTeamStatus(team.Local, PTeamStatus.LightScreen, PTeamStatusAction.Cleared);
                }
                DealDamage(bUser, bTarget, (ushort)(CalculateDamage() * bDamageMultiplier), effectiveness, false);
                if (bLandedCrit)
                    BroadcastCrit();
                FaintCheck(bTarget);
            }
        }
        void Ef_Dig()
        {
            top:
            if (bUser.Status2.HasFlag(PStatus2.Underground))
            {
                if (!bUsedMove)
                {
                    bUsedMove = true;
                    BroadcastMoveUsed();
                }
                bUser.LockedAction.Decision = PDecision.None;
                bUser.Status2 &= ~PStatus2.Underground;
                BroadcastStatus2(bUser, bUser, PStatus2.Underground, PStatusAction.Ended);
                Ef_Hit();
            }
            else
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bUser, bMove);
                bUser.LockedAction = bUser.SelectedAction;
                bUser.Status2 |= PStatus2.Underground;
                BroadcastStatus2(bUser, bUser, PStatus2.Underground, PStatusAction.Added);
                if (bUser.Item == PItem.PowerHerb)
                {
                    bUser.Item = PItem.None;
                    BroadcastItemUsed(bUser, PItem.PowerHerb);
                    goto top;
                }
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
                BroadcastStatus2(bUser, bUser, PStatus2.Underwater, PStatusAction.Ended);
                Ef_Hit();
            }
            else
            {
                bUsedMove = true;
                BroadcastMoveUsed();
                PPReduce(bUser, bMove);
                bUser.LockedAction = bUser.SelectedAction;
                bUser.Status2 |= PStatus2.Underwater;
                BroadcastStatus2(bUser, bUser, PStatus2.Underwater, PStatusAction.Added);
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
        void Ef_Curse()
        {
            BroadcastMoveUsed();
            PPReduce(bUser, bMove);

            PPokemonData pData = PPokemonData.Data[bUser.Species];
            if (pData.HasType(PType.Ghost))
            {
                if (AccuracyCheck())
                    return;
                PFieldPosition prioritizedPos = GetPositionAcross(BattleStyle, bUser.FieldPosition);
                PTarget target;
                if (prioritizedPos == PFieldPosition.Left)
                    target = PTarget.FoeLeft;
                else if (prioritizedPos == PFieldPosition.Center)
                    target = PTarget.FoeCenter;
                else
                    target = PTarget.FoeRight;
                PPokemon[] targets = GetRuntimeTargets(target, bUser, false);
                if (targets.Length == 0)
                {
                    BroadcastFail(PFailReason.NoTarget);
                }
                else
                {
                    bTarget = targets[0];
                    ApplyStatus2IfPossible(PStatus2.Cursed, true);
                }
            }
            else
            {
                if (bUser.SpeedChange == -PSettings.MaxStatChange
                    && bUser.AttackChange == PSettings.MaxStatChange
                    && bUser.DefenseChange == PSettings.MaxStatChange)
                {
                    BroadcastFail(PFailReason.Default);
                }
                else
                {
                    ApplyStatChange(bUser, PStat.Speed, -1);
                    ApplyStatChange(bUser, PStat.Attack, +1);
                    ApplyStatChange(bUser, PStat.Defense, +1);
                }
            }
        }
    }
}
