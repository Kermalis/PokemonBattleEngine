using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using AvaloniaGif;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    public sealed class FieldView : UserControl, INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public new event PropertyChangedEventHandler PropertyChanged;

        public IBitmap BGSource { get; private set; }
        private string _message;
        public string Message
        {
            get => _message;
            private set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged(nameof(Message));
                }
            }
        }
        private bool _messageBoxVisible;
        public bool MessageBoxVisible
        {
            get => _messageBoxVisible;
            private set
            {
                if (_messageBoxVisible != value)
                {
                    _messageBoxVisible = value;
                    OnPropertyChanged(nameof(MessageBoxVisible));
                }
            }
        }

        private BattleView _battleView;
        private static IBrush _hailstormDim, _harshSunlightDim, _rainDim, _sandstormDim;
        private static Dictionary<PBEWeather, Stream> _weathers;

        internal static void CreateResources()
        {
            _hailstormDim = new SolidColorBrush(Color.FromUInt32(0x20D0FFFF));
            _harshSunlightDim = new LinearGradientBrush()
            {
                StartPoint = new RelativePoint(0.0, 1.0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1.0, 0.0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop { Color = Color.FromUInt32(0x60FFF0A0), Offset = 0.0 },
                    new GradientStop { Color = Color.FromUInt32(0x20FFD080), Offset = 1.0 }
                }
            };
            _rainDim = new SolidColorBrush(Color.FromUInt32(0x40000000));
            _sandstormDim = new LinearGradientBrush()
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

            _weathers = new Dictionary<PBEWeather, Stream>
            {
                { PBEWeather.Hailstorm, Utils.GetResourceStream("MISC.WEATHER_Hailstorm.gif") },
                { PBEWeather.Rain, Utils.GetResourceStream("MISC.WEATHER_Rain.gif") }
            };
        }

        public FieldView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);
        }
        internal void SetBattleView(BattleView battleView)
        {
            _battleView = battleView;
            PBEBattle b = _battleView.Client.Battle;
            switch (b.BattleFormat)
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
                default: throw new ArgumentOutOfRangeException(nameof(_battleView.Client.Battle.BattleFormat));
            }
            BGSource = new Bitmap(Utils.GetResourceStream($"BG.BG_{b.BattleTerrain}_{b.BattleFormat}.png"));
            OnPropertyChanged(nameof(BGSource));
        }

        internal void SetMessage(string message)
        {
            Message = message;
            MessageBoxVisible = !string.IsNullOrWhiteSpace(message);
        }

        internal void UpdateWeather()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Rectangle dim = this.FindControl<Rectangle>("WeatherDim");
                Image gif = this.FindControl<Image>("WeatherGif");
                switch (_battleView.Client.Battle.Weather)
                {
                    case PBEWeather.Hailstorm:
                    {
                        dim.Fill = _hailstormDim;
                        dim.IsVisible = true;
                        GifImage.SetSourceStream(gif, _weathers[PBEWeather.Hailstorm]);
                        gif.IsVisible = true;
                        break;
                    }
                    case PBEWeather.HarshSunlight:
                    {
                        dim.Fill = _harshSunlightDim;
                        dim.IsVisible = true;
                        gif.IsVisible = false;
                        break;
                    }
                    case PBEWeather.Rain:
                    {
                        dim.Fill = _rainDim;
                        dim.IsVisible = true;
                        GifImage.SetSourceStream(gif, _weathers[PBEWeather.Rain]);
                        gif.IsVisible = true;
                        break;
                    }
                    case PBEWeather.Sandstorm:
                    {
                        dim.Fill = _sandstormDim;
                        dim.IsVisible = true;
                        gif.IsVisible = false;
                        break;
                    }
                    default:
                    {
                        dim.IsVisible = false;
                        gif.IsVisible = false;
                        break;
                    }
                }
            });
        }
        private void GetPokemonViewStuff(PBEBattlePokemon pkmn, PBEFieldPosition position, out bool backSprite, out HPBarView hpView, out PokemonView pkmnView)
        {
            backSprite = pkmn.Team.Id == 0 ? _battleView.Client.BattleId != 1 : _battleView.Client.BattleId == 1;
            hpView = this.FindControl<HPBarView>($"Bar{(backSprite ? 0 : 1)}_{position}");
            pkmnView = this.FindControl<PokemonView>($"Battler{(backSprite ? 0 : 1)}_{position}");
        }
        private void UpdatePokemon(PBEBattlePokemon pkmn, bool backSprite, HPBarView hpView, PokemonView pkmnView, bool hpBar, bool sprite)
        {
            bool se0 = _battleView.Client.ShowEverything0;
            bool se1 = _battleView.Client.ShowEverything1;
            if (hpBar)
            {
                hpView.Update(pkmn, se0, se1);
            }
            if (sprite)
            {
                pkmnView.Update(pkmn, backSprite, se0, se1);
            }
        }
        // pkmn.FieldPosition must be updated before calling these
        internal void ShowPokemon(PBEBattlePokemon pkmn)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                GetPokemonViewStuff(pkmn, pkmn.FieldPosition, out bool backSprite, out HPBarView hpView, out PokemonView pkmnView);
                UpdatePokemon(pkmn, backSprite, hpView, pkmnView, true, true);
                hpView.IsVisible = true;
                pkmnView.IsVisible = true;
            });
        }
        internal void HidePokemon(PBEBattlePokemon pkmn, PBEFieldPosition oldPosition)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                GetPokemonViewStuff(pkmn, oldPosition, out _, out HPBarView hpView, out PokemonView pkmnView);
                hpView.IsVisible = false;
                pkmnView.IsVisible = false;
            });
        }
        internal void UpdatePokemon(PBEBattlePokemon pkmn, bool hpBar, bool sprite)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                GetPokemonViewStuff(pkmn, pkmn.FieldPosition, out bool backSprite, out HPBarView hpView, out PokemonView pkmnView);
                UpdatePokemon(pkmn, backSprite, hpView, pkmnView, hpBar, sprite);
            });
        }
        internal void MovePokemon(PBEBattlePokemon pkmn, PBEFieldPosition oldPosition)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                GetPokemonViewStuff(pkmn, oldPosition, out _, out HPBarView hpView, out PokemonView pkmnView);
                hpView.IsVisible = false;
                pkmnView.IsVisible = false;
                GetPokemonViewStuff(pkmn, pkmn.FieldPosition, out bool backSprite, out hpView, out pkmnView);
                UpdatePokemon(pkmn, backSprite, hpView, pkmnView, true, true);
                hpView.IsVisible = true;
                pkmnView.IsVisible = true;
            });
        }
    }
}
