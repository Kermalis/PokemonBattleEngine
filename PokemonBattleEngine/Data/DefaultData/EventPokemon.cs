using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kermalis.PokemonBattleEngine.Data.DefaultData
{
    public sealed partial class PBEEventPokemon
    {
        public ReadOnlyCollection<byte> Generations { get; }
        public PBESpecies Species { get; }
        public byte Level { get; }
        public bool? Shiny { get; } // "null" means the Pokémon can be shiny or not shiny
        public PBEGender? Gender { get; } // "null" means the Pokémon can be male or female
        public PBEAlphabeticalList<PBEAbility> PossibleAbilities { get; }
        public PBEAlphabeticalList<PBENature> PossibleNatures { get; }
        public ReadOnlyCollection<byte?> IndividualValues { get; } // A stat being "null" means that stat is random
        public ReadOnlyCollection<PBEMove> Moves { get; }

        private PBEEventPokemon(IList<byte> generations, PBESpecies species, byte level, bool? shiny, PBEGender? gender, PBEAbility[] possibleAbilities, PBEAlphabeticalList<PBENature> possibleNatures, byte?[] ivs, PBEMove[] moves)
        {
            Generations = new ReadOnlyCollection<byte>(generations);
            Species = species;
            Level = level;
            Shiny = shiny;
            Gender = gender;
            PossibleAbilities = new PBEAlphabeticalList<PBEAbility>(possibleAbilities);
            PossibleNatures = possibleNatures;
            IndividualValues = new ReadOnlyCollection<byte?>(ivs);
            Moves = new ReadOnlyCollection<PBEMove>(moves);
        }
        private PBEEventPokemon(IList<byte> generations, PBESpecies species, byte level, bool? shiny, PBEGender? gender, PBEAbility[] possibleAbilities, PBENature[] possibleNatures, byte?[] ivs, PBEMove[] moves)
            : this(generations, species, level, shiny, gender, possibleAbilities, new PBEAlphabeticalList<PBENature>(possibleNatures), ivs, moves)
        {
        }
    }
}
