using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaGif;
using Kermalis.PokemonBattleEngine.Battle;
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
        public new event PropertyChangedEventHandler? PropertyChanged;

        private PBEBattlePokemon _pokemon;
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
        private bool _useKnownInfo;
        public string Description => Utils.CustomPokemonToString(_pokemon, _useKnownInfo);

        private readonly GifImage _sprite;

        public PokemonView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);

            _sprite = this.FindControl<GifImage>("Sprite");
            _pokemon = null!;
        }

        public void Update(PBEBattlePokemon pkmn, bool backSprite, bool useKnownInfo)
        {
            _useKnownInfo = useKnownInfo;
            _pokemon = pkmn;

            PBEStatus2 status2 = useKnownInfo ? _pokemon.KnownStatus2 : _pokemon.Status2;
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
            _sprite.Opacity = opacity;
            _sprite.SourceUri = Utils.GetPokemonSpriteUri(_pokemon, backSprite);
        }
    }
}
