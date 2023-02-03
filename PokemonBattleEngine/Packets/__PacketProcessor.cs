using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

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

		using (var ms = new MemoryStream(data))
		{
			var r = new EndianBinaryReader(ms);
			ushort id = r.ReadUInt16();
			IPBEPacket? ret = TryCreatePacket(data, battle, r, id);
			if (ret is null)
			{
				throw new InvalidDataException($"Invalid packet ID ({id})");
			}
			return ret;
		}
	}

	public static EndianBinaryWriter WritePacketID(MemoryStream ms, ushort id)
	{
		var w = new EndianBinaryWriter(ms);
		w.WriteUInt16(id);
		return w;
	}

	protected virtual IPBEPacket? TryCreatePacket(byte[] data, PBEBattle? battle, EndianBinaryReader r, ushort id)
	{
		switch (id)
		{
			case PBEResponsePacket.ID: return new PBEResponsePacket(data);
			case PBEPlayerJoinedPacket.ID: return new PBEPlayerJoinedPacket(data, r);
			case PBEMatchCancelledPacket.ID: return new PBEMatchCancelledPacket(data);
			case PBEPartyRequestPacket.ID: return new PBEPartyRequestPacket(data, r);
			case PBEPartyResponsePacket.ID: return new PBEPartyResponsePacket(data, r);
			case PBEBattlePacket.ID: return new PBEBattlePacket(data, r);
			case PBEPkmnSwitchInPacket.ID: CheckNull(battle); return new PBEPkmnSwitchInPacket(data, r, battle);
			case PBEActionsRequestPacket.ID: CheckNull(battle); return new PBEActionsRequestPacket(data, r, battle);
			case PBEActionsResponsePacket.ID: return new PBEActionsResponsePacket(data, r);
			case PBEMoveUsedPacket.ID: CheckNull(battle); return new PBEMoveUsedPacket(data, r, battle);
			case PBEPkmnHPChangedPacket.ID: CheckNull(battle); return new PBEPkmnHPChangedPacket(data, r, battle);
			case PBEHazePacket.ID: return new PBEHazePacket(data);
			case PBEPkmnSwitchOutPacket.ID: CheckNull(battle); return new PBEPkmnSwitchOutPacket(data, r, battle);
			case PBEWildPkmnAppearedPacket.ID: return new PBEWildPkmnAppearedPacket(data, r);
			case PBEPkmnFaintedPacket.ID: CheckNull(battle); return new PBEPkmnFaintedPacket(data, r, battle);
			case PBEMoveCritPacket.ID: CheckNull(battle); return new PBEMoveCritPacket(data, r, battle);
			case PBEPkmnStatChangedPacket.ID: CheckNull(battle); return new PBEPkmnStatChangedPacket(data, r, battle);
			case PBEStatus1Packet.ID: CheckNull(battle); return new PBEStatus1Packet(data, r, battle);
			case PBEStatus2Packet.ID: CheckNull(battle); return new PBEStatus2Packet(data, r, battle);
			case PBETeamStatusPacket.ID: CheckNull(battle); return new PBETeamStatusPacket(data, r, battle);
			case PBEWeatherPacket.ID: CheckNull(battle); return new PBEWeatherPacket(data, r);
			case PBEMoveResultPacket.ID: CheckNull(battle); return new PBEMoveResultPacket(data, r, battle);
			case PBEItemPacket.ID: CheckNull(battle); return new PBEItemPacket(data, r, battle);
			case PBEMovePPChangedPacket.ID: CheckNull(battle); return new PBEMovePPChangedPacket(data, r, battle);
			case PBETransformPacket.ID: CheckNull(battle); return new PBETransformPacket(data, r, battle);
			case PBEAbilityPacket.ID: CheckNull(battle); return new PBEAbilityPacket(data, r, battle);
			case PBESpecialMessagePacket.ID: CheckNull(battle); return new PBESpecialMessagePacket(data, r, battle);
			case PBEBattleStatusPacket.ID: return new PBEBattleStatusPacket(data, r);
			case PBEPsychUpPacket.ID: CheckNull(battle); return new PBEPsychUpPacket(data, r, battle);
			case PBESwitchInRequestPacket.ID: CheckNull(battle); return new PBESwitchInRequestPacket(data, r, battle);
			case PBESwitchInResponsePacket.ID: return new PBESwitchInResponsePacket(data, r);
			case PBEIllusionPacket.ID: CheckNull(battle); return new PBEIllusionPacket(data, r, battle);
			case PBEBattleResultPacket.ID: return new PBEBattleResultPacket(data, r);
			case PBETurnBeganPacket.ID: return new PBETurnBeganPacket(data, r);
			case PBEMoveLockPacket.ID: CheckNull(battle); return new PBEMoveLockPacket(data, r, battle);
			case PBEPkmnFormChangedPacket.ID: CheckNull(battle); return new PBEPkmnFormChangedPacket(data, r, battle);
			case PBEAutoCenterPacket.ID: CheckNull(battle); return new PBEAutoCenterPacket(data, r, battle);
			case PBETypeChangedPacket.ID: CheckNull(battle); return new PBETypeChangedPacket(data, r, battle);
			case PBEAbilityReplacedPacket.ID: CheckNull(battle); return new PBEAbilityReplacedPacket(data, r, battle);
			case PBELegalPartyResponsePacket.ID: return new PBELegalPartyResponsePacket(data, r);
			case PBEReflectTypePacket.ID: CheckNull(battle); return new PBEReflectTypePacket(data, r, battle);
			case PBEPkmnFaintedPacket_Hidden.ID: CheckNull(battle); return new PBEPkmnFaintedPacket_Hidden(data, r, battle);
			case PBEAutoCenterPacket_Hidden0.ID: CheckNull(battle); return new PBEAutoCenterPacket_Hidden0(data, r, battle);
			case PBEAutoCenterPacket_Hidden1.ID: CheckNull(battle); return new PBEAutoCenterPacket_Hidden1(data, r, battle);
			case PBEAutoCenterPacket_Hidden01.ID: CheckNull(battle); return new PBEAutoCenterPacket_Hidden01(data, r, battle);
			case PBEReflectTypePacket_Hidden.ID: CheckNull(battle); return new PBEReflectTypePacket_Hidden(data, r, battle);
			case PBEPkmnFormChangedPacket_Hidden.ID: CheckNull(battle); return new PBEPkmnFormChangedPacket_Hidden(data, r, battle);
			case PBEPkmnHPChangedPacket_Hidden.ID: CheckNull(battle); return new PBEPkmnHPChangedPacket_Hidden(data, r, battle);
			case PBEPkmnSwitchInPacket_Hidden.ID: CheckNull(battle); return new PBEPkmnSwitchInPacket_Hidden(data, r, battle);
			case PBEPkmnSwitchOutPacket_Hidden.ID: CheckNull(battle); return new PBEPkmnSwitchOutPacket_Hidden(data, r, battle);
			case PBEFleeResponsePacket.ID: return new PBEFleeResponsePacket(data);
			case PBEFleeFailedPacket.ID: CheckNull(battle); return new PBEFleeFailedPacket(data, r, battle);
			case PBEItemTurnPacket.ID: CheckNull(battle); return new PBEItemTurnPacket(data, r, battle);
			case PBECapturePacket.ID: CheckNull(battle); return new PBECapturePacket(data, r, battle);
			case PBEWildPkmnAppearedPacket_Hidden.ID: return new PBEWildPkmnAppearedPacket_Hidden(data, r);
			case PBEPkmnEXPChangedPacket.ID: CheckNull(battle); return new PBEPkmnEXPChangedPacket(data, r, battle);
			case PBEPkmnEXPEarnedPacket.ID: CheckNull(battle); return new PBEPkmnEXPEarnedPacket(data, r, battle);
			case PBEPkmnLevelChangedPacket.ID: CheckNull(battle); return new PBEPkmnLevelChangedPacket(data, r, battle);
			case PBEWeatherDamagePacket.ID: CheckNull(battle); return new PBEWeatherDamagePacket(data, r, battle);
			case PBETeamStatusDamagePacket.ID: CheckNull(battle); return new PBETeamStatusDamagePacket(data, r, battle);
			default: return null;
		}
	}
}
