using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        bool calledFromOtherMove = false;

        void DoSwitchInEffects(IEnumerable<PBEPokemon> battlers)
        {
            IEnumerable<PBEPokemon> order = GetActingOrder(battlers, true);

            foreach (PBEPokemon pkmn in order)
            {
                // Verified: Spikes then StealthRock then ToxicSpikes before ability
                if (pkmn.Team.TeamStatus.HasFlag(PBETeamStatus.Spikes) && !pkmn.HasType(PBEType.Flying) && pkmn.Ability != PBEAbility.Levitate)
                {
                    DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / (10.0 - (2 * pkmn.Team.SpikeCount))), true);
                    BroadcastTeamStatus(pkmn.Team, PBETeamStatus.Spikes, PBETeamStatusAction.Damage, pkmn);
                    if (FaintCheck(pkmn))
                    {
                        continue;
                    }
                }
                if (pkmn.Team.TeamStatus.HasFlag(PBETeamStatus.StealthRock))
                {
                    double effectiveness = 0.125;
                    effectiveness *= PBEPokemonData.TypeEffectiveness[(int)PBEType.Rock][(int)pkmn.Type1];
                    effectiveness *= PBEPokemonData.TypeEffectiveness[(int)PBEType.Rock][(int)pkmn.Type2];
                    DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP * effectiveness), true);
                    BroadcastTeamStatus(pkmn.Team, PBETeamStatus.StealthRock, PBETeamStatusAction.Damage, pkmn);
                    if (FaintCheck(pkmn))
                    {
                        continue;
                    }
                }
                if (pkmn.Team.TeamStatus.HasFlag(PBETeamStatus.ToxicSpikes))
                {
                    // Grounded Poison types remove the Toxic Spikes
                    if (pkmn.HasType(PBEType.Poison) && pkmn.Ability != PBEAbility.Levitate && !pkmn.HasType(PBEType.Flying))
                    {
                        BroadcastTeamStatus(pkmn.Team, PBETeamStatus.ToxicSpikes, PBETeamStatusAction.Cleared);
                    }
                    // Steel types and floating Pokémon don't get Poisoned
                    else if (pkmn.Status1 == PBEStatus1.None && !pkmn.HasType(PBEType.Steel) && !pkmn.HasType(PBEType.Flying) && pkmn.Ability != PBEAbility.Levitate)
                    {
                        pkmn.Status1 = pkmn.Team.ToxicSpikeCount == 1 ? PBEStatus1.Poisoned : PBEStatus1.BadlyPoisoned;
                        if (pkmn.Status1 == PBEStatus1.BadlyPoisoned)
                        {
                            pkmn.Status1Counter = 1;
                        }
                        BroadcastStatus1(pkmn, pkmn, pkmn.Status1, PBEStatusAction.Added);
                    }
                }

                LimberCheck(pkmn);
                switch (pkmn.Ability)
                {
                    case PBEAbility.Drizzle:
                        {
                            if (Weather != PBEWeather.Rain || WeatherCounter != 0)
                            {
                                Weather = PBEWeather.Rain;
                                WeatherCounter = 0;
                                BroadcastAbility(pkmn, pkmn, PBEAbility.Drizzle, PBEAbilityAction.Weather);
                                BroadcastWeather(PBEWeather.Rain, PBEWeatherAction.Added);
                            }
                            break;
                        }
                    case PBEAbility.Drought:
                        {
                            if (Weather != PBEWeather.HarshSunlight || WeatherCounter != 0)
                            {
                                Weather = PBEWeather.HarshSunlight;
                                WeatherCounter = 0;
                                BroadcastAbility(pkmn, pkmn, PBEAbility.Drought, PBEAbilityAction.Weather);
                                BroadcastWeather(PBEWeather.HarshSunlight, PBEWeatherAction.Added);
                            }
                            break;
                        }
                    case PBEAbility.Imposter:
                        {
                            PBEFieldPosition targetPos = GetPositionAcross(BattleFormat, pkmn.FieldPosition);
                            PBETeam opposingTeam = pkmn.Team == Teams[0] ? Teams[1] : Teams[0];
                            PBEPokemon target = opposingTeam.TryGetPokemon(targetPos);
                            if (target != null
                                && !target.Status2.HasFlag(PBEStatus2.Disguised)
                                && !target.Status2.HasFlag(PBEStatus2.Substitute)
                                && !target.Status2.HasFlag(PBEStatus2.Transformed))
                            {
                                BroadcastAbility(pkmn, target, PBEAbility.Imposter, PBEAbilityAction.ChangedAppearance);
                                ApplyStatus2IfPossible(pkmn, target, PBEStatus2.Transformed, false);
                            }
                            break;
                        }
                    case PBEAbility.SandStream:
                        {
                            if (Weather != PBEWeather.Sandstorm || WeatherCounter != 0)
                            {
                                Weather = PBEWeather.Sandstorm;
                                WeatherCounter = 0;
                                BroadcastAbility(pkmn, pkmn, PBEAbility.SandStream, PBEAbilityAction.Weather);
                                BroadcastWeather(PBEWeather.Sandstorm, PBEWeatherAction.Added);
                            }
                            break;
                        }
                    case PBEAbility.SnowWarning:
                        {
                            if (Weather != PBEWeather.Hailstorm || WeatherCounter != 0)
                            {
                                Weather = PBEWeather.Hailstorm;
                                WeatherCounter = 0;
                                BroadcastAbility(pkmn, pkmn, PBEAbility.SnowWarning, PBEAbilityAction.Weather);
                                BroadcastWeather(PBEWeather.Hailstorm, PBEWeatherAction.Added);
                            }
                            break;
                        }
                }
            }
        }
        /// <summary>
        /// Does effects that take place after hitting such as substitute breaking, rough skin, and victim eating its berry.
        /// </summary>
        /// <param name="user">The Pokémon who used <paramref name="move"/>.</param>
        /// <param name="victim">The Pokémon who was affected by <paramref name="move"/>.</param>
        /// <param name="move">The move <paramref name="user"/> used.</param>
        void DoPostHitEffects(PBEPokemon user, PBEPokemon victim, PBEMove move)
        {
            // TODO: Move Limber to the proper place (DoPostAttackedEffects)
            LimberCheck(victim);
            if (victim.Status2.HasFlag(PBEStatus2.Substitute))
            {
                if (victim.SubstituteHP == 0)
                {
                    victim.Status2 &= ~PBEStatus2.Substitute;
                    BroadcastStatus2(victim, user, PBEStatus2.Substitute, PBEStatusAction.Ended);
                }
            }
            else
            {
                IllusionBreak(victim, user);
                if (PBEMoveData.Data[move].Flags.HasFlag(PBEMoveFlag.MakesContact))
                {
                    if (user.HP > 0 && victim.Ability == PBEAbility.Mummy && user.Ability != PBEAbility.Multitype && user.Ability != PBEAbility.Mummy && user.Ability != PBEAbility.ZenMode)
                    {
                        BroadcastAbility(victim, user, PBEAbility.Mummy, PBEAbilityAction.Damage);
                        user.Ability = PBEAbility.Mummy;
                        BroadcastAbility(user, victim, PBEAbility.Mummy, PBEAbilityAction.Changed);
                    }
                    if (user.HP > 0 && (victim.Ability == PBEAbility.IronBarbs || victim.Ability == PBEAbility.RoughSkin))
                    {
                        BroadcastAbility(victim, user, victim.Ability, PBEAbilityAction.Damage);
                        DealDamage(victim, user, (ushort)(user.MaxHP / 8), true);
                        FaintCheck(user);
                    }
                    if (user.HP > 0 && victim.Item == PBEItem.RockyHelmet)
                    {
                        BroadcastItem(victim, user, PBEItem.RockyHelmet, PBEItemAction.Damage);
                        DealDamage(victim, user, (ushort)(user.MaxHP / 6), true);
                        FaintCheck(user);
                    }
                }
            }

            // TODO: Weak Armor, Justified, Stench, Effect Spore
            // TODO?: Cell Battery
        }
        /// <summary>
        /// Does effects that take place after an attack is completely done such as recoil and life orb.
        /// </summary>
        /// <param name="user">The Pokémon who used the attack.</param>
        /// <param name="ignoreLifeOrb">Whether <see cref="PBEItem.LifeOrb"/> should damage the user or not.</param>
        /// <param name="recoilDamage">The amount of recoil damage <paramref name="user"/> will take.</param>
        void DoPostAttackedEffects(PBEPokemon user, bool ignoreLifeOrb, ushort recoilDamage = 0)
        {
            // TODO: Color Change

            if (user.HP > 0 && recoilDamage > 0)
            {
                BroadcastRecoil(user);
                DealDamage(user, user, recoilDamage, true);
                FaintCheck(user);
            }

            if (user.HP > 0 && !ignoreLifeOrb && user.Item == PBEItem.LifeOrb)
            {
                BroadcastItem(user, user, PBEItem.LifeOrb, PBEItemAction.Damage);
                DealDamage(user, user, (ushort)(user.MaxHP / 10), true);
                FaintCheck(user);
            }
        }
        void DoTurnEndedEffects()
        {
            IEnumerable<PBEPokemon> order = GetActingOrder(ActiveBattlers, true);

            // Verified: Weather before all
            foreach (PBEPokemon pkmn in order)
            {
                if (pkmn.HP > 0)
                {
                    switch (Weather)
                    {
                        case PBEWeather.Hailstorm:
                            {
                                if (pkmn.Ability == PBEAbility.IceBody)
                                {
                                    if (pkmn.HP < pkmn.MaxHP)
                                    {
                                        BroadcastAbility(pkmn, pkmn, PBEAbility.IceBody, PBEAbilityAction.RestoredHP);
                                        HealDamage(pkmn, (ushort)(pkmn.MaxHP / Settings.IceBodyHealDenominator));
                                    }
                                }
                                else if (!pkmn.HasType(PBEType.Ice)
                                    && pkmn.Ability != PBEAbility.Overcoat
                                    && pkmn.Ability != PBEAbility.SnowCloak)
                                {
                                    BroadcastWeather(PBEWeather.Hailstorm, PBEWeatherAction.CausedDamage, pkmn);
                                    DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / Settings.HailDamageDenominator), true);
                                    FaintCheck(pkmn);
                                }
                                break;
                            }
                        case PBEWeather.HarshSunlight:
                            {
                                if (pkmn.Ability == PBEAbility.SolarPower)
                                {
                                    BroadcastAbility(pkmn, pkmn, PBEAbility.SolarPower, PBEAbilityAction.Damage);
                                    DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / 8), true);
                                    FaintCheck(pkmn);
                                }
                                break;
                            }
                        case PBEWeather.Rain:
                            {
                                if (pkmn.Ability == PBEAbility.RainDish && pkmn.HP < pkmn.MaxHP)
                                {
                                    BroadcastAbility(pkmn, pkmn, PBEAbility.RainDish, PBEAbilityAction.RestoredHP);
                                    HealDamage(pkmn, (ushort)(pkmn.MaxHP / 16));
                                }
                                break;
                            }
                        case PBEWeather.Sandstorm:
                            {
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
                                    BroadcastWeather(PBEWeather.Sandstorm, PBEWeatherAction.CausedDamage, pkmn);
                                    DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / Settings.SandstormDamageDenominator), true);
                                    FaintCheck(pkmn);
                                }
                                break;
                            }
                    }
                }
            }

            // Verified: Healer before Leftovers
            foreach (PBEPokemon pkmn in order)
            {
                if (pkmn.HP > 0 && pkmn.Ability == PBEAbility.Healer)
                {
                    foreach (PBEPokemon ally in GetRuntimeSurrounding(pkmn, true, false))
                    {
                        if (ally.Status1 != PBEStatus1.None && PBEUtils.RNG.ApplyChance(30, 100))
                        {
                            BroadcastAbility(pkmn, ally, PBEAbility.Healer, PBEAbilityAction.CuredStatus);
                            PBEStatus1 status = ally.Status1;
                            ally.Status1 = PBEStatus1.None;
                            BroadcastStatus1(ally, pkmn, status, PBEStatusAction.Cured);
                        }
                    }
                }
            }

            // Verified: Leftovers before Leech Seed
            foreach (PBEPokemon pkmn in order)
            {
                if (pkmn.HP > 0)
                {
                    switch (pkmn.Item)
                    {
                        case PBEItem.BlackSludge:
                            {
                                if (pkmn.HasType(PBEType.Poison))
                                {
                                    if (pkmn.HP < pkmn.MaxHP)
                                    {
                                        BroadcastItem(pkmn, pkmn, PBEItem.BlackSludge, PBEItemAction.RestoredHP);
                                        HealDamage(pkmn, (ushort)(pkmn.MaxHP / Settings.BlackSludgeHealDenominator));
                                    }
                                }
                                else
                                {
                                    BroadcastItem(pkmn, pkmn, PBEItem.BlackSludge, PBEItemAction.Damage);
                                    DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / Settings.BlackSludgeDamageDenominator), true);
                                    FaintCheck(pkmn);
                                }
                                break;
                            }
                        case PBEItem.Leftovers:
                            {
                                if (pkmn.HP < pkmn.MaxHP)
                                {
                                    BroadcastItem(pkmn, pkmn, PBEItem.Leftovers, PBEItemAction.RestoredHP);
                                    HealDamage(pkmn, (ushort)(pkmn.MaxHP / Settings.LeftoversHealDenominator));
                                }
                                break;
                            }
                    }
                }
            }

            // Verified: Leech Seed before Status1
            foreach (PBEPokemon pkmn in order)
            {
                if (pkmn.HP > 0)
                {
                    PBETeam opposingTeam = pkmn.Team == Teams[0] ? Teams[1] : Teams[0];
                    IEnumerable<PBEPokemon> peopleToSuck = opposingTeam.ActiveBattlers.Where(p => p.Status2.HasFlag(PBEStatus2.LeechSeed) && p.SeededPosition == pkmn.FieldPosition);
                    foreach (PBEPokemon suck in peopleToSuck)
                    {
                        BroadcastStatus2(suck, pkmn, PBEStatus2.LeechSeed, PBEStatusAction.Damage);
                        ushort amtDealt = DealDamage(pkmn, suck, (ushort)(suck.MaxHP / Settings.LeechSeedDenominator), true);
                        HealDamage(pkmn, amtDealt);
                        FaintCheck(suck);
                    }
                }
            }

            // Verified: Status1 before Curse
            foreach (PBEPokemon pkmn in order)
            {
                if (pkmn.HP > 0)
                {
                    switch (pkmn.Status1)
                    {
                        case PBEStatus1.Burned:
                            {
                                BroadcastStatus1(pkmn, pkmn, PBEStatus1.Burned, PBEStatusAction.Damage);
                                int damage = pkmn.MaxHP / Settings.BurnDamageDenominator;
                                if (pkmn.Ability == PBEAbility.Heatproof)
                                {
                                    damage /= 2;
                                }
                                DealDamage(pkmn, pkmn, (ushort)damage, true);
                                FaintCheck(pkmn);
                                break;
                            }
                        case PBEStatus1.Poisoned:
                            {
                                BroadcastStatus1(pkmn, pkmn, PBEStatus1.Poisoned, PBEStatusAction.Damage);
                                DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / Settings.PoisonDamageDenominator), true);
                                FaintCheck(pkmn);
                                break;
                            }
                        case PBEStatus1.BadlyPoisoned:
                            {
                                BroadcastStatus1(pkmn, pkmn, PBEStatus1.BadlyPoisoned, PBEStatusAction.Damage);
                                DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP * pkmn.Status1Counter / Settings.ToxicDamageDenominator), true);
                                if (FaintCheck(pkmn))
                                {
                                    pkmn.Status1Counter = 0;
                                }
                                else
                                {
                                    pkmn.Status1Counter++;
                                }
                                break;
                            }
                    }
                }
            }

            // Verified: Curse before Orbs
            foreach (PBEPokemon pkmn in order)
            {
                if (pkmn.HP > 0 && pkmn.Status2.HasFlag(PBEStatus2.Cursed))
                {
                    BroadcastStatus2(pkmn, pkmn, PBEStatus2.Cursed, PBEStatusAction.Damage);
                    DealDamage(pkmn, pkmn, (ushort)(pkmn.MaxHP / Settings.CurseDenominator), true);
                    FaintCheck(pkmn);
                }
            }

            // Orbs
            foreach (PBEPokemon pkmn in order)
            {
                if (pkmn.HP > 0)
                {
                    switch (pkmn.Item)
                    {
                        case PBEItem.FlameOrb:
                            {
                                if (pkmn.Status1 == PBEStatus1.None && !pkmn.HasType(PBEType.Fire))
                                {
                                    pkmn.Status1 = PBEStatus1.Burned;
                                    BroadcastItem(pkmn, pkmn, PBEItem.FlameOrb, PBEItemAction.ChangedStatus);
                                    BroadcastStatus1(pkmn, pkmn, PBEStatus1.Burned, PBEStatusAction.Added);
                                }
                                break;
                            }
                        case PBEItem.ToxicOrb:
                            {
                                if (pkmn.Status1 == PBEStatus1.None && !pkmn.HasType(PBEType.Poison) && !pkmn.HasType(PBEType.Steel))
                                {
                                    pkmn.Status1 = PBEStatus1.BadlyPoisoned;
                                    BroadcastItem(pkmn, pkmn, PBEItem.ToxicOrb, PBEItemAction.ChangedStatus);
                                    BroadcastStatus1(pkmn, pkmn, PBEStatus1.BadlyPoisoned, PBEStatusAction.Added);
                                }
                                break;
                            }
                    }
                }
            }
        }

        bool HasActedThisTurn(PBEPokemon pkmn)
        {
            return pkmn.ExecutedMoves.Any(e => e.TurnNumber == TurnNumber);
        }

        void UseMove(PBEPokemon user, PBEMove move, PBETarget requestedTargets)
        {
            if (!calledFromOtherMove && PreMoveStatusCheck(user, move))
            {
                if (user.Status2.HasFlag(PBEStatus2.Airborne))
                {
                    user.TempLockedMove = PBEMove.None;
                    user.TempLockedTargets = PBETarget.None;
                    BroadcastMoveLock(user, user.TempLockedMove, user.TempLockedTargets, PBEMoveLockType.Temporary);
                    user.Status2 &= ~PBEStatus2.Airborne;
                    BroadcastStatus2(user, user, PBEStatus2.Airborne, PBEStatusAction.Ended);
                }
                if (user.Status2.HasFlag(PBEStatus2.Underground))
                {
                    user.TempLockedMove = PBEMove.None;
                    user.TempLockedTargets = PBETarget.None;
                    BroadcastMoveLock(user, user.TempLockedMove, user.TempLockedTargets, PBEMoveLockType.Temporary);
                    user.Status2 &= ~PBEStatus2.Underground;
                    BroadcastStatus2(user, user, PBEStatus2.Underground, PBEStatusAction.Ended);
                }
                if (user.Status2.HasFlag(PBEStatus2.Underwater))
                {
                    user.TempLockedMove = PBEMove.None;
                    user.TempLockedTargets = PBETarget.None;
                    BroadcastMoveLock(user, user.TempLockedMove, user.TempLockedTargets, PBEMoveLockType.Temporary);
                    user.Status2 &= ~PBEStatus2.Underwater;
                    BroadcastStatus2(user, user, PBEStatus2.Underwater, PBEStatusAction.Ended);
                }
                return;
            }
            else
            {
                PBEMoveData mData = PBEMoveData.Data[move];
                PBEPokemon[] targets = GetRuntimeTargets(user, requestedTargets, user.GetMoveTargets(move) == PBEMoveTarget.SingleNotSelf);
                switch (mData.Effect)
                {
                    case PBEMoveEffect.BrickBreak:
                        {
                            Ef_BrickBreak(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Burn:
                        {
                            Ef_TryForceStatus1(user, targets, move, PBEStatus1.Burned);
                            break;
                        }
                    case PBEMoveEffect.ChangeTarget_ACC:
                        {
                            Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Accuracy }, new short[] { (short)mData.EffectParam });
                            break;
                        }
                    case PBEMoveEffect.ChangeTarget_ATK:
                        {
                            Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack }, new short[] { (short)mData.EffectParam });
                            break;
                        }
                    case PBEMoveEffect.ChangeTarget_DEF:
                        {
                            Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Defense }, new short[] { (short)mData.EffectParam });
                            break;
                        }
                    case PBEMoveEffect.ChangeTarget_EVA:
                        {
                            Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Evasion }, new short[] { (short)mData.EffectParam });
                            break;
                        }
                    case PBEMoveEffect.ChangeTarget_SPDEF:
                        {
                            Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpDefense }, new short[] { (short)mData.EffectParam });
                            break;
                        }
                    case PBEMoveEffect.ChangeTarget_SPE:
                        {
                            Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Speed }, new short[] { (short)mData.EffectParam });
                            break;
                        }
                    case PBEMoveEffect.ChangeUser_ATK:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack }, new short[] { (short)mData.EffectParam });
                            break;
                        }
                    case PBEMoveEffect.ChangeUser_DEF:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Defense }, new short[] { (short)mData.EffectParam });
                            break;
                        }
                    case PBEMoveEffect.ChangeUser_EVA:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Evasion }, new short[] { (short)mData.EffectParam });
                            break;
                        }
                    case PBEMoveEffect.ChangeUser_SPATK:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.SpAttack }, new short[] { (short)mData.EffectParam });
                            break;
                        }
                    case PBEMoveEffect.ChangeUser_SPDEF:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.SpDefense }, new short[] { (short)mData.EffectParam });
                            break;
                        }
                    case PBEMoveEffect.ChangeUser_SPE:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Speed }, new short[] { (short)mData.EffectParam });
                            break;
                        }
                    case PBEMoveEffect.Confuse:
                        {
                            Ef_TryForceStatus2(user, targets, move, PBEStatus2.Confused);
                            break;
                        }
                    case PBEMoveEffect.Curse:
                        {
                            Ef_Curse(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Dig:
                        {
                            Ef_Dig(user, targets, move, requestedTargets);
                            break;
                        }
                    case PBEMoveEffect.Dive:
                        {
                            Ef_Dive(user, targets, move, requestedTargets);
                            break;
                        }
                    case PBEMoveEffect.Endeavor:
                        {
                            Ef_Endeavor(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Fail:
                        {
                            Ef_Fail(user, move);
                            break;
                        }
                    case PBEMoveEffect.FinalGambit:
                        {
                            Ef_FinalGambit(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.FlareBlitz:
                        {
                            Ef_Recoil3__MaybeInflictStatus1With10PercentChance(user, targets, move, PBEStatus1.Burned);
                            break;
                        }
                    case PBEMoveEffect.Flatter:
                        {
                            Ef_Flatter(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Fly:
                        {
                            Ef_Fly(user, targets, move, requestedTargets);
                            break;
                        }
                    case PBEMoveEffect.FocusEnergy:
                        {
                            Ef_TryForceStatus2(user, targets, move, PBEStatus2.Pumped);
                            break;
                        }
                    case PBEMoveEffect.GastroAcid:
                        {
                            Ef_GastroAcid(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Growth:
                        {
                            Ef_Growth(user, move);
                            break;
                        }
                    case PBEMoveEffect.Hail:
                        {
                            Ef_TryForceWeather(user, move, PBEWeather.Hailstorm);
                            break;
                        }
                    case PBEMoveEffect.HelpingHand:
                        {
                            Ef_HelpingHand(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Hit:
                        {
                            Ef_Hit(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeBurn:
                        {
                            Ef_Hit__MaybeInflictStatus1(user, targets, move, PBEStatus1.Burned, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeConfuse:
                        {
                            Ef_Hit__MaybeInflictStatus2(user, targets, move, PBEStatus2.Confused, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeFlinch:
                        {
                            Ef_Hit__MaybeInflictStatus2(user, targets, move, PBEStatus2.Flinching, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeFreeze:
                        {
                            Ef_Hit__MaybeInflictStatus1(user, targets, move, PBEStatus1.Frozen, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1:
                        {
                            Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Accuracy }, new short[] { -1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_ATK_By1:
                        {
                            Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack }, new short[] { -1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1:
                        {
                            Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Defense }, new short[] { -1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1:
                        {
                            Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack }, new short[] { -1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1:
                        {
                            Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpDefense }, new short[] { -1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2:
                        {
                            Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpDefense }, new short[] { -2 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1:
                        {
                            Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Speed }, new short[] { -1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeParalyze:
                        {
                            Ef_Hit__MaybeInflictStatus1(user, targets, move, PBEStatus1.Paralyzed, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybePoison:
                        {
                            Ef_Hit__MaybeInflictStatus1(user, targets, move, PBEStatus1.Poisoned, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeLowerUser_ATK_DEF_By1:
                        {
                            Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense }, new short[] { -1, -1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1:
                        {
                            Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Defense, PBEStat.SpDefense }, new short[] { -1, -1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2:
                        {
                            Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack }, new short[] { -2 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeLowerUser_SPE_By1:
                        {
                            Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Speed }, new short[] { -1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1:
                        {
                            Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Speed, PBEStat.Defense, PBEStat.SpDefense }, new short[] { -1, -1, -1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeRaiseUser_ATK_By1:
                        {
                            Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Attack }, new short[] { +1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1:
                        {
                            Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense, PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed }, new short[] { +1, +1, +1, +1, +1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeRaiseUser_DEF_By1:
                        {
                            Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Defense }, new short[] { +1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeRaiseUser_SPATK_By1:
                        {
                            Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack }, new short[] { +1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeRaiseUser_SPE_By1:
                        {
                            Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Speed }, new short[] { +1 }, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Hit__MaybeToxic:
                        {
                            Ef_Hit__MaybeInflictStatus1(user, targets, move, PBEStatus1.BadlyPoisoned, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.HPDrain:
                        {
                            Ef_HPDrain(user, targets, move, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.LeechSeed:
                        {
                            Ef_TryForceStatus2(user, targets, move, PBEStatus2.LeechSeed);
                            break;
                        }
                    case PBEMoveEffect.LightScreen:
                        {
                            Ef_TryForceTeamStatus(user, move, PBETeamStatus.LightScreen);
                            break;
                        }
                    case PBEMoveEffect.LowerTarget_ATK_DEF_By1:
                        {
                            Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense }, new short[] { -1, -1 });
                            break;
                        }
                    case PBEMoveEffect.LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Defense, PBEStat.SpDefense, PBEStat.Attack, PBEStat.SpAttack, PBEStat.Speed }, new short[] { -1, -1, +2, +2, +2 });
                            break;
                        }
                    case PBEMoveEffect.LuckyChant:
                        {
                            Ef_TryForceTeamStatus(user, move, PBETeamStatus.LuckyChant);
                            break;
                        }
                    case PBEMoveEffect.Metronome:
                        {
                            Ef_Metronome(user, move);
                            break;
                        }
                    case PBEMoveEffect.Moonlight:
                        {
                            Ef_Moonlight(user, move);
                            break;
                        }
                    case PBEMoveEffect.OneHitKnockout:
                        {
                            Ef_OneHitKnockout(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.PainSplit:
                        {
                            Ef_PainSplit(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Paralyze:
                        {
                            Ef_TryForceStatus1(user, targets, move, PBEStatus1.Paralyzed);
                            break;
                        }
                    case PBEMoveEffect.Poison:
                        {
                            Ef_TryForceStatus1(user, targets, move, PBEStatus1.Poisoned);
                            break;
                        }
                    case PBEMoveEffect.Protect:
                        {
                            Ef_TryForceStatus2(user, targets, move, PBEStatus2.Protected);
                            break;
                        }
                    case PBEMoveEffect.PsychUp:
                        {
                            Ef_PsychUp(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Psywave:
                        {
                            Ef_Psywave(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.RainDance:
                        {
                            Ef_TryForceWeather(user, move, PBEWeather.Rain);
                            break;
                        }
                    case PBEMoveEffect.RaiseUser_ATK_ACC_By1:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack, PBEStat.Accuracy }, new short[] { +1, +1 });
                            break;
                        }
                    case PBEMoveEffect.RaiseUser_ATK_DEF_By1:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense }, new short[] { +1, +1 });
                            break;
                        }
                    case PBEMoveEffect.RaiseUser_ATK_DEF_ACC_By1:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense, PBEStat.Accuracy }, new short[] { +1, +1, +1 });
                            break;
                        }
                    case PBEMoveEffect.RaiseUser_ATK_SPATK_By1:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack, PBEStat.SpAttack }, new short[] { +1, +1 });
                            break;
                        }
                    case PBEMoveEffect.RaiseUser_ATK_SPE_By1:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack, PBEStat.Speed }, new short[] { +1, +1 });
                            break;
                        }
                    case PBEMoveEffect.RaiseUser_DEF_SPDEF_By1:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Defense, PBEStat.SpDefense }, new short[] { +1, +1 });
                            break;
                        }
                    case PBEMoveEffect.RaiseUser_SPATK_SPDEF_By1:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.SpAttack, PBEStat.SpDefense }, new short[] { +1, +1 });
                            break;
                        }
                    case PBEMoveEffect.RaiseUser_SPATK_SPDEF_SPE_By1:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed }, new short[] { +1, +1, +1 });
                            break;
                        }
                    case PBEMoveEffect.RaiseUser_SPE_By2_ATK_By1:
                        {
                            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Speed, PBEStat.Attack }, new short[] { +2, +1 });
                            break;
                        }
                    case PBEMoveEffect.Recoil:
                        {
                            Ef_Recoil(user, targets, move, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Reflect:
                        {
                            Ef_TryForceTeamStatus(user, move, PBETeamStatus.Reflect);
                            break;
                        }
                    case PBEMoveEffect.RestoreTargetHP:
                        {
                            Ef_RestoreTargetHP(user, targets, move, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.RestoreUserHP:
                        {
                            Ef_RestoreUserHP(user, move, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Sandstorm:
                        {
                            Ef_TryForceWeather(user, move, PBEWeather.Sandstorm);
                            break;
                        }
                    case PBEMoveEffect.SeismicToss:
                        {
                            Ef_SeismicToss(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Selfdestruct:
                        {
                            Ef_Selfdestruct(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.SetDamage:
                        {
                            Ef_SetDamage(user, targets, move, mData.EffectParam);
                            break;
                        }
                    case PBEMoveEffect.Sleep:
                        {
                            Ef_TryForceStatus1(user, targets, move, PBEStatus1.Asleep);
                            break;
                        }
                    case PBEMoveEffect.Spikes:
                        {
                            Ef_TryForceTeamStatus(user, move, PBETeamStatus.Spikes);
                            break;
                        }
                    case PBEMoveEffect.StealthRock:
                        {
                            Ef_TryForceTeamStatus(user, move, PBETeamStatus.StealthRock);
                            break;
                        }
                    case PBEMoveEffect.Struggle:
                        {
                            Ef_Struggle(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Substitute:
                        {
                            Ef_TryForceStatus2(user, targets, move, PBEStatus2.Substitute);
                            break;
                        }
                    case PBEMoveEffect.SunnyDay:
                        {
                            Ef_TryForceWeather(user, move, PBEWeather.HarshSunlight);
                            break;
                        }
                    case PBEMoveEffect.SuperFang:
                        {
                            Ef_SuperFang(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Swagger:
                        {
                            Ef_Swagger(user, targets, move);
                            break;
                        }
                    case PBEMoveEffect.Toxic:
                        {
                            Ef_TryForceStatus1(user, targets, move, PBEStatus1.BadlyPoisoned);
                            break;
                        }
                    case PBEMoveEffect.ToxicSpikes:
                        {
                            Ef_TryForceTeamStatus(user, move, PBETeamStatus.ToxicSpikes);
                            break;
                        }
                    case PBEMoveEffect.Transform:
                        {
                            Ef_TryForceStatus2(user, targets, move, PBEStatus2.Transformed);
                            break;
                        }
                    case PBEMoveEffect.TrickRoom:
                        {
                            Ef_TryForceBattleStatus(user, move, PBEBattleStatus.TrickRoom);
                            break;
                        }
                    case PBEMoveEffect.VoltTackle:
                        {
                            Ef_Recoil3__MaybeInflictStatus1With10PercentChance(user, targets, move, PBEStatus1.Paralyzed);
                            break;
                        }
                    case PBEMoveEffect.Whirlwind:
                        {
                            Ef_Whirlwind(user, targets, move);
                            break;
                        }
                    default: throw new ArgumentOutOfRangeException(nameof(mData.Effect));
                }
            }
        }

        // Returns true if an attack gets cancelled from a status
        // Broadcasts status ending events & status causing immobility events
        bool PreMoveStatusCheck(PBEPokemon user, PBEMove move)
        {
            // Verified: Sleep and Freeze don't interact with Flinch unless they come out of the status
            if (user.Status1 == PBEStatus1.Asleep)
            {
                user.Status1Counter++;
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
            }
            else if (user.Status1 == PBEStatus1.Frozen)
            {
                if (PBEMoveData.Data[move].Flags.HasFlag(PBEMoveFlag.DefrostsUser) || PBEUtils.RNG.ApplyChance(20, 100))
                {
                    user.Status1 = PBEStatus1.None;
                    BroadcastStatus1(user, user, PBEStatus1.Frozen, PBEStatusAction.Ended);
                }
                else
                {
                    BroadcastStatus1(user, user, PBEStatus1.Frozen, PBEStatusAction.Activated);
                    return true;
                }
            }
            // Verified: Flinch before Confusion and Paralysis can do anything
            if (user.Status2.HasFlag(PBEStatus2.Flinching))
            {
                BroadcastStatus2(user, user, PBEStatus2.Flinching, PBEStatusAction.Activated);
                return true;
            }
            // Verified: Confusion before Paralysis
            if (user.Status2.HasFlag(PBEStatus2.Confused))
            {
                user.ConfusionCounter++;
                if (user.ConfusionCounter > user.ConfusionTurns)
                {
                    user.Status2 &= ~PBEStatus2.Confused;
                    user.ConfusionCounter = user.ConfusionTurns = 0;
                    BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Ended);
                }
                else
                {
                    BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Activated);
                    if (PBEUtils.RNG.ApplyChance(50, 100))
                    {
                        BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Damage);
                        ushort damage = CalculateDamage(user, user, PBEMove.None, PBEType.None, PBEMoveCategory.Physical, 40, false);
                        DealDamage(user, user, damage, true);
                        FaintCheck(user);
                        return true;
                    }
                }
            }
            // Paralysis
            if (user.Status1 == PBEStatus1.Paralyzed && PBEUtils.RNG.ApplyChance(25, 100))
            {
                BroadcastStatus1(user, user, PBEStatus1.Paralyzed, PBEStatusAction.Activated);
                return true;
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
                BroadcastStatus2(target, user, PBEStatus2.Protected, PBEStatusAction.Activated);
                return true;
            }
            if (user.Ability == PBEAbility.NoGuard || target.Ability == PBEAbility.NoGuard)
            {
                return false;
            }
            if (target.Status2.HasFlag(PBEStatus2.Airborne) && !mData.Flags.HasFlag(PBEMoveFlag.HitsAirborne))
            {
                goto miss;
            }
            if (target.Status2.HasFlag(PBEStatus2.Underground) && !mData.Flags.HasFlag(PBEMoveFlag.HitsUnderground))
            {
                goto miss;
            }
            if (target.Status2.HasFlag(PBEStatus2.Underwater) && !mData.Flags.HasFlag(PBEMoveFlag.HitsUnderwater))
            {
                goto miss;
            }

            double chance;
            switch (move)
            {
                case PBEMove.Blizzard:
                    {
                        if (Weather == PBEWeather.Hailstorm)
                        {
                            return false;
                        }
                        else
                        {
                            chance = mData.Accuracy;
                        }
                        break;
                    }
                case PBEMove.Fissure:
                case PBEMove.Guillotine:
                case PBEMove.HornDrill:
                case PBEMove.SheerCold:
                    {
                        chance = user.Shell.Level - target.Shell.Level + 30;
                        goto roll; // Skip all modifiers
                    }
                case PBEMove.Hurricane:
                case PBEMove.Thunder:
                    {
                        if (Weather == PBEWeather.Rain)
                        {
                            return false;
                        }
                        else if (Weather == PBEWeather.HarshSunlight)
                        {
                            chance = 50.0;
                        }
                        else
                        {
                            chance = mData.Accuracy;
                        }
                        break;
                    }
                default:
                    {
                        if (target.Ability == PBEAbility.WonderSkin && mData.Category == PBEMoveCategory.Status)
                        {
                            chance = 50;
                        }
                        else if (mData.Accuracy == 0)
                        {
                            return false;
                        }
                        else
                        {
                            chance = mData.Accuracy;
                        }
                        break;
                    }
            }
            chance *= GetStatChangeModifier(user.AccuracyChange, true) / GetStatChangeModifier(target.EvasionChange, true);
            if (user.Ability == PBEAbility.Compoundeyes)
            {
                chance *= 1.3;
            }
            if (user.Team.ActiveBattlers.Any(p => p.Ability == PBEAbility.VictoryStar))
            {
                chance *= 1.1;
            }
            if (user.Ability == PBEAbility.Hustle && mData.Category == PBEMoveCategory.Physical)
            {
                chance *= 0.8;
            }
            if (Weather == PBEWeather.Sandstorm && target.Ability == PBEAbility.SandVeil)
            {
                chance *= 0.8;
            }
            if (Weather == PBEWeather.Hailstorm && target.Ability == PBEAbility.SnowCloak)
            {
                chance *= 0.8;
            }
            if (target.Item == PBEItem.BrightPowder)
            {
                chance *= 0.9;
            }
            if (target.Item == PBEItem.LaxIncense)
            {
                chance *= 0.9;
            }
            if (user.Item == PBEItem.WideLens)
            {
                chance *= 1.1;
            }
            if (target.Ability == PBEAbility.TangledFeet && target.Status2.HasFlag(PBEStatus2.Confused))
            {
                chance *= 0.5;
            }
        roll:
            if (PBEUtils.RNG.ApplyChance((int)chance, 100))
            {
                return false;
            }
        miss:
            BroadcastMoveMissed(user, target);
            return true;
        }

        // Returns true if a critical hit was determined
        bool CritCheck(PBEPokemon user, PBEPokemon target, PBEMove move)
        {
            if (target.Ability == PBEAbility.BattleArmor
                || target.Ability == PBEAbility.ShellArmor
                || target.Team.TeamStatus.HasFlag(PBETeamStatus.LuckyChant))
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
            if (user.Shell.Species == PBESpecies.Chansey && user.Item == PBEItem.LuckyPunch)
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

            return mData.Flags.HasFlag(PBEMoveFlag.AlwaysCrit) || PBEUtils.RNG.ApplyChance((int)(chance * 100), 100 * 100);
        }

        void IllusionBreak(PBEPokemon pkmn, PBEPokemon breaker)
        {
            if (pkmn.Status2.HasFlag(PBEStatus2.Disguised))
            {
                pkmn.Status2 &= ~PBEStatus2.Disguised;
                pkmn.DisguisedAsPokemon = null;
                pkmn.VisualGender = pkmn.Shell.Gender;
                pkmn.VisualNickname = pkmn.Shell.Nickname;
                pkmn.VisualShiny = pkmn.Shell.Shiny;
                pkmn.VisualSpecies = pkmn.Shell.Species;
                BroadcastIllusion(pkmn);
                BroadcastAbility(pkmn, breaker, PBEAbility.Illusion, PBEAbilityAction.ChangedAppearance);
            }
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

        void RecordExecutedMove(PBEPokemon user, PBEMove move, PBEFailReason failReason, IList<PBEExecutedMove.PBETargetSuccess> targets)
        {
            user.ExecutedMoves.Add(new PBEExecutedMove(TurnNumber, move, failReason, targets));
            // Doesn't care if there is a Choice Locked move already. As long as the user knows it, it will become locked. (Metronome calling a move the user knows, Ditto transforming into someone else with transform)
            if ((user.Item == PBEItem.ChoiceBand || user.Item == PBEItem.ChoiceScarf || user.Item == PBEItem.ChoiceSpecs) && user.Moves.Contains(move))
            {
                user.ChoiceLockedMove = move;
                BroadcastMoveLock(user, move, PBETarget.None, PBEMoveLockType.ChoiceItem);
            }
        }

        // Broadcasts the event
        void PPReduce(PBEPokemon pkmn, PBEMove move)
        {
            if (!calledFromOtherMove)
            {
                int moveIndex = Array.IndexOf(pkmn.Moves, move);
                int amtToReduce = 1;
                // TODO: If target is not self and has pressure
                byte oldPP = pkmn.PP[moveIndex];
                pkmn.PP[moveIndex] = (byte)Math.Max(0, pkmn.PP[moveIndex] - amtToReduce);
                BroadcastMovePPChanged(pkmn, move, oldPP, pkmn.PP[moveIndex]);
            }
        }

        // Returns true if the Pokémon fainted & removes it from activeBattlers
        // Broadcasts the event if it fainted
        bool FaintCheck(PBEPokemon pkmn)
        {
            if (pkmn.HP == 0)
            {
                if (Winner == null && pkmn.Team.NumPkmnAlive == 0)
                {
                    Winner = pkmn.Team == Teams[0] ? Teams[1] : Teams[0];
                }
                turnOrder.Remove(pkmn); // Necessary?
                ActiveBattlers.Remove(pkmn);
                PBEFieldPosition oldPos = pkmn.FieldPosition;
                pkmn.FieldPosition = PBEFieldPosition.None;
                BroadcastPkmnFainted(pkmn, oldPos);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Changes a Pokémon's stat.
        /// </summary>
        /// <param name="pkmn">The Pokémon who's stats will be changed.</param>
        /// <param name="stat">The stat to change.</param>
        /// <param name="value">The value to add to the stat.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="stat"/> is invalid.</exception>
        public void ApplyStatChange(PBEPokemon pkmn, PBEStat stat, short value)
        {
            if (pkmn.Ability == PBEAbility.Simple)
            {
                value *= 2;
            }

            sbyte oldValue, newValue;
            // I used to use unsafe pointers, and even though it was cool and compact, I decided to use properties and a more C#-styled approach
            switch (stat)
            {
                case PBEStat.Attack:
                    {
                        oldValue = pkmn.AttackChange;
                        newValue = pkmn.AttackChange = (sbyte)PBEUtils.Clamp(pkmn.AttackChange + value, -Settings.MaxStatChange, Settings.MaxStatChange);
                        break;
                    }
                case PBEStat.Defense:
                    {
                        oldValue = pkmn.DefenseChange;
                        newValue = pkmn.DefenseChange = (sbyte)PBEUtils.Clamp(pkmn.DefenseChange + value, -Settings.MaxStatChange, Settings.MaxStatChange);
                        break;
                    }
                case PBEStat.SpAttack:
                    {
                        oldValue = pkmn.SpAttackChange;
                        newValue = pkmn.SpAttackChange = (sbyte)PBEUtils.Clamp(pkmn.SpAttackChange + value, -Settings.MaxStatChange, Settings.MaxStatChange);
                        break;
                    }
                case PBEStat.SpDefense:
                    {
                        oldValue = pkmn.SpDefenseChange;
                        newValue = pkmn.SpDefenseChange = (sbyte)PBEUtils.Clamp(pkmn.SpDefenseChange + value, -Settings.MaxStatChange, Settings.MaxStatChange);
                        break;
                    }
                case PBEStat.Speed:
                    {
                        oldValue = pkmn.SpeedChange;
                        newValue = pkmn.SpeedChange = (sbyte)PBEUtils.Clamp(pkmn.SpeedChange + value, -Settings.MaxStatChange, Settings.MaxStatChange);
                        break;
                    }
                case PBEStat.Accuracy:
                    {
                        oldValue = pkmn.AccuracyChange;
                        newValue = pkmn.AccuracyChange = (sbyte)PBEUtils.Clamp(pkmn.AccuracyChange + value, -Settings.MaxStatChange, Settings.MaxStatChange);
                        break;
                    }
                case PBEStat.Evasion:
                    {
                        oldValue = pkmn.EvasionChange;
                        newValue = pkmn.EvasionChange = (sbyte)PBEUtils.Clamp(pkmn.EvasionChange + value, -Settings.MaxStatChange, Settings.MaxStatChange);
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(stat));
            }

            BroadcastPkmnStatChanged(pkmn, stat, oldValue, newValue);
        }

        PBEFailReason ApplyStatus1IfPossible(PBEPokemon user, PBEPokemon target, PBEStatus1 status, bool broadcastFailOrEffectiveness)
        {
            if (target.Status1 != PBEStatus1.None || target.Status2.HasFlag(PBEStatus2.Substitute))
            {
                PBEFailReason failReason;
                if (target.Status1 == PBEStatus1.Asleep && status == PBEStatus1.Asleep)
                {
                    failReason = PBEFailReason.AlreadyAsleep;
                }
                else if (target.Status1 == PBEStatus1.Burned && status == PBEStatus1.Burned)
                {
                    failReason = PBEFailReason.AlreadyBurned;
                }
                else if (target.Status1 == PBEStatus1.Paralyzed && status == PBEStatus1.Paralyzed)
                {
                    failReason = PBEFailReason.AlreadyParalyzed;
                }
                else if ((target.Status1 == PBEStatus1.BadlyPoisoned || target.Status1 == PBEStatus1.Poisoned) && (status == PBEStatus1.BadlyPoisoned || status == PBEStatus1.Poisoned))
                {
                    failReason = PBEFailReason.AlreadyPoisoned;
                }
                else
                {
                    failReason = PBEFailReason.Default;
                }

                if (broadcastFailOrEffectiveness)
                {
                    BroadcastMoveFailed(user, target, failReason);
                }
                return failReason;
            }
            if (status == PBEStatus1.Paralyzed && target.Ability == PBEAbility.Limber)
            {
                if (broadcastFailOrEffectiveness)
                {
                    BroadcastAbility(target, target, PBEAbility.Limber, PBEAbilityAction.PreventedStatus);
                    BroadcastEffectiveness(target, PBEEffectiveness.Ineffective);
                }
                return PBEFailReason.Ineffective;
            }
            if ((status == PBEStatus1.Burned && target.HasType(PBEType.Fire))
                || (status == PBEStatus1.Frozen && target.HasType(PBEType.Ice))
                || ((status == PBEStatus1.BadlyPoisoned || status == PBEStatus1.Poisoned) && (target.HasType(PBEType.Poison) || target.HasType(PBEType.Steel))))
            {
                if (broadcastFailOrEffectiveness)
                {
                    BroadcastEffectiveness(target, PBEEffectiveness.Ineffective);
                }
                return PBEFailReason.Ineffective;
            }

            target.Status1 = status;
            if (status == PBEStatus1.BadlyPoisoned)
            {
                target.Status1Counter = 1;
            }
            if (status == PBEStatus1.Asleep)
            {
                target.SleepTurns = (byte)PBEUtils.RNG.Next(Settings.SleepMinTurns, Settings.SleepMaxTurns + 1);
            }
            BroadcastStatus1(target, user, status, PBEStatusAction.Added);
            return PBEFailReason.None;
        }
        PBEFailReason ApplyStatus2IfPossible(PBEPokemon user, PBEPokemon target, PBEStatus2 status, bool broadcastFailOrEffectiveness)
        {
            PBEFailReason failReason;
            switch (status)
            {
                case PBEStatus2.Confused:
                    {
                        bool alreadyConfused = target.Status2.HasFlag(PBEStatus2.Confused);
                        if (!alreadyConfused && !target.Status2.HasFlag(PBEStatus2.Substitute))
                        {
                            target.Status2 |= PBEStatus2.Confused;
                            target.ConfusionTurns = (byte)PBEUtils.RNG.Next(Settings.ConfusionMinTurns, Settings.ConfusionMaxTurns + 1);
                            BroadcastStatus2(target, user, PBEStatus2.Confused, PBEStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = alreadyConfused ? PBEFailReason.AlreadyConfused : PBEFailReason.Default;
                        }
                        break;
                    }
                case PBEStatus2.Cursed:
                    {
                        if (!target.Status2.HasFlag(PBEStatus2.Cursed))
                        {
                            target.Status2 |= PBEStatus2.Cursed;
                            BroadcastStatus2(target, user, PBEStatus2.Cursed, PBEStatusAction.Added);
                            DealDamage(user, user, (ushort)(user.MaxHP / 2), true);
                            FaintCheck(user);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default;
                        }
                        break;
                    }
                case PBEStatus2.Flinching:
                    {
                        if (!target.Status2.HasFlag(PBEStatus2.Substitute))
                        {
                            target.Status2 |= status;
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default; // Never used by broadcastFailOrEffectiveness
                        }
                        break;
                    }
                case PBEStatus2.HelpingHand:
                    {
                        if (!HasActedThisTurn(target))
                        {
                            target.Status2 |= PBEStatus2.HelpingHand;
                            BroadcastStatus2(target, user, PBEStatus2.HelpingHand, PBEStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default;
                        }
                        break;
                    }
                case PBEStatus2.LeechSeed:
                    {
                        if (target.HasType(PBEType.Grass))
                        {
                            if (broadcastFailOrEffectiveness)
                            {
                                BroadcastEffectiveness(target, PBEEffectiveness.Ineffective);
                            }
                            failReason = PBEFailReason.Ineffective;
                        }
                        else if (!target.Status2.HasFlag(PBEStatus2.LeechSeed) && !target.Status2.HasFlag(PBEStatus2.Substitute))
                        {
                            target.Status2 |= PBEStatus2.LeechSeed;
                            target.SeededPosition = user.FieldPosition;
                            BroadcastStatus2(target, user, PBEStatus2.LeechSeed, PBEStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default;
                        }
                        break;
                    }
                case PBEStatus2.Protected:
                    {
                        // TODO: If the user goes last, fail
                        ushort chance = ushort.MaxValue;
                        for (int i = user.ExecutedMoves.Count - 1; i >= 0; i--)
                        {
                            PBEExecutedMove ex = user.ExecutedMoves[i];
                            if ((ex.Move == PBEMove.Detect || ex.Move == PBEMove.Protect) && ex.FailReason == PBEFailReason.None)
                            {
                                chance /= 2;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (PBEUtils.RNG.ApplyChance(chance, ushort.MaxValue))
                        {
                            user.Status2 |= PBEStatus2.Protected;
                            BroadcastStatus2(user, user, PBEStatus2.Protected, PBEStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default;
                        }
                        break;
                    }
                case PBEStatus2.Pumped:
                    {
                        if (!user.Status2.HasFlag(PBEStatus2.Pumped))
                        {
                            user.Status2 |= status;
                            BroadcastStatus2(user, user, PBEStatus2.Pumped, PBEStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default;
                        }
                        break;
                    }
                case PBEStatus2.Substitute:
                    {
                        bool alreadyHasSubstitute = user.Status2.HasFlag(PBEStatus2.Substitute);
                        ushort hpRequired = (ushort)(user.MaxHP / 4);
                        if (!alreadyHasSubstitute && hpRequired > 0 && user.HP > hpRequired)
                        {
                            DealDamage(user, user, hpRequired, true);
                            user.Status2 |= PBEStatus2.Substitute;
                            user.SubstituteHP = hpRequired;
                            BroadcastStatus2(user, user, PBEStatus2.Substitute, PBEStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = alreadyHasSubstitute ? PBEFailReason.AlreadySubstituted : PBEFailReason.Default;
                        }
                        break;
                    }
                case PBEStatus2.Transformed:
                    {
                        if (!target.Status2.HasFlag(PBEStatus2.Disguised)
                            && !target.Status2.HasFlag(PBEStatus2.Substitute)
                            && !user.Status2.HasFlag(PBEStatus2.Transformed)
                            && !target.Status2.HasFlag(PBEStatus2.Transformed))
                        {
                            user.Transform(target);
                            BroadcastTransform(user, target);
                            BroadcastStatus2(user, target, PBEStatus2.Transformed, PBEStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default;
                        }
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(status));
            }
            if (failReason != PBEFailReason.None && broadcastFailOrEffectiveness)
            {
                BroadcastMoveFailed(user, target, failReason);
            }
            return failReason;
        }

        PBEPkmnSwitchInPacket.PBESwitchInInfo CreateSwitchInInfo(PBEPokemon pkmn)
        {
            if (pkmn.Ability == PBEAbility.Illusion)
            {
                PBEPokemon last = pkmn.Team.Party.Last();
                if (last.HP > 0 && last.Shell.Species != pkmn.Shell.Species)
                {
                    pkmn.Status2 |= PBEStatus2.Disguised;
                    pkmn.DisguisedAsPokemon = last;
                    pkmn.VisualGender = last.Shell.Gender;
                    pkmn.VisualNickname = last.Shell.Nickname;
                    pkmn.VisualShiny = last.Shell.Shiny;
                    pkmn.VisualSpecies = last.Shell.Species;
                    return new PBEPkmnSwitchInPacket.PBESwitchInInfo(pkmn.Id, last.Id, last.Shell.Species, last.Shell.Nickname, pkmn.Shell.Level, last.Shell.Shiny, last.Shell.Gender, pkmn.HP, pkmn.MaxHP, pkmn.HPPercentage, pkmn.Status1, pkmn.FieldPosition);
                }
            }
            return new PBEPkmnSwitchInPacket.PBESwitchInInfo(pkmn.Id, pkmn.Id, pkmn.Shell.Species, pkmn.Shell.Nickname, pkmn.Shell.Level, pkmn.Shell.Shiny, pkmn.Shell.Gender, pkmn.HP, pkmn.MaxHP, pkmn.HPPercentage, pkmn.Status1, pkmn.FieldPosition);
        }
        void SwitchTwoPokemon(PBEPokemon pkmnLeaving, PBEPokemon pkmnComing, bool forced)
        {
            PBEFieldPosition pos = pkmnLeaving.FieldPosition;
            pkmnLeaving.ClearForSwitch();
            turnOrder.Remove(pkmnLeaving); // Necessary?
            ActiveBattlers.Remove(pkmnLeaving);
            BroadcastPkmnSwitchOut(pkmnLeaving, pos, forced);
            pkmnComing.FieldPosition = pos;
            ActiveBattlers.Add(pkmnComing);
            BroadcastPkmnSwitchIn(pkmnComing.Team, new[] { CreateSwitchInInfo(pkmnComing) }, forced);
            if (forced)
            {
                BroadcastDraggedOut(pkmnComing);
            }
            DoSwitchInEffects(new[] { pkmnComing });
        }

        void BasicHit(PBEPokemon user, PBEPokemon[] targets, PBEMove move, ref List<PBEExecutedMove.PBETargetSuccess> targetSuccess,
            PBEType? overridingMoveType = null,
            Func<int, int> recoilFunc = null,
            Func<PBEPokemon, PBEFailReason> beforeDoingDamage = null,
            Action<PBEPokemon> beforePostHit = null,
            Action<PBEPokemon, ushort> afterPostHit = null,
            Action beforeTargetsFaint = null)
        {
            byte hit = 0;
            int totalDamageDealt = 0;
            bool lifeOrbDamage = false;
            // Struggle sets overridingMoveType to PBEType.None
            PBEType moveType = overridingMoveType == null ? user.GetMoveType(move) : overridingMoveType.Value;
            double basePower = CalculateBasePower(user, targets, move, moveType);
            foreach (PBEPokemon target in targets)
            {
                var success = new PBEExecutedMove.PBETargetSuccess
                {
                    Target = target,
                    OldHP = target.HP,
                    OldHPPercentage = target.HPPercentage
                };
                if (MissCheck(user, target, move))
                {
                    success.Missed = true;
                }
                else
                {
                    double damageMultiplier = targets.Length > 1 ? 0.75 : 1.0;
                    TypeCheck(user, target, moveType, out PBEEffectiveness moveEffectiveness, ref damageMultiplier, false);
                    if (moveEffectiveness == PBEEffectiveness.Ineffective)
                    {
                        success.FailReason = PBEFailReason.Ineffective;
                        BroadcastEffectiveness(target, PBEEffectiveness.Ineffective);
                    }
                    else
                    {
                        // Brick Break destroys Light Screen and Reflect before doing damage
                        // Dream Eater checks for sleep before doing damage
                        if (beforeDoingDamage != null)
                        {
                            success.FailReason = beforeDoingDamage.Invoke(target);
                            if (success.FailReason != PBEFailReason.None)
                            {
                                goto record;
                            }
                        }

                        success.CriticalHit = CritCheck(user, target, move);
                        damageMultiplier *= CalculateDamageMultiplier(user, target, move, moveType, moveEffectiveness, success.CriticalHit);
                        ushort damage = (ushort)(damageMultiplier * CalculateDamage(user, target, move, moveType, PBEMoveData.Data[move].Category, basePower, success.CriticalHit));
                        ushort damageDealt = DealDamage(user, target, damage, false);
                        totalDamageDealt += damageDealt;

                        BroadcastEffectiveness(target, moveEffectiveness);
                        if (success.CriticalHit)
                        {
                            BroadcastMoveCrit();
                        }

                        hit++;
                        if (!target.Status2.HasFlag(PBEStatus2.Substitute))
                        {
                            lifeOrbDamage = true;
                        }
                        // Target's statuses are assigned and target's stats are changed before post-hit effects
                        beforePostHit?.Invoke(target);
                        DoPostHitEffects(user, target, move);
                        // HP-draining moves restore HP after post-hit effects
                        afterPostHit?.Invoke(target, damageDealt);
                    }
                }
            record:
                success.NewHP = target.HP;
                success.NewHPPercentage = target.HPPercentage;
                targetSuccess.Add(success);
            }

            if (hit > 0)
            {
                // User's stats change before the targets faint if at least one was hit
                beforeTargetsFaint?.Invoke();
                foreach (PBEPokemon target in targets)
                {
                    FaintCheck(target);
                }
            }
            ushort recoilDamage = (ushort)(recoilFunc == null ? 0 : recoilFunc.Invoke(totalDamageDealt));
            DoPostAttackedEffects(user, !lifeOrbDamage, recoilDamage);
        }
        void FixedDamageHit(PBEPokemon user, PBEPokemon[] targets, PBEMove move, ref List<PBEExecutedMove.PBETargetSuccess> targetSuccess, Func<PBEPokemon, ushort> damageFunc,
            Func<PBEPokemon, PBEFailReason> beforeMissCheck = null,
            Action beforeTargetsFaint = null)
        {
            byte hit = 0;
            PBEType moveType = user.GetMoveType(move);
            foreach (PBEPokemon target in targets)
            {
                var success = new PBEExecutedMove.PBETargetSuccess
                {
                    Target = target,
                    OldHP = target.HP,
                    OldHPPercentage = target.HPPercentage
                };
                // Endeavor fails if the target's HP is <= the user's HP
                // One hit knockout moves fail if the target's level is > the user's level
                if (beforeMissCheck != null)
                {
                    success.FailReason = beforeMissCheck.Invoke(target);
                    if (success.FailReason != PBEFailReason.None)
                    {
                        goto record;
                    }
                }
                if (MissCheck(user, target, move))
                {
                    success.Missed = true;
                }
                else
                {
                    double d = 1.0;
                    TypeCheck(user, target, moveType, out PBEEffectiveness moveEffectiveness, ref d, false);
                    if (moveEffectiveness == PBEEffectiveness.Ineffective)
                    {
                        success.FailReason = PBEFailReason.Ineffective;
                        BroadcastEffectiveness(target, PBEEffectiveness.Ineffective);
                    }
                    else
                    {
                        // Damage func is run and the output is dealt to target
                        DealDamage(user, target, damageFunc.Invoke(target), false);

                        hit++;
                        DoPostHitEffects(user, target, move);
                    }
                }
            record:
                success.NewHP = target.HP;
                success.NewHPPercentage = target.HPPercentage;
                targetSuccess.Add(success);
            }

            if (hit > 0)
            {
                // "It's a one-hit KO!"
                beforeTargetsFaint?.Invoke();
                foreach (PBEPokemon target in targets)
                {
                    FaintCheck(target);
                }
            }
            DoPostAttackedEffects(user, true);
        }

        void Ef_TryForceStatus1(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStatus1 status)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                foreach (PBEPokemon target in targets)
                {
                    var success = new PBEExecutedMove.PBETargetSuccess
                    {
                        Target = target,
                        OldHP = target.HP,
                        OldHPPercentage = target.HPPercentage
                    };
                    if (MissCheck(user, target, move))
                    {
                        success.Missed = true;
                    }
                    else
                    {
                        double d = 1.0;
                        PBEType moveType = user.GetMoveType(move);
                        TypeCheck(user, target, moveType, out PBEEffectiveness moveEffectiveness, ref d, true);
                        if (moveEffectiveness == PBEEffectiveness.Ineffective) // Paralysis, Normalize
                        {
                            success.FailReason = PBEFailReason.Ineffective;
                            BroadcastEffectiveness(target, moveEffectiveness);
                        }
                        else
                        {
                            success.FailReason = ApplyStatus1IfPossible(user, target, status, true);
                        }
                    }
                    success.NewHP = target.HP;
                    success.NewHPPercentage = target.HPPercentage;
                    targetSuccess.Add(success);
                }
            }
            RecordExecutedMove(user, move, PBEFailReason.NoTarget, targetSuccess);
            return;
        }
        void Ef_TryForceStatus2(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStatus2 status)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                foreach (PBEPokemon target in targets)
                {
                    var success = new PBEExecutedMove.PBETargetSuccess
                    {
                        Target = target,
                        OldHP = target.HP,
                        OldHPPercentage = target.HPPercentage
                    };
                    if (MissCheck(user, target, move))
                    {
                        success.Missed = true;
                    }
                    else
                    {
                        success.FailReason = ApplyStatus2IfPossible(user, target, status, true);
                    }
                    success.NewHP = target.HP;
                    success.NewHPPercentage = target.HPPercentage;
                    targetSuccess.Add(success);
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_TryForceBattleStatus(PBEPokemon user, PBEMove move, PBEBattleStatus status)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);

            switch (status)
            {
                case PBEBattleStatus.TrickRoom:
                    {
                        if (!BattleStatus.HasFlag(PBEBattleStatus.TrickRoom))
                        {
                            BattleStatus |= PBEBattleStatus.TrickRoom;
                            TrickRoomCount = 5;
                            BroadcastBattleStatus(PBEBattleStatus.TrickRoom, PBEBattleStatusAction.Added);
                        }
                        else
                        {
                            BattleStatus &= ~PBEBattleStatus.TrickRoom;
                            TrickRoomCount = 0;
                            BroadcastBattleStatus(PBEBattleStatus.TrickRoom, PBEBattleStatusAction.Cleared);
                        }
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(status));
            }
            RecordExecutedMove(user, move, PBEFailReason.None, new PBEExecutedMove.PBETargetSuccess[0]);
        }
        void Ef_TryForceTeamStatus(PBEPokemon user, PBEMove move, PBETeamStatus status)
        {
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);

            PBETeam opposingTeam = user.Team == Teams[0] ? Teams[1] : Teams[0];
            switch (status)
            {
                case PBETeamStatus.LightScreen:
                    {
                        if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.LightScreen))
                        {
                            user.Team.TeamStatus |= PBETeamStatus.LightScreen;
                            user.Team.LightScreenCount = (byte)(Settings.LightScreenTurns + (user.Item == PBEItem.LightClay ? Settings.LightClayTurnExtension : 0));
                            BroadcastTeamStatus(user.Team, PBETeamStatus.LightScreen, PBETeamStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default;
                        }
                        break;
                    }
                case PBETeamStatus.LuckyChant:
                    {
                        if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.LuckyChant))
                        {
                            user.Team.TeamStatus |= PBETeamStatus.LuckyChant;
                            user.Team.LuckyChantCount = 5;
                            BroadcastTeamStatus(user.Team, PBETeamStatus.LuckyChant, PBETeamStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default;
                        }
                        break;
                    }
                case PBETeamStatus.Reflect:
                    {
                        if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.Reflect))
                        {
                            user.Team.TeamStatus |= PBETeamStatus.Reflect;
                            user.Team.ReflectCount = (byte)(Settings.ReflectTurns + (user.Item == PBEItem.LightClay ? Settings.LightClayTurnExtension : 0));
                            BroadcastTeamStatus(user.Team, PBETeamStatus.Reflect, PBETeamStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default;
                        }
                        break;
                    }
                case PBETeamStatus.Spikes:
                    {
                        if (opposingTeam.SpikeCount < 3)
                        {
                            opposingTeam.TeamStatus |= PBETeamStatus.Spikes;
                            opposingTeam.SpikeCount++;
                            BroadcastTeamStatus(opposingTeam, PBETeamStatus.Spikes, PBETeamStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default;
                        }
                        break;
                    }
                case PBETeamStatus.StealthRock:
                    {
                        if (!opposingTeam.TeamStatus.HasFlag(PBETeamStatus.StealthRock))
                        {
                            opposingTeam.TeamStatus |= PBETeamStatus.StealthRock;
                            BroadcastTeamStatus(opposingTeam, PBETeamStatus.StealthRock, PBETeamStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default;
                        }
                        break;
                    }
                case PBETeamStatus.ToxicSpikes:
                    {
                        if (opposingTeam.ToxicSpikeCount < 2)
                        {
                            opposingTeam.TeamStatus |= PBETeamStatus.ToxicSpikes;
                            opposingTeam.ToxicSpikeCount++;
                            BroadcastTeamStatus(opposingTeam, PBETeamStatus.ToxicSpikes, PBETeamStatusAction.Added);
                            failReason = PBEFailReason.None;
                        }
                        else
                        {
                            failReason = PBEFailReason.Default;
                        }
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(status));
            }
            if (failReason != PBEFailReason.None)
            {
                BroadcastMoveFailed(user, user, PBEFailReason.Default);
            }
            RecordExecutedMove(user, move, failReason, new PBEExecutedMove.PBETargetSuccess[0]);
        }
        void Ef_TryForceWeather(PBEPokemon user, PBEMove move, PBEWeather weather)
        {
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (Weather == weather)
            {
                failReason = PBEFailReason.Default;
                BroadcastMoveFailed(user, user, failReason);
            }
            else
            {
                failReason = PBEFailReason.None;
                byte turns;
                PBEItem extensionItem;
                byte itemTurnExtension;
                switch (weather)
                {
                    case PBEWeather.Hailstorm:
                        {
                            turns = Settings.HailTurns;
                            extensionItem = PBEItem.IcyRock;
                            itemTurnExtension = Settings.IcyRockTurnExtension;
                            break;
                        }
                    case PBEWeather.HarshSunlight:
                        {
                            turns = Settings.SunTurns;
                            extensionItem = PBEItem.HeatRock;
                            itemTurnExtension = Settings.HeatRockTurnExtension;
                            break;
                        }
                    case PBEWeather.Rain:
                        {
                            turns = Settings.RainTurns;
                            extensionItem = PBEItem.DampRock;
                            itemTurnExtension = Settings.DampRockTurnExtension;
                            break;
                        }
                    case PBEWeather.Sandstorm:
                        {
                            turns = Settings.SandstormTurns;
                            extensionItem = PBEItem.SmoothRock;
                            itemTurnExtension = Settings.SmoothRockTurnExtension;
                            break;
                        }
                    default: throw new ArgumentOutOfRangeException(nameof(weather));
                }
                Weather = weather;
                WeatherCounter = (byte)(turns + (user.Item == extensionItem ? itemTurnExtension : 0));
                BroadcastWeather(Weather, PBEWeatherAction.Added);
            }
            RecordExecutedMove(user, move, failReason, new PBEExecutedMove.PBETargetSuccess[0]);
        }
        void Ef_Hit__MaybeInflictStatus1(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStatus1 status, int chanceToInflict)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                void BeforePostHit(PBEPokemon target)
                {
                    if (target.HP > 0 && !target.Status2.HasFlag(PBEStatus2.Substitute) && PBEUtils.RNG.ApplyChance(chanceToInflict, 100))
                    {
                        ApplyStatus1IfPossible(user, target, status, false);
                    }
                }
                BasicHit(user, targets, move, ref targetSuccess, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Hit__MaybeInflictStatus2(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStatus2 status, int chanceToInflict)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                void BeforePostHit(PBEPokemon target)
                {
                    if (target.HP > 0 && !target.Status2.HasFlag(PBEStatus2.Substitute) && PBEUtils.RNG.ApplyChance(chanceToInflict, 100))
                    {
                        ApplyStatus2IfPossible(user, target, status, false);
                    }
                }
                BasicHit(user, targets, move, ref targetSuccess, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }

        void Ef_ChangeTargetStats(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStat[] stats, short[] changes)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                foreach (PBEPokemon target in targets)
                {
                    var success = new PBEExecutedMove.PBETargetSuccess
                    {
                        Target = target,
                        OldHP = target.HP,
                        OldHPPercentage = target.HPPercentage
                    };
                    if (MissCheck(user, target, move))
                    {
                        success.Missed = true;
                    }
                    else
                    {
                        if (target.Status2.HasFlag(PBEStatus2.Substitute))
                        {
                            success.FailReason = PBEFailReason.Default;
                            BroadcastMoveFailed(user, target, PBEFailReason.Default);
                        }
                        else
                        {
                            for (int i = 0; i < stats.Length; i++)
                            {
                                ApplyStatChange(target, stats[i], changes[i]);
                            }
                        }
                    }
                    success.NewHP = target.HP;
                    success.NewHPPercentage = target.HPPercentage;
                    targetSuccess.Add(success);
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_ChangeUserStats(PBEPokemon user, PBEMove move, PBEStat[] stats, short[] changes)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);

            for (int i = 0; i < stats.Length; i++)
            {
                ApplyStatChange(user, stats[i], changes[i]);
            }

            RecordExecutedMove(user, move, PBEFailReason.None, new PBEExecutedMove.PBETargetSuccess[0]);
        }
        void Ef_Hit__MaybeChangeTargetStats(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStat[] stats, short[] changes, int chanceToChangeStats)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                void BeforePostHit(PBEPokemon target)
                {
                    if (target.HP > 0 && !target.Status2.HasFlag(PBEStatus2.Substitute) && PBEUtils.RNG.ApplyChance(chanceToChangeStats, 100))
                    {
                        for (int i = 0; i < stats.Length; i++)
                        {
                            ApplyStatChange(target, stats[i], changes[i]);
                        }
                    }
                }
                BasicHit(user, targets, move, ref targetSuccess, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Hit__MaybeChangeUserStats(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStat[] stats, short[] changes, int chanceToChangeStats)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                void BeforeTargetsFaint()
                {
                    if (user.HP > 0 && PBEUtils.RNG.ApplyChance(chanceToChangeStats, 100))
                    {
                        for (int i = 0; i < stats.Length; i++)
                        {
                            ApplyStatChange(user, stats[i], changes[i]);
                        }
                    }
                }
                BasicHit(user, targets, move, ref targetSuccess, beforeTargetsFaint: BeforeTargetsFaint);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }

        void Ef_BrickBreak(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                PBEFailReason BeforeDoingDamage(PBEPokemon target)
                {
                    // Verified: Reflect then Light Screen
                    if (target.Team.TeamStatus.HasFlag(PBETeamStatus.Reflect))
                    {
                        target.Team.TeamStatus &= ~PBETeamStatus.Reflect;
                        target.Team.ReflectCount = 0;
                        BroadcastTeamStatus(target.Team, PBETeamStatus.Reflect, PBETeamStatusAction.Cleared);
                    }
                    if (target.Team.TeamStatus.HasFlag(PBETeamStatus.LightScreen))
                    {
                        target.Team.TeamStatus &= ~PBETeamStatus.LightScreen;
                        target.Team.LightScreenCount = 0;
                        BroadcastTeamStatus(target.Team, PBETeamStatus.LightScreen, PBETeamStatusAction.Cleared);
                    }
                    return PBEFailReason.None;
                }
                BasicHit(user, targets, move, ref targetSuccess, beforeDoingDamage: BeforeDoingDamage);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Dig(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBETarget requestedTargets)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
        top:
            if (user.Status2.HasFlag(PBEStatus2.Underground))
            {
                user.TempLockedMove = PBEMove.None;
                user.TempLockedTargets = PBETarget.None;
                BroadcastMoveLock(user, user.TempLockedMove, user.TempLockedTargets, PBEMoveLockType.Temporary);
                user.Status2 &= ~PBEStatus2.Underground;
                BroadcastStatus2(user, user, PBEStatus2.Underground, PBEStatusAction.Ended);
                if (targets.Length == 0)
                {
                    failReason = PBEFailReason.NoTarget;
                    BroadcastMoveFailed(user, user, failReason);
                }
                else
                {
                    failReason = PBEFailReason.None;
                    BasicHit(user, targets, move, ref targetSuccess);
                }
            }
            else
            {
                PPReduce(user, move);
                user.TempLockedMove = move;
                user.TempLockedTargets = requestedTargets;
                BroadcastMoveLock(user, user.TempLockedMove, user.TempLockedTargets, PBEMoveLockType.Temporary);
                user.Status2 |= PBEStatus2.Underground;
                BroadcastStatus2(user, user, PBEStatus2.Underground, PBEStatusAction.Added);
                if (PowerHerbCheck(user))
                {
                    goto top;
                }
                else
                {
                    failReason = PBEFailReason.None;
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Dive(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBETarget requestedTargets)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
        top:
            if (user.Status2.HasFlag(PBEStatus2.Underwater))
            {
                user.TempLockedMove = PBEMove.None;
                user.TempLockedTargets = PBETarget.None;
                BroadcastMoveLock(user, user.TempLockedMove, user.TempLockedTargets, PBEMoveLockType.Temporary);
                user.Status2 &= ~PBEStatus2.Underwater;
                BroadcastStatus2(user, user, PBEStatus2.Underwater, PBEStatusAction.Ended);
                if (targets.Length == 0)
                {
                    failReason = PBEFailReason.NoTarget;
                    BroadcastMoveFailed(user, user, failReason);
                }
                else
                {
                    failReason = PBEFailReason.None;
                    BasicHit(user, targets, move, ref targetSuccess);
                }
            }
            else
            {
                PPReduce(user, move);
                user.TempLockedMove = move;
                user.TempLockedTargets = requestedTargets;
                BroadcastMoveLock(user, user.TempLockedMove, user.TempLockedTargets, PBEMoveLockType.Temporary);
                user.Status2 |= PBEStatus2.Underwater;
                BroadcastStatus2(user, user, PBEStatus2.Underwater, PBEStatusAction.Added);
                if (PowerHerbCheck(user))
                {
                    goto top;
                }
                else
                {
                    failReason = PBEFailReason.None;
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Fail(PBEPokemon user, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            BroadcastMoveFailed(user, user, PBEFailReason.Default);
            RecordExecutedMove(user, move, PBEFailReason.Default, new PBEExecutedMove.PBETargetSuccess[0]);
        }
        void Ef_Fly(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBETarget requestedTargets)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
        top:
            if (user.Status2.HasFlag(PBEStatus2.Airborne))
            {
                user.TempLockedMove = PBEMove.None;
                user.TempLockedTargets = PBETarget.None;
                BroadcastMoveLock(user, user.TempLockedMove, user.TempLockedTargets, PBEMoveLockType.Temporary);
                user.Status2 &= ~PBEStatus2.Airborne;
                BroadcastStatus2(user, user, PBEStatus2.Airborne, PBEStatusAction.Ended);
                if (targets.Length == 0)
                {
                    failReason = PBEFailReason.NoTarget;
                    BroadcastMoveFailed(user, user, failReason);
                }
                else
                {
                    failReason = PBEFailReason.None;
                    BasicHit(user, targets, move, ref targetSuccess);
                }
            }
            else
            {
                PPReduce(user, move);
                user.TempLockedMove = move;
                user.TempLockedTargets = requestedTargets;
                BroadcastMoveLock(user, user.TempLockedMove, user.TempLockedTargets, PBEMoveLockType.Temporary);
                user.Status2 |= PBEStatus2.Airborne;
                BroadcastStatus2(user, user, PBEStatus2.Airborne, PBEStatusAction.Added);
                if (PowerHerbCheck(user))
                {
                    goto top;
                }
                else
                {
                    failReason = PBEFailReason.None;
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Hit(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                BasicHit(user, targets, move, ref targetSuccess);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Selfdestruct(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            // TODO: Damp
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            DealDamage(user, user, user.MaxHP, true, ignoreSturdy: true);
            FaintCheck(user);
            if (targets.Length == 0) // You still faint if there are no targets
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                BasicHit(user, targets, move, ref targetSuccess);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }

        void Ef_Endeavor(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                ushort DamageFunc(PBEPokemon target)
                {
                    return (ushort)(target.HP - user.HP);
                }
                PBEFailReason BeforeMissCheck(PBEPokemon target)
                {
                    if (target.HP <= user.HP)
                    {
                        BroadcastMoveFailed(user, target, PBEFailReason.Default);
                        return PBEFailReason.Default;
                    }
                    return PBEFailReason.None;
                }
                FixedDamageHit(user, targets, move, ref targetSuccess, DamageFunc, beforeMissCheck: BeforeMissCheck);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_FinalGambit(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                ushort DamageFunc(PBEPokemon target)
                {
                    ushort oldHP = user.HP;
                    DealDamage(user, user, oldHP, true);
                    FaintCheck(user);
                    return oldHP;
                }
                FixedDamageHit(user, targets, move, ref targetSuccess, DamageFunc);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_OneHitKnockout(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                ushort DamageFunc(PBEPokemon target)
                {
                    return target.HP;
                }
                PBEFailReason BeforeMissCheck(PBEPokemon target)
                {
                    if (target.Shell.Level > user.Shell.Level)
                    {
                        BroadcastMoveFailed(user, target, PBEFailReason.OneHitKnockoutUnaffected);
                        return PBEFailReason.OneHitKnockoutUnaffected;
                    }
                    else
                    {
                        return PBEFailReason.None;
                    }
                }
                void BeforeTargetsFaint()
                {
                    if (targets.Any(p => p.HP == 0))
                    {
                        BroadcastOneHitKnockout();
                    }
                }
                FixedDamageHit(user, targets, move, ref targetSuccess, DamageFunc, beforeMissCheck: BeforeMissCheck, beforeTargetsFaint: BeforeTargetsFaint);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Psywave(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                ushort DamageFunc(PBEPokemon target)
                {
                    return (ushort)(user.Shell.Level * (PBEUtils.RNG.Next(0, Settings.MaxLevel + 1) + (Settings.MaxLevel / 2)) / Settings.MaxLevel);
                }
                FixedDamageHit(user, targets, move, ref targetSuccess, DamageFunc);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_SeismicToss(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                ushort DamageFunc(PBEPokemon target)
                {
                    return user.Shell.Level;
                }
                FixedDamageHit(user, targets, move, ref targetSuccess, DamageFunc);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_SetDamage(PBEPokemon user, PBEPokemon[] targets, PBEMove move, int damage)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                ushort DamageFunc(PBEPokemon target)
                {
                    return (ushort)damage;
                }
                FixedDamageHit(user, targets, move, ref targetSuccess, DamageFunc);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_SuperFang(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                ushort DamageFunc(PBEPokemon target)
                {
                    return (ushort)(target.HP / 2);
                }
                FixedDamageHit(user, targets, move, ref targetSuccess, DamageFunc);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }

        void Ef_HPDrain(PBEPokemon user, PBEPokemon[] targets, PBEMove move, int percentRestored)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                PBEFailReason BeforeDoingDamage(PBEPokemon target)
                {
                    if (move == PBEMove.DreamEater && target.Status1 != PBEStatus1.Asleep)
                    {
                        BroadcastMoveFailed(user, target, PBEFailReason.Default);
                        return PBEFailReason.Default;
                    }
                    else
                    {
                        return PBEFailReason.None;
                    }
                }
                void AfterPostHit(PBEPokemon target, ushort damageDealt)
                {
                    ushort restoreAmt = (ushort)(damageDealt * (percentRestored / 100.0));
                    if (user.Item == PBEItem.BigRoot)
                    {
                        restoreAmt += (ushort)(restoreAmt * 0.3);
                    }
                    if (target.Ability == PBEAbility.LiquidOoze)
                    {
                        DealDamage(target, user, restoreAmt, true, ignoreSturdy: true); // Verified: it does ignore sturdy.
                        BroadcastAbility(target, user, PBEAbility.LiquidOoze, PBEAbilityAction.Damage);
                        FaintCheck(user);
                    }
                    else
                    {
                        HealDamage(user, restoreAmt);
                        BroadcastHPDrained(target);
                    }
                }
                BasicHit(user, targets, move, ref targetSuccess, beforeDoingDamage: BeforeDoingDamage, afterPostHit: AfterPostHit);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Moonlight(PBEPokemon user, PBEMove move)
        {
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);

            double percentage;
            switch (Weather)
            {
                case PBEWeather.None: percentage = 0.50; break;
                case PBEWeather.HarshSunlight: percentage = 0.66; break;
                default: percentage = 0.25; break;
            }
            ushort amtRestored = HealDamage(user, (ushort)(user.MaxHP * percentage));
            if (amtRestored == 0)
            {
                failReason = PBEFailReason.HPFull;
                BroadcastMoveFailed(user, user, failReason);
            }
            else
            {
                failReason = PBEFailReason.None;
            }
            RecordExecutedMove(user, move, failReason, new PBEExecutedMove.PBETargetSuccess[0]);
        }
        void Ef_PainSplit(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                foreach (PBEPokemon target in targets)
                {
                    var success = new PBEExecutedMove.PBETargetSuccess
                    {
                        Target = target,
                        OldHP = target.HP,
                        OldHPPercentage = target.HPPercentage
                    };
                    if (MissCheck(user, target, move))
                    {
                        success.Missed = true;
                    }
                    else
                    {
                        if (target.Status2.HasFlag(PBEStatus2.Substitute))
                        {
                            success.FailReason = PBEFailReason.Default;
                            BroadcastMoveFailed(user, target, PBEFailReason.Default);
                        }
                        else
                        {
                            ushort total = (ushort)(user.HP + target.HP);
                            ushort hp = (ushort)(total / 2);
                            foreach (PBEPokemon pkmn in new PBEPokemon[] { user, target })
                            {
                                if (hp >= pkmn.HP)
                                {
                                    HealDamage(pkmn, (ushort)(hp - pkmn.HP));
                                }
                                else
                                {
                                    DealDamage(user, pkmn, (ushort)(pkmn.HP - hp), true);
                                    DoPostHitEffects(user, pkmn, move);
                                }
                            }

                            BroadcastPainSplit(user, target);
                        }
                    }
                    success.NewHP = target.HP;
                    success.NewHPPercentage = target.HPPercentage;
                    targetSuccess.Add(success);
                }
                DoPostAttackedEffects(user, true);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_RestoreTargetHP(PBEPokemon user, PBEPokemon[] targets, PBEMove move, int percentRestored)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                foreach (PBEPokemon target in targets)
                {
                    var success = new PBEExecutedMove.PBETargetSuccess
                    {
                        Target = target,
                        OldHP = target.HP,
                        OldHPPercentage = target.HPPercentage
                    };
                    if (MissCheck(user, target, move))
                    {
                        success.Missed = true;
                    }
                    else
                    {
                        if (target.Status2.HasFlag(PBEStatus2.Substitute))
                        {
                            success.FailReason = PBEFailReason.Default;
                            BroadcastMoveFailed(user, target, PBEFailReason.Default);
                        }
                        else
                        {
                            ushort amtRestored = HealDamage(target, (ushort)(target.MaxHP * (percentRestored / 100.0)));
                            if (amtRestored == 0)
                            {
                                success.FailReason = PBEFailReason.HPFull;
                                BroadcastMoveFailed(user, target, PBEFailReason.HPFull);
                            }
                        }
                    }
                    success.NewHP = target.HP;
                    success.NewHPPercentage = target.HPPercentage;
                    targetSuccess.Add(success);
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_RestoreUserHP(PBEPokemon user, PBEMove move, int percentRestored)
        {
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            ushort amtRestored = HealDamage(user, (ushort)(user.MaxHP * (percentRestored / 100.0)));
            if (amtRestored == 0)
            {
                failReason = PBEFailReason.HPFull;
                BroadcastMoveFailed(user, user, PBEFailReason.HPFull);
            }
            else
            {
                failReason = PBEFailReason.None;
            }
            RecordExecutedMove(user, move, failReason, new PBEExecutedMove.PBETargetSuccess[0]);
        }

        void Ef_Recoil(PBEPokemon user, PBEPokemon[] targets, PBEMove move, int denominator)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                int RecoilFunc(int totalDamageDealt)
                {
                    return user.Ability == PBEAbility.RockHead ? 0 : totalDamageDealt / denominator;
                }
                BasicHit(user, targets, move, ref targetSuccess, recoilFunc: RecoilFunc);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Recoil3__MaybeInflictStatus1With10PercentChance(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStatus1 status)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                int RecoilFunc(int totalDamageDealt)
                {
                    return user.Ability == PBEAbility.RockHead ? 0 : totalDamageDealt / 3;
                }
                void BeforePostHit(PBEPokemon target)
                {
                    if (target.HP > 0 && !target.Status2.HasFlag(PBEStatus2.Substitute) && PBEUtils.RNG.ApplyChance(10, 100))
                    {
                        ApplyStatus1IfPossible(user, target, status, false);
                    }
                }
                BasicHit(user, targets, move, ref targetSuccess, recoilFunc: RecoilFunc, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Struggle(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastStruggle(user);
            BroadcastMoveUsed(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                int RecoilFunc(int totalDamageDealt)
                {
                    return user.MaxHP / 4;
                }
                BasicHit(user, targets, move, ref targetSuccess, overridingMoveType: PBEType.None, recoilFunc: RecoilFunc);
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }

        void Ef_Curse(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                if (user.HasType(PBEType.Ghost))
                {
                    PBEPokemon target = targets[0];
                    if (target == user) // Just gained the Ghost type after selecting the move, so get a random target
                    {
                        PBEFieldPosition prioritizedPos = GetPositionAcross(BattleFormat, user.FieldPosition);
                        PBETarget moveTarget;
                        if (prioritizedPos == PBEFieldPosition.Left)
                        {
                            moveTarget = PBETarget.FoeLeft;
                        }
                        else if (prioritizedPos == PBEFieldPosition.Center)
                        {
                            moveTarget = PBETarget.FoeCenter;
                        }
                        else
                        {
                            moveTarget = PBETarget.FoeRight;
                        }

                        PBEPokemon[] runtimeTargets = GetRuntimeTargets(user, moveTarget, false);
                        if (runtimeTargets.Length == 0)
                        {
                            failReason = PBEFailReason.NoTarget;
                            BroadcastMoveFailed(user, user, failReason);
                        }
                        else
                        {
                            failReason = PBEFailReason.None;
                            target = runtimeTargets[0];
                        }
                    }

                    var success = new PBEExecutedMove.PBETargetSuccess
                    {
                        Target = target,
                        OldHP = target.HP,
                        OldHPPercentage = target.HPPercentage
                    };
                    if (MissCheck(user, target, move))
                    {
                        success.Missed = true;
                    }
                    else
                    {
                        success.FailReason = ApplyStatus2IfPossible(user, target, PBEStatus2.Cursed, true);
                    }
                    success.NewHP = target.HP;
                    success.NewHPPercentage = target.HPPercentage;
                    targetSuccess.Add(success);
                }
                else
                {
                    if (user.SpeedChange == -Settings.MaxStatChange
                        && user.AttackChange == Settings.MaxStatChange
                        && user.DefenseChange == Settings.MaxStatChange)
                    {
                        failReason = PBEFailReason.Default;
                        BroadcastMoveFailed(user, user, PBEFailReason.Default);
                    }
                    else
                    {
                        failReason = PBEFailReason.None;
                        ApplyStatChange(user, PBEStat.Speed, -1);
                        ApplyStatChange(user, PBEStat.Attack, +1);
                        ApplyStatChange(user, PBEStat.Defense, +1);
                    }
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Flatter(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                foreach (PBEPokemon target in targets)
                {
                    var success = new PBEExecutedMove.PBETargetSuccess
                    {
                        Target = target,
                        OldHP = target.HP,
                        OldHPPercentage = target.HPPercentage
                    };
                    if (MissCheck(user, target, move))
                    {
                        success.Missed = true;
                    }
                    else
                    {
                        ApplyStatChange(target, PBEStat.SpAttack, +1);
                        ApplyStatus2IfPossible(user, target, PBEStatus2.Confused, true);
                    }
                    success.NewHP = target.HP;
                    success.NewHPPercentage = target.HPPercentage;
                    targetSuccess.Add(success);
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_GastroAcid(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                foreach (PBEPokemon target in targets)
                {
                    var success = new PBEExecutedMove.PBETargetSuccess
                    {
                        Target = target,
                        OldHP = target.HP,
                        OldHPPercentage = target.HPPercentage
                    };
                    if (MissCheck(user, target, move))
                    {
                        success.Missed = true;
                    }
                    else
                    {
                        if (target.Ability == PBEAbility.Multitype || target.Ability == PBEAbility.None)
                        {
                            success.FailReason = PBEFailReason.Default;
                            BroadcastMoveFailed(user, target, PBEFailReason.Default);
                        }
                        else
                        {
                            target.Ability = PBEAbility.None;
                            BroadcastAbility(target, user, PBEAbility.None, PBEAbilityAction.Changed);
                            IllusionBreak(target, user);
                        }
                    }
                    success.NewHP = target.HP;
                    success.NewHPPercentage = target.HPPercentage;
                    targetSuccess.Add(success);
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Growth(PBEPokemon user, PBEMove move)
        {
            short change = (short)(Weather == PBEWeather.HarshSunlight ? +2 : +1);
            Ef_ChangeUserStats(user, move, new PBEStat[] { PBEStat.Attack, PBEStat.SpAttack }, new short[] { change, change });
        }
        void Ef_HelpingHand(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                foreach (PBEPokemon target in targets)
                {
                    var success = new PBEExecutedMove.PBETargetSuccess
                    {
                        Target = target,
                        OldHP = target.HP,
                        OldHPPercentage = target.HPPercentage
                    };
                    // TODO: When triple battle shifting happens, all moves that can target allies but not the user will have to check if the user targetted itself due to shifting.
                    // For now, I'll put this check here, because this is the only move that will attempt to target the user when the move cannot normally do so (single/rotation battle).
                    if (target == user)
                    {
                        success.FailReason = PBEFailReason.NoTarget;
                        BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
                    }
                    else
                    {
                        if (MissCheck(user, target, move))
                        {
                            success.Missed = true;
                        }
                        else
                        {
                            success.FailReason = ApplyStatus2IfPossible(user, target, PBEStatus2.HelpingHand, true);
                        }
                    }
                    success.NewHP = target.HP;
                    success.NewHPPercentage = target.HPPercentage;
                    targetSuccess.Add(success);
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Metronome(PBEPokemon user, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            // Record before the called move is recorded
            RecordExecutedMove(user, move, PBEFailReason.None, new PBEExecutedMove.PBETargetSuccess[0]);

            PBEMove calledMove = PBEMoveData.Data.Where(t => !t.Value.Flags.HasFlag(PBEMoveFlag.BlockedByMetronome)).Select(t => t.Key).Sample();
            calledFromOtherMove = true;
            UseMove(user, calledMove, GetRandomTargetForMetronome(user, calledMove));
            calledFromOtherMove = false;
        }
        void Ef_PsychUp(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                foreach (PBEPokemon target in targets)
                {
                    var success = new PBEExecutedMove.PBETargetSuccess
                    {
                        Target = target,
                        OldHP = target.HP,
                        OldHPPercentage = target.HPPercentage
                    };
                    if (MissCheck(user, target, move))
                    {
                        success.Missed = true;
                    }
                    else
                    {
                        user.AttackChange = target.AttackChange;
                        user.DefenseChange = target.DefenseChange;
                        user.SpAttackChange = target.SpAttackChange;
                        user.SpDefenseChange = target.SpDefenseChange;
                        user.SpeedChange = target.SpeedChange;
                        user.AccuracyChange = target.AccuracyChange;
                        user.EvasionChange = target.EvasionChange;
                        BroadcastPsychUp(user, target);
                    }
                    success.NewHP = target.HP;
                    success.NewHPPercentage = target.HPPercentage;
                    targetSuccess.Add(success);
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Swagger(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                foreach (PBEPokemon target in targets)
                {
                    var success = new PBEExecutedMove.PBETargetSuccess
                    {
                        Target = target,
                        OldHP = target.HP,
                        OldHPPercentage = target.HPPercentage
                    };
                    if (MissCheck(user, target, move))
                    {
                        success.Missed = true;
                    }
                    else
                    {
                        ApplyStatChange(target, PBEStat.Attack, +2);
                        ApplyStatus2IfPossible(user, target, PBEStatus2.Confused, true);
                    }
                    success.NewHP = target.HP;
                    success.NewHPPercentage = target.HPPercentage;
                    targetSuccess.Add(success);
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
        void Ef_Whirlwind(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            var targetSuccess = new List<PBEExecutedMove.PBETargetSuccess>();
            PBEFailReason failReason;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                failReason = PBEFailReason.NoTarget;
                BroadcastMoveFailed(user, user, PBEFailReason.NoTarget);
            }
            else
            {
                failReason = PBEFailReason.None;
                foreach (PBEPokemon target in targets)
                {
                    var success = new PBEExecutedMove.PBETargetSuccess
                    {
                        Target = target,
                        OldHP = target.HP,
                        OldHPPercentage = target.HPPercentage
                    };
                    if (MissCheck(user, target, move))
                    {
                        success.Missed = true;
                    }
                    else
                    {
                        IEnumerable<PBEPokemon> possibleSwitcheroonies = target.Team.Party.Where(p => p.FieldPosition == PBEFieldPosition.None);
                        if (possibleSwitcheroonies.Count() == 0)
                        {
                            success.FailReason = PBEFailReason.Default;
                            BroadcastMoveFailed(user, target, PBEFailReason.Default);
                        }
                        else
                        {
                            SwitchTwoPokemon(target, possibleSwitcheroonies.Sample(), true);
                        }
                    }
                    success.NewHP = target.HP;
                    success.NewHPPercentage = target.HPPercentage;
                    targetSuccess.Add(success);
                }
            }
            RecordExecutedMove(user, move, failReason, targetSuccess);
        }
    }
}
