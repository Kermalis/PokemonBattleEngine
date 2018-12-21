using Ether.Network.Packets;
using Ether.Network.Server;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Linq;

namespace Kermalis.PokemonBattleEngineServer
{
    sealed class BattleServer : NetServer<Player>
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
        PBEBattle battle;
        static readonly PBESettings settings = PBESettings.DefaultSettings;
        Player[] battlers;

        static readonly PBEPacketProcessor packetProcessor = new PBEPacketProcessor(settings);
        public override IPacketProcessor PacketProcessor => packetProcessor;
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
            Configuration.MaximumNumberOfConnections = 2; // Spectators allowed but not yet
            Configuration.BufferSize = 1024;
            Configuration.Blocking = true;
        }
        protected override void Initialize()
        {
            Console.WriteLine("Server online.");
            StopMatchAndReset();
        }
        protected override void OnClientConnected(Player client)
        {
            Console.WriteLine($"Client connected ({client.Id})");
            // Temporarily ignore spectators (no joined packets)
            if (battlers == null && Clients.Count() == 2)
            {
                Console.WriteLine("Two players connected!");
                state = ServerState.WaitingForParties;
                Console.WriteLine("Waiting for parties...");
                battlers = new Player[] { Clients.ElementAt(0), Clients.ElementAt(1) };
                battlers[0].IsSpectator = false;
                battlers[1].IsSpectator = false;
                SendTo(battlers, new PBEPartyRequestPacket());
            }
        }
        protected override void OnClientDisconnected(Player client)
        {
            Console.WriteLine($"Client disconnected ({client.Id})");
            // Temporarily ignore spectators
            if (battlers == null || !battlers.Contains(client))
            {
                return;
            }
            switch (state)
            {
                case ServerState.Startup:
                case ServerState.Resetting:
                case ServerState.Cancelling:
                    break;
                default:
                    CancelMatch();
                    break;
            }
        }
        protected override void OnError(Exception e)
        {
            Console.WriteLine($"Server error: {e}");
        }

        void CancelMatch()
        {
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
                state = ServerState.Resetting;
                foreach (Player c in Clients)
                {
                    c.ResetEvent.Close();
                    DisconnectClient(c.Id);
                }
                battlers = null;
                state = ServerState.WaitingForPlayers;
            }
        }
        public void PartySubmitted(Player player, PBETeamShell team)
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
                player.Shell = team;
                if (battlers[0].Shell != null && battlers[1].Shell != null)
                {
                    try
                    {
                        PBETeamShell.ValidateMany(battlers.Select(b => b.Shell), settings);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Invalid Team data received!");
                        CancelMatch();
                        return;
                    }

                    state = ServerState.StartingMatch;
                    Console.WriteLine("Battle starting!");

                    battle = new PBEBattle(PBEBattleFormat.Double, settings, battlers[0].Shell, battlers[1].Shell);
                    battle.OnNewEvent += PBEBattle.ConsoleBattleEventHandler;
                    battle.OnNewEvent += BattleEventHandler;
                    battle.OnStateChanged += BattleStateHandler;

                    // Send opponent names
                    battlers[0].Send(new PBEPlayerJoinedPacket(battlers[1].Id, battlers[1].Shell.PlayerName));
                    battlers[1].Send(new PBEPlayerJoinedPacket(battlers[0].Id, battlers[0].Shell.PlayerName));
                    WaitForBattlersResponses();
                    // Send players their parties
                    battlers[0].Send(new PBESetPartyPacket(battle.Teams[0].Party.ToArray(), settings));
                    battlers[1].Send(new PBESetPartyPacket(battle.Teams[1].Party.ToArray(), settings));
                    WaitForBattlersResponses();

                    battle.Begin();
                }
            }
        }
        public void ActionsSubmitted(Player player, PBEAction[] actions)
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
                Console.WriteLine($"Received actions from {player.Shell.PlayerName}!");
                bool local = player == battlers[0];
                if (!battle.SelectActionsIfValid(local, actions))
                {
                    Console.WriteLine("Actions are invalid!");
                    player.Send(new PBEActionsRequestPacket(true, battle.Teams[local ? 0 : 1].ActionsRequired));
                }
            }
        }
        public void SwitchesSubmitted(Player player, Tuple<byte, PBEFieldPosition>[] switches)
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
                Console.WriteLine($"Received switches from {player.Shell.PlayerName}!");
                bool local = player == battlers[0];
                if (!battle.SelectSwitchesIfValid(local, switches))
                {
                    Console.WriteLine("Switches are invalid!");
                    player.Send(new PBESwitchInRequestPacket(true, battle.Teams[local ? 0 : 1].SwitchInsRequired));
                }
            }
        }

        void WaitForBattlersResponses()
        {
            battlers[0].ResetEvent.WaitOne();
            battlers[1].ResetEvent.WaitOne();
        }
        void BattleStateHandler(PBEBattle battle)
        {
            Console.WriteLine("Battle state changed: {0}", battle.BattleState);
            switch (battle.BattleState)
            {
                case PBEBattleState.Processing:
                    state = ServerState.BattleProcessing;
                    break;
                case PBEBattleState.ReadyToRunTurn:
                    battle.RunTurn();
                    break;
                case PBEBattleState.WaitingForActions:
                    state = ServerState.WaitingForActions;
                    Console.WriteLine("Waiting for actions...");
                    break;
                case PBEBattleState.WaitingForSwitchIns:
                    state = ServerState.WaitingForSwitchIns;
                    Console.WriteLine("Waiting for switches...");
                    break;
            }
        }
        void BattleEventHandler(PBEBattle battle, INetPacket packet)
        {
            switch (packet)
            {
                case PBEActionsRequestPacket _:
                case PBEPkmnSwitchInPacket _:
                case PBESwitchInRequestPacket _:
                case PBETeamStatusPacket _:
                    // These packets get sent to all players, but the "LocalTeam" argument is changed for team index 1 so its client sees it as local.
                    dynamic pack = packet;
                    foreach (Player client in Clients)
                    {
                        if (client == battlers[1])
                        {
                            pack.LocalTeam = !pack.LocalTeam;
                        }
                        client.Send(packet);
                    }
                    WaitForBattlersResponses();
                    break;
                case PBETransformPacket tp:
                    {
                        PBEPokemon user = battle.GetPokemon(tp.UserId),
                            target = battle.GetPokemon(tp.TargetId);
                        if (user.LocalTeam || target.LocalTeam)
                        {
                            battlers[0].Send(tp);
                        }
                        if (!user.LocalTeam || !target.LocalTeam)
                        {
                            battlers[1].Send(tp);
                        }
                    }
                    break;
                default:
                    SendToAll(packet);
                    WaitForBattlersResponses();
                    break;
            }
        }

    }
}
