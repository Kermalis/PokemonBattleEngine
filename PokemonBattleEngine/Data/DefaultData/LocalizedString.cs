using Kermalis.PokemonBattleEngine.Data.Legality;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBELocalizedString
    {
        public string English { get; }
        public string French { get; }
        public string German { get; }
        public string Italian { get; }
        public string Japanese_Kana { get; }
        public string Japanese_Kanji { get; }
        public string Korean { get; }
        public string Spanish { get; }

        private PBELocalizedString(ISearchResult result)
        {
            English = result.English;
            French = result.French;
            German = result.German;
            Italian = result.Italian;
            Japanese_Kana = result.Japanese_Kana;
            Japanese_Kanji = result.Japanese_Kanji;
            Korean = result.Korean;
            Spanish = result.Spanish;
        }

        public string FromCultureInfo(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }
            string id = cultureInfo.TwoLetterISOLanguageName;
            switch (id)
            {
                case "en": return English;
                case "fr": return French;
                case "de": return German;
                case "it": return Italian;
                case "ja": return Japanese_Kana;
                case "ko": return Korean;
                case "es": return Spanish;
                default: throw new ArgumentOutOfRangeException(nameof(cultureInfo));
            }
        }
        public static bool IsCultureValid(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }
            string id = cultureInfo.TwoLetterISOLanguageName;
            return id == "en" || id == "fr" || id == "de" || id == "it" || id == "ja" || id == "ko" || id == "es";
        }

        public override string ToString()
        {
            return FromCultureInfo(PBEDataProvider.PBECulture);
        }

        #region Database Querying

        private interface ISearchResult
        {
            string English { get; set; }
            string French { get; set; }
            string German { get; set; }
            string Italian { get; set; }
            string Japanese_Kana { get; set; }
            string Japanese_Kanji { get; set; }
            string Korean { get; set; }
            string Spanish { get; set; }
        }
        private class SearchResult : ISearchResult
        {
            public uint Id { get; set; }
            public string English { get; set; }
            public string French { get; set; }
            public string German { get; set; }
            public string Italian { get; set; }
            public string Japanese_Kana { get; set; }
            public string Japanese_Kanji { get; set; }
            public string Korean { get; set; }
            public string Spanish { get; set; }
        }
        private class FormNameSearchResult : ISearchResult
        {
            public ushort Species { get; set; }
            public byte Form { get; set; }
            public string English { get; set; }
            public string French { get; set; }
            public string German { get; set; }
            public string Italian { get; set; }
            public string Japanese_Kana { get; set; }
            public string Japanese_Kanji { get; set; }
            public string Korean { get; set; }
            public string Spanish { get; set; }
        }
        private const string QueryText = "SELECT * FROM {0} WHERE StrCmp(English,'{1}') OR StrCmp(French,'{1}') OR StrCmp(German,'{1}') OR StrCmp(Italian,'{1}') OR StrCmp(Japanese_Kana,'{1}') OR StrCmp(Japanese_Kanji,'{1}') OR StrCmp(Korean,'{1}') OR StrCmp(Spanish,'{1}')";
        private const string QueryId = "SELECT * FROM {0} WHERE Id={1}";
        private const string QuerySpeciesAndText = "SELECT * FROM {0} WHERE (StrCmp(English,'{1}') OR StrCmp(French,'{1}') OR StrCmp(German,'{1}') OR StrCmp(Italian,'{1}') OR StrCmp(Japanese_Kana,'{1}') OR StrCmp(Japanese_Kanji,'{1}') OR StrCmp(Korean,'{1}') OR StrCmp(Spanish,'{1}')) AND (Species={2})";
        private const string QuerySpecies = "SELECT * FROM {0} WHERE Species={1} AND Form={2}";
        private static bool GetEnumValue<TEnum>(string value, out TEnum result) where TEnum : struct
        {
            foreach (TEnum v in Enum.GetValues(typeof(TEnum)))
            {
                if (v.ToString().Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    result = v;
                    return true;
                }
            }
            result = default;
            return false;
        }

        public static PBEAbility? GetAbilityByName(string abilityName)
        {
            PBEAbility ability;
            List<SearchResult> results = PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryText, "AbilityNames", abilityName));
            if (results.Count == 1)
            {
                ability = (PBEAbility)results[0].Id;
            }
            else if (!GetEnumValue(abilityName, out ability) || ability == PBEAbility.MAX)
            {
                return null;
            }
            return ability;
        }
        public static PBELocalizedString GetAbilityDescription(PBEAbility ability)
        {
            if (ability >= PBEAbility.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(ability));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "AbilityDescriptions", (byte)ability))[0]);
        }
        public static PBELocalizedString GetAbilityName(PBEAbility ability)
        {
            if (ability >= PBEAbility.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(ability));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "AbilityNames", (byte)ability))[0]);
        }
        public static PBEForm? GetFormByName(PBESpecies species, string formName)
        {
            PBEForm form;
            List<FormNameSearchResult> results = PBEDataProvider.QueryDatabase<FormNameSearchResult>(string.Format(QuerySpeciesAndText, "FormNames", formName, (ushort)species));
            if (results.Count == 1)
            {
                form = (PBEForm)results[0].Form;
            }
            else if (!GetEnumValue(formName, out form))
            {
                return null;
            }
            return form;
        }
        public static PBELocalizedString GetFormName(PBESpecies species, PBEForm form)
        {
            PBELegalityChecker.ValidateSpecies(species, form, false);
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<FormNameSearchResult>(string.Format(QuerySpecies, "FormNames", (ushort)species, (byte)form))[0]);
        }
        public static PBEGender? GetGenderByName(string genderName)
        {
            PBEGender gender;
            List<SearchResult> results = PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryText, "GenderNames", genderName));
            if (results.Count == 1)
            {
                gender = (PBEGender)results[0].Id;
            }
            else if (!GetEnumValue(genderName, out gender) || gender == PBEGender.MAX)
            {
                return null;
            }
            return gender;
        }
        public static PBELocalizedString GetGenderName(PBEGender gender)
        {
            if (gender >= PBEGender.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(gender));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "GenderNames", (byte)gender))[0]);
        }
        public static PBEItem? GetItemByName(string itemName)
        {
            PBEItem item;
            List<SearchResult> results = PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryText, "ItemNames", itemName));
            if (results.Count == 1)
            {
                item = (PBEItem)results[0].Id;
            }
            else if (!GetEnumValue(itemName, out item))
            {
                return null;
            }
            return item;
        }
        public static PBELocalizedString GetItemDescription(PBEItem item)
        {
            if (!Enum.IsDefined(typeof(PBEItem), item))
            {
                throw new ArgumentOutOfRangeException(nameof(item));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "ItemDescriptions", (ushort)item))[0]);
        }
        public static PBELocalizedString GetItemName(PBEItem item)
        {
            if (!Enum.IsDefined(typeof(PBEItem), item))
            {
                throw new ArgumentOutOfRangeException(nameof(item));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "ItemNames", (ushort)item))[0]);
        }
        public static PBEMove? GetMoveByName(string moveName)
        {
            PBEMove move;
            List<SearchResult> results = PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryText, "MoveNames", moveName));
            if (results.Count == 1)
            {
                move = (PBEMove)results[0].Id;
            }
            else if (!GetEnumValue(moveName, out move) || move == PBEMove.MAX)
            {
                return null;
            }
            return move;
        }
        public static PBELocalizedString GetMoveDescription(PBEMove move)
        {
            if (move >= PBEMove.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "MoveDescriptions", (ushort)move))[0]);
        }
        public static PBELocalizedString GetMoveName(PBEMove move)
        {
            if (move >= PBEMove.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "MoveNames", (ushort)move))[0]);
        }
        public static PBENature? GetNatureByName(string natureName)
        {
            PBENature nature;
            List<SearchResult> results = PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryText, "NatureNames", natureName));
            if (results.Count == 1)
            {
                nature = (PBENature)results[0].Id;
            }
            else if (!GetEnumValue(natureName, out nature) || nature == PBENature.MAX)
            {
                return null;
            }
            return nature;
        }
        public static PBELocalizedString GetNatureName(PBENature nature)
        {
            if (nature >= PBENature.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(nature));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "NatureNames", (byte)nature))[0]);
        }
        public static PBESpecies? GetSpeciesByName(string speciesName)
        {
            PBESpecies species;
            List<SearchResult> results = PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryText, "SpeciesNames", speciesName));
            if (results.Count == 1)
            {
                species = (PBESpecies)results[0].Id;
            }
            else if (!GetEnumValue(speciesName, out species))
            {
                return null;
            }
            return species;
        }
        public static PBELocalizedString GetSpeciesCategory(PBESpecies species)
        {
            if (species <= 0 || species >= PBESpecies.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "SpeciesCategories", (ushort)species))[0]);
        }
        public static PBELocalizedString GetSpeciesEntry(PBESpecies species)
        {
            if (species <= 0 || species >= PBESpecies.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "SpeciesEntries", (ushort)species))[0]);
        }
        public static PBELocalizedString GetSpeciesName(PBESpecies species)
        {
            if (species <= 0 || species >= PBESpecies.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "SpeciesNames", (ushort)species))[0]);
        }
        public static PBEStat? GetStatByName(string statName)
        {
            PBEStat stat;
            List<SearchResult> results = PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryText, "StatNames", statName));
            if (results.Count == 1)
            {
                stat = (PBEStat)results[0].Id;
            }
            else if (!GetEnumValue(statName, out stat))
            {
                return null;
            }
            return stat;
        }
        public static PBELocalizedString GetStatName(PBEStat stat)
        {
            if (!Enum.IsDefined(typeof(PBEStat), stat))
            {
                throw new ArgumentOutOfRangeException(nameof(stat));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "StatNames", (byte)stat))[0]);
        }
        public static PBEType? GetTypeByName(string typeName)
        {
            PBEType type;
            List<SearchResult> results = PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryText, "TypeNames", typeName));
            if (results.Count == 1)
            {
                type = (PBEType)results[0].Id;
            }
            else if (!GetEnumValue(typeName, out type))
            {
                return null;
            }
            return type;
        }
        public static PBELocalizedString GetTypeName(PBEType type)
        {
            if (type >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
            return new PBELocalizedString(PBEDataProvider.QueryDatabase<SearchResult>(string.Format(QueryId, "TypeNames", (byte)type))[0]);
        }

        #endregion
    }
}
