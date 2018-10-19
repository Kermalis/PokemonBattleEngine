using Ether.Network.Common;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Network;
using System.Diagnostics;
using System.Threading;

namespace Kermalis.PokemonBattleEngineServer
{
    sealed class Player : NetUser
    {
        public readonly ManualResetEvent ResetEvent = new ManualResetEvent(true);

        public PTeamShell Team;
        public PSubmitActionsPacket.Action[] Actions;
        
        public override void Send(INetPacketStream packet)
        {
            base.Send(packet);
            ResetEvent.Reset();
        }
        public override void HandleMessage(INetPacketStream packet)
        {
            Debug.WriteLine($"Message received: \"{packet.GetType().Name}\" ({Id})");

            var ser = (BattleServer)Server;
            switch (packet)
            {
                case PSubmitPartyPacket spp:
                    ser.PartySubmitted(this, spp.Team);
                    break;
                case PSubmitActionsPacket sap:
                    ser.ActionsSubmitted(this, sap.Actions);
                    break;
            }

            ResetEvent.Set();
        }
    }
}
