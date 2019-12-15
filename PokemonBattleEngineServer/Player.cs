using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Network;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Diagnostics;
using System.Threading;

namespace Kermalis.PokemonBattleEngineServer
{
    internal sealed class Player
    {
        public BattleServer Server { get; }
        public PBEServerClient Client { get; }
        public ManualResetEvent ResetEvent { get; } = new ManualResetEvent(true);
        public string TrainerName { get; set; }
        public int BattleId { get; set; } = int.MaxValue;
        public PBETeamShell TeamShell { get; private set; }

        public Player(BattleServer server, PBEServerClient client)
        {
            Server = server;
            Client = client;
            Client.MessageReceived += HandleMessage;
        }

        public bool WaitForResponse()
        {
            bool receivedResponseInTime = ResetEvent.WaitOne(1000 * 5);
            if (!receivedResponseInTime)
            {
                Console.WriteLine($"Kicking client ({BattleId} {TrainerName})");
                Server.DisconnectClient(this);
            }
            return receivedResponseInTime;
        }

        public void Send(IPBEPacket packet)
        {
            ResetEvent.Reset();
            Client.Send(packet);
        }
        private void HandleMessage(object sender, IPBEPacket packet)
        {
            Debug.WriteLine($"Message received: \"{packet.GetType().Name}\" ({BattleId})");
            if (Client == null || ResetEvent.SafeWaitHandle.IsClosed)
            {
                return;
            }
            ResetEvent.Set();

            if (BattleId < 2)
            {
                switch (packet)
                {
                    case PBEActionsResponsePacket arp:
                    {
                        Server.ActionsSubmitted(this, arp.Actions);
                        break;
                    }
                    case PBEPartyResponsePacket prp:
                    {
                        Console.WriteLine($"Received team from {TrainerName}!");
                        if (TeamShell == null)
                        {
                            TeamShell = prp.TeamShell;
                            Server.PartySubmitted(this);
                        }
                        else
                        {
                            Console.WriteLine("Team submitted multiple times!");
                        }
                        break;
                    }
                    case PBESwitchInResponsePacket sirp:
                    {
                        Server.SwitchesSubmitted(this, sirp.Switches);
                        break;
                    }
                }
            }
        }
    }
}
