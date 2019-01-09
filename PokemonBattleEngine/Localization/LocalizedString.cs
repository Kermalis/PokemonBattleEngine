using System;

namespace Kermalis.PokemonBattleEngine.Localization
{
    public sealed class PBELocalizedString
    {
        public string English { get; }
        public string French { get; }
        public string German { get; }
        public string Italian { get; }
        public string Japanese { get; }
        public string Korean { get; }
        public string Spanish { get; }

        internal PBELocalizedString(string japanese, string korean, string french, string german, string spanish, string italian, string english)
        {
            English = english;
            French = french;
            German = german;
            Italian = italian;
            Japanese = japanese;
            Korean = korean;
            Spanish = spanish;
        }

        public bool Contains(string str, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            if (English.Equals(str, comparisonType)
                || French.Equals(str, comparisonType)
                || German.Equals(str, comparisonType)
                || Italian.Equals(str, comparisonType)
                || Japanese.Equals(str, comparisonType)
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
    }
}
