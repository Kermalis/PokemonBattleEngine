using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Timers;
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

        public PBattleStyle BattleStyle { get; private set; } = PBattleStyle.Triple;
        readonly BattleView battleView;
        readonly ActionsView actionsView;

        // Fisher-Yates Shuffle
        static void Shuffle<T>(IList<T> source)
        {
            var rng = new Random();
            for (int a = 0; a < source.Count - 1; a++)
            {
                int b = rng.Next(a, source.Count);
                T value = source[a];
                source[a] = source[b];
                source[b] = value;
            }
        }

        public BattleClient(string host, BattleView battleView, ActionsView actionsView)
        {
            Configuration.Host = host;
            Configuration.Port = 8888;
            Configuration.BufferSize = 1024;

            this.battleView = battleView;
            this.battleView.Client = this;
            this.actionsView = actionsView;
            this.actionsView.Client = this;

            packetTimer.Elapsed += PacketTimer_Elapsed;
            packetTimer.Start();
        }

        List<INetPacket> packetQueue = new List<INetPacket>();
        Timer packetTimer = new Timer(2000);
        public override void HandleMessage(INetPacket packet)
        {
            Debug.WriteLine($"Message received: \"{packet.GetType().Name}\"");
            switch (packet)
            {
                case PPlayerJoinedPacket pjp:
                    // TODO: What if it's a spectator?
                    battleView.Message = string.Format("{0} joined the game.", pjp.DisplayName);
                    PKnownInfo.Instance.RemoteDisplayName = pjp.DisplayName;
                    Send(new PResponsePacket());
                    break;
                case PRequestPartyPacket _: // Temporary
                    battleView.Message = "Sending team info...";
                    var team = new PTeamShell { DisplayName = "Sasha", };
                    var possiblePokemon = new List<PPokemonShell>
                    {
                        PCompetitivePokemonShells.Azumarill_UU, PCompetitivePokemonShells.Cresselia_UU, PCompetitivePokemonShells.Darkrai_Uber,
                        PCompetitivePokemonShells.Ditto_UU, PCompetitivePokemonShells.Genesect_Uber, PCompetitivePokemonShells.Latias_OU,
                        PCompetitivePokemonShells.Latios_OU, PCompetitivePokemonShells.Pikachu_NU };
                    Shuffle(possiblePokemon);
                    team.Party.AddRange(possiblePokemon.Take(PSettings.MaxPartySize));
                    //team.Party[0] = PCompetitivePokemonShells.Azumarill_UU;
                    PKnownInfo.Instance.LocalDisplayName = team.DisplayName;
                    Send(new PSubmitPartyPacket(team));
                    break;
                case PSetPartyPacket spp:
                    PKnownInfo.Instance.SetPartyPokemon(spp.Party, true);
                    Send(new PResponsePacket());
                    break;
                default:
                    packetQueue.Add(packet);
                    Send(new PResponsePacket());
                    break;
            }
        }
        void PacketTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            again:
            INetPacket packet = packetQueue.FirstOrDefault();
            if (packet == null)
                return;
            bool a = ProcessPacket(packet);
            packetQueue.RemoveAt(0);
            if (a)
                goto again;
        }
        // Returns true if the next packet should be run right away
        bool ProcessPacket(INetPacket packet)
        {
            PPokemon pkmn;
            int i;
            double d;
            PFieldPosition pos;
            string message;

            switch (packet)
            {
                case PItemUsedPacket iup:
                    pkmn = PKnownInfo.Instance.Pokemon(iup.PokemonId);
                    switch (iup.Item)
                    {
                        case PItem.Leftovers:
                            pkmn.Item = iup.Item;
                            message = "{0} restored a little HP using its Leftovers!";
                            break;
                        case PItem.PowerHerb:
                            pkmn.Item = PItem.None;
                            message = "{0} became fully charged due to its Power Herb!";
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(iup.Item), $"Invalid item used: {iup.Item}");
                    }
                    battleView.Message = string.Format(message, pkmn.Shell.Nickname);
                    break;
                case PLimberPacket lp:
                    pkmn = PKnownInfo.Instance.Pokemon(lp.PokemonId);
                    message = "{0}'s Limber"; // TODO: Ability stuff
                    battleView.Message = string.Format(message, pkmn.Shell.Nickname);
                    break;
                case PMoveCritPacket _:
                    battleView.Message = "A critical hit!";
                    break;
                case PMoveEffectivenessPacket mep:
                    pkmn = PKnownInfo.Instance.Pokemon(mep.PokemonId);
                    switch (mep.Effectiveness)
                    {
                        case PEffectiveness.Ineffective: message = "It doesn't affect {0}..."; break;
                        case PEffectiveness.NotVeryEffective: message = "It's not very effective..."; break;
                        case PEffectiveness.Normal: return true;
                        case PEffectiveness.SuperEffective: message = "It's super effective!"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mep.Effectiveness), $"Invalid effectiveness: {mep.Effectiveness}");
                    }
                    battleView.Message = string.Format(message, pkmn.Shell.Nickname);
                    break;
                case PMoveFailedPacket mfp:
                    pkmn = PKnownInfo.Instance.Pokemon(mfp.PokemonId);
                    switch (mfp.Reason)
                    {
                        case PFailReason.Default: message = "But it failed!"; break;
                        case PFailReason.HPFull: message = "{0}'s HP is full!"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mfp.Reason), $"Invalid fail reason: {mfp.Reason}");
                    }
                    battleView.Message = string.Format(message, pkmn.Shell.Nickname);
                    break;
                case PMoveMissedPacket mmp:
                    pkmn = PKnownInfo.Instance.Pokemon(mmp.PokemonId);
                    message = "{0}'s attack missed!";
                    battleView.Message = string.Format(message, pkmn.Shell.Nickname);
                    break;
                case PMovePPChangedPacket mpcp:
                    pkmn = PKnownInfo.Instance.Pokemon(mpcp.PokemonId);
                    i = Array.IndexOf(pkmn.Moves, mpcp.Move);
                    pkmn.PP[i] = (byte)(pkmn.PP[i] + mpcp.Change);
                    return true;
                case PMoveUsedPacket mup:
                    pkmn = PKnownInfo.Instance.Pokemon(mup.PokemonId);
                    // Reveal move if the pokemon owns it and it's not already revealed
                    if (mup.OwnsMove && !pkmn.Moves.Contains(mup.Move))
                    {
                        // Set the first unknown move to the used move
                        i = Array.IndexOf(pkmn.Moves, PMove.MAX);
                        pkmn.Moves[i] = mup.Move;
                    }
                    message = "{0} used {1}!";
                    battleView.Message = string.Format(message, pkmn.Shell.Nickname, mup.Move);
                    break;
                case PPkmnFaintedPacket pfap:
                    pkmn = PKnownInfo.Instance.Pokemon(pfap.PokemonId);
                    pos = pkmn.FieldPosition;
                    pkmn.FieldPosition = PFieldPosition.None;
                    battleView.PokemonPositionChanged(pkmn, pos);
                    message = "{0} fainted!";
                    battleView.Message = string.Format(message, pkmn.Shell.Nickname);
                    break;
                case PPkmnHPChangedPacket phcp:
                    pkmn = PKnownInfo.Instance.Pokemon(phcp.PokemonId);
                    pkmn.HP = (ushort)(pkmn.HP + phcp.Change);
                    var hp = Math.Abs(phcp.Change);
                    d = (double)hp / pkmn.MaxHP;
                    battleView.Message = string.Format("{0} {3} {1} ({2:P2}) HP!", pkmn.Shell.Nickname, hp, d, phcp.Change <= 0 ? "lost" : "gained");
                    break;
                case PPkmnStatChangedPacket pscp:
                    PBattle.ApplyStatChange(pscp);
                    pkmn = PKnownInfo.Instance.Pokemon(pscp.PokemonId);
                    switch (pscp.Change)
                    {
                        case -2: message = "harshly fell"; break;
                        case -1: message = "fell"; break;
                        case +1: message = "rose"; break;
                        case +2: message = "rose sharply"; break;
                        default:
                            if (pscp.IsTooMuch && pscp.Change < 0)
                                message = "won't go lower";
                            else if (pscp.IsTooMuch && pscp.Change > 0)
                                message = "won't go higher";
                            else if (pscp.Change <= -3)
                                message = "severely fell";
                            else if (pscp.Change >= 3)
                                message = "rose drastically";
                            else
                                throw new ArgumentOutOfRangeException(nameof(pscp.Change), $"Invalid stat change: {pscp.Change}"); // +0
                            break;
                    }
                    battleView.Message = string.Format("{0}'s {1} {2}!", pkmn.Shell.Nickname, pscp.Stat, message);
                    break;
                case PPkmnSwitchInPacket psip:
                    if (!psip.Local)
                        PKnownInfo.Instance.AddRemotePokemon(psip);
                    pkmn = PKnownInfo.Instance.Pokemon(psip.PokemonId);
                    pos = pkmn.FieldPosition;
                    pkmn.FieldPosition = psip.FieldPosition;
                    battleView.PokemonPositionChanged(pkmn, pos);
                    battleView.Message = string.Format("{1} sent out {0}!", pkmn.Shell.Nickname, PKnownInfo.Instance.DisplayName(pkmn.Local));
                    break;
                case PReflectLightScreenPacket rlsp:
                    switch (rlsp.Action)
                    {
                        case PReflectLightScreenAction.Added: message = "{0} raised {2} team's {1}!"; break;
                        case PReflectLightScreenAction.Broke:
                        case PReflectLightScreenAction.Ended: message = "{3} team's {0} wore off!"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(rlsp.Action), $"Invalid reflect/lightscreen action: {rlsp.Action}");
                    }
                    battleView.Message = string.Format(message, rlsp.Reflect ? "Reflect" : "Light Screen", rlsp.Reflect ? PStat.Defense : PStat.SpDefense, rlsp.Local ? "your" : "the opposing", rlsp.Local ? "Your" : "The opposing");
                    break;
                case PStatus1Packet s1p:
                    pkmn = PKnownInfo.Instance.Pokemon(s1p.PokemonId);
                    switch (s1p.Action)
                    {
                        case PStatusAction.Added:
                            pkmn.Status1 = s1p.Status1;
                            break;
                        case PStatusAction.Cured:
                        case PStatusAction.Ended:
                            pkmn.Status1 = PStatus1.None;
                            break;
                    }
                    switch (s1p.Status1)
                    {
                        case PStatus1.Asleep:
                            switch (s1p.Action)
                            {
                                case PStatusAction.Activated: message = "{0} is fast asleep."; break;
                                case PStatusAction.Added: message = "{0} fell asleep!"; break;
                                case PStatusAction.Ended: message = "{0} woke up!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.Action), $"Invalid asleep action: {s1p.Action}");
                            }
                            break;
                        case PStatus1.BadlyPoisoned:
                        case PStatus1.Poisoned:
                            switch (s1p.Action)
                            {
                                case PStatusAction.Added: message = "{0} was poisoned!"; break;
                                case PStatusAction.Damage: message = "{0} was hurt by poison!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.Action), $"Invalid poisoned action: {s1p.Action}");
                            }
                            break;
                        case PStatus1.Burned:
                            switch (s1p.Action)
                            {
                                case PStatusAction.Added: message = "{0} was burned!"; break;
                                case PStatusAction.Damage: message = "{0} was hurt by its burn!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.Action), $"Invalid burned action: {s1p.Action}");
                            }
                            break;
                        case PStatus1.Frozen:
                            switch (s1p.Action)
                            {
                                case PStatusAction.Activated: message = "{0} is frozen solid!"; break;
                                case PStatusAction.Added: message = "{0} was frozen solid!"; break;
                                case PStatusAction.Ended: message = "{0} thawed out!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.Action), $"Invalid frozen action: {s1p.Action}");
                            }
                            break;
                        case PStatus1.Paralyzed:
                            switch (s1p.Action)
                            {
                                case PStatusAction.Activated: message = "{0} is paralyzed! It can't move!"; break;
                                case PStatusAction.Added: message = "{0} is paralyzed! It may be unable to move!"; break;
                                case PStatusAction.Cured: message = "{0} was cured of paralysis."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.Action), $"Invalid paralyzed action: {s1p.Action}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(s1p.Status1), $"Invalid status1: {s1p.Status1}");
                    }
                    battleView.Message = string.Format(message, pkmn.Shell.Nickname);
                    break;
                case PStatus2Packet s2p:
                    pkmn = PKnownInfo.Instance.Pokemon(s2p.PokemonId);
                    switch (s2p.Action)
                    {
                        case PStatusAction.Added:
                            pkmn.Status2 |= s2p.Status2;
                            break;
                        case PStatusAction.Ended:
                            pkmn.Status2 &= ~s2p.Status2;
                            break;
                    }
                    switch (s2p.Status2)
                    {
                        case PStatus2.Confused:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Activated: message = "{0} is confused!"; break;
                                case PStatusAction.Added: message = "{0} became confused!"; break;
                                case PStatusAction.Damage: message = "It hurt itself in its confusion!"; break;
                                case PStatusAction.Ended: message = "{0} snapped out of its confusion."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid confused action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.Flinching:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Activated: message = "{0} flinched and couldn't move!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid flinching action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.Protected:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Activated:
                                case PStatusAction.Added: message = "{0} protected itself!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid protected action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.Pumped:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: message = "{0} is getting pumped!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid pumped action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.Substitute:
                            battleView.PokemonPositionChanged(pkmn, PFieldPosition.None);
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: message = "{0} put in a substitute!"; break;
                                case PStatusAction.Damage: message = "The substitute took damage for {0}!"; break;
                                case PStatusAction.Ended: message = "{0}'s substitute faded!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid substitute action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.Underwater:
                            battleView.PokemonPositionChanged(pkmn, PFieldPosition.None);
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added:
                                    message = "{0} hid underwater!";
                                    pkmn.LockedAction = pkmn.SelectedAction;
                                    break;
                                case PStatusAction.Ended:
                                    pkmn.LockedAction.Decision = PDecision.None;
                                    return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid underwater action: {s2p.Action}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(s2p.Status2), $"Invalid status2: {s2p.Status2}");
                    }
                    battleView.Message = string.Format(message, pkmn.Shell.Nickname);
                    break;
                case PTransformPacket tp:
                    {
                        PPokemon user = PKnownInfo.Instance.Pokemon(tp.UserId),
                            target = PKnownInfo.Instance.Pokemon(tp.TargetId);
                        user.Transform(target, tp.TargetAttack, tp.TargetDefense, tp.TargetSpAttack, tp.TargetSpDefense, tp.TargetSpeed, tp.TargetAbility, tp.TargetMoves);
                        battleView.PokemonPositionChanged(user, PFieldPosition.None);
                        battleView.Message = string.Format("{0} transformed into {1}!", user.Shell.Nickname, target.Shell.Nickname);
                        break;
                    }
                case PWeatherPacket wp:
                    switch (wp.Weather)
                    {
                        case PWeather.Raining:
                            switch (wp.Action)
                            {
                                case PWeatherAction.Added:
                                    PKnownInfo.Instance.Weather = PWeather.Raining;
                                    message = "It started to rain!";
                                    break;
                                case PWeatherAction.Ended:
                                    PKnownInfo.Instance.Weather = PWeather.None;
                                    message = "The rain stopped.";
                                    break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.Action), $"Invalid raining action: {wp.Action}");
                            }
                            break;
                        case PWeather.Sunny:
                            switch (wp.Action)
                            {
                                case PWeatherAction.Added:
                                    PKnownInfo.Instance.Weather = PWeather.Sunny;
                                    message = "The sunlight turned harsh!";
                                    break;
                                case PWeatherAction.Ended:
                                    PKnownInfo.Instance.Weather = PWeather.None;
                                    message = "The sunlight faded.";
                                    break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.Action), $"Invalid sunny action: {wp.Action}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(wp.Weather), $"Invalid weather: {wp.Weather}");
                    }
                    battleView.Message = message;
                    break;
                case PRequestActionsPacket _:
                    ActionsLoop(true);
                    break;
            }
            return false;
        }

        List<PPokemon> actions = new List<PPokemon>(3);
        void ActionsLoop(bool begin)
        {
            PPokemon pkmn;
            if (begin)
            {
                foreach (PPokemon p in PKnownInfo.Instance.LocalParty)
                    p.SelectedAction.Decision = PDecision.None;
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
            int i = actions.FindIndex(p => p.SelectedAction.Decision == PDecision.None);
            if (i == -1)
            {
                battleView.Message = $"Waiting for {PKnownInfo.Instance.RemoteDisplayName}...";
                Send(new PSubmitActionsPacket(actions.Select(p => p.SelectedAction).ToArray()));
            }
            else
            {
                battleView.Message = $"What will {actions[i].Shell.Nickname} do?";
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
            battleView.Message = "Waiting for players...";
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
