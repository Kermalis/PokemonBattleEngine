using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Util;
using System;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        PBattlePokemon efCurAttacker, efCurDefender;
        PMove efCurMove;
        ushort efDamage;
        double efDamageMultiplier;
        bool efLandedCrit;

        void UseMove(PBattlePokemon attacker)
        {
            efCurAttacker = attacker;
            // TODO: Target
            efCurDefender = attacker == battlers[0] ? battlers[1] : battlers[0]; // Temporary
            efCurMove = attacker.SelectedMove;
            efDamage = 0;
            efDamageMultiplier = 1;
            efLandedCrit = false;

            PMoveData mData = PMoveData.Data[efCurMove];
            switch (mData.Effect)
            {
                case PMoveEffect.Hit:
                    Ef_Hit();
                    break;
                case PMoveEffect.Hit__MaybeFlinch:
                    Ef_Hit__MaybeFlinch(mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLower_SPDEF_By1:
                    Ef_Hit__MaybeLower_SPDEF_By1(mData.EffectParam);
                    break;
            }
        }

        bool AttackCancelCheck()
        {
            if (efCurAttacker.Pokemon.Status2.HasFlag(PStatus2.Flinching))
            {
                PrintFlinch();
                return true;
            }
            return false;
        }
        bool AccuracyCheck()
        {
            PMoveData mData = PMoveData.Data[efCurMove];
            if (mData.Accuracy == 0 // Always-hit moves
                || PUtils.ApplyChance(mData.Accuracy) // Got lucky and landed a hit
                )
                return false;
            PrintMiss();
            return true;
        }
        void DealDamage(PBattlePokemon victim)
        {
            ushort total = (ushort)(efDamage * efDamageMultiplier);
            efCurDefender.Pokemon.HP = (ushort)Math.Max(0, victim.Pokemon.HP - total);
        }

        bool Ef_Hit()
        {
            if (AttackCancelCheck())
                return false;
            if (AccuracyCheck())
                return false;
            PrintMoveUsed();
            // PPReduce();
            // CritCheck();
            efDamage = CalculateDamage(efCurAttacker, efCurDefender, efCurMove);
            // TypeCheck();
            DealDamage(efCurDefender);
            PrintDamageDone();
            PrintCrit();
            // TryFaint();
            return true;
        }
        bool Ef_Hit__MaybeFlinch(int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance))
                return false;
            efCurDefender.Pokemon.Status2 |= PStatus2.Flinching;
            return true;
        }
        bool Ef_Hit__MaybeLower_SPDEF_By1(int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance))
                return false;
            // TODO:
            // 
            return true;
        }
    }
}
