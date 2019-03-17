using Kermalis.PokemonBattleEngine.Data;
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
        public static Random RNG { get; } = new Random(); // TODO: Think about the RNG prediction that comes with public RNG

        /// <summary>
        /// Returns a random boolean from a chance.
        /// </summary>
        /// <example><paramref name="chanceNumerator"/> is 30, <paramref name="chanceDenominator"/> is 100, there is a 30% chance to return True and a 70% chance to return False.</example>
        /// <param name="rand">The number generator to use.</param>
        /// <param name="chanceNumerator">The numerator of the chance.</param>
        /// <param name="chanceDenominator">The denominator of the chance.</param>
        public static bool ApplyChance(this Random rand, int chanceNumerator, int chanceDenominator)
        {
            return rand.Next(0, chanceDenominator) < chanceNumerator;
        }
        /// <summary>
        /// Returns a random boolean.
        /// </summary>
        /// <param name="rand">The number generator to use.</param>
        public static bool NextBoolean(this Random rand)
        {
            return rand.NextDouble() >= 0.5;
        }

        public static bool NextShiny(this Random rand)
        {
            return rand.Next(0, ushort.MaxValue + 1) < 8;
        }
        public static PBEGender NextGender(this Random rand, PBESpecies species)
        {
            PBEPokemonData pData = PBEPokemonData.Data[species];
            switch (pData.GenderRatio)
            {
                case PBEGenderRatio.M7_F1: return rand.ApplyChance(875, 1000) ? PBEGender.Male : PBEGender.Female;
                case PBEGenderRatio.M3_F1: return rand.ApplyChance(750, 1000) ? PBEGender.Male : PBEGender.Female;
                case PBEGenderRatio.M1_F1: return rand.ApplyChance(500, 1000) ? PBEGender.Male : PBEGender.Female;
                case PBEGenderRatio.M1_F3: return rand.ApplyChance(250, 1000) ? PBEGender.Male : PBEGender.Female;
                case PBEGenderRatio.M0_F1: return PBEGender.Female;
                case PBEGenderRatio.M0_F0: return PBEGender.Genderless;
                case PBEGenderRatio.M1_F0: return PBEGender.Male;
                default: throw new ArgumentOutOfRangeException(nameof(pData.GenderRatio));
            }
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
        public static string Print<T>(this IEnumerable<T> source)
        {
            return "( " + string.Join(", ", source) + " )";
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
        public static T Sample<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(RNG.Next(0, source.Count()));
        }
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
        /// Takes a string and removes all invalid file name characters from it.
        /// </summary>
        /// <param name="fileName">The string to clean.</param>
        public static string ToSafeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }
            return fileName;
        }

        internal static byte[] StringToBytes(string str)
        {
            var bytes = new List<byte>();
            byte[] nameBytes = Encoding.Unicode.GetBytes(str);
            bytes.Add((byte)nameBytes.Length);
            bytes.AddRange(nameBytes);
            return bytes.ToArray();
        }
        internal static string StringFromBytes(BinaryReader r)
        {
            return Encoding.Unicode.GetString(r.ReadBytes(r.ReadByte()));
        }
    }
}
