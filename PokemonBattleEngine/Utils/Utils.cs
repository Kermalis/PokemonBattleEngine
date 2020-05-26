using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Kermalis.PokemonBattleEngine.Utils
{
    /// <summary>A static class that provides utilities that are used throughout the battle engine.</summary>
    public static class PBEUtils
    {
        private static readonly object _databaseConnectLockObj = new object();
        private static SqliteConnection _databaseConnection;
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
        /// <param name="databasePath">The path of the folder containing PokemonBattleEngine.db.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="databasePath"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when a database connection has already been created.</exception>
        public static void CreateDatabaseConnection(string databasePath)
        {
            if (databasePath == null)
            {
                throw new ArgumentNullException(nameof(databasePath));
            }
            else if (_databaseConnection != null)
            {
                throw new InvalidOperationException("Database connection was already created.");
            }
            else
            {
                SQLitePCL.Batteries_V2.Init();
                _databaseConnection = new SqliteConnection($"Filename={Path.Combine(databasePath, "PokemonBattleEngine.db")};Mode=ReadOnly;");
                _databaseConnection.Open();
                _databaseConnection.CreateFunction("StrCmp", (Func<object, object, bool>)StrCmp);
            }
        }

        /// <summary>Returns a <see cref="string"/> that combines <paramref name="source"/>'s elements' string representations using "and" with commas.</summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IReadOnlyList{T}"/> to create a string from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
        public static string Andify<T>(this IReadOnlyList<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            string str = source[0].ToString();
            for (int i = 1; i < source.Count; i++)
            {
                if (i == source.Count - 1)
                {
                    if (source.Count > 2)
                    {
                        str += ',';
                    }
                    str += " and ";
                }
                else
                {
                    str += ", ";
                }
                str += source[i].ToString();
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
            if (_databaseConnection == null)
            {
                throw new Exception($"You must first call \"{nameof(PBEUtils)}.{nameof(CreateDatabaseConnection)}()\"");
            }
            var list = new List<T>();
            Type type = typeof(T);
            lock (_databaseConnectLockObj)
            {
                using (SqliteCommand cmd = _databaseConnection.CreateCommand())
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
            }
            return list;
        }
        /// <summary>Removes all invalid file name characters from <paramref name="fileName"/>.</summary>
        internal static string ToSafeFileName(string fileName)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            for (int i = 0; i < invalid.Length; i++)
            {
                fileName = fileName.Replace(invalid[i], '-');
            }
            return fileName;
        }
    }
}
