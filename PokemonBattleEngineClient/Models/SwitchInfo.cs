using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using ReactiveUI;
using System;
using System.Reactive;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    public sealed class SwitchInfo
    {
        public ReactiveCommand<Unit, Unit> SelectPokemonCommand { get; }
        public bool Enabled { get; }

        public PokemonInfo Pokemon { get; }
        public string Description { get; }

        internal SwitchInfo(PBEBattlePokemon pkmn, bool locked, Action<PBEBattlePokemon> clickAction)
        {
            Pokemon = new PokemonInfo(pkmn, false);
            Description = Utils.CustomPokemonToString(pkmn, false);
            Enabled = !locked && pkmn.FieldPosition == PBEFieldPosition.None && pkmn.HP > 0;
            SelectPokemonCommand = ReactiveCommand.Create(() => clickAction(pkmn));
        }
    }
}
