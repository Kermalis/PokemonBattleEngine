using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Views;
using System;
using System.Linq;

namespace Kermalis.PokemonBattleEngineClient.Clients;

internal sealed class ActionsBuilder
{
	private readonly BattleView _bv;
	private Action<PBETurnAction[]> _onActionsReady;

	private int _index;
	private readonly PBEBattlePokemon[] _pkmn;
	private readonly PBETurnAction[] _actions;
	private readonly PBEBattlePokemon?[] _standBy;

	public ActionsBuilder(BattleView bv, PBETrainer trainer, Action<PBETurnAction[]> onActionsReady)
	{
		_bv = bv;
		bv.Actions.ActionsBuilder = this;
		_onActionsReady = onActionsReady;
		_pkmn = trainer.ActiveBattlersOrdered.ToArray();
		_actions = new PBETurnAction[_pkmn.Length];
		_standBy = new PBEBattlePokemon?[_pkmn.Length];
		ActionsLoop();
	}

	public bool IsStandBy(PBEBattlePokemon p)
	{
		int i = Array.IndexOf(_standBy, p);
		return i != -1 && i < _index;
	}

	public void Pop()
	{
		_index--;
		ActionsLoop();
	}
	public void PushMove(PBEMove move, PBETurnTarget targets)
	{
		PBEBattlePokemon pkmn = _pkmn[_index];
		var a = new PBETurnAction(pkmn, move, targets);
		pkmn.TurnAction = a;
		_actions[_index] = a;
		_standBy[_index] = null;
		_index++;
		ActionsLoop();
	}
	public void PushSwitch(PBEBattlePokemon switcher)
	{
		PBEBattlePokemon pkmn = _pkmn[_index];
		var a = new PBETurnAction(pkmn, switcher);
		pkmn.TurnAction = a;
		_actions[_index] = a;
		_standBy[_index] = switcher;
		_index++;
		ActionsLoop();
	}

	private void ActionsLoop()
	{
		if (_index == _pkmn.Length)
		{
			_bv.Actions.ActionsBuilder = null;
			_onActionsReady(_actions);
			_onActionsReady = null!;
		}
		else
		{
			PBEBattlePokemon pkmn = _pkmn[_index];
			_bv.AddMessage($"What will {pkmn.Nickname} do?", messageLog: false);
			_bv.Actions.DisplayActions(_index, pkmn);
		}
	}
}
