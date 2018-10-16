using System;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Data
{
    class PPokemonData
    {
        public PGender GenderRatio;
        public PType Type1, Type2;
        public PAbility Ability1, Ability2, AbilityHidden;
        public byte HP, Attack, Defense, SpAttack, SpDefense, Speed;
        public Tuple<int, PMove>[] LevelUpMoves;
        public PMove[] OtherMoves;

        public static Dictionary<PSpecies, PPokemonData> Data = new Dictionary<PSpecies, PPokemonData>()
        {
            {
                PSpecies.Pikachu,
                new PPokemonData
                {
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Electric, Type2 = PType.Electric,
                    Ability1 = PAbility.Static, Ability2 = PAbility.Static, AbilityHidden = PAbility.LightningRod,
                    HP = 35, Attack = 55, Defense = 30, SpAttack = 50, SpDefense = 40, Speed = 90,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(50, PMove.Thunder),
                    }
                }
            },
            {
                PSpecies.Cubone,
                new PPokemonData
                {
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Ground, Type2 = PType.Ground,
                    Ability1 = PAbility.RockHead, Ability2 = PAbility.LightningRod, AbilityHidden = PAbility.BattleArmor,
                    HP = 50, Attack = 50, Defense = 95, SpAttack = 40, SpDefense = 50, Speed = 35,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(47, PMove.Retaliate),
                    }
                }
            },
            {
                PSpecies.Marowak,
                new PPokemonData
                {
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Ground, Type2 = PType.Ground,
                    Ability1 = PAbility.RockHead, Ability2 = PAbility.LightningRod, AbilityHidden = PAbility.BattleArmor,
                    HP = 60, Attack = 80, Defense = 110, SpAttack = 50, SpDefense = 80, Speed = 45,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(59, PMove.Retaliate),
                    }
                }
            },
            {
                PSpecies.Ditto,
                new PPokemonData
                {
                    GenderRatio = PGender.Genderless,
                    Type1 = PType.Normal, Type2 = PType.Normal,
                    Ability1 = PAbility.Limber, Ability2 = PAbility.Limber, AbilityHidden = PAbility.Imposter,
                    HP = 48, Attack = 48, Defense = 48, SpAttack = 48, SpDefense = 48, Speed = 48,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Transform),
                    }
                }
            },
            {
                PSpecies.Azumarill,
                new PPokemonData
                {
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Water, Type2 = PType.Water,
                    Ability1 = PAbility.ThickFat, Ability2 = PAbility.HugePower, AbilityHidden = PAbility.SapSipper,
                    HP = 100, Attack = 50, Defense = 80, SpAttack = 50, SpDefense = 80, Speed = 50,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(46, PMove.HydroPump),
                    },
                    OtherMoves = new PMove[]
                    {
                        PMove.AquaJet,
                        PMove.IcePunch,
                        PMove.Return,
                        PMove.Waterfall,
                    }
                }
            },
            {
                PSpecies.Clamperl,
                new PPokemonData
                {
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Water, Type2 = PType.Water,
                    Ability1 = PAbility.ShellArmor, Ability2 = PAbility.ShellArmor, AbilityHidden = PAbility.Rattled,
                    HP = 35, Attack = 64, Defense = 85, SpAttack = 74, SpDefense = 55, Speed = 32,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(51, PMove.ShellSmash),
                    }
                }
            },
            {
                PSpecies.Latias,
                new PPokemonData
                {
                    GenderRatio = PGender.Female,
                    Type1 = PType.Dragon, Type2 = PType.Psychic,
                    Ability1 = PAbility.Levitate, Ability2 = PAbility.Levitate, AbilityHidden = PAbility.Levitate,
                    HP = 80, Attack = 80, Defense = 90, SpAttack = 110, SpDefense = 130, Speed = 110,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(80, PMove.DragonPulse),
                    }
                }
            },
            {
                PSpecies.Latios,
                new PPokemonData
                {
                    GenderRatio = PGender.Male,
                    Type1 = PType.Dragon, Type2 = PType.Psychic,
                    Ability1 = PAbility.Levitate, Ability2 = PAbility.Levitate, AbilityHidden = PAbility.Levitate,
                    HP = 80, Attack = 90, Defense = 80, SpAttack = 130, SpDefense = 110, Speed = 110,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(80, PMove.DragonPulse),
                    }
                }
            },
            {
                PSpecies.Cresselia,
                new PPokemonData
                {
                    GenderRatio = PGender.Female,
                    Type1 = PType.Psychic, Type2 = PType.Psychic,
                    Ability1 = PAbility.Levitate, Ability2 = PAbility.Levitate, AbilityHidden = PAbility.Levitate,
                    HP = 120, Attack = 70, Defense = 120, SpAttack = 75, SpDefense = 130, Speed = 85,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(93, PMove.Psychic),
                    },
                    OtherMoves = new PMove[]
                    {
                        PMove.IceBeam,
                        PMove.Moonlight,
                        PMove.Toxic,
                    }
                }
            },
            {
                PSpecies.Darkrai,
                new PPokemonData
                {
                    GenderRatio = PGender.Genderless,
                    Type1 = PType.Dark, Type2 = PType.Dark,
                    Ability1 = PAbility.BadDreams, Ability2 = PAbility.BadDreams, AbilityHidden = PAbility.BadDreams,
                    HP = 70, Attack = 90, Defense = 90, SpAttack = 135, SpDefense = 90, Speed = 125,
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(93, PMove.DarkPulse),
                    }
                }
            },
        };
    }
}
