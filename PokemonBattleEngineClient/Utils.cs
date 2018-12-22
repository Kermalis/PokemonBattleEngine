using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kermalis.PokemonBattleEngineClient
{
    static class Utils
    {
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

        static readonly Dictionary<string, Bitmap> LoadedBitmaps = new Dictionary<string, Bitmap>();
        static string GetCharKey(char c)
        {
            string key = ((int)c).ToString("X");
            string questionMark = ((int)'?').ToString("X");
            return DoesResourceExist($"Kermalis.PokemonBattleEngineClient.Assets.Fonts.{key}.png") ? key : questionMark;
        }
        public static Bitmap RenderString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            foreach (char c in str)
            {
                string key = GetCharKey(c);
                if (!LoadedBitmaps.ContainsKey(key))
                {
                    LoadedBitmaps.Add(key, UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.Assets.Fonts.{key}.png?assembly=PokemonBattleEngineClient")));
                }
            }

            int stringWidth = str.Select(c => LoadedBitmaps[GetCharKey(c)].PixelSize.Width).Sum();
            const byte height = 15;

            var rtb = new RenderTargetBitmap(new PixelSize(stringWidth, height));
            using (var ctx = rtb.CreateDrawingContext(null))
            {
                double x = 0;
                foreach (char c in str)
                {
                    Bitmap bmp = LoadedBitmaps[GetCharKey(c)];
                    int width = bmp.PixelSize.Width;
                    ctx.DrawImage(bmp.PlatformImpl, 1, new Rect(0, 0, width, height), new Rect(x, 0, width, height));
                    x += width;
                }
            }
            return rtb;
        }
    }
}
