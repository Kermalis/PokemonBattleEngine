using Kermalis.PokemonBattleEngine.Data.Legality;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Data.DefaultData
{
    public sealed class PBELocalizedString : IPBELocalizedString
    {
        public string English { get; }
        public string French { get; }
        public string German { get; }
        public string Italian { get; }
        public string Japanese_Kana { get; }
        public string Japanese_Kanji { get; }
        public string Korean { get; }
        public string Spanish { get; }

        private PBELocalizedString(IPBELocalizedString other)
        {
            English = other.English;
            French = other.French;
            German = other.German;
            Italian = other.Italian;
            Japanese_Kana = other.Japanese_Kana;
            Japanese_Kanji = other.Japanese_Kanji;
            Korean = other.Korean;
            Spanish = other.Spanish;
        }

        public override string ToString()
        {
            return this.FromGlobalLanguage();
        }

        #region Database Querying

        private interface ISearchResult : IPBELocalizedString
        {
            new string English { get; set; }
            new string French { get; set; }
            new string German { get; set; }
            new string Italian { get; set; }
            new string Japanese_Kana { get; set; }
            new string Japanese_Kanji { get; set; }
            new string Korean { get; set; }
            new string Spanish { get; set; }
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private const string QueryText = "SELECT * FROM {0} WHERE StrCmp(English,'{1}') OR StrCmp(French,'{1}') OR StrCmp(German,'{1}') OR StrCmp(Italian,'{1}') OR StrCmp(Japanese_Kana,'{1}') OR StrCmp(Japanese_Kanji,'{1}') OR StrCmp(Korean,'{1}') OR StrCmp(Spanish,'{1}')";
        private const string QueryId = "SELECT * FROM {0} WHERE Id={1}";
        private const string QuerySpeciesAndText = "SELECT * FROM {0} WHERE (StrCmp(English,'{1}') OR StrCmp(French,'{1}') OR StrCmp(German,'{1}') OR StrCmp(Italian,'{1}') OR StrCmp(Japanese_Kana,'{1}') OR StrCmp(Japanese_Kanji,'{1}') OR StrCmp(Korean,'{1}') OR StrCmp(Spanish,'{1}')) AND (Species={2})";
        private const string QuerySpecies = "SELECT * FROM {0} WHERE Species={1} AND Form={2}";
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
                ability = (PBEAbility)results[0].Id;
            }
            else if (!GetEnumValue(abilityName, out ability) || ability == PBEAbility.MAX)
            {
                ability = null;
                return false;
            }
            return true;
        }
        public static PBELocalizedString GetAbilityDescription(PBEAbility ability)
        {
            if (ability >= PBEAbility.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(ability));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "AbilityDescriptions", (byte)ability));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static PBELocalizedString GetAbilityName(PBEAbility ability)
        {
            if (ability >= PBEAbility.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(ability));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "AbilityNames", (byte)ability));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static bool GetFormByName(PBESpecies species, string formName, [NotNullWhen(true)] out PBEForm? form)
        {
            List<FormNameSearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<FormNameSearchResult>(string.Format(QuerySpeciesAndText, "FormNames", formName, (ushort)species));
            if (results.Count == 1)
            {
                form = (PBEForm)results[0].Form;
            }
            else if (!GetEnumValue(formName, out form))
            {
                form = null;
                return false;
            }
            return true;
        }
        public static PBELocalizedString GetFormName(PBESpecies species, PBEForm form)
        {
            PBELegalityChecker.ValidateSpecies(species, form, false);
            List<FormNameSearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<FormNameSearchResult>(string.Format(QuerySpecies, "FormNames", (ushort)species, (byte)form));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static bool GetGenderByName(string genderName, [NotNullWhen(true)] out PBEGender? gender)
        {
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "GenderNames", genderName));
            if (results.Count == 1)
            {
                gender = (PBEGender)results[0].Id;
            }
            else if (!GetEnumValue(genderName, out gender) || gender == PBEGender.MAX)
            {
                gender = null;
                return false;
            }
            return true;
        }
        public static PBELocalizedString GetGenderName(PBEGender gender)
        {
            if (gender >= PBEGender.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(gender));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "GenderNames", (byte)gender));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static bool GetItemByName(string itemName, [NotNullWhen(true)] out PBEItem? item)
        {
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "ItemNames", itemName));
            if (results.Count == 1)
            {
                item = (PBEItem)results[0].Id;
            }
            else if (!GetEnumValue(itemName, out item))
            {
                item = null;
                return false;
            }
            return true;
        }
        public static PBELocalizedString GetItemDescription(PBEItem item)
        {
            if (!Enum.IsDefined(item))
            {
                throw new ArgumentOutOfRangeException(nameof(item));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "ItemDescriptions", (ushort)item));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static PBELocalizedString GetItemName(PBEItem item)
        {
            if (!Enum.IsDefined(item))
            {
                throw new ArgumentOutOfRangeException(nameof(item));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "ItemNames", (ushort)item));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static bool GetMoveByName(string moveName, [NotNullWhen(true)] out PBEMove? move)
        {
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "MoveNames", moveName));
            if (results.Count == 1)
            {
                move = (PBEMove)results[0].Id;
            }
            else if (!GetEnumValue(moveName, out move) || move == PBEMove.MAX)
            {
                move = null;
                return false;
            }
            return true;
        }
        public static PBELocalizedString GetMoveDescription(PBEMove move)
        {
            if (move >= PBEMove.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "MoveDescriptions", (ushort)move));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static PBELocalizedString GetMoveName(PBEMove move)
        {
            if (move >= PBEMove.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "MoveNames", (ushort)move));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static bool GetNatureByName(string natureName, [NotNullWhen(true)] out PBENature? nature)
        {
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "NatureNames", natureName));
            if (results.Count == 1)
            {
                nature = (PBENature)results[0].Id;
            }
            else if (!GetEnumValue(natureName, out nature) || nature == PBENature.MAX)
            {
                nature = null;
                return false;
            }
            return true;
        }
        public static PBELocalizedString GetNatureName(PBENature nature)
        {
            if (nature >= PBENature.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(nature));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "NatureNames", (byte)nature));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static bool GetSpeciesByName(string speciesName, [NotNullWhen(true)] out PBESpecies? species)
        {
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "SpeciesNames", speciesName));
            if (results.Count == 1)
            {
                species = (PBESpecies)results[0].Id;
            }
            else if (!GetEnumValue(speciesName, out species))
            {
                species = null;
                return false;
            }
            return true;
        }
        public static PBELocalizedString GetSpeciesCategory(PBESpecies species)
        {
            if (species <= 0 || species >= PBESpecies.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "SpeciesCategories", (ushort)species));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static PBELocalizedString GetSpeciesEntry(PBESpecies species)
        {
            if (species <= 0 || species >= PBESpecies.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "SpeciesEntries", (ushort)species));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static PBELocalizedString GetSpeciesName(PBESpecies species)
        {
            if (species <= 0 || species >= PBESpecies.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "SpeciesNames", (ushort)species));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static bool GetStatByName(string statName, [NotNullWhen(true)] out PBEStat? stat)
        {
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "StatNames", statName));
            if (results.Count == 1)
            {
                stat = (PBEStat)results[0].Id;
            }
            else if (!GetEnumValue(statName, out stat))
            {
                stat = null;
                return false;
            }
            return true;
        }
        public static PBELocalizedString GetStatName(PBEStat stat)
        {
            if (!Enum.IsDefined(stat))
            {
                throw new ArgumentOutOfRangeException(nameof(stat));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "StatNames", (byte)stat));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }
        public static bool GetTypeByName(string typeName, [NotNullWhen(true)] out PBEType? type)
        {
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryText, "TypeNames", typeName));
            if (results.Count == 1)
            {
                type = (PBEType)results[0].Id;
            }
            else if (!GetEnumValue(typeName, out type))
            {
                type = null;
                return false;
            }
            return true;
        }
        public static PBELocalizedString GetTypeName(PBEType type)
        {
            if (type >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
            List<SearchResult> results = PBEDefaultDataProvider.Instance.QueryDatabase<SearchResult>(string.Format(QueryId, "TypeNames", (byte)type));
            if (results.Count == 1)
            {
                return new PBELocalizedString(results[0]);
            }
            throw new InvalidDataException();
        }

        #endregion
    }
}
