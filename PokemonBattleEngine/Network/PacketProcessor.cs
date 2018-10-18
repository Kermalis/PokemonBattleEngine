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
                case PResponsePacket.Code: packet = new PResponsePacket(buffer); break;
                case PPlayerJoinedPacket.Code: packet = new PPlayerJoinedPacket(buffer); break;
                case PReadyUpPacket.Code: packet = new PReadyUpPacket(buffer); break;
                case PMatchCancelledPacket.Code: packet = new PMatchCancelledPacket(buffer); break;
                case PRequestTeamPacket.Code: packet = new PRequestTeamPacket(buffer); break;
                case PSendPartyPacket.Code: packet = new PSendPartyPacket(buffer); break;
                case PSwitchInPacket.Code: packet = new PSwitchInPacket(buffer); break;
                case PUsedMovePacket.Code: packet = new PUsedMovePacket(buffer); break;
                default: throw new ArgumentException("Invalid packet code");
            }

            return packet;
        }
    }
}
