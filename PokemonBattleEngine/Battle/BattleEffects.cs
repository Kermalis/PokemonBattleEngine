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

        void DoTurnEndedEffects(PBattlePokemon battler)
        {
            // TODO: Limber
        }

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
                case PMoveEffect.Hit: Ef_Hit(); break;
                case PMoveEffect.Hit__MaybeFlinch: Ef_Hit__MaybeFlinch(mData.EffectParam); break;
                case PMoveEffect.Hit__MaybeFreeze: Ef_Hit__MaybeFreeze(mData.EffectParam); break;
                case PMoveEffect.Hit__MaybeLower_SPDEF_By1: Ef_Hit__MaybeLower_SPDEF_By1(mData.EffectParam); break;
                case PMoveEffect.Hit__MaybeParalyze: Ef_Hit__MaybeParalyze(mData.EffectParam); break;
                case PMoveEffect.Lower_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2: Ef_Lower_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2(); break;
            }
        }

        bool AttackCancelCheck()
        {
            if (efCurAttacker.Pokemon.Status2.HasFlag(PStatus2.Flinching))
            {
                PrintFlinch();
                return true;
            }
            else if (efCurAttacker.Pokemon.Status == PStatus.Frozen)
            {
                // 20% chance to thaw out
                if (PUtils.ApplyChance(20))
                {
                    PrintStatusEnded(efCurAttacker.Pokemon);
                    efCurAttacker.Pokemon.Status = PStatus.NoStatus;
                    return false;
                }
                else
                {
                    PrintStatusCancelledMove(efCurAttacker.Pokemon);
                    return true;
                }
            }
            else if (efCurAttacker.Pokemon.Status == PStatus.Paralyzed)
            {
                // 25% chance to be unable to move
                if (PUtils.ApplyChance(25))
                {
                    PrintStatusCancelledMove(efCurAttacker.Pokemon);
                    return true;
                }
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
            var oldHP = victim.HP;
            victim.HP = (ushort)Math.Max(0, victim.HP - total);
            PrintDamage(victim, oldHP - victim.HP);
        }
        bool TryFaint()
        {
            if (efCurDefender.Pokemon.HP < 1)
            {
                PrintFaint(efCurDefender.Pokemon);
                return true;
            }
            return false;
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
        bool ApplyStatusIfPossible(PPokemon pkmn, PStatus status, bool tryForce)
        {
            // TODO: Limber
            // ice type vs freezing
            pkmn.Status = status;
            PrintStatusChange(pkmn, status);
            return true;
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
            PrintCrit();
            if (TryFaint())
                return false;
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
        bool HitAndMaybeApplyStatus(PStatus status, int chance)
        {
            if (!Ef_Hit())
                return false;
            if (efCurDefender.Pokemon.Status != PStatus.NoStatus || !PUtils.ApplyChance(chance))
                return false;
            if (!ApplyStatusIfPossible(efCurDefender.Pokemon, status, false))
                return false;
            return true;
        }
        bool Ef_Hit__MaybeFreeze(int chance)
        {
            return HitAndMaybeApplyStatus(PStatus.Frozen, chance);
        }
        bool Ef_Hit__MaybeParalyze(int chance)
        {
            return HitAndMaybeApplyStatus(PStatus.Paralyzed, chance);
        }
        bool HitAndMaybeChangeStat(PStat stat, sbyte change, int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance))
                return false;
            ApplyStatChange(efCurDefender.Pokemon, stat, change);
            return true;
        }
        bool Ef_Hit__MaybeLower_SPDEF_By1(int chance)
        {
            return HitAndMaybeChangeStat(PStat.SpDefense, -1, chance);
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
