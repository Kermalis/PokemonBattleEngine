using System;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEBaseStats
    {
        public byte HP { get; }
        public byte Attack { get; }
        public byte Defense { get; }
        public byte SpAttack { get; }
        public byte SpDefense { get; }
        public byte Speed { get; }

        internal PBEBaseStats(byte hp, byte attack, byte defense, byte spAttack, byte spDefense, byte speed)
        {
            HP = hp;
            Attack = attack;
            Defense = defense;
            SpAttack = spAttack;
            SpDefense = spDefense;
            Speed = speed;
        }

        public byte this[PBEStat stat]
        {
            get
            {
                switch (stat)
                {
                    case PBEStat.HP: return HP;
                    case PBEStat.Attack: return Attack;
                    case PBEStat.Defense: return Defense;
                    case PBEStat.SpAttack: return SpAttack;
                    case PBEStat.SpDefense: return SpDefense;
                    case PBEStat.Speed: return Speed;
                    default: throw new ArgumentOutOfRangeException(nameof(stat));
                }
            }
        }
    }
}
