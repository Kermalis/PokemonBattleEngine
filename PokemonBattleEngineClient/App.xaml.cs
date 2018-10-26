using Avalonia;
using Avalonia.Markup.Xaml;

namespace Kermalis.PokemonBattleEngineClient
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
