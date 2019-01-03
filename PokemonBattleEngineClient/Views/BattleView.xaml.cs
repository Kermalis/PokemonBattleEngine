using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class BattleView : UserControl
    {
        public FieldView Field { get; }
        public ActionsView Actions { get; }
        readonly MessageView messages;

        BattleClient client;
        public BattleClient Client
        {
            get => client;
            set
            {
                client = value;
                Field.SetBattleView(this);
            }
        }

        public BattleView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            Field = this.FindControl<FieldView>("Field");
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
