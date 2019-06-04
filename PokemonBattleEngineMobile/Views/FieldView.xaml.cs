using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineMobile.Infrastructure;
using System;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Views
{
    public partial class FieldView : ContentView
    {
        public bool MessageBoxVisible { get; set; }

        private BattleView battleView;
        //private readonly IBrush hailstormDim, harshSunlightDim, rainDim, sandstormDim;

        public FieldView()
        {
            InitializeComponent();
            BindingContext = this;

            /*hailstormDim = new SolidColorBrush(Color.FromUInt32(0x20D0FFFF));
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
            };*/
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
            BG.Source = ImageSource.FromResource($"Kermalis.PokemonBattleEngineMobile.BG.BG_{s}.png");
        }

        public void SetMessage(string message)
        {
            // TODO: Remove this when the converter is there
            Device.BeginInvokeOnMainThread(() =>
            {
                Message.Text = message;
                MessageBoxVisible = !string.IsNullOrWhiteSpace(message);
                OnPropertyChanged(nameof(MessageBoxVisible));
            });
        }

        public void UpdateWeather()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                string resource = $"Kermalis.PokemonBattleEngineMobile.MISC.WEATHER_{battleView.Client.Battle.Weather}.gif";
                switch (battleView.Client.Battle.Weather)
                {
                    case PBEWeather.Hailstorm:
                        //WeatherDim = hailstormDim;
                        WeatherGif.SetGifResource(resource);
                        /*WeatherDimVisible = */
                        WeatherGif.IsVisible = true;
                        break;
                    case PBEWeather.HarshSunlight:
                        //WeatherDim = harshSunlightDim;
                        //WeatherDimVisible = true;
                        WeatherGif.IsVisible = false;
                        break;
                    case PBEWeather.Rain:
                        //WeatherDim = rainDim;
                        WeatherGif.SetGifResource(resource);
                        /*WeatherDimVisible = */
                        WeatherGif.IsVisible = true;
                        break;
                    case PBEWeather.Sandstorm:
                        //WeatherDim = sandstormDim;
                        //WeatherDimVisible = true;
                        WeatherGif.IsVisible = false;
                        break;
                    default: /*WeatherDimVisible = */WeatherGif.IsVisible = false; break;
                }
            });
        }
        // pkmn.FieldPosition must be updated before calling this
        public void UpdatePokemon(PBEPokemon pkmn, PBEFieldPosition oldPosition = PBEFieldPosition.None)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                switch (battleView.Client.Battle.BattleFormat)
                {
                    case PBEBattleFormat.Single:
                    {
                        Canvas.SetLocation(Bar0_Center, new Point(204, 35));

                        Canvas.SetLocation(Bar1_Center, new Point(204, 6));

                        Canvas.SetLocation(Battler0_Center, new Point(75, 53));

                        Canvas.SetLocation(Battler1_Center, new Point(284, 8));
                        break;
                    }
                    case PBEBattleFormat.Double:
                    {
                        Canvas.SetLocation(Bar0_Left, new Point(101, 35));
                        Canvas.SetLocation(Bar0_Right, new Point(307, 35));

                        Canvas.SetLocation(Bar1_Right, new Point(101, 6));
                        Canvas.SetLocation(Bar1_Left, new Point(307, 6));

                        Canvas.SetLocation(Battler0_Left, new Point(-37, 43));
                        Canvas.SetLocation(Battler0_Right, new Point(168, 54));

                        Canvas.SetLocation(Battler1_Right, new Point(242, 9));
                        Canvas.SetLocation(Battler1_Left, new Point(332, 15));
                        break;
                    }
                    case PBEBattleFormat.Triple:
                    {
                        Canvas.SetLocation(Bar0_Left, new Point(50, 35));
                        Canvas.SetLocation(Bar0_Center, new Point(204, 35));
                        Canvas.SetLocation(Bar0_Right, new Point(358, 35));

                        Canvas.SetLocation(Bar1_Right, new Point(50, 6));
                        Canvas.SetLocation(Bar1_Center, new Point(204, 6));
                        Canvas.SetLocation(Bar1_Left, new Point(358, 6));

                        Canvas.SetLocation(Battler0_Left, new Point(-53, 51));
                        Canvas.SetLocation(Battler0_Center, new Point(92, 31));
                        Canvas.SetLocation(Battler0_Right, new Point(221, 76));

                        Canvas.SetLocation(Battler1_Right, new Point(209, -1));
                        Canvas.SetLocation(Battler1_Center, new Point(282, 16));
                        Canvas.SetLocation(Battler1_Left, new Point(362, 8));
                        break;
                    }
                    case PBEBattleFormat.Rotation:
                    {
                        Canvas.SetLocation(Bar0_Left, new Point(50, 35));
                        Canvas.SetLocation(Bar0_Center, new Point(204, 35));
                        Canvas.SetLocation(Bar0_Right, new Point(358, 35));

                        Canvas.SetLocation(Bar1_Right, new Point(50, 6));
                        Canvas.SetLocation(Bar1_Center, new Point(204, 6));
                        Canvas.SetLocation(Bar1_Left, new Point(358, 6));

                        Canvas.SetLocation(Battler0_Left, new Point(-46, 384)); // Hidden
                        Canvas.SetLocation(Battler0_Center, new Point(52, 72));
                        Canvas.SetLocation(Battler0_Right, new Point(228, 384)); // Hidden

                        Canvas.SetLocation(Battler1_Right, new Point(211, -34));
                        Canvas.SetLocation(Battler1_Center, new Point(282, 16));
                        Canvas.SetLocation(Battler1_Left, new Point(421, -24));
                        break;
                    }
                    default: throw new ArgumentOutOfRangeException(nameof(battleView.Client.Battle.BattleFormat));
                }

                HPBarView hpView;
                PokemonView pkmnView;
                bool backSprite = (pkmn.Team.Id == 0 && battleView.Client.BattleId != 1) || (pkmn.Team.Id == 1 && battleView.Client.BattleId == 1);
                if (oldPosition != PBEFieldPosition.None)
                {
                    hpView = this.FindByName<HPBarView>($"Bar{(backSprite ? 0 : 1)}_{oldPosition}");
                    hpView.IsVisible = false;
                    pkmnView = this.FindByName<PokemonView>($"Battler{(backSprite ? 0 : 1)}_{oldPosition}");
                    pkmnView.IsVisible = false;
                }
                if (pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    hpView = this.FindByName<HPBarView>($"Bar{(backSprite ? 0 : 1)}_{pkmn.FieldPosition}");
                    hpView.Update(pkmn);
                    pkmnView = this.FindByName<PokemonView>($"Battler{(backSprite ? 0 : 1)}_{pkmn.FieldPosition}");
                    pkmnView.Update(pkmn, backSprite);
                }
            });
        }
    }
}
