using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.AI;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Kermalis.PokemonBattleEngineExtras
{
    internal class AIBattle
    {
        private const string LogFile = "Test Log.txt";
        private const string ReplayFile = "Test Replay.pbereplay";
        private static StreamWriter writer;
        private static TextWriter oldWriter;

        public static void Test()
        {
            Console.WriteLine("----- Pokémon Battle Engine Test -----");

            var settings = new PBESettings { NumMoves = 8 };
            PBETeamShell team0Shell, team1Shell;

            // Completely Randomized Pokémon
            team0Shell = new PBETeamShell(settings, settings.MaxPartySize, true);
            team1Shell = new PBETeamShell(settings, settings.MaxPartySize, true);

            // Predefined Pokémon
            /*team0Shell = new PBEPokemonShell[]
            {
                PBECompetitivePokemonShells.Zoroark_VGC,
                PBECompetitivePokemonShells.Volcarona_VGC,
                PBECompetitivePokemonShells.Vaporeon_VGC,
                PBECompetitivePokemonShells.Thundurus_VGC,
                PBECompetitivePokemonShells.Vanilluxe_VGC,
                PBECompetitivePokemonShells.Chandelure_VGC
            };
            team1Shell = new PBEPokemonShell[]
            {
                PBECompetitivePokemonShells.Arceus_Uber,
                PBECompetitivePokemonShells.Darkrai_Uber,
                PBECompetitivePokemonShells.Kyurem_White_Uber,
                PBECompetitivePokemonShells.Latias_VGC,
                PBECompetitivePokemonShells.Metagross_VGC,
                PBECompetitivePokemonShells.Victini_Uber
            };*/

            var battle = new PBEBattle(PBEBattleFormat.Double, team0Shell, "Team 1", team1Shell, "Team 2");
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.OnNewEvent += Battle_OnNewEvent;
            battle.OnStateChanged += Battle_OnStateChanged;
            try
            {
                writer = new StreamWriter(new FileStream(LogFile, FileMode.Create, FileAccess.Write));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot open \"{LogFile}\" for writing.");
                Console.WriteLine(e.Message);
                return;
            }
            oldWriter = Console.Out;
            Console.SetOut(writer);
            new Thread(() =>
            {
                try
                {
                    battle.Begin();
                }
                catch (Exception e)
                {
                    CatchException(e);
                }
            })
            { Name = "Battle Thread" }.Start();
        }

        private static void CatchException(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            Console.SetOut(oldWriter);
            writer.Close();
            Console.WriteLine("Test battle threw an exception, check \"{0}\" for details.", LogFile);
            Console.ReadKey();
        }

        private static void Battle_OnNewEvent(PBEBattle battle, INetPacket packet)
        {
            try
            {
                switch (packet)
                {
                    case PBEActionsRequestPacket arp:
                    {
                        PBETeam team = arp.Team;
                        IEnumerable<PBEAction> actions = PBEAIManager.CreateActions(team);
                        if (!PBEBattle.AreActionsValid(team, actions))
                        {
                            throw new Exception($"{team.TrainerName}'s AI created invalid actions!");
                        }
                        PBEBattle.SelectActionsIfValid(team, actions);
                        break;
                    }
                    case PBESwitchInRequestPacket sirp:
                    {
                        PBETeam team = sirp.Team;
                        IEnumerable<(byte PokemonId, PBEFieldPosition Position)> switches = PBEAIManager.CreateSwitches(team);
                        if (!PBEBattle.AreSwitchesValid(team, switches))
                        {
                            throw new Exception($"{team.TrainerName}'s AI created invalid switches!");
                        }
                        PBEBattle.SelectSwitchesIfValid(team, switches);
                        break;
                    }
                    case PBETurnBeganPacket tbp:
                    {
                        Console.SetOut(oldWriter);
                        DateTime time = tbp.Time;
                        Console.WriteLine($"Emulating turn {tbp.TurnNumber}... ({time.Hour:D2}:{time.Minute:D2}:{time.Second:D2}:{time.Millisecond:D3})");
                        Console.SetOut(writer);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                CatchException(e);
            }
        }
        private static void Battle_OnStateChanged(PBEBattle battle)
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
                            battle.SaveReplay(ReplayFile);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error saving replay:");
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }
                        Console.WriteLine("Test battle ended. The battle was saved to \"{0}\" and \"{1}\".", LogFile, ReplayFile);
                        Console.ReadKey();
                        break;
                    }
                    case PBEBattleState.ReadyToRunTurn:
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            PBETeam team = battle.Teams[i];
                            Console.WriteLine();
                            Console.WriteLine("{0}'s team:", team.TrainerName);
                            PBEPokemon[] active = team.ActiveBattlers;
                            for (int j = 0; j < active.Length; j++)
                            {
                                Console.WriteLine(active[j]);
                                Console.WriteLine();
                            }
                        }
                        new Thread(() =>
                        {
                            try
                            {
                                battle.RunTurn();
                            }
                            catch (Exception e)
                            {
                                CatchException(e);
                            }
                        })
                        { Name = "Battle Thread" }.Start();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                CatchException(e);
            }
        }
    }
}
