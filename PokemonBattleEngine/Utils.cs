using Kermalis.PokemonBattleEngine.Data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private static readonly Random rand = new Random();
        public static PBEPokemonShell[] CreateCompletelyRandomTeam(PBESettings settings, bool setToMaxLevel)
        {
            var team = new PBEPokemonShell[settings.MaxPartySize];
            for (int i = 0; i < settings.MaxPartySize; i++)
            {
                team[i] = new PBEPokemonShell(RandomSpecies(), setToMaxLevel ? settings.MaxLevel : (byte)rand.Next(settings.MinLevel, settings.MaxLevel + 1), settings);
            }
            return team;
        }
        internal static bool RandomBool()
        {
            return rand.NextDouble() < 0.5;
        }
        internal static bool RandomBool(int chanceNumerator, int chanceDenominator)
        {
            return rand.Next(chanceDenominator) < chanceNumerator;
        }
        public static T RandomElement<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(rand.Next(0, source.Count()));
        }
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
        internal static int RandomInt(int minValue, int maxValue)
        {
            return rand.Next(minValue, maxValue + 1);
        }
        public static bool RandomShiny()
        {
            return RandomBool(8, 65536);
        }
        public static PBESpecies RandomSpecies()
        {
            IEnumerable<PBESpecies> allSpecies = PBEPokemonShell.AllSpecies.Where(s => ((uint)s >> 0x10) == 0); // All species with form ID 0
            PBESpecies species = allSpecies.RandomElement();
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
            return (PBESpecies)(((ushort)species) | (uint)(rand.Next(numForms) << 0x10)); // Change form ID to a random form
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
        /// <summary>Shuffles the items in a list using the Fisher-Yates Shuffle algorithm.</summary>
        /// <param name="source">The list to shuffle.</param>
        internal static void Shuffle<T>(this IList<T> source)
        {
            for (int a = 0; a < source.Count - 1; a++)
            {
                int b = rand.Next(a, source.Count);
                T value = source[a];
                source[a] = source[b];
                source[b] = value;
            }
        }
        /// <summary>Takes a string and removes all invalid file name characters from it.</summary>
        /// <param name="fileName">The string to clean.</param>
        internal static string ToSafeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }
            return fileName;
        }

        internal static byte[] StringToBytes(string str)
        {
            var bytes = new List<byte>();
            byte[] nameBytes = Encoding.Unicode.GetBytes(str);
            bytes.Add((byte)nameBytes.Length);
            bytes.AddRange(nameBytes);
            return bytes.ToArray();
        }
        internal static string StringFromBytes(BinaryReader r)
        {
            return Encoding.Unicode.GetString(r.ReadBytes(r.ReadByte()));
        }
    }
}
