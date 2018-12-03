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
        readonly PBattleStyle intendedBattleStyle = PBattleStyle.Double; // TODO: Let the client know what kind of style this server is running (matchmaking)
        PBattle battle;
        Player[] battlers;

        void CancelMatch()
        {
            if (state == ServerState.Cancelling)
                return;
            lock (this)
            {
                if (state == ServerState.Cancelling)
                    return;

                state = ServerState.Cancelling;
                Console.WriteLine("Cancelling match...");
                SendToAll(new PMatchCancelledPacket());
                StopMatchAndReset();
            }
        }
        void StopMatchAndReset()
        {
            if (state == ServerState.Resetting)
                return;
            lock (this)
            {
                if (state == ServerState.Resetting)
                    return;

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
        void PlayersFound()
        {
            if (state != ServerState.WaitingForPlayers)
                return;
            lock (this)
            {
                if (state != ServerState.WaitingForPlayers)
                    return;

                Console.WriteLine("Two players connected!");
                state = ServerState.WaitingForParties;
                Console.WriteLine("Waiting for parties...");
                battlers = new Player[] { Clients.ElementAt(0), Clients.ElementAt(1) };
                SendTo(battlers, new PPartyRequestPacket());
            }
        }
        // TODO: Spectators are somewhat blocked, but waste server time
        public void PartySubmitted(Player player, PTeamShell team)
        {
            if (state != ServerState.WaitingForParties)
                return;
            lock (this)
            {
                if (state != ServerState.WaitingForParties)
                    return;

                player.Shell = team;

                if (battlers[0].Shell != null && battlers[1].Shell != null)
                {
                    try
                    {
                        PTeamShell.ValidateMany(battlers.Select(b => b.Shell));
                    }
                    catch
                    {
                        Console.WriteLine("Invalid Team data received!");
                        CancelMatch();
                        return;
                    }

                    state = ServerState.StartingMatch;
                    Console.WriteLine("Battle starting!");

                    battle = new PBattle(intendedBattleStyle, battlers[0].Shell, battlers[1].Shell);
                    battle.OnNewEvent += PBattle.ConsoleBattleEventHandler;
                    battle.OnNewEvent += BattleEventHandler;
                    battle.OnStateChanged += BattleStateHandler;

                    // Send opponent names
                    battlers[0].Send(new PPlayerJoinedPacket(battlers[1].Id, battlers[1].Shell.PlayerName));
                    battlers[1].Send(new PPlayerJoinedPacket(battlers[0].Id, battlers[0].Shell.PlayerName));
                    WaitForBattlersResponses();
                    // Send players their parties
                    battlers[0].Send(new PSetPartyPacket(battle.Teams[0].Party.ToArray()));
                    battlers[1].Send(new PSetPartyPacket(battle.Teams[1].Party.ToArray()));
                    WaitForBattlersResponses();

                    battle.Begin();
                }
            }
        }
        // TODO: Block spectators
        public void ActionsSubmitted(Player player, PAction[] actions)
        {
            if (state != ServerState.WaitingForActions)
                return;
            lock (this)
            {
                if (state != ServerState.WaitingForActions)
                    return;

                Console.WriteLine($"Received actions from {player.Shell.PlayerName}!");

                if (!battle.SelectActionsIfValid(actions))
                {
                    Console.WriteLine("Actions are invalid!");
                    player.Send(new PActionsRequestPacket());
                }
            }
        }
        // TODO: Block spectators
        public void SwitchesSubmitted(Player player, Tuple<byte, PFieldPosition>[] switches)
        {
            if (state != ServerState.WaitingForSwitchIns)
                return;
            lock (this)
            {
                if (state != ServerState.WaitingForSwitchIns)
                    return;

                Console.WriteLine($"Received switches from {player.Shell.PlayerName}!");

                bool local = player == battlers[0];
                if (!battle.SelectSwitchesIfValid(local, switches))
                {
                    Console.WriteLine("Switches are invalid!");
                    player.Send(new PSwitchInRequestPacket(local, battle.Teams[local ? 0 : 1].SwitchInsRequired));
                }
            }
        }

        void WaitForBattlersResponses()
        {
            battlers[0].ResetEvent.WaitOne();
            battlers[1].ResetEvent.WaitOne();
        }
        void BattleStateHandler(PBattle battle)
        {
            Console.WriteLine("Battle state changed: {0}", battle.BattleState);

            switch (battle.BattleState)
            {
                case PBattleState.Processing:
                    state = ServerState.BattleProcessing;
                    break;
                case PBattleState.ReadyToRunTurn:
                    Console.WriteLine("Turn is ready to run!");
                    battle.RunTurn();
                    break;
                case PBattleState.WaitingForActions:
                    state = ServerState.WaitingForActions;
                    Console.WriteLine("Waiting for actions...");
                    SendTo(battlers, new PActionsRequestPacket()); // TODO
                    break;
                case PBattleState.WaitingForSwitchIns:
                    state = ServerState.WaitingForSwitchIns;
                    Console.WriteLine("Waiting for switches...");
                    //SendTo(battlers, new PSwitchInRequestPacket()); // Gets broadcasted in the battle, like requestactions should
                    break;
            }
        }
        void BattleEventHandler(PBattle battle, INetPacket packet)
        {
            switch (packet)
            {
                case PPkmnSwitchInPacket psip:
                case PSwitchInRequestPacket sirp:
                case PTeamStatusPacket tsp:
                    dynamic pack = packet;
                    foreach (Player client in Clients)
                    {
                        if (client == battlers[1])
                            pack.Local = !pack.Local; // Correctly set locally owned for this team
                        client.Send(packet);
                    }
                    WaitForBattlersResponses();
                    break;
                default:
                    SendToAll(packet);
                    WaitForBattlersResponses();
                    break;
            }
        }

        static readonly IPacketProcessor packetProcessor = new PPacketProcessor();
        public override IPacketProcessor PacketProcessor => packetProcessor;
        public BattleServer(string host)
        {
            Configuration.Backlog = 50;
            Configuration.Host = host;
            Configuration.Port = 8888;
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
            // Temporarily ignore spectators
            if (Clients.Count() == 2)
                PlayersFound();
        }
        protected override void OnClientDisconnected(Player client)
        {
            Console.WriteLine($"Client disconnected ({client.Id})");
            // Temporarily ignore spectators
            if (battlers == null || !battlers.Contains(client))
                return;
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
    }
}
