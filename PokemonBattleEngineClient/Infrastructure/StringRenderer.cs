using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    internal static class StringRenderer
    {
        private interface IStringRenderFont
        {
            string FontId { get; }
            int FontHeight { get; }
            ConcurrentDictionary<string, Bitmap> LoadedKeys { get; }
            (string OldKey, string NewKey)[] OverrideKeys { get; }
            // TODO: Cached text?
        }
        private class BattleHPFont : IStringRenderFont
        {
            public string FontId => "BattleHP";
            public int FontHeight => 8;
            public static BattleHPFont Instance { get; } = new();
            public ConcurrentDictionary<string, Bitmap> LoadedKeys { get; } = new();
            public (string OldKey, string NewKey)[] OverrideKeys { get; } = Array.Empty<(string OldKey, string NewKey)>();
        }
        private class BattleLevelFont : IStringRenderFont
        {
            public string FontId => "BattleLevel";
            public int FontHeight => 10;
            public static BattleLevelFont Instance { get; } = new();
            public ConcurrentDictionary<string, Bitmap> LoadedKeys { get; } = new();
            public (string OldKey, string NewKey)[] OverrideKeys { get; } = new (string OldKey, string NewKey)[]
            {
                ("[LV]", "LV")
            };
        }
        private class BattleNameFont : IStringRenderFont
        {
            public string FontId => "BattleName";
            public int FontHeight => 13;
            public static BattleNameFont Instance { get; } = new();
            public ConcurrentDictionary<string, Bitmap> LoadedKeys { get; } = new();
            public (string OldKey, string NewKey)[] OverrideKeys { get; } = new (string OldKey, string NewKey)[]
            {
                ("♂", "246D"),
                ("♀", "246E"),
                ("[PK]", "2486"),
                ("[MN]", "2487")
            };
        }
        private class DefaultFont : IStringRenderFont
        {
            public string FontId => "Default";
            public int FontHeight => 15;
            public static DefaultFont Instance { get; } = new();
            public ConcurrentDictionary<string, Bitmap> LoadedKeys { get; } = new();
            public (string OldKey, string NewKey)[] OverrideKeys { get; } = new (string OldKey, string NewKey)[]
            {
                ("♂", "246D"),
                ("♀", "246E"),
                ("[PK]", "2486"),
                ("[MN]", "2487")
            };
        }

        [return: NotNullIfNotNull("str")]
        public static unsafe Bitmap? Render(string? str, string? style)
        {
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
            uint primary = 0xFFFFFFFF,
                secondary = 0xFF000000,
                tertiary = 0xFF808080;
            switch (style)
            {
                case "BattleHP": primary = 0xFFF7F7F7; secondary = 0xFF101010; tertiary = 0xFF9C9CA5; break;
                case "BattleLevel":
                case "BattleName": primary = 0xFFF7F7F7; secondary = 0xFF181818; break;
                //case "BattleWhite": secondary = 0xF0FFFFFF; break; // Looks horrible because of Avalonia's current issues
                case "MenuBlack": primary = 0xFF5A5252; secondary = 0xFFA5A5AD; break;
                default: secondary = 0xFF848484; break;
            }

            // Measure how large the string will end up
            var lineWidths = new List<int>(20);
            int stringWidth = 0,
                stringHeight = font.FontHeight,
                curLineWidth = 0;
            void RecordLineWidth()
            {
                if (curLineWidth > stringWidth)
                {
                    stringWidth = curLineWidth;
                }
                lineWidths.Add(curLineWidth);
            }
            int index = 0;
            var keys = new List<(string Key, Bitmap? Bitmap)>();
            while (index < str.Length)
            {
                char c = str[index];
                if (c == '\r') // Ignore
                {
                    index++;
                }
                else if (c == '\n')
                {
                    index++;
                    stringHeight += font.FontHeight + 1;
                    RecordLineWidth();
                    curLineWidth = 0;
                    keys.Add(("\n", null));
                }
                else
                {
                    string? key = null;
                    for (int i = 0; i < font.OverrideKeys.Length; i++)
                    {
                        (string oldKey, string newKey) = font.OverrideKeys[i];
                        if (index + oldKey.Length <= str.Length && str.Substring(index, oldKey.Length) == oldKey)
                        {
                            key = newKey;
                            index += oldKey.Length;
                            break;
                        }
                    }
                    if (key is null)
                    {
                        key = ((ushort)str[index]).ToString("X4");
                        index++;
                    }
                    string resource = "FONT." + font.FontId + ".F_" + key + ".png";
                    if (!Utils.DoesResourceExist(resource))
                    {
                        key = "003F"; // 003F is '?'
                    }
                    if (!font.LoadedKeys.TryGetValue(key, out Bitmap? bmp))
                    {
                        bmp = new Bitmap(Utils.GetResourceStream(resource));
                        font.LoadedKeys.TryAdd(key, bmp);
                    }
                    curLineWidth += bmp.PixelSize.Width;
                    keys.Add((key, bmp));
                }
            }
            RecordLineWidth();

            // Draw the string
            var dpi = new Vector(96, 96);
            var wb = new WriteableBitmap(new PixelSize(stringWidth, stringHeight), dpi, PixelFormat.Bgra8888, AlphaFormat.Premul);
            using (IRenderTarget rtb = Utils.RenderInterface.CreateRenderTarget(new[] { new WriteableBitmapSurface(wb) }))
            using (IDrawingContextImpl ctx = rtb.CreateDrawingContext(null))
            {
                int x = 0,
                    y = 0;
                for (int i = 0; i < keys.Count; i++)
                {
                    (string key, Bitmap? bmp) = keys[i];
                    if (key == "\n")
                    {
                        y += font.FontHeight + 1;
                        x = 0;
                    }
                    else
                    {
                        var size = new Size(bmp!.PixelSize.Width, bmp.PixelSize.Height); // TODO: Verify and/or try other options with different dpi
                        ctx.DrawBitmap(bmp.PlatformImpl, 1d, new Rect(size), new Rect(new Point(x, y), size));
                        x += bmp.PixelSize.Width;
                    }
                }
            }
            // Edit colors
            using (ILockedFramebuffer l = wb.Lock())
            {
                long startAddress = l.Address.ToInt64();
                for (int i = 0; i < lineWidths.Count; i++)
                {
                    int w = lineWidths[i];
                    for (int x = 0; x < w; x++)
                    {
                        for (int y = 0; y < font.FontHeight; y++)
                        {
                            uint* address = (uint*)(startAddress + (x * sizeof(uint)) + ((y + ((font.FontHeight + 1) * i)) * l.RowBytes));
                            uint pixel = *address;
                            if (pixel == 0xFFFFFFFF)
                            {
                                *address = primary;
                            }
                            else if (pixel == 0xFF000000)
                            {
                                *address = secondary;
                            }
                            else if (pixel == 0xFF808080)
                            {
                                *address = tertiary;
                            }
                        }
                    }
                }
            }
            return wb;
        }
    }
}
