using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngineClient.Views;

namespace Kermalis.PokemonBattleEngineClient
{
    class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);

            var client = new BattleClient("127.0.0.1", 8888, this.FindControl<BattleView>("Battle1"));
            client.Connect();
        }

        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>();
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
