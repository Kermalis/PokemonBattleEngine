using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine
{
    /// <summary>
    /// Contains utilities used in the battle engine.
    /// </summary>
    public static class PBEUtils
    {
        /// <summary>
        /// An ordinary pseudo-random number generator.
        /// </summary>
        public static readonly Random RNG = new Random();

        /// <summary>
        /// Generates a random boolean from a chance.
        /// </summary>
        /// <example><paramref name="chanceNumerator"/> is 30, <paramref name="chanceDenominator"/> is 100, there is a 30% chance to return True and a 70% chance to return False.</example>
        /// <param name="chanceNumerator">The numerator of the chance.</param>
        /// <param name="chanceDenominator">The denominator of the chance.</param>
        public static bool ApplyChance(int chanceNumerator, int chanceDenominator)
        {
            return RNG.Next(0, chanceDenominator) < chanceNumerator;
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
            {
                return min;
            }
            else if (val.CompareTo(max) > 0)
            {
                return max;
            }
            else
            {
                return val;
            }
        }
        public static string Print<T>(this IEnumerable<T> source, bool includeParenthesis)
        {
            string str = includeParenthesis ? "( " : "";
            str += string.Join(", ", source);
            str += includeParenthesis ? " )" : "";
            return str;
        }
        public static string Andify<T>(this IEnumerable<T> source)
        {
            T[] array = source.ToArray();
            string str = array[0].ToString();
            for (int i = 1; i < array.Length; i++)
            {
                if (i == array.Length - 1)
                {
                    if (array.Length > 2)
                    {
                        str += ',';
                    }
                    str += " and ";
                }
                else
                {
                    str += ", ";
                }
                str += array[i].ToString();
            }
            return str;
        }
        public static T Sample<T>(this IEnumerable<T> source) => source.ElementAt(RNG.Next(0, source.Count()));
        /// <summary>
        /// Shuffles the items in a list using the Fisher-Yates Shuffle algorithm.
        /// </summary>
        /// <param name="source">The list to shuffle.</param>
        public static void Shuffle<T>(this IList<T> source)
        {
            for (int a = 0; a < source.Count - 1; a++)
            {
                int b = RNG.Next(a, source.Count);
                T value = source[a];
                source[a] = source[b];
                source[b] = value;
            }
        }
        /// <summary>
        /// Returns a random boolean.
        /// </summary>
        /// <param name="rand"></param>
        /// <returns>A random boolean.</returns>
        public static bool NextBoolean(this Random rand)
        {
            return rand.NextDouble() >= 0.5;
        }

        internal static byte[] StringToBytes(string str)
        {
            var bytes = new List<byte>();
            byte[] nameBytes = Encoding.ASCII.GetBytes(str);
            bytes.Add((byte)nameBytes.Length);
            bytes.AddRange(nameBytes);
            return bytes.ToArray();
        }
        internal static string StringFromBytes(BinaryReader r)
        {
            return Encoding.ASCII.GetString(r.ReadBytes(r.ReadByte()));
        }
    }
}
