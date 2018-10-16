using Kermalis.PokemonBattleEngine.Data;

namespace Kermalis.PokemonBattleEngine
{
    partial class Battle
    {
        ushort CalculateBasePower(BattlePokemon attacker, BattlePokemon defender, PMove move)
        {
            MoveData mData = MoveData.Data[move];
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
            if (attacker.Mon.Item == PItem.LightBall && attacker.Mon.Species == PSpecies.Pikachu)
                basePower *= 2;
            // Retaliate doubles power if the team has a pokemon that fainted the previous turn
            if (move == PMove.Retaliate && attacker.Team.MonFaintedLastTurn)
                basePower *= 2;
            // Overgrow gives a 1.5x boost to Grass attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Grass && attacker.Mon.Ability == PAbility.Overgrow && attacker.Mon.HP <= attacker.Mon.MaxHP / 3)
                basePower *= 1.5;
            // Blaze gives a 1.5x boost to Fire attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Fire && attacker.Mon.Ability == PAbility.Blaze && attacker.Mon.HP <= attacker.Mon.MaxHP / 3)
                basePower *= 1.5;
            // Torrent gives a 1.5x boost to Water attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Water && attacker.Mon.Ability == PAbility.Torrent && attacker.Mon.HP <= attacker.Mon.MaxHP / 3)
                basePower *= 1.5;
            // Swarm gives a 1.5x boost to Bug attacks if the attacker is below 1/3 max HP
            if (mData.Type == PType.Bug && attacker.Mon.Ability == PAbility.Swarm && attacker.Mon.HP <= attacker.Mon.MaxHP / 3)
                basePower *= 1.5;
            // A burned pokemon does half the damage when it is burned unless it has the Guts ability
            if (mData.Category == PMoveCategory.Physical && attacker.Mon.Status == PStatus.Burned && attacker.Mon.Ability != PAbility.Guts)
                basePower /= 2;
            // Damage is halved when using Fire or Ice moves against a pokemon with the Thick Fat ability
            if (defender.Mon.Ability == PAbility.ThickFat && (mData.Type == PType.Fire || mData.Type == PType.Ice))
                basePower /= 2;

            return (ushort)basePower;
        }

        ushort CalculateAttack(BattlePokemon attacker, BattlePokemon defender, PMove move)
        {
            MoveData mData = MoveData.Data[move];
            double attack = attacker.Mon.Attack;

            // Pokemon with the Huge Power or Pure Power ability get a 2x attack boost
            if (attacker.Mon.Ability == PAbility.HugePower || attacker.Mon.Ability == PAbility.PurePower)
                attack *= 2;
            // A Cubone or Marowak holding a Thick Club gets a 2x attack boost
            if (attacker.Mon.Item == PItem.ThickClub && (attacker.Mon.Species == PSpecies.Cubone || attacker.Mon.Species == PSpecies.Marowak))
                attack *= 2;
            // A pokemon with the Hustle ability gets a 1.5x attack boost
            if (attacker.Mon.Ability == PAbility.Hustle)
                attack *= 1.5;
            // A pokemon with the Guts ability gets a 1.5x attack boost when afflicted with a status
            if (attacker.Mon.Ability == PAbility.Guts && attacker.Mon.Status != PStatus.None)
                attack *= 1.5;
            // A pokemon holding a Choice Band gets a 1.5x attack boost
            if (attacker.Mon.Item == PItem.ChoiceBand)
                attack *= 1.5;

            return (ushort)attack;
        }

        ushort CalculateDefense(BattlePokemon attacker, BattlePokemon defender, PMove move)
        {
            MoveData mData = MoveData.Data[move];
            double defense = attacker.Mon.Defense;

            // A Ditto holding a Metal Powder gets a 2x defense boost
            if (defender.Mon.Item == PItem.MetalPowder && defender.Mon.Species == PSpecies.Ditto)
                defense *= 2;
            // A pokemon with the Marvel Scale ability gets a 1.5x defense boost when afflicted with a status
            if (defender.Mon.Ability == PAbility.MarvelScale && defender.Mon.Status != PStatus.None)
                defense *= 1.5;

            return (ushort)defense;
        }

        ushort CalculateSpAttack(BattlePokemon attacker, BattlePokemon defender, PMove move)
        {
            MoveData mData = MoveData.Data[move];
            double spAttack = attacker.Mon.SpAttack;

            // TODO:
            // A pokemon with the Plus ability gets a 1.5x spAttack boost if a teammate has the Minus ability
            /* if (attacker.Mon.Ability == Ability.Plus && PartnerHasAbility(Ability.Minus))
             * spAttack *= 1.5;*/
            // A pokemon with the Minus ability gets a 1.5x spAttack boost if a teammate has the Plus ability
            /* if (attacker.Mon.Ability == Ability.Minus && PartnerHasAbility(Ability.Plus))
             * spAttack *= 1.5;*/

            // A Clamperl holding a Deep Sea Tooth gets a 2x spAttack boost
            if (attacker.Mon.Item == PItem.DeepSeaTooth && attacker.Mon.Species == PSpecies.Clamperl)
                spAttack *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spAttack boost
            if (attacker.Mon.Item == PItem.SoulDew && (attacker.Mon.Species == PSpecies.Latios || attacker.Mon.Species == PSpecies.Latias))
                spAttack *= 1.5;

            return (ushort)spAttack;
        }

        ushort CalculateSpDefense(BattlePokemon attacker, BattlePokemon defender, PMove move)
        {
            MoveData mData = MoveData.Data[move];
            double spDefense = attacker.Mon.SpDefense;

            // A Clamperl holding a Deep Sea Scale gets a 2x spDefense boost
            if (defender.Mon.Item == PItem.DeepSeaScale && defender.Mon.Species == PSpecies.Clamperl)
                spDefense *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spDefense boost
            if (defender.Mon.Item == PItem.SoulDew && (defender.Mon.Species == PSpecies.Latios || defender.Mon.Species == PSpecies.Latias))
                spDefense *= 1.5;

            return (ushort)spDefense;
        }

        ushort CalculateDamage(BattlePokemon attacker, BattlePokemon defender, PMove move)
        {
            MoveData mData = MoveData.Data[move];
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

            damage = (ushort)(2 * attacker.Mon.Level / 5 + 2);
            damage = (ushort)(damage * a * p / d);
            damage /= 50;
            return (ushort)(damage + 2);
        }
    }
}
