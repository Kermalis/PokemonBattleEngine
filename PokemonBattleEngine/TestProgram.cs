using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Util;
using System;

namespace Kermalis.PokemonBattleEngine
{
    class PTestProgram
    {
        static readonly PPokemonShell
            pikachu = new PPokemonShell
            {
                Species = PSpecies.Pikachu,
                Nickname = "Jerry",
                Level = 100,
                Friendship = 255,
                Item = PItem.LightBall,
                Ability = PAbility.LightningRod,
                Gender = PGender.Male,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                EVs = new byte[] { 0, 0, 4, 252, 0, 252 },
                Moves = new PMove[] { PMove.Thunder, PMove.Thunder, PMove.Thunder, PMove.Thunder }, // substitute, thunderbolt, hidden power ice, grass knot
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            azumarill = new PPokemonShell
            {
                Species = PSpecies.Azumarill,
                Nickname = "ZuWEEE",
                Level = 100,
                Friendship = 255,
                Item = PItem.ChoiceBand,
                Ability = PAbility.HugePower,
                Gender = PGender.Male,
                Nature = PNature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                EVs = new byte[] { 252, 252, 0, 0, 0, 4 },
                Moves = new PMove[] { PMove.Waterfall, PMove.AquaJet, PMove.Return, PMove.IcePunch },
                PPUps = new byte[] { 3, 3, 3, 3 }
            },
            cresselia = new PPokemonShell
            {
                Species = PSpecies.Cresselia,
                Nickname = "Crest",
                Level = 100,
                Friendship = 255,
                Item = PItem.Leftovers,
                Ability = PAbility.Levitate,
                Gender = PGender.Female,
                Nature = PNature.Bold,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                EVs = new byte[] { 252, 0, 252, 0, 0, 4 },
                Moves = new PMove[] { PMove.Psychic, PMove.Moonlight, PMove.IceBeam, PMove.Toxic },
                PPUps = new byte[] { 3, 3, 3, 3 }
            };

        public static void Main(string[] args)
        {
            Console.WriteLine("Pokémon Battle Engine Test");
            Console.WriteLine();

            PTeamShell team1 = new PTeamShell
            {
                DisplayName = "Sasha",
                Party = { azumarill }
            };
            PTeamShell team2 = new PTeamShell
            {
                DisplayName = "Jess",
                Party = { cresselia }
            };

            PBattle battle = new PBattle(team1, team2);
            battle.OnNewEvent += PBattle.ConsoleBattleEventHandler;
            battle.Start();
            PPokemon p1 = PKnownInfo.Instance.LocalParty[0];
            PPokemon p2 = PKnownInfo.Instance.RemoteParty[0];

            Console.WriteLine();
            Console.WriteLine(p1);
            Console.WriteLine(p2);

            while (p1.HP > 0 && p2.HP > 0)
            {
                Console.WriteLine();

                // Temporary
                do
                {
                    byte move;
                    bool valid;

                    move = (byte)PUtils.RNG.Next(0, PConstants.NumMoves);
                    valid = battle.SelectActionIfValid(p1.Id, move);
                    Console.WriteLine("{0} ({1}) valid: {2}", move, p1.Shell.Moves[move], valid);

                    move = (byte)PUtils.RNG.Next(0, PConstants.NumMoves);
                    valid = battle.SelectActionIfValid(p2.Id, move);
                    Console.WriteLine("{0} ({1}) valid: {2}", move, p2.Shell.Moves[move], valid);

                    Console.WriteLine();
                } while (!battle.IsReadyToRunTurn());
                battle.RunTurn();

                Console.WriteLine();
                Console.WriteLine(p1);
                Console.WriteLine(p2);
            }
            Console.ReadKey();
        }
    }
}
