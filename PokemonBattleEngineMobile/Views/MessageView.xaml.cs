using Kermalis.PokemonBattleEngineMobile.Infrastructure;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Views
{
    public partial class MessageView : ContentView
    {
        public class Message
        {
            public ImageSource ImageSource { get; }
            public LayoutOptions Alignment { get; }
            public double Scale { get; }

            public Message(string message)
            {
                SKBitmap bmp = StringRenderer.Render(message, "MenuBlack");
                MatchCollection matches = Regex.Matches(message, @"Turn \d{1,}");
                if (matches.Count == 1 && matches[0].Value == message)
                {
                    Alignment = LayoutOptions.Center;
                    Scale = 2d;
                }
                else
                {
                    Alignment = LayoutOptions.Start;
                    Scale = 1d;
                }
                ImageSource = new SKBitmapImageSource { Bitmap = bmp };
            }
        }
        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();

        public MessageView()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public void AddMessage(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var m = new Message(message);
                Messages.Add(m);
                Listv.ScrollTo(m, ScrollToPosition.MakeVisible, true);
            });
        }
    }
}
