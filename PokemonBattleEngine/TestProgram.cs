using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine
{
    class PBETestProgram
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Pokémon Battle Engine Test");
            Console.WriteLine();
            Console.WriteLine("The test is not updated at this time.");

            /*PBETeamShell team0 = new PBETeamShell
            {
                PlayerName = "Sasha",
                Party = { PBECompetitivePokemonShells.Ditto_Uber, PBECompetitivePokemonShells.Azumarill_VGC }
            };
            PBETeamShell team1 = new PBETeamShell
            {
                PlayerName = "Jess",
                Party = { PBECompetitivePokemonShells.Darkrai_Uber, PBECompetitivePokemonShells.Latios_VGC }
            };

            PBEBattle battle = new PBEBattle(PBEBattleStyle.Single, team0, team1);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.Begin();

            PBEPokemon lCenter = battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Center);
            PBEPokemon rCenter = battle.Teams[1].PokemonAtPosition(PBEFieldPosition.Center);
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
                    PBEAction[] actions = new PBEAction[1];
                    bool valid;

                    move = (byte)PBEUtils.RNG.Next(0, PBESettings.NumMoves);
                    actions[0].PokemonId = lCenter.Id;
                    actions[0].Decision = PBEDecision.Fight;
                    actions[0].FightMove = lCenter.Moves[move];
                    actions[0].FightTargets = PBETarget.FoeCenter;
                    valid = battle.SelectActionsIfValid(actions);

                    move = (byte)PBEUtils.RNG.Next(0, PBESettings.NumMoves);
                    actions[0].PokemonId = rCenter.Id;
                    actions[0].Decision = PBEDecision.Fight;
                    actions[0].FightMove = rCenter.Moves[move];
                    actions[0].FightTargets = PBETarget.FoeCenter;
                    valid = battle.SelectActionsIfValid(actions);

                } while (!battle.IsReadyToRunTurn());
                try
                {
                    battle.RunTurn();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }

                lCenter = battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Center);
                rCenter = battle.Teams[1].PokemonAtPosition(PBEFieldPosition.Center);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(lCenter);
                Console.WriteLine();
                Console.WriteLine(rCenter);
                Console.WriteLine();
            }*/
            Console.ReadKey();
        }
    }
}
