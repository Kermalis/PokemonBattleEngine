using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class MessageView : UserControl, INotifyPropertyChanged
    {
        ObservableCollection<Bitmap> Messages { get; } = new ObservableCollection<Bitmap>();

        ListBox listBox;
        public MessageView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            listBox = this.FindControl<ListBox>("List");
        }

        public void Add(string value)
        {
            Bitmap bmp = Utils.RenderString(value);
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Messages.Add(bmp);
                listBox.ScrollIntoView(bmp);
            });
        }
    }
}
