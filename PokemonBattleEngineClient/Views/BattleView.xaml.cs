using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    public class BattleView : UserControl
    {
        public readonly FieldView Field;
        public readonly ActionsView Actions;
        private readonly MessageView messages;

        public readonly BattleClient Client;

        public BattleView()
        {
            // This constructor only exists so xaml compiles
            AvaloniaXamlLoader.Load(this);
        }
        public BattleView(BattleClient client)
        {
            AvaloniaXamlLoader.Load(this);

            Client = client;
            //Field = this.FindControl<FieldView>("Field"); // Field will be null
            Field = TemporaryFix<FieldView>("Field");
            Field.SetBattleView(this);
            //Actions = this.FindControl<ActionsView>("Actions"); // Actions will be null
            Actions = TemporaryFix<ActionsView>("Actions");
            Actions.BattleView = this;
            //messages = this.FindControl<MessageView>("Messages"); // Messages will be null
            messages = TemporaryFix<MessageView>("Messages");
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

        // Temporary fix for https://github.com/AvaloniaUI/Avalonia/issues/2562
        private T TemporaryFix<T>(string name) where T : UserControl
        {
            T Recursion(System.Collections.Generic.IEnumerable<Avalonia.LogicalTree.ILogical> list)
            {
                foreach (Avalonia.LogicalTree.ILogical i in list)
                {
                    if (i is Avalonia.INamed named && named is T ret && ret.Name == name)
                    {
                        return ret;
                    }
                    else
                    {
                        T r = Recursion(i.LogicalChildren);
                        if (r != null)
                        {
                            return r;
                        }
                    }
                }
                return null;
            }
            return Recursion(LogicalChildren);
        }
    }
}
