using Kermalis.PokemonBattleEngine.Data;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    public sealed class TeamInfo
    {
        public string Name { get; }
        public PBETeamShell Shell { get; }

        internal TeamInfo(string name, PBETeamShell shell)
        {
            Name = name;
            Shell = shell;
        }
    }
}
