using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine
{
    class TestProgram
    {
        static readonly PPokemonShell
            pikachu = new PPokemonShell
            {
                Species = PSpecies.Pikachu,
                Item = PItem.LightBall,
                Ability = PAbility.LightningRod,
                Gender = PGender.Male,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                EVs = new byte[] { 0, 0, 4, 252, 0, 252 },
                Moves = new PMove[] { PMove.Thunder, PMove.Thunder, PMove.Thunder, PMove.Thunder } // substitute, thunderbolt, hidden power ice, grass knot
            },
            azumarill = new PPokemonShell
            {
                Species = PSpecies.Azumarill,
                Item = PItem.ChoiceBand,
                Ability = PAbility.HugePower,
                Gender = PGender.Male,
                Nature = PNature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                EVs = new byte[] { 252, 252, 0, 0, 0, 4 },
                Moves = new PMove[] { PMove.Waterfall, PMove.AquaJet, PMove.Return, PMove.IcePunch }
            },
            cresselia = new PPokemonShell
            {
                Species = PSpecies.Cresselia,
                Item = PItem.Leftovers,
                Ability = PAbility.Levitate,
                Gender = PGender.Female,
                Nature = PNature.Bold,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                EVs = new byte[] { 252, 0, 252, 0, 0, 4 },
                Moves = new PMove[] { PMove.IceBeam, PMove.Moonlight, PMove.Psychic, PMove.Toxic }
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

            try
            {
                PPokemonShell.ValidateMany(team1.Party);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine($"Invalid {e.ParamName} in Team 0 Pokémon Shell");
            }
            try
            {
                PPokemonShell.ValidateMany(team2.Party);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine($"Invalid {e.ParamName} in Team 1 Pokémon Shell");
            }

            PBattle battle = new PBattle(team1, team2);
            battle.NewEvent += PBattle.ConsoleBattleEventHandler;
            battle.Start();
            PPokemon p1 = PKnownInfo.Instance.LocalParty[0];
            PPokemon p2 = PKnownInfo.Instance.RemoteParty[0];

            Console.WriteLine();
            Console.WriteLine(p1);
            Console.WriteLine(p2);

            while (p1.HP > 0 && p2.HP > 0)
            {
                Console.WriteLine();
                battle.SelectMove(0, 0, 0, PTarget.FoeLeft);
                battle.SelectMove(1, 0, 0, PTarget.FoeLeft);

                Console.WriteLine();
                Console.WriteLine(p1);
                Console.WriteLine(p2);
            }
            Console.ReadKey();
        }
    }
}
