using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PPokemonData
    {
        public byte HP, Attack, Defense, SpAttack, SpDefense, Speed;
        public PGender GenderRatio;
        public PType Type1, Type2;
        public PAbility[] Abilities;
        public Tuple<int, PMove>[] LevelUpMoves;
        public PMove[] OtherMoves;

        public bool HasAbility(PAbility ability) => Abilities.Contains(ability);
        public bool HasType(PType type) => Type1 == type || Type2 == type;

        // First is attacker, second is defender
        // Cast PType to an int for the indices
        // [0,1] = bug attacker, dark defender
        public static readonly double[,] TypeEffectiveness = new double[,]
        {
            // Defender
            //    Bug     Dark   Dragon Electric Fighting     Fire   Flying    Ghost    Grass   Ground      Ice   Normal   Poison  Psychic     Rock    Steel    Water
            {     1.0,     2.0,     1.0,     1.0,     0.5,     0.5,     0.5,     0.5,     2.0,     1.0,     1.0,     1.0,     0.5,     2.0,     1.0,     0.5,     1.0}, // Bug
            {     1.0,     0.5,     1.0,     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0}, // Dark
            {     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     1.0}, // Dragon
            {     1.0,     1.0,     0.5,     0.5,     1.0,     1.0,     2.0,     1.0,     0.5,     0.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0}, // Electric
            {     0.5,     2.0,     1.0,     1.0,     1.0,     1.0,     0.5,     0.0,     1.0,     1.0,     2.0,     2.0,     0.5,     0.5,     2.0,     2.0,     1.0}, // Fighting
            {     2.0,     1.0,     0.5,     1.0,     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     2.0,     1.0,     1.0,     1.0,     0.5,     2.0,     0.5}, // Fire
            {     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     0.5,     1.0}, // Flying
            {     1.0,     0.5,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     0.0,     1.0,     2.0,     1.0,     1.0,     1.0}, // Ghost
            {     0.5,     1.0,     0.5,     1.0,     1.0,     0.5,     0.5,     1.0,     1.0,     2.0,     1.0,     1.0,     0.5,     1.0,     2.0,     0.5,     2.0}, // Grass
            {     0.5,     1.0,     1.0,     2.0,     1.0,     2.0,     0.0,     1.0,     0.5,     1.0,     1.0,     1.0,     2.0,     1.0,     2.0,     2.0,     1.0}, // Ground
            {     1.0,     1.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     2.0,     2.0,     0.5,     1.0,     1.0,     1.0,     1.0,     0.5,     0.5}, // Ice
            {     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     0.5,     1.0}, // Normal
            {     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     2.0,     0.5,     1.0,     1.0,     0.5,     1.0,     0.5,     0.0,     1.0}, // Poison
            {     1.0,     0.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0,     0.5,     1.0,     0.5,     1.0}, // Psychic
            {     2.0,     1.0,     1.0,     1.0,     0.5,     2.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     1.0,     0.5,     1.0}, // Rock
            {     1.0,     1.0,     1.0,     0.5,     1.0,     0.5,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     2.0,     0.5,     0.5}, // Steel
            {     1.0,     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     0.5}, // Water
                                                                                                                                                                        // Attacker
        };
        public static readonly Dictionary<PNature, sbyte[]> NatureBoosts = new Dictionary<PNature, sbyte[]>
        {
            //                                Atk   Def SpAtk SpDef   Spd
            { PNature.Adamant, new sbyte[] {   +1,    0,   -1,    0,    0} },
            { PNature.Bashful, new sbyte[] {    0,    0,    0,    0,    0} },
            { PNature.Bold,    new sbyte[] {   -1,   +1,    0,    0,    0} },
            { PNature.Brave,   new sbyte[] {   +1,    0,    0,    0,   -1} },
            { PNature.Calm,    new sbyte[] {   -1,    0,    0,   +1,    0} },
            { PNature.Careful, new sbyte[] {    0,    0,   -1,   +1,    0} },
            { PNature.Docile,  new sbyte[] {    0,    0,    0,    0,    0} },
            { PNature.Gentle,  new sbyte[] {    0,   -1,    0,   +1,    0} },
            { PNature.Hardy,   new sbyte[] {    0,    0,    0,    0,    0} },
            { PNature.Hasty,   new sbyte[] {    0,   -1,    0,    0,   +1} },
            { PNature.Impish,  new sbyte[] {    0,   +1,   -1,    0,    0} },
            { PNature.Jolly,   new sbyte[] {    0,    0,   -1,    0,   +1} },
            { PNature.Lax,     new sbyte[] {    0,   +1,    0,   -1,    0} },
            { PNature.Loney,   new sbyte[] {   +1,   -1,    0,    0,    0} },
            { PNature.Mild,    new sbyte[] {    0,   -1,   +1,    0,    0} },
            { PNature.Modest,  new sbyte[] {   -1,    0,   +1,    0,    0} },
            { PNature.Naive,   new sbyte[] {    0,    0,    0,   -1,   +1} },
            { PNature.Naughty, new sbyte[] {   +1,    0,    0,   -1,    0} },
            { PNature.Quiet,   new sbyte[] {    0,    0,   +1,    0,   -1} },
            { PNature.Quirky,  new sbyte[] {    0,    0,    0,    0,    0} },
            { PNature.Rash,    new sbyte[] {    0,    0,   +1,   -1,    0} },
            { PNature.Relaxed, new sbyte[] {    0,   +1,    0,    0,   -1} },
            { PNature.Sassy,   new sbyte[] {    0,    0,    0,   +1,   -1} },
            { PNature.Serious, new sbyte[] {    0,    0,    0,    0,    0} },
            { PNature.Timid,   new sbyte[] {   -1,    0,    0,    0,   +1} },
        };
        public static readonly PType[] HiddenPowerTypes = new PType[]
        {
            PType.Fighting, // 7.8125 %
            PType.Flying,   // 6.2500 %
            PType.Poison,   // 6.2500 %
            PType.Ground,   // 6.2500 %
            PType.Rock,     // 6.2500 %
            PType.Bug,      // 7.8125 %
            PType.Ghost,    // 6.2500 %
            PType.Steel,    // 6.2500 %
            PType.Fire,     // 6.2500 %
            PType.Water,    // 6.2500 %
            PType.Grass,    // 7.8125 %
            PType.Electric, // 6.2500 %
            PType.Psychic,  // 6.2500 %
            PType.Ice,      // 6.2500 %
            PType.Dragon,   // 6.2500 %
            PType.Dark      // 1.5625 %
        };


        public static Dictionary<PSpecies, PPokemonData> Data = new Dictionary<PSpecies, PPokemonData>()
        {
            {
                PSpecies.Pikachu,
                new PPokemonData
                {
                    HP = 35, Attack = 55, Defense = 30, SpAttack = 50, SpDefense = 40, Speed = 90,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Electric, Type2 = PType.Electric,
                    Abilities = new PAbility[] { PAbility.Static, PAbility.LightningRod },
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Growl),
                        Tuple.Create(18, PMove.NastyPlot), // As Pichu
                    },
                    OtherMoves = new PMove[]
                    {
                        PMove.Frustration,
                        PMove.HiddenPower,
                        PMove.Return,
                        PMove.Thunder,
                        PMove.Thunderbolt,
                    }
                }
            },
            {
                PSpecies.Cubone,
                new PPokemonData
                {
                    HP = 50, Attack = 50, Defense = 95, SpAttack = 40, SpDefense = 50, Speed = 35,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Ground, Type2 = PType.Ground,
                    Abilities = new PAbility[] { PAbility.RockHead, PAbility.LightningRod, PAbility.BattleArmor },
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Growl),
                        Tuple.Create(47, PMove.Retaliate),
                    },
                    OtherMoves = new PMove[]
                    {
                        PMove.Frustration,
                        PMove.HiddenPower,
                        PMove.Return,
                    }
                }
            },
            {
                PSpecies.Marowak,
                new PPokemonData
                {
                    HP = 60, Attack = 80, Defense = 110, SpAttack = 50, SpDefense = 80, Speed = 45,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Ground, Type2 = PType.Ground,
                    Abilities = new PAbility[] { PAbility.RockHead, PAbility.LightningRod, PAbility.BattleArmor },
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Growl),
                        Tuple.Create(59, PMove.Retaliate),
                    },
                    OtherMoves = new PMove[]
                    {
                        PMove.Frustration,
                        PMove.HiddenPower,
                        PMove.Return,
                    }
                }
            },
            {
                PSpecies.Ditto,
                new PPokemonData
                {
                    HP = 48, Attack = 48, Defense = 48, SpAttack = 48, SpDefense = 48, Speed = 48,
                    GenderRatio = PGender.Genderless,
                    Type1 = PType.Normal, Type2 = PType.Normal,
                    Abilities = new PAbility[] { PAbility.Limber, PAbility.Imposter },
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(1, PMove.Transform),
                    },
                    OtherMoves = new PMove[]
                    {

                    }
                }
            },
            {
                PSpecies.Azumarill,
                new PPokemonData
                {
                    HP = 100, Attack = 50, Defense = 80, SpAttack = 50, SpDefense = 80, Speed = 50,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Water, Type2 = PType.Water,
                    Abilities = new PAbility[] { PAbility.ThickFat, PAbility.HugePower, PAbility.SapSipper },
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(46, PMove.HydroPump),
                    },
                    OtherMoves = new PMove[]
                    {
                        PMove.AquaJet,
                        PMove.Frustration,
                        PMove.HiddenPower,
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
                    HP = 35, Attack = 64, Defense = 85, SpAttack = 74, SpDefense = 55, Speed = 32,
                    GenderRatio = PGender.M1F1,
                    Type1 = PType.Water, Type2 = PType.Water,
                    Abilities = new PAbility[] { PAbility.ShellArmor, PAbility.Rattled },
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(51, PMove.ShellSmash),
                    },
                    OtherMoves = new PMove[]
                    {
                        PMove.Frustration,
                        PMove.HiddenPower,
                        PMove.Return,
                    }
                }
            },
            {
                PSpecies.Latias,
                new PPokemonData
                {
                    HP = 80, Attack = 80, Defense = 90, SpAttack = 110, SpDefense = 130, Speed = 110,
                    GenderRatio = PGender.Female,
                    Type1 = PType.Dragon, Type2 = PType.Psychic,
                    Abilities = new PAbility[] { PAbility.Levitate },
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(80, PMove.DragonPulse),
                    },
                    OtherMoves = new PMove[]
                    {
                        PMove.Frustration,
                        PMove.HiddenPower,
                        PMove.Return,
                        PMove.Thunder,
                        PMove.Thunderbolt,
                    }
                }
            },
            {
                PSpecies.Latios,
                new PPokemonData
                {
                    HP = 80, Attack = 90, Defense = 80, SpAttack = 130, SpDefense = 110, Speed = 110,
                    GenderRatio = PGender.Male,
                    Type1 = PType.Dragon, Type2 = PType.Psychic,
                    Abilities = new PAbility[] { PAbility.Levitate },
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(80, PMove.DragonPulse),
                    },
                    OtherMoves = new PMove[]
                    {
                        PMove.Frustration,
                        PMove.HiddenPower,
                        PMove.Return,
                        PMove.Thunder,
                        PMove.Thunderbolt,
                    }
                }
            },
            {
                PSpecies.Cresselia,
                new PPokemonData
                {
                    HP = 120, Attack = 70, Defense = 120, SpAttack = 75, SpDefense = 130, Speed = 85,
                    GenderRatio = PGender.Female,
                    Type1 = PType.Psychic, Type2 = PType.Psychic,
                    Abilities = new PAbility[] { PAbility.Levitate },
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(93, PMove.Psychic),
                    },
                    OtherMoves = new PMove[]
                    {
                        PMove.Frustration,
                        PMove.HiddenPower,
                        PMove.IceBeam,
                        PMove.Moonlight,
                        PMove.Return,
                        PMove.Toxic,
                    }
                }
            },
            {
                PSpecies.Darkrai,
                new PPokemonData
                {
                    HP = 70, Attack = 90, Defense = 90, SpAttack = 135, SpDefense = 90, Speed = 125,
                    GenderRatio = PGender.Genderless,
                    Type1 = PType.Dark, Type2 = PType.Dark,
                    Abilities = new PAbility[] { PAbility.BadDreams },
                    LevelUpMoves = new Tuple<int, PMove>[]
                    {
                        Tuple.Create(75, PMove.NastyPlot),
                        Tuple.Create(93, PMove.DarkPulse),
                    },
                    OtherMoves = new PMove[]
                    {
                        PMove.Frustration,
                        PMove.HiddenPower,
                        PMove.Return,
                        PMove.Thunder,
                        PMove.Thunderbolt,
                    }
                }
            },
        };
    }
}
