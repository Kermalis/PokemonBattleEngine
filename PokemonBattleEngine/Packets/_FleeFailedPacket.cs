using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEFleeFailedPacket : IPBEPacket
{
	public const ushort ID = 0x39;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public PBEFieldPosition Pokemon { get; }

	internal PBEFleeFailedPacket(PBEBattlePokemon pokemon)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = pokemon.Trainer).Id);
			w.WriteEnum(Pokemon = pokemon.FieldPosition);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEFleeFailedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		Pokemon = r.ReadEnum<PBEFieldPosition>();
	}
}
