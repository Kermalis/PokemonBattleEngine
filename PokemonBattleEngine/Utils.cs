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
    /// <summary>
    /// Contains utilities used in the battle engine.
    /// </summary>
    public static class PBEUtils
    {
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
            SQLitePCL.Batteries_V2.Init();
            databaseConnection = new SqliteConnection($"Filename={Path.Combine(databasePath, "PokemonBattleEngine.db")};Mode=ReadOnly;");
            databaseConnection.Open();
            databaseConnection.CreateFunction("StrCmp", (Func<object, object, bool>)StrCmp);
        }

        private static SqliteConnection databaseConnection;
        public static SqliteConnection DatabaseConnection => databaseConnection ?? throw new Exception($"You must first call {nameof(CreateDatabaseConnection)}.");

        /// <summary>
        /// An ordinary pseudo-random number generator.
        /// </summary>
        public static Random RNG { get; } = new Random();

        /// <summary>
        /// Returns a random boolean from a chance.
        /// </summary>
        /// <example><paramref name="chanceNumerator"/> is 30, <paramref name="chanceDenominator"/> is 100, there is a 30% chance to return True and a 70% chance to return False.</example>
        /// <param name="rand">The number generator to use.</param>
        /// <param name="chanceNumerator">The numerator of the chance.</param>
        /// <param name="chanceDenominator">The denominator of the chance.</param>
        public static bool ApplyChance(this Random rand, int chanceNumerator, int chanceDenominator)
        {
            return rand.Next(chanceDenominator) < chanceNumerator;
        }
        /// <summary>
        /// Returns a random boolean.
        /// </summary>
        /// <param name="rand">The number generator to use.</param>
        public static bool NextBoolean(this Random rand)
        {
            return rand.NextDouble() >= 0.5;
        }

        public static bool NextShiny(this Random rand)
        {
            return rand.ApplyChance(8, ushort.MaxValue + 1);
        }
        public static PBEGender NextGender(this Random rand, PBEGenderRatio genderRatio)
        {
            switch (genderRatio)
            {
                case PBEGenderRatio.M7_F1: return rand.ApplyChance(875, 1000) ? PBEGender.Male : PBEGender.Female;
                case PBEGenderRatio.M3_F1: return rand.ApplyChance(750, 1000) ? PBEGender.Male : PBEGender.Female;
                case PBEGenderRatio.M1_F1: return rand.ApplyChance(500, 1000) ? PBEGender.Male : PBEGender.Female;
                case PBEGenderRatio.M1_F3: return rand.ApplyChance(250, 1000) ? PBEGender.Male : PBEGender.Female;
                case PBEGenderRatio.M0_F1: return PBEGender.Female;
                case PBEGenderRatio.M0_F0: return PBEGender.Genderless;
                case PBEGenderRatio.M1_F0: return PBEGender.Male;
                default: throw new ArgumentOutOfRangeException(nameof(genderRatio));
            }
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
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
        public static List<T> Query<T>(this SqliteConnection databaseConnection, string commandText) where T : new()
        {
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
        public static string Print<T>(this IEnumerable<T> source)
        {
            return "( " + string.Join(", ", source) + " )";
        }
        public static T Sample<T>(this IEnumerable<T> source)
        {
            return source.ElementAt(RNG.Next(0, source.Count()));
        }
        /// <summary>
        /// Shuffles the items in a list using the Fisher-Yates Shuffle algorithm.
        /// </summary>
        /// <param name="source">The list to shuffle.</param>
        public static void Shuffle<T>(this IList<T> source)
        {
            for (int a = 0; a < source.Count - 1; a++)
            {
                int b = RNG.Next(a, source.Count);
                T value = source[a];
                source[a] = source[b];
                source[b] = value;
            }
        }
        public static bool DoesNamespaceExist(string @namespace)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                TypeInfo[] array = assembly.DefinedTypes.ToArray();
                if (array.Length > 0 && array[0].Namespace == @namespace)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Takes a string and removes all invalid file name characters from it.
        /// </summary>
        /// <param name="fileName">The string to clean.</param>
        public static string ToSafeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }
            return fileName;
        }

        public static PBEPokemonShell[] CreateCompletelyRandomTeam(PBESettings settings)
        {
            var team = new PBEPokemonShell[settings.MaxPartySize];
            IEnumerable<PBESpecies> allSpecies = Enum.GetValues(typeof(PBESpecies)).Cast<PBESpecies>().Where(s => ((uint)s >> 0x10) == 0); // All species with form ID 0
            for (int i = 0; i < settings.MaxPartySize; i++)
            {
                PBESpecies species = allSpecies.Sample();
                int numForms;
                switch (species)
                {
                    case PBESpecies.Basculin_Blue: numForms = 2; break;
                    case PBESpecies.Burmy_Plant: numForms = 3; break;
                    case PBESpecies.Deerling_Autumn: numForms = 4; break;
                    case PBESpecies.Deoxys: numForms = 4; break;
                    case PBESpecies.Gastrodon_East: numForms = 2; break;
                    case PBESpecies.Kyurem: numForms = 3; break;
                    case PBESpecies.Landorus: numForms = 2; break;
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
                species = (PBESpecies)(((ushort)species) | (uint)(RNG.Next(numForms) << 0x10)); // Change form ID to a random form
                var pData = PBEPokemonData.GetData(species);
                var shell = new PBEPokemonShell
                {
                    Species = species,
                    Ability = pData.Abilities.Sample(),
                    Gender = RNG.NextGender(pData.GenderRatio),
                    Level = settings.MaxLevel,
                    Friendship = (byte)RNG.Next(byte.MaxValue + 1),
                    Nature = (PBENature)RNG.Next((int)PBENature.MAX),
                    Nickname = PBELocalizedString.GetSpeciesName(species).FromUICultureInfo(),
                    Shiny = RNG.NextShiny(),
                    EVs = new byte[6],
                    IVs = new byte[6],
                    PPUps = new byte[settings.NumMoves],
                    Moves = new PBEMove[settings.NumMoves],
                    Item = PBEItem.None
                };
                for (int j = 0; j < 6; j++)
                {
                    shell.IVs[j] = (byte)RNG.Next(settings.MaxIVs + 1);
                }
                var legalMoves = PBELegalityChecker.GetLegalMoves(species, shell.Level).ToList();
                for (int j = 0; j < settings.NumMoves; j++)
                {
                    if (legalMoves.Count == 0)
                    {
                        break;
                    }
                    PBEMove move = legalMoves.Sample();
                    shell.Moves[j] = move;
                    legalMoves.Remove(move);
                }
                team[i] = shell;
            }
            return team;
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
