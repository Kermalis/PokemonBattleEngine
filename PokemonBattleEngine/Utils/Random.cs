using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Utils
{
    public static class PBERandom
    {
        private static readonly object _randLockObj = new object();
        private static Random _rand = new Random();

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
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="settings"/> is null.</exception>
        public static byte RandomLevel(PBESettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            return (byte)RandomInt(settings.MinLevel, settings.MaxLevel);
        }
        /// <summary>Returns a random <see cref="bool"/> value that represents shininess using shiny odds.</summary>
        public static bool RandomShiny()
        {
            return RandomBool(8, 65536);
        }
        /// <summary>Returns a random <see cref="PBESpecies"/> with a random form. All species are weighted equally. Forms that cannot be maintained outside of battle are not considered.</summary>
        public static PBESpecies RandomSpecies()
        {
            PBESpecies species = PBEPokemonShell.AllSpeciesBaseForm.RandomElement();
            int numForms;
            switch (species)
            {
                case PBESpecies.Arceus: numForms = 17; break;
                case PBESpecies.Basculin_Blue: numForms = 2; break;
                case PBESpecies.Burmy_Plant: numForms = 3; break;
                // Castform's alternate forms cannot be used outside of battle
                // Cherrims's alternate form cannot be used outside of battle
                // Darmanitan's alternate form cannot be used outside of battle
                case PBESpecies.Deerling_Autumn: numForms = 4; break;
                case PBESpecies.Deoxys: numForms = 4; break;
                case PBESpecies.Gastrodon_East: numForms = 2; break;
                case PBESpecies.Genesect: numForms = 5; break;
                case PBESpecies.Giratina: numForms = 2; break;
                case PBESpecies.Keldeo: numForms = 2; break;
                case PBESpecies.Kyurem: numForms = 3; break;
                case PBESpecies.Landorus: numForms = 2; break;
                // Meloetta's alternate form cannot be used outside of battle
                case PBESpecies.Rotom: numForms = 6; break;
                case PBESpecies.Sawsbuck_Autumn: numForms = 4; break;
                case PBESpecies.Shaymin: numForms = 2; break;
                case PBESpecies.Shellos_East: numForms = 2; break;
                case PBESpecies.Thundurus: numForms = 2; break;
                case PBESpecies.Tornadus: numForms = 2; break;
                case PBESpecies.Unown_A: numForms = 28; break;
                case PBESpecies.Wormadam_Plant: numForms = 3; break;
                default: numForms = 1; break;
            }
            return (PBESpecies)(((ushort)species) | (uint)(RandomInt(0, numForms - 1) << 0x10)); // Change form ID to a random form
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
