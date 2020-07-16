using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
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

        static Utils()
        {
            void Add(string resource, List<PBESpecies> list)
            {
                using (var reader = new StreamReader(GetResourceStream(resource)))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
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

        private static readonly object _resourceExistsCacheLockObj = new object();
        private static readonly Dictionary<string, bool> _resourceExistsCache = new Dictionary<string, bool>();
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
            return _assembly.GetManifestResourceStream(AssemblyPrefix + resource);
        }
        public static Uri GetResourceUri(string resource)
        {
            return new Uri("resm:" + AssemblyPrefix + resource);
        }

        public static string WorkingDirectory { get; private set; }
        public static void SetWorkingDirectory(string workingDirectory)
        {
            PBEUtils.InitEngine(workingDirectory);
            WorkingDirectory = workingDirectory;
        }

        private static readonly object _femaleSpriteLookupLockObj = new object();
        private static readonly List<PBESpecies> _femaleMinispriteLookup = new List<PBESpecies>();
        private static readonly List<PBESpecies> _femaleSpriteLookup = new List<PBESpecies>();
        private static bool HasFemaleSprite(PBESpecies species, bool minisprite)
        {
            lock (_femaleSpriteLookupLockObj)
            {
                return (minisprite ? _femaleMinispriteLookup : _femaleSpriteLookup).Contains(species);
            }
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
                    sb.AppendLine($"Disguised as: {pkmn.DisguisedAsPokemon.Nickname}");
                }
                if (pkmn.Battle.BattleFormat != PBEBattleFormat.Single)
                {
                    if (status2.HasFlag(PBEStatus2.Infatuated))
                    {
                        sb.AppendLine($"Infatuated with: {GetTeamNickname(pkmn.InfatuatedWithPokemon)}");
                    }
                    if (status2.HasFlag(PBEStatus2.LeechSeed))
                    {
                        sb.AppendLine($"Seeded position: {pkmn.SeededTeam.CombinedName}'s {pkmn.SeededPosition}");
                    }
                    if (status2.HasFlag(PBEStatus2.LockOn))
                    {
                        sb.AppendLine($"Taking aim at: {GetTeamNickname(pkmn.LockOnPokemon)}");
                    }
                }
            }

            if (useKnownInfo)
            {
                var pData = PBEPokemonData.GetData(pkmn.KnownSpecies, pkmn.KnownForm);
                string formStr = PBEDataUtils.HasForms(pkmn.KnownSpecies, false) ? $" ({PBELocalizedString.GetFormName(pkmn.KnownSpecies, pkmn.KnownForm)})" : string.Empty;
                sb.AppendLine($"{pkmn.KnownNickname}/{pkmn.KnownSpecies}{formStr} {(pkmn.KnownStatus2.HasFlag(PBEStatus2.Transformed) ? pkmn.Gender.ToSymbol() : pkmn.KnownGender.ToSymbol())} Lv.{pkmn.Level}");
                sb.AppendLine($"HP: {pkmn.HPPercentage:P2}");
                sb.Append($"Known types: {PBELocalizedString.GetTypeName(pkmn.KnownType1)}");
                if (pkmn.KnownType2 != PBEType.None)
                {
                    sb.Append($"/{PBELocalizedString.GetTypeName(pkmn.KnownType2)}");
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
                    sb.AppendLine($"Possible abilities: {string.Join(", ", pData.Abilities.Select(a => PBELocalizedString.GetAbilityName(a).ToString()))}");
                }
                else
                {
                    sb.AppendLine($"Known ability: {PBELocalizedString.GetAbilityName(pkmn.KnownAbility)}");
                }
                sb.AppendLine($"Known item: {(pkmn.KnownItem == (PBEItem)ushort.MaxValue ? "???" : PBELocalizedString.GetItemName(pkmn.KnownItem).ToString())}");
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
                    sb.Append(move == PBEMove.MAX ? "???" : PBELocalizedString.GetMoveName(move).ToString());
                    if (move != PBEMove.None && move != PBEMove.MAX)
                    {
                        sb.Append($" ({pp}{(maxPP == 0 ? ")" : $"/{maxPP})")}");
                    }
                }
            }
            else
            {
                string formStr = PBEDataUtils.HasForms(pkmn.Species, false) ? $" ({PBELocalizedString.GetFormName(pkmn.Species, pkmn.Form)})" : string.Empty;
                sb.AppendLine($"{pkmn.Nickname}/{pkmn.Species}{formStr} {pkmn.Gender.ToSymbol()} Lv.{pkmn.Level}");
                sb.AppendLine($"HP: {pkmn.HP}/{pkmn.MaxHP} ({pkmn.HPPercentage:P2})");
                sb.Append($"Types: {PBELocalizedString.GetTypeName(pkmn.Type1)}");
                if (pkmn.Type2 != PBEType.None)
                {
                    sb.Append($"/{PBELocalizedString.GetTypeName(pkmn.Type2)}");
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
                sb.AppendLine($"Ability: {PBELocalizedString.GetAbilityName(pkmn.Ability)}");
                sb.AppendLine($"Item: {PBELocalizedString.GetItemName(pkmn.Item)}");
                if (pkmn.Moves.Contains(PBEMoveEffect.Frustration) || pkmn.Moves.Contains(PBEMoveEffect.Return))
                {
                    sb.AppendLine($"Friendship: {pkmn.Friendship} ({pkmn.Friendship / (double)byte.MaxValue:P2})");
                }
                if (pkmn.Moves.Contains(PBEMoveEffect.HiddenPower))
                {
                    sb.AppendLine($"{PBELocalizedString.GetMoveName(PBEMove.HiddenPower)}: {PBELocalizedString.GetTypeName(pkmn.IndividualValues.GetHiddenPowerType())}|{pkmn.IndividualValues.GetHiddenPowerBasePower(pkmn.Battle.Settings)}");
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
    }
}
