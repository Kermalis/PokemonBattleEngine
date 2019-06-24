using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEReadOnlyObservableCollection<T> : IReadOnlyList<T>, INotifyCollectionChanged
    {
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly List<T> list;
        public int Count => list.Count;
        public T this[int index] => list[index];

        internal PBEReadOnlyObservableCollection(List<T> list)
        {
            this.list = list;
        }

        internal void Add(T item)
        {
            int index = list.Count;
            list.Insert(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }
        internal bool Remove(T item)
        {
            int index = list.IndexOf(item);
            bool b = index != -1;
            if (b)
            {
                list.RemoveAt(index);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }
            return b;
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
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
