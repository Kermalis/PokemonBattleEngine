using Discord;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Kermalis.PokemonBattleEngineDiscord
{
    internal static class Utils
    {
        public const string URL = "https://github.com/Kermalis/PokemonBattleEngine";
        public static readonly Dictionary<PBEType, Color> TypeColors = new Dictionary<PBEType, Color>
        {
            { PBEType.None, new Color(173, 165, 148) },
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
        public static readonly Dictionary<PBEType, Emote> TypeEmotes = new Dictionary<PBEType, Emote>
        {
            { PBEType.None, Emote.Parse("<:Normal:708768400167665755>") },
            { PBEType.Bug, Emote.Parse("<:Bug:708768296731934751>") },
            { PBEType.Dark, Emote.Parse("<:Dark:708768299248386109>") },
            { PBEType.Dragon, Emote.Parse("<:Dragon:708768299420483675>") },
            { PBEType.Electric, Emote.Parse("<:Electric:708768297792831549>") },
            { PBEType.Fighting, Emote.Parse("<:Fighting:708768297386246154>") },
            { PBEType.Fire, Emote.Parse("<:Fire:708768299319820369>") },
            { PBEType.Flying, Emote.Parse("<:Flying:708768299252711535>") },
            { PBEType.Ghost, Emote.Parse("<:Ghost:708768299231739964>") },
            { PBEType.Grass, Emote.Parse("<:Grass:708768299319558164>") },
            { PBEType.Ground, Emote.Parse("<:Ground:708768298829086822>") },
            { PBEType.Ice, Emote.Parse("<:Ice:708768398104068158>") },
            { PBEType.Normal, Emote.Parse("<:Normal:708768400167665755>") },
            { PBEType.Poison, Emote.Parse("<:Poison:708768399928590337>") },
            { PBEType.Psychic, Emote.Parse("<:Psychic:708768399161032725>") },
            { PBEType.Rock, Emote.Parse("<:Rock:708768399311765577>") },
            { PBEType.Steel, Emote.Parse("<:Steel:708768399383330867>") },
            { PBEType.Water, Emote.Parse("<:Water:708768402356830268>") }
        };

        // https://stackoverflow.com/a/3722337
        public static Color Blend(this Color color, Color backColor, double depth = 0.5)
        {
            byte r = (byte)((color.R * depth) + (backColor.R * (1 - depth)));
            byte g = (byte)((color.G * depth) + (backColor.G * (1 - depth)));
            byte b = (byte)((color.B * depth) + (backColor.B * (1 - depth)));
            return new Color(r, g, b);
        }
        public static Color GetColor(PBEType type1, PBEType type2)
        {
            Color color = TypeColors[type1];
            if (type2 != PBEType.None)
            {
                color = color.Blend(TypeColors[type2]);
            }
            return color;
        }
        public static Color GetColor(PBEPokemon pkmn)
        {
            return GetColor(pkmn.KnownType1, pkmn.KnownType2);
        }

        private static readonly Random _rand = new Random();
        public static Color RandomColor()
        {
            byte[] bytes = new byte[3];
            _rand.NextBytes(bytes);
            return new Color(bytes[0], bytes[1], bytes[2]);
        }
        public static T RandomElement<T>(this T[] source)
        {
            int count = source.Length;
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(source), $"\"{nameof(source)}\" must have at least one element.");
            }
            return source.ElementAt(_rand.Next(count));
        }

        private static readonly Dictionary<string, bool> _urlCache = new Dictionary<string, bool>();
        // https://stackoverflow.com/questions/1979915/can-i-check-if-a-file-exists-at-a-url
        public static bool URLExists(string url)
        {
            if (_urlCache.TryGetValue(url, out bool value))
            {
                return value;
            }
            else
            {
                value = false;
                var webRequest = WebRequest.Create(url);
                webRequest.Timeout = 2000;
                webRequest.Method = "HEAD";
                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse)webRequest.GetResponse();
                    value = true;
                }
                catch { }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }
                }
                _urlCache.Add(url, value);
                return value;
            }
        }
        public static string GetPokemonSprite(PBEPokemon pokemon)
        {
            return GetPokemonSprite(pokemon.KnownSpecies, pokemon.KnownShiny, pokemon.KnownGender, pokemon.Status2.HasFlag(PBEStatus2.Substitute), false);
        }
        public static string GetPokemonSprite(PBESpecies species, bool shiny, PBEGender gender, bool behindSubstitute, bool backSprite)
        {
            const string path = "https://raw.githubusercontent.com/Kermalis/PokemonBattleEngine/master/Shared%20Assets/PKMN/";
            string orientation = backSprite ? "_B" : "_F";
            if (behindSubstitute)
            {
                return path + "STATUS2_Substitute" + orientation + ".gif";
            }
            else
            {
                ushort speciesID = (ushort)species;
                uint formID = (uint)species >> 0x10;
                string sss = speciesID + (formID > 0 ? ("_" + formID) : string.Empty) + orientation + (shiny ? "_S" : string.Empty);
                string genderStr = gender == PBEGender.Female && URLExists(path + "PKMN_" + sss + "_F.gif") ? "_F" : string.Empty;
                return path + "PKMN_" + sss + genderStr + ".gif";
            }
        }
    }
}
