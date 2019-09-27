using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public sealed class PBEPacketProcessor : IPacketProcessor
    {
        private readonly PBEBattle _battle;

        public PBEPacketProcessor(PBEBattle battle)
        {
            _battle = battle;
        }

        /// <inheritdoc />
        public INetPacket CreatePacket(byte[] bytes)
        {
            INetPacket packet;
            using (var r = new BinaryReader(new MemoryStream(bytes)))
            {
                var list = new List<byte>(bytes);
                list.InsertRange(0, BitConverter.GetBytes((short)bytes.Length));
                var buffer = new ReadOnlyCollection<byte>(list);
                short code = r.ReadInt16();
                switch (code)
                {
                    case PBEResponsePacket.Code: packet = new PBEResponsePacket(buffer, r, _battle); break;
                    case PBEPlayerJoinedPacket.Code: packet = new PBEPlayerJoinedPacket(buffer, r, _battle); break;
                    case PBEMatchCancelledPacket.Code: packet = new PBEMatchCancelledPacket(buffer, r, _battle); break;
                    case PBEPartyRequestPacket.Code: packet = new PBEPartyRequestPacket(buffer, r, _battle); break;
                    case PBEPartyResponsePacket.Code: packet = new PBEPartyResponsePacket(buffer, r, _battle); break;
                    case PBETeamPacket.Code: packet = new PBETeamPacket(buffer, r, _battle); break;
                    case PBEPkmnSwitchInPacket.Code: packet = new PBEPkmnSwitchInPacket(buffer, r, _battle); break;
                    case PBEActionsRequestPacket.Code: packet = new PBEActionsRequestPacket(buffer, r, _battle); break;
                    case PBEActionsResponsePacket.Code: packet = new PBEActionsResponsePacket(buffer, r, _battle); break;
                    case PBEMoveUsedPacket.Code: packet = new PBEMoveUsedPacket(buffer, r, _battle); break;
                    case PBEPkmnHPChangedPacket.Code: packet = new PBEPkmnHPChangedPacket(buffer, r, _battle); break;
                    case PBEHazePacket.Code: packet = new PBEHazePacket(buffer, r, _battle); break;
                    case PBEPkmnSwitchOutPacket.Code: packet = new PBEPkmnSwitchOutPacket(buffer, r, _battle); break;
                    case PBEMoveMissedPacket.Code: packet = new PBEMoveMissedPacket(buffer, r, _battle); break;
                    case PBEPkmnFaintedPacket.Code: packet = new PBEPkmnFaintedPacket(buffer, r, _battle); break;
                    case PBEMoveCritPacket.Code: packet = new PBEMoveCritPacket(buffer, r, _battle); break;
                    case PBEPkmnStatChangedPacket.Code: packet = new PBEPkmnStatChangedPacket(buffer, r, _battle); break;
                    case PBEStatus1Packet.Code: packet = new PBEStatus1Packet(buffer, r, _battle); break;
                    case PBEStatus2Packet.Code: packet = new PBEStatus2Packet(buffer, r, _battle); break;
                    case PBETeamStatusPacket.Code: packet = new PBETeamStatusPacket(buffer, r, _battle); break;
                    case PBEWeatherPacket.Code: packet = new PBEWeatherPacket(buffer, r, _battle); break;
                    case PBEMoveResultPacket.Code: packet = new PBEMoveResultPacket(buffer, r, _battle); break;
                    case PBEItemPacket.Code: packet = new PBEItemPacket(buffer, r, _battle); break;
                    case PBEMovePPChangedPacket.Code: packet = new PBEMovePPChangedPacket(buffer, r, _battle); break;
                    case PBETransformPacket.Code: packet = new PBETransformPacket(buffer, r, _battle); break;
                    case PBEAbilityPacket.Code: packet = new PBEAbilityPacket(buffer, r, _battle); break;
                    case PBESpecialMessagePacket.Code: packet = new PBESpecialMessagePacket(buffer, r, _battle); break;
                    case PBEBattleStatusPacket.Code: packet = new PBEBattleStatusPacket(buffer, r, _battle); break;
                    case PBEPsychUpPacket.Code: packet = new PBEPsychUpPacket(buffer, r, _battle); break;
                    case PBESwitchInRequestPacket.Code: packet = new PBESwitchInRequestPacket(buffer, r, _battle); break;
                    case PBESwitchInResponsePacket.Code: packet = new PBESwitchInResponsePacket(buffer, r, _battle); break;
                    case PBEIllusionPacket.Code: packet = new PBEIllusionPacket(buffer, r, _battle); break;
                    case PBEWinnerPacket.Code: packet = new PBEWinnerPacket(buffer, r, _battle); break;
                    case PBETurnBeganPacket.Code: packet = new PBETurnBeganPacket(buffer, r, _battle); break;
                    case PBEMoveLockPacket.Code: packet = new PBEMoveLockPacket(buffer, r, _battle); break;
                    case PBEPkmnFormChangedPacket.Code: packet = new PBEPkmnFormChangedPacket(buffer, r, _battle); break;
                    case PBEAutoCenterPacket.Code: packet = new PBEAutoCenterPacket(buffer, r, _battle); break;
                    case PBETypeChangedPacket.Code: packet = new PBETypeChangedPacket(buffer, r, _battle); break;
                    default: throw new ArgumentOutOfRangeException(nameof(code));
                }
            }
            return packet;
        }
    }
}
