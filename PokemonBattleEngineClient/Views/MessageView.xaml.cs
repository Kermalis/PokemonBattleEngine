using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    public class MessageView : UserControl, INotifyPropertyChanged
    {
        public class Message
        {
            public Bitmap Bitmap { get; }
            public HorizontalAlignment Alignment { get; }
            public double Scale { get; }
            public double Height { get; }

            public Message(string message)
            {
                Bitmap = Utils.RenderString(message, Utils.StringRenderStyle.MenuBlack);
                MatchCollection matches = Regex.Matches(message, @"Turn \d{1,}");
                if (matches.Count == 1 && matches[0].Value == message)
                {
                    Alignment = HorizontalAlignment.Center;
                    Scale = 2.0;
                }
                else
                {
                    Alignment = HorizontalAlignment.Left;
                    Scale = 1.0;
                }
                Height = Bitmap.PixelSize.Height * Scale;
            }
        }
        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();
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
                var m = new Message(message);
                Messages.Add(m);
                listBox.ScrollIntoView(m);
            });
        }
    }
}
