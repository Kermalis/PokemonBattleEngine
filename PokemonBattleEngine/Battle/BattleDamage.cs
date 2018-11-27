using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        // "forMissing" is true if the multiplier will be used for accuracy or evasion
        public double GetStatMultiplier(sbyte change, bool forMissing = false)
        {
            double baseVal = forMissing ? 3 : 2;
            double numerator = Math.Max(baseVal, baseVal + change);
            double denominator = Math.Max(baseVal, baseVal - change);
            return numerator / denominator;
        }

        void TypeCheck(PPokemon user, PPokemon target, PMove move, out PType moveType, out PEffectiveness effectiveness)
        {
            PPokemonData targetPData = PPokemonData.Data[target.Species];

            moveType = PMoveData.GetMoveTypeForPokemon(user, move);

            double mult = PPokemonData.TypeEffectiveness[(int)moveType, (int)targetPData.Type1];
            mult *= PPokemonData.TypeEffectiveness[(int)moveType, (int)targetPData.Type2];

            if (mult == 0)
                effectiveness = PEffectiveness.Ineffective;
            else if (mult == 0.5)
                effectiveness = PEffectiveness.NotVeryEffective;
            else if (mult == 1.0)
                effectiveness = PEffectiveness.Normal;
            else
                effectiveness = PEffectiveness.SuperEffective;
        }

        ushort CalculateBasePower(PPokemon user, PPokemon target, PMove move, PType moveType, byte power, PMoveCategory moveCategory, bool ignoreReflectLightScreen, bool ignoreLifeOrb, bool criticalHit)
        {
            PPokemonData userPData = PPokemonData.Data[user.Species];
            PPokemonData targetPData = PPokemonData.Data[target.Species];
            double basePower = power;

            if (power == 0)
            {
                switch (move)
                {
                    case PMove.Frustration:
                        basePower = Math.Max(1, (byte.MaxValue - user.Shell.Friendship) / 2.5);
                        break;
                    case PMove.GrassKnot:
                    case PMove.LowKick:
                        if (targetPData.Weight >= 200.0)
                            basePower = 120;
                        else if (targetPData.Weight >= 100.0)
                            basePower = 100;
                        else if (targetPData.Weight >= 50.0)
                            basePower = 80;
                        else if (targetPData.Weight >= 25.0)
                            basePower = 60;
                        else if (targetPData.Weight >= 10.0)
                            basePower = 40;
                        else
                            basePower = 20;
                        break;
                    case PMove.HiddenPower:
                        basePower = user.GetHiddenPowerBasePower();
                        break;
                    case PMove.Retaliate:
                        basePower = PMoveData.Data[move].Power;
                        // Retaliate gets a 100% power boost if the user's team has a Pokémon that fainted during the previous turn
                        if (Teams[user.Local ? 0 : 1].MonFaintedLastTurn)
                            basePower *= 2.0;
                        break;
                    case PMove.Return:
                        basePower = Math.Max(1, user.Shell.Friendship / 2.5);
                        break;
                    case PMove.Steamroller:
                    case PMove.Stomp:
                        basePower = PMoveData.Data[move].Power;
                        // Stomp and Steamroller get a 100% power boost if the target is minimized
                        if (target.Status2.HasFlag(PStatus2.Minimized))
                            basePower *= 2.0;
                        break;
                    default:
                        basePower = PMoveData.Data[move].Power;
                        break;
                }
            }

            switch (Weather)
            {
                case PWeather.Raining:
                    if (moveType == PType.Water)
                        basePower *= 1.5;
                    else if (moveType == PType.Fire)
                        basePower *= 0.5;
                    break;
                case PWeather.Sunny:
                    if (moveType == PType.Fire)
                        basePower *= 1.5;
                    else if (moveType == PType.Water)
                        basePower *= 0.5;
                    break;
            }

            // Reflect & Light Screen reduce damage by 50% if there is one active battler or by 33% if there is more than one
            if (!ignoreReflectLightScreen && !criticalHit)
            {
                PTeam defenderTeam = Teams[target.Local ? 0 : 1];
                if ((defenderTeam.Status.HasFlag(PTeamStatus.Reflect) && moveCategory == PMoveCategory.Physical)
                    || (defenderTeam.Status.HasFlag(PTeamStatus.LightScreen) && moveCategory == PMoveCategory.Special))
                {
                    if (defenderTeam.NumPkmnOnField == 1)
                        basePower *= 0.5;
                    else
                        basePower *= 0.66;
                }
            }

            // If a Pokémon uses a move that shares a type with it, it gains a 50% power boost (100% if it has Adaptability)
            if (userPData.HasType(moveType))
            {
                if (user.Ability == PAbility.Adaptability)
                    basePower *= 2.0;
                else
                    basePower *= 1.5;
            }
            switch (moveType)
            {
                case PType.Bug:
                    if (user.Item == PItem.InsectPlate)
                        basePower *= 1.2;
                    // Swarm gives a 50% power boost to Bug attacks if the user is below 1/3 max HP
                    if (user.Ability == PAbility.Swarm && user.HP <= user.MaxHP / 3)
                        basePower *= 1.5;
                    break;
                case PType.Dark:
                    if (user.Item == PItem.DreadPlate)
                        basePower *= 1.2;
                    break;
                case PType.Dragon:
                    if (user.Item == PItem.DracoPlate)
                        basePower *= 1.2;
                    break;
                case PType.Electric:
                    if (user.Item == PItem.ZapPlate)
                        basePower *= 1.2;
                    break;
                case PType.Fighting:
                    if (user.Item == PItem.FistPlate)
                        basePower *= 1.2;
                    break;
                case PType.Fire:
                    if (user.Item == PItem.FlamePlate)
                        basePower *= 1.2;
                    // Blaze gives a 50% power boost to Fire attacks if the user is below 1/3 max HP
                    if (user.Ability == PAbility.Blaze && user.HP <= user.MaxHP / 3)
                        basePower *= 1.5;
                    // Pokémon with Heatproof take half as much damage from fire attacks
                    if (target.Ability == PAbility.Heatproof)
                        basePower *= 0.5;
                    break;
                case PType.Flying:
                    if (user.Item == PItem.SkyPlate)
                        basePower *= 1.2;
                    break;
                case PType.Ghost:
                    if (user.Item == PItem.SpookyPlate)
                        basePower *= 1.2;
                    break;
                case PType.Grass:
                    if (user.Item == PItem.MeadowPlate)
                        basePower *= 1.2;
                    // Overgrow gives a 50% power boost to Grass attacks if the user is below 1/3 max HP
                    if (user.Ability == PAbility.Overgrow && user.HP <= user.MaxHP / 3)
                        basePower *= 1.5;
                    break;
                case PType.Ground:
                    if (user.Item == PItem.EarthPlate)
                        basePower *= 1.2;
                    break;
                case PType.Ice:
                    if (user.Item == PItem.IciclePlate)
                        basePower *= 1.2;
                    break;
                case PType.Normal:
                    break;
                case PType.Poison:
                    if (user.Item == PItem.ToxicPlate)
                        basePower *= 1.2;
                    break;
                case PType.Psychic:
                    if (user.Item == PItem.MindPlate)
                        basePower *= 1.2;
                    break;
                case PType.Rock:
                    if (user.Item == PItem.StonePlate)
                        basePower *= 1.2;
                    break;
                case PType.Steel:
                    if (user.Item == PItem.IronPlate)
                        basePower *= 1.2;
                    break;
                case PType.Water:
                    if (user.Item == PItem.SplashPlate)
                        basePower *= 1.2;
                    // Torrent gives a 50% power boost to Water attacks if the user is below 1/3 max HP
                    if (user.Ability == PAbility.Torrent && user.HP <= user.MaxHP / 3)
                        basePower *= 1.5;
                    break;
            }

            // If a Pokémon is underground and gets hit by a move that can hit underground targets, power is boosted by 100%
            if (target.Status2.HasFlag(PStatus2.Underground) && PMoveData.Data[move].Flags.HasFlag(PMoveFlag.HitsUnderground))
                basePower *= 2.0;
            // If a Pokémon is underwater and gets hit by a move that can hit underwater targets, power is boosted by 100%
            if (target.Status2.HasFlag(PStatus2.Underwater) && PMoveData.Data[move].Flags.HasFlag(PMoveFlag.HitsUnderwater))
                basePower *= 2.0;

            // Life Orb boosts power but deals damage to the user
            if (!ignoreLifeOrb && user.Item == PItem.LifeOrb)
                basePower = basePower * 5324 / 4096;
            // A Pikachu holding a Light Ball gets a 100% power boost
            if (user.Item == PItem.LightBall && user.Shell.Species == PSpecies.Pikachu)
                basePower *= 2.0;
            // Damage is halved from a Burned Pokémon unless it has Guts
            if (moveCategory == PMoveCategory.Physical && user.Status1 == PStatus1.Burned && user.Ability != PAbility.Guts)
                basePower *= 0.5;
            // Damage is halved when using Fire or Ice moves against a Pokémon with Thick Fat
            if (target.Ability == PAbility.ThickFat && (moveType == PType.Fire || moveType == PType.Ice))
                basePower *= 0.5;

            return (ushort)basePower;
        }
        ushort CalculateAttack(PPokemon user, PPokemon target, bool criticalHit)
        {
            // Negative Attack changes are ignored for critical hits
            double attack = user.Attack * GetStatMultiplier(criticalHit ? Math.Max((sbyte)0, user.AttackChange) : user.AttackChange);

            // Pokemon with Huge Power or Pure Power get a 100% Attack boost
            if (user.Ability == PAbility.HugePower || user.Ability == PAbility.PurePower)
                attack *= 2.0;
            // A Cubone or Marowak holding a Thick Club gets a 100% Attack boost
            if (user.Item == PItem.ThickClub && (user.Shell.Species == PSpecies.Cubone || user.Shell.Species == PSpecies.Marowak))
                attack *= 2.0;
            // A Pokémon with Hustle gets a 50% Attack boost
            if (user.Ability == PAbility.Hustle)
                attack *= 1.5;
            // A Pokémon with Guts gets a 50% Attack boost when afflicted with a status
            if (user.Ability == PAbility.Guts && user.Status1 != PStatus1.None)
                attack *= 1.5;
            // A Pokémon holding a Choice Band gets a 50% Attack boost
            if (user.Item == PItem.ChoiceBand)
                attack *= 1.5;

            return (ushort)attack;
        }
        ushort CalculateDefense(PPokemon user, PPokemon target, bool criticalHit)
        {
            // Positive Defense changes are ignored for critical hits
            double defense = user.Defense * GetStatMultiplier(criticalHit ? Math.Min((sbyte)0, target.DefenseChange) : target.DefenseChange);

            // A Ditto holding a Metal Powder gets a 100% Defense boost
            if (target.Item == PItem.MetalPowder && target.Species == PSpecies.Ditto)
                defense *= 2.0;
            // A Pokémon with Marvel Scale gets a 50% Defense boost when afflicted with a status
            if (target.Ability == PAbility.MarvelScale && target.Status1 != PStatus1.None)
                defense *= 1.5;

            return (ushort)defense;
        }
        ushort CalculateSpAttack(PPokemon user, PPokemon target, bool criticalHit)
        {
            // Negative SpAttack changes are ignored for critical hits
            double spAttack = user.SpAttack * GetStatMultiplier(criticalHit ? Math.Max((sbyte)0, user.SpAttackChange) : user.SpAttackChange);

            // A Clamperl holding a Deep Sea Tooth gets a 100% SpAttack boost
            if (user.Item == PItem.DeepSeaTooth && user.Shell.Species == PSpecies.Clamperl)
                spAttack *= 2.0;
            // A Latios or Latias holding a Soul Dew gets a 50% SpAttack boost
            if (user.Item == PItem.SoulDew && (user.Shell.Species == PSpecies.Latias || user.Shell.Species == PSpecies.Latios))
                spAttack *= 1.5;
            // A Pokémon holding a Choice Specs gets a 50% SpAttack boost
            if (user.Item == PItem.ChoiceSpecs)
                spAttack *= 1.5;

            return (ushort)spAttack;
        }
        ushort CalculateSpDefense(PPokemon user, PPokemon target, bool criticalHit)
        {
            // Positive SpDefense changes are ignored for critical hits
            double spDefense = user.SpDefense * GetStatMultiplier(criticalHit ? Math.Min((sbyte)0, target.SpDefenseChange) : target.SpDefenseChange);

            // A Clamperl holding a Deep Sea Scale gets a 100% SpDefense boost
            if (target.Item == PItem.DeepSeaScale && target.Shell.Species == PSpecies.Clamperl)
                spDefense *= 2.0;
            // A Latios or Latias holding a Soul Dew gets a 50% SpDefense boost
            if (target.Item == PItem.SoulDew && (target.Shell.Species == PSpecies.Latias || target.Shell.Species == PSpecies.Latios))
                spDefense *= 1.5;

            return (ushort)spDefense;
        }

        // If moveCategory is PMoveCategory.MAX, category is determined by the move
        // If power is 0, power is determined by the move
        ushort CalculateDamage(PPokemon user, PPokemon target, PMove move, PType moveType, byte power = 0, PMoveCategory moveCategory = PMoveCategory.MAX, bool ignoreReflectLightScreen = false, bool ignoreLifeOrb = false, bool criticalHit = false)
        {
            if (moveCategory == PMoveCategory.MAX)
                moveCategory = PMoveData.Data[move].Category;
            ushort damage;
            ushort a = 0, d = 0,
                p = CalculateBasePower(user, target, move, moveType, power, moveCategory, ignoreReflectLightScreen, ignoreLifeOrb, criticalHit);

            if (moveCategory == PMoveCategory.Physical)
            {
                a = CalculateAttack(user, target, criticalHit);
                d = CalculateDefense(user, target, criticalHit);
            }
            else if (moveCategory == PMoveCategory.Special)
            {
                a = CalculateSpAttack(user, target, criticalHit);
                d = CalculateSpDefense(user, target, criticalHit);
            }

            damage = (ushort)(2 * user.Shell.Level / 5 + 2);
            damage = (ushort)(damage * a * p / d);
            damage /= 50;
            return (ushort)(damage + 2);
        }
    }
}
