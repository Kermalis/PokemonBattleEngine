using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Legality;
using Kermalis.PokemonBattleEngine.Utils;
using Kermalis.PokemonBattleEngineClient.Views;
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
            _teamBuilder = this.FindControl<TeamBuilderView>("TeamBuilder");
            _ip = this.FindControl<TextBox>("IP");
            _port = this.FindControl<NumericUpDown>("Port");
            _connect = this.FindControl<Button>("Connect");
            _connect.IsEnabled = true;
        }

        public void Connect()
        {
            _connect.IsEnabled = false;
            ConnectText = "Connecting...";
            string host = _ip.Text;
            ushort port = (ushort)_port.Value;
            var client = new NetworkClient(PBEBattleFormat.Double, _teamBuilder.Team.Party); // Must be called on UI thread
            new Thread(() =>
            {
                bool b;
                try
                {
                    b = client.Connect(host, port);
                }
                catch
                {
                    b = false;
                }
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (b)
                    {
                        Add(client);
                    }
                    else
                    {
                        client.Dispose();
                    }
                    ConnectText = "Connect";
                    _connect.IsEnabled = true;
                });
            })
            {
                Name = "Connect Thread"
            }.Start();
        }
        public void WatchReplay()
        {
            const string path = "SinglePlayer Battle.pbereplay";
            //const string path = @"C:\Users\Kermalis\Documents\Development\GitHub\PokeI\bin\Release\netcoreapp3.1\AI Final Replay.pbereplay";
            Add(new ReplayClient(path));
        }
        public void SinglePlayer()
        {
            PBESettings settings = PBESettings.DefaultSettings;
            PBELegalPokemonCollection p0, p1;
            // Competitively Randomized Pokémon
            p0 = PBERandomTeamGenerator.CreateRandomTeam(settings.MaxPartySize);
            p1 = PBERandomTeamGenerator.CreateRandomTeam(settings.MaxPartySize);

            Add(new SinglePlayerClient(PBEBattleFormat.Double, new PBETeamInfo(p0, "Dawn"), new PBETeamInfo(p1, "Champion Cynthia"), settings));
        }

        // TODO: Removing battles (with disposing)
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

        internal void HandleClosing()
        {
            foreach (BattleClient bc in _battles)
            {
                bc.Dispose();
            }
        }
    }
}
