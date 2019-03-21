using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
using System;
using System.Collections.Generic;
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

            void AddStatChanges()
            {
                PBEStat[] statChanges = pkmn.GetChangedStats();
                if (statChanges.Length > 0)
                {
                    var statStrs = new List<string>(7);
                    if (Array.IndexOf(statChanges, PBEStat.Attack) != -1)
                    {
                        statStrs.Add($"[A] x{PBEBattle.GetStatChangeModifier(pkmn.AttackChange, false):0.00}");
                    }
                    if (Array.IndexOf(statChanges, PBEStat.Defense) != -1)
                    {
                        statStrs.Add($"[D] x{PBEBattle.GetStatChangeModifier(pkmn.DefenseChange, false):0.00}");
                    }
                    if (Array.IndexOf(statChanges, PBEStat.SpAttack) != -1)
                    {
                        statStrs.Add($"[SA] x{PBEBattle.GetStatChangeModifier(pkmn.SpAttackChange, false):0.00}");
                    }
                    if (Array.IndexOf(statChanges, PBEStat.SpDefense) != -1)
                    {
                        statStrs.Add($"[SD] x{PBEBattle.GetStatChangeModifier(pkmn.SpDefenseChange, false):0.00}");
                    }
                    if (Array.IndexOf(statChanges, PBEStat.Speed) != -1)
                    {
                        statStrs.Add($"[S] x{PBEBattle.GetStatChangeModifier(pkmn.SpeedChange, false):0.00}");
                    }
                    if (Array.IndexOf(statChanges, PBEStat.Accuracy) != -1)
                    {
                        statStrs.Add($"[AC] x{PBEBattle.GetStatChangeModifier(pkmn.AccuracyChange, true):0.00}");
                    }
                    if (Array.IndexOf(statChanges, PBEStat.Evasion) != -1)
                    {
                        statStrs.Add($"[E] x{PBEBattle.GetStatChangeModifier(pkmn.EvasionChange, true):0.00}");
                    }
                    sb.AppendLine($"Stat changes: {string.Join(", ", statStrs)}");
                }
            }

            if (pkmn.Id == byte.MaxValue) // Unknown remote Pokémon
            {
                sb.AppendLine($"{pkmn.KnownNickname}/{pkmn.KnownSpecies} {pkmn.KnownGenderSymbol} Lv.{pkmn.Level}");
                sb.AppendLine($"HP: {pkmn.HPPercentage:P2}");
                sb.AppendLine($"Known types: {pkmn.KnownType1}/{pkmn.KnownType2}");
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    sb.AppendLine($"Position: {pkmn.FieldPosition}");
                }
                if (pkmn.Status1 != PBEStatus1.None)
                {
                    sb.AppendLine($"Main status: {pkmn.Status1}");
                }
                if (pkmn.FieldPosition != PBEFieldPosition.None && pkmn.Status2 != PBEStatus2.None)
                {
                    sb.AppendLine($"Volatile status: {pkmn.Status2}");
                }
                PBEPokemonData.GetStatRange(PBEStat.HP, pkmn.KnownSpecies, pkmn.Level, pkmn.Team.Battle.Settings, out ushort lowHP, out ushort highHP);
                PBEPokemonData.GetStatRange(PBEStat.Attack, pkmn.KnownSpecies, pkmn.Level, pkmn.Team.Battle.Settings, out ushort lowAttack, out ushort highAttack);
                PBEPokemonData.GetStatRange(PBEStat.Defense, pkmn.KnownSpecies, pkmn.Level, pkmn.Team.Battle.Settings, out ushort lowDefense, out ushort highDefense);
                PBEPokemonData.GetStatRange(PBEStat.SpAttack, pkmn.KnownSpecies, pkmn.Level, pkmn.Team.Battle.Settings, out ushort lowSpAttack, out ushort highSpAttack);
                PBEPokemonData.GetStatRange(PBEStat.SpDefense, pkmn.KnownSpecies, pkmn.Level, pkmn.Team.Battle.Settings, out ushort lowSpDefense, out ushort highSpDefense);
                PBEPokemonData.GetStatRange(PBEStat.Speed, pkmn.KnownSpecies, pkmn.Level, pkmn.Team.Battle.Settings, out ushort lowSpeed, out ushort highSpeed);
                sb.AppendLine($"Stat range: [HP] {lowHP}-{highHP}, [A] {lowAttack}-{highAttack}, [D] {lowDefense}-{highDefense}, [SA] {lowSpAttack}-{highSpAttack}, [SD] {lowSpDefense}-{highSpDefense}, [S] {lowSpeed}-{highSpeed}, [W] {pkmn.KnownWeight:0.0}");
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    AddStatChanges();
                }
                if (pkmn.KnownAbility == PBEAbility.MAX)
                {
                    sb.AppendLine($"Possible abilities: {string.Join(", ", PBEPokemonData.Data[pkmn.KnownSpecies].Abilities.Select(a => PBEAbilityLocalization.Names[a].FromUICultureInfo()))}");
                }
                else
                {
                    sb.AppendLine($"Known ability: {PBEAbilityLocalization.Names[pkmn.KnownAbility].FromUICultureInfo()}");
                }
                sb.AppendLine($"Known item: {(pkmn.KnownItem == (PBEItem)ushort.MaxValue ? "???" : PBEItemLocalization.Names[pkmn.KnownItem].FromUICultureInfo())}");
                sb.Append($"Known moves: {string.Join(", ", pkmn.KnownMoves.Select(m => m == PBEMove.MAX ? "???" : PBEMoveLocalization.Names[m].FromUICultureInfo()))}");
            }
            else
            {
                sb.AppendLine($"{pkmn.Nickname}/{pkmn.OriginalSpecies} {pkmn.GenderSymbol} Lv.{pkmn.Level}");
                sb.AppendLine($"HP: {pkmn.HP}/{pkmn.MaxHP} ({pkmn.HPPercentage:P2})");
                sb.AppendLine($"Types: {pkmn.Type1}/{pkmn.Type2}");
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    sb.AppendLine($"Position: {pkmn.FieldPosition}");
                }
                if (pkmn.Status1 != PBEStatus1.None)
                {
                    sb.AppendLine($"Main status: {pkmn.Status1}");
                }
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    if (pkmn.Status2 != PBEStatus2.None)
                    {
                        sb.AppendLine($"Volatile status: {pkmn.Status2}");
                        if (pkmn.Status2.HasFlag(PBEStatus2.Disguised))
                        {
                            sb.AppendLine($"Disguised as: {pkmn.DisguisedAsPokemon.Nickname}");
                        }
                        if (pkmn.Status2.HasFlag(PBEStatus2.LeechSeed))
                        {
                            sb.AppendLine($"Seeded position: {pkmn.SeededPosition}");
                        }
                    }
                }
                sb.AppendLine($"Stats: [A] {pkmn.Attack}, [D] {pkmn.Defense}, [SA] {pkmn.SpAttack}, [SD] {pkmn.SpDefense}, [S] {pkmn.Speed}, [W] {pkmn.Weight:0.0}");
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    AddStatChanges();
                }
                sb.AppendLine($"Ability: {PBEAbilityLocalization.Names[pkmn.Ability].FromUICultureInfo()}");
                sb.AppendLine($"Item: {PBEItemLocalization.Names[pkmn.Item].FromUICultureInfo()}");
                if (Array.IndexOf(pkmn.Moves, PBEMove.HiddenPower) != -1)
                {
                    sb.AppendLine($"Hidden Power: {pkmn.GetHiddenPowerType()}/{pkmn.GetHiddenPowerBasePower()}");
                }
                string[] moveStrs = new string[pkmn.Moves.Length];
                for (int i = 0; i < moveStrs.Length; i++)
                {
                    moveStrs[i] = $"{PBEMoveLocalization.Names[pkmn.Moves[i]].FromUICultureInfo()} {pkmn.PP[i]}/{pkmn.MaxPP[i]}";
                }
                sb.AppendLine($"Moves: {string.Join(", ", moveStrs)}");
                sb.Append($"Usable moves: {string.Join(", ", pkmn.GetUsableMoves().Select(m => PBEMoveLocalization.Names[m].FromUICultureInfo()))}");
            }
            return sb.ToString();
        }
    }
}
