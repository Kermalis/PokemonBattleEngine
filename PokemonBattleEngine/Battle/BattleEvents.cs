using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        public delegate void BattleEvent(PBattle battle, INetPacket packet);
        public event BattleEvent OnNewEvent;

        void BroadcastSwitchIn(PPokemon pkmn)
            => OnNewEvent?.Invoke(this, new PPkmnSwitchInPacket(pkmn));
        void BroadcastSwitchOut(PPokemon pkmn)
            => OnNewEvent?.Invoke(this, new PPkmnSwitchOutPacket(pkmn));
        void BroadcastSwitchInRequest(bool local, byte amount)
            => OnNewEvent?.Invoke(this, new PSwitchInRequestPacket(local, amount));
        void BroadcastActionsRequest(bool local, IEnumerable<PPokemon> pkmn)
            => OnNewEvent?.Invoke(this, new PActionsRequestPacket(local, pkmn));
        void BroadcastMoveUsed(PPokemon culprit, PMove move)
            => OnNewEvent?.Invoke(this, new PMoveUsedPacket(culprit, move));
        void BroadcastMiss(PPokemon culprit)
            => OnNewEvent?.Invoke(this, new PMoveMissedPacket(culprit));
        void BroadcastHPChanged(PPokemon victim, int change)
            => OnNewEvent?.Invoke(this, new PPkmnHPChangedPacket(victim, change));
        void BroadcastEffectiveness(PPokemon victim, PEffectiveness effectiveness)
            => OnNewEvent?.Invoke(this, new PMoveEffectivenessPacket(victim, effectiveness));
        void BroadcastFaint(PPokemon victim)
            => OnNewEvent?.Invoke(this, new PPkmnFaintedPacket(victim));
        void BroadcastCrit()
            => OnNewEvent?.Invoke(this, new PMoveCritPacket());
        void BroadcastFail(PPokemon culprit, PFailReason reason)
            => OnNewEvent?.Invoke(this, new PMoveFailedPacket(culprit, reason));
        void BroadcastStatChange(PPokemon victim, PStat stat, sbyte change, bool isTooMuch)
            => OnNewEvent?.Invoke(this, new PPkmnStatChangedPacket(victim, stat, change, isTooMuch));
        void BroadcastStatus1(PPokemon culprit, PPokemon victim, PStatus1 status, PStatusAction statusAction)
            => OnNewEvent?.Invoke(this, new PStatus1Packet(culprit, victim, status, statusAction));
        void BroadcastStatus2(PPokemon culprit, PPokemon victim, PStatus2 status, PStatusAction statusAction)
            => OnNewEvent?.Invoke(this, new PStatus2Packet(culprit, victim, status, statusAction));
        void BroadcastTeamStatus(bool local, PTeamStatus status, PTeamStatusAction action, byte victimId = 0)
            => OnNewEvent?.Invoke(this, new PTeamStatusPacket(local, status, action, victimId));
        void BroadcastWeather(PWeather weather, PWeatherAction action)
            => OnNewEvent?.Invoke(this, new PWeatherPacket(weather, action));
        void BroadcastItemUsed(PPokemon culprit, PItem item)
            => OnNewEvent?.Invoke(this, new PItemUsedPacket(culprit, item));
        void BroadcastPPChanged(PPokemon victim, PMove move, int change)
            => OnNewEvent?.Invoke(this, new PMovePPChangedPacket(victim, move, change));
        void BroadcastLimber(PPokemon pkmn, bool prevented) // Prevented or cured
            => OnNewEvent?.Invoke(this, new PLimberPacket(pkmn, prevented));
        void BroadcastTransform(PPokemon culprit, PPokemon victim)
            => OnNewEvent?.Invoke(this, new PTransformPacket(culprit, victim));
        void BroadcastMagnitude(byte magnitude)
            => OnNewEvent?.Invoke(this, new PMagnitudePacket(magnitude));
        void BroadcastPainSplit()
            => OnNewEvent?.Invoke(this, new PPainSplitPacket());
        void BroadcastPsychUp(PPokemon culprit, PPokemon victim)
            => OnNewEvent?.Invoke(this, new PPsychUpPacket(culprit, victim));



        public static void ConsoleBattleEventHandler(PBattle battle, INetPacket packet)
        {
            PPokemon culprit, victim;
            string message;
            double d;
            bool b1, b2;

            switch (packet)
            {
                case PItemUsedPacket iup:
                    culprit = battle.GetPokemon(iup.CulpritId);
                    switch (iup.Item)
                    {
                        case PItem.Leftovers: message = "restored a little HP using its Leftovers"; break;
                        case PItem.PowerHerb: message = "became fully charged due to its Power Herb"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(iup.Item), $"Invalid item used: {iup.Item}");
                    }
                    Console.WriteLine("{0} {1}!", culprit.NameForTrainer(true), message);
                    break;
                case PLimberPacket lp:
                    victim = battle.GetPokemon(lp.PokemonId);
                    Console.Write("{0}'s Limber: ", victim.Shell.Nickname);
                    break;
                case PMagnitudePacket mp:
                    Console.WriteLine("Magnitude {0}!", mp.Magnitude);
                    break;
                case PMoveCritPacket _:
                    Console.WriteLine("A critical hit!");
                    break;
                case PMoveEffectivenessPacket mep:
                    victim = battle.GetPokemon(mep.VictimId);
                    switch (mep.Effectiveness)
                    {
                        case PEffectiveness.Ineffective: message = "It doesn't affect {0}..."; break;
                        case PEffectiveness.NotVeryEffective: message = "It's not very effective..."; break;
                        case PEffectiveness.Normal: return;
                        case PEffectiveness.SuperEffective: message = "It's super effective!"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mep.Effectiveness), $"Invalid effectiveness: {mep.Effectiveness}");
                    }
                    Console.WriteLine(message, victim.NameForTrainer(false));
                    break;
                case PMoveFailedPacket mfp:
                    culprit = battle.GetPokemon(mfp.CulpritId);
                    switch (mfp.Reason)
                    {
                        case PFailReason.Default: message = "But it failed!"; break;
                        case PFailReason.HPFull: message = "{0}'s HP is full!"; break;
                        case PFailReason.NoTarget: message = "There was no target..."; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mfp.Reason), $"Invalid fail reason: {mfp.Reason}");
                    }
                    Console.WriteLine(message, culprit.NameForTrainer(true));
                    break;
                case PMoveMissedPacket mmp:
                    culprit = battle.GetPokemon(mmp.CulpritId);
                    Console.WriteLine("{0}'s attack missed!", culprit.NameForTrainer(true));
                    break;
                case PMovePPChangedPacket mpcp:
                    victim = battle.GetPokemon(mpcp.VictimId);
                    Console.WriteLine("{0}'s {1} {3} {2} PP!", victim.NameForTrainer(true), mpcp.Move, Math.Abs(mpcp.Change), mpcp.Change <= 0 ? "lost" : "gained");
                    break;
                case PMoveUsedPacket mup:
                    culprit = battle.GetPokemon(mup.CulpritId);
                    Console.WriteLine("{0} used {1}!", culprit.NameForTrainer(true), mup.Move);
                    break;
                case PPainSplitPacket psp:
                    Console.WriteLine("The battlers shared their pain!");
                    break;
                case PPkmnFaintedPacket pfap:
                    victim = battle.GetPokemon(pfap.VictimId);
                    Console.WriteLine("{0} fainted!", victim.NameForTrainer(true));
                    break;
                case PPkmnHPChangedPacket phcp:
                    victim = battle.GetPokemon(phcp.VictimId);
                    var hp = Math.Abs(phcp.Change);
                    d = (double)hp / victim.MaxHP;
                    Console.WriteLine("{0} {3} {1} ({2:P2}) HP!", victim.NameForTrainer(true), hp, d, phcp.Change <= 0 ? "lost" : "gained");
                    break;
                case PPkmnStatChangedPacket pscp:
                    victim = battle.GetPokemon(pscp.VictimId);
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
                    Console.WriteLine("{0}'s {1} {2}!", victim.NameForTrainer(true), pscp.Stat, message);
                    break;
                case PPkmnSwitchInPacket psip:
                    culprit = battle.GetPokemon(psip.PokemonId);
                    Console.WriteLine("{1} sent out {0}!", culprit.Shell.Nickname, battle.Teams[culprit.Local ? 0 : 1].TrainerName);
                    break;
                case PPkmnSwitchOutPacket psop:
                    culprit = battle.GetPokemon(psop.PokemonId);
                    Console.WriteLine("{1} withdrew {0}!", culprit.Shell.Nickname, battle.Teams[culprit.Local ? 0 : 1].TrainerName);
                    break;
                case PPsychUpPacket pup:
                    culprit = battle.GetPokemon(pup.CulpritId);
                    victim = battle.GetPokemon(pup.VictimId);
                    Console.WriteLine("{0} copied {1}'s stat changes!", culprit.Shell.Nickname, victim.Shell.Nickname);
                    break;
                case PStatus1Packet s1p:
                    culprit = battle.GetPokemon(s1p.CulpritId);
                    victim = battle.GetPokemon(s1p.VictimId);
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
                    Console.WriteLine(message, victim.NameForTrainer(true));
                    break;
                case PStatus2Packet s2p:
                    b1 = true; // victim caps
                    b2 = false; // culprit caps
                    culprit = battle.GetPokemon(s2p.CulpritId);
                    victim = battle.GetPokemon(s2p.VictimId);
                    switch (s2p.Status)
                    {
                        case PStatus2.Airborne:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: message = "{0} flew up high!"; break;
                                case PStatusAction.Ended: return;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid {s2p.Status} action: {s2p.Action}");
                            }
                            break;
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
                        case PStatus2.Cursed:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: message = "{1} cut its own HP and laid a curse on {0}!"; b2 = true; b1 = false; break;
                                case PStatusAction.Damage: message = "{0} is afflicted by the curse!"; break;
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
                                case PStatusAction.Added: return;
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
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: message = "{0} put in a substitute!"; break;
                                case PStatusAction.Damage: message = "The substitute took damage for {0}!"; b1 = false; break;
                                case PStatusAction.Ended: message = "{0}'s substitute faded!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid {s2p.Status} action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.Underground:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: message = "{0} burrowed its way under the ground!"; break;
                                case PStatusAction.Ended: return;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid {s2p.Status} action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.Underwater:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: message = "{0} hid underwater!"; break;
                                case PStatusAction.Ended: return;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid {s2p.Status} action: {s2p.Action}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(s2p.Status), $"Invalid status2: {s2p.Status}");
                    }
                    Console.WriteLine(message, victim.NameForTrainer(b1), culprit.NameForTrainer(b2));
                    break;
                case PTeamStatusPacket tsp:
                    b1 = false;
                    victim = battle.GetPokemon(tsp.VictimId);
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
                                case PTeamStatusAction.Added: message = "Spikes were scattered all around the feet of {2} team!"; break;
                                case PTeamStatusAction.Cleared: message = "The spikes disappeared from around {2} team's feet!"; break;
                                case PTeamStatusAction.Damage: message = "{4} is hurt by the spikes!"; b1 = true; break;
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
                                case PTeamStatusAction.Added: message = "Poison spikes were scattered all around {2} team's feet!"; break;
                                case PTeamStatusAction.Cleared: message = "The poison spikes disappeared from around {2} team's feet!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.Action), $"Invalid {tsp.Status} action: {tsp.Action}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(tsp.Status), $"Invalid team status: {tsp.Status}");
                    }
                    Console.WriteLine(message,
                        tsp.Local ? "your" : "the opposing",
                        tsp.Local ? "Your" : "The opposing",
                        tsp.Local ? "your" : "the foe's",
                        tsp.Local ? "your" : "your foe's",
                        victim.NameForTrainer(b1)
                        );
                    break;
                case PTransformPacket tp:
                    culprit = battle.GetPokemon(tp.CulpritId);
                    victim = battle.GetPokemon(tp.VictimId);
                    Console.WriteLine("{0} transformed into {1}!", culprit.NameForTrainer(true), victim.NameForTrainer(false));
                    break;
                case PWeatherPacket wp:
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
                        // It started to hail!
                        default: throw new ArgumentOutOfRangeException(nameof(wp.Weather), $"Invalid weather: {wp.Weather}");
                    }
                    Console.WriteLine(message);
                    break;
                case PActionsRequestPacket arp:
                    Console.WriteLine("{0} must submit actions for {1} Pokémon.", battle.Teams[arp.Local ? 0 : 1].TrainerName, arp.PokemonIDs.Length);
                    break;
                case PSwitchInRequestPacket sirp:
                    Console.WriteLine("{0} must send in {1} Pokémon.", battle.Teams[sirp.Local ? 0 : 1].TrainerName, sirp.Amount);
                    break;
            }
        }
    }
}
