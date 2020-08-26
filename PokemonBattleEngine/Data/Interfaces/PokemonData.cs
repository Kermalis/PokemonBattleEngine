using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public interface IPBEPokemonData : IPBEPokemonTypes, IPBESpeciesForm
    {
        IPBEReadOnlyStatCollection BaseStats { get; }
        PBEGenderRatio GenderRatio { get; }
        byte CatchRate { get; }
        byte FleeRate { get; }
        /// <summary>Weight in Kilograms</summary>
        double Weight { get; }
        IReadOnlyList<PBEAbility> Abilities { get; }
        IReadOnlyList<(PBESpecies Species, PBEForm Form)> PreEvolutions { get; }
        IReadOnlyList<(PBESpecies Species, PBEForm Form)> Evolutions { get; }
        IReadOnlyList<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)> LevelUpMoves { get; }
        IReadOnlyList<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)> OtherMoves { get; }
    }

    public static class PBEPokemonDataExtensions
    {
        public static bool HasAbility(this IPBEPokemonData pData, PBEAbility ability)
        {
            if (ability == PBEAbility.None || ability >= PBEAbility.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(ability));
            }
            return pData.Abilities.Contains(ability);
        }
    }
}
