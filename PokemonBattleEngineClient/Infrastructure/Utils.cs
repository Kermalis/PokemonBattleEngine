using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    static class Utils
    {
        static string[] resources = null;
        public static bool DoesResourceExist(string resource)
        {
            if (resources == null)
            {
                resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            }
            return Array.IndexOf(resources, resource) != -1;
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
    }
}
