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
                case PMatchCancelledPacket.Code: packet = new PMatchCancelledPacket(buffer); break;
                case PRequestPartyPacket.Code: packet = new PRequestPartyPacket(buffer); break;
                case PSubmitPartyPacket.Code: packet = new PSubmitPartyPacket(buffer); break;
                case PSetPartyPacket.Code: packet = new PSetPartyPacket(buffer); break;
                case PPkmnSwitchInPacket.Code: packet = new PPkmnSwitchInPacket(buffer); break;
                case PRequestActionPacket.Code: packet = new PRequestActionPacket(buffer); break;
                case PSubmitActionsPacket.Code: packet = new PSubmitActionsPacket(buffer); break;
                case PPkmnMovePacket.Code: packet = new PPkmnMovePacket(buffer); break;
                case PPkmnDamagedPacket.Code: packet = new PPkmnDamagedPacket(buffer); break;
                case PAtkEffectivenessPacket.Code: packet = new PAtkEffectivenessPacket(buffer); break;
                default: throw new ArgumentException("Invalid packet code");
            }

            return packet;
        }
    }
}
