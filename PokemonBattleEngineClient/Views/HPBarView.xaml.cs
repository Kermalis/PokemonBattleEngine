using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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

        readonly SolidColorBrush greenSides, greenMid, yellowSides, yellowMid, redSides, redMid;

        public HPBarView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;

            greenSides = new SolidColorBrush(0xFF008C29);
            greenMid = new SolidColorBrush(0xFF00FF4A);
            yellowSides = new SolidColorBrush(0xFF9C6310);
            yellowMid = new SolidColorBrush(0xFFF7B500);
            redSides = new SolidColorBrush(0xFF942131);
            redMid = new SolidColorBrush(0xFFFF3142);
        }

        public void Update(PBEPokemon pkmn)
        {
            if (pkmn == null || pkmn.FieldPosition == PBEFieldPosition.None)
            {
                IsVisible = false;
            }
            else
            {
                var wb = new WriteableBitmap(new PixelSize(104, 27), new Vector(96, 96), PixelFormat.Bgra8888);
                using (IRenderTarget rtb = AvaloniaLocator.Current.GetService<IPlatformRenderInterface>().CreateRenderTarget(new[] { new WriteableBitmapSurface(wb) }))
                using (IDrawingContextImpl ctx = rtb.CreateDrawingContext(null))
                {
                    string bar;
                    byte yOffset;
                    if (pkmn.Id == byte.MaxValue)
                    {
                        bar = "resm:Kermalis.PokemonBattleEngineClient.MISC.HPBAR_Foe.png?assembly=PokemonBattleEngineClient";
                        yOffset = 2;
                    }
                    else
                    {
                        bar = "resm:Kermalis.PokemonBattleEngineClient.MISC.HPBAR_Ally.png?assembly=PokemonBattleEngineClient";
                        yOffset = 0;
                    }
                    Bitmap hpBar = Utils.UriToBitmap(new Uri(bar));
                    ctx.DrawImage(hpBar.PlatformImpl, 1.0, new Rect(0, 0, hpBar.PixelSize.Width, hpBar.PixelSize.Height), new Rect(0, 11 + yOffset, hpBar.PixelSize.Width, hpBar.PixelSize.Height));

                    Bitmap nickname = Utils.RenderString(pkmn.VisualNickname, Utils.StringRenderStyle.BattleName);
                    ctx.DrawImage(nickname.PlatformImpl, 1.0, new Rect(0, 0, nickname.PixelSize.Width, nickname.PixelSize.Height), new Rect(72 - Math.Max(54, nickname.PixelSize.Width), yOffset, nickname.PixelSize.Width, nickname.PixelSize.Height));

                    PBEPokemon disguisedAs = pkmn.DisguisedAsPokemon ?? pkmn; // Don't use visual gender because of transform
                    Bitmap level = Utils.RenderString($"{(disguisedAs.Shell.Gender == PBEGender.Female ? "♀" : disguisedAs.Shell.Gender == PBEGender.Male ? "♂" : " ")}[LV]{pkmn.Shell.Level}", Utils.StringRenderStyle.BattleLevel);
                    ctx.DrawImage(level.PlatformImpl, 1.0, new Rect(0, 0, level.PixelSize.Width, level.PixelSize.Height), new Rect(70, 1 + yOffset, level.PixelSize.Width, level.PixelSize.Height));

                    if (pkmn.Status1 != PBEStatus1.None)
                    {
                        Bitmap status = Utils.UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.MISC.STATUS1_{pkmn.Status1}.png?assembly=PokemonBattleEngineClient"));
                        ctx.DrawImage(status.PlatformImpl, 1.0, new Rect(0, 0, status.PixelSize.Width, status.PixelSize.Height), new Rect(1, 11 + yOffset, status.PixelSize.Width, status.PixelSize.Height));
                    }

                    IBrush hpSides, hpMid;
                    if (pkmn.HPPercentage <= 0.20)
                    {
                        hpSides = redSides;
                        hpMid = redMid;
                    }
                    else if (pkmn.HPPercentage <= 0.50)
                    {
                        hpSides = yellowSides;
                        hpMid = yellowMid;
                    }
                    else
                    {
                        hpSides = greenSides;
                        hpMid = greenMid;
                    }
                    const byte lineW = 48;
                    int theW = (int)(lineW * pkmn.HPPercentage);
                    if (theW == 0 && pkmn.HPPercentage > 0)
                    {
                        theW = 1;
                    }
                    ctx.FillRectangle(hpSides, new Rect(38, 13 + yOffset, theW, 1));
                    ctx.FillRectangle(hpMid, new Rect(38, 13 + yOffset + 1, theW, 1));
                    ctx.FillRectangle(hpSides, new Rect(38, 13 + yOffset + 2, theW, 1));

                    if (pkmn.Id != byte.MaxValue)
                    {
                        Bitmap hp = Utils.RenderString(pkmn.HP.ToString(), Utils.StringRenderStyle.BattleHP);
                        ctx.DrawImage(hp.PlatformImpl, 1.0, new Rect(0, 0, hp.PixelSize.Width, hp.PixelSize.Height), new Rect(62 - hp.PixelSize.Width, 16 + yOffset, hp.PixelSize.Width, hp.PixelSize.Height));
                        Bitmap maxHP = Utils.RenderString(pkmn.MaxHP.ToString(), Utils.StringRenderStyle.BattleHP);
                        ctx.DrawImage(maxHP.PlatformImpl, 1.0, new Rect(0, 0, maxHP.PixelSize.Width, maxHP.PixelSize.Height), new Rect(70, 16 + yOffset, maxHP.PixelSize.Width, maxHP.PixelSize.Height));
                    }
                }
                this.FindControl<Image>("Drawn").Source = wb;

                IsVisible = true;
            }
        }
    }
}
