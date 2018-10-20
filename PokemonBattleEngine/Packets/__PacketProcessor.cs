using Ether.Network.Packets;
using System;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PPacketProcessor : IPacketProcessor
    {
        public int HeaderSize => 2;
        public bool IncludeHeader => false;

        public int GetMessageLength(byte[] buffer) => BitConverter.ToInt16(buffer, 0);
        public INetPacket CreatePacket(byte[] buffer)
        {
            short code = BitConverter.ToInt16(buffer, 0);

            INetPacket packet;
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
                case PMoveEffectivenessPacket.Code: packet = new PMoveEffectivenessPacket(buffer); break;
                case PPkmnFlinchedPacket.Code: packet = new PPkmnFlinchedPacket(buffer); break;
                case PMoveMissedPacket.Code: packet = new PMoveMissedPacket(buffer); break;
                case PPkmnFaintedPacket.Code: packet = new PPkmnFaintedPacket(buffer); break;
                case PMoveCritPacket.Code: packet = new PMoveCritPacket(buffer); break;
                case PPkmnStatChangePacket.Code: packet = new PPkmnStatChangePacket(buffer); break;
                case PStatusChangePacket.Code: packet = new PStatusChangePacket(buffer); break;
                case PStatusEndedPacket.Code: packet = new PStatusEndedPacket(buffer); break;
                case PStatusCausedImmobilityPacket.Code: packet = new PStatusCausedImmobilityPacket(buffer); break;
                case PStatusCausedDamagePacket.Code: packet = new PStatusCausedDamagePacket(buffer); break;
                default: throw new ArgumentException($"Invalid packet code: {code}");
            }

            return packet;
        }
    }
}
