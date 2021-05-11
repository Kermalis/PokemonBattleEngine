using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Network;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace Kermalis.PokemonBattleEngineServer
{
    internal sealed class BattleServer
    {
        // TODO: Events still sent after someone disconnects during a turn, need to return out of event subscription
        // Server does not support wild battles yet
        private enum ServerState
        {
            Resetting,           // Server is currently resetting the game
            WaitingForPlayers,   // Server is waiting for players to connect
            WaitingForActions,   // Server is waiting for players to select actions
            WaitingForSwitchIns, // Server is waiting for players to switch in new Pokémon
            BattleProcessing,    // Battle is running and sending events
            BattleEnded          // Battle ended
        }
        public readonly bool RequireLegalParties;
        private readonly PBEServer _server;
        private ServerState _state = ServerState.Resetting;
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(true);
        private PBEBattle _battle;
        public readonly PBESettings Settings = PBESettings.DefaultSettings;
        private const PBEBattleFormat BattleFormat = PBEBattleFormat.Double;
        private const int NumTrainersPerTeam = 1; // Must be changed if BattleFormat is changed
        private const int NumBattlers = 2 * NumTrainersPerTeam;
        private Player[] _battlers;
        private readonly List<IPBEPacket> _spectatorPackets = new List<IPBEPacket>();
        private readonly Dictionary<PBEServerClient, Player> _readyPlayers = new Dictionary<PBEServerClient, Player>();

        private byte _battlerCounter = 0;
        private PBETrainerInfo[][] _incomingTrainers;

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:\tPokemonBattleEngineServer.dll {ip} {port} {requireLegalParties}");
            Console.WriteLine("Example:\tPokemonBattleEngineServer.dll 127.0.0.1 8888 true");
        }
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                PrintUsage();
            }
            else
            {
                PBEDataProvider.InitEngine(string.Empty);
                new BattleServer(args);
            }
        }
        private BattleServer(string[] args)
        {
            if (!IPAddress.TryParse(args[0], out IPAddress ip)
                || !ushort.TryParse(args[1], out ushort port)
                || !bool.TryParse(args[2], out bool requireLegalParties))
            {
                PrintUsage();
            }
            else
            {
                using (_server = new PBEServer())
                {
                    _server.ClientConnected += OnClientConnected;
                    _server.ClientDisconnected += OnClientDisconnected;
                    _server.ClientRefused += OnClientRefused;
                    _server.Error += OnError; // Events unsubscribe in _server.Dispose()
                    _server.Start(new IPEndPoint(ip, port), 100);
                    RequireLegalParties = requireLegalParties;
                    Console.WriteLine("Server online.");
                    Reset();
                    Thread.Sleep(-1);
                }
            }
        }
        private void OnClientConnected(object sender, PBEServerClient client)
        {
            // Need to spawn a new thread so "WaitOne()" doesn't block the thread that receives client packets
            new Thread(() =>
            {
                lock (this)
                {
                    // Wait for the server to be in a state where no events will be sent
                    _resetEvent.WaitOne();

                    string name = PBEDataProvider.GlobalRandom.RandomElement(new string[] { "Sasha", "Nikki", "Lara", "Violet", "Naomi", "Rose", "Sabrina", "Nicole" });
                    if (_battlerCounter < NumBattlers)
                    {
                        byte i = _battlerCounter;
                        Console.WriteLine($"Client connected ({client.IP} {i} {name})");
                        var newPlayer = new Player(this, client, i, name);
                        IPBEPokemonCollection party = newPlayer.AskForParty(RequireLegalParties);
                        if (party == null)
                        {
                            newPlayer.Dispose();
                            return;
                        }
                        _incomingTrainers[i / NumTrainersPerTeam][i % NumTrainersPerTeam] = new PBETrainerInfo(party, name, false);
                        _battlers[i] = newPlayer;
                        _readyPlayers.Add(client, newPlayer);

                        // Start battle
                        if (++_battlerCounter == NumBattlers)
                        {
                            Console.WriteLine("All players connected!");
                            _battle = new PBEBattle(BattleFormat, Settings, _incomingTrainers[0], _incomingTrainers[1]);
                            _incomingTrainers = null;
                            _battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
                            _battle.OnNewEvent += BattleEventHandler;
                            _battle.OnStateChanged += BattleStateHandler;
                            _server.Battle = _battle;
                            BattleStateHandler(_battle); // Call RunTurn, which sends battle packet
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Client connected ({client.IP} {byte.MaxValue} {name})");
                        var newPlayer = new Player(this, client, byte.MaxValue, name);
                        var chakoPackay = new PBEPlayerJoinedPacket(name);
                        foreach (Player p in _readyPlayers.Values.ToArray())
                        {
                            p.Send(chakoPackay);
                            if (!p.WaitForResponse(typeof(PBEResponsePacket)) && p.BattleId != byte.MaxValue)
                            {
                                newPlayer.Dispose();
                                return;
                            }
                        }
                        _spectatorPackets.Add(chakoPackay);
                        foreach (IPBEPacket packet in _spectatorPackets)
                        {
                            newPlayer.Send(packet);
                            if (!newPlayer.WaitForResponse(typeof(PBEResponsePacket)))
                            {
                                newPlayer.Dispose();
                                return;
                            }
                        }
                        _readyPlayers.Add(client, newPlayer);
                    }
                }
            })
            {
                Name = "Client Connected Thread"
            }.Start();
        }
        private void OnClientDisconnected(object sender, PBEServerClient client)
        {
            // Need to spawn a new thread so "WaitOne()" doesn't block the thread that receives client packets
            new Thread(() =>
            {
                lock (this)
                {
                    // Wait for the server to be in a state where no events will be sent
                    _resetEvent.WaitOne();

                    if (_readyPlayers.TryGetValue(client, out Player player))
                    {
                        _readyPlayers.Remove(client);
                        player.Dispose();

                        Console.WriteLine($"Client disconnected ({player.BattleId} {player.TrainerName})");
                        if (player.BattleId != byte.MaxValue)
                        {
                            CancelMatch();
                        }
                        else
                        {
                            // Temporarily ignore spectators
                        }
                    }
                }
            })
            {
                Name = "Client Disconnected Thread"
            }.Start();
        }
        private void OnClientRefused(object sender, IPEndPoint clientIP, bool refusedForBan)
        {
            Console.WriteLine($"Client refused ({clientIP} {(refusedForBan ? "banned" : "no more room")})");
        }
        private void OnError(object sender, Exception ex)
        {
            Console.WriteLine($"Server error:{Environment.NewLine}{ex}");
        }

        private void CancelMatch()
        {
            if (_state == ServerState.Resetting)
            {
                return;
            }
            lock (this)
            {
                if (_state == ServerState.Resetting)
                {
                    return;
                }
                _state = ServerState.Resetting;
                Console.WriteLine("Cancelling match...");
                SendToAll(new PBEMatchCancelledPacket());
                Reset();
            }
        }
        private void Reset()
        {
            Console.WriteLine("Resetting...");
            _resetEvent.Reset();
            _state = ServerState.Resetting;
            foreach (Player c in _readyPlayers.Values.ToArray())
            {
                DisconnectClient(c);
            }
            _battlerCounter = 0;
            if (_battle != null)
            {
                _battle.OnNewEvent -= PBEBattle.ConsoleBattleEventHandler;
                _battle.OnNewEvent -= BattleEventHandler;
                _battle.OnStateChanged -= BattleStateHandler;
            }
            _battlers = new Player[NumBattlers];
            _incomingTrainers = new PBETrainerInfo[2][];
            for (int i = 0; i < 2; i++)
            {
                _incomingTrainers[i] = new PBETrainerInfo[NumTrainersPerTeam];
            }
            _spectatorPackets.Clear();
            _state = ServerState.WaitingForPlayers;
            _resetEvent.Set();
            Console.WriteLine("Reset complete.");
        }
        public void ActionsSubmitted(Player player, IReadOnlyList<PBETurnAction> actions)
        {
            if (_state != ServerState.WaitingForActions)
            {
                return;
            }
            lock (this)
            {
                if (_state != ServerState.WaitingForActions)
                {
                    return;
                }
                Console.WriteLine($"Received actions ({player.BattleId} {player.TrainerName})");
                string valid = _battle.Trainers[player.BattleId].SelectActionsIfValid(actions);
                if (valid != null)
                {
                    Console.WriteLine("Actions are invalid! - {0}", valid);
                    CancelMatch();
                }
            }
        }
        public void FleeSubmitted(Player player)
        {
            if (_state != ServerState.WaitingForActions && _state != ServerState.WaitingForSwitchIns)
            {
                return;
            }
            lock (this)
            {
                if (_state != ServerState.WaitingForActions && _state != ServerState.WaitingForSwitchIns)
                {
                    return;
                }
                Console.WriteLine($"Received flee request ({player.BattleId} {player.TrainerName})");
                string valid = _battle.Trainers[player.BattleId].SelectFleeIfValid();
                if (valid != null)
                {
                    Console.WriteLine("Flee is invalid! - {0}", valid);
                    CancelMatch();
                }
            }
        }
        public void SwitchesSubmitted(Player player, IReadOnlyList<PBESwitchIn> switches)
        {
            if (_state != ServerState.WaitingForSwitchIns)
            {
                return;
            }
            lock (this)
            {
                if (_state != ServerState.WaitingForSwitchIns)
                {
                    return;
                }
                Console.WriteLine($"Received switches ({player.BattleId} {player.TrainerName})");
                string valid = _battle.Trainers[player.BattleId].SelectSwitchesIfValid(switches);
                if (valid != null)
                {
                    Console.WriteLine("Switches are invalid! - {0}", valid);
                    CancelMatch();
                }
            }
        }

        private void BattleStateHandler(PBEBattle battle)
        {
            Console.WriteLine("Battle state changed: {0}", battle.BattleState);
            switch (battle.BattleState)
            {
                case PBEBattleState.Ended:
                {
                    _battle.SaveReplay("Server Battle.pbereplay");
                    _resetEvent.Set();
                    _state = ServerState.BattleEnded;
                    break;
                }
                case PBEBattleState.Processing:
                {
                    _resetEvent.Reset();
                    _state = ServerState.BattleProcessing;
                    break;
                }
                case PBEBattleState.ReadyToBegin:
                {
                    _resetEvent.Reset();
                    Console.WriteLine("Battle starting!");
                    new Thread(battle.Begin) { Name = "Battle Thread" }.Start();
                    break;
                }
                case PBEBattleState.ReadyToRunSwitches:
                {
                    new Thread(battle.RunSwitches) { Name = "Battle Thread" }.Start();
                    break;
                }
                case PBEBattleState.ReadyToRunTurn:
                {
                    new Thread(battle.RunTurn) { Name = "Battle Thread" }.Start();
                    break;
                }
            }
        }
        private void BattleEventHandler(PBEBattle battle, IPBEPacket packet)
        {
            void SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(IPBEPacket realPacket, IPBEPacket hiddenInfo, byte trainerId)
            {
                Player trainer = _battlers[trainerId];
                trainer.Send(realPacket);
                if (!trainer.WaitForResponse(typeof(PBEResponsePacket)))
                {
                    return;
                }
                foreach (Player p in _readyPlayers.Values.Except(new[] { trainer }))
                {
                    p.Send(hiddenInfo);
                    if (!p.WaitForResponse(typeof(PBEResponsePacket)) && p.BattleId != byte.MaxValue)
                    {
                        return;
                    }
                }
                _spectatorPackets.Add(hiddenInfo);
            }

            switch (packet)
            {
                case PBEMoveLockPacket mlp:
                {
                    Player p = _battlers[mlp.MoveUserTrainer.Id];
                    p.Send(mlp);
                    if (!p.WaitForResponse(typeof(PBEResponsePacket)))
                    {
                        return;
                    }
                    break;
                }
                case PBEPkmnFaintedPacket pfp:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(pfp, new PBEPkmnFaintedPacket_Hidden(pfp), pfp.PokemonTrainer.Id);
                    break;
                }
                case PBEPkmnFormChangedPacket pfcp:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(pfcp, new PBEPkmnFormChangedPacket_Hidden(pfcp), pfcp.PokemonTrainer.Id);
                    break;
                }
                case PBEPkmnHPChangedPacket phcp:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(phcp, new PBEPkmnHPChangedPacket_Hidden(phcp), phcp.PokemonTrainer.Id);
                    break;
                }
                case PBEPkmnSwitchInPacket psip:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(psip, new PBEPkmnSwitchInPacket_Hidden(psip), psip.Trainer.Id);
                    break;
                }
                case PBEPkmnSwitchOutPacket psop:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(psop, new PBEPkmnSwitchOutPacket_Hidden(psop), psop.PokemonTrainer.Id);
                    break;
                }
                /*case PBEWildPkmnAppearedPacket wpap:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(wpap, new PBEWildPkmnAppearedPacket_Hidden(wpap), wpap.Trainer.Id);
                    break;
                }*/
                case PBEReflectTypePacket rtp:
                {
                    var hidden = new PBEReflectTypePacket_Hidden(rtp);
                    foreach (Player p in _battlers)
                    {
                        p.Send(rtp.UserTrainer.Id == p.BattleId || rtp.TargetTrainer.Id == p.BattleId ? (IPBEPacket)rtp : hidden);
                        if (!p.WaitForResponse(typeof(PBEResponsePacket)))
                        {
                            return;
                        }
                    }
                    foreach (Player player in _readyPlayers.Values.Except(_battlers))
                    {
                        player.Send(hidden);
                        player.WaitForResponse(typeof(PBEResponsePacket));
                    }
                    _spectatorPackets.Add(hidden);
                    break;
                }
                case PBETransformPacket tp:
                {
                    foreach (Player p in _battlers)
                    {
                        if (tp.UserTrainer.Id == p.BattleId || tp.TargetTrainer.Id == p.BattleId)
                        {
                            p.Send(tp);
                            if (!p.WaitForResponse(typeof(PBEResponsePacket)))
                            {
                                return;
                            }
                        }
                    }
                    break;
                }
                case PBEActionsRequestPacket arp:
                {
                    _state = ServerState.WaitingForActions;
                    foreach (Player player in _readyPlayers.Values.ToArray())
                    {
                        player.Send(packet);
                        if (!player.WaitForResponse(typeof(PBEResponsePacket)) && player.BattleId != byte.MaxValue)
                        {
                            return;
                        }
                    }
                    _battlers[arp.Trainer.Id].SetWaitingForActions(typeof(PBEActionsResponsePacket));
                    _spectatorPackets.Add(packet);
                    _resetEvent.Set();
                    break;
                }
                case PBEAutoCenterPacket acp:
                {
                    var team0 = new PBEAutoCenterPacket_Hidden0(acp);
                    var team1 = new PBEAutoCenterPacket_Hidden1(acp);
                    var spectators = new PBEAutoCenterPacket_Hidden01(acp);
                    foreach (Player p in _readyPlayers.Values.ToArray())
                    {
                        IPBEAutoCenterPacket chakoPackay = p.BattleId == byte.MaxValue
                            ? spectators
                            : acp.Pokemon0Trainer.Id == p.BattleId ? team0 : acp.Pokemon1Trainer.Id == p.BattleId ? (IPBEAutoCenterPacket)team1: spectators;
                        p.Send(chakoPackay);
                        if (!p.WaitForResponse(typeof(PBEResponsePacket)) && p.BattleId != byte.MaxValue)
                        {
                            return;
                        }
                    }
                    _spectatorPackets.Add(spectators);
                    break;
                }
                case PBEBattlePacket bp:
                {
                    foreach (Player p in _battlers)
                    {
                        p.Send(new PBEBattlePacket(bp, p.BattleId));
                        if (!p.WaitForResponse(typeof(PBEResponsePacket)))
                        {
                            return;
                        }
                    }
                    var spectators = new PBEBattlePacket(bp, null);
                    foreach (Player player in _readyPlayers.Values.Except(_battlers))
                    {
                        player.Send(spectators);
                        player.WaitForResponse(typeof(PBEResponsePacket));
                    }
                    _spectatorPackets.Add(spectators);
                    break;
                }
                case PBESwitchInRequestPacket sirp:
                {
                    _state = ServerState.WaitingForSwitchIns;
                    foreach (Player player in _readyPlayers.Values.ToArray())
                    {
                        player.Send(packet);
                        if (!player.WaitForResponse(typeof(PBEResponsePacket)) && player.BattleId != byte.MaxValue)
                        {
                            return;
                        }
                    }
                    _battlers[sirp.Trainer.Id].SetWaitingForActions(typeof(PBESwitchInResponsePacket));
                    _spectatorPackets.Add(packet);
                    _resetEvent.Set();
                    break;
                }
                default:
                {
                    foreach (Player player in _readyPlayers.Values.ToArray())
                    {
                        player.Send(packet);
                        if (!player.WaitForResponse(typeof(PBEResponsePacket)) && player.BattleId != byte.MaxValue)
                        {
                            return;
                        }
                    }
                    _spectatorPackets.Add(packet);
                    break;
                }
            }
        }

        public void DisconnectClient(Player player)
        {
            Console.WriteLine($"Disconnecting client ({player.BattleId} {player.TrainerName})");
            _server.DisconnectClient(player.Client);
        }
        private void SendToAll(IPBEPacket packet)
        {
            foreach (Player p in _readyPlayers.Values.ToArray())
            {
                p.Send(packet);
            }
        }
    }
}
