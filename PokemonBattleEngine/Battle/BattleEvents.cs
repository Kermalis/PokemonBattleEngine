using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        public delegate void BattleEvent(INetPacket packet);
        public event BattleEvent OnNewEvent;

        void BroadcastSwitchIn(PPokemon pkmn)
            => OnNewEvent?.Invoke(new PPkmnSwitchInPacket(pkmn));
        void BroadcastMoveUsed()
            => OnNewEvent?.Invoke(new PPkmnMovePacket(bAttacker, bMove));
        void BroadcastMiss()
            => OnNewEvent?.Invoke(new PMoveMissedPacket(bAttacker));
        void BroadcastHPChanged(PPokemon pkmn, int change)
            => OnNewEvent?.Invoke(new PPkmnHPChangedPacket(pkmn, change));
        void BroadcastEffectiveness(double effectiveness)
            => OnNewEvent?.Invoke(new PMoveEffectivenessPacket(bDefender, effectiveness));
        void BroadcastFaint(PPokemon pkmn)
            => OnNewEvent?.Invoke(new PPkmnFaintedPacket(pkmn));
        void BroadcastCrit()
        {
            if (bLandedCrit)
                OnNewEvent?.Invoke(new PMoveCritPacket());
        }

        void BroadcastStatChange(PPokemon pkmn, PStat stat, sbyte change, bool isTooMuch)
            => OnNewEvent?.Invoke(new PPkmnStatChangedPacket(pkmn, stat, change, isTooMuch));
        void BroadcastStatus1(PPokemon pkmn, PStatus1 status, PStatusAction statusAction)
            => OnNewEvent?.Invoke(new PStatus1Packet(pkmn, status, statusAction));
        void BroadcastStatus2(PPokemon pkmn, PStatus2 status, PStatusAction statusAction)
            => OnNewEvent?.Invoke(new PStatus2Packet(pkmn, status, statusAction));
        void BroadcastFail()
            => OnNewEvent?.Invoke(new PMoveFailPacket());
        void BroadcastItemUsed(PPokemon pkmn)
            => OnNewEvent?.Invoke(new PItemUsedPacket(pkmn));
        void BroadcastPPChanged(PPokemon pkmn, PMove move, int change)
            => OnNewEvent?.Invoke(new PMovePPChangedPacket(pkmn, move, change));
        void BroadcastLimber(PPokemon pkmn, bool prevented) // Prevented or cured
            => OnNewEvent?.Invoke(new PAbilityLimberPacket(pkmn, prevented));



        public static void ConsoleBattleEventHandler(INetPacket packet)
        {
            PPokemon pkmn;
            string message;
            double percentage;

            switch (packet)
            {
                case PPkmnSwitchInPacket psip:
                    Console.WriteLine("{1} sent out {0}!", PKnownInfo.Instance.Pokemon(psip.PokemonId).Shell.Nickname, PKnownInfo.Instance.DisplayName(psip.Local));
                    break;
                case PPkmnMovePacket pmp:
                    Console.WriteLine("{0} used {1}!", PKnownInfo.Instance.Pokemon(pmp.PokemonId).Shell.Nickname, pmp.Move);
                    break;
                case PPkmnHPChangedPacket phcp:
                    pkmn = PKnownInfo.Instance.Pokemon(phcp.PokemonId);
                    var hp = Math.Abs(phcp.Change);
                    percentage = (double)hp / pkmn.MaxHP;
                    Console.WriteLine("{0} {3} {1} ({2:P2}) HP!", pkmn.Shell.Nickname, hp, percentage, phcp.Change < 0 ? "lost" : "gained");
                    break;
                case PMoveEffectivenessPacket mep:
                    if (mep.Effectiveness == 0)
                        message = "It doesn't affect {0}...";
                    else if (mep.Effectiveness > 1)
                        message = "It's super effective!";
                    else if (mep.Effectiveness < 1)
                        message = "It's not very effective...";
                    else
                        break;
                    Console.WriteLine(message, PKnownInfo.Instance.Pokemon(mep.PokemonId).Shell.Nickname);
                    break;
                case PMoveMissedPacket mmp:
                    Console.WriteLine("{0}'s attack missed!", PKnownInfo.Instance.Pokemon(mmp.PokemonId).Shell.Nickname);
                    break;
                case PPkmnFaintedPacket pfap:
                    Console.WriteLine("{0} fainted!", PKnownInfo.Instance.Pokemon(pfap.PokemonId).Shell.Nickname);
                    break;
                case PMoveCritPacket _:
                    Console.WriteLine("A critical hit!");
                    break;
                case PMoveFailPacket _:
                    Console.WriteLine("But it failed!");
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
                    Console.WriteLine("{0}'s {1} {2}!", PKnownInfo.Instance.Pokemon(pscp.PokemonId).Shell.Nickname, pscp.Stat, message);
                    break;
                case PStatus1Packet s1p:
                    switch (s1p.Status1)
                    {
                        case PStatus1.Asleep:
                            switch (s1p.StatusAction)
                            {
                                case PStatusAction.Activated: message = "{0} is fast asleep."; break;
                                case PStatusAction.Added: message = "{0} fell asleep!"; break;
                                case PStatusAction.Ended: message = "{0} woke up!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction), $"Invalid asleep action: {s1p.StatusAction}");
                            }
                            break;
                        case PStatus1.BadlyPoisoned:
                        case PStatus1.Poisoned:
                            switch (s1p.StatusAction)
                            {
                                case PStatusAction.Added: message = "{0} was poisoned!"; break;
                                case PStatusAction.CausedDamage: message = "{0} was hurt by poison!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction), $"Invalid poisoned action: {s1p.StatusAction}");
                            }
                            break;
                        case PStatus1.Burned:
                            switch (s1p.StatusAction)
                            {
                                case PStatusAction.Added: message = "{0} was burned!"; break;
                                case PStatusAction.CausedDamage: message = "{0} was hurt by its burn!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction), $"Invalid burned action: {s1p.StatusAction}");
                            }
                            break;
                        case PStatus1.Frozen:
                            switch (s1p.StatusAction)
                            {
                                case PStatusAction.Activated: message = "{0} is frozen solid!"; break;
                                case PStatusAction.Added: message = "{0} was frozen solid!"; break;
                                case PStatusAction.Ended: message = "{0} thawed out!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction), $"Invalid frozen action: {s1p.StatusAction}");
                            }
                            break;
                        case PStatus1.Paralyzed:
                            switch (s1p.StatusAction)
                            {
                                case PStatusAction.Activated: message = "{0} is paralyzed! It can't move!"; break;
                                case PStatusAction.Added: message = "{0} is paralyzed! It may be unable to move!"; break;
                                case PStatusAction.Cured: message = "{0} was cured of paralysis."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction), $"Invalid paralyzed action: {s1p.StatusAction}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(s1p.Status1), $"Invalid status1: {s1p.Status1}");
                    }
                    Console.WriteLine(message, PKnownInfo.Instance.Pokemon(s1p.PokemonId).Shell.Nickname);
                    break;
                case PStatus2Packet s2p:
                    switch (s2p.Status2)
                    {
                        case PStatus2.Confused:
                            switch (s2p.StatusAction)
                            {
                                case PStatusAction.Activated: message = "{0} is confused!"; break;
                                case PStatusAction.Added: message = "{0} became confused!"; break;
                                case PStatusAction.CausedDamage: message = "It hurt itself in its confusion!"; break;
                                case PStatusAction.Ended: message = "{0} snapped out of its confusion."; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid confused action: {s2p.StatusAction}");
                            }
                            break;
                        case PStatus2.Flinching:
                            switch (s2p.StatusAction)
                            {
                                case PStatusAction.Activated: message = "{0} flinched and couldn't move!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid flinching action: {s2p.StatusAction}");
                            }
                            break;
                        case PStatus2.Protected:
                            switch (s2p.StatusAction)
                            {
                                case PStatusAction.Activated:
                                case PStatusAction.Added: message = "{0} protected itself!"; break;
                                default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction), $"Invalid protected action: {s2p.StatusAction}");
                            }
                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(s2p.Status2), $"Invalid status2: {s2p.Status2}");
                    }
                    Console.WriteLine(message, PKnownInfo.Instance.Pokemon(s2p.PokemonId).Shell.Nickname);
                    break;
                case PItemUsedPacket iup:
                    switch (iup.Item)
                    {
                        case PItem.Leftovers: message = "restored a little HP using its Leftovers"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(iup.Item), $"Invalid item used: {iup.Item}");
                    }
                    Console.WriteLine("{0} {1}!", PKnownInfo.Instance.Pokemon(iup.PokemonId).Shell.Nickname, message);
                    break;
                case PAbilityLimberPacket alp:
                    Console.Write("{0}'s Limber: ", PKnownInfo.Instance.Pokemon(alp.PokemonId).Shell.Nickname);
                    break;
            }
        }
    }
}
