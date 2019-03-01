using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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
            return GetPokemonSpriteUri(pokemon.KnownSpecies, pokemon.KnownShiny, pokemon.KnownGender, pokemon.Status2.HasFlag(PBEStatus2.Substitute), backSprite);
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

        public static string CustomPokemonToString(PBEPokemon pkmn)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{pkmn.Shell.Nickname}/{pkmn.Shell.Species} {pkmn.GenderSymbol} Lv.{pkmn.Shell.Level}");
            if (pkmn.Id == byte.MaxValue)
            {
                sb.AppendLine($"HP: {pkmn.HPPercentage:P2}");
            }
            else
            {
                sb.AppendLine($"HP: {pkmn.HP}/{pkmn.MaxHP} ({pkmn.HPPercentage:P2})");
            }
            sb.AppendLine($"Position: {pkmn.FieldPosition}");
            sb.AppendLine($"Types: {pkmn.Type1}/{pkmn.Type2}");
            sb.AppendLine($"Status1: {pkmn.Status1}");
            sb.AppendLine($"Status2: {pkmn.Status2}");
            if (pkmn.Id != byte.MaxValue && pkmn.Status2.HasFlag(PBEStatus2.Disguised))
            {
                sb.AppendLine($"Disguised as: {pkmn.DisguisedAsPokemon.Shell.Nickname}");
            }
            if (pkmn.Id != byte.MaxValue && pkmn.Status2.HasFlag(PBEStatus2.LeechSeed))
            {
                sb.AppendLine($"Seeded position: {pkmn.SeededPosition}");
            }
            if (pkmn.Id != byte.MaxValue)
            {
                sb.AppendLine($"Stats: A: {pkmn.Attack} D: {pkmn.Defense} SA: {pkmn.SpAttack} SD: {pkmn.SpDefense} S: {pkmn.Speed}");
            }
            else
            {
                PBEPokemonData.GetStatRange(PBEStat.HP, pkmn.Shell.Species, pkmn.Shell.Level, pkmn.Team.Battle.Settings, out ushort lowHP, out ushort highHP);
                PBEPokemonData.GetStatRange(PBEStat.Attack, pkmn.Shell.Species, pkmn.Shell.Level, pkmn.Team.Battle.Settings, out ushort lowAttack, out ushort highAttack);
                PBEPokemonData.GetStatRange(PBEStat.Defense, pkmn.Shell.Species, pkmn.Shell.Level, pkmn.Team.Battle.Settings, out ushort lowDefense, out ushort highDefense);
                PBEPokemonData.GetStatRange(PBEStat.SpAttack, pkmn.Shell.Species, pkmn.Shell.Level, pkmn.Team.Battle.Settings, out ushort lowSpAttack, out ushort highSpAttack);
                PBEPokemonData.GetStatRange(PBEStat.SpDefense, pkmn.Shell.Species, pkmn.Shell.Level, pkmn.Team.Battle.Settings, out ushort lowSpDefense, out ushort highSpDefense);
                PBEPokemonData.GetStatRange(PBEStat.Speed, pkmn.Shell.Species, pkmn.Shell.Level, pkmn.Team.Battle.Settings, out ushort lowSpeed, out ushort highSpeed);
                sb.AppendLine($"Stat range: HP: {lowHP}-{highHP} A: {lowAttack}-{highAttack} D: {lowDefense}-{highDefense} SA: {lowSpAttack}-{highSpAttack} SD: {lowSpDefense}-{highSpDefense} S: {lowSpeed}-{highSpeed}");
            }
            sb.AppendLine($"Stat changes: A: {pkmn.AttackChange} D: {pkmn.DefenseChange} SA: {pkmn.SpAttackChange} SD: {pkmn.SpDefenseChange} S: {pkmn.SpeedChange} AC: {pkmn.AccuracyChange} E: {pkmn.EvasionChange}");
            sb.AppendLine($"Item: {(pkmn.Item == (PBEItem)ushort.MaxValue ? "???" : PBEItemLocalization.Names[pkmn.Item].English)}");
            if (pkmn.Ability == PBEAbility.MAX)
            {
                PBEPokemonData pData = PBEPokemonData.Data[pkmn.KnownSpecies];
                sb.AppendLine($"Possible abilities: {string.Join(", ", pData.Abilities.Select(a => PBEAbilityLocalization.Names[a].English))}");
            }
            else
            {
                sb.AppendLine($"Ability: {PBEAbilityLocalization.Names[pkmn.Ability].English}");
            }
            if (pkmn.Id != byte.MaxValue)
            {
                sb.AppendLine($"Nature: {pkmn.Shell.Nature}");
            }
            if (pkmn.Id != byte.MaxValue)
            {
                sb.AppendLine($"Hidden Power: {pkmn.GetHiddenPowerType()}/{pkmn.GetHiddenPowerBasePower()}");
            }
            string[] moveStrs = new string[pkmn.Moves.Length];
            for (int i = 0; i < moveStrs.Length; i++)
            {
                string mStr = pkmn.Moves[i] == PBEMove.MAX ? "???" : PBEMoveLocalization.Names[pkmn.Moves[i]].English;
                if (pkmn.Id != byte.MaxValue)
                {
                    mStr += $" {pkmn.PP[i]}/{pkmn.MaxPP[i]}";
                }
                moveStrs[i] = mStr;
            }
            sb.Append($"Moves: {string.Join(", ", moveStrs)}");
            return sb.ToString();
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
            using (IRenderTarget rtb = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>().CreateRenderTarget(new[] { new WriteableBitmapSurface(wb) }))
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
                        ctx.DrawImage(bmp.PlatformImpl, 1.0, new Rect(0, 0, bmp.PixelSize.Width, charHeight), new Rect(x, y, bmp.PixelSize.Width, charHeight));
                        x += bmp.PixelSize.Width;
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
                using (IDrawingContextImpl ctx = rtb.CreateDrawingContext(null))
                {
                    ctx.DrawImage(bmp.PlatformImpl, 1, new Rect(0, 0, bmp.PixelSize.Width, bmp.PixelSize.Height), new Rect(0, 1, bmp.PixelSize.Width, bmp.PixelSize.Height));
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
                using (IDrawingContextImpl ctx = rtb.CreateDrawingContext(null))
                {
                    var rect = new Rect(0, 0, bmp.PixelSize.Width, bmp.PixelSize.Height);
                    ctx.DrawImage(bmp.PlatformImpl, 1.0, rect, rect);
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
