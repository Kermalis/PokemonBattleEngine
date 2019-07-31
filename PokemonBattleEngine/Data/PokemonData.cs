using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed partial class PBEPokemonData
    {
        public ReadOnlyCollection<byte> BaseStats { get; }
        public PBEType Type1 { get; }
        public PBEType Type2 { get; }
        public PBEGenderRatio GenderRatio { get; }
        public double Weight { get; } // Kilograms
        public ReadOnlyCollection<PBESpecies> PreEvolutions { get; }
        public ReadOnlyCollection<PBESpecies> Evolutions { get; }
        public ReadOnlyCollection<PBEAbility> Abilities { get; }
        public ReadOnlyCollection<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)> LevelUpMoves { get; }
        public ReadOnlyCollection<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)> OtherMoves { get; }

        private PBEPokemonData(byte[] baseStats,
            PBEType type1, PBEType type2, PBEGenderRatio genderRatio, double weight,
            List<PBESpecies> preEvolutions,
            List<PBESpecies> evolutions,
            List<PBEAbility> abilities,
            List<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)> levelUpMoves,
            List<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)> otherMoves)
        {
            BaseStats = new ReadOnlyCollection<byte>(baseStats);
            Type1 = type1; Type2 = type2; GenderRatio = genderRatio; Weight = weight;
            PreEvolutions = new ReadOnlyCollection<PBESpecies>(preEvolutions);
            Evolutions = new ReadOnlyCollection<PBESpecies>(evolutions);
            Abilities = new ReadOnlyCollection<PBEAbility>(abilities);
            LevelUpMoves = new ReadOnlyCollection<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)>(levelUpMoves);
            OtherMoves = new ReadOnlyCollection<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)>(otherMoves);
        }

        public bool HasAbility(PBEAbility ability)
        {
            return Abilities.Contains(ability);
        }
        public bool HasType(PBEType type)
        {
            return Type1 == type || Type2 == type;
        }

        /// <summary>The type effectiveness table. The first key is the attacking type and the second key is the defending type.</summary>
        public static ReadOnlyDictionary<PBEType, ReadOnlyDictionary<PBEType, double>> TypeEffectiveness { get; } = new ReadOnlyDictionary<PBEType, ReadOnlyDictionary<PBEType, double>>(new Dictionary<PBEType, ReadOnlyDictionary<PBEType, double>>
        {
            { PBEType.None, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 1.0 },
                { PBEType.Water, 1.0 },
            })
            },
            { PBEType.Bug, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 2.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 0.5 },
                { PBEType.Fire, 0.5 },
                { PBEType.Flying, 0.5 },
                { PBEType.Ghost, 0.5 },
                { PBEType.Grass, 2.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 0.5 },
                { PBEType.Psychic, 2.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            })
            },
            { PBEType.Dark, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 0.5 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 0.5 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 2.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 2.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            })
            },
            { PBEType.Dragon, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 2.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            })
            },
            { PBEType.Electric, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 0.5 },
                { PBEType.Electric, 0.5 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 2.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 0.5 },
                { PBEType.Ground, 0.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 1.0 },
                { PBEType.Water, 2.0 },
            })
            },
            { PBEType.Fighting, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 0.5 },
                { PBEType.Dark, 2.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 0.5 },
                { PBEType.Ghost, 0.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 2.0 },
                { PBEType.Normal, 2.0 },
                { PBEType.Poison, 0.5 },
                { PBEType.Psychic, 0.5 },
                { PBEType.Rock, 2.0 },
                { PBEType.Steel, 2.0 },
                { PBEType.Water, 1.0 },
            })
            },
            { PBEType.Fire, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 2.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 0.5 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 0.5 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 2.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 2.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 0.5 },
                { PBEType.Steel, 2.0 },
                { PBEType.Water, 0.5 },
            })
            },
            { PBEType.Flying, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 2.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 0.5 },
                { PBEType.Fighting, 2.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 2.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 0.5 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            })
            },
            { PBEType.Ghost, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 0.5 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 2.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 0.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 2.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            })
            },
            { PBEType.Grass, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 0.5 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 0.5 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 0.5 },
                { PBEType.Flying, 0.5 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 0.5 },
                { PBEType.Ground, 2.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 0.5 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 2.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 2.0 },
            })
            },
            { PBEType.Ground, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 0.5 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 2.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 2.0 },
                { PBEType.Flying, 0.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 0.5 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 2.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 2.0 },
                { PBEType.Steel, 2.0 },
                { PBEType.Water, 1.0 },
            })
            },
            { PBEType.Ice, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 2.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 0.5 },
                { PBEType.Flying, 2.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 2.0 },
                { PBEType.Ground, 2.0 },
                { PBEType.Ice, 0.5 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 0.5 },
            })
            },
            { PBEType.Normal, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 0.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 0.5 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            })
            },
            { PBEType.Poison, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 0.5 },
                { PBEType.Grass, 2.0 },
                { PBEType.Ground, 0.5 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 0.5 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 0.5 },
                { PBEType.Steel, 0.0 },
                { PBEType.Water, 1.0 },
            })
            },
            { PBEType.Psychic, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 0.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 2.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 2.0 },
                { PBEType.Psychic, 0.5 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            })
            },
            { PBEType.Rock, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 2.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 0.5 },
                { PBEType.Fire, 2.0 },
                { PBEType.Flying, 2.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 0.5 },
                { PBEType.Ice, 2.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            })
            },
            { PBEType.Steel, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 0.5 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 0.5 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 2.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 2.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 0.5 },
            })
            },
            { PBEType.Water, new ReadOnlyDictionary<PBEType, double>(new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 0.5 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 2.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 0.5 },
                { PBEType.Ground, 2.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 2.0 },
                { PBEType.Steel, 1.0 },
                { PBEType.Water, 0.5 },
            })
            }
        });
        public static ReadOnlyDictionary<PBENature, ReadOnlyCollection<sbyte>> NatureBoosts { get; } = new ReadOnlyDictionary<PBENature, ReadOnlyCollection<sbyte>>(new Dictionary<PBENature, ReadOnlyCollection<sbyte>>
        {
            //                                                                Atk   Def SpAtk SpDef   Spd
            { PBENature.Adamant, new ReadOnlyCollection<sbyte>(new sbyte[] {   +1,    0,   -1,    0,    0}) },
            { PBENature.Bashful, new ReadOnlyCollection<sbyte>(new sbyte[] {    0,    0,    0,    0,    0}) },
            { PBENature.Bold,    new ReadOnlyCollection<sbyte>(new sbyte[] {   -1,   +1,    0,    0,    0}) },
            { PBENature.Brave,   new ReadOnlyCollection<sbyte>(new sbyte[] {   +1,    0,    0,    0,   -1}) },
            { PBENature.Calm,    new ReadOnlyCollection<sbyte>(new sbyte[] {   -1,    0,    0,   +1,    0}) },
            { PBENature.Careful, new ReadOnlyCollection<sbyte>(new sbyte[] {    0,    0,   -1,   +1,    0}) },
            { PBENature.Docile,  new ReadOnlyCollection<sbyte>(new sbyte[] {    0,    0,    0,    0,    0}) },
            { PBENature.Gentle,  new ReadOnlyCollection<sbyte>(new sbyte[] {    0,   -1,    0,   +1,    0}) },
            { PBENature.Hardy,   new ReadOnlyCollection<sbyte>(new sbyte[] {    0,    0,    0,    0,    0}) },
            { PBENature.Hasty,   new ReadOnlyCollection<sbyte>(new sbyte[] {    0,   -1,    0,    0,   +1}) },
            { PBENature.Impish,  new ReadOnlyCollection<sbyte>(new sbyte[] {    0,   +1,   -1,    0,    0}) },
            { PBENature.Jolly,   new ReadOnlyCollection<sbyte>(new sbyte[] {    0,    0,   -1,    0,   +1}) },
            { PBENature.Lax,     new ReadOnlyCollection<sbyte>(new sbyte[] {    0,   +1,    0,   -1,    0}) },
            { PBENature.Lonely,  new ReadOnlyCollection<sbyte>(new sbyte[] {   +1,   -1,    0,    0,    0}) },
            { PBENature.Mild,    new ReadOnlyCollection<sbyte>(new sbyte[] {    0,   -1,   +1,    0,    0}) },
            { PBENature.Modest,  new ReadOnlyCollection<sbyte>(new sbyte[] {   -1,    0,   +1,    0,    0}) },
            { PBENature.Naive,   new ReadOnlyCollection<sbyte>(new sbyte[] {    0,    0,    0,   -1,   +1}) },
            { PBENature.Naughty, new ReadOnlyCollection<sbyte>(new sbyte[] {   +1,    0,    0,   -1,    0}) },
            { PBENature.Quiet,   new ReadOnlyCollection<sbyte>(new sbyte[] {    0,    0,   +1,    0,   -1}) },
            { PBENature.Quirky,  new ReadOnlyCollection<sbyte>(new sbyte[] {    0,    0,    0,    0,    0}) },
            { PBENature.Rash,    new ReadOnlyCollection<sbyte>(new sbyte[] {    0,    0,   +1,   -1,    0}) },
            { PBENature.Relaxed, new ReadOnlyCollection<sbyte>(new sbyte[] {    0,   +1,    0,    0,   -1}) },
            { PBENature.Sassy,   new ReadOnlyCollection<sbyte>(new sbyte[] {    0,    0,    0,   +1,   -1}) },
            { PBENature.Serious, new ReadOnlyCollection<sbyte>(new sbyte[] {    0,    0,    0,    0,    0}) },
            { PBENature.Timid,   new ReadOnlyCollection<sbyte>(new sbyte[] {   -1,    0,    0,    0,   +1}) }
        });
        public static ReadOnlyCollection<PBEType> HiddenPowerTypes { get; } = new ReadOnlyCollection<PBEType>(new PBEType[]
        {
            PBEType.Fighting, // 7.8125 %
            PBEType.Flying,   // 6.2500 %
            PBEType.Poison,   // 6.2500 %
            PBEType.Ground,   // 6.2500 %
            PBEType.Rock,     // 6.2500 %
            PBEType.Bug,      // 7.8125 %
            PBEType.Ghost,    // 6.2500 %
            PBEType.Steel,    // 6.2500 %
            PBEType.Fire,     // 6.2500 %
            PBEType.Water,    // 6.2500 %
            PBEType.Grass,    // 7.8125 %
            PBEType.Electric, // 6.2500 %
            PBEType.Psychic,  // 6.2500 %
            PBEType.Ice,      // 6.2500 %
            PBEType.Dragon,   // 6.2500 %
            PBEType.Dark      // 1.5625 %
        });

        /// <summary>
        /// Calculates a species's stats based on its level, IVs, EVs, and nature.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="stat"/> is invalid.</exception>
        public static ushort CalculateStat(PBEStat stat, PBESpecies species, PBENature nature, byte evs, byte ivs, byte level, PBESettings settings)
        {
            switch (stat)
            {
                case PBEStat.HP:
                {
                    return (ushort)(species == PBESpecies.Shedinja ? 1 : ((((2 * GetData(species).BaseStats[0]) + ivs + (evs / 4)) * level / settings.MaxLevel) + level + 10));
                }
                case PBEStat.Attack:
                case PBEStat.Defense:
                case PBEStat.SpAttack:
                case PBEStat.SpDefense:
                case PBEStat.Speed:
                {
                    int statIndex = (int)stat;
                    double natureMultiplier = 1.0 + (NatureBoosts[nature][statIndex - 1] * settings.NatureStatBoost);
                    return (ushort)(((((2 * GetData(species).BaseStats[statIndex]) + ivs + (evs / 4)) * level / settings.MaxLevel) + 5) * natureMultiplier);
                }
                default: throw new ArgumentOutOfRangeException(nameof(stat));
            }
        }
        public static void GetStatRange(PBEStat stat, PBESpecies species, byte level, PBESettings settings, out ushort low, out ushort high)
        {
            switch (stat)
            {
                case PBEStat.HP:
                {
                    low = CalculateStat(PBEStat.HP, species, PBENature.Bashful, 0, 0, level, settings);
                    high = CalculateStat(PBEStat.HP, species, PBENature.Bashful, byte.MaxValue, settings.MaxIVs, level, settings);
                    break;
                }
                case PBEStat.Attack:
                {
                    low = CalculateStat(PBEStat.Attack, species, PBENature.Bold, 0, 0, level, settings);
                    high = CalculateStat(PBEStat.Attack, species, PBENature.Adamant, byte.MaxValue, settings.MaxIVs, level, settings);
                    break;
                }
                case PBEStat.Defense:
                {
                    low = CalculateStat(PBEStat.Defense, species, PBENature.Gentle, 0, 0, level, settings);
                    high = CalculateStat(PBEStat.Defense, species, PBENature.Bold, byte.MaxValue, settings.MaxIVs, level, settings);
                    break;
                }
                case PBEStat.SpAttack:
                {
                    low = CalculateStat(PBEStat.SpAttack, species, PBENature.Adamant, 0, 0, level, settings);
                    high = CalculateStat(PBEStat.SpAttack, species, PBENature.Mild, byte.MaxValue, settings.MaxIVs, level, settings);
                    break;
                }
                case PBEStat.SpDefense:
                {
                    low = CalculateStat(PBEStat.SpDefense, species, PBENature.Lax, 0, 0, level, settings);
                    high = CalculateStat(PBEStat.SpDefense, species, PBENature.Gentle, byte.MaxValue, settings.MaxIVs, level, settings);
                    break;
                }
                case PBEStat.Speed:
                {
                    low = CalculateStat(PBEStat.Speed, species, PBENature.Brave, 0, 0, level, settings);
                    high = CalculateStat(PBEStat.Speed, species, PBENature.Hasty, byte.MaxValue, settings.MaxIVs, level, settings);
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(stat));
            }
        }
        public static PBEType GetHiddenPowerType(byte hpIV, byte attackIV, byte defenseIV, byte spAttackIV, byte spDefenseIV, byte speedIV)
        {
            int a = hpIV & 1,
                b = attackIV & 1,
                c = defenseIV & 1,
                d = speedIV & 1,
                e = spAttackIV & 1,
                f = spDefenseIV & 1;
            return HiddenPowerTypes[(((1 << 0) * a) + ((1 << 1) * b) + ((1 << 2) * c) + ((1 << 3) * d) + ((1 << 4) * e) + ((1 << 5) * f)) * (HiddenPowerTypes.Count - 1) / ((1 << 6) - 1)];
        }
        // TODO: Make hidden power min and max damage settings
        public static byte GetHiddenPowerBasePower(byte hpIV, byte attackIV, byte defenseIV, byte spAttackIV, byte spDefenseIV, byte speedIV)
        {
            const byte mininumBasePower = 30,
                maximumBasePower = 70;
            int a = (hpIV & 2) == 2 ? 1 : 0,
                b = (attackIV & 2) == 2 ? 1 : 0,
                c = (defenseIV & 2) == 2 ? 1 : 0,
                d = (speedIV & 2) == 2 ? 1 : 0,
                e = (spAttackIV & 2) == 2 ? 1 : 0,
                f = (spDefenseIV & 2) == 2 ? 1 : 0;
            return (byte)(((((1 << 0) * a) + ((1 << 1) * b) + ((1 << 2) * c) + ((1 << 3) * d) + ((1 << 4) * e) + ((1 << 5) * f)) * (maximumBasePower - mininumBasePower) / ((1 << 6) - 1)) + mininumBasePower);
        }

        #region Database Querying

        private class SearchResult
        {
            public uint Id { get; set; }
            public string Json { get; set; }
        }
        public static PBEPokemonData GetData(PBESpecies species)
        {
            string json = PBEUtils.QueryDatabase<SearchResult>($"SELECT * FROM PokemonData WHERE Id={(uint)species}")[0].Json;
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                reader.Read(); // {
                reader.Read(); // "BaseStats":
                reader.Read(); // [
                byte[] baseStats = new byte[6];
                for (int i = 0; i < 6; i++)
                {
                    reader.Read();
                    baseStats[i] = Convert.ToByte(reader.Value);
                }
                reader.Read(); // ]
                reader.Read(); // "Type1":
                reader.Read();
                var type1 = (PBEType)Convert.ToByte(reader.Value);
                reader.Read(); // "Type2":
                reader.Read();
                var type2 = (PBEType)Convert.ToByte(reader.Value);
                reader.Read(); // "GenderRatio":
                reader.Read();
                var genderRatio = (PBEGenderRatio)Convert.ToByte(reader.Value);
                reader.Read(); // "Weight":
                reader.Read();
                double weight = Convert.ToDouble(reader.Value);
                reader.Read(); // "PreEvolutions":
                reader.Read(); // [
                var preEvolutions = new List<PBESpecies>();
                while (true)
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.Integer)
                    {
                        preEvolutions.Add((PBESpecies)Convert.ToUInt32(reader.Value));
                    }
                    else if (reader.TokenType == JsonToken.EndArray) // ]
                    {
                        break;
                    }
                }
                reader.Read(); // "Evolutions":
                reader.Read(); // [
                var evolutions = new List<PBESpecies>();
                while (true)
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.Integer)
                    {
                        evolutions.Add((PBESpecies)Convert.ToUInt32(reader.Value));
                    }
                    else if (reader.TokenType == JsonToken.EndArray) // ]
                    {
                        break;
                    }
                }
                reader.Read(); // "Abilities":
                reader.Read(); // [
                var abilities = new List<PBEAbility>();
                while (true)
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.Integer)
                    {
                        abilities.Add((PBEAbility)Convert.ToByte(reader.Value));
                    }
                    else if (reader.TokenType == JsonToken.EndArray) // ]
                    {
                        break;
                    }
                }
                reader.Read(); // "LevelUpMoves":
                reader.Read(); // [
                var levelUpMoves = new List<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)>();
                while (true)
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.StartArray) // [
                    {
                        reader.Read();
                        var move = (PBEMove)Convert.ToUInt16(reader.Value);
                        reader.Read();
                        byte level = Convert.ToByte(reader.Value);
                        reader.Read();
                        var method = (PBEMoveObtainMethod)Convert.ToUInt64(reader.Value);
                        levelUpMoves.Add((move, level, method));
                        reader.Read(); // ]
                    }
                    else if (reader.TokenType == JsonToken.EndArray) // ]
                    {
                        break;
                    }
                }
                reader.Read(); // "OtherMoves":
                reader.Read(); // [
                var otherMoves = new List<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)>();
                while (true)
                {
                    reader.Read();
                    if (reader.TokenType == JsonToken.StartArray) // [
                    {
                        reader.Read();
                        var move = (PBEMove)Convert.ToUInt16(reader.Value);
                        reader.Read();
                        var method = (PBEMoveObtainMethod)Convert.ToUInt64(reader.Value);
                        otherMoves.Add((move, method));
                        reader.Read(); // ]
                    }
                    else if (reader.TokenType == JsonToken.EndArray) // ]
                    {
                        break;
                    }
                }
                return new PBEPokemonData(baseStats, type1, type2, genderRatio, weight, preEvolutions, evolutions, abilities, levelUpMoves, otherMoves); // TODO: Cache?
            }
        }

        #endregion
    }
}
