using Ether.Network.Client;
using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngineClient.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Timers;

namespace Kermalis.PokemonBattleEngineClient
{
    class BattleClient : NetClient
    {
        readonly PBEPacketProcessor packetProcessor;
        public override IPacketProcessor PacketProcessor => packetProcessor;

        public PBEBattle Battle;
        public BattleView BattleView { get; }
        readonly ActionsView actionsView;
        readonly MessageView messageView;

        public BattleClient(string host, int port, BattleView battleView, ActionsView actionsView, MessageView messageView)
        {
            Configuration.Host = host;
            Configuration.Port = port;
            Configuration.BufferSize = 1024;

            PBESettings settings = PBESettings.DefaultSettings;
            Battle = new PBEBattle(PBEBattleFormat.Double, settings);
            packetProcessor = new PBEPacketProcessor(settings);
            BattleView = battleView;
            BattleView.SetBattle(Battle);
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
            string message;
            switch (packet)
            {
                case PBEPlayerJoinedPacket pjp:
                    // TODO: What if it's a spectator?
                    message = string.Format("{0} joined the game.", pjp.TrainerName);
                    BattleView.SetMessage(message);
                    messageView.Add(message);
                    Battle.Teams[1].TrainerName = pjp.TrainerName;
                    Send(new PBEResponsePacket());
                    break;
                case PBEPartyRequestPacket _: // Temporary
                    message = "Sending team info...";
                    BattleView.SetMessage(message);
                    messageView.Add(message);
                    PBETeamShell team = PBECompetitivePokemonShells.CreateRandomTeam(Battle.Settings.MaxPartySize);
                    Battle.Teams[0].TrainerName = team.PlayerName;
                    Send(new PBEPartyResponsePacket(team, Battle.Settings));
                    break;
                case PBESetPartyPacket spp:
                    Battle.SetTeamParty(true, spp.Party);
                    Send(new PBEResponsePacket());
                    break;
                default:
                    packetQueue.Add(packet);
                    Send(new PBEResponsePacket());
                    break;
            }
        }
        void PacketTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
        again:
            INetPacket packet = packetQueue.FirstOrDefault();
            if (packet == null)
            {
                return;
            }

            bool a = ProcessPacket(packet);
            packetQueue.RemoveAt(0);
            if (a)
            {
                goto again;
            }
        }
        // Returns true if the next packet should be run right away
        bool ProcessPacket(INetPacket packet)
        {
            switch (packet)
            {
                case PBEAbilityPacket ap:
                    {
                        PBEPokemon culprit = Battle.GetPokemon(ap.CulpritId),
                            victim = Battle.GetPokemon(ap.VictimId);
                        culprit.Ability = ap.Ability;
                        string message;
                        switch (ap.Ability)
                        {
                            case PBEAbility.Drizzle:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Weather: message = "{0}'s Drizzle activated!"; break; // Message is displayed from a weather packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                                break;
                            case PBEAbility.Drought:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Weather: message = "{0}'s Drought activated!"; break; // Message is displayed from a weather packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                                break;
                            case PBEAbility.IceBody:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.RestoredHP: message = "{0}'s Ice Body activated!"; break; // Message is displayed from a hp changed packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                                break;
                            case PBEAbility.Imposter:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.ChangedAppearance: message = "{0}'s Imposter activated!"; break; // Message is displayed from a status2 packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                                break;
                            case PBEAbility.Levitate:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Damage: message = "{0}'s Levitate activated!"; break; // Message is displayed from an effectiveness packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                                break;
                            case PBEAbility.Limber:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.CuredStatus: // Message is displayed from a status1 packet
                                    case PBEAbilityAction.PreventedStatus: message = "{0}'s Limber activated!"; break; // Message is displayed from an effectiveness packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                                break;
                            case PBEAbility.RainDish:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.RestoredHP: message = "{0}'s Rain Dish activated!"; break; // Message is displayed from a hp changed packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                                break;
                            case PBEAbility.SandStream:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Weather: message = "{0}'s Sand Stream activated!"; break; // Message is displayed from a weather packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                                break;
                            case PBEAbility.SnowWarning:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Weather: message = "{0}'s Snow Warning activated!"; break; // Message is displayed from a weather packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                                break;
                            case PBEAbility.SolarPower:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Damage: message = "{0}'s Solar Power activated!"; break; // Message is displayed from a hp changed packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                                break;
                            default: throw new ArgumentOutOfRangeException(nameof(ap.Ability), $"Invalid ability: {ap.Ability}");
                        }
                        message = string.Format(message, culprit.NameForTrainer(true), victim.NameForTrainer(false));
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEItemPacket ip:
                    {
                        PBEPokemon culprit = Battle.GetPokemon(ip.CulpritId),
                            victim = Battle.GetPokemon(ip.VictimId);
                        switch (ip.ItemAction)
                        {
                            case PBEItemAction.CausedDamage:
                            case PBEItemAction.RestoredHP:
                                culprit.Item = ip.Item;
                                break;
                            case PBEItemAction.Consumed:
                                culprit.Item = PBEItem.None;
                                break;
                        }
                        string message;
                        switch (ip.Item)
                        {
                            case PBEItem.BlackSludge:
                                switch (ip.ItemAction)
                                {
                                    case PBEItemAction.CausedDamage: message = "{0} is hurt by its Black Sludge!"; break;
                                    case PBEItemAction.RestoredHP: message = "{0} restored a little HP using its Black Sludge!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction), $"Invalid {ip.Item} action: {ip.ItemAction}");
                                }
                                break;
                            case PBEItem.Leftovers:
                                switch (ip.ItemAction)
                                {
                                    case PBEItemAction.RestoredHP: message = "{0} restored a little HP using its Leftovers!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction), $"Invalid {ip.Item} action: {ip.ItemAction}");
                                }
                                break;
                            case PBEItem.PowerHerb:
                                switch (ip.ItemAction)
                                {
                                    case PBEItemAction.Consumed: message = "{0} became fully charged due to its Power Herb!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction), $"Invalid {ip.Item} action: {ip.ItemAction}");
                                }
                                break;
                            default: throw new ArgumentOutOfRangeException(nameof(ip.Item), $"Invalid item: {ip.Item}");
                        }
                        message = string.Format(message, culprit.NameForTrainer(true), victim.NameForTrainer(false));
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEMagnitudePacket mp:
                    {
                        string message = string.Format("Magnitude {0}!", mp.Magnitude);
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEMoveCritPacket _:
                    {
                        string message = "A critical hit!";
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEMoveEffectivenessPacket mep:
                    {
                        PBEPokemon victim = Battle.GetPokemon(mep.VictimId);
                        string message;
                        switch (mep.Effectiveness)
                        {
                            case PBEEffectiveness.Ineffective: message = "It doesn't affect {0}..."; break;
                            case PBEEffectiveness.NotVeryEffective: message = "It's not very effective..."; break;
                            case PBEEffectiveness.Normal: return true;
                            case PBEEffectiveness.SuperEffective: message = "It's super effective!"; break;
                            default: throw new ArgumentOutOfRangeException(nameof(mep.Effectiveness), $"Invalid effectiveness: {mep.Effectiveness}");
                        }
                        message = string.Format(message, victim.NameForTrainer(false));
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEMoveFailedPacket mfp:
                    {
                        PBEPokemon culprit = Battle.GetPokemon(mfp.CulpritId),
                            victim = Battle.GetPokemon(mfp.VictimId);
                        string message;
                        switch (mfp.FailReason)
                        {
                            case PBEFailReason.AlreadyConfused: message = "{1} is already confused!"; break;
                            case PBEFailReason.Default: message = "But it failed!"; break;
                            case PBEFailReason.HPFull: message = "{0}'s HP is full!"; break;
                            case PBEFailReason.NoTarget: message = "There was no target..."; break;
                            default: throw new ArgumentOutOfRangeException(nameof(mfp.FailReason), $"Invalid fail reason: {mfp.FailReason}");
                        }
                        message = string.Format(message, culprit.NameForTrainer(true), victim.NameForTrainer(true));
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEMoveMissedPacket mmp:
                    {
                        PBEPokemon culprit = Battle.GetPokemon(mmp.CulpritId);
                        string message = string.Format("{0}'s attack missed!", culprit.NameForTrainer(true));
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEMovePPChangedPacket mpcp:
                    {
                        PBEPokemon victim = Battle.GetPokemon(mpcp.VictimId);
                        if (victim.LocalTeam)
                        {
                            int i = Array.IndexOf(victim.Moves, mpcp.Move);
                            victim.PP[i] = (byte)(victim.PP[i] + mpcp.Change);
                        }
                        return true;
                    }
                case PBEMoveUsedPacket mup:
                    {
                        PBEPokemon culprit = Battle.GetPokemon(mup.CulpritId);
                        // Reveal move if the Pokémon owns it and it's not already revealed
                        if (mup.OwnsMove && !culprit.Moves.Contains(mup.Move))
                        {
                            // Set the first unknown move to the used move
                            int i = Array.IndexOf(culprit.Moves, PBEMove.MAX);
                            culprit.Moves[i] = mup.Move;
                        }
                        string message = string.Format("{0} used {1}!", culprit.NameForTrainer(true), mup.Move);
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEPainSplitPacket psp:
                    {
                        string message = "The battlers shared their pain!";
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEPkmnFaintedPacket pfap:
                    {
                        PBEPokemon victim = Battle.GetPokemon(pfap.VictimId);
                        Battle.ActiveBattlers.Remove(victim);
                        PBEFieldPosition oldPos = victim.FieldPosition;
                        victim.FieldPosition = PBEFieldPosition.None;
                        BattleView.UpdatePokemon(victim, oldPos);
                        string message = string.Format("{0} fainted!", victim.NameForTrainer(true));
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEPkmnHPChangedPacket phcp:
                    {
                        PBEPokemon victim = Battle.GetPokemon(phcp.VictimId);
                        victim.HP = (ushort)(victim.HP + phcp.Change);
                        BattleView.UpdatePokemon(victim);
                        int hp = Math.Abs(phcp.Change);
                        double percentage = (double)hp / victim.MaxHP;
                        string message = string.Format("{0} {1} {2} ({3:P2}) HP!", victim.NameForTrainer(true), phcp.Change <= 0 ? "lost" : "gained", hp, percentage);
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEPkmnStatChangedPacket pscp:
                    {
                        PBEPokemon pkmn = Battle.GetPokemon(pscp.PokemonId);
                        PBEBattle.ApplyStatChange(Battle, pkmn, pscp.Stat, pscp.Change, false, true);
                        string message;
                        switch (pscp.Change)
                        {
                            case -2: message = "harshly fell"; break;
                            case -1: message = "fell"; break;
                            case +1: message = "rose"; break;
                            case +2: message = "rose sharply"; break;
                            default:
                                if (pscp.IsTooMuch && pscp.Change < 0)
                                {
                                    message = "won't go lower";
                                }
                                else if (pscp.IsTooMuch && pscp.Change > 0)
                                {
                                    message = "won't go higher";
                                }
                                else if (pscp.Change <= -3)
                                {
                                    message = "severely fell";
                                }
                                else if (pscp.Change >= 3)
                                {
                                    message = "rose drastically";
                                }
                                else
                                {
                                    throw new ArgumentOutOfRangeException(nameof(pscp.Change), $"Invalid stat change: {pscp.Change}"); // +0
                                }
                                break;
                        }
                        message = string.Format("{0}'s {1} {2}!", pkmn.NameForTrainer(true), pscp.Stat, message);
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEPkmnSwitchInPacket psip:
                    {
                        // Add new unknown foes
                        if (!psip.LocalTeam)
                        {
                            Battle.RemotePokemonSwitchedIn(psip);
                        }
                        foreach (PBEPkmnSwitchInPacket.PBESwitchInInfo info in psip.SwitchIns)
                        {
                            PBEPokemon pkmn = Battle.GetPokemon(info.PokemonId);
                            Battle.ActiveBattlers.Add(pkmn);
                            pkmn.FieldPosition = info.FieldPosition;
                            BattleView.UpdatePokemon(pkmn);
                        }
                        string message = string.Format("{1} sent out {0}!",
                            PBEUtils.Andify(psip.SwitchIns.Select(s => s.Nickname)),
                            Battle.Teams[psip.LocalTeam ? 0 : 1].TrainerName);
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEPkmnSwitchOutPacket psop:
                    {
                        string message;
                        PBEPokemon pkmn = Battle.GetPokemon(psop.PokemonId);
                        Battle.ActiveBattlers.Remove(pkmn);
                        PBEFieldPosition oldPos = pkmn.FieldPosition;
                        pkmn.ClearForSwitch(Battle.Settings);
                        BattleView.UpdatePokemon(pkmn, oldPos);
                        message = string.Format("{1} withdrew {0}!", pkmn.Shell.Nickname, Battle.Teams[pkmn.LocalTeam ? 0 : 1].TrainerName);
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEPsychUpPacket pup:
                    {
                        PBEPokemon user = Battle.GetPokemon(pup.UserId),
                            target = Battle.GetPokemon(pup.TargetId);
                        user.AttackChange = target.AttackChange = pup.AttackChange;
                        user.DefenseChange = target.DefenseChange = pup.DefenseChange;
                        user.SpAttackChange = target.SpAttackChange = pup.SpAttackChange;
                        user.SpDefenseChange = target.SpDefenseChange = pup.SpDefenseChange;
                        user.SpeedChange = target.SpeedChange = pup.SpeedChange;
                        user.AccuracyChange = target.AccuracyChange = pup.AccuracyChange;
                        user.EvasionChange = target.EvasionChange = pup.EvasionChange;
                        string message = string.Format("{0} copied {1}'s stat changes!", user.Shell.Nickname, target.Shell.Nickname);
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEStatus1Packet s1p:
                    {
                        PBEPokemon culprit = Battle.GetPokemon(s1p.CulpritId),
                            victim = Battle.GetPokemon(s1p.VictimId);
                        switch (s1p.StatusAction)
                        {
                            case PBEStatusAction.Added:
                                victim.Status1 = s1p.Status1;
                                break;
                            case PBEStatusAction.Cured:
                            case PBEStatusAction.Ended:
                                victim.Status1 = PBEStatus1.None;
                                break;
                        }
                        BattleView.UpdatePokemon(victim);
                        string message;
                        switch (s1p.Status1)
                        {
                            case PBEStatus1.Asleep:
                                switch (s1p.StatusAction)
                                {
                                    case PBEStatusAction.Activated: message = "{0} is fast asleep."; break;
                                    case PBEStatusAction.Added: message = "{0} fell asleep!"; break;
                                    case PBEStatusAction.Ended: message = "{0} woke up!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction), $"Invalid {s1p.Status1} action: {s1p.StatusAction}");
                                }
                                break;
                            case PBEStatus1.BadlyPoisoned:
                                switch (s1p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{0} was badly poisoned!"; break;
                                    case PBEStatusAction.Damage: message = "{0} was hurt by poison!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction), $"Invalid {s1p.Status1} action: {s1p.StatusAction}");
                                }
                                break;
                            case PBEStatus1.Poisoned:
                                switch (s1p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{0} was poisoned!"; break;
                                    case PBEStatusAction.Damage: message = "{0} was hurt by poison!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction), $"Invalid {s1p.Status1} action: {s1p.StatusAction}");
                                }
                                break;
                            case PBEStatus1.Burned:
                                switch (s1p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{0} was burned!"; break;
                                    case PBEStatusAction.Damage: message = "{0} was hurt by its burn!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction), $"Invalid {s1p.Status1} action: {s1p.StatusAction}");
                                }
                                break;
                            case PBEStatus1.Frozen:
                                switch (s1p.StatusAction)
                                {
                                    case PBEStatusAction.Activated: message = "{0} is frozen solid!"; break;
                                    case PBEStatusAction.Added: message = "{0} was frozen solid!"; break;
                                    case PBEStatusAction.Ended: message = "{0} thawed out!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction), $"Invalid {s1p.Status1} action: {s1p.StatusAction}");
                                }
                                break;
                            case PBEStatus1.Paralyzed:
                                switch (s1p.StatusAction)
                                {
                                    case PBEStatusAction.Activated: message = "{0} is paralyzed! It can't move!"; break;
                                    case PBEStatusAction.Added: message = "{0} is paralyzed! It may be unable to move!"; break;
                                    case PBEStatusAction.Cured: message = "{0} was cured of paralysis."; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction), $"Invalid {s1p.Status1} action: {s1p.StatusAction}");
                                }
                                break;
                            default: throw new ArgumentOutOfRangeException(nameof(s1p.Status1), $"Invalid status1: {s1p.Status1}");
                        }
                        message = string.Format(message, victim.NameForTrainer(true));
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEStatus2Packet s2p:
                    {
                        PBEPokemon culprit = Battle.GetPokemon(s2p.CulpritId),
                            victim = Battle.GetPokemon(s2p.VictimId);
                        switch (s2p.StatusAction)
                        {
                            case PBEStatusAction.Added:
                                victim.Status2 |= s2p.Status2;
                                break;
                            case PBEStatusAction.Ended:
                                victim.Status2 &= ~s2p.Status2;
                                break;
                        }
                        string message;
                        bool culpritCaps = false,
                            victimCaps = true;
                        switch (s2p.Status2)
                        {
                            case PBEStatus2.Airborne:
                                BattleView.UpdatePokemon(victim);
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added:
                                        message = "{0} flew up high!";
                                        if (victim.LocalTeam)
                                        {
                                            victim.TempLockedMove = victim.SelectedAction.FightMove;
                                            victim.TempLockedTargets = victim.SelectedAction.FightTargets;
                                        }
                                        break;
                                    case PBEStatusAction.Ended:
                                        if (victim.LocalTeam)
                                        {
                                            victim.TempLockedMove = PBEMove.None;
                                            victim.TempLockedTargets = PBETarget.None;
                                        }
                                        return true;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.Confused:
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Activated: message = "{0} is confused!"; break;
                                    case PBEStatusAction.Added: message = "{0} became confused!"; break;
                                    case PBEStatusAction.Damage: message = "It hurt itself in its confusion!"; break;
                                    case PBEStatusAction.Ended: message = "{0} snapped out of its confusion."; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.Cursed:
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{1} cut its own HP and laid a curse on {0}!"; culpritCaps = true; victimCaps = false; break;
                                    case PBEStatusAction.Damage: message = "{0} is afflicted by the curse!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.Flinching:
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Activated: message = "{0} flinched and couldn't move!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.LeechSeed:
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{0} was seeded!"; break;
                                    case PBEStatusAction.Damage: message = "{0}'s health is sapped by Leech Seed!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.Minimized:
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added: return true;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                            case PBEStatus2.Protected:
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Activated:
                                    case PBEStatusAction.Added: message = "{0} protected itself!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.Pumped:
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{0} is getting pumped!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.Substitute:
                                BattleView.UpdatePokemon(victim);
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{0} put in a substitute!"; break;
                                    case PBEStatusAction.Damage: message = "The substitute took damage for {0}!"; victimCaps = false; break;
                                    case PBEStatusAction.Ended: message = "{0}'s substitute faded!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.Transformed: // Victim is the user, culprit is the target (victim receives the transformed flag)
                                victim.Transform(culprit, Battle.Settings);
                                BattleView.UpdatePokemon(victim);
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{0} transformed into {1}!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.Underground:
                                BattleView.UpdatePokemon(victim);
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added:
                                        message = "{0} burrowed its way under the ground!";
                                        if (victim.LocalTeam)
                                        {
                                            victim.TempLockedMove = victim.SelectedAction.FightMove;
                                            victim.TempLockedTargets = victim.SelectedAction.FightTargets;
                                        }
                                        break;
                                    case PBEStatusAction.Ended:
                                        if (victim.LocalTeam)
                                        {
                                            victim.TempLockedMove = PBEMove.None;
                                            victim.TempLockedTargets = PBETarget.None;
                                        }
                                        return true;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.Underwater:
                                BattleView.UpdatePokemon(victim);
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added:
                                        message = "{0} hid underwater!";
                                        if (victim.LocalTeam)
                                        {
                                            victim.TempLockedMove = victim.SelectedAction.FightMove;
                                            victim.TempLockedTargets = victim.SelectedAction.FightTargets;
                                        }
                                        break;
                                    case PBEStatusAction.Ended:
                                        if (victim.LocalTeam)
                                        {
                                            victim.TempLockedMove = PBEMove.None;
                                            victim.TempLockedTargets = PBETarget.None;
                                        }
                                        return true;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            default: throw new ArgumentOutOfRangeException(nameof(s2p.Status2), $"Invalid status2: {s2p.Status2}");
                        }
                        message = string.Format(message, victim.NameForTrainer(victimCaps), culprit.NameForTrainer(culpritCaps));
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBETeamStatusPacket tsp:
                    {
                        PBETeam team = Battle.Teams[tsp.LocalTeam ? 0 : 1];
                        PBEPokemon victim = Battle.GetPokemon(tsp.VictimId);
                        switch (tsp.TeamStatusAction)
                        {
                            case PBETeamStatusAction.Added:
                                team.Status |= tsp.TeamStatus;
                                break;
                            case PBETeamStatusAction.Cleared:
                            case PBETeamStatusAction.Ended:
                                team.Status &= ~tsp.TeamStatus;
                                break;
                        }
                        string message;
                        bool victimCaps = false;
                        switch (tsp.TeamStatus)
                        {
                            case PBETeamStatus.LightScreen:
                                switch (tsp.TeamStatusAction)
                                {
                                    case PBETeamStatusAction.Added: message = "Light Screen raised {0} team's Special Defense!"; break;
                                    case PBETeamStatusAction.Cleared:
                                    case PBETeamStatusAction.Ended: message = "{1} team's Light Screen wore off!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction), $"Invalid {tsp.TeamStatus} action: {tsp.TeamStatusAction}");
                                }
                                break;
                            case PBETeamStatus.Reflect:
                                switch (tsp.TeamStatusAction)
                                {
                                    case PBETeamStatusAction.Added: message = "Reflect raised {0} team's Defense!"; break;
                                    case PBETeamStatusAction.Cleared:
                                    case PBETeamStatusAction.Ended: message = "{1} team's Reflect wore off!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction), $"Invalid {tsp.TeamStatus} action: {tsp.TeamStatusAction}");
                                }
                                break;
                            case PBETeamStatus.Spikes:
                                switch (tsp.TeamStatusAction)
                                {
                                    case PBETeamStatusAction.Added:
                                        team.SpikeCount++;
                                        message = "Spikes were scattered all around the feet of {2} team!";
                                        break;
                                    case PBETeamStatusAction.Cleared:
                                        team.SpikeCount = 0;
                                        message = "The spikes disappeared from around {2} team's feet!";
                                        break;
                                    case PBETeamStatusAction.Damage: message = "{4} is hurt by the spikes!"; victimCaps = true; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction), $"Invalid {tsp.TeamStatus} action: {tsp.TeamStatusAction}");
                                }
                                break;
                            case PBETeamStatus.StealthRock:
                                switch (tsp.TeamStatusAction)
                                {
                                    case PBETeamStatusAction.Added: message = "Pointed stones float in the air around {3} team!"; break;
                                    case PBETeamStatusAction.Cleared: message = "The pointed stones disappeared from around {2} team!"; break;
                                    case PBETeamStatusAction.Damage: message = "Pointed stones dug into {4}!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction), $"Invalid {tsp.TeamStatus} action: {tsp.TeamStatusAction}");
                                }
                                break;
                            case PBETeamStatus.ToxicSpikes:
                                switch (tsp.TeamStatusAction)
                                {
                                    case PBETeamStatusAction.Added:
                                        team.ToxicSpikeCount++;
                                        message = "Poison spikes were scattered all around {2} team's feet!";
                                        break;
                                    case PBETeamStatusAction.Cleared:
                                        team.ToxicSpikeCount = 0;
                                        message = "The poison spikes disappeared from around {2} team's feet!";
                                        break;
                                    default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction), $"Invalid {tsp.TeamStatus} action: {tsp.TeamStatusAction}");
                                }
                                break;
                            default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatus), $"Invalid team status: {tsp.TeamStatus}");
                        }
                        message = string.Format(message,
                            tsp.LocalTeam ? "your" : "the opposing",
                            tsp.LocalTeam ? "Your" : "The opposing",
                            tsp.LocalTeam ? "your" : "the foe's",
                            tsp.LocalTeam ? "your" : "your foe's",
                            victim.NameForTrainer(victimCaps)
                            );
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBETransformPacket tp:
                    {
                        PBEPokemon user = Battle.GetPokemon(tp.UserId),
                            target = Battle.GetPokemon(tp.TargetId);
                        target.Attack = tp.TargetAttack;
                        target.Defense = tp.TargetDefense;
                        target.SpAttack = tp.TargetSpAttack;
                        target.SpDefense = tp.TargetSpDefense;
                        target.Speed = tp.TargetSpeed;
                        target.AttackChange = tp.TargetAttackChange;
                        target.DefenseChange = tp.TargetDefenseChange;
                        target.SpAttackChange = tp.TargetSpAttackChange;
                        target.SpDefenseChange = tp.TargetSpDefenseChange;
                        target.SpeedChange = tp.TargetSpeedChange;
                        target.AccuracyChange = tp.TargetAccuracyChange;
                        target.EvasionChange = tp.TargetEvasionChange;
                        target.Ability = tp.TargetAbility;
                        target.Type1 = tp.TargetType1;
                        target.Type2 = tp.TargetType2;
                        target.Weight = tp.TargetWeight;
                        target.Moves = tp.TargetMoves;
                        return true;
                    }
                case PBEWeatherPacket wp:
                    {
                        PBEPokemon victim = Battle.GetPokemon(wp.VictimId);
                        switch (wp.WeatherAction)
                        {
                            case PBEWeatherAction.Added:
                                Battle.Weather = wp.Weather;
                                break;
                            case PBEWeatherAction.Ended:
                                Battle.Weather = PBEWeather.None;
                                break;
                        }
                        string message;
                        switch (wp.Weather)
                        {
                            case PBEWeather.Hailstorm:
                                switch (wp.WeatherAction)
                                {
                                    case PBEWeatherAction.Added: message = "It started to hail!"; break;
                                    case PBEWeatherAction.CausedDamage: message = "{0} is buffeted by the hail!"; break;
                                    case PBEWeatherAction.Ended: message = "The hail stopped."; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction), $"Invalid {wp.Weather} action: {wp.WeatherAction}");
                                }
                                break;
                            case PBEWeather.HarshSunlight:
                                switch (wp.WeatherAction)
                                {
                                    case PBEWeatherAction.Added: message = "The sunlight turned harsh!"; break;
                                    case PBEWeatherAction.Ended: message = "The sunlight faded."; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction), $"Invalid {wp.Weather} action: {wp.WeatherAction}");
                                }
                                break;
                            case PBEWeather.Rain:
                                switch (wp.WeatherAction)
                                {
                                    case PBEWeatherAction.Added: message = "It started to rain!"; break;
                                    case PBEWeatherAction.Ended: message = "The rain stopped."; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction), $"Invalid {wp.Weather} action: {wp.WeatherAction}");
                                }
                                break;
                            case PBEWeather.Sandstorm:
                                switch (wp.WeatherAction)
                                {
                                    case PBEWeatherAction.Added: message = "A sandstorm kicked up!"; break;
                                    case PBEWeatherAction.CausedDamage: message = "{0} is buffeted by the sandstorm!"; break;
                                    case PBEWeatherAction.Ended: message = "The sandstorm subsided."; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction), $"Invalid {wp.Weather} action: {wp.WeatherAction}");
                                }
                                break;
                            default: throw new ArgumentOutOfRangeException(nameof(wp.Weather), $"Invalid weather: {wp.Weather}");
                        }
                        message = string.Format(message, victim.NameForTrainer(true));
                        BattleView.SetMessage(message);
                        messageView.Add(message);
                        break;
                    }
                case PBEActionsRequestPacket arp:
                    {
                        if (arp.LocalTeam)
                        {
                            ActionsLoop(true);
                        }
                        return true;
                    }
                case PBESwitchInRequestPacket sirp:
                    {
                        Battle.Teams[sirp.LocalTeam ? 0 : 1].SwitchInsRequired = sirp.Amount;
                        if (sirp.LocalTeam)
                        {
                            SwitchesLoop(true);
                        }
                        else if (Battle.Teams[0].SwitchInsRequired == 0) // Don't display this message if we're in switchesloop because it'd overwrite the messages we need to see.
                        {
                            BattleView.SetMessage($"Waiting for {Battle.Teams[1].TrainerName}...");
                        }
                        return true;
                    }
            }
            return false;
        }

        readonly List<PBEPokemon> actions = new List<PBEPokemon>(3);
        public List<PBEPokemon> StandBy { get; } = new List<PBEPokemon>(3);
        public void ActionsLoop(bool begin)
        {
            if (begin)
            {
                foreach (PBEPokemon pkmn in Battle.Teams[0].Party)
                {
                    pkmn.SelectedAction.Decision = PBEDecision.None;
                }
                actions.Clear();
                actions.AddRange(Battle.Teams[0].ActiveBattlers.OrderBy(p => p.FieldPosition));
                StandBy.Clear();
            }
            int i = actions.FindIndex(p => p.SelectedAction.Decision == PBEDecision.None);
            if (i == -1)
            {
                foreach (PBEPokemon pkmn in actions)
                {
                    if (pkmn.SelectedAction.Decision == PBEDecision.Fight)
                    {
                        if (pkmn.Item == PBEItem.ChoiceBand || pkmn.Item == PBEItem.ChoiceScarf || pkmn.Item == PBEItem.ChoiceSpecs)
                        {
                            pkmn.ChoiceLockedMove = pkmn.SelectedAction.FightMove;
                        }
                    }
                }
                BattleView.SetMessage($"Waiting for {Battle.Teams[1].TrainerName}...");
                Send(new PBEActionsResponsePacket(actions.Select(p => p.SelectedAction)));
            }
            else
            {
                BattleView.SetMessage($"What will {actions[i].Shell.Nickname} do?");
                actionsView.DisplayActions(actions[i]);
            }
        }
        public List<Tuple<byte, PBEFieldPosition>> Switches { get; } = new List<Tuple<byte, PBEFieldPosition>>(3);
        public List<PBEFieldPosition> PositionStandBy { get; } = new List<PBEFieldPosition>(3);
        public void SwitchesLoop(bool begin)
        {
            if (begin)
            {
                Switches.Clear();
                StandBy.Clear();
                PositionStandBy.Clear();
            }
            else
            {
                Battle.Teams[0].SwitchInsRequired--;
            }
            if (Battle.Teams[0].SwitchInsRequired == 0)
            {
                BattleView.SetMessage($"Waiting for {(Battle.Teams[1].SwitchInsRequired > 0 ? Battle.Teams[1].TrainerName : "server")}...");
                Send(new PBESwitchInResponsePacket(Switches));
            }
            else
            {
                BattleView.SetMessage($"You must send in {Battle.Teams[0].SwitchInsRequired} Pokémon.");
                actionsView.DisplaySwitches();
            }
        }

        protected override void OnConnected()
        {
            Debug.WriteLine("Connected to {0}", Socket.RemoteEndPoint);
            string message = "Waiting for players...";
            BattleView.SetMessage(message);
            messageView.Add(message);
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
