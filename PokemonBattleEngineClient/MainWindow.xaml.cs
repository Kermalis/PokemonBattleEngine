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
using System.Linq;

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

        readonly IEnumerable<PBEAbility> allAbilities = PBEAbilityLocalization.Names.OrderBy(k => k.Value.English).Select(k => k.Key);
        readonly IEnumerable<PBEGender> allGenders = Enum.GetValues(typeof(PBEGender)).Cast<PBEGender>().Except(new[] { PBEGender.MAX });
        readonly IEnumerable<PBEItem> allItems = PBEItemLocalization.Names.OrderBy(k => k.Value.English).Select(k => k.Key);

        IEnumerable<PBESpecies> AvailableSpecies { get; } = PBEPokemonLocalization.Names.OrderBy(k => k.Value.English).Select(k => k.Key);
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

        PBEPokemonShell shell;
        ObservableCollection<PBEPokemonShell> Shells { get; } = new ObservableCollection<PBEPokemonShell>();

        ReactiveCommand AddCommand { get; }
        ReactiveCommand ConnectCommand { get; }

        readonly PBESettings settings = PBESettings.DefaultSettings;
        readonly List<BattleClient> battles = new List<BattleClient>();

        readonly TabControl tabs;
        readonly TextBox ip, nickname;
        readonly NumericUpDown port, level, friendship;
        readonly ListBox party;
        readonly CheckBox illegal, shiny;
        readonly DropDown species, ability, nature, gender, item;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            this.FindControl<Button>("Add").Command = ReactiveCommand.Create(AddShell);
            this.FindControl<Button>("Connect").Command = ReactiveCommand.Create(Connect);
            tabs = this.FindControl<TabControl>("Tabs");
            ip = this.FindControl<TextBox>("IP");
            port = this.FindControl<NumericUpDown>("Port");
            party = this.FindControl<ListBox>("Party");
            party.SelectionChanged += (s, e) =>
            {
                if (party.SelectedIndex > -1)
                {
                    shell = Shells[party.SelectedIndex];
                    Update(false, true, true, true);
                    Update(true, false, false, true);
                }
            };
            illegal = this.FindControl<CheckBox>("Illegal");
            illegal.Command = ReactiveCommand.Create(() => Update(false, true, false));
            species = this.FindControl<DropDown>("Species");
            species.SelectionChanged += (s, e) => Update(true, true, true);
            nickname = this.FindControl<TextBox>("Nickname");
            nickname.LostFocus += (s, e) => Update(true, false, true);
            level = this.FindControl<NumericUpDown>("Level");
            level.ValueChanged += (s, e) => Update(true, false, false);
            friendship = this.FindControl<NumericUpDown>("Friendship");
            friendship.ValueChanged += (s, e) => Update(true, false, false);
            shiny = this.FindControl<CheckBox>("Shiny");
            shiny.Command = ReactiveCommand.Create(() => Update(true, false, true));
            ability = this.FindControl<DropDown>("Ability");
            ability.SelectionChanged += (s, e) => Update(true, false, false);
            nature = this.FindControl<DropDown>("Nature");
            nature.SelectionChanged += (s, e) => Update(true, false, false);
            gender = this.FindControl<DropDown>("Gender");
            gender.SelectionChanged += (s, e) => Update(true, false, true);
            item = this.FindControl<DropDown>("Item");
            item.SelectionChanged += (s, e) => Update(true, false, false);
        }
        byte shows = 0;
        public override void Show()
        {
            base.Show();
            if (shows++ == 1)
            {
                AddShell();
                party.SelectedIndex = 0;
            }
        }

        void AddShell()
        {
            PBESpecies species = AvailableSpecies.First();
            Shells.Add(new PBEPokemonShell { Species = species, Nickname = PBEPokemonLocalization.Names[species].English });
        }

        bool ignoreUpdate = false;
        void Update(bool updateShell, bool updateControls, bool updateSprites, bool toggleIgnore = false)
        {
            if (toggleIgnore)
            {
                ignoreUpdate = !ignoreUpdate;
            }
            else if (ignoreUpdate)
            {
                return;
            }

            string prevSpeciesName = PBEPokemonLocalization.Names[shell.Species].English;

            if (updateShell)
            {
                shell.Species = (PBESpecies)species.SelectedItem;
                if (!illegal.IsChecked.Value && nickname.Text?.Length > settings.MaxPokemonNameLength)
                {
                    nickname.Text = new string(nickname.Text.Take(settings.MaxPokemonNameLength).ToArray());
                }
                else if ((!illegal.IsChecked.Value && string.IsNullOrWhiteSpace(nickname.Text)) || prevSpeciesName.Equals(nickname.Text))
                {
                    nickname.Text = PBEPokemonLocalization.Names[shell.Species].English;
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
            }
            if (updateSprites)
            {
                Source = Utils.GetPokemonSpriteUri(shell);
                // Force redraw
                party.Items = new object[0];
                party.Items = Shells;
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
                shell.Level = (byte)level.Value.Clamp(level.Minimum, level.Maximum);
                if (level.Value != shell.Level)
                {
                    //level.Value = shell.Level // Crashing
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
                    Update(false, false, true);
                }

                if (illegal.IsChecked.Value)
                {
                    AvailableAbilities = allAbilities;
                }
                else
                {
                    AvailableAbilities = pData.Abilities.OrderBy(a => PBEAbilityLocalization.Names[a].English);
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
            }
        }

        void Connect()
        {
            var client = new BattleClient(ip.Text, (int)port.Value, PBEBattleFormat.Double, settings);
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
