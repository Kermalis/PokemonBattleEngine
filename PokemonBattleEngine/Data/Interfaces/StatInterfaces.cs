using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data.Utils;
using System;
using System.Text.Json;

namespace Kermalis.PokemonBattleEngine.Data;

public interface IPBEReadOnlyStatCollection
{
	byte HP { get; }
	byte Attack { get; }
	byte Defense { get; }
	byte SpAttack { get; }
	byte SpDefense { get; }
	byte Speed { get; }
}
public interface IPBEStatCollection : IPBEReadOnlyStatCollection
{
	new byte HP { get; set; }
	new byte Attack { get; set; }
	new byte Defense { get; set; }
	new byte SpAttack { get; set; }
	new byte SpDefense { get; set; }
	new byte Speed { get; set; }
}

public static class PBEStatInterfaceExtensions
{
	public static byte GetStat(this IPBEReadOnlyStatCollection stats, PBEStat stat)
	{
		switch (stat)
		{
			case PBEStat.HP: return stats.HP;
			case PBEStat.Attack: return stats.Attack;
			case PBEStat.Defense: return stats.Defense;
			case PBEStat.SpAttack: return stats.SpAttack;
			case PBEStat.SpDefense: return stats.SpDefense;
			case PBEStat.Speed: return stats.Speed;
			default: throw new ArgumentOutOfRangeException(nameof(stat));
		}
	}
	public static void SetStat(this IPBEStatCollection stats, PBEStat stat, byte value)
	{
		switch (stat)
		{
			case PBEStat.HP: stats.HP = value; break;
			case PBEStat.Attack: stats.Attack = value; break;
			case PBEStat.Defense: stats.Defense = value; break;
			case PBEStat.SpAttack: stats.SpAttack = value; break;
			case PBEStat.SpDefense: stats.SpDefense = value; break;
			case PBEStat.Speed: stats.Speed = value; break;
			default: throw new ArgumentOutOfRangeException(nameof(stat));
		}
	}

	public static byte GetHiddenPowerBasePower(this IPBEReadOnlyStatCollection stats, PBESettings settings)
	{
		return PBEDataUtils.GetHiddenPowerBasePower(stats.HP, stats.Attack, stats.Defense, stats.SpAttack, stats.SpDefense, stats.Speed, settings);
	}
	public static PBEType GetHiddenPowerType(this IPBEReadOnlyStatCollection stats)
	{
		return PBEDataUtils.GetHiddenPowerType(stats.HP, stats.Attack, stats.Defense, stats.SpAttack, stats.SpDefense, stats.Speed);
	}

	internal static void ToBytes(this IPBEReadOnlyStatCollection stats, EndianBinaryWriter w)
	{
		w.WriteByte(stats.HP);
		w.WriteByte(stats.Attack);
		w.WriteByte(stats.Defense);
		w.WriteByte(stats.SpAttack);
		w.WriteByte(stats.SpDefense);
		w.WriteByte(stats.Speed);
	}
	internal static void ToJson(this IPBEReadOnlyStatCollection stats, Utf8JsonWriter w)
	{
		w.WriteStartObject();

		w.WriteNumber(nameof(IPBEReadOnlyStatCollection.HP), stats.HP);
		w.WriteNumber(nameof(IPBEReadOnlyStatCollection.Attack), stats.Attack);
		w.WriteNumber(nameof(IPBEReadOnlyStatCollection.Defense), stats.Defense);
		w.WriteNumber(nameof(IPBEReadOnlyStatCollection.SpAttack), stats.SpAttack);
		w.WriteNumber(nameof(IPBEReadOnlyStatCollection.SpDefense), stats.SpDefense);
		w.WriteNumber(nameof(IPBEReadOnlyStatCollection.Speed), stats.Speed);

		w.WriteEndObject();
	}
}
