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
        public byte BattleId { get; }
        public string TrainerName { get; }
        public BattleServer Server { get; }
        public PBEServerClient Client { get; }

        private Type _packetType;
        private Type _actionType;
        private readonly ManualResetEvent resetEvent = new ManualResetEvent(true);

        public Player(BattleServer server, PBEServerClient client, byte battleId, string name)
        {
            BattleId = battleId;
            TrainerName = name;
            Server = server;
            Client = client;
            Client.PacketReceived += OnPacketReceived;
        }

        public bool WaitForResponse(Type packetType)
        {
            _packetType = packetType;
            bool receivedResponseInTime = resetEvent.WaitOne(1000 * 5);
            if (!receivedResponseInTime)
            {
                Server.DisconnectClient(this);
            }
            return receivedResponseInTime;
        }
        public void SetWaitingForActions(Type packetType)
        {
            _actionType = packetType;
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
        private IPBEPokemonCollection _party;
        public IPBEPokemonCollection AskForParty(bool requireLegal)
        {
            Send(new PBEPartyRequestPacket(BattleId, requireLegal));
            WaitForResponse(requireLegal ? typeof(PBELegalPartyResponsePacket) : typeof(PBEPartyResponsePacket));
            IPBEPokemonCollection ret = _party;
            _party = null;
            return ret;
        }

        private void OnPacketReceived(object sender, IPBEPacket packet)
        {
            // TODO: Kick players who are sending broken packets or sending too many
            Type type = packet.GetType();
            Debug.WriteLine($"Packet received ({BattleId} {TrainerName} \"{type.Name}\")");
            if (type.Equals(_packetType))
            {
                _packetType = null;
                switch (packet)
                {
                    case PBELegalPartyResponsePacket lprp:
                    {
                        Console.WriteLine($"Received party ({BattleId} {TrainerName})");
                        _party = lprp.Party;
                        resetEvent.Set();
                        break;
                    }
                    case PBEPartyResponsePacket prp:
                    {
                        Console.WriteLine($"Received party ({BattleId} {TrainerName})");
                        _party = prp.Party;
                        resetEvent.Set();
                        break;
                    }
                    default: resetEvent.Set(); break;
                }
            }
            else if (type.Equals(_actionType))
            {
                _actionType = null;
                switch (packet)
                {
                    case PBEActionsResponsePacket arp: Server.ActionsSubmitted(this, arp.Actions); break;
                    case PBESwitchInResponsePacket sirp: Server.SwitchesSubmitted(this, sirp.Switches); break;
                    default: throw new ArgumentOutOfRangeException(nameof(packet));
                }
            }
        }

        public void Dispose()
        {
            resetEvent.Dispose();
            Client.PacketReceived -= OnPacketReceived;
        }
    }
}
