using System;

namespace Kermalis.PokemonBattleEngine.Localization
{
    public sealed class PBELocalizedString
    {
        public string English { get; }
        public string Japanese { get; }

        internal PBELocalizedString(string english, string japanese)
        {
            English = english;
            Japanese = japanese;
        }

        public bool Contains(string str, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            if (English.Equals(str, comparisonType)
                || Japanese.Equals(str, comparisonType))
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
