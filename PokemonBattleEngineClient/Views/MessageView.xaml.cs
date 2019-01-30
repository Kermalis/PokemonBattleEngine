using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class MessageView : UserControl, INotifyPropertyChanged
    {
        ObservableCollection<Bitmap> Messages { get; } = new ObservableCollection<Bitmap>();
        readonly ListBox listBox;

        public MessageView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            listBox = this.FindControl<ListBox>("List");
        }

        public void AddMessage(string message)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Bitmap bmp = Utils.RenderString(message, Utils.StringRenderStyle.MenuBlack);
                Messages.Add(bmp);
                listBox.ScrollIntoView(bmp);
            });
        }
    }
}
