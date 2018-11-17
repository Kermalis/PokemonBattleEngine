using Avalonia.Media;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Views;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    class PokemonInfo : INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public event PropertyChangedEventHandler PropertyChanged;

        ReactiveCommand SelectPokemonCommand { get; }

        string label;
        string Label
        {
            get => label;
            set
            {
                label = value;
                OnPropertyChanged(nameof(Label));
            }
        }

        public readonly PPokemon Pokemon;

        public PokemonInfo(PPokemon pkmn, ActionsView parent, ref List<PPokemon> standBy)
        {
            Pokemon = pkmn;
            Label = pkmn.Shell.Nickname + pkmn.GenderSymbol;

            bool enabled = pkmn != parent.Pokemon && pkmn.FieldPosition == PFieldPosition.None && !standBy.Contains(pkmn) && pkmn.HP > 0;

            var sub = new Subject<bool>();
            SelectPokemonCommand = ReactiveCommand.Create(() => parent.SelectPokemon(this), sub);
            sub.OnNext(enabled);
        }
    }
}
