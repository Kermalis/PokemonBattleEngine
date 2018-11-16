using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
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

            PPokemon lCenter = PKnownInfo.Instance.PokemonAtPosition(true, PFieldPosition.Center);
            PPokemon rCenter = PKnownInfo.Instance.PokemonAtPosition(false, PFieldPosition.Center);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(lCenter);
            Console.WriteLine();
            Console.WriteLine(rCenter);
            Console.WriteLine();

            while (battle.TemporaryKeepBattlingBool)
            {
                Console.WriteLine();
                Console.WriteLine();

                // Temporary
                do
                {
                    byte move;
                    PAction action = new PAction();
                    bool valid;

                    move = (byte)PUtils.RNG.Next(0, PSettings.NumMoves);
                    action.PokemonId = lCenter.Id;
                    action.Decision = PDecision.Fight;
                    action.FightMove = lCenter.Moves[move];
                    action.FightTargets = PTarget.FoeCenter;
                    valid = battle.SelectActionIfValid(action);

                    move = (byte)PUtils.RNG.Next(0, PSettings.NumMoves);
                    action.PokemonId = rCenter.Id;
                    action.Decision = PDecision.Fight;
                    action.FightMove = rCenter.Moves[move];
                    action.FightTargets = PTarget.FoeCenter;
                    valid = battle.SelectActionIfValid(action);

                } while (!battle.IsReadyToRunTurn());
                try
                {
                    battle.RunTurn();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }

                lCenter = PKnownInfo.Instance.PokemonAtPosition(true, PFieldPosition.Center);
                rCenter = PKnownInfo.Instance.PokemonAtPosition(false, PFieldPosition.Center);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(lCenter);
                Console.WriteLine();
                Console.WriteLine(rCenter);
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
