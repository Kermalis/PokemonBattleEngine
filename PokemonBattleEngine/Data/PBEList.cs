using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEList<T> : INotifyCollectionChanged, INotifyPropertyChanged, IReadOnlyList<T>
    {
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

        private readonly List<T> _list;
        public int Count => _list.Count;
        public T this[int index]
        {
            get
            {
                if (index >= _list.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return _list[index];
            }
        }

        internal PBEList()
        {
            _list = new List<T>();
        }
        internal PBEList(int capacity)
        {
            _list = new List<T>(capacity);
        }

        internal void Add(T item)
        {
            int index = _list.Count;
            _list.Insert(index, item);
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }
        internal void Insert(int index, T item)
        {
            _list.Insert(index, item);
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged("Item[]");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }
        internal bool Remove(T item)
        {
            int index = _list.IndexOf(item);
            bool b = index != -1;
            if (b)
            {
                _list.RemoveAt(index);
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }
            return b;
        }
        internal void RemoveAt(int index)
        {
            if (index < 0 || index >= _list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            else
            {
                T item = _list[index];
                _list.RemoveAt(index);
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }
        }
        internal void Reset(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            int oldCount = _list.Count;
            if (!_list.SequenceEqual(collection))
            {
                _list.Clear();
                _list.AddRange(collection);
                if (oldCount != _list.Count)
                {
                    OnPropertyChanged(nameof(Count));
                }
                OnPropertyChanged("Item[]");
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public bool Contains(T item)
        {
            return _list.IndexOf(item) != -1;
        }
        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                yield return _list[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
