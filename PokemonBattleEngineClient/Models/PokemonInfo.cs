using Avalonia.Media.Imaging;
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

        public PBEPokemon Pokemon { get; }
        IBitmap Label { get; }

        public PokemonInfo(PBEPokemon pkmn, ActionsView parent, List<PBEPokemon> standBy)
        {
            Pokemon = pkmn;
            Label = Utils.RenderString(pkmn.NameWithGender);

            bool enabled = parent.Pokemon.LockedAction.Decision != PBEDecision.Fight && pkmn.FieldPosition == PBEFieldPosition.None && !standBy.Contains(pkmn) && pkmn.HP > 0;

            var sub = new Subject<bool>();
            SelectPokemonCommand = ReactiveCommand.Create(() => parent.SelectPokemon(this), sub);
            sub.OnNext(enabled);
        }
    }
}
