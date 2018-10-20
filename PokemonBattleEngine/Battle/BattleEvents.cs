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
            => OnNewEvent?.Invoke(new PPkmnMovePacket(bAttacker.Mon, bMove));
        void BroadcastMiss()
            => OnNewEvent?.Invoke(new PMoveMissedPacket(bAttacker.Mon));
        void BroadcastFlinch()
            => OnNewEvent?.Invoke(new PPkmnFlinchedPacket(bAttacker.Mon));
        void BroadcastDamage(PPokemon pkmn, ushort amt)
            => OnNewEvent?.Invoke(new PPkmnDamagedPacket(pkmn, amt));
        void BroadcastEffectiveness()
            => OnNewEvent?.Invoke(new PMoveEffectivenessPacket(bDefender.Mon, bEffectiveness));
        void BroadcastFaint(PPokemon pkmn)
            => OnNewEvent?.Invoke(new PPkmnFaintedPacket(pkmn));
        void BroadcastCrit()
        {
            if (bLandedCrit)
                OnNewEvent?.Invoke(new PMoveCritPacket());
        }
        void BroadcastStatChange(PPokemon pkmn, PStat stat, sbyte change, bool isTooMuch)
            => OnNewEvent?.Invoke(new PPkmnStatChangePacket(pkmn, stat, change, isTooMuch));
        void BroadcastStatusChange(PPokemon pkmn)
            => OnNewEvent?.Invoke(new PStatusChangePacket(pkmn));
        void BroadcastStatusEnded(PPokemon pkmn)
            => OnNewEvent?.Invoke(new PStatusEndedPacket(pkmn));
        void PrintStatusCausedImmobility(PPokemon pkmn)
            => OnNewEvent?.Invoke(new PStatusCausedImmobilityPacket(pkmn));



        public static void ConsoleBattleEventHandler(INetPacket packet)
        {
            PPokemon pkmn;
            string message;

            switch (packet)
            {
                case PPkmnSwitchInPacket psip:
                    Console.WriteLine("{1} sent out {0}!", PKnownInfo.Instance.Pokemon(psip.PokemonId).Shell.Species, PKnownInfo.Instance.DisplayName(psip.LocallyOwned));
                    break;
                case PPkmnMovePacket pmp:
                    Console.WriteLine("{0} used {1}!", PKnownInfo.Instance.Pokemon(pmp.PokemonId).Shell.Species, pmp.Move);
                    break;
                case PPkmnDamagedPacket pdp:
                    pkmn = PKnownInfo.Instance.Pokemon(pdp.PokemonId);
                    double percentage = (double)pdp.Damage / pkmn.MaxHP;
                    Console.WriteLine("{0} took {1} ({2:P2}) damage!", pkmn.Shell.Species, pdp.Damage, percentage);
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
                    Console.WriteLine(message, PKnownInfo.Instance.Pokemon(mep.PokemonId).Shell.Species);
                    break;
                case PPkmnFlinchedPacket pflp:
                    Console.WriteLine("{0} flinched!", PKnownInfo.Instance.Pokemon(pflp.PokemonId).Shell.Species);
                    break;
                case PMoveMissedPacket mmp:
                    Console.WriteLine("{0}'s attack missed!", PKnownInfo.Instance.Pokemon(mmp.PokemonId).Shell.Species);
                    break;
                case PPkmnFaintedPacket pfap:
                    Console.WriteLine("{0} fainted!", PKnownInfo.Instance.Pokemon(pfap.PokemonId).Shell.Species);
                    break;
                case PMoveCritPacket mcp:
                    Console.WriteLine("A critical hit!");
                    break;
                case PPkmnStatChangePacket pscp:
                    switch (pscp.Change)
                    {
                        case -2: message = "harshly fell"; break;
                        case -1: message = "fell"; break;
                        case +1: message = "rose"; break;
                        case +2: message = "sharply rose"; break;
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
                    Console.WriteLine("{0}'s {1} {2}!", PKnownInfo.Instance.Pokemon(pscp.PokemonId).Shell.Species, pscp.Stat, message);
                    break;
                case PStatusChangePacket scp:
                    switch (scp.Status)
                    {
                        case PStatus.Frozen: message = "was frozen solid"; break;
                        case PStatus.Paralyzed: message = "is paralyzed! It may be unable to move"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(scp.Status), $"Invalid status change: {scp.Status}");
                    }
                    Console.WriteLine("{0} {1}!", PKnownInfo.Instance.Pokemon(scp.PokemonId).Shell.Species, message);
                    break;
                case PStatusEndedPacket sep:
                    switch (sep.Status)
                    {
                        case PStatus.Frozen: message = "thawed out"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(sep.Status), $"Invalid status ending: {sep.Status}");
                    }
                    Console.WriteLine("{0} {1}!", PKnownInfo.Instance.Pokemon(sep.PokemonId).Shell.Species, message);
                    break;
                case PStatusCausedImmobilityPacket scip:
                    switch (scip.Status)
                    {
                        case PStatus.Frozen: message = "is frozen solid"; break;
                        case PStatus.Paralyzed: message = "is paralyzed! It can't move"; break;
                        default: throw new ArgumentOutOfRangeException(nameof(scip.Status), $"Invalid status causing immobility: {scip.Status}");
                    }
                    Console.WriteLine("{0} {1}!", PKnownInfo.Instance.Pokemon(scip.PokemonId).Shell.Species, message);
                    break;
            }
        }
    }
}
