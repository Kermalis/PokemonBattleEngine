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
        static readonly IPacketProcessor packetProcessor = new PacketProcessor();
        protected override IPacketProcessor PacketProcessor => packetProcessor;

        public Battle battle { get; private set; }

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
            Console.WriteLine("Server is online");
        }
        protected override void OnClientConnected(Player connection)
        {
            Console.WriteLine($"Client connected ({connection.Id})");
            if (Clients.Count() == Configuration.MaximumNumberOfConnections)
            {
                SendReadyPackets();
            }
        }
        protected override void OnClientDisconnected(Player connection)
        {
            Console.WriteLine($"Client disconnected ({connection.Id})");
        }
        protected override void OnError(Exception exception)
        {
            Console.WriteLine($"Server error: {exception}");
        }

        void SendReadyPackets()
        {
            Console.WriteLine("Game starting...");
            using (var packet = new ReadyUpPacket())
                SendToAll(packet);
        }
    }
}
