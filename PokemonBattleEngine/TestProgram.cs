using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine
{
    public class TestProgram
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Pokémon Battle Engine Test");

            PTeamShell team1 = new PTeamShell
            {
                PlayerName = "Sasha",
                Pokemon =
                {
                    new PPokemonShell
                    {
                        Species = PSpecies.Azumarill,
                        Item = PItem.ChoiceBand,
                        Ability = PAbility.HugePower,
                        Nature = PNature.Adamant,
                        IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                        EVs = new byte[] { 252, 252, 0, 0, 0, 4 },
                        Moves = new PMove[] { PMove.Waterfall, PMove.AquaJet, PMove.Return, PMove.IcePunch },
                    }
                }
            };

            PTeamShell team2 = new PTeamShell
            {
                PlayerName = "Jess",
                Pokemon =
                {
                    new PPokemonShell
                    {
                        Species = PSpecies.Cresselia,
                        Item = PItem.Leftovers,
                        Ability = PAbility.Levitate,
                        Nature = PNature.Bold,
                        IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                        EVs = new byte[] { 252, 0, 252, 0, 0, 4 },
                        Moves = new PMove[] { PMove.Psychic, PMove.Moonlight, PMove.IceBeam, PMove.Toxic },
                    }
                }
            };

            try
            {
                PPokemonShell.ValidateMany(team1.Pokemon);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine($"Invalid {e.ParamName} in Team 1 Pokémon Shell");
            }
            try
            {
                PPokemonShell.ValidateMany(team2.Pokemon);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine($"Invalid {e.ParamName} in Team 2 Pokémon Shell");
            }

            PBattle battle = new PBattle(team1, team2);
            PPokemon p1 = battle.GetBattler(0);
            PPokemon p2 = battle.GetBattler(1);

            Console.WriteLine();
            Console.WriteLine("Battle starting.");
            Console.WriteLine(p1);
            Console.WriteLine(p2);

            Console.WriteLine();
            battle.SelectMove(0, 0, 0, PTarget.FoeLeft);
            battle.SelectMove(1, 0, 0, PTarget.FoeLeft);
            Console.WriteLine($"{p1.Species} used {p1.Moves[0]}");
            Console.WriteLine(p2);
            Console.WriteLine();
            Console.WriteLine($"{p2.Species} used {p2.Moves[0]}");
            Console.WriteLine(p1);
            Console.ReadKey();
        }
    }
}
