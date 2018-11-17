using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Views;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    class PokemonInfo
    {
        ReactiveCommand SelectPokemonCommand { get; }

        public PPokemon Pokemon { get; }
        string Label { get; }

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
