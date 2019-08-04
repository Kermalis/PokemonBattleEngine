using Kermalis.PokemonBattleEngine.Data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Kermalis.PokemonBattleEngine
{
    /// <summary>A static class that provides utilities that are used throughout the battle engine.</summary>
    public static class PBEUtils
    {
        private static SqliteConnection databaseConnection;
        private static bool StrCmp(object arg0, object arg1)
        {
            if (Convert.IsDBNull(arg0) || Convert.IsDBNull(arg1))
            {
                return false;
            }
            else
            {
                return Convert.ToString(arg0).Equals(Convert.ToString(arg1), StringComparison.InvariantCultureIgnoreCase);
            }
        }
        /// <summary>Creates a connection to PokemonBattleEngine.db. This must be called only once; before the database is used.</summary>
        /// <param name="databasePath">The path to the folder containing PokemonBattleEngine.db.</param>
        public static void CreateDatabaseConnection(string databasePath)
        {
            if (databaseConnection != null)
            {
                throw new Exception("Database connection was already created.");
            }
            else
            {
                SQLitePCL.Batteries_V2.Init();
                databaseConnection = new SqliteConnection($"Filename={Path.Combine(databasePath, "PokemonBattleEngine.db")};Mode=ReadOnly;");
                databaseConnection.Open();
                databaseConnection.CreateFunction("StrCmp", (Func<object, object, bool>)StrCmp);
            }
        }

        /// <summary>
        /// When I used <see cref="Random"/>, it would only take 55 consecutive calls to <see cref="RandomElement{T}(IEnumerable{T})"/> with a collection of ints to be able to predict future values.
        /// You would also be able to predict with more work if you called <see cref="RandomSpecies"/> or <see cref="RandomGender(PBEGenderRatio)"/>.
        /// I do not see how that would help attackers because the host would be able to modify the game however it desires anyway.
        /// For these reasons, I decided to have the random functions use the private <see cref="Random"/> without the need to pass one in as a parameter.
        /// Although <see cref="RNGCryptoServiceProvider"/> is slower and does not allow seeds for obvious reasons, I did not use seeds with the <see cref="Random"/> implementation, and Pokémon battles are turn based so the speed doesn't hurt much.
        /// I decided to switch from <see cref="Random"/> to <see cref="RNGCryptoServiceProvider"/> because I did not need the better speed or the seeded constructor and because <see cref="RNGCryptoServiceProvider"/> provides better random outputs.
        /// </summary>
        private static readonly RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
        internal static bool RandomBool()
        {
            return RandomInt(0, 1) == 1;
        }
        internal static bool RandomBool(int chanceNumerator, int chanceDenominator)
        {
            return RandomInt(0, chanceDenominator - 1) < chanceNumerator;
        }
        internal static T RandomElement<T>(this IList<T> source)
        {
            int count = source.Count - 1;
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(source), $"\"{nameof(source)}\" must have at least one element.");
            }
            return source[RandomInt(0, count)];
        }
        /// <summary>Returns a random <see cref="PBEGender"/> for the given <paramref name="genderRatio"/>.</summary>
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
        /// <summary>Returns a random int between the inclusive <paramref name="minValue"/> and inclusive <paramref name="maxValue"/>.</summary>
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
                rand.GetBytes(bytes);
                scale = BitConverter.ToUInt32(bytes, 0);
            }
            double d = scale / (double)uint.MaxValue;
            return (int)(minValue + (((long)maxValue + 1 - minValue) * d)); // Remove "+ 1" for exclusive maxValue
        }
        public static byte RandomLevel(PBESettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            return (byte)RandomInt(settings.MinLevel, settings.MaxLevel);
        }
        /// <summary>Returns a random shiny value.</summary>
        public static bool RandomShiny()
        {
            return RandomBool(8, 65536);
        }
        /// <summary>Returns a random <see cref="PBESpecies"/> with a random form. All species are weighted equally. Forms that cannot be maintained outside of battle are not considered.</summary>
        public static PBESpecies RandomSpecies()
        {
            PBESpecies[] allSpecies = PBEPokemonShell.AllSpecies.Where(s => ((uint)s >> 0x10) == 0).ToArray(); // All species with form ID 0
            PBESpecies species = RandomElement(allSpecies);
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

        public static string Andify<T>(this IEnumerable<T> source)
        {
            T[] array = source.ToArray();
            string str = array[0].ToString();
            for (int i = 1; i < array.Length; i++)
            {
                if (i == array.Length - 1)
                {
                    if (array.Length > 2)
                    {
                        str += ',';
                    }
                    str += " and ";
                }
                else
                {
                    str += ", ";
                }
                str += array[i].ToString();
            }
            return str;
        }
        internal static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
            {
                return min;
            }
            else if (val.CompareTo(max) > 0)
            {
                return max;
            }
            else
            {
                return val;
            }
        }
        internal static string Print<T>(this IEnumerable<T> source)
        {
            return "( " + string.Join(", ", source) + " )";
        }
        // TODO: Keep this internal version and make a public version that only allows operations that retrieve data
        internal static List<T> QueryDatabase<T>(string commandText) where T : new()
        {
            if (databaseConnection == null)
            {
                throw new Exception($"You must first call \"{nameof(PBEUtils)}.{nameof(CreateDatabaseConnection)}()\"");
            }
            var list = new List<T>();
            Type type = typeof(T);
            using (SqliteCommand cmd = databaseConnection.CreateCommand())
            {
                cmd.CommandText = commandText;
                using (SqliteDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        T obj = Activator.CreateInstance<T>();
                        for (int i = 0; i < r.FieldCount; i++)
                        {
                            PropertyInfo property = type.GetProperty(r.GetName(i));
                            if (property != null)
                            {
                                property.SetValue(obj, Convert.ChangeType(r.GetValue(i), property.PropertyType));
                            }
                        }
                        list.Add(obj);
                    }
                }
            }
            return list;
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
        /// <summary>Removes all invalid file name characters from <paramref name="fileName"/>.</summary>
        internal static string ToSafeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }
            return fileName;
        }

        internal static List<byte> StringToBytes(string str)
        {
            var bytes = new List<byte>();
            byte[] nameBytes = Encoding.Unicode.GetBytes(str);
            bytes.Add((byte)nameBytes.Length);
            bytes.AddRange(nameBytes);
            return bytes;
        }
        internal static string StringFromBytes(BinaryReader r)
        {
            return Encoding.Unicode.GetString(r.ReadBytes(r.ReadByte()));
        }
    }
}
