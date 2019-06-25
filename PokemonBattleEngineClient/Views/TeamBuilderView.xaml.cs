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
using System.Reactive.Subjects;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    // TODO: Settings editor, listen to settings changes
    // TODO: PPUps.Maximum, ivs.Maximum
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
        private string teamPath;

        private Subject<bool> addPartyEnabled, removePartyEnabled;
        private ListBox party;

        public TeamBuilderView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);

            // Temporary fix for https://github.com/AvaloniaUI/Avalonia/issues/2656
            Initialized += (sh, he) =>
            {
                addPartyEnabled = new Subject<bool>();
                this.FindControl<Button>("AddParty").Command = ReactiveCommand.Create(AddPartyMember, addPartyEnabled);
                removePartyEnabled = new Subject<bool>();
                this.FindControl<Button>("RemoveParty").Command = ReactiveCommand.Create(RemovePartyMember, removePartyEnabled);
                this.FindControl<Button>("AddTeam").Command = ReactiveCommand.Create(AddTeam);
                this.FindControl<Button>("RemoveTeam").Command = ReactiveCommand.Create(RemoveTeam);
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
            };
        }

        private PBEPokemonShell CreateShell()
        {
            return new PBEPokemonShell(PBEPokemonShell.AllSpecies.Sample(), Settings.MaxLevel, Settings);
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
            addPartyEnabled.OnNext(team.Party.Count < Settings.MaxPartySize);
            // Remove if too many
            if (team.Party.Count > Settings.MaxPartySize)
            {
                int removeAmt = team.Party.Count - Settings.MaxPartySize;
                for (int i = 0; i < removeAmt; i++)
                {
                    team.Party.RemoveAt(team.Party.Count - 1);
                }
            }
            removePartyEnabled.OnNext(team.Party.Count > 1);
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
