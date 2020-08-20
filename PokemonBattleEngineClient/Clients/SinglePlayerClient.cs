using Kermalis.PokemonBattleEngine.AI;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngineClient.Views;
using System.Threading;

namespace Kermalis.PokemonBattleEngineClient.Clients
{
    internal sealed class SinglePlayerClient : BattleClient
    {
        private const string ThreadName = "Battle Thread";
        public override PBEBattle Battle { get; }
        public override PBETrainer Trainer { get; }
        public override BattleView BattleView { get; }
        public override bool HideNonOwned => true;

        public SinglePlayerClient(PBEBattle b, string name) : base(name)
        {
            Battle = b;
            Trainer = b.Trainers[0];
            BattleView = new BattleView(this);
            b.OnNewEvent += SinglePlayerBattle_OnNewEvent;
            b.OnStateChanged += SinglePlayerBattle_OnStateChanged;
            new Thread(b.Begin) { Name = ThreadName }.Start();
        }

        private void SinglePlayerBattle_OnNewEvent(PBEBattle battle, IPBEPacket packet)
        {
            if (!ProcessPacket(packet))
            {
                Thread.Sleep(WaitMilliseconds);
            }
        }
        private void SinglePlayerBattle_OnStateChanged(PBEBattle battle)
        {
            switch (battle.BattleState)
            {
                case PBEBattleState.Ended: battle.SaveReplay("SinglePlayer Battle.pbereplay"); break;
                case PBEBattleState.ReadyToRunSwitches: new Thread(battle.RunSwitches) { Name = ThreadName }.Start(); break;
                case PBEBattleState.ReadyToRunTurn: new Thread(battle.RunTurn) { Name = ThreadName }.Start(); break;
            }
        }

        protected override void OnActionsReady(PBETurnAction[] acts)
        {
            new Thread(() => PBEBattle.SelectActionsIfValid(Trainer, acts)) { Name = ThreadName }.Start();
        }
        protected override void OnSwitchesReady()
        {
            new Thread(() => PBEBattle.SelectSwitchesIfValid(Trainer, Switches)) { Name = ThreadName }.Start();
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
                        new Thread(() => PBEBattle.SelectActionsIfValid(t, PBEAI.CreateActions(t))) { Name = ThreadName }.Start();
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
                        new Thread(() => PBEBattle.SelectSwitchesIfValid(t, PBEAI.CreateSwitches(t))) { Name = ThreadName }.Start();
                    }
                    return true;
                }
            }
            return base.ProcessPacket(packet);
        }
    }
}
