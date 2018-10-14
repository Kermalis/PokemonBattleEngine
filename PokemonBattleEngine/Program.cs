using PokemonBattleEngine.Data;
using System;

namespace PokemonBattleEngine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TeamData team1 = new TeamData
            {
                Pokemon = new Pokemon[] { new Pokemon(Species.Azumarill, Constants.MaxLevel) },
                PlayerName = "Sasha"
            };

            TeamData team2 = new TeamData
            {
                Pokemon = new Pokemon[] { new Pokemon(Species.Cresselia, Constants.MaxLevel) },
                PlayerName = "Jess"
            };

            Battle battle = new Battle(team1, team2);

            Console.WriteLine("Battle starting.");
            Console.WriteLine(team1.Pokemon[0]);
            Console.WriteLine(team2.Pokemon[0]);

            Console.WriteLine();
            battle.SelectMove(0, 0, 0, Target.FoeLeft);
            battle.SelectMove(1, 0, 0, Target.FoeLeft);
            Console.WriteLine($"{team1.Pokemon[0].Species} used {team1.Pokemon[0].Moves[0]}");
            Console.WriteLine(team2.Pokemon[0]);
            Console.WriteLine();
            Console.WriteLine($"{team2.Pokemon[0].Species} used {team2.Pokemon[0].Moves[0]}");
            Console.WriteLine(team1.Pokemon[0]);
            Console.ReadKey();
        }
    }
}
