using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngineClient.Views;

namespace Kermalis.PokemonBattleEngineClient;

public sealed class MainWindow : Window
{
	public MainWindow()
	{
		// TODO: iOS does not support dynamically loading assemblies
		// so we must refer to this resource DLL statically. For
		// now I am doing that here. But we need a better solution!!
		var theme = new Avalonia.Themes.Default.DefaultTheme();
		theme.TryGetResource("Button", out _);
		AvaloniaXamlLoader.Load(this);
	}

	protected override bool HandleClosing()
	{
		this.FindControl<MainView>("Main").HandleClosing();
		return base.HandleClosing();
	}
}
