using Kermalis.EndianBinaryIO;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEPartyRequestPacket : IPBEPacket
{
	public const ushort ID = 0x03;
	public ReadOnlyCollection<byte> Data { get; }

	public byte BattleId { get; }
	public bool RequireLegal { get; }

	public PBEPartyRequestPacket(byte battleId, bool requireLegal)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte(BattleId = battleId);
			w.WriteBoolean(RequireLegal = requireLegal);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPartyRequestPacket(byte[] data, EndianBinaryReader r)
	{
		Data = new ReadOnlyCollection<byte>(data);

		BattleId = r.ReadByte();
		RequireLegal = r.ReadBoolean();
	}
}
