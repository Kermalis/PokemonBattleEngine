using Ether.Network.Packets;
using System;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PacketProcessor : IPacketProcessor
    {
        public int HeaderSize => 4;
        public bool IncludeHeader => false;

        public int GetMessageLength(byte[] buffer)
        {
            int len = BitConverter.ToInt32(buffer, 0);
            Console.WriteLine(len);
            return len;
        }
        public INetPacketStream CreatePacket(byte[] buffer)
        {
            int code = BitConverter.ToInt32(buffer, 0);

            INetPacketStream packet;
            switch (code)
            {
                case ReadyUpPacket.Code: packet = new ReadyUpPacket(buffer); break;
                default: throw new ArgumentException("Invalid packet code");
            }

            return packet;
        }
    }
}
