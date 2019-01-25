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
            IsVisible = false;
        }

        public void Update(PBEPokemon pkmn, bool backSprite)
        {
            if (pkmn == null || pkmn.FieldPosition == PBEFieldPosition.None)
            {
                IsVisible = false;
            }
            else
            {
                Scale = backSprite ? 2.0 : 1.0;

                // Fly/Bounce/SkyDrop / Dig / Dive / ShadowForce
                Opacity = !pkmn.Status2.HasFlag(PBEStatus2.Substitute) && (pkmn.Status2.HasFlag(PBEStatus2.Airborne) || pkmn.Status2.HasFlag(PBEStatus2.Underground) || pkmn.Status2.HasFlag(PBEStatus2.Underwater)) ? 0.4 : 1.0;

                Sprite.Source = Utils.GetPokemonSprite(pkmn, backSprite, out short width, out short height);
                Sprite.WidthRequest = width;
                Sprite.HeightRequest = height;

                IsVisible = true;
            }
        }
    }
}
