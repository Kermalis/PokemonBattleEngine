using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineMobile.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile
{
    public partial class MainPage : TabbedPage
    {
        readonly List<MobileBattleClient> battles = new List<MobileBattleClient>();

        public MainPage()
        {
            InitializeComponent();
        }

        void Connect(object sender, EventArgs e)
        {
            var client = new MobileBattleClient(IP.Text, 8888, PBEBattleFormat.Double, PBESettings.DefaultSettings);
            battles.Add(client);
            Children.Add(new NavigationPage(new ContentPage { Content = new ScrollView { Content = new BattleView(client) } }) { Title = "Battle " + battles.Count });
            client.Connect();
        }
    }
}
