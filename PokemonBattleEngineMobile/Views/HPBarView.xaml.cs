using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineMobile.Infrastructure;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Views
{
    public partial class HPBarView : ContentView
    {
        private PBEStatus1 status1;
        private double hpPercentage;
        private readonly Color green, yellow, red;

        public HPBarView()
        {
            InitializeComponent();
            Bar.Source = ImageSource.FromResource("Kermalis.PokemonBattleEngineMobile.MISC.HPBAR_Ally.png");

            green = Color.FromRgb(0, 255, 41);
            yellow = Color.FromRgb(247, 181, 0);
            red = Color.FromRgb(255, 49, 66);
        }

        public void Update(PBEPokemon pkmn)
        {
            if (pkmn.Status1 != status1)
            {
                Status1.Source = (status1 = pkmn.Status1) == PBEStatus1.None ? null : ImageSource.FromResource($"Kermalis.PokemonBattleEngineMobile.MISC.STATUS1_{pkmn.Status1}.png");
            }

            if (pkmn.HPPercentage != hpPercentage)
            {
                hpPercentage = pkmn.HPPercentage;
                if (hpPercentage <= 0.20)
                {
                    HP.Color = red;
                }
                else if (hpPercentage <= 0.50)
                {
                    HP.Color = yellow;
                }
                else
                {
                    HP.Color = green;
                }
                const byte lineW = 48, lineH = 3;
                Canvas.SetSize(HP, new Size(hpPercentage * lineW, lineH));
            }

            IsVisible = true;
        }
    }
}
