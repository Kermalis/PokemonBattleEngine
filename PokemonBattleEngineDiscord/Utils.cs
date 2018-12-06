using Discord;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngineDiscord
{
    static class Utils
    {
        public static readonly Dictionary<PType, Color> TypeToColor = new Dictionary<PType, Color>
        {
            { PType.Bug, new Color(173, 189, 31) },
            { PType.Dark, new Color(115, 90, 74) },
            { PType.Dragon, new Color(123, 99, 231) },
            { PType.Electric, new Color(255, 198, 49) },
            { PType.Fighting, new Color(165, 82, 57) },
            { PType.Fire, new Color(247, 82, 49) },
            { PType.Flying, new Color(156, 173, 247) },
            { PType.Ghost, new Color(99, 99, 181) },
            { PType.Grass, new Color(123, 206, 82) },
            { PType.Ground, new Color(214, 181, 90) },
            { PType.Ice, new Color(90, 206, 231) },
            { PType.Normal, new Color(173, 165, 148) },
            { PType.Poison, new Color(181, 90, 165) },
            { PType.Psychic, new Color(255, 115, 165) },
            { PType.Rock, new Color(189, 165, 90) },
            { PType.Steel, new Color(173, 173, 198) },
            { PType.Water, new Color(57, 156, 255) }
        };

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
        public static Color GetColor(PType type1, PType type2)
        {
            Color color = TypeToColor[type1];
            if (type2 != PType.None)
            {
                color = color.Blend(TypeToColor[type2]);
            }
            return color;
        }
        public static Color GetColor(PSpecies species)
        {
            PPokemonData pData = PPokemonData.Data[species];
            return GetColor(pData.Type1, pData.Type2);
        }
        public static Color GetColor(PPokemon pkmn)
        {
            return GetColor(pkmn.Type1, pkmn.Type2);
        }
    }
}
