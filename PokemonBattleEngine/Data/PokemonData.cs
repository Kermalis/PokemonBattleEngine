using System.Collections.Generic;

namespace PokemonBattleEngine.Data
{
    class PokemonData
    {
        public Gender GenderRatio { get; private set; }
        public Ability Ability1 { get; private set; }
        public Ability Ability2 { get; private set; }
        public Ability AbilityHidden { get; private set; }
        public Move[] Moves { get; private set; }

        public static Dictionary<Species, PokemonData> Data = new Dictionary<Species, PokemonData>()
        {
            {
                Species.Cresselia,
                new PokemonData
                {
                    GenderRatio = Gender.Female,
                    Ability1 = Ability.Levitate,
                    Moves = new Move[]
                    {
                        Move.Psychic,
                    }
                }
            },
            {
                Species.Darkrai,
                new PokemonData
                {
                    GenderRatio = Gender.Genderless,
                    Ability1 = Ability.BadDreams,
                    Moves = new Move[]
                    {
                        Move.DarkPulse,
                    }
                }
            },
        };
    }
}
