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

        private readonly TabControl tabs;
        private readonly TeamBuilderView teamBuilder;
        private readonly TextBox ip;
        private readonly NumericUpDown port;
        private readonly Button connect;

        public MainView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);

            tabs = this.FindControl<TabControl>("Tabs");
            //teamBuilder = this.FindControl<TeamBuilderView>("TeamBuilder"); // teamBuilder will be null
            teamBuilder = TemporaryFix<TeamBuilderView>("TeamBuilder");
            ip = this.FindControl<TextBox>("IP");
            port = this.FindControl<NumericUpDown>("Port");
            connect = this.FindControl<Button>("Connect");
            connect.Command = ReactiveCommand.Create(Connect);
            connect.IsEnabled = true;
        }

        private void Connect()
        {
            connect.IsEnabled = false;
            ConnectText = "Connecting...";
            var client = new BattleClient(ip.Text, (int)port.Value, PBEBattleFormat.Double, teamBuilder.Team.Shell);
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
                    connect.IsEnabled = true;
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
            PBETeamShell team0Shell, team1Shell;
            // Completely Randomized Pokémon
            team0Shell = new PBETeamShell(settings, settings.MaxPartySize, true);
            team1Shell = new PBETeamShell(settings, settings.MaxPartySize, true);

            var battle = new PBEBattle(PBEBattleFormat.Double, team0Shell, team1Shell);
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
