using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Utils;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Kermalis.PokemonBattleEngine.Data.DefaultData
{
    public class PBEDefaultDataProvider : PBEDataProvider
    {
        public static new PBEDefaultDataProvider Instance => (PBEDefaultDataProvider)PBEDataProvider.Instance;

        private readonly object _databaseConnectLockObj = new();
        private readonly SqliteConnection _databaseConnection;

        private PBEDefaultDataProvider(string databasePath)
        {
            SQLitePCL.Batteries_V2.Init();
            _databaseConnection = new SqliteConnection($"Filename={Path.Combine(databasePath, "PokemonBattleEngine.db")};Mode=ReadOnly;");
            _databaseConnection.Open();
            _databaseConnection.CreateFunction("StrCmp", (Func<object, object, bool>)StrCmp);
        }
        public static void InitEngine(string databasePath, int? randomSeed = null)
        {
            var dp = new PBEDefaultDataProvider(databasePath);
            var cultureInfo = CultureInfo.ReadOnly(CultureInfo.CurrentUICulture);
            PBELanguage lang = cultureInfo.IsValidPBELanguage() ? cultureInfo.ToPBELanguage() : PBELanguage.English;
            dp.Init(lang, new PBERandom(randomSeed));
        }
        public static void InitEngine(string databasePath, PBELanguage language, int? randomSeed = null)
        {
            if (language >= PBELanguage.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(language));
            }
            var dp = new PBEDefaultDataProvider(databasePath);
            dp.Init(language, new PBERandom(randomSeed));
        }

        private static bool StrCmp(object arg0, object arg1)
        {
            if (Convert.IsDBNull(arg0) || Convert.IsDBNull(arg1))
            {
                return false;
            }
            return string.Equals(Convert.ToString(arg0), Convert.ToString(arg1), StringComparison.InvariantCultureIgnoreCase);
        }

        // TODO: Keep this internal version and make a public version that only allows operations that retrieve data?
        internal List<T> QueryDatabase<T>(string commandText) where T : new()
        {
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
                                PropertyInfo? property = type.GetProperty(r.GetName(i));
                                if (property is not null)
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

        #region Data

        public override bool IsBerry(PBEItem item)
        {
            return PBEBerryData.Data.ContainsKey(item);
        }
        public override IPBEBerryData GetBerryData(PBEItem item, bool cache = true)
        {
            return PBEBerryData.Data[item];
        }
        public override bool TryGetBerryData(PBEItem item, [NotNullWhen(true)] out IPBEBerryData? bData, bool cache = true)
        {
            if (IsBerry(item))
            {
                bData = GetBerryData(item, cache);
                return true;
            }
            bData = null;
            return false;
        }
        public override IPBEItemData GetItemData(PBEItem item, bool cache = true)
        {
            return PBEItemData.Data[item];
        }
        public override IPBEMoveData GetMoveData(PBEMove move, bool cache = true)
        {
            return PBEMoveData.Data[move];
        }
        public override bool HasEvolutions(IPBESpeciesForm pkmn, bool cache = true)
        {
            return HasEvolutions(pkmn.Species, pkmn.Form, cache);
        }
        public override bool HasEvolutions(PBESpecies species, PBEForm form, bool cache = true)
        {
            return GetPokemonDataExtended(species, form, cache).HasEvolutions();
        }
        public override IPBEPokemonData GetPokemonData(IPBESpeciesForm pkmn, bool cache = true)
        {
            return GetPokemonData(pkmn.Species, pkmn.Form, cache);
        }
        public override IPBEPokemonData GetPokemonData(PBESpecies species, PBEForm form, bool cache = true)
        {
            return GetPokemonDataExtended(species, form, cache);
        }
        public override IPBEPokemonDataExtended GetPokemonDataExtended(IPBESpeciesForm pkmn, bool cache = true)
        {
            return GetPokemonDataExtended(pkmn.Species, pkmn.Form, cache);
        }
        public override IPBEPokemonDataExtended GetPokemonDataExtended(PBESpecies species, PBEForm form, bool cache = true)
        {
            return PBEPokemonData.GetData(species, form, cache);
        }

        public override int GetSpeciesCaught()
        {
            return 300;
        }

        #endregion

        #region EXP

        public override uint GetEXPRequired(PBEGrowthRate type, byte level)
        {
            return PBEEXPTables.GetEXPRequired(type, level);
        }
        public override byte GetEXPLevel(PBEGrowthRate type, uint exp)
        {
            return PBEEXPTables.GetEXPLevel(type, exp);
        }
        public override float GetEXPModifier(PBEBattle battle)
        {
            return 1;
        }
        public override float GetEXPTradeModifier(PBEBattlePokemon pkmn)
        {
            return 1;
        }

        #endregion

        #region Catching

        public override bool IsDarkGrass(PBEBattle battle)
        {
            return false;
        }
        public override bool IsDuskBallSetting(PBEBattle battle)
        {
            return battle.BattleTerrain == PBEBattleTerrain.Cave;
        }
        public override bool IsFishing(PBEBattle battle)
        {
            return false;
        }
        public override bool IsGuaranteedCapture(PBEBattle battle, IPBESpeciesForm pkmn)
        {
            return IsGuaranteedCapture(battle, pkmn.Species, pkmn.Form);
        }
        public override bool IsGuaranteedCapture(PBEBattle battle, PBESpecies species, PBEForm form)
        {
            return false;
        }
        public override bool IsMoonBallFamily(IPBESpeciesForm pkmn)
        {
            return IsMoonBallFamily(pkmn.Species, pkmn.Form);
        }
        public override bool IsMoonBallFamily(PBESpecies species, PBEForm form)
        {
            return PBEDataUtils.MoonStoneSpecies.Contains(species);
        }
        public override bool IsRepeatBallSpecies(PBESpecies species)
        {
            return false;
        }
        public override bool IsSurfing(PBEBattle battle)
        {
            return battle.BattleTerrain == PBEBattleTerrain.Water;
        }
        public override bool IsUnderwater(PBEBattle battle)
        {
            return false;
        }
        public override float GetCatchRateModifier(PBEBattle battle)
        {
            return 1;
        }

        #endregion

        #region LocalizedString

        public override bool GetAbilityByName(string abilityName, [NotNullWhen(true)] out PBEAbility? ability)
        {
            return PBELocalizedString.GetAbilityByName(abilityName, out ability);
        }
        public override IPBELocalizedString GetAbilityDescription(PBEAbility ability)
        {
            return PBELocalizedString.GetAbilityDescription(ability);
        }
        public override IPBELocalizedString GetAbilityName(PBEAbility ability)
        {
            return PBELocalizedString.GetAbilityName(ability);
        }
        public override bool GetFormByName(PBESpecies species, string formName, [NotNullWhen(true)] out PBEForm? form)
        {
            return PBELocalizedString.GetFormByName(species, formName, out form);
        }
        public override IPBELocalizedString GetFormName(IPBESpeciesForm pkmn)
        {
            return GetFormName(pkmn.Species, pkmn.Form);
        }
        public override IPBELocalizedString GetFormName(PBESpecies species, PBEForm form)
        {
            return PBELocalizedString.GetFormName(species, form);
        }
        public override bool GetGenderByName(string genderName, [NotNullWhen(true)] out PBEGender? gender)
        {
            return PBELocalizedString.GetGenderByName(genderName, out gender);
        }
        public override IPBELocalizedString GetGenderName(PBEGender gender)
        {
            return PBELocalizedString.GetGenderName(gender);
        }
        public override bool GetItemByName(string itemName, [NotNullWhen(true)] out PBEItem? item)
        {
            return PBELocalizedString.GetItemByName(itemName, out item);
        }
        public override IPBELocalizedString GetItemDescription(PBEItem item)
        {
            return PBELocalizedString.GetItemDescription(item);
        }
        public override IPBELocalizedString GetItemName(PBEItem item)
        {
            return PBELocalizedString.GetItemName(item);
        }
        public override bool GetMoveByName(string moveName, [NotNullWhen(true)] out PBEMove? move)
        {
            return PBELocalizedString.GetMoveByName(moveName, out move);
        }
        public override IPBELocalizedString GetMoveDescription(PBEMove move)
        {
            return PBELocalizedString.GetMoveDescription(move);
        }
        public override IPBELocalizedString GetMoveName(PBEMove move)
        {
            return PBELocalizedString.GetMoveName(move);
        }
        public override bool GetNatureByName(string natureName, [NotNullWhen(true)] out PBENature? nature)
        {
            return PBELocalizedString.GetNatureByName(natureName, out nature);
        }
        public override IPBELocalizedString GetNatureName(PBENature nature)
        {
            return PBELocalizedString.GetNatureName(nature);
        }
        public override bool GetSpeciesByName(string speciesName, [NotNullWhen(true)] out PBESpecies? species)
        {
            return PBELocalizedString.GetSpeciesByName(speciesName, out species);
        }
        public override IPBELocalizedString GetSpeciesCategory(PBESpecies species)
        {
            return PBELocalizedString.GetSpeciesCategory(species);
        }
        public override IPBELocalizedString GetSpeciesEntry(PBESpecies species)
        {
            return PBELocalizedString.GetSpeciesEntry(species);
        }
        public override IPBELocalizedString GetSpeciesName(PBESpecies species)
        {
            return PBELocalizedString.GetSpeciesName(species);
        }
        public override bool GetStatByName(string statName, [NotNullWhen(true)] out PBEStat? stat)
        {
            return PBELocalizedString.GetStatByName(statName, out stat);
        }
        public override IPBELocalizedString GetStatName(PBEStat stat)
        {
            return PBELocalizedString.GetStatName(stat);
        }
        public override bool GetTypeByName(string typeName, [NotNullWhen(true)] out PBEType? type)
        {
            return PBELocalizedString.GetTypeByName(typeName, out type);
        }
        public override IPBELocalizedString GetTypeName(PBEType type)
        {
            return PBELocalizedString.GetTypeName(type);
        }

        #endregion
    }
}
