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
using System.ComponentModel;

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
            set
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
            set
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

        internal static void CreateBrushes()
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
                string resource = "MISC.WEATHER_" + _battleView.Client.Battle.Weather + ".gif";
                switch (_battleView.Client.Battle.Weather)
                {
                    case PBEWeather.Hailstorm:
                    {
                        dim.Fill = _hailstormDim;
                        dim.IsVisible = true;
                        GifImage.SetSourceStream(gif, Utils.GetResourceStream(resource));
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
                        GifImage.SetSourceStream(gif, Utils.GetResourceStream(resource));
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
        // pkmn.FieldPosition must be updated before calling this
        internal void UpdatePokemon(PBEPokemon pkmn, PBEFieldPosition oldPosition = PBEFieldPosition.None)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                switch (_battleView.Client.Battle.BattleFormat)
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

                HPBarView hpView;
                PokemonView pkmnView;
                bool backSprite = pkmn.Team.Id == 0 ? _battleView.Client.BattleId != 1 : _battleView.Client.BattleId == 1;
                if (oldPosition != PBEFieldPosition.None)
                {
                    hpView = this.FindControl<HPBarView>($"Bar{(backSprite ? 0 : 1)}_{oldPosition}");
                    hpView.IsVisible = false;
                    pkmnView = this.FindControl<PokemonView>($"Battler{(backSprite ? 0 : 1)}_{oldPosition}");
                    pkmnView.IsVisible = false;
                }
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    hpView = this.FindControl<HPBarView>($"Bar{(backSprite ? 0 : 1)}_{pkmn.FieldPosition}");
                    hpView.Update(pkmn, (pkmn.Team.Id == 0 && _battleView.Client.ShowEverything0) || (pkmn.Team.Id == 1 && _battleView.Client.ShowEverything1));
                    pkmnView = this.FindControl<PokemonView>($"Battler{(backSprite ? 0 : 1)}_{pkmn.FieldPosition}");
                    pkmnView.Update(pkmn, backSprite, _battleView.Client.ShowEverything0, _battleView.Client.ShowEverything1);
                }
            });
        }
    }
}
