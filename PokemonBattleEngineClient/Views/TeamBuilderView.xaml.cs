using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using Kermalis.PokemonBattleEngineClient.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    public class TeamBuilderView : UserControl, INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public new event PropertyChangedEventHandler PropertyChanged;

        private Stream spriteStream;
        public Stream SpriteStream
        {
            get => spriteStream;
            set
            {
                spriteStream = value;
                OnPropertyChanged(nameof(SpriteStream));
            }
        }

        private PBEPokemonShell shell;
        public PBEPokemonShell Shell
        {
            get => shell;
            set
            {
                shell = value;
                OnPropertyChanged(nameof(Shell));
            }
        }
        private TeamInfo team;
        public TeamInfo Team
        {
            get => team;
            set
            {
                team = value;
                OnPropertyChanged(nameof(Team));
            }
        }
        public ObservableCollection<TeamInfo> Teams { get; } = new ObservableCollection<TeamInfo>();

        private readonly string teamPath;

        private readonly Button addParty, removeParty;
        private readonly ListBox party;

        public TeamBuilderView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);

            addParty = this.FindControl<Button>("AddParty");
            addParty.Command = ReactiveCommand.Create(AddPartyMember);
            removeParty = this.FindControl<Button>("RemoveParty");
            removeParty.Command = ReactiveCommand.Create(RemovePartyMember);
            this.FindControl<Button>("AddTeam").Command = ReactiveCommand.Create(AddTeam);
            this.FindControl<Button>("RemoveTeam").Command = ReactiveCommand.Create(RemoveTeam);
            this.FindControl<Button>("SaveTeam").Command = ReactiveCommand.Create(SaveTeam);
            party = this.FindControl<ListBox>("Party");
            this.FindControl<ListBox>("SavedTeams").SelectionChanged += (s, e) =>
            {
                SetButtonEnableds();
                Shell = team.Shell[0];
            };
            this.FindControl<ComboBox>("Species").SelectionChanged += (s, e) => UpdateSprites();
            this.FindControl<CheckBox>("Shiny").Command = ReactiveCommand.Create(UpdateSprites);
            this.FindControl<ComboBox>("Gender").SelectionChanged += (s, e) => UpdateSprites();

            teamPath = Path.Combine(Utils.WorkingDirectory, "Teams");
            if (Directory.Exists(teamPath))
            {
                string[] files = Directory.GetFiles(teamPath);
                if (files.Length > 0)
                {
                    foreach (string f in files)
                    {
                        Teams.Add(new TeamInfo { Name = Path.GetFileNameWithoutExtension(f), Shell = new PBETeamShell(f) });
                    }
                    Team = Teams[0];
                    return;
                }
            }
            else
            {
                Directory.CreateDirectory(teamPath);
            }
            AddTeam();
        }

        private void AddTeam()
        {
            var t = new TeamInfo
            {
                Name = $"Team {DateTime.Now.Ticks}",
                Shell = new PBETeamShell(PBESettings.DefaultSettings, 1, true)
            };
            Teams.Add(t);
            Team = t;
            Shell = t.Shell[0];
        }
        private void RemoveTeam()
        {
            File.Delete(Path.Combine(teamPath, $"{team.Name}.json"));
            TeamInfo old = team;
            if (Teams.Count == 1)
            {
                AddTeam();
            }
            Teams.Remove(old);
        }
        private void SaveTeam()
        {
            team.Shell.ToJsonFile(Path.Combine(teamPath, $"{team.Name}.json"));
        }
        private void AddPartyMember()
        {
            int index = team.Shell.Count;
            team.Shell.Add(PBEUtils.RandomSpecies(), team.Shell.Settings.MaxLevel);
            Shell = team.Shell[index];
            SetButtonEnableds();
        }
        private void RemovePartyMember()
        {
            team.Shell.Remove(shell);
            SetButtonEnableds();
        }
        private void SetButtonEnableds()
        {
            addParty.IsEnabled = team.Shell.Count < team.Shell.Settings.MaxPartySize;
            removeParty.IsEnabled = team.Shell.Count > 1;
        }

        private void UpdateSprites()
        {
            SpriteStream = Utils.GetPokemonSpriteStream(shell);
            // Force redraw of minisprite
            if (party.ItemContainerGenerator.ContainerFromIndex(party.SelectedIndex) is ListBoxItem c)
            {
                object old = c.Content;
                c.Content = null;
                c.Content = old;
            }
        }
    }
}
