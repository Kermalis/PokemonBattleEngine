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
    }
}
