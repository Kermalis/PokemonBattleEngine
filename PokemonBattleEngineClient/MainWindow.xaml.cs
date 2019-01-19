using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using Kermalis.PokemonBattleEngineClient.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;

namespace Kermalis.PokemonBattleEngineClient
{
    class MainWindow : Window, INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        Uri source;
        Uri Source
        {
            get => source; set
            {
                source = value;
                OnPropertyChanged(nameof(Source));
            }
        }

        readonly IEnumerable<PBEAbility> allAbilities = PBEAbilityLocalization.Names.OrderBy(k => k.Value.FromUICultureInfo()).Select(k => k.Key);
        readonly IEnumerable<PBEGender> allGenders = Enum.GetValues(typeof(PBEGender)).Cast<PBEGender>().Except(new[] { PBEGender.MAX });
        readonly IEnumerable<PBEItem> allItems = PBEItemLocalization.Names.OrderBy(k => k.Value.FromUICultureInfo()).Select(k => k.Key);

        IEnumerable<PBESpecies> AvailableSpecies { get; } = PBEPokemonLocalization.Names.OrderBy(k => k.Value.FromUICultureInfo()).Select(k => k.Key);
        IEnumerable<PBEAbility> availableAbilities;
        IEnumerable<PBEAbility> AvailableAbilities
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
        IEnumerable<PBEGender> AvailableGenders
        {
            get => availableGenders;
            set
            {
                availableGenders = value;
                OnPropertyChanged(nameof(AvailableGenders));
            }
        }
        IEnumerable<PBEItem> availableItems;
        IEnumerable<PBEItem> AvailableItems
        {
            get => availableItems;
            set
            {
                availableItems = value;
                OnPropertyChanged(nameof(AvailableItems));
            }
        }
        IEnumerable<PBEMove> AvailableMoves { get; } = PBEMoveLocalization.Names.OrderBy(k => k.Value.FromUICultureInfo()).Select(k => k.Key);

        PBEPokemonShell shell;
        Tuple<string, ObservableCollection<PBEPokemonShell>> team;
        Tuple<string, ObservableCollection<PBEPokemonShell>> Team
        {
            get => team;
            set
            {
                team = value;
                OnPropertyChanged(nameof(Team));
            }
        }
        ObservableCollection<Tuple<string, ObservableCollection<PBEPokemonShell>>> Teams { get; } = new ObservableCollection<Tuple<string, ObservableCollection<PBEPokemonShell>>>();

        readonly Subject<bool> addPartyEnabled, removePartyEnabled;

        readonly PBESettings settings = PBESettings.DefaultSettings;
        readonly List<BattleClient> battles = new List<BattleClient>();

        readonly TabControl tabs;
        readonly TextBox ip, nickname;
        readonly NumericUpDown port, level, friendship;
        readonly NumericUpDown[] evs, ivs, ppups;
        readonly ListBox party, savedTeams;
        readonly CheckBox illegal, shiny;
        readonly DropDown species, ability, nature, gender, item;
        readonly DropDown[] moves;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            addPartyEnabled = new Subject<bool>();
            this.FindControl<Button>("AddParty").Command = ReactiveCommand.Create(AddPartyMember, addPartyEnabled);
            removePartyEnabled = new Subject<bool>();
            this.FindControl<Button>("RemoveParty").Command = ReactiveCommand.Create(RemovePartyMember, removePartyEnabled);
            this.FindControl<Button>("AddTeam").Command = ReactiveCommand.Create(AddTeam);
            this.FindControl<Button>("RemoveTeam").Command = ReactiveCommand.Create(RemoveTeam);
            this.FindControl<Button>("Connect").Command = ReactiveCommand.Create(Connect);
            tabs = this.FindControl<TabControl>("Tabs");
            ip = this.FindControl<TextBox>("IP");
            port = this.FindControl<NumericUpDown>("Port");
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
            void WaitingForAvaloniaPR2254(object s, EventArgs e)
            {
                var n = (NumericUpDown)s;
                n.Text = n.Value.ToString();
            };

            species = this.FindControl<DropDown>("Species");
            species.SelectionChanged += (s, e) => UpdateEditor(true, true, true);
            nickname = this.FindControl<TextBox>("Nickname");
            nickname.LostFocus += (s, e) => UpdateEditor(true, false, true);
            level = this.FindControl<NumericUpDown>("Level");
            level.ValueChanged += shellOnly;
            level.LostFocus += WaitingForAvaloniaPR2254;
            friendship = this.FindControl<NumericUpDown>("Friendship");
            friendship.ValueChanged += shellOnly;
            friendship.LostFocus += WaitingForAvaloniaPR2254;
            shiny = this.FindControl<CheckBox>("Shiny");
            shiny.Command = ReactiveCommand.Create(() => UpdateEditor(true, false, true));
            ability = this.FindControl<DropDown>("Ability");
            ability.SelectionChanged += shellOnly;
            nature = this.FindControl<DropDown>("Nature");
            nature.SelectionChanged += shellOnly;
            gender = this.FindControl<DropDown>("Gender");
            gender.SelectionChanged += (s, e) => UpdateEditor(true, false, true);
            item = this.FindControl<DropDown>("Item");
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
                evs[i].LostFocus += WaitingForAvaloniaPR2254;
                ivs[i].ValueChanged += shellOnly;
                ivs[i].LostFocus += WaitingForAvaloniaPR2254;
            }
            moves = new[]
            {
                this.FindControl<DropDown>("Move0"),
                this.FindControl<DropDown>("Move1"),
                this.FindControl<DropDown>("Move2"),
                this.FindControl<DropDown>("Move3")
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
                ppups[i].LostFocus += WaitingForAvaloniaPR2254;
            }
        }
        byte shows = 0;
        public override void Show()
        {
            base.Show();
            if (shows++ == 1)
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
        }

        void AddTeam()
        {

            Teams.Add(Tuple.Create($"Team {PBEUtils.RNG.Next()}", new ObservableCollection<PBEPokemonShell>()));
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
            PBESpecies species = AvailableSpecies.First();
            team.Item2.Add(new PBEPokemonShell
            {
                Species = species,
                Nickname = PBEPokemonLocalization.Names[species].FromUICultureInfo(),
                Level = PBEPokemonData.Data[species].MinLevel,
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

            string prevSpeciesName = PBEPokemonLocalization.Names[shell.Species].FromUICultureInfo();

            if (updateShell)
            {
                shell.Species = (PBESpecies)species.SelectedItem;
                if (!illegal.IsChecked.Value && nickname.Text?.Length > settings.MaxPokemonNameLength)
                {
                    nickname.Text = new string(nickname.Text.Take(settings.MaxPokemonNameLength).ToArray());
                }
                else if ((!illegal.IsChecked.Value && string.IsNullOrWhiteSpace(nickname.Text)) || prevSpeciesName.Equals(nickname.Text))
                {
                    nickname.Text = PBEPokemonLocalization.Names[shell.Species].FromUICultureInfo();
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
                party.Items = new object[0];
                party.Items = team.Item2;
                party.InvalidateVisual();
            }
            if (updateControls)
            {
                PBEPokemonData pData = PBEPokemonData.Data[shell.Species];

                if (species.SelectedItem != (object)shell.Species)
                {
                    species.SelectedItem = shell.Species;
                }

                if (nickname.Text != shell.Nickname)
                {
                    nickname.Text = shell.Nickname;
                }

                level.Maximum = illegal.IsChecked.Value ? byte.MaxValue : settings.MaxLevel;
                level.Minimum = illegal.IsChecked.Value ? byte.MinValue : pData.MinLevel;
                shell.Level = (byte)PBEUtils.Clamp(shell.Level, level.Minimum, level.Maximum);
                if (level.Value != shell.Level)
                {
                    level.Value = shell.Level;
                }

                if (friendship.Value != shell.Friendship)
                {
                    friendship.Value = shell.Friendship;
                }

                if (!(shiny.IsEnabled = illegal.IsChecked.Value || !pData.ShinyLocked))
                {
                    shell.Shiny = false;
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
                    AvailableAbilities = pData.Abilities.OrderBy(a => PBEAbilityLocalization.Names[a].FromUICultureInfo());
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

        void Connect()
        {
            var client = new BattleClient(ip.Text, (int)port.Value, PBEBattleFormat.Double, settings, team.Item2);
            battles.Add(client);
            List<object> pages = tabs.Items.Cast<object>().ToList();
            pages.Add(new TabItem
            {
                Header = "Battle " + battles.Count,
                Content = new BattleView(client)
            });
            tabs.Items = pages;
            client.Connect();
        }
    }
}
