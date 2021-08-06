using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Data.Legality
{
    public sealed class PBELegalIndividualValues : IPBEStatCollection, IEnumerable<PBELegalIndividualValues.PBELegalIndividualValue>, INotifyPropertyChanged
    {
        public sealed class PBELegalIndividualValue : INotifyPropertyChanged
        {
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
            public event PropertyChangedEventHandler? PropertyChanged;

            private readonly PBELegalIndividualValues _parent;

            public PBEStat Stat { get; }
            private byte _value;
            public byte Value
            {
                get => _value;
                set
                {
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

            internal PBELegalIndividualValue(PBELegalIndividualValues parent, PBEStat stat, byte value)
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
        public event PropertyChangedEventHandler? PropertyChanged;

        public PBESettings Settings { get; }
        private readonly PBELegalIndividualValue[] _ivs;
        public PBELegalIndividualValue this[PBEStat stat]
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

        internal PBELegalIndividualValues(PBESettings settings, EndianBinaryReader r)
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
            _ivs = CreateIVs(hp, attack, defense, spAttack, spDefense, speed);
        }
        internal PBELegalIndividualValues(PBESettings settings, JToken jToken)
        {
            void Validate(byte val, string name)
            {
                if (val > settings.MaxIVs)
                {
                    throw new InvalidDataException($"\"{name}\" individual value must not exceed \"{nameof(settings.MaxIVs)}\" ({settings.MaxIVs})");
                }
            }
            byte hp = jToken.GetSafe(nameof(PBEStat.HP)).Value<byte>();
            Validate(hp, nameof(PBEStat.HP));
            byte attack = jToken.GetSafe(nameof(PBEStat.Attack)).Value<byte>();
            Validate(attack, nameof(PBEStat.Attack));
            byte defense = jToken.GetSafe(nameof(PBEStat.Defense)).Value<byte>();
            Validate(defense, nameof(PBEStat.Defense));
            byte spAttack = jToken.GetSafe(nameof(PBEStat.SpAttack)).Value<byte>();
            Validate(spAttack, nameof(PBEStat.SpAttack));
            byte spDefense = jToken.GetSafe(nameof(PBEStat.SpDefense)).Value<byte>();
            Validate(spDefense, nameof(PBEStat.SpDefense));
            byte speed = jToken.GetSafe(nameof(PBEStat.Speed)).Value<byte>();
            Validate(speed, nameof(PBEStat.Speed));
            Settings = settings;
            _ivs = CreateIVs(hp, attack, defense, spAttack, spDefense, speed);
        }
        internal PBELegalIndividualValues(PBELegalIndividualValues other)
        {
            Settings = other.Settings;
            _ivs = CreateIVs(other.HP, other.Attack, other.Defense, other.SpAttack, other.SpDefense, other.Speed);
        }
        public PBELegalIndividualValues(PBESettings settings, bool randomize)
        {
            settings.ShouldBeReadOnly(nameof(settings));
            Settings = settings;
            _ivs = CreateIVs(0, 0, 0, 0, 0, 0);
            if (randomize)
            {
                Randomize();
            }
        }

        private PBELegalIndividualValue[] CreateIVs(byte hp, byte attack, byte defense, byte spAttack, byte spDefense, byte speed)
        {
            var ivs = new PBELegalIndividualValue[6]
            {
                new PBELegalIndividualValue(this, PBEStat.HP, hp),
                new PBELegalIndividualValue(this, PBEStat.Attack, attack),
                new PBELegalIndividualValue(this, PBEStat.Defense, defense),
                new PBELegalIndividualValue(this, PBEStat.SpAttack, spAttack),
                new PBELegalIndividualValue(this, PBEStat.SpDefense, spDefense),
                new PBELegalIndividualValue(this, PBEStat.Speed, speed)
            };
            UpdateHiddenPower();
            return ivs;
        }
        private void UpdateHiddenPower()
        {
            HiddenPowerType = PBEDataUtils.GetHiddenPowerType(HP, Attack, Defense, SpAttack, SpDefense, Speed);
            HiddenPowerBasePower = PBEDataUtils.GetHiddenPowerBasePower(HP, Attack, Defense, SpAttack, SpDefense, Speed, Settings);
        }

        public void Clear()
        {
            for (int i = 0; i < 6; i++)
            {
                _ivs[i].Value = 0;
            }
        }
        public void Maximize()
        {
            for (int i = 0; i < 6; i++)
            {
                _ivs[i].Value = Settings.MaxIVs;
            }
        }
        public void Randomize()
        {
            for (int i = 0; i < 6; i++)
            {
                _ivs[i].Value = (byte)PBEDataProvider.GlobalRandom.RandomInt(0, Settings.MaxIVs);
            }
        }

        public IEnumerator<PBELegalIndividualValue> GetEnumerator()
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
    }
}
