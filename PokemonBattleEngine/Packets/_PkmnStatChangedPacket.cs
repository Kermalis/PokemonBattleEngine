using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public sealed class PBEPkmnStatChangedPacket : IPBEPacket
{
	public const ushort ID = 0x10;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public PBEFieldPosition Pokemon { get; }
	public PBEStat Stat { get; }
	public sbyte OldValue { get; }
	public sbyte NewValue { get; }

	internal PBEPkmnStatChangedPacket(PBEBattlePokemon pokemon, PBEStat stat, sbyte oldValue, sbyte newValue)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = pokemon.Trainer).Id);
			w.WriteEnum(Pokemon = pokemon.FieldPosition);
			w.WriteEnum(Stat = stat);
			w.WriteSByte(OldValue = oldValue);
			w.WriteSByte(NewValue = newValue);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPkmnStatChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		Pokemon = r.ReadEnum<PBEFieldPosition>();
		Stat = r.ReadEnum<PBEStat>();
		OldValue = r.ReadSByte();
		NewValue = r.ReadSByte();
	}
}
