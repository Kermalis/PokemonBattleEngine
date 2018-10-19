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
        double efEffectiveness, efDamageMultiplier;
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
            efEffectiveness = efDamageMultiplier = 1;
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

        // Returns true if an attack gets cancelled
        bool AttackCancelCheck()
        {
            if (efCurAttacker.Mon.Status2.HasFlag(PStatus2.Flinching))
            {
                PrintFlinch();
                return true;
            }
            else if (efCurAttacker.Mon.Status == PStatus.Frozen)
            {
                // 20% chance to thaw out
                if (PUtils.ApplyChance(20))
                {
                    PrintStatusEnded(efCurAttacker.Mon);
                    efCurAttacker.Mon.Status = PStatus.NoStatus;
                    return false;
                }
                else
                {
                    PrintStatusCancelledMove(efCurAttacker.Mon);
                    return true;
                }
            }
            else if (efCurAttacker.Mon.Status == PStatus.Paralyzed)
            {
                // 25% chance to be unable to move
                if (PUtils.ApplyChance(25))
                {
                    PrintStatusCancelledMove(efCurAttacker.Mon);
                    return true;
                }
            }
            return false;
        }
        // Returns true if an attack misses
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
        void DealDamage()
        {
            PPokemon victim = efCurDefender.Mon;
            ushort total = (ushort)(efDamage * efEffectiveness * efDamageMultiplier);
            var oldHP = victim.HP;
            victim.HP = (ushort)Math.Max(0, victim.HP - total);
            PrintDamage(victim, (ushort)(oldHP - victim.HP));
        }
        // Returns false if an attack is ineffective
        bool TypeCheck()
        {
            PPokemonData attackerPData = PPokemonData.Data[efCurAttacker.Mon.Shell.Species];
            PPokemonData defenderPData = PPokemonData.Data[efCurDefender.Mon.Shell.Species];
            PMoveData mData = PMoveData.Data[efCurMove];

            // If a pokemon uses a move that shares a type with it, it gains a 1.5x power boost
            if (attackerPData.HasType(mData.Type))
                efDamageMultiplier *= 1.5;

            efEffectiveness *= PPokemonData.TypeEffectiveness[(int)mData.Type, (int)defenderPData.Type1];
            // Don't want to halve twice for a mono type
            if (defenderPData.Type1 != defenderPData.Type2)
                efEffectiveness *= PPokemonData.TypeEffectiveness[(int)mData.Type, (int)defenderPData.Type2];

            if (efEffectiveness == 0)
            {
                PrintEffectiveness();
                return false;
            }

            return true;
        }
        // Returns true if the pokemon fainted
        bool TryFaint()
        {
            if (efCurDefender.Mon.HP < 1)
            {
                PrintFaint(efCurDefender.Mon);
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
        // Returns true if the status was applied
        bool ApplyStatusIfPossible(PPokemon pkmn, PStatus status)
        {
            PPokemonData pData = PPokemonData.Data[pkmn.Shell.Species];

            // TODO: Limber

            // An Ice type pokemon cannot be Frozen
            if (status == PStatus.Frozen && pData.HasType(PType.Ice))
                return false;

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
            if (!TypeCheck())
                return false;
            DealDamage();
            PrintEffectiveness();
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
            efCurDefender.Mon.Status2 |= PStatus2.Flinching;
            return true;
        }
        bool HitAndMaybeApplyStatus(PStatus status, int chance)
        {
            if (!Ef_Hit())
                return false;
            if (efCurDefender.Mon.Status != PStatus.NoStatus || !PUtils.ApplyChance(chance))
                return false;
            if (!ApplyStatusIfPossible(efCurDefender.Mon, status))
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
            ApplyStatChange(efCurDefender.Mon, stat, change);
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
            var pkmn = efCurAttacker.Mon;
            ApplyStatChange(pkmn, PStat.Defense, -1);
            ApplyStatChange(pkmn, PStat.SpDefense, -1);
            ApplyStatChange(pkmn, PStat.Attack, +2);
            ApplyStatChange(pkmn, PStat.SpAttack, +2);
            ApplyStatChange(pkmn, PStat.Speed, +2);
            return true;
        }
    }
}
