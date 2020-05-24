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
        private static readonly PBEStat[] _moodyStats = new PBEStat[] { PBEStat.Attack, PBEStat.Defense, PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed, PBEStat.Accuracy, PBEStat.Evasion };
        private static readonly PBEStat[] _starfBerryStats = new PBEStat[] { PBEStat.Attack, PBEStat.Defense, PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed };

        private bool _calledFromOtherMove = false;

        private void DoSwitchInEffects(IEnumerable<PBEPokemon> battlers, PBEPokemon forcedInBy = null)
        {
            PBEPokemon[] order = GetActingOrder(battlers, true);

            foreach (PBEPokemon pkmn in order)
            {
                bool grounded = pkmn.IsGrounded(forcedInBy) == PBEResult.Success;
                // Verified: (Spikes/StealthRock/ToxicSpikes in the order they were applied) before ability
                if (grounded && pkmn.Team.TeamStatus.HasFlag(PBETeamStatus.Spikes))
                {
                    BroadcastTeamStatus(pkmn.Team, PBETeamStatus.Spikes, PBETeamStatusAction.Damage, pkmn);
                    DealDamage(pkmn, pkmn, (int)(pkmn.MaxHP / (10.0 - (2 * pkmn.Team.SpikeCount))), true, ignoreSturdy: true);
                    if (FaintCheck(pkmn))
                    {
                        continue;
                    }
                    LowHPBerryCheck(pkmn, forcedInBy);
                }
                if (pkmn.Team.TeamStatus.HasFlag(PBETeamStatus.StealthRock))
                {
                    BroadcastTeamStatus(pkmn.Team, PBETeamStatus.StealthRock, PBETeamStatusAction.Damage, pkmn);
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
                        pkmn.Team.TeamStatus &= ~PBETeamStatus.ToxicSpikes;
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
        private void DoPostHitEffects(PBEPokemon user, PBEPokemon victim, PBEMove move, PBEType moveType)
        {
            PBEMoveData mData = PBEMoveData.Data[move];

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
                    if (user.HP > 0 && victim.Ability == PBEAbility.CuteCharm && user.IsAttractionPossible(victim) == PBEResult.Success && PBERandom.RandomBool(30, 100))
                    {
                        BroadcastAbility(victim, user, PBEAbility.CuteCharm, PBEAbilityAction.ChangedStatus);
                        CauseInfatuation(user, victim);
                    }
                    if (user.HP > 0 && victim.Ability == PBEAbility.EffectSpore && user.Status1 == PBEStatus1.None)
                    {
                        // Spaghetti code taken from the assembly in generation 5 games
                        int randomNum = PBERandom.RandomInt(0, 99);
                        if (randomNum < 30)
                        {
                            PBEStatus1 status = PBEStatus1.None;
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
                    if (user.HP > 0 && victim.Ability == PBEAbility.FlameBody && user.IsBurnPossible(victim) == PBEResult.Success && PBERandom.RandomBool(30, 100))
                    {
                        BroadcastAbility(victim, user, PBEAbility.FlameBody, PBEAbilityAction.ChangedStatus);
                        user.Status1 = PBEStatus1.Burned;
                        BroadcastStatus1(user, victim, PBEStatus1.Burned, PBEStatusAction.Added);
                        AntiStatusAbilityCheck(user);
                    }
                    if (user.HP > 0 && victim.Ability == PBEAbility.PoisonPoint && user.IsPoisonPossible(victim) == PBEResult.Success && PBERandom.RandomBool(30, 100))
                    {
                        BroadcastAbility(victim, user, PBEAbility.PoisonPoint, PBEAbilityAction.ChangedStatus);
                        user.Status1 = PBEStatus1.Poisoned;
                        BroadcastStatus1(user, victim, PBEStatus1.Poisoned, PBEStatusAction.Added);
                        AntiStatusAbilityCheck(user);
                    }
                    if (user.HP > 0 && victim.Ability == PBEAbility.Static && user.IsParalysisPossible(victim) == PBEResult.Success && PBERandom.RandomBool(30, 100))
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
        private void DoPostAttackedTargetEffects(PBEPokemon victim, PBEType colorChangeType = PBEType.None)
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
        private void DoPostAttackedUserEffects(PBEPokemon user, bool doLifeOrb, int? recoilDamage = null)
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
            PBEPokemon[] order = GetActingOrder(ActiveBattlers, true);
            // Verified: Weather stops before doing damage
            // Verified: Hailstorm/Sandstorm/IceBody/RainDish/SolarPower before all
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
                else if (ShouldDoWeatherEffects())
                {
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
            }

            // Verified: Healer/ShedSkin/BlackSludge/Leftovers before LeechSeed
            foreach (PBEPokemon pkmn in order)
            {
                if (pkmn.HP > 0)
                {
                    switch (pkmn.Ability)
                    {
                        case PBEAbility.Healer:
                        {
                            foreach (PBEPokemon ally in GetRuntimeSurrounding(pkmn, true, false))
                            {
                                if (ally.Status1 != PBEStatus1.None && PBERandom.RandomBool(30, 100))
                                {
                                    BroadcastAbility(pkmn, ally, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                                    PBEStatus1 status1 = ally.Status1;
                                    ally.Status1 = PBEStatus1.None;
                                    BroadcastStatus1(ally, pkmn, status1, PBEStatusAction.Cured);
                                }
                            }
                            break;
                        }
                        case PBEAbility.ShedSkin:
                        {
                            if (pkmn.Status1 != PBEStatus1.None && PBERandom.RandomBool(30, 100))
                            {
                                BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                                PBEStatus1 status1 = pkmn.Status1;
                                pkmn.Status1 = PBEStatus1.None;
                                BroadcastStatus1(pkmn, pkmn, status1, PBEStatusAction.Cured);
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
            foreach (PBEPokemon pkmn in order)
            {
                if (pkmn.HP > 0 && pkmn.Status2.HasFlag(PBEStatus2.LeechSeed))
                {
                    PBEPokemon sucker = pkmn.SeededTeam.TryGetPokemon(pkmn.SeededPosition);
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

            // Verified: Curse before MagnetRise
            foreach (PBEPokemon pkmn in order)
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

            // Verified: MagnetRise before Moody/SlowStart/SpeedBoost
            foreach (PBEPokemon pkmn in order)
            {
                if (pkmn.HP > 0 && pkmn.Status2.HasFlag(PBEStatus2.MagnetRise) && pkmn.MagnetRiseTurns > 0)
                {
                    pkmn.MagnetRiseTurns--;
                    if (pkmn.MagnetRiseTurns == 0)
                    {
                        pkmn.Status2 &= ~PBEStatus2.MagnetRise;
                        BroadcastStatus2(pkmn, pkmn, PBEStatus2.MagnetRise, PBEStatusAction.Ended);
                    }
                }
            }

            // Verified: BadDreams/Moody/SlowStart/SpeedBoost before Orbs, but activate together
            foreach (PBEPokemon pkmn in order)
            {
                if (pkmn.HP > 0)
                {
                    // Ability before Orb
                    switch (pkmn.Ability)
                    {
                        case PBEAbility.BadDreams:
                        {
                            foreach (PBEPokemon victim in GetRuntimeSurrounding(pkmn, false, true).Where(p => p.Status1 == PBEStatus1.Asleep))
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
                            PBEStat[] statsThatCanGoUp = _moodyStats.Where(s => pkmn.GetStatChange(s) < Settings.MaxStatChange).ToArray();
                            PBEStat? upStat = statsThatCanGoUp.Length == 0 ? (PBEStat?)null : statsThatCanGoUp.RandomElement();
                            var statsThatCanGoDown = _moodyStats.Where(s => pkmn.GetStatChange(s) > -Settings.MaxStatChange).ToList();
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
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            // If HP is needed to be above 0, use HPPercentage so clients can continue to use this
            // However, I see no instance of this getting called where an ActiveBattler has 0 hp
            return !ActiveBattlers.Any(p => p.Ability == PBEAbility.AirLock || p.Ability == PBEAbility.CloudNine);
        }
        public bool WillLeafGuardActivate()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            return ShouldDoWeatherEffects() && Weather == PBEWeather.HarshSunlight;
        }

        private void UseMove(PBEPokemon user, PBEMove move, PBETurnTarget requestedTargets)
        {
            if (!_calledFromOtherMove && PreMoveStatusCheck(user, move))
            {
                if (user.Status2.HasFlag(PBEStatus2.Airborne))
                {
                    BroadcastMoveLock(user, PBEMove.None, PBETurnTarget.None, PBEMoveLockType.Temporary);
                    user.Status2 &= ~PBEStatus2.Airborne;
                    BroadcastStatus2(user, user, PBEStatus2.Airborne, PBEStatusAction.Ended);
                }
                if (user.Status2.HasFlag(PBEStatus2.Underground))
                {
                    BroadcastMoveLock(user, PBEMove.None, PBETurnTarget.None, PBEMoveLockType.Temporary);
                    user.Status2 &= ~PBEStatus2.Underground;
                    BroadcastStatus2(user, user, PBEStatus2.Underground, PBEStatusAction.Ended);
                }
                if (user.Status2.HasFlag(PBEStatus2.Underwater))
                {
                    BroadcastMoveLock(user, PBEMove.None, PBETurnTarget.None, PBEMoveLockType.Temporary);
                    user.Status2 &= ~PBEStatus2.Underwater;
                    BroadcastStatus2(user, user, PBEStatus2.Underwater, PBEStatusAction.Ended);
                }
                return;
            }
            else
            {
                PBEPokemon[] targets = GetRuntimeTargets(user, requestedTargets, user.GetMoveTargets(move) == PBEMoveTarget.SingleNotSelf);
                PBEMoveData mData = PBEMoveData.Data[move];
                PBEMoveEffect effect = mData.Effect;
                int effectParam = mData.EffectParam;
                switch (effect)
                {
                    case PBEMoveEffect.Attract:
                    {
                        Ef_TryForceStatus2(user, targets, move, PBEStatus2.Infatuated);
                        break;
                    }
                    case PBEMoveEffect.BellyDrum:
                    {
                        Ef_BellyDrum(user, targets, move);
                        break;
                    }
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
                    case PBEMoveEffect.Camouflage:
                    {
                        Ef_Camouflage(user, targets, move);
                        break;
                    }
                    case PBEMoveEffect.ChangeTarget_ACC:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Accuracy }, new int[] { effectParam });
                        break;
                    }
                    case PBEMoveEffect.ChangeTarget_ATK:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack }, new int[] { effectParam });
                        break;
                    }
                    case PBEMoveEffect.ChangeTarget_DEF:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Defense }, new int[] { effectParam });
                        break;
                    }
                    case PBEMoveEffect.ChangeTarget_EVA:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Evasion }, new int[] { effectParam });
                        break;
                    }
                    case PBEMoveEffect.ChangeTarget_SPATK:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack }, new int[] { effectParam });
                        break;
                    }
                    case PBEMoveEffect.ChangeTarget_SPATK__IfAttractionPossible:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack }, new int[] { effectParam }, requireAttraction: true);
                        break;
                    }
                    case PBEMoveEffect.ChangeTarget_SPDEF:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpDefense }, new int[] { effectParam });
                        break;
                    }
                    case PBEMoveEffect.ChangeTarget_SPE:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Speed }, new int[] { effectParam });
                        break;
                    }
                    case PBEMoveEffect.Confuse:
                    {
                        Ef_TryForceStatus2(user, targets, move, PBEStatus2.Confused);
                        break;
                    }
                    case PBEMoveEffect.Conversion:
                    {
                        Ef_Conversion(user, targets, move);
                        break;
                    }
                    case PBEMoveEffect.Curse:
                    {
                        Ef_Curse(user, targets, move);
                        break;
                    }
                    case PBEMoveEffect.Dig:
                    {
                        SemiInvulnerableChargeMove(user, targets, move, requestedTargets, PBEStatus2.Underground);
                        break;
                    }
                    case PBEMoveEffect.Dive:
                    {
                        SemiInvulnerableChargeMove(user, targets, move, requestedTargets, PBEStatus2.Underwater);
                        break;
                    }
                    case PBEMoveEffect.Endeavor:
                    {
                        Ef_Endeavor(user, targets, move);
                        break;
                    }
                    case PBEMoveEffect.Entrainment:
                    {
                        Ef_Entrainment(user, targets, move);
                        break;
                    }
                    case PBEMoveEffect.FinalGambit:
                    {
                        Ef_FinalGambit(user, targets, move);
                        break;
                    }
                    case PBEMoveEffect.Flatter:
                    {
                        Ef_Flatter(user, targets, move);
                        break;
                    }
                    case PBEMoveEffect.Fly:
                    {
                        SemiInvulnerableChargeMove(user, targets, move, requestedTargets, PBEStatus2.Airborne);
                        break;
                    }
                    case PBEMoveEffect.FocusEnergy:
                    {
                        Ef_TryForceStatus2(user, targets, move, PBEStatus2.Pumped);
                        break;
                    }
                    case PBEMoveEffect.GastroAcid:
                    {
                        Ef_SetOtherAbility(user, targets, move, PBEAbility.None, false);
                        break;
                    }
                    case PBEMoveEffect.Growth:
                    {
                        int change = WillLeafGuardActivate() ? +2 : +1;
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.SpAttack }, new int[] { change, change });
                        break;
                    }
                    case PBEMoveEffect.Hail:
                    {
                        Ef_TryForceWeather(user, move, PBEWeather.Hailstorm);
                        break;
                    }
                    case PBEMoveEffect.Haze:
                    {
                        Ef_Haze(user, move);
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
                    case PBEMoveEffect.Hit__2Times:
                    {
                        Ef_MultiHit(user, targets, move, 2);
                        break;
                    }
                    case PBEMoveEffect.Hit__2Times__MaybePoison:
                    {
                        Ef_MultiHit(user, targets, move, 2, status1: PBEStatus1.Poisoned, chanceToInflictStatus1: effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__2To5Times:
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
                        Ef_MultiHit(user, targets, move, numHits);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeBurn:
                    {
                        Ef_Hit(user, targets, move, status1: PBEStatus1.Burned, chanceToInflictStatus1: effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeBurn__10PercentFlinch:
                    {
                        Ef_Hit(user, targets, move, status1: PBEStatus1.Burned, chanceToInflictStatus1: effectParam, status2: PBEStatus2.Flinching, chanceToInflictStatus2: 10);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeConfuse:
                    {
                        Ef_Hit(user, targets, move, status2: PBEStatus2.Confused, chanceToInflictStatus2: effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeFlinch:
                    {
                        Ef_Hit(user, targets, move, status2: PBEStatus2.Flinching, chanceToInflictStatus2: effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeFreeze:
                    {
                        Ef_Hit(user, targets, move, status1: PBEStatus1.Frozen, chanceToInflictStatus1: effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeFreeze__10PercentFlinch:
                    {
                        Ef_Hit(user, targets, move, status1: PBEStatus1.Frozen, chanceToInflictStatus1: effectParam, status2: PBEStatus2.Flinching, chanceToInflictStatus2: 10);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1:
                    {
                        Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Accuracy }, new int[] { -1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_ATK_By1:
                    {
                        Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack }, new int[] { -1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1:
                    {
                        Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Defense }, new int[] { -1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1:
                    {
                        Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack }, new int[] { -1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1:
                    {
                        Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpDefense }, new int[] { -1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2:
                    {
                        Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpDefense }, new int[] { -2 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1:
                    {
                        Ef_Hit__MaybeChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Speed }, new int[] { -1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeParalyze:
                    {
                        Ef_Hit(user, targets, move, status1: PBEStatus1.Paralyzed, chanceToInflictStatus1: effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeParalyze__10PercentFlinch:
                    {
                        Ef_Hit(user, targets, move, status1: PBEStatus1.Paralyzed, chanceToInflictStatus1: effectParam, status2: PBEStatus2.Flinching, chanceToInflictStatus2: 10);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybePoison:
                    {
                        Ef_Hit(user, targets, move, status1: PBEStatus1.Poisoned, chanceToInflictStatus1: effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeLowerUser_ATK_DEF_By1:
                    {
                        Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense }, new int[] { -1, -1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1:
                    {
                        Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Defense, PBEStat.SpDefense }, new int[] { -1, -1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2:
                    {
                        Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack }, new int[] { -2 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeLowerUser_SPE_By1:
                    {
                        Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Speed }, new int[] { -1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1:
                    {
                        Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Speed, PBEStat.Defense, PBEStat.SpDefense }, new int[] { -1, -1, -1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeRaiseUser_ATK_By1:
                    {
                        Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Attack }, new int[] { +1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1:
                    {
                        Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense, PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed }, new int[] { +1, +1, +1, +1, +1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeRaiseUser_DEF_By1:
                    {
                        Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Defense }, new int[] { +1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeRaiseUser_SPATK_By1:
                    {
                        Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack }, new int[] { +1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeRaiseUser_SPE_By1:
                    {
                        Ef_Hit__MaybeChangeUserStats(user, targets, move, new PBEStat[] { PBEStat.Speed }, new int[] { +1 }, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Hit__MaybeToxic:
                    {
                        Ef_Hit(user, targets, move, status1: PBEStatus1.BadlyPoisoned, chanceToInflictStatus1: effectParam);
                        break;
                    }
                    case PBEMoveEffect.HPDrain:
                    {
                        Ef_HPDrain(user, targets, move, effectParam);
                        break;
                    }
                    case PBEMoveEffect.HPDrain__RequireSleep:
                    {
                        Ef_HPDrain(user, targets, move, effectParam, requireSleep: true);
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
                    case PBEMoveEffect.LockOn:
                    {
                        Ef_TryForceStatus2(user, targets, move, PBEStatus2.LockOn);
                        break;
                    }
                    case PBEMoveEffect.LowerTarget_ATK_DEF_By1:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense }, new int[] { -1, -1 });
                        break;
                    }
                    case PBEMoveEffect.LowerTarget_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Defense, PBEStat.SpDefense, PBEStat.Attack, PBEStat.SpAttack, PBEStat.Speed }, new int[] { -1, -1, +2, +2, +2 });
                        break;
                    }
                    case PBEMoveEffect.LuckyChant:
                    {
                        Ef_TryForceTeamStatus(user, move, PBETeamStatus.LuckyChant);
                        break;
                    }
                    case PBEMoveEffect.MagnetRise:
                    {
                        Ef_TryForceStatus2(user, targets, move, PBEStatus2.MagnetRise);
                        break;
                    }
                    case PBEMoveEffect.Metronome:
                    {
                        Ef_Metronome(user, move);
                        break;
                    }
                    case PBEMoveEffect.MiracleEye:
                    {
                        Ef_TryForceStatus2(user, targets, move, PBEStatus2.MiracleEye);
                        break;
                    }
                    case PBEMoveEffect.Moonlight:
                    {
                        Ef_Moonlight(user, move);
                        break;
                    }
                    case PBEMoveEffect.Nothing:
                    {
                        Ef_Nothing(user, move);
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
                    case PBEMoveEffect.PowerTrick:
                    {
                        Ef_TryForceStatus2(user, targets, move, PBEStatus2.PowerTrick);
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
                    case PBEMoveEffect.RaiseTarget_ATK_ACC_By1:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Accuracy }, new int[] { +1, +1 });
                        break;
                    }
                    case PBEMoveEffect.RaiseTarget_ATK_DEF_By1:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense }, new int[] { +1, +1 });
                        break;
                    }
                    case PBEMoveEffect.RaiseTarget_ATK_DEF_ACC_By1:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Defense, PBEStat.Accuracy }, new int[] { +1, +1, +1 });
                        break;
                    }
                    case PBEMoveEffect.RaiseTarget_ATK_SPATK_By1:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.SpAttack }, new int[] { +1, +1 });
                        break;
                    }
                    case PBEMoveEffect.RaiseTarget_ATK_SPE_By1:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Attack, PBEStat.Speed }, new int[] { +1, +1 });
                        break;
                    }
                    case PBEMoveEffect.RaiseTarget_DEF_SPDEF_By1:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Defense, PBEStat.SpDefense }, new int[] { +1, +1 });
                        break;
                    }
                    case PBEMoveEffect.RaiseTarget_SPATK_SPDEF_By1:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack, PBEStat.SpDefense }, new int[] { +1, +1 });
                        break;
                    }
                    case PBEMoveEffect.RaiseTarget_SPATK_SPDEF_SPE_By1:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.SpAttack, PBEStat.SpDefense, PBEStat.Speed }, new int[] { +1, +1, +1 });
                        break;
                    }
                    case PBEMoveEffect.RaiseTarget_SPE_By2_ATK_By1:
                    {
                        Ef_ChangeTargetStats(user, targets, move, new PBEStat[] { PBEStat.Speed, PBEStat.Attack }, new int[] { +2, +1 });
                        break;
                    }
                    case PBEMoveEffect.Recoil:
                    {
                        Ef_Recoil(user, targets, move, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Recoil__10PercentBurn:
                    {
                        Ef_Recoil(user, targets, move, effectParam, status1: PBEStatus1.Burned, chanceToInflictStatus1: 10);
                        break;
                    }
                    case PBEMoveEffect.Recoil__10PercentParalyze:
                    {
                        Ef_Recoil(user, targets, move, effectParam, status1: PBEStatus1.Paralyzed, chanceToInflictStatus1: 10);
                        break;
                    }
                    case PBEMoveEffect.Reflect:
                    {
                        Ef_TryForceTeamStatus(user, move, PBETeamStatus.Reflect);
                        break;
                    }
                    case PBEMoveEffect.Rest:
                    {
                        Ef_Rest(user, move);
                        break;
                    }
                    case PBEMoveEffect.RestoreTargetHP:
                    {
                        Ef_RestoreTargetHP(user, targets, move, effectParam);
                        break;
                    }
                    case PBEMoveEffect.RolePlay:
                    {
                        Ef_RolePlay(user, targets, move);
                        break;
                    }
                    case PBEMoveEffect.Safeguard:
                    {
                        Ef_TryForceTeamStatus(user, move, PBETeamStatus.Safeguard);
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
                        Ef_SetDamage(user, targets, move, effectParam);
                        break;
                    }
                    case PBEMoveEffect.SimpleBeam:
                    {
                        Ef_SetOtherAbility(user, targets, move, PBEAbility.Simple, true);
                        break;
                    }
                    case PBEMoveEffect.Sleep:
                    {
                        Ef_TryForceStatus1(user, targets, move, PBEStatus1.Asleep);
                        break;
                    }
                    case PBEMoveEffect.Snore:
                    {
                        Ef_Snore(user, targets, move, effectParam);
                        break;
                    }
                    case PBEMoveEffect.Soak:
                    {
                        Ef_Soak(user, targets, move);
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
                    case PBEMoveEffect.SuckerPunch:
                    {
                        Ef_SuckerPunch(user, targets, move);
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
                    case PBEMoveEffect.Tailwind:
                    {
                        Ef_TryForceTeamStatus(user, move, PBETeamStatus.Tailwind);
                        break;
                    }
                    case PBEMoveEffect.Teleport:
                    {
                        Ef_Teleport(user, move);
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
                    case PBEMoveEffect.Whirlwind:
                    {
                        Ef_Whirlwind(user, targets, move);
                        break;
                    }
                    case PBEMoveEffect.WideGuard:
                    {
                        Ef_TryForceTeamStatus(user, move, PBETeamStatus.WideGuard);
                        break;
                    }
                    case PBEMoveEffect.WorrySeed:
                    {
                        Ef_SetOtherAbility(user, targets, move, PBEAbility.Insomnia, true);
                        break;
                    }
                    default: throw new ArgumentOutOfRangeException(nameof(effect));
                }
            }
        }

        private bool PreMoveStatusCheck(PBEPokemon user, PBEMove move)
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
                    user.Status2 &= ~PBEStatus2.Confused;
                    user.ConfusionCounter = 0;
                    user.ConfusionTurns = 0;
                    BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Ended);
                }
                else
                {
                    BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Activated);
                    if (PBERandom.RandomBool(50, 100))
                    {
                        BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Damage);
                        ushort damage = CalculateDamage(user, user, PBEMove.None, PBEType.None, PBEMoveCategory.Physical, 40, false);
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
                BroadcastStatus2(user, user.InfatuatedWithPokemon, PBEStatus2.Infatuated, PBEStatusAction.Activated);
                if (PBERandom.RandomBool(50, 100))
                {
                    BroadcastStatus2(user, user.InfatuatedWithPokemon, PBEStatus2.Infatuated, PBEStatusAction.CausedImmobility);
                    return true;
                }
            }
            return false;
        }
        private bool MissCheck(PBEPokemon user, PBEPokemon target, PBEMove move)
        {
            if (user == target)
            {
                return false;
            }
            PBEMoveData mData = PBEMoveData.Data[move];

            if (target.Status2.HasFlag(PBEStatus2.Protected) && mData.Flags.HasFlag(PBEMoveFlag.AffectedByProtect))
            {
                BroadcastStatus2(target, user, PBEStatus2.Protected, PBEStatusAction.Activated);
                return true;
            }
            if (target.Team.TeamStatus.HasFlag(PBETeamStatus.WideGuard) && mData.Category != PBEMoveCategory.Status && PBEMoveData.IsSpreadMove(user.GetMoveTargets(move)))
            {
                BroadcastTeamStatus(target.Team, PBETeamStatus.WideGuard, PBETeamStatusAction.Damage, target);
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
                    if (ShouldDoWeatherEffects() && Weather == PBEWeather.Hailstorm)
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
                    chance = user.Level - target.Level + 30;
                    if (chance < 1)
                    {
                        goto miss;
                    }
                    else
                    {
                        goto roll; // Skip all modifiers
                    }
                }
                case PBEMove.Hurricane:
                case PBEMove.Thunder:
                {
                    bool doWeather = ShouldDoWeatherEffects();
                    if (doWeather && Weather == PBEWeather.Rain)
                    {
                        return false;
                    }
                    else if (doWeather && Weather == PBEWeather.HarshSunlight)
                    {
                        chance = 50;
                    }
                    else
                    {
                        chance = mData.Accuracy;
                    }
                    break;
                }
                default:
                {
                    if (target.Ability == PBEAbility.WonderSkin && mData.Category == PBEMoveCategory.Status && !user.HasCancellingAbility())
                    {
                        chance = 50;
                    }
                    else
                    {
                        chance = mData.Accuracy;
                    }
                    break;
                }
            }
            if (chance == 0)
            {
                return false;
            }
            double accuracy = target.Ability == PBEAbility.Unaware ? 1 : GetStatChangeModifier(user.AccuracyChange, true);
            double evasion = user.Ability == PBEAbility.Unaware ? 1 : GetStatChangeModifier(target.Status2.HasFlag(PBEStatus2.MiracleEye) ? Math.Min((sbyte)0, target.EvasionChange) : target.EvasionChange, true);
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
        private bool AttackTypeCheck(PBEPokemon user, PBEPokemon target, PBEType moveType, out PBEResult result, out double damageMultiplier)
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
        private bool CritCheck(PBEPokemon user, PBEPokemon target, PBEMove move)
        {
            if (((target.Ability == PBEAbility.BattleArmor || target.Ability == PBEAbility.ShellArmor) && !user.HasCancellingAbility())
                || target.Team.TeamStatus.HasFlag(PBETeamStatus.LuckyChant))
            {
                return false;
            }
            else
            {
                PBEMoveData mData = PBEMoveData.Data[move];
                if (mData.Flags.HasFlag(PBEMoveFlag.AlwaysCrit))
                {
                    return true;
                }
                else
                {
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
            }
        }
        private void TrySetLoser(PBEPokemon pkmn)
        {
            if (Winner == null && pkmn.Team.NumPkmnAlive == 0)
            {
                Winner = pkmn.Team.OpposingTeam;
            }
        }
        private bool FaintCheck(PBEPokemon pkmn)
        {
            if (pkmn.HP == 0)
            {
                _turnOrder.Remove(pkmn);
                ActiveBattlers.Remove(pkmn);
                PBEFieldPosition oldPos = pkmn.FieldPosition;
                pkmn.FieldPosition = PBEFieldPosition.None;
                BroadcastPkmnFainted(pkmn, oldPos);
                RemoveInfatuationsAndLockOns(pkmn);
                pkmn.Team.MonFaintedThisTurn = true;
                TrySetLoser(pkmn);
                CastformCherrimCheckAll();
                return true;
            }
            return false;
        }

        private void ActivateAbility(PBEPokemon pkmn, bool switchIn)
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
                        CastformCherrimCheckAll();
                    }
                    break;
                }
                case PBEAbility.Anticipation:
                {
                    PBEPokemon[] oppActive = pkmn.Team.OpposingTeam.ActiveBattlers;
                    if (oppActive.Length != 0)
                    {
                        foreach (PBEPokemon opponent in oppActive)
                        {
                            foreach (PBEBattleMoveset.PBEBattleMovesetSlot moveSlot in opponent.Moves)
                            {
                                PBEMove move = moveSlot.Move;
                                if (move != PBEMove.None)
                                {
                                    PBEMoveData mData = PBEMoveData.Data[move];
                                    if (mData.Category != PBEMoveCategory.Status)
                                    {
                                        double d = PBETypeEffectiveness.GetEffectiveness(mData.Type, pkmn.Type1, pkmn.Type2);
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
                    PBEPokemon[] oppActive = pkmn.Team.OpposingTeam.ActiveBattlers;
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
                    PBEPokemon target = pkmn.Team.OpposingTeam.TryGetPokemon(targetPos);
                    if (target != null && pkmn.IsTransformPossible(target) == PBEResult.Success)
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
        private void CastformCherrimCheck(PBEPokemon[] order)
        {
            foreach (PBEPokemon pkmn in order)
            {
                CastformCherrimCheck(pkmn);
            }
        }
        private void CastformCherrimCheck(PBEPokemon pkmn)
        {
            // Castform & Cherrim may be changing form because their ability was swapped or suppressed, so check for the specific ability before setting KnownAbility
            if (pkmn.OriginalSpecies == PBESpecies.Castform)
            {
                PBESpecies newSpecies = PBESpecies.Castform;
                if (pkmn.Ability == PBEAbility.Forecast)
                {
                    if (ShouldDoWeatherEffects())
                    {
                        switch (Weather)
                        {
                            case PBEWeather.Hailstorm: newSpecies = PBESpecies.Castform_Snowy; break;
                            case PBEWeather.HarshSunlight: newSpecies = PBESpecies.Castform_Sunny; break;
                            case PBEWeather.Rain: newSpecies = PBESpecies.Castform_Rainy; break;
                        }
                        if (newSpecies != pkmn.Species)
                        {
                            BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedAppearance);
                        }
                    }
                }
                if (newSpecies != pkmn.Species)
                {
                    BroadcastPkmnFormChanged(pkmn, newSpecies, pkmn.Ability, pkmn.KnownAbility);
                }
            }
            else if (pkmn.OriginalSpecies == PBESpecies.Cherrim)
            {
                PBESpecies newSpecies = PBESpecies.Cherrim;
                if (pkmn.Ability == PBEAbility.FlowerGift)
                {
                    if (ShouldDoWeatherEffects())
                    {
                        if (Weather == PBEWeather.HarshSunlight)
                        {
                            newSpecies = PBESpecies.Cherrim_Sunshine;
                        }
                        if (newSpecies != pkmn.Species)
                        {
                            BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedAppearance);
                        }
                    }
                }
                if (newSpecies != pkmn.Species)
                {
                    BroadcastPkmnFormChanged(pkmn, newSpecies, pkmn.Ability, pkmn.KnownAbility);
                }
            }
        }
        private void ShayminCheck(PBEPokemon pkmn)
        {
            // If a Shaymin_Sky is given MagmaArmor and then Frozen, it will change to Shaymin and obtain Shaymin's ability, therefore losing MagmaArmor and as a result will not be cured of its Frozen status.
            if (pkmn.Species == PBESpecies.Shaymin_Sky && pkmn.OriginalSpecies == PBESpecies.Shaymin_Sky && pkmn.Status1 == PBEStatus1.Frozen)
            {
                const PBESpecies newSpecies = PBESpecies.Shaymin;
                pkmn.Shaymin_CannotChangeBackToSkyForm = true;
                BroadcastPkmnFormChanged(pkmn, newSpecies, PBEPokemonData.GetData(newSpecies).Abilities[0], PBEAbility.MAX);
                ActivateAbility(pkmn, false);
            }
        }
        private void IllusionBreak(PBEPokemon pkmn, PBEPokemon breaker)
        {
            if (pkmn.Status2.HasFlag(PBEStatus2.Disguised))
            {
                pkmn.Status2 &= ~PBEStatus2.Disguised;
                pkmn.DisguisedAsPokemon = null;
                pkmn.KnownGender = pkmn.Gender;
                pkmn.KnownNickname = pkmn.Nickname;
                pkmn.KnownShiny = pkmn.Shiny;
                pkmn.KnownSpecies = pkmn.Species;
                pkmn.KnownType1 = pkmn.Type1;
                pkmn.KnownType2 = pkmn.Type2;
                BroadcastIllusion(pkmn);
                BroadcastAbility(pkmn, breaker, PBEAbility.Illusion, PBEAbilityAction.ChangedAppearance);
            }
        }
        private void AntiStatusAbilityCheck(PBEPokemon pkmn)
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
                        BroadcastStatus1(pkmn, pkmn, oldStatus1, PBEStatusAction.Cured);
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
                        BroadcastStatus1(pkmn, pkmn, PBEStatus1.Asleep, PBEStatusAction.Cured);
                    }
                    break;
                }
                case PBEAbility.Limber:
                {
                    if (pkmn.Status1 == PBEStatus1.Paralyzed)
                    {
                        pkmn.Status1 = PBEStatus1.None;
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                        BroadcastStatus1(pkmn, pkmn, PBEStatus1.Paralyzed, PBEStatusAction.Cured);
                    }
                    break;
                }
                case PBEAbility.MagmaArmor:
                {
                    if (pkmn.Status1 == PBEStatus1.Frozen)
                    {
                        pkmn.Status1 = PBEStatus1.None;
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                        BroadcastStatus1(pkmn, pkmn, PBEStatus1.Frozen, PBEStatusAction.Cured);
                    }
                    break;
                }
                case PBEAbility.Oblivious:
                {
                    if (pkmn.Status2.HasFlag(PBEStatus2.Infatuated))
                    {
                        pkmn.Status2 &= ~PBEStatus2.Infatuated;
                        pkmn.InfatuatedWithPokemon = null;
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                        BroadcastStatus2(pkmn, pkmn, PBEStatus2.Infatuated, PBEStatusAction.Cured);
                    }
                    break;
                }
                case PBEAbility.OwnTempo:
                {
                    if (pkmn.Status2.HasFlag(PBEStatus2.Confused))
                    {
                        pkmn.Status2 &= ~PBEStatus2.Confused;
                        pkmn.ConfusionCounter = 0;
                        pkmn.ConfusionTurns = 0;
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                        BroadcastStatus2(pkmn, pkmn, PBEStatus2.Confused, PBEStatusAction.Cured);
                    }
                    break;
                }
                case PBEAbility.WaterVeil:
                {
                    if (pkmn.Status1 == PBEStatus1.Burned)
                    {
                        pkmn.Status1 = PBEStatus1.None;
                        BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.ChangedStatus);
                        BroadcastStatus1(pkmn, pkmn, PBEStatus1.Burned, PBEStatusAction.Cured);
                    }
                    break;
                }
            }
        }
        private void CauseConfusion(PBEPokemon target, PBEPokemon other)
        {
            target.Status2 |= PBEStatus2.Confused;
            target.ConfusionCounter = 0;
            target.ConfusionTurns = (byte)PBERandom.RandomInt(Settings.ConfusionMinTurns, Settings.ConfusionMaxTurns);
            BroadcastStatus2(target, other, PBEStatus2.Confused, PBEStatusAction.Added);
            AntiStatusAbilityCheck(target);
        }
        private void CauseInfatuation(PBEPokemon target, PBEPokemon other)
        {
            target.Status2 |= PBEStatus2.Infatuated;
            target.InfatuatedWithPokemon = other;
            BroadcastStatus2(target, other, PBEStatus2.Infatuated, PBEStatusAction.Added);
            if (target.Item == PBEItem.DestinyKnot && other.IsAttractionPossible(target) == PBEResult.Success)
            {
                BroadcastItem(target, other, PBEItem.DestinyKnot, PBEItemAction.ChangedStatus);
                other.Status2 |= PBEStatus2.Infatuated;
                other.InfatuatedWithPokemon = target;
                BroadcastStatus2(other, target, PBEStatus2.Infatuated, PBEStatusAction.Added);
            }
            AntiStatusAbilityCheck(target);
        }
        // TODO: Use & add packet handlers
        private void WhiteHerbCheck(PBEPokemon pkmn)
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
        private bool PowerHerbCheck(PBEPokemon pkmn)
        {
            if (pkmn.Item == PBEItem.PowerHerb)
            {
                BroadcastItem(pkmn, pkmn, PBEItem.PowerHerb, PBEItemAction.Consumed);
                return true;
            }
            return false;
        }
        private void LowHPBerryCheck(PBEPokemon pkmn, PBEPokemon forcedToEatBy = null)
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
                        PBEStat[] statsThatCanGoUp = _starfBerryStats.Where(s => pkmn.GetStatChange(s) < Settings.MaxStatChange).ToArray();
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
        private void SetAbility(PBEPokemon user, PBEPokemon target, PBEAbility ability)
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

            ActivateAbility(target, true);
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

        private void RecordExecutedMove(PBEPokemon user, PBEMove move)
        {
            user.HasUsedMoveThisTurn = true;
            // Doesn't care if there is a Choice Locked move already. As long as the user knows it, it will become locked. (Metronome calling a move the user knows, Ditto transforming into someone else with transform)
            if ((user.Item == PBEItem.ChoiceBand || user.Item == PBEItem.ChoiceScarf || user.Item == PBEItem.ChoiceSpecs) && user.Moves.Contains(move))
            {
                BroadcastMoveLock(user, move, PBETurnTarget.None, PBEMoveLockType.ChoiceItem);
            }
            if (move == PBEMove.Minimize)
            {
                user.Minimize_Used = true;
            }
        }
        private void PPReduce(PBEPokemon pkmn, PBEMove move)
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

        private void SetSleepTurns(PBEPokemon pkmn, int minTurns, int maxTurns)
        {
            pkmn.SleepTurns = (byte)(PBERandom.RandomInt(minTurns, maxTurns) / (pkmn.Ability == PBEAbility.EarlyBird ? 2 : 1));
        }
        private void DoTransform(PBEPokemon user, PBEPokemon target)
        {
            bool lostPowerTrick = user.Status2.HasFlag(PBEStatus2.PowerTrick);
            user.Transform(target);
            BroadcastTransform(user, target);
            BroadcastStatus2(user, target, PBEStatus2.Transformed, PBEStatusAction.Added);
            if (lostPowerTrick)
            {
                BroadcastStatus2(user, user, PBEStatus2.PowerTrick, PBEStatusAction.Ended);
            }
        }
        private PBEResult ApplyStatus1IfPossible(PBEPokemon user, PBEPokemon target, PBEStatus1 status, bool broadcastUnsuccessful)
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
        private PBEResult ApplyStatus2IfPossible(PBEPokemon user, PBEPokemon target, PBEStatus2 status, bool broadcastUnsuccessful)
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
                        target.Status2 |= PBEStatus2.Cursed;
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
                        target.Status2 |= PBEStatus2.Flinching;
                    }
                    break;
                }
                case PBEStatus2.HelpingHand:
                {
                    if (!target.HasUsedMoveThisTurn)
                    {
                        target.Status2 |= PBEStatus2.HelpingHand;
                        BroadcastStatus2(target, user, PBEStatus2.HelpingHand, PBEStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.InvalidConditions;
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
                        target.Status2 |= PBEStatus2.LeechSeed;
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
                        user.Status2 |= PBEStatus2.LockOn;
                        user.LockOnPokemon = target;
                        user.LockOnTurns = 2;
                        BroadcastStatus2(user, target, PBEStatus2.LockOn, PBEStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        result = PBEResult.Ineffective_Stat;
                    }
                    break;
                }
                case PBEStatus2.MagnetRise:
                {
                    result = target.IsMagnetRisePossible();
                    if (result == PBEResult.Success)
                    {
                        target.Status2 |= PBEStatus2.MagnetRise;
                        target.MagnetRiseTurns = 5;
                        BroadcastStatus2(target, user, PBEStatus2.MagnetRise, PBEStatusAction.Added);
                    }
                    break;
                }
                case PBEStatus2.MiracleEye:
                {
                    if (!target.Status2.HasFlag(PBEStatus2.MiracleEye))
                    {
                        target.Status2 |= PBEStatus2.MiracleEye;
                        BroadcastStatus2(target, user, PBEStatus2.MiracleEye, PBEStatusAction.Added);
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
                    target.Status2 |= PBEStatus2.PowerTrick;
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
                        user.Protection_Counter++;
                        user.Status2 |= PBEStatus2.Protected;
                        BroadcastStatus2(user, user, PBEStatus2.Protected, PBEStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        user.Protection_Counter = 0;
                        result = PBEResult.InvalidConditions;
                    }
                    break;
                }
                case PBEStatus2.Pumped:
                {
                    if (!target.Status2.HasFlag(PBEStatus2.Pumped))
                    {
                        target.Status2 |= PBEStatus2.Pumped;
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
                        target.Status2 |= PBEStatus2.Substitute;
                        target.SubstituteHP = (ushort)hpRequired;
                        BroadcastStatus2(target, user, PBEStatus2.Substitute, PBEStatusAction.Added);
                    }
                    break;
                }
                case PBEStatus2.Transformed:
                {
                    result = user.IsTransformPossible(target);
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
        // Our fallen hero, should we change all old references?
        private void ApplyStatChangeIfPossible(PBEPokemon user, PBEPokemon target, PBEStat stat, int change, bool isSecondaryEffect = false)
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
        private void SetStatAndBroadcast(PBEPokemon pkmn, PBEStat stat, sbyte oldValue, sbyte newValue)
        {
            pkmn.SetStatChange(stat, newValue);
            BroadcastPkmnStatChanged(pkmn, stat, oldValue, newValue);
        }

        private PBEPkmnSwitchInPacket.PBESwitchInInfo CreateSwitchInInfo(PBEPokemon pkmn)
        {
            if (pkmn.Ability == PBEAbility.Illusion)
            {
                PBEPokemon last = pkmn.Team.Party.Last();
                if (last.HP > 0 && last.OriginalSpecies != pkmn.OriginalSpecies)
                {
                    pkmn.Status2 |= PBEStatus2.Disguised;
                    pkmn.DisguisedAsPokemon = last;
                    pkmn.KnownGender = last.Gender;
                    pkmn.KnownNickname = last.Nickname;
                    pkmn.KnownShiny = last.Shiny;
                    pkmn.KnownSpecies = last.OriginalSpecies;
                    var pData = PBEPokemonData.GetData(last.OriginalSpecies);
                    pkmn.KnownType1 = pData.Type1;
                    pkmn.KnownType2 = pData.Type2;
                    return new PBEPkmnSwitchInPacket.PBESwitchInInfo(pkmn.Id, last.Id, last.OriginalSpecies, last.Nickname, pkmn.Level, last.Shiny, last.Gender, pkmn.HP, pkmn.MaxHP, pkmn.HPPercentage, pkmn.Status1, pkmn.FieldPosition);
                }
            }
            return new PBEPkmnSwitchInPacket.PBESwitchInInfo(pkmn.Id, pkmn.Id, pkmn.Species, pkmn.Nickname, pkmn.Level, pkmn.Shiny, pkmn.Gender, pkmn.HP, pkmn.MaxHP, pkmn.HPPercentage, pkmn.Status1, pkmn.FieldPosition);
        }
        private void SwitchTwoPokemon(PBEPokemon pkmnLeaving, PBEPokemon pkmnComing, PBEPokemon forcedByPkmn = null)
        {
            PBEFieldPosition pos = pkmnLeaving.FieldPosition;
            _turnOrder.Remove(pkmnLeaving);
            ActiveBattlers.Remove(pkmnLeaving);
            PBEPokemon disguisedAsPokemon = pkmnLeaving.Status2.HasFlag(PBEStatus2.Disguised) ? pkmnLeaving.DisguisedAsPokemon : pkmnLeaving;
            pkmnLeaving.ClearForSwitch();
            BroadcastPkmnSwitchOut(pkmnLeaving, disguisedAsPokemon, pos, forcedByPkmn);
            RemoveInfatuationsAndLockOns(pkmnLeaving);
            pkmnComing.FieldPosition = pos;
            ActiveBattlers.Add(pkmnComing);
            BroadcastPkmnSwitchIn(pkmnComing.Team, new[] { CreateSwitchInInfo(pkmnComing) }, forcedByPkmn);
            if (forcedByPkmn != null)
            {
                BroadcastDraggedOut(pkmnComing);
            }
            DoSwitchInEffects(new[] { pkmnComing }, forcedByPkmn);
            CastformCherrimCheckAll();
        }
        private void RemoveInfatuationsAndLockOns(PBEPokemon pkmnLeaving)
        {
            foreach (PBEPokemon pkmn in ActiveBattlers)
            {
                if (pkmn.Status2.HasFlag(PBEStatus2.Infatuated) && pkmn.InfatuatedWithPokemon == pkmnLeaving)
                {
                    pkmn.Status2 &= ~PBEStatus2.Infatuated;
                    pkmn.InfatuatedWithPokemon = null;
                    BroadcastStatus2(pkmn, pkmn, PBEStatus2.Infatuated, PBEStatusAction.Ended);
                }
                if (pkmn.Status2.HasFlag(PBEStatus2.LockOn) && pkmn.LockOnPokemon == pkmnLeaving)
                {
                    pkmn.Status2 &= ~PBEStatus2.LockOn;
                    pkmn.LockOnPokemon = null;
                    pkmn.LockOnTurns = 0;
                    BroadcastStatus2(pkmn, pkmn, PBEStatus2.LockOn, PBEStatusAction.Ended);
                }
            }
        }

        // Should there be two versions of BasicHit, one for single target and another for multi target?
        // The games probably have two (some move types like MakesContact/Recoil are guaranteed to be single target), but we might not need two so we can support editing moves/adding moves
        // Some moves types will need to throw an exception if they bypass the rule, so they do not violate behavior (pickpocket and life orb interaction, for example)
        private void BasicHit(PBEPokemon user, PBEPokemon[] targets, PBEMove move,
            Func<int, int?> recoilFunc = null,
            Func<PBEPokemon, PBEResult> beforeDoingDamage = null,
            Action<PBEPokemon> beforePostHit = null,
            Action<PBEPokemon, ushort> afterPostHit = null,
            Action beforeTargetsFaint = null,
            bool hitRegardlessOfUserConciousness = false)
        {
            // TODO: Rocky Helmet tests for user fainting, winner, etc [force battle subway battle flag? or actually do link battles]

            bool hitSomeone = false;
            int totalDamageDealt = 0;
            PBEType moveType = user.GetMoveType(move);
            double basePower = CalculateBasePower(user, targets, move, moveType);
            foreach (PBEPokemon target in targets)
            {
                if (!MissCheck(user, target, move))
                {
                    if (AttackTypeCheck(user, target, moveType, out PBEResult result, out double damageMultiplier))
                    {
                        // Brick Break destroys Light Screen and Reflect before doing damage
                        // Dream Eater checks for sleep before doing damage
                        // Sucker Punch fails before doing damage
                        if (beforeDoingDamage == null || beforeDoingDamage.Invoke(target) == PBEResult.Success)
                        {
                            if (targets.Length > 1)
                            {
                                damageMultiplier *= 0.75;
                            }
                            bool crit = CritCheck(user, target, move);
                            damageMultiplier *= CalculateDamageMultiplier(user, target, move, moveType, result, crit);
                            int damage = (int)(damageMultiplier * CalculateDamage(user, target, move, moveType, PBEMoveData.Data[move].Category, basePower, crit));
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
                            DoPostHitEffects(user, target, move, moveType); // User faints here
                            // HP-draining moves restore HP after post-hit effects
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
                foreach (PBEPokemon target in targets)
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
        private void FixedDamageHit(PBEPokemon user, PBEPokemon[] targets, PBEMove move, Func<PBEPokemon, int> damageFunc,
            Func<PBEPokemon, PBEResult> beforeMissCheck = null,
            Action beforeTargetsFaint = null)
        {
            bool hitSomeone = false;
            PBEType moveType = user.GetMoveType(move);
            foreach (PBEPokemon target in targets)
            {
                // Endeavor fails if the target's HP is <= the user's HP
                // One hit knockout moves fail if the target's level is > the user's level
                if (beforeMissCheck == null || beforeMissCheck.Invoke(target) == PBEResult.Success)
                {
                    if (!MissCheck(user, target, move))
                    {
                        if (AttackTypeCheck(user, target, moveType, out PBEResult _, out double _))
                        {
                            // Damage func is run and the output is dealt to target
                            DealDamage(user, target, damageFunc.Invoke(target), false);

                            DoPostHitEffects(user, target, move, moveType); // User faints here
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
                foreach (PBEPokemon target in targets)
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
        private void MultiHit(PBEPokemon user, PBEPokemon[] targets, PBEMove move, byte numHits,
            bool subsequentMissChecks = false,
            Action<PBEPokemon> beforePostHit = null)
        {
            bool hitSomeone = false;
            PBEType moveType = user.GetMoveType(move);
            double basePower = CalculateBasePower(user, targets, move, moveType); // Verified: Gem boost applies to all hits
            foreach (PBEPokemon target in targets)
            {
                if (!MissCheck(user, target, move))
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
                            bool crit = CritCheck(user, target, move);
                            double curDamageMultiplier = damageMultiplier * CalculateDamageMultiplier(user, target, move, moveType, result, crit);
                            int damage = (int)(curDamageMultiplier * CalculateDamage(user, target, move, moveType, PBEMoveData.Data[move].Category, basePower, crit));
                            DealDamage(user, target, damage, false);
                            if (crit)
                            {
                                BroadcastMoveCrit(target);
                            }
                            // Twineedle has a chance to poison on each strike
                            beforePostHit?.Invoke(target);
                            DoPostHitEffects(user, target, move, moveType); // User faints here
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
        private void SemiInvulnerableChargeMove(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBETurnTarget requestedTargets, PBEStatus2 status2)
        {
            BroadcastMoveUsed(user, move);
        top:
            if (user.Status2.HasFlag(status2))
            {
                BroadcastMoveLock(user, PBEMove.None, PBETurnTarget.None, PBEMoveLockType.Temporary);
                user.Status2 &= ~status2;
                BroadcastStatus2(user, user, status2, PBEStatusAction.Ended);
                if (targets.Length == 0)
                {
                    BroadcastMoveResult(user, user, PBEResult.NoTarget);
                }
                else
                {
                    BasicHit(user, targets, move);
                }
                RecordExecutedMove(user, move); // Should only count as the last used move if it finishes charging
            }
            else
            {
                PPReduce(user, move);
                BroadcastMoveLock(user, move, requestedTargets, PBEMoveLockType.Temporary);
                user.Status2 |= status2;
                BroadcastStatus2(user, user, status2, PBEStatusAction.Added);
                if (PowerHerbCheck(user))
                {
                    goto top;
                }
            }
        }

        private void Ef_TryForceStatus1(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStatus1 status)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
                    {
                        if (move == PBEMove.ThunderWave)
                        {
                            PBEResult result = PBETypeEffectiveness.ThunderWaveTypeCheck(user, target);
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
            RecordExecutedMove(user, move);
            return;
        }
        private void Ef_TryForceStatus2(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStatus2 status)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
                    {
                        ApplyStatus2IfPossible(user, target, status, true);
                        DoPostAttackedTargetEffects(target); // Only necessary for AntiStatusCheck() right now
                    }
                }
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_TryForceBattleStatus(PBEPokemon user, PBEMove move, PBEBattleStatus status)
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
            RecordExecutedMove(user, move);
        }
        private void Ef_TryForceTeamStatus(PBEPokemon user, PBEMove move, PBETeamStatus status)
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
                        user.Team.TeamStatus |= PBETeamStatus.LightScreen;
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
                        user.Team.TeamStatus |= PBETeamStatus.LuckyChant;
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
                case PBETeamStatus.Reflect:
                {
                    if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.Reflect))
                    {
                        user.Team.TeamStatus |= PBETeamStatus.Reflect;
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
                        user.Team.TeamStatus |= PBETeamStatus.Safeguard;
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
                        user.Team.OpposingTeam.TeamStatus |= PBETeamStatus.Spikes;
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
                        user.Team.OpposingTeam.TeamStatus |= PBETeamStatus.StealthRock;
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
                        user.Team.TeamStatus |= PBETeamStatus.Tailwind;
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
                        user.Team.OpposingTeam.TeamStatus |= PBETeamStatus.ToxicSpikes;
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
                        user.Protection_Counter++;
                        user.Team.TeamStatus |= PBETeamStatus.WideGuard;
                        BroadcastTeamStatus(user.Team, PBETeamStatus.WideGuard, PBETeamStatusAction.Added);
                        result = PBEResult.Success;
                    }
                    else
                    {
                        user.Protection_Counter = 0;
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
            RecordExecutedMove(user, move);
        }
        private void Ef_TryForceWeather(PBEPokemon user, PBEMove move, PBEWeather weather)
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
            RecordExecutedMove(user, move);
        }

        private void Ef_ChangeTargetStats(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStat[] stats, int[] changes, bool requireAttraction = false)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
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
                            for (int i = 0; i < stats.Length; i++)
                            {
                                ApplyStatChangeIfPossible(user, target, stats[i], changes[i]);
                            }
                        }
                    }
                }
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Hit__MaybeChangeTargetStats(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStat[] stats, int[] changes, int chanceToChangeStats)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEPokemon target)
                {
                    if (target.HP > 0 && !target.Status2.HasFlag(PBEStatus2.Substitute) && PBERandom.RandomBool(chanceToChangeStats, 100))
                    {
                        for (int i = 0; i < stats.Length; i++)
                        {
                            ApplyStatChangeIfPossible(user, target, stats[i], changes[i], isSecondaryEffect: true);
                        }
                    }
                }
                BasicHit(user, targets, move, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Hit__MaybeChangeUserStats(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStat[] stats, int[] changes, int chanceToChangeStats)
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
                    if (user.HP > 0 && PBERandom.RandomBool(chanceToChangeStats, 100))
                    {
                        for (int i = 0; i < stats.Length; i++)
                        {
                            ApplyStatChangeIfPossible(user, user, stats[i], changes[i], isSecondaryEffect: true);
                        }
                    }
                }
                BasicHit(user, targets, move, beforeTargetsFaint: BeforeTargetsFaint);
            }
            RecordExecutedMove(user, move);
        }

        private void Ef_Entrainment(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
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
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
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
            RecordExecutedMove(user, move);
        }
        private void Ef_RolePlay(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
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
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
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
            RecordExecutedMove(user, move);
        }
        private void Ef_SetOtherAbility(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEAbility ability, bool blockedByTruant)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
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
            RecordExecutedMove(user, move);
        }

        private void Ef_BrickBreak(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                PBEResult BeforeDoingDamage(PBEPokemon target)
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
                    return PBEResult.Success;
                }
                BasicHit(user, targets, move, beforeDoingDamage: BeforeDoingDamage);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Hit(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEStatus1 status1 = PBEStatus1.None, int chanceToInflictStatus1 = 0, PBEStatus2 status2 = PBEStatus2.None, int chanceToInflictStatus2 = 0)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEPokemon target)
                {
                    if (target.HP > 0)
                    {
                        if (status1 != PBEStatus1.None && PBERandom.RandomBool(chanceToInflictStatus1, 100))
                        {
                            ApplyStatus1IfPossible(user, target, status1, false);
                        }
                        if (status2 != PBEStatus2.None && PBERandom.RandomBool(chanceToInflictStatus2, 100))
                        {
                            ApplyStatus2IfPossible(user, target, status2, false);
                        }
                    }
                }
                BasicHit(user, targets, move, beforePostHit: status1 != PBEStatus1.None || status2 != PBEStatus2.None ? BeforePostHit : (Action<PBEPokemon>)null);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_MultiHit(PBEPokemon user, PBEPokemon[] targets, PBEMove move, byte numHits, bool subsequentMissChecks = false, PBEStatus1 status1 = PBEStatus1.None, int chanceToInflictStatus1 = 0)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEPokemon target)
                {
                    if (target.HP > 0 && PBERandom.RandomBool(chanceToInflictStatus1, 100))
                    {
                        ApplyStatus1IfPossible(user, target, status1, false);
                    }
                }
                MultiHit(user, targets, move, numHits, subsequentMissChecks: subsequentMissChecks, beforePostHit: status1 != PBEStatus1.None ? BeforePostHit : (Action<PBEPokemon>)null); // Doesn't need to be its own func but neater
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Recoil(PBEPokemon user, PBEPokemon[] targets, PBEMove move, int denominator, PBEStatus1 status1 = PBEStatus1.None, int chanceToInflictStatus1 = 0, PBEStatus2 status2 = PBEStatus2.None, int chanceToInflictStatus2 = 0)
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
                    return user.Ability == PBEAbility.RockHead || totalDamageDealt == 0 ? (int?)null : totalDamageDealt / denominator;
                }
                void BeforePostHit(PBEPokemon target)
                {
                    if (target.HP > 0)
                    {
                        if (status1 != PBEStatus1.None && PBERandom.RandomBool(chanceToInflictStatus1, 100))
                        {
                            ApplyStatus1IfPossible(user, target, status1, false);
                        }
                        if (status2 != PBEStatus2.None && PBERandom.RandomBool(chanceToInflictStatus2, 100))
                        {
                            ApplyStatus2IfPossible(user, target, status2, false);
                        }
                    }
                }
                BasicHit(user, targets, move, recoilFunc: RecoilFunc, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Selfdestruct(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
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
                BasicHit(user, targets, move, hitRegardlessOfUserConciousness: true);
            }
            FaintCheck(user);
            RecordExecutedMove(user, move);
        }
        private void Ef_Snore(PBEPokemon user, PBEPokemon[] targets, PBEMove move, int chanceToFlinch)
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
                void BeforePostHit(PBEPokemon target)
                {
                    if (target.HP > 0 && PBERandom.RandomBool(chanceToFlinch, 100))
                    {
                        ApplyStatus2IfPossible(user, target, PBEStatus2.Flinching, false);
                    }
                }
                BasicHit(user, targets, move, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Struggle(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
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
                    return user.MaxHP / 4;
                }
                BasicHit(user, targets, move, recoilFunc: RecoilFunc);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_SuckerPunch(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                PBEResult BeforeDoingDamage(PBEPokemon target)
                {
                    if (target.TurnAction.Decision != PBETurnDecision.Fight
                        || PBEMoveData.Data[target.TurnAction.FightMove].Category == PBEMoveCategory.Status
                        || target.HasUsedMoveThisTurn)
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
                BasicHit(user, targets, move, beforeDoingDamage: BeforeDoingDamage);
            }
            RecordExecutedMove(user, move);
        }

        private void Ef_Endeavor(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEPokemon target)
                {
                    return target.HP - user.HP;
                }
                PBEResult BeforeMissCheck(PBEPokemon target)
                {
                    if (target.HP <= user.HP)
                    {
                        PBEResult result = PBEResult.Ineffective_Stat;
                        BroadcastMoveResult(user, target, result);
                        return result;
                    }
                    return PBEResult.Success;
                }
                FixedDamageHit(user, targets, move, DamageFunc, beforeMissCheck: BeforeMissCheck);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_FinalGambit(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEPokemon target)
                {
                    int oldHP = user.HP;
                    DealDamage(user, user, oldHP, true);
                    FaintCheck(user);
                    return oldHP;
                }
                FixedDamageHit(user, targets, move, DamageFunc);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_OneHitKnockout(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEPokemon target)
                {
                    return target.HP;
                }
                PBEResult BeforeMissCheck(PBEPokemon target)
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
                FixedDamageHit(user, targets, move, DamageFunc, beforeMissCheck: BeforeMissCheck, beforeTargetsFaint: BeforeTargetsFaint);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Psywave(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEPokemon target)
                {
                    return user.Level * (PBERandom.RandomInt(0, 100) + 50) / 100;
                }
                FixedDamageHit(user, targets, move, DamageFunc);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_SeismicToss(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEPokemon target)
                {
                    return user.Level;
                }
                FixedDamageHit(user, targets, move, DamageFunc);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_SetDamage(PBEPokemon user, PBEPokemon[] targets, PBEMove move, int damage)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEPokemon target)
                {
                    return damage;
                }
                FixedDamageHit(user, targets, move, DamageFunc);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_SuperFang(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                int DamageFunc(PBEPokemon target)
                {
                    return target.HP / 2;
                }
                FixedDamageHit(user, targets, move, DamageFunc);
            }
            RecordExecutedMove(user, move);
        }

        private void Ef_HPDrain(PBEPokemon user, PBEPokemon[] targets, PBEMove move, int percentRestored, bool requireSleep = false)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                PBEResult BeforeDoingDamage(PBEPokemon target)
                {
                    if (requireSleep && target.Status1 != PBEStatus1.Asleep)
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
                void AfterPostHit(PBEPokemon target, ushort damageDealt)
                {
                    int restoreAmt = (int)(damageDealt * (percentRestored / 100.0));
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
                BasicHit(user, targets, move, beforeDoingDamage: BeforeDoingDamage, afterPostHit: AfterPostHit);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Moonlight(PBEPokemon user, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            double percentage;
            if (ShouldDoWeatherEffects())
            {
                switch (Weather)
                {
                    case PBEWeather.None: percentage = 0.50; break;
                    case PBEWeather.HarshSunlight: percentage = 0.66; break;
                    default: percentage = 0.25; break;
                }
            }
            else
            {
                percentage = 0.50;
            }
            ushort amtRestored = HealDamage(user, (int)(user.MaxHP * percentage));
            if (amtRestored == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.Ineffective_Stat);
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_PainSplit(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
                    {
                        if (target.Status2.HasFlag(PBEStatus2.Substitute))
                        {
                            BroadcastMoveResult(user, target, PBEResult.Ineffective_Substitute);
                        }
                        else
                        {
                            int total = user.HP + target.HP;
                            int hp = total / 2;
                            foreach (PBEPokemon pkmn in new PBEPokemon[] { user, target })
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
            RecordExecutedMove(user, move);
        }
        private void Ef_Rest(PBEPokemon user, PBEMove move)
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
            RecordExecutedMove(user, move);
        }
        private void Ef_RestoreTargetHP(PBEPokemon user, PBEPokemon[] targets, PBEMove move, int percentRestored)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
                    {
                        if (user != target && target.Status2.HasFlag(PBEStatus2.Substitute))
                        {
                            BroadcastMoveResult(user, target, PBEResult.Ineffective_Substitute);
                        }
                        else
                        {
                            ushort amtRestored = HealDamage(target, (int)(target.MaxHP * (percentRestored / 100.0)));
                            if (amtRestored == 0)
                            {
                                BroadcastMoveResult(user, target, PBEResult.Ineffective_Stat);
                            }
                        }
                    }
                }
            }
            RecordExecutedMove(user, move);
        }

        private void Ef_BellyDrum(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
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
            RecordExecutedMove(user, move);
        }
        private void Ef_Camouflage(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
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
            RecordExecutedMove(user, move);
        }
        private void Ef_Conversion(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
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
            RecordExecutedMove(user, move);
        }
        private void Ef_Curse(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
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
                        foreach (PBEPokemon target in targets)
                        {
                            if (!MissCheck(user, target, move))
                            {
                                ApplyStatus2IfPossible(user, target, PBEStatus2.Cursed, true);
                            }
                        }
                    }
                }
                else
                {
                    foreach (PBEPokemon target in targets)
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
            RecordExecutedMove(user, move);
        }
        private void Ef_Flatter(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
                    {
                        ApplyStatChangeIfPossible(user, target, PBEStat.SpAttack, +1);
                        ApplyStatus2IfPossible(user, target, PBEStatus2.Confused, true);
                    }
                }
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Haze(PBEPokemon user, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            foreach (PBEPokemon pkmn in ActiveBattlers)
            {
                pkmn.ClearStatChanges();
            }
            BroadcastHaze();
            RecordExecutedMove(user, move);
        }
        private void Ef_HelpingHand(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    // TODO: When triple battle shifting happens, all moves that can target allies but not the user will have to check if the user targetted itself due to shifting.
                    // For now, I'll put this check here, because this is the only move that will attempt to target the user when the move cannot normally do so (single/rotation battle).
                    if (target == user)
                    {
                        BroadcastMoveResult(user, user, PBEResult.NoTarget);
                    }
                    else
                    {
                        if (!MissCheck(user, target, move))
                        {
                            ApplyStatus2IfPossible(user, target, PBEStatus2.HelpingHand, true);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Metronome(PBEPokemon user, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            // Record before the called move is recorded
            RecordExecutedMove(user, move);

            PBEMove calledMove = PBEMoveData.Data.Where(t => !t.Value.Flags.HasFlag(PBEMoveFlag.BlockedByMetronome)).Select(t => t.Key).ToArray().RandomElement();
            _calledFromOtherMove = true;
            UseMove(user, calledMove, GetRandomTargetForMetronome(user, calledMove));
            _calledFromOtherMove = false;
        }
        private void Ef_Nothing(PBEPokemon user, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            BroadcastNothingHappened();
            RecordExecutedMove(user, move);
        }
        private void Ef_PsychUp(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
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
                }
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Soak(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
                    {
                        BroadcastTypeChanged(target, PBEType.Water, PBEType.None);
                    }
                }
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Swagger(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
                    {
                        ApplyStatChangeIfPossible(user, target, PBEStat.Attack, +2);
                        ApplyStatus2IfPossible(user, target, PBEStatus2.Confused, true);
                    }
                }
            }
            RecordExecutedMove(user, move);
        }
        private void Ef_Teleport(PBEPokemon user, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            BroadcastMoveResult(user, user, PBEResult.InvalidConditions);
            RecordExecutedMove(user, move);
        }
        private void Ef_Whirlwind(PBEPokemon user, PBEPokemon[] targets, PBEMove move)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                foreach (PBEPokemon target in targets)
                {
                    if (!MissCheck(user, target, move))
                    {
                        PBEPokemon[] possibleSwitcheroonies = target.Team.Party.Where(p => p.FieldPosition == PBEFieldPosition.None && p.HP > 0).ToArray();
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
            RecordExecutedMove(user, move);
        }
    }
}
