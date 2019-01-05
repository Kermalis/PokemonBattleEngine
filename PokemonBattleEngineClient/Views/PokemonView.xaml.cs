using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class PokemonView : UserControl, INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        PBEPokemon pokemon;
        PBEPokemon Pokemon
        {
            get => pokemon;
            set
            {
                pokemon = value;
                OnPropertyChanged(nameof(Pokemon));
            }
        }
        double scale;
        double Scale
        {
            get => scale;
            set
            {
                scale = value;
                OnPropertyChanged(nameof(Scale));
            }
        }
        double spriteOpacity;
        double SpriteOpacity
        {
            get => spriteOpacity;
            set
            {
                spriteOpacity = value;
                OnPropertyChanged(nameof(SpriteOpacity));
            }
        }
        bool visible;
        bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                OnPropertyChanged(nameof(Visible));
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
        Uri source;
        Uri Source
        {
            get => source;
            set
            {
                source = value;
                OnPropertyChanged(nameof(Source));
            }
        }

        public PokemonView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
        }

        public void Update(PBEPokemon pkmn, bool backSprite)
        {
            Pokemon = pkmn;
            if (pokemon == null || pokemon.FieldPosition == PBEFieldPosition.None)
            {
                Visible = false;
            }
            else
            {
                Scale = backSprite ? 2.0 : 1.0;

                // Fly/Bounce/SkyDrop / Dig / Dive / ShadowForce
                SpriteOpacity = !pokemon.Status2.HasFlag(PBEStatus2.Substitute) && (pokemon.Status2.HasFlag(PBEStatus2.Airborne) || pokemon.Status2.HasFlag(PBEStatus2.Underground) || pokemon.Status2.HasFlag(PBEStatus2.Underwater)) ? 0.4 : 1.0;

                Source = Utils.GetPokemonSpriteUri(pokemon, backSprite);

                Visible = true;
            }
        }
    }
}
