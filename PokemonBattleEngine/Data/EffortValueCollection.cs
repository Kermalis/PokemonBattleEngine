using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    // TODO: Listen to settings changes
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

        private readonly PBESettings settings;
        private PBEEffortValue[] evs;

        public ushort StatTotal
        {
            get
            {
                ushort sum = 0;
                for (int i = 0; i < 6; i++)
                {
                    sum += evs[i].Value;
                }
                return sum;
            }
        }

        public PBEEffortValueCollection(PBESettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            this.settings = settings;
            CreateEVs();
            Randomize();
        }
        public PBEEffortValueCollection(PBESettings settings, byte hp = 0, byte attack = 0, byte defense = 0, byte spAttack = 0, byte spDefense = 0, byte speed = 0)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            this.settings = settings;
            CreateEVs();
            Set(PBEStat.HP, hp);
            Set(PBEStat.Attack, attack);
            Set(PBEStat.Defense, defense);
            Set(PBEStat.SpAttack, spAttack);
            Set(PBEStat.SpDefense, spDefense);
            Set(PBEStat.Speed, speed);
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
            this.settings = settings;
            CreateEVs();
            Set(PBEStat.HP, other[PBEStat.HP].Value);
            Set(PBEStat.Attack, other[PBEStat.Attack].Value);
            Set(PBEStat.Defense, other[PBEStat.Defense].Value);
            Set(PBEStat.SpAttack, other[PBEStat.SpAttack].Value);
            Set(PBEStat.SpDefense, other[PBEStat.SpDefense].Value);
            Set(PBEStat.Speed, other[PBEStat.Speed].Value);
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
            ushort sum = 0;
            for (int i = 0; i < 6; i++)
            {
                if (i != statIndex)
                {
                    sum += evs[i].Value;
                }
            }
            if (sum + value > settings.MaxTotalEVs)
            {
                int amountNeededToFree = sum + value - settings.MaxTotalEVs;
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
            if (settings.MaxTotalEVs != 0)
            {
                int[] a = Enumerable.Repeat(0, 6 - 1)
                    .Select(x => PBEUtils.RNG.Next(1, settings.MaxTotalEVs))
                    .Concat(new int[] { settings.MaxTotalEVs })
                    .OrderBy(x => x)
                    .ToArray();
                ushort sum = 0;
                for (int i = 0; i < 6; i++)
                {
                    byte b = (byte)Math.Min(byte.MaxValue, a[i] - sum);
                    vals[i] = b;
                    sum += b;
                }
            }
            for (int i = 0; i < 6; i++)
            {
                UpdateEV(i, vals[i]);
            }
        }

        private void UpdateEV(int index, byte v)
        {
            PBEEffortValue ev = evs[index];
            if (ev.Value != v)
            {
                ev.Update(v);
                OnPropertyChanged(nameof(StatTotal));
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
    }
}
