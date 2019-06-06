using SkiaSharp;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngineMobile.Infrastructure
{
    internal static class StringRenderer
    {
        private interface IStringRenderFont
        {
            string FontId { get; }
            int FontHeight { get; }
            ConcurrentDictionary<string, SKBitmap> LoadedKeys { get; }
            List<(string OldKey, string NewKey)> OverrideKeys { get; }
            // TODO: Cached text?
        }
        private class BattleHPFont : IStringRenderFont
        {
            public string FontId => "BattleHP";
            public int FontHeight => 8;
            public static BattleHPFont Instance { get; } = new BattleHPFont();
            public ConcurrentDictionary<string, SKBitmap> LoadedKeys { get; } = new ConcurrentDictionary<string, SKBitmap>();
            public List<(string OldKey, string NewKey)> OverrideKeys { get; } = new List<(string OldKey, string NewKey)>();
        }
        private class BattleLevelFont : IStringRenderFont
        {
            public string FontId => "BattleLevel";
            public int FontHeight => 10;
            public static BattleLevelFont Instance { get; } = new BattleLevelFont();
            public ConcurrentDictionary<string, SKBitmap> LoadedKeys { get; } = new ConcurrentDictionary<string, SKBitmap>();
            public List<(string OldKey, string NewKey)> OverrideKeys { get; } = new List<(string OldKey, string NewKey)>
            {
                ("[LV]", "LV")
            };
        }
        private class BattleNameFont : IStringRenderFont
        {
            public string FontId => "BattleName";
            public int FontHeight => 13;
            public static BattleNameFont Instance { get; } = new BattleNameFont();
            public ConcurrentDictionary<string, SKBitmap> LoadedKeys { get; } = new ConcurrentDictionary<string, SKBitmap>();
            public List<(string OldKey, string NewKey)> OverrideKeys { get; } = new List<(string OldKey, string NewKey)>
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
            public static DefaultFont Instance { get; } = new DefaultFont();
            public ConcurrentDictionary<string, SKBitmap> LoadedKeys { get; } = new ConcurrentDictionary<string, SKBitmap>();
            public List<(string OldKey, string NewKey)> OverrideKeys { get; } = new List<(string OldKey, string NewKey)>
            {
                ("♂", "246D"),
                ("♀", "246E"),
                ("[PK]", "2486"),
                ("[MN]", "2487")
            };
        }
        public static SKBitmap RenderString(string str, string style)
        {
            // Return null for bad strings
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            else
            {
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
                    case "BattleWhite": secondary = 0xF0FFFFFF; break; // TODO: Fine tune
                    case "MenuBlack": primary = 0xFF5A5252; secondary = 0xFFA5A5AD; break;
                    default: secondary = 0xFF848484; break;
                }

                // Measure how large the string will end up
                int stringWidth = 0,
                stringHeight = font.FontHeight,
                curLineWidth = 0;
                int index = 0;
                var keys = new List<(string Key, SKBitmap Bitmap)>();
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
                        if (curLineWidth > stringWidth)
                        {
                            stringWidth = curLineWidth;
                        }
                        curLineWidth = 0;
                        keys.Add(("\n", null));
                    }
                    else
                    {
                        string key = null;
                        for (int i = 0; i < font.OverrideKeys.Count; i++)
                        {
                            (string oldKey, string newKey) = font.OverrideKeys[i];
                            if (index + oldKey.Length <= str.Length && str.Substring(index, oldKey.Length) == oldKey)
                            {
                                key = newKey;
                                index += oldKey.Length;
                                break;
                            }
                        }
                        if (key == null)
                        {
                            key = ((ushort)str[index]).ToString("X4");
                            index++;
                        }
                        string resource = "FONT." + font.FontId + ".F_" + key + ".png";
                        if (!Utils.DoesResourceExist(resource))
                        {
                            key = "003F"; // 003F is '?'
                        }
                        if (!font.LoadedKeys.TryGetValue(key, out SKBitmap bmp))
                        {
                            bmp = SKBitmap.Decode(Utils.GetResourceStream(resource));
                            font.LoadedKeys.TryAdd(key, bmp);
                        }
                        curLineWidth += bmp.Width;
                        keys.Add((key, bmp));
                    }
                }
                if (curLineWidth > stringWidth)
                {
                    stringWidth = curLineWidth;
                }

                // Draw the string
                var wb = new SKBitmap(stringWidth, stringHeight);
                using (var canvas = new SKCanvas(wb))
                {
                    canvas.Clear(SKColors.Transparent);
                    int x = 0,
                    y = 0;
                    for (int i = 0; i < keys.Count; i++)
                    {
                        (string key, SKBitmap bmp) = keys[i];
                        if (key == "\n")
                        {
                            y += font.FontHeight + 1;
                            x = 0;
                        }
                        else
                        {
                            canvas.DrawBitmap(bmp, x, y);
                            x += bmp.Width;
                        }
                    }
                }
                // Edit colors
                for (int x = 0; x < stringWidth; x++)
                {
                    for (int y = 0; y < stringHeight; y++)
                    {
                        SKColor pixel = wb.GetPixel(x, y);
                        if (pixel == 0xFFFFFFFF)
                        {
                            wb.SetPixel(x, y, primary);
                        }
                        else if (pixel == 0xFF000000)
                        {
                            wb.SetPixel(x, y, secondary);
                        }
                        else if (pixel == 0xFF808080)
                        {
                            wb.SetPixel(x, y, tertiary);
                        }
                    }
                }
                return wb;
            }
        }
    }
}
