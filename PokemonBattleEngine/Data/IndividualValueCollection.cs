using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngine.Data
{
    // TODO: Listen to settings changes
    public sealed class PBEIndividualValueCollection : IEnumerable<PBEIndividualValueCollection.PBEIndividualValue>
    {
        public sealed class PBEIndividualValue : INotifyPropertyChanged
        {
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private readonly PBESettings settings;

            public PBEStat Stat { get; }
            private byte value;
            public byte Value
            {
                get => value;
                set
                {
                    byte newVal = Math.Min(value, settings.MaxIVs);
                    if (this.value != newVal)
                    {
                        this.value = newVal;
                        OnPropertyChanged(nameof(Value));
                    }
                }
            }

            internal PBEIndividualValue(PBESettings settings, PBEStat stat)
            {
                this.settings = settings;
                Stat = stat;
            }
        }

        private readonly PBESettings settings;
        private PBEIndividualValue[] ivs;

        public PBEIndividualValueCollection(PBESettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            this.settings = settings;
            CreateIVs(settings);
            Randomize();
        }
        public PBEIndividualValueCollection(PBESettings settings, byte hp = 0, byte attack = 0, byte defense = 0, byte spAttack = 0, byte spDefense = 0, byte speed = 0)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            this.settings = settings;
            CreateIVs(settings);
            this[PBEStat.HP].Value = hp;
            this[PBEStat.Attack].Value = attack;
            this[PBEStat.Defense].Value = defense;
            this[PBEStat.SpAttack].Value = spAttack;
            this[PBEStat.SpDefense].Value = spDefense;
            this[PBEStat.Speed].Value = speed;
        }
        public PBEIndividualValueCollection(PBESettings settings, PBEIndividualValueCollection other)
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
            CreateIVs(settings);
            this[PBEStat.HP].Value = other[PBEStat.HP].Value;
            this[PBEStat.Attack].Value = other[PBEStat.Attack].Value;
            this[PBEStat.Defense].Value = other[PBEStat.Defense].Value;
            this[PBEStat.SpAttack].Value = other[PBEStat.SpAttack].Value;
            this[PBEStat.SpDefense].Value = other[PBEStat.SpDefense].Value;
            this[PBEStat.Speed].Value = other[PBEStat.Speed].Value;
        }
        private void CreateIVs(PBESettings settings)
        {
            ivs = new PBEIndividualValue[6]
            {
                new PBEIndividualValue(settings, PBEStat.HP),
                new PBEIndividualValue(settings, PBEStat.Attack),
                new PBEIndividualValue(settings, PBEStat.Defense),
                new PBEIndividualValue(settings, PBEStat.SpAttack),
                new PBEIndividualValue(settings, PBEStat.SpDefense),
                new PBEIndividualValue(settings, PBEStat.Speed)
            };
        }

        public PBEIndividualValue this[PBEStat stat]
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
                    return ivs[statIndex];
                }
            }
        }

        public void Randomize()
        {
            for (int i = 0; i < 6; i++)
            {
                ivs[i].Value = (byte)PBEUtils.RNG.Next(settings.MaxIVs + 1);
            }
        }

        public IEnumerator<PBEIndividualValue> GetEnumerator()
        {
            for (int i = 0; i < 6; i++)
            {
                yield return ivs[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
