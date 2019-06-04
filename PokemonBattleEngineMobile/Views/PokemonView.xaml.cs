using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineMobile.Infrastructure;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Views
{
    public partial class PokemonView : ContentView
    {
        public PokemonView()
        {
            InitializeComponent();
        }

        public void Update(PBEPokemon pkmn, bool backSprite)
        {
            // Fly/Bounce/SkyDrop / Dig / Dive / ShadowForce
            Sprite.Opacity = !pkmn.Status2.HasFlag(PBEStatus2.Substitute) && (pkmn.Status2.HasFlag(PBEStatus2.Airborne) || pkmn.Status2.HasFlag(PBEStatus2.Underground) || pkmn.Status2.HasFlag(PBEStatus2.Underwater)) ? 0.4 : 1.0;
            Sprite.SetGifResource(Utils.GetPokemonSpriteResource(pkmn, backSprite));

            IsVisible = true;
        }
    }
}
