using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data.Utils;
using Newtonsoft.Json;
using System;

namespace Kermalis.PokemonBattleEngine.Data
{
    public interface IPBEReadOnlyStatCollection
    {
        byte HP { get; }
        byte Attack { get; }
        byte Defense { get; }
        byte SpAttack { get; }
        byte SpDefense { get; }
        byte Speed { get; }
    }
    public interface IPBEStatCollection : IPBEReadOnlyStatCollection
    {
        new byte HP { get; set; }
        new byte Attack { get; set; }
        new byte Defense { get; set; }
        new byte SpAttack { get; set; }
        new byte SpDefense { get; set; }
        new byte Speed { get; set; }
    }

    public static class PBEStatInterfaceExtensions
    {
        public static byte GetStat(this IPBEReadOnlyStatCollection stats, PBEStat stat)
        {
            switch (stat)
            {
                case PBEStat.HP: return stats.HP;
                case PBEStat.Attack: return stats.Attack;
                case PBEStat.Defense: return stats.Defense;
                case PBEStat.SpAttack: return stats.SpAttack;
                case PBEStat.SpDefense: return stats.SpDefense;
                case PBEStat.Speed: return stats.Speed;
                default: throw new ArgumentOutOfRangeException(nameof(stat));
            }
        }
        public static void SetStat(this IPBEStatCollection stats, PBEStat stat, byte value)
        {
            switch (stat)
            {
                case PBEStat.HP: stats.HP = value; break;
                case PBEStat.Attack: stats.Attack = value; break;
                case PBEStat.Defense: stats.Defense = value; break;
                case PBEStat.SpAttack: stats.SpAttack = value; break;
                case PBEStat.SpDefense: stats.SpDefense = value; break;
                case PBEStat.Speed: stats.Speed = value; break;
                default: throw new ArgumentOutOfRangeException(nameof(stat));
            }
        }

        public static byte GetHiddenPowerBasePower(this IPBEReadOnlyStatCollection stats, PBESettings settings)
        {
            return PBEDataUtils.GetHiddenPowerBasePower(stats.HP, stats.Attack, stats.Defense, stats.SpAttack, stats.SpDefense, stats.Speed, settings);
        }
        public static PBEType GetHiddenPowerType(this IPBEReadOnlyStatCollection stats)
        {
            return PBEDataUtils.GetHiddenPowerType(stats.HP, stats.Attack, stats.Defense, stats.SpAttack, stats.SpDefense, stats.Speed);
        }

        internal static void ToBytes(this IPBEReadOnlyStatCollection stats, EndianBinaryWriter w)
        {
            w.Write(stats.HP);
            w.Write(stats.Attack);
            w.Write(stats.Defense);
            w.Write(stats.SpAttack);
            w.Write(stats.SpDefense);
            w.Write(stats.Speed);
        }
        internal static void ToJson(this IPBEReadOnlyStatCollection stats, JsonTextWriter w)
        {
            w.WriteStartObject();
            w.WritePropertyName(nameof(IPBEReadOnlyStatCollection.HP));
            w.WriteValue(stats.HP);
            w.WritePropertyName(nameof(IPBEReadOnlyStatCollection.Attack));
            w.WriteValue(stats.Attack);
            w.WritePropertyName(nameof(IPBEReadOnlyStatCollection.Defense));
            w.WriteValue(stats.Defense);
            w.WritePropertyName(nameof(IPBEReadOnlyStatCollection.SpAttack));
            w.WriteValue(stats.SpAttack);
            w.WritePropertyName(nameof(IPBEReadOnlyStatCollection.SpDefense));
            w.WriteValue(stats.SpDefense);
            w.WritePropertyName(nameof(IPBEReadOnlyStatCollection.Speed));
            w.WriteValue(stats.Speed);
            w.WriteEndObject();
        }
    }
}
