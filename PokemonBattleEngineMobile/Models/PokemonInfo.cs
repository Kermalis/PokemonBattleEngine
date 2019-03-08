using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Models
{
    public class PokemonInfo
    {
        public Command SelectPokemonCommand { get; }

        public PBEPokemon Pokemon { get; }

        public PokemonInfo(PBEPokemon pkmn, bool locked, Action<PBEPokemon> clickAction)
        {
            Pokemon = pkmn;

            bool enabled = !locked && pkmn.FieldPosition == PBEFieldPosition.None && pkmn.HP > 0;

            SelectPokemonCommand = new Command(() => clickAction(pkmn), () => enabled);
        }
    }
}
