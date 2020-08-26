using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Utils
{
    public class PBERandom
    {
        private readonly object _randLockObj = new object();
        private Random _rand;
        private int _seed;
        /// <summary>Gets or sets the seed of this <see cref="PBERandom"/>. The chain will be reset even if the seed is the same as the previous seed.</summary>
        public int Seed
        {
            get => _seed;
            set
            {
                lock (_randLockObj)
                {
                    _rand = new Random(value);
                    _seed = value;
                }
            }
        }

        public PBERandom() : this(Environment.TickCount) { }
        public PBERandom(int? seed) : this(seed ?? Environment.TickCount) { }
        public PBERandom(int seed)
        {
            _rand = new Random(seed);
            _seed = seed;
        }

        public PBEBattleTerrain RandomBattleTerrain()
        {
            return (PBEBattleTerrain)RandomInt(0, (int)PBEBattleTerrain.MAX - 1);
        }
        public bool RandomBool()
        {
            return RandomInt(0, 1) == 1;
        }
        public bool RandomBool(int chanceNumerator, int chanceDenominator)
        {
            if (chanceDenominator < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(chanceDenominator), $"\"{nameof(chanceDenominator)}\" must be at least 1.");
            }
            if (chanceNumerator >= chanceDenominator)
            {
                return true;
            }
            if (chanceNumerator == 0)
            {
                return false;
            }
            return RandomInt(0, chanceDenominator - 1) < chanceNumerator;
        }
        public T RandomElement<T>(IReadOnlyList<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            int count = source.Count - 1;
            if (count == 0)
            {
                return source[0];
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(source), $"\"{nameof(source)}\" must have at least one element.");
            }
            return source[RandomInt(0, count)];
        }
        /// <summary>Returns a random <see cref="PBEGender"/> for the given <paramref name="genderRatio"/>.</summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="genderRatio"/> is invalid.</exception>
        public PBEGender RandomGender(PBEGenderRatio genderRatio)
        {
            switch (genderRatio)
            {
                case PBEGenderRatio.M7_F1: return RandomBool(875, 1000) ? PBEGender.Male : PBEGender.Female;
                case PBEGenderRatio.M3_F1: return RandomBool(750, 1000) ? PBEGender.Male : PBEGender.Female;
                case PBEGenderRatio.M1_F1: return RandomBool(500, 1000) ? PBEGender.Male : PBEGender.Female;
                case PBEGenderRatio.M1_F3: return RandomBool(250, 1000) ? PBEGender.Male : PBEGender.Female;
                case PBEGenderRatio.M0_F1: return PBEGender.Female;
                case PBEGenderRatio.M0_F0: return PBEGender.Genderless;
                case PBEGenderRatio.M1_F0: return PBEGender.Male;
                default: throw new ArgumentOutOfRangeException(nameof(genderRatio));
            }
        }
        public int RandomInt()
        {
            return RandomInt(int.MinValue, int.MaxValue);
        }
        /// <summary>Returns a random <see cref="int"/> value between the inclusive <paramref name="minValue"/> and inclusive <paramref name="maxValue"/>.</summary>
        public int RandomInt(int minValue, int maxValue)
        {
            if (minValue == maxValue)
            {
                return minValue;
            }
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), $"\"{nameof(minValue)}\" cannot exceed \"{nameof(maxValue)}\".");
            }
            byte[] bytes = new byte[sizeof(uint)];
            uint scale;
            do
            {
                lock (_randLockObj)
                {
                    _rand.NextBytes(bytes);
                }
                scale = (uint)((bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0]); // Convert as little endian each time regardless of system endianness
            } while (scale == uint.MaxValue); // "d" should not be 1.0
            double d = scale / (double)uint.MaxValue;
            return (int)(minValue + (((long)maxValue + 1 - minValue) * d)); // Remove "+ 1" for exclusive maxValue
        }
        /// <summary>Returns a random <see cref="byte"/> value that is between <paramref name="settings"/>'s <see cref="PBESettings.MinLevel"/> and <see cref="PBESettings.MaxLevel"/>.</summary>
        /// <param name="settings">The <see cref="PBESettings"/> object to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="settings"/> == null.</exception>
        public byte RandomLevel(PBESettings settings)
        {
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            return (byte)RandomInt(settings.MinLevel, settings.MaxLevel);
        }
        /// <summary>Returns a random <see cref="bool"/> value that represents shininess using shiny odds.</summary>
        public bool RandomShiny()
        {
            return RandomBool(8, 65536);
        }
        /// <summary>Returns a random <see cref="PBESpecies"/> with a random <see cref="PBEForm"/>.</summary>
        public (PBESpecies, PBEForm) RandomSpecies(bool requireUsableOutsideOfBattle)
        {
            return RandomSpecies(PBEDataUtils.AllSpecies, requireUsableOutsideOfBattle);
        }
        public (PBESpecies, PBEForm) RandomSpecies(IReadOnlyList<PBESpecies> eligible, bool requireUsableOutsideOfBattle)
        {
            PBESpecies species = RandomElement(eligible);
            IReadOnlyList<PBEForm> forms = PBEDataUtils.GetForms(species, requireUsableOutsideOfBattle);
            PBEForm form = forms.Count > 0 ? RandomElement(forms) : 0;
            return (species, form);
        }
        /// <summary>Shuffles the items in <paramref name="source"/> using the Fisher-Yates Shuffle algorithm.</summary>
        public void Shuffle<T>(IList<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            int count = source.Count - 1;
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(source), $"\"{nameof(source)}\" must have at least one element.");
            }
            for (int a = 0; a < count; a++)
            {
                int b = RandomInt(a, count);
                T value = source[a];
                source[a] = source[b];
                source[b] = value;
            }
        }
    }
}
