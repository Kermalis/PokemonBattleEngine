using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using ReactiveUI;
using System;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    public sealed class PokemonInfo
    {
        public ReactiveCommand SelectPokemonCommand { get; }
        public bool Enabled { get; }

        public PBEPokemon Pokemon { get; }
        public string Description => Utils.CustomPokemonToString(Pokemon, Pokemon.Team.Id == 0, Pokemon.Team.Id == 1);

        public PokemonInfo(PBEPokemon pkmn, bool locked, Action<PBEPokemon> clickAction)
        {
            Pokemon = pkmn;
            Enabled = !locked && pkmn.FieldPosition == PBEFieldPosition.None && pkmn.HP > 0;
            SelectPokemonCommand = ReactiveCommand.Create(() => clickAction(pkmn));
        }
    }
}
