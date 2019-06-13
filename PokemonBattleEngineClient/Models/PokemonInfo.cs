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
        public string Description => Utils.CustomPokemonToString(Pokemon, Pokemon.Team.Id == 0, Pokemon.Team.Id == 1);

        public PokemonInfo(PBEPokemon pkmn, bool locked, Action<PBEPokemon> clickAction)
        {
            Pokemon = pkmn;
            var sub = new Subject<bool>();
            SelectPokemonCommand = ReactiveCommand.Create(() => clickAction(pkmn), sub);
            sub.OnNext(!locked && pkmn.FieldPosition == PBEFieldPosition.None && pkmn.HP > 0);
        }
    }
}
