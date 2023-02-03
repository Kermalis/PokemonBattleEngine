using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Utils;
using Kermalis.PokemonBattleEngine.DefaultData.Data;
using Kermalis.PokemonBattleEngine.Utils;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Kermalis.PokemonBattleEngine.DefaultData;

public class PBEDefaultDataProvider : PBEDataProvider
{
	public static new PBEDefaultDataProvider Instance => (PBEDefaultDataProvider)PBEDataProvider.Instance;

	private readonly object _databaseConnectLockObj = new();
	private readonly SqliteConnection _databaseConnection;

	protected PBEDefaultDataProvider(string databasePath, PBELanguage language, PBERandom rand)
		: base(language, rand)
	{
		SQLitePCL.Batteries_V2.Init();
		_databaseConnection = new SqliteConnection($"Filename={Path.Combine(databasePath, "PokemonBattleEngine.db")};Mode=ReadOnly;");
		_databaseConnection.Open();
		_databaseConnection.CreateFunction("StrCmp", (Func<object, object, bool>)StrCmp);
	}
	public static void InitEngine(string databasePath, int? randomSeed = null)
	{
		var cultureInfo = CultureInfo.ReadOnly(CultureInfo.CurrentUICulture);
		if (!cultureInfo.ToPBELanguage(out PBELanguage? lang))
		{
			lang = PBELanguage.English;
		}
		_ = new PBEDefaultDataProvider(databasePath, lang.Value, new PBERandom(randomSeed));
	}
	public static void InitEngine(string databasePath, PBELanguage language, int? randomSeed = null)
	{
		if (language >= PBELanguage.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(language));
		}
		_ = new PBEDefaultDataProvider(databasePath, language, new PBERandom(randomSeed));
	}

	private static bool StrCmp(object arg0, object arg1)
	{
		if (Convert.IsDBNull(arg0) || Convert.IsDBNull(arg1))
		{
			return false;
		}
		return string.Equals(Convert.ToString(arg0), Convert.ToString(arg1), StringComparison.InvariantCultureIgnoreCase);
	}

	// TODO: Keep this internal version and make a public version that only allows operations that retrieve data?
	internal List<T> QueryDatabase<T>(string commandText) where T : new()
	{
		var list = new List<T>();
		Type type = typeof(T);
		lock (_databaseConnectLockObj)
		{
			using (SqliteCommand cmd = _databaseConnection.CreateCommand())
			{
				cmd.CommandText = commandText;
				using (SqliteDataReader r = cmd.ExecuteReader())
				{
					while (r.Read())
					{
						T obj = Activator.CreateInstance<T>();
						for (int i = 0; i < r.FieldCount; i++)
						{
							PropertyInfo? property = type.GetProperty(r.GetName(i));
							property?.SetValue(obj, Convert.ChangeType(r.GetValue(i), property.PropertyType));
						}
						list.Add(obj);
					}
				}
			}
		}
		return list;
	}

	#region Data

	public override bool IsBerry(PBEItem item)
	{
		return PBEDDBerryData.Data.ContainsKey(item);
	}
	public override IPBEBerryData GetBerryData(PBEItem item, bool cache = true)
	{
		return PBEDDBerryData.Data[item];
	}
	public override bool TryGetBerryData(PBEItem item, [NotNullWhen(true)] out IPBEBerryData? bData, bool cache = true)
	{
		if (IsBerry(item))
		{
			bData = GetBerryData(item, cache: cache);
			return true;
		}
		bData = null;
		return false;
	}
	public override IPBEItemData GetItemData(PBEItem item, bool cache = true)
	{
		return PBEDDItemData.Data[item];
	}
	public override IPBEMoveData GetMoveData(PBEMove move, bool cache = true)
	{
		return PBEDDMoveData.Data[move];
	}
	public override bool HasEvolutions(PBESpecies species, PBEForm form, bool cache = true)
	{
		return PBEDDPokemonData.GetData(species, form, cache: cache).HasEvolutions();
	}
	public override IPBEPokemonData GetPokemonData(PBESpecies species, PBEForm form, bool cache = true)
	{
		return PBEDDPokemonData.GetData(species, form, cache: cache);
	}
	public virtual IPBEDDPokemonDataExtended GetPokemonDataExtended(PBESpecies species, PBEForm form, bool cache = true)
	{
		return PBEDDPokemonData.GetData(species, form, cache: cache);
	}

	public override int GetSpeciesCaught()
	{
		return 300;
	}

	public override IReadOnlyCollection<PBEMove> GetLegalMoves(PBESpecies species, PBEForm form, byte level)
	{
		return PBEDDLegalityChecker.GetLegalMoves(species, form, level);
	}

	public virtual IPBEDDPokemonDataExtended GetPokemonDataExtended(IPBESpeciesForm pkmn, bool cache = true)
	{
		return GetPokemonDataExtended(pkmn.Species, pkmn.Form, cache: cache);
	}

	#endregion

	#region EXP

	public override uint GetEXPRequired(PBEGrowthRate type, byte level)
	{
		return PBEDDEXPTables.GetEXPRequired(type, level);
	}
	public override byte GetEXPLevel(PBEGrowthRate type, uint exp)
	{
		return PBEDDEXPTables.GetEXPLevel(type, exp);
	}
	public override float GetEXPModifier(PBEBattle battle)
	{
		return 1;
	}
	public override float GetEXPTradeModifier(PBEBattlePokemon pkmn)
	{
		return 1;
	}

	#endregion

	#region Catching

	public override bool IsDarkGrass(PBEBattle battle)
	{
		return false;
	}
	public override bool IsDuskBallSetting(PBEBattle battle)
	{
		return battle.BattleTerrain == PBEBattleTerrain.Cave;
	}
	public override bool IsFishing(PBEBattle battle)
	{
		return false;
	}
	public override bool IsGuaranteedCapture(PBEBattle battle, IPBESpeciesForm pkmn)
	{
		return IsGuaranteedCapture(battle, pkmn.Species, pkmn.Form);
	}
	public override bool IsGuaranteedCapture(PBEBattle battle, PBESpecies species, PBEForm form)
	{
		return false;
	}
	public override bool IsMoonBallFamily(IPBESpeciesForm pkmn)
	{
		return IsMoonBallFamily(pkmn.Species, pkmn.Form);
	}
	public override bool IsMoonBallFamily(PBESpecies species, PBEForm form)
	{
		return PBEDataUtils.MoonStoneSpecies.Contains(species);
	}
	public override bool IsRepeatBallSpecies(PBESpecies species)
	{
		return false;
	}
	public override bool IsSurfing(PBEBattle battle)
	{
		return battle.BattleTerrain == PBEBattleTerrain.Water;
	}
	public override bool IsUnderwater(PBEBattle battle)
	{
		return false;
	}
	public override float GetCatchRateModifier(PBEBattle battle)
	{
		return 1;
	}

	#endregion

	#region LocalizedString

	public override bool GetAbilityByName(string abilityName, [NotNullWhen(true)] out PBEAbility? ability)
	{
		return PBEDDLocalizedString.GetAbilityByName(abilityName, out ability);
	}
	public virtual IPBEReadOnlyLocalizedString GetAbilityDescription(PBEAbility ability)
	{
		return PBEDDLocalizedString.GetAbilityDescription(ability);
	}
	public override IPBEReadOnlyLocalizedString GetAbilityName(PBEAbility ability)
	{
		return PBEDDLocalizedString.GetAbilityName(ability);
	}
	public override bool GetFormByName(PBESpecies species, string formName, [NotNullWhen(true)] out PBEForm? form)
	{
		return PBEDDLocalizedString.GetFormByName(species, formName, out form);
	}
	public override IPBEReadOnlyLocalizedString GetFormName(PBESpecies species, PBEForm form)
	{
		return PBEDDLocalizedString.GetFormName(species, form);
	}
	public override bool GetGenderByName(string genderName, [NotNullWhen(true)] out PBEGender? gender)
	{
		return PBEDDLocalizedString.GetGenderByName(genderName, out gender);
	}
	public override IPBEReadOnlyLocalizedString GetGenderName(PBEGender gender)
	{
		return PBEDDLocalizedString.GetGenderName(gender);
	}
	public override bool GetItemByName(string itemName, [NotNullWhen(true)] out PBEItem? item)
	{
		return PBEDDLocalizedString.GetItemByName(itemName, out item);
	}
	public virtual IPBEReadOnlyLocalizedString GetItemDescription(PBEItem item)
	{
		return PBEDDLocalizedString.GetItemDescription(item);
	}
	public override IPBEReadOnlyLocalizedString GetItemName(PBEItem item)
	{
		return PBEDDLocalizedString.GetItemName(item);
	}
	public override bool GetMoveByName(string moveName, [NotNullWhen(true)] out PBEMove? move)
	{
		return PBEDDLocalizedString.GetMoveByName(moveName, out move);
	}
	public virtual IPBEReadOnlyLocalizedString GetMoveDescription(PBEMove move)
	{
		return PBEDDLocalizedString.GetMoveDescription(move);
	}
	public override IPBEReadOnlyLocalizedString GetMoveName(PBEMove move)
	{
		return PBEDDLocalizedString.GetMoveName(move);
	}
	public override bool GetNatureByName(string natureName, [NotNullWhen(true)] out PBENature? nature)
	{
		return PBEDDLocalizedString.GetNatureByName(natureName, out nature);
	}
	public override IPBEReadOnlyLocalizedString GetNatureName(PBENature nature)
	{
		return PBEDDLocalizedString.GetNatureName(nature);
	}
	public override bool GetSpeciesByName(string speciesName, [NotNullWhen(true)] out PBESpecies? species)
	{
		return PBEDDLocalizedString.GetSpeciesByName(speciesName, out species);
	}
	public virtual IPBEReadOnlyLocalizedString GetSpeciesCategory(PBESpecies species)
	{
		return PBEDDLocalizedString.GetSpeciesCategory(species);
	}
	public virtual IPBEReadOnlyLocalizedString GetSpeciesEntry(PBESpecies species)
	{
		return PBEDDLocalizedString.GetSpeciesEntry(species);
	}
	public override IPBEReadOnlyLocalizedString GetSpeciesName(PBESpecies species)
	{
		return PBEDDLocalizedString.GetSpeciesName(species);
	}
	public override bool GetStatByName(string statName, [NotNullWhen(true)] out PBEStat? stat)
	{
		return PBEDDLocalizedString.GetStatByName(statName, out stat);
	}
	public override IPBEReadOnlyLocalizedString GetStatName(PBEStat stat)
	{
		return PBEDDLocalizedString.GetStatName(stat);
	}
	public override bool GetTypeByName(string typeName, [NotNullWhen(true)] out PBEType? type)
	{
		return PBEDDLocalizedString.GetTypeByName(typeName, out type);
	}
	public override IPBEReadOnlyLocalizedString GetTypeName(PBEType type)
	{
		return PBEDDLocalizedString.GetTypeName(type);
	}

	#endregion
}
