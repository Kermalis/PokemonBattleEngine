using Kermalis.EndianBinaryIO;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEMatchCancelledPacket : IPBEPacket
{
	public const ushort ID = 0x02;
	public ReadOnlyCollection<byte> Data { get; }

	public PBEMatchCancelledPacket()
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEMatchCancelledPacket(byte[] data)
	{
		Data = new ReadOnlyCollection<byte>(data);
	}
}
