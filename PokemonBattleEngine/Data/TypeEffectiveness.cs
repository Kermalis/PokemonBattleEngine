using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Data
{
    public static class PBETypeEffectiveness
    {
        #region Static Collections
        /// <summary>The type effectiveness table. The first key is the attacking type and the second key is the defending type.</summary>
        private static readonly Dictionary<PBEType, Dictionary<PBEType, double>> _table = new Dictionary<PBEType, Dictionary<PBEType, double>>
        {
            { PBEType.None, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 1.0 },
                { PBEType.Water, 1.0 },
            }
            },
            { PBEType.Bug, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 2.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 0.5 },
                { PBEType.Fire, 0.5 },
                { PBEType.Flying, 0.5 },
                { PBEType.Ghost, 0.5 },
                { PBEType.Grass, 2.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 0.5 },
                { PBEType.Psychic, 2.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            }
            },
            { PBEType.Dark, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 0.5 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 0.5 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 2.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 2.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            }
            },
            { PBEType.Dragon, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 2.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            }
            },
            { PBEType.Electric, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 0.5 },
                { PBEType.Electric, 0.5 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 2.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 0.5 },
                { PBEType.Ground, 0.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 1.0 },
                { PBEType.Water, 2.0 },
            }
            },
            { PBEType.Fighting, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 0.5 },
                { PBEType.Dark, 2.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 0.5 },
                { PBEType.Ghost, 0.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 2.0 },
                { PBEType.Normal, 2.0 },
                { PBEType.Poison, 0.5 },
                { PBEType.Psychic, 0.5 },
                { PBEType.Rock, 2.0 },
                { PBEType.Steel, 2.0 },
                { PBEType.Water, 1.0 },
            }
            },
            { PBEType.Fire, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 2.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 0.5 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 0.5 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 2.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 2.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 0.5 },
                { PBEType.Steel, 2.0 },
                { PBEType.Water, 0.5 },
            }
            },
            { PBEType.Flying, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 2.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 0.5 },
                { PBEType.Fighting, 2.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 2.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 0.5 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            }
            },
            { PBEType.Ghost, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 0.5 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 2.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 0.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 2.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            }
            },
            { PBEType.Grass, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 0.5 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 0.5 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 0.5 },
                { PBEType.Flying, 0.5 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 0.5 },
                { PBEType.Ground, 2.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 0.5 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 2.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 2.0 },
            }
            },
            { PBEType.Ground, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 0.5 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 2.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 2.0 },
                { PBEType.Flying, 0.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 0.5 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 2.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 2.0 },
                { PBEType.Steel, 2.0 },
                { PBEType.Water, 1.0 },
            }
            },
            { PBEType.Ice, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 2.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 0.5 },
                { PBEType.Flying, 2.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 2.0 },
                { PBEType.Ground, 2.0 },
                { PBEType.Ice, 0.5 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 0.5 },
            }
            },
            { PBEType.Normal, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 0.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 0.5 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            }
            },
            { PBEType.Poison, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 0.5 },
                { PBEType.Grass, 2.0 },
                { PBEType.Ground, 0.5 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 0.5 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 0.5 },
                { PBEType.Steel, 0.0 },
                { PBEType.Water, 1.0 },
            }
            },
            { PBEType.Psychic, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 0.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 2.0 },
                { PBEType.Fire, 1.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 2.0 },
                { PBEType.Psychic, 0.5 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            }
            },
            { PBEType.Rock, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 2.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 0.5 },
                { PBEType.Fire, 2.0 },
                { PBEType.Flying, 2.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 0.5 },
                { PBEType.Ice, 2.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 1.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 1.0 },
            }
            },
            { PBEType.Steel, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 1.0 },
                { PBEType.Electric, 0.5 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 0.5 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 1.0 },
                { PBEType.Ground, 1.0 },
                { PBEType.Ice, 2.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 2.0 },
                { PBEType.Steel, 0.5 },
                { PBEType.Water, 0.5 },
            }
            },
            { PBEType.Water, new Dictionary<PBEType, double>
            {
                { PBEType.None, 1.0 },
                { PBEType.Bug, 1.0 },
                { PBEType.Dark, 1.0 },
                { PBEType.Dragon, 0.5 },
                { PBEType.Electric, 1.0 },
                { PBEType.Fighting, 1.0 },
                { PBEType.Fire, 2.0 },
                { PBEType.Flying, 1.0 },
                { PBEType.Ghost, 1.0 },
                { PBEType.Grass, 0.5 },
                { PBEType.Ground, 2.0 },
                { PBEType.Ice, 1.0 },
                { PBEType.Normal, 1.0 },
                { PBEType.Poison, 1.0 },
                { PBEType.Psychic, 1.0 },
                { PBEType.Rock, 2.0 },
                { PBEType.Steel, 1.0 },
                { PBEType.Water, 0.5 },
            }
            }
        };
        #endregion

        public static PBEResult IsAffectedByAttack(PBEPokemon user, PBEPokemon target, PBEType moveType, out double damageMultiplier, bool useKnownInfo = false)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            if (moveType >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(moveType));
            }

            PBEResult result;
            if (moveType == PBEType.Ground)
            {
                result = target.IsGrounded(user, useKnownInfo: useKnownInfo);
                if (result != PBEResult.Success)
                {
                    damageMultiplier = 0;
                    return result;
                }
            }
            bool ignoreGhost = user.Ability == PBEAbility.Scrappy || target.Status2.HasFlag(PBEStatus2.Identified),
                ignoreDark = target.Status2.HasFlag(PBEStatus2.MiracleEye);
            damageMultiplier = GetEffectiveness(moveType, useKnownInfo ? target.KnownType1 : target.Type1, useKnownInfo ? target.KnownType2 : target.Type2, ignoreGhost: ignoreGhost, ignoreDark: ignoreDark);
            if (damageMultiplier <= 0) // (-infinity, 0]
            {
                damageMultiplier = 0;
                return PBEResult.Ineffective_Type;
            }
            else if (damageMultiplier < 1) // (0, 1)
            {
                result = PBEResult.NotVeryEffective_Type;
            }
            else if (damageMultiplier == 1) // [1, 1]
            {
                result = PBEResult.Success;
            }
            else // (1, infinity)
            {
                return PBEResult.SuperEffective_Type;
            }
            PBEAbility kAbility = useKnownInfo ? target.KnownAbility : target.Ability;
            if (!user.HasCancellingAbility() && kAbility == PBEAbility.WonderGuard)
            {
                damageMultiplier = 0;
                result = PBEResult.Ineffective_Ability;
            }
            return result;
        }
        /// <summary>Checks if <see cref="PBEMove.ThunderWave"/>'s type affects the target, taking into account <see cref="PBEAbility.Normalize"/>.</summary>
        public static PBEResult ThunderWaveTypeCheck(PBEPokemon user, PBEPokemon target, bool useKnownInfo = false)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            PBEType moveType = user.GetMoveType(PBEMove.ThunderWave);
            double d = GetEffectiveness(moveType, useKnownInfo ? target.KnownType1 : target.Type1, useKnownInfo ? target.KnownType2 : target.Type2);
            if (d <= 0)
            {
                return PBEResult.Ineffective_Type;
            }
            return PBEResult.Success;
        }

        public static double GetEffectiveness(PBEType attackingType, PBEType defendingType, bool ignoreGhost = false, bool ignoreDark = false)
        {
            if (attackingType >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(attackingType));
            }
            if (defendingType >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(defendingType));
            }

            double d = _table[attackingType][defendingType];
            if (d <= 0 && ((ignoreGhost && defendingType == PBEType.Ghost) || (ignoreDark && defendingType == PBEType.Dark)))
            {
                return 1;
            }
            return d;
        }
        public static double GetEffectiveness(PBEType attackingType, PBEType defendingType1, PBEType defendingType2, bool ignoreGhost = false, bool ignoreDark = false)
        {
            double d = GetEffectiveness(attackingType, defendingType1, ignoreGhost: ignoreGhost, ignoreDark: ignoreDark);
            d *= GetEffectiveness(attackingType, defendingType2, ignoreGhost: ignoreGhost, ignoreDark: ignoreDark);
            return d;
        }
        public static double GetEffectiveness(PBEType attackingType, IPBEPokemonTypes defendingTypes, bool ignoreGhost = false, bool ignoreDark = false)
        {
            if (defendingTypes == null)
            {
                throw new ArgumentNullException(nameof(defendingTypes));
            }
            return GetEffectiveness(attackingType, defendingTypes.Type1, defendingTypes.Type2, ignoreGhost: ignoreGhost, ignoreDark: ignoreDark);
        }
        public static double GetStealthRockMultiplier(PBEType type1, PBEType type2)
        {
            if (type1 >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(type1));
            }
            if (type2 >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(type2));
            }
            double d = 0.125;
            d *= _table[PBEType.Rock][type1];
            d *= _table[PBEType.Rock][type2];
            return d;
        }
    }
}
