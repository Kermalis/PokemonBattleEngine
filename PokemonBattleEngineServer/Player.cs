using Ether.Network.Common;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Kermalis.PokemonBattleEngineServer
{
    class Player : NetUser
    {
        public ManualResetEvent ResetEvent { get; } = new ManualResetEvent(true);
        public string PlayerName { get; set; }
        public int BattleId { get; set; } = int.MaxValue;
        public IEnumerable<PBEPokemonShell> Party { get; private set; }

        public bool WaitForResponse()
        {
            bool receivedResponseInTime = ResetEvent.WaitOne(1000 * 5);
            if (!receivedResponseInTime)
            {
                Console.WriteLine($"Kicking client ({BattleId} {PlayerName})");
                Server.DisconnectClient(Id);
            }
            return receivedResponseInTime;
        }

        public override void Send(INetPacket packet)
        {
            ResetEvent.Reset();
            base.Send(packet);
        }
        public override void HandleMessage(INetPacket packet)
        {
            if (Socket == null)
            {
                return;
            }
            Debug.WriteLine($"Message received: \"{packet.GetType().Name}\" ({BattleId})");
            ResetEvent.Set();

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
        }
    }
}
