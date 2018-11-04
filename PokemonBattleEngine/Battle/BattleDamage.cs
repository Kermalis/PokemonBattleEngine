using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        // "forMissing" is true if the multiplier will be used for accuracy or evasion
        // If this is used for evasion, "change" should be multiplied by -1 before being passed in (because evasion is for reducing a hit chance, not increasing)
        public double GetStatMultiplier(sbyte change, bool forMissing = false)
        {
            double baseVal = forMissing ? 3 : 2;
            double numerator = Math.Max(baseVal, baseVal + change);
            double denominator = Math.Max(baseVal, baseVal - change);
            return numerator / denominator;
        }

        // Returns false and broadcasts if an attack is ineffective
        bool TypeCheck()
        {
            PPokemonData attackerPData = PPokemonData.Data[bAttacker.Shell.Species];
            PPokemonData defenderPData = PPokemonData.Data[bDefender.Shell.Species];
            PMoveData mData = PMoveData.Data[bMove];

            switch (bMove)
            {
                case PMove.HiddenPower:
                    bMoveType = bAttacker.GetHiddenPowerType();
                    break;
                default:
                    bMoveType = mData.Type;
                    break;
            }

            // If a pokemon uses a move that shares a type with it, it gains a 1.5x power boost
            if (attackerPData.HasType(bMoveType))
                bDamageMultiplier *= 1.5;

            bEffectiveness *= PPokemonData.TypeEffectiveness[(int)bMoveType, (int)defenderPData.Type1];
            // Don't want to halve twice for a mono type
            if (defenderPData.Type1 != defenderPData.Type2)
                bEffectiveness *= PPokemonData.TypeEffectiveness[(int)bMoveType, (int)defenderPData.Type2];

            if (bEffectiveness == 0)
            {
                BroadcastEffectiveness(0);
                return false;
            }

            return true;
        }

        ushort CalculateBasePower()
        {
            PMoveData mData = PMoveData.Data[bMove];
            double basePower = mData.Power;

            // Moves with variable base power
            switch (bMove)
            {
                case PMove.Frustration:
                    basePower = Math.Max(1, (byte.MaxValue - bAttacker.Shell.Friendship) / 2.5);
                    break;
                case PMove.HiddenPower:
                    basePower = bAttacker.GetHiddenPowerBasePower();
                    break;
                case PMove.Return:
                    basePower = Math.Max(1, bAttacker.Shell.Friendship / 2.5);
                    break;
            }

            // TODO: Stuff like mystic water

            // Damage is halved when using electric moves while mud sport is active
            /* if (type == Type.Electric && MudSportActive())
             * basePower /= 2;*/
            // Damage is halved when using fire moves while water sport is active
            /* if (type == Type.Fire && WaterSportActive())
             * basePower /= 2;*/
            // An underground pokemon that gets hit with Earthquake takes twice the damage
            /* if (move == PMove.Earthquake && efCurDefender.Pokemon.Status2.HasFlag(PStatus2.Underground))
                basePower *= 2;*/
            // An underwater pokemon that gets hit with Surf takes twice the damage
            /* if (move == PMove.Surf && efCurDefender.Pokemon.Status2.HasFlag(PStatus2.Underwater))
                basePower *= 2;*/

            // A Pikachu holding a Light Ball gets a 2x power boost
            if (bAttacker.Shell.Item == PItem.LightBall && bAttacker.Shell.Species == PSpecies.Pikachu)
                basePower *= 2;
            // Retaliate doubles power if the team has a pokemon that fainted the previous turn
            if (bMove == PMove.Retaliate && teams[bAttacker.Local ? 0 : 1].MonFaintedLastTurn)
                basePower *= 2;
            // Overgrow gives a 1.5x boost to Grass attacks if the efCurAttacker is below 1/3 max HP
            if (bMoveType == PType.Grass && bAttacker.Ability == PAbility.Overgrow && bAttacker.HP <= bAttacker.MaxHP / 3)
                basePower *= 1.5;
            // Blaze gives a 1.5x boost to Fire attacks if the efCurAttacker is below 1/3 max HP
            if (bMoveType == PType.Fire && bAttacker.Ability == PAbility.Blaze && bAttacker.HP <= bAttacker.MaxHP / 3)
                basePower *= 1.5;
            // Torrent gives a 1.5x boost to Water attacks if the efCurAttacker is below 1/3 max HP
            if (bMoveType == PType.Water && bAttacker.Ability == PAbility.Torrent && bAttacker.HP <= bAttacker.MaxHP / 3)
                basePower *= 1.5;
            // Swarm gives a 1.5x boost to Bug attacks if the efCurAttacker is below 1/3 max HP
            if (bMoveType == PType.Bug && bAttacker.Ability == PAbility.Swarm && bAttacker.HP <= bAttacker.MaxHP / 3)
                basePower *= 1.5;
            // A Burned pokemon does half the damage when it is Burned unless it has the Guts ability
            if (mData.Category == PMoveCategory.Physical && bAttacker.Status1 == PStatus1.Burned && bAttacker.Ability != PAbility.Guts)
                basePower /= 2;
            // Damage is halved when using Fire or Ice moves against a pokemon with the Thick Fat ability
            if (bDefender.Ability == PAbility.ThickFat && (bMoveType == PType.Fire || bMoveType == PType.Ice))
                basePower /= 2;

            return (ushort)basePower;
        }
        ushort CalculateAttack()
        {
            PMoveData mData = PMoveData.Data[bMove];
            double attack = bAttacker.Attack * GetStatMultiplier(bAttacker.AttackChange);

            // Pokemon with the Huge Power or Pure Power ability get a 2x attack boost
            if (bAttacker.Ability == PAbility.HugePower || bAttacker.Ability == PAbility.PurePower)
                attack *= 2;
            // A Cubone or Marowak holding a Thick Club gets a 2x attack boost
            if (bAttacker.Shell.Item == PItem.ThickClub && (bAttacker.Shell.Species == PSpecies.Cubone || bAttacker.Shell.Species == PSpecies.Marowak))
                attack *= 2;
            // A pokemon with the Hustle ability gets a 1.5x attack boost
            if (bAttacker.Ability == PAbility.Hustle)
                attack *= 1.5;
            // A pokemon with the Guts ability gets a 1.5x attack boost when afflicted with a status
            if (bAttacker.Ability == PAbility.Guts && bAttacker.Status1 != PStatus1.None)
                attack *= 1.5;
            // A pokemon holding a Choice Band gets a 1.5x attack boost
            if (bAttacker.Shell.Item == PItem.ChoiceBand)
                attack *= 1.5;

            return (ushort)attack;
        }
        ushort CalculateDefense()
        {
            PMoveData mData = PMoveData.Data[bMove];
            double defense = bAttacker.Defense * GetStatMultiplier(bDefender.DefenseChange);

            // A Ditto holding a Metal Powder gets a 2x defense boost
            if (bDefender.Shell.Item == PItem.MetalPowder && bDefender.Shell.Species == PSpecies.Ditto)
                defense *= 2;
            // A pokemon with the Marvel Scale ability gets a 1.5x defense boost when afflicted with a status
            if (bDefender.Ability == PAbility.MarvelScale && bDefender.Status1 != PStatus1.None)
                defense *= 1.5;

            return (ushort)defense;
        }
        ushort CalculateSpAttack()
        {
            PMoveData mData = PMoveData.Data[bMove];
            double spAttack = bAttacker.SpAttack * GetStatMultiplier(bAttacker.SpAttackChange);

            // TODO:
            // A pokemon with the Plus ability gets a 1.5x spAttack boost if a teammate has the Minus ability
            /* if (efCurAttacker.Ability == Ability.Plus && PartnerHasAbility(Ability.Minus))
             * spAttack *= 1.5;*/
            // A pokemon with the Minus ability gets a 1.5x spAttack boost if a teammate has the Plus ability
            /* if (efCurAttacker.Ability == Ability.Minus && PartnerHasAbility(Ability.Plus))
             * spAttack *= 1.5;*/

            // A Clamperl holding a Deep Sea Tooth gets a 2x spAttack boost
            if (bAttacker.Shell.Item == PItem.DeepSeaTooth && bAttacker.Shell.Species == PSpecies.Clamperl)
                spAttack *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spAttack boost
            if (bAttacker.Shell.Item == PItem.SoulDew && (bAttacker.Shell.Species == PSpecies.Latios || bAttacker.Shell.Species == PSpecies.Latias))
                spAttack *= 1.5;

            return (ushort)spAttack;
        }
        ushort CalculateSpDefense()
        {
            PMoveData mData = PMoveData.Data[bMove];
            double spDefense = bAttacker.SpDefense * GetStatMultiplier(bDefender.SpDefenseChange);

            // A Clamperl holding a Deep Sea Scale gets a 2x spDefense boost
            if (bDefender.Shell.Item == PItem.DeepSeaScale && bDefender.Shell.Species == PSpecies.Clamperl)
                spDefense *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spDefense boost
            if (bDefender.Shell.Item == PItem.SoulDew && (bDefender.Shell.Species == PSpecies.Latios || bDefender.Shell.Species == PSpecies.Latias))
                spDefense *= 1.5;

            return (ushort)spDefense;
        }
        ushort CalculateDamage()
        {
            PMoveData mData = PMoveData.Data[bMove];
            ushort damage;
            ushort a = 0, d = 0,
                p = CalculateBasePower();

            // TODO: Determine a and d for moves like Foul Play and Psyshock

            if (mData.Category == PMoveCategory.Physical)
            {
                a = CalculateAttack();
                d = CalculateDefense();
            }
            else if (mData.Category == PMoveCategory.Special)
            {
                a = CalculateSpAttack();
                d = CalculateSpDefense();
            }

            damage = (ushort)(2 * bAttacker.Shell.Level / 5 + 2);
            damage = (ushort)(damage * a * p / d);
            damage /= 50;
            return (ushort)(damage + 2);
        }
    }
}
