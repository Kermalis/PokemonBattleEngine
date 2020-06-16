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
                Server.DisconnectClient(this);
            }
            return receivedResponseInTime;
        }

        public void Send(IPBEPacket packet)
        {
            if (Client.IsConnected)
            {
                Debug.WriteLine($"Packet sent ({BattleId} {TrainerName} \"{packet.GetType().Name}\")");
                resetEvent.Reset();
                Client.Send(packet);
            }
        }
        private void PartySubmitted(IPBEPokemonCollection party)
        {
            Console.WriteLine($"Received party ({BattleId} {TrainerName})");
            if (!submittedTeam)
            {
                submittedTeam = true;
                Server.PartySubmitted(this, party);
            }
            else
            {
                Console.WriteLine("Party submitted multiple times!");
            }
        }
        private void OnPacketReceived(object sender, IPBEPacket packet)
        {
            Debug.WriteLine($"Packet received ({BattleId} {TrainerName} \"{packet.GetType().Name}\")");
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
                    case PBELegalPartyResponsePacket lprp:
                    {
                        PartySubmitted(lprp.Party);
                        break;
                    }
                    case PBEPartyResponsePacket prp:
                    {
                        if (Server.RequireLegalParties)
                        {
                            Console.WriteLine("An unchecked party was submitted, despite requiring legal parties. Ignoring...");
                        }
                        else
                        {
                            PartySubmitted(prp.Party);
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
