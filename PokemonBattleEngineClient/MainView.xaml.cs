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
using System.Threading;

namespace Kermalis.PokemonBattleEngineClient
{
    public sealed class MainView : UserControl, INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public new event PropertyChangedEventHandler PropertyChanged;

        private string _connectText = "Connect";
        public string ConnectText
        {
            get => _connectText;
            private set
            {
                if (_connectText != value)
                {
                    _connectText = value;
                    OnPropertyChanged(nameof(ConnectText));
                }
            }
        }

        private readonly List<BattleClient> _battles = new List<BattleClient>();

        private readonly TabControl _tabs;
        private readonly TeamBuilderView _teamBuilder;
        private readonly TextBox _ip;
        private readonly NumericUpDown _port;
        private readonly Button _connect;

        public MainView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);

            _tabs = this.FindControl<TabControl>("Tabs");
            //teamBuilder = this.FindControl<TeamBuilderView>("TeamBuilder"); // teamBuilder will be null
            _teamBuilder = TemporaryFix<TeamBuilderView>("TeamBuilder");
            _ip = this.FindControl<TextBox>("IP");
            _port = this.FindControl<NumericUpDown>("Port");
            _connect = this.FindControl<Button>("Connect");
            _connect.Command = ReactiveCommand.Create(Connect);
            _connect.IsEnabled = true;
        }

        private void Connect()
        {
            _connect.IsEnabled = false;
            ConnectText = "Connecting...";
            var client = new BattleClient(_ip.Text, (int)_port.Value, PBEBattleFormat.Double, _teamBuilder.Team.Shell);
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
                    _connect.IsEnabled = true;
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
            PBETeamShell team1Shell, team2Shell;
            // Completely Randomized Pokémon
            team1Shell = new PBETeamShell(settings, settings.MaxPartySize, true);
            team2Shell = new PBETeamShell(settings, settings.MaxPartySize, true);

            var battle = new PBEBattle(PBEBattleFormat.Double, team1Shell, "May", team2Shell, "Champion Steven");
            Add(new BattleClient(battle, BattleClient.ClientMode.SinglePlayer));
            new Thread(battle.Begin) { Name = "Battle Thread" }.Start();
        }

        // TODO: Removing battles
        private void Add(BattleClient client)
        {
            _battles.Add(client);
            var pages = _tabs.Items.Cast<object>().ToList();
            var tab = new TabItem
            {
                Header = "Battle " + _battles.Count,
                Content = client.BattleView
            };
            pages.Add(tab);
            _tabs.Items = pages;
            _tabs.SelectedItem = tab;
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
