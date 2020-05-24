using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System.Threading;

namespace Kermalis.PokemonBattleEngineClient
{
    internal sealed class SinglePlayerClient : BattleClient
    {
        public SinglePlayerClient(PBEBattleFormat battleFormat, PBETeamShell team1Shell, string team1TrainerName, PBETeamShell team2Shell, string team2TrainerName)
            : base(new PBEBattle(PBERandom.RandomBattleTerrain(), battleFormat, team1Shell, team1TrainerName, team2Shell, team2TrainerName), ClientMode.SinglePlayer)
        {
            BattleId = 0;
            Team = Battle.Teams[0];
            ShowRawValues0 = true;
            ShowRawValues1 = false;
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
                case PBEBattleState.ReadyToRunTurn:
                {
                    battle.RunTurn();
                    break;
                }
            }
        }

        protected override void OnActionsReady(PBETurnAction[] acts)
        {
            if (!Battle.IsDisposed)
            {
                new Thread(() => PBEBattle.SelectActionsIfValid(Team, acts)) { Name = "Battle Thread" }.Start();
            }
        }
        protected override void OnSwitchesReady()
        {
            if (!Battle.IsDisposed)
            {
                new Thread(() => PBEBattle.SelectSwitchesIfValid(Team, Switches)) { Name = "Battle Thread" }.Start();
            }
        }

        public override void Dispose()
        {
            Battle.Dispose();
            Battle.OnNewEvent -= SinglePlayerBattle_OnNewEvent;
            Battle.OnStateChanged -= SinglePlayerBattle_OnStateChanged;
        }
    }
}
