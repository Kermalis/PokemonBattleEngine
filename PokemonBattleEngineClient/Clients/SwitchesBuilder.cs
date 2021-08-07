using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngineClient.Views;
using System;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngineClient.Clients
{
    internal sealed class SwitchesBuilder
    {
        private readonly BattleView _bv;
        private Action<Queue<PBESwitchIn>> _onSwitchesReady;
        public byte SwitchesRequired;
        private int _index;
        private readonly Queue<PBESwitchIn> _switches;
        private readonly Queue<PBEBattlePokemon> _standBy;
        private readonly Queue<PBEFieldPosition> _positionStandBy;

        public SwitchesBuilder(BattleView bv, byte amount, Action<Queue<PBESwitchIn>> onSwitchesReady)
        {
            _bv = bv;
            bv.Actions.SwitchesBuilder = this;
            _onSwitchesReady = onSwitchesReady;
            SwitchesRequired = amount;
            _switches = new Queue<PBESwitchIn>(amount);
            _standBy = new Queue<PBEBattlePokemon>(amount);
            _positionStandBy = new Queue<PBEFieldPosition>(amount);
            SwitchesLoop();
        }

        public bool IsStandBy(PBEBattlePokemon p)
        {
            return _standBy.Contains(p);
        }
        public bool IsStandBy(PBEFieldPosition p)
        {
            return _positionStandBy.Contains(p);
        }

        public void Enqueue(PBEBattlePokemon pkmn, PBEFieldPosition pos)
        {
            _switches.Enqueue(new PBESwitchIn(pkmn, pos));
            _standBy.Enqueue(pkmn);
            _positionStandBy.Enqueue(pos);
            _index++;
            SwitchesLoop();
        }

        private void SwitchesLoop()
        {
            if (_index == SwitchesRequired)
            {
                _bv.Actions.SwitchesBuilder = null;
                _onSwitchesReady(_switches);
                _onSwitchesReady = null!;
            }
            else
            {
                _bv.AddMessage($"You must send in {SwitchesRequired - _index} Pokémon.", messageLog: false);
                _bv.Actions.DisplaySwitches();
            }
        }
    }
}
