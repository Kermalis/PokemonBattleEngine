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
        private bool _showEverything0;
        private bool _showEverything1;
        public string Description => Utils.CustomPokemonToString(_pokemon, _showEverything0, _showEverything1);

        public PokemonView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);
        }

        public void Update(PBEPokemon pkmn, bool backSprite, bool showEverything0, bool showEverything1)
        {
            _showEverything0 = showEverything0;
            _showEverything1 = showEverything1;
            _pokemon = pkmn;

            Image sprite = this.FindControl<Image>("Sprite");
            // Bounce/Fly/SkyDrop / Dig / Dive / ShadowForce
            PBEStatus2 status2 = Utils.ShouldShowEverything(pkmn.Team, showEverything0, showEverything1) ? _pokemon.Status2 : _pokemon.KnownStatus2;
            double opacity = 1;
            if (!status2.HasFlag(PBEStatus2.Substitute))
            {
                if (status2.HasFlag(PBEStatus2.Disguised))
                {
                    opacity *= 0.7;
                }
                if (status2.HasFlag(PBEStatus2.Airborne) || status2.HasFlag(PBEStatus2.ShadowForce) || status2.HasFlag(PBEStatus2.Underground) || status2.HasFlag(PBEStatus2.Underwater))
                {
                    opacity *= 0.4;
                }
            }
            sprite.Opacity = opacity;
            GifImage.SetSourceStream(sprite, Utils.GetPokemonSpriteStream(_pokemon, backSprite));

            IsVisible = true;
        }
    }
}
