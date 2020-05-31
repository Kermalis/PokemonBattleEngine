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
using System.Reactive.Linq;

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
        private readonly ComboBox _abilityComboBox;

        private IDisposable Test(IObserver<object> thing)
        {
            thing.OnNext(PBEAbility.Adaptability);
            return null;
        }

        public TeamBuilderView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);

            _addPartyButton = this.FindControl<Button>("AddParty");
            //this.FindControl<ComboBox>("Form").bin
            _removePartyButton = this.FindControl<Button>("RemoveParty");
            _partyListBox = this.FindControl<ListBox>("Party");
            _abilityComboBox = this.FindControl<ComboBox>("Ability");
            _abilityComboBox.Bind(ComboBox.SelectedItemProperty, Observable.Create<object>(Test));
            this.FindControl<ListBox>("SavedTeams").SelectionChanged += OnSelectedTeamChanged;
            this.FindControl<ComboBox>("Species").SelectionChanged += OnVisualChanged;
            this.FindControl<ComboBox>("Form").SelectionChanged += OnVisualChanged;
            this.FindControl<ComboBox>("Gender").SelectionChanged += OnVisualChanged;

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
