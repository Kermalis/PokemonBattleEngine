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
                case PMoveEffect.Lower_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2:
                    Ef_Lower_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2();
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
        void DealDamage(PPokemon victim)
        {
            ushort total = (ushort)(efDamage * efDamageMultiplier);
            victim.HP = (ushort)Math.Max(0, victim.HP - total);
        }
        unsafe void ApplyStatChange(PPokemon pkmn, PStat stat, sbyte change)
        {
            bool tooMuch = false;
            fixed (sbyte* ptr = &pkmn.AttackChange)
            {
                sbyte* scPtr = ptr + (stat - PStat.Attack); // Points to the proper stat change sbyte
                if (*scPtr < -PConstants.MaxStatChange || *scPtr > PConstants.MaxStatChange)
                    tooMuch = true;
                else
                    *scPtr = (sbyte)PUtils.Clamp(*scPtr + change, -PConstants.MaxStatChange, PConstants.MaxStatChange);
            }
            PrintStatChange(pkmn, stat, change, tooMuch);
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
            DealDamage(efCurDefender.Pokemon);
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
            ApplyStatChange(efCurDefender.Pokemon, PStat.SpDefense, -1);
            return true;
        }
        bool Ef_Lower_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2()
        {
            if (AttackCancelCheck())
                return false;
            if (AccuracyCheck())
                return false;
            PrintMoveUsed();
            // PPReduce();
            var pkmn = efCurAttacker.Pokemon;
            ApplyStatChange(pkmn, PStat.Defense, -1);
            ApplyStatChange(pkmn, PStat.SpDefense, -1);
            ApplyStatChange(pkmn, PStat.Attack, +2);
            ApplyStatChange(pkmn, PStat.SpAttack, +2);
            ApplyStatChange(pkmn, PStat.Speed, +2);
            return true;
        }
    }
}
