using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Views;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace Kermalis.PokemonBattleEngineClient
{
    public class MainView : UserControl, INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public new event PropertyChangedEventHandler PropertyChanged;

        private string connectText = "Connect";
        public string ConnectText
        {
            get => connectText;
            set
            {
                connectText = value;
                OnPropertyChanged(nameof(ConnectText));
            }
        }

        private readonly List<BattleClient> battles = new List<BattleClient>();

        private /*readonly*/ TabControl tabs;
        private /*readonly*/ TeamBuilderView teamBuilder;
        private /*readonly*/ TextBox ip;
        private /*readonly*/ NumericUpDown port;
        private /*readonly*/ Subject<bool> connectEnabled;

        public MainView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            // Temporary fix for https://github.com/AvaloniaUI/Avalonia/issues/2562 (remove readonly comments too when fixed) (stopped working when switching to PR build)
            // Also a fix for https://github.com/AvaloniaUI/Avalonia/issues/2656
            Initialized += (s, e) =>
            {
                tabs = this.FindControl<TabControl>("Tabs");
                teamBuilder = this.FindControl<TeamBuilderView>("TeamBuilder"); // teamBuilder will be null
                ip = this.FindControl<TextBox>("IP");
                port = this.FindControl<NumericUpDown>("Port");
                connectEnabled = new Subject<bool>();
                this.FindControl<Button>("Connect").Command = ReactiveCommand.Create(WatchReplay, connectEnabled);
                connectEnabled.OnNext(true);
            };
        }

        private void Connect()
        {
            connectEnabled.OnNext(false);
            ConnectText = "Connecting...";
            var client = new BattleClient(ip.Text, (int)port.Value, PBEBattleFormat.Double, teamBuilder.settings, teamBuilder.Team.Party);
            new Thread(() =>
            {
                client.Connect();
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (client.IsConnected)
                    {
                        Add(client);
                    }
                    ConnectText = "Connect";
                    connectEnabled.OnNext(true);
                });
            })
            {
                Name = "Connect Thread"
            }.Start();
        }
        private void WatchReplay()
        {
            Add(new BattleClient(PBEBattle.LoadReplay(@"D:\Development\GitHub\PokemonBattleEngine\PokemonBattleEngineExtras\bin\Debug\netcoreapp2.1\Test Replay.pbereplay"), BattleClient.ClientMode.Replay));
        }
        private void SinglePlayer()
        {
            PBESettings settings = PBESettings.DefaultSettings;
            PBEPokemonShell[] team0Party, team1Party;
            team0Party = PBEUtils.CreateCompletelyRandomTeam(settings);
            team1Party = PBEUtils.CreateCompletelyRandomTeam(settings);
            var battle = new PBEBattle(PBEBattleFormat.Double, settings, team0Party, team1Party);
            battle.Teams[0].TrainerName = "May";
            battle.Teams[1].TrainerName = "Champion Steven";
            Add(new BattleClient(battle, BattleClient.ClientMode.SinglePlayer));
            new Thread(battle.Begin) { Name = "Battle Thread" }.Start();
        }

        // TODO: Removing battles
        private void Add(BattleClient client)
        {
            var battleView = new BattleView(client);
            battles.Add(client);
            var pages = tabs.Items.Cast<object>().ToList();
            var tab = new TabItem
            {
                Header = "Battle " + battles.Count,
                Content = battleView
            };
            pages.Add(tab);
            tabs.Items = pages;
            tabs.SelectedItem = tab;
        }
    }
}
