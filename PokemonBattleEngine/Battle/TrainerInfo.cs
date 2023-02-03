using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Legality;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle;

public abstract class PBETrainerInfoBase
{
	public ReadOnlyCollection<IPBEPokemon> Party { get; }
	private readonly PBESettings? _requiredSettings;

	protected PBETrainerInfoBase(IPBEPokemonCollection party)
	{
		if (party is IPBEPartyPokemonCollection ppc)
		{
			if (!ppc.Any(p => p.HP > 0 && !p.PBEIgnore))
			{
				throw new ArgumentException("Party must have at least 1 conscious battler", nameof(party));
			}
		}
		else
		{
			if (!party.Any(p => !p.PBEIgnore))
			{
				throw new ArgumentException("Party must have at least 1 conscious battler", nameof(party));
			}
		}
		if (party is PBELegalPokemonCollection lp)
		{
			_requiredSettings = lp.Settings;
		}
		Party = new ReadOnlyCollection<IPBEPokemon>(party.ToArray());
	}

	public bool IsOkayForSettings(PBESettings settings)
	{
		settings.ShouldBeReadOnly(nameof(settings));
		if (_requiredSettings is not null)
		{
			return settings.Equals(_requiredSettings);
		}
		if (this is PBETrainerInfo ti && ti.GainsEXP && settings.MaxLevel > 100)
		{
			throw new ArgumentException("Cannot start a battle with EXP enabled and a higher MaxLevel than 100. Not supported.");
		}
		return true;
	}
}
public sealed class PBETrainerInfo : PBETrainerInfoBase
{
	public string Name { get; }
	public bool GainsEXP { get; }
	public ReadOnlyCollection<(PBEItem Item, uint Quantity)> Inventory { get; }

	public PBETrainerInfo(IPBEPokemonCollection party, string name, bool gainsEXP, IList<(PBEItem Item, uint Quantity)>? inventory = null)
		: base(party)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentOutOfRangeException(nameof(name));
		}
		Name = name;
		GainsEXP = gainsEXP;
		if (inventory is null || inventory.Count == 0)
		{
			Inventory = PBEEmptyReadOnlyCollection<(PBEItem, uint)>.Value;
		}
		else
		{
			Inventory = new ReadOnlyCollection<(PBEItem, uint)>(inventory);
		}
	}
}
public sealed class PBEWildInfo : PBETrainerInfoBase
{
	public PBEWildInfo(IPBEPokemonCollection party)
		: base(party) { }
}
