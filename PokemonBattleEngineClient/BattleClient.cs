using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using Ether.Network.Client;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngineClient.Views;

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
        Nickname = "Pikachu",
        Level = 100,
        Friendship = 255,
        Item = PItem.LightBall,
        Ability = PAbility.LightningRod,
        Gender = PGender.Female,
        Nature = PNature.Timid,
        IVs = new byte[] { 31, 31, 31, 31, 31, 30 }, // Hidden Power Ice/70
        EVs = new byte[] { 0, 0, 4, 252, 0, 252 },
        Moves = new PMove[] { PMove.Thunderbolt, PMove.Thunderbolt, PMove.HiddenPower, PMove.Thunderbolt }, // substitute, thunderbolt, hidden power ice, grass knot
        PPUps = new byte[] { 3, 3, 3, 3 }
    },
    azumarill = new PPokemonShell
    {
        Species = PSpecies.Azumarill,
        Nickname = "ZuWEEE",
        Level = 100,
        Friendship = 255,
        Item = PItem.ChoiceBand,
        Ability = PAbility.HugePower,
        Gender = PGender.Male,
        Nature = PNature.Adamant,
        IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power Dark/70
        EVs = new byte[] { 252, 252, 0, 0, 0, 4 },
        Moves = new PMove[] { PMove.Waterfall, PMove.AquaJet, PMove.Return, PMove.IcePunch },
        PPUps = new byte[] { 3, 3, 3, 3 }
    },
    cresselia = new PPokemonShell
    {
        Species = PSpecies.Cresselia,
        Nickname = "Crest",
        Level = 100,
        Friendship = 255,
        Item = PItem.Leftovers,
        Ability = PAbility.Levitate,
        Gender = PGender.Female,
        Nature = PNature.Bold,
        IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power Dark/70
        EVs = new byte[] { 252, 0, 252, 0, 0, 4 },
        Moves = new PMove[] { PMove.Psychic, PMove.Moonlight, PMove.IceBeam, PMove.Toxic },
        PPUps = new byte[] { 3, 3, 3, 3 }
    },
    darkrai = new PPokemonShell
    {
        Species = PSpecies.Darkrai,
        Nickname = "Darkrai",
        Level = 100,
        Friendship = 255,
        Item = PItem.Leftovers,
        Ability = PAbility.BadDreams,
        Gender = PGender.Genderless,
        Nature = PNature.Timid,
        IVs = new byte[] { 31, 31, 31, 31, 31, 31 }, // Hidden Power Dark/70
        EVs = new byte[] { 4, 0, 0, 252, 0, 252 },
        Moves = new PMove[] { PMove.DarkPulse, PMove.DarkPulse, PMove.NastyPlot, PMove.DarkPulse }, // dark void, dark pulse, nasty plot, substitute
        PPUps = new byte[] { 3, 3, 3, 3 }
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
                Party = { pikachu }
            };
        static PTeamShell chosenTeam = new Random().Next(0, 2) == 0 ? team1 : team2;

        readonly BattleView view;

        public BattleClient(string host, BattleView view)
        {
            Configuration.Host = host;
            Configuration.Port = 8888;
            Configuration.BufferSize = 1024;

            this.view = view;
        }

        public override void HandleMessage(INetPacket packet)
        {
            Debug.WriteLine($"Message received: \"{packet.GetType().Name}\"");
            PPokemon pkmn;
            int i;
            double d;

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
                    view.AddMessage(string.Format("{0} joined the game.", pjp.DisplayName), true);
                    // TODO: What if it's a spectator?
                    PKnownInfo.Instance.RemoteDisplayName = pjp.DisplayName;
                    Send(new PResponsePacket());
                    break;
                case PRequestPartyPacket _:
                    view.AddMessage("Sending team info...", true);
                    PKnownInfo.Instance.LocalDisplayName = chosenTeam.DisplayName;
                    Send(new PSubmitPartyPacket(chosenTeam));
                    break;
                case PSetPartyPacket spp:
                    PKnownInfo.Instance.SetPartyPokemon(spp.Party, true);
                    Send(new PResponsePacket());
                    break;
                case PPkmnSwitchInPacket psip:
                    if (!psip.LocallyOwned)
                        PKnownInfo.Instance.AddRemotePokemon(psip.PokemonId, psip.Species, psip.Nickname, psip.Level, psip.HP, psip.MaxHP, psip.Gender);
                    pkmn = PKnownInfo.Instance.Pokemon(psip.PokemonId);
                    Console.WriteLine(pkmn);
                    view.PokemonViews[psip.LocallyOwned ? 0 : 3].Pokemon = pkmn; // Temporary
                    view.AddMessage(string.Format("{1} sent out {0}!", pkmn.Shell.Nickname, PKnownInfo.Instance.DisplayName(pkmn.LocallyOwned)), true);
                    Send(new PResponsePacket());
                    break;
                case PRequestActionPacket _:
                    // TODO
                    var actions = new PSubmitActionsPacket.Action[1];
                    actions[0] = new PSubmitActionsPacket.Action(PKnownInfo.Instance.LocalParty[0].Id, 0);
                    Send(new PSubmitActionsPacket(actions));
                    break;
                case PPkmnHPChangedPacket phcp:
                    pkmn = PKnownInfo.Instance.Pokemon(phcp.PokemonId);
                    pkmn.HP = (ushort)(pkmn.HP + phcp.Change);

                    var hp = Math.Abs(phcp.Change);
                    d = (double)hp / pkmn.MaxHP;
                    view.AddMessage(string.Format("{0} {3} {1} ({2:P2}) HP!", pkmn.Shell.Nickname, hp, d, phcp.Change < 0 ? "lost" : "gained"), true);

                    Send(new PResponsePacket());
                    break;
                case PMovePPChangedPacket mpcp:
                    pkmn = PKnownInfo.Instance.Pokemon(mpcp.PokemonId);
                    i = pkmn.Shell.Moves.ToList().IndexOf(mpcp.Move);
                    pkmn.PP[i] = (byte)(pkmn.PP[i] + mpcp.Change);
                    Send(new PResponsePacket());
                    break;
                case PPkmnMovePacket pmp:
                    pkmn = PKnownInfo.Instance.Pokemon(pmp.PokemonId);
                    // Reveal move if the pokemon owns it and it's not already revealed
                    if (pmp.OwnsMove && !pkmn.Shell.Moves.Contains(pmp.Move))
                    {
                        // Set the first unknown move to the used move
                        i = pkmn.Shell.Moves.ToList().IndexOf(PMove.MAX);
                        pkmn.Shell.Moves[i] = pmp.Move;
                    }
                    Send(new PResponsePacket());
                    break;
                case PPkmnStatChangedPacket pscp:
                    PBattle.ApplyStatChange(pscp);
                    Send(new PResponsePacket());
                    break;
                case PStatus1ChangePacket scp:
                    PKnownInfo.Instance.Pokemon(scp.PokemonId).Status1 = scp.Status1;
                    Send(new PResponsePacket());
                    break;
                case PStatus1EndedPacket sep:
                    PKnownInfo.Instance.Pokemon(sep.PokemonId).Status1 = PStatus1.None;
                    Send(new PResponsePacket());
                    break;
                case PItemUsedPacket iup:
                    PKnownInfo.Instance.Pokemon(iup.PokemonId).Shell.Item = iup.Item;
                    Send(new PResponsePacket());
                    break;
            }
        }

        protected override void OnConnected()
        {
            Debug.WriteLine("Connected to {0}", Socket.RemoteEndPoint);
            PKnownInfo.Instance.Clear();
            view.AddMessage("Waiting for players...");
        }
        protected override void OnDisconnected()
        {
            Debug.WriteLine("Disconnected from server");
            Environment.Exit(0);
        }
        protected override void OnSocketError(SocketError socketError)
        {
            Debug.WriteLine("Socket Error: {0}", socketError);
        }
    }
}
