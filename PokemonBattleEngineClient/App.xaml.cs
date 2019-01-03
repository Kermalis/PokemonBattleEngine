using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;

namespace Kermalis.PokemonBattleEngineClient
{
    public class App : Application
    {
        static void Main(string[] args)
            => BuildAvaloniaApp().Start<MainWindow>();
        public static AppBuilder BuildAvaloniaApp()
           => AppBuilder.Configure<App>()
               .UsePlatformDetect()
               .UseReactiveUI()
               .LogToDebug();
        public override void Initialize() =>
            AvaloniaXamlLoader.Load(this);
    }
}
