using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class BattleView : UserControl, INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        IBitmap bgSource;
        IBitmap BGSource
        {
            get => bgSource;
            set
            {
                bgSource = value;
                OnPropertyChanged(nameof(BGSource));
            }
        }
        string message;
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
                IsMessageBoxVisible = !string.IsNullOrWhiteSpace(message);
                OnPropertyChanged(nameof(IsMessageBoxVisible));
            }
        }
        bool IsMessageBoxVisible { get; set; }

        PBattle battle;

        public BattleView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
        }

        public void SetBattle(PBattle battle)
        {
            this.battle = battle;
            string s;
            switch (battle.BattleStyle)
            {
                case PBattleStyle.Single: s = new string[] { "1-s", "2-s", "3-s", "4-s", "5-s", "6-s", "8-s" }.Sample(); break;
                case PBattleStyle.Double: s = new string[] { "1-d", "7-d" }.Sample(); break;
                case PBattleStyle.Triple: s = new string[] { "1-t", "4-t", "5-t", "8-t" }.Sample(); break;
                case PBattleStyle.Rotation: s = new string[] { "1-r", "2-r" }.Sample(); break;
                default: throw new ArgumentOutOfRangeException(nameof(battle.BattleStyle));
            }
            BGSource = Utils.UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.Assets.Backgrounds.{s}.png?assembly=PokemonBattleEngineClient"));
        }

        // pkmn.FieldPosition must be updated before calling this
        public void UpdatePokemon(PPokemon pkmn, PFieldPosition oldPosition = PFieldPosition.None)
        {
            switch (battle.BattleStyle)
            {
                case PBattleStyle.Single:
                    this.FindControl<HPBarView>("Bar0_Center").Location = new Point(206, 35);

                    this.FindControl<HPBarView>("Bar1_Center").Location = new Point(206, 6);

                    this.FindControl<PokemonView>("Battler0_Center").Location = new Point(75, 53);

                    this.FindControl<PokemonView>("Battler1_Center").Location = new Point(284, 8);
                    break;
                case PBattleStyle.Double:
                    this.FindControl<HPBarView>("Bar0_Left").Location = new Point(104, 35);
                    this.FindControl<HPBarView>("Bar0_Right").Location = new Point(308, 35);

                    this.FindControl<HPBarView>("Bar1_Right").Location = new Point(104, 6);
                    this.FindControl<HPBarView>("Bar1_Left").Location = new Point(308, 6);

                    this.FindControl<PokemonView>("Battler0_Left").Location = new Point(-37, 43);
                    this.FindControl<PokemonView>("Battler0_Right").Location = new Point(168, 54);

                    this.FindControl<PokemonView>("Battler1_Right").Location = new Point(242, 9);
                    this.FindControl<PokemonView>("Battler1_Left").Location = new Point(332, 15);
                    break;
                case PBattleStyle.Triple:
                    this.FindControl<HPBarView>("Bar0_Left").Location = new Point(53, 35);
                    this.FindControl<HPBarView>("Bar0_Center").Location = new Point(206, 35);
                    this.FindControl<HPBarView>("Bar0_Right").Location = new Point(359, 35);

                    this.FindControl<HPBarView>("Bar1_Right").Location = new Point(53, 6);
                    this.FindControl<HPBarView>("Bar1_Center").Location = new Point(206, 6);
                    this.FindControl<HPBarView>("Bar1_Left").Location = new Point(359, 6);

                    this.FindControl<PokemonView>("Battler0_Left").Location = new Point(-53, 51);
                    this.FindControl<PokemonView>("Battler0_Center").Location = new Point(92, 31);
                    this.FindControl<PokemonView>("Battler0_Right").Location = new Point(221, 76);

                    this.FindControl<PokemonView>("Battler1_Right").Location = new Point(209, -1);
                    this.FindControl<PokemonView>("Battler1_Center").Location = new Point(282, 16);
                    this.FindControl<PokemonView>("Battler1_Left").Location = new Point(362, 8);
                    break;
                case PBattleStyle.Rotation:
                    this.FindControl<HPBarView>("Bar0_Left").Location = new Point(53, 35);
                    this.FindControl<HPBarView>("Bar0_Center").Location = new Point(206, 35);
                    this.FindControl<HPBarView>("Bar0_Right").Location = new Point(359, 35);

                    this.FindControl<HPBarView>("Bar1_Right").Location = new Point(53, 6);
                    this.FindControl<HPBarView>("Bar1_Center").Location = new Point(206, 6);
                    this.FindControl<HPBarView>("Bar1_Left").Location = new Point(359, 6);

                    this.FindControl<PokemonView>("Battler0_Left").Location = new Point(-46, 384); // Hidden
                    this.FindControl<PokemonView>("Battler0_Center").Location = new Point(52, 72);
                    this.FindControl<PokemonView>("Battler0_Right").Location = new Point(228, 384); // Hidden

                    this.FindControl<PokemonView>("Battler1_Right").Location = new Point(211, -34);
                    this.FindControl<PokemonView>("Battler1_Center").Location = new Point(282, 16);
                    this.FindControl<PokemonView>("Battler1_Left").Location = new Point(421, -24);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(battle.BattleStyle));
            }

            HPBarView hpView;
            PokemonView pkmnView;
            if (oldPosition != PFieldPosition.None)
            {
                hpView = this.FindControl<HPBarView>($"Bar{(pkmn.Local ? 0 : 1)}_{oldPosition}");
                hpView.Update();
                pkmnView = this.FindControl<PokemonView>($"Battler{(pkmn.Local ? 0 : 1)}_{oldPosition}");
                pkmnView.Update();
            }
            if (pkmn.FieldPosition != PFieldPosition.None)
            {
                hpView = this.FindControl<HPBarView>($"Bar{(pkmn.Local ? 0 : 1)}_{pkmn.FieldPosition}");
                hpView.Pokemon = pkmn;
                hpView.Update();
                pkmnView = this.FindControl<PokemonView>($"Battler{(pkmn.Local ? 0 : 1)}_{pkmn.FieldPosition}");
                pkmnView.Pokemon = pkmn;
                pkmnView.Update();
            }
        }
    }
}
