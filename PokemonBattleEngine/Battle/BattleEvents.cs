using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        void PrintMoveUsed()
        {
            Console.WriteLine("{0} used {1}!", efCurAttacker.Pokemon.Shell.Species, efCurMove);
        }
        void PrintMiss()
        {
            Console.WriteLine("{0}'s attack missed!", efCurAttacker.Pokemon.Shell.Species);
        }
        void PrintFlinch()
        {
            Console.WriteLine("{0} flinched!", efCurAttacker.Pokemon.Shell.Species);
        }
        void PrintDamageDone()
        {
            double total = efDamage * efDamageMultiplier;
            double percentage = total / efCurDefender.Pokemon.MaxHP;
            Console.WriteLine("{0} took {1} ({2:P2}) damage!", efCurDefender.Pokemon.Shell.Species, total, percentage);
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
                    else if (change <= -3) description = "severely fell";
                    else if (change >= 3) description = "rose drastically";
                    else return; // +0
                    break;
            }
            Console.WriteLine("{0}'s {1} {2}!", pkmn.Shell.Species, stat, description);
        }
    }
}
