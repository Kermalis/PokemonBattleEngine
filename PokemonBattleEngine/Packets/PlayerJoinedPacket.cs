using Kermalis.EndianBinaryIO;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEPlayerJoinedPacket : IPBEPacket
{
	public const ushort ID = 0x01;
	public ReadOnlyCollection<byte> Data { get; }

	public string TrainerName { get; }

	public PBEPlayerJoinedPacket(string trainerName)
	{
		if (string.IsNullOrWhiteSpace(trainerName))
		{
			throw new ArgumentOutOfRangeException(nameof(trainerName));
		}
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteChars_NullTerminated(TrainerName = trainerName);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPlayerJoinedPacket(byte[] data, EndianBinaryReader r)
	{
		Data = new ReadOnlyCollection<byte>(data);

		TrainerName = r.ReadString_NullTerminated();
	}
}
