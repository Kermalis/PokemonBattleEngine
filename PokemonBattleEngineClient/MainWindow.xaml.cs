using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Localization;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using Kermalis.PokemonBattleEngineClient.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;

namespace Kermalis.PokemonBattleEngineClient
{
    public class MainWindow : Window, INotifyPropertyChanged
    {
        readonly List<BattleClient> battles = new List<BattleClient>();
        readonly TeamBuilderView teamBuilder;
        readonly TabControl tabs;
        readonly TextBox ip;
        readonly NumericUpDown port;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            this.FindControl<Button>("Connect").Command = ReactiveCommand.Create(Connect);
            teamBuilder = this.FindControl<TeamBuilderView>("TeamBuilder");
            ip = this.FindControl<TextBox>("IP");
            port = this.FindControl<NumericUpDown>("Port");
            tabs = this.FindControl<TabControl>("Tabs");
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
            var client = new BattleClient(ip.Text, (int)port.Value, PBEBattleFormat.Double, teamBuilder.settings, teamBuilder.team.Item2);
            battles.Add(client);
            var pages = tabs.Items.Cast<object>().ToList();
            pages.Add(new TabItem
            {
                Header = "Battle " + battles.Count,
                Content = new BattleView(client)
            });
            tabs.Items = pages;
            client.Connect();
        }
    }
}
