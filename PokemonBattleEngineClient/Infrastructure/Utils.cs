using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    public static class Utils
    {
        private const string AssemblyPrefix = "Kermalis.PokemonBattleEngineClient.";
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private static readonly string[] _resources = _assembly.GetManifestResourceNames();
        private static IPlatformRenderInterface _renderInterface = null;
        public static IPlatformRenderInterface RenderInterface
        {
            get
            {
                // This is done because the static constructor of Utils is called (by SetWorkingDirectory) before the Avalonia app is built
                if (_renderInterface == null)
                {
                    _renderInterface = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>();
                }
                return _renderInterface;
            }
        }
        private static readonly Dictionary<string, bool> _resourceExistsCache = new Dictionary<string, bool>();
        public static bool DoesResourceExist(string resource)
        {
            if (!_resourceExistsCache.TryGetValue(resource, out bool value))
            {
                value = Array.IndexOf(_resources, AssemblyPrefix + resource) != -1;
                _resourceExistsCache.Add(resource, value);
            }
            return value;
        }
        public static Stream GetResourceStream(string resource)
        {
            return _assembly.GetManifestResourceStream(AssemblyPrefix + resource);
        }

        public static string WorkingDirectory;
        public static void SetWorkingDirectory(string workingDirectory)
        {
            PBEUtils.CreateDatabaseConnection(workingDirectory);
            WorkingDirectory = workingDirectory;
        }

        public static Bitmap GetMinisprite(PBESpecies species, PBEGender gender, bool shiny)
        {
            ushort speciesID = (ushort)species;
            uint formID = (uint)species >> 0x10;
            string sss = speciesID + (formID > 0 ? ("_" + formID) : string.Empty) + (shiny ? "_S" : string.Empty);
            string genderStr = gender == PBEGender.Female && DoesResourceExist("PKMN.PKMN_" + sss + "_F.png") ? "_F" : string.Empty;
            return new Bitmap(GetResourceStream("PKMN.PKMN_" + sss + genderStr + ".png"));
        }
        public static Stream GetPokemonSpriteStream(PBEPokemon pokemon, bool backSprite)
        {
            return GetPokemonSpriteStream(pokemon.KnownSpecies, pokemon.KnownShiny, pokemon.KnownGender, pokemon.Status2.HasFlag(PBEStatus2.Substitute), backSprite);
        }
        public static Stream GetPokemonSpriteStream(PBEPokemonShell shell)
        {
            return GetPokemonSpriteStream(shell.Species, shell.Shiny, shell.Gender, false, false);
        }
        public static Stream GetPokemonSpriteStream(PBESpecies species, bool shiny, PBEGender gender, bool behindSubstitute, bool backSprite)
        {
            string orientation = backSprite ? "_B" : "_F";
            if (behindSubstitute)
            {
                return GetResourceStream("PKMN.STATUS2_Substitute" + orientation + ".gif");
            }
            else
            {
                ushort speciesID = (ushort)species;
                uint formID = (uint)species >> 0x10;
                string sss = speciesID + (formID > 0 ? ("_" + formID) : string.Empty) + orientation + (shiny ? "_S" : string.Empty);
                string genderStr = gender == PBEGender.Female && DoesResourceExist("PKMN.PKMN_" + sss + "_F.gif") ? "_F" : string.Empty;
                return GetResourceStream("PKMN.PKMN_" + sss + genderStr + ".gif");
            }
        }

        public static string CustomPokemonToString(PBEPokemon pkmn, bool showRawValues0, bool showRawValues1)
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

            if ((pkmn.Team.Id == 0 && !showRawValues0) || (pkmn.Team.Id == 1 && !showRawValues1))
            {
                sb.AppendLine($"{pkmn.KnownNickname}/{pkmn.KnownSpecies} {(pkmn.Status2.HasFlag(PBEStatus2.Transformed) ? pkmn.GenderSymbol : pkmn.KnownGenderSymbol)} Lv.{pkmn.Level}");
                sb.AppendLine($"HP: {pkmn.HPPercentage:P2}");
                sb.Append($"Known types: {PBELocalizedString.GetTypeName(pkmn.KnownType1).ToString()}");
                if (pkmn.KnownType2 != PBEType.None)
                {
                    sb.Append($"/{PBELocalizedString.GetTypeName(pkmn.KnownType2).ToString()}");
                }
                sb.AppendLine();
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    sb.AppendLine($"Position: {pkmn.Team.TrainerName}'s {pkmn.FieldPosition}");
                }
                if (pkmn.Status1 != PBEStatus1.None)
                {
                    sb.AppendLine($"Main status: {pkmn.Status1}");
                }
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    PBEStatus2 cleanStatus2 = pkmn.Status2;
                    cleanStatus2 &= ~PBEStatus2.Disguised;
                    if (cleanStatus2 != PBEStatus2.None)
                    {
                        sb.AppendLine($"Volatile status: {cleanStatus2}");
                        if (cleanStatus2.HasFlag(PBEStatus2.Infatuated))
                        {
                            sb.AppendLine($"Infatuated with: {((pkmn.InfatuatedWithPokemon.Team.Id == 0 && showRawValues0) || (pkmn.InfatuatedWithPokemon.Team.Id == 1 && showRawValues1) ? pkmn.InfatuatedWithPokemon.Nickname : pkmn.InfatuatedWithPokemon.KnownNickname)}");
                        }
                        if (cleanStatus2.HasFlag(PBEStatus2.LeechSeed))
                        {
                            sb.AppendLine($"Seeded position: {pkmn.SeededTeam.TrainerName}'s {pkmn.SeededPosition}");
                        }
                    }
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
                    sb.AppendLine($"Possible abilities: {string.Join(", ", PBEPokemonData.GetData(pkmn.KnownSpecies).Abilities.Select(a => PBELocalizedString.GetAbilityName(a).ToString()))}");
                }
                else
                {
                    sb.AppendLine($"Known ability: {PBELocalizedString.GetAbilityName(pkmn.KnownAbility).ToString()}");
                }
                sb.AppendLine($"Known item: {(pkmn.KnownItem == (PBEItem)ushort.MaxValue ? "???" : PBELocalizedString.GetItemName(pkmn.KnownItem).ToString())}");
                sb.Append("Known moves: ");
                for (int i = 0; i < pkmn.Team.Battle.Settings.NumMoves; i++)
                {
                    PBEBattleMoveset.PBEBattleMovesetSlot slot = pkmn.KnownMoves[i];
                    PBEMove move = slot.Move;
                    int pp = slot.PP;
                    int maxPP = slot.MaxPP;
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(move == PBEMove.MAX ? "???" : PBELocalizedString.GetMoveName(move).ToString());
                    if (move != PBEMove.None && move != PBEMove.MAX)
                    {
                        sb.Append($" ({pp}{(maxPP == 0 ? ")" : $"/{maxPP})")}");
                    }
                }
            }
            else
            {
                sb.AppendLine($"{pkmn.Nickname}/{pkmn.Species} {pkmn.GenderSymbol} Lv.{pkmn.Level}");
                sb.AppendLine($"HP: {pkmn.HP}/{pkmn.MaxHP} ({pkmn.HPPercentage:P2})");
                sb.Append($"Types: {PBELocalizedString.GetTypeName(pkmn.Type1).ToString()}");
                if (pkmn.Type2 != PBEType.None)
                {
                    sb.Append($"/{PBELocalizedString.GetTypeName(pkmn.Type2).ToString()}");
                }
                sb.AppendLine();
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    sb.AppendLine($"Position: {pkmn.Team.TrainerName}'s {pkmn.FieldPosition}");
                }
                if (pkmn.Status1 != PBEStatus1.None)
                {
                    sb.AppendLine($"Main status: {pkmn.Status1}");
                }
                if (pkmn.FieldPosition != PBEFieldPosition.None && pkmn.Status2 != PBEStatus2.None)
                {
                    sb.AppendLine($"Volatile status: {pkmn.Status2}");
                    if (pkmn.Status2.HasFlag(PBEStatus2.Disguised))
                    {
                        sb.AppendLine($"Disguised as: {pkmn.DisguisedAsPokemon.Nickname}");
                    }
                    if (pkmn.Status2.HasFlag(PBEStatus2.Infatuated))
                    {
                        sb.AppendLine($"Infatuated with: {((pkmn.InfatuatedWithPokemon.Team.Id == 0 && showRawValues0) || (pkmn.InfatuatedWithPokemon.Team.Id == 1 && showRawValues1) ? pkmn.InfatuatedWithPokemon.Nickname : pkmn.InfatuatedWithPokemon.KnownNickname)}");
                    }
                    if (pkmn.Status2.HasFlag(PBEStatus2.LeechSeed))
                    {
                        sb.AppendLine($"Seeded position: {pkmn.SeededTeam.TrainerName}'s {pkmn.SeededPosition}");
                    }
                }
                sb.AppendLine($"Stats: [A] {pkmn.Attack}, [D] {pkmn.Defense}, [SA] {pkmn.SpAttack}, [SD] {pkmn.SpDefense}, [S] {pkmn.Speed}, [W] {pkmn.Weight:0.0}");
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    AddStatChanges();
                }
                sb.AppendLine($"Ability: {PBELocalizedString.GetAbilityName(pkmn.Ability).ToString()}");
                sb.AppendLine($"Item: {PBELocalizedString.GetItemName(pkmn.Item).ToString()}");
                if (pkmn.Moves.Contains(PBEMove.Frustration) || pkmn.Moves.Contains(PBEMove.Return))
                {
                    sb.AppendLine($"Friendship: {pkmn.Friendship} ({pkmn.Friendship / (double)byte.MaxValue:P2})");
                }
                if (pkmn.Moves.Contains(PBEMove.HiddenPower))
                {
                    sb.AppendLine($"{PBELocalizedString.GetMoveName(PBEMove.HiddenPower).ToString()}: {PBELocalizedString.GetTypeName(pkmn.IndividualValues.HiddenPowerType).ToString()}:{pkmn.IndividualValues.HiddenPowerBasePower}");
                }
                sb.Append("Moves: ");
                for (int i = 0; i < pkmn.Team.Battle.Settings.NumMoves; i++)
                {
                    PBEBattleMoveset.PBEBattleMovesetSlot slot = pkmn.Moves[i];
                    PBEMove move = slot.Move;
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(PBELocalizedString.GetMoveName(slot.Move).ToString());
                    if (move != PBEMove.None)
                    {
                        sb.Append($" ({slot.PP}/{slot.MaxPP})");
                    }
                }
                sb.AppendLine();
                sb.Append($"Usable moves: {string.Join(", ", pkmn.GetUsableMoves().Select(m => PBELocalizedString.GetMoveName(m).ToString()))}");
            }
            return sb.ToString();
        }

        private static readonly Random _rand = new Random();
        public static T RandomElement<T>(this T[] source)
        {
            int count = source.Length;
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(source), $"\"{nameof(source)}\" must have at least one element.");
            }
            return source[_rand.Next(count)];
        }
    }
}
