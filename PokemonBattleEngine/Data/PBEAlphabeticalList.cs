using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEAlphabeticalList<T> : INotifyCollectionChanged, INotifyPropertyChanged, IReadOnlyList<T>
    {
        private sealed class PBEAlphabeticalListEntry
        {
            public T Key { get; }
            public IPBELocalizedString Value { get; }

            public PBEAlphabeticalListEntry(T key, object parameter)
            {
                switch (key)
                {
                    case PBEAbility ability: Value = PBEDataProvider.Instance.GetAbilityName(ability); break;
                    case PBEForm form: Value = PBEDataProvider.Instance.GetFormName((PBESpecies)parameter, form); break;
                    case PBEGender gender: Value = PBEDataProvider.Instance.GetGenderName(gender); break;
                    case PBEItem item: Value = PBEDataProvider.Instance.GetItemName(item); break;
                    case PBEMove move: Value = PBEDataProvider.Instance.GetMoveName(move); break;
                    case PBENature nature: Value = PBEDataProvider.Instance.GetNatureName(nature); break;
                    case PBESpecies species: Value = PBEDataProvider.Instance.GetSpeciesName(species); break;
                    case PBEStat stat: Value = PBEDataProvider.Instance.GetStatName(stat); break;
                    case PBEType type: Value = PBEDataProvider.Instance.GetTypeName(type); break;
                    default: throw new ArgumentOutOfRangeException(nameof(key));
                }
                Key = key;
            }
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

        private PBEAlphabeticalListEntry[] _list;
        public int Count => _list.Length;
        public T this[int index]
        {
            get
            {
                if (index >= _list.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _list[index].Key;
            }
        }

        internal PBEAlphabeticalList()
        {
            _list = Array.Empty<PBEAlphabeticalListEntry>();
        }
        internal PBEAlphabeticalList(IEnumerable<T> collection, object parameter = null)
        {
            Reset(collection, parameter: parameter);
        }

        private void Sort(PBEAlphabeticalListEntry[] old)
        {
            if (old == null || old == _list)
            {
                old = (PBEAlphabeticalListEntry[])_list.Clone();
            }
            Array.Sort(_list, (x, y) => x.Value.FromPBECultureInfo().CompareTo(y.Value.FromPBECultureInfo()));
            if (!_list.SequenceEqual(old))
            {
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        internal void Reset(IEnumerable<T> collection, object parameter = null)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            PBEAlphabeticalListEntry[] old = _list;
            if (collection is PBEAlphabeticalList<T> other)
            {
                _list = (PBEAlphabeticalListEntry[])other._list.Clone();
            }
            else
            {
                _list = collection.Select(t => new PBEAlphabeticalListEntry(t, parameter)).ToArray();
            }
            if (old != null && old.Length != _list.Length)
            {
                OnPropertyChanged(nameof(Count));
            }
            Sort(old);
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }
        public int IndexOf(T item)
        {
            if (item != null)
            {
                for (int i = 0; i < _list.Length; i++)
                {
                    if (item.Equals(_list[i].Key))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _list.Length; i++)
            {
                yield return _list[i].Key;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
