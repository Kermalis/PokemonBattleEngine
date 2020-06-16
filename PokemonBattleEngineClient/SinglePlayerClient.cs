using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System.Threading;

namespace Kermalis.PokemonBattleEngineClient
{
    internal sealed class SinglePlayerClient : BattleClient
    {
        public SinglePlayerClient(PBEBattleFormat battleFormat, PBETeamInfo ti0, PBETeamInfo ti1, PBESettings settings)
            : base(new PBEBattle(PBERandom.RandomBattleTerrain(), battleFormat, ti0, ti1, settings), ClientMode.SinglePlayer)
        {
            BattleId = 0;
            Team = Battle.Teams[0];
            ShowEverything0 = true;
            ShowEverything1 = false;
            Battle.OnNewEvent += SinglePlayerBattle_OnNewEvent;
            Battle.OnStateChanged += SinglePlayerBattle_OnStateChanged;
            new Thread(Battle.Begin) { Name = "Battle Thread" }.Start();
        }

        private void SinglePlayerBattle_OnNewEvent(PBEBattle battle, IPBEPacket packet)
        {
            if (!ProcessPacket(packet))
            {
                Thread.Sleep(WaitMilliseconds);
            }
        }
        // Called from Battle Thread
        private void SinglePlayerBattle_OnStateChanged(PBEBattle battle)
        {
            switch (battle.BattleState)
            {
                case PBEBattleState.Ended: battle.SaveReplay("SinglePlayer Battle.pbereplay"); break;
                case PBEBattleState.ReadyToRunSwitches: battle.RunSwitches(); break;
                case PBEBattleState.ReadyToRunTurn: battle.RunTurn(); break;
            }
        }

        protected override void OnActionsReady(PBETurnAction[] acts)
        {
            new Thread(() => PBEBattle.SelectActionsIfValid(Team, acts)) { Name = "Battle Thread" }.Start();
        }
        protected override void OnSwitchesReady()
        {
            new Thread(() => PBEBattle.SelectSwitchesIfValid(Team, Switches)) { Name = "Battle Thread" }.Start();
        }

        public override void Dispose()
        {
            base.Dispose();
            Battle.SetEnded(); // Events unsubscribed
        }
    }
}
