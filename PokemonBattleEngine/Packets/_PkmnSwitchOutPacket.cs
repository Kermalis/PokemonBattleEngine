using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public interface IPBEPkmnSwitchOutPacket : IPBEPacket
{
	PBETrainer PokemonTrainer { get; }
	PBEFieldPosition OldPosition { get; }
	bool Forced { get; }
	PBETrainer? ForcedByPokemonTrainer { get; }
	PBEFieldPosition ForcedByPokemon { get; }
}
public sealed class PBEPkmnSwitchOutPacket : IPBEPkmnSwitchOutPacket
{
	public const ushort ID = 0x0C;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public byte Pokemon { get; }
	public PBEFieldPosition OldPosition { get; }
	public bool Forced { get; }
	public PBETrainer? ForcedByPokemonTrainer { get; }
	public PBEFieldPosition ForcedByPokemon { get; }

	internal PBEPkmnSwitchOutPacket(PBEBattlePokemon pokemon, PBEFieldPosition oldPosition, PBEBattlePokemon? forcedByPokemon = null)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = pokemon.Trainer).Id);
			w.WriteByte(Pokemon = pokemon.Id);
			w.WriteEnum(OldPosition = oldPosition);
			w.WriteBoolean(Forced = forcedByPokemon is not null);
			if (forcedByPokemon is not null)
			{
				w.WriteByte((ForcedByPokemonTrainer = forcedByPokemon.Trainer).Id);
				w.WriteEnum(ForcedByPokemon = forcedByPokemon.FieldPosition);
			}

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPkmnSwitchOutPacket(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		Pokemon = r.ReadByte();
		OldPosition = r.ReadEnum<PBEFieldPosition>();
		Forced = r.ReadBoolean();
		if (Forced)
		{
			ForcedByPokemonTrainer = battle.Trainers[r.ReadByte()];
			ForcedByPokemon = r.ReadEnum<PBEFieldPosition>();
		}
	}
}
public sealed class PBEPkmnSwitchOutPacket_Hidden : IPBEPkmnSwitchOutPacket
{
	public const ushort ID = 0x37;
	public ReadOnlyCollection<byte> Data { get; }

	public PBETrainer PokemonTrainer { get; }
	public PBEFieldPosition OldPosition { get; }
	public bool Forced { get; }
	public PBETrainer? ForcedByPokemonTrainer { get; }
	public PBEFieldPosition ForcedByPokemon { get; }

	public PBEPkmnSwitchOutPacket_Hidden(PBEPkmnSwitchOutPacket other)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			w.WriteByte((PokemonTrainer = other.PokemonTrainer).Id);
			w.WriteEnum(OldPosition = other.OldPosition);
			w.WriteBoolean(Forced = other.Forced);
			if (Forced)
			{
				w.WriteByte((ForcedByPokemonTrainer = other.ForcedByPokemonTrainer!).Id);
				w.WriteEnum(ForcedByPokemon = other.ForcedByPokemon);
			}

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEPkmnSwitchOutPacket_Hidden(byte[] data, EndianBinaryReader r, PBEBattle battle)
	{
		Data = new ReadOnlyCollection<byte>(data);

		PokemonTrainer = battle.Trainers[r.ReadByte()];
		OldPosition = r.ReadEnum<PBEFieldPosition>();
		Forced = r.ReadBoolean();
		if (Forced)
		{
			ForcedByPokemonTrainer = battle.Trainers[r.ReadByte()];
			ForcedByPokemon = r.ReadEnum<PBEFieldPosition>();
		}
	}
}
