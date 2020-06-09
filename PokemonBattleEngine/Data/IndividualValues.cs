using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEIndividualValues : IDisposable, IEnumerable<PBEIndividualValues.PBEIndividualValue>, INotifyPropertyChanged
    {
        public sealed class PBEIndividualValue : INotifyPropertyChanged
        {
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private readonly PBEIndividualValues _parent;

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
                        if (value > _parent.Settings.MaxIVs)
                        {
                            throw new ArgumentOutOfRangeException(nameof(value));
                        }
                        _value = value;
                        OnPropertyChanged(nameof(Value));
                        _parent.UpdateHiddenPower();
                    }
                }
            }

            internal PBEIndividualValue(PBEIndividualValues parent, PBEStat stat, byte value)
            {
                _parent = parent;
                Stat = stat;
                _value = value;
            }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public PBESettings Settings { get; }
        private PBEIndividualValue[] _ivs;
        public PBEIndividualValue this[PBEStat stat]
        {
            get
            {
                int statIndex = (int)stat;
                if (statIndex >= 6)
                {
                    throw new ArgumentOutOfRangeException(nameof(stat));
                }
                return _ivs[statIndex];
            }
        }

        private PBEType _hiddenPowerType;
        public PBEType HiddenPowerType
        {
            get => _hiddenPowerType;
            private set
            {
                if (_hiddenPowerType != value)
                {
                    _hiddenPowerType = value;
                    OnPropertyChanged(nameof(HiddenPowerType));
                }
            }
        }
        private byte _hiddenPowerBasePower;
        public byte HiddenPowerBasePower
        {
            get => _hiddenPowerBasePower;
            private set
            {
                if (_hiddenPowerBasePower != value)
                {
                    _hiddenPowerBasePower = value;
                    OnPropertyChanged(nameof(HiddenPowerBasePower));
                }
            }
        }

        public byte HP
        {
            get => _ivs[0].Value;
            set => _ivs[0].Value = value;
        }
        public byte Attack
        {
            get => _ivs[1].Value;
            set => _ivs[1].Value = value;
        }
        public byte Defense
        {
            get => _ivs[2].Value;
            set => _ivs[2].Value = value;
        }
        public byte SpAttack
        {
            get => _ivs[3].Value;
            set => _ivs[3].Value = value;
        }
        public byte SpDefense
        {
            get => _ivs[4].Value;
            set => _ivs[4].Value = value;
        }
        public byte Speed
        {
            get => _ivs[5].Value;
            set => _ivs[5].Value = value;
        }

        internal PBEIndividualValues(PBESettings settings, EndianBinaryReader r)
        {
            void Validate(byte val)
            {
                if (val > settings.MaxIVs)
                {
                    throw new InvalidDataException();
                }
            }
            byte hp = r.ReadByte();
            Validate(hp);
            byte attack = r.ReadByte();
            Validate(attack);
            byte defense = r.ReadByte();
            Validate(defense);
            byte spAttack = r.ReadByte();
            Validate(spAttack);
            byte spDefense = r.ReadByte();
            Validate(spDefense);
            byte speed = r.ReadByte();
            Validate(speed);
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            CreateIVs(hp, attack, defense, spAttack, spDefense, speed);
        }
        internal PBEIndividualValues(PBESettings settings, JToken jToken)
        {
            void Validate(byte val, string name)
            {
                if (val > settings.MaxIVs)
                {
                    throw new ArgumentOutOfRangeException(nameof(PBEPokemonShell.IndividualValues), $"\"{name}\" individual value must not exceed \"{nameof(settings.MaxIVs)}\" ({settings.MaxIVs})");
                }
            }
            byte hp = jToken[nameof(PBEStat.HP)].Value<byte>();
            Validate(hp, nameof(PBEStat.HP));
            byte attack = jToken[nameof(PBEStat.Attack)].Value<byte>();
            Validate(attack, nameof(PBEStat.Attack));
            byte defense = jToken[nameof(PBEStat.Defense)].Value<byte>();
            Validate(defense, nameof(PBEStat.Defense));
            byte spAttack = jToken[nameof(PBEStat.SpAttack)].Value<byte>();
            Validate(spAttack, nameof(PBEStat.SpAttack));
            byte spDefense = jToken[nameof(PBEStat.SpDefense)].Value<byte>();
            Validate(spDefense, nameof(PBEStat.SpDefense));
            byte speed = jToken[nameof(PBEStat.Speed)].Value<byte>();
            Validate(speed, nameof(PBEStat.Speed));
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            CreateIVs(hp, attack, defense, spAttack, spDefense, speed);
        }
        internal PBEIndividualValues(PBEIndividualValues other)
        {
            Settings = other.Settings;
            Settings.PropertyChanged += OnSettingsChanged;
            CreateIVs(other.HP, other.Attack, other.Defense, other.SpAttack, other.SpDefense, other.Speed);
        }
        public PBEIndividualValues(PBESettings settings, bool randomize)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            Settings = settings;
            Settings.PropertyChanged += OnSettingsChanged;
            _canDispose = true;
            CreateIVs(0, 0, 0, 0, 0, 0);
            if (randomize)
            {
                Randomize();
            }
        }

        private void CreateIVs(byte hp, byte attack, byte defense, byte spAttack, byte spDefense, byte speed)
        {
            _ivs = new PBEIndividualValue[6]
            {
                new PBEIndividualValue(this, PBEStat.HP, hp),
                new PBEIndividualValue(this, PBEStat.Attack, attack),
                new PBEIndividualValue(this, PBEStat.Defense, defense),
                new PBEIndividualValue(this, PBEStat.SpAttack, spAttack),
                new PBEIndividualValue(this, PBEStat.SpDefense, spDefense),
                new PBEIndividualValue(this, PBEStat.Speed, speed)
            };
            UpdateHiddenPower();
        }
        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.MaxIVs):
                {
                    for (int i = 0; i < 6; i++)
                    {
                        PBEIndividualValue iv = _ivs[i];
                        if (iv.Value > Settings.MaxIVs)
                        {
                            iv.Value = Settings.MaxIVs;
                        }
                    }
                    break;
                }
            }
        }
        private void UpdateHiddenPower()
        {
            HiddenPowerType = PBEDataUtils.GetHiddenPowerType(HP, Attack, Defense, SpAttack, SpDefense, Speed);
            HiddenPowerBasePower = PBEDataUtils.GetHiddenPowerBasePower(HP, Attack, Defense, SpAttack, SpDefense, Speed, Settings);
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

        public void Maximize()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            for (int i = 0; i < 6; i++)
            {
                _ivs[i].Value = Settings.MaxIVs;
            }
        }
        public void Randomize()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            for (int i = 0; i < 6; i++)
            {
                _ivs[i].Value = (byte)PBERandom.RandomInt(0, Settings.MaxIVs);
            }
        }

        public IEnumerator<PBEIndividualValue> GetEnumerator()
        {
            for (int i = 0; i < 6; i++)
            {
                yield return _ivs[i];
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
                w.Write(_ivs[i].Value);
            }
        }
        internal void ToJson(JsonTextWriter w)
        {
            w.WriteStartObject();
            for (int i = 0; i < 6; i++)
            {
                PBEIndividualValue iv = _ivs[i];
                w.WritePropertyName(iv.Stat.ToString());
                w.WriteValue(iv.Value);
            }
            w.WriteEndObject();
        }
    }
}
