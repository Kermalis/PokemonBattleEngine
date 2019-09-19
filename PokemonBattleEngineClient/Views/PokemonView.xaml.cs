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
    public sealed class PokemonView : UserControl, INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public new event PropertyChangedEventHandler PropertyChanged;

        private PBEPokemon _pokemon;
        private Point _location;
        public Point Location
        {
            get => _location;
            internal set
            {
                if (!_location.Equals(value))
                {
                    _location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }
        private bool _showRawValues0;
        private bool _showRawValues1;
        public string Description => Utils.CustomPokemonToString(_pokemon, _showRawValues0, _showRawValues1);

        public PokemonView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);
        }

        public void Update(PBEPokemon pkmn, bool backSprite, bool showRawValues0, bool showRawValues1)
        {
            _showRawValues0 = showRawValues0;
            _showRawValues1 = showRawValues1;
            _pokemon = pkmn;

            Image sprite = this.FindControl<Image>("Sprite");
            // Fly/Bounce/SkyDrop / Dig / Dive / ShadowForce
            sprite.Opacity = !_pokemon.Status2.HasFlag(PBEStatus2.Substitute) && (_pokemon.Status2.HasFlag(PBEStatus2.Airborne) || _pokemon.Status2.HasFlag(PBEStatus2.Underground) || _pokemon.Status2.HasFlag(PBEStatus2.Underwater)) ? 0.4 : 1.0;
            GifImage.SetSourceStream(sprite, Utils.GetPokemonSpriteStream(_pokemon, backSprite));

            IsVisible = true;
        }
    }
}
