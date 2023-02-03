using Kermalis.EndianBinaryIO;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Data;

public sealed class PBEReadOnlyPokemonCollection : IPBEPokemonCollection, IPBEPokemonCollection<PBEReadOnlyPokemon>
{
	private readonly PBEReadOnlyPokemon[] _list;
	public int Count => _list.Length;
	public PBEReadOnlyPokemon this[int index]
	{
		get
		{
			if (index >= _list.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			return _list[index];
		}
	}
	IPBEPokemon IReadOnlyList<IPBEPokemon>.this[int index] => this[index];

	internal PBEReadOnlyPokemonCollection(EndianBinaryReader r)
	{
		byte count = r.ReadByte();
		_list = new PBEReadOnlyPokemon[count];
		for (int i = 0; i < count; i++)
		{
			_list[i] = new PBEReadOnlyPokemon(r);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _list.GetEnumerator();
	}
	IEnumerator<IPBEPokemon> IEnumerable<IPBEPokemon>.GetEnumerator()
	{
		return ((IEnumerable<PBEReadOnlyPokemon>)_list).GetEnumerator();
	}
	public IEnumerator<PBEReadOnlyPokemon> GetEnumerator()
	{
		return ((IEnumerable<PBEReadOnlyPokemon>)_list).GetEnumerator();
	}
}
