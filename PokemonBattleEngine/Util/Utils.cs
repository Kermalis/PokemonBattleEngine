using System;

namespace PokemonBattleEngine.Util
{
    static class Utils
    {
        static readonly Random rng = new Random();

        public static uint GetRandomUint() => (uint)rng.Next();
    }
}
