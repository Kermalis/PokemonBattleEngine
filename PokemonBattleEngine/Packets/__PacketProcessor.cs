using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets
{
    public class PBEPacketProcessor
    {
        protected static void CheckNull([NotNull] PBEBattle? battle)
        {
            if (battle is null)
            {
                throw new ArgumentNullException(nameof(battle));
            }
        }

        public IPBEPacket CreatePacket(byte[] data, PBEBattle? battle)
        {
            if (data.Length < 2)
            {
                throw new InvalidDataException();
            }
            using (var r = new EndianBinaryReader(new MemoryStream(data), encoding: EncodingType.UTF16))
            {
                ushort code = r.ReadUInt16();
                IPBEPacket? ret = TryCreatePacket(data, battle, r, code);
                if (ret is null)
                {
                    throw new InvalidDataException("Invalid packet code.");
                }
                return ret;
            }
        }

        protected virtual IPBEPacket? TryCreatePacket(byte[] data, PBEBattle? battle, EndianBinaryReader r, ushort code)
        {
            switch (code)
            {
                case PBEResponsePacket.Code: return new PBEResponsePacket(data);
                case PBEPlayerJoinedPacket.Code: return new PBEPlayerJoinedPacket(data, r);
                case PBEMatchCancelledPacket.Code: return new PBEMatchCancelledPacket(data);
                case PBEPartyRequestPacket.Code: return new PBEPartyRequestPacket(data, r);
                case PBEPartyResponsePacket.Code: return new PBEPartyResponsePacket(data, r);
                case PBEBattlePacket.Code: return new PBEBattlePacket(data, r);
                case PBEPkmnSwitchInPacket.Code: CheckNull(battle); return new PBEPkmnSwitchInPacket(data, r, battle);
                case PBEActionsRequestPacket.Code: CheckNull(battle); return new PBEActionsRequestPacket(data, r, battle);
                case PBEActionsResponsePacket.Code: return new PBEActionsResponsePacket(data, r);
                case PBEMoveUsedPacket.Code: CheckNull(battle); return new PBEMoveUsedPacket(data, r, battle);
                case PBEPkmnHPChangedPacket.Code: CheckNull(battle); return new PBEPkmnHPChangedPacket(data, r, battle);
                case PBEHazePacket.Code: return new PBEHazePacket(data);
                case PBEPkmnSwitchOutPacket.Code: CheckNull(battle); return new PBEPkmnSwitchOutPacket(data, r, battle);
                case PBEWildPkmnAppearedPacket.Code: return new PBEWildPkmnAppearedPacket(data, r);
                case PBEPkmnFaintedPacket.Code: CheckNull(battle); return new PBEPkmnFaintedPacket(data, r, battle);
                case PBEMoveCritPacket.Code: CheckNull(battle); return new PBEMoveCritPacket(data, r, battle);
                case PBEPkmnStatChangedPacket.Code: CheckNull(battle); return new PBEPkmnStatChangedPacket(data, r, battle);
                case PBEStatus1Packet.Code: CheckNull(battle); return new PBEStatus1Packet(data, r, battle);
                case PBEStatus2Packet.Code: CheckNull(battle); return new PBEStatus2Packet(data, r, battle);
                case PBETeamStatusPacket.Code: CheckNull(battle); return new PBETeamStatusPacket(data, r, battle);
                case PBEWeatherPacket.Code: CheckNull(battle); return new PBEWeatherPacket(data, r, battle);
                case PBEMoveResultPacket.Code: CheckNull(battle); return new PBEMoveResultPacket(data, r, battle);
                case PBEItemPacket.Code: CheckNull(battle); return new PBEItemPacket(data, r, battle);
                case PBEMovePPChangedPacket.Code: CheckNull(battle); return new PBEMovePPChangedPacket(data, r, battle);
                case PBETransformPacket.Code: CheckNull(battle); return new PBETransformPacket(data, r, battle);
                case PBEAbilityPacket.Code: CheckNull(battle); return new PBEAbilityPacket(data, r, battle);
                case PBESpecialMessagePacket.Code: CheckNull(battle); return new PBESpecialMessagePacket(data, r, battle);
                case PBEBattleStatusPacket.Code: return new PBEBattleStatusPacket(data, r);
                case PBEPsychUpPacket.Code: CheckNull(battle); return new PBEPsychUpPacket(data, r, battle);
                case PBESwitchInRequestPacket.Code: CheckNull(battle); return new PBESwitchInRequestPacket(data, r, battle);
                case PBESwitchInResponsePacket.Code: return new PBESwitchInResponsePacket(data, r);
                case PBEIllusionPacket.Code: CheckNull(battle); return new PBEIllusionPacket(data, r, battle);
                case PBEBattleResultPacket.Code: return new PBEBattleResultPacket(data, r);
                case PBETurnBeganPacket.Code: return new PBETurnBeganPacket(data, r);
                case PBEMoveLockPacket.Code: CheckNull(battle); return new PBEMoveLockPacket(data, r, battle);
                case PBEPkmnFormChangedPacket.Code: CheckNull(battle); return new PBEPkmnFormChangedPacket(data, r, battle);
                case PBEAutoCenterPacket.Code: CheckNull(battle); return new PBEAutoCenterPacket(data, r, battle);
                case PBETypeChangedPacket.Code: CheckNull(battle); return new PBETypeChangedPacket(data, r, battle);
                case PBEAbilityReplacedPacket.Code: CheckNull(battle); return new PBEAbilityReplacedPacket(data, r, battle);
                case PBELegalPartyResponsePacket.Code: return new PBELegalPartyResponsePacket(data, r);
                case PBEReflectTypePacket.Code: CheckNull(battle); return new PBEReflectTypePacket(data, r, battle);
                case PBEPkmnFaintedPacket_Hidden.Code: CheckNull(battle); return new PBEPkmnFaintedPacket_Hidden(data, r, battle);
                case PBEAutoCenterPacket_Hidden0.Code: CheckNull(battle); return new PBEAutoCenterPacket_Hidden0(data, r, battle);
                case PBEAutoCenterPacket_Hidden1.Code: CheckNull(battle); return new PBEAutoCenterPacket_Hidden1(data, r, battle);
                case PBEAutoCenterPacket_Hidden01.Code: CheckNull(battle); return new PBEAutoCenterPacket_Hidden01(data, r, battle);
                case PBEReflectTypePacket_Hidden.Code: CheckNull(battle); return new PBEReflectTypePacket_Hidden(data, r, battle);
                case PBEPkmnFormChangedPacket_Hidden.Code: CheckNull(battle); return new PBEPkmnFormChangedPacket_Hidden(data, r, battle);
                case PBEPkmnHPChangedPacket_Hidden.Code: CheckNull(battle); return new PBEPkmnHPChangedPacket_Hidden(data, r, battle);
                case PBEPkmnSwitchInPacket_Hidden.Code: CheckNull(battle); return new PBEPkmnSwitchInPacket_Hidden(data, r, battle);
                case PBEPkmnSwitchOutPacket_Hidden.Code: CheckNull(battle); return new PBEPkmnSwitchOutPacket_Hidden(data, r, battle);
                case PBEFleeResponsePacket.Code: return new PBEFleeResponsePacket(data);
                case PBEFleeFailedPacket.Code: CheckNull(battle); return new PBEFleeFailedPacket(data, r, battle);
                case PBEItemTurnPacket.Code: CheckNull(battle); return new PBEItemTurnPacket(data, r, battle);
                case PBECapturePacket.Code: CheckNull(battle); return new PBECapturePacket(data, r, battle);
                case PBEWildPkmnAppearedPacket_Hidden.Code: return new PBEWildPkmnAppearedPacket_Hidden(data, r);
                case PBEPkmnEXPChangedPacket.Code: CheckNull(battle); return new PBEPkmnEXPChangedPacket(data, r, battle);
                case PBEPkmnEXPEarnedPacket.Code: CheckNull(battle); return new PBEPkmnEXPEarnedPacket(data, r, battle);
                case PBEPkmnLevelChangedPacket.Code: CheckNull(battle); return new PBEPkmnLevelChangedPacket(data, r, battle);
                case PBEWeatherDamagePacket.Code: CheckNull(battle); return new PBEWeatherDamagePacket(data, r, battle);
                case PBETeamStatusDamagePacket.Code: CheckNull(battle); return new PBETeamStatusDamagePacket(data, r, battle);
                default: return null;
            }
        }
    }
}
