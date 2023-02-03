using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Legality;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEPartyResponsePacket : IPBEPacket
{
	public const ushort ID = 0x04;
	public ReadOnlyCollection<byte> Data { get; }

	public IPBEPokemonCollection Party { get; }

	public PBEPartyResponsePacket(IPBEPokemonCollection party)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			(Party = party).ToBytes(w);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPartyResponsePacket(byte[] data, EndianBinaryReader r)
	{
		Data = new ReadOnlyCollection<byte>(data);

		Party = new PBEReadOnlyPokemonCollection(r);
	}
}
public sealed class PBELegalPartyResponsePacket : IPBEPacket
{
	public const ushort ID = 0x2D;
	public ReadOnlyCollection<byte> Data { get; }

	public PBELegalPokemonCollection Party { get; }

	public PBELegalPartyResponsePacket(PBELegalPokemonCollection party)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteBytes(party.Settings.ToBytes());
			(Party = party).ToBytes(w);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBELegalPartyResponsePacket(byte[] data, EndianBinaryReader r)
	{
		Data = new ReadOnlyCollection<byte>(data);

		var s = new PBESettings(r);
		s.MakeReadOnly();
		Party = new PBELegalPokemonCollection(s, r);
	}
}
