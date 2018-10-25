using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Util;
using System;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        PBattlePokemon bAttacker, bDefender;
        PMove bMove; PType bMoveType;
        ushort bDamage;
        double bEffectiveness, bDamageMultiplier;
        bool bLandedCrit;

        void DoTurnEndedEffects(PBattlePokemon battler)
        {
            // TODO: Limber

            // Major statuses
            switch (battler.Mon.Status1)
            {
                case PStatus1.Burned:
                    BroadcastStatus1CausedDamage(battler.Mon);
                    DealDamage(battler.Mon, (ushort)(battler.Mon.MaxHP / PConstants.BurnDamageDenominator));
                    TryFaint(battler);
                    break;
                case PStatus1.Poisoned:
                    BroadcastStatus1CausedDamage(battler.Mon);
                    DealDamage(battler.Mon, (ushort)(battler.Mon.MaxHP / PConstants.PoisonDamageDenominator));
                    TryFaint(battler);
                    break;
                case PStatus1.BadlyPoisoned:
                    BroadcastStatus1CausedDamage(battler.Mon);
                    DealDamage(battler.Mon, (ushort)(battler.Mon.MaxHP * battler.Status1Counter / PConstants.ToxicDamageDenominator));
                    if (!TryFaint(battler))
                        battler.Status1Counter++;
                    break;
            }

            // Items
            switch (battler.Mon.Shell.Item)
            {
                case PItem.Leftovers:
                    if (HealDamage(battler.Mon, (ushort)(battler.Mon.MaxHP / PConstants.LeftoversDenominator)))
                        BroadcastItemUsed(battler.Mon);
                    break;
            }
        }

        void UseMove(PBattlePokemon attacker)
        {
            bAttacker = attacker;
            // TODO: Target
            bDefender = attacker == battlers[0] ? battlers[1] : battlers[0]; // Temporary
            bMove = attacker.SelectedMove; // bMoveType gets set in BattleDamage.cs->TypeCheck()
            bDamage = 0;
            bEffectiveness = bDamageMultiplier = 1;
            bLandedCrit = false;

            PMoveData mData = PMoveData.Data[bMove];
            switch (mData.Effect)
            {
                case PMoveEffect.Change_Opponent_ATK:
                    ChangeStat(bDefender, PStat.Attack, mData.EffectParam);
                    break;
                case PMoveEffect.Change_Opponent_DEF:
                    ChangeStat(bDefender, PStat.Defense, mData.EffectParam);
                    break;
                case PMoveEffect.Change_User_DEF:
                    ChangeStat(bAttacker, PStat.Defense, mData.EffectParam);
                    break;
                case PMoveEffect.Change_User_SPATK:
                    ChangeStat(bAttacker, PStat.SpAttack, mData.EffectParam);
                    break;
                case PMoveEffect.Hit:
                    Ef_Hit();
                    break;
                case PMoveEffect.Hit__MaybeBurn:
                    HitAndMaybeApplyStatus1(PStatus1.Burned, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeFlinch:
                    Ef_Hit__MaybeFlinch(mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeFreeze:
                    HitAndMaybeApplyStatus1(PStatus1.Frozen, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeLower_SPDEF_By1:
                    HitAndMaybeChangeStat(PStat.SpDefense, -1, mData.EffectParam);
                    break;
                case PMoveEffect.Hit__MaybeParalyze:
                    HitAndMaybeApplyStatus1(PStatus1.Paralyzed, mData.EffectParam);
                    break;
                case PMoveEffect.Lower_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2:
                    Ef_Lower_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2();
                    break;
                case PMoveEffect.Toxic:
                    Ef_Toxic();
                    break;
                case PMoveEffect.Transform:
                case PMoveEffect.Moonlight:
                    // TODO
                    BroadcastMoveUsed();
                    PPReduce();
                    BroadcastFail();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mData.Effect), $"Invalid move effect: {mData.Effect}");
            }
        }

        // Returns true if an attack gets cancelled
        // Broadcasts status ending events & status causing immobility events
        bool AttackCancelCheck()
        {
            // Flinch first
            if (bAttacker.Mon.Status2.HasFlag(PStatus2.Flinching))
            {
                BroadcastFlinch();
                return true;
            }

            // Major statuses
            switch (bAttacker.Mon.Status1)
            {
                case PStatus1.Frozen:
                    // 20% chance to thaw out
                    if (PUtils.ApplyChance(20))
                    {
                        BroadcastStatus1Ended(bAttacker.Mon);
                        bAttacker.Mon.Status1 = PStatus1.None;
                        return false;
                    }
                    // Didn't thaw out
                    BroadcastStatus1CausedImmobility(bAttacker.Mon);
                    return true;
                case PStatus1.Paralyzed:
                    // 25% chance to be unable to move
                    if (PUtils.ApplyChance(25))
                    {
                        BroadcastStatus1CausedImmobility(bAttacker.Mon);
                        return true;
                    }
                    break;
            }

            return false;
        }

        // Returns true if an attack misses
        // Broadcasts the event if it missed
        bool AccuracyCheck()
        {
            PMoveData mData = PMoveData.Data[bMove];
            if (mData.Accuracy == 0 // Always-hit moves
                || PUtils.ApplyChance(mData.Accuracy) // Got lucky and landed a hit
                )
                return false;
            BroadcastMiss();
            return true;
        }

        // Broadcasts the event
        void DealDamage()
            => DealDamage(bDefender.Mon, (ushort)(bDamage * bEffectiveness * bDamageMultiplier));
        void DealDamage(PPokemon pkmn, ushort hp)
        {
            var oldHP = pkmn.HP;
            pkmn.HP = (ushort)Math.Max(0, pkmn.HP - Math.Max((ushort)1, hp)); // Always try to lose at least 1 HP
            var damageAmt = oldHP - pkmn.HP;
            BroadcastHPChanged(pkmn, -damageAmt);
        }

        // Returns true if it healed
        // Broadcasts the event if it healed
        bool HealDamage(PPokemon pkmn, ushort hp)
        {
            var oldHP = pkmn.HP;
            pkmn.HP = (ushort)Math.Min(pkmn.MaxHP, pkmn.HP + Math.Max((ushort)1, hp)); // Always try to heal at least 1 HP
            var healAmt = pkmn.HP - oldHP;
            if (healAmt < 1)
                return false;
            BroadcastHPChanged(pkmn, healAmt);
            return true;
        }

        // Broadcasts the event
        void PPReduce()
            => PPReduce(bAttacker.Mon, bMove);
        void PPReduce(PPokemon pkmn, PMove move)
        {
            var moveIndex = pkmn.Shell.Moves.ToList().IndexOf(move);
            int amtToReduce = 1;
            // TODO: If target is not self and has pressure
            var oldPP = pkmn.PP[moveIndex];
            pkmn.PP[moveIndex] = (byte)Math.Max(0, pkmn.PP[moveIndex] - amtToReduce);
            var reduceAmt = oldPP - pkmn.PP[moveIndex];
            BroadcastPPChanged(pkmn, move, -reduceAmt);
        }

        // Returns true if the pokemon fainted
        // Broadcasts the event if it did
        bool TryFaint()
            => TryFaint(bDefender);
        bool TryFaint(PBattlePokemon pkmn)
        {
            if (pkmn.Mon.HP < 1)
            {
                pkmn.Status1Counter = 0;
                BroadcastFaint(pkmn.Mon);
                return true;
            }
            return false;
        }

        // Does not broadcast the event
        public static void ApplyStatChange(PPkmnStatChangedPacket packet)
            => ApplyStatChange(PKnownInfo.Instance.Pokemon(packet.PokemonId), packet.Stat, packet.Change, null);
        // Broadcasts the event
        void ApplyStatChange(PPokemon pkmn, PStat stat, sbyte change)
            => ApplyStatChange(pkmn, stat, change, this);
        // Broadcasts the event if "battle" is not null
        static unsafe void ApplyStatChange(PPokemon pkmn, PStat stat, sbyte change, PBattle battle)
        {
            bool isTooMuch = false;
            fixed (sbyte* ptr = &pkmn.AttackChange)
            {
                sbyte* scPtr = ptr + (stat - PStat.Attack); // Points to the proper stat change sbyte
                if (*scPtr < -PConstants.MaxStatChange || *scPtr > PConstants.MaxStatChange)
                    isTooMuch = true;
                else
                    *scPtr = (sbyte)PUtils.Clamp(*scPtr + change, -PConstants.MaxStatChange, PConstants.MaxStatChange);
            }
            battle?.BroadcastStatChange(pkmn, stat, change, isTooMuch);
        }

        // Returns true if the status was applied
        // Broadcasts the change if applied
        bool ApplyStatus1IfPossible(PBattlePokemon pkmn, PStatus1 status)
        {
            if (pkmn.Mon.Status1 != PStatus1.None)
                return false;

            PPokemonData pData = PPokemonData.Data[pkmn.Mon.Shell.Species];

            // TODO: Limber

            // An Ice type pokemon cannot be Frozen
            if (status == PStatus1.Frozen && pData.HasType(PType.Ice))
                return false;
            // A Fire type pokemon cannot be burned
            if (status == PStatus1.Burned && pData.HasType(PType.Fire))
                return false;
            // A Poison or Steel type pokemon cannot be poisoned or badly poisoned
            if ((status == PStatus1.BadlyPoisoned || status == PStatus1.Poisoned) && (pData.HasType(PType.Poison) || pData.HasType(PType.Steel)))
                return false;


            pkmn.Mon.Status1 = status;
            // Start toxic counter
            if (status == PStatus1.BadlyPoisoned)
                pkmn.Status1Counter = 1;
            BroadcastStatus1Change(pkmn.Mon);

            return true;
        }

        bool Ef_Hit()
        {
            if (AttackCancelCheck())
                return false;
            BroadcastMoveUsed();
            PPReduce();
            if (AccuracyCheck())
                return false;
            // CritCheck();
            bDamage = CalculateDamage();
            if (!TypeCheck())
                return false;
            DealDamage();
            BroadcastEffectiveness();
            BroadcastCrit();
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
            bDefender.Mon.Status2 |= PStatus2.Flinching;
            return true;
        }

        bool HitAndMaybeApplyStatus1(PStatus1 status, int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance))
                return false;
            if (!ApplyStatus1IfPossible(bDefender, status))
                return false;
            return true;
        }

        bool HitAndMaybeChangeStat(PStat stat, sbyte change, int chance)
        {
            if (!Ef_Hit())
                return false;
            if (!PUtils.ApplyChance(chance))
                return false;
            ApplyStatChange(bDefender.Mon, stat, change);
            return true;
        }

        bool ChangeStat(PBattlePokemon pkmn, PStat stat, int change)
        {
            if (AttackCancelCheck())
                return false;
            BroadcastMoveUsed();
            PPReduce();
            ApplyStatChange(pkmn.Mon, stat, (sbyte)change);
            return true;
        }
        bool Ef_Lower_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2()
        {
            if (AttackCancelCheck())
                return false;
            BroadcastMoveUsed();
            PPReduce();
            var pkmn = bAttacker.Mon;
            ApplyStatChange(pkmn, PStat.Defense, -1);
            ApplyStatChange(pkmn, PStat.SpDefense, -1);
            ApplyStatChange(pkmn, PStat.Attack, +2);
            ApplyStatChange(pkmn, PStat.SpAttack, +2);
            ApplyStatChange(pkmn, PStat.Speed, +2);
            return true;
        }

        bool Ef_Toxic()
        {
            if (AttackCancelCheck())
                return false;
            BroadcastMoveUsed();
            PPReduce();
            if (AccuracyCheck())
                return false;
            if (!ApplyStatus1IfPossible(bDefender, PStatus1.BadlyPoisoned))
            {
                BroadcastFail();
                return false;
            }
            return true;
        }
    }
}
