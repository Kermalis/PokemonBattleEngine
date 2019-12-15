using Kermalis.PokemonBattleEngine.Battle;
using System;

namespace Kermalis.PokemonBattleEngineClient
{
    internal sealed class ReplayClient : BattleClient
    {
        public ReplayClient(string path)
            : base(PBEBattle.LoadReplay(path), ClientMode.Replay)
        {
            ShowRawValues0 = true;
            ShowRawValues1 = true;
            StartPacketThread();
        }

        protected override void OnActionsReady(PBETurnAction[] acts)
        {
            throw new NotImplementedException();
        }
        protected override void OnSwitchesReady()
        {
            throw new NotImplementedException();
        }
    }
}
