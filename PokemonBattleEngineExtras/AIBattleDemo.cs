using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.AI;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.IO;
using System.Threading;

namespace Kermalis.PokemonBattleEngineExtras
{
    internal sealed class AIBattleDemo
    {
        private const string LogFile = "AI Demo Log.txt";
        private const string ReplayFile = "AI Demo Replay.pbereplay";
        private static PBEBattle _battle;
        private static StreamWriter _writer;
        private static TextWriter _oldWriter;

        public static void Run()
        {
            Console.WriteLine("----- Pokémon Battle Engine - AI Battle Demo -----");
            try
            {
                _writer = new StreamWriter(new FileStream(LogFile, FileMode.Create, FileAccess.Write));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot open \"{LogFile}\" for writing.");
                Console.WriteLine(e.Message);
                return;
            }

            var settings = new PBESettings { NumMoves = 8 };
            PBETeamShell team1Shell, team2Shell;

            // Completely Randomized Pokémon
            team1Shell = new PBETeamShell(settings, settings.MaxPartySize, true);
            team2Shell = new PBETeamShell(settings, settings.MaxPartySize, true);

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

            _battle = new PBEBattle(PBEBattleFormat.Double, team1Shell, "Team 1", team2Shell, "Team 2");
            _battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            _battle.OnNewEvent += Battle_OnNewEvent;
            _battle.OnStateChanged += Battle_OnStateChanged;
            _oldWriter = Console.Out;
            Console.SetOut(_writer);
            new Thread(() =>
            {
                try
                {
                    _battle.Begin();
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
            Console.SetOut(_oldWriter);
            _writer.Dispose();
            _battle.Dispose();
            Console.WriteLine("Demo battle threw an exception; check \"{0}\" for details.", LogFile);
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
                        PBETurnAction[] actions = PBEAI.CreateActions(team);
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
                        PBESwitchIn[] switches = PBEAI.CreateSwitches(team);
                        if (!PBEBattle.AreSwitchesValid(team, switches))
                        {
                            throw new Exception($"{team.TrainerName}'s AI created invalid switches!");
                        }
                        PBEBattle.SelectSwitchesIfValid(team, switches);
                        break;
                    }
                    case PBETurnBeganPacket tbp:
                    {
                        Console.SetOut(_oldWriter);
                        DateTime time = DateTime.Now;
                        Console.WriteLine($"Emulating turn {tbp.TurnNumber}... ({time.Hour:D2}:{time.Minute:D2}:{time.Second:D2}:{time.Millisecond:D3})");
                        Console.SetOut(_writer);
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
                switch (_battle.BattleState)
                {
                    case PBEBattleState.Ended:
                    {
                        Console.SetOut(_oldWriter);
                        _writer.Dispose();
                        try
                        {
                            _battle.SaveReplay(ReplayFile);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error saving replay:");
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }
                        _battle.Dispose();
                        Console.WriteLine("Test battle ended. The battle was saved to \"{0}\" and \"{1}\".", LogFile, ReplayFile);
                        Console.ReadKey();
                        break;
                    }
                    case PBEBattleState.ReadyToRunTurn:
                    {
                        foreach (PBETeam team in _battle.Teams)
                        {
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
                                _battle.RunTurn();
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
