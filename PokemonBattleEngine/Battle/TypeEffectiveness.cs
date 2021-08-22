using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public static class PBETypeEffectiveness
    {
        #region Static Collections
        /// <summary>The type effectiveness table. The first key is the attacking type and the second key is the defending type.</summary>
        private static readonly Dictionary<PBEType, Dictionary<PBEType, float>> _table = new()
        {
            {
                PBEType.None,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 1.0f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 1.0f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 1.0f },
                { PBEType.Flying, 1.0f },
                { PBEType.Ghost, 1.0f },
                { PBEType.Grass, 1.0f },
                { PBEType.Ground, 1.0f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 1.0f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 1.0f },
                { PBEType.Steel, 1.0f },
                { PBEType.Water, 1.0f },
            }
            },
            {
                PBEType.Bug,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 1.0f },
                { PBEType.Dark, 2.0f },
                { PBEType.Dragon, 1.0f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 0.5f },
                { PBEType.Fire, 0.5f },
                { PBEType.Flying, 0.5f },
                { PBEType.Ghost, 0.5f },
                { PBEType.Grass, 2.0f },
                { PBEType.Ground, 1.0f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 0.5f },
                { PBEType.Psychic, 2.0f },
                { PBEType.Rock, 1.0f },
                { PBEType.Steel, 0.5f },
                { PBEType.Water, 1.0f },
            }
            },
            {
                PBEType.Dark,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 1.0f },
                { PBEType.Dark, 0.5f },
                { PBEType.Dragon, 1.0f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 0.5f },
                { PBEType.Fire, 1.0f },
                { PBEType.Flying, 1.0f },
                { PBEType.Ghost, 2.0f },
                { PBEType.Grass, 1.0f },
                { PBEType.Ground, 1.0f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 1.0f },
                { PBEType.Psychic, 2.0f },
                { PBEType.Rock, 1.0f },
                { PBEType.Steel, 0.5f },
                { PBEType.Water, 1.0f },
            }
            },
            {
                PBEType.Dragon,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 1.0f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 2.0f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 1.0f },
                { PBEType.Flying, 1.0f },
                { PBEType.Ghost, 1.0f },
                { PBEType.Grass, 1.0f },
                { PBEType.Ground, 1.0f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 1.0f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 1.0f },
                { PBEType.Steel, 0.5f },
                { PBEType.Water, 1.0f },
            }
            },
            {
                PBEType.Electric,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 1.0f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 0.5f },
                { PBEType.Electric, 0.5f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 1.0f },
                { PBEType.Flying, 2.0f },
                { PBEType.Ghost, 1.0f },
                { PBEType.Grass, 0.5f },
                { PBEType.Ground, 0.0f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 1.0f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 1.0f },
                { PBEType.Steel, 1.0f },
                { PBEType.Water, 2.0f },
            }
            },
            {
                PBEType.Fighting,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 0.5f },
                { PBEType.Dark, 2.0f },
                { PBEType.Dragon, 1.0f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 1.0f },
                { PBEType.Flying, 0.5f },
                { PBEType.Ghost, 0.0f },
                { PBEType.Grass, 1.0f },
                { PBEType.Ground, 1.0f },
                { PBEType.Ice, 2.0f },
                { PBEType.Normal, 2.0f },
                { PBEType.Poison, 0.5f },
                { PBEType.Psychic, 0.5f },
                { PBEType.Rock, 2.0f },
                { PBEType.Steel, 2.0f },
                { PBEType.Water, 1.0f },
            }
            },
            {
                PBEType.Fire,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 2.0f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 0.5f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 0.5f },
                { PBEType.Flying, 1.0f },
                { PBEType.Ghost, 1.0f },
                { PBEType.Grass, 2.0f },
                { PBEType.Ground, 1.0f },
                { PBEType.Ice, 2.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 1.0f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 0.5f },
                { PBEType.Steel, 2.0f },
                { PBEType.Water, 0.5f },
            }
            },
            {
                PBEType.Flying,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 2.0f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 1.0f },
                { PBEType.Electric, 0.5f },
                { PBEType.Fighting, 2.0f },
                { PBEType.Fire, 1.0f },
                { PBEType.Flying, 1.0f },
                { PBEType.Ghost, 1.0f },
                { PBEType.Grass, 2.0f },
                { PBEType.Ground, 1.0f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 1.0f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 0.5f },
                { PBEType.Steel, 0.5f },
                { PBEType.Water, 1.0f },
            }
            },
            {
                PBEType.Ghost,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 1.0f },
                { PBEType.Dark, 0.5f },
                { PBEType.Dragon, 1.0f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 1.0f },
                { PBEType.Flying, 1.0f },
                { PBEType.Ghost, 2.0f },
                { PBEType.Grass, 1.0f },
                { PBEType.Ground, 1.0f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 0.0f },
                { PBEType.Poison, 1.0f },
                { PBEType.Psychic, 2.0f },
                { PBEType.Rock, 1.0f },
                { PBEType.Steel, 0.5f },
                { PBEType.Water, 1.0f },
            }
            },
            {
                PBEType.Grass,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 0.5f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 0.5f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 0.5f },
                { PBEType.Flying, 0.5f },
                { PBEType.Ghost, 1.0f },
                { PBEType.Grass, 0.5f },
                { PBEType.Ground, 2.0f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 0.5f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 2.0f },
                { PBEType.Steel, 0.5f },
                { PBEType.Water, 2.0f },
            }
            },
            {
                PBEType.Ground,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 0.5f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 1.0f },
                { PBEType.Electric, 2.0f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 2.0f },
                { PBEType.Flying, 0.0f },
                { PBEType.Ghost, 1.0f },
                { PBEType.Grass, 0.5f },
                { PBEType.Ground, 1.0f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 2.0f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 2.0f },
                { PBEType.Steel, 2.0f },
                { PBEType.Water, 1.0f },
            }
            },
            {
                PBEType.Ice,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 1.0f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 2.0f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 0.5f },
                { PBEType.Flying, 2.0f },
                { PBEType.Ghost, 1.0f },
                { PBEType.Grass, 2.0f },
                { PBEType.Ground, 2.0f },
                { PBEType.Ice, 0.5f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 1.0f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 1.0f },
                { PBEType.Steel, 0.5f },
                { PBEType.Water, 0.5f },
            }
            },
            {
                PBEType.Normal,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 1.0f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 1.0f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 1.0f },
                { PBEType.Flying, 1.0f },
                { PBEType.Ghost, 0.0f },
                { PBEType.Grass, 1.0f },
                { PBEType.Ground, 1.0f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 1.0f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 0.5f },
                { PBEType.Steel, 0.5f },
                { PBEType.Water, 1.0f },
            }
            },
            {
                PBEType.Poison,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 1.0f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 1.0f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 1.0f },
                { PBEType.Flying, 1.0f },
                { PBEType.Ghost, 0.5f },
                { PBEType.Grass, 2.0f },
                { PBEType.Ground, 0.5f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 0.5f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 0.5f },
                { PBEType.Steel, 0.0f },
                { PBEType.Water, 1.0f },
            }
            },
            {
                PBEType.Psychic,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 1.0f },
                { PBEType.Dark, 0.0f },
                { PBEType.Dragon, 1.0f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 2.0f },
                { PBEType.Fire, 1.0f },
                { PBEType.Flying, 1.0f },
                { PBEType.Ghost, 1.0f },
                { PBEType.Grass, 1.0f },
                { PBEType.Ground, 1.0f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 2.0f },
                { PBEType.Psychic, 0.5f },
                { PBEType.Rock, 1.0f },
                { PBEType.Steel, 0.5f },
                { PBEType.Water, 1.0f },
            }
            },
            {
                PBEType.Rock,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 2.0f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 1.0f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 0.5f },
                { PBEType.Fire, 2.0f },
                { PBEType.Flying, 2.0f },
                { PBEType.Ghost, 1.0f },
                { PBEType.Grass, 1.0f },
                { PBEType.Ground, 0.5f },
                { PBEType.Ice, 2.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 1.0f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 1.0f },
                { PBEType.Steel, 0.5f },
                { PBEType.Water, 1.0f },
            }
            },
            {
                PBEType.Steel,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 1.0f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 1.0f },
                { PBEType.Electric, 0.5f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 0.5f },
                { PBEType.Flying, 1.0f },
                { PBEType.Ghost, 1.0f },
                { PBEType.Grass, 1.0f },
                { PBEType.Ground, 1.0f },
                { PBEType.Ice, 2.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 1.0f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 2.0f },
                { PBEType.Steel, 0.5f },
                { PBEType.Water, 0.5f },
            }
            },
            {
                PBEType.Water,
                new Dictionary<PBEType, float>
            {
                { PBEType.None, 1.0f },
                { PBEType.Bug, 1.0f },
                { PBEType.Dark, 1.0f },
                { PBEType.Dragon, 0.5f },
                { PBEType.Electric, 1.0f },
                { PBEType.Fighting, 1.0f },
                { PBEType.Fire, 2.0f },
                { PBEType.Flying, 1.0f },
                { PBEType.Ghost, 1.0f },
                { PBEType.Grass, 0.5f },
                { PBEType.Ground, 2.0f },
                { PBEType.Ice, 1.0f },
                { PBEType.Normal, 1.0f },
                { PBEType.Poison, 1.0f },
                { PBEType.Psychic, 1.0f },
                { PBEType.Rock, 2.0f },
                { PBEType.Steel, 1.0f },
                { PBEType.Water, 0.5f },
            }
            }
        };
        #endregion

        public static PBEResult IsAffectedByAttack(PBEBattlePokemon user, PBEBattlePokemon target, PBEType moveType, out float damageMultiplier, bool useKnownInfo = false)
        {
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
            damageMultiplier = GetEffectiveness(moveType, target, useKnownInfo, ignoreGhost: ignoreGhost, ignoreDark: ignoreDark);
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
            if (kAbility == PBEAbility.WonderGuard && !user.HasCancellingAbility())
            {
                damageMultiplier = 0;
                result = PBEResult.Ineffective_Ability;
            }
            return result;
        }
        /// <summary>Checks if <see cref="PBEMoveEffect.ThunderWave"/>'s type affects the target, taking into account <see cref="PBEAbility.Normalize"/>.</summary>
        public static PBEResult ThunderWaveTypeCheck(PBEBattlePokemon user, PBEBattlePokemon target, PBEMove move, bool useKnownInfo = false)
        {
            PBEType moveType = user.GetMoveType(move);
            float d = GetEffectiveness(moveType, target, useKnownInfo);
            if (d <= 0)
            {
                return PBEResult.Ineffective_Type;
            }
            return PBEResult.Success;
        }

        public static float GetEffectiveness(PBEType attackingType, PBEType defendingType, bool ignoreGhost = false, bool ignoreDark = false)
        {
            if (attackingType >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(attackingType));
            }
            if (defendingType >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(defendingType));
            }

            float d = _table[attackingType][defendingType];
            if (d <= 0 && ((ignoreGhost && defendingType == PBEType.Ghost) || (ignoreDark && defendingType == PBEType.Dark)))
            {
                return 1;
            }
            return d;
        }
        public static float GetEffectiveness(PBEType attackingType, PBEType defendingType1, PBEType defendingType2, bool ignoreGhost = false, bool ignoreDark = false)
        {
            float d = GetEffectiveness(attackingType, defendingType1, ignoreGhost: ignoreGhost, ignoreDark: ignoreDark);
            d *= GetEffectiveness(attackingType, defendingType2, ignoreGhost: ignoreGhost, ignoreDark: ignoreDark);
            return d;
        }
        public static float GetEffectiveness(PBEType attackingType, IPBEPokemonTypes defendingTypes, bool ignoreGhost = false, bool ignoreDark = false)
        {
            return GetEffectiveness(attackingType, defendingTypes.Type1, defendingTypes.Type2, ignoreGhost: ignoreGhost, ignoreDark: ignoreDark);
        }
        public static float GetEffectiveness_Known(PBEType attackingType, IPBEPokemonKnownTypes defendingTypes, bool ignoreGhost = false, bool ignoreDark = false)
        {
            return GetEffectiveness(attackingType, defendingTypes.KnownType1, defendingTypes.KnownType2, ignoreGhost: ignoreGhost, ignoreDark: ignoreDark);
        }
        public static float GetEffectiveness<T>(PBEType attackingType, T defendingTypes, bool useKnownInfo, bool ignoreGhost = false, bool ignoreDark = false)
            where T : IPBEPokemonTypes, IPBEPokemonKnownTypes
        {
            return GetEffectiveness(attackingType, useKnownInfo ? defendingTypes.KnownType1 : defendingTypes.Type1, useKnownInfo ? defendingTypes.KnownType2 : defendingTypes.Type2, ignoreGhost: ignoreGhost, ignoreDark: ignoreDark);
        }
        public static float GetStealthRockMultiplier(PBEType type1, PBEType type2)
        {
            if (type1 >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(type1));
            }
            if (type2 >= PBEType.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(type2));
            }
            float d = 0.125f;
            d *= _table[PBEType.Rock][type1];
            d *= _table[PBEType.Rock][type2];
            return d;
        }
    }
}
