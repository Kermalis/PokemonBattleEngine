using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEEffortValueCollection : IEnumerable<PBEEffortValueCollection.PBEEffortValue>, INotifyPropertyChanged
    {
        public sealed class PBEEffortValue : INotifyPropertyChanged
        {
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private readonly PBEEffortValueCollection parent;

            public PBEStat Stat { get; }
            private byte value;
            public byte Value
            {
                get => value;
                set
                {
                    if (this.value != value)
                    {
                        parent.Set(Stat, value);
                    }
                }
            }

            internal PBEEffortValue(PBEEffortValueCollection parent, PBEStat stat)
            {
                this.parent = parent;
                Stat = stat;
            }

            internal void Update(byte value)
            {
                this.value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public PBESettings Settings { get; }
        private PBEEffortValue[] evs;

        public ushort StatTotal
        {
            get
            {
                ushort total = 0;
                for (int i = 0; i < 6; i++)
                {
                    total += evs[i].Value;
                }
                return total;
            }
        }

        public PBEEffortValueCollection(PBESettings settings, bool randomize)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            CreateEVs();
            if (randomize)
            {
                Randomize();
            }
        }
        public PBEEffortValueCollection(PBESettings settings, byte hp = 0, byte attack = 0, byte defense = 0, byte spAttack = 0, byte spDefense = 0, byte speed = 0)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            CreateEVs();
            TrySet(hp, attack, defense, spAttack, spDefense, speed);
        }
        public PBEEffortValueCollection(PBESettings settings, PBEEffortValueCollection other)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            CreateEVs();
            TrySet(other[PBEStat.HP].Value, other[PBEStat.Attack].Value, other[PBEStat.Defense].Value, other[PBEStat.SpAttack].Value, other[PBEStat.SpDefense].Value, other[PBEStat.Speed].Value);
        }
        private void CreateEVs()
        {
            evs = new PBEEffortValue[6]
            {
                new PBEEffortValue(this, PBEStat.HP),
                new PBEEffortValue(this, PBEStat.Attack),
                new PBEEffortValue(this, PBEStat.Defense),
                new PBEEffortValue(this, PBEStat.SpAttack),
                new PBEEffortValue(this, PBEStat.SpDefense),
                new PBEEffortValue(this, PBEStat.Speed)
            };
        }
        private void UpdateEV(int statIndex, byte value)
        {
            PBEEffortValue ev = evs[statIndex];
            if (ev.Value != value)
            {
                ev.Update(value);
                OnPropertyChanged(nameof(StatTotal));
            }
        }
        private void TrySet(byte hp, byte attack, byte defense, byte spAttack, byte spDefense, byte speed)
        {
            Set(PBEStat.HP, hp);
            Set(PBEStat.Attack, attack);
            Set(PBEStat.Defense, defense);
            Set(PBEStat.SpAttack, spAttack);
            Set(PBEStat.SpDefense, spDefense);
            Set(PBEStat.Speed, speed);
        }

        public PBEEffortValue this[PBEStat stat]
        {
            get
            {
                int statIndex = (int)stat;
                if (statIndex >= 6)
                {
                    throw new ArgumentOutOfRangeException(nameof(stat));
                }
                else
                {
                    return evs[statIndex];
                }
            }
        }

        public void Set(PBEStat stat, byte value)
        {
            int statIndex = (int)stat;
            if (statIndex >= 6)
            {
                throw new ArgumentOutOfRangeException(nameof(stat));
            }
            ushort oldTotal = StatTotal;
            if (oldTotal + value > Settings.MaxTotalEVs)
            {
                int amountNeededToFree = oldTotal + value - Settings.MaxTotalEVs;
                UpdateEV(statIndex, (byte)(value - amountNeededToFree));
            }
            else
            {
                UpdateEV(statIndex, value);
            }
        }
        public void Randomize()
        {
            byte[] vals = new byte[6];
            if (Settings.MaxTotalEVs != 0)
            {
                int[] a = Enumerable.Repeat(0, 6 - 1)
                    .Select(x => PBEUtils.RandomInt(1, Settings.MaxTotalEVs - 1))
                    .Concat(new int[] { Settings.MaxTotalEVs })
                    .OrderBy(x => x)
                    .ToArray();
                ushort total = 0;
                for (int i = 0; i < 6; i++)
                {
                    byte b = (byte)Math.Min(byte.MaxValue, a[i] - total);
                    vals[i] = b;
                    total += b;
                }
                // This "while" will fix the issue where the speed stat was supposed to be above 255
                var notMax = new List<int>(5);
                while (total != Settings.MaxTotalEVs)
                {
                    notMax.Clear();
                    for (int i = 0; i < 6; i++)
                    {
                        if (vals[i] != byte.MaxValue)
                        {
                            notMax.Add(i);
                        }
                    }
                    int index = notMax.RandomElement();
                    byte old = vals[index];
                    byte b = (byte)Math.Min(byte.MaxValue, old + (Settings.MaxTotalEVs - total));
                    vals[index] = b;
                    total += (byte)(b - old);
                }
            }
            for (int i = 0; i < 6; i++)
            {
                UpdateEV(i, vals[i]);
            }
        }

        public IEnumerator<PBEEffortValue> GetEnumerator()
        {
            for (int i = 0; i < 6; i++)
            {
                yield return evs[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.MaxTotalEVs):
                {
                    ushort oldTotal = StatTotal;
                    if (oldTotal > Settings.MaxTotalEVs)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            UpdateEV(i, (byte)(Settings.MaxTotalEVs * ((double)evs[i].Value / oldTotal)));
                        }
                    }
                    break;
                }
            }
        }
    }
}
