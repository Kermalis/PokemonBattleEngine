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

        private Type? _packetType;
        private Type? _actionType;
        private readonly ManualResetEvent _resetEvent = new(true);

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
            bool receivedResponseInTime = _resetEvent.WaitOne(1000 * 5);
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
                _resetEvent.Reset();
                Client.Send(packet);
            }
        }
        private IPBEPokemonCollection? _party;
        public IPBEPokemonCollection? AskForParty(bool requireLegal)
        {
            Send(new PBEPartyRequestPacket(BattleId, requireLegal));
            WaitForResponse(requireLegal ? typeof(PBELegalPartyResponsePacket) : typeof(PBEPartyResponsePacket));
            IPBEPokemonCollection? ret = _party;
            _party = null;
            return ret;
        }

        private void OnPacketReceived(object? sender, IPBEPacket packet)
        {
            // TODO: Kick players who are sending broken packets or sending too many
            Type type = packet.GetType();
            Debug.WriteLine($"Packet received ({BattleId} {TrainerName} \"{type.Name}\")");
            if (_packetType is not null && type.Equals(_packetType))
            {
                _packetType = null;
                switch (packet)
                {
                    case PBELegalPartyResponsePacket lprp:
                    {
                        Console.WriteLine($"Received party ({BattleId} {TrainerName})");
                        if (!Server.Settings.Equals(lprp.Party.Settings))
                        {
                            Console.WriteLine("Party does not have matching settings!");
                            Console.WriteLine("\tServer: \"{0}\"", Server.Settings);
                            Console.WriteLine("\tParty: \"{0}\"", lprp.Party.Settings);
                        }
                        else
                        {
                            _party = lprp.Party;
                            _resetEvent.Set();
                        }
                        break;
                    }
                    case PBEPartyResponsePacket prp:
                    {
                        Console.WriteLine($"Received party ({BattleId} {TrainerName})");
                        _party = prp.Party;
                        _resetEvent.Set();
                        break;
                    }
                    default: _resetEvent.Set(); break;
                }
            }
            else if (_actionType is not null && (type.Equals(_actionType) || type.Equals(typeof(PBEFleeResponsePacket))))
            {
                _actionType = null;
                switch (packet)
                {
                    case PBEActionsResponsePacket arp: Server.ActionsSubmitted(this, arp.Actions); break;
                    case PBEFleeResponsePacket _: Server.FleeSubmitted(this); break;
                    case PBESwitchInResponsePacket sirp: Server.SwitchesSubmitted(this, sirp.Switches); break;
                    default: throw new ArgumentOutOfRangeException(nameof(packet));
                }
            }
        }

        public void Dispose()
        {
            _resetEvent.Dispose();
            Client.PacketReceived -= OnPacketReceived;
        }
    }
}
