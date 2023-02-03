using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data.Utils;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Kermalis.PokemonBattleEngine.Data;

// Not separating this into IPBEWildPokemon for these reasons:
// 1: A lot of work to do that
// 2: If someone wants to do pal park or catch released Pokémon etc, they'd need all these things
// 3: If they want just some things (like effort values pre-seeded) they'd also need this
public interface IPBEPokemon : IPBESpeciesForm
{
	/// <summary>This marks the Pokémon to be ignored by the battle engine. The Pokémon will be treated like an egg or fainted Pokémon.
	/// Therefore, it won't be sent out, copied with <see cref="PBEAbility.Illusion"/>, or count as a battler if the rest of the team faints.</summary>
	bool PBEIgnore { get; }
	PBEGender Gender { get; }
	string Nickname { get; }
	bool Shiny { get; }
	byte Level { get; }
	uint EXP { get; }
	bool Pokerus { get; }
	PBEItem Item { get; }
	byte Friendship { get; }
	PBEAbility Ability { get; }
	PBENature Nature { get; }
	PBEItem CaughtBall { get; }
	IPBEStatCollection EffortValues { get; }
	IPBEReadOnlyStatCollection IndividualValues { get; }
	IPBEMoveset Moveset { get; }
}
public interface IPBEPartyPokemon : IPBEPokemon
{
	ushort HP { get; }
	PBEStatus1 Status1 { get; }
	byte SleepTurns { get; }
	new IPBEPartyMoveset Moveset { get; }
}
public interface IPBEPokemonCollection<T> : IReadOnlyList<T> where T : IPBEPokemon
{
}
public interface IPBEPokemonCollection : IReadOnlyList<IPBEPokemon>
{
}
public interface IPBEPartyPokemonCollection<T> : IReadOnlyList<T> where T : IPBEPartyPokemon
{
}
public interface IPBEPartyPokemonCollection : IReadOnlyList<IPBEPartyPokemon>
{
}

public interface IPBEPokemonTypes
{
	/// <summary>The Pokémon's first type.</summary>
	PBEType Type1 { get; }
	/// <summary>The Pokémon's second type.</summary>
	PBEType Type2 { get; }
}
public interface IPBEPokemonKnownTypes
{
	/// <summary>The first type everyone believes the Pokémon has.</summary>
	PBEType KnownType1 { get; }
	/// <summary>The second type everyone believes the Pokémon has.</summary>
	PBEType KnownType2 { get; }
}
public interface IPBESpeciesForm
{
	PBESpecies Species { get; }
	PBEForm Form { get; }
}

public static class PBEPokemonInterfaceExtensions
{
	public static bool HasType(this IPBEPokemonTypes pkmn, PBEType type)
	{
		if (type >= PBEType.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(type));
		}
		return pkmn.Type1 == type || pkmn.Type2 == type;
	}
	public static bool HasType_Known(this IPBEPokemonKnownTypes pkmn, PBEType type)
	{
		if (type >= PBEType.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(type));
		}
		return pkmn.KnownType1 == type || pkmn.KnownType2 == type;
	}
	public static bool HasType<T>(this T pkmn, PBEType type, bool useKnownInfo) where T : IPBEPokemonKnownTypes, IPBEPokemonTypes
	{
		return useKnownInfo ? HasType_Known(pkmn, type) : HasType(pkmn, type);
	}
	public static bool ReceivesSTAB(this IPBEPokemonTypes pkmn, PBEType type)
	{
		return type != PBEType.None && HasType(pkmn, type);
	}
	public static bool ReceivesSTAB_Known(this IPBEPokemonKnownTypes pkmn, PBEType type)
	{
		return type != PBEType.None && HasType_Known(pkmn, type);
	}
	public static bool ReceivesSTAB<T>(this T pkmn, PBEType type, bool useKnownInfo) where T : IPBEPokemonKnownTypes, IPBEPokemonTypes
	{
		return useKnownInfo ? ReceivesSTAB_Known(pkmn, type) : ReceivesSTAB(pkmn, type);
	}

	internal static void ToBytes(this IPBEPokemon pkmn, EndianBinaryWriter w)
	{
		w.WriteEnum(pkmn.Species);
		w.WriteEnum(pkmn.Form);
		w.WriteChars_NullTerminated(pkmn.Nickname);
		w.WriteByte(pkmn.Level);
		w.WriteUInt32(pkmn.EXP);
		w.WriteByte(pkmn.Friendship);
		w.WriteBoolean(pkmn.Shiny);
		w.WriteBoolean(pkmn.Pokerus);
		w.WriteEnum(pkmn.Ability);
		w.WriteEnum(pkmn.Nature);
		w.WriteEnum(pkmn.CaughtBall);
		w.WriteEnum(pkmn.Gender);
		w.WriteEnum(pkmn.Item);
		pkmn.EffortValues.ToBytes(w);
		pkmn.IndividualValues.ToBytes(w);
		pkmn.Moveset.ToBytes(w);
	}
	internal static void ToJson(this IPBEPokemon pkmn, Utf8JsonWriter w)
	{
		w.WriteStartObject();

		PBESpecies species = pkmn.Species;
		w.WriteString(nameof(IPBEPokemon.Species), species.ToString());
		if (PBEDataUtils.HasForms(species, true))
		{
			w.WriteString(nameof(IPBEPokemon.Form), PBEDataUtils.GetNameOfForm(species, pkmn.Form));
		}
		w.WriteString(nameof(IPBEPokemon.Nickname), pkmn.Nickname);
		w.WriteNumber(nameof(IPBEPokemon.Level), pkmn.Level);
		w.WriteNumber(nameof(IPBEPokemon.EXP), pkmn.EXP);
		w.WriteNumber(nameof(IPBEPokemon.Friendship), pkmn.Friendship);
		w.WriteBoolean(nameof(IPBEPokemon.Shiny), pkmn.Shiny);
		w.WriteBoolean(nameof(IPBEPokemon.Pokerus), pkmn.Pokerus);
		w.WriteString(nameof(IPBEPokemon.Ability), pkmn.Ability.ToString());
		w.WriteString(nameof(IPBEPokemon.Nature), pkmn.Nature.ToString());
		w.WriteString(nameof(IPBEPokemon.CaughtBall), pkmn.CaughtBall.ToString());
		w.WriteString(nameof(IPBEPokemon.Gender), pkmn.Gender.ToString());
		w.WriteString(nameof(IPBEPokemon.Item), pkmn.Item.ToString());
		w.WritePropertyName(nameof(IPBEPokemon.EffortValues));
		pkmn.EffortValues.ToJson(w);
		w.WritePropertyName(nameof(IPBEPokemon.IndividualValues));
		pkmn.IndividualValues.ToJson(w);
		w.WritePropertyName(nameof(IPBEPokemon.Moveset));
		pkmn.Moveset.ToJson(w);

		w.WriteEndObject();
	}

	internal static void ToBytes(this IPBEPokemonCollection party, EndianBinaryWriter w)
	{
		byte count = (byte)party.Count;
		w.WriteByte(count);
		for (int i = 0; i < count; i++)
		{
			party[i].ToBytes(w);
		}
	}
}
