using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using ReactiveUI;
using System;
using System.Reactive.Subjects;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    public class PokemonInfo
    {
        public ReactiveCommand SelectPokemonCommand { get; }

        public PBEPokemon Pokemon { get; }
        public string Description => Utils.CustomPokemonToString(Pokemon, true);

        public PokemonInfo(PBEPokemon pkmn, bool locked, Action<PBEPokemon> clickAction)
        {
            Pokemon = pkmn;

            bool enabled = !locked && pkmn.FieldPosition == PBEFieldPosition.None && pkmn.HP > 0;

            var sub = new Subject<bool>();
            SelectPokemonCommand = ReactiveCommand.Create(() => clickAction(pkmn), sub);
            sub.OnNext(enabled);
        }
    }
}
