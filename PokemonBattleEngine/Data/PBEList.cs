using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data;

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
	public event NotifyCollectionChangedEventHandler? CollectionChanged;
	public event PropertyChangedEventHandler? PropertyChanged;

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
	internal PBEList(IEnumerable<T> collection)
	{
		_list = new List<T>(collection);
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
	internal void Swap(T a, T b)
	{
		int aIndex = IndexOf(a);
		if (aIndex == -1)
		{
			throw new ArgumentOutOfRangeException(nameof(a));
		}
		int bIndex = IndexOf(b);
		if (bIndex == -1)
		{
			throw new ArgumentOutOfRangeException(nameof(b));
		}
		_list[aIndex] = b;
		_list[bIndex] = a;
		OnPropertyChanged("Item[]");
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, a, b, bIndex));
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, b, a, aIndex));
	}

	public bool Contains(T item)
	{
		return _list.IndexOf(item) != -1;
	}
	public List<T> FindAll(Predicate<T> match)
	{
		return _list.FindAll(match);
	}
	public int IndexOf(T item)
	{
		return _list.IndexOf(item);
	}

	public T[] ToArray()
	{
		return _list.ToArray();
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
		return ((IEnumerable<T>)this).GetEnumerator();
	}
}
