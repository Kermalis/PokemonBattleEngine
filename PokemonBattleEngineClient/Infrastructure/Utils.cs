using Avalonia;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Concurrent;
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
            BattleName,
            MAX,
        }
        private class WbFb : IFramebufferPlatformSurface
        {
            WriteableBitmap _bitmap;
            public WbFb(WriteableBitmap bmp) => _bitmap = bmp;
            public ILockedFramebuffer Lock() => _bitmap.Lock();
        }
        static ConcurrentDictionary<string, ConcurrentDictionary<string, Bitmap>> LoadedBitmaps { get; } = new ConcurrentDictionary<string, ConcurrentDictionary<string, Bitmap>>();
        static string GetCharKey(string path, char c)
        {
            string key = ((int)c).ToString("X");
            const string questionMark = "3F";
            return DoesResourceExist($"Kermalis.PokemonBattleEngineClient.Assets.Fonts.{path}.{key}.png") ? key : questionMark;
        }
        public static Bitmap RenderString(string str, StringRenderStyle style)
        {
            // Return null for bad strings
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            if (style >= StringRenderStyle.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(style), "Invalid style.");
            }

            string path; int charHeight, spaceWidth;
            switch (style)
            {
                case StringRenderStyle.BattleName: path = "BattleName"; charHeight = 11; spaceWidth = 2; break;
                default: path = "Default"; charHeight = 15; spaceWidth = 4; break;
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
                    string key = GetCharKey(path, c);
                    if (!LoadedBitmaps.ContainsKey(path))
                    {
                        LoadedBitmaps.TryAdd(path, new ConcurrentDictionary<string, Bitmap>());
                    }
                    if (!LoadedBitmaps[path].ContainsKey(key))
                    {
                        LoadedBitmaps[path].TryAdd(key, UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.Assets.Fonts.{path}.{key}.png?assembly=PokemonBattleEngineClient")));
                    }
                }
            }

            // Measure how large the string will end up
            int stringWidth = 0, stringHeight = charHeight, curLineWidth = 0;
            foreach (char c in str)
            {
                if (c == ' ')
                {
                    curLineWidth += spaceWidth;
                }
                else if (c == '\r')
                {
                    continue;
                }
                else if (c == '\n')
                {
                    stringHeight += charHeight + 1;
                    if (curLineWidth > stringWidth)
                    {
                        stringWidth = curLineWidth;
                    }
                    curLineWidth = 0;
                }
                else
                {
                    curLineWidth += LoadedBitmaps[path][GetCharKey(path, c)].PixelSize.Width;
                }
            }
            if (curLineWidth > stringWidth)
            {
                stringWidth = curLineWidth;
            }

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
                            x += spaceWidth;
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
                            Bitmap bmp = LoadedBitmaps[path][GetCharKey(path, c)];
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
                uint primary = 0xFFFFFFFF, secondary = 0xFF000000, tertiary = 0xFF808080;
                switch (style)
                {
                    case StringRenderStyle.MenuBlack: primary = 0xFF5A5252; secondary = 0xFFA5A5AD; break;
                    case StringRenderStyle.BattleWhite: //secondary = 0xF0FFFFFF; break; // Looks horrible because of Avalonia's current issues
                    case StringRenderStyle.MenuWhite: secondary = 0xFF848484; break;
                    case StringRenderStyle.BattleName: primary = 0xFFF7F7F7; secondary = 0xFF181818; break;
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
                        else if (pixel == 0xFF808080)
                        {
                            Marshal.WriteInt32(address, (int)tertiary);
                        }
                    }
                }
            }
            return wb;
        }
        /*public static void SizeFix()
        {
            foreach (string file in System.IO.Directory.GetFiles(@"D:\Development\GitHub\PokemonBattleEngine\PokemonBattleEngineClient\Assets\Fonts\BattleName"))
            {
                var bmp = new Bitmap(file);
                var wb = new WriteableBitmap(new PixelSize(bmp.PixelSize.Width, 11), new Vector(96, 96), PixelFormat.Bgra8888);
                using (IRenderTarget rtb = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>().CreateRenderTarget(new[] { new WbFb(wb) }))
                {
                    using (IDrawingContextImpl ctx = rtb.CreateDrawingContext(null))
                    {
                        ctx.DrawImage(bmp.PlatformImpl, 1, new Rect(0, 0, bmp.PixelSize.Width, bmp.PixelSize.Height), new Rect(0, 1, bmp.PixelSize.Width, bmp.PixelSize.Height));
                    }
                }
                wb.Save(file);
            }
        }*/
        /*public static void ColorFix()
        {
            foreach (string file in System.IO.Directory.GetFiles(@"D:\Development\GitHub\PokemonBattleEngine\PokemonBattleEngineClient\Assets\Fonts\Default"))
            {
                var bmp = new Bitmap(file);
                var wb = new WriteableBitmap(bmp.PixelSize, new Vector(96, 96), PixelFormat.Bgra8888);
                using (IRenderTarget rtb = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>().CreateRenderTarget(new[] { new WbFb(wb) }))
                {
                    using (IDrawingContextImpl ctx = rtb.CreateDrawingContext(null))
                    {
                        var rect = new Rect(0, 0, bmp.PixelSize.Width, bmp.PixelSize.Height);
                        ctx.DrawImage(bmp.PlatformImpl, 1, rect, rect);
                    }
                }
                using (ILockedFramebuffer l = wb.Lock())
                {
                    for (int x = 0; x < bmp.PixelSize.Width; x++)
                    {
                        for (int y = 0; y < bmp.PixelSize.Height; y++)
                        {
                            var address = new IntPtr(l.Address.ToInt64() + (x * sizeof(uint)) + (y * l.RowBytes));
                            uint pixel = (uint)Marshal.ReadInt32(address);
                            if (pixel == 0xFFEFEFEF)
                            {
                                Marshal.WriteInt32(address, unchecked((int)0xFFFFFFFF));
                            }
                            else if (pixel == 0xFF848484)
                            {
                                Marshal.WriteInt32(address, unchecked((int)0xFF000000));
                            }
                            else if (pixel != 0xFFFFFFFF && pixel != 0xFF000000 && pixel != 0x00000000)
                            {
                                ;
                            }
                        }
                    }
                }
                wb.Save(file);
            }
        }*/
        #endregion
    }
}
