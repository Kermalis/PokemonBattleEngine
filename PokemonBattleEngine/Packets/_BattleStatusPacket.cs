using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEBattleStatusPacket : IPBEPacket
{
	public const ushort ID = 0x21;
	public ReadOnlyCollection<byte> Data { get; }

	public PBEBattleStatus BattleStatus { get; }
	public PBEBattleStatusAction BattleStatusAction { get; }

	internal PBEBattleStatusPacket(PBEBattleStatus battleStatus, PBEBattleStatusAction battleStatusAction)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteEnum(BattleStatus = battleStatus);
			w.WriteEnum(BattleStatusAction = battleStatusAction);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEBattleStatusPacket(byte[] data, EndianBinaryReader r)
	{
		Data = new ReadOnlyCollection<byte>(data);

		BattleStatus = r.ReadEnum<PBEBattleStatus>();
		BattleStatusAction = r.ReadEnum<PBEBattleStatusAction>();
	}
}
