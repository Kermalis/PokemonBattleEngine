using Kermalis.PokemonBattleEngine.AI;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine
{
    class PBETestProgram
    {
        static readonly string logFile = "Test Log.txt",
            replayFile = "Test Replay.pbereplay";
        static StreamWriter writer;
        static TextWriter oldWriter;

        public static void Main(string[] args)
        {
            Console.WriteLine("----- Pokémon Battle Engine Test -----");

            PBESettings settings = PBESettings.DefaultSettings;
            PBEPokemonShell[] team0Party = PBECompetitivePokemonShells.CreateRandomTeam(settings.MaxPartySize).ToArray();
            PBEPokemonShell[] team1Party = PBECompetitivePokemonShells.CreateRandomTeam(settings.MaxPartySize).ToArray();
            var battle = new PBEBattle(PBEBattleFormat.Double, settings, team0Party, team1Party);
            battle.Teams[0].TrainerName = "Team 1";
            battle.Teams[1].TrainerName = "Team 2";
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.OnStateChanged += Battle_OnStateChanged;
            try
            {
                writer = new StreamWriter(new FileStream(logFile, FileMode.Create, FileAccess.Write));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot open \"{logFile}\" for writing.");
                Console.WriteLine(e.Message);
                return;
            }
            oldWriter = Console.Out;
            Console.SetOut(writer);
            battle.Begin();
        }
        static void Battle_OnStateChanged(PBEBattle battle)
        {
            try
            {
                switch (battle.BattleState)
                {
                    case PBEBattleState.Ended:
                        {
                            Console.SetOut(oldWriter);
                            writer.Close();
                            try
                            {
                                battle.SaveReplay(replayFile);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error saving replay:");
                                Console.WriteLine(e.Message);
                                Console.WriteLine(e.StackTrace);
                            }
                            Console.WriteLine("Test battle ended. The battle was saved to \"{0}\" and \"{1}\".", logFile, replayFile);
                            Console.ReadKey();
                            break;
                        }
                    case PBEBattleState.ReadyToRunTurn:
                        {
                            foreach (PBETeam team in battle.Teams)
                            {
                                Console.WriteLine();
                                Console.WriteLine("{0}'s team:", team.TrainerName);
                                foreach (PBEPokemon pkmn in team.ActiveBattlers)
                                {
                                    Console.WriteLine(pkmn);
                                    Console.WriteLine();
                                }
                            }
                            battle.RunTurn();
                            break;
                        }
                    case PBEBattleState.WaitingForActions:
                        {
                            foreach (PBETeam team in battle.Teams)
                            {
                                IEnumerable<PBEAction> actions = AIManager.CreateActions(team);
                                bool valid = PBEBattle.AreActionsValid(team, actions);
                                if (!valid)
                                {
                                    throw new Exception($"{team.TrainerName}'s AI created invalid actions!");
                                }
                                PBEBattle.SelectActionsIfValid(team, actions);
                            }
                            break;
                        }
                    case PBEBattleState.WaitingForSwitchIns:
                        {
                            foreach (PBETeam team in battle.Teams.Where(t => t.SwitchInsRequired > 0))
                            {
                                IEnumerable<Tuple<byte, PBEFieldPosition>> switches = AIManager.CreateSwitches(team);
                                bool valid = PBEBattle.AreSwitchesValid(team, switches);
                                if (!valid)
                                {
                                    throw new Exception($"{team.TrainerName}'s AI created invalid switches!");
                                }
                                PBEBattle.SelectSwitchesIfValid(team, switches);
                            }
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.SetOut(oldWriter);
                writer.Close();
                Console.WriteLine("Test battle threw an exception, check \"{0}\" for details.", logFile);
                Console.ReadKey();
                Environment.Exit(-1);
            }
        }
    }
}
