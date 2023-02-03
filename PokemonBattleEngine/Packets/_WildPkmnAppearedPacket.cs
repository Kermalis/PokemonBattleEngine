using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Packets;

public interface IPBEPkmnAppearedInfo_Hidden : IPBESpeciesForm
{
	string Nickname { get; }
	byte Level { get; }
	bool Shiny { get; }
	PBEGender Gender { get; }
	float HPPercentage { get; }
	PBEStatus1 Status1 { get; }
	PBEFieldPosition FieldPosition { get; }
}
public interface IPBEWildPkmnAppearedPacket : IPBEPacket
{
	IReadOnlyList<IPBEPkmnAppearedInfo_Hidden> Pokemon { get; }
}
public sealed class PBEPkmnAppearedInfo : IPBEPkmnSwitchInInfo_Hidden
{
	public byte Pokemon { get; }
	public bool IsDisguised { get; }
	public PBESpecies Species { get; }
	public PBEForm Form { get; }
	public string Nickname { get; }
	public byte Level { get; }
	public bool Shiny { get; }
	public PBEGender Gender { get; }
	public PBEItem CaughtBall { get; }
	public ushort HP { get; }
	public ushort MaxHP { get; }
	public float HPPercentage { get; }
	public PBEStatus1 Status1 { get; }
	public PBEFieldPosition FieldPosition { get; }

	internal PBEPkmnAppearedInfo(PBEBattlePokemon pkmn)
	{
		Pokemon = pkmn.Id;
		IsDisguised = pkmn.Status2.HasFlag(PBEStatus2.Disguised);
		Species = pkmn.KnownSpecies;
		Form = pkmn.KnownForm;
		Nickname = pkmn.KnownNickname;
		Level = pkmn.Level;
		Shiny = pkmn.KnownShiny;
		Gender = pkmn.KnownGender;
		CaughtBall = pkmn.KnownCaughtBall;
		HP = pkmn.HP;
		MaxHP = pkmn.MaxHP;
		HPPercentage = pkmn.HPPercentage;
		Status1 = pkmn.Status1;
		FieldPosition = pkmn.FieldPosition;
	}
	internal PBEPkmnAppearedInfo(EndianBinaryReader r)
	{
		Pokemon = r.ReadByte();
		IsDisguised = r.ReadBoolean();
		Species = r.ReadEnum<PBESpecies>();
		Form = r.ReadEnum<PBEForm>();
		Nickname = r.ReadString_NullTerminated();
		Level = r.ReadByte();
		Shiny = r.ReadBoolean();
		Gender = r.ReadEnum<PBEGender>();
		CaughtBall = r.ReadEnum<PBEItem>();
		HP = r.ReadUInt16();
		MaxHP = r.ReadUInt16();
		HPPercentage = r.ReadSingle();
		Status1 = r.ReadEnum<PBEStatus1>();
		FieldPosition = r.ReadEnum<PBEFieldPosition>();
	}

	internal void ToBytes(EndianBinaryWriter w)
	{
		w.WriteByte(Pokemon);
		w.WriteBoolean(IsDisguised);
		w.WriteEnum(Species);
		w.WriteEnum(Form);
		w.WriteChars_NullTerminated(Nickname);
		w.WriteByte(Level);
		w.WriteBoolean(Shiny);
		w.WriteEnum(Gender);
		w.WriteEnum(CaughtBall);
		w.WriteUInt16(HP);
		w.WriteUInt16(MaxHP);
		w.WriteSingle(HPPercentage);
		w.WriteEnum(Status1);
		w.WriteEnum(FieldPosition);
	}
}
public sealed class PBEWildPkmnAppearedPacket : IPBEWildPkmnAppearedPacket
{
	public const ushort ID = 0x0D;
	public ReadOnlyCollection<byte> Data { get; }

	public ReadOnlyCollection<PBEPkmnAppearedInfo> Pokemon { get; }
	IReadOnlyList<IPBEPkmnAppearedInfo_Hidden> IPBEWildPkmnAppearedPacket.Pokemon => Pokemon;

	internal PBEWildPkmnAppearedPacket(IList<PBEPkmnAppearedInfo> pokemon)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			byte count = (byte)(Pokemon = new ReadOnlyCollection<PBEPkmnAppearedInfo>(pokemon)).Count;
			w.WriteByte(count);
			for (int i = 0; i < count; i++)
			{
				Pokemon[i].ToBytes(w);
			}

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEWildPkmnAppearedPacket(byte[] data, EndianBinaryReader r)
	{
		Data = new ReadOnlyCollection<byte>(data);

		var pokemon = new PBEPkmnAppearedInfo[r.ReadByte()];
		for (int i = 0; i < pokemon.Length; i++)
		{
			pokemon[i] = new PBEPkmnAppearedInfo(r);
		}
		Pokemon = new ReadOnlyCollection<PBEPkmnAppearedInfo>(pokemon);
	}
}
public sealed class PBEWildPkmnAppearedPacket_Hidden : IPBEWildPkmnAppearedPacket
{
	public const ushort ID = 0x3C;
	public ReadOnlyCollection<byte> Data { get; }

	public sealed class PBEWildPkmnInfo : IPBEPkmnAppearedInfo_Hidden
	{
		public PBESpecies Species { get; }
		public PBEForm Form { get; }
		public string Nickname { get; }
		public byte Level { get; }
		public bool Shiny { get; }
		public PBEGender Gender { get; }
		public float HPPercentage { get; }
		public PBEStatus1 Status1 { get; }
		public PBEFieldPosition FieldPosition { get; }

		internal PBEWildPkmnInfo(PBEPkmnAppearedInfo other)
		{
			Species = other.Species;
			Form = other.Form;
			Nickname = other.Nickname;
			Level = other.Level;
			Shiny = other.Shiny;
			Gender = other.Gender;
			HPPercentage = other.HPPercentage;
			Status1 = other.Status1;
			FieldPosition = other.FieldPosition;
		}
		internal PBEWildPkmnInfo(EndianBinaryReader r)
		{
			Species = r.ReadEnum<PBESpecies>();
			Form = r.ReadEnum<PBEForm>();
			Nickname = r.ReadString_NullTerminated();
			Level = r.ReadByte();
			Shiny = r.ReadBoolean();
			Gender = r.ReadEnum<PBEGender>();
			HPPercentage = r.ReadSingle();
			Status1 = r.ReadEnum<PBEStatus1>();
			FieldPosition = r.ReadEnum<PBEFieldPosition>();
		}

		internal void ToBytes(EndianBinaryWriter w)
		{
			w.WriteEnum(Species);
			w.WriteEnum(Form);
			w.WriteChars_NullTerminated(Nickname);
			w.WriteByte(Level);
			w.WriteBoolean(Shiny);
			w.WriteEnum(Gender);
			w.WriteSingle(HPPercentage);
			w.WriteEnum(Status1);
			w.WriteEnum(FieldPosition);
		}
	}

	public ReadOnlyCollection<PBEWildPkmnInfo> Pokemon { get; }
	IReadOnlyList<IPBEPkmnAppearedInfo_Hidden> IPBEWildPkmnAppearedPacket.Pokemon => Pokemon;

	public PBEWildPkmnAppearedPacket_Hidden(PBEWildPkmnAppearedPacket other)
	{
		using (var ms = new MemoryStream())
		{
			EndianBinaryWriter w = PBEPacketProcessor.WritePacketID(ms, ID);

			var pokemon = new PBEWildPkmnInfo[other.Pokemon.Count];
			for (int i = 0; i < pokemon.Length; i++)
			{
				pokemon[i] = new PBEWildPkmnInfo(other.Pokemon[i]);
			}
			byte count = (byte)(Pokemon = new ReadOnlyCollection<PBEWildPkmnInfo>(pokemon)).Count;
			w.WriteByte(count);
			for (int i = 0; i < count; i++)
			{
				Pokemon[i].ToBytes(w);
			}

			Data = new ReadOnlyCollection<byte>(ms.ToArray());
		}
	}
	internal PBEWildPkmnAppearedPacket_Hidden(byte[] data, EndianBinaryReader r)
	{
		Data = new ReadOnlyCollection<byte>(data);

		var pokemon = new PBEWildPkmnInfo[r.ReadByte()];
		for (int i = 0; i < pokemon.Length; i++)
		{
			pokemon[i] = new PBEWildPkmnInfo(r);
		}
		Pokemon = new ReadOnlyCollection<PBEWildPkmnInfo>(pokemon);
	}
}
