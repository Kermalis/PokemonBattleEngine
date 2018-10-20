using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using Ether.Network.Client;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;

namespace Kermalis.PokemonBattleEngineClient
{
    class BattleClient : NetClient
    {
        static readonly IPacketProcessor packetProcessor = new PPacketProcessor();
        public override IPacketProcessor PacketProcessor => packetProcessor;

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
                Moves = new PMove[] { PMove.Psychic, PMove.Moonlight, PMove.IceBeam, PMove.Toxic }
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

        public override void HandleMessage(INetPacket packet)
        {
            Debug.WriteLine($"Message received: \"{packet.GetType().Name}\"");
            PPokemon pkmn;

            switch (packet)
            {
                // TODO List for UI
                case PMoveEffectivenessPacket _:
                case PPkmnFlinchedPacket _:
                case PMoveMissedPacket _:
                case PPkmnFaintedPacket _:
                case PStatus1CausedImmobilityPacket _:
                case PStatus1CausedDamagePacket _:
                case PMoveFailPacket _:
                    Send(new PResponsePacket());
                    break;

                case PPlayerJoinedPacket pjp:
                    Console.WriteLine("{0} joined the game.", pjp.DisplayName);
                    // TODO: What if it's a spectator?
                    PKnownInfo.Instance.RemoteDisplayName = pjp.DisplayName;
                    Send(new PResponsePacket());
                    break;
                case PRequestPartyPacket _:
                    Console.WriteLine("Sending team info...");
                    PKnownInfo.Instance.LocalDisplayName = chosenTeam.DisplayName;
                    Send(new PSubmitPartyPacket(chosenTeam));
                    break;
                case PSetPartyPacket spp:
                    PKnownInfo.Instance.SetPartyPokemon(spp.Party, true);
                    Send(new PResponsePacket());
                    break;
                case PPkmnSwitchInPacket psip:
                    if (!psip.LocallyOwned)
                        PKnownInfo.Instance.AddRemotePokemon(psip.PokemonId, psip.Species, psip.Level, psip.HP, psip.MaxHP, psip.Gender);
                    Send(new PResponsePacket());
                    break;
                case PRequestActionPacket _:
                    // TODO
                    var actions = new PSubmitActionsPacket.Action[1];
                    actions[0] = new PSubmitActionsPacket.Action(PKnownInfo.Instance.LocalParty[0].Id, 0);
                    Send(new PSubmitActionsPacket(actions));
                    break;
                case PPkmnDamagedPacket pdp:
                    PKnownInfo.Instance.Pokemon(pdp.PokemonId).HP -= pdp.Damage;
                    PrintBattlers();
                    Send(new PResponsePacket());
                    break;
                case PPkmnMovePacket pmp:
                    pkmn = PKnownInfo.Instance.Pokemon(pmp.PokemonId);
                    // Reveal move if the pokemon owns it and it's not already revealed
                    if (pmp.OwnsMove && !pkmn.Shell.Moves.Contains(pmp.Move))
                    {
                        // Set the first unknown move to the used move
                        // TODO: Set PP
                        var index = pkmn.Shell.Moves.ToList().IndexOf(PMove.MAX);
                        pkmn.Shell.Moves[index] = pmp.Move;
                    }
                    Send(new PResponsePacket());
                    break;
                case PPkmnStatChangePacket pscp:
                    PBattle.ApplyStatChange(pscp);
                    Send(new PResponsePacket());
                    break;
                case PStatus1ChangePacket scp:
                    PKnownInfo.Instance.Pokemon(scp.PokemonId).Status1 = scp.Status1;
                    Send(new PResponsePacket());
                    break;
                case PStatus1EndedPacket sep:
                    PKnownInfo.Instance.Pokemon(sep.PokemonId).Status1 = PStatus1.NoStatus;
                    Send(new PResponsePacket());
                    break;
            }

            PBattle.ConsoleBattleEventHandler(packet);
        }

        void PrintBattlers()
        {
            Console.WriteLine("{1}: {0}", PKnownInfo.Instance.LocalParty[0], PKnownInfo.Instance.LocalDisplayName);
            Console.WriteLine("{1}: {0}", PKnownInfo.Instance.RemoteParty[0], PKnownInfo.Instance.RemoteDisplayName);
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
