using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;

namespace Kermalis.PokemonBattleEngineClient
{
    public class App : Application
    {
        public static void Main()
        {
            BuildAvaloniaApp().Start<MainWindow>();
        }
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                          .UsePlatformDetect()
                          .UseReactiveUI()
                          .LogToDebug();
        }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
