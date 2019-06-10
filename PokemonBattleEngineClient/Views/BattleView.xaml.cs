using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    public class BattleView : UserControl
    {
        public /*readonly*/ FieldView Field;
        public /*readonly*/ ActionsView Actions;
        private /*readonly*/ MessageView messages;

        public BattleClient Client { get; }

        public BattleView()
        {
            // This constructor only exists so xaml compiles
            AvaloniaXamlLoader.Load(this);
        }
        public BattleView(BattleClient client)
        {
            AvaloniaXamlLoader.Load(this);

            Client = client;
            Client.BattleView = this;
            Initialized += (s, e) => // Temporary fix (remove readonly comments too when fixed)
            {
                Field = this.FindControl<FieldView>("Field");
                Field.SetBattleView(this);
                Actions = this.FindControl<ActionsView>("Actions");
                Actions.BattleView = this;
                messages = this.FindControl<MessageView>("Messages");
            };
        }

        public void AddMessage(string message, bool messageBox, bool messageLog)
        {
            if (messageBox)
            {
                Field.SetMessage(message);
            }
            if (messageLog)
            {
                messages.AddMessage(message);
            }
        }
    }
}
