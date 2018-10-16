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
        }
        ServerState state = ServerState.Startup;
        Player[] battlers;
        public PBattle battle { get; private set; }

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

            foreach (var c in Clients)
                DisconnectClient(c.Id);
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
                    PPokemonShell.ValidateMany(battlers[0].Team.Pokemon.Concat(battlers[1].Team.Pokemon));
                }
                catch
                {
                    Console.WriteLine("Invalid Pokémon data!");
                    CancelMatch();
                    return;
                }
                Console.WriteLine("Both players ready!");
            }
        }

        static readonly IPacketProcessor packetProcessor = new PPacketProcessor();
        protected override IPacketProcessor PacketProcessor => packetProcessor;
        public BattleServer(string host)
        {
            Configuration.Backlog = 50;
            Configuration.Host = host;
            Configuration.Port = 8888;
            Configuration.MaximumNumberOfConnections = 2;
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
