using Kermalis.PokemonBattleEngine.AI;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.IO;
using System.Linq;

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
            switch (battle.BattleState)
            {
                case PBEBattleState.Ended:
                    Console.SetOut(oldWriter);
                    writer.Close();
                    Console.WriteLine($"Test battle ended. \"{logFile}\" contains the battle.");
                    Console.ReadKey();
                    break;
                case PBEBattleState.ReadyToRunTurn:
                    Console.WriteLine();
                    foreach (PBEPokemon pkmn in battle.Teams[0].ActiveBattlers.Concat(battle.Teams[1].ActiveBattlers))
                    {
                        Console.WriteLine(pkmn);
                        Console.WriteLine();
                    }
                    battle.RunTurn();
                    break;
                case PBEBattleState.WaitingForActions:
                    battle.SelectActionsIfValid(true, AIManager.CreateActions(battle, true));
                    battle.SelectActionsIfValid(false, AIManager.CreateActions(battle, false));
                    break;
                case PBEBattleState.WaitingForSwitchIns:
                    if (battle.Teams[0].SwitchInsRequired > 0)
                    {
                        battle.SelectSwitchesIfValid(true, AIManager.CreateSwitches(battle, true));
                    }
                    if (battle.Teams[1].SwitchInsRequired > 0)
                    {
                        battle.SelectSwitchesIfValid(false, AIManager.CreateSwitches(battle, false));
                    }
                    break;
            }
        }
    }
}
