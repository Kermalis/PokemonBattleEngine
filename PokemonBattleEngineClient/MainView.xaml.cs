using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Legality;
using Kermalis.PokemonBattleEngine.Network;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using Kermalis.PokemonBattleEngineClient.Clients;
using Kermalis.PokemonBattleEngineClient.Views;
using System;
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

        private string _connectText;
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
            ResetConnectButton();
        }
        private void ResetConnectButton()
        {
            ConnectText = "Connect";
            _connect.IsEnabled = true;
        }

        public void Connect()
        {
            _connect.IsEnabled = false;
            ConnectText = "Connecting...";
            string host = _ip.Text;
            ushort port = (ushort)_port.Value;
            new Thread(() =>
            {
                NetworkClientConnection con = null;
                void ConnectHandler(object arg)
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        if (arg is null)
                        {
                            ResetConnectButton();
                            con?.Dispose();
                        }
                        else if (arg is string str)
                        {
                            ConnectText = str;
                        }
                        else
                        {
                            // No need to dispose con because NetworkClient.Dispose disposes the same thing
                            var tup = (Tuple<PBEClient, PBEBattlePacket, byte>)arg;
                            Add(new NetworkClient(tup.Item1, tup.Item2, tup.Item3, $"MP {_battles.Count + 1}"));
                            ResetConnectButton();
                        }
                    });
                }
                try
                {
                    con = new NetworkClientConnection(host, port, _teamBuilder.Team.Party, ConnectHandler);
                }
                catch
                {
                    con = null;
                }
            })
            {
                Name = "Connect Thread"
            }.Start();
        }
        public void WatchReplay()
        {
            const string path = "SinglePlayer Battle.pbereplay";
            //const string path = @"C:\Users\Kermalis\Documents\Development\GitHub\PokemonBattleEngine\PokemonBattleEngineExtras\bin\Debug\netcoreapp3.1\AI Demo.pbereplay";
            //const string path = @"C:\Users\Kermalis\Documents\Development\GitHub\PokeI\bin\Release\netcoreapp3.1\AI Final Replay.pbereplay";
            Add(new ReplayClient(path, $"Replay {_battles.Count + 1}"));
        }
        // Trainer battle
        /*public void SinglePlayer()
        {
            // Competitively Randomized Pokémon
            PBESettings settings = PBESettings.DefaultSettings;
            PBEBattleFormat battleFormat;
            IReadOnlyList<PBETrainerInfo> t0, t1;
            bool multiBattle = true;
            bool triple = true;
            if (multiBattle)
            {
                if (triple)
                {
                    PBELegalPokemonCollection p0, p1, p2, p3, p4, p5;
                    int numPerTrainer = settings.MaxPartySize / 3;
                    p0 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    p1 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    p2 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    p3 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    p4 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    p5 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    t0 = new[] { new PBETrainerInfo(p0, "Dawn"), new PBETrainerInfo(p1, "Barry"), new PBETrainerInfo(p2, "Lucas") };
                    t1 = new[] { new PBETrainerInfo(p3, "Champion Cynthia"), new PBETrainerInfo(p4, "Leader Volkner"), new PBETrainerInfo(p5, "Elite Four Flint") };
                    battleFormat = PBEBattleFormat.Triple;
                }
                else
                {
                    PBELegalPokemonCollection p0, p1, p2, p3;
                    int numPerTrainer = settings.MaxPartySize / 2;
                    p0 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    p1 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    p2 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    p3 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    t0 = new[] { new PBETrainerInfo(p0, "Dawn"), new PBETrainerInfo(p1, "Barry") };
                    t1 = new[] { new PBETrainerInfo(p2, "Leader Volkner"), new PBETrainerInfo(p3, "Elite Four Flint") };
                    battleFormat = PBEBattleFormat.Double;
                }
            }
            else
            {
                PBELegalPokemonCollection p0, p1;
                int numPerTrainer = settings.MaxPartySize;
                p0 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                p1 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                t0 = new[] { new PBETrainerInfo(p0, "Dawn") };
                t1 = new[] { new PBETrainerInfo(p1, "Champion Cynthia") };
                battleFormat = triple ? PBEBattleFormat.Triple : PBEBattleFormat.Double;
            }
            var b = new PBEBattle(battleFormat, settings, t0, t1,
                battleTerrain: PBEUtils.GlobalRandom.RandomBattleTerrain());
            Add(new SinglePlayerClient(b, $"SP {_battles.Count + 1}"));
        }*/
        // Wild battle
        public void SinglePlayer()
        {
            // Competitively Randomized Pokémon
            PBESettings settings = PBESettings.DefaultSettings;
            PBEBattleFormat battleFormat;
            IReadOnlyList<PBETrainerInfo> t0;
            PBEWildInfo wi;
            bool multiBattle = false;
            bool triple = false;
            if (multiBattle)
            {
                if (triple)
                {
                    PBELegalPokemonCollection p0, p1, p2, w;
                    int numPerTrainer = settings.MaxPartySize / 3;
                    p0 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    p1 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    p2 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    w = PBERandomTeamGenerator.CreateRandomTeam(3);
                    t0 = new[] { new PBETrainerInfo(p0, "Dawn"), new PBETrainerInfo(p1, "Barry"), new PBETrainerInfo(p2, "Lucas") };
                    wi = new PBEWildInfo(w);
                    battleFormat = PBEBattleFormat.Triple;
                }
                else
                {
                    PBELegalPokemonCollection p0, p1, w;
                    int numPerTrainer = settings.MaxPartySize / 2;
                    p0 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    p1 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                    w = PBERandomTeamGenerator.CreateRandomTeam(2);
                    t0 = new[] { new PBETrainerInfo(p0, "Dawn"), new PBETrainerInfo(p1, "Barry") };
                    wi = new PBEWildInfo(w);
                    battleFormat = PBEBattleFormat.Double;
                }
            }
            else
            {
                PBELegalPokemonCollection p0, w;
                int numPerTrainer = settings.MaxPartySize;
                p0 = PBERandomTeamGenerator.CreateRandomTeam(numPerTrainer);
                w = PBERandomTeamGenerator.CreateRandomTeam(2);
                t0 = new[] { new PBETrainerInfo(p0, "Dawn") };
                wi = new PBEWildInfo(w);
                battleFormat = triple ? PBEBattleFormat.Triple : PBEBattleFormat.Double;
            }
            var b = new PBEBattle(battleFormat, settings, t0, wi,
                battleTerrain: PBEUtils.GlobalRandom.RandomBattleTerrain());
            Add(new SinglePlayerClient(b, $"SP {_battles.Count + 1}"));
        }

        // TODO: Removing battles (with disposing)
        private void Add(BattleClient client)
        {
            _battles.Add(client);
            var pages = _tabs.Items.Cast<object>().ToList();
            var tab = new TabItem
            {
                Header = client.Name,
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
