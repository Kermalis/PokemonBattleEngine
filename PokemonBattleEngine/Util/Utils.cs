using System;

namespace Kermalis.PokemonBattleEngine.Util
{
    static class PUtils
    {
        public static readonly Random RNG = new Random();

        public static bool ApplyChance(int percentChance) => RNG.Next(0, 100) < percentChance;
    }
}
