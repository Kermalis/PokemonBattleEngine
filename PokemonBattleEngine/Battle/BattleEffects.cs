using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        private bool _calledFromOtherMove = false;

        private void DoSwitchInEffects(IEnumerable<PBEBattlePokemon> battlers, PBEBattlePokemon forcedInBy = null)
        {
            PBEBattlePokemon[] order = GetActingOrder(battlers, true);

            foreach (PBEBattlePokemon pkmn in order)
            {
                bool grounded = pkmn.IsGrounded(forcedInBy) == PBEResult.Success;
                // Verified: (Spikes/StealthRock/ToxicSpikes in the order they were applied) before ability
                if (grounded && pkmn.Team.TeamStatus.HasFlag(PBETeamStatus.Spikes))
                {
                    BroadcastTeamStatus(pkmn.Team, PBETeamStatus.Spikes, PBETeamStatusAction.Damage, damageVictim: pkmn);
                    DealDamage(pkmn, pkmn, (int)(pkmn.MaxHP / (10.0 - (2 * pkmn.Team.SpikeCount))), true, ignoreSturdy: true);
                    if (FaintCheck(pkmn))
                    {
                        continue;
                    }
                    LowHPBerryCheck(pkmn, forcedInBy);
                }
                if (pkmn.Team.TeamStatus.HasFlag(PBETeamStatus.StealthRock))
                {
                    BroadcastTeamStatus(pkmn.Team, PBETeamStatus.StealthRock, PBETeamStatusAction.Damage, damageVictim: pkmn);
                    DealDamage(pkmn, pkmn, (int)(pkmn.MaxHP * PBETypeEffectiveness.GetStealthRockMultiplier(pkmn.Type1, pkmn.Type2)), true, ignoreSturdy: true);
                    if (FaintCheck(pkmn))
                    {
                        continue;
                    }
                    LowHPBerryCheck(pkmn, forcedInBy);
                }
                if (grounded && pkmn.Team.TeamStatus.HasFlag(PBETeamStatus.ToxicSpikes))
                {
                    if (pkmn.HasType(PBEType.Poison))
                    {
                        pkmn.Team.ToxicSpikeCount = 0;
                        BroadcastTeamStatus(pkmn.Team, PBETeamStatus.ToxicSpikes, PBETeamStatusAction.Cleared);
                    }
                    else if (pkmn.IsPoisonPossible(forcedInBy, ignoreSubstitute: true) == PBEResult.Success)
                    {
                        if (pkmn.Team.ToxicSpikeCount == 1)
                        {
                            pkmn.Status1 = PBEStatus1.Poisoned;
                        }
                        else
                        {
                            pkmn.Status1 = PBEStatus1.BadlyPoisoned;
                            pkmn.Status1Counter = 1;
                        }
                        BroadcastStatus1(pkmn, pkmn, pkmn.Status1, PBEStatusAction.Added);
                        // Immunity activates in ActivateAbility() below
                    }
                }

                ActivateAbility(pkmn, true);
            }

            // Verified: Castform/Cherrim transformation goes last. Even if multiple weather abilities activate, they will not change until every ability has been activated
            CastformCherrimCheck(order);
        }
        private void DoPostHitEffects(PBEBattlePokemon user, PBEBattlePokemon victim, PBEMoveData mData, PBEType moveType)
        {
            if (victim.Status2.HasFlag(PBEStatus2.Substitute))
            {
                if (victim.SubstituteHP == 0)
                {
                    BroadcastStatus2(victim, user, PBEStatus2.Substitute, PBEStatusAction.Ended);
                }
            }
            else
            {
                IllusionBreak(victim, user); // Verified: Illusion before Rocky Helmet
                if (victim.HP > 0 && victim.Ability == PBEAbility.Justified && moveType == PBEType.Dark) // Verified: Justified before Rocky Helmet
                {
                    BroadcastAbility(victim, user, PBEAbility.Justified, PBEAbilityAction.Damage);
                    ApplyStatChangeIfPossible(victim, victim, PBEStat.Attack, +1);
                }
                if (victim.HP > 0 && victim.Ability == PBEAbility.Rattled && (moveType == PBEType.Bug || moveType == PBEType.Dark || moveType == PBEType.Ghost)) // Verified: Rattled before Rocky Helmet
                {
                    BroadcastAbility(victim, user, PBEAbility.Rattled, PBEAbilityAction.Damage);
                    ApplyStatChangeIfPossible(victim, victim, PBEStat.Speed, +1);
                }
                if (victim.HP > 0 && victim.Ability == PBEAbility.WeakArmor && mData.Category == PBEMoveCategory.Physical) // Verified: Weak Armor before Rocky Helmet
                {
                    BroadcastAbility(victim, user, PBEAbility.WeakArmor, PBEAbilityAction.Damage);
                    ApplyStatChangeIfPossible(victim, victim, PBEStat.Defense, -1);
                    ApplyStatChangeIfPossible(victim, victim, PBEStat.Speed, +1);
                }

                if (mData.Flags.HasFlag(PBEMoveFlag.MakesContact))
                {
                    if (user.HP > 0 && victim.Ability == PBEAbility.Mummy && user.Ability != PBEAbility.Multitype && user.Ability != PBEAbility.Mummy && user.Ability != PBEAbility.ZenMode)
                    {
                        BroadcastAbility(victim, user, PBEAbility.Mummy, PBEAbilityAction.Damage);
                        SetAbility(victim, user, PBEAbility.Mummy);
                    }
                    if (user.HP > 0 && (victim.Ability == PBEAbility.IronBarbs || victim.Ability == PBEAbility.RoughSkin))
                    {
                        BroadcastAbility(victim, user, victim.Ability, PBEAbilityAction.Damage);
                        DealDamage(victim, user, user.MaxHP / 8, true);
                        if (!FaintCheck(user))
                        {
                            LowHPBerryCheck(user);
                        }
                    }
                    // Verified: Cute Charm can activate when victim is about to faint
                    if (user.HP > 0 && victim.Ability == PBEAbility.CuteCharm && user.IsAttractionPossible(victim) == PBEResult.Success && GetManipulableChance(victim, 30))
                    {
                        BroadcastAbility(victim, user, PBEAbility.CuteCharm, PBEAbilityAction.ChangedStatus);
                        CauseInfatuation(user, victim);
                    }
                    if (user.HP > 0 && victim.Ability == PBEAbility.EffectSpore && user.Status1 == PBEStatus1.None)
                    {
                        // Commented in case the Rainbow affects Effect Spore
                        //int randomNum = PBERandom.RandomInt(0, 99);
                        if (GetManipulableChance(victim, 30))
                        {
                            // Spaghetti code taken from the assembly in generation 5 games
                            PBEStatus1 status = PBEStatus1.None;
                            int randomNum = PBERandom.RandomInt(0, 29);
                            if (randomNum <= 20)
                            {
                                if (randomNum > 10) // 11-20 (10%)
                                {
                                    if (!user.HasType(PBEType.Electric) && user.IsParalysisPossible(victim) == PBEResult.Success)
                                    {
                                        status = PBEStatus1.Paralyzed;
                                    }
                                }
                                else // 0-10 (11%)
                                {
                                    if (user.IsSleepPossible(victim) == PBEResult.Success)
                                    {
                                        status = PBEStatus1.Asleep;
                                    }
                                }
                            }
                            else // 21-29 (9%)
                            {
                                if (user.IsPoisonPossible(victim) == PBEResult.Success)
                                {
                                    status = PBEStatus1.Poisoned;
                                }
                            }
                            if (status != PBEStatus1.None)
                            {
                                BroadcastAbility(victim, user, PBEAbility.EffectSpore, PBEAbilityAction.ChangedStatus);
                                user.Status1 = status;
                                if (status == PBEStatus1.Asleep)
                                {
                                    SetSleepTurns(user, Settings.SleepMinTurns, Settings.SleepMaxTurns);
                                }
                                user.Status1Counter = 0;
                                BroadcastStatus1(user, victim, status, PBEStatusAction.Added);
                                AntiStatusAbilityCheck(user);
                            }
                        }
                    }
                    if (user.HP > 0 && victim.Ability == PBEAbility.FlameBody && user.IsBurnPossible(victim) == PBEResult.Success && GetManipulableChance(victim, 30))
                    {
                        BroadcastAbility(victim, user, PBEAbility.FlameBody, PBEAbilityAction.ChangedStatus);
                        user.Status1 = PBEStatus1.Burned;
                        BroadcastStatus1(user, victim, PBEStatus1.Burned, PBEStatusAction.Added);
                        AntiStatusAbilityCheck(user);
                    }
                    if (user.HP > 0 && victim.Ability == PBEAbility.PoisonPoint && user.IsPoisonPossible(victim) == PBEResult.Success && GetManipulableChance(victim, 30))
                    {
                        BroadcastAbility(victim, user, PBEAbility.PoisonPoint, PBEAbilityAction.ChangedStatus);
                        user.Status1 = PBEStatus1.Poisoned;
                        BroadcastStatus1(user, victim, PBEStatus1.Poisoned, PBEStatusAction.Added);
                        AntiStatusAbilityCheck(user);
                    }
                    if (user.HP > 0 && victim.Ability == PBEAbility.Static && user.IsParalysisPossible(victim) == PBEResult.Success && GetManipulableChance(victim, 30))
                    {
                        BroadcastAbility(victim, user, PBEAbility.Static, PBEAbilityAction.ChangedStatus);
                        user.Status1 = PBEStatus1.Paralyzed;
                        BroadcastStatus1(user, victim, PBEStatus1.Paralyzed, PBEStatusAction.Added);
                        AntiStatusAbilityCheck(user);
                    }
                    // Verified: Above abilities before Rocky Helmet
                    if (user.HP > 0 && victim.Item == PBEItem.RockyHelmet)
                    {
                        BroadcastItem(victim, user, PBEItem.RockyHelmet, PBEItemAction.Damage);
                        DealDamage(victim, user, user.MaxHP / 6, true);
                        if (!FaintCheck(user))
                        {
                            LowHPBerryCheck(user);
                        }
                    }
                }
            }

            if (victim.HP > 0)
            {
                // Verified: Berry after Rough Skin (for victim)
                // Verified: Own Tempo will be ignored if hit, and will not cure until the move is complete
                LowHPBerryCheck(victim, user);
            }

            // TODO: King's Rock, Stench, etc
            // TODO?: Cell Battery
        }
        private void DoPostAttackedTargetEffects(PBEBattlePokemon victim, PBEType colorChangeType = PBEType.None)
        {
            if (victim.HP > 0)
            {
                if (victim.Ability == PBEAbility.ColorChange && colorChangeType != PBEType.None && !victim.HasType(colorChangeType))
                {
                    BroadcastAbility(victim, victim, PBEAbility.ColorChange, PBEAbilityAction.ChangedAppearance);
                    BroadcastTypeChanged(victim, colorChangeType, PBEType.None);
                }
                AntiStatusAbilityCheck(victim); // Heal a status that was given with the user's Mold Breaker
            }
        }
        private void DoPostAttackedUserEffects(PBEBattlePokemon user, bool doLifeOrb, int? recoilDamage = null)
        {
            if (user.HP > 0 && recoilDamage.HasValue)
            {
                BroadcastRecoil(user);
                DealDamage(user, user, recoilDamage.Value, true, ignoreSturdy: true);
                if (!FaintCheck(user))
                {
                    LowHPBerryCheck(user);
                }
            }

            if (user.HP > 0 && doLifeOrb && user.Item == PBEItem.LifeOrb)
            {
                BroadcastItem(user, user, PBEItem.LifeOrb, PBEItemAction.Damage);
                DealDamage(user, user, user.MaxHP / 10, true);
                FaintCheck(user);
            }
        }
        private void DoTurnEndedEffects()
        {
            PBEBattlePokemon[] order = GetActingOrder(ActiveBattlers, true);
            // Verified: Weather stops before doing damage
            if (Weather != PBEWeather.None && WeatherCounter > 0)
            {
                WeatherCounter--;
                if (WeatherCounter == 0)
                {
                    PBEWeather w = Weather;
                    Weather = PBEWeather.None;
                    BroadcastWeather(w, PBEWeatherAction.Ended);
                    CastformCherrimCheck(order);
                }
            }
            // Verified: Hailstorm/Sandstorm/IceBody/RainDish/SolarPower before all
            if (Weather != PBEWeather.None && ShouldDoWeatherEffects())
            {
                foreach (PBEBattlePokemon pkmn in order)
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
                                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.RestoredHP);
                                        HealDamage(pkmn, pkmn.MaxHP / Settings.IceBodyHealDenominator);
                                    }
                                }
                                else if (!pkmn.HasType(PBEType.Ice)
                                    && pkmn.Ability != PBEAbility.Overcoat
                                    && pkmn.Ability != PBEAbility.SnowCloak)
                                {
                                    BroadcastWeather(PBEWeather.Hailstorm, PBEWeatherAction.CausedDamage, pkmn);
                                    DealDamage(pkmn, pkmn, pkmn.MaxHP / Settings.HailDamageDenominator, true);
                                    if (!FaintCheck(pkmn))
                                    {
                                        LowHPBerryCheck(pkmn);
                                    }
                                }
                                break;
                            }
                            case PBEWeather.HarshSunlight:
                            {
                                if (pkmn.Ability == PBEAbility.SolarPower)
                                {
                                    BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.Damage);
                                    DealDamage(pkmn, pkmn, pkmn.MaxHP / 8, true);
                                    if (!FaintCheck(pkmn))
                                    {
                                        LowHPBerryCheck(pkmn);
                                    }
                                }
                                break;
                            }
                            case PBEWeather.Rain:
                            {
                                if (pkmn.Ability == PBEAbility.RainDish && pkmn.HP < pkmn.MaxHP)
                                {
                                    BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.RestoredHP);
                                    HealDamage(pkmn, pkmn.MaxHP / 16);
                                }
                                break;
                            }
                            case PBEWeather.Sandstorm:
                            {
                                if (!pkmn.HasType(PBEType.Rock)
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
                                    DealDamage(pkmn, pkmn, pkmn.MaxHP / Settings.SandstormDamageDenominator, true);
                                    if (!FaintCheck(pkmn))
                                    {
                                        LowHPBerryCheck(pkmn);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }

            // Verified: Healer/ShedSkin/BlackSludge/Leftovers before LeechSeed
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP > 0)
                {
                    switch (pkmn.Ability)
                    {
                        case PBEAbility.Healer:
                        {
                            foreach (PBEBattlePokemon ally in GetRuntimeSurrounding(pkmn, true, false))
                            {
                                // TODO: #265
                                if (ally.Status1 != PBEStatus1.None && GetManipulableChance(pkmn, 30))
                                {
                                    BroadcastAbility(pkmn, ally, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                                    PBEStatus1 status1 = ally.Status1;
                                    ally.Status1 = PBEStatus1.None;
                                    BroadcastStatus1(ally, pkmn, status1, PBEStatusAction.Cleared);
                                    if (status1 == PBEStatus1.Asleep)
                                    {
                                        CureNightmare(ally, pkmn);
                                    }
                                }
                            }
                            break;
                        }
                        case PBEAbility.ShedSkin:
                        {
                            if (pkmn.Status1 != PBEStatus1.None && GetManipulableChance(pkmn, 30))
                            {
                                BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                                PBEStatus1 status1 = pkmn.Status1;
                                pkmn.Status1 = PBEStatus1.None;
                                BroadcastStatus1(pkmn, pkmn, status1, PBEStatusAction.Cleared);
                                if (status1 == PBEStatus1.Asleep)
                                {
                                    CureNightmare(pkmn, pkmn);
                                }
                            }
                            break;
                        }
                    }
                    switch (pkmn.Item)
                    {
                        case PBEItem.BlackSludge:
                        {
                            if (pkmn.HasType(PBEType.Poison))
                            {
                                if (pkmn.HP < pkmn.MaxHP)
                                {
                                    BroadcastItem(pkmn, pkmn, pkmn.Item, PBEItemAction.RestoredHP);
                                    HealDamage(pkmn, pkmn.MaxHP / Settings.BlackSludgeHealDenominator);
                                }
                            }
                            else
                            {
                                BroadcastItem(pkmn, pkmn, pkmn.Item, PBEItemAction.Damage);
                                DealDamage(pkmn, pkmn, pkmn.MaxHP / Settings.BlackSludgeDamageDenominator, true);
                                FaintCheck(pkmn); // No need to call HealingBerryCheck() because if you are holding BlackSludge you are not holding a healing berry
                            }
                            break;
                        }
                        case PBEItem.Leftovers:
                        {
                            if (pkmn.HP < pkmn.MaxHP)
                            {
                                BroadcastItem(pkmn, pkmn, pkmn.Item, PBEItemAction.RestoredHP);
                                HealDamage(pkmn, pkmn.MaxHP / Settings.LeftoversHealDenominator);
                            }
                            break;
                        }
                    }
                }
            }

            // Verified: LeechSeed before Status1
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP > 0 && pkmn.Status2.HasFlag(PBEStatus2.LeechSeed))
                {
                    PBEBattlePokemon sucker = pkmn.SeededTeam.TryGetPokemon(pkmn.SeededPosition);
                    if (sucker != null)
                    {
                        BroadcastStatus2(pkmn, sucker, PBEStatus2.LeechSeed, PBEStatusAction.Damage);
                        int amtDealt = DealDamage(sucker, pkmn, pkmn.MaxHP / Settings.LeechSeedDenominator, true);
                        HealDamage(sucker, amtDealt);
                        if (!FaintCheck(pkmn))
                        {
                            LowHPBerryCheck(pkmn);
                        }
                    }
                }
            }

            // Verified: Status1 before Curse
            foreach (PBEBattlePokemon pkmn in order)
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
                            DealDamage(pkmn, pkmn, damage, true);
                            if (!FaintCheck(pkmn))
                            {
                                LowHPBerryCheck(pkmn);
                            }
                            break;
                        }
                        case PBEStatus1.Poisoned:
                        {
                            BroadcastStatus1(pkmn, pkmn, PBEStatus1.Poisoned, PBEStatusAction.Damage);
                            DealDamage(pkmn, pkmn, pkmn.MaxHP / Settings.PoisonDamageDenominator, true);
                            if (!FaintCheck(pkmn))
                            {
                                LowHPBerryCheck(pkmn);
                            }
                            break;
                        }
                        case PBEStatus1.BadlyPoisoned:
                        {
                            BroadcastStatus1(pkmn, pkmn, PBEStatus1.BadlyPoisoned, PBEStatusAction.Damage);
                            DealDamage(pkmn, pkmn, pkmn.MaxHP * pkmn.Status1Counter / Settings.ToxicDamageDenominator, true);
                            if (!FaintCheck(pkmn))
                            {
                                pkmn.Status1Counter++;
                                LowHPBerryCheck(pkmn);
                            }
                            break;
                        }
                    }
                }
            }

            // Verified: Nightmare before Curse, not same loop
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP > 0 && pkmn.Status2.HasFlag(PBEStatus2.Nightmare))
                {
                    BroadcastStatus2(pkmn, pkmn, PBEStatus2.Nightmare, PBEStatusAction.Damage);
                    DealDamage(pkmn, pkmn, pkmn.MaxHP / 4, true);
                    if (!FaintCheck(pkmn))
                    {
                        LowHPBerryCheck(pkmn);
                    }
                }
            }

            // Verified: Curse before MagnetRise
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP > 0 && pkmn.Status2.HasFlag(PBEStatus2.Cursed))
                {
                    BroadcastStatus2(pkmn, pkmn, PBEStatus2.Cursed, PBEStatusAction.Damage);
                    DealDamage(pkmn, pkmn, pkmn.MaxHP / Settings.CurseDenominator, true);
                    if (!FaintCheck(pkmn))
                    {
                        LowHPBerryCheck(pkmn);
                    }
                }
            }

            // Verified: MagnetRise before Abilities/Orbs
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP > 0 && pkmn.Status2.HasFlag(PBEStatus2.MagnetRise) && pkmn.MagnetRiseTurns > 0)
                {
                    pkmn.MagnetRiseTurns--;
                    if (pkmn.MagnetRiseTurns == 0)
                    {
                        BroadcastStatus2(pkmn, pkmn, PBEStatus2.MagnetRise, PBEStatusAction.Ended);
                    }
                }
            }

            // Verified: BadDreams/Moody/SlowStart/SpeedBoost before Orbs, but activate together
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP > 0)
                {
                    // Ability before Orb
                    switch (pkmn.Ability)
                    {
                        case PBEAbility.BadDreams:
                        {
                            foreach (PBEBattlePokemon victim in GetRuntimeSurrounding(pkmn, false, true).Where(p => p.Status1 == PBEStatus1.Asleep))
                            {
                                BroadcastAbility(pkmn, victim, PBEAbility.BadDreams, PBEAbilityAction.Damage);
                                DealDamage(pkmn, victim, pkmn.MaxHP / 8, true);
                                if (!FaintCheck(victim))
                                {
                                    LowHPBerryCheck(victim);
                                }
                            }
                            break;
                        }
                        case PBEAbility.Moody:
                        {
                            PBEStat[] statsThatCanGoUp = PBEDataUtils.MoodyStats.Where(s => pkmn.GetStatChange(s) < Settings.MaxStatChange).ToArray();
                            PBEStat? upStat = statsThatCanGoUp.Length == 0 ? (PBEStat?)null : statsThatCanGoUp.RandomElement();
                            var statsThatCanGoDown = PBEDataUtils.MoodyStats.Where(s => pkmn.GetStatChange(s) > -Settings.MaxStatChange).ToList();
                            if (upStat.HasValue)
                            {
                                statsThatCanGoDown.Remove(upStat.Value);
                            }
                            PBEStat? downStat = statsThatCanGoDown.Count == 0 ? (PBEStat?)null : statsThatCanGoDown.RandomElement();
                            if (upStat.HasValue || downStat.HasValue)
                            {
                                BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.Stats);
                                if (upStat.HasValue)
                                {
                                    ApplyStatChangeIfPossible(pkmn, pkmn, upStat.Value, +2);
                                }
                                if (downStat.HasValue)
                                {
                                    ApplyStatChangeIfPossible(pkmn, pkmn, downStat.Value, -1);
                                }
                            }
                            break;
                        }
                        case PBEAbility.SlowStart:
                        {
                            if (pkmn.SlowStart_HinderTurnsLeft > 0)
                            {
                                pkmn.SlowStart_HinderTurnsLeft--;
                                if (pkmn.SlowStart_HinderTurnsLeft == 0)
                                {
                                    BroadcastAbility(pkmn, pkmn, PBEAbility.SlowStart, PBEAbilityAction.SlowStart_Ended);
                                }
                            }
                            break;
                        }
                        case PBEAbility.SpeedBoost:
                        {
                            if (pkmn.SpeedBoost_AbleToSpeedBoostThisTurn && pkmn.SpeedChange < Settings.MaxStatChange)
                            {
                                BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.Stats);
                                ApplyStatChangeIfPossible(pkmn, pkmn, PBEStat.Speed, +1);
                            }
                            break;
                        }
                    }
                    // Orb
                    switch (pkmn.Item)
                    {
                        case PBEItem.FlameOrb:
                        {
                            if (pkmn.IsBurnPossible(null, ignoreSubstitute: true, ignoreSafeguard: true) == PBEResult.Success)
                            {
                                pkmn.Status1 = PBEStatus1.Burned;
                                BroadcastItem(pkmn, pkmn, pkmn.Item, PBEItemAction.ChangedStatus);
                                BroadcastStatus1(pkmn, pkmn, PBEStatus1.Burned, PBEStatusAction.Added);
                            }
                            break;
                        }
                        case PBEItem.ToxicOrb:
                        {
                            if (pkmn.IsPoisonPossible(null, ignoreSubstitute: true, ignoreSafeguard: true) == PBEResult.Success)
                            {
                                pkmn.Status1 = PBEStatus1.BadlyPoisoned;
                                pkmn.Status1Counter = 1;
                                BroadcastItem(pkmn, pkmn, pkmn.Item, PBEItemAction.ChangedStatus);
                                BroadcastStatus1(pkmn, pkmn, PBEStatus1.BadlyPoisoned, PBEStatusAction.Added);
                            }
                            break;
                        }
                    }
                }
            }
        }

        public bool ShouldDoWeatherEffects()
        {
            // If HP is needed to be above 0, use HPPercentage so clients can continue to use this
            // However, I see no instance of this getting called where an ActiveBattler has 0 hp
            return !ActiveBattlers.Any(p => p.Ability == PBEAbility.AirLock || p.Ability == PBEAbility.CloudNine);
        }
        public bool WillLeafGuardActivate()
        {
            return ShouldDoWeatherEffects() && Weather == PBEWeather.HarshSunlight;
        }

        private void UseMove(PBEBattlePokemon user, PBEMove move, PBETurnTarget requestedTargets)
        {
            // Cancel the semi-invulnerable move if the user is affected by its status1
            if (!_calledFromOtherMove && PreMoveStatusCheck(user, move))
            {
                if (user.Status2.HasFlag(PBEStatus2.Airborne))
                {
                    BroadcastMoveLock_Temporary(user, PBEMove.None, PBETurnTarget.None);
                    BroadcastStatus2(user, user, PBEStatus2.Airborne, PBEStatusAction.Ended);
                }
                if (user.Status2.HasFlag(PBEStatus2.ShadowForce))
                {
                    BroadcastMoveLock_Temporary(user, PBEMove.None, PBETurnTarget.None);
                    BroadcastStatus2(user, user, PBEStatus2.ShadowForce, PBEStatusAction.Ended);
                }
                if (user.Status2.HasFlag(PBEStatus2.Underground))
                {
                    BroadcastMoveLock_Temporary(user, PBEMove.None, PBETurnTarget.None);
                    BroadcastStatus2(user, user, PBEStatus2.Underground, PBEStatusAction.Ended);
                }
                if (user.Status2.HasFlag(PBEStatus2.Underwater))
                {
                    BroadcastMoveLock_Temporary(user, PBEMove.None, PBETurnTarget.None);
                    BroadcastStatus2(user, user, PBEStatus2.Underwater, PBEStatusAction.Ended);
                }
                return;
            }
            PBEMoveData mData = PBEMoveData.Data[move];
            PBEBattlePokemon[] targets = GetRuntimeTargets(user, requestedTargets, user.GetMoveTargets(mData) == PBEMoveTarget.SingleNotSelf);
            switch (mData.Effect)
            {
                case PBEMoveEffect.Acrobatics:
                case PBEMoveEffect.Brine:
                case PBEMoveEffect.ChipAway:
                case PBEMoveEffect.CrushGrip:
                case PBEMoveEffect.Eruption:
                case PBEMoveEffect.Facade:
                case PBEMoveEffect.Flail:
                case PBEMoveEffect.FoulPlay:
                case PBEMoveEffect.Frustration:
                case PBEMoveEffect.GrassKnot:
                case PBEMoveEffect.HeatCrash:
                case PBEMoveEffect.Hex:
                case PBEMoveEffect.HiddenPower:
                case PBEMoveEffect.Hit:
                case PBEMoveEffect.Judgment:
                case PBEMoveEffect.Magnitude:
                case PBEMoveEffect.Psyshock:
                case PBEMoveEffect.Punishment:
                case PBEMoveEffect.Retaliate:
                case PBEMoveEffect.Return:
                case PBEMoveEffect.StoredPower:
                case PBEMoveEffect.TechnoBlast:
                case PBEMoveEffect.Venoshock:
                case PBEMoveEffect.WeatherBall: Ef_Hit(user, targets, move, mData); break;
                case PBEMoveEffect.Attract: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.Infatuated); break;
                case PBEMoveEffect.BellyDrum: Ef_BellyDrum(user, targets, move, mData); break;
                case PBEMoveEffect.Bounce: Ef_Bounce(user, targets, move, mData, requestedTargets); break;
                case PBEMoveEffect.BrickBreak: Ef_BrickBreak(user, targets, move, mData); break;
                case PBEMoveEffect.Burn: Ef_TryForceStatus1(user, targets, move, mData, PBEStatus1.Burned); break;
                case PBEMoveEffect.Camouflage: Ef_Camouflage(user, targets, move, mData); break;
                case PBEMoveEffect.ChangeTarget_ACC: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Accuracy, mData.EffectParam) }); break;
                case PBEMoveEffect.ChangeTarget_ATK: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Attack, mData.EffectParam) }); break;
                case PBEMoveEffect.ChangeTarget_DEF: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Defense, mData.EffectParam) }); break;
                case PBEMoveEffect.ChangeTarget_EVA:
                case PBEMoveEffect.Minimize: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Evasion, mData.EffectParam) }); break;
                case PBEMoveEffect.ChangeTarget_SPATK: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.SpAttack, mData.EffectParam) }); break;
                case PBEMoveEffect.ChangeTarget_SPATK__IfAttractionPossible: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.SpAttack, mData.EffectParam) }, requireAttraction: true); break;
                case PBEMoveEffect.ChangeTarget_SPDEF: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.SpDefense, mData.EffectParam) }); break;
                case PBEMoveEffect.ChangeTarget_SPE: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Speed, mData.EffectParam) }); break;
                case PBEMoveEffect.Confuse: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.Confused); break;
                case PBEMoveEffect.Conversion: Ef_Conversion(user, targets, move, mData); break;
                case PBEMoveEffect.Curse: Ef_Curse(user, targets, move, mData); break;
                case PBEMoveEffect.Dig: SemiInvulnerableChargeMove(user, targets, move, mData, requestedTargets, PBEStatus2.Underground); break;
                case PBEMoveEffect.Dive: SemiInvulnerableChargeMove(user, targets, move, mData, requestedTargets, PBEStatus2.Underwater); break;
                case PBEMoveEffect.Endeavor: Ef_Endeavor(user, targets, move, mData); break;
                case PBEMoveEffect.Entrainment: Ef_Entrainment(user, targets, move, mData); break;
                case PBEMoveEffect.Feint: Ef_Feint(user, targets, move, mData); break;
                case PBEMoveEffect.FinalGambit: Ef_FinalGambit(user, targets, move, mData); break;
                case PBEMoveEffect.Flatter: Ef_Flatter(user, targets, move, mData); break;
                case PBEMoveEffect.Fly: SemiInvulnerableChargeMove(user, targets, move, mData, requestedTargets, PBEStatus2.Airborne); break;
                case PBEMoveEffect.FocusEnergy: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.Pumped); break;
                case PBEMoveEffect.Foresight: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.Identified); break;
                case PBEMoveEffect.GastroAcid: Ef_SetOtherAbility(user, targets, move, mData, PBEAbility.None, false); break;
                case PBEMoveEffect.Growth: Ef_Growth(user, targets, move, mData); break;
                case PBEMoveEffect.Hail: Ef_TryForceWeather(user, move, mData, PBEWeather.Hailstorm); break;
                case PBEMoveEffect.Haze: Ef_Haze(user, targets, move, mData); break;
                case PBEMoveEffect.HelpingHand: Ef_HelpingHand(user, targets, move, mData); break;
                case PBEMoveEffect.Hit__2Times: Ef_MultiHit(user, targets, move, mData, 2); break;
                case PBEMoveEffect.Hit__2Times__MaybePoison: Ef_MultiHit(user, targets, move, mData, 2, status1: PBEStatus1.Poisoned, chanceToInflictStatus1: mData.EffectParam); break;
                case PBEMoveEffect.Hit__2To5Times: Ef_MultiHit_2To5(user, targets, move, mData); break;
                case PBEMoveEffect.Hit__MaybeBurn: Ef_Hit(user, targets, move, mData, status1: PBEStatus1.Burned, chanceToInflictStatus1: mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeBurn__10PercentFlinch: Ef_Hit(user, targets, move, mData, status1: PBEStatus1.Burned, chanceToInflictStatus1: mData.EffectParam, status2: PBEStatus2.Flinching, chanceToInflictStatus2: 10); break;
                case PBEMoveEffect.Hit__MaybeBurnFreezeParalyze: Ef_Hit__MaybeBurnFreezeParalyze(user, targets, move, mData); break;
                case PBEMoveEffect.Hit__MaybeConfuse: Ef_Hit(user, targets, move, mData, status2: PBEStatus2.Confused, chanceToInflictStatus2: mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeFlinch: Ef_Hit(user, targets, move, mData, status2: PBEStatus2.Flinching, chanceToInflictStatus2: mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeFreeze: Ef_Hit(user, targets, move, mData, status1: PBEStatus1.Frozen, chanceToInflictStatus1: mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeFreeze__10PercentFlinch: Ef_Hit(user, targets, move, mData, status1: PBEStatus1.Frozen, chanceToInflictStatus1: mData.EffectParam, status2: PBEStatus2.Flinching, chanceToInflictStatus2: 10); break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1: Ef_Hit__MaybeChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Accuracy, -1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_ATK_By1: Ef_Hit__MaybeChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Attack, -1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1: Ef_Hit__MaybeChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Defense, -1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1: Ef_Hit__MaybeChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.SpAttack, -1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1: Ef_Hit__MaybeChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.SpDefense, -1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2: Ef_Hit__MaybeChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.SpDefense, -2) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1: Ef_Hit__MaybeChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Speed, -1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeParalyze: Ef_Hit(user, targets, move, mData, status1: PBEStatus1.Paralyzed, chanceToInflictStatus1: mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeParalyze__10PercentFlinch: Ef_Hit(user, targets, move, mData, status1: PBEStatus1.Paralyzed, chanceToInflictStatus1: mData.EffectParam, status2: PBEStatus2.Flinching, chanceToInflictStatus2: 10); break;
                case PBEMoveEffect.Hit__MaybePoison: Ef_Hit(user, targets, move, mData, status1: PBEStatus1.Poisoned, chanceToInflictStatus1: mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeLowerUser_ATK_DEF_By1: Ef_Hit__MaybeChangeUserStats(user, targets, move, mData, new[] { (PBEStat.Attack, -1), (PBEStat.Defense, -1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1: Ef_Hit__MaybeChangeUserStats(user, targets, move, mData, new[] { (PBEStat.Defense, -1), (PBEStat.SpDefense, -1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2: Ef_Hit__MaybeChangeUserStats(user, targets, move, mData, new[] { (PBEStat.SpAttack, -2) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeLowerUser_SPE_By1: Ef_Hit__MaybeChangeUserStats(user, targets, move, mData, new[] { (PBEStat.Speed, -1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1: Ef_Hit__MaybeChangeUserStats(user, targets, move, mData, new[] { (PBEStat.Speed, -1), (PBEStat.Defense, -1), (PBEStat.SpDefense, -1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeRaiseUser_ATK_By1: Ef_Hit__MaybeChangeUserStats(user, targets, move, mData, new[] { (PBEStat.Attack, +1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1: Ef_Hit__MaybeChangeUserStats(user, targets, move, mData, new[] { (PBEStat.Attack, +1), (PBEStat.Defense, +1), (PBEStat.SpAttack, +1), (PBEStat.SpDefense, +1), (PBEStat.Speed, +1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeRaiseUser_DEF_By1: Ef_Hit__MaybeChangeUserStats(user, targets, move, mData, new[] { (PBEStat.Defense, +1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeRaiseUser_SPATK_By1: Ef_Hit__MaybeChangeUserStats(user, targets, move, mData, new[] { (PBEStat.SpAttack, +1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeRaiseUser_SPE_By1: Ef_Hit__MaybeChangeUserStats(user, targets, move, mData, new[] { (PBEStat.Speed, +1) }, mData.EffectParam); break;
                case PBEMoveEffect.Hit__MaybeToxic: Ef_Hit(user, targets, move, mData, status1: PBEStatus1.BadlyPoisoned, chanceToInflictStatus1: mData.EffectParam); break;
                case PBEMoveEffect.HPDrain: Ef_HPDrain(user, targets, move, mData); break;
                case PBEMoveEffect.HPDrain__RequireSleep: Ef_HPDrain(user, targets, move, mData, requireSleep: true); break;
                case PBEMoveEffect.LeechSeed: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.LeechSeed); break;
                case PBEMoveEffect.LightScreen: Ef_TryForceTeamStatus(user, move, mData, PBETeamStatus.LightScreen); break;
                case PBEMoveEffect.LockOn: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.LockOn); break;
                case PBEMoveEffect.LowerTarget_ATK_DEF_By1: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Attack, -1), (PBEStat.Defense, -1) }); break;
                case PBEMoveEffect.LowerTarget_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Defense, -1), (PBEStat.SpDefense, -1), (PBEStat.Attack, +2), (PBEStat.SpAttack, +2), (PBEStat.Speed, +2) }); break;
                case PBEMoveEffect.LuckyChant: Ef_TryForceTeamStatus(user, move, mData, PBETeamStatus.LuckyChant); break;
                case PBEMoveEffect.MagnetRise: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.MagnetRise); break;
                case PBEMoveEffect.Metronome: Ef_Metronome(user, move, mData); break;
                case PBEMoveEffect.MiracleEye: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.MiracleEye); break;
                case PBEMoveEffect.Moonlight: Ef_Moonlight(user, targets, move, mData); break;
                case PBEMoveEffect.Nightmare: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.Nightmare); break;
                case PBEMoveEffect.Nothing: Ef_Nothing(user, move, mData); break;
                case PBEMoveEffect.OneHitKnockout: Ef_OneHitKnockout(user, targets, move, mData); break;
                case PBEMoveEffect.PainSplit: Ef_PainSplit(user, targets, move, mData); break;
                case PBEMoveEffect.Paralyze: Ef_TryForceStatus1(user, targets, move, mData, PBEStatus1.Paralyzed); break;
                case PBEMoveEffect.PayDay: Ef_PayDay(user, targets, move, mData); break;
                case PBEMoveEffect.Poison: Ef_TryForceStatus1(user, targets, move, mData, PBEStatus1.Poisoned); break;
                case PBEMoveEffect.PowerTrick: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.PowerTrick); break;
                case PBEMoveEffect.Protect: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.Protected); break;
                case PBEMoveEffect.PsychUp: Ef_PsychUp(user, targets, move, mData); break;
                case PBEMoveEffect.Psywave: Ef_Psywave(user, targets, move, mData); break;
                case PBEMoveEffect.QuickGuard: Ef_TryForceTeamStatus(user, move, mData, PBETeamStatus.QuickGuard); break;
                case PBEMoveEffect.RainDance: Ef_TryForceWeather(user, move, mData, PBEWeather.Rain); break;
                case PBEMoveEffect.RaiseTarget_ATK_ACC_By1: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Attack, +1), (PBEStat.Accuracy, +1) }); break;
                case PBEMoveEffect.RaiseTarget_ATK_DEF_By1: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Attack, +1), (PBEStat.Defense, +1) }); break;
                case PBEMoveEffect.RaiseTarget_ATK_DEF_ACC_By1: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Attack, +1), (PBEStat.Defense, +1), (PBEStat.Accuracy, +1) }); break;
                case PBEMoveEffect.RaiseTarget_ATK_SPATK_By1: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Attack, +1), (PBEStat.SpAttack, +1) }); break;
                case PBEMoveEffect.RaiseTarget_ATK_SPE_By1: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Attack, +1), (PBEStat.Speed, +1) }); break;
                case PBEMoveEffect.RaiseTarget_DEF_SPDEF_By1: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Defense, +1), (PBEStat.SpDefense, +1) }); break;
                case PBEMoveEffect.RaiseTarget_SPATK_SPDEF_By1: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.SpAttack, +1), (PBEStat.SpDefense, +1) }); break;
                case PBEMoveEffect.RaiseTarget_SPATK_SPDEF_SPE_By1: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.SpAttack, +1), (PBEStat.SpDefense, +1), (PBEStat.Speed, +1) }); break;
                case PBEMoveEffect.RaiseTarget_SPE_By2_ATK_By1: Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Speed, +2), (PBEStat.Attack, +1) }); break;
                case PBEMoveEffect.Recoil: Ef_Recoil(user, targets, move, mData); break;
                case PBEMoveEffect.Recoil__10PercentBurn: Ef_Recoil(user, targets, move, mData, status1: PBEStatus1.Burned, chanceToInflictStatus1: 10); break;
                case PBEMoveEffect.Recoil__10PercentParalyze: Ef_Recoil(user, targets, move, mData, status1: PBEStatus1.Paralyzed, chanceToInflictStatus1: 10); break;
                case PBEMoveEffect.Reflect: Ef_TryForceTeamStatus(user, move, mData, PBETeamStatus.Reflect); break;
                case PBEMoveEffect.ReflectType: Ef_ReflectType(user, targets, move, mData); break;
                case PBEMoveEffect.Refresh: Ef_Refresh(user, targets, move, mData); break;
                case PBEMoveEffect.Rest: Ef_Rest(user, move, mData); break;
                case PBEMoveEffect.RestoreTargetHP: Ef_RestoreTargetHP(user, targets, move, mData); break;
                case PBEMoveEffect.RolePlay: Ef_RolePlay(user, targets, move, mData); break;
                case PBEMoveEffect.Roost: Ef_Roost(user, targets, move, mData); break;
                case PBEMoveEffect.Safeguard: Ef_TryForceTeamStatus(user, move, mData, PBETeamStatus.Safeguard); break;
                case PBEMoveEffect.Sandstorm: Ef_TryForceWeather(user, move, mData, PBEWeather.Sandstorm); break;
                case PBEMoveEffect.SecretPower: Ef_SecretPower(user, targets, move, mData); break;
                case PBEMoveEffect.SeismicToss: Ef_SeismicToss(user, targets, move, mData); break;
                case PBEMoveEffect.Selfdestruct: Ef_Selfdestruct(user, targets, move, mData); break;
                case PBEMoveEffect.SetDamage: Ef_SetDamage(user, targets, move, mData); break;
                case PBEMoveEffect.ShadowForce: Ef_ShadowForce(user, targets, move, mData, requestedTargets); break;
                case PBEMoveEffect.SimpleBeam: Ef_SetOtherAbility(user, targets, move, mData, PBEAbility.Simple, true); break;
                case PBEMoveEffect.Sleep: Ef_TryForceStatus1(user, targets, move, mData, PBEStatus1.Asleep); break;
                case PBEMoveEffect.Snore: Ef_Snore(user, targets, move, mData); break;
                case PBEMoveEffect.Soak: Ef_Soak(user, targets, move, mData); break;
                case PBEMoveEffect.Spikes: Ef_TryForceTeamStatus(user, move, mData, PBETeamStatus.Spikes); break;
                case PBEMoveEffect.StealthRock: Ef_TryForceTeamStatus(user, move, mData, PBETeamStatus.StealthRock); break;
                case PBEMoveEffect.Struggle: Ef_Struggle(user, targets, move, mData); break;
                case PBEMoveEffect.Substitute: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.Substitute); break;
                case PBEMoveEffect.SuckerPunch: Ef_SuckerPunch(user, targets, move, mData); break;
                case PBEMoveEffect.SunnyDay: Ef_TryForceWeather(user, move, mData, PBEWeather.HarshSunlight); break;
                case PBEMoveEffect.SuperFang: Ef_SuperFang(user, targets, move, mData); break;
                case PBEMoveEffect.Swagger: Ef_Swagger(user, targets, move, mData); break;
                case PBEMoveEffect.Tailwind: Ef_TryForceTeamStatus(user, move, mData, PBETeamStatus.Tailwind); break;
                case PBEMoveEffect.Teleport: Ef_Teleport(user, move, mData); break;
                case PBEMoveEffect.ThunderWave: Ef_TryForceStatus1(user, targets, move, mData, PBEStatus1.Paralyzed, thunderWave: true); break;
                case PBEMoveEffect.Toxic: Ef_TryForceStatus1(user, targets, move, mData, PBEStatus1.BadlyPoisoned); break;
                case PBEMoveEffect.ToxicSpikes: Ef_TryForceTeamStatus(user, move, mData, PBETeamStatus.ToxicSpikes); break;
                case PBEMoveEffect.Transform: Ef_TryForceStatus2(user, targets, move, mData, PBEStatus2.Transformed); break;
                case PBEMoveEffect.TrickRoom: Ef_TryForceBattleStatus(user, move, mData, PBEBattleStatus.TrickRoom); break;
                case PBEMoveEffect.Whirlwind: Ef_Whirlwind(user, targets, move, mData); break;
                case PBEMoveEffect.WideGuard: Ef_TryForceTeamStatus(user, move, mData, PBETeamStatus.WideGuard); break;
                case PBEMoveEffect.WorrySeed: Ef_SetOtherAbility(user, targets, move, mData, PBEAbility.Insomnia, true); break;
                default: throw new ArgumentOutOfRangeException(nameof(mData.Effect));
            }
        }

        private bool PreMoveStatusCheck(PBEBattlePokemon user, PBEMove move)
        {
            PBEMoveData mData = PBEMoveData.Data[move];

            // Verified: Sleep and Freeze don't interact with Flinch unless they come out of the status
            // Sleep causes Confusion, Flinch, and Infatuation to activate if the user is trying to use Snore
            if (user.Status1 == PBEStatus1.Asleep)
            {
                user.Status1Counter++;
                if (user.Status1Counter > user.SleepTurns)
                {
                    user.Status1 = PBEStatus1.None;
                    user.Status1Counter = 0;
                    user.SleepTurns = 0;
                    BroadcastStatus1(user, user, PBEStatus1.Asleep, PBEStatusAction.Ended);
                    CureNightmare(user, user);
                }
                else if (mData.Effect != PBEMoveEffect.Snore)
                {
                    BroadcastStatus1(user, user, PBEStatus1.Asleep, PBEStatusAction.CausedImmobility);
                    return true;
                }
            }
            else if (user.Status1 == PBEStatus1.Frozen)
            {
                if (mData.Flags.HasFlag(PBEMoveFlag.DefrostsUser) || PBERandom.RandomBool(20, 100))
                {
                    user.Status1 = PBEStatus1.None;
                    BroadcastStatus1(user, user, PBEStatus1.Frozen, PBEStatusAction.Ended);
                }
                else
                {
                    BroadcastStatus1(user, user, PBEStatus1.Frozen, PBEStatusAction.CausedImmobility);
                    return true;
                }
            }
            // Verified: Flinch before Confusion, Infatuation, and Paralysis can do anything
            if (user.Status2.HasFlag(PBEStatus2.Flinching))
            {
                BroadcastStatus2(user, user, PBEStatus2.Flinching, PBEStatusAction.CausedImmobility);
                if (user.Ability == PBEAbility.Steadfast && user.SpeedChange < Settings.MaxStatChange)
                {
                    BroadcastAbility(user, user, PBEAbility.Steadfast, PBEAbilityAction.Stats);
                    ApplyStatChangeIfPossible(user, user, PBEStat.Speed, +1);
                }
                return true;
            }
            // Verified: Confusion before Infatuation and Paralysis
            if (user.Status2.HasFlag(PBEStatus2.Confused))
            {
                user.ConfusionCounter++;
                if (user.ConfusionCounter > user.ConfusionTurns)
                {
                    user.ConfusionCounter = 0;
                    user.ConfusionTurns = 0;
                    BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Ended);
                }
                else
                {
                    BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Announced);
                    if (PBERandom.RandomBool(50, 100))
                    {
                        BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Damage);
                        ushort damage = CalculateConfusionDamage(user);
                        DealDamage(user, user, damage, true);
                        if (!FaintCheck(user))
                        {
                            // BUG: In generation 5+, confusion damage does not activate these items
#if BUGFIX
                            LowHPBerryCheck(user);
#endif
                        }
                        return true;
                    }
                }
            }
            // Verified: Paralysis before Infatuation
            if (user.Status1 == PBEStatus1.Paralyzed && PBERandom.RandomBool(25, 100))
            {
                BroadcastStatus1(user, user, PBEStatus1.Paralyzed, PBEStatusAction.CausedImmobility);
                return true;
            }
            // Infatuation
            if (user.Status2.HasFlag(PBEStatus2.Infatuated))
            {
                BroadcastStatus2(user, user.InfatuatedWithPokemon, PBEStatus2.Infatuated, PBEStatusAction.Announced);
                if (PBERandom.RandomBool(50, 100))
                {
                    BroadcastStatus2(user, user.InfatuatedWithPokemon, PBEStatus2.Infatuated, PBEStatusAction.CausedImmobility);
                    return true;
                }
            }
            return false;
        }
        private bool MissCheck(PBEBattlePokemon user, PBEBattlePokemon target, PBEMoveData mData)
        {
            if (user == target)
            {
                return false;
            }
            if (target.Status2.HasFlag(PBEStatus2.Protected) && mData.Flags.HasFlag(PBEMoveFlag.AffectedByProtect))
            {
                BroadcastStatus2(target, user, PBEStatus2.Protected, PBEStatusAction.Damage);
                return true;
            }
            if (target.Team.TeamStatus.HasFlag(PBETeamStatus.WideGuard) && mData.Category != PBEMoveCategory.Status && PBEMoveData.IsSpreadMove(user.GetMoveTargets(mData)))
            {
                BroadcastTeamStatus(target.Team, PBETeamStatus.WideGuard, PBETeamStatusAction.Damage, damageVictim: target);
                return true;
            }
            // Feint ignores Quick Guard unless the target is an ally
            if (target.Team.TeamStatus.HasFlag(PBETeamStatus.QuickGuard) && mData.Priority > 0 && (mData.Effect != PBEMoveEffect.Feint || user.Team == target.Team))
            {
                BroadcastTeamStatus(target.Team, PBETeamStatus.QuickGuard, PBETeamStatusAction.Damage, damageVictim: target);
                return true;
            }
            if (user.Status2.HasFlag(PBEStatus2.LockOn) && user.LockOnPokemon == target)
            {
                return false;
            }
            if (user.Ability == PBEAbility.NoGuard || target.Ability == PBEAbility.NoGuard)
            {
                return false;
            }
            if (target.Status2.HasFlag(PBEStatus2.Airborne) && !(mData.Flags.HasFlag(PBEMoveFlag.DoubleDamageAirborne) || mData.Flags.HasFlag(PBEMoveFlag.HitsAirborne)))
            {
                goto miss;
            }
            if (target.Status2.HasFlag(PBEStatus2.ShadowForce))
            {
                goto miss;
            }
            if (target.Status2.HasFlag(PBEStatus2.Underground) && !(mData.Flags.HasFlag(PBEMoveFlag.DoubleDamageUnderground) || mData.Flags.HasFlag(PBEMoveFlag.HitsUnderground)))
            {
                goto miss;
            }
            if (target.Status2.HasFlag(PBEStatus2.Underwater) && !(mData.Flags.HasFlag(PBEMoveFlag.DoubleDamageUnderwater) || mData.Flags.HasFlag(PBEMoveFlag.HitsUnderwater)))
            {
                goto miss;
            }
            // These go after semi-invulnerable
            double chance = mData.Accuracy;
            if (chance == 0) // Moves that don't miss
            {
                return false;
            }
            if (ShouldDoWeatherEffects())
            {
                if (Weather == PBEWeather.Hailstorm && mData.Flags.HasFlag(PBEMoveFlag.NeverMissHail))
                {
                    return false;
                }
                if (mData.Flags.HasFlag(PBEMoveFlag.NeverMissRain))
                {
                    if (Weather == PBEWeather.Rain)
                    {
                        return false;
                    }
                    if (Weather == PBEWeather.HarshSunlight)
                    {
                        chance = Math.Min(50, chance);
                    }
                }
            }
            if (mData.Effect == PBEMoveEffect.OneHitKnockout)
            {
                chance = user.Level - target.Level + chance;
                if (chance < 1)
                {
                    goto miss;
                }
                else
                {
                    goto roll; // Skip all modifiers
                }
            }
            if (target.Ability == PBEAbility.WonderSkin && mData.Category == PBEMoveCategory.Status && !user.HasCancellingAbility())
            {
                chance = Math.Min(50, chance);
            }
            bool ignoreA = mData.Category != PBEMoveCategory.Status && target.Ability == PBEAbility.Unaware && !user.HasCancellingAbility();
            bool ignoreE = mData.Effect == PBEMoveEffect.ChipAway || (mData.Category != PBEMoveCategory.Status && user.Ability == PBEAbility.Unaware);
            double accuracy = ignoreA ? 1 : GetStatChangeModifier(user.AccuracyChange, true);
            double evasion;
            if (ignoreE)
            {
                evasion = 1;
            }
            else
            {
                bool ignorePositive = target.Status2.HasFlag(PBEStatus2.Identified) || target.Status2.HasFlag(PBEStatus2.MiracleEye);
                evasion = GetStatChangeModifier(ignorePositive ? Math.Min((sbyte)0, target.EvasionChange) : target.EvasionChange, true);
            }
            chance *= accuracy / evasion;
            if (user.Ability == PBEAbility.Compoundeyes)
            {
                chance *= 1.3;
            }
            if (Array.Exists(user.Team.ActiveBattlers, p => p.Ability == PBEAbility.VictoryStar))
            {
                chance *= 1.1;
            }
            if (user.Ability == PBEAbility.Hustle && mData.Category == PBEMoveCategory.Physical)
            {
                chance *= 0.8;
            }
            if (!user.HasCancellingAbility() && ShouldDoWeatherEffects())
            {
                if (Weather == PBEWeather.Sandstorm && target.Ability == PBEAbility.SandVeil)
                {
                    chance *= 0.8;
                }
                if (Weather == PBEWeather.Hailstorm && target.Ability == PBEAbility.SnowCloak)
                {
                    chance *= 0.8;
                }
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
            if (target.Ability == PBEAbility.TangledFeet && target.Status2.HasFlag(PBEStatus2.Confused) && !user.HasCancellingAbility())
            {
                chance *= 0.5;
            }
        roll:
            if (PBERandom.RandomBool((int)chance, 100))
            {
                return false;
            }
        miss:
            BroadcastMoveMissed(user, target);
            return true;
        }
        private bool AttackTypeCheck(PBEBattlePokemon user, PBEBattlePokemon target, PBEType moveType, out PBEResult result, out double damageMultiplier)
        {
            result = PBETypeEffectiveness.IsAffectedByAttack(user, target, moveType, out damageMultiplier);
            if (result == PBEResult.Ineffective_Ability)
            {
                BroadcastAbility(target, target, target.Ability, PBEAbilityAction.Damage);
            }
            if (result != PBEResult.NotVeryEffective_Type && result != PBEResult.Success && result != PBEResult.SuperEffective_Type)
            {
                BroadcastMoveResult(user, target, result);
                return false;
            }
            return true;
        }
        private bool CritCheck(PBEBattlePokemon user, PBEBattlePokemon target, PBEMoveData mData)
        {
            if (((target.Ability == PBEAbility.BattleArmor || target.Ability == PBEAbility.ShellArmor) && !user.HasCancellingAbility())
                || target.Team.TeamStatus.HasFlag(PBETeamStatus.LuckyChant))
            {
                return false;
            }
            if (mData.Flags.HasFlag(PBEMoveFlag.AlwaysCrit))
            {
                return true;
            }
            byte stage = 0;
            if (user.Status2.HasFlag(PBEStatus2.Pumped))
            {
                stage += 2;
            }
            if (user.OriginalSpecies == PBESpecies.Chansey && user.Item == PBEItem.LuckyPunch)
            {
                stage += 2;
            }
            if (user.OriginalSpecies == PBESpecies.Farfetchd && user.Item == PBEItem.Stick)
            {
                stage += 2;
            }
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
            double chance;
            switch (stage)
            {
                case 0: chance = 6.25; break;
                case 1: chance = 12.5; break;
                case 2: chance = 25; break;
                case 3: chance = 33.3; break;
                default: chance = 50; break;
            }
            return PBERandom.RandomBool((int)(chance * 100), 100 * 100);
        }
        private void TrySetLoser(PBEBattlePokemon pkmn)
        {
            if (Winner == null && pkmn.Team.NumConsciousPkmn == 0)
            {
                Winner = pkmn.Team.OpposingTeam;
            }
        }
        private bool FaintCheck(PBEBattlePokemon pkmn)
        {
            if (pkmn.HP == 0)
            {
                _turnOrder.Remove(pkmn);
                ActiveBattlers.Remove(pkmn);
                PBEFieldPosition oldPos = pkmn.FieldPosition;
                PBEBattlePokemon disguisedAsPokemon = pkmn.Status2.HasFlag(PBEStatus2.Disguised) ? pkmn.DisguisedAsPokemon : pkmn;
                pkmn.FieldPosition = PBEFieldPosition.None;
                BroadcastPkmnFainted(pkmn, disguisedAsPokemon, oldPos);
                RemoveInfatuationsAndLockOns(pkmn);
                pkmn.Team.MonFaintedThisTurn = true;
                TrySetLoser(pkmn);
                CastformCherrimCheckAll();
                return true;
            }
            return false;
        }
        private bool GetManipulableChance(PBEBattlePokemon pkmn, int chance)
        {
            // TODO: Does the Rainbow affect abilities activating, such as CuteCharm/Static, Healer/ShedSkin, etc, and which side of the field would they activate from? Victim?
            // TODO: If it does affect abilities, does it affect Effect Spore? It uses its own weird rng
            if (pkmn.Ability == PBEAbility.SereneGrace)
            {
                chance *= 2;
            }
            return PBERandom.RandomBool(chance, 100);
        }

        private void ActivateAbility(PBEBattlePokemon pkmn, bool switchIn)
        {
            if (!switchIn)
            {
                CastformCherrimCheck(pkmn); // Switch-Ins check this after all Pokémon are sent out
            }
            AntiStatusAbilityCheck(pkmn);
            switch (pkmn.Ability)
            {
                case PBEAbility.AirLock:
                case PBEAbility.CloudNine:
                {
                    if (switchIn)
                    {
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.Weather);
                    }
                    else
                    {
                        CastformCherrimCheckAll();
                    }
                    break;
                }
                case PBEAbility.Anticipation:
                {
                    PBEBattlePokemon[] oppActive = pkmn.Team.OpposingTeam.ActiveBattlers;
                    if (oppActive.Length != 0)
                    {
                        foreach (PBEBattlePokemon opponent in oppActive)
                        {
                            foreach (PBEBattleMoveset.PBEBattleMovesetSlot moveSlot in opponent.Moves)
                            {
                                PBEMove move = moveSlot.Move;
                                if (move != PBEMove.None)
                                {
                                    PBEMoveData mData = PBEMoveData.Data[move];
                                    if (mData.Category != PBEMoveCategory.Status)
                                    {
                                        double d = PBETypeEffectiveness.GetEffectiveness(mData.Type, pkmn);
                                        if (d > 1)
                                        {
                                            BroadcastAbility(pkmn, pkmn, PBEAbility.Anticipation, PBEAbilityAction.Announced);
                                            goto bottomAnticipation;
                                        }
                                    }
                                }
                            }
                        }
                    }
                bottomAnticipation:
                    break;
                }
                case PBEAbility.Download:
                {
                    PBEBattlePokemon[] oppActive = pkmn.Team.OpposingTeam.ActiveBattlers;
                    if (oppActive.Length != 0)
                    {
                        PBEStat stat = oppActive.Average(p => p.Defense * GetStatChangeModifier(p.DefenseChange, false))
                                < oppActive.Average(p => p.SpDefense * GetStatChangeModifier(p.SpDefenseChange, false))
                                ? PBEStat.Attack : PBEStat.SpAttack;
                        if (pkmn.GetStatChange(stat) < Settings.MaxStatChange)
                        {
                            BroadcastAbility(pkmn, pkmn, PBEAbility.Download, PBEAbilityAction.Stats);
                            ApplyStatChangeIfPossible(pkmn, pkmn, stat, +1);
                        }
                    }
                    break;
                }
                case PBEAbility.Drizzle:
                {
                    if (Weather != PBEWeather.Rain || WeatherCounter != 0)
                    {
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.Weather);
                        SetWeather(PBEWeather.Rain, 0, switchIn);
                    }
                    break;
                }
                case PBEAbility.Drought:
                {
                    if (Weather != PBEWeather.HarshSunlight || WeatherCounter != 0)
                    {
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.Weather);
                        SetWeather(PBEWeather.HarshSunlight, 0, switchIn);
                    }
                    break;
                }
                case PBEAbility.Imposter:
                {
                    PBEFieldPosition targetPos = GetPositionAcross(BattleFormat, pkmn.FieldPosition);
                    PBEBattlePokemon target = pkmn.Team.OpposingTeam.TryGetPokemon(targetPos);
                    if (target != null && target.IsTransformPossible(pkmn) == PBEResult.Success)
                    {
                        BroadcastAbility(pkmn, target, pkmn.Ability, PBEAbilityAction.ChangedAppearance);
                        DoTransform(pkmn, target);
                    }
                    break;
                }
                case PBEAbility.MoldBreaker:
                case PBEAbility.Teravolt:
                case PBEAbility.Turboblaze:
                {
                    BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.Announced);
                    break;
                }
                case PBEAbility.SandStream:
                {
                    if (Weather != PBEWeather.Sandstorm || WeatherCounter != 0)
                    {
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.Weather);
                        SetWeather(PBEWeather.Sandstorm, 0, switchIn);
                    }
                    break;
                }
                case PBEAbility.SlowStart:
                {
                    pkmn.SlowStart_HinderTurnsLeft = 5;
                    BroadcastAbility(pkmn, pkmn, PBEAbility.SlowStart, PBEAbilityAction.Announced);
                    break;
                }
                case PBEAbility.SnowWarning:
                {
                    if (Weather != PBEWeather.Hailstorm || WeatherCounter != 0)
                    {
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.Weather);
                        SetWeather(PBEWeather.Hailstorm, 0, switchIn);
                    }
                    break;
                }
            }
        }
        private void CastformCherrimCheckAll()
        {
            CastformCherrimCheck(GetActingOrder(ActiveBattlers, true));
        }
        private void CastformCherrimCheck(PBEBattlePokemon[] order)
        {
            foreach (PBEBattlePokemon pkmn in order)
            {
                CastformCherrimCheck(pkmn);
            }
        }
        private void CastformCherrimCheck(PBEBattlePokemon pkmn)
        {
            if (pkmn.Species == PBESpecies.Castform && pkmn.OriginalSpecies == PBESpecies.Castform)
            {
                PBEForm newForm = PBEForm.Castform;
                if (pkmn.Ability == PBEAbility.Forecast && ShouldDoWeatherEffects())
                {
                    switch (Weather)
                    {
                        case PBEWeather.Hailstorm: newForm = PBEForm.Castform_Snowy; break;
                        case PBEWeather.HarshSunlight: newForm = PBEForm.Castform_Sunny; break;
                        case PBEWeather.Rain: newForm = PBEForm.Castform_Rainy; break;
                    }
                    if (newForm != pkmn.Form)
                    {
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedAppearance);
                    }
                }
                if (newForm != pkmn.Form)
                {
                    BroadcastPkmnFormChanged(pkmn, newForm, pkmn.Ability, pkmn.KnownAbility, false);
                }
            }
            else if (pkmn.Species == PBESpecies.Cherrim && pkmn.OriginalSpecies == PBESpecies.Cherrim)
            {
                PBEForm newForm = PBEForm.Cherrim;
                if (pkmn.Ability == PBEAbility.FlowerGift && ShouldDoWeatherEffects())
                {
                    if (Weather == PBEWeather.HarshSunlight)
                    {
                        newForm = PBEForm.Cherrim_Sunshine;
                    }
                    if (newForm != pkmn.Form)
                    {
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedAppearance);
                    }
                }
                if (newForm != pkmn.Form)
                {
                    BroadcastPkmnFormChanged(pkmn, newForm, pkmn.Ability, pkmn.KnownAbility, false);
                }
            }
        }
        private void ShayminCheck(PBEBattlePokemon pkmn)
        {
            // If a Shaymin_Sky is given MagmaArmor and then Frozen, it will change to Shaymin and obtain Shaymin's ability, therefore losing MagmaArmor and as a result will not be cured of its Frozen status.
            if (pkmn.Species == PBESpecies.Shaymin && pkmn.OriginalSpecies == PBESpecies.Shaymin && pkmn.Form == PBEForm.Shaymin_Sky && pkmn.Status1 == PBEStatus1.Frozen)
            {
                const PBEForm newForm = PBEForm.Shaymin;
                PBEAbility newAbility = PBEPokemonData.GetData(PBESpecies.Shaymin, newForm).Abilities[0];
                BroadcastPkmnFormChanged(pkmn, newForm, newAbility, PBEAbility.MAX, true);
                ActivateAbility(pkmn, false);
            }
        }
        private void IllusionBreak(PBEBattlePokemon pkmn, PBEBattlePokemon breaker)
        {
            if (pkmn.Status2.HasFlag(PBEStatus2.Disguised))
            {
                pkmn.DisguisedAsPokemon = null;
                pkmn.KnownGender = pkmn.Gender;
                pkmn.KnownNickname = pkmn.Nickname;
                pkmn.KnownShiny = pkmn.Shiny;
                pkmn.KnownSpecies = pkmn.Species;
                pkmn.KnownType1 = pkmn.Type1;
                pkmn.KnownType2 = pkmn.Type2;
                BroadcastIllusion(pkmn);
                BroadcastAbility(pkmn, breaker, PBEAbility.Illusion, PBEAbilityAction.ChangedAppearance);
                BroadcastStatus2(pkmn, breaker, PBEStatus2.Disguised, PBEStatusAction.Ended);
            }
        }
        private void AntiStatusAbilityCheck(PBEBattlePokemon pkmn)
        {
            switch (pkmn.Ability)
            {
                case PBEAbility.Immunity:
                {
                    if (pkmn.Status1 == PBEStatus1.BadlyPoisoned || pkmn.Status1 == PBEStatus1.Poisoned)
                    {
                        PBEStatus1 oldStatus1 = pkmn.Status1;
                        pkmn.Status1 = PBEStatus1.None;
                        pkmn.Status1Counter = 0;
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                        BroadcastStatus1(pkmn, pkmn, oldStatus1, PBEStatusAction.Cleared);
                    }
                    break;
                }
                case PBEAbility.Insomnia:
                case PBEAbility.VitalSpirit:
                {
                    if (pkmn.Status1 == PBEStatus1.Asleep)
                    {
                        pkmn.Status1 = PBEStatus1.None;
                        pkmn.Status1Counter = 0;
                        pkmn.SleepTurns = 0;
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                        BroadcastStatus1(pkmn, pkmn, PBEStatus1.Asleep, PBEStatusAction.Cleared);
                        CureNightmare(pkmn, pkmn);
                    }
                    break;
                }
                case PBEAbility.Limber:
                {
                    if (pkmn.Status1 == PBEStatus1.Paralyzed)
                    {
                        pkmn.Status1 = PBEStatus1.None;
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                        BroadcastStatus1(pkmn, pkmn, PBEStatus1.Paralyzed, PBEStatusAction.Cleared);
                    }
                    break;
                }
                case PBEAbility.MagmaArmor:
                {
                    if (pkmn.Status1 == PBEStatus1.Frozen)
                    {
                        pkmn.Status1 = PBEStatus1.None;
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                        BroadcastStatus1(pkmn, pkmn, PBEStatus1.Frozen, PBEStatusAction.Cleared);
                    }
                    break;
                }
                case PBEAbility.Oblivious:
                {
                    if (pkmn.Status2.HasFlag(PBEStatus2.Infatuated))
                    {
                        pkmn.InfatuatedWithPokemon = null;
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                        BroadcastStatus2(pkmn, pkmn, PBEStatus2.Infatuated, PBEStatusAction.Cleared);
                    }
                    break;
                }
                case PBEAbility.OwnTempo:
                {
                    if (pkmn.Status2.HasFlag(PBEStatus2.Confused))
                    {
                        pkmn.ConfusionCounter = 0;
                        pkmn.ConfusionTurns = 0;
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                        BroadcastStatus2(pkmn, pkmn, PBEStatus2.Confused, PBEStatusAction.Cleared);
                    }
                    break;
                }
                case PBEAbility.WaterVeil:
                {
                    if (pkmn.Status1 == PBEStatus1.Burned)
                    {
                        pkmn.Status1 = PBEStatus1.None;
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                        BroadcastStatus1(pkmn, pkmn, PBEStatus1.Burned, PBEStatusAction.Cleared);
                    }
                    break;
                }
            }
        }
        private void CauseConfusion(PBEBattlePokemon target, PBEBattlePokemon other)
        {
            target.ConfusionCounter = 0;
            target.ConfusionTurns = (byte)PBERandom.RandomInt(Settings.ConfusionMinTurns, Settings.ConfusionMaxTurns);
            BroadcastStatus2(target, other, PBEStatus2.Confused, PBEStatusAction.Added);
            AntiStatusAbilityCheck(target);
        }
        private void CauseInfatuation(PBEBattlePokemon target, PBEBattlePokemon other)
        {
            target.InfatuatedWithPokemon = other;
            BroadcastStatus2(target, other, PBEStatus2.Infatuated, PBEStatusAction.Added);
            if (target.Item == PBEItem.DestinyKnot && other.IsAttractionPossible(target) == PBEResult.Success)
            {
                BroadcastItem(target, other, PBEItem.DestinyKnot, PBEItemAction.ChangedStatus);
                other.InfatuatedWithPokemon = target;
                BroadcastStatus2(other, target, PBEStatus2.Infatuated, PBEStatusAction.Added);
            }
            AntiStatusAbilityCheck(target);
        }
        // TODO: Use & add packet handlers
        private void WhiteHerbCheck(PBEBattlePokemon pkmn)
        {
            if (pkmn.Item == PBEItem.WhiteHerb)
            {
                PBEStat[] negStats = pkmn.GetStatsLessThan(0);
                if (negStats.Length > 0)
                {
                    foreach (PBEStat s in negStats)
                    {
                        pkmn.SetStatChange(s, 0);
                    }
                    BroadcastItem(pkmn, pkmn, PBEItem.WhiteHerb, PBEItemAction.Consumed);
                }
            }
        }
        private bool PowerHerbCheck(PBEBattlePokemon pkmn)
        {
            if (pkmn.Item == PBEItem.PowerHerb)
            {
                BroadcastItem(pkmn, pkmn, PBEItem.PowerHerb, PBEItemAction.Consumed);
                return true;
            }
            return false;
        }
        private void LowHPBerryCheck(PBEBattlePokemon pkmn, PBEBattlePokemon forcedToEatBy = null)
        {
            forcedToEatBy = forcedToEatBy ?? pkmn;
            void DoConfuseBerry(PBEFlavor flavor)
            {
                BroadcastItem(pkmn, forcedToEatBy, pkmn.Item, PBEItemAction.Consumed);
                HealDamage(pkmn, pkmn.MaxHP / 8);
                // Verified: Ignores Safeguard & Substitute, but not Own Tempo
                // Mold Breaker etc actually affect whether Own Tempo is ignored, which is what forcedToEatBy is for
                // I verified each of the times the Pokémon eats to check if Mold Breaker affected the outcome
                if (pkmn.Nature.GetRelationshipToFlavor(flavor) < 0 && pkmn.IsConfusionPossible(forcedToEatBy, ignoreSubstitute: true, ignoreSafeguard: true) == PBEResult.Success)
                {
                    CauseConfusion(pkmn, forcedToEatBy);
                }
            }
            void DoHealItem(int hp)
            {
                BroadcastItem(pkmn, forcedToEatBy, pkmn.Item, PBEItemAction.Consumed);
                HealDamage(pkmn, hp);
            }
            void DoStatItem(PBEStat stat, int change)
            {
                // Verified: Mold Breaker affects Contrary/Simple here, unlike with Belly Drum
                if (pkmn.IsStatChangePossible(stat, forcedToEatBy, change, out sbyte oldValue, out sbyte newValue) == PBEResult.Success)
                {
                    BroadcastItem(pkmn, forcedToEatBy, pkmn.Item, PBEItemAction.Consumed);
                    SetStatAndBroadcast(pkmn, stat, oldValue, newValue);
                }
            }

            if (pkmn.HP <= pkmn.MaxHP / 4)
            {
                switch (pkmn.Item)
                {
                    case PBEItem.ApicotBerry: DoStatItem(PBEStat.SpDefense, +1); break;
                    case PBEItem.GanlonBerry: DoStatItem(PBEStat.Defense, +1); break;
                    case PBEItem.LiechiBerry: DoStatItem(PBEStat.Attack, +1); break;
                    case PBEItem.PetayaBerry: DoStatItem(PBEStat.SpAttack, +1); break;
                    case PBEItem.SalacBerry: DoStatItem(PBEStat.Speed, +1); break;
                    case PBEItem.StarfBerry:
                    {
                        // Verified: Starf Berry does not activate for Accuracy or Evasion, or if all other stats are maximized
                        PBEStat[] statsThatCanGoUp = PBEDataUtils.StarfBerryStats.Where(s => pkmn.GetStatChange(s) < Settings.MaxStatChange).ToArray();
                        if (statsThatCanGoUp.Length > 0)
                        {
                            DoStatItem(statsThatCanGoUp.RandomElement(), +2);
                        }
                        break;
                    }
                }
            }
            if (pkmn.HP <= pkmn.MaxHP / 2)
            {
                switch (pkmn.Item)
                {
                    case PBEItem.AguavBerry: DoConfuseBerry(PBEFlavor.Bitter); break;
                    case PBEItem.BerryJuice: DoHealItem(20); break;
                    case PBEItem.FigyBerry: DoConfuseBerry(PBEFlavor.Spicy); break;
                    case PBEItem.IapapaBerry: DoConfuseBerry(PBEFlavor.Sour); break;
                    case PBEItem.MagoBerry: DoConfuseBerry(PBEFlavor.Sweet); break;
                    case PBEItem.OranBerry: DoHealItem(10); break;
                    case PBEItem.SitrusBerry: DoHealItem(pkmn.MaxHP / 4); break;
                    case PBEItem.WikiBerry: DoConfuseBerry(PBEFlavor.Dry); break;
                }
            }
        }
        private void SetAbility(PBEBattlePokemon user, PBEBattlePokemon target, PBEAbility ability)
        {
            // This func assumes new ability is different from current
            PBEAbility oldAbility = target.Ability;
            BroadcastAbilityReplaced(target, ability);

            switch (oldAbility)
            {
                case PBEAbility.Illusion:
                {
                    IllusionBreak(target, user);
                    break;
                }
                case PBEAbility.SlowStart:
                {
                    target.SlowStart_HinderTurnsLeft = 0;
                    break;
                }
                case PBEAbility.SpeedBoost:
                {
                    target.SpeedBoost_AbleToSpeedBoostThisTurn = false;
                    break;
                }
            }

            ActivateAbility(target, false);
        }
        private void SetWeather(PBEWeather weather, byte weatherCounter, bool switchIn)
        {
            Weather = weather;
            WeatherCounter = weatherCounter;
            BroadcastWeather(Weather, PBEWeatherAction.Added);
            if (!switchIn)
            {
                CastformCherrimCheckAll(); // Switch-Ins check this after all Pokémon are sent out
            }
        }

        private void RecordExecutedMove(PBEBattlePokemon user, PBEMove move, PBEMoveData mData)
        {
            user.HasUsedMoveThisTurn = true;
            // Doesn't care if there is a Choice Locked move already. As long as the user knows it, it will become locked. (Metronome calling a move the user knows, Ditto transforming into someone else with transform)
            if ((user.Item == PBEItem.ChoiceBand || user.Item == PBEItem.ChoiceScarf || user.Item == PBEItem.ChoiceSpecs) && user.Moves.Contains(move))
            {
                BroadcastMoveLock_ChoiceItem(user, move);
            }
            if (mData.Effect == PBEMoveEffect.Minimize)
            {
                user.Minimize_Used = true;
            }
        }
        private void PPReduce(PBEBattlePokemon pkmn, PBEMove move)
        {
            if (!_calledFromOtherMove)
            {
                const int amountToReduce = 1;
                // TODO: If target is not self and has pressure
                PBEBattleMoveset.PBEBattleMovesetSlot slot = pkmn.Moves[move];
                int oldPP = slot.PP;
                int newPP = Math.Max(0, oldPP - amountToReduce);
                int amountReduced = oldPP - newPP;
                slot.PP = newPP;
                pkmn.UpdateKnownPP(move, amountReduced);
                BroadcastMovePPChanged(pkmn, move, amountReduced);
            }
        }

        private void CureNightmare(PBEBattlePokemon wakingUp, PBEBattlePokemon pokemon2)
        {
            if (wakingUp.Status2.HasFlag(PBEStatus2.Nightmare))
            {
                BroadcastStatus2(wakingUp, pokemon2, PBEStatus2.Nightmare, PBEStatusAction.Ended);
            }
        }
        private void SetSleepTurns(PBEBattlePokemon pkmn, int minTurns, int maxTurns)
        {
            pkmn.SleepTurns = (byte)(PBERandom.RandomInt(minTurns, maxTurns) / (pkmn.Ability == PBEAbility.EarlyBird ? 2 : 1));
        }
        private void DoTransform(PBEBattlePokemon user, PBEBattlePokemon target)
        {
            user.Transform(target);
            BroadcastTransform(user, target);
            BroadcastStatus2(user, target, PBEStatus2.Transformed, PBEStatusAction.Added);
            if (user.Status2.HasFlag(PBEStatus2.PowerTrick))
            {
                BroadcastStatus2(user, user, PBEStatus2.PowerTrick, PBEStatusAction.Ended);
            }
            if (!user.Moves.Contains(user.ChoiceLockedMove))
            {
                BroadcastMoveLock_ChoiceItem(user, PBEMove.None);
            }
        }
        private PBEResult ApplyStatus1IfPossible(PBEBattlePokemon user, PBEBattlePokemon target, PBEStatus1 status, bool broadcastUnsuccessful)
        {
            PBEResult result;
            switch (status)
            {
                case PBEStatus1.Asleep: result = target.IsSleepPossible(user); break;
                case PBEStatus1.BadlyPoisoned:
                case PBEStatus1.Poisoned: result = target.IsPoisonPossible(user); break;
                case PBEStatus1.Burned: result = target.IsBurnPossible(user); break;
                case PBEStatus1.Frozen: result = target.IsFreezePossible(user); break;
                case PBEStatus1.Paralyzed: result = target.IsParalysisPossible(user); break;
                default: throw new ArgumentOutOfRangeException(nameof(status));
            }
            if (result == PBEResult.Success)
            {
                target.Status1 = status;
                if (status == PBEStatus1.BadlyPoisoned)
                {
                    target.Status1Counter = 1;
                }
                else if (status == PBEStatus1.Asleep)
                {
                    SetSleepTurns(target, Settings.SleepMinTurns, Settings.SleepMaxTurns);
                    target.Status1Counter = 0;
                }
                BroadcastStatus1(target, user, status, PBEStatusAction.Added);
                ShayminCheck(target);
            }
            else if (broadcastUnsuccessful)
            {
                if (result == PBEResult.Ineffective_Ability)
                {
                    BroadcastAbility(target, user, target.Ability, PBEAbilityAction.PreventedStatus);
                }
                BroadcastMoveResult(user, target, result);
            }
            return result;
        }
        private PBEResult ApplyStatus2IfPossible(PBEBattlePokemon user, PBEBattlePokemon target, PBEStatus2 status, bool broadcastUnsuccessful)
        {
            PBEResult result;
            switch (status)
            {
                case PBEStatus2.Confused:
                {
                    result = target.IsConfusionPossible(user);
                    if (result == PBEResult.Success)
                    {
                        CauseConfusion(target, user);
                    }
                    break;
                }
                case PBEStatus2.Cursed:
                {
                    if (!target.Status2.HasFlag(PBEStatus2.Cursed))
                    {
                        BroadcastStatus2(target, user, PBEStatus2.Cursed, PBEStatusAction.Added);
                        DealDamage(user, user, user.MaxHP / 2, true);
                        if (!FaintCheck(user))
                        {
                            LowHPBerryCheck(user);
                        }
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBEStatus2.Flinching:
                {
                    result = target.IsFlinchPossible(user);
                    if (result == PBEResult.Success)
                    {
                        target.Status2 |= PBEStatus2.Flinching; // No broadcast, not known
                    }
                    break;
                }
                case PBEStatus2.HelpingHand:
                {
                    if (!target.HasUsedMoveThisTurn)
                    {
                        BroadcastStatus2(target, user, PBEStatus2.HelpingHand, PBEStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.InvalidConditions;
                    }
                    break;
                }
                case PBEStatus2.Identified:
                {
                    if (!target.Status2.HasFlag(PBEStatus2.Identified))
                    {
                        BroadcastStatus2(target, user, PBEStatus2.Identified, PBEStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBEStatus2.Infatuated:
                {
                    result = target.IsAttractionPossible(user);
                    if (result == PBEResult.Success)
                    {
                        CauseInfatuation(target, user);
                    }
                    break;
                }
                case PBEStatus2.LeechSeed:
                {
                    result = target.IsLeechSeedPossible();
                    if (result == PBEResult.Success)
                    {
                        target.SeededPosition = user.FieldPosition;
                        target.SeededTeam = user.Team;
                        BroadcastStatus2(target, user, PBEStatus2.LeechSeed, PBEStatusAction.Added);
                    }
                    break;
                }
                case PBEStatus2.LockOn:
                {
                    if (!user.Status2.HasFlag(PBEStatus2.LockOn))
                    {
                        user.LockOnPokemon = target;
                        user.LockOnTurns = 2;
                        BroadcastStatus2(user, target, PBEStatus2.LockOn, PBEStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBEStatus2.MagnetRise:
                {
                    result = target.IsMagnetRisePossible();
                    if (result == PBEResult.Success)
                    {
                        target.MagnetRiseTurns = 5;
                        BroadcastStatus2(target, user, PBEStatus2.MagnetRise, PBEStatusAction.Added);
                    }
                    break;
                }
                case PBEStatus2.MiracleEye:
                {
                    if (!target.Status2.HasFlag(PBEStatus2.MiracleEye))
                    {
                        BroadcastStatus2(target, user, PBEStatus2.MiracleEye, PBEStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBEStatus2.Nightmare:
                {
                    if (target.Status1 == PBEStatus1.Asleep && !target.Status2.HasFlag(PBEStatus2.Nightmare))
                    {
                        BroadcastStatus2(target, user, PBEStatus2.Nightmare, PBEStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBEStatus2.PowerTrick:
                {
                    target.ApplyPowerTrickChange();
                    BroadcastStatus2(target, user, PBEStatus2.PowerTrick, PBEStatusAction.Added);
                    result = PBEResult.Success;
                    break;
                }
                case PBEStatus2.Protected:
                {
                    // TODO: If the user goes last, fail
                    if (PBERandom.RandomBool(user.GetProtectionChance(), ushort.MaxValue))
                    {
                        user.Protection_Used = true;
                        BroadcastStatus2(user, user, PBEStatus2.Protected, PBEStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.InvalidConditions;
                    }
                    break;
                }
                case PBEStatus2.Pumped:
                {
                    if (!target.Status2.HasFlag(PBEStatus2.Pumped))
                    {
                        BroadcastStatus2(target, user, PBEStatus2.Pumped, PBEStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBEStatus2.Substitute:
                {
                    result = target.IsSubstitutePossible();
                    if (result == PBEResult.Success)
                    {
                        int hpRequired = target.MaxHP / 4;
                        DealDamage(user, target, hpRequired, true);
                        LowHPBerryCheck(target);
                        target.SubstituteHP = (ushort)hpRequired;
                        BroadcastStatus2(target, user, PBEStatus2.Substitute, PBEStatusAction.Added);
                    }
                    break;
                }
                case PBEStatus2.Transformed:
                {
                    result = target.IsTransformPossible(user);
                    if (result == PBEResult.Success)
                    {
                        DoTransform(user, target);
                    }
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(status));
            }
            if (broadcastUnsuccessful && result != PBEResult.Success)
            {
                if (result == PBEResult.Ineffective_Ability)
                {
                    BroadcastAbility(target, user, target.Ability, PBEAbilityAction.PreventedStatus);
                }
                BroadcastMoveResult(user, target, result);
            }
            return result;
        }
        private void ApplyStatChangeIfPossible(PBEBattlePokemon user, PBEBattlePokemon target, PBEStat stat, int change, bool isSecondaryEffect = false)
        {
            PBEResult result = target.IsStatChangePossible(stat, user, change, out sbyte oldValue, out sbyte newValue);
            bool broadcast;
            if (result == PBEResult.Success)
            {
                target.SetStatChange(stat, newValue);
                broadcast = true;
            }
            else
            {
                if (result == PBEResult.Ineffective_Ability)
                {
                    if (!isSecondaryEffect)
                    {
                        BroadcastAbility(target, user, target.Ability, PBEAbilityAction.Stats);
                    }
                    return;
                }
                else
                {
                    // Do not broadcast "could not be lowered!" for Mud-Slap, etc
                    broadcast = !isSecondaryEffect;
                }
            }
            if (broadcast)
            {
                BroadcastPkmnStatChanged(target, stat, oldValue, newValue);
            }
        }
        private void SetStatAndBroadcast(PBEBattlePokemon pkmn, PBEStat stat, sbyte oldValue, sbyte newValue)
        {
            pkmn.SetStatChange(stat, newValue);
            BroadcastPkmnStatChanged(pkmn, stat, oldValue, newValue);
        }

        private PBEPkmnSwitchInPacket.PBESwitchInInfo CreateSwitchInInfo(PBEBattlePokemon pkmn)
        {
            if (pkmn.Ability == PBEAbility.Illusion)
            {
                PBEList<PBEBattlePokemon> party = pkmn.Team.Party;
                for (int i = party.Count - 1; i >= 0; i--)
                {
                    PBEBattlePokemon p = party[i];
                    if (p.HP > 0)
                    {
                        // If this Pokémon is the "last" conscious one, it will go out as itself (loop breaks)
                        // The only way to disguise as a Pokémon that's on the battlefield is the first turn of a Double/Triple/Rotation battle
                        if (p.OriginalSpecies != pkmn.OriginalSpecies)
                        {
                            pkmn.Status2 |= PBEStatus2.Disguised; // No broadcast, not known
                            pkmn.DisguisedAsPokemon = p;
                            pkmn.KnownGender = p.Gender;
                            pkmn.KnownNickname = p.Nickname;
                            pkmn.KnownShiny = p.Shiny;
                            pkmn.KnownSpecies = p.OriginalSpecies;
                            pkmn.KnownForm = p.Form;
                            var pData = PBEPokemonData.GetData(pkmn.KnownSpecies, pkmn.KnownForm);
                            pkmn.KnownType1 = pData.Type1;
                            pkmn.KnownType2 = pData.Type2;
                        }
                        break;
                    }
                }
            }
            return new PBEPkmnSwitchInPacket.PBESwitchInInfo(pkmn);
        }
        private void SwitchTwoPokemon(PBEBattlePokemon pkmnLeaving, PBEBattlePokemon pkmnComing, PBEBattlePokemon forcedByPkmn = null)
        {
            PBEFieldPosition pos = pkmnLeaving.FieldPosition;
            pkmnLeaving.FieldPosition = PBEFieldPosition.None;
            _turnOrder.Remove(pkmnLeaving);
            ActiveBattlers.Remove(pkmnLeaving);
            PBEBattlePokemon disguisedAsPokemon = pkmnLeaving.Status2.HasFlag(PBEStatus2.Disguised) ? pkmnLeaving.DisguisedAsPokemon : pkmnLeaving;
            pkmnLeaving.ClearForSwitch();
            BroadcastPkmnSwitchOut(pkmnLeaving, disguisedAsPokemon, pos, forcedByPkmn);
            RemoveInfatuationsAndLockOns(pkmnLeaving);
            pkmnComing.FieldPosition = pos;
            var switches = new PBEPkmnSwitchInPacket.PBESwitchInInfo[] { CreateSwitchInInfo(pkmnComing) };
            PBETeam.SwitchTwoPokemon(pkmnLeaving, pkmnComing);
            BroadcastPkmnSwitchIn(pkmnComing.Team, switches, forcedByPkmn);
            ActiveBattlers.Add(pkmnComing);
            if (forcedByPkmn != null)
            {
                BroadcastDraggedOut(pkmnComing);
            }
            DoSwitchInEffects(new[] { pkmnComing }, forcedByPkmn);
            CastformCherrimCheckAll();
        }
        private void RemoveInfatuationsAndLockOns(PBEBattlePokemon pkmnLeaving)
        {
            foreach (PBEBattlePokemon pkmn in ActiveBattlers)
            {
                if (pkmn.Status2.HasFlag(PBEStatus2.Infatuated) && pkmn.InfatuatedWithPokemon == pkmnLeaving)
                {
                    pkmn.InfatuatedWithPokemon = null;
                    BroadcastStatus2(pkmn, pkmn, PBEStatus2.Infatuated, PBEStatusAction.Ended);
                }
                if (pkmn.Status2.HasFlag(PBEStatus2.LockOn) && pkmn.LockOnPokemon == pkmnLeaving)
                {
                    pkmn.LockOnPokemon = null;
                    pkmn.LockOnTurns = 0;
                    BroadcastStatus2(pkmn, pkmn, PBEStatus2.LockOn, PBEStatusAction.Ended);
                }
            }
        }

        // Should there be two versions of BasicHit, one for single target and another for multi target?
        // The games probably have two (some move types like MakesContact/Recoil are guaranteed to be single target), but we might not need two so we can support editing moves/adding moves
        // Some moves types will need to throw an exception if they bypass the rule, so they do not violate behavior (pickpocket and life orb interaction, for example)
        private void BasicHit(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMoveData mData,
            Func<int, int?> recoilFunc = null,
            Func<PBEBattlePokemon, PBEResult> beforeDoingDamage = null,
            Action<PBEBattlePokemon> beforePostHit = null,
            Action<PBEBattlePokemon, ushort> afterPostHit = null,
            Action beforeTargetsFaint = null,
            bool hitRegardlessOfUserConciousness = false)
        {
            // TODO: Rocky Helmet tests for user fainting, winner, etc [force battle subway battle flag? or actually do link battles]

            bool hitSomeone = false;
            int totalDamageDealt = 0;
            PBEType moveType = user.GetMoveType(mData);
            double basePower = CalculateBasePower(user, targets, mData, moveType);
            foreach (PBEBattlePokemon target in targets)
            {
                if (!MissCheck(user, target, mData))
                {
                    if (AttackTypeCheck(user, target, moveType, out PBEResult result, out double damageMultiplier))
                    {
                        // Brick Break destroys Light Screen and Reflect before doing damage
                        // Dream Eater checks for sleep before doing damage
                        // Sucker Punch fails before doing damage
                        // Feint destroys protection
                        if (beforeDoingDamage == null || beforeDoingDamage.Invoke(target) == PBEResult.Success)
                        {
                            if (targets.Length > 1)
                            {
                                damageMultiplier *= 0.75;
                            }
                            bool crit = CritCheck(user, target, mData);
                            damageMultiplier *= CalculateDamageMultiplier(user, target, mData, moveType, result, crit);
                            int damage = (int)(damageMultiplier * CalculateDamage(user, target, mData, moveType, basePower, crit));
                            ushort damageDealt = DealDamage(user, target, damage, false);
                            totalDamageDealt += damageDealt;
                            if (result != PBEResult.Success)
                            {
                                BroadcastMoveResult(user, target, result);
                            }
                            if (crit)
                            {
                                BroadcastMoveCrit(target);
                            }
                            // Target's statuses are assigned and target's stats are changed before post-hit effects
                            // Snore has a chance to flinch
                            beforePostHit?.Invoke(target);
                            DoPostHitEffects(user, target, mData, moveType); // User faints here
                            // HP-draining moves restore HP after post-hit effects
                            // Shadow Force destroys protection
                            afterPostHit?.Invoke(target, damageDealt);
                            DoPostAttackedTargetEffects(target, colorChangeType: moveType);
                            hitSomeone = true;
                            // This is not necessary for any official move since no contact moves hit multiple targets, but keeping it here for custom moves
                            if (!hitRegardlessOfUserConciousness && (user.HP == 0 || user.Status1 == PBEStatus1.Asleep))
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (hitSomeone)
            {
                // User's stats change before the targets faint if at least one was hit
                beforeTargetsFaint?.Invoke();
                foreach (PBEBattlePokemon target in targets)
                {
                    FaintCheck(target);
                }
            }
            if (user.HP > 0)
            {
                DoPostAttackedUserEffects(user, hitSomeone, recoilDamage: recoilFunc?.Invoke(totalDamageDealt));
            }
            // Official order: target faints (each target), recoil, Life Orb, user faints/target eats berry (each target), target AntiStatusAbilityCheck() (each target)
        }
        // None of these moves are multi-target
        private void FixedDamageHit(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMoveData mData, Func<PBEBattlePokemon, int> damageFunc,
            Func<PBEBattlePokemon, PBEResult> beforeMissCheck = null,
            Action beforeTargetsFaint = null)
        {
            bool hitSomeone = false;
            PBEType moveType = user.GetMoveType(mData);
            foreach (PBEBattlePokemon target in targets)
            {
                // Endeavor fails if the target's HP is <= the user's HP
                // One hit knockout moves fail if the target's level is > the user's level
                if (beforeMissCheck == null || beforeMissCheck.Invoke(target) == PBEResult.Success)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        if (AttackTypeCheck(user, target, moveType, out PBEResult _, out double _))
                        {
                            // Damage func is run and the output is dealt to target
                            DealDamage(user, target, damageFunc.Invoke(target), false);

                            DoPostHitEffects(user, target, mData, moveType); // User faints here
                            DoPostAttackedTargetEffects(target, colorChangeType: moveType);
                            hitSomeone = true;
                            // This is not necessary for any official move since no contact moves hit multiple targets, but keeping it here for custom moves
                            if (user.HP == 0 || user.Status1 == PBEStatus1.Asleep)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            if (hitSomeone)
            {
                // "It's a one-hit KO!"
                beforeTargetsFaint?.Invoke();
                foreach (PBEBattlePokemon target in targets)
                {
                    FaintCheck(target);
                }
            }
            if (user.HP > 0)
            {
                DoPostAttackedUserEffects(user, hitSomeone);
            }
            // Official order: target faints, Life Orb, user faints/target eats berry, target AntiStatusAbilityCheck()
        }
        // None of these moves are multi-target
        private void MultiHit(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMoveData mData, byte numHits,
            bool subsequentMissChecks = false,
            Action<PBEBattlePokemon> beforePostHit = null)
        {
            bool hitSomeone = false;
            PBEType moveType = user.GetMoveType(mData);
            double basePower = CalculateBasePower(user, targets, mData, moveType); // Verified: Gem boost applies to all hits
            foreach (PBEBattlePokemon target in targets)
            {
                if (!MissCheck(user, target, mData))
                {
                    if (AttackTypeCheck(user, target, moveType, out PBEResult result, out double damageMultiplier))
                    {
                        if (targets.Length > 1)
                        {
                            damageMultiplier *= 0.75;
                        }
                        byte hit = 0;
                        for (int hitNumber = 0; hitNumber < numHits; hitNumber++)
                        {
                            bool crit = CritCheck(user, target, mData);
                            double curDamageMultiplier = damageMultiplier * CalculateDamageMultiplier(user, target, mData, moveType, result, crit);
                            int damage = (int)(curDamageMultiplier * CalculateDamage(user, target, mData, moveType, basePower, crit));
                            DealDamage(user, target, damage, false);
                            if (crit)
                            {
                                BroadcastMoveCrit(target);
                            }
                            // Twineedle has a chance to poison on each strike
                            beforePostHit?.Invoke(target);
                            DoPostHitEffects(user, target, mData, moveType); // User faints here
                            hit++;
                            hitSomeone = true;
                            if (user.HP == 0 || user.Status1 == PBEStatus1.Asleep || target.HP == 0)
                            {
                                break;
                            }
                        }
                        if (result != PBEResult.Success)
                        {
                            BroadcastMoveResult(user, target, result);
                        }
                        BroadcastMultiHit(hit);
                        if (!FaintCheck(target))
                        {
                            DoPostAttackedTargetEffects(target, colorChangeType: moveType); // AntiStatusAbilityCheck() in DoPostAttackedTargetEffects()?
                        }
                        // This is not necessary for any official move since no contact moves hit multiple targets, but keeping it here for custom moves
                        if (user.HP == 0 || user.Status1 == PBEStatus1.Asleep)
                        {
                            break;
                        }
                    }
                }
            }
            if (user.HP > 0)
            {
                DoPostAttackedUserEffects(user, hitSomeone);
            }
            // Official order: user faints/target eats berry, effectiveness, "Hit 4 times!", target faints, Life Orb, target AntiStatusAbilityCheck()
        }
        private void SemiInvulnerableChargeMove(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, PBETurnTarget requestedTargets, PBEStatus2 status2,
            Action<PBEBattlePokemon> beforePostHit = null,
            Action<PBEBattlePokemon, ushort> afterPostHit = null)
        {
            BroadcastMoveUsed(user, move);
        top:
            if (user.Status2.HasFlag(status2))
            {
                BroadcastMoveLock_Temporary(user, PBEMove.None, PBETurnTarget.None);
                BroadcastStatus2(user, user, status2, PBEStatusAction.Ended);
                if (targets.Length == 0)
                {
                    BroadcastMoveResult(user, user, PBEResult.NoTarget);
                }
                else
                {
                    BasicHit(user, targets, mData, beforePostHit: beforePostHit, afterPostHit: afterPostHit);
                }
                RecordExecutedMove(user, move, mData); // Should only count as the last used move if it finishes charging
            }
            else
            {
                PPReduce(user, move);
                BroadcastMoveLock_Temporary(user, move, requestedTargets);
                BroadcastStatus2(user, user, status2, PBEStatusAction.Added);
                if (PowerHerbCheck(user))
                {
                    goto top;
                }
            }
        }

        private void Ef_TryForceStatus1(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, PBEStatus1 status, bool thunderWave = false)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        if (thunderWave)
                        {
                            PBEResult result = PBETypeEffectiveness.ThunderWaveTypeCheck(user, target, move);
                            if (result != PBEResult.Success)
                            {
                                BroadcastMoveResult(user, target, result);
                                continue;
                            }
                        }
                        ApplyStatus1IfPossible(user, target, status, true);
                        DoPostAttackedTargetEffects(target); // Only necessary for AntiStatusCheck() right now
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
            return;
        }
        private void Ef_TryForceStatus2(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, PBEStatus2 status)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        ApplyStatus2IfPossible(user, target, status, true);
                        DoPostAttackedTargetEffects(target); // Only necessary for AntiStatusCheck() right now
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_TryForceBattleStatus(PBEBattlePokemon user, PBEMove move, PBEMoveData mData, PBEBattleStatus status)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            switch (status)
            {
                case PBEBattleStatus.TrickRoom:
                {
                    if (!BattleStatus.HasFlag(PBEBattleStatus.TrickRoom))
                    {
                        TrickRoomCount = 5;
                        BroadcastBattleStatus(PBEBattleStatus.TrickRoom, PBEBattleStatusAction.Added);
                    }
                    else
                    {
                        TrickRoomCount = 0;
                        BroadcastBattleStatus(PBEBattleStatus.TrickRoom, PBEBattleStatusAction.Cleared);
                    }
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(status));
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_TryForceTeamStatus(PBEBattlePokemon user, PBEMove move, PBEMoveData mData, PBETeamStatus status)
        {
            PBEResult result;
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            switch (status)
            {
                case PBETeamStatus.LightScreen:
                {
                    if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.LightScreen))
                    {
                        user.Team.LightScreenCount = (byte)(Settings.LightScreenTurns + (user.Item == PBEItem.LightClay ? Settings.LightClayTurnExtension : 0));
                        BroadcastTeamStatus(user.Team, PBETeamStatus.LightScreen, PBETeamStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBETeamStatus.LuckyChant:
                {
                    if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.LuckyChant))
                    {
                        user.Team.LuckyChantCount = 5;
                        BroadcastTeamStatus(user.Team, PBETeamStatus.LuckyChant, PBETeamStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBETeamStatus.QuickGuard:
                {
                    if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.QuickGuard) && PBERandom.RandomBool(user.GetProtectionChance(), ushort.MaxValue))
                    {
                        user.Protection_Used = true;
                        BroadcastTeamStatus(user.Team, PBETeamStatus.QuickGuard, PBETeamStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBETeamStatus.Reflect:
                {
                    if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.Reflect))
                    {
                        user.Team.ReflectCount = (byte)(Settings.ReflectTurns + (user.Item == PBEItem.LightClay ? Settings.LightClayTurnExtension : 0));
                        BroadcastTeamStatus(user.Team, PBETeamStatus.Reflect, PBETeamStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBETeamStatus.Safeguard:
                {
                    if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
                    {
                        user.Team.SafeguardCount = 5;
                        BroadcastTeamStatus(user.Team, PBETeamStatus.Safeguard, PBETeamStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBETeamStatus.Spikes:
                {
                    if (user.Team.OpposingTeam.SpikeCount < 3)
                    {
                        user.Team.OpposingTeam.SpikeCount++;
                        BroadcastTeamStatus(user.Team.OpposingTeam, PBETeamStatus.Spikes, PBETeamStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBETeamStatus.StealthRock:
                {
                    if (!user.Team.OpposingTeam.TeamStatus.HasFlag(PBETeamStatus.StealthRock))
                    {
                        BroadcastTeamStatus(user.Team.OpposingTeam, PBETeamStatus.StealthRock, PBETeamStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBETeamStatus.Tailwind:
                {
                    if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.Tailwind))
                    {
                        user.Team.TailwindCount = 4;
                        BroadcastTeamStatus(user.Team, PBETeamStatus.Tailwind, PBETeamStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBETeamStatus.ToxicSpikes:
                {
                    if (user.Team.OpposingTeam.ToxicSpikeCount < 2)
                    {
                        user.Team.OpposingTeam.ToxicSpikeCount++;
                        BroadcastTeamStatus(user.Team.OpposingTeam, PBETeamStatus.ToxicSpikes, PBETeamStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                case PBETeamStatus.WideGuard:
                {
                    if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.WideGuard) && PBERandom.RandomBool(user.GetProtectionChance(), ushort.MaxValue))
                    {
                        user.Protection_Used = true;
                        BroadcastTeamStatus(user.Team, PBETeamStatus.WideGuard, PBETeamStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Status;
                    }
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(status));
            }
            if (result != PBEResult.Success)
            {
                BroadcastMoveResult(user, user, result);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_TryForceWeather(PBEBattlePokemon user, PBEMove move, PBEMoveData mData, PBEWeather weather)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (Weather == weather)
            {
                BroadcastMoveResult(user, user, PBEResult.Ineffective_Status);
            }
            else
            {
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
                SetWeather(weather, (byte)(turns + (user.Item == extensionItem ? itemTurnExtension : 0)), false);
            }
            RecordExecutedMove(user, move, mData);
        }

        private void Ef_Growth(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            int change = WillLeafGuardActivate() ? +2 : +1;
            Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Attack, change), (PBEStat.SpAttack, change) });
        }
        private void Ef_ChangeTargetStats(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, (PBEStat, int)[] statChanges, bool requireAttraction = false)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        PBEResult result = requireAttraction ? target.IsAttractionPossible(user, ignoreCurrentStatus: true) : PBEResult.Success;
                        if (result != PBEResult.Success)
                        {
                            BroadcastMoveResult(user, target, result);
                        }
                        else if (user != target && target.Status2.HasFlag(PBEStatus2.Substitute))
                        {
                            BroadcastMoveResult(user, target, PBEResult.Ineffective_Substitute);
                        }
                        else
                        {
                            for (int i = 0; i < statChanges.Length; i++)
                            {
                                (PBEStat stat, int change) = statChanges[i];
                                ApplyStatChangeIfPossible(user, target, stat, change);
                            }
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Hit__MaybeChangeTargetStats(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, (PBEStat, int)[] statChanges, int chanceToChangeStats)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEBattlePokemon target)
                {
                    if (target.HP > 0 && !target.Status2.HasFlag(PBEStatus2.Substitute) && GetManipulableChance(user, chanceToChangeStats))
                    {
                        for (int i = 0; i < statChanges.Length; i++)
                        {
                            (PBEStat stat, int change) = statChanges[i];
                            ApplyStatChangeIfPossible(user, target, stat, change, isSecondaryEffect: true);
                        }
                    }
                }
                BasicHit(user, targets, mData, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Hit__MaybeChangeUserStats(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, (PBEStat, int)[] statChanges, int chanceToChangeStats)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforeTargetsFaint()
                {
                    if (user.HP > 0 && GetManipulableChance(user, chanceToChangeStats))
                    {
                        for (int i = 0; i < statChanges.Length; i++)
                        {
                            (PBEStat stat, int change) = statChanges[i];
                            ApplyStatChangeIfPossible(user, user, stat, change, isSecondaryEffect: true);
                        }
                    }
                }
                BasicHit(user, targets, mData, beforeTargetsFaint: BeforeTargetsFaint);
            }
            RecordExecutedMove(user, move, mData);
        }

        private void Ef_Entrainment(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            var blockedUserAbilities = new PBEAbility[] { PBEAbility.FlowerGift, PBEAbility.Forecast, PBEAbility.Illusion,
                PBEAbility.Imposter, PBEAbility.Trace };

            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        if (target.Ability == user.Ability || blockedUserAbilities.Contains(user.Ability))
                        {
                            BroadcastMoveResult(user, target, PBEResult.InvalidConditions);
                        }
                        else if (target.Ability == PBEAbility.Multitype || target.Ability == PBEAbility.Truant)
                        {
                            BroadcastMoveResult(user, target, PBEResult.Ineffective_Ability);
                        }
                        else
                        {
                            SetAbility(user, target, user.Ability);
                            // TODO: #234 - Reveal other Pokémon's Ability
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_RolePlay(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            var blockedUserAbilities = new PBEAbility[] { PBEAbility.Imposter, PBEAbility.Multitype, PBEAbility.ZenMode };
            var blockedTargetAbilities = new PBEAbility[] { PBEAbility.FlowerGift, PBEAbility.Forecast, PBEAbility.Illusion,
                PBEAbility.Imposter, PBEAbility.Multitype, PBEAbility.Trace, PBEAbility.WonderGuard, PBEAbility.ZenMode };

            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        if (target.Ability == user.Ability || blockedUserAbilities.Contains(user.Ability) || blockedTargetAbilities.Contains(target.Ability))
                        {
                            BroadcastMoveResult(user, target, PBEResult.InvalidConditions);
                        }
                        else
                        {
                            SetAbility(user, user, target.Ability);
                            // TODO: #234 - Reveal other Pokémon's Ability
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_SetOtherAbility(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, PBEAbility ability, bool blockedByTruant)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        if (target.Ability == ability)
                        {
                            BroadcastMoveResult(user, target, PBEResult.InvalidConditions);
                        }
                        else if (target.Ability == PBEAbility.Multitype || (blockedByTruant && target.Ability == PBEAbility.Truant))
                        {
                            BroadcastMoveResult(user, target, PBEResult.Ineffective_Ability);
                        }
                        else
                        {
                            SetAbility(user, target, ability);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }

        private void Ef_Bounce(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, PBETurnTarget requestedTargets)
        {
            void BeforePostHit(PBEBattlePokemon target)
            {
                if (target.HP > 0 && GetManipulableChance(user, mData.EffectParam))
                {
                    ApplyStatus1IfPossible(user, target, PBEStatus1.Paralyzed, false);
                }
            }
            SemiInvulnerableChargeMove(user, targets, move, mData, requestedTargets, PBEStatus2.Airborne, beforePostHit: BeforePostHit);
        }
        private void Ef_ShadowForce(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, PBETurnTarget requestedTargets)
        {
            void AfterPostHit(PBEBattlePokemon target, ushort damageDealt)
            {
                if (target.HP > 0 && target.Status2.HasFlag(PBEStatus2.Protected))
                {
                    BroadcastStatus2(target, user, PBEStatus2.Protected, PBEStatusAction.Cleared);
                }
                if (target.Team.TeamStatus.HasFlag(PBETeamStatus.QuickGuard))
                {
                    BroadcastTeamStatus(target.Team, PBETeamStatus.QuickGuard, PBETeamStatusAction.Cleared, damageVictim: target);
                }
                if (target.Team.TeamStatus.HasFlag(PBETeamStatus.WideGuard))
                {
                    BroadcastTeamStatus(target.Team, PBETeamStatus.WideGuard, PBETeamStatusAction.Cleared, damageVictim: target);
                }
            }
            SemiInvulnerableChargeMove(user, targets, move, mData, requestedTargets, PBEStatus2.ShadowForce, afterPostHit: AfterPostHit);
        }

        private void Ef_BrickBreak(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                PBEResult BeforeDoingDamage(PBEBattlePokemon target)
                {
                    // Verified: Reflect then Light Screen
                    if (target.Team.TeamStatus.HasFlag(PBETeamStatus.Reflect))
                    {
                        target.Team.ReflectCount = 0;
                        BroadcastTeamStatus(target.Team, PBETeamStatus.Reflect, PBETeamStatusAction.Cleared);
                    }
                    if (target.Team.TeamStatus.HasFlag(PBETeamStatus.LightScreen))
                    {
                        target.Team.LightScreenCount = 0;
                        BroadcastTeamStatus(target.Team, PBETeamStatus.LightScreen, PBETeamStatusAction.Cleared);
                    }
                    return PBEResult.Success;
                }
                BasicHit(user, targets, mData, beforeDoingDamage: BeforeDoingDamage);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Feint(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                PBEResult BeforeDoingDamage(PBEBattlePokemon target)
                {
                    if (target.HP > 0 && target.Status2.HasFlag(PBEStatus2.Protected))
                    {
                        BroadcastStatus2(target, user, PBEStatus2.Protected, PBEStatusAction.Cleared);
                    }
                    if (target.Team == user.Team.OpposingTeam)
                    {
                        if (target.Team.TeamStatus.HasFlag(PBETeamStatus.QuickGuard))
                        {
                            BroadcastTeamStatus(target.Team, PBETeamStatus.QuickGuard, PBETeamStatusAction.Cleared, damageVictim: target);
                        }
                        if (target.Team.TeamStatus.HasFlag(PBETeamStatus.WideGuard))
                        {
                            BroadcastTeamStatus(target.Team, PBETeamStatus.WideGuard, PBETeamStatusAction.Cleared, damageVictim: target);
                        }
                    }
                    return PBEResult.Success;
                }
                BasicHit(user, targets, mData, beforeDoingDamage: BeforeDoingDamage);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Hit(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, PBEStatus1 status1 = PBEStatus1.None, int chanceToInflictStatus1 = 0, PBEStatus2 status2 = PBEStatus2.None, int chanceToInflictStatus2 = 0)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEBattlePokemon target)
                {
                    if (target.HP > 0)
                    {
                        if (status1 != PBEStatus1.None && GetManipulableChance(user, chanceToInflictStatus1))
                        {
                            ApplyStatus1IfPossible(user, target, status1, false);
                        }
                        if (status2 != PBEStatus2.None && GetManipulableChance(user, chanceToInflictStatus2))
                        {
                            ApplyStatus2IfPossible(user, target, status2, false);
                        }
                    }
                }
                BasicHit(user, targets, mData, beforePostHit: status1 != PBEStatus1.None || status2 != PBEStatus2.None ? BeforePostHit : (Action<PBEBattlePokemon>)null);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Hit__MaybeBurnFreezeParalyze(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEBattlePokemon target)
                {
                    if (target.HP > 0 && GetManipulableChance(user, mData.EffectParam))
                    {
                        PBEStatus1 status1;
                        int val = PBERandom.RandomInt(0, 2);
                        if (val == 0)
                        {
                            status1 = PBEStatus1.Burned;
                        }
                        else if (val == 1)
                        {
                            status1 = PBEStatus1.Frozen;
                        }
                        else
                        {
                            status1 = PBEStatus1.Paralyzed;
                        }
                        ApplyStatus1IfPossible(user, target, status1, false);
                    }
                }
                BasicHit(user, targets, mData, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_MultiHit(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, byte numHits, bool subsequentMissChecks = false, PBEStatus1 status1 = PBEStatus1.None, int chanceToInflictStatus1 = 0)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEBattlePokemon target)
                {
                    if (target.HP > 0 && GetManipulableChance(user, chanceToInflictStatus1))
                    {
                        ApplyStatus1IfPossible(user, target, status1, false);
                    }
                }
                MultiHit(user, targets, mData, numHits, subsequentMissChecks: subsequentMissChecks, beforePostHit: status1 != PBEStatus1.None ? BeforePostHit : (Action<PBEBattlePokemon>)null); // Doesn't need to be its own func but neater
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_MultiHit_2To5(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            byte numHits;
            if (user.Ability == PBEAbility.SkillLink)
            {
                numHits = 5;
            }
            else
            {
                int val = PBERandom.RandomInt(0, 5);
                if (val < 2)
                {
                    numHits = 2;
                }
                else if (val < 4)
                {
                    numHits = 3;
                }
                else if (val < 5)
                {
                    numHits = 4;
                }
                else
                {
                    numHits = 5;
                }
            }
            Ef_MultiHit(user, targets, move, mData, numHits);
        }
        private void Ef_PayDay(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                PBEResult BeforeDoingDamage(PBEBattlePokemon target)
                {
                    BroadcastPayDay();
                    return PBEResult.Success;
                }
                BasicHit(user, targets, mData, beforeDoingDamage: BeforeDoingDamage);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Recoil(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, PBEStatus1 status1 = PBEStatus1.None, int chanceToInflictStatus1 = 0, PBEStatus2 status2 = PBEStatus2.None, int chanceToInflictStatus2 = 0)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int? RecoilFunc(int totalDamageDealt)
                {
                    return user.Ability == PBEAbility.RockHead || totalDamageDealt == 0 ? (int?)null : totalDamageDealt / mData.EffectParam;
                }
                void BeforePostHit(PBEBattlePokemon target)
                {
                    if (target.HP > 0)
                    {
                        if (status1 != PBEStatus1.None && GetManipulableChance(user, chanceToInflictStatus1))
                        {
                            ApplyStatus1IfPossible(user, target, status1, false);
                        }
                        if (status2 != PBEStatus2.None && GetManipulableChance(user, chanceToInflictStatus2))
                        {
                            ApplyStatus2IfPossible(user, target, status2, false);
                        }
                    }
                }
                BasicHit(user, targets, mData, recoilFunc: RecoilFunc, beforePostHit: status1 != PBEStatus1.None || status2 != PBEStatus2.None ? BeforePostHit : (Action<PBEBattlePokemon>)null);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_SecretPower(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEBattlePokemon target)
                {
                    // BUG: In Generation 5, Secret Power is unaffected by Serene Grace and the Rainbow
                    if (target.HP > 0 &&
#if !BUGFIX
                        PBERandom.RandomBool(mData.EffectParam, 100)
#else
                        GetManipulableChance(user, mData.EffectParam)
#endif
                        )
                    {
                        switch (BattleTerrain)
                        {
                            case PBEBattleTerrain.Cave: ApplyStatus2IfPossible(user, target, PBEStatus2.Flinching, false); break;
                            case PBEBattleTerrain.Grass: ApplyStatus1IfPossible(user, target, PBEStatus1.Asleep, false); break;
                            case PBEBattleTerrain.Plain: ApplyStatus1IfPossible(user, target, PBEStatus1.Paralyzed, false); break;
                            case PBEBattleTerrain.Puddle: ApplyStatChangeIfPossible(user, target, PBEStat.Speed, -1, isSecondaryEffect: true); break;
                            case PBEBattleTerrain.Sand: ApplyStatChangeIfPossible(user, target, PBEStat.Accuracy, -1, isSecondaryEffect: true); break;
                            case PBEBattleTerrain.Snow: ApplyStatus1IfPossible(user, target, PBEStatus1.Frozen, false); break;
                            case PBEBattleTerrain.Water: ApplyStatChangeIfPossible(user, target, PBEStat.Attack, -1, isSecondaryEffect: true); break;
                            default: throw new ArgumentOutOfRangeException(nameof(BattleTerrain));
                        }
                    }
                }
                BasicHit(user, targets, mData, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Selfdestruct(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            // TODO: Damp
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            // In gen 5, the user faints first (and loses if possible)
            // Due to technical limitations, we cannot faint first, but we should still make the user lose so it is the same behavior
            DealDamage(user, user, user.MaxHP, true, ignoreSturdy: true);
            TrySetLoser(user);
            if (targets.Length == 0) // You still faint if there are no targets
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                BasicHit(user, targets, mData, hitRegardlessOfUserConciousness: true);
            }
            FaintCheck(user);
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Snore(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else if (user.Status1 != PBEStatus1.Asleep)
            {
                BroadcastMoveResult(user, user, PBEResult.Ineffective_Status);
            }
            else
            {
                void BeforePostHit(PBEBattlePokemon target)
                {
                    if (target.HP > 0 && GetManipulableChance(user, mData.EffectParam))
                    {
                        ApplyStatus2IfPossible(user, target, PBEStatus2.Flinching, false);
                    }
                }
                BasicHit(user, targets, mData, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Struggle(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastStruggle(user);
            BroadcastMoveUsed(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int? RecoilFunc(int totalDamageDealt)
                {
                    return user.MaxHP / mData.EffectParam;
                }
                BasicHit(user, targets, mData, recoilFunc: RecoilFunc);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_SuckerPunch(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                PBEResult BeforeDoingDamage(PBEBattlePokemon target)
                {
                    if (target.TurnAction == null // Just switched in
                        || target.HasUsedMoveThisTurn
                        || target.TurnAction.Decision != PBETurnDecision.Fight
                        || PBEMoveData.Data[target.TurnAction.FightMove].Category == PBEMoveCategory.Status)
                    {
                        PBEResult result = PBEResult.InvalidConditions;
                        BroadcastMoveResult(user, target, result);
                        return result;
                    }
                    else
                    {
                        return PBEResult.Success;
                    }
                }
                BasicHit(user, targets, mData, beforeDoingDamage: BeforeDoingDamage);
            }
            RecordExecutedMove(user, move, mData);
        }

        private void Ef_Endeavor(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEBattlePokemon target)
                {
                    return target.HP - user.HP;
                }
                PBEResult BeforeMissCheck(PBEBattlePokemon target)
                {
                    if (target.HP <= user.HP)
                    {
                        PBEResult result = PBEResult.InvalidConditions;
                        BroadcastMoveResult(user, target, result);
                        return result;
                    }
                    return PBEResult.Success;
                }
                FixedDamageHit(user, targets, mData, DamageFunc, beforeMissCheck: BeforeMissCheck);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_FinalGambit(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int oldHP = user.HP;
                int DamageFunc(PBEBattlePokemon target)
                {
                    if (user.HP > 0)
                    {
                        DealDamage(user, user, oldHP, true);
                        FaintCheck(user);
                    }
                    return oldHP;
                }
                FixedDamageHit(user, targets, mData, DamageFunc);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_OneHitKnockout(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEBattlePokemon target)
                {
                    return target.HP;
                }
                PBEResult BeforeMissCheck(PBEBattlePokemon target)
                {
                    if (target.Level > user.Level)
                    {
                        PBEResult result = PBEResult.Ineffective_Level;
                        BroadcastMoveResult(user, target, result);
                        return result;
                    }
                    else
                    {
                        return PBEResult.Success;
                    }
                }
                void BeforeTargetsFaint()
                {
                    if (targets.Any(p => p.HP == 0))
                    {
                        BroadcastOneHitKnockout();
                    }
                }
                FixedDamageHit(user, targets, mData, DamageFunc, beforeMissCheck: BeforeMissCheck, beforeTargetsFaint: BeforeTargetsFaint);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Psywave(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEBattlePokemon target)
                {
                    return user.Level * (PBERandom.RandomInt(0, 100) + 50) / 100;
                }
                FixedDamageHit(user, targets, mData, DamageFunc);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_SeismicToss(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEBattlePokemon target)
                {
                    return user.Level;
                }
                FixedDamageHit(user, targets, mData, DamageFunc);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_SetDamage(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEBattlePokemon target)
                {
                    return mData.EffectParam;
                }
                FixedDamageHit(user, targets, mData, DamageFunc);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_SuperFang(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEBattlePokemon target)
                {
                    return target.HP / 2;
                }
                FixedDamageHit(user, targets, mData, DamageFunc);
            }
            RecordExecutedMove(user, move, mData);
        }

        private void Ef_HPDrain(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData, bool requireSleep = false)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                PBEResult BeforeDoingDamage(PBEBattlePokemon target)
                {
                    if (target.Status1 != PBEStatus1.Asleep)
                    {
                        PBEResult result = PBEResult.Ineffective_Status;
                        BroadcastMoveResult(user, target, result);
                        return result;
                    }
                    else
                    {
                        return PBEResult.Success;
                    }
                }
                void AfterPostHit(PBEBattlePokemon target, ushort damageDealt)
                {
                    if (user.HP > 0)
                    {
                        int restoreAmt = (int)(damageDealt * (mData.EffectParam / 100.0));
                        if (user.Item == PBEItem.BigRoot)
                        {
                            restoreAmt += (int)(restoreAmt * 0.3);
                        }
                        if (target.Ability == PBEAbility.LiquidOoze)
                        {
                            DealDamage(target, user, restoreAmt, true, ignoreSturdy: true); // Verified: It does ignore Sturdy
                            BroadcastAbility(target, user, PBEAbility.LiquidOoze, PBEAbilityAction.Damage);
                            FaintCheck(user);
                        }
                        else
                        {
                            if (HealDamage(user, restoreAmt) > 0)
                            {
                                BroadcastHPDrained(target);
                            }
                        }
                    }
                }
                BasicHit(user, targets, mData, beforeDoingDamage: requireSleep ? BeforeDoingDamage : (Func<PBEBattlePokemon, PBEResult>)null, afterPostHit: AfterPostHit);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Moonlight(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int denominator;
                if (ShouldDoWeatherEffects())
                {
                    switch (Weather)
                    {
                        case PBEWeather.None: denominator = 2; break;
                        case PBEWeather.HarshSunlight: denominator = 3; break;
                        default: denominator = 4; break;
                    }
                }
                else
                {
                    denominator = 2;
                }
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        ushort amtRestored = HealDamage(user, user.MaxHP / denominator);
                        if (amtRestored == 0)
                        {
                            BroadcastMoveResult(user, user, PBEResult.Ineffective_Stat);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_PainSplit(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        if (target.Status2.HasFlag(PBEStatus2.Substitute))
                        {
                            BroadcastMoveResult(user, target, PBEResult.Ineffective_Substitute);
                        }
                        else
                        {
                            int total = user.HP + target.HP;
                            int hp = total / 2;
                            foreach (PBEBattlePokemon pkmn in new PBEBattlePokemon[] { user, target })
                            {
                                if (hp > pkmn.HP)
                                {
                                    HealDamage(pkmn, hp - pkmn.HP);
                                }
                                else if (hp < pkmn.HP)
                                {
                                    DealDamage(user, pkmn, pkmn.HP - hp, true);
                                }
                            }
                            BroadcastPainSplit(user, target);
                            LowHPBerryCheck(user);
                            LowHPBerryCheck(target, user); // Verified: Berry is activated but no illusion breaking
                            DoPostAttackedTargetEffects(target); // Verified: Color Change is ignored
                        }
                    }
                }
                //DoPostAttackedUserEffects(user, false); // Do we need this? Life Orb doesn't activate.
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Rest(PBEBattlePokemon user, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (user.Status1 == PBEStatus1.Asleep)
            {
                BroadcastMoveResult(user, user, PBEResult.Ineffective_Status);
            }
            else if (user.HP == user.MaxHP)
            {
                BroadcastMoveResult(user, user, PBEResult.Ineffective_Stat);
            }
            else
            {
                PBEResult result = user.IsSleepPossible(null, ignoreSubstitute: true, ignoreCurrentStatus: true, ignoreSafeguard: true);
                if (result == PBEResult.Ineffective_Ability)
                {
                    BroadcastAbility(user, user, user.Ability, PBEAbilityAction.PreventedStatus);
                }
                if (result != PBEResult.Success)
                {
                    BroadcastMoveResult(user, user, result);
                }
                else
                {
                    user.Status1 = PBEStatus1.Asleep;
                    SetSleepTurns(user, Settings.SleepMaxTurns, Settings.SleepMaxTurns); // Not a typo; Rest always sleeps for max turns
                    user.Status1Counter = 0;
                    BroadcastStatus1(user, user, PBEStatus1.Asleep, PBEStatusAction.Added);
                    HealDamage(user, user.MaxHP);
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_RestoreTargetHP(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        if (user != target && target.Status2.HasFlag(PBEStatus2.Substitute))
                        {
                            BroadcastMoveResult(user, target, PBEResult.Ineffective_Substitute);
                        }
                        else
                        {
                            ushort amtRestored = HealDamage(target, (int)(target.MaxHP * (mData.EffectParam / 100.0)));
                            if (amtRestored == 0)
                            {
                                BroadcastMoveResult(user, target, PBEResult.Ineffective_Stat);
                            }
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Roost(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        if (target.Status2.HasFlag(PBEStatus2.Roost))
                        {
                            BroadcastMoveResult(user, target, PBEResult.Ineffective_Status);
                        }
                        else
                        {
                            ushort amtRestored = HealDamage(target, (int)(target.MaxHP * (mData.EffectParam / 100.0)));
                            if (amtRestored == 0)
                            {
                                BroadcastMoveResult(user, target, PBEResult.Ineffective_Stat);
                            }
                            else
                            {
                                target.StartRoost();
                                BroadcastStatus2(target, user, PBEStatus2.Roost, PBEStatusAction.Added);
                            }
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }

        private void Ef_BellyDrum(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        int requirement = target.MaxHP / 2;
                        // BUG: The games do not check if the target has Contrary
#if !BUGFIX
                        if (target.HP <= requirement || target.AttackChange == Settings.MaxStatChange)
#else
                        if (target.HP <= requirement || target.IsStatChangePossible(PBEStat.Attack, user, byte.MaxValue, out sbyte oldValue, out sbyte newValue) != PBEResult.Success)
#endif
                        {
                            BroadcastMoveResult(user, target, PBEResult.InvalidConditions);
                        }
                        else
                        {
                            DealDamage(user, target, requirement, true);
#if !BUGFIX
                            ApplyStatChangeIfPossible(user, target, PBEStat.Attack, byte.MaxValue); // byte.MaxValue will work for all PBESettings
#else
                            SetStatAndBroadcast(target, PBEStat.Attack, oldValue, newValue);
#endif
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Camouflage(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        PBEType type;
                        switch (BattleTerrain)
                        {
                            case PBEBattleTerrain.Cave: type = PBEType.Rock; break;
                            case PBEBattleTerrain.Grass: type = PBEType.Grass; break;
                            case PBEBattleTerrain.Plain: type = PBEType.Normal; break;
                            case PBEBattleTerrain.Puddle: type = PBEType.Ground; break;
                            case PBEBattleTerrain.Sand: type = PBEType.Ground; break;
                            case PBEBattleTerrain.Snow: type = PBEType.Ice; break;
                            case PBEBattleTerrain.Water: type = PBEType.Water; break;
                            default: throw new ArgumentOutOfRangeException(nameof(BattleTerrain));
                        }
                        // Verified: Works on dual-type, fails on single-type
                        if (target.Type1 == type && target.Type2 == PBEType.None)
                        {
                            BroadcastMoveResult(user, target, PBEResult.InvalidConditions);
                        }
                        else
                        {
                            BroadcastTypeChanged(target, type, PBEType.None);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Conversion(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        PBEBattleMoveset moves = target.Moves;
                        int count = moves.Count;
                        var available = new List<PBEType>(count);
                        for (int i = 0; i < count; i++)
                        {
                            PBEMove m = moves[i].Move;
                            if (m != PBEMove.None && m != move)
                            {
                                PBEType type = PBEMoveData.Data[m].Type;
                                if (!target.HasType(type))
                                {
                                    available.Add(type);
                                }
                            }
                        }
                        if (available.Count == 0)
                        {
                            BroadcastMoveResult(user, target, PBEResult.InvalidConditions);
                        }
                        else
                        {
                            BroadcastTypeChanged(target, available.RandomElement(), PBEType.None);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Curse(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                if (user.HasType(PBEType.Ghost))
                {
                    if (targets.Length == 1 && targets[0] == user) // Just gained the Ghost type after selecting the move, so get a random target
                    {
                        PBEFieldPosition prioritizedPos = GetPositionAcross(BattleFormat, user.FieldPosition);
                        PBETurnTarget moveTarget = prioritizedPos == PBEFieldPosition.Left ? PBETurnTarget.FoeLeft : prioritizedPos == PBEFieldPosition.Center ? PBETurnTarget.FoeCenter : PBETurnTarget.FoeRight;
                        targets = GetRuntimeTargets(user, moveTarget, false);
                    }
                    if (targets.Length == 0)
                    {
                        BroadcastMoveResult(user, user, PBEResult.NoTarget);
                    }
                    else
                    {
                        foreach (PBEBattlePokemon target in targets)
                        {
                            if (!MissCheck(user, target, mData))
                            {
                                ApplyStatus2IfPossible(user, target, PBEStatus2.Cursed, true);
                            }
                        }
                    }
                }
                else
                {
                    foreach (PBEBattlePokemon target in targets)
                    {
                        if (target.SpeedChange == -Settings.MaxStatChange
                            && target.AttackChange == Settings.MaxStatChange
                            && target.DefenseChange == Settings.MaxStatChange)
                        {
                            BroadcastMoveResult(user, target, PBEResult.Ineffective_Stat);
                        }
                        else
                        {
                            ApplyStatChangeIfPossible(user, target, PBEStat.Speed, -1);
                            ApplyStatChangeIfPossible(user, target, PBEStat.Attack, +1);
                            ApplyStatChangeIfPossible(user, target, PBEStat.Defense, +1);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Flatter(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        ApplyStatChangeIfPossible(user, target, PBEStat.SpAttack, +1);
                        ApplyStatus2IfPossible(user, target, PBEStatus2.Confused, true);
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Haze(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PBEBattlePokemon pkmn in targets)
            {
                pkmn.ClearStatChanges();
            }
            BroadcastHaze();
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_HelpingHand(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    // TODO: When triple battle shifting happens, all moves that can target allies but not the user will have to check if the user targetted itself due to shifting.
                    // For now, I'll put this check here, because this is the only move that will attempt to target the user when the move cannot normally do so (single/rotation battle).
                    if (target == user)
                    {
                        BroadcastMoveResult(user, user, PBEResult.NoTarget);
                    }
                    else
                    {
                        ApplyStatus2IfPossible(user, target, PBEStatus2.HelpingHand, true); // No MissCheck because should be able to hit through protect, shadowforce, etc
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Metronome(PBEBattlePokemon user, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            // Record before the called move is recorded
            RecordExecutedMove(user, move, mData);

            PBEMove calledMove = PBEDataUtils.MetronomeMoves.RandomElement();
            _calledFromOtherMove = true;
            UseMove(user, calledMove, GetRandomTargetForMetronome(user, calledMove));
            _calledFromOtherMove = false;
        }
        private void Ef_Nothing(PBEBattlePokemon user, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            BroadcastNothingHappened();
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_PsychUp(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        BroadcastPsychUp(user, target);
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_ReflectType(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        // Fail if pure flying-type roosts
                        if (target.Type1 == PBEType.None && target.Type2 == PBEType.None)
                        {
                            BroadcastMoveResult(user, target, PBEResult.InvalidConditions);
                        }
                        else
                        {
                            BroadcastReflectType(user, target);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Refresh(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        PBEStatus1 status1 = target.Status1;
                        if (status1 == PBEStatus1.None || status1 == PBEStatus1.Asleep || status1 == PBEStatus1.Frozen)
                        {
                            BroadcastMoveResult(user, target, PBEResult.InvalidConditions);
                        }
                        else
                        {
                            target.Status1 = PBEStatus1.None;
                            target.Status1Counter = 0;
                            BroadcastStatus1(target, user, status1, PBEStatusAction.Cleared);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Soak(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        BroadcastTypeChanged(target, PBEType.Water, PBEType.None);
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Swagger(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        ApplyStatChangeIfPossible(user, target, PBEStat.Attack, +2);
                        ApplyStatus2IfPossible(user, target, PBEStatus2.Confused, true);
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Teleport(PBEBattlePokemon user, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            BroadcastMoveResult(user, user, PBEResult.InvalidConditions);
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Whirlwind(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, PBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEBattlePokemon target in targets)
                {
                    if (!MissCheck(user, target, mData))
                    {
                        PBEBattlePokemon[] possibleSwitcheroonies = target.Team.Party.Where(p => p.FieldPosition == PBEFieldPosition.None && p.HP > 0).ToArray();
                        if (possibleSwitcheroonies.Length == 0)
                        {
                            BroadcastMoveResult(user, target, PBEResult.InvalidConditions);
                        }
                        else
                        {
                            SwitchTwoPokemon(target, possibleSwitcheroonies.RandomElement(), user);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
    }
}
