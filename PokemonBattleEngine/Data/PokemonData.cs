using System;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Data
{
    class PokemonData
    {
        public Gender GenderRatio;
        public Ability Ability1, Ability2, AbilityHidden;
        public byte HP, Attack, Defense, SpAttack, SpDefense, Speed;
        public Tuple<int, Move>[] LevelUpMoves;
        public Move[] OtherMoves;

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
                Species.Cubone,
                new PokemonData
                {
                    GenderRatio = Gender.M1F1,
                    Ability1 = Ability.RockHead, Ability2 = Ability.LightningRod, AbilityHidden = Ability.BattleArmor,
                    HP = 50, Attack = 50, Defense = 95, SpAttack = 40, SpDefense = 50, Speed = 35,
                    LevelUpMoves = new Tuple<int, Move>[]
                    {
                        Tuple.Create(47, Move.Retaliate),
                    }
                }
            },
            {
                Species.Marowak,
                new PokemonData
                {
                    GenderRatio = Gender.M1F1,
                    Ability1 = Ability.RockHead, Ability2 = Ability.LightningRod, AbilityHidden = Ability.BattleArmor,
                    HP = 60, Attack = 80, Defense = 110, SpAttack = 50, SpDefense = 80, Speed = 45,
                    LevelUpMoves = new Tuple<int, Move>[]
                    {
                        Tuple.Create(59, Move.Retaliate),
                    }
                }
            },
            {
                Species.Ditto,
                new PokemonData
                {
                    GenderRatio = Gender.Genderless,
                    Ability1 = Ability.Limber, AbilityHidden = Ability.Imposter,
                    HP = 48, Attack = 48, Defense = 48, SpAttack = 48, SpDefense = 48, Speed = 48,
                    LevelUpMoves = new Tuple<int, Move>[]
                    {
                        Tuple.Create(1, Move.Transform),
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
                    },
                    OtherMoves = new Move[]
                    {
                        Move.AquaJet,
                        Move.IcePunch,
                        Move.Return,
                        Move.Waterfall,
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
                    },
                    OtherMoves = new Move[]
                    {
                        Move.IceBeam,
                        Move.Moonlight,
                        Move.Toxic,
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
