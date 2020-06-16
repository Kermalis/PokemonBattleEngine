using Kermalis.PokemonBattleEngine.Battle;
using System;

namespace Kermalis.PokemonBattleEngineClient
{
    internal sealed class ReplayClient : BattleClient
    {
        public ReplayClient(string path)
            : base(PBEBattle.LoadReplay(path), ClientMode.Replay)
        {
            ShowEverything0 = true;
            ShowEverything1 = true;
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
