using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class BattleView : UserControl, INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        IBitmap BGSource { get; set; }
        string Message { get; set; }
        bool IsMessageBoxVisible { get; set; }

        PBEBattle battle;

        public BattleView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
        }

        public void SetMessage(string str)
        {
            Message = str;
            OnPropertyChanged(nameof(Message));
            IsMessageBoxVisible = !string.IsNullOrWhiteSpace(str);
            OnPropertyChanged(nameof(IsMessageBoxVisible));
        }
        public void SetBattle(PBEBattle battle)
        {
            this.battle = battle;
            string s;
            switch (battle.BattleFormat)
            {
                case PBEBattleFormat.Single: s = PBEUtils.Sample(new string[] { "1-s", "2-s", "3-s", "4-s", "5-s", "6-s", "8-s" }); break;
                case PBEBattleFormat.Double: s = PBEUtils.Sample(new string[] { "1-d", "7-d" }); break;
                case PBEBattleFormat.Triple: s = PBEUtils.Sample(new string[] { "1-t", "4-t", "5-t", "8-t" }); break;
                case PBEBattleFormat.Rotation: s = PBEUtils.Sample(new string[] { "1-r", "2-r" }); break;
                default: throw new ArgumentOutOfRangeException(nameof(battle.BattleFormat));
            }
            BGSource = Utils.UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.Assets.Backgrounds.{s}.png?assembly=PokemonBattleEngineClient"));
            OnPropertyChanged(nameof(BGSource));
        }

        // pkmn.FieldPosition must be updated before calling this
        public void UpdatePokemon(PBEPokemon pkmn, PBEFieldPosition oldPosition = PBEFieldPosition.None)
        {
            switch (battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                    this.FindControl<HPBarView>("Bar0_Center").Location = new Point(206, 35);

                    this.FindControl<HPBarView>("Bar1_Center").Location = new Point(206, 6);

                    this.FindControl<PokemonView>("Battler0_Center").Location = new Point(75, 53);

                    this.FindControl<PokemonView>("Battler1_Center").Location = new Point(284, 8);
                    break;
                case PBEBattleFormat.Double:
                    this.FindControl<HPBarView>("Bar0_Left").Location = new Point(104, 35);
                    this.FindControl<HPBarView>("Bar0_Right").Location = new Point(308, 35);

                    this.FindControl<HPBarView>("Bar1_Right").Location = new Point(104, 6);
                    this.FindControl<HPBarView>("Bar1_Left").Location = new Point(308, 6);

                    this.FindControl<PokemonView>("Battler0_Left").Location = new Point(-37, 43);
                    this.FindControl<PokemonView>("Battler0_Right").Location = new Point(168, 54);

                    this.FindControl<PokemonView>("Battler1_Right").Location = new Point(242, 9);
                    this.FindControl<PokemonView>("Battler1_Left").Location = new Point(332, 15);
                    break;
                case PBEBattleFormat.Triple:
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
                case PBEBattleFormat.Rotation:
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
                default: throw new ArgumentOutOfRangeException(nameof(battle.BattleFormat));
            }

            HPBarView hpView;
            PokemonView pkmnView;
            if (oldPosition != PBEFieldPosition.None)
            {
                hpView = this.FindControl<HPBarView>($"Bar{(pkmn.LocalTeam ? 0 : 1)}_{oldPosition}");
                hpView.Update();
                pkmnView = this.FindControl<PokemonView>($"Battler{(pkmn.LocalTeam ? 0 : 1)}_{oldPosition}");
                pkmnView.Update();
            }
            if (pkmn.FieldPosition != PBEFieldPosition.None)
            {
                hpView = this.FindControl<HPBarView>($"Bar{(pkmn.LocalTeam ? 0 : 1)}_{pkmn.FieldPosition}");
                hpView.Pokemon = pkmn;
                hpView.Update();
                pkmnView = this.FindControl<PokemonView>($"Battler{(pkmn.LocalTeam ? 0 : 1)}_{pkmn.FieldPosition}");
                pkmnView.Pokemon = pkmn;
                pkmnView.Update();
            }
        }
    }
}
