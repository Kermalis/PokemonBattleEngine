using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

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

        private PBELocalizedString(SearchResult result)
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

        public string FromUICultureInfo()
        {
            return FromCultureInfo(Thread.CurrentThread.CurrentUICulture);
        }
        public string FromCultureInfo(CultureInfo cultureInfo)
        {
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

        #region Database Querying

        private class SearchResult
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
        private const string queryText = "SELECT * FROM {0} WHERE StrCmp(English,'{1}') OR StrCmp(French,'{1}') OR StrCmp(German,'{1}') OR StrCmp(Italian,'{1}') OR StrCmp(Japanese_Kana,'{1}') OR StrCmp(Japanese_Kanji,'{1}') OR StrCmp(Korean,'{1}') OR StrCmp(Spanish,'{1}')";
        private const string queryId = "SELECT * FROM {0} WHERE Id={1}";
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
            List<SearchResult> results = PBEUtils.QueryDatabase<SearchResult>(string.Format(queryText, "AbilityNames", abilityName));
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
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "AbilityDescriptions", (byte)ability))[0]);
        }
        public static PBELocalizedString GetAbilityName(PBEAbility ability)
        {
            if (ability >= PBEAbility.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(ability));
            }
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "AbilityNames", (byte)ability))[0]);
        }
        public static PBEGender? GetGenderByName(string genderName)
        {
            PBEGender gender;
            List<SearchResult> results = PBEUtils.QueryDatabase<SearchResult>(string.Format(queryText, "GenderNames", genderName));
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
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "GenderNames", (byte)gender))[0]);
        }
        public static PBEItem? GetItemByName(string itemName)
        {
            PBEItem item;
            List<SearchResult> results = PBEUtils.QueryDatabase<SearchResult>(string.Format(queryText, "ItemNames", itemName));
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
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "ItemDescriptions", (ushort)item))[0]);
        }
        public static PBELocalizedString GetItemName(PBEItem item)
        {
            if (!Enum.IsDefined(typeof(PBEItem), item))
            {
                throw new ArgumentOutOfRangeException(nameof(item));
            }
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "ItemNames", (ushort)item))[0]);
        }
        public static PBEMove? GetMoveByName(string moveName)
        {
            PBEMove move;
            List<SearchResult> results = PBEUtils.QueryDatabase<SearchResult>(string.Format(queryText, "MoveNames", moveName));
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
            if (move >= PBEMove.MAX || !Enum.IsDefined(typeof(PBEMove), move))
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "MoveDescriptions", (ushort)move))[0]);
        }
        public static PBELocalizedString GetMoveName(PBEMove move)
        {
            if (move >= PBEMove.MAX || !Enum.IsDefined(typeof(PBEMove), move))
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "MoveNames", (ushort)move))[0]);
        }
        public static PBENature? GetNatureByName(string natureName)
        {
            PBENature nature;
            List<SearchResult> results = PBEUtils.QueryDatabase<SearchResult>(string.Format(queryText, "NatureNames", natureName));
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
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "NatureNames", (byte)nature))[0]);
        }
        public static PBESpecies? GetSpeciesByName(string speciesName)
        {
            PBESpecies species;
            List<SearchResult> results = PBEUtils.QueryDatabase<SearchResult>(string.Format(queryText, "SpeciesNames", speciesName));
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
            uint speciesId = (ushort)species;
            if (!Enum.IsDefined(typeof(PBESpecies), speciesId))
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "SpeciesCategories", speciesId))[0]);
        }
        public static PBELocalizedString GetSpeciesEntry(PBESpecies species)
        {
            uint speciesId = (ushort)species;
            if (!Enum.IsDefined(typeof(PBESpecies), speciesId))
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "SpeciesEntries", speciesId))[0]);
        }
        public static PBELocalizedString GetSpeciesName(PBESpecies species)
        {
            uint speciesId = (ushort)species;
            if (!Enum.IsDefined(typeof(PBESpecies), speciesId))
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "SpeciesNames", speciesId))[0]);
        }
        public static PBEStat? GetStatByName(string statName)
        {
            PBEStat stat;
            List<SearchResult> results = PBEUtils.QueryDatabase<SearchResult>(string.Format(queryText, "StatNames", statName));
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
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "StatNames", (byte)stat))[0]);
        }
        public static PBEType? GetTypeByName(string typeName)
        {
            PBEType type;
            List<SearchResult> results = PBEUtils.QueryDatabase<SearchResult>(string.Format(queryText, "TypeNames", typeName));
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
            if (!Enum.IsDefined(typeof(PBEType), type))
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
            return new PBELocalizedString(PBEUtils.QueryDatabase<SearchResult>(string.Format(queryId, "TypeNames", (byte)type))[0]);
        }

        #endregion
    }
}
