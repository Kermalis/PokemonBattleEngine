using Avalonia;
using Avalonia.Markup.Xaml;

namespace Kermalis.PokemonBattleEngineClient
{
    public sealed class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
