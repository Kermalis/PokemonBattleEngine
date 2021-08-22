using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public interface IPBEPokemonData : IPBEPokemonTypes, IPBESpeciesForm
    {
        IPBEReadOnlyStatCollection BaseStats { get; }
        PBEGenderRatio GenderRatio { get; }
        PBEGrowthRate GrowthRate { get; }
        ushort BaseEXPYield { get; }
        byte CatchRate { get; }
        byte FleeRate { get; }
        /// <summary>Weight in Kilograms</summary>
        float Weight { get; }
        IReadOnlyList<PBEAbility> Abilities { get; }
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
