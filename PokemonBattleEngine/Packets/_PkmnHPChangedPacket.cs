using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public interface IPBEPkmnHPChangedPacket : IPBEPacket
{
	PBETrainer PokemonTrainer { get; }
	PBEFieldPosition Pokemon { get; }
	float OldHPPercentage { get; }
	float NewHPPercentage { get; }
}
public sealed class PBEPkmnHPChangedPacket : IPBEPkmnHPChangedPacket
{
	public const ushort ID = 0x0A;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public PBEFieldPosition Pokemon { get; }
	public ushort OldHP { get; }
	public ushort NewHP { get; }
	public float OldHPPercentage { get; }
	public float NewHPPercentage { get; }

	internal PBEPkmnHPChangedPacket(PBEBattlePokemon pokemon, ushort oldHP, float oldHPPercentage)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = pokemon.Trainer).Id);
			w.WriteEnum(Pokemon = pokemon.FieldPosition);
			w.WriteUInt16(OldHP = oldHP);
			w.WriteUInt16(NewHP = pokemon.HP);
			w.WriteSingle(OldHPPercentage = oldHPPercentage);
			w.WriteSingle(NewHPPercentage = pokemon.HPPercentage);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPkmnHPChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		Pokemon = r.ReadEnum<PBEFieldPosition>();
		OldHP = r.ReadUInt16();
		NewHP = r.ReadUInt16();
		OldHPPercentage = r.ReadSingle();
		NewHPPercentage = r.ReadSingle();
	}
}
public sealed class PBEPkmnHPChangedPacket_Hidden : IPBEPkmnHPChangedPacket
{
	public const ushort ID = 0x35;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public PBEFieldPosition Pokemon { get; }
	public float OldHPPercentage { get; }
	public float NewHPPercentage { get; }

	public PBEPkmnHPChangedPacket_Hidden(PBEPkmnHPChangedPacket other)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = other.PokemonTrainer).Id);
			w.WriteEnum(Pokemon = other.Pokemon);
			w.WriteSingle(OldHPPercentage = other.OldHPPercentage);
			w.WriteSingle(NewHPPercentage = other.NewHPPercentage);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPkmnHPChangedPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		Pokemon = r.ReadEnum<PBEFieldPosition>();
		OldHPPercentage = r.ReadSingle();
		NewHPPercentage = r.ReadSingle();
	}
}
