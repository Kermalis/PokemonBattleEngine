using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using Ether.Network.Client;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Util;
using Kermalis.PokemonBattleEngineClient.Views;

namespace Kermalis.PokemonBattleEngineClient
{
    class BattleClient : NetClient
    {
        static readonly IPacketProcessor packetProcessor = new PPacketProcessor();
        public override IPacketProcessor PacketProcessor => packetProcessor;

        static readonly PTeamShell
            team0 = new PTeamShell
            {
                DisplayName = "Sasha",
                Party = { CompetitivePokemonShells.Cresselia, CompetitivePokemonShells.Latios, CompetitivePokemonShells.Darkrai }
            },
            team1 = new PTeamShell
            {
                DisplayName = "Jess",
                Party = { CompetitivePokemonShells.Azumarill, CompetitivePokemonShells.Pikachu, CompetitivePokemonShells.Latias }
            };
        static PTeamShell chosenTeam = new Random().Next(0, 2) == 0 ? team0 : team1; // Temporary

        public PBattleStyle BattleStyle { get; private set; } = PBattleStyle.Triple;
        readonly BattleView battleView;
        readonly ActionsView actionsView;

        public BattleClient(string host, BattleView battleView, ActionsView actionsView)
        {
            Configuration.Host = host;
            Configuration.Port = 8888;
            Configuration.BufferSize = 1024;

            this.battleView = battleView;
            this.battleView.Client = this;
            this.actionsView = actionsView;
            this.actionsView.Client = this;
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
                case PMoveMissedPacket _:
                case PPkmnFaintedPacket _:
                case PMoveFailPacket _:
                case PLimberPacket _:
                case PReflectLightScreenPacket _:
                    Send(new PResponsePacket());
                    break;

                case PPlayerJoinedPacket pjp:
                    battleView.AddMessage(string.Format("{0} joined the game.", pjp.DisplayName), true);
                    // TODO: What if it's a spectator?
                    PKnownInfo.Instance.RemoteDisplayName = pjp.DisplayName;
                    Send(new PResponsePacket());
                    break;
                case PRequestPartyPacket _:
                    battleView.AddMessage("Sending team info...", true);
                    PKnownInfo.Instance.LocalDisplayName = chosenTeam.DisplayName;
                    Send(new PSubmitPartyPacket(chosenTeam));
                    break;
                case PSetPartyPacket spp:
                    PKnownInfo.Instance.SetPartyPokemon(spp.Party, true);
                    Send(new PResponsePacket());
                    break;
                case PPkmnSwitchInPacket psip:
                    if (!psip.Local)
                        PKnownInfo.Instance.AddRemotePokemon(psip.PokemonId, psip.Species, psip.Nickname, psip.Level, psip.Shiny, psip.HP, psip.MaxHP, psip.Gender);
                    pkmn = PKnownInfo.Instance.Pokemon(psip.PokemonId);
                    pkmn.FieldPosition = psip.FieldPosition;
                    battleView.PokemonPositionChanged(pkmn);
                    battleView.AddMessage(string.Format("{1} sent out {0}!", pkmn.Shell.Nickname, PKnownInfo.Instance.DisplayName(pkmn.Local)), true);
                    Send(new PResponsePacket());
                    break;
                case PRequestActionsPacket _:
                    ActionsLoop(true);
                    break;
                case PPkmnHPChangedPacket phcp:
                    pkmn = PKnownInfo.Instance.Pokemon(phcp.PokemonId);
                    pkmn.HP = (ushort)(pkmn.HP + phcp.Change);

                    var hp = Math.Abs(phcp.Change);
                    d = (double)hp / pkmn.MaxHP;
                    battleView.AddMessage(string.Format("{0} {3} {1} ({2:P2}) HP!", pkmn.Shell.Nickname, hp, d, phcp.Change < 0 ? "lost" : "gained"), true);

                    Send(new PResponsePacket());
                    break;
                case PMovePPChangedPacket mpcp:
                    pkmn = PKnownInfo.Instance.Pokemon(mpcp.PokemonId);
                    i = Array.IndexOf(pkmn.Shell.Moves, mpcp.Move);
                    pkmn.PP[i] = (byte)(pkmn.PP[i] + mpcp.Change);
                    Send(new PResponsePacket());
                    break;
                case PMoveUsedPacket mup:
                    pkmn = PKnownInfo.Instance.Pokemon(mup.PokemonId);
                    // Reveal move if the pokemon owns it and it's not already revealed
                    if (mup.OwnsMove && !pkmn.Shell.Moves.Contains(mup.Move))
                    {
                        // Set the first unknown move to the used move
                        i = Array.IndexOf(pkmn.Shell.Moves, PMove.MAX);
                        pkmn.Shell.Moves[i] = mup.Move;
                    }
                    Send(new PResponsePacket());
                    break;
                case PPkmnStatChangedPacket pscp:
                    PBattle.ApplyStatChange(pscp);
                    Send(new PResponsePacket());
                    break;
                case PStatus1Packet s1p:
                    switch (s1p.Action)
                    {
                        case PStatusAction.Added:
                            PKnownInfo.Instance.Pokemon(s1p.PokemonId).Status1 = s1p.Status1;
                            break;
                        case PStatusAction.Ended:
                            PKnownInfo.Instance.Pokemon(s1p.PokemonId).Status1 = PStatus1.None;
                            break;
                    }
                    Send(new PResponsePacket());
                    break;
                case PStatus2Packet s2p:
                    switch (s2p.Action)
                    {
                        case PStatusAction.Added:
                            PKnownInfo.Instance.Pokemon(s2p.PokemonId).Status2 |= s2p.Status2;
                            break;
                        case PStatusAction.Ended:
                            PKnownInfo.Instance.Pokemon(s2p.PokemonId).Status2 &= ~s2p.Status2;
                            break;
                    }
                    Send(new PResponsePacket());
                    break;
                case PItemUsedPacket iup:
                    PKnownInfo.Instance.Pokemon(iup.PokemonId).Shell.Item = iup.Item;
                    Send(new PResponsePacket());
                    break;
            }

            PBattle.ConsoleBattleEventHandler(packet);
        }

        List<PPokemon> actions = new List<PPokemon>(3);
        void ActionsLoop(bool begin)
        {
            PPokemon pkmn;
            if (begin)
            {
                foreach (PPokemon p in PKnownInfo.Instance.LocalParty)
                    p.Action.Decision = PDecision.None;
                actions.Clear();
                switch (BattleStyle)
                {
                    case PBattleStyle.Single:
                    case PBattleStyle.Rotation:
                        actions.Add(PKnownInfo.Instance.PokemonAtPosition(true, PFieldPosition.Center));
                        break;
                    case PBattleStyle.Double:
                        pkmn = PKnownInfo.Instance.PokemonAtPosition(true, PFieldPosition.Left);
                        if (pkmn != null)
                            actions.Add(pkmn);
                        pkmn = PKnownInfo.Instance.PokemonAtPosition(true, PFieldPosition.Right);
                        if (pkmn != null)
                            actions.Add(pkmn);
                        break;
                    case PBattleStyle.Triple:
                        pkmn = PKnownInfo.Instance.PokemonAtPosition(true, PFieldPosition.Left);
                        if (pkmn != null)
                            actions.Add(pkmn);
                        pkmn = PKnownInfo.Instance.PokemonAtPosition(true, PFieldPosition.Center);
                        if (pkmn != null)
                            actions.Add(pkmn);
                        pkmn = PKnownInfo.Instance.PokemonAtPosition(true, PFieldPosition.Right);
                        if (pkmn != null)
                            actions.Add(pkmn);
                        break;
                }
            }
            int i = actions.FindIndex(p => p.Action.Decision == PDecision.None);
            if (i == -1)
            {
                battleView.AddMessage($"Waiting for {PKnownInfo.Instance.RemoteDisplayName}...", true);
                Send(new PSubmitActionsPacket(actions.Select(p => p.Action).ToArray()));
            }
            else
            {
                battleView.AddMessage($"What will {actions[i].Shell.Nickname} do?", true);
                actionsView.DisplayMoves(actions[i]);
            }
        }
        public void ActionSet()
        {
            ActionsLoop(false);
        }

        protected override void OnConnected()
        {
            Debug.WriteLine("Connected to {0}", Socket.RemoteEndPoint);
            PKnownInfo.Instance.Clear();
            battleView.AddMessage("Waiting for players...");
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
