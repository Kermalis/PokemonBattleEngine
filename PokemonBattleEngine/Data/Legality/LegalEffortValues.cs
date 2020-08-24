using Kermalis.EndianBinaryIO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data.Legality
{
    public sealed class PBELegalEffortValues : IPBEStatCollection, IEnumerable<PBELegalEffortValues.PBELegalEffortValue>, INotifyPropertyChanged
    {
        public sealed class PBELegalEffortValue : INotifyPropertyChanged
        {
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private readonly PBELegalEffortValues _parent;

            public PBEStat Stat { get; }
            private byte _value;
            public byte Value
            {
                get => _value;
                set
                {
                    if (_value != value)
                    {
                        ushort oldTotal = _parent.StatTotal;
                        int newTotal = oldTotal - _value + value;
                        if (newTotal > _parent.Settings.MaxTotalEVs)
                        {
                            byte newValue = (byte)(value - (newTotal - _parent.Settings.MaxTotalEVs));
                            if (_value != newValue)
                            {
                                Update(newValue);
                            }
                        }
                        else
                        {
                            Update(value);
                        }
                    }
                }
            }

            internal PBELegalEffortValue(PBELegalEffortValues parent, PBEStat stat, byte value)
            {
                _parent = parent;
                Stat = stat;
                _value = value;
            }

            private void Update(byte newValue)
            {
                _value = newValue;
                OnPropertyChanged(nameof(Value));
                _parent.OnPropertyChanged(nameof(_parent.StatTotal));
            }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public PBESettings Settings { get; }
        private PBELegalEffortValue[] _evs;
        public PBELegalEffortValue this[PBEStat stat]
        {
            get
            {
                int statIndex = (int)stat;
                if (statIndex >= 6)
                {
                    throw new ArgumentOutOfRangeException(nameof(stat));
                }
                return _evs[statIndex];
            }
        }

        public ushort StatTotal
        {
            get
            {
                ushort total = 0;
                for (int i = 0; i < 6; i++)
                {
                    total += _evs[i].Value;
                }
                return total;
            }
        }

        public byte HP
        {
            get => _evs[0].Value;
            set => _evs[0].Value = value;
        }
        public byte Attack
        {
            get => _evs[1].Value;
            set => _evs[1].Value = value;
        }
        public byte Defense
        {
            get => _evs[2].Value;
            set => _evs[2].Value = value;
        }
        public byte SpAttack
        {
            get => _evs[3].Value;
            set => _evs[3].Value = value;
        }
        public byte SpDefense
        {
            get => _evs[4].Value;
            set => _evs[4].Value = value;
        }
        public byte Speed
        {
            get => _evs[5].Value;
            set => _evs[5].Value = value;
        }

        internal PBELegalEffortValues(PBESettings settings, EndianBinaryReader r)
        {
            byte hp = r.ReadByte();
            byte attack = r.ReadByte();
            byte defense = r.ReadByte();
            byte spAttack = r.ReadByte();
            byte spDefense = r.ReadByte();
            byte speed = r.ReadByte();
            if (hp + attack + defense + spAttack + spDefense + speed > settings.MaxTotalEVs)
            {
                throw new InvalidDataException();
            }
            Settings = settings;
            CreateEVs(hp, attack, defense, spAttack, spDefense, speed);
        }
        internal PBELegalEffortValues(PBESettings settings, JToken jToken)
        {
            byte hp = jToken[nameof(PBEStat.HP)].Value<byte>();
            byte attack = jToken[nameof(PBEStat.Attack)].Value<byte>();
            byte defense = jToken[nameof(PBEStat.Defense)].Value<byte>();
            byte spAttack = jToken[nameof(PBEStat.SpAttack)].Value<byte>();
            byte spDefense = jToken[nameof(PBEStat.SpDefense)].Value<byte>();
            byte speed = jToken[nameof(PBEStat.Speed)].Value<byte>();
            if (hp + attack + defense + spAttack + spDefense + speed > settings.MaxTotalEVs)
            {
                throw new ArgumentOutOfRangeException(nameof(IPBEPokemon.EffortValues), $"Effort values total must not exceed \"{nameof(settings.MaxTotalEVs)}\" ({settings.MaxTotalEVs})");
            }
            Settings = settings;
            CreateEVs(hp, attack, defense, spAttack, spDefense, speed);
        }
        internal PBELegalEffortValues(PBELegalEffortValues other)
        {
            Settings = other.Settings;
            CreateEVs(other.HP, other.Attack, other.Defense, other.SpAttack, other.SpDefense, other.Speed);
        }
        public PBELegalEffortValues(PBESettings settings, bool randomize)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            Settings = settings;
            CreateEVs(0, 0, 0, 0, 0, 0);
            if (randomize)
            {
                Randomize();
            }
        }

        private void CreateEVs(byte hp, byte attack, byte defense, byte spAttack, byte spDefense, byte speed)
        {
            _evs = new PBELegalEffortValue[6]
            {
                new PBELegalEffortValue(this, PBEStat.HP, hp),
                new PBELegalEffortValue(this, PBEStat.Attack, attack),
                new PBELegalEffortValue(this, PBEStat.Defense, defense),
                new PBELegalEffortValue(this, PBEStat.SpAttack, spAttack),
                new PBELegalEffortValue(this, PBEStat.SpDefense, spDefense),
                new PBELegalEffortValue(this, PBEStat.Speed, speed)
            };
        }

        public void Clear()
        {
            for (int i = 0; i < 6; i++)
            {
                _evs[i].Value = 0;
            }
        }
        public void Equalize()
        {
            Clear();
            for (int i = 0; i < 6; i++)
            {
                _evs[i].Value = (byte)(Settings.MaxTotalEVs / 6);
            }
        }
        public void Randomize()
        {
            if (Settings.MaxTotalEVs != 0)
            {
                byte[] vals = new byte[6];
                int[] a = Enumerable.Repeat(0, 6 - 1)
                    .Select(x => PBEDataProvider.GlobalRandom.RandomInt(1, Settings.MaxTotalEVs - 1))
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
                    int index = PBEDataProvider.GlobalRandom.RandomElement(notMax);
                    byte old = vals[index];
                    byte b = (byte)Math.Min(byte.MaxValue, old + (Settings.MaxTotalEVs - total));
                    vals[index] = b;
                    total += (byte)(b - old);
                }
                for (int i = 0; i < 6; i++)
                {
                    _evs[i].Value = vals[i];
                }
            }
        }

        public IEnumerator<PBELegalEffortValue> GetEnumerator()
        {
            for (int i = 0; i < 6; i++)
            {
                yield return _evs[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
