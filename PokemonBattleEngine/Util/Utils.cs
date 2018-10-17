using System;

namespace Kermalis.PokemonBattleEngine.Util
{
    static class PUtils
    {
        public static readonly Random RNG = new Random();

        public static bool ApplyChance(int percentChance) => RNG.Next(0, 100) < percentChance;

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
