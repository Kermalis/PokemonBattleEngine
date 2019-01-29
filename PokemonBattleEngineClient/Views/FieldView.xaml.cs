using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class FieldView : UserControl, INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        IBitmap BGSource { get; set; }
        string Message { get; set; }
        bool MessageBoxVisible { get; set; }
        IBrush weatherDim;
        IBrush WeatherDim
        {
            get => weatherDim;
            set
            {
                weatherDim = value;
                OnPropertyChanged(nameof(WeatherDim));
            }
        }
        Uri weatherGif;
        Uri WeatherGif
        {
            get => weatherGif; set
            {
                weatherGif = value;
                OnPropertyChanged(nameof(WeatherGif));
            }
        }
        bool weatherDimVisible;
        bool WeatherDimVisible
        {
            get => weatherDimVisible;
            set
            {
                weatherDimVisible = value;
                OnPropertyChanged(nameof(WeatherDimVisible));
            }
        }
        bool weatherGifVisible;
        bool WeatherGifVisible
        {
            get => weatherGifVisible;
            set
            {
                weatherGifVisible = value;
                OnPropertyChanged(nameof(WeatherGifVisible));
            }
        }

        BattleView battleView;
        readonly IBrush hailstormDim, harshSunlightDim, rainDim, sandstormDim;

        public FieldView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            hailstormDim = new SolidColorBrush(Color.FromUInt32(0x20D0FFFF));
            harshSunlightDim = new LinearGradientBrush()
            {
                StartPoint = new RelativePoint(0.0, 1.0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1.0, 0.0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.FromUInt32(0x60FFF0A0), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0x20FFD080), Offset = 1.0 }
                }
            };
            rainDim = new SolidColorBrush(Color.FromUInt32(0x40000000));
            sandstormDim = new LinearGradientBrush()
            {
                StartPoint = new RelativePoint(0.0, 0.0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0.0, 1.0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.FromUInt32(0x40FF7F00), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0x60FFF0A0), Offset = 0.3 },
                    new GradientStop { Color = Color.FromUInt32(0x30FFC000), Offset = 1.0 }
                }
            };
        }
        public void SetBattleView(BattleView battleView)
        {
            this.battleView = battleView;
            string s;
            switch (battleView.Client.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single: s = PBEUtils.Sample(new string[] { "1_S", "2_S", "3_S", "4_S", "5_S", "6_S", "8_S" }); break;
                case PBEBattleFormat.Double: s = PBEUtils.Sample(new string[] { "1_D", "6_D", "7_D" }); break;
                case PBEBattleFormat.Triple: s = PBEUtils.Sample(new string[] { "1_T", "4_T", "5_T", "8_T" }); break;
                case PBEBattleFormat.Rotation: s = PBEUtils.Sample(new string[] { "1_R", "2_R" }); break;
                default: throw new ArgumentOutOfRangeException(nameof(battleView.Client.Battle.BattleFormat));
            }
            BGSource = Utils.UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.BG.BG_{s}.png?assembly=PokemonBattleEngineClient"));
            OnPropertyChanged(nameof(BGSource));
        }

        public void SetMessage(string message)
        {
            Message = message;
            OnPropertyChanged(nameof(Message));
            MessageBoxVisible = !string.IsNullOrWhiteSpace(message);
            OnPropertyChanged(nameof(MessageBoxVisible));
        }

        public void UpdateWeather()
        {
            Uri uri = new Uri($"resm:Kermalis.PokemonBattleEngineClient.MISC.WEATHER_{battleView.Client.Battle.Weather}.gif?assembly=PokemonBattleEngineClient");
            switch (battleView.Client.Battle.Weather)
            {
                case PBEWeather.Hailstorm:
                    WeatherDim = hailstormDim;
                    WeatherGif = uri;
                    WeatherDimVisible = WeatherGifVisible = true;
                    break;
                case PBEWeather.HarshSunlight:
                    WeatherDim = harshSunlightDim;
                    WeatherDimVisible = true;
                    WeatherGifVisible = false;
                    break;
                case PBEWeather.Rain:
                    WeatherDim = rainDim;
                    WeatherGif = uri;
                    WeatherDimVisible = WeatherGifVisible = true;
                    break;
                case PBEWeather.Sandstorm:
                    WeatherDim = sandstormDim;
                    WeatherDimVisible = true;
                    WeatherGifVisible = false;
                    break;
                default: WeatherDimVisible = WeatherGifVisible = false; break;
            }
        }
        // pkmn.FieldPosition must be updated before calling this
        public void UpdatePokemon(PBEPokemon pkmn, PBEFieldPosition oldPosition = PBEFieldPosition.None)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                switch (battleView.Client.Battle.BattleFormat)
                {
                    case PBEBattleFormat.Single:
                        {
                            this.FindControl<HPBarView>("Bar0_Center").Location = new Point(204, 35);

                            this.FindControl<HPBarView>("Bar1_Center").Location = new Point(204, 6);

                            this.FindControl<PokemonView>("Battler0_Center").Location = new Point(75, 53);

                            this.FindControl<PokemonView>("Battler1_Center").Location = new Point(284, 8);
                            break;
                        }
                    case PBEBattleFormat.Double:
                        {
                            this.FindControl<HPBarView>("Bar0_Left").Location = new Point(101, 35);
                            this.FindControl<HPBarView>("Bar0_Right").Location = new Point(307, 35);

                            this.FindControl<HPBarView>("Bar1_Right").Location = new Point(101, 6);
                            this.FindControl<HPBarView>("Bar1_Left").Location = new Point(307, 6);

                            this.FindControl<PokemonView>("Battler0_Left").Location = new Point(-37, 43);
                            this.FindControl<PokemonView>("Battler0_Right").Location = new Point(168, 54);

                            this.FindControl<PokemonView>("Battler1_Right").Location = new Point(242, 9);
                            this.FindControl<PokemonView>("Battler1_Left").Location = new Point(332, 15);
                            break;
                        }
                    case PBEBattleFormat.Triple:
                        {
                            this.FindControl<HPBarView>("Bar0_Left").Location = new Point(50, 35);
                            this.FindControl<HPBarView>("Bar0_Center").Location = new Point(204, 35);
                            this.FindControl<HPBarView>("Bar0_Right").Location = new Point(358, 35);

                            this.FindControl<HPBarView>("Bar1_Right").Location = new Point(50, 6);
                            this.FindControl<HPBarView>("Bar1_Center").Location = new Point(204, 6);
                            this.FindControl<HPBarView>("Bar1_Left").Location = new Point(358, 6);

                            this.FindControl<PokemonView>("Battler0_Left").Location = new Point(-53, 51);
                            this.FindControl<PokemonView>("Battler0_Center").Location = new Point(92, 31);
                            this.FindControl<PokemonView>("Battler0_Right").Location = new Point(221, 76);

                            this.FindControl<PokemonView>("Battler1_Right").Location = new Point(209, -1);
                            this.FindControl<PokemonView>("Battler1_Center").Location = new Point(282, 16);
                            this.FindControl<PokemonView>("Battler1_Left").Location = new Point(362, 8);
                            break;
                        }
                    case PBEBattleFormat.Rotation:
                        {
                            this.FindControl<HPBarView>("Bar0_Left").Location = new Point(50, 35);
                            this.FindControl<HPBarView>("Bar0_Center").Location = new Point(204, 35);
                            this.FindControl<HPBarView>("Bar0_Right").Location = new Point(358, 35);

                            this.FindControl<HPBarView>("Bar1_Right").Location = new Point(50, 6);
                            this.FindControl<HPBarView>("Bar1_Center").Location = new Point(204, 6);
                            this.FindControl<HPBarView>("Bar1_Left").Location = new Point(358, 6);

                            this.FindControl<PokemonView>("Battler0_Left").Location = new Point(-46, 384); // Hidden
                            this.FindControl<PokemonView>("Battler0_Center").Location = new Point(52, 72);
                            this.FindControl<PokemonView>("Battler0_Right").Location = new Point(228, 384); // Hidden

                            this.FindControl<PokemonView>("Battler1_Right").Location = new Point(211, -34);
                            this.FindControl<PokemonView>("Battler1_Center").Location = new Point(282, 16);
                            this.FindControl<PokemonView>("Battler1_Left").Location = new Point(421, -24);
                            break;
                        }
                    default: throw new ArgumentOutOfRangeException(nameof(battleView.Client.Battle.BattleFormat));
                }

                HPBarView hpView;
                PokemonView pkmnView;
                bool backSprite = (pkmn.Team.Id == 0 && battleView.Client.Index != 1) || (pkmn.Team.Id == 1 && battleView.Client.Index == 1);
                if (oldPosition != PBEFieldPosition.None)
                {
                    hpView = this.FindControl<HPBarView>($"Bar{(backSprite ? 0 : 1)}_{oldPosition}");
                    hpView.Update(pkmn);
                    pkmnView = this.FindControl<PokemonView>($"Battler{(backSprite ? 0 : 1)}_{oldPosition}");
                    pkmnView.Update(pkmn, backSprite);
                }
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    hpView = this.FindControl<HPBarView>($"Bar{(backSprite ? 0 : 1)}_{pkmn.FieldPosition}");
                    hpView.Update(pkmn);
                    pkmnView = this.FindControl<PokemonView>($"Battler{(backSprite ? 0 : 1)}_{pkmn.FieldPosition}");
                    pkmnView.Update(pkmn, backSprite);
                }
            });
        }
    }
}
