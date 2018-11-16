using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        // "forMissing" is true if the multiplier will be used for accuracy or evasion
        public double GetStatMultiplier(sbyte change, bool forMissing = false)
        {
            double baseVal = forMissing ? 3 : 2;
            double numerator = Math.Max(baseVal, baseVal + change);
            double denominator = Math.Max(baseVal, baseVal - change);
            return numerator / denominator;
        }

        PEffectiveness TypeCheck(PPokemon user, PPokemon target)
        {
            PPokemonData userPData = PPokemonData.Data[user.Species];
            PPokemonData targetPData = PPokemonData.Data[target.Species];
            double effectiveness = 1;

            bMoveType = PMoveData.GetMoveTypeForPokemon(user, bMove);

            // If a pokemon uses a move that shares a type with it, it gains a 1.5x power boost (2x if it has adaptability)
            if (userPData.HasType(bMoveType))
            {
                if (user.Ability == PAbility.Adaptability)
                    bDamageMultiplier *= 2.0;
                else
                    bDamageMultiplier *= 1.5;
            }
            // Pokémon with the heatproof take half as much damage from fire attacks
            if (bMoveType == PType.Fire && target.Ability == PAbility.Heatproof)
                bDamageMultiplier *= 0.5;

            effectiveness *= PPokemonData.TypeEffectiveness[(int)bMoveType, (int)targetPData.Type1];
            // Don't want to halve twice for a mono type
            if (targetPData.Type1 != targetPData.Type2)
                effectiveness *= PPokemonData.TypeEffectiveness[(int)bMoveType, (int)targetPData.Type2];

            if (effectiveness == 0)
                return PEffectiveness.Ineffective;
            else if (effectiveness == 0.5)
                return PEffectiveness.NotVeryEffective;
            else if (effectiveness == 1.0)
                return PEffectiveness.Normal;
            else
                return PEffectiveness.SuperEffective;
        }

        // If power is 0, power is determined by bMove
        ushort CalculateBasePower(PPokemon user, PPokemon target, byte power, PMoveCategory category, bool ignoreReflectLightScreen, bool ignoreLifeOrb)
        {
            PPokemonData targetPData = PPokemonData.Data[target.Species];
            double basePower = power;

            // Moves with variable base power
            if (power == 0)
            {
                switch (bMove)
                {
                    case PMove.Frustration:
                        basePower = Math.Max(1, (byte.MaxValue - user.Shell.Friendship) / 2.5);
                        break;
                    case PMove.GrassKnot:
                    case PMove.LowKick:
                        if (targetPData.Weight >= 200.0)
                            basePower = 120;
                        else if (targetPData.Weight >= 100.0)
                            basePower = 100;
                        else if (targetPData.Weight >= 50.0)
                            basePower = 80;
                        else if (targetPData.Weight >= 25.0)
                            basePower = 60;
                        else if (targetPData.Weight >= 10.0)
                            basePower = 40;
                        else
                            basePower = 20;
                        break;
                    case PMove.HiddenPower:
                        basePower = user.GetHiddenPowerBasePower();
                        break;
                    case PMove.Retaliate:
                        basePower = PMoveData.Data[bMove].Power;
                        // Retaliate doubles power if the team has a Pokémon that fainted the previous turn
                        if (teams[user.Local ? 0 : 1].MonFaintedLastTurn)
                            basePower *= 2;
                        break;
                    case PMove.Return:
                        basePower = Math.Max(1, user.Shell.Friendship / 2.5);
                        break;
                    default:
                        basePower = PMoveData.Data[bMove].Power;
                        break;
                }
            }

            switch (Weather)
            {
                case PWeather.Raining:
                    if (bMoveType == PType.Water)
                        basePower *= 1.5;
                    else if (bMoveType == PType.Fire)
                        basePower *= 0.5;
                    break;
                case PWeather.Sunny:
                    if (bMoveType == PType.Fire)
                        basePower *= 1.5;
                    else if (bMoveType == PType.Water)
                        basePower *= 0.5;
                    break;
            }

            // Reflect & Light Screen reduce damage by 50% if there is one active battler or by 33% if there is more than one
            if (!ignoreReflectLightScreen && !bLandedCrit)
            {
                PTeam defenderTeam = teams[target.Local ? 0 : 1];
                if ((defenderTeam.ReflectCount > 0 && category == PMoveCategory.Physical)
                    || (defenderTeam.LightScreenCount > 0 && category == PMoveCategory.Special))
                {
                    if (defenderTeam.NumPkmnOnField == 1)
                        basePower *= 0.5;
                    else
                        basePower *= 0.66;
                }
            }

            switch (bMoveType)
            {
                case PType.Bug:
                    if (user.Item == PItem.InsectPlate)
                        basePower *= 1.2;
                    break;
                case PType.Dark:
                    if (user.Item == PItem.DreadPlate)
                        basePower *= 1.2;
                    break;
                case PType.Dragon:
                    if (user.Item == PItem.DracoPlate)
                        basePower *= 1.2;
                    break;
                case PType.Electric:
                    if (user.Item == PItem.ZapPlate)
                        basePower *= 1.2;
                    break;
                case PType.Fighting:
                    if (user.Item == PItem.FistPlate)
                        basePower *= 1.2;
                    break;
                case PType.Fire:
                    if (user.Item == PItem.FlamePlate)
                        basePower *= 1.2;
                    break;
                case PType.Flying:
                    if (user.Item == PItem.SkyPlate)
                        basePower *= 1.2;
                    break;
                case PType.Ghost:
                    if (user.Item == PItem.SpookyPlate)
                        basePower *= 1.2;
                    break;
                case PType.Grass:
                    if (user.Item == PItem.MeadowPlate)
                        basePower *= 1.2;
                    break;
                case PType.Ground:
                    if (user.Item == PItem.EarthPlate)
                        basePower *= 1.2;
                    break;
                case PType.Ice:
                    if (user.Item == PItem.IciclePlate)
                        basePower *= 1.2;
                    break;
                case PType.Normal:
                    break;
                case PType.Poison:
                    if (user.Item == PItem.ToxicPlate)
                        basePower *= 1.2;
                    break;
                case PType.Psychic:
                    if (user.Item == PItem.MindPlate)
                        basePower *= 1.2;
                    break;
                case PType.Rock:
                    if (user.Item == PItem.StonePlate)
                        basePower *= 1.2;
                    break;
                case PType.Steel:
                    if (user.Item == PItem.IronPlate)
                        basePower *= 1.2;
                    break;
                case PType.Water:
                    if (user.Item == PItem.SplashPlate)
                        basePower *= 1.2;
                    break;
            }

            // Life Orb boosts power but deals damage to the user
            if (!ignoreLifeOrb && user.Item == PItem.LifeOrb)
                basePower = basePower * 5324 / 4096;
            // A Pikachu holding a Light Ball gets a 2x power boost
            if (user.Item == PItem.LightBall && user.Shell.Species == PSpecies.Pikachu)
                basePower *= 2;
            // Overgrow gives a 1.5x boost to Grass attacks if the efCurAttacker is below 1/3 max HP
            if (bMoveType == PType.Grass && user.Ability == PAbility.Overgrow && user.HP <= user.MaxHP / 3)
                basePower *= 1.5;
            // Blaze gives a 1.5x boost to Fire attacks if the efCurAttacker is below 1/3 max HP
            if (bMoveType == PType.Fire && user.Ability == PAbility.Blaze && user.HP <= user.MaxHP / 3)
                basePower *= 1.5;
            // Torrent gives a 1.5x boost to Water attacks if the efCurAttacker is below 1/3 max HP
            if (bMoveType == PType.Water && user.Ability == PAbility.Torrent && user.HP <= user.MaxHP / 3)
                basePower *= 1.5;
            // Swarm gives a 1.5x boost to Bug attacks if the efCurAttacker is below 1/3 max HP
            if (bMoveType == PType.Bug && user.Ability == PAbility.Swarm && user.HP <= user.MaxHP / 3)
                basePower *= 1.5;
            // A Burned pokemon does half the damage when it is Burned unless it has the Guts ability
            if (category == PMoveCategory.Physical && user.Status1 == PStatus1.Burned && user.Ability != PAbility.Guts)
                basePower /= 2;
            // Damage is halved when using Fire or Ice moves against a pokemon with the Thick Fat ability
            if (target.Ability == PAbility.ThickFat && (bMoveType == PType.Fire || bMoveType == PType.Ice))
                basePower /= 2;

            return (ushort)basePower;
        }
        ushort CalculateAttack(PPokemon user, PPokemon target)
        {
            // Negative Attack changes are ignored for critical hits
            double attack = user.Attack * GetStatMultiplier(bLandedCrit ? Math.Max((sbyte)0, user.AttackChange) : user.AttackChange);

            // Pokemon with the Huge Power or Pure Power ability get a 2x attack boost
            if (user.Ability == PAbility.HugePower || user.Ability == PAbility.PurePower)
                attack *= 2;
            // A Cubone or Marowak holding a Thick Club gets a 2x attack boost
            if (user.Item == PItem.ThickClub && (user.Shell.Species == PSpecies.Cubone || user.Shell.Species == PSpecies.Marowak))
                attack *= 2;
            // A pokemon with the Hustle ability gets a 1.5x attack boost
            if (user.Ability == PAbility.Hustle)
                attack *= 1.5;
            // A pokemon with the Guts ability gets a 1.5x attack boost when afflicted with a status
            if (user.Ability == PAbility.Guts && user.Status1 != PStatus1.None)
                attack *= 1.5;
            // A pokemon holding a Choice Band gets a 1.5x attack boost
            if (user.Item == PItem.ChoiceBand)
                attack *= 1.5;

            return (ushort)attack;
        }
        ushort CalculateDefense(PPokemon user, PPokemon target)
        {
            // Positive Defense changes are ignored for critical hits
            double defense = user.Defense * GetStatMultiplier(bLandedCrit ? Math.Min((sbyte)0, target.DefenseChange) : target.DefenseChange);

            // A Ditto holding a Metal Powder gets a 2x defense boost
            if (target.Item == PItem.MetalPowder && target.Species == PSpecies.Ditto)
                defense *= 2;
            // A pokemon with the Marvel Scale ability gets a 1.5x defense boost when afflicted with a status
            if (target.Ability == PAbility.MarvelScale && target.Status1 != PStatus1.None)
                defense *= 1.5;

            return (ushort)defense;
        }
        ushort CalculateSpAttack(PPokemon user, PPokemon target)
        {
            // Negative SpAttack changes are ignored for critical hits
            double spAttack = user.SpAttack * GetStatMultiplier(bLandedCrit ? Math.Max((sbyte)0, user.SpAttackChange) : user.SpAttackChange);

            // TODO:
            // A pokemon with the Plus ability gets a 1.5x spAttack boost if a teammate has the Minus ability
            /* if (efCurAttacker.Ability == Ability.Plus && PartnerHasAbility(Ability.Minus))
             * spAttack *= 1.5;*/
            // A pokemon with the Minus ability gets a 1.5x spAttack boost if a teammate has the Plus ability
            /* if (efCurAttacker.Ability == Ability.Minus && PartnerHasAbility(Ability.Plus))
             * spAttack *= 1.5;*/

            // A Clamperl holding a Deep Sea Tooth gets a 2x spAttack boost
            if (user.Item == PItem.DeepSeaTooth && user.Shell.Species == PSpecies.Clamperl)
                spAttack *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spAttack boost
            if (user.Item == PItem.SoulDew && (user.Shell.Species == PSpecies.Latias || user.Shell.Species == PSpecies.Latios))
                spAttack *= 1.5;
            // A Pokémon holding a Choice Specs gets a 1.5x spAttack boost
            if (user.Item == PItem.ChoiceSpecs)
                spAttack *= 1.5;

            return (ushort)spAttack;
        }
        ushort CalculateSpDefense(PPokemon user, PPokemon target)
        {
            // Positive SpDefense changes are ignored for critical hits
            double spDefense = user.SpDefense * GetStatMultiplier(bLandedCrit ? Math.Min((sbyte)0, target.SpDefenseChange) : target.SpDefenseChange);

            // A Clamperl holding a Deep Sea Scale gets a 2x spDefense boost
            if (target.Item == PItem.DeepSeaScale && target.Shell.Species == PSpecies.Clamperl)
                spDefense *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spDefense boost
            if (target.Item == PItem.SoulDew && (target.Shell.Species == PSpecies.Latias || target.Shell.Species == PSpecies.Latios))
                spDefense *= 1.5;

            return (ushort)spDefense;
        }

        ushort CalculateDamage()
            => CalculateDamage(bUser, bTarget, 0, PMoveData.Data[bMove].Category, false, false);
        // If power is 0, power is determined by bMove
        ushort CalculateDamage(PPokemon user, PPokemon target, byte power, PMoveCategory category, bool ignoreReflectLightScreen, bool ignoreLifeOrb)
        {
            ushort damage;
            ushort a = 0, d = 0,
                p = CalculateBasePower(user, target, power, category, ignoreReflectLightScreen, ignoreLifeOrb);

            // TODO: Determine a and d for moves like Foul Play and Psyshock

            if (category == PMoveCategory.Physical)
            {
                a = CalculateAttack(user, target);
                d = CalculateDefense(user, target);
            }
            else if (category == PMoveCategory.Special)
            {
                a = CalculateSpAttack(user, target);
                d = CalculateSpDefense(user, target);
            }

            damage = (ushort)(2 * user.Shell.Level / 5 + 2);
            damage = (ushort)(damage * a * p / d);
            damage /= 50;
            return (ushort)(damage + 2);
        }
    }
}
