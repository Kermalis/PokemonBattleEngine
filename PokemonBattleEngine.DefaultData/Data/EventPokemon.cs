using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kermalis.PokemonBattleEngine.DefaultData.Data
{
    public sealed partial class PBEDDEventPokemon
    {
        public ReadOnlyCollection<byte> Generations { get; }
        public PBESpecies Species { get; }
        public byte Level { get; }
        /// <summary><see langword="null"/> means the Pokémon can be shiny or not shiny</summary>
        public bool? Shiny { get; }
        /// <summary><see langword="null"/> means the Pokémon can be male or female</summary>
        public PBEGender? Gender { get; }
        public ReadOnlyCollection<PBEAbility> PossibleAbilities { get; }
        public ReadOnlyCollection<PBENature> PossibleNatures { get; }
        public ReadOnlyCollection<byte?> IndividualValues { get; } // A stat being "null" means that stat is random
        public ReadOnlyCollection<PBEMove> Moves { get; }

        private PBEDDEventPokemon(IList<byte> generations, PBESpecies species, byte level, bool? shiny, PBEGender? gender, PBEAbility[] possibleAbilities, ReadOnlyCollection<PBENature> possibleNatures, byte?[] ivs, PBEMove[] moves)
        {
            Generations = new ReadOnlyCollection<byte>(generations);
            Species = species;
            Level = level;
            Shiny = shiny;
            Gender = gender;
            PossibleAbilities = new ReadOnlyCollection<PBEAbility>(possibleAbilities);
            PossibleNatures = possibleNatures;
            IndividualValues = new ReadOnlyCollection<byte?>(ivs);
            Moves = new ReadOnlyCollection<PBEMove>(moves);
        }
        private PBEDDEventPokemon(IList<byte> generations, PBESpecies species, byte level, bool? shiny, PBEGender? gender, PBEAbility[] possibleAbilities, PBENature[] possibleNatures, byte?[] ivs, PBEMove[] moves)
            : this(generations, species, level, shiny, gender, possibleAbilities, new ReadOnlyCollection<PBENature>(possibleNatures), ivs, moves)
        {
        }
        private PBEDDEventPokemon(IList<byte> generations, PBESpecies species, byte level, bool? shiny, PBEGender? gender, PBEAbility[] possibleAbilities, PBEAlphabeticalList<PBENature> possibleNatures, byte?[] ivs, PBEMove[] moves)
            : this(generations, species, level, shiny, gender, possibleAbilities, possibleNatures.AsReadOnly(), ivs, moves)
        {
        }
    }
}
