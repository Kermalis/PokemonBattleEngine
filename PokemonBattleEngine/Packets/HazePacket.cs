using Kermalis.EndianBinaryIO;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEHazePacket : IPBEPacket
{
	public const ushort ID = 0x0B;
	public ReadOnlyCollection<byte> Data { get; }

	internal PBEHazePacket()
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEHazePacket(byte[] data)
	{
		Data = new ReadOnlyCollection<byte>(data);
	}
}
