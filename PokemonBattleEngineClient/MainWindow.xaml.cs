using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngineClient.Views;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngineClient
{
    class MainWindow : Window
    {
        ReactiveCommand ConnectCommand { get; }
        readonly List<BattleClient> battles = new List<BattleClient>();

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
            ConnectCommand = ReactiveCommand.Create<string>(Connect);
        }

        void Connect(string arg)
        {
            var client = new BattleClient(this.FindControl<TextBox>("IP").Text, (int)this.FindControl<NumericUpDown>("Port").Value);
            var tabControl = this.FindControl<TabControl>("Tabs");
            List<object> tabs = tabControl.Items.Cast<object>().ToList();
            tabs.Add(new TabItem
            {
                Header = "Battle " + (battles.Count + 1),
                Content = new BattleView(client)
            });
            tabControl.Items = tabs;
            battles.Add(client);
            client.Connect();
        }
    }
}
