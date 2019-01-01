using Ether.Network.Common;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Kermalis.PokemonBattleEngineServer
{
    sealed class Player : NetUser
    {
        public readonly ManualResetEvent ResetEvent = new ManualResetEvent(true);
        public string PlayerName { get; set; }
        public byte Index { get; set; }
        public IEnumerable<PBEPokemonShell> Party { get; private set; }

        public override void Send(INetPacket packet)
        {
            base.Send(packet);
            ResetEvent.Reset();
        }
        public override void HandleMessage(INetPacket packet)
        {
            Debug.WriteLine($"Message received: \"{packet.GetType().Name}\" ({Id})");

            if (Index < 2)
            {
                var ser = (BattleServer)Server;
                switch (packet)
                {
                    case PBEActionsResponsePacket arp:
                        ser.ActionsSubmitted(this, arp.Actions);
                        break;
                    case PBEPartyResponsePacket prp:
                        Party = prp.Party;
                        ser.PartySubmitted(this);
                        break;
                    case PBESwitchInResponsePacket sirp:
                        ser.SwitchesSubmitted(this, sirp.Switches);
                        break;
                }
            }

            ResetEvent.Set();
        }
    }
}
