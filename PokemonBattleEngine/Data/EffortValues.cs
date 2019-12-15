using Kermalis.EndianBinaryIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEEffortValues : IDisposable, IEnumerable<PBEEffortValues.PBEEffortValue>, INotifyPropertyChanged
    {
        public sealed class PBEEffortValue : INotifyPropertyChanged
        {
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private readonly PBEEffortValues _parent;

            public PBEStat Stat { get; }
            private byte _value;
            public byte Value
            {
                get => _value;
                set
                {
                    if (_parent.IsDisposed)
                    {
                        throw new ObjectDisposedException(null);
                    }
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

            internal PBEEffortValue(PBEEffortValues parent, PBEStat stat, byte value)
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
        private PBEEffortValue[] _evs;
        public PBEEffortValue this[PBEStat stat]
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

        internal PBEEffortValues(PBESettings settings, EndianBinaryReader r)
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
            Settings.PropertyChanged += OnSettingsChanged;
            CreateEVs(hp, attack, defense, spAttack, spDefense, speed);
        }
        internal PBEEffortValues(PBESettings settings, JToken jToken)
        {
            byte hp = jToken[nameof(PBEStat.HP)].Value<byte>();
            byte attack = jToken[nameof(PBEStat.Attack)].Value<byte>();
            byte defense = jToken[nameof(PBEStat.Defense)].Value<byte>();
            byte spAttack = jToken[nameof(PBEStat.SpAttack)].Value<byte>();
            byte spDefense = jToken[nameof(PBEStat.SpDefense)].Value<byte>();
            byte speed = jToken[nameof(PBEStat.Speed)].Value<byte>();
            if (hp + attack + defense + spAttack + spDefense + speed > settings.MaxTotalEVs)
            {
                throw new ArgumentOutOfRangeException(nameof(PBEPokemonShell.EffortValues), $"Effort values total must not exceed \"{nameof(settings.MaxTotalEVs)}\" ({settings.MaxTotalEVs})");
            }
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            CreateEVs(hp, attack, defense, spAttack, spDefense, speed);
        }
        internal PBEEffortValues(PBEEffortValues other)
        {
            Settings = other.Settings;
            Settings.PropertyChanged += OnSettingsChanged;
            CreateEVs(other[PBEStat.HP].Value, other[PBEStat.Attack].Value, other[PBEStat.Defense].Value, other[PBEStat.SpAttack].Value, other[PBEStat.SpDefense].Value, other[PBEStat.Speed].Value);
        }
        public PBEEffortValues(PBESettings settings, bool randomize)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            _canDispose = true;
            CreateEVs(0, 0, 0, 0, 0, 0);
            if (randomize)
            {
                Randomize();
            }
        }

        private void CreateEVs(byte hp, byte attack, byte defense, byte spAttack, byte spDefense, byte speed)
        {
            _evs = new PBEEffortValue[6]
            {
                new PBEEffortValue(this, PBEStat.HP, hp),
                new PBEEffortValue(this, PBEStat.Attack, attack),
                new PBEEffortValue(this, PBEStat.Defense, defense),
                new PBEEffortValue(this, PBEStat.SpAttack, spAttack),
                new PBEEffortValue(this, PBEStat.SpDefense, spDefense),
                new PBEEffortValue(this, PBEStat.Speed, speed)
            };
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
                            PBEEffortValue ev = _evs[i];
                            ev.Value = (byte)(Settings.MaxTotalEVs * ((double)ev.Value / oldTotal));
                        }
                    }
                    break;
                }
            }
        }

        private bool _canDispose;
        public bool CanDispose
        {
            get => _canDispose;
            internal set
            {
                if (_canDispose != value)
                {
                    _canDispose = value;
                    OnPropertyChanged(nameof(CanDispose));
                }
            }
        }
        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            if (!_canDispose)
            {
                throw new InvalidOperationException();
            }
            if (!IsDisposed)
            {
                IsDisposed = true;
                OnPropertyChanged(nameof(IsDisposed));
                Settings.PropertyChanged -= OnSettingsChanged;
            }
        }

        public void Randomize()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            if (Settings.MaxTotalEVs != 0)
            {
                byte[] vals = new byte[6];
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
                for (int i = 0; i < 6; i++)
                {
                    _evs[i].Value = vals[i];
                }
            }
        }

        public IEnumerator<PBEEffortValue> GetEnumerator()
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

        internal void ToBytes(EndianBinaryWriter w)
        {
            for (int i = 0; i < 6; i++)
            {
                w.Write(_evs[i].Value);
            }
        }
        internal void ToJson(JsonTextWriter w)
        {
            w.WriteStartObject();
            for (int i = 0; i < 6; i++)
            {
                PBEEffortValue ev = _evs[i];
                w.WritePropertyName(ev.Stat.ToString());
                w.WriteValue(ev.Value);
            }
            w.WriteEndObject();
        }
    }
}
