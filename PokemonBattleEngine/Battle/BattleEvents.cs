using Ether.Network.Packets;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Network;
using System;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        public delegate void BattleEvent(INetPacketStream packet);
        public event BattleEvent NewEvent;

        void PrintSwitchIn(PPokemon pkmn)
        {
            NewEvent?.Invoke(new PSwitchInPacket(pkmn));
        }
        void PrintMoveUsed()
        {
            var pkmn = efCurAttacker.Mon;
            NewEvent?.Invoke(new PUsedMovePacket(pkmn.Id, efCurMove, pkmn.Shell.Moves.Contains(efCurMove)));
        }
        void PrintMiss()
        {
            Console.WriteLine("{0}'s attack missed!", efCurAttacker.Mon.Shell.Species);
        }
        void PrintFlinch()
        {
            Console.WriteLine("{0} flinched!", efCurAttacker.Mon.Shell.Species);
        }
        void PrintDamage(PPokemon pkmn, int amt)
        {
            double percentage = (double)amt / efCurDefender.Mon.MaxHP;
            Console.WriteLine("{0} took {1} ({2:P2}) damage!", pkmn.Shell.Species, amt, percentage);
        }
        void PrintEffectiveness()
        {
            string message;
            if (efEffectiveness == 0)
                message = "It doesn't affect {0}...";
            else if (efEffectiveness > 1)
                message = "It's super effective!";
            else if (efEffectiveness < 1)
                message = "It's not very effective...";
            else
                return;

            Console.WriteLine(message, efCurDefender.Mon.Shell.Species);
        }
        void PrintFaint(PPokemon pkmn)
        {
            Console.WriteLine("{0} fainted!", pkmn.Shell.Species);
        }
        void PrintCrit()
        {
            if (efLandedCrit)
                Console.WriteLine("A critical hit!");
        }
        void PrintStatChange(PPokemon pkmn, PStat stat, sbyte change, bool tooMuch)
        {
            string description;
            switch (change)
            {
                case -2: description = "harshly fell"; break;
                case -1: description = "fell"; break;
                case +1: description = "rose"; break;
                case +2: description = "sharply rose"; break;
                default:
                    if (tooMuch && change < 0)
                        description = "won't go lower";
                    else if (tooMuch && change > 0)
                        description = "won't go higher";
                    else if (change <= -3)
                        description = "severely fell";
                    else if (change >= 3)
                        description = "rose drastically";
                    else
                        throw new ArgumentOutOfRangeException(nameof(change), $"Invalid stat change: {change}"); // +0
                    break;
            }
            Console.WriteLine("{0}'s {1} {2}!", pkmn.Shell.Species, stat, description);
        }
        void PrintStatusChange(PPokemon pkmn, PStatus status)
        {
            string description;
            switch (status)
            {
                case PStatus.Frozen: description = "was frozen solid"; break;
                case PStatus.Paralyzed: description = "is paralyzed! It may be unable to move"; break;
                default: throw new ArgumentOutOfRangeException(nameof(status), $"Invalid status change: {status}");
            }
            Console.WriteLine("{0} {1}!", pkmn.Shell.Species, description);
        }
        void PrintStatusEnded(PPokemon pkmn)
        {
            var status = pkmn.Status;
            string description;
            switch (status)
            {
                case PStatus.Frozen: description = "thawed out"; break;
                default: throw new ArgumentOutOfRangeException(nameof(status), $"Invalid status ending: {status}");
            }
            Console.WriteLine("{0} {1}!", pkmn.Shell.Species, description);
        }
        void PrintStatusCancelledMove(PPokemon pkmn)
        {
            var status = pkmn.Status;
            string description;
            switch (status)
            {
                case PStatus.Frozen: description = "is frozen solid"; break;
                case PStatus.Paralyzed: description = "is paralyzed! It can't move"; break;
                default: throw new ArgumentOutOfRangeException(nameof(status), $"Invalid status cancelling move: {status}");
            }
            Console.WriteLine("{0} {1}!", pkmn.Shell.Species, description);
        }



        public static void ConsoleBattleEventHandler(INetPacketStream packet)
        {
            switch (packet)
            {
                case PSwitchInPacket sip:
                    Console.WriteLine("{1} sent out {0}!", PKnownInfo.Instance[sip.PokemonId].Shell.Species, PKnownInfo.Instance.DisplayName(sip.LocallyOwned));
                    break;
                case PUsedMovePacket ump:
                    Console.WriteLine("{0} used {1}!", PKnownInfo.Instance[ump.PokemonId].Shell.Species, ump.Move);
                    break;
            }
        }
    }
}
