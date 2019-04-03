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

        public string FromCurrentCultureInfo()
        {
            return FromCultureInfo(Thread.CurrentThread.CurrentCulture);
        }
        public string FromUICultureInfo()
        {
            return FromCultureInfo(Thread.CurrentThread.CurrentUICulture);
        }
        public string FromCultureInfo(CultureInfo cultureInfo)
        {
            switch (cultureInfo.TwoLetterISOLanguageName)
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
        public static PBEAbility? GetAbilityByName(string abilityName)
        {
            PBEAbility ability = PBEAbility.None;
            List<SearchResult> results = PBEUtils.DatabaseConnection.Query<SearchResult>(string.Format("SELECT * FROM AbilityNames WHERE StrCmp(English,'{0}') OR StrCmp(French,'{0}') OR StrCmp(German,'{0}') OR StrCmp(Italian,'{0}') OR StrCmp(Japanese_Kana,'{0}') OR StrCmp(Japanese_Kanji,'{0}') OR StrCmp(Korean,'{0}') OR StrCmp(Spanish,'{0}')", abilityName));
            if (results.Count == 1)
            {
                ability = (PBEAbility)results[0].Id;
            }
            else
            {
                Enum.TryParse(abilityName, true, out ability);
            }

            if (ability == PBEAbility.None || ability >= PBEAbility.MAX)
            {
                return null;
            }
            else
            {
                return ability;
            }
        }
        public static PBELocalizedString GetAbilityDescription(PBEAbility ability)
        {
            if (ability >= PBEAbility.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(ability));
            }
            return new PBELocalizedString(PBEUtils.DatabaseConnection.Query<SearchResult>($"SELECT * FROM AbilityDescriptions WHERE Id={(byte)ability}")[0]);
        }
        public static PBELocalizedString GetAbilityName(PBEAbility ability)
        {
            if (ability >= PBEAbility.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(ability));
            }
            return new PBELocalizedString(PBEUtils.DatabaseConnection.Query<SearchResult>($"SELECT * FROM AbilityNames WHERE Id={(byte)ability}")[0]);
        }
        public static PBEItem? GetItemByName(string itemName)
        {
            PBEItem item = PBEItem.None;
            List<SearchResult> results = PBEUtils.DatabaseConnection.Query<SearchResult>(string.Format("SELECT * FROM ItemNames WHERE StrCmp(English,'{0}') OR StrCmp(French,'{0}') OR StrCmp(German,'{0}') OR StrCmp(Italian,'{0}') OR StrCmp(Japanese_Kana,'{0}') OR StrCmp(Japanese_Kanji,'{0}') OR StrCmp(Korean,'{0}') OR StrCmp(Spanish,'{0}')", itemName));
            if (results.Count == 1)
            {
                item = (PBEItem)results[0].Id;
            }
            else
            {
                Enum.TryParse(itemName, true, out item);
            }

            if (item == PBEItem.None)
            {
                return null;
            }
            else
            {
                return item;
            }
        }
        public static PBELocalizedString GetItemDescription(PBEItem item)
        {
            if (!Enum.IsDefined(typeof(PBEItem), item))
            {
                throw new ArgumentOutOfRangeException(nameof(item));
            }
            return new PBELocalizedString(PBEUtils.DatabaseConnection.Query<SearchResult>($"SELECT * FROM ItemDescriptions WHERE Id={(ushort)item}")[0]);
        }
        public static PBELocalizedString GetItemName(PBEItem item)
        {
            if (!Enum.IsDefined(typeof(PBEItem), item))
            {
                throw new ArgumentOutOfRangeException(nameof(item));
            }
            return new PBELocalizedString(PBEUtils.DatabaseConnection.Query<SearchResult>($"SELECT * FROM ItemNames WHERE Id={(ushort)item}")[0]);
        }
        public static PBEMove? GetMoveByName(string moveName)
        {
            PBEMove move = PBEMove.None;
            List<SearchResult> results = PBEUtils.DatabaseConnection.Query<SearchResult>(string.Format("SELECT * FROM MoveNames WHERE StrCmp(English,'{0}') OR StrCmp(French,'{0}') OR StrCmp(German,'{0}') OR StrCmp(Italian,'{0}') OR StrCmp(Japanese_Kana,'{0}') OR StrCmp(Japanese_Kanji,'{0}') OR StrCmp(Korean,'{0}') OR StrCmp(Spanish,'{0}')", moveName));
            if (results.Count == 1)
            {
                move = (PBEMove)results[0].Id;
            }
            else
            {
                Enum.TryParse(moveName, true, out move);
            }

            if (move == PBEMove.None || move >= PBEMove.MAX)
            {
                return null;
            }
            else
            {
                return move;
            }
        }
        public static PBELocalizedString GetMoveDescription(PBEMove move)
        {
            if (move >= PBEMove.MAX || !Enum.IsDefined(typeof(PBEMove), move))
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            return new PBELocalizedString(PBEUtils.DatabaseConnection.Query<SearchResult>($"SELECT * FROM MoveDescriptions WHERE Id={(ushort)move}")[0]);
        }
        public static PBELocalizedString GetMoveName(PBEMove move)
        {
            if (move >= PBEMove.MAX || !Enum.IsDefined(typeof(PBEMove), move))
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            return new PBELocalizedString(PBEUtils.DatabaseConnection.Query<SearchResult>($"SELECT * FROM MoveNames WHERE Id={(ushort)move}")[0]);
        }
        public static PBESpecies? GetSpeciesByName(string speciesName)
        {
            PBESpecies species = 0;
            List<SearchResult> results = PBEUtils.DatabaseConnection.Query<SearchResult>(string.Format("SELECT * FROM SpeciesNames WHERE StrCmp(English,'{0}') OR StrCmp(French,'{0}') OR StrCmp(German,'{0}') OR StrCmp(Italian,'{0}') OR StrCmp(Japanese_Kana,'{0}') OR StrCmp(Japanese_Kanji,'{0}') OR StrCmp(Korean,'{0}') OR StrCmp(Spanish,'{0}')", speciesName));
            if (results.Count == 1)
            {
                species = (PBESpecies)results[0].Id;
            }
            else
            {
                Enum.TryParse(speciesName, true, out species);
            }

            if (species == 0)
            {
                return null;
            }
            else
            {
                return species;
            }
        }
        public static PBELocalizedString GetSpeciesCategory(PBESpecies species)
        {
            uint speciesId = (uint)species & 0xFFFF;
            if (!Enum.IsDefined(typeof(PBESpecies), speciesId))
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            return new PBELocalizedString(PBEUtils.DatabaseConnection.Query<SearchResult>($"SELECT * FROM SpeciesCategories WHERE Id={speciesId}")[0]);
        }
        public static PBELocalizedString GetSpeciesEntry(PBESpecies species)
        {
            uint speciesId = (uint)species & 0xFFFF;
            if (!Enum.IsDefined(typeof(PBESpecies), speciesId))
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            return new PBELocalizedString(PBEUtils.DatabaseConnection.Query<SearchResult>($"SELECT * FROM SpeciesEntries WHERE Id={speciesId}")[0]);
        }
        public static PBELocalizedString GetSpeciesName(PBESpecies species)
        {
            uint speciesId = (uint)species & 0xFFFF;
            if (!Enum.IsDefined(typeof(PBESpecies), speciesId))
            {
                throw new ArgumentOutOfRangeException(nameof(species));
            }
            return new PBELocalizedString(PBEUtils.DatabaseConnection.Query<SearchResult>($"SELECT * FROM SpeciesNames WHERE Id={speciesId}")[0]);
        }

        #endregion
    }
}
