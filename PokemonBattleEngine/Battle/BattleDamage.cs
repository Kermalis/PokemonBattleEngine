using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        // "forMissing" is true if the multiplier will be used for accuracy or evasion
        // If this is used for evasion, "change" should be multiplied by -1 before being passed in (because evasion is for reducing a hit chance, not increasing)
        public double GetStatMultiplier(sbyte change, bool forMissing = false)
        {
            double baseVal = forMissing ? 3 : 2;
            double numerator = Math.Max(baseVal, baseVal + change);
            double denominator = Math.Max(baseVal, baseVal - change);
            return numerator / denominator;
        }

        PEffectiveness TypeCheck(PPokemon attacker, PPokemon defender)
        {
            PPokemonData attackerPData = PPokemonData.Data[attacker.Shell.Species];
            PPokemonData defenderPData = PPokemonData.Data[defender.Shell.Species];
            double effectiveness = 1;

            bMoveType = PMoveData.GetMoveTypeForPokemon(attacker, bMove);

            // If a pokemon uses a move that shares a type with it, it gains a 1.5x power boost
            if (attackerPData.HasType(bMoveType))
                bDamageMultiplier *= 1.5;

            effectiveness *= PPokemonData.TypeEffectiveness[(int)bMoveType, (int)defenderPData.Type1];
            // Don't want to halve twice for a mono type
            if (defenderPData.Type1 != defenderPData.Type2)
                effectiveness *= PPokemonData.TypeEffectiveness[(int)bMoveType, (int)defenderPData.Type2];

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
        ushort CalculateBasePower(PPokemon attacker, PPokemon defender, byte power, PMoveCategory category, bool ignoreReflectLightScreen)
        {
            PPokemonData defenderPData = PPokemonData.Data[defender.Shell.Species];
            double basePower = power;

            // Moves with variable base power
            if (power == 0)
            {
                switch (bMove)
                {
                    case PMove.Frustration:
                        basePower = Math.Max(1, (byte.MaxValue - attacker.Shell.Friendship) / 2.5);
                        break;
                    case PMove.GrassKnot:
                    case PMove.LowKick:
                        if (defenderPData.Weight >= 200.0)
                            basePower = 120;
                        else if (defenderPData.Weight >= 100.0)
                            basePower = 100;
                        else if (defenderPData.Weight >= 50.0)
                            basePower = 80;
                        else if (defenderPData.Weight >= 25.0)
                            basePower = 60;
                        else if (defenderPData.Weight >= 10.0)
                            basePower = 40;
                        else
                            basePower = 20;
                        break;
                    case PMove.HiddenPower:
                        basePower = attacker.GetHiddenPowerBasePower();
                        break;
                    case PMove.Return:
                        basePower = Math.Max(1, attacker.Shell.Friendship / 2.5);
                        break;
                    default:
                        basePower = PMoveData.Data[bMove].Power;
                        break;
                }
            }

            // Reflect & Light Screen reduce damage by 50% if there is one active battler or by 33% if there is more than one
            if (!ignoreReflectLightScreen && !bLandedCrit)
            {
                PTeam defenderTeam = teams[defender.Local ? 0 : 1];
                if ((defenderTeam.ReflectCount > 0 && category == PMoveCategory.Physical)
                    || (defenderTeam.LightScreenCount > 0 && category == PMoveCategory.Special))
                {
                    if (defenderTeam.NumPkmnOnField == 1)
                        basePower *= 0.5;
                    else
                        basePower *= 0.66;
                }
            }

            // TODO: Stuff like mystic water

            // Damage is halved when using electric moves while mud sport is active
            /* if (type == Type.Electric && MudSportActive())
             * basePower /= 2;*/
            // Damage is halved when using fire moves while water sport is active
            /* if (type == Type.Fire && WaterSportActive())
             * basePower /= 2;*/

            // A Pikachu holding a Light Ball gets a 2x power boost
            if (attacker.Item == PItem.LightBall && attacker.Shell.Species == PSpecies.Pikachu)
                basePower *= 2;
            // Retaliate doubles power if the team has a pokemon that fainted the previous turn
            if (bMove == PMove.Retaliate && teams[attacker.Local ? 0 : 1].MonFaintedLastTurn)
                basePower *= 2;
            // Overgrow gives a 1.5x boost to Grass attacks if the efCurAttacker is below 1/3 max HP
            if (bMoveType == PType.Grass && attacker.Ability == PAbility.Overgrow && attacker.HP <= attacker.MaxHP / 3)
                basePower *= 1.5;
            // Blaze gives a 1.5x boost to Fire attacks if the efCurAttacker is below 1/3 max HP
            if (bMoveType == PType.Fire && attacker.Ability == PAbility.Blaze && attacker.HP <= attacker.MaxHP / 3)
                basePower *= 1.5;
            // Torrent gives a 1.5x boost to Water attacks if the efCurAttacker is below 1/3 max HP
            if (bMoveType == PType.Water && attacker.Ability == PAbility.Torrent && attacker.HP <= attacker.MaxHP / 3)
                basePower *= 1.5;
            // Swarm gives a 1.5x boost to Bug attacks if the efCurAttacker is below 1/3 max HP
            if (bMoveType == PType.Bug && attacker.Ability == PAbility.Swarm && attacker.HP <= attacker.MaxHP / 3)
                basePower *= 1.5;
            // A Burned pokemon does half the damage when it is Burned unless it has the Guts ability
            if (category == PMoveCategory.Physical && attacker.Status1 == PStatus1.Burned && attacker.Ability != PAbility.Guts)
                basePower /= 2;
            // Damage is halved when using Fire or Ice moves against a pokemon with the Thick Fat ability
            if (defender.Ability == PAbility.ThickFat && (bMoveType == PType.Fire || bMoveType == PType.Ice))
                basePower /= 2;

            return (ushort)basePower;
        }
        ushort CalculateAttack(PPokemon attacker, PPokemon defender)
        {
            double attack = attacker.Attack * GetStatMultiplier(attacker.AttackChange);

            // Pokemon with the Huge Power or Pure Power ability get a 2x attack boost
            if (attacker.Ability == PAbility.HugePower || attacker.Ability == PAbility.PurePower)
                attack *= 2;
            // A Cubone or Marowak holding a Thick Club gets a 2x attack boost
            if (attacker.Item == PItem.ThickClub && (attacker.Shell.Species == PSpecies.Cubone || attacker.Shell.Species == PSpecies.Marowak))
                attack *= 2;
            // A pokemon with the Hustle ability gets a 1.5x attack boost
            if (attacker.Ability == PAbility.Hustle)
                attack *= 1.5;
            // A pokemon with the Guts ability gets a 1.5x attack boost when afflicted with a status
            if (attacker.Ability == PAbility.Guts && attacker.Status1 != PStatus1.None)
                attack *= 1.5;
            // A pokemon holding a Choice Band gets a 1.5x attack boost
            if (attacker.Item == PItem.ChoiceBand)
                attack *= 1.5;

            return (ushort)attack;
        }
        ushort CalculateDefense(PPokemon attacker, PPokemon defender)
        {
            double defense = attacker.Defense * GetStatMultiplier(defender.DefenseChange);

            // A Ditto holding a Metal Powder gets a 2x defense boost
            if (defender.Item == PItem.MetalPowder && defender.Shell.Species == PSpecies.Ditto)
                defense *= 2;
            // A pokemon with the Marvel Scale ability gets a 1.5x defense boost when afflicted with a status
            if (defender.Ability == PAbility.MarvelScale && defender.Status1 != PStatus1.None)
                defense *= 1.5;

            return (ushort)defense;
        }
        ushort CalculateSpAttack(PPokemon attacker, PPokemon defender)
        {
            double spAttack = attacker.SpAttack * GetStatMultiplier(attacker.SpAttackChange);

            // TODO:
            // A pokemon with the Plus ability gets a 1.5x spAttack boost if a teammate has the Minus ability
            /* if (efCurAttacker.Ability == Ability.Plus && PartnerHasAbility(Ability.Minus))
             * spAttack *= 1.5;*/
            // A pokemon with the Minus ability gets a 1.5x spAttack boost if a teammate has the Plus ability
            /* if (efCurAttacker.Ability == Ability.Minus && PartnerHasAbility(Ability.Plus))
             * spAttack *= 1.5;*/

            // A Clamperl holding a Deep Sea Tooth gets a 2x spAttack boost
            if (attacker.Item == PItem.DeepSeaTooth && attacker.Shell.Species == PSpecies.Clamperl)
                spAttack *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spAttack boost
            if (attacker.Item == PItem.SoulDew && (attacker.Shell.Species == PSpecies.Latios || attacker.Shell.Species == PSpecies.Latias))
                spAttack *= 1.5;

            return (ushort)spAttack;
        }
        ushort CalculateSpDefense(PPokemon attacker, PPokemon defender)
        {
            double spDefense = attacker.SpDefense * GetStatMultiplier(defender.SpDefenseChange);

            // A Clamperl holding a Deep Sea Scale gets a 2x spDefense boost
            if (defender.Item == PItem.DeepSeaScale && defender.Shell.Species == PSpecies.Clamperl)
                spDefense *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spDefense boost
            if (defender.Item == PItem.SoulDew && (defender.Shell.Species == PSpecies.Latios || defender.Shell.Species == PSpecies.Latias))
                spDefense *= 1.5;

            return (ushort)spDefense;
        }

        ushort CalculateDamage()
            => CalculateDamage(bAttacker, bDefender, 0, PMoveData.Data[bMove].Category, false);
        // If power is 0, power is determined by bMove
        ushort CalculateDamage(PPokemon attacker, PPokemon defender, byte power, PMoveCategory category, bool ignoreReflectLightScreen)
        {
            ushort damage;
            ushort a = 0, d = 0,
                p = CalculateBasePower(attacker, defender, power, category, ignoreReflectLightScreen);

            // TODO: Determine a and d for moves like Foul Play and Psyshock

            if (category == PMoveCategory.Physical)
            {
                a = CalculateAttack(attacker, defender);
                d = CalculateDefense(attacker, defender);
            }
            else if (category == PMoveCategory.Special)
            {
                a = CalculateSpAttack(attacker, defender);
                d = CalculateSpDefense(attacker, defender);
            }

            damage = (ushort)(2 * attacker.Shell.Level / 5 + 2);
            damage = (ushort)(damage * a * p / d);
            damage /= 50;
            return (ushort)(damage + 2);
        }
    }
}
