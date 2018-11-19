using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;

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
        void BroadcastMoveUsed()
            => OnNewEvent?.Invoke(this, new PMoveUsedPacket(bUser, bMove));
        void BroadcastMiss()
            => OnNewEvent?.Invoke(this, new PMoveMissedPacket(bUser));
        void BroadcastHPChanged(PPokemon pkmn, int change)
            => OnNewEvent?.Invoke(this, new PPkmnHPChangedPacket(pkmn, change));
        void BroadcastEffectiveness(PPokemon pkmn, PEffectiveness effectiveness)
            => OnNewEvent?.Invoke(this, new PMoveEffectivenessPacket(pkmn, effectiveness));
        void BroadcastFaint(PPokemon pkmn)
            => OnNewEvent?.Invoke(this, new PPkmnFaintedPacket(pkmn));
        void BroadcastCrit()
            => OnNewEvent?.Invoke(this, new PMoveCritPacket());
        void BroadcastFail(PFailReason reason)
            => OnNewEvent?.Invoke(this, new PMoveFailedPacket(bUser, reason));
        void BroadcastStatChange(PPokemon pkmn, PStat stat, sbyte change, bool isTooMuch)
            => OnNewEvent?.Invoke(this, new PPkmnStatChangedPacket(pkmn, stat, change, isTooMuch));
        void BroadcastStatus1(PPokemon pkmn, PStatus1 status, PStatusAction statusAction)
            => OnNewEvent?.Invoke(this, new PStatus1Packet(pkmn, status, statusAction));
        void BroadcastStatus2(PPokemon pkmn, PStatus2 status, PStatusAction statusAction)
            => OnNewEvent?.Invoke(this, new PStatus2Packet(pkmn, status, statusAction));
        void BroadcastTeamStatus(bool local, PTeamStatus status, PTeamStatusAction action, byte victimId = 0)
            => OnNewEvent?.Invoke(this, new PTeamStatusPacket(local, status, action, victimId));
        void BroadcastWeather(PWeather weather, PWeatherAction action)
            => OnNewEvent?.Invoke(this, new PWeatherPacket(weather, action));
        void BroadcastItemUsed(PPokemon pkmn, PItem item)
            => OnNewEvent?.Invoke(this, new PItemUsedPacket(pkmn, item));
        void BroadcastPPChanged(PPokemon pkmn, PMove move, int change)
            => OnNewEvent?.Invoke(this, new PMovePPChangedPacket(pkmn, move, change));
        void BroadcastLimber(PPokemon pkmn, bool prevented) // Prevented or cured
            => OnNewEvent?.Invoke(this, new PLimberPacket(pkmn, prevented));
        void BroadcastTransform()
            => OnNewEvent?.Invoke(this, new PTransformPacket(bUser, bTarget));



        public static void ConsoleBattleEventHandler(PBattle battle, INetPacket packet)
        {
            PPokemon pkmn;
            string message;
            double d;
            bool b;

            switch (packet)
            {
                case PItemUsedPacket iup:
                    switch (iup.Item)
                    {
                        case PItem.Leftovers: message = "restored a little HP using its Leftovers"; break;
                        case PItem.PowerHerb: message = "became fully charged due to its Power Herb"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(iup.Item), $"Invalid item used: {iup.Item}");
                    }
                    Console.WriteLine("{0} {1}!", battle.GetPokemon(iup.PokemonId).NameForTrainer(true), message);
                    break;
                case PLimberPacket lp:
                    Console.Write("{0}'s Limber: ", battle.GetPokemon(lp.PokemonId).Shell.Nickname);
                    break;
                case PMoveCritPacket _:
                    Console.WriteLine("A critical hit!");
                    break;
                case PMoveEffectivenessPacket mep:
                    switch (mep.Effectiveness)
                    {
                        case PEffectiveness.Ineffective: message = "It doesn't affect {0}..."; break;
                        case PEffectiveness.NotVeryEffective: message = "It's not very effective..."; break;
                        case PEffectiveness.Normal: return;
                        case PEffectiveness.SuperEffective: message = "It's super effective!"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mep.Effectiveness), $"Invalid effectiveness: {mep.Effectiveness}");
                    }
                    Console.WriteLine(message, battle.GetPokemon(mep.PokemonId).NameForTrainer(false));
                    break;
                case PMoveFailedPacket mfp:
                    switch (mfp.Reason)
                    {
                        case PFailReason.Default: message = "But it failed!"; break;
                        case PFailReason.HPFull: message = "{0}'s HP is full!"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mfp.Reason), $"Invalid fail reason: {mfp.Reason}");
                    }
                    Console.WriteLine(message, battle.GetPokemon(mfp.PokemonId).NameForTrainer(true));
                    break;
                case PMoveMissedPacket mmp:
                    Console.WriteLine("{0}'s attack missed!", battle.GetPokemon(mmp.PokemonId).NameForTrainer(true));
                    break;
                case PMoveUsedPacket mup:
                    Console.WriteLine("{0} used {1}!", battle.GetPokemon(mup.PokemonId).NameForTrainer(true), mup.Move);
                    break;
                case PPkmnFaintedPacket pfap:
                    Console.WriteLine("{0} fainted!", battle.GetPokemon(pfap.PokemonId).NameForTrainer(true));
                    break;
                case PPkmnHPChangedPacket phcp:
                    pkmn = battle.GetPokemon(phcp.PokemonId);
                    var hp = Math.Abs(phcp.Change);
                    d = (double)hp / pkmn.MaxHP;
                    Console.WriteLine("{0} {3} {1} ({2:P2}) HP!", pkmn.NameForTrainer(true), hp, d, phcp.Change <= 0 ? "lost" : "gained");
                    break;
                case PPkmnStatChangedPacket pscp:
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
                    Console.WriteLine("{0}'s {1} {2}!", battle.GetPokemon(pscp.PokemonId).NameForTrainer(true), pscp.Stat, message);
                    break;
                case PPkmnSwitchInPacket psip:
                    Console.WriteLine("{1} sent out {0}!", battle.GetPokemon(psip.PokemonId).Shell.Nickname, battle.Teams[psip.Local ? 0 : 1].TrainerName);
                    break;
                case PPkmnSwitchOutPacket psop:
                    pkmn = battle.GetPokemon(psop.PokemonId);
                    Console.WriteLine("{1} withdrew {0}!", pkmn.Shell.Nickname, battle.Teams[pkmn.Local ? 0 : 1].TrainerName);
                    break;
                case PStatus1Packet s1p:
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
                    Console.WriteLine(message, battle.GetPokemon(s1p.PokemonId).NameForTrainer(true));
                    break;
                case PStatus2Packet s2p:
                    b = true;
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
                        case PStatus2.Minimized:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: return;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid minimized action: {s2p.Action}");
                            }
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
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: message = "{0} put in a substitute!"; break;
                                case PStatusAction.Damage: message = "The substitute took damage for {0}!"; b = false; break;
                                case PStatusAction.Ended: message = "{0}'s substitute faded!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid substitute action: {s2p.Action}");
                            }
                            break;
                        case PStatus2.Underwater:
                            switch (s2p.Action)
                            {
                                case PStatusAction.Added: message = "{0} hid underwater!"; break;
                                case PStatusAction.Ended: return;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.Action), $"Invalid underwater action: {s2p.Action}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(s2p.Status2), $"Invalid status2: {s2p.Status2}");
                    }
                    Console.WriteLine(message, battle.GetPokemon(s2p.PokemonId).NameForTrainer(b));
                    break;
                case PTeamStatusPacket tsp:
                    switch (tsp.Status)
                    {
                        case PTeamStatus.LightScreen:
                            switch (tsp.Action)
                            {
                                case PTeamStatusAction.Added: message = "Light Screen raised {0} team's Special Defense!"; break;
                                case PTeamStatusAction.Cleared:
                                case PTeamStatusAction.Ended: message = "{1} team's Light Screen wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.Action), $"Invalid light screen action: {tsp.Action}");
                            }
                            break;
                        case PTeamStatus.Reflect:
                            switch (tsp.Action)
                            {
                                case PTeamStatusAction.Added: message = "Reflect raised {0} team's Defense!"; break;
                                case PTeamStatusAction.Cleared:
                                case PTeamStatusAction.Ended: message = "{1} team's Reflect wore off!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.Action), $"Invalid reflect action: {tsp.Action}");
                            }
                            break;
                        case PTeamStatus.Spikes:
                            switch (tsp.Action)
                            {
                                case PTeamStatusAction.Added: message = "Spikes were scattered all around the feet of {2} team!"; break;
                                case PTeamStatusAction.Cleared: message = "The spikes disappeared from around {2} team's feet!"; break;
                                case PTeamStatusAction.Damage: message = "{3} is hurt by the spikes!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(tsp.Action), $"Invalid spikes action: {tsp.Action}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(tsp.Status), $"Invalid team status: {tsp.Status}");
                    }
                    Console.WriteLine(message,
                        tsp.Local ? "your" : "the opposing",
                        tsp.Local ? "Your" : "The opposing",
                        tsp.Local ? "your" : "the foe's",
                        battle.GetPokemon(tsp.VictimId).NameForTrainer(true)
                        );
                    break;
                case PTransformPacket tp:
                    Console.WriteLine("{0} transformed into {1}!", battle.GetPokemon(tp.UserId).NameForTrainer(true), battle.GetPokemon(tp.TargetId).NameForTrainer(false));
                    break;
                case PWeatherPacket wp:
                    switch (wp.Weather)
                    {
                        case PWeather.Raining:
                            switch (wp.Action)
                            {
                                case PWeatherAction.Added: message = "It started to rain!"; break;
                                case PWeatherAction.Ended: message = "The rain stopped."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.Action), $"Invalid raining action: {wp.Action}");
                            }
                            break;
                        case PWeather.Sunny:
                            switch (wp.Action)
                            {
                                case PWeatherAction.Added: message = "The sunlight turned harsh!"; break;
                                case PWeatherAction.Ended: message = "The sunlight faded."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(wp.Action), $"Invalid sunny action: {wp.Action}");
                            }
                            break;
                        // It started to hail!
                        default: throw new ArgumentOutOfRangeException(nameof(wp.Weather), $"Invalid weather: {wp.Weather}");
                    }
                    Console.WriteLine(message);
                    break;
            }
        }
    }
}
