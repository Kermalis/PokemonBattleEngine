using Avalonia;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Kermalis.PokemonBattleEngine.Data;
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

        public static Bitmap GetMinisprite(PBESpecies species, PBEGender gender, bool shiny)
        {
            uint speciesID = (uint)species & 0xFFFF;
            uint formeID = (uint)species >> 0x10;
            string sss = $"{speciesID}{(formeID > 0 ? $"_{formeID}" : string.Empty)}{(shiny ? "_S" : string.Empty)}";
            bool spriteIsGenderNeutral = DoesResourceExist($"Kermalis.PokemonBattleEngineClient.PKMN.PKMN_{sss}.png");
            string genderStr = spriteIsGenderNeutral ? string.Empty : gender == PBEGender.Female ? "_F" : "_M";
            return UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.PKMN.PKMN_{sss}{genderStr}.png?assembly=PokemonBattleEngineClient"));
        }
        public static Uri GetPokemonSpriteUri(PBEPokemon pokemon, bool backSprite)
        {
            return GetPokemonSpriteUri(pokemon.VisualSpecies, pokemon.VisualShiny, pokemon.VisualGender, pokemon.Status2.HasFlag(PBEStatus2.Substitute), backSprite);
        }
        public static Uri GetPokemonSpriteUri(PBEPokemonShell shell)
        {
            return GetPokemonSpriteUri(shell.Species, shell.Shiny, shell.Gender, false, false);
        }
        public static Uri GetPokemonSpriteUri(PBESpecies species, bool shiny, PBEGender gender, bool behindSubstitute, bool backSprite)
        {
            string orientation = backSprite ? "_B" : "_F";
            if (behindSubstitute)
            {
                return new Uri($"resm:Kermalis.PokemonBattleEngineClient.PKMN.STATUS2_Substitute{orientation}.gif?assembly=PokemonBattleEngineClient");
            }
            else
            {
                uint speciesID = (uint)species & 0xFFFF;
                uint formeID = (uint)species >> 0x10;
                string sss = $"{speciesID}{(formeID > 0 ? $"_{formeID}" : string.Empty)}{orientation}{(shiny ? "_S" : string.Empty)}";
                bool spriteIsGenderNeutral = DoesResourceExist($"Kermalis.PokemonBattleEngineClient.PKMN.PKMN_{sss}.gif");
                string genderStr = spriteIsGenderNeutral ? string.Empty : gender == PBEGender.Female ? "_F" : "_M";
                return new Uri($"resm:Kermalis.PokemonBattleEngineClient.PKMN.PKMN_{sss}{genderStr}.gif?assembly=PokemonBattleEngineClient");
            }
        }

        #region String Rendering
        public enum StringRenderStyle
        {
            MenuWhite,
            MenuBlack,
            BattleWhite,
            BattleName,
            BattleLevel,
            BattleHP,
            MAX,
        }
        private class WbFb : IFramebufferPlatformSurface
        {
            WriteableBitmap _bitmap;
            public WbFb(WriteableBitmap bmp) => _bitmap = bmp;
            public ILockedFramebuffer Lock() => _bitmap.Lock();
        }
        static readonly ConcurrentDictionary<string, Bitmap> loadedBitmaps = new ConcurrentDictionary<string, Bitmap>();
        public static Bitmap RenderString(string str, StringRenderStyle style)
        {
            // Return null for bad strings
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            string path; int charHeight, spaceWidth;
            switch (style)
            {
                case StringRenderStyle.BattleName: path = "BattleName"; charHeight = 11; spaceWidth = 2; break;
                case StringRenderStyle.BattleLevel: path = "BattleLevel"; charHeight = 10; spaceWidth = 7; break;
                case StringRenderStyle.BattleHP: path = "BattleHP"; charHeight = 8; spaceWidth = 0; break;
                default: path = "Default"; charHeight = 15; spaceWidth = 4; break;
            }

            int index;
            string GetCharKey()
            {
                string key = $"FONT_{path}_";
                if (index + 6 <= str.Length && str.Substring(index, 6) == "[PKMN]")
                {
                    key += "PKMN";
                    index += 6;
                }
                else if (index + 4 <= str.Length && str.Substring(index, 4) == "[LV]")
                {
                    key += "LV";
                    index += 4;
                }
                else
                {
                    key += ((int)str[index]).ToString("X");
                    index++;
                }
                const string questionMark = "FONT_Default_3F";
                return DoesResourceExist($"Kermalis.PokemonBattleEngineClient.FONT.{path}.{key}.png") ? key : questionMark;
            }

            // Measure how large the string will end up
            int stringWidth = 0, stringHeight = charHeight, curLineWidth = 0;
            index = 0;
            while (index < str.Length)
            {
                if (str[index] == ' ')
                {
                    index++;
                    curLineWidth += spaceWidth;
                }
                else if (str[index] == '\r')
                {
                    index++;
                    continue;
                }
                else if (str[index] == '\n')
                {
                    index++;
                    stringHeight += charHeight + 1;
                    if (curLineWidth > stringWidth)
                    {
                        stringWidth = curLineWidth;
                    }
                    curLineWidth = 0;
                }
                else
                {
                    string key = GetCharKey();
                    if (!loadedBitmaps.ContainsKey(key))
                    {
                        loadedBitmaps.TryAdd(key, UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.FONT.{path}.{key}.png?assembly=PokemonBattleEngineClient")));
                    }
                    curLineWidth += loadedBitmaps[key].PixelSize.Width;
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
                    index = 0;
                    while (index < str.Length)
                    {
                        if (str[index] == ' ')
                        {
                            index++;
                            x += spaceWidth;
                        }
                        else if (str[index] == '\r')
                        {
                            index++;
                            continue;
                        }
                        else if (str[index] == '\n')
                        {
                            index++;
                            y += charHeight + 1;
                            x = 0;
                        }
                        else
                        {
                            Bitmap bmp = loadedBitmaps[GetCharKey()];
                            ctx.DrawImage(bmp.PlatformImpl, 1, new Rect(0, 0, bmp.PixelSize.Width, charHeight), new Rect(x, y, bmp.PixelSize.Width, charHeight));
                            x += bmp.PixelSize.Width;
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
                    case StringRenderStyle.BattleName:
                    case StringRenderStyle.BattleLevel: primary = 0xFFF7F7F7; secondary = 0xFF181818; break;
                    case StringRenderStyle.BattleHP: primary = 0xFFF7F7F7; secondary = 0xFF101010; tertiary = 0xFF9C9CA5; break;
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
            //foreach (string file in System.IO.Directory.GetFiles(@"D:\Development\GitHub\PokemonBattleEngine\PokemonBattleEngineClient\Assets\Fonts\Default"))
            foreach (string file in System.IO.Directory.GetFiles(@"D:\Development\GitHub\PokemonBattleEngine\PokemonBattleEngineClient\Assets\Fonts\BattleName"))
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
                            //if (pixel == 0xFFEFEFEF) // Default
                            if (pixel == 0xFFF7F7F7) // BattleName
                            {
                                Marshal.WriteInt32(address, unchecked((int)0xFFFFFFFF));
                            }
                            //else if (pixel == 0xFF848484) // Default
                            else if (pixel == 0xFF181818) // BattleName
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
