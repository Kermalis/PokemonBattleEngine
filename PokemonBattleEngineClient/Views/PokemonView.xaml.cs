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
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        PBEPokemon pokemon;
        public PBEPokemon Pokemon
        {
            get => pokemon;
            set
            {
                pokemon = value;
                OnPropertyChanged(nameof(Pokemon));
            }
        }
        double scale;
        public double Scale
        {
            get => scale;
            set
            {
                scale = value;
                OnPropertyChanged(nameof(Scale));
            }
        }
        Point location;
        public Point Location
        {
            get => location;
            set
            {
                location = value;
                OnPropertyChanged(nameof(Location));
            }
        }
        bool showRawValues;
        public string Description => Utils.CustomPokemonToString(pokemon, showRawValues);

        public PokemonView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
            IsVisible = false;
        }

        public void Update(PBEPokemon pkmn, bool backSprite, bool showRawValues)
        {
            this.showRawValues = showRawValues;
            Pokemon = pkmn;
            if (pokemon == null || pokemon.FieldPosition == PBEFieldPosition.None)
            {
                IsVisible = false;
            }
            else
            {
                Scale = backSprite ? 2.0 : 1.0;

                Image sprite = this.FindControl<Image>("Sprite");
                // Fly/Bounce/SkyDrop / Dig / Dive / ShadowForce
                sprite.Opacity = !pokemon.Status2.HasFlag(PBEStatus2.Substitute) && (pokemon.Status2.HasFlag(PBEStatus2.Airborne) || pokemon.Status2.HasFlag(PBEStatus2.Underground) || pokemon.Status2.HasFlag(PBEStatus2.Underwater)) ? 0.4 : 1.0;

                GifImage.SetSourceUri(sprite, Utils.GetPokemonSpriteUri(pokemon, backSprite));

                IsVisible = true;
            }
        }
    }
}
