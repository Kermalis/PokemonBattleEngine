using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Kermalis.PokemonBattleEngineClient
{
    static class Utils
    {
        public static readonly Random RNG = new Random();

        public static T Sample<T>(this IEnumerable<T> source) => source.ElementAt(RNG.Next(0, source.Count()));
        // Fisher-Yates Shuffle
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

        public static bool DoesResourceExist(string resource)
        {
            string[] resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            return resources.Contains(resource);
        }
        public static Bitmap UriToBitmap(Uri uri)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            return new Bitmap(assets.Open(uri));
        }

        static readonly Dictionary<char, string> SpecialCharKeys = new Dictionary<char, string>()
        {
            { '*', "Asterisk" },
            { '^', "Caret" },
            { ':', "Colon" },
            { '♀', "Female" },
            { '#', "Hashtag" },
            { '♂', "Male" },
            { '%', "Percent" },
            { '.', "Period" },
            { '?', "QuestionMark" },
            { '"', "QuotationMark" },
            { ' ', "Space" }
        };
        static readonly Dictionary<string, Bitmap> LoadedBitmaps = new Dictionary<string, Bitmap>();
        public static Bitmap RenderString(string str)
        {
            string GetKey(char c)
            {
                if (SpecialCharKeys.ContainsKey(c))
                {
                    return SpecialCharKeys[c];
                }
                string key = $"{(char.IsUpper(c) ? "U" : "")}{c}";
                return DoesResourceExist($"Kermalis.PokemonBattleEngineClient.Assets.Fonts.{key}.png") ? key : SpecialCharKeys['?'];
            }

            foreach (char c in str)
            {
                string key = GetKey(c);
                if (!LoadedBitmaps.ContainsKey(key))
                {
                    LoadedBitmaps.Add(key, UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.Assets.Fonts.{key}.png?assembly=PokemonBattleEngineClient")));
                }
            }

            int stringWidth = str.Select(c => LoadedBitmaps[GetKey(c)].PixelSize.Width).Sum();
            const byte height = 14;

            var rtb = new RenderTargetBitmap(new PixelSize(stringWidth, height));
            using (var ctx = rtb.CreateDrawingContext(null))
            {
                double x = 0;
                foreach (char c in str)
                {
                    Bitmap bmp = LoadedBitmaps[GetKey(c)];
                    int width = bmp.PixelSize.Width;
                    ctx.DrawImage(bmp.PlatformImpl, 1, new Rect(0, 0, width, height), new Rect(x, 0, width, height));
                    x += width;
                }
            }
            return rtb;
        }
    }
}
