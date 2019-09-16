using Ether.Network.Packets;
using Ether.Network.Server;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Kermalis.PokemonBattleEngineServer
{
    internal sealed class BattleServer : NetServer<Player>
    {
        private enum ServerState
        {
            Resetting,           // Server is currently resetting the game
            WaitingForPlayers,   // Server is waiting for 2 players to connect
            WaitingForParties,   // Server is waiting for both players to send their party
            WaitingForActions,   // Server is waiting for players to select actions
            WaitingForSwitchIns, // Server is waiting for players to switch in new Pokémon
            BattleProcessing,    // Battle is running and sending events
            BattleEnded          // Battle ended
        }
        private ServerState _state = ServerState.Resetting;
        private PBEBattle _battle;
        private Player[] _battlers;
        private readonly List<INetPacket> _spectatorPackets = new List<INetPacket>();
        private readonly List<Player> _readyPlayers = new List<Player>();
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(true);

        private IPacketProcessor _packetProcessor;
        public override IPacketProcessor PacketProcessor => _packetProcessor;
        public static void Main(string[] args)
        {
            using (var server = new BattleServer(args[0], int.Parse(args[1])))
            {
                server.Start();
            }
        }
        public BattleServer(string host, int port)
        {
            Configuration.Backlog = 50;
            Configuration.Host = host;
            Configuration.Port = port;
            Configuration.MaximumNumberOfConnections = 100;
            Configuration.BufferSize = 1024;
            Configuration.Blocking = true;
        }
        protected override void Initialize()
        {
            PBEUtils.CreateDatabaseConnection(string.Empty);
            Console.WriteLine("Server online.");
            Reset();
        }
        protected override void OnClientConnected(Player client)
        {
            // Need to spawn a new thread so "WaitOne()" doesn't block the thread that receives client packets
            new Thread(() =>
            {
                lock (this)
                {
                    // Wait for the server to be in a state where no events will be sent
                    _resetEvent.WaitOne();

                    // Set new player's info
                    for (int i = 0; i < int.MaxValue; i++)
                    {
                        if (!_readyPlayers.Any(p => p.BattleId == i))
                        {
                            client.BattleId = i;
                            break;
                        }
                    }
                    client.TrainerName = Utils.RandomElement(new string[] { "Sasha", "Nikki", "Lara", "Violet", "Naomi", "Rose", "Sabrina", "Nicole" });
                    _readyPlayers.Add(client);
                    Console.WriteLine($"Client connected ({client.BattleId} {client.TrainerName})");

                    foreach (Player player in _readyPlayers.ToArray()) // Copy so a disconnect doesn't cause an exception
                    {
                        // Alert new player of all other players that have already joined
                        if (player != client)
                        {
                            client.Send(new PBEPlayerJoinedPacket(false, player.BattleId, player.TrainerName));
                            if (!client.WaitForResponse())
                            {
                                return;
                            }
                        }
                        // Alert all players that this new player joined (including him/herself)
                        if (player.Socket != null)
                        {
                            player.Send(new PBEPlayerJoinedPacket(player == client, client.BattleId, client.TrainerName));
                            if (!player.WaitForResponse() && player.BattleId < 2)
                            {
                                return;
                            }
                        }
                    }

                    if (client.BattleId >= 2) // Catch up spectator
                    {
                        foreach (INetPacket packet in _spectatorPackets)
                        {
                            client.Send(packet);
                            if (!client.WaitForResponse())
                            {
                                return;
                            }
                        }
                    }
                    else if (_readyPlayers.Count == 2) // Try to start the battle
                    {
                        _state = ServerState.WaitingForParties;
                        Console.WriteLine("Two players connected! Waiting for parties...");
                        _battlers = _readyPlayers.ToArray();
                        SendTo(_battlers, new PBEPartyRequestPacket());
                    }
                }
            })
            {
                Name = "Client Connected Thread"
            }.Start();
        }
        protected override void OnClientDisconnected(Player client)
        {
            // Need to spawn a new thread so "WaitOne()" doesn't block the thread that receives client packets
            new Thread(() =>
            {
                lock (this)
                {
                    // Wait for the server to be in a state where no events will be sent
                    _resetEvent.WaitOne();
                    _readyPlayers.Remove(client);

                    Console.WriteLine($"Client disconnected ({client.BattleId} {client.TrainerName})");
                    if (client.BattleId < 2)
                    {
                        if (_state != ServerState.WaitingForPlayers)
                        {
                            CancelMatch();
                        }
                    }
                    else
                    {
                        // Temporarily ignore spectators
                    }
                }
            })
            {
                Name = "Client Disconnected Thread"
            }.Start();
        }
        protected override void OnError(Exception e)
        {
            Console.WriteLine($"Server error: {e}");
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
            _resetEvent.Reset();
            _state = ServerState.Resetting;
            foreach (Player c in _readyPlayers.ToArray())
            {
                DisconnectClient(c.Id);
                c.ResetEvent.Close();
            }
            _battle = new PBEBattle(PBEBattleFormat.Double, PBESettings.DefaultSettings);
            _battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            _battle.OnNewEvent += BattleEventHandler;
            _battle.OnStateChanged += BattleStateHandler;
            _packetProcessor = new PBEPacketProcessor(_battle);
            _battlers = null;
            _spectatorPackets.Clear();
            _state = ServerState.WaitingForPlayers;
            _resetEvent.Set();
        }
        public void PartySubmitted(Player player)
        {
            if (_state != ServerState.WaitingForParties)
            {
                return;
            }
            lock (this)
            {
                if (_state != ServerState.WaitingForParties)
                {
                    return;
                }
                PBEBattle.CreateTeamParty(_battle.Teams[player.BattleId], player.TeamShell, player.TrainerName);
            }
        }
        public void ActionsSubmitted(Player player, IList<PBETurnAction> actions)
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
                PBETeam team = _battle.Teams[player.BattleId];
                Console.WriteLine($"Received actions from {player.TrainerName}!");
                if (!PBEBattle.SelectActionsIfValid(team, actions))
                {
                    Console.WriteLine("Actions are invalid!");
                    CancelMatch();
                }
            }
        }
        public void SwitchesSubmitted(Player player, IList<PBESwitchIn> switches)
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
                PBETeam team = _battle.Teams[player.BattleId];
                Console.WriteLine($"Received switches from {player.TrainerName}!");
                if (!PBEBattle.SelectSwitchesIfValid(team, switches))
                {
                    Console.WriteLine("Switches are invalid!");
                    CancelMatch();
                }
            }
        }

        private void BattleStateHandler(PBEBattle battle)
        {
            Console.WriteLine("Battle state changed: {0}", battle.BattleState);
            switch (battle.BattleState)
            {
                case PBEBattleState.ReadyToBegin:
                {
                    _resetEvent.Reset();
                    foreach (Player player in _battlers)
                    {
                        foreach (PBEPokemonShell shell in player.TeamShell)
                        {
                            try
                            {
                                // Not currently necessary, but it would be necessary eventually because PBEMovesetBuilder cannot check if a moveset "makes sense" for the method the Pokémon was obtained in
                                // Eventually we would probably want to store that sort of information in PBEPokemonShell
                                PBELegalityChecker.MoveLegalityCheck(shell.Moveset);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Illegal moveset received from {player.TrainerName}");
                                Console.WriteLine(e.Message);
                                CancelMatch();
                                return;
                            }
                        }
                    }
                    Console.WriteLine("Battle starting!");
                    new Thread(battle.Begin) { Name = "Battle Thread" }.Start();
                    break;
                }
                case PBEBattleState.Processing:
                {
                    _resetEvent.Reset();
                    _state = ServerState.BattleProcessing;
                    break;
                }
                case PBEBattleState.ReadyToRunTurn:
                {
                    new Thread(battle.RunTurn) { Name = "Battle Thread" }.Start();
                    break;
                }
                case PBEBattleState.Ended:
                {
                    _resetEvent.Set();
                    _state = ServerState.BattleEnded;
                    break;
                }
            }
        }
        private void BattleEventHandler(PBEBattle battle, INetPacket packet)
        {
            void SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(INetPacket realPacket, INetPacket hiddenInfo, byte teamOwnerId)
            {
                _spectatorPackets.Add(hiddenInfo);
                Player teamOwner = _battlers[teamOwnerId];
                teamOwner.Send(realPacket);
                if (!teamOwner.WaitForResponse())
                {
                    return;
                }
                foreach (Player player in _readyPlayers.Except(new[] { teamOwner }))
                {
                    if (player.Socket != null)
                    {
                        player.Send(hiddenInfo);
                        if (!player.WaitForResponse() && player.BattleId < 2)
                        {
                            return;
                        }
                    }
                }
            }

            switch (packet)
            {
                case PBEMoveLockPacket mlp:
                {
                    Player teamOwner = _battlers[mlp.MoveUserTeam.Id];
                    teamOwner.Send(mlp);
                    if (!teamOwner.WaitForResponse())
                    {
                        return;
                    }
                    break;
                }
                case PBEPkmnFaintedPacket pfp:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(pfp, pfp.MakeHidden(), pfp.PokemonTeam.Id);
                    break;
                }
                case PBEPkmnFormChangedPacket pfcp:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(pfcp, pfcp.MakeHidden(), pfcp.PokemonTeam.Id);
                    break;
                }
                case PBEPkmnHPChangedPacket phcp:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(phcp, phcp.MakeHidden(), phcp.PokemonTeam.Id);
                    break;
                }
                case PBEPkmnSwitchInPacket psip:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(psip, psip.MakeHidden(), psip.Team.Id);
                    break;
                }
                case PBEPkmnSwitchOutPacket psop:
                {
                    SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfo(psop, psop.MakeHidden(), psop.PokemonTeam.Id);
                    break;
                }
                case PBETransformPacket tp:
                {
                    if (tp.UserTeam.Id == 0 || tp.TargetTeam.Id == 0)
                    {
                        _battlers[0].Send(tp);
                        if (!_battlers[0].WaitForResponse())
                        {
                            return;
                        }
                    }
                    if (tp.UserTeam.Id == 1 || tp.TargetTeam.Id == 1)
                    {
                        _battlers[1].Send(tp);
                        if (!_battlers[1].WaitForResponse())
                        {
                            return;
                        }
                    }
                    break;
                }
                case PBEActionsRequestPacket _:
                {
                    _state = ServerState.WaitingForActions;
                    _spectatorPackets.Add(packet);
                    foreach (Player player in _readyPlayers.ToArray())
                    {
                        player.Send(packet);
                    }
                    _resetEvent.Set();
                    break;
                }
                case PBEAutoCenterPacket acp:
                {
                    PBEAutoCenterPacket team0 = acp.MakeHidden(false, true);
                    PBEAutoCenterPacket team1 = acp.MakeHidden(true, false);
                    PBEAutoCenterPacket spectators = acp.MakeHidden(true, true);
                    _spectatorPackets.Add(spectators);
                    _battlers[0].Send(team0);
                    if (!_battlers[0].WaitForResponse())
                    {
                        return;
                    }
                    _battlers[1].Send(team1);
                    if (!_battlers[1].WaitForResponse())
                    {
                        return;
                    }
                    foreach (Player player in _readyPlayers.Except(_battlers))
                    {
                        if (player.Socket != null)
                        {
                            player.Send(spectators);
                            player.WaitForResponse();
                        }
                    }
                    break;
                }
                case PBETeamPacket tp:
                {
                    Player teamOwner = _battlers[tp.Team.Id];
                    teamOwner.Send(tp);
                    if (!teamOwner.WaitForResponse())
                    {
                        return;
                    }
                    break;
                }
                case PBESwitchInRequestPacket _:
                {
                    _state = ServerState.WaitingForSwitchIns;
                    _spectatorPackets.Add(packet);
                    foreach (Player player in _readyPlayers.ToArray())
                    {
                        player.Send(packet);
                    }
                    _resetEvent.Set();
                    break;
                }
                default:
                {
                    _spectatorPackets.Add(packet);
                    foreach (Player player in _readyPlayers.ToArray())
                    {
                        if (player.Socket != null)
                        {
                            player.Send(packet);
                            if (!player.WaitForResponse() && player.BattleId < 2)
                            {
                                return;
                            }
                        }
                    }
                    break;
                }
            }
        }
    }
}
