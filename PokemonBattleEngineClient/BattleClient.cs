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
        static readonly IPacketProcessor packetProcessor = new PBEPacketProcessor();
        public override IPacketProcessor PacketProcessor => packetProcessor;

        public PBEBattle Battle;
        readonly BattleView battleView;
        readonly ActionsView actionsView;
        readonly MessageView messageView;

        public BattleClient(string host, int port, BattleView battleView, ActionsView actionsView, MessageView messageView)
        {
            Configuration.Host = host;
            Configuration.Port = port;
            Configuration.BufferSize = 1024;

            Battle = new PBEBattle(PBEBattleStyle.Double);
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
            string message;
            switch (packet)
            {
                case PBEPlayerJoinedPacket pjp:
                    // TODO: What if it's a spectator?
                    message = string.Format("{0} joined the game.", pjp.TrainerName);
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    Battle.Teams[1].TrainerName = pjp.TrainerName;
                    Send(new PBEResponsePacket());
                    break;
                case PBEPartyRequestPacket _: // Temporary
                    message = "Sending team info...";
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    var team = new PBETeamShell { PlayerName = new string[] { "Sasha", "Nikki", "Lara", "Violet", "Naomi", "Rose", "Sabrina" }.Sample() };
                    var possiblePokemon = new List<PBEPokemonShell>
                    {
                        PBECompetitivePokemonShells.Absol_RU, PBECompetitivePokemonShells.Arceus_Normal_Uber, PBECompetitivePokemonShells.Azelf_VGC,
                        PBECompetitivePokemonShells.Azumarill_VGC,
                        PBECompetitivePokemonShells.Beedrill_NU, PBECompetitivePokemonShells.Blastoise_UU, PBECompetitivePokemonShells.Butterfree_RU,
                        PBECompetitivePokemonShells.Charizard_VGC, PBECompetitivePokemonShells.Cofagrigus_VGC, PBECompetitivePokemonShells.Cradily_OU,
                        PBECompetitivePokemonShells.Cresselia_VGC,
                        PBECompetitivePokemonShells.Crobat_VGC, PBECompetitivePokemonShells.Darkrai_Uber, PBECompetitivePokemonShells.Dialga_Uber,
                        PBECompetitivePokemonShells.Ditto_Uber, PBECompetitivePokemonShells.Espeon_Uber, PBECompetitivePokemonShells.Farfetchd_OU,
                        PBECompetitivePokemonShells.Flareon_RU,
                        PBECompetitivePokemonShells.Genesect_Uber, PBECompetitivePokemonShells.Giratina_Origin_Uber,
                        PBECompetitivePokemonShells.Jirachi_Uber, PBECompetitivePokemonShells.Jolteon_VGC, PBECompetitivePokemonShells.Latias_VGC,
                        PBECompetitivePokemonShells.Latios_VGC,
                        PBECompetitivePokemonShells.Marowak_VGC, PBECompetitivePokemonShells.Mesprit_UU, PBECompetitivePokemonShells.Mismagius_UU,
                        PBECompetitivePokemonShells.Ninetales_VGC, PBECompetitivePokemonShells.Palkia_Uber, PBECompetitivePokemonShells.Pikachu_VGC,
                        PBECompetitivePokemonShells.Rotom_Wash_VGC, PBECompetitivePokemonShells.Umbreon_UU, PBECompetitivePokemonShells.Uxie_VGC,
                        PBECompetitivePokemonShells.Vaporeon_VGC, PBECompetitivePokemonShells.Venusaur_VGC, PBECompetitivePokemonShells.Victini_Uber,
                    };
                    possiblePokemon.Shuffle();
                    team.Party.AddRange(possiblePokemon.Take(PBESettings.MaxPartySize));
                    Battle.Teams[0].TrainerName = team.PlayerName;
                    Send(new PBEPartyResponsePacket(team));
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
            PBEPokemon culprit, victim;
            int i;
            double d;
            bool b1, b2;
            PBEFieldPosition pos;
            string message;
            PBETeam team;

            switch (packet)
            {
                case PBEItemPacket ip:
                    b1 = true; // culprit caps
                    b2 = false; // victim caps
                    culprit = Battle.GetPokemon(ip.CulpritId);
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
                    message = string.Format(message,
                        culprit.NameForTrainer(b1),
                        victim.NameForTrainer(b2)
                        );
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBELimberPacket lp:
                    victim = Battle.GetPokemon(lp.PokemonId);
                    message = string.Format("{0}'s Limber", victim.Shell.Nickname); // TODO: Ability stuff
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEMagnitudePacket mp:
                    message = string.Format("Magnitude {0}!", mp.Magnitude);
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEMoveCritPacket _:
                    message = "A critical hit!";
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEMoveEffectivenessPacket mep:
                    victim = Battle.GetPokemon(mep.VictimId);
                    switch (mep.Effectiveness)
                    {
                        case PBEEffectiveness.Ineffective: message = "It doesn't affect {0}..."; break;
                        case PBEEffectiveness.NotVeryEffective: message = "It's not very effective..."; break;
                        case PBEEffectiveness.Normal: return true;
                        case PBEEffectiveness.SuperEffective: message = "It's super effective!"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mep.Effectiveness), $"Invalid effectiveness: {mep.Effectiveness}");
                    }
                    message = string.Format(message, victim.NameForTrainer(false));
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEMoveFailedPacket mfp:
                    culprit = Battle.GetPokemon(mfp.CulpritId);
                    victim = Battle.GetPokemon(mfp.VictimId);
                    switch (mfp.FailReason)
                    {
                        case PBEFailReason.AlreadyConfused: message = "{1} is already confused!"; break;
                        case PBEFailReason.Default: message = "But it failed!"; break;
                        case PBEFailReason.HPFull: message = "{0}'s HP is full!"; break;
                        case PBEFailReason.NoTarget: message = "There was no target..."; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mfp.FailReason), $"Invalid fail reason: {mfp.FailReason}");
                    }
                    message = string.Format(message, culprit.NameForTrainer(true), victim.NameForTrainer(true));
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEMoveMissedPacket mmp:
                    culprit = Battle.GetPokemon(mmp.CulpritId);
                    message = string.Format("{0}'s attack missed!", culprit.NameForTrainer(true));
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEMovePPChangedPacket mpcp:
                    victim = Battle.GetPokemon(mpcp.VictimId);
                    if (victim.LocalTeam)
                    {
                        i = Array.IndexOf(victim.Moves, mpcp.Move);
                        victim.PP[i] = (byte)(victim.PP[i] + mpcp.Change);
                    }
                    return true;
                case PBEMoveUsedPacket mup:
                    culprit = Battle.GetPokemon(mup.CulpritId);
                    // Reveal move if the Pokémon owns it and it's not already revealed
                    if (mup.OwnsMove && !culprit.Moves.Contains(mup.Move))
                    {
                        // Set the first unknown move to the used move
                        i = Array.IndexOf(culprit.Moves, PBEMove.MAX);
                        culprit.Moves[i] = mup.Move;
                    }
                    message = string.Format("{0} used {1}!", culprit.NameForTrainer(true), mup.Move);
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEPainSplitPacket psp:
                    message = "The battlers shared their pain!";
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEPkmnFaintedPacket pfap:
                    victim = Battle.GetPokemon(pfap.VictimId);
                    pos = victim.FieldPosition;
                    victim.FieldPosition = PBEFieldPosition.None;
                    battleView.UpdatePokemon(victim, pos);
                    message = string.Format("{0} fainted!", victim.NameForTrainer(true));
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEPkmnHPChangedPacket phcp:
                    victim = Battle.GetPokemon(phcp.VictimId);
                    victim.HP = (ushort)(victim.HP + phcp.Change);
                    battleView.UpdatePokemon(victim);
                    var hp = Math.Abs(phcp.Change);
                    d = (double)hp / victim.MaxHP;
                    message = string.Format("{0} {3} {1} ({2:P2}) HP!", victim.NameForTrainer(true), hp, d, phcp.Change <= 0 ? "lost" : "gained");
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEPkmnStatChangedPacket pscp:
                    victim = Battle.GetPokemon(pscp.VictimId);
                    PBEBattle.ApplyStatChange(victim, pscp.Stat, pscp.Change, ignoreSimple: true);
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
                    message = string.Format("{0}'s {1} {2}!", victim.NameForTrainer(true), pscp.Stat, message);
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEPkmnSwitchInPacket psip:
                    if (!psip.LocalTeam)
                    {
                        Battle.RemotePokemonSwitchedIn(psip);
                    }
                    culprit = Battle.GetPokemon(psip.PokemonId);
                    pos = culprit.FieldPosition;
                    culprit.FieldPosition = psip.FieldPosition;
                    battleView.UpdatePokemon(culprit, pos);
                    message = string.Format("{1} sent out {0}!", culprit.Shell.Nickname, Battle.Teams[culprit.LocalTeam ? 0 : 1].TrainerName);
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEPkmnSwitchOutPacket psop:
                    culprit = Battle.GetPokemon(psop.PokemonId);
                    pos = culprit.FieldPosition;
                    culprit.ClearForSwitch();
                    battleView.UpdatePokemon(culprit, pos);
                    message = string.Format("{1} withdrew {0}!", culprit.Shell.Nickname, Battle.Teams[culprit.LocalTeam ? 0 : 1].TrainerName);
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEPsychUpPacket pup:
                    culprit = Battle.GetPokemon(pup.CulpritId);
                    victim = Battle.GetPokemon(pup.VictimId);
                    culprit.AttackChange = victim.AttackChange = pup.AttackChange;
                    culprit.DefenseChange = victim.DefenseChange = pup.DefenseChange;
                    culprit.SpAttackChange = victim.SpAttackChange = pup.SpAttackChange;
                    culprit.SpDefenseChange = victim.SpDefenseChange = pup.SpDefenseChange;
                    culprit.SpeedChange = victim.SpeedChange = pup.SpeedChange;
                    culprit.AccuracyChange = victim.AccuracyChange = pup.AccuracyChange;
                    culprit.EvasionChange = victim.EvasionChange = pup.EvasionChange;
                    message = string.Format("{0} copied {1}'s stat changes!", culprit.Shell.Nickname, victim.Shell.Nickname);
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEStatus1Packet s1p:
                    culprit = Battle.GetPokemon(s1p.CulpritId);
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
                    battleView.UpdatePokemon(victim);
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
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEStatus2Packet s2p:
                    b1 = true; // victim caps
                    b2 = false; // culprit caps
                    culprit = Battle.GetPokemon(s2p.CulpritId);
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
                    switch (s2p.Status2)
                    {
                        case PBEStatus2.Airborne:
                            battleView.UpdatePokemon(victim);
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added:
                                    message = "{0} flew up high!";
                                    victim.LockedAction = victim.SelectedAction;
                                    break;
                                case PBEStatusAction.Ended:
                                    victim.LockedAction.Decision = PBEDecision.None;
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
                                case PBEStatusAction.Added: message = "{1} cut its own HP and laid a curse on {0}!"; b2 = true; b1 = false; break;
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
                            battleView.UpdatePokemon(victim);
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: message = "{0} put in a substitute!"; break;
                                case PBEStatusAction.Damage: message = "The substitute took damage for {0}!"; b1 = false; break;
                                case PBEStatusAction.Ended: message = "{0}'s substitute faded!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                            }
                            break;
                        case PBEStatus2.Underground:
                            battleView.UpdatePokemon(victim);
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added:
                                    message = "{0} burrowed its way under the ground!";
                                    victim.LockedAction = victim.SelectedAction;
                                    break;
                                case PBEStatusAction.Ended:
                                    victim.LockedAction.Decision = PBEDecision.None;
                                    return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                            }
                            break;
                        case PBEStatus2.Underwater:
                            battleView.UpdatePokemon(victim);
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added:
                                    message = "{0} hid underwater!";
                                    victim.LockedAction = victim.SelectedAction;
                                    break;
                                case PBEStatusAction.Ended:
                                    victim.LockedAction.Decision = PBEDecision.None;
                                    return true;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(s2p.Status2), $"Invalid status2: {s2p.Status2}");
                    }
                    message = string.Format(message, victim.NameForTrainer(b1), culprit.NameForTrainer(b2));
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBETeamStatusPacket tsp:
                    b1 = false;
                    team = Battle.Teams[tsp.LocalTeam ? 0 : 1];
                    victim = Battle.GetPokemon(tsp.VictimId);
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
                                case PBETeamStatusAction.Damage: message = "{4} is hurt by the spikes!"; b1 = true; break;
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
                        victim.NameForTrainer(b1)
                        );
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBETransformPacket tp:
                    culprit = Battle.GetPokemon(tp.CulpritId);
                    victim = Battle.GetPokemon(tp.VictimId);
                    culprit.Transform(victim, tp.TargetAttack, tp.TargetDefense, tp.TargetSpAttack, tp.TargetSpDefense, tp.TargetSpeed, tp.TargetAbility, tp.TargetType1, tp.TargetType2, tp.TargetMoves);
                    battleView.UpdatePokemon(culprit);
                    message = string.Format("{0} transformed into {1}!", culprit.NameForTrainer(true), victim.NameForTrainer(false));
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEWeatherPacket wp:
                    switch (wp.WeatherAction)
                    {
                        case PBEWeatherAction.Added:
                            Battle.Weather = wp.Weather;
                            break;
                        case PBEWeatherAction.Ended:
                            Battle.Weather = PBEWeather.None;
                            break;
                    }
                    switch (wp.Weather)
                    {
                        case PBEWeather.Raining:
                            switch (wp.WeatherAction)
                            {
                                case PBEWeatherAction.Added: message = "It started to rain!"; break;
                                case PBEWeatherAction.Ended: message = "The rain stopped."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction), $"Invalid {wp.Weather} action: {wp.WeatherAction}");
                            }
                            break;
                        case PBEWeather.Sunny:
                            switch (wp.WeatherAction)
                            {
                                case PBEWeatherAction.Added: message = "The sunlight turned harsh!"; break;
                                case PBEWeatherAction.Ended: message = "The sunlight faded."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction), $"Invalid {wp.Weather} action: {wp.WeatherAction}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(wp.Weather), $"Invalid weather: {wp.Weather}");
                    }
                    battleView.SetMessage(message);
                    messageView.Add(message);
                    break;
                case PBEActionsRequestPacket arp:
                    if (!arp.LocalTeam)
                    {
                        return true;
                    }
                    ActionsLoop(true);
                    break;
                case PBESwitchInRequestPacket sirp:
                    {
                        if (!sirp.LocalTeam)
                        {
                            return true;
                        }
                        int amt = sirp.Amount;
                        var switches = new List<Tuple<byte, PBEFieldPosition>>(amt);
                        PBEPokemon[] available = Battle.Teams[0].Party.Where(p => p.FieldPosition == PBEFieldPosition.None && p.HP > 0).ToArray();
                        var availablePositions = new List<PBEFieldPosition>();
                        switch (Battle.BattleStyle)
                        {
                            case PBEBattleStyle.Single:
                                availablePositions.Add(PBEFieldPosition.Center);
                                break;
                            case PBEBattleStyle.Double:
                                if (Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Left) == null)
                                {
                                    availablePositions.Add(PBEFieldPosition.Left);
                                }
                                if (Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Right) == null)
                                {
                                    availablePositions.Add(PBEFieldPosition.Right);
                                }
                                break;
                            case PBEBattleStyle.Triple:
                            case PBEBattleStyle.Rotation:
                                if (Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Left) == null)
                                {
                                    availablePositions.Add(PBEFieldPosition.Left);
                                }
                                if (Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Center) == null)
                                {
                                    availablePositions.Add(PBEFieldPosition.Center);
                                }
                                if (Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Right) == null)
                                {
                                    availablePositions.Add(PBEFieldPosition.Right);
                                }
                                break;
                        }
                        for (i = 0; i < amt; i++)
                        {
                            switches.Add(Tuple.Create(available[i].Id, availablePositions[i]));
                        }
                        Send(new PBESwitchInResponsePacket(switches.ToArray()));
                    }
                    break;
            }
            return false;
        }

        List<PBEPokemon> actions = new List<PBEPokemon>(3);
        List<PBEPokemon> standBy = new List<PBEPokemon>(3);
        public void ActionsLoop(bool begin)
        {
            PBEPokemon pkmn;
            if (begin)
            {
                foreach (PBEPokemon p in Battle.Teams[0].Party)
                {
                    p.SelectedAction.Decision = PBEDecision.None;
                }

                actions.Clear();
                standBy.Clear();
                switch (Battle.BattleStyle)
                {
                    case PBEBattleStyle.Single:
                    case PBEBattleStyle.Rotation:
                        actions.Add(Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Center));
                        break;
                    case PBEBattleStyle.Double:
                        pkmn = Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Left);
                        if (pkmn != null)
                        {
                            actions.Add(pkmn);
                        }

                        pkmn = Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Right);
                        if (pkmn != null)
                        {
                            actions.Add(pkmn);
                        }

                        break;
                    case PBEBattleStyle.Triple:
                        pkmn = Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Left);
                        if (pkmn != null)
                        {
                            actions.Add(pkmn);
                        }

                        pkmn = Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Center);
                        if (pkmn != null)
                        {
                            actions.Add(pkmn);
                        }

                        pkmn = Battle.Teams[0].PokemonAtPosition(PBEFieldPosition.Right);
                        if (pkmn != null)
                        {
                            actions.Add(pkmn);
                        }

                        break;
                }
            }
            int i = actions.FindIndex(p => p.SelectedAction.Decision == PBEDecision.None);
            if (i == -1)
            {
                battleView.SetMessage($"Waiting for {Battle.Teams[1].TrainerName}...");
                Send(new PBEActionsResponsePacket(actions.Select(p => p.SelectedAction).ToArray()));
            }
            else
            {
                if (i != 0)
                {
                    PBEAction prevAction = actions[i - 1].SelectedAction;
                    if (prevAction.Decision == PBEDecision.Switch)
                    {
                        standBy.Add(Battle.GetPokemon(prevAction.SwitchPokemonId));
                    }
                }
                battleView.SetMessage($"What will {actions[i].Shell.Nickname} do?");
                actionsView.DisplayActions(Battle.Teams[0].Party, actions[i], standBy);
            }
        }

        protected override void OnConnected()
        {
            Debug.WriteLine("Connected to {0}", Socket.RemoteEndPoint);
            string message = "Waiting for players...";
            battleView.SetMessage(message);
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
