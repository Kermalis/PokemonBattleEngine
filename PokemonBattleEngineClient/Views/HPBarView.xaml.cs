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
        ushort hp;
        ushort HP
        {
            get => hp;
            set
            {
                hp = value;
                OnPropertyChanged(nameof(HP));
            }
        }
        ushort maxHP;
        ushort MaxHP
        {
            get => maxHP;
            set
            {
                maxHP = value;
                OnPropertyChanged(nameof(MaxHP));
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

        public void Update(PBEPokemon pkmn)
        {
            if (pkmn == null || pkmn.FieldPosition == PBEFieldPosition.None)
            {
                Visible = false;
            }
            else
            {
                Nickname = pkmn.VisualNickname;
                PBEPokemon disguisedAs = pkmn.DisguisedAsPokemon ?? pkmn; // Don't use visual gender because of transform
                Level = $"{(disguisedAs.Shell.Gender == PBEGender.Female ? "♀" : disguisedAs.Shell.Gender == PBEGender.Male ? "♂" : " ")}[LV]{pkmn.Shell.Level}";
                HP = pkmn.HP;
                MaxHP = pkmn.MaxHP;
                Status = pkmn.Status1 == PBEStatus1.None ? null : Utils.UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.MISC.STATUS1_{pkmn.Status1}.png?assembly=PokemonBattleEngineClient"));

                double hpLeft = (double)pkmn.HP / pkmn.MaxHP;
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
                const byte lineX = 49, lineY = 14, lineW = 49;
                HPEndLocation = new Point(hpLeft * lineW + lineX, lineY);

                Visible = true;
            }
        }
    }
}
