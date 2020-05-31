using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using Kermalis.PokemonBattleEngineClient.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    public sealed class TeamBuilderView : UserControl, INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public new event PropertyChangedEventHandler PropertyChanged;

        private Stream _spriteStream;
        public Stream SpriteStream
        {
            get => _spriteStream;
            private set
            {
                if (_spriteStream != value)
                {
                    _spriteStream = value;
                    OnPropertyChanged(nameof(SpriteStream));
                }
            }
        }

        private PBEPokemonShell _shell;
        public PBEPokemonShell Shell
        {
            get => _shell;
            set
            {
                if (_shell != value)
                {
                    PBEPokemonShell old = _shell;
                    if (old != null)
                    {
                        old.PropertyChanged -= OnShellPropertyChanged;
                    }
                    _shell = value;
                    value.PropertyChanged += OnShellPropertyChanged;
                    _ignoreComboBoxChanges = true;
                    OnPropertyChanged(nameof(Shell));
                    UpdateComboBoxes(null);
                    _ignoreComboBoxChanges = false;
                }
            }
        }
        private TeamInfo _team;
        public TeamInfo Team
        {
            get => _team;
            set
            {
                if (_team != value)
                {
                    _team = value;
                    OnPropertyChanged(nameof(Team));
                }
            }
        }
        public ObservableCollection<TeamInfo> Teams { get; } = new ObservableCollection<TeamInfo>();

        private readonly string _teamPath;
        private readonly Button _addPartyButton;
        private readonly Button _removePartyButton;
        private readonly ListBox _partyListBox;
        private bool _ignoreComboBoxChanges = false;
        private readonly ComboBox _abilityComboBox;
        private readonly ComboBox _formComboBox;
        private readonly ComboBox _genderComboBox;
        private readonly ComboBox _itemComboBox;
        private readonly ComboBox _speciesComboBox;

        private void UpdateComboBoxes(string property)
        {
            bool all = property == null;
            bool ability = all;
            bool form = all;
            bool gender = all;
            bool item = all;
            bool species = all;
            if (!all)
            {
                switch (property)
                {
                    case nameof(PBEPokemonShell.Ability): ability = true; break;
                    case nameof(PBEPokemonShell.Form): form = true; break;
                    case nameof(PBEPokemonShell.Gender): gender = true; break;
                    case nameof(PBEPokemonShell.Item): item = true; break;
                    case nameof(PBEPokemonShell.Species): species = true; break;
                }
            }
            if (ability)
            {
                _abilityComboBox.SelectedItem = _shell.Ability;
            }
            if (form)
            {
                _formComboBox.SelectedItem = _shell.Form;
            }
            if (gender)
            {
                _genderComboBox.SelectedItem = _shell.Gender;
            }
            if (item)
            {
                _itemComboBox.SelectedItem = _shell.Item;
            }
            if (species)
            {
                _speciesComboBox.SelectedItem = _shell.Species;
            }
        }
        private void OnShellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateComboBoxes(e.PropertyName);
        }
        private void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs thing)
        {
            if (!_ignoreComboBoxChanges)
            {
                _ignoreComboBoxChanges = true;
                var c = (ComboBox)sender;
                if (c == _abilityComboBox)
                {
                    _shell.Ability = (PBEAbility)c.SelectedItem;
                }
                else if (c == _formComboBox)
                {
                    _shell.Form = (PBEForm)c.SelectedItem;
                }
                else if (c == _genderComboBox)
                {
                    _shell.Gender = (PBEGender)c.SelectedItem;
                }
                else if (c == _itemComboBox)
                {
                    _shell.Item = (PBEItem)c.SelectedItem;
                }
                else if (c == _speciesComboBox)
                {
                    _shell.Species = (PBESpecies)c.SelectedItem;
                }
                _ignoreComboBoxChanges = false;
            }
        }

        public TeamBuilderView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);

            _abilityComboBox = this.FindControl<ComboBox>("Ability");
            _abilityComboBox.SelectionChanged += OnComboBoxSelectionChanged;
            _formComboBox = this.FindControl<ComboBox>("Form");
            _formComboBox.SelectionChanged += OnComboBoxSelectionChanged;
            _formComboBox.SelectionChanged += OnVisualChanged;
            _genderComboBox = this.FindControl<ComboBox>("Gender");
            _genderComboBox.SelectionChanged += OnComboBoxSelectionChanged;
            _genderComboBox.SelectionChanged += OnVisualChanged;
            _itemComboBox = this.FindControl<ComboBox>("Item");
            _itemComboBox.SelectionChanged += OnComboBoxSelectionChanged;
            _speciesComboBox = this.FindControl<ComboBox>("Species");
            _speciesComboBox.SelectionChanged += OnComboBoxSelectionChanged;
            _speciesComboBox.SelectionChanged += OnVisualChanged;
            this.FindControl<ListBox>("SavedTeams").SelectionChanged += OnSelectedTeamChanged;
            _addPartyButton = this.FindControl<Button>("AddParty");
            _removePartyButton = this.FindControl<Button>("RemoveParty");
            _partyListBox = this.FindControl<ListBox>("Party");

            _teamPath = Path.Combine(Utils.WorkingDirectory, "Teams");
            if (Directory.Exists(_teamPath))
            {
                string[] files = Directory.GetFiles(_teamPath);
                if (files.Length > 0)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        string file = files[i];
                        Teams.Add(new TeamInfo(Path.GetFileNameWithoutExtension(file), new PBETeamShell(file)));
                    }
                    Team = Teams[0];
                    return;
                }
            }
            else
            {
                Directory.CreateDirectory(_teamPath);
            }
            AddTeam();
        }

        public void AddTeam()
        {
            var t = new TeamInfo($"Team {DateTime.Now.Ticks}", new PBETeamShell(new PBESettings(PBESettings.DefaultSettings), 1, true));
            Teams.Add(t);
            Team = t;
            Shell = t.Shell[0];
        }
        public void RemoveTeam()
        {
            File.Delete(Path.Combine(_teamPath, $"{_team.Name}.json"));
            TeamInfo old = _team;
            if (Teams.Count == 1)
            {
                AddTeam();
            }
            Teams.Remove(old);
        }
        public void SaveTeam()
        {
            _team.Shell.ToJsonFile(Path.Combine(_teamPath, $"{_team.Name}.json"));
        }
        public void AddPartyMember()
        {
            int index = _team.Shell.Count;
            _team.Shell.AddRandom(true);
            Shell = _team.Shell[index];
        }
        public void RemovePartyMember()
        {
            _team.Shell.Remove(_shell);
        }
        private void OnSelectedTeamSizeChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _addPartyButton.IsEnabled = _team.Shell.Count < _team.Shell.Settings.MaxPartySize;
            _removePartyButton.IsEnabled = _team.Shell.Count > 1;
        }
        private void OnSelectedTeamChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < e.RemovedItems.Count; i++)
            {
                ((TeamInfo)e.RemovedItems[i]).Shell.CollectionChanged -= OnSelectedTeamSizeChanged;
            }
            _team.Shell.CollectionChanged += OnSelectedTeamSizeChanged;
            OnSelectedTeamSizeChanged(null, null);
            Shell = _team.Shell[0];
        }
        private void OnVisualChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSprites();
        }
        public void UpdateSprites()
        {
            SpriteStream = Utils.GetPokemonSpriteStream(_shell);
            // Force redraw of minisprite
            if (_partyListBox.ItemContainerGenerator.ContainerFromIndex(_partyListBox.SelectedIndex) is ListBoxItem item)
            {
                object old = item.Content;
                item.Content = null;
                item.Content = old;
            }
        }
    }
}
