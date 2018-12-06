using Discord;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngineDiscord
{
    static class Utils
    {
        public static string Print<T>(this IEnumerable<T> source, bool parenthesis = true)
        {
            string str = parenthesis ? "( " : "";
            str += string.Join(", ", source);
            str += parenthesis ? " )" : "";
            return str;
        }

        // https://stackoverflow.com/a/3722337
        public static Color Blend(this Color color, Color backColor, double depth = 0.5)
        {
            byte r = (byte)((color.R * depth) + backColor.R * (1 - depth));
            byte g = (byte)((color.G * depth) + backColor.G * (1 - depth));
            byte b = (byte)((color.B * depth) + backColor.B * (1 - depth));
            return new Color(r, g, b);
        }
    }
}
