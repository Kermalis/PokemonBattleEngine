using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngineClient.Models;
using Kermalis.PokemonBattleEngineClient.Views;
using System;

namespace Kermalis.PokemonBattleEngineClient
{
    public sealed class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            MoveInfo.CreateBrushes();
            HPBarView.CreateResources();
            FieldView.CreateResources();
            switch (ApplicationLifetime)
            {
                case null: break;
                case IClassicDesktopStyleApplicationLifetime desktop: desktop.MainWindow = new MainWindow(); break;
                case ISingleViewApplicationLifetime singleView: singleView.MainView = new MainView(); break;
                default: throw new PlatformNotSupportedException();
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
