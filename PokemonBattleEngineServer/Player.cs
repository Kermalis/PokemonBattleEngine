using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Network;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Diagnostics;
using System.Threading;

namespace Kermalis.PokemonBattleEngineServer
{
    internal sealed class Player : IDisposable
    {
        public BattleServer Server { get; }
        public PBEServerClient Client { get; }
        public string TrainerName { get; set; }
        public int BattleId { get; set; } = int.MaxValue;

        private readonly ManualResetEvent resetEvent = new ManualResetEvent(true);
        private bool submittedTeam;

        public Player(BattleServer server, PBEServerClient client)
        {
            Server = server;
            Client = client;
            Client.PacketReceived += OnPacketReceived;
        }

        public bool WaitForResponse()
        {
            bool receivedResponseInTime = resetEvent.WaitOne(1000 * 5);
            if (!receivedResponseInTime)
            {
                Console.WriteLine($"Kicking client ({BattleId} {TrainerName})");
                Server.DisconnectClient(this);
            }
            return receivedResponseInTime;
        }

        public void Send(IPBEPacket packet)
        {
            if (Client.IsConnected)
            {
                Debug.WriteLine($"Packet sent ({BattleId} \"{packet.GetType().Name}\")");
                resetEvent.Reset();
                Client.Send(packet);
            }
        }
        private void OnPacketReceived(object sender, IPBEPacket packet)
        {
            Debug.WriteLine($"Packet received ({BattleId} \"{packet.GetType().Name}\")");
            resetEvent.Set();
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
                        if (!submittedTeam)
                        {
                            submittedTeam = true;
                            PBETeamShell s = prp.TeamShell;
                            Server.PartySubmitted(this, s);
                            s.Dispose();
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
            // TODO: Kick players who are sending bogus packets or sending too many
        }

        public void Dispose()
        {
            resetEvent.Dispose();
            Client.PacketReceived -= OnPacketReceived;
        }
    }
}
