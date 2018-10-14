using PokemonBattleEngine.Data;
using System;

namespace PokemonBattleEngine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TeamData team1 = new TeamData { PlayerName = "Sasha" };
            team1.Pokemon[0] = new Pokemon(Species.Azumarill, Constants.MaxLevel);

            TeamData team2 = new TeamData { PlayerName = "Jess" };
            team2.Pokemon[0] = new Pokemon(Species.Darkrai, Constants.MaxLevel);

            Battle battle = new Battle(team1, team2);

            Console.WriteLine(team1.Pokemon[0]);
            Console.WriteLine(team2.Pokemon[0]);

            battle.SelectMove(0, 0);
            battle.SelectMove(1, 0);

            Console.WriteLine(team1.Pokemon[0]);
            Console.WriteLine(team2.Pokemon[0]);
            Console.ReadKey();
        }
    }
}
