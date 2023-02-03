using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.DefaultData.Data;

public sealed class PBEDDPokemonData : IPBEDDPokemonDataExtended
{
	public PBESpecies Species { get; }
	public PBEForm Form { get; }
	public PBEReadOnlyStatCollection BaseStats { get; }
	IPBEReadOnlyStatCollection IPBEPokemonData.BaseStats => BaseStats;
	public PBEType Type1 { get; }
	public PBEType Type2 { get; }
	public PBEGenderRatio GenderRatio { get; }
	public PBEGrowthRate GrowthRate { get; }
	public ushort BaseEXPYield { get; }
	public byte CatchRate { get; }
	public byte FleeRate { get; }
	/// <summary>Weight in Kilograms</summary>
	public float Weight { get; }
	public ReadOnlyCollection<PBEAbility> Abilities { get; }
	IReadOnlyList<PBEAbility> IPBEPokemonData.Abilities => Abilities;
	public ReadOnlyCollection<(PBESpecies Species, PBEForm Form)> PreEvolutions { get; }
	IReadOnlyList<(PBESpecies Species, PBEForm Form)> IPBEDDPokemonDataExtended.PreEvolutions => PreEvolutions;
	public ReadOnlyCollection<(PBESpecies Species, PBEForm Form)> Evolutions { get; }
	IReadOnlyList<(PBESpecies Species, PBEForm Form)> IPBEDDPokemonDataExtended.Evolutions => Evolutions;
	public ReadOnlyCollection<(PBEMove Move, byte Level, PBEDDMoveObtainMethod ObtainMethod)> LevelUpMoves { get; }
	IReadOnlyList<(PBEMove Move, byte Level, PBEDDMoveObtainMethod ObtainMethod)> IPBEDDPokemonDataExtended.LevelUpMoves => LevelUpMoves;
	public ReadOnlyCollection<(PBEMove Move, PBEDDMoveObtainMethod ObtainMethod)> OtherMoves { get; }
	IReadOnlyList<(PBEMove Move, PBEDDMoveObtainMethod ObtainMethod)> IPBEDDPokemonDataExtended.OtherMoves => OtherMoves;

	private PBEDDPokemonData(SearchResult result)
	{
		BaseStats = new PBEReadOnlyStatCollection(result);
		Type1 = (PBEType)result.Type1;
		Type2 = (PBEType)result.Type2;
		GenderRatio = (PBEGenderRatio)result.GenderRatio;
		GrowthRate = (PBEGrowthRate)result.GrowthRate;
		BaseEXPYield = result.BaseEXPYield;
		CatchRate = result.CatchRate;
		FleeRate = result.FleeRate;
		Weight = result.Weight;

		const char Split1Char = '+';
		const char Split2Char = '|';

		string[] split1 = result.PreEvolutions.Split(Split1Char, StringSplitOptions.RemoveEmptyEntries);
		var preEvolutions = new (PBESpecies, PBEForm)[split1.Length];
		for (int i = 0; i < preEvolutions.Length; i++)
		{
			string[] split2 = split1[i].Split(Split2Char);
			preEvolutions[i] = (Enum.Parse<PBESpecies>(split2[0]), Enum.Parse<PBEForm>(split2[1]));
		}
		PreEvolutions = new ReadOnlyCollection<(PBESpecies, PBEForm)>(preEvolutions);

		split1 = result.Evolutions.Split(Split1Char, StringSplitOptions.RemoveEmptyEntries);
		var evolutions = new (PBESpecies, PBEForm)[split1.Length];
		for (int i = 0; i < evolutions.Length; i++)
		{
			string[] split2 = split1[i].Split(Split2Char);
			evolutions[i] = (Enum.Parse<PBESpecies>(split2[0]), Enum.Parse<PBEForm>(split2[1]));
		}
		Evolutions = new ReadOnlyCollection<(PBESpecies, PBEForm)>(evolutions);

		split1 = result.Abilities.Split(Split1Char, StringSplitOptions.RemoveEmptyEntries);
		var abilities = new PBEAbility[split1.Length];
		for (int i = 0; i < abilities.Length; i++)
		{
			abilities[i] = Enum.Parse<PBEAbility>(split1[i]);
		}
		Abilities = new ReadOnlyCollection<PBEAbility>(abilities);

		split1 = result.LevelUpMoves.Split(Split1Char, StringSplitOptions.RemoveEmptyEntries);
		var levelUpMoves = new (PBEMove, byte, PBEDDMoveObtainMethod)[split1.Length];
		for (int i = 0; i < levelUpMoves.Length; i++)
		{
			string[] split2 = split1[i].Split(Split2Char);
			levelUpMoves[i] = (Enum.Parse<PBEMove>(split2[0]), byte.Parse(split2[1]), Enum.Parse<PBEDDMoveObtainMethod>(split2[2]));
		}
		LevelUpMoves = new ReadOnlyCollection<(PBEMove, byte, PBEDDMoveObtainMethod)>(levelUpMoves);

		split1 = result.OtherMoves.Split(Split1Char, StringSplitOptions.RemoveEmptyEntries);
		var otherMoves = new (PBEMove, PBEDDMoveObtainMethod)[split1.Length];
		for (int i = 0; i < otherMoves.Length; i++)
		{
			string[] split2 = split1[i].Split(Split2Char);
			otherMoves[i] = (Enum.Parse<PBEMove>(split2[0]), Enum.Parse<PBEDDMoveObtainMethod>(split2[1]));
		}
		OtherMoves = new ReadOnlyCollection<(PBEMove, PBEDDMoveObtainMethod)>(otherMoves);
	}

	#region Database Querying

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	private class SearchResult : IPBEStatCollection
	{
		public string Species { get; set; }
		public string Form { get; set; }
		public byte HP { get; set; }
		public byte Attack { get; set; }
		public byte Defense { get; set; }
		public byte SpAttack { get; set; }
		public byte SpDefense { get; set; }
		public byte Speed { get; set; }
		public byte Type1 { get; set; }
		public byte Type2 { get; set; }
		public byte GenderRatio { get; set; }
		public byte GrowthRate { get; set; }
		public ushort BaseEXPYield { get; set; }
		public byte CatchRate { get; set; }
		public byte FleeRate { get; set; }
		public float Weight { get; set; }
		public string PreEvolutions { get; set; }
		public string Evolutions { get; set; }
		public string Abilities { get; set; }
		public string LevelUpMoves { get; set; }
		public string OtherMoves { get; set; }
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

#pragma warning disable IDE0060 // Remove unused parameter
	public static PBEDDPokemonData GetData(PBESpecies species, PBEForm form, bool cache = true)
#pragma warning restore IDE0060 // Remove unused parameter
	{
		PBEDataUtils.ValidateSpecies(species, form, false);
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>($"SELECT * FROM PokemonData WHERE Species='{species}' AND Form='{PBEDataUtils.GetNameOfForm(species, form) ?? "0"}'");
		if (results.Count == 1)
		{
			return new PBEDDPokemonData(results[0]);
		}
		throw new InvalidDataException();
	}

	#endregion
}
