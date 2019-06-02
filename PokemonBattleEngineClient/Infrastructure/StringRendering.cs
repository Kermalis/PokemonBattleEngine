using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    internal static class StringRendering
    {
        private interface IStringRenderFont
        {
            string FontId { get; }
            int FontHeight { get; }
            ConcurrentDictionary<string, Bitmap> LoadedKeys { get; }
            Dictionary<string, string> OverrideKeys { get; }
            // TODO: Cached text?
        }
        private class BattleHPFont : IStringRenderFont
        {
            public string FontId => "BattleHP";
            public int FontHeight => 8;
            public static BattleHPFont Instance { get; } = new BattleHPFont();
            public ConcurrentDictionary<string, Bitmap> LoadedKeys { get; } = new ConcurrentDictionary<string, Bitmap>();
            public Dictionary<string, string> OverrideKeys { get; } = new Dictionary<string, string>();
        }
        private class BattleLevelFont : IStringRenderFont
        {
            public string FontId => "BattleLevel";
            public int FontHeight => 10;
            public static BattleLevelFont Instance { get; } = new BattleLevelFont();
            public ConcurrentDictionary<string, Bitmap> LoadedKeys { get; } = new ConcurrentDictionary<string, Bitmap>();
            public Dictionary<string, string> OverrideKeys { get; } = new Dictionary<string, string>
            {
                { "[LV]", "LV" }
            };
        }
        private class BattleNameFont : IStringRenderFont
        {
            public string FontId => "BattleName";
            public int FontHeight => 13;
            public static BattleNameFont Instance { get; } = new BattleNameFont();
            public ConcurrentDictionary<string, Bitmap> LoadedKeys { get; } = new ConcurrentDictionary<string, Bitmap>();
            public Dictionary<string, string> OverrideKeys { get; } = new Dictionary<string, string>
            {
                { "♂", "246D" },
                { "♀", "246E" },
                { "[PK]", "2486" },
                { "[MN]", "2487" }
            };
        }
        private class DefaultFont : IStringRenderFont
        {
            public string FontId => "Default";
            public int FontHeight => 15;
            public static DefaultFont Instance { get; } = new DefaultFont();
            public ConcurrentDictionary<string, Bitmap> LoadedKeys { get; } = new ConcurrentDictionary<string, Bitmap>();
            public Dictionary<string, string> OverrideKeys { get; } = new Dictionary<string, string>
            {
                { "♂", "246D" },
                { "♀", "246E" },
                { "[PK]", "2486" },
                { "[MN]", "2487" }
            };
        }

        public static Bitmap RenderString(string str, string style)
        {
            // Return null for bad strings
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            IStringRenderFont font;
            switch (style)
            {
                case "BattleHP": font = BattleHPFont.Instance; break;
                case "BattleLevel": font = BattleLevelFont.Instance; break;
                case "BattleName": font = BattleNameFont.Instance; break;
                default: font = DefaultFont.Instance; break;
            }
            uint primary = 0xFFFFFFFF, secondary = 0xFF000000, tertiary = 0xFF808080;
            switch (style)
            {
                case "BattleHP": primary = 0xFFF7F7F7; secondary = 0xFF101010; tertiary = 0xFF9C9CA5; break;
                case "BattleLevel":
                case "BattleName": primary = 0xFFF7F7F7; secondary = 0xFF181818; break;
                //case "BattleWhite": secondary = 0xF0FFFFFF; break; // Looks horrible because of Avalonia's current issues
                case "MenuBlack": primary = 0xFF5A5252; secondary = 0xFFA5A5AD; break;
                default: secondary = 0xFF848484; break;
            }

            int index;
            string GetCharKey()
            {
                string key = null;
                foreach (KeyValuePair<string, string> pair in font.OverrideKeys)
                {
                    if (index + pair.Key.Length <= str.Length && str.Substring(index, pair.Key.Length) == pair.Key)
                    {
                        key = pair.Value;
                        index += pair.Key.Length;
                        break;
                    }
                }
                if (key == null)
                {
                    key = ((int)str[index]).ToString("X4");
                    index++;
                }
                return Utils.DoesResourceExist($"Kermalis.PokemonBattleEngineClient.FONT.{font.FontId}.F_{key}.png") ? key : "003F"; // 003F is '?'
            }

            // Measure how large the string will end up
            int stringWidth = 0, stringHeight = font.FontHeight, curLineWidth = 0;
            index = 0;
            while (index < str.Length)
            {
                if (str[index] == '\r') // Ignore
                {
                    index++;
                    continue;
                }
                else if (str[index] == '\n')
                {
                    index++;
                    stringHeight += font.FontHeight + 1;
                    if (curLineWidth > stringWidth)
                    {
                        stringWidth = curLineWidth;
                    }
                    curLineWidth = 0;
                }
                else
                {
                    string key = GetCharKey();
                    if (!font.LoadedKeys.ContainsKey(key))
                    {
                        font.LoadedKeys.TryAdd(key, Utils.UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.FONT.{font.FontId}.F_{key}.png?assembly=PokemonBattleEngineClient")));
                    }
                    curLineWidth += font.LoadedKeys[key].PixelSize.Width;
                }
            }
            if (curLineWidth > stringWidth)
            {
                stringWidth = curLineWidth;
            }

            // Draw the string
            var wb = new WriteableBitmap(new PixelSize(stringWidth, stringHeight), new Vector(96, 96), PixelFormat.Bgra8888);
            using (IRenderTarget rtb = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>().CreateRenderTarget(new[] { new WriteableBitmapSurface(wb) }))
            using (IDrawingContextImpl ctx = rtb.CreateDrawingContext(null))
            {
                double x = 0, y = 0;
                index = 0;
                while (index < str.Length)
                {
                    if (str[index] == '\r') // Ignore
                    {
                        index++;
                        continue;
                    }
                    else if (str[index] == '\n')
                    {
                        index++;
                        y += font.FontHeight + 1;
                        x = 0;
                    }
                    else
                    {
                        Bitmap bmp = font.LoadedKeys[GetCharKey()];
                        ctx.DrawImage(bmp.PlatformImpl, 1.0, new Rect(0, 0, bmp.PixelSize.Width, font.FontHeight), new Rect(x, y, bmp.PixelSize.Width, font.FontHeight));
                        x += bmp.PixelSize.Width;
                    }
                }
            }
            // Edit colors
            using (ILockedFramebuffer l = wb.Lock())
            {
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
    }
}
