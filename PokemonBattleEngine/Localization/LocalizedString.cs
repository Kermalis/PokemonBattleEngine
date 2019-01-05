using System;

namespace Kermalis.PokemonBattleEngine.Localization
{
    public sealed class PBELocalizedString
    {
        public string English { get; }

        internal PBELocalizedString(string english)
        {
            English = english;
        }

        public bool Contains(string str, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            if (English.Equals(str, comparisonType))
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
