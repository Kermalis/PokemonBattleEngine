using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.DefaultData.AI;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngineClient.Views;
using System.Threading;

namespace Kermalis.PokemonBattleEngineClient.Clients
{
    internal sealed class SinglePlayerClient : BattleClient
    {
        public override PBEBattle Battle { get; }
        public override PBETrainer? Trainer { get; }
        public override BattleView BattleView { get; }
        public override bool HideNonOwned => true;
        private readonly PBEDDAI[] _ais;

        public SinglePlayerClient(PBEBattle b, string name) : base(name)
        {
            Battle = b;
            Trainer = b.Trainers[0];
            BattleView = new BattleView(this);
            b.OnNewEvent += SinglePlayerBattle_OnNewEvent;
            b.OnStateChanged += SinglePlayerBattle_OnStateChanged;
            _ais = new PBEDDAI[b.Trainers.Count - 1];
            for (int i = 0; i < _ais.Length; i++)
            {
                _ais[i] = new PBEDDAI(b.Trainers[i + 1]);
            }
            ShowAllPokemon();
            CreateBattleThread(b.Begin);
        }
        private static void CreateBattleThread(ThreadStart start)
        {
            new Thread(start) { Name = "Battle Thread" }.Start();
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
                case PBEBattleState.ReadyToRunSwitches: CreateBattleThread(battle.RunSwitches); break;
                case PBEBattleState.ReadyToRunTurn: CreateBattleThread(battle.RunTurn); break;
            }
        }

        private void OnActionsReady(PBETurnAction[] acts)
        {
            CreateBattleThread(() => Trainer!.SelectActionsIfValid(out _, acts));
        }
        private void OnSwitchesReady(PBESwitchIn[] switches)
        {
            CreateBattleThread(() => Trainer!.SelectSwitchesIfValid(out _, switches));
        }

        private PBEDDAI GetAI(PBETrainer t)
        {
            return _ais[t.Id - 1];
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
                        _ = new ActionsBuilder(BattleView, Trainer, OnActionsReady);
                    }
                    else
                    {
                        CreateBattleThread(GetAI(t).CreateActions);
                    }
                    return true;
                }
                case PBESwitchInRequestPacket sirp:
                {
                    PBETrainer t = sirp.Trainer;
                    if (t == Trainer)
                    {
                        _ = new SwitchesBuilder(BattleView, sirp.Amount, OnSwitchesReady);
                    }
                    else
                    {
                        CreateBattleThread(GetAI(t).CreateAISwitches);
                    }
                    return true;
                }
            }
            return base.ProcessPacket(packet);
        }
    }
}
