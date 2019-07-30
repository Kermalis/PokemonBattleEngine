using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        /// <summary>
        /// Gets the influence a stat change has on a stat.
        /// </summary>
        /// <param name="change">The stat change.</param>
        /// <param name="forMissing">True if the stat is <see cref="PBEStat.Accuracy"/> or <see cref="PBEStat.Evasion"/>.</param>
        public static double GetStatChangeModifier(sbyte change, bool forMissing)
        {
            double baseVal = forMissing ? 3 : 2;
            double numerator = Math.Max(baseVal, baseVal + change);
            double denominator = Math.Max(baseVal, baseVal - change);
            return numerator / denominator;
        }

        /// <summary>
        /// Deals damage to <paramref name="victim"/> and broadcasts the HP changing and substitute damage.
        /// </summary>
        /// <param name="culprit">The Pokémon responsible for the damage.</param>
        /// <param name="victim">The Pokémon receiving the damage.</param>
        /// <param name="hp">The amount of HP <paramref name="victim"/> will try to lose.</param>
        /// <param name="ignoreSubstitute">Whether the damage should ignore <paramref name="victim"/>'s <see cref="PBEStatus2.Substitute"/>.</param>
        /// <param name="ignoreSturdy">Whether the damage should ignore <paramref name="victim"/>'s <see cref="PBEAbility.Sturdy"/>, <see cref="PBEItem.FocusBand"/>, or <see cref="PBEItem.FocusSash"/>.</param>
        /// <returns>The amount of damage dealt.</returns>
        private ushort DealDamage(PBEPokemon culprit, PBEPokemon victim, ushort hp, bool ignoreSubstitute, bool ignoreSturdy = false)
        {
            if (!ignoreSubstitute && victim.Status2.HasFlag(PBEStatus2.Substitute))
            {
                ushort oldHP = victim.SubstituteHP;
                victim.SubstituteHP = (ushort)Math.Max(0, victim.SubstituteHP - Math.Max((ushort)1, hp)); // Always lose at least 1 HP
                ushort damageAmt = (ushort)(oldHP - victim.SubstituteHP);
                BroadcastStatus2(victim, culprit, PBEStatus2.Substitute, PBEStatusAction.Damage);
                return damageAmt;
            }
            else
            {
                ushort oldHP = victim.HP;
                double oldPercentage = victim.HPPercentage;
                victim.HP = (ushort)Math.Max(0, victim.HP - Math.Max((ushort)1, hp)); // Always lose at least 1 HP
                bool sturdyHappened = false, focusBandHappened = false, focusSashHappened = false;
                if (!ignoreSturdy && victim.HP == 0)
                {
                    // TODO: Endure
                    if (oldHP == victim.MaxHP && victim.Ability == PBEAbility.Sturdy) // TODO: Mold Breaker
                    {
                        sturdyHappened = true;
                        victim.HP = 1;
                    }
                    else if (victim.Item == PBEItem.FocusBand && PBEUtils.RandomBool(10, 100))
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
                victim.HPPercentage = (double)victim.HP / victim.MaxHP;
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
        }
        /// <summary>
        /// Restores HP to <paramref name="pkmn"/> and broadcasts the HP changing if it changes.
        /// </summary>
        /// <param name="pkmn">The Pokémon receiving the HP.</param>
        /// <param name="hp">The amount of HP <paramref name="pkmn"/> will try to gain.</param>
        /// <returns>The amount of HP restored.</returns>
        private ushort HealDamage(PBEPokemon pkmn, ushort hp)
        {
            ushort oldHP = pkmn.HP;
            double oldPercentage = pkmn.HPPercentage;
            pkmn.HP = (ushort)Math.Min(pkmn.MaxHP, pkmn.HP + Math.Max((ushort)1, hp)); // Always try to heal at least 1 HP
            ushort healAmt = (ushort)(pkmn.HP - oldHP);
            if (healAmt > 0)
            {
                pkmn.HPPercentage = (double)pkmn.HP / pkmn.MaxHP;
                BroadcastPkmnHPChanged(pkmn, oldHP, oldPercentage);
            }
            return healAmt;
        }
        private void TypeCheck(PBEPokemon user, PBEPokemon target, PBEType moveType, out PBEEffectiveness moveEffectiveness, ref double moveEffectivenessMultiplier, bool ignoreWonderGuard)
        {
            double m = PBEPokemonData.TypeEffectiveness[moveType][target.Type1];
            m *= PBEPokemonData.TypeEffectiveness[moveType][target.Type2];

            if (m <= 0) // (-infinity, 0]
            {
                moveEffectiveness = PBEEffectiveness.Ineffective;
            }
            else if (m < 1) // (0, 1)
            {
                moveEffectiveness = PBEEffectiveness.NotVeryEffective;
            }
            else if (m == 1.0) // [1, 1]
            {
                moveEffectiveness = PBEEffectiveness.Normal;
            }
            else // (1, infinity)
            {
                moveEffectiveness = PBEEffectiveness.SuperEffective;
            }
            moveEffectivenessMultiplier *= m;

            if (moveEffectiveness != PBEEffectiveness.Ineffective)
            {
                if ((target.Ability == PBEAbility.Levitate && moveType == PBEType.Ground)
                    || (!ignoreWonderGuard && target.Ability == PBEAbility.WonderGuard && moveEffectiveness != PBEEffectiveness.SuperEffective))
                {
                    moveEffectiveness = PBEEffectiveness.Ineffective;
                    moveEffectivenessMultiplier = 0;
                    BroadcastAbility(target, target, target.Ability, PBEAbilityAction.Damage);
                }
            }
        }

        private double CalculateBasePower(PBEPokemon user, PBEPokemon[] targets, PBEMove move, PBEType moveType)
        {
            PBEMoveData mData = PBEMoveData.Data[move];
            double basePower = mData.Power;

            // Moves with variable base power
            switch (move)
            {
                case PBEMove.Eruption:
                case PBEMove.WaterSpout:
                {
                    basePower = Math.Max(1, 150 * user.HP / user.MaxHP);
                    break;
                }
                case PBEMove.Flail:
                case PBEMove.Reversal:
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
                    else if (val < 9)
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
                case PBEMove.Frustration:
                {
                    basePower = (int)Math.Max(1, (byte.MaxValue - user.Friendship) / 2.5);
                    break;
                }
                case PBEMove.GrassKnot:
                case PBEMove.LowKick:
                {
                    if (targets[0].Weight >= 200.0)
                    {
                        basePower = 120;
                    }
                    else if (targets[0].Weight >= 100.0)
                    {
                        basePower = 100;
                    }
                    else if (targets[0].Weight >= 50.0)
                    {
                        basePower = 80;
                    }
                    else if (targets[0].Weight >= 25.0)
                    {
                        basePower = 60;
                    }
                    else if (targets[0].Weight >= 10.0)
                    {
                        basePower = 40;
                    }
                    else
                    {
                        basePower = 20;
                    }
                    break;
                }
                case PBEMove.HeatCrash:
                case PBEMove.HeavySlam:
                {
                    double relative = user.Weight / targets[0].Weight;
                    if (relative < 2)
                    {
                        basePower = 40;
                    }
                    else if (relative < 3)
                    {
                        basePower = 60;
                    }
                    else if (relative < 4)
                    {
                        basePower = 80;
                    }
                    else if (relative < 5)
                    {
                        basePower = 100;
                    }
                    else
                    {
                        basePower = 120;
                    }
                    break;
                }
                case PBEMove.HiddenPower:
                {
                    basePower = user.GetHiddenPowerBasePower();
                    break;
                }
                case PBEMove.Magnitude:
                {
                    int val = PBEUtils.RandomInt(0, 99);
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
                case PBEMove.Return:
                {
                    basePower = (int)Math.Max(1, user.Friendship / 2.5);
                    break;
                }
            }

            // Item-specific power boosts
            bool canUseGems = !mData.Flags.HasFlag(PBEMoveFlag.UnaffectedByGems);
            switch (moveType)
            {
                case PBEType.Bug:
                    if (user.Ability == PBEAbility.Swarm && user.HP <= user.MaxHP / 3)
                    {
                        basePower *= 1.5;
                    }
                    switch (user.Item)
                    {
                        case PBEItem.InsectPlate:
                        case PBEItem.SilverPowder:
                            basePower *= 1.2;
                            break;
                        case PBEItem.BugGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.BugGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Dark:
                    switch (user.Item)
                    {
                        case PBEItem.BlackGlasses:
                        case PBEItem.DreadPlate:
                            basePower *= 1.2;
                            break;
                        case PBEItem.DarkGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.DarkGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Dragon:
                    switch (user.Item)
                    {
                        case PBEItem.AdamantOrb:
                            if (user.OriginalSpecies == PBESpecies.Dialga)
                            {
                                basePower *= 1.2;
                            }
                            break;
                        case PBEItem.DracoPlate:
                        case PBEItem.DragonFang:
                            basePower *= 1.2;
                            break;
                        case PBEItem.GriseousOrb:
                            if (user.OriginalSpecies == PBESpecies.Giratina_Origin)
                            {
                                basePower *= 1.2;
                            }
                            break;
                        case PBEItem.LustrousOrb:
                            if (user.OriginalSpecies == PBESpecies.Palkia)
                            {
                                basePower *= 1.2;
                            }
                            break;
                        case PBEItem.DragonGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.DragonGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Electric:
                    switch (user.Item)
                    {
                        case PBEItem.Magnet:
                        case PBEItem.ZapPlate:
                            basePower *= 1.2;
                            break;
                        case PBEItem.ElectricGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.ElectricGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Fighting:
                    switch (user.Item)
                    {
                        case PBEItem.BlackBelt:
                        case PBEItem.FistPlate:
                            basePower *= 1.2;
                            break;
                        case PBEItem.FightingGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.FightingGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Fire:
                    switch (user.Item)
                    {
                        case PBEItem.Charcoal:
                        case PBEItem.FlamePlate:
                            basePower *= 1.2;
                            break;
                        case PBEItem.FireGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.FireGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Flying:
                    switch (user.Item)
                    {
                        case PBEItem.SharpBeak:
                        case PBEItem.SkyPlate:
                            basePower *= 1.2;
                            break;
                        case PBEItem.FlyingGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.FlyingGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Ghost:
                    switch (user.Item)
                    {
                        case PBEItem.GriseousOrb:
                            if (user.OriginalSpecies == PBESpecies.Giratina_Origin)
                            {
                                basePower *= 1.2;
                            }
                            break;
                        case PBEItem.SpellTag:
                        case PBEItem.SpookyPlate:
                            basePower *= 1.2;
                            break;
                        case PBEItem.GhostGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.GhostGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Grass:
                    switch (user.Item)
                    {
                        case PBEItem.MeadowPlate:
                        case PBEItem.MiracleSeed:
                        case PBEItem.RoseIncense:
                            basePower *= 1.2;
                            break;
                        case PBEItem.GrassGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.GrassGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Ground:
                    switch (user.Item)
                    {
                        case PBEItem.EarthPlate:
                        case PBEItem.SoftSand:
                            basePower *= 1.2;
                            break;
                        case PBEItem.GroundGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.GroundGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Ice:
                    switch (user.Item)
                    {
                        case PBEItem.IciclePlate:
                        case PBEItem.NeverMeltIce:
                            basePower *= 1.2;
                            break;
                        case PBEItem.IceGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.IceGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Normal:
                    switch (user.Item)
                    {
                        case PBEItem.SilkScarf:
                            basePower *= 1.2;
                            break;
                        case PBEItem.NormalGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.NormalGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Poison:
                    switch (user.Item)
                    {
                        case PBEItem.PoisonBarb:
                        case PBEItem.ToxicPlate:
                            basePower *= 1.2;
                            break;
                        case PBEItem.PoisonGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.PoisonGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Psychic:
                    switch (user.Item)
                    {
                        case PBEItem.MindPlate:
                        case PBEItem.OddIncense:
                        case PBEItem.TwistedSpoon:
                            basePower *= 1.2;
                            break;
                        case PBEItem.PsychicGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.PsychicGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Rock:
                    switch (user.Item)
                    {
                        case PBEItem.HardStone:
                        case PBEItem.RockIncense:
                        case PBEItem.StonePlate:
                            basePower *= 1.2;
                            break;
                        case PBEItem.RockGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.RockGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Steel:
                    switch (user.Item)
                    {
                        case PBEItem.AdamantOrb:
                            if (user.OriginalSpecies == PBESpecies.Dialga)
                            {
                                basePower *= 1.2;
                            }
                            break;
                        case PBEItem.IronPlate:
                        case PBEItem.MetalCoat:
                            basePower *= 1.2;
                            break;
                        case PBEItem.SteelGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.SteelGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
                case PBEType.Water:
                    switch (user.Item)
                    {
                        case PBEItem.LustrousOrb:
                            if (user.OriginalSpecies == PBESpecies.Palkia)
                            {
                                basePower *= 1.2;
                            }
                            break;
                        case PBEItem.MysticWater:
                        case PBEItem.SeaIncense:
                        case PBEItem.SplashPlate:
                        case PBEItem.WaveIncense:
                            basePower *= 1.2;
                            break;
                        case PBEItem.WaterGem:
                            if (canUseGems)
                            {
                                BroadcastItem(user, user, PBEItem.WaterGem, PBEItemAction.Consumed);
                                basePower *= 1.5;
                            }
                            break;
                    }
                    break;
            }

            // Move-specific power boosts
            switch (move)
            {
                case PBEMove.Acrobatics:
                    if (user.Item == PBEItem.None)
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.Brine:
                    if (targets[0].HP <= targets[0].HP / 2)
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.Facade:
                    if (user.Status1 == PBEStatus1.Burned || user.Status1 == PBEStatus1.Paralyzed || user.Status1 == PBEStatus1.Poisoned || user.Status1 == PBEStatus1.BadlyPoisoned)
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.Hex:
                    if (targets[0].Status1 != PBEStatus1.None)
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.Retaliate:
                    if (user.Team.MonFaintedLastTurn)
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.Venoshock:
                    if (targets[0].Status1 == PBEStatus1.Poisoned || targets[0].Status1 == PBEStatus1.BadlyPoisoned)
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.WeatherBall:
                {
                    if (ShouldDoWeatherEffects() && Weather != PBEWeather.None)
                    {
                        basePower *= 2.0;
                    }
                    break;
                }
            }

            if (ShouldDoWeatherEffects())
            {
                switch (Weather)
                {
                    case PBEWeather.HarshSunlight:
                    {
                        if (moveType == PBEType.Fire)
                        {
                            basePower *= 1.5;
                        }
                        else if (moveType == PBEType.Water)
                        {
                            basePower *= 0.5;
                        }
                        break;
                    }
                    case PBEWeather.Rain:
                    {
                        if (moveType == PBEType.Water)
                        {
                            basePower *= 1.5;
                        }
                        else if (moveType == PBEType.Fire)
                        {
                            basePower *= 0.5;
                        }
                        break;
                    }
                    case PBEWeather.Sandstorm:
                    {
                        if (user.Ability == PBEAbility.SandForce && (user.HasType(PBEType.Rock) || user.HasType(PBEType.Ground) || user.HasType(PBEType.Steel)))
                        {
                            basePower *= 1.3;
                        }
                        break;
                    }
                }
            }

            if (user.Status2.HasFlag(PBEStatus2.HelpingHand))
            {
                basePower *= 1.5;
            }
            if (user.Ability == PBEAbility.ToxicBoost && mData.Category == PBEMoveCategory.Physical && (user.Status1 == PBEStatus1.Poisoned || user.Status1 == PBEStatus1.BadlyPoisoned))
            {
                basePower *= 1.5;
            }
            if (user.Item == PBEItem.LifeOrb)
            {
                basePower *= 1.3;
            }
            if (user.Ability == PBEAbility.IronFist && mData.Flags.HasFlag(PBEMoveFlag.AffectedByIronFist))
            {
                basePower *= 1.2;
            }
            if (user.Ability == PBEAbility.Reckless && mData.Flags.HasFlag(PBEMoveFlag.AffectedByReckless))
            {
                basePower *= 1.2;
            }
            if (mData.Category == PBEMoveCategory.Physical && user.Item == PBEItem.MuscleBand)
            {
                basePower *= 1.1;
            }
            if (mData.Category == PBEMoveCategory.Special && user.Item == PBEItem.WiseGlasses)
            {
                basePower *= 1.1;
            }

            return basePower;
        }
        private double CalculateDamageMultiplier(PBEPokemon user, PBEPokemon target, PBEMove move, PBEType moveType, PBEEffectiveness moveEffectiveness, bool criticalHit)
        {
            PBEMoveData mData = PBEMoveData.Data[move];
            double damageMultiplier = 1;
            switch (move)
            {
                case PBEMove.Gust:
                    if (target.Status2.HasFlag(PBEStatus2.Airborne))
                    {
                        damageMultiplier *= 2.0;
                    }
                    break;
                case PBEMove.Earthquake:
                case PBEMove.Magnitude:
                    if (target.Status2.HasFlag(PBEStatus2.Underground))
                    {
                        damageMultiplier *= 2.0;
                    }
                    break;
                case PBEMove.Steamroller:
                case PBEMove.Stomp:
                    if (target.ExecutedMoves.Any(m => m.Move == PBEMove.Minimize)) // TODO: Could minimize ever fail?
                    {
                        damageMultiplier *= 2.0;
                    }
                    break;
                case PBEMove.Surf:
                    if (target.Status2.HasFlag(PBEStatus2.Underwater))
                    {
                        damageMultiplier *= 2.0;
                    }
                    break;
            }

            if (criticalHit)
            {
                damageMultiplier *= Settings.CritMultiplier;
                if (user.Ability == PBEAbility.Sniper)
                {
                    damageMultiplier *= 1.5;
                }
            }
            else
            {
                if ((target.Team.TeamStatus.HasFlag(PBETeamStatus.Reflect) && mData.Category == PBEMoveCategory.Physical)
                    || (target.Team.TeamStatus.HasFlag(PBETeamStatus.LightScreen) && mData.Category == PBEMoveCategory.Special))
                {
                    if (target.Team.NumPkmnOnField == 1)
                    {
                        damageMultiplier *= 0.5;
                    }
                    else
                    {
                        damageMultiplier *= 0.66;
                    }
                }
            }

            switch (moveEffectiveness)
            {
                case PBEEffectiveness.NotVeryEffective:
                    if (user.Ability == PBEAbility.TintedLens)
                    {
                        damageMultiplier *= 2.0;
                    }
                    break;
                case PBEEffectiveness.SuperEffective:
                    if (target.Ability == PBEAbility.Filter || target.Ability == PBEAbility.SolidRock)
                    {
                        damageMultiplier *= 0.75;
                    }
                    if (user.Item == PBEItem.ExpertBelt)
                    {
                        damageMultiplier *= 1.2;
                    }
                    break;
            }
            if (user.HasType(moveType))
            {
                if (user.Ability == PBEAbility.Adaptability)
                {
                    damageMultiplier *= 2.0;
                }
                else
                {
                    damageMultiplier *= 1.5;
                }
            }
            if (mData.Category == PBEMoveCategory.Physical && user.Status1 == PBEStatus1.Burned && user.Ability != PBEAbility.Guts)
            {
                damageMultiplier *= 0.5;
            }
            if (moveType == PBEType.Fire && target.Ability == PBEAbility.Heatproof)
            {
                damageMultiplier *= 0.5;
            }

            return damageMultiplier;
        }

        private double CalculateAttack(PBEPokemon user, PBEPokemon target, PBEType moveType, double initialAttack)
        {
            double attack = initialAttack;

            if (user.Ability == PBEAbility.HugePower || user.Ability == PBEAbility.PurePower)
            {
                attack *= 2.0;
            }
            if (user.Item == PBEItem.ThickClub && (user.OriginalSpecies == PBESpecies.Cubone || user.OriginalSpecies == PBESpecies.Marowak))
            {
                attack *= 2.0;
            }
            if (user.Item == PBEItem.LightBall && user.OriginalSpecies == PBESpecies.Pikachu)
            {
                attack *= 2.0;
            }
            if (moveType == PBEType.Fire && user.Ability == PBEAbility.Blaze && user.HP <= user.MaxHP / 3)
            {
                attack *= 1.5;
            }
            if (moveType == PBEType.Grass && user.Ability == PBEAbility.Overgrow && user.HP <= user.MaxHP / 3)
            {
                attack *= 1.5;
            }
            if (moveType == PBEType.Water && user.Ability == PBEAbility.Torrent && user.HP <= user.MaxHP / 3)
            {
                attack *= 1.5;
            }
            if (user.Ability == PBEAbility.Hustle)
            {
                attack *= 1.5;
            }
            if (user.Ability == PBEAbility.Guts && user.Status1 != PBEStatus1.None)
            {
                attack *= 1.5;
            }
            if (user.Item == PBEItem.ChoiceBand)
            {
                attack *= 1.5;
            }
            if (ShouldDoWeatherEffects() && Weather == PBEWeather.HarshSunlight && user.Team.ActiveBattlers.Any(p => p.Ability == PBEAbility.FlowerGift))
            {
                attack *= 1.5;
            }
            if ((moveType == PBEType.Fire || moveType == PBEType.Ice) && target.Ability == PBEAbility.ThickFat)
            {
                attack *= 0.5;
            }
            if (user.Ability == PBEAbility.Defeatist && user.HP <= user.MaxHP / 2)
            {
                attack *= 0.5;
            }

            return attack;
        }
        private double CalculateDefense(PBEPokemon user, PBEPokemon target, double initialDefense)
        {
            double defense = initialDefense;

            if (target.Item == PBEItem.MetalPowder && target.OriginalSpecies == PBESpecies.Ditto && !target.Status2.HasFlag(PBEStatus2.Transformed))
            {
                defense *= 2.0;
            }
            if (target.Ability == PBEAbility.MarvelScale && target.Status1 != PBEStatus1.None)
            {
                defense *= 1.5;
            }
            if (target.Item == PBEItem.Eviolite && PBEPokemonData.GetData(target.OriginalSpecies).Evolutions.Count > 0)
            {
                defense *= 1.5;
            }

            return defense;
        }
        private double CalculateSpAttack(PBEPokemon user, PBEPokemon target, PBEType moveType, double initialSpAttack)
        {
            double spAttack = initialSpAttack;

            if (user.Item == PBEItem.DeepSeaTooth && user.OriginalSpecies == PBESpecies.Clamperl)
            {
                spAttack *= 2.0;
            }
            if (user.Item == PBEItem.LightBall && user.OriginalSpecies == PBESpecies.Pikachu)
            {
                spAttack *= 2.0;
            }
            if (moveType == PBEType.Fire && user.Ability == PBEAbility.Blaze && user.HP <= user.MaxHP / 3)
            {
                spAttack *= 1.5;
            }
            if (moveType == PBEType.Grass && user.Ability == PBEAbility.Overgrow && user.HP <= user.MaxHP / 3)
            {
                spAttack *= 1.5;
            }
            if (moveType == PBEType.Water && user.Ability == PBEAbility.Torrent && user.HP <= user.MaxHP / 3)
            {
                spAttack *= 1.5;
            }
            if (ShouldDoWeatherEffects() && Weather == PBEWeather.HarshSunlight && user.Ability == PBEAbility.SolarPower)
            {
                spAttack *= 1.5;
            }
            if (user.Item == PBEItem.SoulDew && (user.OriginalSpecies == PBESpecies.Latias || user.OriginalSpecies == PBESpecies.Latios))
            {
                spAttack *= 1.5;
            }
            if (user.Item == PBEItem.ChoiceSpecs)
            {
                spAttack *= 1.5;
            }
            if ((user.Ability == PBEAbility.Minus || user.Ability == PBEAbility.Plus) && user.Team.ActiveBattlers.Any(p => p != user && (p.Ability == PBEAbility.Minus || p.Ability == PBEAbility.Plus)))
            {
                spAttack *= 1.5;
            }
            if ((moveType == PBEType.Fire || moveType == PBEType.Ice) && target.Ability == PBEAbility.ThickFat)
            {
                spAttack *= 0.5;
            }
            if (user.Ability == PBEAbility.Defeatist && user.HP <= user.MaxHP / 2)
            {
                spAttack *= 0.5;
            }

            return spAttack;
        }
        private double CalculateSpDefense(PBEPokemon user, PBEPokemon target, double initialSpDefense)
        {
            double spDefense = initialSpDefense;

            if (target.Item == PBEItem.DeepSeaScale && target.OriginalSpecies == PBESpecies.Clamperl)
            {
                spDefense *= 2.0;
            }
            if (target.Item == PBEItem.SoulDew && (target.OriginalSpecies == PBESpecies.Latias || target.OriginalSpecies == PBESpecies.Latios))
            {
                spDefense *= 1.5;
            }
            if (target.Item == PBEItem.Eviolite && PBEPokemonData.GetData(target.OriginalSpecies).Evolutions.Count > 0)
            {
                spDefense *= 1.5;
            }
            if (ShouldDoWeatherEffects())
            {
                if (Weather == PBEWeather.Sandstorm && target.HasType(PBEType.Rock))
                {
                    spDefense *= 1.5;
                }
                if (Weather == PBEWeather.HarshSunlight && user.Team.ActiveBattlers.Any(p => p.Ability == PBEAbility.FlowerGift))
                {
                    spDefense *= 1.5;
                }
            }

            return spDefense;
        }

        private ushort CalculateDamage(PBEPokemon user, PBEPokemon target, PBEMove move, PBEType moveType, PBEMoveCategory moveCategory, double basePower, bool criticalHit)
        {
            ushort damage;
            double a = 0, d = 0;

            bool unawareA = user != target && target.Ability == PBEAbility.Unaware;
            bool unawareD = user != target && user.Ability == PBEAbility.Unaware;

            switch (move)
            {
                case PBEMove.FoulPlay:
                {
                    double aMod = unawareA ? 1.0 : GetStatChangeModifier(criticalHit ? Math.Max((sbyte)0, target.AttackChange) : target.AttackChange, false);
                    a = CalculateAttack(user, target, moveType, target.Attack * aMod);
                    double dMod = unawareD ? 1.0 : GetStatChangeModifier(criticalHit ? Math.Min((sbyte)0, target.DefenseChange) : target.DefenseChange, false);
                    d = CalculateDefense(user, target, target.Defense * dMod);
                    break;
                }
                case PBEMove.Psyshock:
                case PBEMove.Psystrike:
                case PBEMove.SecretSword:
                {
                    double aMod = unawareA ? 1.0 : GetStatChangeModifier(criticalHit ? Math.Max((sbyte)0, user.SpAttackChange) : user.SpAttackChange, false);
                    a = CalculateSpAttack(user, target, moveType, user.SpAttack * aMod);
                    double dMod = unawareD ? 1.0 : GetStatChangeModifier(criticalHit ? Math.Min((sbyte)0, target.DefenseChange) : target.DefenseChange, false);
                    d = CalculateDefense(user, target, target.Defense * dMod);
                    break;
                }
                default:
                {
                    if (moveCategory == PBEMoveCategory.Physical)
                    {
                        double aMod = unawareA ? 1.0 : GetStatChangeModifier(criticalHit ? Math.Max((sbyte)0, user.AttackChange) : user.AttackChange, false);
                        a = CalculateAttack(user, target, moveType, user.Attack * aMod);
                        double dMod = unawareD ? 1.0 : GetStatChangeModifier(criticalHit ? Math.Min((sbyte)0, target.DefenseChange) : target.DefenseChange, false);
                        d = CalculateDefense(user, target, target.Defense * dMod);
                    }
                    else if (moveCategory == PBEMoveCategory.Special)
                    {
                        double aMod = unawareA ? 1.0 : GetStatChangeModifier(criticalHit ? Math.Max((sbyte)0, user.SpAttackChange) : user.SpAttackChange, false);
                        a = CalculateSpAttack(user, target, moveType, user.SpAttack * aMod);
                        double dMod = unawareD ? 1.0 : GetStatChangeModifier(criticalHit ? Math.Min((sbyte)0, target.SpDefenseChange) : target.SpDefenseChange, false);
                        d = CalculateSpDefense(user, target, target.SpDefense * dMod);
                    }
                    break;
                }
            }

            damage = (ushort)((2 * user.Level / 5) + 2);
            damage = (ushort)(damage * a * basePower / d);
            damage /= 50;
            damage += 2;
            return (ushort)(damage * (100 - PBEUtils.RandomInt(0, 15)) / 100);
        }
    }
}
