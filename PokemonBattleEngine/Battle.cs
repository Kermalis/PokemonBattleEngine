using PokemonBattleEngine.Data;

namespace PokemonBattleEngine
{
    class Team
    {
        public Pokemon[] Pokemon = new Pokemon[Constants.MaxPokemon];
        public string PlayerName;
    }

    class Battle
    {
        readonly Team[] teams;

        public Battle(Team[] teams)
        {
            this.teams = teams;
        }
    }
}
