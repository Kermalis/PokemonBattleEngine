using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Kermalis.PokemonBattleEngine.DefaultData;

public static class PBEDDLocalizedString
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	private sealed class SearchResult : IPBELocalizedString
	{
		public string Id { get; set; }
		public string English { get; set; }
		public string French { get; set; }
		public string German { get; set; }
		public string Italian { get; set; }
		public string Japanese_Kana { get; set; }
		public string Japanese_Kanji { get; set; }
		public string Korean { get; set; }
		public string Spanish { get; set; }
	}
	private sealed class FormNameSearchResult : IPBELocalizedString
	{
		public string Species { get; set; }
		public string Form { get; set; }
		public string English { get; set; }
		public string French { get; set; }
		public string German { get; set; }
		public string Italian { get; set; }
		public string Japanese_Kana { get; set; }
		public string Japanese_Kanji { get; set; }
		public string Korean { get; set; }
		public string Spanish { get; set; }
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	private const string QueryText = "SELECT * FROM {0} WHERE StrCmp(English,'{1}') OR StrCmp(French,'{1}') OR StrCmp(German,'{1}') OR StrCmp(Italian,'{1}') OR StrCmp(Japanese_Kana,'{1}') OR StrCmp(Japanese_Kanji,'{1}') OR StrCmp(Korean,'{1}') OR StrCmp(Spanish,'{1}')";
	private const string QueryId = "SELECT * FROM {0} WHERE Id='{1}'";
	private const string QuerySpeciesAndText = "SELECT * FROM {0} WHERE (StrCmp(English,'{1}') OR StrCmp(French,'{1}') OR StrCmp(German,'{1}') OR StrCmp(Italian,'{1}') OR StrCmp(Japanese_Kana,'{1}') OR StrCmp(Japanese_Kanji,'{1}') OR StrCmp(Korean,'{1}') OR StrCmp(Spanish,'{1}')) AND (Species='{2}')";
	private const string QuerySpecies = "SELECT * FROM {0} WHERE Species='{1}' AND Form='{2}'";
	private static bool GetEnumValue<TEnum>(string value, [NotNullWhen(true)] out TEnum? result) where TEnum : struct, Enum
	{
		foreach (TEnum v in Enum.GetValues<TEnum>())
		{
			if (v.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
			{
				result = v;
				return true;
			}
		}
		result = null;
		return false;
	}

	public static bool GetAbilityByName(string abilityName, [NotNullWhen(true)] out PBEAbility? ability)
	{
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "AbilityNames", abilityName));
		if (results.Count == 1)
		{
			ability = Enum.Parse<PBEAbility>(results[0].Id);
		}
		else if (!GetEnumValue(abilityName, out ability) || ability == PBEAbility.MAX)
		{
			ability = null;
			return false;
		}
		return true;
	}
	public static PBEReadOnlyLocalizedString GetAbilityDescription(PBEAbility ability)
	{
		if (ability >= PBEAbility.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(ability));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "AbilityDescriptions", ability));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static PBEReadOnlyLocalizedString GetAbilityName(PBEAbility ability)
	{
		if (ability >= PBEAbility.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(ability));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "AbilityNames", ability));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static bool GetFormByName(PBESpecies species, string formName, [NotNullWhen(true)] out PBEForm? form)
	{
		List<FormNameSearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<FormNameSearchResult>(string.Format(QuerySpeciesAndText, "FormNames", formName, species));
		if (results.Count == 1)
		{
			form = Enum.Parse<PBEForm>(results[0].Form);
		}
		else if (!GetEnumValue(formName, out form))
		{
			form = null;
			return false;
		}
		return true;
	}
	public static PBEReadOnlyLocalizedString GetFormName(PBESpecies species, PBEForm form)
	{
		PBEDataUtils.ValidateSpecies(species, form, false);
		List<FormNameSearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<FormNameSearchResult>(string.Format(QuerySpecies, "FormNames", species, PBEDataUtils.GetNameOfForm(species, form) ?? "0"));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static bool GetGenderByName(string genderName, [NotNullWhen(true)] out PBEGender? gender)
	{
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "GenderNames", genderName));
		if (results.Count == 1)
		{
			gender = Enum.Parse<PBEGender>(results[0].Id);
		}
		else if (!GetEnumValue(genderName, out gender) || gender == PBEGender.MAX)
		{
			gender = null;
			return false;
		}
		return true;
	}
	public static PBEReadOnlyLocalizedString GetGenderName(PBEGender gender)
	{
		if (gender >= PBEGender.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(gender));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "GenderNames", gender));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static bool GetItemByName(string itemName, [NotNullWhen(true)] out PBEItem? item)
	{
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "ItemNames", itemName));
		if (results.Count == 1)
		{
			item = Enum.Parse<PBEItem>(results[0].Id);
		}
		else if (!GetEnumValue(itemName, out item))
		{
			item = null;
			return false;
		}
		return true;
	}
	public static PBEReadOnlyLocalizedString GetItemDescription(PBEItem item)
	{
		if (!Enum.IsDefined(item))
		{
			throw new ArgumentOutOfRangeException(nameof(item));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "ItemDescriptions", item));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static PBEReadOnlyLocalizedString GetItemName(PBEItem item)
	{
		if (!Enum.IsDefined(item))
		{
			throw new ArgumentOutOfRangeException(nameof(item));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "ItemNames", item));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static bool GetMoveByName(string moveName, [NotNullWhen(true)] out PBEMove? move)
	{
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "MoveNames", moveName));
		if (results.Count == 1)
		{
			move = Enum.Parse<PBEMove>(results[0].Id);
		}
		else if (!GetEnumValue(moveName, out move) || move == PBEMove.MAX)
		{
			move = null;
			return false;
		}
		return true;
	}
	public static PBEReadOnlyLocalizedString GetMoveDescription(PBEMove move)
	{
		if (move >= PBEMove.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(move));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "MoveDescriptions", move));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static PBEReadOnlyLocalizedString GetMoveName(PBEMove move)
	{
		if (move >= PBEMove.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(move));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "MoveNames", move));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static bool GetNatureByName(string natureName, [NotNullWhen(true)] out PBENature? nature)
	{
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "NatureNames", natureName));
		if (results.Count == 1)
		{
			nature = Enum.Parse<PBENature>(results[0].Id);
		}
		else if (!GetEnumValue(natureName, out nature) || nature == PBENature.MAX)
		{
			nature = null;
			return false;
		}
		return true;
	}
	public static PBEReadOnlyLocalizedString GetNatureName(PBENature nature)
	{
		if (nature >= PBENature.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(nature));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "NatureNames", nature));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static bool GetSpeciesByName(string speciesName, [NotNullWhen(true)] out PBESpecies? species)
	{
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "SpeciesNames", speciesName));
		if (results.Count == 1)
		{
			species = Enum.Parse<PBESpecies>(results[0].Id);
		}
		else if (!GetEnumValue(speciesName, out species))
		{
			species = null;
			return false;
		}
		return true;
	}
	public static PBEReadOnlyLocalizedString GetSpeciesCategory(PBESpecies species)
	{
		if (species <= 0 || species >= PBESpecies.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(species));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "SpeciesCategories", species));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static PBEReadOnlyLocalizedString GetSpeciesEntry(PBESpecies species)
	{
		if (species <= 0 || species >= PBESpecies.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(species));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "SpeciesEntries", species));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static PBEReadOnlyLocalizedString GetSpeciesName(PBESpecies species)
	{
		if (species <= 0 || species >= PBESpecies.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(species));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "SpeciesNames", species));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static bool GetStatByName(string statName, [NotNullWhen(true)] out PBEStat? stat)
	{
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "StatNames", statName));
		if (results.Count == 1)
		{
			stat = Enum.Parse<PBEStat>(results[0].Id);
		}
		else if (!GetEnumValue(statName, out stat))
		{
			stat = null;
			return false;
		}
		return true;
	}
	public static PBEReadOnlyLocalizedString GetStatName(PBEStat stat)
	{
		if (!Enum.IsDefined(stat))
		{
			throw new ArgumentOutOfRangeException(nameof(stat));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "StatNames", stat));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
	public static bool GetTypeByName(string typeName, [NotNullWhen(true)] out PBEType? type)
	{
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "TypeNames", typeName));
		if (results.Count == 1)
		{
			type = Enum.Parse<PBEType>(results[0].Id);
		}
		else if (!GetEnumValue(typeName, out type))
		{
			type = null;
			return false;
		}
		return true;
	}
	public static PBEReadOnlyLocalizedString GetTypeName(PBEType type)
	{
		if (type >= PBEType.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(type));
		}
		List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "TypeNames", type));
		if (results.Count == 1)
		{
			return new PBEReadOnlyLocalizedString(results[0]);
		}
		throw new InvalidDataException();
	}
}
