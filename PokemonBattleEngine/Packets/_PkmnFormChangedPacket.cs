using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public interface IPBEPkmnFormChangedPacket : IPBEPacket
{
	PBETrainer PokemonTrainer { get; }
	PBEFieldPosition Pokemon { get; }
	float NewHPPercentage { get; }
	PBEAbility NewKnownAbility { get; }
	PBEForm NewForm { get; }
	PBEType NewType1 { get; }
	PBEType NewType2 { get; }
	float NewWeight { get; }
	bool IsRevertForm { get; }
}
public sealed class PBEPkmnFormChangedPacket : IPBEPkmnFormChangedPacket
{
	public const ushort ID = 0x29;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public PBEFieldPosition Pokemon { get; }
	public ushort NewHP { get; }
	public ushort NewMaxHP { get; }
	public float NewHPPercentage { get; }
	public ushort NewAttack { get; }
	public ushort NewDefense { get; }
	public ushort NewSpAttack { get; }
	public ushort NewSpDefense { get; }
	public ushort NewSpeed { get; }
	public PBEAbility NewAbility { get; }
	public PBEAbility NewKnownAbility { get; }
	public PBEForm NewForm { get; }
	public PBEType NewType1 { get; }
	public PBEType NewType2 { get; }
	public float NewWeight { get; }
	public bool IsRevertForm { get; }

	internal PBEPkmnFormChangedPacket(PBEBattlePokemon pokemon, bool isRevertForm)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = pokemon.Trainer).Id);
			w.WriteEnum(Pokemon = pokemon.FieldPosition);
			w.WriteUInt16(NewHP = pokemon.HP);
			w.WriteUInt16(NewMaxHP = pokemon.MaxHP);
			w.WriteSingle(NewHPPercentage = pokemon.HPPercentage);
			w.WriteUInt16(NewAttack = pokemon.Attack);
			w.WriteUInt16(NewDefense = pokemon.Defense);
			w.WriteUInt16(NewSpAttack = pokemon.SpAttack);
			w.WriteUInt16(NewSpDefense = pokemon.SpDefense);
			w.WriteUInt16(NewSpeed = pokemon.Speed);
			w.WriteEnum(NewAbility = pokemon.Ability);
			w.WriteEnum(NewKnownAbility = pokemon.KnownAbility);
			w.WriteEnum(NewForm = pokemon.Form);
			w.WriteEnum(NewType1 = pokemon.Type1);
			w.WriteEnum(NewType2 = pokemon.Type2);
			w.WriteSingle(NewWeight = pokemon.Weight);
			w.WriteBoolean(IsRevertForm = isRevertForm);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPkmnFormChangedPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		Pokemon = r.ReadEnum<PBEFieldPosition>();
		NewHP = r.ReadUInt16();
		NewMaxHP = r.ReadUInt16();
		NewHPPercentage = r.ReadSingle();
		NewAttack = r.ReadUInt16();
		NewDefense = r.ReadUInt16();
		NewSpAttack = r.ReadUInt16();
		NewSpDefense = r.ReadUInt16();
		NewSpeed = r.ReadUInt16();
		NewAbility = r.ReadEnum<PBEAbility>();
		NewKnownAbility = r.ReadEnum<PBEAbility>();
		NewForm = r.ReadEnum<PBEForm>();
		NewType1 = r.ReadEnum<PBEType>();
		NewType2 = r.ReadEnum<PBEType>();
		NewWeight = r.ReadSingle();
	}
}
public sealed class PBEPkmnFormChangedPacket_Hidden : IPBEPkmnFormChangedPacket
{
	public const ushort ID = 0x34;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public PBEFieldPosition Pokemon { get; }
	public float NewHPPercentage { get; }
	public PBEAbility NewKnownAbility { get; }
	public PBEForm NewForm { get; }
	public PBEType NewType1 { get; }
	public PBEType NewType2 { get; }
	public float NewWeight { get; }
	public bool IsRevertForm { get; }

	public PBEPkmnFormChangedPacket_Hidden(PBEPkmnFormChangedPacket other)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = other.PokemonTrainer).Id);
			w.WriteEnum(Pokemon = other.Pokemon);
			w.WriteSingle(NewHPPercentage = other.NewHPPercentage);
			w.WriteEnum(NewKnownAbility = other.NewKnownAbility);
			w.WriteEnum(NewForm = other.NewForm);
			w.WriteEnum(NewType1 = other.NewType1);
			w.WriteEnum(NewType2 = other.NewType2);
			w.WriteSingle(NewWeight = other.NewWeight);
			w.WriteBoolean(IsRevertForm = other.IsRevertForm);

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPkmnFormChangedPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		Pokemon = r.ReadEnum<PBEFieldPosition>();
		NewHPPercentage = r.ReadSingle();
		NewKnownAbility = r.ReadEnum<PBEAbility>();
		NewForm = r.ReadEnum<PBEForm>();
		NewType1 = r.ReadEnum<PBEType>();
		NewType2 = r.ReadEnum<PBEType>();
		NewWeight = r.ReadSingle();
	}
}
