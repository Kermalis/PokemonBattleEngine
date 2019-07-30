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
    // TODO: Settings editor, listen to settings changes
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

        public PBESettings Settings { get; } = PBESettings.DefaultSettings;
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
                EvaluatePartySize();
                Shell = team.Party[0];
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
                        Teams.Add(new TeamInfo { Name = Path.GetFileNameWithoutExtension(f), Party = new ObservableCollection<PBEPokemonShell>(PBEPokemonShell.TeamFromJsonFile(f)) });
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

        private PBEPokemonShell CreateShell()
        {
            return new PBEPokemonShell(PBEUtils.RandomSpecies(), Settings.MaxLevel, Settings);
        }
        private void AddTeam()
        {
            PBEPokemonShell p = CreateShell();
            var t = new TeamInfo
            {
                Name = $"Team {DateTime.Now.Ticks}",
                Party = new ObservableCollection<PBEPokemonShell>()
                {
                    p
                }
            };
            Teams.Add(t);
            Team = t;
            Shell = p;
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
            PBEPokemonShell.TeamToJsonFile(Path.Combine(teamPath, $"{team.Name}.json"), team.Party);
        }
        private void AddPartyMember()
        {
            PBEPokemonShell p = CreateShell();
            team.Party.Add(p);
            Shell = p;
            EvaluatePartySize();
        }
        private void RemovePartyMember()
        {
            team.Party.Remove(shell);
            EvaluatePartySize();
        }
        private void EvaluatePartySize()
        {
            addParty.IsEnabled = team.Party.Count < Settings.MaxPartySize;
            // Remove if too many
            if (team.Party.Count > Settings.MaxPartySize)
            {
                int removeAmt = team.Party.Count - Settings.MaxPartySize;
                for (int i = 0; i < removeAmt; i++)
                {
                    team.Party.RemoveAt(team.Party.Count - 1);
                }
            }
            removeParty.IsEnabled = team.Party.Count > 1;
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
