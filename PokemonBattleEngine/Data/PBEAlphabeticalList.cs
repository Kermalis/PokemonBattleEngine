using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEAlphabeticalList<T> : IDisposable, INotifyCollectionChanged, INotifyPropertyChanged, IReadOnlyList<T>
    {
        private sealed class PBEAlphabeticalListEntry<T>
        {
            public T Key { get; }
            public PBELocalizedString Value { get; }

            public PBEAlphabeticalListEntry(T key)
            {
                switch (key)
                {
                    case PBEAbility ability: Value = PBELocalizedString.GetAbilityName(ability); break;
                    case PBEGender gender: Value = PBELocalizedString.GetGenderName(gender); break;
                    case PBEItem item: Value = PBELocalizedString.GetItemName(item); break;
                    case PBEMove move: Value = PBELocalizedString.GetMoveName(move); break;
                    case PBENature nature: Value = PBELocalizedString.GetNatureName(nature); break;
                    case PBESpecies species: Value = PBELocalizedString.GetSpeciesName(species); break;
                    case PBEStat stat: Value = PBELocalizedString.GetStatName(stat); break;
                    case PBEType type: Value = PBELocalizedString.GetTypeName(type); break;
                    default: throw new ArgumentOutOfRangeException(nameof(key));
                }
                Key = key;
            }
        }

        private void FireEvents(NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged("Item[]");
            OnCollectionChanged(e);
        }
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private PBEAlphabeticalListEntry<T>[] list;
        public int Count => list.Length;
        public T this[int index] => list[index].Key;

        internal PBEAlphabeticalList()
        {
            list = Array.Empty<PBEAlphabeticalListEntry<T>>();
            PBELocalizedString.PBECultureChanged += OnCultureChanged;
        }
        internal PBEAlphabeticalList(IEnumerable<T> collection)
        {
            PBELocalizedString.PBECultureChanged += OnCultureChanged;
            Reset(collection);
        }

        private void OnCultureChanged(CultureInfo oldPBECultureInfo)
        {
            if (!oldPBECultureInfo.TwoLetterISOLanguageName.Equals(PBELocalizedString.PBECulture.TwoLetterISOLanguageName))
            {
                Sort();
            }
        }
        private void Sort()
        {
            Array.Sort(list, (x, y) => x.Value.ToString().CompareTo(y.Value.ToString()));
            FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal void Reset(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (collection is PBEAlphabeticalList<T> other)
            {
                list = (PBEAlphabeticalListEntry<T>[])other.list.Clone();
            }
            else
            {
                list = collection.Select(t => new PBEAlphabeticalListEntry<T>(t)).ToArray();
            }
            Sort();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }
        public int IndexOf(T item)
        {
            if (item != null)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    if (item.Equals(list[i].Key))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public void Dispose()
        {
            PBELocalizedString.PBECultureChanged -= OnCultureChanged;
        }
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < list.Length; i++)
            {
                yield return list[i].Key;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
