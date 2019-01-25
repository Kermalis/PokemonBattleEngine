using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class BattleView : UserControl
    {
        public FieldView Field { get; }
        public ActionsView Actions { get; }
        readonly MessageView messages;

        public BattleClient Client { get; }

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
            Field.SetMessage(messageBox ? message : string.Empty);
            if (messageLog)
            {
                messages.AddMessage(message);
            }
        }
    }
}
