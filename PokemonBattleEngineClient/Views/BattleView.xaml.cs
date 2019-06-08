using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    public class BattleView : UserControl
    {
        public FieldView Field { get; }
        public ActionsView Actions { get; }
        private readonly MessageView messages;

        public BattleClient Client { get; }

        public BattleView()
        {
            AvaloniaXamlLoader.Load(this); // Only exists so xaml compiles
        }
        public BattleView(BattleClient client)
        {
            AvaloniaXamlLoader.Load(this);
            Client = client;
            Client.BattleView = this;
            Field = this.FindControl<FieldView>("Field");
            Field.SetBattleView(this);
            Actions = this.FindControl<ActionsView>("Actions");
            Actions.BattleView = this;
            messages = this.FindControl<MessageView>("Messages");
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
