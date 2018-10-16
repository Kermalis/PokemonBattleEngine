﻿using Kermalis.PokemonBattleEngine.Data;

namespace Kermalis.PokemonBattleEngine.Battle
{
    partial class PBattle
    {
        ushort CalculateBasePower(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double basePower = mData.Power;


            // TODO: Stuff like mystic water
            // STAB
            // Return

            // Damage is halved when using electric moves while mud sport is active
            /* if (type == Type.Electric && MudSportActive())
             * basePower /= 2;*/
            // Damage is halved when using fire moves while water sport is active
            /* if (type == Type.Fire && WaterSportActive())
             * basePower /= 2;*/

            // A Pikachu holding a Light Ball gets a 2x power boost
            if (attacker.Pokemon.Shell.Item == PItem.LightBall && attacker.Pokemon.Shell.Species == PSpecies.Pikachu)
                basePower *= 2;
            // Retaliate doubles power if the team has a pokemon that fainted the previous turn
            if (move == PMove.Retaliate && attacker.Team.MonFaintedLastTurn)
                basePower *= 2;
            // Overgrow gives a 1.5x boost to Grass attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Grass && attacker.Pokemon.Shell.Ability == PAbility.Overgrow && attacker.Pokemon.HP <= attacker.Pokemon.MaxHP / 3)
                basePower *= 1.5;
            // Blaze gives a 1.5x boost to Fire attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Fire && attacker.Pokemon.Shell.Ability == PAbility.Blaze && attacker.Pokemon.HP <= attacker.Pokemon.MaxHP / 3)
                basePower *= 1.5;
            // Torrent gives a 1.5x boost to Water attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Water && attacker.Pokemon.Shell.Ability == PAbility.Torrent && attacker.Pokemon.HP <= attacker.Pokemon.MaxHP / 3)
                basePower *= 1.5;
            // Swarm gives a 1.5x boost to Bug attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Bug && attacker.Pokemon.Shell.Ability == PAbility.Swarm && attacker.Pokemon.HP <= attacker.Pokemon.MaxHP / 3)
                basePower *= 1.5;
            // A burned pokemon does half the damage when it is burned unless it has the Guts ability
            if (mData.Category == PMoveCategory.Physical && attacker.Pokemon.Status == PStatus.Burned && attacker.Pokemon.Shell.Ability != PAbility.Guts)
                basePower /= 2;
            // Damage is halved when using Fire or Ice moves against a pokemon with the Thick Fat ability
            if (defender.Pokemon.Shell.Ability == PAbility.ThickFat && (mData.Type == PType.Fire || mData.Type == PType.Ice))
                basePower /= 2;

            return (ushort)basePower;
        }

        ushort CalculateAttack(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double attack = attacker.Pokemon.Attack;

            // Pokemon with the Huge Power or Pure Power ability get a 2x attack boost
            if (attacker.Pokemon.Shell.Ability == PAbility.HugePower || attacker.Pokemon.Shell.Ability == PAbility.PurePower)
                attack *= 2;
            // A Cubone or Marowak holding a Thick Club gets a 2x attack boost
            if (attacker.Pokemon.Shell.Item == PItem.ThickClub && (attacker.Pokemon.Shell.Species == PSpecies.Cubone || attacker.Pokemon.Shell.Species == PSpecies.Marowak))
                attack *= 2;
            // A pokemon with the Hustle ability gets a 1.5x attack boost
            if (attacker.Pokemon.Shell.Ability == PAbility.Hustle)
                attack *= 1.5;
            // A pokemon with the Guts ability gets a 1.5x attack boost when afflicted with a status
            if (attacker.Pokemon.Shell.Ability == PAbility.Guts && attacker.Pokemon.Status != PStatus.None)
                attack *= 1.5;
            // A pokemon holding a Choice Band gets a 1.5x attack boost
            if (attacker.Pokemon.Shell.Item == PItem.ChoiceBand)
                attack *= 1.5;

            return (ushort)attack;
        }

        ushort CalculateDefense(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double defense = attacker.Pokemon.Defense;

            // A Ditto holding a Metal Powder gets a 2x defense boost
            if (defender.Pokemon.Shell.Item == PItem.MetalPowder && defender.Pokemon.Shell.Species == PSpecies.Ditto)
                defense *= 2;
            // A pokemon with the Marvel Scale ability gets a 1.5x defense boost when afflicted with a status
            if (defender.Pokemon.Shell.Ability == PAbility.MarvelScale && defender.Pokemon.Status != PStatus.None)
                defense *= 1.5;

            return (ushort)defense;
        }

        ushort CalculateSpAttack(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double spAttack = attacker.Pokemon.SpAttack;

            // TODO:
            // A pokemon with the Plus ability gets a 1.5x spAttack boost if a teammate has the Minus ability
            /* if (attacker.Mon.Ability == Ability.Plus && PartnerHasAbility(Ability.Minus))
             * spAttack *= 1.5;*/
            // A pokemon with the Minus ability gets a 1.5x spAttack boost if a teammate has the Plus ability
            /* if (attacker.Mon.Ability == Ability.Minus && PartnerHasAbility(Ability.Plus))
             * spAttack *= 1.5;*/

            // A Clamperl holding a Deep Sea Tooth gets a 2x spAttack boost
            if (attacker.Pokemon.Shell.Item == PItem.DeepSeaTooth && attacker.Pokemon.Shell.Species == PSpecies.Clamperl)
                spAttack *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spAttack boost
            if (attacker.Pokemon.Shell.Item == PItem.SoulDew && (attacker.Pokemon.Shell.Species == PSpecies.Latios || attacker.Pokemon.Shell.Species == PSpecies.Latias))
                spAttack *= 1.5;

            return (ushort)spAttack;
        }

        ushort CalculateSpDefense(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double spDefense = attacker.Pokemon.SpDefense;

            // A Clamperl holding a Deep Sea Scale gets a 2x spDefense boost
            if (defender.Pokemon.Shell.Item == PItem.DeepSeaScale && defender.Pokemon.Shell.Species == PSpecies.Clamperl)
                spDefense *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spDefense boost
            if (defender.Pokemon.Shell.Item == PItem.SoulDew && (defender.Pokemon.Shell.Species == PSpecies.Latios || defender.Pokemon.Shell.Species == PSpecies.Latias))
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

            damage = (ushort)(2 * attacker.Pokemon.Shell.Level / 5 + 2);
            damage = (ushort)(damage * a * p / d);
            damage /= 50;
            return (ushort)(damage + 2);
        }
    }
}