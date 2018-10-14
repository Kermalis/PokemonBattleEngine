using System;
using System.Collections.Generic;

namespace PokemonBattleEngine.Data
{
    class PokemonData
    {
        public Gender GenderRatio { get; private set; }
        public Ability Ability1 { get; private set; }
        public Ability Ability2 { get; private set; }
        public Ability AbilityHidden { get; private set; }
        public Tuple<int, Move>[] LevelUpMoves { get; private set; }

        public static Dictionary<Species, PokemonData> Data = new Dictionary<Species, PokemonData>()
        {
            {
                Species.Pikachu,
                new PokemonData
                {
                    GenderRatio = Gender.M1F1,
                    Ability1 = Ability.Static, AbilityHidden = Ability.LightningRod,
                    LevelUpMoves = new Tuple<int, Move>[]
                    {
                        Tuple.Create(50, Move.Thunder),
                    }
                }
            },
            {
                Species.Azumarill,
                new PokemonData
                {
                    GenderRatio = Gender.M1F1,
                    Ability1 = Ability.ThickFat, Ability2 = Ability.HugePower, AbilityHidden = Ability.SapSipper,
                    LevelUpMoves = new Tuple<int, Move>[]
                    {
                        Tuple.Create(46, Move.HydroPump),
                    }
                }
            },
            {
                Species.Latias,
                new PokemonData
                {
                    GenderRatio = Gender.Female,
                    Ability1 = Ability.Levitate,
                    LevelUpMoves = new Tuple<int, Move>[]
                    {
                        Tuple.Create(80, Move.DragonPulse),
                    }
                }
            },
            {
                Species.Latios,
                new PokemonData
                {
                    GenderRatio = Gender.Male,
                    Ability1 = Ability.Levitate,
                    LevelUpMoves = new Tuple<int, Move>[]
                    {
                        Tuple.Create(80, Move.DragonPulse),
                    }
                }
            },
            {
                Species.Cresselia,
                new PokemonData
                {
                    GenderRatio = Gender.Female,
                    Ability1 = Ability.Levitate,
                    LevelUpMoves = new Tuple<int, Move>[]
                    {
                        Tuple.Create(93, Move.Psychic),
                    }
                }
            },
            {
                Species.Darkrai,
                new PokemonData
                {
                    GenderRatio = Gender.Genderless,
                    Ability1 = Ability.BadDreams,
                    LevelUpMoves = new Tuple<int, Move>[]
                    {
                        Tuple.Create(93, Move.DarkPulse),
                    }
                }
            },
        };
    }
}
