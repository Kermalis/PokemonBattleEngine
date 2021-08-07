using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Legality;
using Kermalis.PokemonBattleEngine.DefaultData;
using Kermalis.PokemonBattleEngine.DefaultData.AI;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.IO;
using System.Threading;

namespace Kermalis.PokemonBattleEngineExtras
{
    internal sealed class AIBattleDemo
    {
        private const string LogFile = "AI Demo Log.txt";
        private const string ReplayFile = "AI Demo.pbereplay";
        private readonly PBEBattle _battle;
        private readonly PBEDDAI _ai0, _ai1;
        private readonly StreamWriter _writer;
        private readonly TextWriter _oldWriter;

        public AIBattleDemo()
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
                _battle = null!;
                _ai0 = null!;
                _ai1 = null!;
                _writer = null!;
                _oldWriter = null!;
                return;
            }

            PBESettings settings = PBESettings.DefaultSettings;
            //var settings = new PBESettings { NumMoves = 8, MaxPartySize = 10 };
            //settings.MakeReadOnly();
            PBELegalPokemonCollection p0, p1;

            // Competitively Randomized Pokémon
            p0 = PBEDDRandomTeamGenerator.CreateRandomTeam(settings.MaxPartySize);
            p1 = PBEDDRandomTeamGenerator.CreateRandomTeam(settings.MaxPartySize);

            _battle = new PBEBattle(PBEBattleFormat.Double, settings, new PBETrainerInfo(p0, "Trainer 0", false), new PBETrainerInfo(p1, "Trainer 1", false),
                battleTerrain: PBEDataProvider.GlobalRandom.RandomBattleTerrain());
            _battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            _battle.OnNewEvent += Battle_OnNewEvent;
            _battle.OnStateChanged += Battle_OnStateChanged;
            _ai0 = new PBEDDAI(_battle.Trainers[0]);
            _ai1 = new PBEDDAI(_battle.Trainers[1]);
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

        private void CatchException(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            Console.SetOut(_oldWriter);
            _writer.Dispose();
            _battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
            _battle.OnNewEvent -= Battle_OnNewEvent;
            _battle.OnStateChanged -= Battle_OnStateChanged;
            Console.WriteLine("Demo battle threw an exception; check \"{0}\" for details.", LogFile);
            Console.ReadKey();
        }

        private PBEDDAI GetAI(PBETrainer t)
        {
            return t.Id == 0 ? _ai0 : _ai1;
        }

        private void Battle_OnNewEvent(PBEBattle battle, IPBEPacket packet)
        {
            try
            {
                switch (packet)
                {
                    case PBEActionsRequestPacket arp:
                    {
                        GetAI(arp.Trainer).CreateActions();
                        break;
                    }
                    case PBESwitchInRequestPacket sirp:
                    {
                        GetAI(sirp.Trainer).CreateAISwitches();
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
        private void Battle_OnStateChanged(PBEBattle battle)
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
                        Console.WriteLine("Test battle ended. The battle was saved to \"{0}\" and \"{1}\".", LogFile, ReplayFile);
                        Console.ReadKey();
                        break;
                    }
                    case PBEBattleState.ReadyToRunSwitches:
                    {
                        new Thread(() =>
                        {
                            try
                            {
                                _battle.RunSwitches();
                            }
                            catch (Exception e)
                            {
                                CatchException(e);
                            }
                        })
                        { Name = "Battle Thread" }.Start();
                        break;
                    }
                    case PBEBattleState.ReadyToRunTurn:
                    {
                        foreach (PBETrainer t in _battle.Trainers)
                        {
                            Console.WriteLine();
                            Console.WriteLine("{0}'s team:", t.Name);
                            foreach (PBEBattlePokemon p in t.ActiveBattlersOrdered)
                            {
                                Console.WriteLine(p);
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
