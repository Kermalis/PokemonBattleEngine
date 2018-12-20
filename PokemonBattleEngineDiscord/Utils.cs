using Discord;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngineDiscord
{
    static class Utils
    {
        public static readonly Dictionary<PBEType, Color> TypeToColor = new Dictionary<PBEType, Color>
        {
            { PBEType.Bug, new Color(173, 189, 31) },
            { PBEType.Dark, new Color(115, 90, 74) },
            { PBEType.Dragon, new Color(123, 99, 231) },
            { PBEType.Electric, new Color(255, 198, 49) },
            { PBEType.Fighting, new Color(165, 82, 57) },
            { PBEType.Fire, new Color(247, 82, 49) },
            { PBEType.Flying, new Color(156, 173, 247) },
            { PBEType.Ghost, new Color(99, 99, 181) },
            { PBEType.Grass, new Color(123, 206, 82) },
            { PBEType.Ground, new Color(214, 181, 90) },
            { PBEType.Ice, new Color(90, 206, 231) },
            { PBEType.Normal, new Color(173, 165, 148) },
            { PBEType.Poison, new Color(181, 90, 165) },
            { PBEType.Psychic, new Color(255, 115, 165) },
            { PBEType.Rock, new Color(189, 165, 90) },
            { PBEType.Steel, new Color(173, 173, 198) },
            { PBEType.Water, new Color(57, 156, 255) }
        };

        // https://stackoverflow.com/a/3722337
        public static Color Blend(this Color color, Color backColor, double depth = 0.5)
        {
            byte r = (byte)((color.R * depth) + backColor.R * (1 - depth));
            byte g = (byte)((color.G * depth) + backColor.G * (1 - depth));
            byte b = (byte)((color.B * depth) + backColor.B * (1 - depth));
            return new Color(r, g, b);
        }
        public static Color GetColor(PBEType type1, PBEType type2)
        {
            Color color = TypeToColor[type1];
            if (type2 != PBEType.None)
            {
                color = color.Blend(TypeToColor[type2]);
            }
            return color;
        }
        public static Color GetColor(PBESpecies species)
        {
            PBEPokemonData pData = PBEPokemonData.Data[species];
            return GetColor(pData.Type1, pData.Type2);
        }
        public static Color GetColor(PBEPokemon pkmn)
        {
            return GetColor(pkmn.Type1, pkmn.Type2);
        }
    }
}
