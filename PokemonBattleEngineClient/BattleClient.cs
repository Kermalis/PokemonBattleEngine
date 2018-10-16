using System;
using System.Net.Sockets;
using Ether.Network.Client;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;

namespace Kermalis.PokemonBattleEngineClient
{
    class BattleClient : NetClient
    {
        static readonly IPacketProcessor packetProcessor = new PacketProcessor();
        protected override IPacketProcessor PacketProcessor => packetProcessor;

        public BattleClient(string host)
        {
            Configuration.Host = host;
            Configuration.Port = 8888;
            Configuration.BufferSize = 1024;
        }

        public override void HandleMessage(INetPacketStream packet)
        {
            Console.WriteLine($"Message received: \"{packet.GetType().Name}\"");

            if (packet is ReadyUpPacket)
            {
                Console.WriteLine("Sending team info...");
                TeamShell team1 = new TeamShell
                {
                    Pokemon =
                    {
                        new PokemonShell
                        {
                            Species = PSpecies.Azumarill,
                            Item = PItem.ChoiceBand,
                            Ability = PAbility.HugePower,
                            Nature = PNature.Adamant,
                            IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                            EVs = new byte[] { 252, 252, 0, 0, 0, 4 },
                            Moves = new PMove[] { PMove.Waterfall, PMove.AquaJet, PMove.Return, PMove.IcePunch },
                        }
                    },
                    PlayerName = "Sasha"
                };
                using (var pack = new RequestTeamPacket(team1))
                    Send(pack);
            }
        }

        protected override void OnConnected()
        {
            Console.WriteLine("Connected to {0}", Socket.RemoteEndPoint);
        }
        protected override void OnDisconnected()
        {
            Console.WriteLine("Disconnected from server");
        }
        protected override void OnSocketError(SocketError socketError)
        {
            Console.WriteLine("Socket Error: {0}", socketError);
        }
    }
}
