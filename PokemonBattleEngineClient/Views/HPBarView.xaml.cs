using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class HPBarView : UserControl, INotifyPropertyChanged
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
        string level;
        string Level
        {
            get => level;
            set
            {
                level = value;
                OnPropertyChanged(nameof(Level));
            }
        }
        IBitmap status;
        IBitmap Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        IBrush hpColor;
        IBrush HPColor
        {
            get => hpColor;
            set
            {
                hpColor = value;
                OnPropertyChanged(nameof(HPColor));
            }
        }
        Point hpEndLocation;
        Point HPEndLocation
        {
            get => hpEndLocation;
            set
            {
                hpEndLocation = value;
                OnPropertyChanged(nameof(HPEndLocation));
            }
        }

        readonly SolidColorBrush green, yellow, red;
        public HPBarView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            green = new SolidColorBrush(Color.FromRgb(0, 255, 41));
            yellow = new SolidColorBrush(Color.FromRgb(247, 181, 0));
            red = new SolidColorBrush(Color.FromRgb(255, 49, 66));
        }

        public void Update()
        {
            if (pokemon == null || pokemon.FieldPosition == PBEFieldPosition.None)
            {
                Visible = false;
            }
            else
            {
                Level = $"{pokemon.GenderSymbol} Lv.{pokemon.Shell.Level}";
                Status = pokemon.Status1 == PBEStatus1.None ? null : Utils.UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.Assets.Misc.{pokemon.Status1}.png?assembly=PokemonBattleEngineClient"));

                const byte lineX = 50, lineY = 18, lineW = 47;
                double hpLeft = (double)pokemon.HP / pokemon.MaxHP;
                if (hpLeft <= 0.20)
                {
                    HPColor = red;
                }
                else if (hpLeft <= 0.50)
                {
                    HPColor = yellow;
                }
                else
                {
                    HPColor = green;
                }
                HPEndLocation = new Point(hpLeft * lineW + lineX, lineY);

                Visible = true;
            }
        }
    }
}
