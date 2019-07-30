using System;

namespace Kermalis.PokemonBattleEngineServer
{
    internal static class Utils
    {
        private static readonly Random rand = new Random();
        public static T RandomElement<T>(this T[] source)
        {
            int count = source.Length;
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(source), $"\"{nameof(source)}\" must have at least one element.");
            }
            return source[rand.Next(count)];
        }
    }
}
