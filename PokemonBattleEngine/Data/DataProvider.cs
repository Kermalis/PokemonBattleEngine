using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data.Legality;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Kermalis.PokemonBattleEngine.Data;

public abstract class PBEDataProvider
{
	public static PBEDataProvider Instance { get; private set; } = null!;

	public static PBELanguage GlobalLanguage { get; private set; } = default;
	public static PBERandom GlobalRandom { get; private set; } = null!;

	protected PBEDataProvider(PBELanguage language, PBERandom rand)
	{
		if (language >= PBELanguage.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(language));
		}
		GlobalLanguage = language;
		GlobalRandom = rand;
		Instance = this;
	}

	#region Data

	public abstract bool IsBerry(PBEItem item);
	public abstract IPBEBerryData GetBerryData(PBEItem item, bool cache = true);
	public abstract bool TryGetBerryData(PBEItem item, [NotNullWhen(true)] out IPBEBerryData? bData, bool cache = true);
	public abstract IPBEItemData GetItemData(PBEItem item, bool cache = true);
	public abstract IPBEMoveData GetMoveData(PBEMove move, bool cache = true);
	public abstract bool HasEvolutions(PBESpecies species, PBEForm form, bool cache = true);
	public virtual bool HasEvolutions(IPBESpeciesForm pkmn, bool cache = true)
	{
		return HasEvolutions(pkmn.Species, pkmn.Form, cache);
	}
	public abstract IPBEPokemonData GetPokemonData(PBESpecies species, PBEForm form, bool cache = true);

	public abstract int GetSpeciesCaught();

	/// <summary>Used by <see cref="PBELegalMoveset"/>.</summary>
	public abstract IReadOnlyCollection<PBEMove> GetLegalMoves(PBESpecies species, PBEForm form, byte level);

	public virtual IPBEPokemonData GetPokemonData(IPBESpeciesForm pkmn, bool cache = true)
	{
		return GetPokemonData(pkmn.Species, pkmn.Form, cache: cache);
	}

	#endregion

	#region EXP

	public abstract uint GetEXPRequired(PBEGrowthRate type, byte level);
	public abstract byte GetEXPLevel(PBEGrowthRate type, uint exp);
	/// <summary>This is the boost to the EXP rate. In generation 5, Pass Powers boost the EXP rate.</summary>
	public abstract float GetEXPModifier(PBEBattle battle);
	/// <summary>In generation 5, this is 1 for ot, 1.5 for domestic trade, and 1.7 for international trade.</summary>
	public abstract float GetEXPTradeModifier(PBEBattlePokemon pkmn);

	#endregion

	#region Catching

	public abstract bool IsDarkGrass(PBEBattle battle);
	public abstract bool IsDuskBallSetting(PBEBattle battle);
	public abstract bool IsFishing(PBEBattle battle);
	public abstract bool IsGuaranteedCapture(PBEBattle battle, PBESpecies species, PBEForm form);
	public abstract bool IsMoonBallFamily(PBESpecies species, PBEForm form);
	public abstract bool IsRepeatBallSpecies(PBESpecies species);
	public abstract bool IsSurfing(PBEBattle battle);
	public abstract bool IsUnderwater(PBEBattle battle);
	/// <summary>This is the boost to the catch rate. In generation 5, Capture Powers boost the catch rate.</summary>
	public abstract float GetCatchRateModifier(PBEBattle battle);

	public virtual bool IsGuaranteedCapture(PBEBattle battle, IPBESpeciesForm pkmn)
	{
		return IsGuaranteedCapture(battle, pkmn.Species, pkmn.Form);
	}
	public virtual bool IsMoonBallFamily(IPBESpeciesForm pkmn)
	{
		return IsMoonBallFamily(pkmn.Species, pkmn.Form);
	}

	#endregion

	#region LocalizedString

	public abstract bool GetAbilityByName(string abilityName, [NotNullWhen(true)] out PBEAbility? ability);
	public abstract IPBEReadOnlyLocalizedString GetAbilityName(PBEAbility ability);
	public abstract bool GetFormByName(PBESpecies species, string formName, [NotNullWhen(true)] out PBEForm? form);
	public abstract IPBEReadOnlyLocalizedString GetFormName(PBESpecies species, PBEForm form);
	public abstract bool GetGenderByName(string genderName, [NotNullWhen(true)] out PBEGender? gender);
	public abstract IPBEReadOnlyLocalizedString GetGenderName(PBEGender gender);
	public abstract bool GetItemByName(string itemName, [NotNullWhen(true)] out PBEItem? item);
	public abstract IPBEReadOnlyLocalizedString GetItemName(PBEItem item);
	public abstract bool GetMoveByName(string moveName, [NotNullWhen(true)] out PBEMove? move);
	public abstract IPBEReadOnlyLocalizedString GetMoveName(PBEMove move);
	public abstract bool GetNatureByName(string natureName, [NotNullWhen(true)] out PBENature? nature);
	public abstract IPBEReadOnlyLocalizedString GetNatureName(PBENature nature);
	public abstract bool GetSpeciesByName(string speciesName, [NotNullWhen(true)] out PBESpecies? species);
	public abstract IPBEReadOnlyLocalizedString GetSpeciesName(PBESpecies species);
	public abstract bool GetStatByName(string statName, [NotNullWhen(true)] out PBEStat? stat);
	public abstract IPBEReadOnlyLocalizedString GetStatName(PBEStat stat);
	public abstract bool GetTypeByName(string typeName, [NotNullWhen(true)] out PBEType? type);
	public abstract IPBEReadOnlyLocalizedString GetTypeName(PBEType type);

	public virtual IPBEReadOnlyLocalizedString GetFormName(IPBESpeciesForm pkmn)
	{
		return GetFormName(pkmn.Species, pkmn.Form);
	}

	#endregion
}
