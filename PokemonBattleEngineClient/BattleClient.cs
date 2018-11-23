using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Timers;
using Ether.Network.Client;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine;
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

        public PBattle Battle;
        readonly BattleView battleView;
        readonly ActionsView actionsView;
        readonly MessageView messageView;

        public BattleClient(string host, BattleView battleView, ActionsView actionsView, MessageView messageView)
        {
            Configuration.Host = host;
            Configuration.Port = 8888;
            Configuration.BufferSize = 1024;

            Battle = new PBattle(PBattleStyle.Double);
            this.battleView = battleView;
            this.battleView.SetBattle(Battle);
            this.actionsView = actionsView;
            this.actionsView.Client = this;
            this.messageView = messageView;

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
                    messageView.Add(battleView.Message = string.Format("{0} joined the game.", pjp.DisplayName));
                    Battle.Teams[1].TrainerName = pjp.DisplayName;
                    Send(new PResponsePacket());
                    break;
                case PRequestPartyPacket _: // Temporary
                    messageView.Add(battleView.Message = "Sending team info...");
                    var team = new PTeamShell { PlayerName = new string[] { "Sasha", "Nikki", "Lara", "Violet", "Naomi", "Rose", "Sabrina" }.Sample(), };
                    var possiblePokemon = new List<PPokemonShell>
                    {
                        PCompetitivePokemonShells.Absol_RU, PCompetitivePokemonShells.Azumarill_UU, PCompetitivePokemonShells.Cofagrigus_UU,
                        PCompetitivePokemonShells.Cresselia_UU, PCompetitivePokemonShells.Darkrai_Uber, PCompetitivePokemonShells.Ditto_UU,
                        PCompetitivePokemonShells.Genesect_Uber, PCompetitivePokemonShells.Latias_OU, PCompetitivePokemonShells.Latios_OU,
                        PCompetitivePokemonShells.Marowak_VGC, PCompetitivePokemonShells.Pikachu_NU, PCompetitivePokemonShells.Rotom_Wash_OU
                    };
                    possiblePokemon.Shuffle();
                    team.Party.AddRange(possiblePokemon.Take(PSettings.MaxPartySize));
                    Battle.Teams[0].TrainerName = team.PlayerName;
                    Send(new PSubmitPartyPacket(team));
                    break;
                case PSetPartyPacket spp:
                    Battle.Teams[0].SetParty(spp.Party);
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
            bool b;
            PFieldPosition pos;
            string message;
            PTeam team;

            switch (packet)
            {
                case PItemUsedPacket iup:
                    pkmn = Battle.GetPokemon(iup.PokemonId);
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
                    messageView.Add(battleView.Message = string.Format(message, pkmn.NameForTrainer(true)));
                    break;
                case PLimberPacket lp:
                    pkmn = Battle.GetPokemon(lp.PokemonId);
                    message = "{0}'s Limber"; // TODO: Ability stuff
                    messageView.Add(battleView.Message = string.Format(message, pkmn.Shell.Nickname));
                    break;
                case PMoveCritPacket _:
                    messageView.Add(battleView.Message = "A critical hit!");
                    break;
                case PMoveEffectivenessPacket mep:
                    pkmn = Battle.GetPokemon(mep.PokemonId);
                    switch (mep.Effectiveness)
                    {
                        case PEffectiveness.Ineffective: message = "It doesn't affect {0}..."; break;
                        case PEffectiveness.NotVeryEffective: message = "It's not very effective..."; break;
                        case PEffectiveness.Normal: return true;
                        case PEffectiveness.SuperEffective: message = "It's super effective!"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mep.Effectiveness), $"Invalid effectiveness: {mep.Effectiveness}");
                    }
                    messageView.Add(battleView.Message = string.Format(message, pkmn.NameForTrainer(false)));
                    break;
                case PMoveFailedPacket mfp:
                    pkmn = Battle.GetPokemon(mfp.PokemonId);
                    switch (mfp.Reason)
                    {
                        case PFailReason.Default: message = "But it failed!"; break;
                        case PFailReason.HPFull: message = "{0}'s HP is full!"; break;
                        case PFailReason.NoTarget: message = "There was no target..."; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mfp.Reason), $"Invalid fail reason: {mfp.Reason}");
                    }
                    messageView.Add(battleView.Message = string.Format(message, pkmn.NameForTrainer(true)));
                    break;
                case PMoveMissedPacket mmp:
                    pkmn = Battle.GetPokemon(mmp.PokemonId);
                    message = "{0}'s attack missed!";
                    messageView.Add(battleView.Message = string.Format(message, pkmn.NameForTrainer(true)));
                    break;
                case PMovePPChangedPacket mpcp:
                    pkmn = Battle.GetPokemon(mpcp.PokemonId);
                    i = Array.IndexOf(pkmn.Moves, mpcp.Move);
                    pkmn.PP[i] = (byte)(pkmn.PP[i] + mpcp.Change);
                    return true;
                case PMoveUsedPacket mup:
                    pkmn = Battle.GetPokemon(mup.PokemonId);
                    // Reveal move if the pokemon owns it and it's not already revealed
                    if (mup.OwnsMove && !pkmn.Moves.Contains(mup.Move))
                    {
                        // Set the first unknown move to the used move
                        i = Array.IndexOf(pkmn.Moves, PMove.MAX);
                        pkmn.Moves[i] = mup.Move;
                    }
                    message = "{0} used {1}!";
                    messageView.Add(battleView.Message = string.Format(message, pkmn.NameForTrainer(true), mup.Move));
                    break;
                case PPkmnFaintedPacket pfap:
                    pkmn = Battle.GetPokemon(pfap.PokemonId);
                    pos = pkmn.FieldPosition;
                    pkmn.FieldPosition = PFieldPosition.None;
                    battleView.PokemonPositionChanged(pkmn, pos);
                    message = "{0} fainted!";
                    messageView.Add(battleView.Message = string.Format(message, pkmn.NameForTrainer(true)));
                    break;
                case PPkmnHPChangedPacket phcp:
                    pkmn = Battle.GetPokemon(phcp.PokemonId);
                    pkmn.HP = (ushort)(pkmn.HP + phcp.Change);
                    var hp = Math.Abs(phcp.Change);
                    d = (double)hp / pkmn.MaxHP;
                    messageView.Add(battleView.Message = string.Format("{0} {3} {1} ({2:P2}) HP!", pkmn.NameForTrainer(true), hp, d, phcp.Change <= 0 ? "lost" : "gained"));
                    break;
                case PPkmnStatChangedPacket pscp:
                    PBattle.ApplyStatChange(Battle.GetPokemon(pscp.PokemonId), pscp.Stat, pscp.Change, true, null);
                    pkmn = Battle.GetPokemon(pscp.PokemonId);
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
                    messageView.Add(battleView.Message = string.Format("{0}'s {1} {2}!", pkmn.NameForTrainer(true), pscp.Stat, message));
                    break;
                case PPkmnSwitchInPacket psip:
                    if (!psip.Local)
                        Battle.RemotePokemonSwitchedIn(psip);
                    pkmn = Battle.GetPokemon(psip.PokemonId);
                    pos = pkmn.FieldPosition;
                    pkmn.FieldPosition = psip.FieldPosition;
                    battleView.PokemonPositionChanged(pkmn, pos);
                    messageView.Add(battleView.Message = string.Format("{1} sent out {0}!", pkmn.Shell.Nickname, Battle.Teams[pkmn.Local ? 0 : 1].TrainerName));
                    break;
                case PPkmnSwitchOutPacket psop:
                    pkmn = Battle.GetPokemon(psop.PokemonId);
                    pos = pkmn.FieldPosition;
                    pkmn.ClearForSwitch();
                    battleView.PokemonPositionChanged(pkmn, pos);
                    messageView.Add(battleView.Message = string.Format("{1} withdrew {0}!", pkmn.Shell.Nickname, Battle.Teams[pkmn.Local ? 0 : 1].TrainerName));
                    break;
                case PStatus1Packet s1p:
                    pkmn = Battle.GetPokemon(s1p.PokemonId);
                    switch (s1p.Action)
                    {
                        case PStatusAction.Added:
                            pkmn.Status1 = s1p.Status;
                            break;
                        case PStatusAction.Cured:
                        case PStatusAction.Ended:
                            pkmn.Status1 = PStatus1.None;
                            break;
                    }
                    switch (s1p.Status)
                    {
                        case PStatus1.Asleep:
                            switch (s1p.Action)
                            {
                                case PStatusAction.Activated: message = "{0} is fast asleep."; break;
                                case PStatusAction.Added: message = "{0} fell asleep!"; break;
                                case PStatusAction.Ended: message = "{0} woke up!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.Action), $"Invalid {s1p.Status} action: {s1p.Action}");
                            }
                            break;
                        case PStatus1.BadlyPoisoned:
                            switch (s1p.Action)
                            {
                                case PStatusAction.Added: message = "{0} was badly poisoned!"; break;
                                case PStatusAction.Damage: message = "{0} was hurt by poison!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.Action), $"Invalid {s1p.Status} action: {s1p.Action}");
                            }
                            break;
                        case PStatus1.Poisoned:
                            switch (s1p.Action)
                            {
                                case PStatusAction.Added: message = "{0} was poisoned!"; break;
                                case PStatusAction.Damage: message = "{0} was hurt by poison!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.Action), $"Invalid {s1p.Status} action: {s1p.Action}");
                            }
                            break;
                        case PStatus1.Burned:
                            switch (s1p.Action)
                            {
                                case PStatusAction.Added: message = "{0} was burned!"; break;
                                case PStatusAction.Damage: message = "{0} was hurt by its burn!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.Action), $"Invalid {s1p.Status} action: {s1p.Action}");
                            }
                            break;
                        case PStatus1.Frozen:
                            switch (s1p.Action)
                            {
                                case PStatusAction.Activated: message = "{0} is frozen solid!"; break;
                                case PStatusAction.Added: message = "{0} was frozen solid!"; break;
                                case PStatusAction.Ended: message = "{0} thawed out!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.Action), $"Invalid {s1p.Status} action: {s1p.Action}");
                            }
                            break;
                        case PStatus1.Paralyzed:
                            switch (s1p.Action)
                            {
                                case PStatusAction.Activated: message = "{0} is paralyzed! It can't move!"; break;
                                case PStatusAction.Added: message = "{0} is paralyzed! It may be unable to move!"; break;
                                case PStatusAction.Cured: message = "{0} was cured of paralysis."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.Action), $"Invalid {s1p.Status} action: {s1p.Action}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(s1p.Status), $"Invalid status1: {s1p.Status}");
                    }
                    messageView.Add(battleView.Message = string.Format(message, pkmn.NameForTrainer(true)));
                    break;
                case PStatus2Packet s2p:
                    b = true;
                    pkmn = Battle.GetPokemon(s2p.PokemonId);
                    switch (s2p.Action)
                    {
                        case PStatusAction.Added:
                            pkmn.Status2 |= s2p.Status;
                            break;
                        case PStatusAction.Ended:
                            pkmn.Status2 &= ~s2p.Status;
                            break;
                    }
                    switch (s2p.Status)
                    {
                        case PStatus2.Confused:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Activated: message = "{0} is confused!"; break;
                                case PStatusAction.Added: message = "{0} became confused!"; break;
                                case PStatusAction.Damage: message = "It hurt itself in its confusion!"; break;
                                case PStatusAction.Ended: message = "{0} snapped out of its confusion."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid {s2p.Status} action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.Flinching:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Activated: message = "{0} flinched and couldn't move!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid {s2p.Status} action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.LeechSeed:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: message = "{0} was seeded!"; break;
                                case PStatusAction.Damage: message = "{0}'s health is sapped by Leech Seed!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid {s2p.Status} action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.Minimized:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid {s2p.Status} action: {s2p.Action}");
                            }
                        case PStatus2.Protected:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Activated:
                                case PStatusAction.Added: message = "{0} protected itself!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid {s2p.Status} action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.Pumped:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: message = "{0} is getting pumped!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid {s2p.Status} action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.Substitute:
                            battleView.PokemonPositionChanged(pkmn, PFieldPosition.None);
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: message = "{0} put in a substitute!"; break;
                                case PStatusAction.Damage: message = "The substitute took damage for {0}!"; b = false; break;
                                case PStatusAction.Ended: message = "{0}'s substitute faded!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid {s2p.Status} action: {s2p.Action}");
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
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid {s2p.Status} action: {s2p.Action}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(s2p.Status), $"Invalid status2: {s2p.Status}");
                    }
                    messageView.Add(battleView.Message = string.Format(message, pkmn.NameForTrainer(b)));
                    break;
                case PTeamStatusPacket tsp:
                    b = false;
                    team = Battle.Teams[tsp.Local ? 0 : 1];
                    switch (tsp.Action)
                    {
                        case PTeamStatusAction.Added:
                            team.Status |= tsp.Status;
                            break;
                        case PTeamStatusAction.Cleared:
                        case PTeamStatusAction.Ended:
                            team.Status &= ~tsp.Status;
                            break;
                    }
                    switch (tsp.Status)
                    {
                        case PTeamStatus.LightScreen:
                            switch (tsp.Action)
                            {
                                case PTeamStatusAction.Added: message = "Light Screen raised {0} team's Special Defense!"; break;
                                case PTeamStatusAction.Cleared:
                                case PTeamStatusAction.Ended: message = "{1} team's Light Screen wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.Action), $"Invalid {tsp.Status} action: {tsp.Action}");
                            }
                            break;
                        case PTeamStatus.Reflect:
                            switch (tsp.Action)
                            {
                                case PTeamStatusAction.Added: message = "Reflect raised {0} team's Defense!"; break;
                                case PTeamStatusAction.Cleared:
                                case PTeamStatusAction.Ended: message = "{1} team's Reflect wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.Action), $"Invalid {tsp.Status} action: {tsp.Action}");
                            }
                            break;
                        case PTeamStatus.Spikes:
                            switch (tsp.Action)
                            {
                                case PTeamStatusAction.Added:
                                    team.SpikeCount++;
                                    message = "Spikes were scattered all around the feet of {2} team!";
                                    break;
                                case PTeamStatusAction.Cleared:
                                    team.SpikeCount = 0;
                                    message = "The spikes disappeared from around {2} team's feet!";
                                    break;
                                case PTeamStatusAction.Damage: message = "{4} is hurt by the spikes!"; b = true; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.Action), $"Invalid {tsp.Status} action: {tsp.Action}");
                            }
                            break;
                        case PTeamStatus.StealthRock:
                            switch (tsp.Action)
                            {
                                case PTeamStatusAction.Added: message = "Pointed stones float in the air around {3} team!"; break;
                                case PTeamStatusAction.Cleared: message = "The pointed stones disappeared from around {2} team!"; break;
                                case PTeamStatusAction.Damage: message = "Pointed stones dug into {4}!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.Action), $"Invalid {tsp.Status} action: {tsp.Action}");
                            }
                            break;
                        case PTeamStatus.ToxicSpikes:
                            switch (tsp.Action)
                            {
                                case PTeamStatusAction.Added:
                                    team.ToxicSpikeCount++;
                                    message = "Poison spikes were scattered all around {2} team's feet!";
                                    break;
                                case PTeamStatusAction.Cleared:
                                    team.ToxicSpikeCount = 0;
                                    message = "The poison spikes disappeared from around {2} team's feet!";
                                    break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.Action), $"Invalid {tsp.Status} action: {tsp.Action}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(tsp.Status), $"Invalid team status: {tsp.Status}");
                    }
                    messageView.Add(battleView.Message = string.Format(message,
                        tsp.Local ? "your" : "the opposing",
                        tsp.Local ? "Your" : "The opposing",
                        tsp.Local ? "your" : "the foe's",
                        tsp.Local ? "your" : "your foe's",
                        Battle.GetPokemon(tsp.VictimId).NameForTrainer(b)
                        ));
                    break;
                case PTransformPacket tp:
                    {
                        PPokemon user = Battle.GetPokemon(tp.UserId),
                            target = Battle.GetPokemon(tp.TargetId);
                        user.Transform(target, tp.TargetAttack, tp.TargetDefense, tp.TargetSpAttack, tp.TargetSpDefense, tp.TargetSpeed, tp.TargetAbility, tp.TargetMoves);
                        battleView.PokemonPositionChanged(user, PFieldPosition.None);
                        messageView.Add(battleView.Message = string.Format("{0} transformed into {1}!", user.NameForTrainer(true), target.NameForTrainer(false)));
                        break;
                    }
                case PWeatherPacket wp:
                    switch (wp.Action)
                    {
                        case PWeatherAction.Added:
                            Battle.Weather = wp.Weather;
                            break;
                        case PWeatherAction.Ended:
                            Battle.Weather = PWeather.None;
                            break;
                    }
                    switch (wp.Weather)
                    {
                        case PWeather.Raining:
                            switch (wp.Action)
                            {
                                case PWeatherAction.Added: message = "It started to rain!"; break;
                                case PWeatherAction.Ended: message = "The rain stopped."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.Action), $"Invalid {wp.Weather} action: {wp.Action}");
                            }
                            break;
                        case PWeather.Sunny:
                            switch (wp.Action)
                            {
                                case PWeatherAction.Added: message = "The sunlight turned harsh!"; break;
                                case PWeatherAction.Ended: message = "The sunlight faded."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.Action), $"Invalid {wp.Weather} action: {wp.Action}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(wp.Weather), $"Invalid weather: {wp.Weather}");
                    }
                    messageView.Add(battleView.Message = message);
                    break;
                case PRequestActionsPacket _:
                    ActionsLoop(true);
                    break;
            }
            return false;
        }

        List<PPokemon> actions = new List<PPokemon>(3);
        List<PPokemon> standBy = new List<PPokemon>(3);
        void ActionsLoop(bool begin)
        {
            PPokemon pkmn;
            if (begin)
            {
                foreach (PPokemon p in Battle.Teams[0].Party)
                    p.SelectedAction.Decision = PDecision.None;
                actions.Clear();
                standBy.Clear();
                switch (Battle.BattleStyle)
                {
                    case PBattleStyle.Single:
                    case PBattleStyle.Rotation:
                        actions.Add(Battle.Teams[0].PokemonAtPosition(PFieldPosition.Center));
                        break;
                    case PBattleStyle.Double:
                        pkmn = Battle.Teams[0].PokemonAtPosition(PFieldPosition.Left);
                        if (pkmn != null)
                            actions.Add(pkmn);
                        pkmn = Battle.Teams[0].PokemonAtPosition(PFieldPosition.Right);
                        if (pkmn != null)
                            actions.Add(pkmn);
                        break;
                    case PBattleStyle.Triple:
                        pkmn = Battle.Teams[0].PokemonAtPosition(PFieldPosition.Left);
                        if (pkmn != null)
                            actions.Add(pkmn);
                        pkmn = Battle.Teams[0].PokemonAtPosition(PFieldPosition.Center);
                        if (pkmn != null)
                            actions.Add(pkmn);
                        pkmn = Battle.Teams[0].PokemonAtPosition(PFieldPosition.Right);
                        if (pkmn != null)
                            actions.Add(pkmn);
                        break;
                }
            }
            int i = actions.FindIndex(p => p.SelectedAction.Decision == PDecision.None);
            if (i == -1)
            {
                battleView.Message = $"Waiting for {Battle.Teams[1].TrainerName}...";
                Send(new PSubmitActionsPacket(actions.Select(p => p.SelectedAction).ToArray()));
            }
            else
            {
                if (i != 0)
                {
                    PAction prevAction = actions[i - 1].SelectedAction;
                    if (prevAction.Decision == PDecision.Switch)
                        standBy.Add(Battle.GetPokemon(prevAction.SwitchPokemonId));
                }
                battleView.Message = $"What will {actions[i].Shell.Nickname} do?";
                actionsView.DisplayActions(Battle.Teams[0].Party, actions[i], standBy);
            }
        }
        public void ActionSet()
        {
            ActionsLoop(false);
        }

        protected override void OnConnected()
        {
            Debug.WriteLine("Connected to {0}", Socket.RemoteEndPoint);
            messageView.Add(battleView.Message = "Waiting for players...");
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
