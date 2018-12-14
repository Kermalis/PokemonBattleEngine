using Avalonia.Media.Imaging;
using Kermalis.PokemonBattleEngine.Data;
using ReactiveUI;
using System;
using System.Reactive.Subjects;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    class PokemonInfo
    {
        ReactiveCommand SelectPokemonCommand { get; }

        public PBEPokemon Pokemon { get; }
        IBitmap Label { get; }

        public PokemonInfo(PBEPokemon pkmn, bool locked, Action<PokemonInfo> clickAction)
        {
            Pokemon = pkmn;
            Label = Utils.RenderString(pkmn.NameWithGender);

            bool enabled = !locked && pkmn.FieldPosition == PBEFieldPosition.None && pkmn.HP > 0;

            var sub = new Subject<bool>();
            SelectPokemonCommand = ReactiveCommand.Create(() => clickAction(this), sub);
            sub.OnNext(enabled);
        }
    }
}
