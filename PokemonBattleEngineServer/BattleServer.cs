using Ether.Network.Packets;
using Ether.Network.Server;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Network;
using System;
using System.Linq;

namespace Kermalis.PokemonBattleEngineServer
{
    class BattleServer : NetServer<Player>
    {
        private enum ServerState
        {
            Startup, // Server is starting up
            Resetting, // Server is currently resetting the game
            WaitingForPlayers, // Server is waiting for 2 players to connect
            WaitingForTeams, // Server is waiting for both players to send their teams
            StartingMatch, // Server is starting the battle
            WaitingForMoves, // Server is waiting for players to select moves
        }
        ServerState state = ServerState.Startup;
        Player[] battlers;
        PBattle battle;

        public void Forfeit(Player player)
        {
            // TODO
            CancelMatch(); // Temporary
        }
        void CancelMatch()
        {
            Console.WriteLine("Cancelling match...");
            SendToAll(new PMatchCancelledPacket());
            StopMatchAndReset();
        }
        void StopMatchAndReset()
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
        void PlayersFound()
        {
            Console.WriteLine("Two players connected!");

            battlers = new Player[] { Clients.ElementAt(0), Clients.ElementAt(1) };
            SendTo(battlers, new PReadyUpPacket());

            Console.WriteLine("Waiting for teams...");
            state = ServerState.WaitingForTeams;
        }
        public void TeamUpdated(Player player, PTeamShell team)
        {
            if (state != ServerState.WaitingForTeams)
                return;
            player.Team = team;

            if (battlers[0].Team != null && battlers[1].Team != null)
            {
                try
                {
                    PPokemonShell.ValidateMany(battlers[0].Team.Party.Concat(battlers[1].Team.Party));
                }
                catch
                {
                    Console.WriteLine("Invalid Pokémon data received!");
                    CancelMatch();
                    return;
                }
                Console.WriteLine("Both players ready!");
                StartMatch();
            }
        }
        void StartMatch()
        {
            if (state == ServerState.StartingMatch)
                return;
            state = ServerState.StartingMatch;

            Console.WriteLine("Battle starting!");

            battle = new PBattle(battlers[0].Team, battlers[1].Team);
            battle.NewEvent += BattleEventHandler;
            battle.NewEvent += PBattle.ConsoleBattleEventHandler;

            // Send opponent names
            battlers[0].Send(new PPlayerJoinedPacket(battlers[1].Id, battlers[1].Team.DisplayName));
            battlers[1].Send(new PPlayerJoinedPacket(battlers[0].Id, battlers[0].Team.DisplayName));
            WaitForBattlersResponses();
            // Send players their parties
            battlers[0].Send(new PSendPartyPacket(PKnownInfo.Instance.LocalParty));
            battlers[1].Send(new PSendPartyPacket(PKnownInfo.Instance.RemoteParty));
            WaitForBattlersResponses();

            battle.Start();
            WaitForBattlersResponses(); // Wait for switch-ins to be received

            state = ServerState.WaitingForMoves;
        }
        
        void WaitForBattlersResponses()
        {
            battlers[0].ResetEvent.WaitOne();
            battlers[1].ResetEvent.WaitOne();
        }
        void BattleEventHandler(INetPacketStream packet)
        {
            foreach (Player client in Clients)
            {
                switch (packet)
                {
                    case PSwitchInPacket sip:
                        if (client == battlers[1])
                            sip.LocallyOwned = !sip.LocallyOwned; // Correctly set locally owned for this team
                        break;
                }
                
                client.Send(packet);
            }
        }

        static readonly IPacketProcessor packetProcessor = new PPacketProcessor();
        protected override IPacketProcessor PacketProcessor => packetProcessor;
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
            // TODO: If both players connect while the server is resetting
            if (state == ServerState.WaitingForPlayers && Clients.Count() == 2)
                PlayersFound();
        }
        protected override void OnClientDisconnected(Player client)
        {
            Console.WriteLine($"Client disconnected ({client.Id})");
            switch (state)
            {
                case ServerState.Startup:
                case ServerState.Resetting:
                    break;
                case ServerState.WaitingForPlayers:
                case ServerState.WaitingForTeams:
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
