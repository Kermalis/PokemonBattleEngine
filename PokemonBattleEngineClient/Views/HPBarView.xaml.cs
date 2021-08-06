using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    public sealed class HPBarView : UserControl, INotifyPropertyChanged
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

        private readonly Image _drawn;

        private static SolidColorBrush _greenSides = null!,
            _greenMid = null!,
            _yellowSides = null!,
            _yellowMid = null!,
            _redSides = null!,
            _redMid = null!;
        private static Bitmap[] _hpBars = null!;
        private static Dictionary<PBEStatus1, Bitmap> _status1s = null!;

        internal static void CreateResources()
        {
            _greenSides = new SolidColorBrush(0xFF008C29);
            _greenMid = new SolidColorBrush(0xFF00FF4A);
            _yellowSides = new SolidColorBrush(0xFF9C6310);
            _yellowMid = new SolidColorBrush(0xFFF7B500);
            _redSides = new SolidColorBrush(0xFF942131);
            _redMid = new SolidColorBrush(0xFFFF3142);

            _hpBars = new Bitmap[2] { new Bitmap(Utils.GetResourceStream("MISC.HPBAR_Ally.png")), new Bitmap(Utils.GetResourceStream("MISC.HPBAR_Foe.png")) };
            _status1s = new Dictionary<PBEStatus1, Bitmap>();
            for (PBEStatus1 s = PBEStatus1.None + 1; s < PBEStatus1.MAX; s++)
            {
                _status1s.Add(s, new Bitmap(Utils.GetResourceStream("MISC.STATUS1_" + s + ".png")));
            }
        }

        public HPBarView()
        {
            DataContext = this;
            AvaloniaXamlLoader.Load(this);

            _drawn = this.FindControl<Image>("Drawn");
            _pokemon = null!;
        }

        internal void Update(PBEBattlePokemon pkmn, bool useKnownInfo)
        {
            _useKnownInfo = useKnownInfo;
            _pokemon = pkmn;

            var wb = new WriteableBitmap(new PixelSize(104, 27), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
            using (IRenderTarget rtb = Utils.RenderInterface.CreateRenderTarget(new[] { new WriteableBitmapSurface(wb) }))
            using (IDrawingContextImpl ctx = rtb.CreateDrawingContext(null))
            {
                int barResource;
                byte yOffset;
                if (useKnownInfo)
                {
                    barResource = 1;
                    yOffset = 2;
                }
                else
                {
                    barResource = 0;
                    yOffset = 0;
                }
                Bitmap hpBar = _hpBars[barResource];
                ctx.DrawBitmap(hpBar.PlatformImpl, 1.0, new Rect(0, 0, hpBar.PixelSize.Width, hpBar.PixelSize.Height), new Rect(0, 11 + yOffset, hpBar.PixelSize.Width, hpBar.PixelSize.Height));

                Bitmap nickname = StringRenderer.Render(pkmn.KnownNickname, "BattleName");
                ctx.DrawBitmap(nickname.PlatformImpl, 1.0, new Rect(0, 0, nickname.PixelSize.Width, nickname.PixelSize.Height), new Rect(72 - Math.Max(54, nickname.PixelSize.Width), yOffset, nickname.PixelSize.Width, nickname.PixelSize.Height));

                PBEGender gender = useKnownInfo && !pkmn.KnownStatus2.HasFlag(PBEStatus2.Transformed) ? pkmn.KnownGender : pkmn.Gender;
                Bitmap level = StringRenderer.Render($"{(gender == PBEGender.Female ? "♀" : gender == PBEGender.Male ? "♂" : " ")}[LV]{pkmn.Level}", "BattleLevel");
                ctx.DrawBitmap(level.PlatformImpl, 1.0, new Rect(0, 0, level.PixelSize.Width, level.PixelSize.Height), new Rect(70, 1 + yOffset, level.PixelSize.Width, level.PixelSize.Height));

                if (pkmn.Status1 != PBEStatus1.None)
                {
                    Bitmap status = _status1s[pkmn.Status1];
                    ctx.DrawBitmap(status.PlatformImpl, 1.0, new Rect(0, 0, status.PixelSize.Width, status.PixelSize.Height), new Rect(1, 11 + yOffset, status.PixelSize.Width, status.PixelSize.Height));
                }

                IBrush hpSides, hpMid;
                if (pkmn.HPPercentage <= 0.20)
                {
                    hpSides = _redSides;
                    hpMid = _redMid;
                }
                else if (pkmn.HPPercentage <= 0.50)
                {
                    hpSides = _yellowSides;
                    hpMid = _yellowMid;
                }
                else
                {
                    hpSides = _greenSides;
                    hpMid = _greenMid;
                }
                const byte lineW = 48;
                int theW = (int)(lineW * pkmn.HPPercentage);
                if (theW == 0 && pkmn.HPPercentage > 0)
                {
                    theW = 1;
                }
                ctx.DrawRectangle(hpSides, null, new Rect(38, 13 + yOffset, theW, 1));
                ctx.DrawRectangle(hpMid, null, new Rect(38, 13 + yOffset + 1, theW, 1));
                ctx.DrawRectangle(hpSides, null, new Rect(38, 13 + yOffset + 2, theW, 1));

                if (!useKnownInfo)
                {
                    Bitmap hp = StringRenderer.Render(pkmn.HP.ToString(), "BattleHP");
                    ctx.DrawBitmap(hp.PlatformImpl, 1.0, new Rect(0, 0, hp.PixelSize.Width, hp.PixelSize.Height), new Rect(62 - hp.PixelSize.Width, 16 + yOffset, hp.PixelSize.Width, hp.PixelSize.Height));
                    Bitmap maxHP = StringRenderer.Render(pkmn.MaxHP.ToString(), "BattleHP");
                    ctx.DrawBitmap(maxHP.PlatformImpl, 1.0, new Rect(0, 0, maxHP.PixelSize.Width, maxHP.PixelSize.Height), new Rect(70, 16 + yOffset, maxHP.PixelSize.Width, maxHP.PixelSize.Height));
                }
            }
            _drawn.Source = wb;
        }
    }
}
