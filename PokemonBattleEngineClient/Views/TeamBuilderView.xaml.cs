using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using Kermalis.PokemonBattleEngineClient.Models;
using ReactiveUI;
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
                    _shell = value;
                    OnPropertyChanged(nameof(Shell));
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

        public TeamBuilderView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);

            _addPartyButton = this.FindControl<Button>("AddParty");
            _addPartyButton.Command = ReactiveCommand.Create(AddPartyMember);
            _removePartyButton = this.FindControl<Button>("RemoveParty");
            _removePartyButton.Command = ReactiveCommand.Create(RemovePartyMember);
            this.FindControl<Button>("AddTeam").Command = ReactiveCommand.Create(AddTeam);
            this.FindControl<Button>("RemoveTeam").Command = ReactiveCommand.Create(RemoveTeam);
            this.FindControl<Button>("SaveTeam").Command = ReactiveCommand.Create(SaveTeam);
            _partyListBox = this.FindControl<ListBox>("Party");
            this.FindControl<ListBox>("SavedTeams").SelectionChanged += OnSelectedTeamChanged;
            this.FindControl<ComboBox>("Species").SelectionChanged += (s, e) => UpdateSprites();
            this.FindControl<CheckBox>("Shiny").Command = ReactiveCommand.Create(UpdateSprites);
            this.FindControl<ComboBox>("Gender").SelectionChanged += (s, e) => UpdateSprites();

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

        private void AddTeam()
        {
            var t = new TeamInfo($"Team {DateTime.Now.Ticks}", new PBETeamShell(new PBESettings(PBESettings.DefaultSettings), 1, true));
            Teams.Add(t);
            Team = t;
            Shell = t.Shell[0];
        }
        private void RemoveTeam()
        {
            File.Delete(Path.Combine(_teamPath, $"{_team.Name}.json"));
            TeamInfo old = _team;
            if (Teams.Count == 1)
            {
                AddTeam();
            }
            Teams.Remove(old);
        }
        private void SaveTeam()
        {
            _team.Shell.ToJsonFile(Path.Combine(_teamPath, $"{_team.Name}.json"));
        }
        private void AddPartyMember()
        {
            int index = _team.Shell.Count;
            _team.Shell.Add(PBEUtils.RandomSpecies(), _team.Shell.Settings.MaxLevel);
            Shell = _team.Shell[index];
        }
        private void RemovePartyMember()
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
        private void UpdateSprites()
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
