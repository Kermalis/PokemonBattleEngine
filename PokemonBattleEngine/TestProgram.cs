using Kermalis.PokemonBattleEngine.AI;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.IO;

namespace Kermalis.PokemonBattleEngine
{
    class PBETestProgram
    {
        static readonly string logFile = "Test Log.txt";
        static StreamWriter writer;
        static TextWriter oldWriter;

        public static void Main(string[] args)
        {
            Console.WriteLine("----- Pokémon Battle Engine Test -----");

            PBESettings settings = PBESettings.DefaultSettings;
            PBEBattle battle = new PBEBattle(PBEBattleFormat.Triple, settings, PBECompetitivePokemonShells.CreateRandomTeam(settings.MaxPartySize), PBECompetitivePokemonShells.CreateRandomTeam(settings.MaxPartySize));
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
                        Console.SetOut(oldWriter);
                        writer.Close();
                        Console.WriteLine($"Test battle ended. \"{logFile}\" contains the battle.");
                        Console.ReadKey();
                        break;
                    case PBEBattleState.ReadyToRunTurn:
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
                    case PBEBattleState.WaitingForActions:
                        foreach (PBETeam team in battle.Teams)
                        {
                            PBEBattle.SelectActionsIfValid(team, AIManager.CreateActions(team));
                        }
                        break;
                    case PBEBattleState.WaitingForSwitchIns:
                        foreach (PBETeam team in battle.Teams)
                        {
                            if (team.SwitchInsRequired > 0)
                            {
                                PBEBattle.SelectSwitchesIfValid(team, AIManager.CreateSwitches(team));
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.SetOut(oldWriter);
                writer.Close();
                Console.WriteLine($"Test battle threw an exception, check \"{logFile}\" for details.");
                Console.ReadKey();
                Environment.Exit(-1);
            }
        }
    }
}
