using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.DefaultData;
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
        private static IPlatformRenderInterface? _renderInterface = null;
        public static IPlatformRenderInterface RenderInterface
        {
            get
            {
                // This is done because the static constructor of Utils is called (by SetWorkingDirectory) before the Avalonia app is built
                if (_renderInterface is null)
                {
                    _renderInterface = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>();
                }
                return _renderInterface;
            }
        }

        static Utils()
        {
            _femaleMinispriteLookup = new List<PBESpecies>();
            _femaleSpriteLookup = new List<PBESpecies>();
            static void Add(string resource, List<PBESpecies> list)
            {
                using (var reader = new StreamReader(GetResourceStream(resource)))
                {
                    string? line;
                    while ((line = reader.ReadLine()) is not null)
                    {
                        if (!Enum.TryParse(line, out PBESpecies species))
                        {
                            throw new InvalidDataException($"Failed to parse \"{resource}\"");
                        }
                        list.Add(species);
                    }
                }
            }
            Add("PKMN.FemaleMinispriteLookup.txt", _femaleMinispriteLookup);
            Add("PKMN.FemaleSpriteLookup.txt", _femaleSpriteLookup);
        }

        private static readonly object _resourceExistsCacheLockObj = new();
        private static readonly Dictionary<string, bool> _resourceExistsCache = new();
        public static bool DoesResourceExist(string resource)
        {
            lock (_resourceExistsCacheLockObj)
            {
                if (!_resourceExistsCache.TryGetValue(resource, out bool value))
                {
                    value = Array.IndexOf(_resources, AssemblyPrefix + resource) != -1;
                    _resourceExistsCache.Add(resource, value);
                }
                return value;
            }
        }
        public static Stream GetResourceStream(string resource)
        {
            Stream? ret = _assembly.GetManifestResourceStream(AssemblyPrefix + resource);
            if (ret is null)
            {
                throw new ArgumentOutOfRangeException(nameof(resource), "Resource not found: " + resource);
            }
            return ret;
        }
        public static Uri GetResourceUri(string resource)
        {
            return new Uri("resm:" + AssemblyPrefix + resource);
        }

        public static string WorkingDirectory { get; private set; } = null!;
        public static void SetWorkingDirectory(string workingDirectory)
        {
            PBEDefaultDataProvider.InitEngine(workingDirectory);
            WorkingDirectory = workingDirectory;
        }

        private static readonly List<PBESpecies> _femaleMinispriteLookup;
        private static readonly List<PBESpecies> _femaleSpriteLookup;
        private static bool HasFemaleSprite(PBESpecies species, bool minisprite)
        {
            return (minisprite ? _femaleMinispriteLookup : _femaleSpriteLookup).Contains(species);
        }
        public static Bitmap GetMinispriteBitmap(PBESpecies species, PBEForm form, PBEGender gender, bool shiny)
        {
            string speciesStr = PBEDataUtils.GetNameOfForm(species, form) ?? species.ToString();
            string genderStr = gender == PBEGender.Female && HasFemaleSprite(species, true) ? "_F" : string.Empty;
            return new Bitmap(GetResourceStream("PKMN.PKMN_" + speciesStr + (shiny ? "_S" : string.Empty) + genderStr + ".png"));
        }
        public static Uri GetPokemonSpriteUri(PBEBattlePokemon pkmn, bool backSprite)
        {
            return GetPokemonSpriteUri(pkmn.KnownSpecies, pkmn.KnownForm, pkmn.KnownShiny, pkmn.KnownGender, pkmn.KnownStatus2.HasFlag(PBEStatus2.Substitute), backSprite);
        }
        public static Uri GetPokemonSpriteUri(IPBEPokemon pkmn)
        {
            return GetPokemonSpriteUri(pkmn.Species, pkmn.Form, pkmn.Shiny, pkmn.Gender, false, false);
        }
        public static Uri GetPokemonSpriteUri(PBESpecies species, PBEForm form, bool shiny, PBEGender gender, bool behindSubstitute, bool backSprite)
        {
            string orientation = backSprite ? "_B" : "_F";
            if (behindSubstitute)
            {
                return GetResourceUri("PKMN.STATUS2_Substitute" + orientation + ".gif");
            }
            else
            {
                string speciesStr = PBEDataUtils.GetNameOfForm(species, form) ?? species.ToString();
                string genderStr = gender == PBEGender.Female && HasFemaleSprite(species, false) ? "_F" : string.Empty;
                return GetResourceUri("PKMN.PKMN_" + speciesStr + orientation + (shiny ? "_S" : string.Empty) + genderStr + ".gif");
            }
        }

        public static string CustomPokemonToString(PBEBattlePokemon pkmn, bool useKnownInfo)
        {
            var sb = new StringBuilder();

            string GetTeamNickname(PBEBattlePokemon p)
            {
                return $"{p.Trainer.Name}'s {(useKnownInfo ? p.KnownNickname : p.Nickname)}";
            }

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
            void AddStatus1()
            {
                if (pkmn.Status1 != PBEStatus1.None)
                {
                    sb.AppendLine($"Main status: {pkmn.Status1}");
                    if (pkmn.Status1 == PBEStatus1.Asleep)
                    {
                        sb.AppendLine($"Asleep turns: {pkmn.Status1Counter}");
                    }
                    else if (pkmn.Status1 == PBEStatus1.BadlyPoisoned)
                    {
                        sb.AppendLine($"Toxic counter: {pkmn.Status1Counter}");
                    }
                }
            }
            void AddStatus2(PBEStatus2 status2)
            {
                status2 &= ~PBEStatus2.Flinching; // Don't show flinching
                sb.AppendLine($"Volatile status: {status2}");
                if (status2.HasFlag(PBEStatus2.Disguised))
                {
                    string formStr = PBEDataUtils.HasForms(pkmn.KnownSpecies, false) ? $" ({PBEDataProvider.Instance.GetFormName(pkmn.KnownSpecies, pkmn.KnownForm).FromGlobalLanguage()})" : string.Empty;
                    sb.AppendLine($"Disguised as: {pkmn.KnownNickname}/{PBEDataProvider.Instance.GetSpeciesName(pkmn.KnownSpecies).FromGlobalLanguage()}{formStr} {pkmn.KnownGender.ToSymbol()}");
                }
                if (pkmn.Battle.BattleFormat != PBEBattleFormat.Single)
                {
                    if (status2.HasFlag(PBEStatus2.Infatuated))
                    {
                        sb.AppendLine($"Infatuated with: {GetTeamNickname(pkmn.InfatuatedWithPokemon!)}");
                    }
                    if (status2.HasFlag(PBEStatus2.LeechSeed))
                    {
                        sb.AppendLine($"Seeded position: {pkmn.SeededTeam!.CombinedName}'s {pkmn.SeededPosition}");
                    }
                    if (status2.HasFlag(PBEStatus2.LockOn))
                    {
                        sb.AppendLine($"Taking aim at: {GetTeamNickname(pkmn.LockOnPokemon!)}");
                    }
                }
            }

            if (useKnownInfo)
            {
                IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(pkmn.KnownSpecies, pkmn.KnownForm);
                string formStr = PBEDataUtils.HasForms(pkmn.KnownSpecies, false) ? $" ({PBEDataProvider.Instance.GetFormName(pkmn.KnownSpecies, pkmn.KnownForm).FromGlobalLanguage()})" : string.Empty;
                sb.AppendLine($"{pkmn.KnownNickname}/{PBEDataProvider.Instance.GetSpeciesName(pkmn.KnownSpecies).FromGlobalLanguage()}{formStr} {(pkmn.KnownStatus2.HasFlag(PBEStatus2.Transformed) ? pkmn.Gender.ToSymbol() : pkmn.KnownGender.ToSymbol())} Lv.{pkmn.Level}");
                sb.AppendLine($"HP: {pkmn.HPPercentage:P2}");
                sb.Append($"Known types: {PBEDataProvider.Instance.GetTypeName(pkmn.KnownType1).FromGlobalLanguage()}");
                if (pkmn.KnownType2 != PBEType.None)
                {
                    sb.Append($"/{PBEDataProvider.Instance.GetTypeName(pkmn.KnownType2).FromGlobalLanguage()}");
                }
                sb.AppendLine();
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    sb.AppendLine($"Position: {pkmn.Team.CombinedName}'s {pkmn.FieldPosition}");
                }
                AddStatus1();
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    if (pkmn.KnownStatus2 != PBEStatus2.None)
                    {
                        AddStatus2(pkmn.KnownStatus2);
                    }
                }
                PBEDataUtils.GetStatRange(pData, PBEStat.HP, pkmn.Level, pkmn.Battle.Settings, out ushort lowHP, out ushort highHP);
                PBEDataUtils.GetStatRange(pData, PBEStat.Attack, pkmn.Level, pkmn.Battle.Settings, out ushort lowAttack, out ushort highAttack);
                PBEDataUtils.GetStatRange(pData, PBEStat.Defense, pkmn.Level, pkmn.Battle.Settings, out ushort lowDefense, out ushort highDefense);
                PBEDataUtils.GetStatRange(pData, PBEStat.SpAttack, pkmn.Level, pkmn.Battle.Settings, out ushort lowSpAttack, out ushort highSpAttack);
                PBEDataUtils.GetStatRange(pData, PBEStat.SpDefense, pkmn.Level, pkmn.Battle.Settings, out ushort lowSpDefense, out ushort highSpDefense);
                PBEDataUtils.GetStatRange(pData, PBEStat.Speed, pkmn.Level, pkmn.Battle.Settings, out ushort lowSpeed, out ushort highSpeed);
                sb.AppendLine($"Stat range: [HP] {lowHP}-{highHP}, [A] {lowAttack}-{highAttack}, [D] {lowDefense}-{highDefense}, [SA] {lowSpAttack}-{highSpAttack}, [SD] {lowSpDefense}-{highSpDefense}, [S] {lowSpeed}-{highSpeed}, [W] {pkmn.KnownWeight:0.0}");
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    AddStatChanges();
                }
                if (pkmn.KnownAbility == PBEAbility.MAX)
                {
                    sb.AppendLine($"Possible abilities: {string.Join(", ", pData.Abilities.Select(a => PBEDataProvider.Instance.GetAbilityName(a).FromGlobalLanguage()))}");
                }
                else
                {
                    sb.AppendLine($"Known ability: {PBEDataProvider.Instance.GetAbilityName(pkmn.KnownAbility).FromGlobalLanguage()}");
                }
                sb.AppendLine($"Known item: {(pkmn.KnownItem == (PBEItem)ushort.MaxValue ? "???" : PBEDataProvider.Instance.GetItemName(pkmn.KnownItem).FromGlobalLanguage())}");
                sb.Append("Known moves: ");
                for (int i = 0; i < pkmn.Battle.Settings.NumMoves; i++)
                {
                    PBEBattleMoveset.PBEBattleMovesetSlot slot = pkmn.KnownMoves[i];
                    PBEMove move = slot.Move;
                    int pp = slot.PP;
                    int maxPP = slot.MaxPP;
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(move == PBEMove.MAX ? "???" : PBEDataProvider.Instance.GetMoveName(move).FromGlobalLanguage());
                    if (move != PBEMove.None && move != PBEMove.MAX)
                    {
                        sb.Append($" ({pp}{(maxPP == 0 ? ")" : $"/{maxPP})")}");
                    }
                }
            }
            else
            {
                string formStr = PBEDataUtils.HasForms(pkmn.Species, false) ? $" ({PBEDataProvider.Instance.GetFormName(pkmn).FromGlobalLanguage()})" : string.Empty;
                sb.AppendLine($"{pkmn.Nickname}/{PBEDataProvider.Instance.GetSpeciesName(pkmn.Species).FromGlobalLanguage()}{formStr} {pkmn.Gender.ToSymbol()} Lv.{pkmn.Level}");
                sb.AppendLine($"HP: {pkmn.HP}/{pkmn.MaxHP} ({pkmn.HPPercentage:P2})");
                sb.Append($"Types: {PBEDataProvider.Instance.GetTypeName(pkmn.Type1).FromGlobalLanguage()}");
                if (pkmn.Type2 != PBEType.None)
                {
                    sb.Append($"/{PBEDataProvider.Instance.GetTypeName(pkmn.Type2).FromGlobalLanguage()}");
                }
                sb.AppendLine();
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    sb.AppendLine($"Position: {pkmn.Team.CombinedName}'s {pkmn.FieldPosition}");
                }
                AddStatus1();
                if (pkmn.FieldPosition != PBEFieldPosition.None && pkmn.Status2 != PBEStatus2.None)
                {
                    AddStatus2(pkmn.Status2);
                }
                sb.AppendLine($"Stats: [A] {pkmn.Attack}, [D] {pkmn.Defense}, [SA] {pkmn.SpAttack}, [SD] {pkmn.SpDefense}, [S] {pkmn.Speed}, [W] {pkmn.Weight:0.0}");
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    AddStatChanges();
                }
                sb.AppendLine($"Ability: {PBEDataProvider.Instance.GetAbilityName(pkmn.Ability).FromGlobalLanguage()}");
                sb.AppendLine($"Item: {PBEDataProvider.Instance.GetItemName(pkmn.Item).FromGlobalLanguage()}");
                if (pkmn.Moves.Contains(PBEMoveEffect.Frustration) || pkmn.Moves.Contains(PBEMoveEffect.Return))
                {
                    sb.AppendLine($"Friendship: {pkmn.Friendship} ({pkmn.Friendship / (float)byte.MaxValue:P2})");
                }
                if (pkmn.Moves.Contains(PBEMoveEffect.HiddenPower))
                {
                    PBEReadOnlyStatCollection ivs = pkmn.IndividualValues!;
                    sb.AppendLine($"{PBEDataProvider.Instance.GetMoveName(PBEMove.HiddenPower).FromGlobalLanguage()}: {PBEDataProvider.Instance.GetTypeName(ivs.GetHiddenPowerType()).FromGlobalLanguage()}|{ivs.GetHiddenPowerBasePower(pkmn.Battle.Settings)}");
                }
                sb.Append("Moves: ");
                for (int i = 0; i < pkmn.Battle.Settings.NumMoves; i++)
                {
                    PBEBattleMoveset.PBEBattleMovesetSlot slot = pkmn.Moves[i];
                    PBEMove move = slot.Move;
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(PBEDataProvider.Instance.GetMoveName(slot.Move).FromGlobalLanguage());
                    if (move != PBEMove.None)
                    {
                        sb.Append($" ({slot.PP}/{slot.MaxPP})");
                    }
                }
                sb.AppendLine();
                sb.Append($"Usable moves: {string.Join(", ", pkmn.GetUsableMoves().Select(m => PBEDataProvider.Instance.GetMoveName(m).FromGlobalLanguage()))}");
            }
            return sb.ToString();
        }
    }
}
