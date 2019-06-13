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
            // Temporary fix for https://github.com/AvaloniaUI/Avalonia/issues/2562 (remove readonly comments too when fixed) (stopped working when switching to PR build)
            Initialized += (s, e) =>
            {
                Field = this.FindControl<FieldView>("Field"); // Field will be null
                Field.SetBattleView(this);
                Actions = this.FindControl<ActionsView>("Actions"); // Actions will be null
                Actions.BattleView = this;
                messages = this.FindControl<MessageView>("Messages"); // Messages will be null
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
