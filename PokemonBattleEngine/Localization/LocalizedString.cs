using System;
using System.Globalization;
using System.Threading;

namespace Kermalis.PokemonBattleEngine.Localization
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

        internal PBELocalizedString(string english, string french, string german, string italian, string japanese_kana, string japanese_kanji, string korean, string spanish)
        {
            English = english;
            French = french;
            German = german;
            Italian = italian;
            Japanese_Kana = japanese_kana;
            Japanese_Kanji = japanese_kanji;
            Korean = korean;
            Spanish = spanish;
        }

        public bool Contains(string str, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            if (English.Equals(str, comparisonType)
                || French.Equals(str, comparisonType)
                || German.Equals(str, comparisonType)
                || Italian.Equals(str, comparisonType)
                || Japanese_Kana.Equals(str, comparisonType)
                || Japanese_Kanji.Equals(str, comparisonType)
                || Korean.Equals(str, comparisonType)
                || Spanish.Equals(str, comparisonType))
            {
                return true;
            }
            else
            {
                return false;
            }
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
                default: throw new InvalidOperationException();
            }
        }
    }
}
