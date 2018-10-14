using System;
using System.Collections.Generic;

namespace PokemonBattleEngine.Data
{
    class PokemonData
    {
        public Gender GenderRatio;
        public Ability Ability1, Ability2, AbilityHidden;
        public byte HP, Attack, Defense, SpAttack, SpDefense, Speed;
        public Tuple<int, Move>[] LevelUpMoves;

        public static Dictionary<Species, PokemonData> Data = new Dictionary<Species, PokemonData>()
        {
            {
                Species.Pikachu,
                new PokemonData
                {
                    GenderRatio = Gender.M1F1,
                    Ability1 = Ability.Static, AbilityHidden = Ability.LightningRod,
                    HP = 35, Attack = 55, Defense = 30, SpAttack = 50, SpDefense = 40, Speed = 90,
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
                    HP = 100, Attack = 50, Defense = 80, SpAttack = 50, SpDefense = 80, Speed = 50,
                    LevelUpMoves = new Tuple<int, Move>[]
                    {
                        Tuple.Create(46, Move.HydroPump),
                    }
                }
            },
            {
                Species.Clamperl,
                new PokemonData
                {
                    GenderRatio = Gender.M1F1,
                    Ability1 = Ability.ShellArmor, AbilityHidden = Ability.Rattled,
                    HP = 35, Attack = 64, Defense = 85, SpAttack = 74, SpDefense = 55, Speed = 32,
                    LevelUpMoves = new Tuple<int, Move>[]
                    {
                        Tuple.Create(51, Move.ShellSmash),
                    }
                }
            },
            {
                Species.Latias,
                new PokemonData
                {
                    GenderRatio = Gender.Female,
                    Ability1 = Ability.Levitate,
                    HP = 80, Attack = 80, Defense = 90, SpAttack = 110, SpDefense = 130, Speed = 110,
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
                    HP = 80, Attack = 90, Defense = 80, SpAttack = 130, SpDefense = 110, Speed = 110,
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
                    HP = 120, Attack = 70, Defense = 120, SpAttack = 75, SpDefense = 130, Speed = 85,
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
                    HP = 70, Attack = 90, Defense = 90, SpAttack = 135, SpDefense = 90, Speed = 125,
                    LevelUpMoves = new Tuple<int, Move>[]
                    {
                        Tuple.Create(93, Move.DarkPulse),
                    }
                }
            },
        };
    }
}
