using Ether.Network.Packets;
using System;

namespace Kermalis.PokemonBattleEngine.Network
{
    public sealed class PPacketProcessor : IPacketProcessor
    {
        public int HeaderSize => 4;
        public bool IncludeHeader => false;

        public int GetMessageLength(byte[] buffer) => BitConverter.ToInt32(buffer, 0);
        public INetPacketStream CreatePacket(byte[] buffer)
        {
            int code = BitConverter.ToInt32(buffer, 0);

            INetPacketStream packet;
            switch (code)
            {
                case PReadyUpPacket.Code: packet = new PReadyUpPacket(buffer); break;
                case PRequestTeamPacket.Code: packet = new PRequestTeamPacket(buffer); break;
                case PMatchCancelledPacket.Code: packet = new PMatchCancelledPacket(buffer); break;
                default: throw new ArgumentException("Invalid packet code");
            }

            return packet;
        }
    }
}
