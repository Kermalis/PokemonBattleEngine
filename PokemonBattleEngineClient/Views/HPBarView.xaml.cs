using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Kermalis.PokemonBattleEngine.Data;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class HPBarView : UserControl, INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        PPokemon pokemon;
        public PPokemon Pokemon
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
        string nickname;
        string Nickname
        {
            get => nickname;
            set
            {
                nickname = value;
                OnPropertyChanged(nameof(Nickname));
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
        string status;
        string Status
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
            if (pokemon == null || pokemon.FieldPosition == PFieldPosition.None)
            {
                Visible = false;
            }
            else
            {
                Nickname = pokemon.Shell.Nickname;
                Level = $"{pokemon.GenderSymbol} Lv.{pokemon.Shell.Level}";
                switch (pokemon.Status1)
                {
                    case PStatus1.Asleep: Status = "SLP"; break;
                    case PStatus1.BadlyPoisoned:
                    case PStatus1.Poisoned: Status = "PSN"; break;
                    case PStatus1.Burned: Status = "BRN"; break;
                    case PStatus1.Frozen: Status = "FRZ"; break;
                    case PStatus1.Paralyzed: Status = "PAR"; break;
                    default: Status = string.Empty; break;
                }

                const byte lineX = 49, lineY = 17, lineW = 47;
                double hpLeft = (double)pokemon.HP / pokemon.MaxHP;
                if (hpLeft <= 0.20)
                    HPColor = red;
                else if (hpLeft <= 0.50)
                    HPColor = yellow;
                else
                    HPColor = green;
                HPEndLocation = new Point(hpLeft * lineW + lineX, lineY);

                Visible = true;
            }
        }
    }
}
