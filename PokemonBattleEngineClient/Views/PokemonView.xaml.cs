using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaGif;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    public class PokemonView : UserControl, INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public new event PropertyChangedEventHandler PropertyChanged;

        private PBEPokemon pokemon;
        private double scale;
        public double Scale
        {
            get => scale;
            set
            {
                scale = value;
                OnPropertyChanged(nameof(Scale));
            }
        }
        private Point location;
        public Point Location
        {
            get => location;
            set
            {
                location = value;
                OnPropertyChanged(nameof(Location));
            }
        }
        private bool showRawValues0, showRawValues1;
        public string Description => Utils.CustomPokemonToString(pokemon, showRawValues0, showRawValues1);

        public PokemonView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);
        }

        public void Update(PBEPokemon pkmn, bool backSprite, bool showRawValues0, bool showRawValues1)
        {
            this.showRawValues0 = showRawValues0;
            this.showRawValues1 = showRawValues1;
            pokemon = pkmn;

            Image sprite = this.FindControl<Image>("Sprite");
            // Fly/Bounce/SkyDrop / Dig / Dive / ShadowForce
            sprite.Opacity = !pokemon.Status2.HasFlag(PBEStatus2.Substitute) && (pokemon.Status2.HasFlag(PBEStatus2.Airborne) || pokemon.Status2.HasFlag(PBEStatus2.Underground) || pokemon.Status2.HasFlag(PBEStatus2.Underwater)) ? 0.4 : 1.0;
            GifImage.SetSourceStream(sprite, Utils.GetPokemonSpriteStream(pokemon, backSprite));

            IsVisible = true;
        }
    }
}
