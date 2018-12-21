using Kermalis.PokemonBattleEngine.Data;
using System;

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
        /// <returns>The amount of damage dealt.</returns>
        ushort DealDamage(PBEPokemon culprit, PBEPokemon victim, ushort hp, bool ignoreSubstitute)
        {
            if (!ignoreSubstitute && victim.Status2.HasFlag(PBEStatus2.Substitute))
            {
                ushort oldHP = victim.SubstituteHP;
                victim.SubstituteHP = (ushort)Math.Max(0, victim.SubstituteHP - Math.Max((ushort)1, hp)); // Always lose at least 1 HP
                ushort damageAmt = (ushort)(oldHP - victim.SubstituteHP);
                BroadcastStatus2(culprit, victim, PBEStatus2.Substitute, PBEStatusAction.Damage);
                return damageAmt;
            }
            else
            {
                ushort oldHP = victim.HP;
                victim.HP = (ushort)Math.Max(0, victim.HP - Math.Max((ushort)1, hp)); // Always lose at least 1 HP
                ushort damageAmt = (ushort)(oldHP - victim.HP);
                BroadcastPkmnHPChanged(victim, -damageAmt);
                return damageAmt;
            }
        }
        /// <summary>
        /// Restores HP to <paramref name="pkmn"/> and broadcasts the HP changing if it changes.
        /// </summary>
        /// <param name="pkmn">The Pokémon receiving the HP.</param>
        /// <param name="hp">The amount of HP <paramref name="pkmn"/> will try to gain.</param>
        /// <returns>The amount of HP restored.</returns>
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
        void TypeCheck(PBEPokemon user, PBEPokemon target, PBEMove move, out PBEType moveType, out PBEEffectiveness effectiveness, ref double damageMultiplier, bool ignoreWonderGuard = false)
        {
            PBEPokemonData targetPData = PBEPokemonData.Data[target.Species];
            moveType = PBEMoveData.GetMoveTypeForPokemon(user, move);
            double mult = PBEPokemonData.TypeEffectiveness[(int)moveType, (int)targetPData.Type1];
            mult *= PBEPokemonData.TypeEffectiveness[(int)moveType, (int)targetPData.Type2];

            if (mult <= 0) // (-infinity, 0]
            {
                effectiveness = PBEEffectiveness.Ineffective;
            }
            else if (mult < 1) // (0, 1)
            {
                effectiveness = PBEEffectiveness.NotVeryEffective;
                if (user.Ability == PBEAbility.TintedLens)
                {
                    mult *= 2.0;
                }
            }
            else if (mult == 1.0) // [1, 1]
            {
                effectiveness = PBEEffectiveness.Normal;
            }
            else // (1, infinity)
            {
                effectiveness = PBEEffectiveness.SuperEffective;
                if (target.Ability == PBEAbility.Filter || target.Ability == PBEAbility.SolidRock)
                {
                    mult *= 0.75;
                }
            }
            damageMultiplier *= mult;

            if (effectiveness != PBEEffectiveness.Ineffective)
            {
                if ((target.Ability == PBEAbility.Levitate && moveType == PBEType.Ground)
                    || (!ignoreWonderGuard && target.Ability == PBEAbility.WonderGuard && effectiveness != PBEEffectiveness.SuperEffective))
                {
                    effectiveness = PBEEffectiveness.Ineffective;
                    damageMultiplier = 0;
                    BroadcastAbility(target, target, target.Ability, PBEAbilityAction.Damage);
                }
            }
        }

        ushort CalculateBasePower(PBEPokemon user, PBEPokemon target, PBEMove move, PBEType moveType, PBEMoveCategory moveCategory, byte power = 0, bool ignoreReflectLightScreen = false, bool ignoreLifeOrb = false, bool criticalHit = false)
        {
            double basePower = power;

            // If no overriding power is given, determine the move's basePower
            if (power == 0)
            {
                switch (move)
                {
                    case PBEMove.Eruption:
                    case PBEMove.WaterSpout:
                        basePower = Math.Min(1, 150 * user.HP / user.MaxHP);
                        break;
                    case PBEMove.Frustration:
                        basePower = Math.Max(1, (byte.MaxValue - user.Shell.Friendship) / 2.5);
                        break;
                    case PBEMove.GrassKnot:
                    case PBEMove.LowKick:
                        if (target.Weight >= 200.0)
                        {
                            basePower = 120;
                        }
                        else if (target.Weight >= 100.0)
                        {
                            basePower = 100;
                        }
                        else if (target.Weight >= 50.0)
                        {
                            basePower = 80;
                        }
                        else if (target.Weight >= 25.0)
                        {
                            basePower = 60;
                        }
                        else if (target.Weight >= 10.0)
                        {
                            basePower = 40;
                        }
                        else
                        {
                            basePower = 20;
                        }
                        break;
                    case PBEMove.HeatCrash:
                    case PBEMove.HeavySlam:
                        double relative = target.Weight / user.Weight;
                        if (relative <= 1 / 5D)
                        {
                            basePower = 120;
                        }
                        else if (relative <= 1 / 4D)
                        {
                            basePower = 100;
                        }
                        else if (relative <= 1 / 3D)
                        {
                            basePower = 80;
                        }
                        else if (relative <= 1 / 2D)
                        {
                            basePower = 60;
                        }
                        else
                        {
                            basePower = 40;
                        }
                        break;
                    case PBEMove.HiddenPower:
                        basePower = user.GetHiddenPowerBasePower();
                        break;
                    case PBEMove.Return:
                        basePower = Math.Max(1, user.Shell.Friendship / 2.5);
                        break;
                    default:
                        basePower = PBEMoveData.Data[move].Power;
                        break;
                }
            }
            // Move-specific power boosts
            switch (move)
            {
                case PBEMove.Brine:
                    // Brine gets a 100% power boost if the target is at or below 50% health
                    if (target.HP <= target.HP / 2)
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.Earthquake:
                case PBEMove.Magnitude:
                    // Earthquake and Magnitude get a 100% power boost if the target is Underground
                    if (target.Status2.HasFlag(PBEStatus2.Underground))
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.Facade:
                    // Facade gets a 100% power boost if the user is Burned, Paralyzed, Poisoned, or Badly Poisoned
                    if (user.Status1 == PBEStatus1.Burned || user.Status1 == PBEStatus1.Paralyzed || user.Status1 == PBEStatus1.Poisoned || user.Status1 == PBEStatus1.BadlyPoisoned)
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.Hex:
                    // Hex gets a 100% power boost if the target is afflicted with a status
                    if (target.Status1 != PBEStatus1.None)
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.Retaliate:
                    // Retaliate gets a 100% power boost if the user's team has a Pokémon that fainted during the previous turn
                    if (Teams[user.LocalTeam ? 0 : 1].MonFaintedLastTurn)
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.Steamroller:
                case PBEMove.Stomp:
                    // Stomp and Steamroller get a 100% power boost if the target is Minimized
                    if (target.Status2.HasFlag(PBEStatus2.Minimized))
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.Surf:
                    // Surf gets a 100% power boost if the target is Underwater
                    if (target.Status2.HasFlag(PBEStatus2.Underwater))
                    {
                        basePower *= 2.0;
                    }
                    break;
                case PBEMove.Venoshock:
                    // Venoshock gets a 100% power boost if the target is Poisoned
                    if (target.Status1 == PBEStatus1.Poisoned || target.Status1 == PBEStatus1.BadlyPoisoned)
                    {
                        basePower *= 2.0;
                    }
                    break;
            }

            switch (Weather)
            {
                case PBEWeather.HarshSunlight:
                    if (moveType == PBEType.Fire)
                    {
                        basePower *= 1.5;
                    }
                    else if (moveType == PBEType.Water)
                    {
                        basePower *= 0.5;
                    }
                    break;
                case PBEWeather.Rain:
                    if (moveType == PBEType.Water)
                    {
                        basePower *= 1.5;
                    }
                    else if (moveType == PBEType.Fire)
                    {
                        basePower *= 0.5;
                    }
                    break;
                case PBEWeather.Sandstorm:
                    if (user.Ability == PBEAbility.SandForce && (user.HasType(PBEType.Rock) || user.HasType(PBEType.Ground) || user.HasType(PBEType.Steel)))
                    {
                        basePower *= 1.3;
                    }
                    break;
            }

            // Reflect & Light Screen reduce damage by 50% if there is one active battler or by 33% if there is more than one
            if (!ignoreReflectLightScreen && !criticalHit)
            {
                PBETeam defenderTeam = Teams[target.LocalTeam ? 0 : 1];
                if ((defenderTeam.Status.HasFlag(PBETeamStatus.Reflect) && moveCategory == PBEMoveCategory.Physical)
                    || (defenderTeam.Status.HasFlag(PBETeamStatus.LightScreen) && moveCategory == PBEMoveCategory.Special))
                {
                    if (defenderTeam.NumPkmnOnField == 1)
                    {
                        basePower *= 0.5;
                    }
                    else
                    {
                        basePower *= 0.66;
                    }
                }
            }

            if (user.HasType(moveType))
            {
                if (user.Ability == PBEAbility.Adaptability)
                {
                    basePower *= 2.0;
                }
                else
                {
                    basePower *= 1.5;
                }
            }
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
                    }
                    break;
                case PBEType.Dark:
                    switch (user.Item)
                    {
                        case PBEItem.BlackGlasses:
                        case PBEItem.DreadPlate:
                            basePower *= 1.2;
                            break;
                    }
                    break;
                case PBEType.Dragon:
                    switch (user.Item)
                    {
                        case PBEItem.AdamantOrb:
                            if (user.Shell.Species == PBESpecies.Dialga)
                            {
                                basePower *= 1.2;
                            }
                            break;
                        case PBEItem.DracoPlate:
                        case PBEItem.DragonFang:
                            basePower *= 1.2;
                            break;
                        case PBEItem.GriseousOrb:
                            if (user.Shell.Species == PBESpecies.Giratina_Origin)
                            {
                                basePower *= 1.2;
                            }
                            break;
                        case PBEItem.LustrousOrb:
                            if (user.Shell.Species == PBESpecies.Palkia)
                            {
                                basePower *= 1.2;
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
                    }
                    break;
                case PBEType.Fighting:
                    switch (user.Item)
                    {
                        case PBEItem.BlackBelt:
                        case PBEItem.FistPlate:
                            basePower *= 1.2;
                            break;
                    }
                    break;
                case PBEType.Fire:
                    if (user.Ability == PBEAbility.Blaze && user.HP <= user.MaxHP / 3)
                    {
                        basePower *= 1.5;
                    }
                    if (target.Ability == PBEAbility.Heatproof)
                    {
                        basePower *= 0.5;
                    }
                    if (target.Ability == PBEAbility.ThickFat)
                    {
                        basePower *= 0.5;
                    }
                    switch (user.Item)
                    {
                        case PBEItem.Charcoal:
                        case PBEItem.FlamePlate:
                            basePower *= 1.2;
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
                    }
                    break;
                case PBEType.Ghost:
                    switch (user.Item)
                    {
                        case PBEItem.GriseousOrb:
                            if (user.Shell.Species == PBESpecies.Giratina_Origin)
                            {
                                basePower *= 1.2;
                            }
                            break;
                        case PBEItem.SpellTag:
                        case PBEItem.SpookyPlate:
                            basePower *= 1.2;
                            break;
                    }
                    break;
                case PBEType.Grass:
                    if (user.Ability == PBEAbility.Overgrow && user.HP <= user.MaxHP / 3)
                    {
                        basePower *= 1.5;
                    }
                    switch (user.Item)
                    {
                        case PBEItem.MeadowPlate:
                        case PBEItem.MiracleSeed:
                        case PBEItem.RoseIncense:
                            basePower *= 1.2;
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
                    }
                    break;
                case PBEType.Ice:
                    if (target.Ability == PBEAbility.ThickFat)
                    {
                        basePower *= 0.5;
                    }
                    switch (user.Item)
                    {
                        case PBEItem.IciclePlate:
                        case PBEItem.NeverMeltIce:
                            basePower *= 1.2;
                            break;
                    }
                    break;
                case PBEType.Normal:
                    switch (user.Item)
                    {
                        case PBEItem.SilkScarf:
                            basePower *= 1.2;
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
                    }
                    break;
                case PBEType.Steel:
                    switch (user.Item)
                    {
                        case PBEItem.AdamantOrb:
                            if (user.Shell.Species == PBESpecies.Dialga)
                            {
                                basePower *= 1.2;
                            }
                            break;
                        case PBEItem.IronPlate:
                        case PBEItem.MetalCoat:
                            basePower *= 1.2;
                            break;
                    }
                    break;
                case PBEType.Water:
                    if (user.Ability == PBEAbility.Torrent && user.HP <= user.MaxHP / 3)
                    {
                        basePower *= 1.5;
                    }
                    switch (user.Item)
                    {
                        case PBEItem.LustrousOrb:
                            if (user.Shell.Species == PBESpecies.Palkia)
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
                    }
                    break;
            }

            // Life Orb boosts power but deals damage to the user
            if (!ignoreLifeOrb && user.Item == PBEItem.LifeOrb)
            {
                basePower = basePower * 5324 / 4096;
            }
            // A Pikachu holding a Light Ball gets a 100% power boost
            if (user.Item == PBEItem.LightBall && user.Shell.Species == PBESpecies.Pikachu)
            {
                basePower *= 2.0;
            }
            // Physical moves' power gets a 10% boost if the user is holding a Muscle Band
            if (moveCategory == PBEMoveCategory.Physical && user.Item == PBEItem.MuscleBand)
            {
                basePower *= 1.1;
            }
            // Special moves' power gets a 10% boost if the user is holding a Wise Glasses
            if (moveCategory == PBEMoveCategory.Special && user.Item == PBEItem.WiseGlasses)
            {
                basePower *= 1.1;
            }
            // Physical moves' power is halved from a Burned Pokémon unless it has Guts
            if (moveCategory == PBEMoveCategory.Physical && user.Status1 == PBEStatus1.Burned && user.Ability != PBEAbility.Guts)
            {
                basePower *= 0.5;
            }

            return (ushort)basePower;
        }
        ushort CalculateAttack(PBEPokemon user, PBEPokemon target, bool criticalHit = false)
        {
            // Negative Attack changes are ignored for critical hits
            double attack = user.Attack * GetStatChangeModifier(criticalHit ? Math.Max((sbyte)0, user.AttackChange) : user.AttackChange, false);

            if (user.Ability == PBEAbility.HugePower || user.Ability == PBEAbility.PurePower)
            {
                attack *= 2.0;
            }
            // A Cubone or Marowak holding a Thick Club gets a 100% Attack boost
            if (user.Item == PBEItem.ThickClub && (user.Shell.Species == PBESpecies.Cubone || user.Shell.Species == PBESpecies.Marowak))
            {
                attack *= 2.0;
            }
            if (user.Ability == PBEAbility.Hustle)
            {
                attack *= 1.5;
            }
            if (user.Ability == PBEAbility.Guts && user.Status1 != PBEStatus1.None)
            {
                attack *= 1.5;
            }
            // A Pokémon holding a Choice Band gets a 50% Attack boost
            if (user.Item == PBEItem.ChoiceBand)
            {
                attack *= 1.5;
            }

            return (ushort)attack;
        }
        ushort CalculateDefense(PBEPokemon user, PBEPokemon target, bool criticalHit = false)
        {
            // Positive Defense changes are ignored for critical hits
            double defense = user.Defense * GetStatChangeModifier(criticalHit ? Math.Min((sbyte)0, target.DefenseChange) : target.DefenseChange, false);

            // A Ditto holding a Metal Powder gets a 100% Defense boost
            if (target.Item == PBEItem.MetalPowder && target.Species == PBESpecies.Ditto)
            {
                defense *= 2.0;
            }
            if (target.Ability == PBEAbility.MarvelScale && target.Status1 != PBEStatus1.None)
            {
                defense *= 1.5;
            }

            return (ushort)defense;
        }
        ushort CalculateSpAttack(PBEPokemon user, PBEPokemon target, bool criticalHit = false)
        {
            // Negative SpAttack changes are ignored for critical hits
            double spAttack = user.SpAttack * GetStatChangeModifier(criticalHit ? Math.Max((sbyte)0, user.SpAttackChange) : user.SpAttackChange, false);

            // A Clamperl holding a Deep Sea Tooth gets a 100% SpAttack boost
            if (user.Item == PBEItem.DeepSeaTooth && user.Shell.Species == PBESpecies.Clamperl)
            {
                spAttack *= 2.0;
            }
            if (Weather == PBEWeather.HarshSunlight && user.Ability == PBEAbility.SolarPower)
            {
                spAttack *= 1.5;
            }
            // A Latios or Latias holding a Soul Dew gets a 50% SpAttack boost
            if (user.Item == PBEItem.SoulDew && (user.Shell.Species == PBESpecies.Latias || user.Shell.Species == PBESpecies.Latios))
            {
                spAttack *= 1.5;
            }
            // A Pokémon holding a Choice Specs gets a 50% SpAttack boost
            if (user.Item == PBEItem.ChoiceSpecs)
            {
                spAttack *= 1.5;
            }

            return (ushort)spAttack;
        }
        ushort CalculateSpDefense(PBEPokemon user, PBEPokemon target, bool criticalHit = false)
        {
            // Positive SpDefense changes are ignored for critical hits
            double spDefense = user.SpDefense * GetStatChangeModifier(criticalHit ? Math.Min((sbyte)0, target.SpDefenseChange) : target.SpDefenseChange, false);

            // A Clamperl holding a Deep Sea Scale gets a 100% SpDefense boost
            if (target.Item == PBEItem.DeepSeaScale && target.Shell.Species == PBESpecies.Clamperl)
            {
                spDefense *= 2.0;
            }
            // A Latios or Latias holding a Soul Dew gets a 50% SpDefense boost
            if (target.Item == PBEItem.SoulDew && (target.Shell.Species == PBESpecies.Latias || target.Shell.Species == PBESpecies.Latios))
            {
                spDefense *= 1.5;
            }
            // A Rock-type Pokémon in a Sandstorm gets a 50% SpDefense boost
            if (Weather == PBEWeather.Sandstorm && target.HasType(PBEType.Rock))
            {
                spDefense *= 1.5;
            }

            return (ushort)spDefense;
        }

        // If moveCategory is PBEMoveCategory.MAX, category is determined by the move
        // If power is 0, power is determined by the move
        ushort CalculateDamage(PBEPokemon user, PBEPokemon target, PBEMove move, PBEType moveType, PBEMoveCategory moveCategory = PBEMoveCategory.MAX, byte power = 0, bool ignoreReflectLightScreen = false, bool ignoreLifeOrb = false, bool criticalHit = false)
        {
            if (moveCategory == PBEMoveCategory.MAX)
            {
                moveCategory = PBEMoveData.Data[move].Category;
            }
            ushort damage;
            ushort a = 0, d = 0,
                p = CalculateBasePower(user, target, move, moveType, moveCategory, power, ignoreReflectLightScreen, ignoreLifeOrb, criticalHit);

            switch (move)
            {
                case PBEMove.Psyshock:
                case PBEMove.Psystrike:
                case PBEMove.SecretSword:
                    a = CalculateSpAttack(user, target, criticalHit);
                    d = CalculateDefense(user, target, criticalHit);
                    break;
                default:
                    if (moveCategory == PBEMoveCategory.Physical)
                    {
                        a = CalculateAttack(user, target, criticalHit);
                        d = CalculateDefense(user, target, criticalHit);
                    }
                    else if (moveCategory == PBEMoveCategory.Special)
                    {
                        a = CalculateSpAttack(user, target, criticalHit);
                        d = CalculateSpDefense(user, target, criticalHit);
                    }
                    break;
            }

            damage = (ushort)(2 * user.Shell.Level / 5 + 2);
            damage = (ushort)(damage * a * p / d);
            damage /= 50;
            return (ushort)(damage + 2);
        }
    }
}
