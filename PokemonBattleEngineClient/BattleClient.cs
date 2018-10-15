using System;
using System.Net.Sockets;
using Ether.Network.Client;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Packets;

namespace Kermalis.PokemonBattleEngineClient
{
    class BattleClient : NetClient
    {
        static readonly IPacketProcessor packetProcessor = new PacketProcessor();
        protected override IPacketProcessor PacketProcessor => packetProcessor;

        public BattleClient(string host)
        {
            Configuration.Host = host;
            Configuration.Port = 8888;
            Configuration.BufferSize = 1024;
        }

        public override void HandleMessage(INetPacketStream packet)
        {
            Console.WriteLine("Response received: Message: \"{0}\"", packet);

            if (packet is ReadyUpPacket)
            {

            }
        }

        protected override void OnConnected()
        {
            Console.WriteLine("Connected to {0}", Socket.RemoteEndPoint);
        }
        protected override void OnDisconnected()
        {
            Console.WriteLine("Disconnected from server");
        }
        protected override void OnSocketError(SocketError socketError)
        {
            Console.WriteLine("Socket Error: {0}", socketError);
        }
    }
}
