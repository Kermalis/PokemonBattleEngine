using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBETypeChangedPacket : IPBEPacket
{
	public const ushort ID = 0x2B;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public PBEFieldPosition Pokemon { get; }
	public PBEType Type1 { get; }
	public PBEType Type2 { get; }

	internal PBETypeChangedPacket(PBEBattlePokemon pokemon, PBEType type1, PBEType type2)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = pokemon.Trainer).Id);
			w.WriteEnum(Pokemon = pokemon.FieldPosition);
			w.WriteEnum(Type1 = type1);
			w.WriteEnum(Type2 = type2);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBETypeChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		Pokemon = r.ReadEnum<PBEFieldPosition>();
		Type1 = r.ReadEnum<PBEType>();
		Type2 = r.ReadEnum<PBEType>();
	}
}
