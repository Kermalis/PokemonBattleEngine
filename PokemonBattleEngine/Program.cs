using PokemonBattleEngine.Data;
using System;

namespace PokemonBattleEngine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Team team1 = new Team
            {
                Pokemon = new Pokemon[] { new Pokemon(Species.Cresselia) },
                PlayerName = "Sasha"
            };
            Team team2 = new Team
            {
                Pokemon = new Pokemon[] { new Pokemon(Species.Darkrai) },
                PlayerName = "Jess"
            };

            Console.WriteLine(team1.Pokemon[0]);
            Console.WriteLine(team2.Pokemon[0]);
            Console.ReadKey();
        }
    }
}
