using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPacketProcessor : IPacketProcessor
    {
        public int HeaderSize => 2;
        public bool IncludeHeader => false;
        readonly PBESettings settings;

        public PBEPacketProcessor(PBESettings settings)
        {
            this.settings = settings;
        }

        public int GetMessageLength(byte[] buffer) => BitConverter.ToInt16(buffer, 0);
        public INetPacket CreatePacket(byte[] buffer)
        {
            INetPacket packet;
            short code = BitConverter.ToInt16(buffer, 0);
            switch (code)
            {
                case PBEResponsePacket.Code: packet = new PBEResponsePacket(buffer); break;
                case PBEPlayerJoinedPacket.Code: packet = new PBEPlayerJoinedPacket(buffer); break;
                case PBEMatchCancelledPacket.Code: packet = new PBEMatchCancelledPacket(buffer); break;
                case PBEPartyRequestPacket.Code: packet = new PBEPartyRequestPacket(buffer); break;
                case PBEPartyResponsePacket.Code: packet = new PBEPartyResponsePacket(buffer, settings); break;
                case PBESetPartyPacket.Code: packet = new PBESetPartyPacket(buffer, settings); break;
                case PBEPkmnSwitchInPacket.Code: packet = new PBEPkmnSwitchInPacket(buffer); break;
                case PBEActionsRequestPacket.Code: packet = new PBEActionsRequestPacket(buffer); break;
                case PBEActionsResponsePacket.Code: packet = new PBEActionsResponsePacket(buffer); break;
                case PBEMoveUsedPacket.Code: packet = new PBEMoveUsedPacket(buffer); break;
                case PBEPkmnHPChangedPacket.Code: packet = new PBEPkmnHPChangedPacket(buffer); break;
                case PBEMoveEffectivenessPacket.Code: packet = new PBEMoveEffectivenessPacket(buffer); break;
                case PBEPkmnSwitchOutPacket.Code: packet = new PBEPkmnSwitchOutPacket(buffer); break;
                case PBEMoveMissedPacket.Code: packet = new PBEMoveMissedPacket(buffer); break;
                case PBEPkmnFaintedPacket.Code: packet = new PBEPkmnFaintedPacket(buffer); break;
                case PBEMoveCritPacket.Code: packet = new PBEMoveCritPacket(buffer); break;
                case PBEPkmnStatChangedPacket.Code: packet = new PBEPkmnStatChangedPacket(buffer); break;
                case PBEStatus1Packet.Code: packet = new PBEStatus1Packet(buffer); break;
                case PBEStatus2Packet.Code: packet = new PBEStatus2Packet(buffer); break;
                case PBETeamStatusPacket.Code: packet = new PBETeamStatusPacket(buffer); break;
                case PBEWeatherPacket.Code: packet = new PBEWeatherPacket(buffer); break;
                case PBEMoveFailedPacket.Code: packet = new PBEMoveFailedPacket(buffer); break;
                case PBEItemPacket.Code: packet = new PBEItemPacket(buffer); break;
                case PBEMovePPChangedPacket.Code: packet = new PBEMovePPChangedPacket(buffer); break;
                case PBETransformPacket.Code: packet = new PBETransformPacket(buffer, settings); break;
                case PBEAbilityPacket.Code: packet = new PBEAbilityPacket(buffer); break;
                case PBEMagnitudePacket.Code: packet = new PBEMagnitudePacket(buffer); break;
                case PBEPainSplitPacket.Code: packet = new PBEPainSplitPacket(buffer); break;
                case PBEPsychUpPacket.Code: packet = new PBEPsychUpPacket(buffer); break;
                case PBESwitchInRequestPacket.Code: packet = new PBESwitchInRequestPacket(buffer); break;
                case PBESwitchInResponsePacket.Code: packet = new PBESwitchInResponsePacket(buffer); break;
                default: throw new ArgumentException($"Invalid packet code: {code}");
            }
            return packet;
        }
    }
}
