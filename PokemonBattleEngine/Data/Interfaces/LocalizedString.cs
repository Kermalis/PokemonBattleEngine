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

    public static class PBELocalizedStringExtensions
    {
        public static string FromPBECultureInfo(this IPBELocalizedString str)
        {
            return str.FromCultureInfo(PBEDataProvider.PBECulture);
        }
        public static string FromCultureInfo(this IPBELocalizedString str, CultureInfo cultureInfo)
        {
            if (cultureInfo is null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }
            string id = cultureInfo.TwoLetterISOLanguageName;
            switch (id)
            {
                case "en": return str.English;
                case "fr": return str.French;
                case "de": return str.German;
                case "it": return str.Italian;
                case "ja": return str.Japanese_Kana;
                case "ko": return str.Korean;
                case "es": return str.Spanish;
                default: throw new ArgumentOutOfRangeException(nameof(cultureInfo));
            }
        }
    }
}
