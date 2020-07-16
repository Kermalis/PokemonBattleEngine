using Kermalis.PokemonBattleEngine.AI;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using Kermalis.PokemonBattleEngineClient.Views;
using System.Collections.Generic;
using System.Threading;

namespace Kermalis.PokemonBattleEngineClient.Clients
{
    internal sealed class SinglePlayerClient : BattleClient
    {
        public override PBEBattle Battle { get; }
        public override PBETrainer Trainer { get; }
        public override BattleView BattleView { get; }
        public override bool HideNonOwned => true;

        public SinglePlayerClient(PBEBattleFormat battleFormat, PBESettings settings, IReadOnlyList<PBETrainerInfo> ti0, IReadOnlyList<PBETrainerInfo> ti1, string name) : base(name)
        {
            var b = new PBEBattle(battleFormat, settings, ti0, ti1,
                battleTerrain: PBEUtils.GlobalRandom.RandomBattleTerrain());
            Battle = b;
            Trainer = b.Trainers[0];
            BattleView = new BattleView(this);
            b.OnNewEvent += SinglePlayerBattle_OnNewEvent;
            b.OnStateChanged += SinglePlayerBattle_OnStateChanged;
            new Thread(b.Begin) { Name = "Battle Thread" }.Start();
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
            new Thread(() => PBEBattle.SelectActionsIfValid(Trainer, acts)) { Name = "Battle Thread" }.Start();
        }
        protected override void OnSwitchesReady()
        {
            new Thread(() => PBEBattle.SelectSwitchesIfValid(Trainer, Switches)) { Name = "Battle Thread" }.Start();
        }

        public override void Dispose()
        {
            Battle.SetEnded(); // Events unsubscribed
        }

        protected override bool ProcessPacket(IPBEPacket packet)
        {
            switch (packet)
            {
                case PBEActionsRequestPacket arp:
                {
                    PBETrainer t = arp.Trainer;
                    if (t == Trainer)
                    {
                        ActionsLoop(true);
                    }
                    else
                    {
                        new Thread(() => PBEBattle.SelectActionsIfValid(t, PBEAI.CreateActions(t))) { Name = "Battle Thread" }.Start();
                    }
                    return true;
                }
                case PBESwitchInRequestPacket sirp:
                {
                    PBETrainer t = sirp.Trainer;
                    if (t == Trainer)
                    {
                        _switchesRequired = sirp.Amount;
                        SwitchesLoop(true);
                    }
                    else
                    {
                        new Thread(() => PBEBattle.SelectSwitchesIfValid(t, PBEAI.CreateSwitches(t))) { Name = "Battle Thread" }.Start();
                    }
                    return true;
                }
            }
            return base.ProcessPacket(packet);
        }
    }
}
