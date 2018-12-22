using Avalonia;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
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

        #region String Rendering
        public enum StringRenderStyle
        {
            MenuWhite,
            MenuBlack,
            BattleWhite,
        }
        private class WbFb : IFramebufferPlatformSurface
        {
            WriteableBitmap _bitmap;
            public WbFb(WriteableBitmap bmp) => _bitmap = bmp;
            public ILockedFramebuffer Lock() => _bitmap.Lock();
        }
        static Dictionary<string, Bitmap> LoadedBitmaps { get; } = new Dictionary<string, Bitmap>();
        static string GetCharKey(char c)
        {
            string key = ((int)c).ToString("X");
            string questionMark = ((int)'?').ToString("X");
            return DoesResourceExist($"Kermalis.PokemonBattleEngineClient.Assets.Fonts.{key}.png") ? key : questionMark;
        }
        public static Bitmap RenderString(string str, StringRenderStyle style)
        {
            // Return null for bad strings
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            // Load char bitmaps
            foreach (char c in str)
            {
                if (c == ' ' || c == '\r' || c == '\n')
                {
                    continue;
                }
                else
                {
                    string key = GetCharKey(c);
                    if (!LoadedBitmaps.ContainsKey(key))
                    {
                        LoadedBitmaps.Add(key, UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.Assets.Fonts.{key}.png?assembly=PokemonBattleEngineClient")));
                    }
                }
            }

            // Measure how large the string will end up
            const int charHeight = 15;
            int lineWidth = 0, stringHeight = charHeight;
            var eachLineWidth = new List<int>();
            foreach (char c in str)
            {
                if (c == ' ')
                {
                    lineWidth += 4;
                }
                else if (c == '\r')
                {
                    continue;
                }
                else if (c == '\n')
                {
                    stringHeight += charHeight + 1;
                    eachLineWidth.Add(lineWidth);
                    lineWidth = 0;
                }
                else
                {
                    lineWidth += LoadedBitmaps[GetCharKey(c)].PixelSize.Width;
                }
            }
            eachLineWidth.Add(lineWidth);
            int stringWidth = eachLineWidth.Max();

            // Draw the string
            var wb = new WriteableBitmap(new PixelSize(stringWidth, stringHeight), new Vector(96, 96), PixelFormat.Bgra8888);
            using (IRenderTarget rtb = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>().CreateRenderTarget(new[] { new WbFb(wb) }))
            {
                using (IDrawingContextImpl ctx = rtb.CreateDrawingContext(null))
                {
                    double x = 0, y = 0;
                    foreach (char c in str)
                    {
                        if (c == ' ')
                        {
                            x += 4;
                        }
                        else if (c == '\r')
                        {
                            continue;
                        }
                        else if (c == '\n')
                        {
                            y += charHeight + 1;
                            x = 0;
                        }
                        else
                        {
                            Bitmap bmp = LoadedBitmaps[GetCharKey(c)];
                            int charWidth = bmp.PixelSize.Width;
                            ctx.DrawImage(bmp.PlatformImpl, 1, new Rect(0, 0, charWidth, charHeight), new Rect(x, y, charWidth, charHeight));
                            x += charWidth;
                        }
                    }
                }
            }
            // Edit colors
            using (ILockedFramebuffer l = wb.Lock())
            {
                uint primary = 0xFFFFFFFF, secondary = 0xFF000000;
                switch (style)
                {
                    case StringRenderStyle.MenuBlack: primary = 0xFF5A5252; secondary = 0xFFA5A5AD; break;
                    case StringRenderStyle.BattleWhite: //secondary = 0xF0FFFFFF; break; // Looks horrible because of Avalonia's current issues
                    case StringRenderStyle.MenuWhite: secondary = 0xFF848484; break;
                }
                for (int x = 0; x < stringWidth; x++)
                {
                    for (int y = 0; y < stringHeight; y++)
                    {
                        var address = new IntPtr(l.Address.ToInt64() + (x * sizeof(uint)) + (y * l.RowBytes));
                        uint pixel = (uint)Marshal.ReadInt32(address);
                        if (pixel == 0xFFFFFFFF)
                        {
                            Marshal.WriteInt32(address, (int)primary);
                        }
                        else if (pixel == 0xFF000000)
                        {
                            Marshal.WriteInt32(address, (int)secondary);
                        }
                    }
                }
            }
            return wb;
        }
        #endregion
    }
}
