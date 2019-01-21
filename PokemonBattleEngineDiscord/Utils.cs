using Discord;
using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;
using System.Net;

namespace Kermalis.PokemonBattleEngineDiscord
{
    static class Utils
    {
        public static Dictionary<PBEType, Color> TypeToColor { get; } = new Dictionary<PBEType, Color>
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

        // https://stackoverflow.com/questions/1979915/can-i-check-if-a-file-exists-at-a-url
        public static bool URLExists(string url)
        {
            bool result = false;
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Timeout = 2000;
            webRequest.Method = "HEAD";
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                result = true;
            }
            catch { }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }
        public static string GetPokemonSprite(PBESpecies species, bool shiny, PBEGender gender, bool behindSubstitute, bool backSprite)
        {
            string orientation = backSprite ? "-B" : "-F";
            if (behindSubstitute)
            {
                return $"https://raw.githubusercontent.com/Kermalis/PokemonBattleEngine/master/PokemonBattleEngineClient/Assets/Pokemon_Sprites/Substitute{orientation}.gif";
            }
            else
            {
                uint speciesID = (uint)species & 0xFFFF;
                uint formeID = (uint)species >> 0x10;
                string sss = $"{speciesID}{(formeID > 0 ? $"-{formeID}" : string.Empty)}{orientation}{(shiny ? "-S" : string.Empty)}";
                // Following will be false if the species sprites are sss-M.gif and sss-F.gif
                bool spriteIsGenderNeutral = URLExists($"https://raw.githubusercontent.com/Kermalis/PokemonBattleEngine/master/PokemonBattleEngineClient/Assets/Pokemon_Sprites/{sss}.gif");
                string genderStr = spriteIsGenderNeutral ? string.Empty : gender == PBEGender.Female ? "-F" : "-M";
                return $"https://raw.githubusercontent.com/Kermalis/PokemonBattleEngine/master/PokemonBattleEngineClient/Assets/Pokemon_Sprites/{sss}{genderStr}.gif";
            }
        }
    }
}
