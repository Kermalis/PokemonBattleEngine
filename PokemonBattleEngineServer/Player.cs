using Ether.Network.Common;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Diagnostics;
using System.Threading;

namespace Kermalis.PokemonBattleEngineServer
{
    internal sealed class Player : NetUser
    {
        public ManualResetEvent ResetEvent { get; } = new ManualResetEvent(true);
        public string TrainerName { get; set; }
        public int BattleId { get; set; } = int.MaxValue;
        public PBETeamShell TeamShell { get; private set; }

        public bool WaitForResponse()
        {
            bool receivedResponseInTime = ResetEvent.WaitOne(1000 * 5);
            if (!receivedResponseInTime)
            {
                Console.WriteLine($"Kicking client ({BattleId} {TrainerName})");
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
            Debug.WriteLine($"Message received: \"{packet.GetType().Name}\" ({BattleId})");
            if (Socket == null || ResetEvent.SafeWaitHandle.IsClosed)
            {
                return;
            }
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
                        Console.WriteLine($"Received team from {TrainerName}!");
                        if (TeamShell == null)
                        {
                            TeamShell = prp.TeamShell;
                            ser.PartySubmitted(this);
                        }
                        else
                        {
                            Console.WriteLine("Team submitted multiple times!");
                        }
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
