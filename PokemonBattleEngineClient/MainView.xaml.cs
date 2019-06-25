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

        private TabControl tabs;
        private TeamBuilderView teamBuilder;
        private TextBox ip;
        private NumericUpDown port;
        private Subject<bool> connectEnabled;

        public MainView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);

            // Temporary fix for https://github.com/AvaloniaUI/Avalonia/issues/2656
            Initialized += (s, e) =>
            {
                tabs = this.FindControl<TabControl>("Tabs");
                //teamBuilder = this.FindControl<TeamBuilderView>("TeamBuilder"); // teamBuilder will be null
                teamBuilder = TemporaryFix<TeamBuilderView>("TeamBuilder");
                ip = this.FindControl<TextBox>("IP");
                port = this.FindControl<NumericUpDown>("Port");
                connectEnabled = new Subject<bool>();
                this.FindControl<Button>("Connect").Command = ReactiveCommand.Create(Connect, connectEnabled);
                connectEnabled.OnNext(true);
            };
        }

        private void Connect()
        {
            connectEnabled.OnNext(false);
            ConnectText = "Connecting...";
            var client = new BattleClient(ip.Text, (int)port.Value, PBEBattleFormat.Double, teamBuilder.Settings, teamBuilder.Team.Party);
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
            team0Party = PBEUtils.CreateCompletelyRandomTeam(settings, true);
            team1Party = PBEUtils.CreateCompletelyRandomTeam(settings, true);
            var battle = new PBEBattle(PBEBattleFormat.Double, settings, team0Party, team1Party);
            battle.Teams[0].TrainerName = "May";
            battle.Teams[1].TrainerName = "Champion Steven";
            Add(new BattleClient(battle, BattleClient.ClientMode.SinglePlayer));
            new Thread(battle.Begin) { Name = "Battle Thread" }.Start();
        }

        // TODO: Removing battles
        private void Add(BattleClient client)
        {
            battles.Add(client);
            var pages = tabs.Items.Cast<object>().ToList();
            var tab = new TabItem
            {
                Header = "Battle " + battles.Count,
                Content = client.BattleView
            };
            pages.Add(tab);
            tabs.Items = pages;
            tabs.SelectedItem = tab;
        }

        // Temporary fix for https://github.com/AvaloniaUI/Avalonia/issues/2562
        private T TemporaryFix<T>(string name) where T : UserControl
        {
            T Recursion(IEnumerable<Avalonia.LogicalTree.ILogical> list)
            {
                foreach (Avalonia.LogicalTree.ILogical i in list)
                {
                    if (i is Avalonia.INamed named && named is T ret && ret.Name == name)
                    {
                        return ret;
                    }
                    else
                    {
                        T r = Recursion(i.LogicalChildren);
                        if (r != null)
                        {
                            return r;
                        }
                    }
                }
                return null;
            }
            return Recursion(LogicalChildren);
        }
    }
}
