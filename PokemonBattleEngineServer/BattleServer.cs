using Ether.Network.Packets;
using Ether.Network.Server;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Linq;

namespace Kermalis.PokemonBattleEngineServer
{
    class BattleServer : NetServer<Player>
    {
        private enum State
        {
            WaitingForPlayers, // Server is waiting for 2 players to connect
            WaitingForTeams, // Server is waiting for both players to send their teams
            Resetting, // Server is currently resetting the game
        }
        State state;
        Player[] battlers;
        public Battle battle { get; private set; }
        
        void StopBattleAndReset()
        {
            if (state == State.Resetting)
                return;
            state = State.Resetting;

            foreach (var c in Clients)
                DisconnectClient(c.Id);
            battlers = null;

            state = State.WaitingForPlayers;
        }
        void PlayersFound()
        {
            Console.WriteLine("Two players connected!");

            battlers = new Player[] { Clients.ElementAt(0), Clients.ElementAt(1) };
            using (var packet = new ReadyUpPacket())
                SendTo(battlers, packet);

            Console.WriteLine("Waiting for teams...");
            state = State.WaitingForTeams;
        }
        public void TeamUpdated(Player player)
        {
            if (state != State.WaitingForTeams)
                return;

            if(battlers[0].Team != null && battlers[1].Team != null)
            {
                Console.WriteLine("Both players ready!");
            }
        }

        static readonly IPacketProcessor packetProcessor = new PacketProcessor();
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
            StopBattleAndReset();
        }
        protected override void OnClientConnected(Player client)
        {
            Console.WriteLine($"Client connected ({client.Id})");
            if (state == State.WaitingForPlayers && Clients.Count() == 2)
                PlayersFound();
        }
        protected override void OnClientDisconnected(Player client)
        {
            Console.WriteLine($"Client disconnected ({client.Id})");
            StopBattleAndReset();
        }
        protected override void OnError(Exception e)
        {
            Console.WriteLine($"Server error: {e}");
        }
    }
}
