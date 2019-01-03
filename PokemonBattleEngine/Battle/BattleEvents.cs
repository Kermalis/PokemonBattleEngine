using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        public delegate void BattleEvent(PBEBattle battle, INetPacket packet);
        public event BattleEvent OnNewEvent;

        void BroadcastAbility(PBEPokemon culprit, PBEPokemon victim, PBEAbility ability, PBEAbilityAction abilityAction)
            => OnNewEvent?.Invoke(this, new PBEAbilityPacket(culprit, victim, ability, abilityAction));
        void BroadcastItem(PBEPokemon culprit, PBEPokemon victim, PBEItem item, PBEItemAction itemAction)
            => OnNewEvent?.Invoke(this, new PBEItemPacket(culprit, victim, item, itemAction));
        void BroadcastMoveCrit()
            => OnNewEvent?.Invoke(this, new PBEMoveCritPacket());
        void BroadcastEffectiveness(PBEPokemon victim, PBEEffectiveness effectiveness)
            => OnNewEvent?.Invoke(this, new PBEMoveEffectivenessPacket(victim, effectiveness));
        void BroadcastMoveFailed(PBEPokemon culprit, PBEPokemon victim, PBEFailReason failReason)
            => OnNewEvent?.Invoke(this, new PBEMoveFailedPacket(culprit, victim, failReason));
        void BroadcastMoveMissed(PBEPokemon culprit, PBEPokemon victim)
            => OnNewEvent?.Invoke(this, new PBEMoveMissedPacket(culprit, victim));
        void BroadcastMovePPChanged(PBEPokemon victim, PBEMove move, short change)
            => OnNewEvent?.Invoke(this, new PBEMovePPChangedPacket(victim, move, change));
        void BroadcastMoveUsed(PBEPokemon culprit, PBEMove move)
            => OnNewEvent?.Invoke(this, new PBEMoveUsedPacket(culprit, move));
        void BroadcastPkmnFainted(PBEPokemon victim)
            => OnNewEvent?.Invoke(this, new PBEPkmnFaintedPacket(victim));
        void BroadcastPkmnHPChanged(PBEPokemon victim, int change)
            => OnNewEvent?.Invoke(this, new PBEPkmnHPChangedPacket(victim, change));
        void BroadcastPkmnStatChanged(PBEPokemon victim, PBEStat stat, short change, bool isTooMuch)
            => OnNewEvent?.Invoke(this, new PBEPkmnStatChangedPacket(victim, stat, change, isTooMuch));
        void BroadcastPkmnSwitchIn(PBETeam team, IEnumerable<PBEPokemon> pokemon, bool forced)
            => OnNewEvent?.Invoke(this, new PBEPkmnSwitchInPacket(team, pokemon, forced));
        void BroadcastPkmnSwitchOut(PBEPokemon pokemon, bool forced)
            => OnNewEvent?.Invoke(this, new PBEPkmnSwitchOutPacket(pokemon, forced));
        void BroadcastPsychUp(PBEPokemon user, PBEPokemon target)
            => OnNewEvent?.Invoke(this, new PBEPsychUpPacket(user, target));

        void BroadcastDraggedOut(PBEPokemon victim)
            => OnNewEvent?.Invoke(this, new PBESpecialMessagePacket(PBESpecialMessage.DraggedOut, victim.Id));
        void BroadcastMagnitude(byte magnitude)
            => OnNewEvent?.Invoke(this, new PBESpecialMessagePacket(PBESpecialMessage.Magnitude, magnitude));
        void BroadcastPainSplit(PBEPokemon user, PBEPokemon target)
            => OnNewEvent?.Invoke(this, new PBESpecialMessagePacket(PBESpecialMessage.PainSplit, user.Id, target.Id));
        void BroadcastRecoil(PBEPokemon victim)
            => OnNewEvent?.Invoke(this, new PBESpecialMessagePacket(PBESpecialMessage.Recoil, victim.Id));
        void BroadcastStruggle(PBEPokemon user)
            => OnNewEvent?.Invoke(this, new PBESpecialMessagePacket(PBESpecialMessage.Struggle, user.Id));

        void BroadcastStatus1(PBEPokemon culprit, PBEPokemon victim, PBEStatus1 status1, PBEStatusAction statusAction)
            => OnNewEvent?.Invoke(this, new PBEStatus1Packet(culprit, victim, status1, statusAction));
        void BroadcastStatus2(PBEPokemon culprit, PBEPokemon victim, PBEStatus2 status2, PBEStatusAction statusAction)
            => OnNewEvent?.Invoke(this, new PBEStatus2Packet(culprit, victim, status2, statusAction));
        void BroadcastTeamStatus(PBETeam team, PBETeamStatus teamStatus, PBETeamStatusAction teamStatusAction, PBEPokemon victim = null)
            => OnNewEvent?.Invoke(this, new PBETeamStatusPacket(team, teamStatus, teamStatusAction, victim));
        void BroadcastTransform(PBEPokemon user, PBEPokemon target)
            => OnNewEvent?.Invoke(this, new PBETransformPacket(user, target));
        void BroadcastWeather(PBEWeather weather, PBEWeatherAction weatherAction, PBEPokemon victim = null)
            => OnNewEvent?.Invoke(this, new PBEWeatherPacket(weather, weatherAction, victim));
        void BroadcastActionsRequest(PBETeam team)
            => OnNewEvent?.Invoke(this, new PBEActionsRequestPacket(team));
        void BroadcastSwitchInRequest(PBETeam team)
            => OnNewEvent?.Invoke(this, new PBESwitchInRequestPacket(team));


        /// <summary>
        /// Writes battle events to <see cref="Console.Out"/> in English.
        /// </summary>
        /// <param name="battle">The battle that <paramref name="packet"/> belongs to.</param>
        /// <param name="packet">The battle event packet.</param>
        public static void ConsoleBattleEventHandler(PBEBattle battle, INetPacket packet)
        {
            string NameForTrainer(PBEPokemon pkmn)
            {
                if (pkmn == null)
                {
                    return string.Empty;
                }
                else
                {
                    return $"{pkmn.Team.TrainerName}'s {pkmn.Shell.Nickname}";
                }
            }

            switch (packet)
            {
                case PBEAbilityPacket ap:
                    {
                        PBEPokemon culprit = battle.TryGetPokemon(ap.Culprit),
                            victim = battle.TryGetPokemon(ap.Victim);
                        string nameForCulprit = NameForTrainer(culprit);
                        Console.WriteLine("{0}'s {1} activated!", nameForCulprit, ap.Ability);
                        string message;
                        switch (ap.Ability)
                        {
                            case PBEAbility.Drizzle:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Weather: return; // Message is displayed from a weather packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                            case PBEAbility.Drought:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Weather: return; // Message is displayed from a weather packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                            case PBEAbility.IceBody:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.RestoredHP: return; // Message is displayed from a hp changed packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                            case PBEAbility.Imposter:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.ChangedAppearance: return; // Message is displayed from a status2 packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                            case PBEAbility.Levitate:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Damage: return; // Message is displayed from an effectiveness packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                            case PBEAbility.Limber:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.CuredStatus: return; // Message is displayed from a status1 packet
                                    case PBEAbilityAction.PreventedStatus: return; // Message is displayed from an effectiveness packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                            case PBEAbility.RainDish:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.RestoredHP: return; // Message is displayed from a hp changed packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                            case PBEAbility.SandStream:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Weather: return; // Message is displayed from a weather packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                            case PBEAbility.SnowWarning:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Weather: return; // Message is displayed from a weather packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                            case PBEAbility.SolarPower:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Damage: return; // Message is displayed from a hp changed packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                            case PBEAbility.WonderGuard:
                                switch (ap.AbilityAction)
                                {
                                    case PBEAbilityAction.Damage: return; // Message is displayed from an effectiveness packet
                                    default: throw new ArgumentOutOfRangeException(nameof(ap.AbilityAction), $"Invalid {ap.Ability} action: {ap.AbilityAction}");
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(ap.Ability), $"Invalid ability: {ap.Ability}");
                        }
                        Console.WriteLine(message, nameForCulprit, NameForTrainer(victim));
                        break;
                    }
                case PBEItemPacket ip:
                    {
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
                            case PBEItem.FlameOrb:
                                switch (ip.ItemAction)
                                {
                                    case PBEItemAction.ChangedStatus: message = "{0} was burned by Flame Orb!"; break;
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
                            case PBEItem.LifeOrb:
                                switch (ip.ItemAction)
                                {
                                    case PBEItemAction.CausedDamage: message = "{0} lost some of its HP!"; break;
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
                            case PBEItem.ToxicOrb:
                                switch (ip.ItemAction)
                                {
                                    case PBEItemAction.ChangedStatus: message = "{0} was badly poisoned by the Toxic Orb!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction), $"Invalid {ip.Item} action: {ip.ItemAction}");
                                }
                                break;
                            default: throw new ArgumentOutOfRangeException(nameof(ip.Item), $"Invalid item: {ip.Item}");
                        }
                        Console.WriteLine(message, NameForTrainer(battle.TryGetPokemon(ip.Culprit)), NameForTrainer(battle.TryGetPokemon(ip.Victim)));
                        break;
                    }
                case PBEMoveCritPacket _:
                    {
                        Console.WriteLine("A critical hit!");
                        break;
                    }
                case PBEMoveEffectivenessPacket mep:
                    {
                        string message;
                        switch (mep.Effectiveness)
                        {
                            case PBEEffectiveness.Ineffective: message = "It doesn't affect {0}..."; break;
                            case PBEEffectiveness.NotVeryEffective: message = "It's not very effective..."; break;
                            case PBEEffectiveness.Normal: message = "It's normally effective."; break;
                            case PBEEffectiveness.SuperEffective: message = "It's super effective!"; break;
                            default: throw new ArgumentOutOfRangeException(nameof(mep.Effectiveness), $"Invalid effectiveness: {mep.Effectiveness}");
                        }
                        Console.WriteLine(message, NameForTrainer(battle.TryGetPokemon(mep.Victim)));
                        break;
                    }
                case PBEMoveFailedPacket mfp:
                    {
                        string message;
                        switch (mfp.FailReason)
                        {
                            case PBEFailReason.AlreadyConfused: message = "{1} is already confused!"; break;
                            case PBEFailReason.Default: message = "But it failed!"; break;
                            case PBEFailReason.HPFull: message = "{0}'s HP is full!"; break;
                            case PBEFailReason.NoTarget: message = "There was no target..."; break;
                            default: throw new ArgumentOutOfRangeException(nameof(mfp.FailReason), $"Invalid fail reason: {mfp.FailReason}");
                        }
                        Console.WriteLine(message, NameForTrainer(battle.TryGetPokemon(mfp.Culprit)), NameForTrainer(battle.TryGetPokemon(mfp.Victim)));
                        break;
                    }
                case PBEMoveMissedPacket mmp:
                    {
                        Console.WriteLine("{0}'s attack missed!", NameForTrainer(battle.TryGetPokemon(mmp.Culprit)));
                        break;
                    }
                case PBEMovePPChangedPacket mpcp:
                    {
                        Console.WriteLine("{0}'s {1} {3} {2} PP!", NameForTrainer(battle.TryGetPokemon(mpcp.Victim)), mpcp.Move, Math.Abs(mpcp.Change), mpcp.Change <= 0 ? "lost" : "gained");
                        break;
                    }
                case PBEMoveUsedPacket mup:
                    {
                        Console.WriteLine("{0} used {1}!", NameForTrainer(battle.TryGetPokemon(mup.Culprit)), mup.Move);
                        break;
                    }
                case PBEPkmnFaintedPacket pfap:
                    {
                        Console.WriteLine("{0} fainted!", NameForTrainer(battle.TryGetPokemon(pfap.Victim)));
                        break;
                    }
                case PBEPkmnHPChangedPacket phcp:
                    {
                        PBEPokemon victim = battle.TryGetPokemon(phcp.Victim);
                        int hp = Math.Abs(phcp.Change);
                        Console.WriteLine("{0} {1} {2} ({3:P2}) HP!", NameForTrainer(victim), phcp.Change <= 0 ? "lost" : "gained", hp, (double)hp / victim.MaxHP);
                        break;
                    }
                case PBEPkmnStatChangedPacket pscp:
                    {
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
                        Console.WriteLine("{0}'s {1} {2}!", NameForTrainer(battle.TryGetPokemon(pscp.Victim)), pscp.Stat, message);
                        break;
                    }
                case PBEPkmnSwitchInPacket psip:
                    {
                        if (!psip.Forced)
                        {
                            Console.WriteLine("{1} sent out {0}!", psip.SwitchIns.Select(s => s.Nickname).Andify(), psip.Team.TrainerName);
                        }
                        break;
                    }
                case PBEPkmnSwitchOutPacket psop:
                    {
                        if (!psop.Forced)
                        {
                            PBEPokemon pkmn = battle.TryGetPokemon(psop.Pokemon);
                            Console.WriteLine("{1} withdrew {0}!", pkmn.Shell.Nickname, pkmn.Team.TrainerName);
                        }
                        break;
                    }
                case PBEPsychUpPacket pup:
                    {
                        Console.WriteLine("{0} copied {1}'s stat changes!", NameForTrainer(battle.TryGetPokemon(pup.User)), NameForTrainer(battle.TryGetPokemon(pup.Target)));
                        break;
                    }
                case PBESpecialMessagePacket smp:
                    {
                        switch (smp.Message)
                        {
                            case PBESpecialMessage.DraggedOut:
                                Console.WriteLine("{0} was dragged out!", NameForTrainer(battle.TryGetPokemon((byte)smp.Params[0])));
                                break;
                            case PBESpecialMessage.Magnitude:
                                Console.WriteLine("Magnitude {0}!", (byte)smp.Params[0]);
                                break;
                            case PBESpecialMessage.PainSplit:
                                Console.WriteLine("The battlers shared their pain!");
                                break;
                            case PBESpecialMessage.Recoil:
                                Console.WriteLine("{0} is damaged by recoil!", NameForTrainer(battle.TryGetPokemon((byte)smp.Params[0])));
                                break;
                            case PBESpecialMessage.Struggle:
                                Console.WriteLine("{0} has no moves left!", NameForTrainer(battle.TryGetPokemon((byte)smp.Params[0])));
                                break;
                        }
                        break;
                    }
                case PBEStatus1Packet s1p:
                    {
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
                        Console.WriteLine(message, NameForTrainer(battle.TryGetPokemon(s1p.Victim)));
                        break;
                    }
                case PBEStatus2Packet s2p:
                    {
                        string message;
                        switch (s2p.Status2)
                        {
                            case PBEStatus2.Airborne:
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{0} flew up high!"; break;
                                    case PBEStatusAction.Ended: return;
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
                                    case PBEStatusAction.Added: message = "{1} cut its own HP and laid a curse on {0}!"; break;
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
                                    case PBEStatusAction.Added: return;
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
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{0} put in a substitute!"; break;
                                    case PBEStatusAction.Damage: message = "The substitute took damage for {0}!"; break;
                                    case PBEStatusAction.Ended: message = "{0}'s substitute faded!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.Transformed: // user is victim
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{0} transformed into {1}!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.Underground:
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{0} burrowed its way under the ground!"; break;
                                    case PBEStatusAction.Ended: return;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            case PBEStatus2.Underwater:
                                switch (s2p.StatusAction)
                                {
                                    case PBEStatusAction.Added: message = "{0} hid underwater!"; break;
                                    case PBEStatusAction.Ended: return;
                                    default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid {s2p.Status2} action: {s2p.StatusAction}");
                                }
                                break;
                            default: throw new ArgumentOutOfRangeException(nameof(s2p.Status2), $"Invalid status2: {s2p.Status2}");
                        }
                        Console.WriteLine(message, NameForTrainer(battle.TryGetPokemon(s2p.Victim)), NameForTrainer(battle.TryGetPokemon(s2p.Culprit)));
                        break;
                    }
                case PBETeamStatusPacket tsp:
                    {
                        string message;
                        switch (tsp.TeamStatus)
                        {
                            case PBETeamStatus.LightScreen:
                                switch (tsp.TeamStatusAction)
                                {
                                    case PBETeamStatusAction.Added: message = "Light Screen raised {0}'s team's Special Defense!"; break;
                                    case PBETeamStatusAction.Cleared:
                                    case PBETeamStatusAction.Ended: message = "{0}'s team's Light Screen wore off!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction), $"Invalid {tsp.TeamStatus} action: {tsp.TeamStatusAction}");
                                }
                                break;
                            case PBETeamStatus.LuckyChant:
                                switch (tsp.TeamStatusAction)
                                {
                                    case PBETeamStatusAction.Added: message = "The Lucky Chant shielded {0}'s team from critical hits!"; break;
                                    case PBETeamStatusAction.Ended: message = "{0}'s team's Lucky Chant wore off!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction), $"Invalid {tsp.TeamStatus} action: {tsp.TeamStatusAction}");
                                }
                                break;
                            case PBETeamStatus.Reflect:
                                switch (tsp.TeamStatusAction)
                                {
                                    case PBETeamStatusAction.Added: message = "Reflect raised {0}' team's Defense!"; break;
                                    case PBETeamStatusAction.Cleared:
                                    case PBETeamStatusAction.Ended: message = "{0}'s team's Reflect wore off!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction), $"Invalid {tsp.TeamStatus} action: {tsp.TeamStatusAction}");
                                }
                                break;
                            case PBETeamStatus.Spikes:
                                switch (tsp.TeamStatusAction)
                                {
                                    case PBETeamStatusAction.Added: message = "Spikes were scattered all around the feet of {0}'s team!"; break;
                                    case PBETeamStatusAction.Cleared: message = "The spikes disappeared from around {0}'s team's feet!"; break;
                                    case PBETeamStatusAction.Damage: message = "{1} is hurt by the spikes!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction), $"Invalid {tsp.TeamStatus} action: {tsp.TeamStatusAction}");
                                }
                                break;
                            case PBETeamStatus.StealthRock:
                                switch (tsp.TeamStatusAction)
                                {
                                    case PBETeamStatusAction.Added: message = "Pointed stones float in the air around {0}'s team!"; break;
                                    case PBETeamStatusAction.Cleared: message = "The pointed stones disappeared from around {0}'s team!"; break;
                                    case PBETeamStatusAction.Damage: message = "Pointed stones dug into {1}!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction), $"Invalid {tsp.TeamStatus} action: {tsp.TeamStatusAction}");
                                }
                                break;
                            case PBETeamStatus.ToxicSpikes:
                                switch (tsp.TeamStatusAction)
                                {
                                    case PBETeamStatusAction.Added: message = "Poison spikes were scattered all around {0}'s team's feet!"; break;
                                    case PBETeamStatusAction.Cleared: message = "The poison spikes disappeared from around {0}'s team's feet!"; break;
                                    default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction), $"Invalid {tsp.TeamStatus} action: {tsp.TeamStatusAction}");
                                }
                                break;
                            default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatus), $"Invalid team status: {tsp.TeamStatus}");
                        }
                        Console.WriteLine(message, tsp.Team.TrainerName, NameForTrainer(battle.TryGetPokemon(tsp.Victim)));
                        break;
                    }
                case PBEWeatherPacket wp:
                    {
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
                        Console.WriteLine(message, NameForTrainer(battle.TryGetPokemon(wp.Victim)));
                        break;
                    }
                case PBEActionsRequestPacket arp:
                    {
                        Console.WriteLine("{0} must submit actions for {1} Pokémon.", arp.Team.TrainerName, arp.Pokemon.Count);
                        break;
                    }
                case PBESwitchInRequestPacket sirp:
                    {
                        Console.WriteLine("{0} must send in {1} Pokémon.", sirp.Team.TrainerName, sirp.Amount);
                        break;
                    }
            }
        }
    }
}
