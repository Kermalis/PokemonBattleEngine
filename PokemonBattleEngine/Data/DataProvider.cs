using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Kermalis.PokemonBattleEngine.Data
{
    public abstract class PBEDataProvider
    {
        public static PBEDataProvider Instance { get; private set; } = null!;

        public static PBELanguage GlobalLanguage { get; private set; } = default;
        public static PBERandom GlobalRandom { get; private set; } = null!;

        protected void Init(PBELanguage lang, PBERandom rand)
        {
            if (lang >= PBELanguage.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(lang));
            }
            GlobalLanguage = lang;
            GlobalRandom = rand;
            Instance = this;
        }

        #region Data

        public abstract bool IsBerry(PBEItem item);
        public abstract IPBEBerryData GetBerryData(PBEItem item, bool cache = true);
        public abstract bool TryGetBerryData(PBEItem item, [NotNullWhen(true)] out IPBEBerryData? bData, bool cache = true);
        public abstract IPBEItemData GetItemData(PBEItem item, bool cache = true);
        public abstract IPBEMoveData GetMoveData(PBEMove move, bool cache = true);
        public abstract bool HasEvolutions(IPBESpeciesForm pkmn, bool cache = true);
        public abstract bool HasEvolutions(PBESpecies species, PBEForm form, bool cache = true);
        public abstract IPBEPokemonData GetPokemonData(IPBESpeciesForm pkmn, bool cache = true);
        public abstract IPBEPokemonData GetPokemonData(PBESpecies species, PBEForm form, bool cache = true);
        public abstract IPBEPokemonDataExtended GetPokemonDataExtended(IPBESpeciesForm pkmn, bool cache = true);
        public abstract IPBEPokemonDataExtended GetPokemonDataExtended(PBESpecies species, PBEForm form, bool cache = true);

        public abstract int GetSpeciesCaught();

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
        public abstract bool IsGuaranteedCapture(PBEBattle battle, IPBESpeciesForm pkmn);
        public abstract bool IsGuaranteedCapture(PBEBattle battle, PBESpecies species, PBEForm form);
        public abstract bool IsMoonBallFamily(IPBESpeciesForm pkmn);
        public abstract bool IsMoonBallFamily(PBESpecies species, PBEForm form);
        public abstract bool IsRepeatBallSpecies(PBESpecies species);
        public abstract bool IsSurfing(PBEBattle battle);
        public abstract bool IsUnderwater(PBEBattle battle);
        /// <summary>This is the boost to the catch rate. In generation 5, Capture Powers boost the catch rate.</summary>
        public abstract float GetCatchRateModifier(PBEBattle battle);

        #endregion

        #region LocalizedString

        public abstract bool GetAbilityByName(string abilityName, [NotNullWhen(true)] out PBEAbility? ability);
        public abstract IPBELocalizedString GetAbilityDescription(PBEAbility ability);
        public abstract IPBELocalizedString GetAbilityName(PBEAbility ability);
        public abstract bool GetFormByName(PBESpecies species, string formName, [NotNullWhen(true)] out PBEForm? form);
        public abstract IPBELocalizedString GetFormName(IPBESpeciesForm pkmn);
        public abstract IPBELocalizedString GetFormName(PBESpecies species, PBEForm form);
        public abstract bool GetGenderByName(string genderName, [NotNullWhen(true)] out PBEGender? gender);
        public abstract IPBELocalizedString GetGenderName(PBEGender gender);
        public abstract bool GetItemByName(string itemName, [NotNullWhen(true)] out PBEItem? item);
        public abstract IPBELocalizedString GetItemDescription(PBEItem item);
        public abstract IPBELocalizedString GetItemName(PBEItem item);
        public abstract bool GetMoveByName(string moveName, [NotNullWhen(true)] out PBEMove? move);
        public abstract IPBELocalizedString GetMoveDescription(PBEMove move);
        public abstract IPBELocalizedString GetMoveName(PBEMove move);
        public abstract bool GetNatureByName(string natureName, [NotNullWhen(true)] out PBENature? nature);
        public abstract IPBELocalizedString GetNatureName(PBENature nature);
        public abstract bool GetSpeciesByName(string speciesName, [NotNullWhen(true)] out PBESpecies? species);
        public abstract IPBELocalizedString GetSpeciesCategory(PBESpecies species);
        public abstract IPBELocalizedString GetSpeciesEntry(PBESpecies species);
        public abstract IPBELocalizedString GetSpeciesName(PBESpecies species);
        public abstract bool GetStatByName(string statName, [NotNullWhen(true)] out PBEStat? stat);
        public abstract IPBELocalizedString GetStatName(PBEStat stat);
        public abstract bool GetTypeByName(string typeName, [NotNullWhen(true)] out PBEType? type);
        public abstract IPBELocalizedString GetTypeName(PBEType type);

        #endregion
    }
}
