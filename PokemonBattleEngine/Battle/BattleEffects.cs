using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        void DoSwitchInEffects(PPokemon pkmn)
        {
            PTeam team = Teams[pkmn.Local ? 0 : 1];

            // Entry Hazards
            if (team.Status.HasFlag(PTeamStatus.Spikes) && !pkmn.HasType(PType.Flying) && pkmn.Ability != PAbility.Levitate)
            {
                double denominator = 10.0 - (2 * team.SpikeCount);
                ushort damage = (ushort)(pkmn.MaxHP / denominator);
                DealDamage(pkmn, pkmn, damage, PEffectiveness.Normal, true);
                BroadcastTeamStatus(team.Local, PTeamStatus.Spikes, PTeamStatusAction.Damage, pkmn.Id);
                if (FaintCheck(pkmn))
                {
                    return;
                }
            }
            if (team.Status.HasFlag(PTeamStatus.StealthRock))
            {
                double effectiveness = 0.125;
                effectiveness *= PPokemonData.TypeEffectiveness[(int)PType.Rock, (int)pkmn.Type1];
                effectiveness *= PPokemonData.TypeEffectiveness[(int)PType.Rock, (int)pkmn.Type2];
                ushort damage = (ushort)(pkmn.MaxHP * effectiveness);
                DealDamage(pkmn, pkmn, damage, PEffectiveness.Normal, true);
                BroadcastTeamStatus(team.Local, PTeamStatus.StealthRock, PTeamStatusAction.Damage, pkmn.Id);
                if (FaintCheck(pkmn))
                {
                    return;
                }
            }
            if (team.Status.HasFlag(PTeamStatus.ToxicSpikes))
            {
                // Grounded Poison types remove the Toxic Spikes
                if (pkmn.HasType(PType.Poison) && pkmn.Ability != PAbility.Levitate && !pkmn.HasType(PType.Flying))
                {
                    BroadcastTeamStatus(team.Local, PTeamStatus.ToxicSpikes, PTeamStatusAction.Cleared);
                }
                // Steel types and floating Pokémon don't get Poisoned
                else if (pkmn.Status1 == PStatus1.None && !pkmn.HasType(PType.Steel) && !pkmn.HasType(PType.Flying) && pkmn.Ability != PAbility.Levitate)
                {
                    PStatus1 status = team.ToxicSpikeCount == 1 ? PStatus1.Poisoned : PStatus1.BadlyPoisoned;
                    pkmn.Status1 = status;
                    if (status == PStatus1.BadlyPoisoned)
                    {
                        pkmn.Status1Counter = 1;
                    }
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
                    if (HealDamage(pkmn, (ushort)(pkmn.MaxHP / PSettings.LeftoversDenominator)) > 0)
                    {
                        BroadcastItemUsed(pkmn, PItem.Leftovers);
                    }
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
            PMove move = user.SelectedAction.FightMove; // bMoveType gets set in BattleDamage.cs->TypeCheck()

            if (PreMoveStatusCheck(user, move))
                return;

            PPokemon[] targets = GetRuntimeTargets(user, user.SelectedAction.FightTargets, PMoveData.Data[move].Targets == PMoveTarget.SingleNotSelf);
            if (targets.Length == 0)
            {
                BroadcastMoveUsed(user, move);
                PPReduce(user, move);
                BroadcastFail(user, PFailReason.NoTarget);
                return;
            }

            PMoveData mData = PMoveData.Data[move];
            switch (mData.Effect)
            {
                case PMoveEffect.BrickBreak:
                    Ef_BrickBreak(user, targets[0]);
                    break;
                case PMoveEffect.Burn:
                    TryForceStatus1(user, targets, move, PStatus1.Burned);
                    break;
                case PMoveEffect.ChangeTarget_ACC:
                    ChangeTargetStats(user, targets, move, new PStat[] { PStat.Accuracy }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeTarget_ATK:
                    ChangeTargetStats(user, targets, move, new PStat[] { PStat.Attack }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeTarget_DEF:
                    ChangeTargetStats(user, targets, move, new PStat[] { PStat.Defense }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeTarget_EVA:
                    ChangeTargetStats(user, targets, move, new PStat[] { PStat.Evasion }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeTarget_SPDEF:
                    ChangeTargetStats(user, targets, move, new PStat[] { PStat.SpDefense }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeTarget_SPE:
                    ChangeTargetStats(user, targets, move, new PStat[] { PStat.Speed }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeUser_ATK:
                    ChangeUserStats(user, move, new PStat[] { PStat.Attack }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeUser_DEF:
                    ChangeUserStats(user, move, new PStat[] { PStat.Defense }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeUser_EVA:
                    ChangeUserStats(user, move, new PStat[] { PStat.Evasion }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeUser_SPATK:
                    ChangeUserStats(user, move, new PStat[] { PStat.SpAttack }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeUser_SPDEF:
                    ChangeUserStats(user, move, new PStat[] { PStat.SpDefense }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.ChangeUser_SPE:
                    ChangeUserStats(user, move, new PStat[] { PStat.Speed }, new sbyte[] { (sbyte)mData.EffectParam });
                    break;
                case PMoveEffect.Confuse:
                    TryForceStatus2(user, targets, move, PStatus2.Confused);
                    break;
                case PMoveEffect.Curse:
                    Ef_Curse(user);
                    break;
                case PMoveEffect.Dig:
                    Ef_Dig(user, targets[0]);
                    break;
                case PMoveEffect.Dive:
                    Ef_Dive(user, targets[0]);
                    break;
                case PMoveEffect.Endeavor:
                    Ef_Endeavor(user, targets[0]);
                    break;
                case PMoveEffect.Fail:
                    Ef_Fail(user, move);
                    break;
                case PMoveEffect.Fly:
                    Ef_Fly(user, targets[0]);
                    break;
                case PMoveEffect.FocusEnergy:
                    TryForceStatus2(user, targets, move, PStatus2.Pumped);
                    break;
                case PMoveEffect.Growth:
                    Ef_Growth(user);
                    break;
                case PMoveEffect.Hit:
                    Ef_Hit(user, targets, move);
                    break;
                case PMoveEffect.Hit__MaybeBurn:
                    HitAndMaybeInflictStatus1(user, targets, move, PStatus1.Burned, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeConfuse:
                    Ef_Hit__MaybeConfuse(user, targets, move, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeFlinch:
                    Ef_Hit__MaybeFlinch(user, targets, move, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeFreeze:
                    HitAndMaybeInflictStatus1(user, targets, move, PStatus1.Frozen, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_ACC_By1:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PStat[] { PStat.Accuracy }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_ATK_By1:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PStat[] { PStat.Attack }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_DEF_By1:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PStat[] { PStat.Defense }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_SPATK_By1:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PStat[] { PStat.SpAttack }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PStat[] { PStat.SpDefense }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PStat[] { PStat.SpDefense }, new sbyte[] { -2 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerTarget_SPE_By1:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PStat[] { PStat.Speed }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeParalyze:
                    HitAndMaybeInflictStatus1(user, targets, move, PStatus1.Paralyzed, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybePoison:
                    HitAndMaybeInflictStatus1(user, targets, move, PStatus1.Poisoned, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_ATK_DEF_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PStat[] { PStat.Attack, PStat.Defense }, new sbyte[] { -1, -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PStat[] { PStat.Defense, PStat.SpDefense }, new sbyte[] { -1, -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_SPATK_By2:
                    HitAndMaybeChangeUserStats(user, targets, move, new PStat[] { PStat.SpAttack }, new sbyte[] { -2 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_SPE_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PStat[] { PStat.Speed }, new sbyte[] { -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PStat[] { PStat.Speed, PStat.Defense, PStat.SpDefense }, new sbyte[] { -1, -1, -1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeRaiseUser_ATK_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PStat[] { PStat.Attack }, new sbyte[] { +1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PStat[] { PStat.Attack, PStat.Defense, PStat.SpAttack, PStat.SpDefense, PStat.Speed }, new sbyte[] { +1, +1, +1, +1, +1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeRaiseUser_DEF_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PStat[] { PStat.Defense }, new sbyte[] { +1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeRaiseUser_SPATK_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PStat[] { PStat.SpAttack }, new sbyte[] { +1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeRaiseUser_SPE_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PStat[] { PStat.Speed }, new sbyte[] { +1 }, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeToxic:
                    HitAndMaybeInflictStatus1(user, targets, move, PStatus1.BadlyPoisoned, mData.EffectParam);
                    break;
                case PMoveEffect.LeechSeed:
                    TryForceStatus2(user, targets, move, PStatus2.LeechSeed);
                    break;
                case PMoveEffect.LightScreen:
                    TryForceTeamStatus(user, move, PTeamStatus.LightScreen);
                    break;
                case PMoveEffect.LowerTarget_ATK_DEF_By1:
                    ChangeTargetStats(user, targets, move, new PStat[] { PStat.Attack, PStat.Defense }, new sbyte[] { -1, -1 });
                    break;
                case PMoveEffect.LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2:
                    ChangeUserStats(user, move, new PStat[] { PStat.Defense, PStat.SpDefense, PStat.Attack, PStat.SpAttack, PStat.Speed }, new sbyte[] { -1, -1, +2, +2, +2 });
                    break;
                case PMoveEffect.Magnitude:
                    Ef_Magnitude(user, targets);
                    break;
                case PMoveEffect.Minimize:
                    TryForceStatus2(user, targets, move, PStatus2.Minimized);
                    break;
                case PMoveEffect.Moonlight:
                    Ef_Moonlight(user);
                    break;
                case PMoveEffect.PainSplit:
                    Ef_PainSplit(user, targets[0]);
                    break;
                case PMoveEffect.Paralyze:
                    TryForceStatus1(user, targets, move, PStatus1.Paralyzed);
                    break;
                case PMoveEffect.Poison:
                    TryForceStatus1(user, targets, move, PStatus1.Poisoned);
                    break;
                case PMoveEffect.Protect:
                    TryForceStatus2(user, targets, move, PStatus2.Protected);
                    break;
                case PMoveEffect.PsychUp:
                    Ef_PsychUp(user, targets[0]);
                    break;
                case PMoveEffect.RainDance:
                    Ef_RainDance(user);
                    break;
                case PMoveEffect.RaiseUser_ATK_ACC_By1:
                    ChangeUserStats(user, move, new PStat[] { PStat.Attack, PStat.Accuracy }, new sbyte[] { +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_ATK_DEF_By1:
                    ChangeUserStats(user, move, new PStat[] { PStat.Attack, PStat.Defense }, new sbyte[] { +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_ATK_DEF_ACC_By1:
                    ChangeUserStats(user, move, new PStat[] { PStat.Attack, PStat.Defense, PStat.Accuracy }, new sbyte[] { +1, +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_ATK_SPATK_By1:
                    ChangeUserStats(user, move, new PStat[] { PStat.Attack, PStat.SpAttack }, new sbyte[] { +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_ATK_SPE_By1:
                    ChangeUserStats(user, move, new PStat[] { PStat.Attack, PStat.Speed }, new sbyte[] { +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_DEF_SPDEF_By1:
                    ChangeUserStats(user, move, new PStat[] { PStat.Defense, PStat.SpDefense }, new sbyte[] { +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_SPATK_SPDEF_By1:
                    ChangeUserStats(user, move, new PStat[] { PStat.SpAttack, PStat.SpDefense }, new sbyte[] { +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_SPATK_SPDEF_SPE_By1:
                    ChangeUserStats(user, move, new PStat[] { PStat.SpAttack, PStat.SpDefense, PStat.Speed }, new sbyte[] { +1, +1, +1 });
                    break;
                case PMoveEffect.RaiseUser_SPE_By2_ATK_By1:
                    ChangeUserStats(user, move, new PStat[] { PStat.Speed, PStat.Attack }, new sbyte[] { +2, +1 });
                    break;
                case PMoveEffect.Reflect:
                    TryForceTeamStatus(user, move, PTeamStatus.Reflect);
                    break;
                case PMoveEffect.Sleep:
                    TryForceStatus1(user, targets, move, PStatus1.Asleep);
                    break;
                case PMoveEffect.Spikes:
                    TryForceTeamStatus(user, move, PTeamStatus.Spikes);
                    break;
                case PMoveEffect.StealthRock:
                    TryForceTeamStatus(user, move, PTeamStatus.StealthRock);
                    break;
                case PMoveEffect.Substitute:
                    TryForceStatus2(user, targets, move, PStatus2.Substitute);
                    break;
                case PMoveEffect.SunnyDay:
                    Ef_SunnyDay(user);
                    break;
                case PMoveEffect.Toxic:
                    TryForceStatus1(user, targets, move, PStatus1.BadlyPoisoned);
                    break;
                case PMoveEffect.ToxicSpikes:
                    TryForceTeamStatus(user, move, PTeamStatus.ToxicSpikes);
                    break;
                case PMoveEffect.Transform:
                    Ef_Transform(user, targets[0]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mData.Effect), $"Invalid move effect: {mData.Effect}");
            }
        }

        // Returns true if an attack gets cancelled from a status
        // Broadcasts status ending events & status causing immobility events
        bool PreMoveStatusCheck(PPokemon user, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];

            // Increment counters first
            if (user.Status2.HasFlag(PStatus2.Confused))
                user.ConfusionCounter++;
            if (user.Status1 == PStatus1.Asleep)
                user.Status1Counter++;

            // Flinch happens before statuses
            // TODO: Put it under sleep
            if (user.Status2.HasFlag(PStatus2.Flinching))
            {
                BroadcastStatus2(user, user, PStatus2.Flinching, PStatusAction.Activated);
                return true;
            }

            // Major statuses
            switch (user.Status1)
            {
                case PStatus1.Asleep:
                    // Check if we can wake up
                    if (user.Status1Counter > user.SleepTurns)
                    {
                        user.Status1 = PStatus1.None;
                        user.Status1Counter = user.SleepTurns = 0;
                        BroadcastStatus1(user, user, PStatus1.Asleep, PStatusAction.Ended);
                    }
                    else
                    {
                        BroadcastStatus1(user, user, PStatus1.Asleep, PStatusAction.Activated);
                        return true;
                    }
                    break;
                case PStatus1.Frozen:
                    // Some moves always defrost the user, but if they don't, there is a 20% chance to thaw out
                    if (mData.Flags.HasFlag(PMoveFlag.DefrostsUser) || PUtils.ApplyChance(20, 100))
                    {
                        user.Status1 = PStatus1.None;
                        BroadcastStatus1(user, user, PStatus1.Frozen, PStatusAction.Ended);
                    }
                    else
                    {
                        BroadcastStatus1(user, user, PStatus1.Frozen, PStatusAction.Activated);
                        return true;
                    }
                    break;
                case PStatus1.Paralyzed:
                    // 25% chance to be unable to move
                    if (PUtils.ApplyChance(25, 100))
                    {
                        BroadcastStatus1(user, user, PStatus1.Paralyzed, PStatusAction.Activated);
                        return true;
                    }
                    break;
            }

            // Minor statuses
            if (user.Status2.HasFlag(PStatus2.Confused))
            {
                // Check if we snap out of confusion
                if (user.ConfusionCounter > user.ConfusionTurns)
                {
                    user.Status2 &= ~PStatus2.Confused;
                    user.ConfusionCounter = user.ConfusionTurns = 0;
                    BroadcastStatus2(user, user, PStatus2.Confused, PStatusAction.Ended);
                }
                else
                {
                    BroadcastStatus2(user, user, PStatus2.Confused, PStatusAction.Activated);
                    // 50% chance to hit itself
                    if (PUtils.ApplyChance(50, 100))
                    {
                        DealDamage(user, user, CalculateDamage(user, user, PMove.None, PType.None, 40, PMoveCategory.Physical, true, true), PEffectiveness.Normal, true);
                        BroadcastStatus2(user, user, PStatus2.Confused, PStatusAction.Damage);
                        FaintCheck(user);
                        return true;
                    }
                }
            }

            return false;
        }

        // Returns true if an attack fails to hit
        // Broadcasts the event if it missed
        // Broadcasts protect if protect activated
        bool MissCheck(PPokemon user, PPokemon target, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];

            if (target.Status2.HasFlag(PStatus2.Protected) && mData.Flags.HasFlag(PMoveFlag.AffectedByProtect))
            {
                BroadcastStatus2(user, target, PStatus2.Protected, PStatusAction.Activated);
                return true;
            }

            // No Guard always hits
            if (user.Ability == PAbility.NoGuard || target.Ability == PAbility.NoGuard)
                return false;

            // Hitting airborne opponents
            if (target.Status2.HasFlag(PStatus2.Airborne) && !mData.Flags.HasFlag(PMoveFlag.HitsAirborne))
                goto miss;
            // Hitting underground opponents
            if (target.Status2.HasFlag(PStatus2.Underground) && !mData.Flags.HasFlag(PMoveFlag.HitsUnderground))
                goto miss;
            // Hitting underwater opponents
            if (target.Status2.HasFlag(PStatus2.Underwater) && !mData.Flags.HasFlag(PMoveFlag.HitsUnderwater))
                goto miss;

            // Moves that always hit
            if (mData.Accuracy == 0)
                return false;

            double chance = mData.Accuracy;
            chance *= GetStatMultiplier(user.AccuracyChange, true) / GetStatMultiplier(target.EvasionChange, true); // Accuracy & Evasion changes
            // Pokémon with Compoundeyes get a 30% Accuracy boost
            if (user.Ability == PAbility.Compoundeyes)
                chance *= 1.3;
            // Pokémon with Hustle get a 20% Accuracy reduction for Physical moves
            if (user.Ability == PAbility.Hustle && mData.Category == PMoveCategory.Physical)
                chance *= 0.8;
            // Pokémon holding a BrightPowder or Lax Incense get a 10% Evasion boost
            if (target.Item == PItem.BrightPowder)
                chance *= 0.9;
            // Pokémon holding a Wide Lens get a 10% Accuracy boost
            if (user.Item == PItem.WideLens)
                chance *= 1.1;
            // Try to hit
            if (PUtils.ApplyChance((int)chance, 100))
                return false;

            miss:
            BroadcastMiss(user);
            return true;
        }

        bool CritCheck(PPokemon user, PPokemon target, PMove move, ref double damageMultiplier)
        {
            // If critical hits cannot be landed, return false
            if (target.Ability == PAbility.BattleArmor
                || target.Ability == PAbility.ShellArmor
                )
                return false;

            PMoveData mData = PMoveData.Data[move];
            byte stage = 0;

            if (mData.Flags.HasFlag(PMoveFlag.HighCritChance))
                stage += 1;
            if (user.Ability == PAbility.SuperLuck)
                stage += 1;
            if (user.Item == PItem.RazorClaw || user.Item == PItem.ScopeLens)
                stage += 1;
            if (user.Status2.HasFlag(PStatus2.Pumped))
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
                damageMultiplier *= PSettings.CritMultiplier;
                if (user.Ability == PAbility.Sniper)
                    damageMultiplier *= 1.5;
                return true;
            }
            return false;
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

        // Returns amount of hp healed
        // Broadcasts the event if it healed more than 0
        ushort HealDamage(PPokemon pkmn, ushort hp)
        {
            ushort oldHP = pkmn.HP;
            pkmn.HP = (ushort)Math.Min(pkmn.MaxHP, pkmn.HP + Math.Max((ushort)1, hp)); // Always try to heal at least 1 HP
            ushort healAmt = (ushort)(pkmn.HP - oldHP);
            if (healAmt > 0)
            {
                BroadcastHPChanged(pkmn, healAmt);
            }
            return healAmt;
        }

        void Ab_LimberCure(PPokemon pkmn)
        {
            if (pkmn.Ability == PAbility.Limber && pkmn.Status1 == PStatus1.Paralyzed)
            {
                pkmn.Status1 = PStatus1.None;
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
        // Broadcasts the event if it fainted
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

        // Pass in the battle to broadcast the event
        public static unsafe void ApplyStatChange(PPokemon pkmn, PStat stat, sbyte change, bool ignoreSimple = false, PBattle battle = null)
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
        bool ApplyStatus1IfPossible(PPokemon user, PPokemon target, PStatus1 status, bool tryingToForce)
        {
            // Cannot change status if already afflicted
            // Cannot change status of a target behind a substitute
            if (target.Status1 != PStatus1.None
                || target.Status2.HasFlag(PStatus2.Substitute))
            {
                if (tryingToForce)
                    BroadcastFail(user, PFailReason.Default);
                return false;
            }

            // A Pokémon with Limber cannot be Paralyzed unless the attacker has Mold Breaker
            if (status == PStatus1.Paralyzed && target.Ability == PAbility.Limber)
            {
                if (tryingToForce)
                {
                    BroadcastLimber(target, true);
                    BroadcastEffectiveness(target, PEffectiveness.Ineffective);
                }
                return false;
            }

            // An Ice type Pokémon cannot be Frozen
            if (status == PStatus1.Frozen && target.HasType(PType.Ice))
            {
                if (tryingToForce)
                    BroadcastEffectiveness(target, PEffectiveness.Ineffective);
                return false;
            }
            // A Fire type Pokémon cannot be Burned
            if (status == PStatus1.Burned && target.HasType(PType.Fire))
            {
                if (tryingToForce)
                    BroadcastEffectiveness(target, PEffectiveness.Ineffective);
                return false;
            }
            // A Poison or Steel type Pokémon cannot be Poisoned or Badly Poisoned
            if ((status == PStatus1.BadlyPoisoned || status == PStatus1.Poisoned) && (target.HasType(PType.Poison) || target.HasType(PType.Steel)))
            {
                if (tryingToForce)
                    BroadcastEffectiveness(target, PEffectiveness.Ineffective);
                return false;
            }


            target.Status1 = status;
            // Start toxic counter
            if (status == PStatus1.BadlyPoisoned)
                target.Status1Counter = 1;
            // Set sleep length
            if (status == PStatus1.Asleep)
                target.SleepTurns = (byte)PUtils.RNG.Next(PSettings.SleepMinTurns, PSettings.SleepMaxTurns + 1);

            BroadcastStatus1(user, target, status, PStatusAction.Added);

            return true;
        }
        // Returns true if the status was applied
        // Broadcasts the change if applied and required
        // "tryingToForce" being true will broadcast failing
        bool ApplyStatus2IfPossible(PPokemon user, PPokemon target, PStatus2 status, bool tryingToForce)
        {
            switch (status)
            {
                case PStatus2.Confused:
                    if (!target.Status2.HasFlag(PStatus2.Confused)
                        && !target.Status2.HasFlag(PStatus2.Substitute))
                    {
                        target.Status2 |= PStatus2.Confused;
                        target.ConfusionTurns = (byte)PUtils.RNG.Next(PSettings.ConfusionMinTurns, PSettings.ConfusionMaxTurns + 1);
                        BroadcastStatus2(user, target, PStatus2.Confused, PStatusAction.Added);
                        return true;
                    }
                    break;
                case PStatus2.Cursed:
                    if (!target.Status2.HasFlag(PStatus2.Cursed))
                    {
                        target.Status2 |= PStatus2.Cursed;
                        BroadcastStatus2(user, target, PStatus2.Cursed, PStatusAction.Added);
                        DealDamage(user, user, (ushort)(user.MaxHP / 2), PEffectiveness.Normal, true);
                        return true;
                    }
                    break;
                case PStatus2.Flinching:
                    if (!target.Status2.HasFlag(PStatus2.Substitute))
                    {
                        target.Status2 |= status;
                        return true;
                    }
                    break;
                case PStatus2.LeechSeed:
                    if (!target.Status2.HasFlag(PStatus2.LeechSeed)
                        && !target.Status2.HasFlag(PStatus2.Substitute)
                        && !target.HasType(PType.Grass))
                    {
                        target.Status2 |= PStatus2.LeechSeed;
                        target.SeededPosition = user.FieldPosition;
                        BroadcastStatus2(user, target, PStatus2.LeechSeed, PStatusAction.Added);
                        return true;
                    }
                    break;
                case PStatus2.Minimized:
                    user.Status2 |= PStatus2.Minimized;
                    BroadcastStatus2(user, user, PStatus2.Minimized, PStatusAction.Added);
                    ApplyStatChange(user, PStat.Evasion, +2, battle: this);
                    return true;
                case PStatus2.Protected:
                    {
                        // TODO: If the user goes last, fail
                        ushort chance = ushort.MaxValue;
                        for (int i = 0; i < user.ProtectCounter; i++)
                            chance /= 2;
                        if (PUtils.ApplyChance(chance, ushort.MaxValue))
                        {
                            user.Status2 |= PStatus2.Protected;
                            user.ProtectCounter++;
                            BroadcastStatus2(user, user, PStatus2.Protected, PStatusAction.Added);
                            return true;
                        }
                        user.ProtectCounter = 0;
                    }
                    break;
                case PStatus2.Pumped:
                    if (!user.Status2.HasFlag(PStatus2.Pumped))
                    {
                        user.Status2 |= status;
                        BroadcastStatus2(user, user, PStatus2.Pumped, PStatusAction.Added);
                        return true;
                    }
                    break;
                case PStatus2.Substitute:
                    {
                        ushort hpRequired = (ushort)(user.MaxHP / 4);
                        if (!user.Status2.HasFlag(PStatus2.Substitute) && hpRequired > 0 && user.HP > hpRequired)
                        {
                            DealDamage(user, user, hpRequired, PEffectiveness.Normal, true);
                            user.Status2 |= PStatus2.Substitute;
                            user.SubstituteHP = hpRequired;
                            BroadcastStatus2(user, user, PStatus2.Substitute, PStatusAction.Added);
                            return true;
                        }
                    }
                    break;
            }
            if (tryingToForce)
                BroadcastFail(user, PFailReason.Default);
            return false;
        }

        void Ef_Hit(PPokemon user, PPokemon[] targets, PMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, move))
                {
                    continue;
                }
                double damageMultiplier = targets.Length > 1 ? 0.75 : 1.0;
                bool crit = CritCheck(user, target, move, ref damageMultiplier);
                TypeCheck(user, target, move, out PType moveType, out PEffectiveness effectiveness);
                ushort damage = CalculateDamage(user, target, move, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastCrit();
                }
                FaintCheck(target);
            }
        }
        // TODO: Convert the following into HitAndMaybeInflictStatus2
        void Ef_Hit__MaybeConfuse(PPokemon user, PPokemon[] targets, PMove move, int chanceToConfuse)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, move))
                {
                    continue;
                }
                double damageMultiplier = targets.Length > 1 ? 0.75 : 1.0;
                bool crit = CritCheck(user, target, move, ref damageMultiplier);
                TypeCheck(user, target, move, out PType moveType, out PEffectiveness effectiveness);
                bool behindSubstitute = target.Status2.HasFlag(PStatus2.Substitute);
                ushort damage = CalculateDamage(user, target, move, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastCrit();
                }
                if (FaintCheck(target))
                {
                    continue;
                }
                if (effectiveness != PEffectiveness.Ineffective && !behindSubstitute && PUtils.ApplyChance(chanceToConfuse, 100))
                {
                    ApplyStatus2IfPossible(user, target, PStatus2.Confused, false);
                }
            }
        }
        void Ef_Hit__MaybeFlinch(PPokemon user, PPokemon[] targets, PMove move, int chanceToFlinch)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, move))
                {
                    continue;
                }
                double damageMultiplier = targets.Length > 1 ? 0.75 : 1.0;
                bool crit = CritCheck(user, target, move, ref damageMultiplier);
                TypeCheck(user, target, move, out PType moveType, out PEffectiveness effectiveness);
                bool behindSubstitute = target.Status2.HasFlag(PStatus2.Substitute);
                ushort damage = CalculateDamage(user, target, move, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastCrit();
                }
                if (FaintCheck(target))
                {
                    continue;
                }
                if (effectiveness != PEffectiveness.Ineffective && !behindSubstitute && PUtils.ApplyChance(chanceToFlinch, 100))
                {
                    ApplyStatus2IfPossible(user, target, PStatus2.Flinching, false);
                }
            }
        }

        void TryForceStatus1(PPokemon user, PPokemon[] targets, PMove move, PStatus1 status)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, move))
                {
                    continue;
                }
                TypeCheck(user, target, move, out PType moveType, out PEffectiveness effectiveness);
                if (effectiveness == PEffectiveness.Ineffective) // Paralysis, Normalize
                {
                    BroadcastEffectiveness(target, effectiveness);
                }
                else
                {
                    ApplyStatus1IfPossible(user, target, status, true);
                }
            }
        }
        void TryForceStatus2(PPokemon user, PPokemon[] targets, PMove move, PStatus2 status)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, move))
                {
                    continue;
                }
                ApplyStatus2IfPossible(user, target, status, true);
            }
        }
        void TryForceTeamStatus(PPokemon user, PMove move, PTeamStatus status)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            PTeam userTeam = Teams[user.Local ? 0 : 1];
            PTeam opposingTeam = Teams[user.Local ? 1 : 0];
            switch (status)
            {
                case PTeamStatus.LightScreen:
                    if (!userTeam.Status.HasFlag(PTeamStatus.LightScreen))
                    {
                        userTeam.Status |= PTeamStatus.LightScreen;
                        userTeam.LightScreenCount = (byte)(PSettings.ReflectLightScreenTurns + (user.Item == PItem.LightClay ? PSettings.LightClayTurnExtension : 0));
                        BroadcastTeamStatus(userTeam.Local, PTeamStatus.LightScreen, PTeamStatusAction.Added);
                        return;
                    }
                    break;
                case PTeamStatus.Reflect:
                    if (!userTeam.Status.HasFlag(PTeamStatus.Reflect))
                    {
                        userTeam.Status |= PTeamStatus.Reflect;
                        userTeam.ReflectCount = (byte)(PSettings.ReflectLightScreenTurns + (user.Item == PItem.LightClay ? PSettings.LightClayTurnExtension : 0));
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
            BroadcastFail(user, PFailReason.Default);
        }
        void HitAndMaybeInflictStatus1(PPokemon user, PPokemon[] targets, PMove move, PStatus1 status, int chanceToInflict)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, move))
                {
                    continue;
                }
                double damageMultiplier = targets.Length > 1 ? 0.75 : 1.0;
                bool crit = CritCheck(user, target, move, ref damageMultiplier);
                TypeCheck(user, target, move, out PType moveType, out PEffectiveness effectiveness);
                bool behindSubstitute = target.Status2.HasFlag(PStatus2.Substitute);
                ushort damage = CalculateDamage(user, target, move, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastCrit();
                }
                if (FaintCheck(target))
                {
                    continue;
                }
                if (effectiveness != PEffectiveness.Ineffective && !behindSubstitute && PUtils.ApplyChance(chanceToInflict, 100))
                {
                    ApplyStatus1IfPossible(user, target, status, false);
                }
            }
        }

        void ChangeTargetStats(PPokemon user, PPokemon[] targets, PMove move, PStat[] stats, sbyte[] changes)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, move))
                {
                    continue;
                }
                if (target.Status2.HasFlag(PStatus2.Substitute))
                {
                    BroadcastFail(user, PFailReason.Default);
                }
                else
                {
                    for (int i = 0; i < stats.Length; i++)
                        ApplyStatChange(target, stats[i], changes[i], battle: this);
                }
            }
        }
        void ChangeUserStats(PPokemon user, PMove move, PStat[] stats, sbyte[] changes)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            for (int i = 0; i < stats.Length; i++)
            {
                ApplyStatChange(user, stats[i], changes[i], battle: this);
            }
        }
        void HitAndMaybeChangeTargetStats(PPokemon user, PPokemon[] targets, PMove move, PStat[] stats, sbyte[] changes, int chanceToChangeStats)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, move))
                {
                    continue;
                }
                double damageMultiplier = targets.Length > 1 ? 0.75 : 1.0;
                bool crit = CritCheck(user, target, move, ref damageMultiplier);
                TypeCheck(user, target, move, out PType moveType, out PEffectiveness effectiveness);
                bool behindSubstitute = target.Status2.HasFlag(PStatus2.Substitute);
                ushort damage = CalculateDamage(user, target, move, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastCrit();
                }
                if (FaintCheck(target))
                {
                    continue;
                }
                if (effectiveness != PEffectiveness.Ineffective && !behindSubstitute && PUtils.ApplyChance(chanceToChangeStats, 100))
                {
                    for (int i = 0; i < stats.Length; i++)
                    {
                        ApplyStatChange(target, stats[i], changes[i], battle: this);
                    }
                }
            }
        }
        void HitAndMaybeChangeUserStats(PPokemon user, PPokemon[] targets, PMove move, PStat[] stats, sbyte[] changes, int chanceToChangeStats)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            byte hit = 0;
            foreach (PPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, move))
                {
                    continue;
                }
                double damageMultiplier = targets.Length > 1 ? 0.75 : 1.0;
                bool crit = CritCheck(user, target, move, ref damageMultiplier);
                TypeCheck(user, target, move, out PType moveType, out PEffectiveness effectiveness);
                ushort damage = CalculateDamage(user, target, move, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastCrit();
                }
                FaintCheck(target);
                if (effectiveness != PEffectiveness.Ineffective)
                {
                    hit++;
                }
            }
            if (hit > 0)
            {
                if (PUtils.ApplyChance(chanceToChangeStats, 100))
                {
                    for (int i = 0; i < stats.Length; i++)
                    {
                        ApplyStatChange(user, stats[i], changes[i], battle: this);
                    }
                }
            }
        }

        void Ef_Fail(PPokemon user, PMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            BroadcastFail(user, PFailReason.Default);
        }
        void Ef_BrickBreak(PPokemon user, PPokemon target)
        {
            BroadcastMoveUsed(user, PMove.BrickBreak);
            PPReduce(user, PMove.BrickBreak);
            if (MissCheck(user, target, PMove.BrickBreak))
            {
                return;
            }
            double damageMultiplier = 1.0;
            bool crit = CritCheck(user, target, PMove.BrickBreak, ref damageMultiplier);
            TypeCheck(user, target, PMove.BrickBreak, out PType moveType, out PEffectiveness effectiveness);
            if (effectiveness == PEffectiveness.Ineffective)
            {
                BroadcastEffectiveness(target, effectiveness);
            }
            else
            {
                PTeam team = Teams[target.Local ? 0 : 1];
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
                ushort damage = CalculateDamage(user, target, PMove.BrickBreak, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastCrit();
                }
                FaintCheck(target);
            }
        }
        void Ef_Dig(PPokemon user, PPokemon target)
        {
            BroadcastMoveUsed(user, PMove.Dig);
            PPReduce(user, PMove.Dig);
            top:
            if (user.Status2.HasFlag(PStatus2.Underground))
            {
                user.LockedAction.Decision = PDecision.None;
                user.Status2 &= ~PStatus2.Underground;
                BroadcastStatus2(user, user, PStatus2.Underground, PStatusAction.Ended);
                if (MissCheck(user, target, PMove.Dig))
                    return;
                double damageMultiplier = 1.0;
                bool crit = CritCheck(user, target, PMove.Dig, ref damageMultiplier);
                TypeCheck(user, target, PMove.Dig, out PType moveType, out PEffectiveness effectiveness);
                ushort damage = CalculateDamage(user, target, PMove.Dig, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastCrit();
                }
                FaintCheck(target);
            }
            else
            {
                user.LockedAction = user.SelectedAction;
                user.Status2 |= PStatus2.Underground;
                BroadcastStatus2(user, user, PStatus2.Underground, PStatusAction.Added);
                if (user.Item == PItem.PowerHerb)
                {
                    user.Item = PItem.None;
                    BroadcastItemUsed(user, PItem.PowerHerb);
                    goto top;
                }
            }
        }
        void Ef_Dive(PPokemon user, PPokemon target)
        {
            BroadcastMoveUsed(user, PMove.Dive);
            PPReduce(user, PMove.Dive);
            top:
            if (user.Status2.HasFlag(PStatus2.Underwater))
            {
                user.LockedAction.Decision = PDecision.None;
                user.Status2 &= ~PStatus2.Underwater;
                BroadcastStatus2(user, user, PStatus2.Underwater, PStatusAction.Ended);
                if (MissCheck(user, target, PMove.Dive))
                {
                    return;
                }
                double damageMultiplier = 1.0;
                bool crit = CritCheck(user, target, PMove.Dive, ref damageMultiplier);
                TypeCheck(user, target, PMove.Dive, out PType moveType, out PEffectiveness effectiveness);
                ushort damage = CalculateDamage(user, target, PMove.Dive, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastCrit();
                }
                FaintCheck(target);
            }
            else
            {
                user.LockedAction = user.SelectedAction;
                user.Status2 |= PStatus2.Underwater;
                BroadcastStatus2(user, user, PStatus2.Underwater, PStatusAction.Added);
                if (user.Item == PItem.PowerHerb)
                {
                    user.Item = PItem.None;
                    BroadcastItemUsed(user, PItem.PowerHerb);
                    goto top;
                }
            }
        }
        void Ef_Fly(PPokemon user, PPokemon target)
        {
            BroadcastMoveUsed(user, PMove.Fly);
            PPReduce(user, PMove.Fly);
            top:
            if (user.Status2.HasFlag(PStatus2.Airborne))
            {
                user.LockedAction.Decision = PDecision.None;
                user.Status2 &= ~PStatus2.Airborne;
                BroadcastStatus2(user, user, PStatus2.Airborne, PStatusAction.Ended);
                if (MissCheck(user, target, PMove.Fly))
                {
                    return;
                }
                double damageMultiplier = 1.0;
                bool crit = CritCheck(user, target, PMove.Fly, ref damageMultiplier);
                TypeCheck(user, target, PMove.Fly, out PType moveType, out PEffectiveness effectiveness);
                ushort damage = CalculateDamage(user, target, PMove.Fly, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastCrit();
                }
                FaintCheck(target);
            }
            else
            {
                user.LockedAction = user.SelectedAction;
                user.Status2 |= PStatus2.Airborne;
                BroadcastStatus2(user, user, PStatus2.Airborne, PStatusAction.Added);
                if (user.Item == PItem.PowerHerb)
                {
                    user.Item = PItem.None;
                    BroadcastItemUsed(user, PItem.PowerHerb);
                    goto top;
                }
            }
        }
        void Ef_RainDance(PPokemon user)
        {
            BroadcastMoveUsed(user, PMove.RainDance);
            PPReduce(user, PMove.RainDance);
            if (Weather == PWeather.Raining)
            {
                BroadcastFail(user, PFailReason.Default);
            }
            else
            {
                Weather = PWeather.Raining;
                WeatherCounter = (byte)(PSettings.RainTurns + (user.Item == PItem.DampRock ? PSettings.DampRockTurnExtension : 0));
                BroadcastWeather(Weather, PWeatherAction.Added);
            }
        }
        void Ef_SunnyDay(PPokemon user)
        {
            BroadcastMoveUsed(user, PMove.SunnyDay);
            PPReduce(user, PMove.SunnyDay);
            if (Weather == PWeather.Sunny)
            {
                BroadcastFail(user, PFailReason.Default);
            }
            else
            {
                Weather = PWeather.Sunny;
                WeatherCounter = (byte)(PSettings.SunTurns + (user.Item == PItem.HeatRock ? PSettings.HeatRockTurnExtension : 0));
                BroadcastWeather(Weather, PWeatherAction.Added);
            }
        }
        void Ef_Growth(PPokemon user)
        {
            sbyte change = (sbyte)(Weather == PWeather.Sunny ? +2 : +1);
            ChangeUserStats(user, PMove.Growth, new PStat[] { PStat.Attack, PStat.SpAttack }, new sbyte[] { change, change });
        }
        void Ef_Moonlight(PPokemon user)
        {
            BroadcastMoveUsed(user, PMove.Moonlight);
            PPReduce(user, PMove.Moonlight);
            double percentage;
            switch (Weather)
            {
                case PWeather.None: percentage = 0.5; break;
                case PWeather.Sunny: percentage = 0.66; break;
                default: percentage = 0.25; break;
            }
            ushort hp = (ushort)(user.MaxHP * percentage);
            if (HealDamage(user, hp) == 0)
            {
                BroadcastFail(user, PFailReason.HPFull);
            }
        }
        void Ef_Transform(PPokemon user, PPokemon target)
        {
            BroadcastMoveUsed(user, PMove.Transform);
            PPReduce(user, PMove.Transform);
            if (user.Status2.HasFlag(PStatus2.Transformed)
                || target.Status2.HasFlag(PStatus2.Transformed)
                || target.Status2.HasFlag(PStatus2.Substitute))
            {
                BroadcastFail(user, PFailReason.Default);
                return;
            }
            if (MissCheck(user, target, PMove.Transform))
            {
                return;
            }
            user.Transform(target, target.Attack, target.Defense, target.SpAttack, target.SpDefense, target.Speed, target.Ability, target.Type1, target.Type2, target.Moves);
            BroadcastTransform(user, target);
        }
        void Ef_Curse(PPokemon user)
        {
            BroadcastMoveUsed(user, PMove.Curse);
            PPReduce(user, PMove.Curse);
            if (user.HasType(PType.Ghost))
            {
                PFieldPosition prioritizedPos = GetPositionAcross(BattleStyle, user.FieldPosition);
                PTarget t;
                if (prioritizedPos == PFieldPosition.Left)
                    t = PTarget.FoeLeft;
                else if (prioritizedPos == PFieldPosition.Center)
                    t = PTarget.FoeCenter;
                else
                    t = PTarget.FoeRight;
                PPokemon[] targets = GetRuntimeTargets(user, t, false);
                if (targets.Length == 0)
                {
                    BroadcastFail(user, PFailReason.NoTarget);
                }
                else if (!MissCheck(user, targets[0], PMove.Curse))
                {
                    ApplyStatus2IfPossible(user, targets[0], PStatus2.Cursed, true);
                }
            }
            else
            {
                if (user.SpeedChange == -PSettings.MaxStatChange
                    && user.AttackChange == PSettings.MaxStatChange
                    && user.DefenseChange == PSettings.MaxStatChange)
                {
                    BroadcastFail(user, PFailReason.Default);
                }
                else
                {
                    ApplyStatChange(user, PStat.Speed, -1, battle: this);
                    ApplyStatChange(user, PStat.Attack, +1, battle: this);
                    ApplyStatChange(user, PStat.Defense, +1, battle: this);
                }
            }
        }
        void Ef_Magnitude(PPokemon user, PPokemon[] targets)
        {
            BroadcastMoveUsed(user, PMove.Magnitude);
            PPReduce(user, PMove.Magnitude);
            int val = PUtils.RNG.Next(0, 100);
            byte magnitude, basePower;
            if (val < 5) // Magnitude 4 - 5%
            {
                magnitude = 4;
                basePower = 10;
            }
            else if (val < 15) // Magnitude 5 - 10%
            {
                magnitude = 5;
                basePower = 30;
            }
            else if (val < 35) // Magnitude 6 - 20%
            {
                magnitude = 6;
                basePower = 50;
            }
            else if (val < 65) // Magnitude 7 - 30%
            {
                magnitude = 7;
                basePower = 70;
            }
            else if (val < 85) // Magnitude 8 - 20%
            {
                magnitude = 8;
                basePower = 90;
            }
            else if (val < 95) // Magnitude 9 - 10%
            {
                magnitude = 9;
                basePower = 110;
            }
            else // Magnitude 10 - 5%
            {
                magnitude = 10;
                basePower = 150;
            }
            BroadcastMagnitude(magnitude);
            foreach (PPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, PMove.Magnitude))
                {
                    continue;
                }
                double damageMultiplier = targets.Length > 1 ? 0.75 : 1.0;
                bool crit = CritCheck(user, target, PMove.Magnitude, ref damageMultiplier);
                TypeCheck(user, target, PMove.Magnitude, out PType moveType, out PEffectiveness effectiveness);
                ushort damage = CalculateDamage(user, target, PMove.Magnitude, moveType, criticalHit: crit, power: basePower);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastCrit();
                }
                FaintCheck(target);
            }
        }
        void Ef_Endeavor(PPokemon user, PPokemon target)
        {
            BroadcastMoveUsed(user, PMove.Endeavor);
            PPReduce(user, PMove.Endeavor);
            if (MissCheck(user, target, PMove.Endeavor))
            {
                return;
            }
            if (target.HP <= user.HP)
            {
                BroadcastFail(user, PFailReason.Default);
                return;
            }
            DealDamage(user, target, (ushort)(target.HP - user.HP), PEffectiveness.Normal, false);
        }
        void Ef_PainSplit(PPokemon user, PPokemon target)
        {
            BroadcastMoveUsed(user, PMove.PainSplit);
            PPReduce(user, PMove.PainSplit);
            if (target.Status2.HasFlag(PStatus2.Substitute))
            {
                BroadcastFail(user, PFailReason.Default);
                return;
            }
            if (MissCheck(user, target, PMove.PainSplit))
            {
                return;
            }
            ushort total = (ushort)(user.HP + target.HP);
            ushort hp = (ushort)(total / 2);
            foreach (PPokemon pkmn in new PPokemon[] { user, target })
            {
                if (hp >= pkmn.HP)
                {
                    HealDamage(user, (ushort)(hp - pkmn.HP));
                }
                else
                {
                    DealDamage(user, pkmn, (ushort)(pkmn.HP - hp), PEffectiveness.Normal, true);
                }
            }
            BroadcastPainSplit();
        }
        void Ef_PsychUp(PPokemon user, PPokemon target)
        {
            BroadcastMoveUsed(user, PMove.PsychUp);
            PPReduce(user, PMove.PsychUp);
            if (MissCheck(user, target, PMove.PsychUp))
            {
                return;
            }
            user.AttackChange = target.AttackChange;
            user.DefenseChange = target.DefenseChange;
            user.SpAttackChange = target.SpAttackChange;
            user.SpDefenseChange = target.SpDefenseChange;
            user.SpeedChange = target.SpeedChange;
            user.AccuracyChange = target.AccuracyChange;
            user.EvasionChange = target.EvasionChange;
            BroadcastPsychUp(user, target);
        }
    }
}
