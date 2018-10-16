using System;
using System.Net.Sockets;
using Ether.Network.Client;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Network;

namespace Kermalis.PokemonBattleEngineClient
{
    class BattleClient : NetClient
    {
        static readonly IPacketProcessor packetProcessor = new PPacketProcessor();
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

            switch (packet)
            {
                case PReadyUpPacket _:
                    Console.WriteLine("Sending team info...");
                    PTeamShell team1 = new PTeamShell
                    {
                        PlayerName = "Sasha",
                        Pokemon =
                        {
                            new PPokemonShell
                            {
                                Species = PSpecies.Azumarill,
                                Item = PItem.ChoiceBand,
                                Ability = PAbility.HugePower,
                                Nature = PNature.Adamant,
                                Gender = PGender.Male,
                                IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                                EVs = new byte[] { 252, 252, 0, 0, 0, 4 },
                                Moves = new PMove[] { PMove.Waterfall, PMove.AquaJet, PMove.Return, PMove.IcePunch },
                            }
                        }
                    };
                    Send(new PRequestTeamPacket(team1));
                    break;
            }
        }

        protected override void OnConnected()
        {
            Console.WriteLine("Connected to {0}", Socket.RemoteEndPoint);
        }
        protected override void OnDisconnected()
        {
            Console.WriteLine("Disconnected from server");
            Environment.Exit(0);
        }
        protected override void OnSocketError(SocketError socketError)
        {
            Console.WriteLine("Socket Error: {0}", socketError);
        }
    }
}
