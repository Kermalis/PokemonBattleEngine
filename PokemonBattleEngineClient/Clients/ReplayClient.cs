using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngineClient.Views;
using System;

namespace Kermalis.PokemonBattleEngineClient.Clients
{
    internal sealed class ReplayClient : NonLocalClient
    {
        public override PBEBattle Battle { get; }
        public override PBETrainer Trainer => null;
        public override BattleView BattleView { get; }
        public override bool HideNonOwned => false;

        public ReplayClient(string path, string name) : base(name)
        {
            Battle = PBEBattle.LoadReplay(path);
            BattleView = new BattleView(this);
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

        protected override bool ProcessPacket(IPBEPacket packet)
        {
            switch (packet)
            {
                case PBEMovePPChangedPacket mpcp:
                {
                    PBEBattlePokemon moveUser = mpcp.MoveUserTrainer.TryGetPokemon(mpcp.MoveUser);
                    moveUser.Moves[mpcp.Move].PP -= mpcp.AmountReduced;
                    break;
                }
            }
            return base.ProcessPacket(packet);
        }
    }
}
