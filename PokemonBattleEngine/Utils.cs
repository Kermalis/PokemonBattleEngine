using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kermalis.PokemonBattleEngine
{
    static class PUtils
    {
        public static readonly Random RNG = new Random();

        // Returns true if you are lucky
        public static bool ApplyChance(int chance, int outOf) => RNG.Next(0, outOf) < chance;

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static string Print<T>(this IEnumerable<T> source, bool parenthesis = true)
        {
            string str = parenthesis ? "( " : "";
            str += string.Join(", ", source);
            str += parenthesis ? " )" : "";
            return str;
        }

        public static byte[] StringToBytes(string str)
        {
            var bytes = new List<byte>();
            byte[] nameBytes = Encoding.ASCII.GetBytes(str);
            bytes.Add((byte)nameBytes.Length);
            bytes.AddRange(nameBytes);
            return bytes.ToArray();
        }
        public static string StringFromBytes(BinaryReader r)
        {
            return Encoding.ASCII.GetString(r.ReadBytes(r.ReadByte()));
        }
    }
}
