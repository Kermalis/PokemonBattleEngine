using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Utils;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        private bool _calledFromOtherMove = false;

        private void DoSwitchInEffects(IEnumerable<PBEBattlePokemon> battlers, PBEBattlePokemon? forcedInBy = null)
        {
            // Set EXP Seens first
            foreach (PBEBattlePokemon pkmn in battlers)
            {
                PBETeam opTeam = pkmn.Team.OpposingTeam;
                foreach (PBEBattlePokemon op in opTeam.ActiveBattlers)
                {
                    op.AddEXPPokemon(pkmn);
                    pkmn.AddEXPPokemon(op);
                }
            }

            IEnumerable<PBEBattlePokemon> order = GetActingOrder(battlers, true);

            foreach (PBEBattlePokemon pkmn in order)
            {
                bool grounded = pkmn.IsGrounded(forcedInBy) == PBEResult.Success;
                // Verified: (Spikes/StealthRock/ToxicSpikes in the order they were applied) before ability
                if (grounded && pkmn.Team.TeamStatus.HasFlag(PBETeamStatus.Spikes))
                {
                    BroadcastTeamStatusDamage(pkmn.Team, PBETeamStatus.Spikes, pkmn);
                    DealDamage(pkmn, pkmn, (int)(pkmn.MaxHP / (10.0 - (2 * pkmn.Team.SpikeCount))));
                    if (FaintCheck(pkmn))
                    {
                        continue;
                    }
                    LowHPBerryCheck(pkmn, forcedToEatBy: forcedInBy);
                }
                if (pkmn.Team.TeamStatus.HasFlag(PBETeamStatus.StealthRock))
                {
                    BroadcastTeamStatusDamage(pkmn.Team, PBETeamStatus.StealthRock, pkmn);
                    DealDamage(pkmn, pkmn, (int)(pkmn.MaxHP * PBETypeEffectiveness.GetStealthRockMultiplier(pkmn.Type1, pkmn.Type2)));
                    if (FaintCheck(pkmn))
                    {
                        continue;
                    }
                    LowHPBerryCheck(pkmn, forcedToEatBy: forcedInBy);
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
        private void DoPostHitEffects(PBEBattlePokemon user, PBEBattlePokemon victim, IPBEMoveData mData, PBEType moveType)
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
                    DealDamage(victim, user, user.MaxHP / 8);
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
                        int randomNum = _rand.RandomInt(0, 29);
                        if (randomNum <= 20)
                        {
                            if (randomNum > 10) // 11-20 (10%)
                            {
                                // TODO: Can it really not paralyze electric? I thought that's gen 6+
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
                    DealDamage(victim, user, user.MaxHP / 6);
                    if (!FaintCheck(user))
                    {
                        LowHPBerryCheck(user);
                    }
                }
            }
        }
        private void DoPostAttackedEffects(PBEBattlePokemon user, List<PBEAttackVictim> allies, List<PBEAttackVictim> foes, bool doLifeOrb,
            int? recoilDamage = null,
            PBEType colorChangeType = PBEType.None)
        {
            #region User
            if (user.HP > 0)
            {
                // Verified: Recoil before LifeOrb
                // Verified: Recoil calls berry check directly, and both can faint here
                if (recoilDamage is not null)
                {
                    BroadcastRecoil(user);
                    DealDamage(user, user, recoilDamage.Value);
                    if (!FaintCheck(user))
                    {
                        LowHPBerryCheck(user);
                    }
                }
                if (user.HP > 0 && doLifeOrb && user.Item == PBEItem.LifeOrb)
                {
                    BroadcastItem(user, user, PBEItem.LifeOrb, PBEItemAction.Damage);
                    DealDamage(user, user, user.MaxHP / 10);
                    FaintCheck(user); // No berry check because we are holding Life Orb
                }
            }
            #endregion

            #region Victims
            void DoColorChange(IEnumerable<PBEBattlePokemon> order)
            {
                foreach (PBEBattlePokemon pkmn in order)
                {
                    if (pkmn.Ability == PBEAbility.ColorChange && !pkmn.HasType(colorChangeType))
                    {
                        BroadcastAbility(pkmn, pkmn, PBEAbility.ColorChange, PBEAbilityAction.ChangedAppearance);
                        BroadcastTypeChanged(pkmn, colorChangeType, PBEType.None);
                    }
                }
            }

            IEnumerable<PBEBattlePokemon> a = allies.Select(v => v.Pkmn).Where(p => p.HP > 0);
            IEnumerable<PBEBattlePokemon> f = foes.Select(v => v.Pkmn).Where(p => p.HP > 0);
            // Verified: ColorChange (foes, allies) before Berry
            if (colorChangeType != PBEType.None)
            {
                DoColorChange(f);
                DoColorChange(a);
            }
            // Verified: Berry (allies, foes) before AntiStatusAbility
            LowHPBerryCheck(a, forcedToEatBy: user);
            LowHPBerryCheck(f, forcedToEatBy: user);
            // Verified: AntiStatusAbility (allies, foes)
            AntiStatusAbilityCheck(a); // Heal a status that was given with the user's Mold Breaker
            AntiStatusAbilityCheck(f);
            #endregion
        }
        private void DoTurnEndedEffects()
        {
            IEnumerable<PBEBattlePokemon> order = GetActingOrder(ActiveBattlers, true);
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
                    if (pkmn.HP == 0)
                    {
                        continue;
                    }
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
                                BroadcastWeatherDamage(PBEWeather.Hailstorm, pkmn);
                                DealDamage(pkmn, pkmn, pkmn.MaxHP / Settings.HailDamageDenominator);
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
                                DealDamage(pkmn, pkmn, pkmn.MaxHP / 8);
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
                                BroadcastWeatherDamage(PBEWeather.Sandstorm, pkmn);
                                DealDamage(pkmn, pkmn, pkmn.MaxHP / Settings.SandstormDamageDenominator);
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

            // Verified: Healer/ShedSkin/BlackSludge/Leftovers before LeechSeed
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP == 0)
                {
                    continue;
                }
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
                                ally.Status1Counter = 0;
                                ally.SleepTurns = 0;
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
                            pkmn.Status1Counter = 0;
                            pkmn.SleepTurns = 0;
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
                        if (!pkmn.HasType(PBEType.Poison))
                        {
                            BroadcastItem(pkmn, pkmn, pkmn.Item, PBEItemAction.Damage);
                            DealDamage(pkmn, pkmn, pkmn.MaxHP / Settings.BlackSludgeDamageDenominator);
                            FaintCheck(pkmn); // No need to call HealingBerryCheck() because if you are holding BlackSludge you are not holding a healing berry
                        }
                        else if (pkmn.HP < pkmn.MaxHP)
                        {
                            BroadcastItem(pkmn, pkmn, pkmn.Item, PBEItemAction.RestoredHP);
                            HealDamage(pkmn, pkmn.MaxHP / Settings.BlackSludgeHealDenominator);
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

            // Verified: LeechSeed before Status1/PoisonHeal
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP == 0 || !pkmn.Status2.HasFlag(PBEStatus2.LeechSeed))
                {
                    continue;
                }
                if (!pkmn.SeededTeam!.TryGetPokemon(pkmn.SeededPosition, out PBEBattlePokemon? sucker))
                {
                    continue;
                }
                BroadcastStatus2(pkmn, sucker, PBEStatus2.LeechSeed, PBEStatusAction.Damage);
                int restoreAmt = DealDamage(sucker, pkmn, pkmn.MaxHP / Settings.LeechSeedDenominator);

                // In the games, the pkmn faints after taking damage (before liquid ooze/heal)
                // We cannot have it faint and then still broadcast ability like the games, similar to why we can't faint before Explosion
                // The faint order should be maintained, though, so the correct winner can be chosen
                ApplyBigRoot(pkmn, ref restoreAmt);
                if (pkmn.Ability == PBEAbility.LiquidOoze)
                {
                    BroadcastAbility(pkmn, sucker, PBEAbility.LiquidOoze, PBEAbilityAction.Damage);
                    DealDamage(pkmn, sucker, restoreAmt);
                    if (!FaintCheck(pkmn))
                    {
                        LowHPBerryCheck(pkmn);
                    }
                    if (!FaintCheck(sucker))
                    {
                        LowHPBerryCheck(sucker);
                    }
                }
                else
                {
                    if (!FaintCheck(pkmn))
                    {
                        LowHPBerryCheck(pkmn);
                    }
                    HealDamage(sucker, restoreAmt);
                }
            }

            // Verified: Status1/PoisonHeal before Curse
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP == 0)
                {
                    continue;
                }
                switch (pkmn.Status1)
                {
                    case PBEStatus1.BadlyPoisoned:
                    case PBEStatus1.Poisoned:
                    {
                        if (pkmn.Ability != PBEAbility.PoisonHeal)
                        {
                            BroadcastStatus1(pkmn, pkmn, pkmn.Status1, PBEStatusAction.Damage);
                            int damage = pkmn.Status1 == PBEStatus1.BadlyPoisoned
                                    ? pkmn.MaxHP * pkmn.Status1Counter / Settings.ToxicDamageDenominator
                                    : pkmn.MaxHP / Settings.PoisonDamageDenominator;
                            DealDamage(pkmn, pkmn, damage);
                            if (!FaintCheck(pkmn))
                            {
                                LowHPBerryCheck(pkmn);
                            }
                        }
                        else if (pkmn.HP < pkmn.MaxHP)
                        {
                            BroadcastAbility(pkmn, pkmn, PBEAbility.PoisonHeal, PBEAbilityAction.RestoredHP);
                            HealDamage(pkmn, pkmn.MaxHP / 8);
                        }
                        if (pkmn.Status1 == PBEStatus1.BadlyPoisoned)
                        {
                            pkmn.Status1Counter++; // Counter still increments if PoisonHeal exists
                        }
                        break;
                    }
                    case PBEStatus1.Burned:
                    {
                        BroadcastStatus1(pkmn, pkmn, pkmn.Status1, PBEStatusAction.Damage);
                        int damage = pkmn.MaxHP / Settings.BurnDamageDenominator;
                        if (pkmn.Ability == PBEAbility.Heatproof)
                        {
                            damage /= 2;
                        }
                        DealDamage(pkmn, pkmn, damage);
                        if (!FaintCheck(pkmn))
                        {
                            LowHPBerryCheck(pkmn);
                        }
                        break;
                    }
                }
            }

            // Verified: Nightmare before Curse, not same loop
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP == 0 || !pkmn.Status2.HasFlag(PBEStatus2.Nightmare))
                {
                    continue;
                }
                BroadcastStatus2(pkmn, pkmn, PBEStatus2.Nightmare, PBEStatusAction.Damage);
                DealDamage(pkmn, pkmn, pkmn.MaxHP / 4);
                if (!FaintCheck(pkmn))
                {
                    LowHPBerryCheck(pkmn);
                }
            }

            // Verified: Curse before MagnetRise
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP == 0 || !pkmn.Status2.HasFlag(PBEStatus2.Cursed))
                {
                    continue;
                }
                BroadcastStatus2(pkmn, pkmn, PBEStatus2.Cursed, PBEStatusAction.Damage);
                DealDamage(pkmn, pkmn, pkmn.MaxHP / Settings.CurseDenominator);
                if (!FaintCheck(pkmn))
                {
                    LowHPBerryCheck(pkmn);
                }
            }

            // Verified: MagnetRise before Abilities/Orbs
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP == 0 || !pkmn.Status2.HasFlag(PBEStatus2.MagnetRise) || pkmn.MagnetRiseTurns == 0)
                {
                    continue;
                }
                pkmn.MagnetRiseTurns--;
                if (pkmn.MagnetRiseTurns == 0)
                {
                    BroadcastStatus2(pkmn, pkmn, PBEStatus2.MagnetRise, PBEStatusAction.Ended);
                }
            }

            // Verified: BadDreams/Moody/SlowStart/SpeedBoost before Orbs, but activate together
            foreach (PBEBattlePokemon pkmn in order)
            {
                if (pkmn.HP == 0)
                {
                    continue;
                }
                // Ability before Orb
                switch (pkmn.Ability)
                {
                    case PBEAbility.BadDreams:
                    {
                        foreach (PBEBattlePokemon victim in GetRuntimeSurrounding(pkmn, false, true).Where(p => p.Status1 == PBEStatus1.Asleep))
                        {
                            BroadcastAbility(pkmn, victim, PBEAbility.BadDreams, PBEAbilityAction.Damage);
                            DealDamage(pkmn, victim, pkmn.MaxHP / 8);
                            if (!FaintCheck(victim))
                            {
                                LowHPBerryCheck(victim);
                            }
                        }
                        break;
                    }
                    case PBEAbility.Moody:
                    {
                        List<PBEStat> statsThatCanGoUp = PBEDataUtils.MoodyStats.FindAll(s => pkmn.GetStatChange(s) < Settings.MaxStatChange);
                        PBEStat? upStat = statsThatCanGoUp.Count == 0 ? null : _rand.RandomElement(statsThatCanGoUp);
                        List<PBEStat> statsThatCanGoDown = PBEDataUtils.MoodyStats.FindAll(s => pkmn.GetStatChange(s) > -Settings.MaxStatChange);
                        if (upStat is not null)
                        {
                            statsThatCanGoDown.Remove(upStat.Value);
                        }
                        PBEStat? downStat = statsThatCanGoDown.Count == 0 ? null : _rand.RandomElement(statsThatCanGoDown);
                        if (upStat is not null || downStat is not null)
                        {
                            BroadcastAbility(pkmn, pkmn, pkmn.Ability, PBEAbilityAction.Stats);
                            if (upStat is not null)
                            {
                                ApplyStatChangeIfPossible(pkmn, pkmn, upStat.Value, +2);
                            }
                            if (downStat is not null)
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
                            BroadcastItem(pkmn, pkmn, pkmn.Item, PBEItemAction.Announced);
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
                            BroadcastItem(pkmn, pkmn, pkmn.Item, PBEItemAction.Announced);
                            BroadcastStatus1(pkmn, pkmn, PBEStatus1.BadlyPoisoned, PBEStatusAction.Added);
                        }
                        break;
                    }
                }
            }
        }

        public bool ShouldDoWeatherEffects()
        {
            // If HP is needed to be above 0, use HPPercentage so clients can continue to use this
            // However, I see no instance of this getting called where an ActiveBattler has 0 hp
            return ActiveBattlers.FindIndex(p => p.Ability == PBEAbility.AirLock || p.Ability == PBEAbility.CloudNine) == -1;
        }
        public bool WillLeafGuardActivate()
        {
            return ShouldDoWeatherEffects() && Weather == PBEWeather.HarshSunlight;
        }

        private void FleeCheck()
        {
            foreach (PBETrainer trainer in Trainers)
            {
                if (!trainer.RequestedFlee)
                {
                    continue;
                }
                if (trainer.IsWild)
                {
                    PBEBattlePokemon wildPkmn = trainer.ActiveBattlersOrdered.First();
                    wildPkmn.TurnAction = new PBETurnAction(wildPkmn); // Convert into a WildFlee turn action for the first Pokémon
                    trainer.RequestedFlee = false;
                    continue;
                }
                PBEBattlePokemon? pkmn = trainer.ActiveBattlersOrdered.FirstOrDefault();
                if (pkmn is not null)
                {
                    // Verified: RunAway before SmokeBall
                    if (pkmn.Ability == PBEAbility.RunAway)
                    {
                        BroadcastAbility(pkmn, pkmn, PBEAbility.RunAway, PBEAbilityAction.Announced);
                        SetEscaped(pkmn);
                        return;
                    }
                    if (pkmn.Item == PBEItem.SmokeBall)
                    {
                        BroadcastItem(pkmn, pkmn, PBEItem.SmokeBall, PBEItemAction.Announced);
                        SetEscaped(pkmn);
                        return;
                    }
                }
                else
                {
                    pkmn = trainer.Party[0]; // Use the first fainted Pokémon's speed if there's no active battler
                }
                // TODO: I'm using the gen 3/4 formula below.
                // TODO: Figure out the gen 5 formula, as well as what to use in a double wild battle
                int a = pkmn.Speed;
                int b = (int)trainer.Team.OpposingTeam.ActiveBattlers.Average(p => p.Speed);
                int c = ++trainer.Team.NumTimesTriedToFlee; // Increment even if guaranteed
                bool success;
                if (a > b)
                {
                    success = true;
                }
                else
                {
                    int f = ((a * 128 / b) + (30 * c)) % 256;
                    success = _rand.RandomInt(0, 255) < f;
                }
                if (success)
                {
                    SetEscaped(pkmn);
                    return;
                }
                else
                {
                    BroadcastFleeFailed(pkmn);
                }
                trainer.RequestedFlee = false;
            }
        }
        private void WildFleeCheck(PBEBattlePokemon pkmn)
        {
            // TODO: Trapping effects
            SetEscaped(pkmn);
        }

        private void CalcEXP(PBEBattlePokemon loser)
        {
            IPBEPokemonData loserPData = PBEDataProvider.Instance.GetPokemonData(loser);
            float modTrainer = loser.IsWild ? 1 : 1.5f;
            int expYield = loserPData.BaseEXPYield;
            int levelLoser = loser.Level;
            float modPassPower = PBEDataProvider.Instance.GetEXPModifier(this);
            int amtParticipated = loser.EXPPokemon.Count(pk => pk.Trainer.GainsEXP && pk.HP > 0);
            int amtEXPShare = loser.EXPPokemon.Count(pk => pk.Trainer.GainsEXP && pk.Item == PBEItem.ExpShare);
            foreach (PBEBattlePokemon victor in loser.EXPPokemon)
            {
                if (!victor.Trainer.GainsEXP || victor.HP == 0 || victor.Level >= Settings.MaxLevel)
                {
                    continue;
                }

                int levelVictor = victor.Level;
                float modParticipators;
                if (amtEXPShare == 0)
                {
                    modParticipators = amtParticipated;
                }
                else if (victor.Item == PBEItem.ExpShare)
                {
                    modParticipators = 2 * amtEXPShare;
                }
                else
                {
                    modParticipators = 2 * amtParticipated;
                }
                float modTraded = PBEDataProvider.Instance.GetEXPTradeModifier(victor);
                float modLuckyEgg = victor.Item == PBEItem.LuckyEgg ? 1.5f : 1;

                float result1H = modTrainer * expYield * levelLoser;
                float result1L = 5 * modParticipators;
                float result1 = result1H / result1L;
                float result2H = MathF.Pow(2 * levelLoser + 10, 2.5f);
                float result2L = MathF.Pow(levelLoser + levelVictor + 10, 2.5f);
                float result2 = result2H / result2L;
                float combined = result1 * result2 + 1;
                float final = combined * modTraded * modLuckyEgg * modPassPower;
                GiveEXP(victor, (uint)final);
            }
        }
        private void GiveEXP(PBEBattlePokemon victor, uint amount)
        {
            // TODO: Should we allow remote battles with learning moves? No packets right now
            BroadcastPkmnEXPEarned(victor, amount);
            PBEGrowthRate growthRate = PBEDataProvider.Instance.GetPokemonData(victor).GrowthRate;
        top:
            uint oldEXP = victor.EXP;
            uint nextLevelAmt = PBEDataProvider.Instance.GetEXPRequired(growthRate, (byte)(victor.Level + 1));
            if (oldEXP + amount >= nextLevelAmt)
            {
                victor.EXP = nextLevelAmt;
                BroadcastPkmnEXPChanged(victor, oldEXP);
                victor.Level++;
                victor.SetStats(true, false);
                BroadcastPkmnLevelChanged(victor);
                // BUG: PBEStatus2.PowerTrick is not cleared when leveling up, even though the stats are replaced (meaning it can still be baton passed)
                if (Settings.BugFix && victor.Status2.HasFlag(PBEStatus2.PowerTrick))
                {
                    BroadcastStatus2(victor, victor, PBEStatus2.PowerTrick, PBEStatusAction.Ended);
                }
                if (victor.Level == Settings.MaxLevel)
                {
                    return;
                }
                uint grewBy = nextLevelAmt - oldEXP;
                amount -= grewBy;
                if (amount > 0)
                {
                    goto top; // Keep gaining and leveling
                }
            }
            else
            {
                victor.EXP = oldEXP + amount;
                BroadcastPkmnEXPChanged(victor, oldEXP);
            }
        }

        private static float PokedexCountTable(int count, float g600, float g450, float g300, float g150, float g30, float ge0)
        {
            if (count > 600)
            {
                return g600;
            }
            if (count > 450)
            {
                return g450;
            }
            if (count > 300)
            {
                return g300;
            }
            if (count > 150)
            {
                return g150;
            }
            if (count > 30)
            {
                return g30;
            }
            return ge0;
        }
        private void GenerateCapture(PBEBattlePokemon user, PBEBattlePokemon wildPkmn, PBEItem ball, out byte shakes, out bool success, out bool isCriticalCapture)
        {
            if (PBEDataProvider.Instance.IsGuaranteedCapture(this, wildPkmn.OriginalSpecies, wildPkmn.RevertForm))
            {
                shakes = 3;
                success = true;
                isCriticalCapture = false;
                return;
            }
            IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(wildPkmn.OriginalSpecies, wildPkmn.RevertForm);
            float rate = pData.CatchRate * PBEDataProvider.Instance.GetCatchRateModifier(this);
            float bonusBall = 1;
            switch (ball)
            {
                case PBEItem.GreatBall:
                case PBEItem.SafariBall:
                case PBEItem.SportBall: bonusBall = 1.5f; break;
                case PBEItem.UltraBall: bonusBall = 2; break;
                case PBEItem.MasterBall:
                case PBEItem.ParkBall: bonusBall = 255; break;
                case PBEItem.FastBall:
                {
                    if (wildPkmn.Speed >= 100)
                    {
                        rate *= 4;
                    }
                    break;
                }
                case PBEItem.LevelBall:
                {
                    int wl = wildPkmn.Level;
                    int ul = user.Level;
                    if (ul > wl * 4)
                    {
                        rate *= 8;
                    }
                    else if (ul > wl * 2)
                    {
                        rate *= 4;
                    }
                    else if (ul > wl)
                    {
                        rate *= 2;
                    }
                    break;
                }
                case PBEItem.LureBall:
                {
                    if (PBEDataProvider.Instance.IsFishing(this))
                    {
                        rate *= 3;
                    }
                    break;
                }
                case PBEItem.HeavyBall:
                {
                    float weight = pData.Weight;
                    if (weight >= 409.6f)
                    {
                        rate += 40;
                    }
                    else if (weight >= 307.2f)
                    {
                        rate += 30;
                    }
                    else if (weight >= 204.8f)
                    {
                        rate += 20;
                    }
                    else
                    {
                        rate -= 20;
                    }
                    break;
                }
                case PBEItem.LoveBall:
                {
                    if (user.Species == wildPkmn.Species && user.Gender.IsOppositeGender(wildPkmn.Gender))
                    {
                        rate *= 8;
                    }
                    break;
                }
                case PBEItem.MoonBall:
                {
                    if (PBEDataProvider.Instance.IsMoonBallFamily(wildPkmn.OriginalSpecies, wildPkmn.RevertForm))
                    {
                        rate *= 4;
                    }
                    break;
                }
                case PBEItem.NetBall:
                {
                    if (wildPkmn.HasType(PBEType.Bug) || wildPkmn.HasType(PBEType.Water))
                    {
                        bonusBall = 3;
                    }
                    break;
                }
                case PBEItem.NestBall:
                {
                    bonusBall = Math.Max(1, (41 - wildPkmn.Level) / 10);
                    break;
                }
                case PBEItem.RepeatBall:
                {
                    if (PBEDataProvider.Instance.IsRepeatBallSpecies(wildPkmn.OriginalSpecies))
                    {
                        bonusBall = 3;
                    }
                    break;
                }
                case PBEItem.TimerBall:
                {
                    bonusBall = Math.Min(4, 1 + (TurnNumber * 0.3f));
                    break;
                }
                case PBEItem.DiveBall:
                {
                    if (PBEDataProvider.Instance.IsFishing(this) || PBEDataProvider.Instance.IsSurfing(this) || PBEDataProvider.Instance.IsUnderwater(this))
                    {
                        bonusBall = 3.5f;
                    }
                    break;
                }
                case PBEItem.DuskBall:
                {
                    if (PBEDataProvider.Instance.IsDuskBallSetting(this))
                    {
                        bonusBall = 3.5f;
                    }
                    break;
                }
                case PBEItem.QuickBall:
                {
                    if (TurnNumber == 1)
                    {
                        bonusBall = 5;
                    }
                    break;
                }
            }
            rate = Math.Clamp(rate, 1, 255);
            float bonusStatus;
            switch (wildPkmn.Status1)
            {
                case PBEStatus1.Asleep:
                case PBEStatus1.Frozen: bonusStatus = 2.5f; break;
                case PBEStatus1.None: bonusStatus = 1; break;
                default: bonusStatus = 1.5f; break;
            }
            float pkmnFactor = (3 * wildPkmn.MaxHP) - (2 * wildPkmn.HP);
            int pkmnCaught = PBEDataProvider.Instance.GetSpeciesCaught();
            if (PBEDataProvider.Instance.IsDarkGrass(this))
            {
                pkmnFactor *= PokedexCountTable(pkmnCaught, 1, 0.9f, 0.8f, 0.7f, 0.5f, 0.3f);
            }
            float a = pkmnFactor * rate * bonusBall / (3 * wildPkmn.MaxHP) * bonusStatus;
            float c = a * PokedexCountTable(pkmnCaught, 2.5f, 2, 1.5f, 1, 0.5f, 0); // Critical capture modifier
            isCriticalCapture = _rand.RandomInt(0, 0xFF) < c / 6;
            byte numShakes = isCriticalCapture ? (byte)1 : (byte)3;
            if (a >= 0xFF)
            {
                shakes = numShakes; // Skip shake checks
                success = true;
                return;
            }
            float b = 0x10000 / MathF.Sqrt(MathF.Sqrt(0xFF / a));
            for (shakes = 0; shakes < numShakes; shakes++)
            {
                if (_rand.RandomInt(0, 0xFFFF) >= b)
                {
                    break; // Shake check fails
                }
            }
            success = shakes == numShakes;
            if (shakes == 2)
            {
                shakes = 3; // If there are only 2 shakes and a failure, shake three times and still fail
            }
        }
        private void UseItem(PBEBattlePokemon user, PBEItem item)
        {
            BroadcastItemTurn(user, item, PBEItemTurnAction.Attempt);
            if (PBEDataUtils.AllBalls.Contains(item))
            {
                user.Trainer.Inventory.Remove(item);
                if (BattleType != PBEBattleType.Wild)
                {
                    goto fail;
                }
                PBEBattlePokemon wildPkmn = user.Team.OpposingTeam.ActiveBattlers.Single();
                GenerateCapture(user, wildPkmn, item, out byte numShakes, out bool success, out bool critical);
                BroadcastCapture(wildPkmn, item, numShakes, success, critical);
                if (success)
                {
                    wildPkmn.CaughtBall = wildPkmn.KnownCaughtBall = item;
                    BattleResult = PBEBattleResult.WildCapture;
                }
                return;
            }
            else
            {
                switch (item)
                {
                    case PBEItem.FluffyTail:
                    case PBEItem.PokeDoll:
                    case PBEItem.PokeToy:
                    {
                        user.Trainer.Inventory.Remove(item);
                        if (BattleType == PBEBattleType.Wild)
                        {
                            SetEscaped(user);
                            return;
                        }
                        goto fail;
                    }
                }
            }
        fail:
            BroadcastItemTurn(user, item, PBEItemTurnAction.NoEffect);
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
            IPBEMoveData mData = PBEDataProvider.Instance.GetMoveData(move);
            PBEBattlePokemon[] targets = GetRuntimeTargets(user, requestedTargets, user.GetMoveTargets(mData) == PBEMoveTarget.SingleNotSelf, _rand);
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
                case PBEMoveEffect.Payback:
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
                case PBEMoveEffect.SmellingSalt: Ef_SmellingSalt(user, targets, move, mData); break;
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
                case PBEMoveEffect.WakeUpSlap: Ef_WakeUpSlap(user, targets, move, mData); break;
                case PBEMoveEffect.Whirlwind: Ef_Whirlwind(user, targets, move, mData); break;
                case PBEMoveEffect.WideGuard: Ef_TryForceTeamStatus(user, move, mData, PBETeamStatus.WideGuard); break;
                case PBEMoveEffect.WorrySeed: Ef_SetOtherAbility(user, targets, move, mData, PBEAbility.Insomnia, true); break;
                default: throw new InvalidDataException(nameof(mData.Effect));
            }
        }

        private bool PreMoveStatusCheck(PBEBattlePokemon user, PBEMove move)
        {
            IPBEMoveData mData = PBEDataProvider.Instance.GetMoveData(move);

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
                if (mData.Flags.HasFlag(PBEMoveFlag.DefrostsUser) || _rand.RandomBool(20, 100))
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
                    if (_rand.RandomBool(50, 100))
                    {
                        int damage = CalculateConfusionDamage(user);
                        DealDamage(user, user, damage, ignoreSturdy: false);
                        BroadcastStatus2(user, user, PBEStatus2.Confused, PBEStatusAction.Damage);
                        // BUG: Confusion damage does not activate these items
                        if (!FaintCheck(user) && Settings.BugFix)
                        {
                            LowHPBerryCheck(user);
                        }
                        return true;
                    }
                }
            }
            // Verified: Paralysis before Infatuation
            if (user.Status1 == PBEStatus1.Paralyzed && _rand.RandomBool(25, 100))
            {
                BroadcastStatus1(user, user, PBEStatus1.Paralyzed, PBEStatusAction.CausedImmobility);
                return true;
            }
            // Infatuation
            if (user.Status2.HasFlag(PBEStatus2.Infatuated))
            {
                BroadcastStatus2(user, user.InfatuatedWithPokemon!, PBEStatus2.Infatuated, PBEStatusAction.Announced);
                if (_rand.RandomBool(50, 100))
                {
                    BroadcastStatus2(user, user.InfatuatedWithPokemon!, PBEStatus2.Infatuated, PBEStatusAction.CausedImmobility);
                    return true;
                }
            }
            return false;
        }
        private bool MissCheck(PBEBattlePokemon user, PBEBattlePokemon target, IPBEMoveData mData)
        {
            if (user == target)
            {
                return false;
            }
            // Verified: WideGuard happens before Protect
            if (target.Team.TeamStatus.HasFlag(PBETeamStatus.WideGuard) && mData.Category != PBEMoveCategory.Status && PBEDataUtils.IsSpreadMove(user.GetMoveTargets(mData)))
            {
                BroadcastTeamStatusDamage(target.Team, PBETeamStatus.WideGuard, target);
                return true;
            }
            // Feint ignores Quick Guard unless the target is an ally
            if (target.Team.TeamStatus.HasFlag(PBETeamStatus.QuickGuard) && mData.Priority > 0 && (mData.Effect != PBEMoveEffect.Feint || user.Team == target.Team))
            {
                BroadcastTeamStatusDamage(target.Team, PBETeamStatus.QuickGuard, target);
                return true;
            }
            if (target.Status2.HasFlag(PBEStatus2.Protected) && mData.Flags.HasFlag(PBEMoveFlag.AffectedByProtect))
            {
                BroadcastStatus2(target, user, PBEStatus2.Protected, PBEStatusAction.Damage);
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
            float chance = mData.Accuracy;
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
            float accuracy = ignoreA ? 1 : GetStatChangeModifier(user.AccuracyChange, true);
            float evasion;
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
                chance *= 1.3f;
            }
            if (user.Team.ActiveBattlers.FindIndex(p => p.Ability == PBEAbility.VictoryStar) != -1)
            {
                chance *= 1.1f;
            }
            if (user.Ability == PBEAbility.Hustle && mData.Category == PBEMoveCategory.Physical)
            {
                chance *= 0.8f;
            }
            if (!user.HasCancellingAbility() && ShouldDoWeatherEffects())
            {
                if (Weather == PBEWeather.Sandstorm && target.Ability == PBEAbility.SandVeil)
                {
                    chance *= 0.8f;
                }
                if (Weather == PBEWeather.Hailstorm && target.Ability == PBEAbility.SnowCloak)
                {
                    chance *= 0.8f;
                }
            }
            if (target.Item == PBEItem.BrightPowder)
            {
                chance *= 0.9f;
            }
            if (target.Item == PBEItem.LaxIncense)
            {
                chance *= 0.9f;
            }
            if (user.Item == PBEItem.WideLens)
            {
                chance *= 1.1f;
            }
            if (target.Ability == PBEAbility.TangledFeet && target.Status2.HasFlag(PBEStatus2.Confused) && !user.HasCancellingAbility())
            {
                chance *= 0.5f;
            }
        roll:
            if (_rand.RandomBool((int)chance, 100))
            {
                return false;
            }
        miss:
            BroadcastMoveResult(user, target, PBEResult.Missed);
            return true;
        }
        private bool AttackTypeCheck(PBEBattlePokemon user, PBEBattlePokemon target, PBEType moveType, out PBEResult result, out float damageMultiplier)
        {
            result = PBETypeEffectiveness.IsAffectedByAttack(user, target, moveType, out damageMultiplier);
            if (result == PBEResult.Ineffective_Ability)
            {
                BroadcastAbility(target, user, target.Ability, PBEAbilityAction.Damage);
            }
            if (result != PBEResult.NotVeryEffective_Type && result != PBEResult.Success && result != PBEResult.SuperEffective_Type)
            {
                BroadcastMoveResult(user, target, result);
                return false;
            }
            return true;
        }
        private bool CritCheck(PBEBattlePokemon user, PBEBattlePokemon target, IPBEMoveData mData)
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
            float chance;
            switch (stage)
            {
                case 0: chance = 6.25f; break;
                case 1: chance = 12.5f; break;
                case 2: chance = 25; break;
                case 3: chance = 33.3f; break;
                default: chance = 50; break;
            }
            return _rand.RandomBool((int)(chance * 100), 100 * 100);
        }
        private void TrySetLoser(PBEBattlePokemon pkmn)
        {
            if (BattleResult is null && pkmn.Team.NumConsciousPkmn == 0)
            {
                BattleResult = pkmn.Team.Id == 0 ? PBEBattleResult.Team1Win : PBEBattleResult.Team0Win;
            }
        }
        private void SetEscaped(PBEBattlePokemon pkmn)
        {
            BattleResult = pkmn.IsWild ? PBEBattleResult.WildFlee : PBEBattleResult.WildEscape;
        }
        private bool FaintCheck(PBEBattlePokemon pkmn)
        {
            if (pkmn.HP == 0)
            {
                _turnOrder.Remove(pkmn);
                ActiveBattlers.Remove(pkmn);
                PBEFieldPosition oldPos = pkmn.FieldPosition;
                pkmn.ClearForFaint();
                BroadcastPkmnFainted(pkmn, oldPos);
                RemoveInfatuationsAndLockOns(pkmn);
                CalcEXP(pkmn);
                pkmn.EXPPokemon.Clear();
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
            return _rand.RandomBool(chance, 100);
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
                    foreach (PBEBattlePokemon opponent in pkmn.Team.OpposingTeam.ActiveBattlers)
                    {
                        foreach (PBEBattleMoveset.PBEBattleMovesetSlot moveSlot in opponent.Moves)
                        {
                            PBEMove move = moveSlot.Move;
                            if (move != PBEMove.None)
                            {
                                IPBEMoveData mData = PBEDataProvider.Instance.GetMoveData(move);
                                if (mData.Category != PBEMoveCategory.Status)
                                {
                                    float d = PBETypeEffectiveness.GetEffectiveness(mData.Type, pkmn);
                                    if (d > 1)
                                    {
                                        BroadcastAbility(pkmn, pkmn, PBEAbility.Anticipation, PBEAbilityAction.Announced);
                                        goto bottomAnticipation;
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
                    List<PBEBattlePokemon> oppActive = pkmn.Team.OpposingTeam.ActiveBattlers;
                    if (oppActive.Count != 0)
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
                    if (pkmn.Team.OpposingTeam.TryGetPokemon(targetPos, out PBEBattlePokemon? target)
                        && target.IsTransformPossible(pkmn) == PBEResult.Success)
                    {
                        BroadcastAbility(pkmn, target, pkmn.Ability, PBEAbilityAction.ChangedAppearance);
                        DoTransform(pkmn, target);
                    }
                    break;
                }
                case PBEAbility.Intimidate:
                {
                    // Verified: Do not announce if the positions are empty
                    IReadOnlyList<PBEBattlePokemon> targets = GetRuntimeSurrounding(pkmn, false, true);
                    if (targets.Count > 0)
                    {
                        // Verified: Announce even if nobody is lowered (due to Substitute, Minimized Attack, or Ability)
                        BroadcastAbility(pkmn, pkmn, PBEAbility.Intimidate, PBEAbilityAction.Stats);
                        foreach (PBEBattlePokemon target in GetActingOrder(targets, true))
                        {
                            ApplyStatChangeIfPossible(pkmn, target, PBEStat.Attack, -1); // Verified: Substitute, Minimized Attack, and Ability are announced
                        }
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
        private void CastformCherrimCheck(IEnumerable<PBEBattlePokemon> order)
        {
            foreach (PBEBattlePokemon pkmn in order)
            {
                CastformCherrimCheck(pkmn);
            }
        }
        private void CastformCherrimCheck(PBEBattlePokemon pkmn)
        {
            if (pkmn.HP == 0)
            {
                return; // #344 - Castform/Cherrim can change form while fainting from Explosion, if they kill someone with a weather-blocking ability
            }
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
                PBEAbility newAbility = PBEDataProvider.Instance.GetPokemonData(PBESpecies.Shaymin, newForm).Abilities[0];
                BroadcastPkmnFormChanged(pkmn, newForm, newAbility, PBEAbility.MAX, true);
                ActivateAbility(pkmn, false);
            }
        }
        private void IllusionBreak(PBEBattlePokemon pkmn, PBEBattlePokemon breaker)
        {
            if (pkmn.Status2.HasFlag(PBEStatus2.Disguised))
            {
                pkmn.KnownGender = pkmn.Gender;
                pkmn.KnownCaughtBall = pkmn.CaughtBall;
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
        private void AntiStatusAbilityCheck(IEnumerable<PBEBattlePokemon> order)
        {
            foreach (PBEBattlePokemon pkmn in order)
            {
                AntiStatusAbilityCheck(pkmn);
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
            target.ConfusionTurns = (byte)_rand.RandomInt(Settings.ConfusionMinTurns, Settings.ConfusionMaxTurns);
            BroadcastStatus2(target, other, PBEStatus2.Confused, PBEStatusAction.Added);
            AntiStatusAbilityCheck(target);
        }
        private void CauseInfatuation(PBEBattlePokemon target, PBEBattlePokemon other)
        {
            target.InfatuatedWithPokemon = other;
            BroadcastStatus2(target, other, PBEStatus2.Infatuated, PBEStatusAction.Added);
            if (target.Item == PBEItem.DestinyKnot && other.IsAttractionPossible(target) == PBEResult.Success)
            {
                BroadcastItem(target, other, PBEItem.DestinyKnot, PBEItemAction.Announced);
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
        private void LowHPBerryCheck(IEnumerable<PBEBattlePokemon> order, PBEBattlePokemon? forcedToEatBy = null)
        {
            foreach (PBEBattlePokemon pkmn in order)
            {
                LowHPBerryCheck(pkmn, forcedToEatBy: forcedToEatBy);
            }
        }
        private void LowHPBerryCheck(PBEBattlePokemon pkmn, PBEBattlePokemon? forcedToEatBy = null)
        {
            forcedToEatBy ??= pkmn;
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
                if (pkmn.IsStatChangePossible(stat, forcedToEatBy, change, out sbyte oldValue, out sbyte newValue, ignoreSubstitute: true) == PBEResult.Success)
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
                        List<PBEStat> statsThatCanGoUp = PBEDataUtils.StarfBerryStats.FindAll(s => pkmn.GetStatChange(s) < Settings.MaxStatChange);
                        if (statsThatCanGoUp.Count > 0)
                        {
                            DoStatItem(_rand.RandomElement(statsThatCanGoUp), +2);
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

        private void RecordExecutedMove(PBEBattlePokemon user, PBEMove move, IPBEMoveData mData)
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
                PBEBattleMoveset.PBEBattleMovesetSlot slot = pkmn.Moves[move]!;
                int oldPP = slot.PP;
                int newPP = Math.Max(0, oldPP - amountToReduce);
                int amountReduced = oldPP - newPP;
                slot.PP = newPP;
                pkmn.UpdateKnownPP(move, amountReduced);
                BroadcastMovePPChanged(pkmn, move, amountReduced);
            }
        }

        private static void ApplyBigRoot(PBEBattlePokemon pkmn, ref int restoreAmt)
        {
            if (pkmn.Item == PBEItem.BigRoot)
            {
                restoreAmt += (int)(restoreAmt * 0.3);
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
            pkmn.SleepTurns = (byte)(_rand.RandomInt(minTurns, maxTurns) / (pkmn.Ability == PBEAbility.EarlyBird ? 2 : 1));
        }
        private void DoTransform(PBEBattlePokemon user, PBEBattlePokemon target)
        {
            user.Transform(target);
            BroadcastTransform(user, target);
            BroadcastStatus2(user, target, PBEStatus2.Transformed, PBEStatusAction.Added);
            // Remove power trick (so it cannot be baton passed)
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
                        DealDamage(user, user, user.MaxHP / 2);
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
                    if (_rand.RandomBool(user.GetProtectionChance(), ushort.MaxValue))
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
                        DealDamage(user, target, hpRequired);
                        LowHPBerryCheck(target); // Verified: Berry is eaten between damage and substitute
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
                if (result == PBEResult.Ineffective_Substitute)
                {
                    if (!isSecondaryEffect)
                    {
                        BroadcastMoveResult(user, target, PBEResult.Ineffective_Substitute);
                    }
                    return;
                }
                // Do not broadcast "could not be lowered!" for Mud-Slap, etc
                broadcast = !isSecondaryEffect;
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

        private static PBEPkmnAppearedInfo CreateSwitchInInfo(PBEBattlePokemon pkmn)
        {
            if (pkmn.Ability == PBEAbility.Illusion)
            {
                PBEBattlePokemon? p = pkmn.GetPkmnWouldDisguiseAs();
                if (p is not null)
                {
                    pkmn.Status2 |= PBEStatus2.Disguised; // No broadcast, not known
                    pkmn.KnownGender = p.Gender;
                    pkmn.KnownCaughtBall = p.CaughtBall;
                    pkmn.KnownNickname = p.Nickname;
                    pkmn.KnownShiny = p.Shiny;
                    pkmn.KnownSpecies = p.OriginalSpecies;
                    pkmn.KnownForm = p.Form;
                    IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(pkmn.KnownSpecies, pkmn.KnownForm);
                    pkmn.KnownType1 = pData.Type1;
                    pkmn.KnownType2 = pData.Type2;
                }
            }
            return new PBEPkmnAppearedInfo(pkmn);
        }
        private void SwitchTwoPokemon(PBEBattlePokemon pkmnLeaving, PBEBattlePokemon pkmnComing, PBEBattlePokemon? forcedByPkmn = null)
        {
            _turnOrder.Remove(pkmnLeaving);
            ActiveBattlers.Remove(pkmnLeaving);
            PBEFieldPosition pos = pkmnLeaving.FieldPosition;
            pkmnLeaving.ClearForSwitch();
            BroadcastPkmnSwitchOut(pkmnLeaving, pos, forcedByPkmn);
            RemoveInfatuationsAndLockOns(pkmnLeaving);
            pkmnComing.FieldPosition = pos;
            var switches = new PBEPkmnAppearedInfo[] { CreateSwitchInInfo(pkmnComing) };
            PBETrainer.SwitchTwoPokemon(pkmnLeaving, pkmnComing); // Swap after Illusion
            ActiveBattlers.Add(pkmnComing); // Add to active before broadcast
            BroadcastPkmnSwitchIn(pkmnComing.Trainer, switches, forcedByPkmn);
            if (forcedByPkmn is not null)
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

        private void SemiInvulnerableChargeMove(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, PBETurnTarget requestedTargets, PBEStatus2 status2,
            Action<PBEBattlePokemon, ushort>? beforePostHit = null,
            Action<PBEBattlePokemon>? afterPostHit = null)
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

        private void Ef_TryForceStatus1(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, PBEStatus1 status, bool thunderWave = false)
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
                        AntiStatusAbilityCheck(target);
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
            return;
        }
        private void Ef_TryForceStatus2(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, PBEStatus2 status)
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
                        AntiStatusAbilityCheck(target);
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_TryForceBattleStatus(PBEBattlePokemon user, PBEMove move, IPBEMoveData mData, PBEBattleStatus status)
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
        private void Ef_TryForceTeamStatus(PBEBattlePokemon user, PBEMove move, IPBEMoveData mData, PBETeamStatus status)
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
                    if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.QuickGuard) && _rand.RandomBool(user.GetProtectionChance(), ushort.MaxValue))
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
                    if (!user.Team.TeamStatus.HasFlag(PBETeamStatus.WideGuard) && _rand.RandomBool(user.GetProtectionChance(), ushort.MaxValue))
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
        private void Ef_TryForceWeather(PBEBattlePokemon user, PBEMove move, IPBEMoveData mData, PBEWeather weather)
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

        private void Ef_Growth(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            int change = WillLeafGuardActivate() ? +2 : +1;
            Ef_ChangeTargetStats(user, targets, move, mData, new[] { (PBEStat.Attack, change), (PBEStat.SpAttack, change) });
        }
        private void Ef_ChangeTargetStats(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, (PBEStat, int)[] statChanges, bool requireAttraction = false)
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
        private void Ef_Hit__MaybeChangeTargetStats(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, (PBEStat, int)[] statChanges, int chanceToChangeStats)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEBattlePokemon target, ushort damageDealt)
                {
                    if (target.HP == 0 || !GetManipulableChance(user, chanceToChangeStats))
                    {
                        return;
                    }
                    for (int i = 0; i < statChanges.Length; i++)
                    {
                        (PBEStat stat, int change) = statChanges[i];
                        ApplyStatChangeIfPossible(user, target, stat, change, isSecondaryEffect: true);
                    }
                }
                BasicHit(user, targets, mData, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Hit__MaybeChangeUserStats(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, (PBEStat, int)[] statChanges, int chanceToChangeStats)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEBattlePokemon target, ushort damageDealt)
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
                BasicHit(user, targets, mData, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, mData);
        }

        private void Ef_Entrainment(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
        private void Ef_RolePlay(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
        private void Ef_SetOtherAbility(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, PBEAbility ability, bool blockedByTruant)
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

        private void Ef_Bounce(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, PBETurnTarget requestedTargets)
        {
            void BeforePostHit(PBEBattlePokemon target, ushort damageDealt)
            {
                if (target.HP > 0 && GetManipulableChance(user, mData.EffectParam))
                {
                    ApplyStatus1IfPossible(user, target, PBEStatus1.Paralyzed, false);
                }
            }
            SemiInvulnerableChargeMove(user, targets, move, mData, requestedTargets, PBEStatus2.Airborne, beforePostHit: BeforePostHit);
        }
        private void Ef_ShadowForce(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, PBETurnTarget requestedTargets)
        {
            void AfterPostHit(PBEBattlePokemon target)
            {
                if (target.HP > 0 && target.Status2.HasFlag(PBEStatus2.Protected))
                {
                    BroadcastStatus2(target, user, PBEStatus2.Protected, PBEStatusAction.Cleared);
                }
                if (target.Team.TeamStatus.HasFlag(PBETeamStatus.QuickGuard))
                {
                    BroadcastTeamStatus(target.Team, PBETeamStatus.QuickGuard, PBETeamStatusAction.Cleared);
                }
                if (target.Team.TeamStatus.HasFlag(PBETeamStatus.WideGuard))
                {
                    BroadcastTeamStatus(target.Team, PBETeamStatus.WideGuard, PBETeamStatusAction.Cleared);
                }
            }
            SemiInvulnerableChargeMove(user, targets, move, mData, requestedTargets, PBEStatus2.ShadowForce, afterPostHit: AfterPostHit);
        }

        private void Ef_BrickBreak(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforeDoingDamage(PBEBattlePokemon target)
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
                }
                BasicHit(user, targets, mData, beforeDoingDamage: BeforeDoingDamage);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Feint(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforeDoingDamage(PBEBattlePokemon target)
                {
                    if (target.HP > 0 && target.Status2.HasFlag(PBEStatus2.Protected))
                    {
                        BroadcastStatus2(target, user, PBEStatus2.Protected, PBEStatusAction.Cleared);
                    }
                    if (target.Team == user.Team.OpposingTeam)
                    {
                        if (target.Team.TeamStatus.HasFlag(PBETeamStatus.QuickGuard))
                        {
                            BroadcastTeamStatus(target.Team, PBETeamStatus.QuickGuard, PBETeamStatusAction.Cleared);
                        }
                        if (target.Team.TeamStatus.HasFlag(PBETeamStatus.WideGuard))
                        {
                            BroadcastTeamStatus(target.Team, PBETeamStatus.WideGuard, PBETeamStatusAction.Cleared);
                        }
                    }
                }
                BasicHit(user, targets, mData, beforeDoingDamage: BeforeDoingDamage);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Hit(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, PBEStatus1 status1 = PBEStatus1.None, int chanceToInflictStatus1 = 0, PBEStatus2 status2 = PBEStatus2.None, int chanceToInflictStatus2 = 0)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEBattlePokemon target, ushort damageDealt)
                {
                    if (target.HP == 0)
                    {
                        return;
                    }
                    if (status1 != PBEStatus1.None && GetManipulableChance(user, chanceToInflictStatus1))
                    {
                        ApplyStatus1IfPossible(user, target, status1, false);
                    }
                    if (status2 != PBEStatus2.None && GetManipulableChance(user, chanceToInflictStatus2))
                    {
                        ApplyStatus2IfPossible(user, target, status2, false);
                    }
                }
                BasicHit(user, targets, mData, beforePostHit: status1 != PBEStatus1.None || status2 != PBEStatus2.None ? BeforePostHit : null);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Hit__MaybeBurnFreezeParalyze(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEBattlePokemon target, ushort damageDealt)
                {
                    if (target.HP == 0 || !GetManipulableChance(user, mData.EffectParam))
                    {
                        return;
                    }
                    PBEStatus1 status1;
                    int val = _rand.RandomInt(0, 2);
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
                BasicHit(user, targets, mData, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_MultiHit(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, byte numHits, bool subsequentMissChecks = false, PBEStatus1 status1 = PBEStatus1.None, int chanceToInflictStatus1 = 0)
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
                MultiHit(user, targets, mData, numHits, subsequentMissChecks: subsequentMissChecks, beforePostHit: status1 != PBEStatus1.None ? BeforePostHit : null); // Doesn't need to be its own func but neater
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_MultiHit_2To5(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            byte numHits;
            if (user.Ability == PBEAbility.SkillLink)
            {
                numHits = 5;
            }
            else
            {
                int val = _rand.RandomInt(0, 5);
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
        private void Ef_PayDay(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforeDoingDamage(PBEBattlePokemon target)
                {
                    BroadcastPayDay();
                }
                BasicHit(user, targets, mData, beforeDoingDamage: BeforeDoingDamage);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Recoil(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, PBEStatus1 status1 = PBEStatus1.None, int chanceToInflictStatus1 = 0, PBEStatus2 status2 = PBEStatus2.None, int chanceToInflictStatus2 = 0)
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
                    return user.Ability == PBEAbility.RockHead || totalDamageDealt == 0 ? null : totalDamageDealt / mData.EffectParam;
                }
                void BeforePostHit(PBEBattlePokemon target, ushort damageDealt)
                {
                    if (target.HP == 0)
                    {
                        return;
                    }
                    if (status1 != PBEStatus1.None && GetManipulableChance(user, chanceToInflictStatus1))
                    {
                        ApplyStatus1IfPossible(user, target, status1, false);
                    }
                    if (status2 != PBEStatus2.None && GetManipulableChance(user, chanceToInflictStatus2))
                    {
                        ApplyStatus2IfPossible(user, target, status2, false);
                    }
                }
                BasicHit(user, targets, mData, recoilFunc: RecoilFunc, beforePostHit: status1 != PBEStatus1.None || status2 != PBEStatus2.None ? BeforePostHit : null);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_SecretPower(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void BeforePostHit(PBEBattlePokemon target, ushort damageDealt)
                {
                    // BUG: SecretPower is unaffected by SereneGrace and the Rainbow
                    if (target.HP > 0
                        && (Settings.BugFix
                        ? GetManipulableChance(user, mData.EffectParam)
                        : _rand.RandomBool(mData.EffectParam, 100))
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
                            default: throw new InvalidDataException(nameof(BattleTerrain));
                        }
                    }
                }
                BasicHit(user, targets, mData, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Selfdestruct(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            // TODO: Damp
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            // In gen 5, the user faints first (and loses if possible)
            // Due to technical limitations, we cannot faint first, but we should still make the user lose so it is the same behavior
            DealDamage(user, user, user.MaxHP);
            TrySetLoser(user);
            if (targets.Length == 0) // You still faint if there are no targets
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                BasicHit(user, targets, mData);
            }
            FaintCheck(user); // No berry check because we are always fainted
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_SmellingSalt(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void AfterPostHit(PBEBattlePokemon target)
                {
                    if (target.HP > 0 && target.Status1 == PBEStatus1.Paralyzed)
                    {
                        target.Status1 = PBEStatus1.None;
                        BroadcastStatus1(target, user, PBEStatus1.Paralyzed, PBEStatusAction.Cleared);
                    }
                }
                BasicHit(user, targets, mData, afterPostHit: AfterPostHit);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Snore(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            // TODO: Snore etc fail in BasicHit?
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
                void BeforePostHit(PBEBattlePokemon target, ushort damageDealt)
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
        private void Ef_Struggle(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
        private void Ef_SuckerPunch(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                PBEResult FailFunc(PBEBattlePokemon target)
                {
                    if (target.TurnAction is null // Just switched in, used item, etc
                        || target.HasUsedMoveThisTurn
                        || target.TurnAction.Decision != PBETurnDecision.Fight
                        || PBEDataProvider.Instance.GetMoveData(target.TurnAction.FightMove).Category == PBEMoveCategory.Status)
                    {
                        const PBEResult result = PBEResult.InvalidConditions;
                        BroadcastMoveResult(user, target, result);
                        return result;
                    }
                    return PBEResult.Success;
                }
                BasicHit(user, targets, mData, failFunc: FailFunc);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_WakeUpSlap(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                void AfterPostHit(PBEBattlePokemon target)
                {
                    if (target.HP > 0 && target.Status1 == PBEStatus1.Asleep)
                    {
                        target.Status1 = PBEStatus1.None;
                        target.Status1Counter = 0;
                        target.SleepTurns = 0;
                        BroadcastStatus1(target, user, PBEStatus1.Asleep, PBEStatusAction.Cleared);
                        CureNightmare(target, user);
                    }
                }
                BasicHit(user, targets, mData, afterPostHit: AfterPostHit);
            }
            RecordExecutedMove(user, move, mData);
        }

        private void Ef_Endeavor(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                PBEResult FailFunc(PBEBattlePokemon target)
                {
                    if (target.HP <= user.HP)
                    {
                        const PBEResult result = PBEResult.InvalidConditions;
                        BroadcastMoveResult(user, target, result);
                        return result;
                    }
                    return PBEResult.Success;
                }
                FixedDamageHit(user, targets, mData, DamageFunc, failFunc: FailFunc);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_FinalGambit(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                    // Only faint/deal damage first time
                    if (user.HP > 0)
                    {
                        DealDamage(user, user, oldHP);
                        FaintCheck(user); // No berry check because we are always fainted
                    }
                    return oldHP;
                }
                FixedDamageHit(user, targets, mData, DamageFunc);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_OneHitKnockout(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                PBEResult FailFunc(PBEBattlePokemon target)
                {
                    if (target.Level > user.Level)
                    {
                        const PBEResult result = PBEResult.Ineffective_Level;
                        BroadcastMoveResult(user, target, result);
                        return result;
                    }
                    else
                    {
                        return PBEResult.Success;
                    }
                }
                void BeforePostHit()
                {
                    // This Any is for Sturdy survivors
                    if (Array.FindIndex(targets, p => p.HP == 0) != -1)
                    {
                        BroadcastOneHitKnockout();
                    }
                }
                FixedDamageHit(user, targets, mData, DamageFunc, failFunc: FailFunc, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Psywave(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                    return user.Level * (_rand.RandomInt(0, 100) + 50) / 100;
                }
                FixedDamageHit(user, targets, mData, DamageFunc);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_SeismicToss(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
        private void Ef_SetDamage(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
        private void Ef_SuperFang(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                static int DamageFunc(PBEBattlePokemon target)
                {
                    return target.HP / 2;
                }
                FixedDamageHit(user, targets, mData, DamageFunc);
            }
            RecordExecutedMove(user, move, mData);
        }

        private void Ef_HPDrain(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData, bool requireSleep = false)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            if (targets.Length == 0)
            {
                BroadcastMoveResult(user, user, PBEResult.NoTarget);
            }
            else
            {
                PBEResult FailFunc(PBEBattlePokemon target)
                {
                    if (target.Status1 != PBEStatus1.Asleep)
                    {
                        const PBEResult result = PBEResult.Ineffective_Status;
                        BroadcastMoveResult(user, target, result);
                        return result;
                    }
                    return PBEResult.Success;
                }
                void BeforePostHit(PBEBattlePokemon target, ushort damageDealt)
                {
                    if (user.HP == 0)
                    {
                        return;
                    }
                    int restoreAmt = (int)(damageDealt * (mData.EffectParam / 100.0));
                    ApplyBigRoot(user, ref restoreAmt);
                    if (target.Ability == PBEAbility.LiquidOoze)
                    {
                        // Verified: User faints first here, target faints at normal spot afterwards
                        BroadcastAbility(target, user, PBEAbility.LiquidOoze, PBEAbilityAction.Damage);
                        DealDamage(target, user, restoreAmt);
                        if (!FaintCheck(user))
                        {
                            LowHPBerryCheck(user);
                        }
                    }
                    else if (HealDamage(user, restoreAmt) > 0)
                    {
                        BroadcastHPDrained(target);
                    }
                }
                BasicHit(user, targets, mData, failFunc: requireSleep ? FailFunc : null, beforePostHit: BeforePostHit);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Moonlight(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                        if (HealDamage(user, user.MaxHP / denominator) == 0)
                        {
                            BroadcastMoveResult(user, user, PBEResult.Ineffective_Stat);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_PainSplit(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                                    DealDamage(user, pkmn, pkmn.HP - hp);
                                }
                            }
                            BroadcastPainSplit(user, target);
                            LowHPBerryCheck(user);
                            LowHPBerryCheck(target, forcedToEatBy: user); // Verified: Berry is activated but no illusion breaking
                            // Verified: ColorChange/LifeOrb is ignored
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Rest(PBEBattlePokemon user, PBEMove move, IPBEMoveData mData)
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
        private void Ef_RestoreTargetHP(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                            if (HealDamage(target, (int)(target.MaxHP * (mData.EffectParam / 100.0))) == 0)
                            {
                                BroadcastMoveResult(user, target, PBEResult.Ineffective_Stat);
                            }
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Roost(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                            if (HealDamage(target, (int)(target.MaxHP * (mData.EffectParam / 100.0))) == 0)
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

        private void Ef_BellyDrum(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                        sbyte oldValue = 0, newValue = 0;
                        if (target.HP <= requirement
                            || (Settings.BugFix
                            ? target.IsStatChangePossible(PBEStat.Attack, user, byte.MaxValue, out oldValue, out newValue) != PBEResult.Success
                            : target.AttackChange == Settings.MaxStatChange)
                            )
                        {
                            BroadcastMoveResult(user, target, PBEResult.InvalidConditions);
                        }
                        else
                        {
                            DealDamage(user, target, requirement);
                            if (Settings.BugFix)
                            {
                                SetStatAndBroadcast(target, PBEStat.Attack, oldValue, newValue);
                            }
                            else
                            {
                                ApplyStatChangeIfPossible(user, target, PBEStat.Attack, byte.MaxValue); // byte.MaxValue will work for all PBESettings
                            }
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Camouflage(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                            default: throw new InvalidDataException(nameof(BattleTerrain));
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
        private void Ef_Conversion(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                                PBEType type = PBEDataProvider.Instance.GetMoveData(m).Type;
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
                            BroadcastTypeChanged(target, _rand.RandomElement(available), PBEType.None);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Curse(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                        targets = GetRuntimeTargets(user, moveTarget, false, _rand);
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
        private void Ef_Flatter(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                            ApplyStatChangeIfPossible(user, target, PBEStat.SpAttack, +1);
                            ApplyStatus2IfPossible(user, target, PBEStatus2.Confused, true);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Haze(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
        private void Ef_HelpingHand(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
        private void Ef_Metronome(PBEBattlePokemon user, PBEMove move, IPBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            // Record before the called move is recorded
            RecordExecutedMove(user, move, mData);

            PBEMove calledMove = _rand.RandomElement(PBEDataUtils.MetronomeMoves);
            _calledFromOtherMove = true;
            UseMove(user, calledMove, GetRandomTargetForMetronome(user, calledMove, _rand));
            _calledFromOtherMove = false;
        }
        private void Ef_Nothing(PBEBattlePokemon user, PBEMove move, IPBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            BroadcastNothingHappened();
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_PsychUp(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
        private void Ef_ReflectType(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
        private void Ef_Refresh(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
        private void Ef_Soak(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
        private void Ef_Swagger(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                            ApplyStatChangeIfPossible(user, target, PBEStat.Attack, +2);
                            ApplyStatus2IfPossible(user, target, PBEStatus2.Confused, true);
                        }
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Teleport(PBEBattlePokemon user, PBEMove move, IPBEMoveData mData)
        {
            BroadcastMoveUsed(user, move);
            PPReduce(user, move);
            // TODO: Trapping effects, SmokeBall
            // In gen 5 there is a bug that prevents wild Pokémon holding a SmokeBall from escaping if they are affected by trapping effects
            if (BattleType == PBEBattleType.Wild && BattleFormat == PBEBattleFormat.Single)
            {
                SetEscaped(user);
            }
            else
            {
                BroadcastMoveResult(user, user, PBEResult.InvalidConditions);
            }
            RecordExecutedMove(user, move, mData);
        }
        private void Ef_Whirlwind(PBEBattlePokemon user, PBEBattlePokemon[] targets, PBEMove move, IPBEMoveData mData)
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
                    if (MissCheck(user, target, mData))
                    {
                        continue;
                    }
                    // TODO: Trapping effects
                    if (BattleType == PBEBattleType.Wild)
                    {
                        if (BattleFormat == PBEBattleFormat.Single)
                        {
                            // Wild single battle requires user's level to be >= target's level, then it'll end the battle
                            if (user.Level < target.Level)
                            {
                                BroadcastMoveResult(user, target, PBEResult.Ineffective_Level);
                                continue;
                            }
                            SetEscaped(target);
                            break;
                        }
                        else
                        {
                            // Trainer using whirlwind in a wild double+ battle will cause it to fail (even if there's only one wild Pokémon left)
                            if (!user.IsWild)
                            {
                                BroadcastMoveResult(user, target, PBEResult.InvalidConditions);
                                continue;
                            }
                            // A wild Pokémon using it will cause it to switch the target out (as normal below)
                        }
                    }
                    List<PBEBattlePokemon> possibleSwitcheroonies = target.Trainer.Party.FindAll(p => p.FieldPosition == PBEFieldPosition.None && p.CanBattle);
                    if (possibleSwitcheroonies.Count == 0)
                    {
                        BroadcastMoveResult(user, target, PBEResult.InvalidConditions);
                    }
                    else
                    {
                        SwitchTwoPokemon(target, _rand.RandomElement(possibleSwitcheroonies), user);
                    }
                }
            }
            RecordExecutedMove(user, move, mData);
        }
    }
}
