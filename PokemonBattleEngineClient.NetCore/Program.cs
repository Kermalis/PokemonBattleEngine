using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System;

namespace Kermalis.PokemonBattleEngineClient.NetCore
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Utils.SetWorkingDirectory(string.Empty);
            BuildAvaloniaApp().Start<MainWindow>();
        }
        /// <summary>
        /// This method is needed for IDE previewer infrastructure
        /// </summary>
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                           .UsePlatformDetect()
                           .UseReactiveUI()
                           .LogToDebug();
        }
    }
}
