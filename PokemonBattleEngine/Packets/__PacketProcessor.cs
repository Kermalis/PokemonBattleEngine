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
                case PRequestActionsPacket.Code: packet = new PRequestActionsPacket(buffer); break;
                case PSubmitActionsPacket.Code: packet = new PSubmitActionsPacket(buffer); break;
                case PMoveUsedPacket.Code: packet = new PMoveUsedPacket(buffer); break;
                case PPkmnHPChangedPacket.Code: packet = new PPkmnHPChangedPacket(buffer); break;
                case PMoveEffectivenessPacket.Code: packet = new PMoveEffectivenessPacket(buffer); break;
                case PPkmnSwitchOutPacket.Code: packet = new PPkmnSwitchOutPacket(buffer); break;
                case PMoveMissedPacket.Code: packet = new PMoveMissedPacket(buffer); break;
                case PPkmnFaintedPacket.Code: packet = new PPkmnFaintedPacket(buffer); break;
                case PMoveCritPacket.Code: packet = new PMoveCritPacket(buffer); break;
                case PPkmnStatChangedPacket.Code: packet = new PPkmnStatChangedPacket(buffer); break;
                case PStatus1Packet.Code: packet = new PStatus1Packet(buffer); break;
                case PStatus2Packet.Code: packet = new PStatus2Packet(buffer); break;
                case PTeamStatusPacket.Code: packet = new PTeamStatusPacket(buffer); break;
                case PWeatherPacket.Code: packet = new PWeatherPacket(buffer); break;
                case PMoveFailedPacket.Code: packet = new PMoveFailedPacket(buffer); break;
                case PItemUsedPacket.Code: packet = new PItemUsedPacket(buffer); break;
                case PMovePPChangedPacket.Code: packet = new PMovePPChangedPacket(buffer); break;
                case PTransformPacket.Code: packet = new PTransformPacket(buffer); break;
                case PLimberPacket.Code: packet = new PLimberPacket(buffer); break;
                case PMagnitudePacket.Code: packet = new PMagnitudePacket(buffer); break;
                //case PStatus2ActivatedPacket.Code: packet = new PStatus2ActivatedPacket(buffer); break;
                //case PStatus2CausedDamagePacket.Code: packet = new PStatus2CausedDamagePacket(buffer); break;
                default: throw new ArgumentException($"Invalid packet code: {code}");
            }

            return packet;
        }
    }
}
