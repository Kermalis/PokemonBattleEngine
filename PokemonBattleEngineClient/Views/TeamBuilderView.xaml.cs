using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    public class TeamBuilderView : UserControl, INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        Uri source;
        public Uri Source
        {
            get => source; set
            {
                source = value;
                OnPropertyChanged(nameof(Source));
            }
        }

        // TODO: Legal species vs non-legal (Cherrim Sunny in a pokeball etc)
        readonly IEnumerable<PBEAbility> allAbilities = Enum.GetValues(typeof(PBEAbility)).Cast<PBEAbility>().Except(new[] { PBEAbility.MAX });
        readonly IEnumerable<PBEGender> allGenders = Enum.GetValues(typeof(PBEGender)).Cast<PBEGender>().Except(new[] { PBEGender.MAX });
        readonly IEnumerable<PBEItem> allItems = Enum.GetValues(typeof(PBEItem)).Cast<PBEItem>();

        public IEnumerable<PBESpecies> Species { get; } = Enum.GetValues(typeof(PBESpecies)).Cast<PBESpecies>();
        IEnumerable<PBEAbility> availableAbilities;
        public IEnumerable<PBEAbility> AvailableAbilities
        {
            get => availableAbilities;
            set
            {
                availableAbilities = value;
                OnPropertyChanged(nameof(AvailableAbilities));
            }
        }
        IEnumerable<PBENature> Natures { get; } = Enum.GetValues(typeof(PBENature)).Cast<PBENature>().Except(new[] { PBENature.MAX });
        IEnumerable<PBEGender> availableGenders;
        public IEnumerable<PBEGender> AvailableGenders
        {
            get => availableGenders;
            set
            {
                availableGenders = value;
                OnPropertyChanged(nameof(AvailableGenders));
            }
        }
        IEnumerable<PBEItem> availableItems;
        public IEnumerable<PBEItem> AvailableItems
        {
            get => availableItems;
            set
            {
                availableItems = value;
                OnPropertyChanged(nameof(AvailableItems));
            }
        }
        public IEnumerable<PBEMove> AvailableMoves { get; } = Enum.GetValues(typeof(PBEMove)).Cast<PBEMove>().Except(new[] { PBEMove.MAX });

        PBEPokemonShell shell;
        public Tuple<string, ObservableCollection<PBEPokemonShell>> team;
        public Tuple<string, ObservableCollection<PBEPokemonShell>> Team
        {
            get => team;
            set
            {
                team = value;
                OnPropertyChanged(nameof(Team));
            }
        }
        public ObservableCollection<Tuple<string, ObservableCollection<PBEPokemonShell>>> Teams { get; } = new ObservableCollection<Tuple<string, ObservableCollection<PBEPokemonShell>>>();

        readonly Subject<bool> addPartyEnabled, removePartyEnabled;

        public readonly PBESettings settings = PBESettings.DefaultSettings;

        readonly TextBox nickname;
        readonly NumericUpDown level, friendship;
        readonly NumericUpDown[] evs, ivs, ppups;
        readonly ListBox party, savedTeams;
        readonly CheckBox illegal, shiny;
        readonly ComboBox species, ability, nature, gender, item;
        readonly ComboBox[] moves;

        public TeamBuilderView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            addPartyEnabled = new Subject<bool>();
            this.FindControl<Button>("AddParty").Command = ReactiveCommand.Create(AddPartyMember, addPartyEnabled);
            removePartyEnabled = new Subject<bool>();
            this.FindControl<Button>("RemoveParty").Command = ReactiveCommand.Create(RemovePartyMember, removePartyEnabled);
            this.FindControl<Button>("AddTeam").Command = ReactiveCommand.Create(AddTeam);
            this.FindControl<Button>("RemoveTeam").Command = ReactiveCommand.Create(RemoveTeam);
            party = this.FindControl<ListBox>("Party");
            party.SelectionChanged += (s, e) =>
            {
                if (party.SelectedIndex > -1)
                {
                    shell = team.Item2[party.SelectedIndex];
                    UpdateEditor(false, true, true, true);
                    UpdateEditor(true, false, false, true);
                }
            };
            savedTeams = this.FindControl<ListBox>("SavedTeams");
            savedTeams.SelectionChanged += (s, e) =>
            {
                if (savedTeams.SelectedIndex > -1)
                {
                    Team = Teams[savedTeams.SelectedIndex];
                    party.SelectedIndex = 0;
                    EvaluatePartySize();
                }
            };
            illegal = this.FindControl<CheckBox>("Illegal");
            illegal.Command = ReactiveCommand.Create(IllegalChanged);

            void shellOnly(object s, EventArgs e) => UpdateEditor(true, false, false);

            species = this.FindControl<ComboBox>("Species");
            species.SelectionChanged += (s, e) => UpdateEditor(true, true, true);
            nickname = this.FindControl<TextBox>("Nickname");
            nickname.LostFocus += (s, e) => UpdateEditor(true, false, true);
            level = this.FindControl<NumericUpDown>("Level");
            level.ValueChanged += shellOnly;
            friendship = this.FindControl<NumericUpDown>("Friendship");
            friendship.ValueChanged += shellOnly;
            shiny = this.FindControl<CheckBox>("Shiny");
            shiny.Command = ReactiveCommand.Create(() => UpdateEditor(true, false, true));
            ability = this.FindControl<ComboBox>("Ability");
            ability.SelectionChanged += shellOnly;
            nature = this.FindControl<ComboBox>("Nature");
            nature.SelectionChanged += shellOnly;
            gender = this.FindControl<ComboBox>("Gender");
            gender.SelectionChanged += (s, e) => UpdateEditor(true, false, true);
            item = this.FindControl<ComboBox>("Item");
            item.SelectionChanged += shellOnly;
            evs = new[]
            {
                this.FindControl<NumericUpDown>("HPEV"),
                this.FindControl<NumericUpDown>("ATKEV"),
                this.FindControl<NumericUpDown>("DEFEV"),
                this.FindControl<NumericUpDown>("SPATKEV"),
                this.FindControl<NumericUpDown>("SPDEFEV"),
                this.FindControl<NumericUpDown>("SPEEV")
            };
            ivs = new[]
            {
                this.FindControl<NumericUpDown>("HPIV"),
                this.FindControl<NumericUpDown>("ATKIV"),
                this.FindControl<NumericUpDown>("DEFIV"),
                this.FindControl<NumericUpDown>("SPATKIV"),
                this.FindControl<NumericUpDown>("SPDEFIV"),
                this.FindControl<NumericUpDown>("SPEIV")
            };
            for (int i = 0; i < 6; i++)
            {
                evs[i].ValueChanged += shellOnly;
                ivs[i].ValueChanged += shellOnly;
            }
            moves = new[]
            {
                this.FindControl<ComboBox>("Move0"),
                this.FindControl<ComboBox>("Move1"),
                this.FindControl<ComboBox>("Move2"),
                this.FindControl<ComboBox>("Move3")
            };
            ppups = new[]
            {
                this.FindControl<NumericUpDown>("PPUps0"),
                this.FindControl<NumericUpDown>("PPUps1"),
                this.FindControl<NumericUpDown>("PPUps2"),
                this.FindControl<NumericUpDown>("PPUps3")
            };
            for (int i = 0; i < moves.Length; i++)
            {
                moves[i].SelectionChanged += shellOnly;
                ppups[i].ValueChanged += shellOnly;
            }
        }
        public void Load()
        {
            if (Directory.Exists("Teams"))
            {
                string[] files = Directory.GetFiles("Teams");
                if (files.Length > 0)
                {
                    foreach (string f in files)
                    {
                        Teams.Add(Tuple.Create(Path.GetFileNameWithoutExtension(f), new ObservableCollection<PBEPokemonShell>(PBEPokemonShell.TeamFromTextFile(f))));
                    }
                    savedTeams.SelectedIndex = 0;
                    return;
                }
            }
            else
            {
                Directory.CreateDirectory("Teams");
            }

            AddTeam();
        }

        void AddTeam()
        {
            Teams.Add(Tuple.Create($"Team {DateTime.Now.Ticks}", new ObservableCollection<PBEPokemonShell>()));
            savedTeams.SelectedIndex = Teams.Count - 1;
            AddPartyMember();
        }
        void RemoveTeam()
        {
            Teams.Remove(team);
            //File.Delete($"Teams\\{team.Item1}.txt");
            if (Teams.Count == 0)
            {
                AddTeam();
            }
            else
            {
                savedTeams.SelectedIndex = Teams.Count - 1;
            }
        }
        void AddPartyMember()
        {
            PBESpecies species = Species.Sample();
            team.Item2.Add(new PBEPokemonShell
            {
                Species = species,
                Nickname = PBELocalizedString.GetSpeciesName(species).FromUICultureInfo(),
                Level = settings.MinLevel,
                EVs = new byte[6],
                IVs = new byte[6],
                Moves = new PBEMove[moves.Length],
                PPUps = new byte[moves.Length]
            });
            party.SelectedIndex = team.Item2.Count - 1;
            EvaluatePartySize();
        }
        void RemovePartyMember()
        {
            team.Item2.Remove(shell);
            party.SelectedIndex = team.Item2.Count - 1;
            EvaluatePartySize();
        }
        void EvaluatePartySize()
        {
            if (illegal.IsChecked.Value)
            {
                addPartyEnabled.OnNext(true);
            }
            else
            {
                addPartyEnabled.OnNext(team.Item2.Count < settings.MaxPartySize);
                // Remove if too many
                if (team.Item2.Count > settings.MaxPartySize)
                {
                    party.SelectedIndex = settings.MaxPartySize - 1;
                    int removeAmt = team.Item2.Count - settings.MaxPartySize;
                    for (int i = 0; i < removeAmt; i++)
                    {
                        team.Item2.RemoveAt(team.Item2.Count - 1);
                    }
                }
            }
            removePartyEnabled.OnNext(team.Item2.Count > 1);
        }
        void IllegalChanged()
        {
            EvaluatePartySize();
            UpdateEditor(false, true, false);
        }

        bool ignoreUpdate = false;
        void UpdateEditor(bool updateShell, bool updateControls, bool updateSprites, bool toggleIgnore = false)
        {
            if (toggleIgnore)
            {
                ignoreUpdate = !ignoreUpdate;
            }
            else if (ignoreUpdate)
            {
                return;
            }

            string prevSpeciesName = PBELocalizedString.GetSpeciesName(shell.Species).FromUICultureInfo();

            if (updateShell)
            {
                shell.Species = (PBESpecies)species.SelectedItem;
                if (!illegal.IsChecked.Value && nickname.Text?.Length > settings.MaxPokemonNameLength)
                {
                    nickname.Text = new string(nickname.Text.Take(settings.MaxPokemonNameLength).ToArray());
                }
                else if ((!illegal.IsChecked.Value && string.IsNullOrWhiteSpace(nickname.Text)) || prevSpeciesName.Equals(nickname.Text))
                {
                    nickname.Text = PBELocalizedString.GetSpeciesName(shell.Species).FromUICultureInfo();
                }
                shell.Nickname = nickname.Text;
                shell.Level = (byte)level.Value;
                shell.Friendship = (byte)friendship.Value;
                shell.Shiny = shiny.IsChecked.Value;
                if (ability.SelectedItem != null)
                {
                    shell.Ability = (PBEAbility)ability.SelectedItem;
                }
                shell.Nature = (PBENature)nature.SelectedItem;
                if (gender.SelectedItem != null)
                {
                    shell.Gender = (PBEGender)gender.SelectedItem;
                }
                if (item.SelectedItem != null)
                {
                    shell.Item = (PBEItem)item.SelectedItem;
                }
                for (int i = 0; i < 6; i++)
                {
                    shell.EVs[i] = (byte)evs[i].Value;
                    shell.IVs[i] = (byte)ivs[i].Value;
                }
                for (int i = 0; i < moves.Length; i++)
                {
                    if (moves[i].SelectedItem != null)
                    {
                        shell.Moves[i] = (PBEMove)moves[i].SelectedItem;
                    }
                    shell.PPUps[i] = (byte)ppups[i].Value;
                }

                //PBEPokemonShell.TeamToTextFile($"Teams\\{team.Item1}.txt", team.Item2);
            }
            if (updateSprites)
            {
                Source = Utils.GetPokemonSpriteUri(shell);
                // Force redraw
                party.Items = Array.Empty<object>();
                party.Items = team.Item2;
                party.InvalidateVisual();
            }
            if (updateControls)
            {
                var pData = PBEPokemonData.GetData(shell.Species);

                if (species.SelectedItem != (object)shell.Species)
                {
                    species.SelectedItem = shell.Species;
                }

                if (nickname.Text != shell.Nickname)
                {
                    nickname.Text = shell.Nickname;
                }

                level.Maximum = illegal.IsChecked.Value ? byte.MaxValue : settings.MaxLevel;
                level.Minimum = illegal.IsChecked.Value ? byte.MinValue : settings.MinLevel;
                shell.Level = (byte)PBEUtils.Clamp(shell.Level, level.Minimum, level.Maximum);
                if (level.Value != shell.Level)
                {
                    level.Value = shell.Level;
                }

                if (friendship.Value != shell.Friendship)
                {
                    friendship.Value = shell.Friendship;
                }

                if (shiny.IsChecked != shell.Shiny)
                {
                    shiny.IsChecked = shell.Shiny;
                    UpdateEditor(false, false, true);
                }

                if (illegal.IsChecked.Value)
                {
                    AvailableAbilities = allAbilities;
                }
                else
                {
                    AvailableAbilities = pData.Abilities;
                }
                shell.Ability = availableAbilities.Contains(shell.Ability) ? shell.Ability : availableAbilities.First();
                if (ability.SelectedItem != (object)shell.Ability)
                {
                    ability.SelectedItem = shell.Ability;
                }
                ability.IsEnabled = availableAbilities.Count() > 1;

                if (nature.SelectedItem != (object)shell.Nature)
                {
                    nature.SelectedItem = shell.Nature;
                }

                if (illegal.IsChecked.Value)
                {
                    AvailableGenders = allGenders;
                }
                else
                {
                    switch (pData.GenderRatio)
                    {
                        case PBEGenderRatio.M0_F0: AvailableGenders = new[] { PBEGender.Genderless }; break;
                        case PBEGenderRatio.M1_F0: AvailableGenders = new[] { PBEGender.Male }; break;
                        case PBEGenderRatio.M0_F1: AvailableGenders = new[] { PBEGender.Female }; break;
                        default: AvailableGenders = new[] { PBEGender.Female, PBEGender.Male }; break;
                    }
                }
                shell.Gender = availableGenders.Contains(shell.Gender) ? shell.Gender : availableGenders.First();
                if (gender.SelectedItem != (object)shell.Gender)
                {
                    gender.SelectedItem = shell.Gender;
                }
                gender.IsEnabled = availableGenders.Count() > 1;

                if (illegal.IsChecked.Value)
                {
                    AvailableItems = allItems;
                }
                else
                {
                    switch (shell.Species)
                    {
                        case PBESpecies.Giratina: AvailableItems = allItems.Except(new[] { PBEItem.GriseousOrb }); break;
                        case PBESpecies.Giratina_Origin: AvailableItems = new[] { PBEItem.GriseousOrb }; break;
                        case PBESpecies.Arceus:
                            AvailableItems = allItems.Except(new[] { PBEItem.DracoPlate, PBEItem.DreadPlate, PBEItem.EarthPlate, PBEItem.FistPlate,
                                PBEItem.FlamePlate, PBEItem.IciclePlate, PBEItem.InsectPlate, PBEItem.IronPlate, PBEItem.MeadowPlate, PBEItem.MindPlate, PBEItem.SkyPlate,
                                PBEItem.SplashPlate, PBEItem.SpookyPlate, PBEItem.StonePlate, PBEItem.ToxicPlate, PBEItem.ZapPlate });
                            break;
                        case PBESpecies.Arceus_Bug: AvailableItems = new[] { PBEItem.InsectPlate }; break;
                        case PBESpecies.Arceus_Dark: AvailableItems = new[] { PBEItem.DreadPlate }; break;
                        case PBESpecies.Arceus_Dragon: AvailableItems = new[] { PBEItem.DracoPlate }; break;
                        case PBESpecies.Arceus_Electric: AvailableItems = new[] { PBEItem.ZapPlate }; break;
                        case PBESpecies.Arceus_Fighting: AvailableItems = new[] { PBEItem.FistPlate }; break;
                        case PBESpecies.Arceus_Fire: AvailableItems = new[] { PBEItem.FlamePlate }; break;
                        case PBESpecies.Arceus_Flying: AvailableItems = new[] { PBEItem.SkyPlate }; break;
                        case PBESpecies.Arceus_Ghost: AvailableItems = new[] { PBEItem.SpookyPlate }; break;
                        case PBESpecies.Arceus_Grass: AvailableItems = new[] { PBEItem.MeadowPlate }; break;
                        case PBESpecies.Arceus_Ground: AvailableItems = new[] { PBEItem.EarthPlate }; break;
                        case PBESpecies.Arceus_Ice: AvailableItems = new[] { PBEItem.IciclePlate }; break;
                        case PBESpecies.Arceus_Poison: AvailableItems = new[] { PBEItem.ToxicPlate }; break;
                        case PBESpecies.Arceus_Psychic: AvailableItems = new[] { PBEItem.MindPlate }; break;
                        case PBESpecies.Arceus_Rock: AvailableItems = new[] { PBEItem.StonePlate }; break;
                        case PBESpecies.Arceus_Steel: AvailableItems = new[] { PBEItem.IronPlate }; break;
                        case PBESpecies.Arceus_Water: AvailableItems = new[] { PBEItem.SplashPlate }; break;
                        case PBESpecies.Genesect: AvailableItems = allItems.Except(new[] { PBEItem.BurnDrive, PBEItem.ChillDrive, PBEItem.DouseDrive, PBEItem.ShockDrive }); break;
                        case PBESpecies.Genesect_Burn: AvailableItems = new[] { PBEItem.BurnDrive }; break;
                        case PBESpecies.Genesect_Chill: AvailableItems = new[] { PBEItem.ChillDrive }; break;
                        case PBESpecies.Genesect_Douse: AvailableItems = new[] { PBEItem.DouseDrive }; break;
                        case PBESpecies.Genesect_Shock: AvailableItems = new[] { PBEItem.ShockDrive }; break;
                        default: AvailableItems = allItems; break;
                    }
                }
                shell.Item = availableItems.Contains(shell.Item) ? shell.Item : availableItems.First();
                if (item.SelectedItem != (object)shell.Item)
                {
                    item.SelectedItem = shell.Item;
                }
                item.IsEnabled = availableItems.Count() > 1;

                byte maxIVs = illegal.IsChecked.Value ? byte.MaxValue : settings.MaxIVs;
                for (int i = 0; i < 6; i++)
                {
                    if (evs[i].Value != shell.EVs[i])
                    {
                        evs[i].Value = shell.EVs[i];
                    }
                    ivs[i].Maximum = maxIVs;
                    shell.IVs[i] = shell.IVs[i].Clamp(byte.MinValue, maxIVs);
                    if (ivs[i].Value != shell.IVs[i])
                    {
                        ivs[i].Value = shell.IVs[i];
                    }
                }

                byte maxPPUps = illegal.IsChecked.Value ? byte.MaxValue : settings.MaxPPUps;
                for (int i = 0; i < moves.Length; i++)
                {
                    if (moves[i].SelectedItem != (object)shell.Moves[i])
                    {
                        moves[i].SelectedItem = shell.Moves[i];
                    }
                    ppups[i].Maximum = maxPPUps;
                    shell.PPUps[i] = shell.PPUps[i].Clamp(byte.MinValue, maxPPUps);
                    if (ppups[i].Value != shell.PPUps[i])
                    {
                        ppups[i].Value = shell.PPUps[i];
                    }
                }
            }
        }
    }
}
