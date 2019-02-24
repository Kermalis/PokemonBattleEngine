using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
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
    public class MainWindow : Window, INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        string connectText = "Connect";
        public string ConnectText
        {
            get => connectText; set
            {
                connectText = value;
                OnPropertyChanged(nameof(ConnectText));
            }
        }
        readonly Subject<bool> connectEnabled;

        readonly List<BattleClient> battles = new List<BattleClient>();

        readonly TabControl tabs;
        readonly TeamBuilderView teamBuilder;
        readonly TextBox ip;
        readonly NumericUpDown port;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            tabs = this.FindControl<TabControl>("Tabs");
            teamBuilder = this.FindControl<TeamBuilderView>("TeamBuilder");
            ip = this.FindControl<TextBox>("IP");
            port = this.FindControl<NumericUpDown>("Port");
            connectEnabled = new Subject<bool>();
            this.FindControl<Button>("Connect").Command = ReactiveCommand.Create(Connect, connectEnabled);
            connectEnabled.OnNext(true);
        }
        byte shows = 0;
        public override void Show()
        {
            base.Show();
            if (shows++ == 1)
            {
                teamBuilder.Load();
            }
        }

        void Connect()
        {
            connectEnabled.OnNext(false);
            ConnectText = "Connecting...";
            var client = new BattleClient(ip.Text, (int)port.Value, PBEBattleFormat.Double, teamBuilder.settings, teamBuilder.team.Item2);
            var battleView = new BattleView(client);

            new Thread(() =>
            {
                client.Connect();
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (client.IsConnected)
                    {
                        battles.Add(client);
                        var pages = tabs.Items.Cast<object>().ToList();
                        pages.Add(new TabItem
                        {
                            Header = "Battle " + battles.Count,
                            Content = battleView
                        });
                        tabs.Items = pages;
                    }
                    ConnectText = "Connect";
                    connectEnabled.OnNext(true);
                });
            })
            {
                Name = "Connect Thread"
            }.Start();
        }
        void WatchReplay()
        {
            var battle = PBEBattle.LoadReplay(@"D:\Development\GitHub\PokemonBattleEngine\PokemonBattleEngine\bin\Debug\netstandard2.0\Test Replay.pbereplay");

            var client = new BattleClient(ip.Text, (int)port.Value, battle.BattleFormat, teamBuilder.settings, teamBuilder.team.Item2);
            var battleView = new BattleView(client);

            battles.Add(client);
            var pages = tabs.Items.Cast<object>().ToList();
            pages.Add(new TabItem
            {
                Header = "Battle " + battles.Count,
                Content = battleView
            });
            tabs.Items = pages;

            client.Battle = battle;
        }
    }
}
