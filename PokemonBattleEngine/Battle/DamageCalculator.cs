using Kermalis.PokemonBattleEngine.Data;

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
            if (attacker.PokeMon.Item == PItem.LightBall && attacker.PokeMon.Species == PSpecies.Pikachu)
                basePower *= 2;
            // Retaliate doubles power if the team has a pokemon that fainted the previous turn
            if (move == PMove.Retaliate && attacker.Team.MonFaintedLastTurn)
                basePower *= 2;
            // Overgrow gives a 1.5x boost to Grass attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Grass && attacker.PokeMon.Ability == PAbility.Overgrow && attacker.PokeMon.HP <= attacker.PokeMon.MaxHP / 3)
                basePower *= 1.5;
            // Blaze gives a 1.5x boost to Fire attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Fire && attacker.PokeMon.Ability == PAbility.Blaze && attacker.PokeMon.HP <= attacker.PokeMon.MaxHP / 3)
                basePower *= 1.5;
            // Torrent gives a 1.5x boost to Water attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Water && attacker.PokeMon.Ability == PAbility.Torrent && attacker.PokeMon.HP <= attacker.PokeMon.MaxHP / 3)
                basePower *= 1.5;
            // Swarm gives a 1.5x boost to Bug attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Bug && attacker.PokeMon.Ability == PAbility.Swarm && attacker.PokeMon.HP <= attacker.PokeMon.MaxHP / 3)
                basePower *= 1.5;
            // A burned pokemon does half the damage when it is burned unless it has the Guts ability
            if (mData.Category == PMoveCategory.Physical && attacker.PokeMon.Status == PStatus.Burned && attacker.PokeMon.Ability != PAbility.Guts)
                basePower /= 2;
            // Damage is halved when using Fire or Ice moves against a pokemon with the Thick Fat ability
            if (defender.PokeMon.Ability == PAbility.ThickFat && (mData.Type == PType.Fire || mData.Type == PType.Ice))
                basePower /= 2;

            return (ushort)basePower;
        }

        ushort CalculateAttack(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double attack = attacker.PokeMon.Attack;

            // Pokemon with the Huge Power or Pure Power ability get a 2x attack boost
            if (attacker.PokeMon.Ability == PAbility.HugePower || attacker.PokeMon.Ability == PAbility.PurePower)
                attack *= 2;
            // A Cubone or Marowak holding a Thick Club gets a 2x attack boost
            if (attacker.PokeMon.Item == PItem.ThickClub && (attacker.PokeMon.Species == PSpecies.Cubone || attacker.PokeMon.Species == PSpecies.Marowak))
                attack *= 2;
            // A pokemon with the Hustle ability gets a 1.5x attack boost
            if (attacker.PokeMon.Ability == PAbility.Hustle)
                attack *= 1.5;
            // A pokemon with the Guts ability gets a 1.5x attack boost when afflicted with a status
            if (attacker.PokeMon.Ability == PAbility.Guts && attacker.PokeMon.Status != PStatus.None)
                attack *= 1.5;
            // A pokemon holding a Choice Band gets a 1.5x attack boost
            if (attacker.PokeMon.Item == PItem.ChoiceBand)
                attack *= 1.5;

            return (ushort)attack;
        }

        ushort CalculateDefense(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double defense = attacker.PokeMon.Defense;

            // A Ditto holding a Metal Powder gets a 2x defense boost
            if (defender.PokeMon.Item == PItem.MetalPowder && defender.PokeMon.Species == PSpecies.Ditto)
                defense *= 2;
            // A pokemon with the Marvel Scale ability gets a 1.5x defense boost when afflicted with a status
            if (defender.PokeMon.Ability == PAbility.MarvelScale && defender.PokeMon.Status != PStatus.None)
                defense *= 1.5;

            return (ushort)defense;
        }

        ushort CalculateSpAttack(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double spAttack = attacker.PokeMon.SpAttack;

            // TODO:
            // A pokemon with the Plus ability gets a 1.5x spAttack boost if a teammate has the Minus ability
            /* if (attacker.Mon.Ability == Ability.Plus && PartnerHasAbility(Ability.Minus))
             * spAttack *= 1.5;*/
            // A pokemon with the Minus ability gets a 1.5x spAttack boost if a teammate has the Plus ability
            /* if (attacker.Mon.Ability == Ability.Minus && PartnerHasAbility(Ability.Plus))
             * spAttack *= 1.5;*/

            // A Clamperl holding a Deep Sea Tooth gets a 2x spAttack boost
            if (attacker.PokeMon.Item == PItem.DeepSeaTooth && attacker.PokeMon.Species == PSpecies.Clamperl)
                spAttack *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spAttack boost
            if (attacker.PokeMon.Item == PItem.SoulDew && (attacker.PokeMon.Species == PSpecies.Latios || attacker.PokeMon.Species == PSpecies.Latias))
                spAttack *= 1.5;

            return (ushort)spAttack;
        }

        ushort CalculateSpDefense(PBattlePokemon attacker, PBattlePokemon defender, PMove move)
        {
            PMoveData mData = PMoveData.Data[move];
            double spDefense = attacker.PokeMon.SpDefense;

            // A Clamperl holding a Deep Sea Scale gets a 2x spDefense boost
            if (defender.PokeMon.Item == PItem.DeepSeaScale && defender.PokeMon.Species == PSpecies.Clamperl)
                spDefense *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spDefense boost
            if (defender.PokeMon.Item == PItem.SoulDew && (defender.PokeMon.Species == PSpecies.Latios || defender.PokeMon.Species == PSpecies.Latias))
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

            damage = (ushort)(2 * attacker.PokeMon.Level / 5 + 2);
            damage = (ushort)(damage * a * p / d);
            damage /= 50;
            return (ushort)(damage + 2);
        }
    }
}
