using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineMobile.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile
{
    public partial class MainPage : TabbedPage
    {
        private readonly List<MobileBattleClient> battles = new List<MobileBattleClient>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void Connect(object sender, EventArgs e)
        {
            var client = new MobileBattleClient(IP.Text, 8888, PBEBattleFormat.Double, PBESettings.DefaultSettings);
            battles.Add(client);
            var page = new NavigationPage(new ContentPage { Content = new ScrollView { Content = new BattleView(client) } }) { Title = "Battle " + battles.Count };
            Children.Add(page);
            SelectedItem = page;
            client.Connect();
        }
    }
}
