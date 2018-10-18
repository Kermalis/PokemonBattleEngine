using System;
using System.Net.Sockets;
using Ether.Network.Client;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Network;

namespace Kermalis.PokemonBattleEngineClient
{
    class BattleClient : NetClient
    {
        static readonly IPacketProcessor packetProcessor = new PPacketProcessor();
        protected override IPacketProcessor PacketProcessor => packetProcessor;

        static readonly PPokemonShell
            pikachu = new PPokemonShell
            {
                Species = PSpecies.Pikachu,
                Item = PItem.LightBall,
                Ability = PAbility.LightningRod,
                Gender = PGender.Male,
                Nature = PNature.Timid,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                EVs = new byte[] { 0, 0, 4, 252, 0, 252 },
                Moves = new PMove[] { PMove.Thunder, PMove.Thunder, PMove.Thunder, PMove.Thunder } // substitute, thunderbolt, hidden power ice, grass knot
            },
            azumarill = new PPokemonShell
            {
                Species = PSpecies.Azumarill,
                Item = PItem.ChoiceBand,
                Ability = PAbility.HugePower,
                Gender = PGender.Male,
                Nature = PNature.Adamant,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                EVs = new byte[] { 252, 252, 0, 0, 0, 4 },
                Moves = new PMove[] { PMove.Waterfall, PMove.AquaJet, PMove.Return, PMove.IcePunch }
            },
            cresselia = new PPokemonShell
            {
                Species = PSpecies.Cresselia,
                Item = PItem.Leftovers,
                Ability = PAbility.Levitate,
                Gender = PGender.Female,
                Nature = PNature.Bold,
                IVs = new byte[] { 31, 31, 31, 31, 31, 31 },
                EVs = new byte[] { 252, 0, 252, 0, 0, 4 },
                Moves = new PMove[] { PMove.IceBeam, PMove.Moonlight, PMove.Psychic, PMove.Toxic }
            };
        static readonly PTeamShell
            team1 = new PTeamShell
            {
                DisplayName = "Sasha",
                Party = { azumarill }
            },
            team2 = new PTeamShell
            {
                DisplayName = "Jess",
                Party = { cresselia }
            };
        static PTeamShell chosenTeam = new Random().Next(0, 2) == 0 ? team1 : team2;

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
                case PPlayerJoinedPacket pjp:
                    Console.WriteLine("{0} joined the game.", pjp.DisplayName);
                    // TODO: What if it's a spectator?
                    PKnownInfo.Instance.RemoteDisplayName = pjp.DisplayName;
                    Send(new PResponsePacket());
                    break;
                case PReadyUpPacket _:
                    Console.WriteLine("Sending team info...");
                    PKnownInfo.Instance.LocalDisplayName = chosenTeam.DisplayName;
                    Send(new PRequestTeamPacket(chosenTeam));
                    break;
                case PSendPartyPacket spp:
                    PKnownInfo.Instance.SetPartyPokemon(spp.Pokemon, true);
                    Send(new PResponsePacket());
                    break;
                case PSwitchInPacket sip:
                    if (!sip.LocallyOwned)
                        PKnownInfo.Instance.AddRemotePokemon(sip.PokemonId, sip.Species, sip.Level, sip.HP, sip.MaxHP, sip.Gender);
                    Console.WriteLine("{1} is using: {0}", PKnownInfo.Instance[sip.PokemonId], PKnownInfo.Instance.DisplayName(sip.LocallyOwned));
                    Send(new PResponsePacket());
                    break;
            }
        }

        protected override void OnConnected()
        {
            Console.WriteLine("Connected to {0}", Socket.RemoteEndPoint);
            PKnownInfo.Instance.Clear();
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
