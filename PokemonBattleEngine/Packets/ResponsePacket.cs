using Kermalis.EndianBinaryIO;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEResponsePacket : IPBEPacket
{
	public const ushort ID = 0x00;
	public ReadOnlyCollection<byte> Data { get; }

	public PBEResponsePacket()
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEResponsePacket(byte[] data)
	{
		Data = new ReadOnlyCollection<byte>(data);
	}
}
