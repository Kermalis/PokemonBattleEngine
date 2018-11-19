using Ether.Network.Common;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System.Diagnostics;
using System.Threading;

namespace Kermalis.PokemonBattleEngineServer
{
    sealed class Player : NetUser
    {
        public readonly ManualResetEvent ResetEvent = new ManualResetEvent(true);

        public PTeamShell Shell;

        public override void Send(INetPacket packet)
        {
            base.Send(packet);
            ResetEvent.Reset();
        }
        public override void HandleMessage(INetPacket packet)
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
