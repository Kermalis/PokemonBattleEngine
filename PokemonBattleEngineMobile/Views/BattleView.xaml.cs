using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Views
{
    public partial class BattleView : ContentView
    {
        public MobileBattleClient Client { get; }

        public BattleView(MobileBattleClient client)
        {
            InitializeComponent();
            Client = client;
            Client.BattleView = this;
            Field.SetBattleView(this);
            Actions.BattleView = this;
        }

        public void AddMessage(string message, bool messageBox, bool messageLog)
        {
            if (messageBox)
            {
                Field.SetMessage(message);
            }
            if (messageLog)
            {
                Messages.AddMessage(message);
            }
        }
    }
}
