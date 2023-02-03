using System;
using System.Collections.ObjectModel;

namespace Kermalis.PokemonBattleEngine.Utils;

internal static class PBEEmptyReadOnlyCollection<T>
{
	public static readonly ReadOnlyCollection<T> Value = new(Array.Empty<T>());
}
