using System;

namespace Kermalis.PokemonBattleEngine.Data
{
    public interface IPBEPokemonTypes
    {
        PBEType Type1 { get; }
        PBEType Type2 { get; }
    }

    public static class PBEPokemonTypesExtensions
    {
        public static bool HasType(this IPBEPokemonTypes pkmn, PBEType type)
        {
            if (pkmn == null)
            {
                throw new ArgumentException(nameof(pkmn));
            }
            if (type >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }
            return pkmn.Type1 == type || pkmn.Type2 == type;
        }
        public static bool ReceivesSTAB(this IPBEPokemonTypes pkmn, PBEType type)
        {
            return type != PBEType.None && HasType(pkmn, type);
        }
    }
}
