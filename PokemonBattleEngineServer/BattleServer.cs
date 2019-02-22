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
            Startup, // Server is starting up
            Resetting, // Server is currently resetting the game
            Cancelling, // Server is cancelling the game
            WaitingForPlayers, // Server is waiting for 2 players to connect
            WaitingForParties, // Server is waiting for both players to send their party
            StartingMatch, // Server is starting the battle
            WaitingForActions, // Server is waiting for players to select actions
            BattleProcessing, // Battle is running and sending events
            WaitingForSwitchIns, // Server is waiting for players to switch in new Pokémon
        }
        ServerState state = ServerState.Startup;
        readonly PBEBattle battle;
        Player[] battlers;
        int idCounter;
        readonly List<INetPacket> spectatorPackets = new List<INetPacket>();
        readonly List<Player> upToDateSpectators = new List<Player>();
        readonly ManualResetEvent resetEvent = new ManualResetEvent(true);

        public override IPacketProcessor PacketProcessor { get; }
        public static void Main(string[] args)
        {
            using (var server = new BattleServer("127.0.0.1", 8888))
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

            battle = new PBEBattle(PBEBattleFormat.Double, PBESettings.DefaultSettings);
            battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
            battle.OnNewEvent += BattleEventHandler;
            battle.OnStateChanged += BattleStateHandler;
            PacketProcessor = new PBEPacketProcessor(battle);
        }
        protected override void Initialize()
        {
            Console.WriteLine("Server online.");
            StopMatchAndReset();
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
                    client.BattleId = idCounter++;
                    client.PlayerName = PBEUtils.Sample(new string[] { "Sasha", "Nikki", "Lara", "Violet", "Naomi", "Rose", "Sabrina", "Nicole" });
                    Console.WriteLine($"Client connected ({client.Id} {client.BattleId} {client.PlayerName})");

                    foreach (Player player in Clients)
                    {
                        // Alert all other players that this new player joined
                        player.Send(new PBEPlayerJoinedPacket(player == client, client.BattleId, client.PlayerName));
                        player.ResetEvent.WaitOne();
                        // Alert new player of all other players that have already joined
                        if (player != client)
                        {
                            client.Send(new PBEPlayerJoinedPacket(false, player.BattleId, player.PlayerName));
                            client.ResetEvent.WaitOne();
                        }
                    }

                    if (battlers != null) // Catch up spectators
                    {
                        foreach (INetPacket packet in spectatorPackets)
                        {
                            client.Send(packet);
                            client.ResetEvent.WaitOne();
                        }
                        upToDateSpectators.Add(client);
                    }
                    else if (battlers == null && Clients.Count() == 2) // Try to start the battle
                    {
                        Console.WriteLine("Two players connected!");
                        state = ServerState.WaitingForParties;
                        Console.WriteLine("Waiting for parties...");
                        battlers = Clients.OrderBy(c => c.BattleId).Take(2).ToArray();
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

                    Console.WriteLine($"Client disconnected ({client.Id})");
                    if (client.BattleId < 2)
                    {
                        switch (state)
                        {
                            case ServerState.Startup:
                            case ServerState.Resetting:
                            case ServerState.Cancelling: break;
                            default: CancelMatch(); break;
                        }
                    }
                    else
                    {
                        // Temporarily ignore spectators
                        upToDateSpectators.Remove(client);
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
            Environment.Exit(0); // Temporary
            if (state == ServerState.Cancelling)
            {
                return;
            }
            lock (this)
            {
                if (state == ServerState.Cancelling)
                {
                    return;
                }
                state = ServerState.Cancelling;
                Console.WriteLine("Cancelling match...");
                SendToAll(new PBEMatchCancelledPacket());
                StopMatchAndReset();
            }
        }
        void StopMatchAndReset()
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
                resetEvent.Reset();
                state = ServerState.Resetting;
                foreach (Player c in Clients)
                {
                    c.ResetEvent.Close();
                    DisconnectClient(c.Id);
                }
                // TODO: Create a new battle
                battlers = null;
                idCounter = 0;
                spectatorPackets.Clear();
                state = ServerState.WaitingForPlayers;
                resetEvent.Set();
            }
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
                        state = ServerState.StartingMatch;
                        Console.WriteLine("Battle starting!");
                        battlers[0].Send(new PBESetPartyPacket(battle.Teams[0]));
                        battlers[1].Send(new PBESetPartyPacket(battle.Teams[1]));
                        battlers[0].ResetEvent.WaitOne();
                        battlers[1].ResetEvent.WaitOne();
                        new Thread(battle.Begin) { Name = "Battle Thread" }.Start();
                        return;
                    }
                case PBEBattleState.Processing:
                    {
                        resetEvent.Reset();
                        state = ServerState.BattleProcessing;
                        return;
                    }
                case PBEBattleState.ReadyToRunTurn:
                    {
                        new Thread(battle.RunTurn) { Name = "Battle Thread" }.Start();
                        return;
                    }
                case PBEBattleState.WaitingForActions:
                    {
                        state = ServerState.WaitingForActions;
                        resetEvent.Set();
                        return;
                    }
                case PBEBattleState.WaitingForSwitchIns:
                    {
                        state = ServerState.WaitingForSwitchIns;
                        resetEvent.Set();
                        return;
                    }
            }
        }
        void BattleEventHandler(PBEBattle battle, INetPacket packet)
        {
            IEnumerable<Player> readyToReceivePackets = upToDateSpectators.Concat(battlers);
            switch (packet)
            {
                case PBEMoveLockPacket mlp:
                    {
                        Player teamOwner = battlers[mlp.MoveUserTeam.Id];
                        teamOwner.Send(mlp);
                        teamOwner.ResetEvent.WaitOne();
                        break;
                    }
                case PBEMovePPChangedPacket mpcp:
                    {
                        Player teamOwner = battlers[mpcp.MoveUserTeam.Id];
                        teamOwner.Send(mpcp);
                        teamOwner.ResetEvent.WaitOne();
                        break;
                    }
                case PBEPkmnFaintedPacket pfp:
                    {
                        var hiddenId = new PBEPkmnFaintedPacket(byte.MaxValue, pfp.PokemonPosition, pfp.PokemonTeam);
                        spectatorPackets.Add(hiddenId);
                        Player teamOwner = battlers[pfp.PokemonTeam.Id];
                        teamOwner.Send(pfp);
                        teamOwner.ResetEvent.WaitOne();
                        foreach (Player player in readyToReceivePackets.Except(new[] { teamOwner }))
                        {
                            player.Send(hiddenId);
                            player.ResetEvent.WaitOne();
                        }
                        break;
                    }
                case PBEPkmnHPChangedPacket phcp:
                    {
                        var hiddenId = new PBEPkmnHPChangedPacket(phcp.Pokemon, phcp.PokemonTeam, 0, 0, phcp.OldHPPercentage, phcp.NewHPPercentage);
                        spectatorPackets.Add(hiddenId);
                        Player teamOwner = battlers[phcp.PokemonTeam.Id];
                        teamOwner.Send(phcp);
                        teamOwner.ResetEvent.WaitOne();
                        foreach (Player player in readyToReceivePackets.Except(new[] { teamOwner }))
                        {
                            player.Send(hiddenId);
                            player.ResetEvent.WaitOne();
                        }
                        break;
                    }
                case PBEPkmnSwitchInPacket psip:
                    {
                        var hiddenId = new PBEPkmnSwitchInPacket(psip.Team, psip.SwitchIns.Select(s => new PBEPkmnSwitchInPacket.PBESwitchInInfo(byte.MaxValue, byte.MaxValue, s.Species, s.Nickname, s.Level, s.Shiny, s.Gender, 0, 0, s.HPPercentage, s.Status1, s.FieldPosition)), psip.Forced);
                        spectatorPackets.Add(hiddenId);
                        Player teamOwner = battlers[psip.Team.Id];
                        teamOwner.Send(psip);
                        teamOwner.ResetEvent.WaitOne();
                        foreach (Player player in readyToReceivePackets.Except(new[] { teamOwner }))
                        {
                            player.Send(hiddenId);
                            player.ResetEvent.WaitOne();
                        }
                        break;
                    }
                case PBEPkmnSwitchOutPacket psop:
                    {
                        var hiddenId = new PBEPkmnSwitchOutPacket(byte.MaxValue, psop.PokemonPosition, psop.PokemonTeam, psop.Forced);
                        spectatorPackets.Add(hiddenId);
                        Player teamOwner = battlers[psop.PokemonTeam.Id];
                        teamOwner.Send(psop);
                        teamOwner.ResetEvent.WaitOne();
                        foreach (Player player in readyToReceivePackets.Except(new[] { teamOwner }))
                        {
                            player.Send(hiddenId);
                            player.ResetEvent.WaitOne();
                        }
                        break;
                    }
                case PBETransformPacket tp:
                    {
                        if (tp.UserTeam.Id == 0 || tp.TargetTeam.Id == 0)
                        {
                            battlers[0].Send(tp);
                            battlers[0].ResetEvent.WaitOne();
                        }
                        if (tp.UserTeam.Id == 1 || tp.TargetTeam.Id == 1)
                        {
                            battlers[1].Send(tp);
                            battlers[1].ResetEvent.WaitOne();
                        }
                        break;
                    }
                default:
                    {
                        spectatorPackets.Add(packet);
                        foreach (Player player in readyToReceivePackets)
                        {
                            player.Send(packet);
                            player.ResetEvent.WaitOne();
                        }
                        break;
                    }
            }
        }
    }
}
