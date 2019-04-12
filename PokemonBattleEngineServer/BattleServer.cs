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
    class BattleServer : NetServer<Player>
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
        ServerState state = ServerState.Resetting;
        PBEBattle battle;
        Player[] battlers;
        readonly List<INetPacket> spectatorPackets = new List<INetPacket>();
        readonly List<Player> readyPlayers = new List<Player>();
        readonly ManualResetEvent resetEvent = new ManualResetEvent(true);

        IPacketProcessor packetProcessor;
        public override IPacketProcessor PacketProcessor => packetProcessor;
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
                    resetEvent.WaitOne();

                    // Set new player's info
                    for (int i = 0; i < int.MaxValue; i++)
                    {
                        if (!readyPlayers.Any(p => p.BattleId == i))
                        {
                            client.BattleId = i;
                            break;
                        }
                    }
                    client.PlayerName = PBEUtils.Sample(new string[] { "Sasha", "Nikki", "Lara", "Violet", "Naomi", "Rose", "Sabrina", "Nicole" });
                    readyPlayers.Add(client);
                    Console.WriteLine($"Client connected ({client.BattleId} {client.PlayerName})");

                    foreach (Player player in readyPlayers.ToArray()) // Copy so a disconnect doesn't cause an exception
                    {
                        // Alert new player of all other players that have already joined
                        if (player != client)
                        {
                            client.Send(new PBEPlayerJoinedPacket(false, player.BattleId, player.PlayerName));
                            if (!client.WaitForResponse())
                            {
                                return;
                            }
                        }
                        // Alert all players that this new player joined (including him/herself)
                        if (player.Socket != null)
                        {
                            player.Send(new PBEPlayerJoinedPacket(player == client, client.BattleId, client.PlayerName));
                            if (!player.WaitForResponse() && player.BattleId < 2)
                            {
                                return;
                            }
                        }
                    }

                    if (client.BattleId >= 2) // Catch up spectator
                    {
                        foreach (INetPacket packet in spectatorPackets)
                        {
                            client.Send(packet);
                            if (!client.WaitForResponse())
                            {
                                return;
                            }
                        }
                    }
                    else if (readyPlayers.Count == 2) // Try to start the battle
                    {
                        state = ServerState.WaitingForParties;
                        Console.WriteLine("Two players connected! Waiting for parties...");
                        battlers = readyPlayers.ToArray();
                        battle.Teams[0].TrainerName = battlers[0].PlayerName;
                        battle.Teams[1].TrainerName = battlers[1].PlayerName;
                        SendTo(battlers, new PBEPartyRequestPacket());
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
                    resetEvent.WaitOne();
                    readyPlayers.Remove(client);

                    Console.WriteLine($"Client disconnected ({client.BattleId} {client.PlayerName})");
                    if (client.BattleId < 2)
                    {
                        if (state != ServerState.WaitingForPlayers)
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

        void CancelMatch()
        {
            if (state == ServerState.Resetting)
            {
                return;
            }
            lock (this)
            {
                if (state == ServerState.Resetting)
                {
                    return;
                }
                state = ServerState.Resetting;
                Console.WriteLine("Cancelling match...");
                SendToAll(new PBEMatchCancelledPacket());
                Reset();
            }
        }
        void Reset()
        {
            resetEvent.Reset();
            state = ServerState.Resetting;
            foreach (Player c in readyPlayers.ToArray())
            {
                DisconnectClient(c.Id);
                c.ResetEvent.Close();
            }
            battle = new PBEBattle(PBEBattleFormat.Double, PBESettings.DefaultSettings);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.OnNewEvent += BattleEventHandler;
            battle.OnStateChanged += BattleStateHandler;
            packetProcessor = new PBEPacketProcessor(battle);
            battlers = null;
            spectatorPackets.Clear();
            state = ServerState.WaitingForPlayers;
            resetEvent.Set();
        }
        public void PartySubmitted(Player player)
        {
            if (state != ServerState.WaitingForParties)
            {
                return;
            }
            lock (this)
            {
                if (state != ServerState.WaitingForParties)
                {
                    return;
                }
                PBEBattle.CreateTeamParty(battle.Teams[player.BattleId], player.Party);
            }
        }
        public void ActionsSubmitted(Player player, IEnumerable<PBEAction> actions)
        {
            if (state != ServerState.WaitingForActions)
            {
                return;
            }
            lock (this)
            {
                if (state != ServerState.WaitingForActions)
                {
                    return;
                }
                PBETeam team = battle.Teams[player.BattleId];
                Console.WriteLine($"Received actions from {team.TrainerName}!");
                if (!PBEBattle.SelectActionsIfValid(team, actions))
                {
                    Console.WriteLine("Actions are invalid!");
                    player.Send(new PBEActionsRequestPacket(team));
                }
            }
        }
        public void SwitchesSubmitted(Player player, IEnumerable<Tuple<byte, PBEFieldPosition>> switches)
        {
            if (state != ServerState.WaitingForSwitchIns)
            {
                return;
            }
            lock (this)
            {
                if (state != ServerState.WaitingForSwitchIns)
                {
                    return;
                }
                PBETeam team = battle.Teams[player.BattleId];
                Console.WriteLine($"Received switches from {team.TrainerName}!");
                if (!PBEBattle.SelectSwitchesIfValid(team, switches))
                {
                    Console.WriteLine("Switches are invalid!");
                    player.Send(new PBESwitchInRequestPacket(team));
                }
            }
        }

        void BattleStateHandler(PBEBattle battle)
        {
            Console.WriteLine("Battle state changed: {0}", battle.BattleState);
            switch (battle.BattleState)
            {
                case PBEBattleState.ReadyToBegin:
                    {
                        resetEvent.Reset();
                        foreach (Player player in battlers)
                        {
                            foreach (PBEPokemonShell shell in player.Party)
                            {
                                try
                                {
                                    shell.ValidateShell(battle.Settings);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Invalid team data received from {player.PlayerName}:");
                                    Console.WriteLine(e.Message);
                                    CancelMatch();
                                    return;
                                }
                            }
                        }
                        Console.WriteLine("Battle starting!");
                        battlers[0].Send(new PBESetPartyPacket(battle.Teams[0]));
                        if (!battlers[0].WaitForResponse())
                        {
                            CancelMatch();
                            return;
                        }
                        battlers[1].Send(new PBESetPartyPacket(battle.Teams[1]));
                        if (!battlers[1].WaitForResponse())
                        {
                            CancelMatch();
                            return;
                        }
                        new Thread(battle.Begin) { Name = "Battle Thread" }.Start();
                        break;
                    }
                case PBEBattleState.Processing:
                    {
                        resetEvent.Reset();
                        state = ServerState.BattleProcessing;
                        break;
                    }
                case PBEBattleState.ReadyToRunTurn:
                    {
                        new Thread(battle.RunTurn) { Name = "Battle Thread" }.Start();
                        break;
                    }
                case PBEBattleState.Ended:
                    {
                        resetEvent.Set();
                        state = ServerState.BattleEnded;
                        break;
                    }
            }
        }
        void BattleEventHandler(PBEBattle battle, INetPacket packet)
        {
            void SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfoHAHAHAHAHA(INetPacket realPacket, INetPacket hiddenInfo, byte teamOwnerId)
            {
                spectatorPackets.Add(hiddenInfo);
                Player teamOwner = battlers[teamOwnerId];
                teamOwner.Send(realPacket);
                if (!teamOwner.WaitForResponse())
                {
                    return;
                }
                foreach (Player player in readyPlayers.Except(new[] { teamOwner }))
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
                        Player teamOwner = battlers[mlp.MoveUserTeam.Id];
                        teamOwner.Send(mlp);
                        if (!teamOwner.WaitForResponse())
                        {
                            return;
                        }
                        break;
                    }
                case PBEMovePPChangedPacket mpcp:
                    {
                        Player teamOwner = battlers[mpcp.MoveUserTeam.Id];
                        teamOwner.Send(mpcp);
                        if (!teamOwner.WaitForResponse())
                        {
                            return;
                        }
                        break;
                    }
                case PBEPkmnFaintedPacket pfp:
                    {
                        var hiddenInfo = new PBEPkmnFaintedPacket(byte.MaxValue, pfp.PokemonPosition, pfp.PokemonTeam);
                        SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfoHAHAHAHAHA(pfp, hiddenInfo, pfp.PokemonTeam.Id);
                        break;
                    }
                case PBEPkmnFormChangedPacket pfcp:
                    {
                        var hiddenInfo = new PBEPkmnFormChangedPacket(pfcp.Pokemon, pfcp.PokemonTeam, ushort.MinValue, ushort.MinValue, ushort.MinValue, ushort.MinValue, ushort.MinValue, pfcp.NewKnownAbility != PBEAbility.MAX ? pfcp.NewAbility : PBEAbility.MAX, pfcp.NewKnownAbility, pfcp.NewSpecies, pfcp.NewType1, pfcp.NewType2, pfcp.NewWeight);
                        SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfoHAHAHAHAHA(pfcp, hiddenInfo, pfcp.PokemonTeam.Id);
                        break;
                    }
                case PBEPkmnHPChangedPacket phcp:
                    {
                        var hiddenInfo = new PBEPkmnHPChangedPacket(phcp.Pokemon, phcp.PokemonTeam, ushort.MinValue, ushort.MinValue, phcp.OldHPPercentage, phcp.NewHPPercentage);
                        SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfoHAHAHAHAHA(phcp, hiddenInfo, phcp.PokemonTeam.Id);
                        break;
                    }
                case PBEPkmnSwitchInPacket psip:
                    {
                        var hiddenInfo = new PBEPkmnSwitchInPacket(psip.Team, psip.SwitchIns.Select(s => new PBEPkmnSwitchInPacket.PBESwitchInInfo(byte.MaxValue, byte.MaxValue, s.Species, s.Nickname, s.Level, s.Shiny, s.Gender, 0, 0, s.HPPercentage, s.Status1, s.FieldPosition)), psip.Forced);
                        SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfoHAHAHAHAHA(psip, hiddenInfo, psip.Team.Id);
                        break;
                    }
                case PBEPkmnSwitchOutPacket psop:
                    {
                        var hiddenInfo = new PBEPkmnSwitchOutPacket(byte.MaxValue, byte.MaxValue, psop.PokemonPosition, psop.PokemonTeam, psop.Forced);
                        SendOriginalPacketToTeamOwnerAndEveryoneElseGetsAPacketWithHiddenInfoHAHAHAHAHA(psop, hiddenInfo, psop.PokemonTeam.Id);
                        break;
                    }
                case PBETransformPacket tp:
                    {
                        if (tp.UserTeam.Id == 0 || tp.TargetTeam.Id == 0)
                        {
                            battlers[0].Send(tp);
                            if (!battlers[0].WaitForResponse())
                            {
                                return;
                            }
                        }
                        if (tp.UserTeam.Id == 1 || tp.TargetTeam.Id == 1)
                        {
                            battlers[1].Send(tp);
                            if (!battlers[1].WaitForResponse())
                            {
                                return;
                            }
                        }
                        break;
                    }
                case PBEActionsRequestPacket _:
                    {
                        state = ServerState.WaitingForActions;
                        spectatorPackets.Add(packet);
                        foreach (Player player in readyPlayers.ToArray())
                        {
                            player.Send(packet);
                        }
                        resetEvent.Set();
                        break;
                    }
                case PBEAutoCenterPacket acp:
                    {
                        var team0 = new PBEAutoCenterPacket(acp.Pokemon1Id, acp.Pokemon1Position, acp.Pokemon1Team, byte.MaxValue, acp.Pokemon2Position, acp.Pokemon2Team);
                        var team1 = new PBEAutoCenterPacket(byte.MaxValue, acp.Pokemon1Position, acp.Pokemon1Team, acp.Pokemon2Id, acp.Pokemon2Position, acp.Pokemon2Team);
                        var spectators = new PBEAutoCenterPacket(byte.MaxValue, acp.Pokemon1Position, acp.Pokemon1Team, byte.MaxValue, acp.Pokemon2Position, acp.Pokemon2Team);
                        spectatorPackets.Add(spectators);
                        battlers[0].Send(team0);
                        if (!battlers[0].WaitForResponse())
                        {
                            return;
                        }
                        battlers[1].Send(team1);
                        if (!battlers[1].WaitForResponse())
                        {
                            return;
                        }
                        foreach (Player player in readyPlayers.Except(battlers))
                        {
                            if (player.Socket != null)
                            {
                                player.Send(spectators);
                                player.WaitForResponse();
                            }
                        }
                        break;
                    }
                case PBESwitchInRequestPacket _:
                    {
                        state = ServerState.WaitingForSwitchIns;
                        spectatorPackets.Add(packet);
                        foreach (Player player in readyPlayers.ToArray())
                        {
                            player.Send(packet);
                        }
                        resetEvent.Set();
                        break;
                    }
                default:
                    {
                        spectatorPackets.Add(packet);
                        foreach (Player player in readyPlayers.ToArray())
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
