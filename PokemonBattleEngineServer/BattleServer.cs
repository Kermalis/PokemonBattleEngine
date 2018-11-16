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
        }
        ServerState state = ServerState.Startup;
        readonly PBattleStyle intendedBattleStyle = PBattleStyle.Triple; // TODO: Let the client know what kind of style this server is running (matchmaking)
        PBattle battle;
        Player[] battlers;

        public void Forfeit(Player player)
        {
            // TODO
            CancelMatch(); // Temporary
        }
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

                battlers = new Player[] { Clients.ElementAt(0), Clients.ElementAt(1) };
                SendTo(battlers, new PRequestPartyPacket());

                Console.WriteLine("Waiting for teams...");
                state = ServerState.WaitingForParties;
            }
        }
        public void PartySubmitted(Player player, PTeamShell team)
        {
            if (state != ServerState.WaitingForParties)
                return;
            lock (this)
            {
                if (state != ServerState.WaitingForParties)
                    return;

                player.Team = team;

                if (battlers[0].Team != null && battlers[1].Team != null)
                {
                    // Temporary:
                    if (false)
                    {
                        try
                        {
                            PTeamShell.ValidateMany(battlers.Select(b => b.Team));
                        }
                        catch
                        {
                            Console.WriteLine("Invalid Team data received!");
                            CancelMatch();
                            return;
                        }
                    }
                    //
                    Console.WriteLine("Both players ready!");
                    StartMatch();
                }
            }
        }
        void StartMatch()
        {
            if (state == ServerState.StartingMatch)
                return;
            lock (this)
            {
                if (state == ServerState.StartingMatch)
                    return;
                state = ServerState.StartingMatch;

                Console.WriteLine("Battle starting!");

                battle = new PBattle(intendedBattleStyle, battlers[0].Team, battlers[1].Team);
                battle.OnNewEvent += PBattle.ConsoleBattleEventHandler;
                battle.OnNewEvent += BattleEventHandler;

                // Send opponent names
                battlers[0].Send(new PPlayerJoinedPacket(battlers[1].Id, battlers[1].Team.DisplayName));
                battlers[1].Send(new PPlayerJoinedPacket(battlers[0].Id, battlers[0].Team.DisplayName));
                WaitForBattlersResponses();
                // Send players their parties
                battlers[0].Send(new PSetPartyPacket(PKnownInfo.Instance.LocalParty));
                battlers[1].Send(new PSetPartyPacket(PKnownInfo.Instance.RemoteParty));
                WaitForBattlersResponses();

                state = ServerState.BattleProcessing;
                battle.Start();
                SendTo(battlers, new PRequestActionsPacket());
                Console.WriteLine("Waiting for actions...");
                state = ServerState.WaitingForActions;
            }
        }
        public void ActionsSubmitted(Player player, PAction[] actions)
        {
            if (state != ServerState.WaitingForActions)
                return;
            lock (this)
            {
                if (state != ServerState.WaitingForActions)
                    return;

                Console.WriteLine($"Received actions from {player.Team.DisplayName}");

                bool valid = true;
                foreach (PAction action in actions)
                {
                    if (!battle.SelectActionIfValid(action))
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    Console.WriteLine("Actions are valid.");
                    if (battle.IsReadyToRunTurn())
                    {
                        Console.WriteLine("Players selected actions!");
                        state = ServerState.BattleProcessing;
                        battle.RunTurn();
                        if (battle.TemporaryKeepBattlingBool)
                        {
                            SendTo(battlers, new PRequestActionsPacket());
                            Console.WriteLine("Waiting for actions...");
                            state = ServerState.WaitingForActions;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Actions are invalid!");
                    player.Send(new PRequestActionsPacket());
                }
            }
        }

        void WaitForBattlersResponses()
        {
            battlers[0].ResetEvent.WaitOne();
            battlers[1].ResetEvent.WaitOne();
        }
        void BattleEventHandler(INetPacket packet)
        {
            switch (packet)
            {
                case PPkmnSwitchInPacket psip:
                    foreach (Player client in Clients)
                    {
                        if (client == battlers[1])
                            psip.Local = !psip.Local; // Correctly set locally owned for this team
                        client.Send(packet);
                    }
                    WaitForBattlersResponses();
                    break;
                case PReflectLightScreenPacket rlsp:
                    foreach (Player client in Clients)
                    {
                        if (client == battlers[1])
                            rlsp.Local = !rlsp.Local; // Correctly set locally owned for this team
                        client.Send(packet);
                    }
                    WaitForBattlersResponses();
                    break;
                case PMovePPChangedPacket mpcp:
                    // Send only to the owner's client
                    int i = PKnownInfo.Instance.Pokemon(mpcp.PokemonId).Local ? 0 : 1;
                    battlers[i].Send(mpcp);
                    battlers[i].ResetEvent.WaitOne(); // Wait for response
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
                    break;
                case ServerState.WaitingForPlayers:
                case ServerState.WaitingForParties:
                    CancelMatch();
                    break;
                default:
                    Forfeit(client);
                    break;
            }
        }
        protected override void OnError(Exception e)
        {
            Console.WriteLine($"Server error: {e}");
        }
    }
}
