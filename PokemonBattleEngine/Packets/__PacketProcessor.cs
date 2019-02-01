using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPacketProcessor : IPacketProcessor
    {
        public int HeaderSize => 2;
        public bool IncludeHeader => false;
        readonly PBEBattle battle;

        public PBEPacketProcessor(PBEBattle battle)
        {
            this.battle = battle;
        }

        public int GetMessageLength(byte[] buffer) => BitConverter.ToInt16(buffer, 0);
        public INetPacket CreatePacket(byte[] buffer)
        {
            INetPacket packet;
            short code = BitConverter.ToInt16(buffer, 0);
            switch (code)
            {
                case PBEResponsePacket.Code: packet = new PBEResponsePacket(buffer, battle); break;
                case PBEPlayerJoinedPacket.Code: packet = new PBEPlayerJoinedPacket(buffer, battle); break;
                case PBEMatchCancelledPacket.Code: packet = new PBEMatchCancelledPacket(buffer, battle); break;
                case PBEPartyRequestPacket.Code: packet = new PBEPartyRequestPacket(buffer, battle); break;
                case PBEPartyResponsePacket.Code: packet = new PBEPartyResponsePacket(buffer, battle); break;
                case PBESetPartyPacket.Code: packet = new PBESetPartyPacket(buffer, battle); break;
                case PBEPkmnSwitchInPacket.Code: packet = new PBEPkmnSwitchInPacket(buffer, battle); break;
                case PBEActionsRequestPacket.Code: packet = new PBEActionsRequestPacket(buffer, battle); break;
                case PBEActionsResponsePacket.Code: packet = new PBEActionsResponsePacket(buffer, battle); break;
                case PBEMoveUsedPacket.Code: packet = new PBEMoveUsedPacket(buffer, battle); break;
                case PBEPkmnHPChangedPacket.Code: packet = new PBEPkmnHPChangedPacket(buffer, battle); break;
                case PBEMoveEffectivenessPacket.Code: packet = new PBEMoveEffectivenessPacket(buffer, battle); break;
                case PBEPkmnSwitchOutPacket.Code: packet = new PBEPkmnSwitchOutPacket(buffer, battle); break;
                case PBEMoveMissedPacket.Code: packet = new PBEMoveMissedPacket(buffer, battle); break;
                case PBEPkmnFaintedPacket.Code: packet = new PBEPkmnFaintedPacket(buffer, battle); break;
                case PBEMoveCritPacket.Code: packet = new PBEMoveCritPacket(buffer, battle); break;
                case PBEPkmnStatChangedPacket.Code: packet = new PBEPkmnStatChangedPacket(buffer, battle); break;
                case PBEStatus1Packet.Code: packet = new PBEStatus1Packet(buffer, battle); break;
                case PBEStatus2Packet.Code: packet = new PBEStatus2Packet(buffer, battle); break;
                case PBETeamStatusPacket.Code: packet = new PBETeamStatusPacket(buffer, battle); break;
                case PBEWeatherPacket.Code: packet = new PBEWeatherPacket(buffer, battle); break;
                case PBEMoveFailedPacket.Code: packet = new PBEMoveFailedPacket(buffer, battle); break;
                case PBEItemPacket.Code: packet = new PBEItemPacket(buffer, battle); break;
                case PBEMovePPChangedPacket.Code: packet = new PBEMovePPChangedPacket(buffer, battle); break;
                case PBETransformPacket.Code: packet = new PBETransformPacket(buffer, battle); break;
                case PBEAbilityPacket.Code: packet = new PBEAbilityPacket(buffer, battle); break;
                case PBESpecialMessagePacket.Code: packet = new PBESpecialMessagePacket(buffer, battle); break;
                case PBEBattleStatusPacket.Code: packet = new PBEBattleStatusPacket(buffer, battle); break;
                case PBEPsychUpPacket.Code: packet = new PBEPsychUpPacket(buffer, battle); break;
                case PBESwitchInRequestPacket.Code: packet = new PBESwitchInRequestPacket(buffer, battle); break;
                case PBESwitchInResponsePacket.Code: packet = new PBESwitchInResponsePacket(buffer, battle); break;
                case PBEIllusionPacket.Code: packet = new PBEIllusionPacket(buffer, battle); break;
                case PBEWinnerPacket.Code: packet = new PBEWinnerPacket(buffer, battle); break;
                case PBETurnBeganPacket.Code: packet = new PBETurnBeganPacket(buffer, battle); break;
                default: throw new ArgumentException($"Invalid packet code: {code}");
            }
            return packet;
        }
    }
}
