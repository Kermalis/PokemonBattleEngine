using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        void DoSwitchInEffects(PBEPokemon pkmn)
        {
            PBETeam team = Teams[pkmn.LocalTeam ? 0 : 1];

            // Entry Hazards
            if (team.Status.HasFlag(PBETeamStatus.Spikes) && !pkmn.HasType(PBEType.Flying) && pkmn.Ability != PBEAbility.Levitate)
            {
                double denominator = 10.0 - (2 * team.SpikeCount);
                ushort damage = (ushort)(pkmn.MaxHP / denominator);
                DealDamage(pkmn, pkmn, damage, PBEEffectiveness.Normal, true);
                BroadcastTeamStatus(team.LocalTeam, PBETeamStatus.Spikes, PBETeamStatusAction.Damage, pkmn.Id);
                if (FaintCheck(pkmn))
                {
                    return;
                }
            }
            if (team.Status.HasFlag(PBETeamStatus.StealthRock))
            {
                double effectiveness = 0.125;
                effectiveness *= PBEPokemonData.TypeEffectiveness[(int)PBEType.Rock, (int)pkmn.Type1];
                effectiveness *= PBEPokemonData.TypeEffectiveness[(int)PBEType.Rock, (int)pkmn.Type2];
                ushort damage = (ushort)(pkmn.MaxHP * effectiveness);
                DealDamage(pkmn, pkmn, damage, PBEEffectiveness.Normal, true);
                BroadcastTeamStatus(team.LocalTeam, PBETeamStatus.StealthRock, PBETeamStatusAction.Damage, pkmn.Id);
                if (FaintCheck(pkmn))
                {
                    return;
                }
            }
            if (team.Status.HasFlag(PBETeamStatus.ToxicSpikes))
            {
                // Grounded Poison types remove the Toxic Spikes
                if (pkmn.HasType(PBEType.Poison) && pkmn.Ability != PBEAbility.Levitate && !pkmn.HasType(PBEType.Flying))
                {
                    BroadcastTeamStatus(team.LocalTeam, PBETeamStatus.ToxicSpikes, PBETeamStatusAction.Cleared);
                }
                // Steel types and floating Pokémon don't get Poisoned
                else if (pkmn.Status1 == PBEStatus1.None && !pkmn.HasType(PBEType.Steel) && !pkmn.HasType(PBEType.Flying) && pkmn.Ability != PBEAbility.Levitate)
                {
                    PBEStatus1 status = team.ToxicSpikeCount == 1 ? PBEStatus1.Poisoned : PBEStatus1.BadlyPoisoned;
                    pkmn.Status1 = status;
                    if (status == PBEStatus1.BadlyPoisoned)
                    {
                        pkmn.Status1Counter = 1;
                    }
                    BroadcastStatus1(pkmn, pkmn, status, PBEStatusAction.Added);
                }
            }

            // Abilities
            LimberCheck(pkmn);
        }
        void DoPreMoveEffects(PBEPokemon pkmn)
        {
            // Abilities
            LimberCheck(pkmn);
        }
        void DoTurnEndedEffects(PBEPokemon pkmn)
        {
            PBETeam userTeam = Teams[pkmn.LocalTeam ? 0 : 1];
            PBETeam opposingTeam = Teams[pkmn.LocalTeam ? 1 : 0];

            // Weather
            switch (Weather)
            {
                case PBEWeather.Hailstorm:
                    if (!pkmn.HasType(PBEType.Ice)
                        && pkmn.Ability != PBEAbility.Overcoat
                        && pkmn.Ability != PBEAbility.SnowCloak)
                    {
                        if (pkmn.Ability == PBEAbility.IceBody)
                        {
                            ushort hp = (ushort)(pkmn.MaxHP / Settings.IceBodyHealDenominator);
                            if (pkmn.HP < pkmn.MaxHP && pkmn.HP + hp <= pkmn.MaxHP)
                            {
                                BroadcastAbility(pkmn, pkmn, PBEAbility.IceBody, PBEAbilityAction.RestoredHP);
                                HealDamage(pkmn, hp);
                            }
                        }
                        else
                        {
                            BroadcastWeather(PBEWeather.Hailstorm, PBEWeatherAction.CausedDamage, pkmn.Id);
                            DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / Settings.HailDamageDenominator), PBEEffectiveness.Normal, true);
                            if (FaintCheck(pkmn))
                            {
                                return;
                            }
                        }
                    }
                    break;
                case PBEWeather.Sandstorm:
                    if (!pkmn.HasType(PBEType.Ice)
                        && !pkmn.HasType(PBEType.Ground)
                        && !pkmn.HasType(PBEType.Steel)
                        && pkmn.Ability != PBEAbility.Overcoat
                        && pkmn.Ability != PBEAbility.SandForce
                        && pkmn.Ability != PBEAbility.SandRush
                        && pkmn.Ability != PBEAbility.SandVeil
                        && !pkmn.Status2.HasFlag(PBEStatus2.Underground)
                        && !pkmn.Status2.HasFlag(PBEStatus2.Underwater))
                    {
                        BroadcastWeather(PBEWeather.Sandstorm, PBEWeatherAction.CausedDamage, pkmn.Id);
                        DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / Settings.SandstormDamageDenominator), PBEEffectiveness.Normal, true);
                        if (FaintCheck(pkmn))
                        {
                            return;
                        }
                    }
                    break;
            }

            // Items
            switch (pkmn.Item)
            {
                case PBEItem.BlackSludge:
                    if (pkmn.HasType(PBEType.Poison))
                    {
                        ushort hp = (ushort)(pkmn.MaxHP / Settings.BlackSludgeHealDenominator);
                        if (pkmn.HP < pkmn.MaxHP && pkmn.HP + hp <= pkmn.MaxHP)
                        {
                            HealDamage(pkmn, hp);
                            BroadcastItem(pkmn, pkmn, PBEItem.BlackSludge, PBEItemAction.RestoredHP);
                        }
                    }
                    else
                    {
                        DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / Settings.BlackSludgeDamageDenominator), PBEEffectiveness.Normal, true);
                        BroadcastItem(pkmn, pkmn, PBEItem.BlackSludge, PBEItemAction.CausedDamage);
                        if (FaintCheck(pkmn))
                        {
                            return;
                        }
                    }
                    break;
                case PBEItem.Leftovers:
                    {
                        ushort hp = (ushort)(pkmn.MaxHP / Settings.LeftoversHealDenominator);
                        if (pkmn.HP < pkmn.MaxHP && pkmn.HP + hp <= pkmn.MaxHP)
                        {
                            HealDamage(pkmn, hp);
                            BroadcastItem(pkmn, pkmn, PBEItem.Leftovers, PBEItemAction.RestoredHP);
                        }
                    }
                    break;
            }

            // Major statuses
            switch (pkmn.Status1)
            {
                case PBEStatus1.Burned:
                    int damage = pkmn.MaxHP / Settings.BurnDamageDenominator;
                    // Pokémon with the Heatproof ability take half as much damage from burns
                    if (pkmn.Ability == PBEAbility.Heatproof)
                    {
                        damage /= 2;
                    }
                    DealDamage(pkmn, pkmn, (ushort)damage, PBEEffectiveness.Normal, true);
                    BroadcastStatus1(pkmn, pkmn, PBEStatus1.Burned, PBEStatusAction.Damage);
                    if (FaintCheck(pkmn))
                    {
                        return;
                    }
                    break;
                case PBEStatus1.Poisoned:
                    DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / Settings.PoisonDamageDenominator), PBEEffectiveness.Normal, true);
                    BroadcastStatus1(pkmn, pkmn, PBEStatus1.Poisoned, PBEStatusAction.Damage);
                    FaintCheck(pkmn);
                    break;
                case PBEStatus1.BadlyPoisoned:
                    DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP * pkmn.Status1Counter / Settings.ToxicDamageDenominator), PBEEffectiveness.Normal, true);
                    BroadcastStatus1(pkmn, pkmn, PBEStatus1.BadlyPoisoned, PBEStatusAction.Damage);
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
            if (pkmn.Status2.HasFlag(PBEStatus2.LeechSeed))
            {
                PBEPokemon seeder = opposingTeam.PokemonAtPosition(pkmn.SeededPosition);
                if (seeder != null)
                {
                    ushort hp = (ushort)(pkmn.MaxHP / Settings.LeechSeedDenominator);
                    ushort amtDealt = DealDamage(seeder, pkmn, hp, PBEEffectiveness.Normal, true);
                    HealDamage(seeder, amtDealt);
                    BroadcastStatus2(seeder, pkmn, PBEStatus2.LeechSeed, PBEStatusAction.Damage);
                    if (FaintCheck(pkmn))
                    {
                        return;
                    }
                }
            }
            if (pkmn.Status2.HasFlag(PBEStatus2.Cursed))
            {
                DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / Settings.CurseDenominator), PBEEffectiveness.Normal, true);
                BroadcastStatus2(pkmn, pkmn, PBEStatus2.Cursed, PBEStatusAction.Damage);
                if (FaintCheck(pkmn))
                {
                    return;
                }
            }

            // Abilities
            LimberCheck(pkmn);
        }

        void UseMove(PBEPokemon user)
        {
            PBEMove move = user.SelectedAction.FightMove; // bMoveType gets set in BattleDamage.cs->TypeCheck()
            if (PreMoveStatusCheck(user, move))
            {
                return;
            }
            PBEPokemon[] targets = GetRuntimeTargets(user, user.SelectedAction.FightTargets, PBEMoveData.GetMoveTargetsForPokemon(user, move) == PBEMoveTarget.SingleNotSelf);
            if (targets.Length == 0)
            {
                BroadcastMoveUsed(user, move);
                PPReduce(user, move);
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
                return;
            }
            PBEMoveData mData = PBEMoveData.Data[move];
            switch (mData.Effect)
            {
                case PBEMoveEffect.BrickBreak:
                    Ef_BrickBreak(user, targets[0]);
                    break;
                case PBEMoveEffect.Burn:
                    TryForceStatus1(user, targets, move, PBEStatus1.Burned);
                    break;
                case PBEMoveEffect.ChangeTarget_ACC:
                    ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Accuracy }, new short[] { (short)mData.EffectParam });
                    break;
                case PBEMoveEffect.ChangeTarget_ATK:
                    ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack }, new short[] { (short)mData.EffectParam });
                    break;
                case PBEMoveEffect.ChangeTarget_DEF:
                    ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Defense }, new short[] { (short)mData.EffectParam });
                    break;
                case PBEMoveEffect.ChangeTarget_EVA:
                    ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Evasion }, new short[] { (short)mData.EffectParam });
                    break;
                case PBEMoveEffect.ChangeTarget_SPDEF:
                    ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpDefense }, new short[] { (short)mData.EffectParam });
                    break;
                case PBEMoveEffect.ChangeTarget_SPE:
                    ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Speed }, new short[] { (short)mData.EffectParam });
                    break;
                case PBEMoveEffect.ChangeUser_ATK:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack }, new short[] { (short)mData.EffectParam });
                    break;
                case PBEMoveEffect.ChangeUser_DEF:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.Defense }, new short[] { (short)mData.EffectParam });
                    break;
                case PBEMoveEffect.ChangeUser_EVA:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.Evasion }, new short[] { (short)mData.EffectParam });
                    break;
                case PBEMoveEffect.ChangeUser_SPATK:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.SpAttack }, new short[] { (short)mData.EffectParam });
                    break;
                case PBEMoveEffect.ChangeUser_SPDEF:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.SpDefense }, new short[] { (short)mData.EffectParam });
                    break;
                case PBEMoveEffect.ChangeUser_SPE:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.Speed }, new short[] { (short)mData.EffectParam });
                    break;
                case PBEMoveEffect.Confuse:
                    TryForceStatus2(user, targets, move, PBEStatus2.Confused);
                    break;
                case PBEMoveEffect.Curse:
                    Ef_Curse(user, targets[0]);
                    break;
                case PBEMoveEffect.Dig:
                    Ef_Dig(user, targets[0]);
                    break;
                case PBEMoveEffect.Dive:
                    Ef_Dive(user, targets[0]);
                    break;
                case PBEMoveEffect.Endeavor:
                    Ef_Endeavor(user, targets[0]);
                    break;
                case PBEMoveEffect.Fail:
                    Ef_Fail(user, move);
                    break;
                case PBEMoveEffect.Flatter:
                    Ef_Flatter(user, targets[0]);
                    break;
                case PBEMoveEffect.Fly:
                    Ef_Fly(user, targets[0]);
                    break;
                case PBEMoveEffect.FocusEnergy:
                    TryForceStatus2(user, targets, move, PBEStatus2.Pumped);
                    break;
                case PBEMoveEffect.Growth:
                    Ef_Growth(user);
                    break;
                case PBEMoveEffect.Hail:
                    TryForceWeather(user, move, PBEWeather.Hailstorm);
                    break;
                case PBEMoveEffect.Hit:
                    Ef_Hit(user, targets, move);
                    break;
                case PBEMoveEffect.Hit__MaybeBurn:
                    HitAndMaybeInflictStatus1(user, targets, move, PBEStatus1.Burned, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeConfuse:
                    Ef_Hit__MaybeConfuse(user, targets, move, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeFlinch:
                    Ef_Hit__MaybeFlinch(user, targets, move, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeFreeze:
                    HitAndMaybeInflictStatus1(user, targets, move, PBEStatus1.Frozen, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Accuracy }, new short[] { -1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_ATK_By1:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack }, new short[] { -1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Defense }, new short[] { -1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack }, new short[] { -1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpDefense }, new short[] { -1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpDefense }, new short[] { -2 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1:
                    HitAndMaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Speed }, new short[] { -1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeParalyze:
                    HitAndMaybeInflictStatus1(user, targets, move, PBEStatus1.Paralyzed, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybePoison:
                    HitAndMaybeInflictStatus1(user, targets, move, PBEStatus1.Poisoned, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeLowerUser_ATK_DEF_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense }, new short[] { -1, -1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Defense, PBEStat.SpDefense }, new short[] { -1, -1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2:
                    HitAndMaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack }, new short[] { -2 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeLowerUser_SPE_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Speed }, new short[] { -1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Speed, PBEStat.Defense, PBEStat.SpDefense }, new short[] { -1, -1, -1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeRaiseUser_ATK_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Attack }, new short[] { +1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense, PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed }, new short[] { +1, +1, +1, +1, +1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeRaiseUser_DEF_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Defense }, new short[] { +1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeRaiseUser_SPATK_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack }, new short[] { +1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeRaiseUser_SPE_By1:
                    HitAndMaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Speed }, new short[] { +1 }, mData.EffectParam);
                    break;
                case PBEMoveEffect.Hit__MaybeToxic:
                    HitAndMaybeInflictStatus1(user, targets, move, PBEStatus1.BadlyPoisoned, mData.EffectParam);
                    break;
                case PBEMoveEffect.LeechSeed:
                    TryForceStatus2(user, targets, move, PBEStatus2.LeechSeed);
                    break;
                case PBEMoveEffect.LightScreen:
                    TryForceTeamStatus(user, move, PBETeamStatus.LightScreen);
                    break;
                case PBEMoveEffect.LowerTarget_ATK_DEF_By1:
                    ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense }, new short[] { -1, -1 });
                    break;
                case PBEMoveEffect.LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.Defense, PBEStat.SpDefense, PBEStat.Attack, PBEStat.SpAttack, PBEStat.Speed }, new short[] { -1, -1, +2, +2, +2 });
                    break;
                case PBEMoveEffect.Magnitude:
                    Ef_Magnitude(user, targets);
                    break;
                case PBEMoveEffect.Minimize:
                    TryForceStatus2(user, targets, move, PBEStatus2.Minimized);
                    break;
                case PBEMoveEffect.Moonlight:
                    Ef_Moonlight(user);
                    break;
                case PBEMoveEffect.PainSplit:
                    Ef_PainSplit(user, targets[0]);
                    break;
                case PBEMoveEffect.Paralyze:
                    TryForceStatus1(user, targets, move, PBEStatus1.Paralyzed);
                    break;
                case PBEMoveEffect.Poison:
                    TryForceStatus1(user, targets, move, PBEStatus1.Poisoned);
                    break;
                case PBEMoveEffect.Protect:
                    TryForceStatus2(user, targets, move, PBEStatus2.Protected);
                    break;
                case PBEMoveEffect.PsychUp:
                    Ef_PsychUp(user, targets[0]);
                    break;
                case PBEMoveEffect.RainDance:
                    TryForceWeather(user, move, PBEWeather.Rain);
                    break;
                case PBEMoveEffect.RaiseUser_ATK_ACC_By1:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack, PBEStat.Accuracy }, new short[] { +1, +1 });
                    break;
                case PBEMoveEffect.RaiseUser_ATK_DEF_By1:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense }, new short[] { +1, +1 });
                    break;
                case PBEMoveEffect.RaiseUser_ATK_DEF_ACC_By1:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense, PBEStat.Accuracy }, new short[] { +1, +1, +1 });
                    break;
                case PBEMoveEffect.RaiseUser_ATK_SPATK_By1:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack, PBEStat.SpAttack }, new short[] { +1, +1 });
                    break;
                case PBEMoveEffect.RaiseUser_ATK_SPE_By1:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack, PBEStat.Speed }, new short[] { +1, +1 });
                    break;
                case PBEMoveEffect.RaiseUser_DEF_SPDEF_By1:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.Defense, PBEStat.SpDefense }, new short[] { +1, +1 });
                    break;
                case PBEMoveEffect.RaiseUser_SPATK_SPDEF_By1:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.SpAttack, PBEStat.SpDefense }, new short[] { +1, +1 });
                    break;
                case PBEMoveEffect.RaiseUser_SPATK_SPDEF_SPE_By1:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed }, new short[] { +1, +1, +1 });
                    break;
                case PBEMoveEffect.RaiseUser_SPE_By2_ATK_By1:
                    ChangeUserStats(user, move, new PBEStat[] { PBEStat.Speed, PBEStat.Attack }, new short[] { +2, +1 });
                    break;
                case PBEMoveEffect.Reflect:
                    TryForceTeamStatus(user, move, PBETeamStatus.Reflect);
                    break;
                case PBEMoveEffect.RestoreUserHealth:
                    Ef_RestoreUserHealth(user, move, mData.EffectParam);
                    break;
                case PBEMoveEffect.Sandstorm:
                    TryForceWeather(user, move, PBEWeather.Sandstorm);
                    break;
                case PBEMoveEffect.Sleep:
                    TryForceStatus1(user, targets, move, PBEStatus1.Asleep);
                    break;
                case PBEMoveEffect.Spikes:
                    TryForceTeamStatus(user, move, PBETeamStatus.Spikes);
                    break;
                case PBEMoveEffect.StealthRock:
                    TryForceTeamStatus(user, move, PBETeamStatus.StealthRock);
                    break;
                case PBEMoveEffect.Substitute:
                    TryForceStatus2(user, targets, move, PBEStatus2.Substitute);
                    break;
                case PBEMoveEffect.SunnyDay:
                    TryForceWeather(user, move, PBEWeather.HarshSunlight);
                    break;
                case PBEMoveEffect.Swagger:
                    Ef_Swagger(user, targets[0]);
                    break;
                case PBEMoveEffect.Toxic:
                    TryForceStatus1(user, targets, move, PBEStatus1.BadlyPoisoned);
                    break;
                case PBEMoveEffect.ToxicSpikes:
                    TryForceTeamStatus(user, move, PBETeamStatus.ToxicSpikes);
                    break;
                case PBEMoveEffect.Transform:
                    Ef_Transform(user, targets[0]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mData.Effect), $"Invalid move effect: {mData.Effect}");
            }
        }

        // Returns true if an attack gets cancelled from a status
        // Broadcasts status ending events & status causing immobility events
        bool PreMoveStatusCheck(PBEPokemon user, PBEMove move)
        {
            PBEMoveData mData = PBEMoveData.Data[move];

            // Increment counters first
            if (user.Status2.HasFlag(PBEStatus2.Confused))
            {
                user.ConfusionCounter++;
            }
            if (user.Status1 == PBEStatus1.Asleep)
            {
                user.Status1Counter++;
            }

            // Flinch happens before statuses
            // TODO: Put it under sleep
            if (user.Status2.HasFlag(PBEStatus2.Flinching))
            {
                BroadcastStatus2(user, user, PBEStatus2.Flinching, PBEStatusAction.Activated);
                return true;
            }

            // Major statuses
            switch (user.Status1)
            {
                case PBEStatus1.Asleep:
                    // Check if we can wake up
                    if (user.Status1Counter > user.SleepTurns)
                    {
                        user.Status1 = PBEStatus1.None;
                        user.Status1Counter = user.SleepTurns = 0;
                        BroadcastStatus1(user, user, PBEStatus1.Asleep, PBEStatusAction.Ended);
                    }
                    else
                    {
                        BroadcastStatus1(user, user, PBEStatus1.Asleep, PBEStatusAction.Activated);
                        return true;
                    }
                    break;
                case PBEStatus1.Frozen:
                    // Some moves always defrost the user, but if they don't, there is a 20% chance to thaw out
                    if (mData.Flags.HasFlag(PBEMoveFlag.DefrostsUser) || PBEUtils.ApplyChance(20, 100))
                    {
                        user.Status1 = PBEStatus1.None;
                        BroadcastStatus1(user, user, PBEStatus1.Frozen, PBEStatusAction.Ended);
                    }
                    else
                    {
                        BroadcastStatus1(user, user, PBEStatus1.Frozen, PBEStatusAction.Activated);
                        return true;
                    }
                    break;
                case PBEStatus1.Paralyzed:
                    // 25% chance to be unable to move
                    if (PBEUtils.ApplyChance(25, 100))
                    {
                        BroadcastStatus1(user, user, PBEStatus1.Paralyzed, PBEStatusAction.Activated);
                        return true;
                    }
                    break;
            }

            // Minor statuses
            if (user.Status2.HasFlag(PBEStatus2.Confused))
            {
                // Check if we snap out of confusion
                if (user.ConfusionCounter > user.ConfusionTurns)
                {
                    user.Status2 &= ~PBEStatus2.Confused;
                    user.ConfusionCounter = user.ConfusionTurns = 0;
                    BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Ended);
                }
                else
                {
                    BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Activated);
                    // 50% chance to hit itself
                    if (PBEUtils.ApplyChance(50, 100))
                    {
                        DealDamage(user, user, CalculateDamage(user, user, PBEMove.None, PBEType.None, PBEMoveCategory.Physical, 40, true, true), PBEEffectiveness.Normal, true);
                        BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Damage);
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
        bool MissCheck(PBEPokemon user, PBEPokemon target, PBEMove move)
        {
            PBEMoveData mData = PBEMoveData.Data[move];

            if (target.Status2.HasFlag(PBEStatus2.Protected) && mData.Flags.HasFlag(PBEMoveFlag.AffectedByProtect))
            {
                BroadcastStatus2(user, target, PBEStatus2.Protected, PBEStatusAction.Activated);
                return true;
            }
            // No Guard always hits
            if (user.Ability == PBEAbility.NoGuard || target.Ability == PBEAbility.NoGuard)
            {
                return false;
            }
            // Hitting airborne opponents
            if (target.Status2.HasFlag(PBEStatus2.Airborne) && !mData.Flags.HasFlag(PBEMoveFlag.HitsAirborne))
            {
                goto miss;
            }
            // Hitting underground opponents
            if (target.Status2.HasFlag(PBEStatus2.Underground) && !mData.Flags.HasFlag(PBEMoveFlag.HitsUnderground))
            {
                goto miss;
            }
            // Hitting underwater opponents
            if (target.Status2.HasFlag(PBEStatus2.Underwater) && !mData.Flags.HasFlag(PBEMoveFlag.HitsUnderwater))
            {
                goto miss;
            }
            // Moves that always hit
            if (mData.Accuracy == 0)
            {
                return false;
            }
            double chance = mData.Accuracy;
            chance *= GetStatMultiplier(user.AccuracyChange, true) / GetStatMultiplier(target.EvasionChange, true);
            // Pokémon with Compoundeyes get a 30% Accuracy boost
            if (user.Ability == PBEAbility.Compoundeyes)
            {
                chance *= 1.3;
            }
            // Pokémon with Hustle get a 20% Accuracy reduction for Physical moves
            if (user.Ability == PBEAbility.Hustle && mData.Category == PBEMoveCategory.Physical)
            {
                chance *= 0.8;
            }
            if (Weather == PBEWeather.Sandstorm && user.Ability == PBEAbility.SandVeil)
            {
                chance *= 0.8;
            }
            if (Weather == PBEWeather.Hailstorm && user.Ability == PBEAbility.SnowCloak)
            {
                chance *= 0.8;
            }
            // Pokémon holding a BrightPowder or Lax Incense get a 10% Evasion boost
            if (target.Item == PBEItem.BrightPowder)
            {
                chance *= 0.9;
            }
            // Pokémon holding a Wide Lens get a 10% Accuracy boost
            if (user.Item == PBEItem.WideLens)
            {
                chance *= 1.1;
            }
            // Try to hit
            if (PBEUtils.ApplyChance((int)chance, 100))
            {
                return false;
            }
        miss:
            BroadcastMoveMissed(user);
            return true;
        }

        // Returns true if a critical hit was determined
        bool CritCheck(PBEPokemon user, PBEPokemon target, PBEMove move, ref double damageMultiplier)
        {
            // If critical hits cannot be landed, return false
            if (target.Ability == PBEAbility.BattleArmor
                || target.Ability == PBEAbility.ShellArmor)
            {
                return false;
            }

            PBEMoveData mData = PBEMoveData.Data[move];
            byte stage = 0;

            if (mData.Flags.HasFlag(PBEMoveFlag.HighCritChance))
            {
                stage += 1;
            }
            if (user.Ability == PBEAbility.SuperLuck)
            {
                stage += 1;
            }
            if (user.Item == PBEItem.RazorClaw || user.Item == PBEItem.ScopeLens)
            {
                stage += 1;
            }
            if (user.Status2.HasFlag(PBEStatus2.Pumped))
            {
                stage += 2;
            }
            if (user.Shell.Species == PBESpecies.Farfetchd && user.Item == PBEItem.Stick)
            {
                stage += 2;
            }

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
            if (mData.Flags.HasFlag(PBEMoveFlag.AlwaysCrit)
                || PBEUtils.ApplyChance((int)(chance * 100), 100 * 100))
            {
                damageMultiplier *= Settings.CritMultiplier;
                if (user.Ability == PBEAbility.Sniper)
                {
                    damageMultiplier *= 1.5;
                }
                return true;
            }
            return false;
        }

        // Returns amount of damage done
        // Broadcasts the hp changing, effectiveness, substitute
        ushort DealDamage(PBEPokemon culprit, PBEPokemon victim, ushort hp, PBEEffectiveness effectiveness, bool ignoreSubstitute)
        {
            if (effectiveness == PBEEffectiveness.Ineffective)
            {
                BroadcastEffectiveness(victim, effectiveness);
                return 0;
            }
            if (!ignoreSubstitute && victim.Status2.HasFlag(PBEStatus2.Substitute))
            {
                ushort oldHP = victim.SubstituteHP;
                victim.SubstituteHP = (ushort)Math.Max(0, victim.SubstituteHP - Math.Max((ushort)1, hp)); // Always lose at least 1 HP
                ushort damageAmt = (ushort)(oldHP - victim.SubstituteHP);
                BroadcastStatus2(culprit, victim, PBEStatus2.Substitute, PBEStatusAction.Damage);
                BroadcastEffectiveness(victim, effectiveness);
                if (victim.SubstituteHP == 0)
                {
                    victim.Status2 &= ~PBEStatus2.Substitute;
                    BroadcastStatus2(culprit, victim, PBEStatus2.Substitute, PBEStatusAction.Ended);
                }
                return damageAmt;
            }
            else
            {
                ushort oldHP = victim.HP;
                victim.HP = (ushort)Math.Max(0, victim.HP - Math.Max((ushort)1, hp)); // Always lose at least 1 HP
                ushort damageAmt = (ushort)(oldHP - victim.HP);
                BroadcastPkmnHPChanged(victim, -damageAmt);
                BroadcastEffectiveness(victim, effectiveness);
                return damageAmt;
            }
        }

        // Returns amount of hp healed
        // Broadcasts the event if it healed more than 0
        ushort HealDamage(PBEPokemon pkmn, ushort hp)
        {
            ushort oldHP = pkmn.HP;
            pkmn.HP = (ushort)Math.Min(pkmn.MaxHP, pkmn.HP + Math.Max((ushort)1, hp)); // Always try to heal at least 1 HP
            ushort healAmt = (ushort)(pkmn.HP - oldHP);
            if (healAmt > 0)
            {
                BroadcastPkmnHPChanged(pkmn, healAmt);
            }
            return healAmt;
        }

        // Will cure Paralysis if the Pokémon has Limber
        void LimberCheck(PBEPokemon pkmn)
        {
            if (pkmn.Ability == PBEAbility.Limber && pkmn.Status1 == PBEStatus1.Paralyzed)
            {
                pkmn.Status1 = PBEStatus1.None;
                BroadcastAbility(pkmn, pkmn, PBEAbility.Limber, PBEAbilityAction.CuredStatus);
                BroadcastStatus1(pkmn, pkmn, PBEStatus1.Paralyzed, PBEStatusAction.Cured);
            }
        }
        // Will consume a held Power Herb
        bool PowerHerbCheck(PBEPokemon pkmn)
        {
            if (pkmn.Item == PBEItem.PowerHerb)
            {
                pkmn.Item = PBEItem.None;
                BroadcastItem(pkmn, pkmn, PBEItem.PowerHerb, PBEItemAction.Consumed);
                return true;
            }
            return false;
        }

        // Broadcasts the event
        void PPReduce(PBEPokemon pkmn, PBEMove move)
        {
            int moveIndex = Array.IndexOf(pkmn.Moves, move);
            int amtToReduce = 1;
            // TODO: If target is not self and has pressure
            byte oldPP = pkmn.PP[moveIndex];
            pkmn.PP[moveIndex] = (byte)Math.Max(0, pkmn.PP[moveIndex] - amtToReduce);
            int change = oldPP - pkmn.PP[moveIndex];
            BroadcastMovePPChanged(pkmn, move, (short)-change);
        }

        // Returns true if the Pokémon fainted & removes it from activeBattlers
        // Broadcasts the event if it fainted
        bool FaintCheck(PBEPokemon pkmn)
        {
            if (pkmn.HP < 1)
            {
                ActiveBattlers.Remove(pkmn);
                pkmn.FieldPosition = PBEFieldPosition.None;
                BroadcastPkmnFainted(pkmn);
                return true;
            }
            return false;
        }

        public static unsafe void ApplyStatChange(PBEBattle battle, PBEPokemon pkmn, PBEStat stat, short change, bool broadcast = true, bool ignoreSimple = false)
        {
            if (!ignoreSimple && pkmn.Ability == PBEAbility.Simple)
            {
                change *= 2;
            }
            bool isTooMuch = false;
            fixed (sbyte* ptr = &pkmn.AttackChange)
            {
                sbyte* scPtr = ptr + (stat - PBEStat.Attack); // Points to the proper stat change sbyte
                if (*scPtr <= -battle.Settings.MaxStatChange || *scPtr >= battle.Settings.MaxStatChange)
                {
                    isTooMuch = true;
                }
                else
                {
                    *scPtr = (sbyte)PBEUtils.Clamp(*scPtr + change, -battle.Settings.MaxStatChange, battle.Settings.MaxStatChange);
                }
            }
            if (broadcast)
            {
                battle.BroadcastPkmnStatChanged(pkmn, stat, change, isTooMuch);
            }
        }

        // Returns true if the status was applied
        // Broadcasts the change if applied
        bool ApplyStatus1IfPossible(PBEPokemon user, PBEPokemon target, PBEStatus1 status, bool broadcastFailOrEffectiveness)
        {
            // Cannot change status if already afflicted
            // Cannot change status of a target behind a substitute
            if (target.Status1 != PBEStatus1.None
                || target.Status2.HasFlag(PBEStatus2.Substitute))
            {
                if (broadcastFailOrEffectiveness)
                {
                    BroadcastMoveFailed(user, target, PBEFailReason.Default);
                }
                return false;
            }
            // A Pokémon with Limber cannot be Paralyzed unless the attacker has Mold Breaker
            if (status == PBEStatus1.Paralyzed && target.Ability == PBEAbility.Limber)
            {
                if (broadcastFailOrEffectiveness)
                {
                    BroadcastAbility(target, target, PBEAbility.Limber, PBEAbilityAction.PreventedStatus);
                    BroadcastEffectiveness(target, PBEEffectiveness.Ineffective);
                }
                return false;
            }
            // An Ice type Pokémon cannot be Frozen
            if (status == PBEStatus1.Frozen && target.HasType(PBEType.Ice))
            {
                if (broadcastFailOrEffectiveness)
                {
                    BroadcastEffectiveness(target, PBEEffectiveness.Ineffective);
                }
                return false;
            }
            // A Fire type Pokémon cannot be Burned
            if (status == PBEStatus1.Burned && target.HasType(PBEType.Fire))
            {
                if (broadcastFailOrEffectiveness)
                {
                    BroadcastEffectiveness(target, PBEEffectiveness.Ineffective);
                }
                return false;
            }
            // A Poison or Steel type Pokémon cannot be Poisoned or Badly Poisoned
            if ((status == PBEStatus1.BadlyPoisoned || status == PBEStatus1.Poisoned) && (target.HasType(PBEType.Poison) || target.HasType(PBEType.Steel)))
            {
                if (broadcastFailOrEffectiveness)
                {
                    BroadcastEffectiveness(target, PBEEffectiveness.Ineffective);
                }
                return false;
            }

            target.Status1 = status;
            // Start toxic counter
            if (status == PBEStatus1.BadlyPoisoned)
            {
                target.Status1Counter = 1;
            }
            // Set sleep length
            if (status == PBEStatus1.Asleep)
            {
                target.SleepTurns = (byte)PBEUtils.RNG.Next(Settings.SleepMinTurns, Settings.SleepMaxTurns + 1);
            }
            BroadcastStatus1(user, target, status, PBEStatusAction.Added);
            return true;
        }
        // Returns true if the status was applied
        // Broadcasts the change if applied and required
        bool ApplyStatus2IfPossible(PBEPokemon user, PBEPokemon target, PBEStatus2 status, bool broadcastFail, PBEFailReason failReason = PBEFailReason.Default)
        {
            switch (status)
            {
                case PBEStatus2.Confused:
                    if (!target.Status2.HasFlag(PBEStatus2.Confused)
                        && !target.Status2.HasFlag(PBEStatus2.Substitute))
                    {
                        target.Status2 |= PBEStatus2.Confused;
                        target.ConfusionTurns = (byte)PBEUtils.RNG.Next(Settings.ConfusionMinTurns, Settings.ConfusionMaxTurns + 1);
                        BroadcastStatus2(user, target, PBEStatus2.Confused, PBEStatusAction.Added);
                        return true;
                    }
                    break;
                case PBEStatus2.Cursed:
                    if (!target.Status2.HasFlag(PBEStatus2.Cursed))
                    {
                        target.Status2 |= PBEStatus2.Cursed;
                        BroadcastStatus2(user, target, PBEStatus2.Cursed, PBEStatusAction.Added);
                        DealDamage(user, user, (ushort)(user.MaxHP / 2), PBEEffectiveness.Normal, true);
                        FaintCheck(user);
                        return true;
                    }
                    break;
                case PBEStatus2.Flinching:
                    if (!target.Status2.HasFlag(PBEStatus2.Substitute))
                    {
                        target.Status2 |= status;
                        return true;
                    }
                    break;
                case PBEStatus2.LeechSeed:
                    if (!target.Status2.HasFlag(PBEStatus2.LeechSeed)
                        && !target.Status2.HasFlag(PBEStatus2.Substitute)
                        && !target.HasType(PBEType.Grass))
                    {
                        target.Status2 |= PBEStatus2.LeechSeed;
                        target.SeededPosition = user.FieldPosition;
                        BroadcastStatus2(user, target, PBEStatus2.LeechSeed, PBEStatusAction.Added);
                        return true;
                    }
                    break;
                case PBEStatus2.Minimized:
                    user.Status2 |= PBEStatus2.Minimized;
                    BroadcastStatus2(user, user, PBEStatus2.Minimized, PBEStatusAction.Added);
                    ApplyStatChange(this, user, PBEStat.Evasion, +2);
                    return true;
                case PBEStatus2.Protected:
                    {
                        // TODO: If the user goes last, fail
                        ushort chance = ushort.MaxValue;
                        for (int i = 0; i < user.ProtectCounter; i++)
                        {
                            chance /= 2;
                        }

                        if (PBEUtils.ApplyChance(chance, ushort.MaxValue))
                        {
                            user.Status2 |= PBEStatus2.Protected;
                            user.ProtectCounter++;
                            BroadcastStatus2(user, user, PBEStatus2.Protected, PBEStatusAction.Added);
                            return true;
                        }
                        user.ProtectCounter = 0;
                    }
                    break;
                case PBEStatus2.Pumped:
                    if (!user.Status2.HasFlag(PBEStatus2.Pumped))
                    {
                        user.Status2 |= status;
                        BroadcastStatus2(user, user, PBEStatus2.Pumped, PBEStatusAction.Added);
                        return true;
                    }
                    break;
                case PBEStatus2.Substitute:
                    {
                        ushort hpRequired = (ushort)(user.MaxHP / 4);
                        if (!user.Status2.HasFlag(PBEStatus2.Substitute) && hpRequired > 0 && user.HP > hpRequired)
                        {
                            DealDamage(user, user, hpRequired, PBEEffectiveness.Normal, true);
                            user.Status2 |= PBEStatus2.Substitute;
                            user.SubstituteHP = hpRequired;
                            BroadcastStatus2(user, user, PBEStatus2.Substitute, PBEStatusAction.Added);
                            return true;
                        }
                    }
                    break;
            }
            if (broadcastFail)
            {
                BroadcastMoveFailed(user, target, failReason);
            }
            return false;
        }

        void Ef_Hit(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PBEPokemon target in targets)
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
                TypeCheck(user, target, move, out PBEType moveType, out PBEEffectiveness effectiveness, ref damageMultiplier);
                ushort damage = CalculateDamage(user, target, move, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastMoveCrit();
                }
                FaintCheck(target);
            }
        }
        // TODO: Convert the following into HitAndMaybeInflictStatus2
        void Ef_Hit__MaybeConfuse(PBEPokemon user, PBEPokemon[] targets, PBEMove move, int chanceToConfuse)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PBEPokemon target in targets)
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
                TypeCheck(user, target, move, out PBEType moveType, out PBEEffectiveness effectiveness, ref damageMultiplier);
                bool behindSubstitute = target.Status2.HasFlag(PBEStatus2.Substitute);
                ushort damage = CalculateDamage(user, target, move, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastMoveCrit();
                }
                if (FaintCheck(target))
                {
                    continue;
                }
                if (effectiveness != PBEEffectiveness.Ineffective && !behindSubstitute && PBEUtils.ApplyChance(chanceToConfuse, 100))
                {
                    ApplyStatus2IfPossible(user, target, PBEStatus2.Confused, false);
                }
            }
        }
        void Ef_Hit__MaybeFlinch(PBEPokemon user, PBEPokemon[] targets, PBEMove move, int chanceToFlinch)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PBEPokemon target in targets)
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
                TypeCheck(user, target, move, out PBEType moveType, out PBEEffectiveness effectiveness, ref damageMultiplier);
                bool behindSubstitute = target.Status2.HasFlag(PBEStatus2.Substitute);
                ushort damage = CalculateDamage(user, target, move, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastMoveCrit();
                }
                if (FaintCheck(target))
                {
                    continue;
                }
                if (effectiveness != PBEEffectiveness.Ineffective && !behindSubstitute && PBEUtils.ApplyChance(chanceToFlinch, 100))
                {
                    ApplyStatus2IfPossible(user, target, PBEStatus2.Flinching, false);
                }
            }
        }

        void TryForceStatus1(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStatus1 status)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PBEPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, move))
                {
                    continue;
                }
                double d = 1.0;
                TypeCheck(user, target, move, out PBEType moveType, out PBEEffectiveness effectiveness, ref d);
                if (effectiveness == PBEEffectiveness.Ineffective) // Paralysis, Normalize
                {
                    BroadcastEffectiveness(target, effectiveness);
                }
                else
                {
                    ApplyStatus1IfPossible(user, target, status, true);
                }
            }
        }
        void TryForceStatus2(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStatus2 status)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PBEPokemon target in targets)
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
        void TryForceTeamStatus(PBEPokemon user, PBEMove move, PBETeamStatus status)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            PBETeam userTeam = Teams[user.LocalTeam ? 0 : 1];
            PBETeam opposingTeam = Teams[user.LocalTeam ? 1 : 0];
            switch (status)
            {
                case PBETeamStatus.LightScreen:
                    if (!userTeam.Status.HasFlag(PBETeamStatus.LightScreen))
                    {
                        userTeam.Status |= PBETeamStatus.LightScreen;
                        userTeam.LightScreenCount = (byte)(Settings.LightScreenTurns + (user.Item == PBEItem.LightClay ? Settings.LightClayTurnExtension : 0));
                        BroadcastTeamStatus(userTeam.LocalTeam, PBETeamStatus.LightScreen, PBETeamStatusAction.Added);
                        return;
                    }
                    break;
                case PBETeamStatus.Reflect:
                    if (!userTeam.Status.HasFlag(PBETeamStatus.Reflect))
                    {
                        userTeam.Status |= PBETeamStatus.Reflect;
                        userTeam.ReflectCount = (byte)(Settings.ReflectTurns + (user.Item == PBEItem.LightClay ? Settings.LightClayTurnExtension : 0));
                        BroadcastTeamStatus(userTeam.LocalTeam, PBETeamStatus.Reflect, PBETeamStatusAction.Added);
                        return;
                    }
                    break;
                case PBETeamStatus.Spikes:
                    if (opposingTeam.SpikeCount < 3)
                    {
                        opposingTeam.Status |= PBETeamStatus.Spikes;
                        opposingTeam.SpikeCount++;
                        BroadcastTeamStatus(opposingTeam.LocalTeam, PBETeamStatus.Spikes, PBETeamStatusAction.Added);
                        return;
                    }
                    break;
                case PBETeamStatus.StealthRock:
                    if (!opposingTeam.Status.HasFlag(PBETeamStatus.StealthRock))
                    {
                        opposingTeam.Status |= PBETeamStatus.StealthRock;
                        BroadcastTeamStatus(opposingTeam.LocalTeam, PBETeamStatus.StealthRock, PBETeamStatusAction.Added);
                        return;
                    }
                    break;
                case PBETeamStatus.ToxicSpikes:
                    if (opposingTeam.ToxicSpikeCount < 2)
                    {
                        opposingTeam.Status |= PBETeamStatus.ToxicSpikes;
                        opposingTeam.ToxicSpikeCount++;
                        BroadcastTeamStatus(opposingTeam.LocalTeam, PBETeamStatus.ToxicSpikes, PBETeamStatusAction.Added);
                        return;
                    }
                    break;
            }
            BroadcastMoveFailed(user, user, PBEFailReason.Default);
        }
        void TryForceWeather(PBEPokemon user, PBEMove move, PBEWeather weather)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (Weather == weather)
            {
                BroadcastMoveFailed(user, user, PBEFailReason.Default);
            }
            else
            {
                byte turns;
                PBEItem extensionItem;
                byte itemTurnExtension;
                switch (weather)
                {
                    case PBEWeather.Hailstorm:
                        turns = Settings.HailTurns;
                        extensionItem = PBEItem.IcyRock;
                        itemTurnExtension = Settings.IcyRockTurnExtension;
                        break;
                    case PBEWeather.HarshSunlight:
                        turns = Settings.SunTurns;
                        extensionItem = PBEItem.HeatRock;
                        itemTurnExtension = Settings.HeatRockTurnExtension;
                        break;
                    case PBEWeather.Rain:
                        turns = Settings.RainTurns;
                        extensionItem = PBEItem.DampRock;
                        itemTurnExtension = Settings.DampRockTurnExtension;
                        break;
                    case PBEWeather.Sandstorm:
                        turns = Settings.SandstormTurns;
                        extensionItem = PBEItem.SmoothRock;
                        itemTurnExtension = Settings.SmoothRockTurnExtension;
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(weather), $"Invalid weather: {weather}");
                }

                Weather = weather;
                WeatherCounter = (byte)(turns + (user.Item == extensionItem ? itemTurnExtension : 0));
                BroadcastWeather(Weather, PBEWeatherAction.Added);
            }
        }
        void HitAndMaybeInflictStatus1(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStatus1 status, int chanceToInflict)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PBEPokemon target in targets)
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
                TypeCheck(user, target, move, out PBEType moveType, out PBEEffectiveness effectiveness, ref damageMultiplier);
                bool behindSubstitute = target.Status2.HasFlag(PBEStatus2.Substitute);
                ushort damage = CalculateDamage(user, target, move, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastMoveCrit();
                }
                if (FaintCheck(target))
                {
                    continue;
                }
                if (effectiveness != PBEEffectiveness.Ineffective && !behindSubstitute && PBEUtils.ApplyChance(chanceToInflict, 100))
                {
                    ApplyStatus1IfPossible(user, target, status, false);
                }
            }
        }

        void ChangeTargetStats(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStat[] stats, short[] changes)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PBEPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, move))
                {
                    continue;
                }
                if (target.Status2.HasFlag(PBEStatus2.Substitute))
                {
                    BroadcastMoveFailed(user, target, PBEFailReason.Default);
                }
                else
                {
                    for (int i = 0; i < stats.Length; i++)
                    {
                        ApplyStatChange(this, target, stats[i], changes[i]);
                    }
                }
            }
        }
        void ChangeUserStats(PBEPokemon user, PBEMove move, PBEStat[] stats, short[] changes)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            for (int i = 0; i < stats.Length; i++)
            {
                ApplyStatChange(this, user, stats[i], changes[i]);
            }
        }
        void HitAndMaybeChangeTargetStats(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStat[] stats, short[] changes, int chanceToChangeStats)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PBEPokemon target in targets)
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
                TypeCheck(user, target, move, out PBEType moveType, out PBEEffectiveness effectiveness, ref damageMultiplier);
                bool behindSubstitute = target.Status2.HasFlag(PBEStatus2.Substitute);
                ushort damage = CalculateDamage(user, target, move, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastMoveCrit();
                }
                if (FaintCheck(target))
                {
                    continue;
                }
                if (effectiveness != PBEEffectiveness.Ineffective && !behindSubstitute && PBEUtils.ApplyChance(chanceToChangeStats, 100))
                {
                    for (int i = 0; i < stats.Length; i++)
                    {
                        ApplyStatChange(this, target, stats[i], changes[i]);
                    }
                }
            }
        }
        void HitAndMaybeChangeUserStats(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStat[] stats, short[] changes, int chanceToChangeStats)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            byte hit = 0;
            foreach (PBEPokemon target in targets)
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
                TypeCheck(user, target, move, out PBEType moveType, out PBEEffectiveness effectiveness, ref damageMultiplier);
                ushort damage = CalculateDamage(user, target, move, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastMoveCrit();
                }
                FaintCheck(target);
                if (effectiveness != PBEEffectiveness.Ineffective)
                {
                    hit++;
                }
            }
            if (hit > 0)
            {
                if (PBEUtils.ApplyChance(chanceToChangeStats, 100))
                {
                    for (int i = 0; i < stats.Length; i++)
                    {
                        ApplyStatChange(this, user, stats[i], changes[i]);
                    }
                }
            }
        }

        void Ef_Fail(PBEPokemon user, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            BroadcastMoveFailed(user, user, PBEFailReason.Default);
        }
        void Ef_BrickBreak(PBEPokemon user, PBEPokemon target)
        {
            BroadcastMoveUsed(user, PBEMove.BrickBreak);
            PPReduce(user, PBEMove.BrickBreak);
            if (MissCheck(user, target, PBEMove.BrickBreak))
            {
                return;
            }
            double damageMultiplier = 1.0;
            bool crit = CritCheck(user, target, PBEMove.BrickBreak, ref damageMultiplier);
            TypeCheck(user, target, PBEMove.BrickBreak, out PBEType moveType, out PBEEffectiveness effectiveness, ref damageMultiplier);
            if (effectiveness == PBEEffectiveness.Ineffective)
            {
                BroadcastEffectiveness(target, effectiveness);
            }
            else
            {
                PBETeam team = Teams[target.LocalTeam ? 0 : 1];
                if (team.Status.HasFlag(PBETeamStatus.Reflect))
                {
                    team.Status &= ~PBETeamStatus.Reflect;
                    team.ReflectCount = 0;
                    BroadcastTeamStatus(team.LocalTeam, PBETeamStatus.Reflect, PBETeamStatusAction.Cleared);
                }
                if (team.Status.HasFlag(PBETeamStatus.LightScreen))
                {
                    team.Status &= ~PBETeamStatus.LightScreen;
                    team.LightScreenCount = 0;
                    BroadcastTeamStatus(team.LocalTeam, PBETeamStatus.LightScreen, PBETeamStatusAction.Cleared);
                }
                ushort damage = CalculateDamage(user, target, PBEMove.BrickBreak, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastMoveCrit();
                }
                FaintCheck(target);
            }
        }
        void Ef_Dig(PBEPokemon user, PBEPokemon target)
        {
            BroadcastMoveUsed(user, PBEMove.Dig);
            PPReduce(user, PBEMove.Dig);
        top:
            if (user.Status2.HasFlag(PBEStatus2.Underground))
            {
                user.LockedAction.Decision = PBEDecision.None;
                user.Status2 &= ~PBEStatus2.Underground;
                BroadcastStatus2(user, user, PBEStatus2.Underground, PBEStatusAction.Ended);
                if (MissCheck(user, target, PBEMove.Dig))
                {
                    return;
                }

                double damageMultiplier = 1.0;
                bool crit = CritCheck(user, target, PBEMove.Dig, ref damageMultiplier);
                TypeCheck(user, target, PBEMove.Dig, out PBEType moveType, out PBEEffectiveness effectiveness, ref damageMultiplier);
                ushort damage = CalculateDamage(user, target, PBEMove.Dig, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastMoveCrit();
                }
                FaintCheck(target);
            }
            else
            {
                user.LockedAction = user.SelectedAction;
                user.Status2 |= PBEStatus2.Underground;
                BroadcastStatus2(user, user, PBEStatus2.Underground, PBEStatusAction.Added);
                if (PowerHerbCheck(user))
                {
                    goto top;
                }
            }
        }
        void Ef_Dive(PBEPokemon user, PBEPokemon target)
        {
            BroadcastMoveUsed(user, PBEMove.Dive);
            PPReduce(user, PBEMove.Dive);
        top:
            if (user.Status2.HasFlag(PBEStatus2.Underwater))
            {
                user.LockedAction.Decision = PBEDecision.None;
                user.Status2 &= ~PBEStatus2.Underwater;
                BroadcastStatus2(user, user, PBEStatus2.Underwater, PBEStatusAction.Ended);
                if (MissCheck(user, target, PBEMove.Dive))
                {
                    return;
                }
                double damageMultiplier = 1.0;
                bool crit = CritCheck(user, target, PBEMove.Dive, ref damageMultiplier);
                TypeCheck(user, target, PBEMove.Dive, out PBEType moveType, out PBEEffectiveness effectiveness, ref damageMultiplier);
                ushort damage = CalculateDamage(user, target, PBEMove.Dive, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastMoveCrit();
                }
                FaintCheck(target);
            }
            else
            {
                user.LockedAction = user.SelectedAction;
                user.Status2 |= PBEStatus2.Underwater;
                BroadcastStatus2(user, user, PBEStatus2.Underwater, PBEStatusAction.Added);
                if (PowerHerbCheck(user))
                {
                    goto top;
                }
            }
        }
        void Ef_Fly(PBEPokemon user, PBEPokemon target)
        {
            BroadcastMoveUsed(user, PBEMove.Fly);
            PPReduce(user, PBEMove.Fly);
        top:
            if (user.Status2.HasFlag(PBEStatus2.Airborne))
            {
                user.LockedAction.Decision = PBEDecision.None;
                user.Status2 &= ~PBEStatus2.Airborne;
                BroadcastStatus2(user, user, PBEStatus2.Airborne, PBEStatusAction.Ended);
                if (MissCheck(user, target, PBEMove.Fly))
                {
                    return;
                }
                double damageMultiplier = 1.0;
                bool crit = CritCheck(user, target, PBEMove.Fly, ref damageMultiplier);
                TypeCheck(user, target, PBEMove.Fly, out PBEType moveType, out PBEEffectiveness effectiveness, ref damageMultiplier);
                ushort damage = CalculateDamage(user, target, PBEMove.Fly, moveType, criticalHit: crit);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastMoveCrit();
                }
                FaintCheck(target);
            }
            else
            {
                user.LockedAction = user.SelectedAction;
                user.Status2 |= PBEStatus2.Airborne;
                BroadcastStatus2(user, user, PBEStatus2.Airborne, PBEStatusAction.Added);
                if (PowerHerbCheck(user))
                {
                    goto top;
                }
            }
        }
        void Ef_Flatter(PBEPokemon user, PBEPokemon target)
        {
            BroadcastMoveUsed(user, PBEMove.Flatter);
            PPReduce(user, PBEMove.Flatter);
            if (MissCheck(user, target, PBEMove.Flatter))
            {
                return;
            }
            ApplyStatChange(this, target, PBEStat.SpAttack, +1);
            ApplyStatus2IfPossible(user, target, PBEStatus2.Confused, true, PBEFailReason.AlreadyConfused);
        }
        void Ef_Swagger(PBEPokemon user, PBEPokemon target)
        {
            BroadcastMoveUsed(user, PBEMove.Swagger);
            PPReduce(user, PBEMove.Swagger);
            if (MissCheck(user, target, PBEMove.Swagger))
            {
                return;
            }
            ApplyStatChange(this, target, PBEStat.Attack, +2);
            ApplyStatus2IfPossible(user, target, PBEStatus2.Confused, true, PBEFailReason.AlreadyConfused);
        }
        void Ef_Growth(PBEPokemon user)
        {
            short change = (short)(Weather == PBEWeather.HarshSunlight ? +2 : +1);
            ChangeUserStats(user, PBEMove.Growth, new PBEStat[] { PBEStat.Attack, PBEStat.SpAttack }, new short[] { change, change });
        }
        void Ef_RestoreUserHealth(PBEPokemon user, PBEMove move, int percent)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (HealDamage(user, (ushort)(user.MaxHP * (percent / 100D))) == 0)
            {
                BroadcastMoveFailed(user, user, PBEFailReason.HPFull);
            }
        }
        void Ef_Moonlight(PBEPokemon user)
        {
            BroadcastMoveUsed(user, PBEMove.Moonlight);
            PPReduce(user, PBEMove.Moonlight);
            double percentage;
            switch (Weather)
            {
                case PBEWeather.None: percentage = 0.5; break;
                case PBEWeather.HarshSunlight: percentage = 0.66; break;
                default: percentage = 0.25; break;
            }
            ushort hp = (ushort)(user.MaxHP * percentage);
            if (HealDamage(user, hp) == 0)
            {
                BroadcastMoveFailed(user, user, PBEFailReason.HPFull);
            }
        }
        void Ef_Transform(PBEPokemon user, PBEPokemon target)
        {
            BroadcastMoveUsed(user, PBEMove.Transform);
            PPReduce(user, PBEMove.Transform);
            if (user.Status2.HasFlag(PBEStatus2.Transformed)
                || target.Status2.HasFlag(PBEStatus2.Transformed)
                || target.Status2.HasFlag(PBEStatus2.Substitute))
            {
                BroadcastMoveFailed(user, target, PBEFailReason.Default);
                return;
            }
            if (MissCheck(user, target, PBEMove.Transform))
            {
                return;
            }
            user.Transform(target, Settings, target.Attack, target.Defense, target.SpAttack, target.SpDefense, target.Speed, target.Ability, target.Type1, target.Type2, target.Moves);
            BroadcastTransform(user, target);
        }
        void Ef_Curse(PBEPokemon user, PBEPokemon target)
        {
            BroadcastMoveUsed(user, PBEMove.Curse);
            PPReduce(user, PBEMove.Curse);
            if (user.HasType(PBEType.Ghost))
            {
                if (target == user) // Just gained the Ghost type after selecting the move, so get a target
                {
                    PBEFieldPosition prioritizedPos = GetPositionAcross(BattleFormat, user.FieldPosition);
                    PBETarget t;
                    if (prioritizedPos == PBEFieldPosition.Left)
                    {
                        t = PBETarget.FoeLeft;
                    }
                    else if (prioritizedPos == PBEFieldPosition.Center)
                    {
                        t = PBETarget.FoeCenter;
                    }
                    else
                    {
                        t = PBETarget.FoeRight;
                    }

                    PBEPokemon[] targets = GetRuntimeTargets(user, t, false);
                    if (targets.Length == 0)
                    {
                        BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
                        return;
                    }
                    target = targets[0];
                }
                if (!MissCheck(user, target, PBEMove.Curse))
                {
                    ApplyStatus2IfPossible(user, target, PBEStatus2.Cursed, true);
                }
            }
            else
            {
                if (user.SpeedChange == -Settings.MaxStatChange
                    && user.AttackChange == Settings.MaxStatChange
                    && user.DefenseChange == Settings.MaxStatChange)
                {
                    BroadcastMoveFailed(user, target, PBEFailReason.Default);
                }
                else
                {
                    ApplyStatChange(this, user, PBEStat.Speed, -1);
                    ApplyStatChange(this, user, PBEStat.Attack, +1);
                    ApplyStatChange(this, user, PBEStat.Defense, +1);
                }
            }
        }
        void Ef_Magnitude(PBEPokemon user, PBEPokemon[] targets)
        {
            BroadcastMoveUsed(user, PBEMove.Magnitude);
            PPReduce(user, PBEMove.Magnitude);
            int val = PBEUtils.RNG.Next(0, 100);
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
            foreach (PBEPokemon target in targets)
            {
                if (target.HP < 1)
                {
                    continue;
                }
                if (MissCheck(user, target, PBEMove.Magnitude))
                {
                    continue;
                }
                double damageMultiplier = targets.Length > 1 ? 0.75 : 1.0;
                bool crit = CritCheck(user, target, PBEMove.Magnitude, ref damageMultiplier);
                TypeCheck(user, target, PBEMove.Magnitude, out PBEType moveType, out PBEEffectiveness effectiveness, ref damageMultiplier);
                ushort damage = CalculateDamage(user, target, PBEMove.Magnitude, moveType, criticalHit: crit, power: basePower);
                DealDamage(user, target, (ushort)(damage * damageMultiplier), effectiveness, false);
                if (crit)
                {
                    BroadcastMoveCrit();
                }
                FaintCheck(target);
            }
        }
        void Ef_Endeavor(PBEPokemon user, PBEPokemon target)
        {
            BroadcastMoveUsed(user, PBEMove.Endeavor);
            PPReduce(user, PBEMove.Endeavor);
            if (MissCheck(user, target, PBEMove.Endeavor))
            {
                return;
            }
            if (target.HP <= user.HP)
            {
                BroadcastMoveFailed(user, target, PBEFailReason.Default);
                return;
            }
            DealDamage(user, target, (ushort)(target.HP - user.HP), PBEEffectiveness.Normal, false);
        }
        void Ef_PainSplit(PBEPokemon user, PBEPokemon target)
        {
            BroadcastMoveUsed(user, PBEMove.PainSplit);
            PPReduce(user, PBEMove.PainSplit);
            if (target.Status2.HasFlag(PBEStatus2.Substitute))
            {
                BroadcastMoveFailed(user, target, PBEFailReason.Default);
                return;
            }
            if (MissCheck(user, target, PBEMove.PainSplit))
            {
                return;
            }
            ushort total = (ushort)(user.HP + target.HP);
            ushort hp = (ushort)(total / 2);
            foreach (PBEPokemon pkmn in new PBEPokemon[] { user, target })
            {
                if (hp >= pkmn.HP)
                {
                    HealDamage(user, (ushort)(hp - pkmn.HP));
                }
                else
                {
                    DealDamage(user, pkmn, (ushort)(pkmn.HP - hp), PBEEffectiveness.Normal, true);
                }
            }
            BroadcastPainSplit();
        }
        void Ef_PsychUp(PBEPokemon user, PBEPokemon target)
        {
            BroadcastMoveUsed(user, PBEMove.PsychUp);
            PPReduce(user, PBEMove.PsychUp);
            if (MissCheck(user, target, PBEMove.PsychUp))
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
