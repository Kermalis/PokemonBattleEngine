using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Utils
{
    public static class PBERandom
    {
        private static readonly object _randLockObj = new object();
        private static Random _rand;

        internal static void Init()
        {
            _rand = new Random();
        }
        internal static void Init(int seed)
        {
            _rand = new Random(seed);
        }

        public static PBEBattleTerrain RandomBattleTerrain()
        {
            return (PBEBattleTerrain)RandomInt(0, (int)PBEBattleTerrain.MAX - 1);
        }
        internal static bool RandomBool()
        {
            return RandomInt(0, 1) == 1;
        }
        internal static bool RandomBool(int chanceNumerator, int chanceDenominator)
        {
            return RandomInt(0, chanceDenominator - 1) < chanceNumerator;
        }
        internal static T RandomElement<T>(this IReadOnlyList<T> source)
        {
            int count = source.Count - 1;
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(source), $"\"{nameof(source)}\" must have at least one element.");
            }
            return source[RandomInt(0, count)];
        }
        /// <summary>Returns a random <see cref="PBEGender"/> for the given <paramref name="genderRatio"/>.</summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="genderRatio"/> is invalid.</exception>
        public static PBEGender RandomGender(PBEGenderRatio genderRatio)
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
        /// <summary>Returns a random <see cref="int"/> value between the inclusive <paramref name="minValue"/> and inclusive <paramref name="maxValue"/>.</summary>
        internal static int RandomInt(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), $"\"{nameof(minValue)}\" cannot exceed \"{nameof(maxValue)}\".");
            }
            uint scale = uint.MaxValue;
            byte[] bytes = new byte[sizeof(uint)];
            while (scale == uint.MaxValue) // "d" should not be 1.0
            {
                lock (_randLockObj)
                {
                    _rand.NextBytes(bytes);
                }
                scale = BitConverter.ToUInt32(bytes, 0);
            }
            double d = scale / (double)uint.MaxValue;
            return (int)(minValue + (((long)maxValue + 1 - minValue) * d)); // Remove "+ 1" for exclusive maxValue
        }
        /// <summary>Returns a random <see cref="byte"/> value that is between <paramref name="settings"/>'s <see cref="PBESettings.MinLevel"/> and <see cref="PBESettings.MaxLevel"/>.</summary>
        /// <param name="settings">The <see cref="PBESettings"/> object to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="settings"/> == null.</exception>
        public static byte RandomLevel(PBESettings settings)
        {
            if (settings == null)
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
        public static bool RandomShiny()
        {
            return RandomBool(8, 65536);
        }
        /// <summary>Returns a random <see cref="PBESpecies"/> with a random <see cref="PBEForm"/>.</summary>
        public static (PBESpecies, PBEForm) RandomSpecies(bool requireUsableOutsideOfBattle)
        {
            return PBEDataUtils.AllSpecies.RandomSpecies(requireUsableOutsideOfBattle);
        }
        public static (PBESpecies, PBEForm) RandomSpecies(this IReadOnlyList<PBESpecies> eligible, bool requireUsableOutsideOfBattle)
        {
            PBESpecies species = eligible.RandomElement();
            IReadOnlyList<PBEForm> forms = PBEDataUtils.GetForms(species, requireUsableOutsideOfBattle);
            PBEForm form = forms.Count > 0 ? forms.RandomElement() : 0;
            return (species, form);
        }
        public static void SetSeed(int seed)
        {
            lock (_randLockObj)
            {
                _rand = new Random(seed);
            }
        }
        /// <summary>Shuffles the items in <paramref name="source"/> using the Fisher-Yates Shuffle algorithm.</summary>
        internal static void Shuffle<T>(this IList<T> source)
        {
            int count = source.Count - 1;
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
