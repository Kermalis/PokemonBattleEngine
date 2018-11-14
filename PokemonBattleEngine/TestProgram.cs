using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Util;
using System;

namespace Kermalis.PokemonBattleEngine
{
    class PTestProgram
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Pokémon Battle Engine Test");
            Console.WriteLine();

            PTeamShell team0 = new PTeamShell
            {
                DisplayName = "Sasha",
                Party = { PCompetitivePokemonShells.Ditto_UU, PCompetitivePokemonShells.Azumarill_UU }
            };
            PTeamShell team1 = new PTeamShell
            {
                DisplayName = "Jess",
                Party = { PCompetitivePokemonShells.Darkrai_Uber, PCompetitivePokemonShells.Latios_OU }
            };

            PBattle battle = new PBattle(PBattleStyle.Single, team0, team1);
            battle.OnNewEvent += PBattle.ConsoleBattleEventHandler;
            battle.Start();
            PPokemon p0_0 = PKnownInfo.Instance.LocalParty[0];
            //PPokemon p0_1 = PKnownInfo.Instance.LocalParty[1];
            PPokemon p1_0 = PKnownInfo.Instance.RemoteParty[0];
            //PPokemon p1_1 = PKnownInfo.Instance.RemoteParty[1];

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(p0_0);
            Console.WriteLine();
            //Console.WriteLine(p0_1);
            Console.WriteLine(p1_0);
            //Console.WriteLine(p1_1);

            while ((p0_0.HP > 0/* || p0_1.HP > 0*/) && (p1_0.HP > 0/* || p1_1.HP > 0*/))
            {
                Console.WriteLine();
                Console.WriteLine();

                // Temporary
                do
                {
                    byte move;
                    PAction action;
                    bool valid;

                    move = (byte)PUtils.RNG.Next(0, PSettings.NumMoves);
                    action.PokemonId = p0_0.Id;
                    action.Decision = PDecision.Fight;
                    action.Move = p0_0.Moves[move];
                    action.Targets = PTarget.FoeCenter;
                    valid = battle.SelectActionIfValid(action);

                    /*move = (byte)PUtils.RNG.Next(0, PConstants.NumMoves);
                    action = new PAction
                    {
                        PokemonId = p0_1.Id,
                        Move = p0_1.Shell.Moves[move],
                        Targets = PTarget.FoeLeft
                    };
                    valid = battle.SelectActionIfValid(action);*/

                    move = (byte)PUtils.RNG.Next(0, PSettings.NumMoves);
                    action.PokemonId = p1_0.Id;
                    action.Decision = PDecision.Fight;
                    action.Move = p1_0.Moves[move];
                    action.Targets = PTarget.FoeCenter;
                    valid = battle.SelectActionIfValid(action);

                    /*move = (byte)PUtils.RNG.Next(0, PConstants.NumMoves);
                    action = new PAction
                    {
                        PokemonId = p1_1.Id,
                        Move = p1_1.Shell.Moves[move],
                        Targets = PTarget.FoeLeft
                    };
                    valid = battle.SelectActionIfValid(action);*/
                } while (!battle.IsReadyToRunTurn());
                battle.RunTurn();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(p0_0);
                Console.WriteLine();
                //Console.WriteLine(p0_1);
                Console.WriteLine(p1_0);
                //Console.WriteLine(p1_1);
            }
            Console.ReadKey();
        }
    }
}
