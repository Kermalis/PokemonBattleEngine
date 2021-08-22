using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngineClient.Views;
using System;

namespace Kermalis.PokemonBattleEngineClient.Clients
{
    internal sealed class SwitchesBuilder
    {
        private readonly BattleView _bv;
        private Action<PBESwitchIn[]> _onSwitchesReady;
        public readonly byte SwitchesRequired;

        private int _index;
        private readonly PBESwitchIn[] _switches;
        private readonly PBEBattlePokemon[] _standBy;
        private readonly PBEFieldPosition[] _positionStandBy;

        public SwitchesBuilder(BattleView bv, byte amount, Action<PBESwitchIn[]> onSwitchesReady)
        {
            _bv = bv;
            bv.Actions.SwitchesBuilder = this;
            _onSwitchesReady = onSwitchesReady;
            SwitchesRequired = amount;
            _switches = new PBESwitchIn[amount];
            _standBy = new PBEBattlePokemon[amount];
            _positionStandBy = new PBEFieldPosition[amount];
            SwitchesLoop();
        }

        public bool IsStandBy(PBEBattlePokemon p)
        {
            int i = Array.IndexOf(_standBy, p);
            return i != -1 && i < _index;
        }
        public bool IsStandBy(PBEFieldPosition p)
        {
            int i = Array.IndexOf(_positionStandBy, p);
            return i != -1 && i < _index;
        }

        public void Pop()
        {
            _index--;
            SwitchesLoop();
        }
        public void Push(PBEBattlePokemon pkmn, PBEFieldPosition pos)
        {
            _switches[_index] = new PBESwitchIn(pkmn, pos);
            _standBy[_index] = pkmn;
            _positionStandBy[_index] = pos;
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
                _bv.Actions.DisplaySwitches(_index);
            }
        }
    }
}
