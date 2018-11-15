using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class MessageView : UserControl, INotifyPropertyChanged
    {
        ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        ListBox listBox;
        public MessageView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            listBox = this.FindControl<ListBox>("List");
        }

        public void Add(string value)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Messages.Add(value);
                listBox.ScrollIntoView(value);
            });
        }
    }
}
