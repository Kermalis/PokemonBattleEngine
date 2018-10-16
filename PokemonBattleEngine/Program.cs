using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Pokémon Battle Engine Test");

            TeamShell team1 = new TeamShell
            {
                Pokemon =
                {
                    new PokemonShell
                    {
                        Species = Species.Azumarill,
                        Item = Item.ChoiceBand,
                        Ability = Ability.HugePower,
                        Nature = Nature.Adamant,
                        IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                        EVs = new byte[] { 252, 252, 0, 0, 0, 4 },
                        Moves = new Move[] { Move.Waterfall, Move.AquaJet, Move.Return, Move.IcePunch },
                    }
                },
                PlayerName = "Sasha"
            };

            TeamShell team2 = new TeamShell
            {
                Pokemon =
                {
                    new PokemonShell
                    {
                        Species = Species.Cresselia,
                        Item = Item.Leftovers,
                        Ability = Ability.Levitate,
                        Nature = Nature.Bold,
                        IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                        EVs = new byte[] { 252, 0, 252, 0, 0, 4 },
                        Moves = new Move[] { Move.Psychic, Move.Moonlight, Move.IceBeam, Move.Toxic },
                    }
                },
                PlayerName = "Jess"
            };

            try
            {
                PokemonShell.ValidateMany(team1.Pokemon);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine($"Invalid {e.ParamName} in Team 1 Pokémon Shell");
            }
            try
            {
                PokemonShell.ValidateMany(team2.Pokemon);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine($"Invalid {e.ParamName} in Team 2 Pokémon Shell");
            }

            Battle battle = new Battle(team1, team2);
            Pokemon p1 = battle.GetBattler(0);
            Pokemon p2 = battle.GetBattler(1);

            Console.WriteLine();
            Console.WriteLine("Battle starting.");
            Console.WriteLine(p1);
            Console.WriteLine(p2);

            Console.WriteLine();
            battle.SelectMove(0, 0, 0, Target.FoeLeft);
            battle.SelectMove(1, 0, 0, Target.FoeLeft);
            Console.WriteLine($"{p1.Species} used {p1.Moves[0]}");
            Console.WriteLine(p2);
            Console.WriteLine();
            Console.WriteLine($"{p2.Species} used {p2.Moves[0]}");
            Console.WriteLine(p1);
            Console.ReadKey();
        }
    }
}
