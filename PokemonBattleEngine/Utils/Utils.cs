using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Utils
{
    /// <summary>A static class that provides utilities that are used throughout the battle engine.</summary>
    public static class PBEUtils
    {
        /// <summary>Returns a <see cref="string"/> that combines <paramref name="source"/>'s elements' string representations using "and" with commas.</summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IReadOnlyList{T}"/> to create a string from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> == null.</exception>
        public static string Andify<T>(this IReadOnlyList<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            string str = source[0].ToString();
            for (int i = 1; i < source.Count; i++)
            {
                if (i == source.Count - 1)
                {
                    if (source.Count > 2)
                    {
                        str += ',';
                    }
                    str += " and ";
                }
                else
                {
                    str += ", ";
                }
                str += source[i].ToString();
            }
            return str;
        }
        internal static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
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
        internal static string Print<T>(this IEnumerable<T> source)
        {
            return "( " + string.Join(", ", source) + " )";
        }
        /// <summary>Removes all invalid file name characters from <paramref name="fileName"/>.</summary>
        internal static string ToSafeFileName(string fileName)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            for (int i = 0; i < invalid.Length; i++)
            {
                fileName = fileName.Replace(invalid[i], '-');
            }
            return fileName;
        }

        public static bool IsOppositeGender(this PBEGender gender, PBEGender otherGender)
        {
            return gender != PBEGender.Genderless && otherGender != PBEGender.Genderless && gender != otherGender;
        }
        public static string ToSymbol(this PBEGender gender)
        {
            return gender == PBEGender.Female ? "♀" : gender == PBEGender.Male ? "♂" : string.Empty;
        }
    }
}
