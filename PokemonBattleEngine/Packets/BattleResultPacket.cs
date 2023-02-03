using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEBattleResultPacket : IPBEPacket
{
	public const ushort ID = 0x26;
	public ReadOnlyCollection<byte> Data { get; }

	public PBEBattleResult BattleResult { get; }

	internal PBEBattleResultPacket(PBEBattleResult battleResult)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteEnum(BattleResult = battleResult);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEBattleResultPacket(byte[] data, EndianBinaryReader r)
	{
		Data = new ReadOnlyCollection<byte>(data);

		BattleResult = r.ReadEnum<PBEBattleResult>();
	}
}
