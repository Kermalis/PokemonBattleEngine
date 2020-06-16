using Kermalis.EndianBinaryIO;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEReadOnlyStatCollection : IPBEReadOnlyStatCollection
    {
        public byte HP { get; }
        public byte Attack { get; }
        public byte Defense { get; }
        public byte SpAttack { get; }
        public byte SpDefense { get; }
        public byte Speed { get; }

        public PBEReadOnlyStatCollection(byte hp, byte attack, byte defense, byte spAttack, byte spDefense, byte speed)
        {
            HP = hp;
            Attack = attack;
            Defense = defense;
            SpAttack = spAttack;
            SpDefense = spDefense;
            Speed = speed;
        }
        internal PBEReadOnlyStatCollection(EndianBinaryReader r)
            : this(r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte()) { }
        public PBEReadOnlyStatCollection(IPBEReadOnlyStatCollection stats)
            : this(stats.HP, stats.Attack, stats.Defense, stats.SpAttack, stats.SpDefense, stats.Speed) { }

        public byte this[PBEStat stat] => this.GetStat(stat);
    }
    public sealed class PBEStatCollection : IPBEStatCollection
    {
        public byte HP { get; set; }
        public byte Attack { get; set; }
        public byte Defense { get; set; }
        public byte SpAttack { get; set; }
        public byte SpDefense { get; set; }
        public byte Speed { get; set; }

        public PBEStatCollection(byte hp, byte attack, byte defense, byte spAttack, byte spDefense, byte speed)
        {
            HP = hp;
            Attack = attack;
            Defense = defense;
            SpAttack = spAttack;
            SpDefense = spDefense;
            Speed = speed;
        }
        internal PBEStatCollection(EndianBinaryReader r)
            : this(r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte()) { }
        public PBEStatCollection(IPBEReadOnlyStatCollection stats)
            : this(stats.HP, stats.Attack, stats.Defense, stats.SpAttack, stats.SpDefense, stats.Speed) { }

        public byte this[PBEStat stat]
        {
            get => this.GetStat(stat);
            set => this.SetStat(stat, value);
        }
    }
}
