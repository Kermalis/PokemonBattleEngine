using Newtonsoft.Json.Linq;
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
        public static string Andify<T>(this IReadOnlyList<T> source)
        {
            string str = source[0]?.ToString() ?? string.Empty;
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
                str += source[i]?.ToString() ?? string.Empty;
            }
            return str;
        }
        public static IEnumerable<T> ExceptOne<T>(this IEnumerable<T> source, T one)
        {
            foreach (T t in source)
            {
                if (Equals(t, one))
                {
                    yield return t;
                }
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

        internal static JToken GetSafe(this JToken j, string key)
        {
            JToken? ret = j[key];
            if (ret is null)
            {
                throw new InvalidDataException($"\"{key}\" was not found");
            }
            return ret;
        }
        internal static string GetSafeString(this JToken j, string key)
        {
            string? ret = j.GetSafe(key).Value<string>();
            if (ret is null)
            {
                throw new InvalidDataException($"\"{key}\" is null");
            }
            return ret;
        }
    }
}
