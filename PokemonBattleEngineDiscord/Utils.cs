using Discord;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Kermalis.PokemonBattleEngineDiscord
{
    internal static class Utils
    {
        public const string URL = "https://github.com/Kermalis/PokemonBattleEngine";
        private const string ImageURL = "https://raw.githubusercontent.com/Kermalis/PokemonBattleEngine/master/Shared%20Assets/PKMN/";

        #region Static Collections
        public static readonly Dictionary<PBEType, Color> TypeColors = new()
        {
            { PBEType.None, new Color(146, 154, 156) },
            { PBEType.Bug, new Color(162, 212, 56) },
            { PBEType.Dark, new Color(106, 122, 156) },
            { PBEType.Dragon, new Color(80, 136, 188) },
            { PBEType.Electric, new Color(246, 216, 48) },
            { PBEType.Fighting, new Color(244, 100, 138) },
            { PBEType.Fire, new Color(255, 152, 56) },
            { PBEType.Flying, new Color(80, 124, 212) },
            { PBEType.Ghost, new Color(94, 100, 208) },
            { PBEType.Grass, new Color(64, 208, 112) },
            { PBEType.Ground, new Color(232, 130, 68) },
            { PBEType.Ice, new Color(98, 204, 212) },
            { PBEType.Normal, new Color(146, 154, 156) },
            { PBEType.Poison, new Color(188, 82, 232) },
            { PBEType.Psychic, new Color(255, 136, 130) },
            { PBEType.Rock, new Color(196, 174, 112) },
            { PBEType.Steel, new Color(94, 160, 178) },
            { PBEType.Water, new Color(58, 176, 232) }
        };
        public static readonly Dictionary<PBEType, Emote> TypeEmotes = new()
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
        public static readonly Dictionary<PBEStatus1, Emote> Status1Emotes = new()
        {
            { PBEStatus1.Asleep, Emote.Parse("<:Asleep:708841651690537030>") },
            { PBEStatus1.BadlyPoisoned, Emote.Parse("<:BadlyPoisoned:708841652151910400>") },
            { PBEStatus1.Burned, Emote.Parse("<:Burned:708841651711246442>") },
            { PBEStatus1.Frozen, Emote.Parse("<:Frozen:708841651359186995>") },
            { PBEStatus1.Paralyzed, Emote.Parse("<:Paralyzed:708841651568771093>") },
            { PBEStatus1.Poisoned, Emote.Parse("<:Poisoned:708841651787005962>") }
        };
        public static readonly Dictionary<PBEWeather, Emote> WeatherEmotes = new()
        {
            { PBEWeather.Hailstorm, Emote.Parse("<a:Hailstorm:709213589612920834>") },
            { PBEWeather.HarshSunlight, Emote.Parse("<a:HarshSunlight:709213589587755121>") },
            { PBEWeather.Rain, Emote.Parse("<a:Rain:709213589977694239>") },
            { PBEWeather.Sandstorm, Emote.Parse("<a:Sandstorm:709213589826830357>") }
        };
        #endregion

        // https://stackoverflow.com/a/3722337
        public static Color Blend(this Color color, Color backColor, float depth = 0.5f)
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
        public static Color GetColor(PBEBattlePokemon pkmn)
        {
            return GetColor(pkmn.KnownType1, pkmn.KnownType2);
        }

        private static readonly Random _rand = new();
        public static Color RandomColor()
        {
            byte[] bytes = new byte[3];
            _rand.NextBytes(bytes);
            return new Color(bytes[0], bytes[1], bytes[2]);
        }
        public static T RandomElement<T>(this IReadOnlyList<T> source)
        {
            int count = source.Count;
            if (count == 1)
            {
                return source[0];
            }
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(source), $"\"{nameof(source)}\" must have at least one element.");
            }
            return source[_rand.Next(count)];
        }

        private static readonly List<PBESpecies> _femaleSpriteLookup = new();
        public static void InitFemaleSpriteLookup()
        {
            const string address = ImageURL + "FemaleSpriteLookup.txt";
            using (var w = new WebClient())
            using (var reader = new StringReader(w.DownloadString(address)))
            {
                string? line;
                while ((line = reader.ReadLine()) is not null)
                {
                    if (!Enum.TryParse(line, out PBESpecies species))
                    {
                        throw new InvalidDataException($"Failed to parse \"{address}\"");
                    }
                    _femaleSpriteLookup.Add(species);
                }
            }
        }
        private static bool HasFemaleSprite(PBESpecies species)
        {
            return _femaleSpriteLookup.Contains(species);
        }
        public static string GetPokemonSprite(PBEBattlePokemon pokemon)
        {
            return GetPokemonSprite(pokemon.KnownSpecies, pokemon.KnownForm, pokemon.KnownShiny, pokemon.KnownGender, pokemon.KnownStatus2.HasFlag(PBEStatus2.Substitute), false);
        }
        public static string GetPokemonSprite(PBESpecies species, PBEForm form, bool shiny, PBEGender gender, bool behindSubstitute, bool backSprite)
        {
            string orientation = backSprite ? "_B" : "_F";
            if (behindSubstitute)
            {
                return ImageURL + "STATUS2_Substitute" + orientation + ".gif";
            }
            else
            {
                string speciesStr = PBEDataUtils.GetNameOfForm(species, form) ?? species.ToString();
                string genderStr = gender == PBEGender.Female && HasFemaleSprite(species) ? "_F" : string.Empty;
                return ImageURL + "PKMN_" + speciesStr + orientation + (shiny ? "_S" : string.Empty) + genderStr + ".gif";
            }
        }
    }
}
