using System;
using System.Globalization;

namespace Kermalis.PokemonBattleEngine.Data
{
    public interface IPBELocalizedString
    {
        string English { get; }
        string French { get; }
        string German { get; }
        string Italian { get; }
        string Japanese_Kana { get; }
        string Japanese_Kanji { get; }
        string Korean { get; }
        string Spanish { get; }
    }

    public static class PBELanguageExtensions
    {
        public static bool IsValidPBELanguage(this CultureInfo cultureInfo)
        {
            string id = cultureInfo.TwoLetterISOLanguageName;
            return id == "en" || id == "fr" || id == "de" || id == "it" || id == "ja" || id == "ko" || id == "es";
        }
        public static PBELanguage ToPBELanguage(this CultureInfo cultureInfo)
        {
            string id = cultureInfo.TwoLetterISOLanguageName;
            switch (id)
            {
                case "en": return PBELanguage.English;
                case "fr": return PBELanguage.French;
                case "de": return PBELanguage.German;
                case "it": return PBELanguage.Italian;
                case "ja": return PBELanguage.Japanese_Kana;
                case "ko": return PBELanguage.Korean;
                case "es": return PBELanguage.Spanish;
                default: throw new ArgumentOutOfRangeException(nameof(cultureInfo));
            }
        }
        public static CultureInfo ToCultureInfo(this PBELanguage language)
        {
            switch (language)
            {
                case PBELanguage.English: return CultureInfo.GetCultureInfo("en-US");
                case PBELanguage.French: return CultureInfo.GetCultureInfo("fr-FR");
                case PBELanguage.German: return CultureInfo.GetCultureInfo("de-DE");
                case PBELanguage.Italian: return CultureInfo.GetCultureInfo("it-IT");
                case PBELanguage.Japanese_Kana:
                case PBELanguage.Japanese_Kanji: return CultureInfo.GetCultureInfo("ja-JP");
                case PBELanguage.Korean: return CultureInfo.GetCultureInfo("ko-KR");
                case PBELanguage.Spanish: return CultureInfo.GetCultureInfo("es-ES");
                default: throw new ArgumentOutOfRangeException(nameof(language));
            }
        }

        public static string FromGlobalLanguage(this IPBELocalizedString str)
        {
            return str.Get(PBEDataProvider.GlobalLanguage);
        }
        public static string Get(this IPBELocalizedString str, PBELanguage lang)
        {
            switch (lang)
            {
                case PBELanguage.English: return str.English;
                case PBELanguage.French: return str.French;
                case PBELanguage.German: return str.German;
                case PBELanguage.Italian: return str.Italian;
                case PBELanguage.Japanese_Kana: return str.Japanese_Kana;
                case PBELanguage.Japanese_Kanji: return str.Japanese_Kanji;
                case PBELanguage.Korean: return str.Korean;
                case PBELanguage.Spanish: return str.Spanish;
                default: throw new ArgumentOutOfRangeException(nameof(lang));
            }
        }
    }
}
