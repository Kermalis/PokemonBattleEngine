using Kermalis.PokemonBattleEngine.Utils;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Kermalis.PokemonBattleEngine.Data
{
    public class PBEDataProvider
    {
        public static PBEDataProvider Instance { get; private set; }

        private static readonly object _databaseConnectLockObj = new object();
        private static SqliteConnection _databaseConnection;
        public static CultureInfo PBECulture { get; private set; }
        public static PBERandom GlobalRandom { get; private set; }

        protected PBEDataProvider() { }

        #region Static
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
        private static void InitDB(string databasePath)
        {
            if (databasePath == null)
            {
                throw new ArgumentNullException(nameof(databasePath));
            }
            if (_databaseConnection != null)
            {
                throw new InvalidOperationException("Engine already initialized.");
            }
            SQLitePCL.Batteries_V2.Init();
            _databaseConnection = new SqliteConnection($"Filename={Path.Combine(databasePath, "PokemonBattleEngine.db")};Mode=ReadOnly;");
            _databaseConnection.Open();
            _databaseConnection.CreateFunction("StrCmp", (Func<object, object, bool>)StrCmp);
        }
        public static void InitEngine(string databasePath, int? randomSeed = null, PBEDataProvider dataProvider = null)
        {
            InitDB(databasePath);
            var cultureInfo = CultureInfo.ReadOnly(CultureInfo.CurrentUICulture);
            PBECulture = PBELocalizedString.IsCultureValid(cultureInfo) ? cultureInfo : CultureInfo.GetCultureInfo("en-US");
            GlobalRandom = new PBERandom(randomSeed);
            Instance = dataProvider ?? new PBEDataProvider();
        }
        public static void InitEngine(string databasePath, CultureInfo cultureInfo, int? randomSeed = null, PBEDataProvider dataProvider = null)
        {
            if (cultureInfo is null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }
            InitDB(databasePath);
            cultureInfo = CultureInfo.ReadOnly(cultureInfo);
            PBECulture = PBELocalizedString.IsCultureValid(cultureInfo) ? cultureInfo : throw new ArgumentOutOfRangeException(nameof(cultureInfo));
            GlobalRandom = new PBERandom(randomSeed);
            Instance = dataProvider ?? new PBEDataProvider();
        }

        // TODO: Keep this internal version and make a public version that only allows operations that retrieve data
        internal static List<T> QueryDatabase<T>(string commandText) where T : new()
        {
            if (_databaseConnection is null)
            {
                throw new Exception($"You must first call \"{nameof(PBEDataProvider)}.{nameof(InitEngine)}()\"");
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
        #endregion

        public bool IsBerry(PBEItem item)
        {
            return PBEBerryData.Data.ContainsKey(item);
        }

        public IPBEBerryData GetBerryData(PBEItem item, bool cache = true)
        {
            return PBEBerryData.Data[item];
        }
        public bool TryGetBerryData(PBEItem item, out IPBEBerryData bData, bool cache = true)
        {
            if (IsBerry(item))
            {
                bData = GetBerryData(item, cache);
                return true;
            }
            bData = default;
            return false;
        }
        public IPBEItemData GetItemData(PBEItem item, bool cache = true)
        {
            return PBEItemData.Data[item];
        }
        public IPBEMoveData GetMoveData(PBEMove move, bool cache = true)
        {
            return PBEMoveData.Data[move];
        }
        public IPBEPokemonData GetPokemonData(IPBESpeciesForm pkmn, bool cache = true)
        {
            return GetPokemonData(pkmn.Species, pkmn.Form, cache);
        }
        public IPBEPokemonData GetPokemonData(PBESpecies species, PBEForm form, bool cache = true)
        {
            return PBEPokemonData.GetData(species, form, cache);
        }

        #region LocalizedString
        public PBEAbility? GetAbilityByName(string abilityName)
        {
            return PBELocalizedString.GetAbilityByName(abilityName);
        }
        public IPBELocalizedString GetAbilityDescription(PBEAbility ability)
        {
            return PBELocalizedString.GetAbilityDescription(ability);
        }
        public IPBELocalizedString GetAbilityName(PBEAbility ability)
        {
            return PBELocalizedString.GetAbilityName(ability);
        }
        public PBEForm? GetFormByName(PBESpecies species, string formName)
        {
            return PBELocalizedString.GetFormByName(species, formName);
        }
        public IPBELocalizedString GetFormName(IPBESpeciesForm pkmn)
        {
            return GetFormName(pkmn.Species, pkmn.Form);
        }
        public IPBELocalizedString GetFormName(PBESpecies species, PBEForm form)
        {
            return PBELocalizedString.GetFormName(species, form);
        }
        public PBEGender? GetGenderByName(string genderName)
        {
            return PBELocalizedString.GetGenderByName(genderName);
        }
        public IPBELocalizedString GetGenderName(PBEGender gender)
        {
            return PBELocalizedString.GetGenderName(gender);
        }
        public PBEItem? GetItemByName(string itemName)
        {
            return PBELocalizedString.GetItemByName(itemName);
        }
        public IPBELocalizedString GetItemDescription(PBEItem item)
        {
            return PBELocalizedString.GetItemDescription(item);
        }
        public IPBELocalizedString GetItemName(PBEItem item)
        {
            return PBELocalizedString.GetItemName(item);
        }
        public PBEMove? GetMoveByName(string moveName)
        {
            return PBELocalizedString.GetMoveByName(moveName);
        }
        public IPBELocalizedString GetMoveDescription(PBEMove move)
        {
            return PBELocalizedString.GetMoveDescription(move);
        }
        public IPBELocalizedString GetMoveName(PBEMove move)
        {
            return PBELocalizedString.GetMoveName(move);
        }
        public PBENature? GetNatureByName(string natureName)
        {
            return PBELocalizedString.GetNatureByName(natureName);
        }
        public IPBELocalizedString GetNatureName(PBENature nature)
        {
            return PBELocalizedString.GetNatureName(nature);
        }
        public PBESpecies? GetSpeciesByName(string speciesName)
        {
            return PBELocalizedString.GetSpeciesByName(speciesName);
        }
        public IPBELocalizedString GetSpeciesCategory(PBESpecies species)
        {
            return PBELocalizedString.GetSpeciesCategory(species);
        }
        public IPBELocalizedString GetSpeciesEntry(PBESpecies species)
        {
            return PBELocalizedString.GetSpeciesEntry(species);
        }
        public IPBELocalizedString GetSpeciesName(PBESpecies species)
        {
            return PBELocalizedString.GetSpeciesName(species);
        }
        public PBEStat? GetStatByName(string statName)
        {
            return PBELocalizedString.GetStatByName(statName);
        }
        public IPBELocalizedString GetStatName(PBEStat stat)
        {
            return PBELocalizedString.GetStatName(stat);
        }
        public PBEType? GetTypeByName(string typeName)
        {
            return PBELocalizedString.GetTypeByName(typeName);
        }
        public IPBELocalizedString GetTypeName(PBEType type)
        {
            return PBELocalizedString.GetTypeName(type);
        }
        #endregion
    }
}
