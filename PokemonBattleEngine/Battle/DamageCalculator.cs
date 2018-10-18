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
            double numerator = Math.Min(baseVal, baseVal + change);
            double denominator = Math.Min(baseVal, baseVal - change);
            return numerator / denominator;
        }

        ushort CalculateBasePower(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double basePower = mData.Power;


            // TODO: Stuff like mystic water
            // Return/Frustration

            // Damage is halved when using electric moves while mud sport is active
            /* if (type == Type.Electric && MudSportActive())
             * basePower /= 2;*/
            // Damage is halved when using fire moves while water sport is active
            /* if (type == Type.Fire && WaterSportActive())
             * basePower /= 2;*/
            // An underground pokemon that gets hit with Earthquake takes twice the damage
            /* if (move == PMove.Earthquake && defender.Pokemon.Status2.HasFlag(PStatus2.Underground))
                basePower *= 2;*/
            // An underwater pokemon that gets hit with Surf takes twice the damage
            /* if (move == PMove.Surf && defender.Pokemon.Status2.HasFlag(PStatus2.Underwater))
                basePower *= 2;*/

            // A Pikachu holding a Light Ball gets a 2x power boost
            if (attacker.Mon.Shell.Item == PItem.LightBall && attacker.Mon.Shell.Species == PSpecies.Pikachu)
                basePower *= 2;
            // Retaliate doubles power if the team has a pokemon that fainted the previous turn
            if (move == PMove.Retaliate && attacker.Team.MonFaintedLastTurn)
                basePower *= 2;
            // Overgrow gives a 1.5x boost to Grass attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Grass && attacker.Mon.Shell.Ability == PAbility.Overgrow && attacker.Mon.HP <= attacker.Mon.MaxHP / 3)
                basePower *= 1.5;
            // Blaze gives a 1.5x boost to Fire attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Fire && attacker.Mon.Shell.Ability == PAbility.Blaze && attacker.Mon.HP <= attacker.Mon.MaxHP / 3)
                basePower *= 1.5;
            // Torrent gives a 1.5x boost to Water attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Water && attacker.Mon.Shell.Ability == PAbility.Torrent && attacker.Mon.HP <= attacker.Mon.MaxHP / 3)
                basePower *= 1.5;
            // Swarm gives a 1.5x boost to Bug attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Bug && attacker.Mon.Shell.Ability == PAbility.Swarm && attacker.Mon.HP <= attacker.Mon.MaxHP / 3)
                basePower *= 1.5;
            // A burned pokemon does half the damage when it is Burned unless it has the Guts ability
            if (mData.Category == PMoveCategory.Physical && attacker.Mon.Status == PStatus.Burned && attacker.Mon.Shell.Ability != PAbility.Guts)
                basePower /= 2;
            // Damage is halved when using Fire or Ice moves against a pokemon with the Thick Fat ability
            if (defender.Mon.Shell.Ability == PAbility.ThickFat && (mData.Type == PType.Fire || mData.Type == PType.Ice))
                basePower /= 2;

            return (ushort)basePower;
        }
        ushort CalculateAttack(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double attack = attacker.Mon.Attack * GetStatMultiplier(attacker.Mon.AttackChange);

            // Pokemon with the Huge Power or Pure Power ability get a 2x attack boost
            if (attacker.Mon.Shell.Ability == PAbility.HugePower || attacker.Mon.Shell.Ability == PAbility.PurePower)
                attack *= 2;
            // A Cubone or Marowak holding a Thick Club gets a 2x attack boost
            if (attacker.Mon.Shell.Item == PItem.ThickClub && (attacker.Mon.Shell.Species == PSpecies.Cubone || attacker.Mon.Shell.Species == PSpecies.Marowak))
                attack *= 2;
            // A pokemon with the Hustle ability gets a 1.5x attack boost
            if (attacker.Mon.Shell.Ability == PAbility.Hustle)
                attack *= 1.5;
            // A pokemon with the Guts ability gets a 1.5x attack boost when afflicted with a status
            if (attacker.Mon.Shell.Ability == PAbility.Guts && attacker.Mon.Status != PStatus.NoStatus)
                attack *= 1.5;
            // A pokemon holding a Choice Band gets a 1.5x attack boost
            if (attacker.Mon.Shell.Item == PItem.ChoiceBand)
                attack *= 1.5;

            return (ushort)attack;
        }
        ushort CalculateDefense(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double defense = attacker.Mon.Defense * GetStatMultiplier(defender.Mon.DefenseChange);

            // A Ditto holding a Metal Powder gets a 2x defense boost
            if (defender.Mon.Shell.Item == PItem.MetalPowder && defender.Mon.Shell.Species == PSpecies.Ditto)
                defense *= 2;
            // A pokemon with the Marvel Scale ability gets a 1.5x defense boost when afflicted with a status
            if (defender.Mon.Shell.Ability == PAbility.MarvelScale && defender.Mon.Status != PStatus.NoStatus)
                defense *= 1.5;

            return (ushort)defense;
        }
        ushort CalculateSpAttack(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double spAttack = attacker.Mon.SpAttack * GetStatMultiplier(attacker.Mon.SpAttackChange);

            // TODO:
            // A pokemon with the Plus ability gets a 1.5x spAttack boost if a teammate has the Minus ability
            /* if (attacker.Mon.Ability == Ability.Plus && PartnerHasAbility(Ability.Minus))
             * spAttack *= 1.5;*/
            // A pokemon with the Minus ability gets a 1.5x spAttack boost if a teammate has the Plus ability
            /* if (attacker.Mon.Ability == Ability.Minus && PartnerHasAbility(Ability.Plus))
             * spAttack *= 1.5;*/

            // A Clamperl holding a Deep Sea Tooth gets a 2x spAttack boost
            if (attacker.Mon.Shell.Item == PItem.DeepSeaTooth && attacker.Mon.Shell.Species == PSpecies.Clamperl)
                spAttack *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spAttack boost
            if (attacker.Mon.Shell.Item == PItem.SoulDew && (attacker.Mon.Shell.Species == PSpecies.Latios || attacker.Mon.Shell.Species == PSpecies.Latias))
                spAttack *= 1.5;

            return (ushort)spAttack;
        }
        ushort CalculateSpDefense(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double spDefense = attacker.Mon.SpDefense * GetStatMultiplier(defender.Mon.SpDefenseChange);

            // A Clamperl holding a Deep Sea Scale gets a 2x spDefense boost
            if (defender.Mon.Shell.Item == PItem.DeepSeaScale && defender.Mon.Shell.Species == PSpecies.Clamperl)
                spDefense *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spDefense boost
            if (defender.Mon.Shell.Item == PItem.SoulDew && (defender.Mon.Shell.Species == PSpecies.Latios || defender.Mon.Shell.Species == PSpecies.Latias))
                spDefense *= 1.5;

            return (ushort)spDefense;
        }
        ushort CalculateDamage(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            ushort damage;
            ushort a = 0, d = 0,
                p = CalculateBasePower(attacker, defender, move);

            // TODO: Determine a and d for moves like Foul Play and Psyshock

            if (mData.Category == PMoveCategory.Physical)
            {
                a = CalculateAttack(attacker, defender, move);
                d = CalculateDefense(attacker, defender, move);
            }
            else if (mData.Category == PMoveCategory.Special)
            {
                a = CalculateSpAttack(attacker, defender, move);
                d = CalculateSpDefense(attacker, defender, move);
            }

            damage = (ushort)(2 * attacker.Mon.Shell.Level / 5 + 2);
            damage = (ushort)(damage * a * p / d);
            damage /= 50;
            return (ushort)(damage + 2);
        }
    }
}
