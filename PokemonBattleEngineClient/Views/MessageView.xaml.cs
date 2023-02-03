using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Kermalis.PokemonBattleEngineClient.Views;

public sealed class MessageView : UserControl
{
	public sealed class Message
	{
		public Bitmap Bitmap { get; }
		public HorizontalAlignment Alignment { get; }
		public double Scale { get; }
		public double Height { get; }

		internal Message(string message)
		{
			Bitmap = StringRenderer.Render(message, "MenuBlack");
			Match m = Regex.Match(message, @"^Turn (\d+)$");
			if (m.Success)
			{
				Alignment = HorizontalAlignment.Center;
				Scale = 2;
			}
			else
			{
				Alignment = HorizontalAlignment.Left;
				Scale = 1;
			}
			Height = Bitmap.PixelSize.Height * Scale;
		}
	}
	public ObservableCollection<Message> Messages { get; } = new();
	private readonly ListBox _listBox;

	public MessageView()
	{
		DataContext = this;
		AvaloniaXamlLoader.Load(this);

		_listBox = this.FindControl<ListBox>("List");
	}

	public void AddMessage(string message)
	{
		Dispatcher.UIThread.InvokeAsync(() =>
		{
			var m = new Message(message);
			Messages.Add(m);
			_listBox.ScrollIntoView(m);
		});
	}
}
