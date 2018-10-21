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
            double numerator = Math.Min(baseVal, baseVal + change);
            double denominator = Math.Min(baseVal, baseVal - change);
            return numerator / denominator;
        }

        // Returns false (and prints) if an attack is ineffective
        bool TypeCheck()
        {
            PPokemonData attackerPData = PPokemonData.Data[bAttacker.Mon.Shell.Species];
            PPokemonData defenderPData = PPokemonData.Data[bDefender.Mon.Shell.Species];
            PMoveData mData = PMoveData.Data[bMove];

            // If a pokemon uses a move that shares a type with it, it gains a 1.5x power boost
            if (attackerPData.HasType(mData.Type))
                bDamageMultiplier *= 1.5;

            bEffectiveness *= PPokemonData.TypeEffectiveness[(int)mData.Type, (int)defenderPData.Type1];
            // Don't want to halve twice for a mono type
            if (defenderPData.Type1 != defenderPData.Type2)
                bEffectiveness *= PPokemonData.TypeEffectiveness[(int)mData.Type, (int)defenderPData.Type2];

            if (bEffectiveness == 0)
            {
                BroadcastEffectiveness();
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
                    basePower = Math.Max(1, (byte.MaxValue - bAttacker.Mon.Shell.Friendship) / 2.5);
                    break;
                case PMove.Return:
                    basePower = Math.Max(1, bAttacker.Mon.Shell.Friendship / 2.5);
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
            if (bAttacker.Mon.Shell.Item == PItem.LightBall && bAttacker.Mon.Shell.Species == PSpecies.Pikachu)
                basePower *= 2;
            // Retaliate doubles power if the team has a pokemon that fainted the previous turn
            if (bMove == PMove.Retaliate && bAttacker.Team.MonFaintedLastTurn)
                basePower *= 2;
            // Overgrow gives a 1.5x boost to Grass attacks if the efCurAttacker is below 1/3 max HP
            if (mData.Type == PType.Grass && bAttacker.Mon.Shell.Ability == PAbility.Overgrow && bAttacker.Mon.HP <= bAttacker.Mon.MaxHP / 3)
                basePower *= 1.5;
            // Blaze gives a 1.5x boost to Fire attacks if the efCurAttacker is below 1/3 max HP
            if (mData.Type == PType.Fire && bAttacker.Mon.Shell.Ability == PAbility.Blaze && bAttacker.Mon.HP <= bAttacker.Mon.MaxHP / 3)
                basePower *= 1.5;
            // Torrent gives a 1.5x boost to Water attacks if the efCurAttacker is below 1/3 max HP
            if (mData.Type == PType.Water && bAttacker.Mon.Shell.Ability == PAbility.Torrent && bAttacker.Mon.HP <= bAttacker.Mon.MaxHP / 3)
                basePower *= 1.5;
            // Swarm gives a 1.5x boost to Bug attacks if the efCurAttacker is below 1/3 max HP
            if (mData.Type == PType.Bug && bAttacker.Mon.Shell.Ability == PAbility.Swarm && bAttacker.Mon.HP <= bAttacker.Mon.MaxHP / 3)
                basePower *= 1.5;
            // A Burned pokemon does half the damage when it is Burned unless it has the Guts ability
            if (mData.Category == PMoveCategory.Physical && bAttacker.Mon.Status1 == PStatus1.Burned && bAttacker.Mon.Shell.Ability != PAbility.Guts)
                basePower /= 2;
            // Damage is halved when using Fire or Ice moves against a pokemon with the Thick Fat ability
            if (bDefender.Mon.Shell.Ability == PAbility.ThickFat && (mData.Type == PType.Fire || mData.Type == PType.Ice))
                basePower /= 2;

            return (ushort)basePower;
        }
        ushort CalculateAttack()
        {
            PMoveData mData = PMoveData.Data[bMove];
            double attack = bAttacker.Mon.Attack * GetStatMultiplier(bAttacker.Mon.AttackChange);

            // Pokemon with the Huge Power or Pure Power ability get a 2x attack boost
            if (bAttacker.Mon.Shell.Ability == PAbility.HugePower || bAttacker.Mon.Shell.Ability == PAbility.PurePower)
                attack *= 2;
            // A Cubone or Marowak holding a Thick Club gets a 2x attack boost
            if (bAttacker.Mon.Shell.Item == PItem.ThickClub && (bAttacker.Mon.Shell.Species == PSpecies.Cubone || bAttacker.Mon.Shell.Species == PSpecies.Marowak))
                attack *= 2;
            // A pokemon with the Hustle ability gets a 1.5x attack boost
            if (bAttacker.Mon.Shell.Ability == PAbility.Hustle)
                attack *= 1.5;
            // A pokemon with the Guts ability gets a 1.5x attack boost when afflicted with a status
            if (bAttacker.Mon.Shell.Ability == PAbility.Guts && bAttacker.Mon.Status1 != PStatus1.NoStatus)
                attack *= 1.5;
            // A pokemon holding a Choice Band gets a 1.5x attack boost
            if (bAttacker.Mon.Shell.Item == PItem.ChoiceBand)
                attack *= 1.5;

            return (ushort)attack;
        }
        ushort CalculateDefense()
        {
            PMoveData mData = PMoveData.Data[bMove];
            double defense = bAttacker.Mon.Defense * GetStatMultiplier(bDefender.Mon.DefenseChange);

            // A Ditto holding a Metal Powder gets a 2x defense boost
            if (bDefender.Mon.Shell.Item == PItem.MetalPowder && bDefender.Mon.Shell.Species == PSpecies.Ditto)
                defense *= 2;
            // A pokemon with the Marvel Scale ability gets a 1.5x defense boost when afflicted with a status
            if (bDefender.Mon.Shell.Ability == PAbility.MarvelScale && bDefender.Mon.Status1 != PStatus1.NoStatus)
                defense *= 1.5;

            return (ushort)defense;
        }
        ushort CalculateSpAttack()
        {
            PMoveData mData = PMoveData.Data[bMove];
            double spAttack = bAttacker.Mon.SpAttack * GetStatMultiplier(bAttacker.Mon.SpAttackChange);

            // TODO:
            // A pokemon with the Plus ability gets a 1.5x spAttack boost if a teammate has the Minus ability
            /* if (efCurAttacker.Mon.Ability == Ability.Plus && PartnerHasAbility(Ability.Minus))
             * spAttack *= 1.5;*/
            // A pokemon with the Minus ability gets a 1.5x spAttack boost if a teammate has the Plus ability
            /* if (efCurAttacker.Mon.Ability == Ability.Minus && PartnerHasAbility(Ability.Plus))
             * spAttack *= 1.5;*/

            // A Clamperl holding a Deep Sea Tooth gets a 2x spAttack boost
            if (bAttacker.Mon.Shell.Item == PItem.DeepSeaTooth && bAttacker.Mon.Shell.Species == PSpecies.Clamperl)
                spAttack *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spAttack boost
            if (bAttacker.Mon.Shell.Item == PItem.SoulDew && (bAttacker.Mon.Shell.Species == PSpecies.Latios || bAttacker.Mon.Shell.Species == PSpecies.Latias))
                spAttack *= 1.5;

            return (ushort)spAttack;
        }
        ushort CalculateSpDefense()
        {
            PMoveData mData = PMoveData.Data[bMove];
            double spDefense = bAttacker.Mon.SpDefense * GetStatMultiplier(bDefender.Mon.SpDefenseChange);

            // A Clamperl holding a Deep Sea Scale gets a 2x spDefense boost
            if (bDefender.Mon.Shell.Item == PItem.DeepSeaScale && bDefender.Mon.Shell.Species == PSpecies.Clamperl)
                spDefense *= 2;
            // A Latios or Latias holding a Soul Dew gets a 1.5x spDefense boost
            if (bDefender.Mon.Shell.Item == PItem.SoulDew && (bDefender.Mon.Shell.Species == PSpecies.Latios || bDefender.Mon.Shell.Species == PSpecies.Latias))
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

            damage = (ushort)(2 * bAttacker.Mon.Shell.Level / 5 + 2);
            damage = (ushort)(damage * a * p / d);
            damage /= 50;
            return (ushort)(damage + 2);
        }
    }
}
