using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        public ReadOnlyCollection<Tuple<PBEMove, byte, PBEMoveObtainMethod>> LevelUpMoves { get; }
        public ReadOnlyCollection<Tuple<PBEMove, PBEMoveObtainMethod>> OtherMoves { get; }

        private PBEPokemonData(byte hp, byte attack, byte defense, byte spAttack, byte spDefense, byte speed,
            PBEType type1, PBEType type2, PBEGenderRatio genderRatio, double weight,
            IList<PBESpecies> preEvolutions,
            IList<PBESpecies> evolutions,
            IList<PBEAbility> abilities,
            IList<Tuple<PBEMove, int, PBEMoveObtainMethod>> levelUpMoves,
            IList<Tuple<PBEMove, PBEMoveObtainMethod>> otherMoves)
        {
            BaseStats = new ReadOnlyCollection<byte>(new byte[] { hp, attack, defense, spAttack, spDefense, speed });
            Type1 = type1; Type2 = type2; GenderRatio = genderRatio; Weight = weight;
            PreEvolutions = new ReadOnlyCollection<PBESpecies>(preEvolutions);
            Evolutions = new ReadOnlyCollection<PBESpecies>(evolutions);
            Abilities = new ReadOnlyCollection<PBEAbility>(abilities);
            LevelUpMoves = new ReadOnlyCollection<Tuple<PBEMove, byte, PBEMoveObtainMethod>>(levelUpMoves.Select(t => Tuple.Create(t.Item1, (byte)t.Item2, t.Item3)).ToArray());
            OtherMoves = new ReadOnlyCollection<Tuple<PBEMove, PBEMoveObtainMethod>>(otherMoves);
        }

        public bool HasAbility(PBEAbility ability) => Abilities.Contains(ability);
        public bool HasType(PBEType type) => Type1 == type || Type2 == type;

        // First is attacker, second is defender
        // TypeEffectiveness[(int)PBEType.Bug][(int)PBEType.Dark] = TypeEffectiveness[1][2] = Bug attacker, Dark defender
        public static ReadOnlyCollection<ReadOnlyCollection<double>> TypeEffectiveness { get; } = new ReadOnlyCollection<ReadOnlyCollection<double>>(new ReadOnlyCollection<double>[]
        {
            //                                  Defender     None      Bug     Dark   Dragon Electric Fighting     Fire   Flying    Ghost    Grass   Ground      Ice   Normal   Poison  Psychic     Rock    Steel    Water
            new ReadOnlyCollection<double>(new double[] {     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0}), // None
            new ReadOnlyCollection<double>(new double[] {     1.0,     1.0,     2.0,     1.0,     1.0,     0.5,     0.5,     0.5,     0.5,     2.0,     1.0,     1.0,     1.0,     0.5,     2.0,     1.0,     0.5,     1.0}), // Bug
            new ReadOnlyCollection<double>(new double[] {     1.0,     1.0,     0.5,     1.0,     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     0.5,     1.0}), // Dark
            new ReadOnlyCollection<double>(new double[] {     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     1.0}), // Dragon
            new ReadOnlyCollection<double>(new double[] {     1.0,     1.0,     1.0,     0.5,     0.5,     1.0,     1.0,     2.0,     1.0,     0.5,     0.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0}), // Electric
            new ReadOnlyCollection<double>(new double[] {     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     1.0,     0.5,     0.0,     1.0,     1.0,     2.0,     2.0,     0.5,     0.5,     2.0,     2.0,     1.0}), // Fighting
            new ReadOnlyCollection<double>(new double[] {     1.0,     2.0,     1.0,     0.5,     1.0,     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     2.0,     1.0,     1.0,     1.0,     0.5,     2.0,     0.5}), // Fire
            new ReadOnlyCollection<double>(new double[] {     1.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     0.5,     1.0}), // Flying
            new ReadOnlyCollection<double>(new double[] {     1.0,     1.0,     0.5,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     0.0,     1.0,     2.0,     1.0,     0.5,     1.0}), // Ghost
            new ReadOnlyCollection<double>(new double[] {     1.0,     0.5,     1.0,     0.5,     1.0,     1.0,     0.5,     0.5,     1.0,     0.5,     2.0,     1.0,     1.0,     0.5,     1.0,     2.0,     0.5,     2.0}), // Grass
            new ReadOnlyCollection<double>(new double[] {     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     2.0,     0.0,     1.0,     0.5,     1.0,     1.0,     1.0,     2.0,     1.0,     2.0,     2.0,     1.0}), // Ground
            new ReadOnlyCollection<double>(new double[] {     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     2.0,     2.0,     0.5,     1.0,     1.0,     1.0,     1.0,     0.5,     0.5}), // Ice
            new ReadOnlyCollection<double>(new double[] {     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     0.5,     1.0}), // Normal
            new ReadOnlyCollection<double>(new double[] {     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     0.5,     2.0,     0.5,     1.0,     1.0,     0.5,     1.0,     0.5,     0.0,     1.0}), // Poison
            new ReadOnlyCollection<double>(new double[] {     1.0,     1.0,     0.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     1.0,     2.0,     0.5,     1.0,     0.5,     1.0}), // Psychic
            new ReadOnlyCollection<double>(new double[] {     1.0,     2.0,     1.0,     1.0,     1.0,     0.5,     2.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     1.0,     0.5,     1.0}), // Rock
            new ReadOnlyCollection<double>(new double[] {     1.0,     1.0,     1.0,     1.0,     0.5,     1.0,     0.5,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     1.0,     1.0,     2.0,     0.5,     0.5}), // Steel
            new ReadOnlyCollection<double>(new double[] {     1.0,     1.0,     1.0,     0.5,     1.0,     1.0,     2.0,     1.0,     1.0,     0.5,     2.0,     1.0,     1.0,     1.0,     1.0,     2.0,     1.0,     0.5})  // Water
                                                                                                                                                                                                                              // Attacker
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
                        return (ushort)(species == PBESpecies.Shedinja ? 1 : (((2 * Data[species].BaseStats[0] + ivs + (evs / 4)) * level / settings.MaxLevel) + level + 10));
                    }
                case PBEStat.Attack:
                case PBEStat.Defense:
                case PBEStat.SpAttack:
                case PBEStat.SpDefense:
                case PBEStat.Speed:
                    {
                        int statIndex = (int)stat;
                        double natureMultiplier = 1.0 + (PBEPokemonData.NatureBoosts[nature][statIndex - 1] * settings.NatureStatBoost);
                        return (ushort)((((2 * Data[species].BaseStats[statIndex] + ivs + (evs / 4)) * level / settings.MaxLevel) + 5) * natureMultiplier);
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
    }
}
