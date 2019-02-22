using Ether.Network.Common;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Kermalis.PokemonBattleEngineServer
{
    class Player : NetUser
    {
        public ManualResetEvent ResetEvent { get; } = new ManualResetEvent(true);
        public string PlayerName { get; set; }
        public int BattleId { get; set; }
        public IEnumerable<PBEPokemonShell> Party { get; private set; }

        public override void Send(INetPacket packet)
        {
            ResetEvent.Reset();
            base.Send(packet);
        }
        public override void HandleMessage(INetPacket packet)
        {
            Debug.WriteLine($"Message received: \"{packet.GetType().Name}\" ({Id})");

            if (BattleId < 2)
            {
                var ser = (BattleServer)Server;
                switch (packet)
                {
                    case PBEActionsResponsePacket arp:
                        {
                            ser.ActionsSubmitted(this, arp.Actions);
                            break;
                        }
                    case PBEPartyResponsePacket prp:
                        {
                            Party = prp.Party;
                            ser.PartySubmitted(this);
                            break;
                        }
                    case PBESwitchInResponsePacket sirp:
                        {
                            ser.SwitchesSubmitted(this, sirp.Switches);
                            break;
                        }
                }
            }

            ResetEvent.Set();
        }
    }
}
