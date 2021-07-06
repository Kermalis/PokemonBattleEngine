using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        /// <summary>Gets the influence a stat change has on a stat.</summary>
        /// <param name="change">The stat change.</param>
        /// <param name="forMissing">True if the stat is <see cref="PBEStat.Accuracy"/> or <see cref="PBEStat.Evasion"/>.</param>
        public static float GetStatChangeModifier(sbyte change, bool forMissing)
        {
            float baseVal = forMissing ? 3 : 2;
            float numerator = Math.Max(baseVal, baseVal + change);
            float denominator = Math.Max(baseVal, baseVal - change);
            return numerator / denominator;
        }

        // Verified: Sturdy and Substitute only activate on damaging attacks (so draining HP or liquid ooze etc can bypass sturdy)
        private ushort DealDamage(PBEBattlePokemon culprit, PBEBattlePokemon victim, int hp, bool ignoreSubstitute = true, bool ignoreSturdy = true)
        {
            if (hp < 1)
            {
                hp = 1;
            }
            if (!ignoreSubstitute && victim.Status2.HasFlag(PBEStatus2.Substitute))
            {
                ushort oldSubHP = victim.SubstituteHP;
                victim.SubstituteHP = (ushort)Math.Max(0, victim.SubstituteHP - hp);
                ushort damageAmt = (ushort)(oldSubHP - victim.SubstituteHP);
                BroadcastStatus2(victim, culprit, PBEStatus2.Substitute, PBEStatusAction.Damage);
                return damageAmt;
            }
            ushort oldHP = victim.HP;
            float oldPercentage = victim.HPPercentage;
            victim.HP = (ushort)Math.Max(0, victim.HP - hp);
            bool sturdyHappened = false, focusBandHappened = false, focusSashHappened = false;
            if (!ignoreSturdy && victim.HP == 0)
            {
                // TODO: Endure
                if (oldHP == victim.MaxHP && victim.Ability == PBEAbility.Sturdy && !culprit.HasCancellingAbility())
                {
                    sturdyHappened = true;
                    victim.HP = 1;
                }
                else if (victim.Item == PBEItem.FocusBand && _rand.RandomBool(10, 100))
                {
                    focusBandHappened = true;
                    victim.HP = 1;
                }
                else if (oldHP == victim.MaxHP && victim.Item == PBEItem.FocusSash)
                {
                    focusSashHappened = true;
                    victim.HP = 1;
                }
            }
            victim.UpdateHPPercentage();
            BroadcastPkmnHPChanged(victim, oldHP, oldPercentage);
            if (sturdyHappened)
            {
                BroadcastAbility(victim, culprit, PBEAbility.Sturdy, PBEAbilityAction.Damage);
                BroadcastEndure(victim);
            }
            else if (focusBandHappened)
            {
                BroadcastItem(victim, culprit, PBEItem.FocusBand, PBEItemAction.Damage);
            }
            else if (focusSashHappened)
            {
                BroadcastItem(victim, culprit, PBEItem.FocusSash, PBEItemAction.Consumed);
            }
            return (ushort)(oldHP - victim.HP);
        }
        /// <summary>Restores HP to <paramref name="pkmn"/> and broadcasts the HP changing if it changes.</summary>
        /// <param name="pkmn">The Pokémon receiving the HP.</param>
        /// <param name="hp">The amount of HP <paramref name="pkmn"/> will try to gain.</param>
        /// <returns>The amount of HP restored.</returns>
        private ushort HealDamage(PBEBattlePokemon pkmn, int hp)
        {
            if (hp < 1)
            {
                hp = 1;
            }
            ushort oldHP = pkmn.HP;
            float oldPercentage = pkmn.HPPercentage;
            pkmn.HP = (ushort)Math.Min(pkmn.MaxHP, pkmn.HP + hp); // Always try to heal at least 1 HP
            ushort healAmt = (ushort)(pkmn.HP - oldHP);
            if (healAmt > 0)
            {
                pkmn.UpdateHPPercentage();
                BroadcastPkmnHPChanged(pkmn, oldHP, oldPercentage);
            }
            return healAmt;
        }

        private float CalculateBasePower(PBEBattlePokemon user, PBEBattlePokemon[] targets, IPBEMoveData mData, PBEType moveType)
        {
            float basePower;

            #region Get move's base power
            switch (mData.Effect)
            {
                case PBEMoveEffect.CrushGrip:
                {
                    basePower = Math.Max(1, targets.Select(t => (float)mData.Power * t.HP / t.MaxHP).Average());
                    break;
                }
                case PBEMoveEffect.Eruption:
                {
                    basePower = Math.Max(1, mData.Power * user.HP / user.MaxHP);
                    break;
                }
                case PBEMoveEffect.Flail:
                {
                    int val = 48 * user.HP / user.MaxHP;
                    if (val < 2)
                    {
                        basePower = 200;
                    }
                    else if (val < 4)
                    {
                        basePower = 150;
                    }
                    else if (val < 8)
                    {
                        basePower = 100;
                    }
                    else if (val < 16)
                    {
                        basePower = 80;
                    }
                    else if (val < 32)
                    {
                        basePower = 40;
                    }
                    else
                    {
                        basePower = 20;
                    }
                    break;
                }
                case PBEMoveEffect.Frustration:
                {
                    basePower = Math.Max(1, (byte.MaxValue - user.Friendship) / 2.5f);
                    break;
                }
                case PBEMoveEffect.GrassKnot:
                {
                    basePower = targets.Select(t =>
                    {
                        if (t.Weight >= 200.0f)
                        {
                            return 120f;
                        }
                        else if (t.Weight >= 100.0f)
                        {
                            return 100f;
                        }
                        else if (t.Weight >= 50.0f)
                        {
                            return 80f;
                        }
                        else if (t.Weight >= 25.0f)
                        {
                            return 60f;
                        }
                        else if (t.Weight >= 10.0f)
                        {
                            return 40f;
                        }
                        return 20f;
                    }).Average();
                    break;
                }
                case PBEMoveEffect.HeatCrash:
                {
                    basePower = targets.Select(t =>
                    {
                        float relative = user.Weight / t.Weight;
                        if (relative < 2)
                        {
                            return 40f;
                        }
                        else if (relative < 3)
                        {
                            return 60f;
                        }
                        else if (relative < 4)
                        {
                            return 80f;
                        }
                        else if (relative < 5)
                        {
                            return 100f;
                        }
                        return 120f;
                    }).Average();
                    break;
                }
                case PBEMoveEffect.HiddenPower:
                {
                    basePower = user.IndividualValues.GetHiddenPowerBasePower(Settings);
                    break;
                }
                case PBEMoveEffect.Magnitude:
                {
                    int val = _rand.RandomInt(0, 99);
                    byte magnitude;
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
                    break;
                }
                case PBEMoveEffect.Punishment:
                {
                    basePower = Math.Max(1, Math.Min(200, targets.Select(t => mData.Power + (20f * t.GetPositiveStatTotal())).Average()));
                    break;
                }
                case PBEMoveEffect.Return:
                {
                    basePower = Math.Max(1, user.Friendship / 2.5f);
                    break;
                }
                case PBEMoveEffect.StoredPower:
                {
                    basePower = mData.Power + (20 * user.GetPositiveStatTotal());
                    break;
                }
                default:
                {
                    basePower = Math.Max(1, (int)mData.Power);
                    break;
                }
            }
            #endregion

            // Technician goes before any other power boosts
            if (user.Ability == PBEAbility.Technician && basePower <= 60)
            {
                basePower *= 1.5f;
            }

            #region Item-specific power boosts
            switch (moveType)
            {
                case PBEType.Bug:
                {
                    switch (user.Item)
                    {
                        case PBEItem.InsectPlate:
                        case PBEItem.SilverPowder:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.BugGem:
                        {
                            BroadcastItem(user, user, PBEItem.BugGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Dark:
                {
                    switch (user.Item)
                    {
                        case PBEItem.BlackGlasses:
                        case PBEItem.DreadPlate:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.DarkGem:
                        {
                            BroadcastItem(user, user, PBEItem.DarkGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Dragon:
                {
                    switch (user.Item)
                    {
                        case PBEItem.AdamantOrb:
                        {
                            if (user.OriginalSpecies == PBESpecies.Dialga)
                            {
                                basePower *= 1.2f;
                            }
                            break;
                        }
                        case PBEItem.DracoPlate:
                        case PBEItem.DragonFang:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.GriseousOrb:
                        {
                            if (user.OriginalSpecies == PBESpecies.Giratina && user.RevertForm == PBEForm.Giratina_Origin)
                            {
                                basePower *= 1.2f;
                            }
                            break;
                        }
                        case PBEItem.LustrousOrb:
                        {
                            if (user.OriginalSpecies == PBESpecies.Palkia)
                            {
                                basePower *= 1.2f;
                            }
                            break;
                        }
                        case PBEItem.DragonGem:
                        {
                            BroadcastItem(user, user, PBEItem.DragonGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Electric:
                {
                    switch (user.Item)
                    {
                        case PBEItem.Magnet:
                        case PBEItem.ZapPlate:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.ElectricGem:
                        {
                            BroadcastItem(user, user, PBEItem.ElectricGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Fighting:
                {
                    switch (user.Item)
                    {
                        case PBEItem.BlackBelt:
                        case PBEItem.FistPlate:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.FightingGem:
                        {
                            BroadcastItem(user, user, PBEItem.FightingGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Fire:
                {
                    switch (user.Item)
                    {
                        case PBEItem.Charcoal:
                        case PBEItem.FlamePlate:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.FireGem:
                        {
                            BroadcastItem(user, user, PBEItem.FireGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Flying:
                {
                    switch (user.Item)
                    {
                        case PBEItem.SharpBeak:
                        case PBEItem.SkyPlate:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.FlyingGem:
                        {
                            BroadcastItem(user, user, PBEItem.FlyingGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Ghost:
                {
                    switch (user.Item)
                    {
                        case PBEItem.GriseousOrb:
                        {
                            if (user.OriginalSpecies == PBESpecies.Giratina && user.RevertForm == PBEForm.Giratina_Origin)
                            {
                                basePower *= 1.2f;
                            }
                            break;
                        }
                        case PBEItem.SpellTag:
                        case PBEItem.SpookyPlate:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.GhostGem:
                        {
                            BroadcastItem(user, user, PBEItem.GhostGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Grass:
                {
                    switch (user.Item)
                    {
                        case PBEItem.MeadowPlate:
                        case PBEItem.MiracleSeed:
                        case PBEItem.RoseIncense:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.GrassGem:
                        {
                            BroadcastItem(user, user, PBEItem.GrassGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Ground:
                {
                    switch (user.Item)
                    {
                        case PBEItem.EarthPlate:
                        case PBEItem.SoftSand:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.GroundGem:
                        {
                            BroadcastItem(user, user, PBEItem.GroundGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Ice:
                {
                    switch (user.Item)
                    {
                        case PBEItem.IciclePlate:
                        case PBEItem.NeverMeltIce:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.IceGem:
                        {
                            BroadcastItem(user, user, PBEItem.IceGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.None:
                {
                    break;
                }
                case PBEType.Normal:
                {
                    switch (user.Item)
                    {
                        case PBEItem.SilkScarf:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.NormalGem:
                        {
                            BroadcastItem(user, user, PBEItem.NormalGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Poison:
                {
                    switch (user.Item)
                    {
                        case PBEItem.PoisonBarb:
                        case PBEItem.ToxicPlate:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.PoisonGem:
                        {
                            BroadcastItem(user, user, PBEItem.PoisonGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Psychic:
                {
                    switch (user.Item)
                    {
                        case PBEItem.MindPlate:
                        case PBEItem.OddIncense:
                        case PBEItem.TwistedSpoon:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.PsychicGem:
                        {
                            BroadcastItem(user, user, PBEItem.PsychicGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Rock:
                {
                    switch (user.Item)
                    {
                        case PBEItem.HardStone:
                        case PBEItem.RockIncense:
                        case PBEItem.StonePlate:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.RockGem:
                        {
                            BroadcastItem(user, user, PBEItem.RockGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Steel:
                {
                    switch (user.Item)
                    {
                        case PBEItem.AdamantOrb:
                        {
                            if (user.OriginalSpecies == PBESpecies.Dialga)
                            {
                                basePower *= 1.2f;
                            }
                            break;
                        }
                        case PBEItem.IronPlate:
                        case PBEItem.MetalCoat:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.SteelGem:
                        {
                            BroadcastItem(user, user, PBEItem.SteelGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                case PBEType.Water:
                {
                    switch (user.Item)
                    {
                        case PBEItem.LustrousOrb:
                        {
                            if (user.OriginalSpecies == PBESpecies.Palkia)
                            {
                                basePower *= 1.2f;
                            }
                            break;
                        }
                        case PBEItem.MysticWater:
                        case PBEItem.SeaIncense:
                        case PBEItem.SplashPlate:
                        case PBEItem.WaveIncense:
                        {
                            basePower *= 1.2f;
                            break;
                        }
                        case PBEItem.WaterGem:
                        {
                            BroadcastItem(user, user, PBEItem.WaterGem, PBEItemAction.Consumed);
                            basePower *= 1.5f;
                            break;
                        }
                    }
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(moveType));
            }
            #endregion

            #region Move-specific power boosts
            switch (mData.Effect)
            {
                case PBEMoveEffect.Acrobatics:
                {
                    if (user.Item == PBEItem.None)
                    {
                        basePower *= 2.0f;
                    }
                    break;
                }
                case PBEMoveEffect.Brine:
                {
                    if (targets.Any(t => t.HP <= t.HP / 2))
                    {
                        basePower *= 2.0f;
                    }
                    break;
                }
                case PBEMoveEffect.Facade:
                {
                    if (user.Status1 == PBEStatus1.Burned || user.Status1 == PBEStatus1.Paralyzed || user.Status1 == PBEStatus1.Poisoned || user.Status1 == PBEStatus1.BadlyPoisoned)
                    {
                        basePower *= 2.0f;
                    }
                    break;
                }
                case PBEMoveEffect.Hex:
                {
                    if (targets.Any(t => t.Status1 != PBEStatus1.None))
                    {
                        basePower *= 2.0f;
                    }
                    break;
                }
                case PBEMoveEffect.Payback:
                {
                    if (targets.Any(t => t.HasUsedMoveThisTurn))
                    {
                        basePower *= 2.0f;
                    }
                    break;
                }
                case PBEMoveEffect.Retaliate:
                {
                    if (user.Team.MonFaintedLastTurn)
                    {
                        basePower *= 2.0f;
                    }
                    break;
                }
                case PBEMoveEffect.SmellingSalt:
                {
                    if (targets.Any(t => t.Status1 == PBEStatus1.Paralyzed))
                    {
                        basePower *= 2.0f;
                    }
                    break;
                }
                case PBEMoveEffect.Venoshock:
                {
                    if (targets.Any(t => t.Status1 == PBEStatus1.Poisoned || t.Status1 == PBEStatus1.BadlyPoisoned))
                    {
                        basePower *= 2.0f;
                    }
                    break;
                }
                case PBEMoveEffect.WakeUpSlap:
                {
                    if (targets.Any(t => t.Status1 == PBEStatus1.Asleep))
                    {
                        basePower *= 2.0f;
                    }
                    break;
                }
                case PBEMoveEffect.WeatherBall:
                {
                    if (ShouldDoWeatherEffects() && Weather != PBEWeather.None)
                    {
                        basePower *= 2.0f;
                    }
                    break;
                }
            }
            #endregion

            #region Weather-specific power boosts
            if (ShouldDoWeatherEffects())
            {
                switch (Weather)
                {
                    case PBEWeather.HarshSunlight:
                    {
                        if (moveType == PBEType.Fire)
                        {
                            basePower *= 1.5f;
                        }
                        else if (moveType == PBEType.Water)
                        {
                            basePower *= 0.5f;
                        }
                        break;
                    }
                    case PBEWeather.Rain:
                    {
                        if (moveType == PBEType.Water)
                        {
                            basePower *= 1.5f;
                        }
                        else if (moveType == PBEType.Fire)
                        {
                            basePower *= 0.5f;
                        }
                        break;
                    }
                    case PBEWeather.Sandstorm:
                    {
                        if (user.Ability == PBEAbility.SandForce && (moveType == PBEType.Rock || moveType == PBEType.Ground || moveType == PBEType.Steel))
                        {
                            basePower *= 1.3f;
                        }
                        break;
                    }
                }
            }
            #endregion

            #region Other power boosts
            if (user.Status2.HasFlag(PBEStatus2.HelpingHand))
            {
                basePower *= 1.5f;
            }
            if (user.Ability == PBEAbility.FlareBoost && mData.Category == PBEMoveCategory.Special && user.Status1 == PBEStatus1.Burned)
            {
                basePower *= 1.5f;
            }
            if (user.Ability == PBEAbility.ToxicBoost && mData.Category == PBEMoveCategory.Physical && (user.Status1 == PBEStatus1.Poisoned || user.Status1 == PBEStatus1.BadlyPoisoned))
            {
                basePower *= 1.5f;
            }
            if (user.Item == PBEItem.LifeOrb)
            {
                basePower *= 1.3f;
            }
            if (user.Ability == PBEAbility.IronFist && mData.Flags.HasFlag(PBEMoveFlag.AffectedByIronFist))
            {
                basePower *= 1.2f;
            }
            if (user.Ability == PBEAbility.Reckless && mData.Flags.HasFlag(PBEMoveFlag.AffectedByReckless))
            {
                basePower *= 1.2f;
            }
            if (user.Item == PBEItem.MuscleBand && mData.Category == PBEMoveCategory.Physical)
            {
                basePower *= 1.1f;
            }
            if (user.Item == PBEItem.WiseGlasses && mData.Category == PBEMoveCategory.Special)
            {
                basePower *= 1.1f;
            }
            #endregion

            return basePower;
        }
        private float CalculateDamageMultiplier(PBEBattlePokemon user, PBEBattlePokemon target, IPBEMoveData mData, PBEType moveType, PBEResult moveResult, bool criticalHit)
        {
            float damageMultiplier = 1;
            if (target.Status2.HasFlag(PBEStatus2.Airborne) && mData.Flags.HasFlag(PBEMoveFlag.DoubleDamageAirborne))
            {
                damageMultiplier *= 2.0f;
            }
            if (target.Minimize_Used && mData.Flags.HasFlag(PBEMoveFlag.DoubleDamageMinimized))
            {
                damageMultiplier *= 2.0f;
            }
            if (target.Status2.HasFlag(PBEStatus2.Underground) && mData.Flags.HasFlag(PBEMoveFlag.DoubleDamageUnderground))
            {
                damageMultiplier *= 2.0f;
            }
            if (target.Status2.HasFlag(PBEStatus2.Underwater) && mData.Flags.HasFlag(PBEMoveFlag.DoubleDamageUnderwater))
            {
                damageMultiplier *= 2.0f;
            }

            if (criticalHit)
            {
                damageMultiplier *= Settings.CritMultiplier;
                if (user.Ability == PBEAbility.Sniper)
                {
                    damageMultiplier *= 1.5f;
                }
            }
            else if (user.Ability != PBEAbility.Infiltrator)
            {
                if ((target.Team.TeamStatus.HasFlag(PBETeamStatus.Reflect) && mData.Category == PBEMoveCategory.Physical)
                    || (target.Team.TeamStatus.HasFlag(PBETeamStatus.LightScreen) && mData.Category == PBEMoveCategory.Special))
                {
                    if (target.Team.NumPkmnOnField == 1)
                    {
                        damageMultiplier *= 0.5f;
                    }
                    else
                    {
                        damageMultiplier *= 0.66f;
                    }
                }
            }

            switch (moveResult)
            {
                case PBEResult.NotVeryEffective_Type:
                {
                    if (user.Ability == PBEAbility.TintedLens)
                    {
                        damageMultiplier *= 2.0f;
                    }
                    break;
                }
                case PBEResult.SuperEffective_Type:
                {
                    if ((target.Ability == PBEAbility.Filter || target.Ability == PBEAbility.SolidRock) && !user.HasCancellingAbility())
                    {
                        damageMultiplier *= 0.75f;
                    }
                    if (user.Item == PBEItem.ExpertBelt)
                    {
                        damageMultiplier *= 1.2f;
                    }
                    break;
                }
            }
            if (user.ReceivesSTAB(moveType))
            {
                if (user.Ability == PBEAbility.Adaptability)
                {
                    damageMultiplier *= 2.0f;
                }
                else
                {
                    damageMultiplier *= 1.5f;
                }
            }
            if (mData.Category == PBEMoveCategory.Physical && user.Status1 == PBEStatus1.Burned && user.Ability != PBEAbility.Guts)
            {
                damageMultiplier *= 0.5f;
            }
            if (moveType == PBEType.Fire && target.Ability == PBEAbility.Heatproof && !user.HasCancellingAbility())
            {
                damageMultiplier *= 0.5f;
            }

            return damageMultiplier;
        }

        private float CalculateAttack(PBEBattlePokemon user, PBEBattlePokemon target, PBEType moveType, float initialAttack)
        {
            float attack = initialAttack;

            if (user.Ability == PBEAbility.HugePower || user.Ability == PBEAbility.PurePower)
            {
                attack *= 2.0f;
            }
            if (user.Item == PBEItem.ThickClub && (user.OriginalSpecies == PBESpecies.Cubone || user.OriginalSpecies == PBESpecies.Marowak))
            {
                attack *= 2.0f;
            }
            if (user.Item == PBEItem.LightBall && user.OriginalSpecies == PBESpecies.Pikachu)
            {
                attack *= 2.0f;
            }
            if (moveType == PBEType.Bug && user.Ability == PBEAbility.Swarm && user.HP <= user.MaxHP / 3)
            {
                attack *= 1.5f;
            }
            if (moveType == PBEType.Fire && user.Ability == PBEAbility.Blaze && user.HP <= user.MaxHP / 3)
            {
                attack *= 1.5f;
            }
            if (moveType == PBEType.Grass && user.Ability == PBEAbility.Overgrow && user.HP <= user.MaxHP / 3)
            {
                attack *= 1.5f;
            }
            if (moveType == PBEType.Water && user.Ability == PBEAbility.Torrent && user.HP <= user.MaxHP / 3)
            {
                attack *= 1.5f;
            }
            if (user.Ability == PBEAbility.Hustle)
            {
                attack *= 1.5f;
            }
            if (user.Ability == PBEAbility.Guts && user.Status1 != PBEStatus1.None)
            {
                attack *= 1.5f;
            }
            if (user.Item == PBEItem.ChoiceBand)
            {
                attack *= 1.5f;
            }
            if (!user.HasCancellingAbility() && ShouldDoWeatherEffects() && Weather == PBEWeather.HarshSunlight && user.Team.ActiveBattlers.Any(p => p.Ability == PBEAbility.FlowerGift))
            {
                attack *= 1.5f;
            }
            if ((moveType == PBEType.Fire || moveType == PBEType.Ice) && target.Ability == PBEAbility.ThickFat && !user.HasCancellingAbility())
            {
                attack *= 0.5f;
            }
            if (user.Ability == PBEAbility.Defeatist && user.HP <= user.MaxHP / 2)
            {
                attack *= 0.5f;
            }
            if (user.Ability == PBEAbility.SlowStart && user.SlowStart_HinderTurnsLeft > 0)
            {
                attack *= 0.5f;
            }

            return attack;
        }
        private float CalculateDefense(PBEBattlePokemon user, PBEBattlePokemon target, float initialDefense)
        {
            float defense = initialDefense;

            if (target.Item == PBEItem.MetalPowder && target.OriginalSpecies == PBESpecies.Ditto && !target.Status2.HasFlag(PBEStatus2.Transformed))
            {
                defense *= 2.0f;
            }
            if (target.Ability == PBEAbility.MarvelScale && target.Status1 != PBEStatus1.None && !user.HasCancellingAbility())
            {
                defense *= 1.5f;
            }
            if (target.Item == PBEItem.Eviolite && PBEDataProvider.Instance.HasEvolutions(target.OriginalSpecies, target.RevertForm))
            {
                defense *= 1.5f;
            }

            return defense;
        }
        private float CalculateSpAttack(PBEBattlePokemon user, PBEBattlePokemon target, PBEType moveType, float initialSpAttack)
        {
            float spAttack = initialSpAttack;

            if (user.Item == PBEItem.DeepSeaTooth && user.OriginalSpecies == PBESpecies.Clamperl)
            {
                spAttack *= 2.0f;
            }
            if (user.Item == PBEItem.LightBall && user.OriginalSpecies == PBESpecies.Pikachu)
            {
                spAttack *= 2.0f;
            }
            if (moveType == PBEType.Bug && user.Ability == PBEAbility.Swarm && user.HP <= user.MaxHP / 3)
            {
                spAttack *= 1.5f;
            }
            if (moveType == PBEType.Fire && user.Ability == PBEAbility.Blaze && user.HP <= user.MaxHP / 3)
            {
                spAttack *= 1.5f;
            }
            if (moveType == PBEType.Grass && user.Ability == PBEAbility.Overgrow && user.HP <= user.MaxHP / 3)
            {
                spAttack *= 1.5f;
            }
            if (moveType == PBEType.Water && user.Ability == PBEAbility.Torrent && user.HP <= user.MaxHP / 3)
            {
                spAttack *= 1.5f;
            }
            if (ShouldDoWeatherEffects() && Weather == PBEWeather.HarshSunlight && user.Ability == PBEAbility.SolarPower)
            {
                spAttack *= 1.5f;
            }
            if (user.Item == PBEItem.SoulDew && (user.OriginalSpecies == PBESpecies.Latias || user.OriginalSpecies == PBESpecies.Latios))
            {
                spAttack *= 1.5f;
            }
            if (user.Item == PBEItem.ChoiceSpecs)
            {
                spAttack *= 1.5f;
            }
            if ((user.Ability == PBEAbility.Minus || user.Ability == PBEAbility.Plus) && user.Team.ActiveBattlers.Any(p => p != user && (p.Ability == PBEAbility.Minus || p.Ability == PBEAbility.Plus)))
            {
                spAttack *= 1.5f;
            }
            if ((moveType == PBEType.Fire || moveType == PBEType.Ice) && target.Ability == PBEAbility.ThickFat && !user.HasCancellingAbility())
            {
                spAttack *= 0.5f;
            }
            if (user.Ability == PBEAbility.Defeatist && user.HP <= user.MaxHP / 2)
            {
                spAttack *= 0.5f;
            }

            return spAttack;
        }
        private float CalculateSpDefense(PBEBattlePokemon user, PBEBattlePokemon target, float initialSpDefense)
        {
            float spDefense = initialSpDefense;

            if (target.Item == PBEItem.DeepSeaScale && target.OriginalSpecies == PBESpecies.Clamperl)
            {
                spDefense *= 2.0f;
            }
            if (target.Item == PBEItem.SoulDew && (target.OriginalSpecies == PBESpecies.Latias || target.OriginalSpecies == PBESpecies.Latios))
            {
                spDefense *= 1.5f;
            }
            if (target.Item == PBEItem.Eviolite && PBEDataProvider.Instance.HasEvolutions(target.OriginalSpecies, target.RevertForm))
            {
                spDefense *= 1.5f;
            }
            if (ShouldDoWeatherEffects())
            {
                if (Weather == PBEWeather.Sandstorm && target.HasType(PBEType.Rock))
                {
                    spDefense *= 1.5f;
                }
                if (!user.HasCancellingAbility() && Weather == PBEWeather.HarshSunlight && target.Team.ActiveBattlers.Any(p => p.Ability == PBEAbility.FlowerGift))
                {
                    spDefense *= 1.5f;
                }
            }

            return spDefense;
        }

        private int CalculateDamage(PBEBattlePokemon user, float a, float d, float basePower)
        {
            float damage;
            damage = (2 * user.Level / 5) + 2;
            damage = damage * a * basePower / d;
            damage /= 50;
            damage += 2;
            return (int)(damage * ((100d - _rand.RandomInt(0, 15)) / 100));
        }
        private int CalculateConfusionDamage(PBEBattlePokemon pkmn)
        {
            // Verified: Unaware has no effect on confusion damage
            float m = GetStatChangeModifier(pkmn.AttackChange, false);
            float a = CalculateAttack(pkmn, pkmn, PBEType.None, pkmn.Attack * m);
            m = GetStatChangeModifier(pkmn.DefenseChange, false);
            float d = CalculateDefense(pkmn, pkmn, pkmn.Defense * m);
            return CalculateDamage(pkmn, a, d, 40);
        }
        private int CalculateDamage(PBEBattlePokemon user, PBEBattlePokemon target, IPBEMoveData mData, PBEType moveType, float basePower, bool criticalHit)
        {
            PBEBattlePokemon aPkmn;
            PBEMoveCategory aCat = mData.Category, dCat;
            switch (mData.Effect)
            {
                case PBEMoveEffect.FoulPlay:
                {
                    aPkmn = target;
                    dCat = aCat;
                    break;
                }
                case PBEMoveEffect.Psyshock:
                {
                    aPkmn = user;
                    dCat = PBEMoveCategory.Physical;
                    break;
                }
                default:
                {
                    aPkmn = user;
                    dCat = aCat;
                    break;
                }
            }

            bool ignoreA = user != target && target.Ability == PBEAbility.Unaware && !user.HasCancellingAbility();
            bool ignoreD = user != target && (mData.Effect == PBEMoveEffect.ChipAway || user.Ability == PBEAbility.Unaware);
            float a, d;
            if (aCat == PBEMoveCategory.Physical)
            {
                float m = ignoreA ? 1 : GetStatChangeModifier(criticalHit ? Math.Max((sbyte)0, aPkmn.AttackChange) : aPkmn.AttackChange, false);
                a = CalculateAttack(user, target, moveType, aPkmn.Attack * m);
            }
            else
            {
                float m = ignoreA ? 1 : GetStatChangeModifier(criticalHit ? Math.Max((sbyte)0, aPkmn.SpAttackChange) : aPkmn.SpAttackChange, false);
                a = CalculateSpAttack(user, target, moveType, aPkmn.SpAttack * m);
            }
            if (dCat == PBEMoveCategory.Physical)
            {
                float m = ignoreD ? 1 : GetStatChangeModifier(criticalHit ? Math.Min((sbyte)0, target.DefenseChange) : target.DefenseChange, false);
                d = CalculateDefense(user, target, target.Defense * m);
            }
            else
            {
                float m = ignoreD ? 1 : GetStatChangeModifier(criticalHit ? Math.Min((sbyte)0, target.SpDefenseChange) : target.SpDefenseChange, false);
                d = CalculateSpDefense(user, target, target.SpDefense * m);
            }

            return CalculateDamage(user, a, d, basePower);
        }
    }
}
