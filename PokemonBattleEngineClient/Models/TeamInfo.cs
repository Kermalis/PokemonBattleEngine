using Kermalis.PokemonBattleEngine.Data.Legality;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    public sealed class TeamInfo
    {
        public string Name { get; }
        public PBELegalPokemonCollection Party { get; }

        internal TeamInfo(string name, PBELegalPokemonCollection party)
        {
            Name = name;
            Party = party;
        }
    }
}
