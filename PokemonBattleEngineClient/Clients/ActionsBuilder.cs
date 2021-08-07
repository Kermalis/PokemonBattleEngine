using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngineClient.Clients
{
    internal sealed class ActionsBuilder
    {
        private readonly BattleView _bv;
        private Action<Queue<PBETurnAction>> _onActionsReady;
        private int _index;
        private readonly PBEBattlePokemon[] _pkmn;
        private readonly Queue<PBETurnAction> _actions;
        private readonly Queue<PBEBattlePokemon?> _standBy;

        public ActionsBuilder(BattleView bv, PBETrainer trainer, Action<Queue<PBETurnAction>> onActionsReady)
        {
            _bv = bv;
            bv.Actions.ActionsBuilder = this;
            _onActionsReady = onActionsReady;
            _pkmn = trainer.ActiveBattlersOrdered.ToArray();
            _actions = new Queue<PBETurnAction>(_pkmn.Length);
            _standBy = new Queue<PBEBattlePokemon?>(_pkmn.Length);
            ActionsLoop();
        }

        public bool IsStandBy(PBEBattlePokemon p)
        {
            return _standBy.Contains(p);
        }

        public void EnqueueMove(PBEMove move, PBETurnTarget targets)
        {
            PBEBattlePokemon pkmn = _pkmn[_index];
            var a = new PBETurnAction(pkmn, move, targets);
            pkmn.TurnAction = a;
            _actions.Enqueue(a);
            _standBy.Enqueue(null);
            _index++;
            ActionsLoop();
        }
        public void EnqueueSwitch(PBEBattlePokemon switcher)
        {
            PBEBattlePokemon pkmn = _pkmn[_index];
            var a = new PBETurnAction(pkmn, switcher);
            pkmn.TurnAction = a;
            _actions.Enqueue(a);
            _standBy.Enqueue(pkmn);
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
                _bv.Actions.DisplayActions(pkmn);
            }
        }
    }
}
