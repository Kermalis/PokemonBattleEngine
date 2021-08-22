using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.DefaultData
{
    public interface IPBEDDPokemonDataExtended : IPBEPokemonData
    {
        IReadOnlyList<(PBESpecies Species, PBEForm Form)> PreEvolutions { get; }
        IReadOnlyList<(PBESpecies Species, PBEForm Form)> Evolutions { get; }
        IReadOnlyList<(PBEMove Move, byte Level, PBEDDMoveObtainMethod ObtainMethod)> LevelUpMoves { get; }
        IReadOnlyList<(PBEMove Move, PBEDDMoveObtainMethod ObtainMethod)> OtherMoves { get; }
    }

    public static class PBEDDPokemonDataExtensions
    {
        public static bool HasEvolutions(this IPBEDDPokemonDataExtended pData)
        {
            return pData.Evolutions.Count > 0;
        }
    }
}
