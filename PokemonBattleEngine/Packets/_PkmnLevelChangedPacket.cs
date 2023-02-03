using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEPkmnLevelChangedPacket : IPBEPacket
{
	public const ushort ID = 0x3F;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public byte Pokemon { get; }
	public byte NewLevel { get; }

	internal PBEPkmnLevelChangedPacket(PBEBattlePokemon pokemon)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = pokemon.Trainer).Id);
			w.WriteByte(Pokemon = pokemon.Id);
			w.WriteByte(NewLevel = pokemon.Level);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPkmnLevelChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		Pokemon = r.ReadByte();
		NewLevel = r.ReadByte();
	}
}
