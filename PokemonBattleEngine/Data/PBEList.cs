using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEList<T> : INotifyCollectionChanged, INotifyPropertyChanged, IReadOnlyList<T>
    {
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

        private readonly List<T> list;
        public int Count => list.Count;
        public T this[int index] => list[index];

        internal PBEList()
        {
            list = new List<T>();
        }
        internal PBEList(int capacity)
        {
            list = new List<T>(capacity);
        }

        internal void Add(T item)
        {
            int index = list.Count;
            list.Insert(index, item);
            FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }
        internal void Insert(int index, T item)
        {
            list.Insert(index, item);
            FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }
        internal bool Remove(T item)
        {
            int index = list.IndexOf(item);
            bool b = index != -1;
            if (b)
            {
                list.RemoveAt(index);
                FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }
            return b;
        }
        internal void RemoveAt(int index)
        {
            if (index < 0 || index >= list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            else
            {
                T item = list[index];
                list.RemoveAt(index);
                FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }
        }
        internal void Reset(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            list.Clear();
            list.AddRange(collection);
            FireEvents(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return list.IndexOf(item) != -1;
        }
        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
