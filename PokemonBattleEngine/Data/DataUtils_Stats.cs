using Kermalis.PokemonBattleEngine.Data.Legality;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public static partial class PBEDataUtils
    {
        #region Static Collections
        public static PBEAlphabeticalList<PBENature> AllNatures { get; } = new PBEAlphabeticalList<PBENature>(Enum.GetValues(typeof(PBENature)).Cast<PBENature>().Except(new[] { PBENature.MAX }));
        public static PBEAlphabeticalList<PBESpecies> MoonStoneSpecies { get; } = new PBEAlphabeticalList<PBESpecies>(new[] { PBESpecies.Nidoran_F, PBESpecies.Nidorina, PBESpecies.Nidoqueen,
            PBESpecies.Nidoran_M, PBESpecies.Nidorino, PBESpecies.Nidoking, PBESpecies.Cleffa, PBESpecies.Clefairy, PBESpecies.Clefable, PBESpecies.Igglybuff, PBESpecies.Jigglypuff,
            PBESpecies.Wigglytuff, PBESpecies.Skitty, PBESpecies.Delcatty, PBESpecies.Munna, PBESpecies.Musharna });
        private static readonly Dictionary<PBENature, (PBEFlavor Favored, PBEFlavor Disliked)> _natureBoosts = new Dictionary<PBENature, (PBEFlavor Favored, PBEFlavor Disliked)>
        {
            { PBENature.Adamant, (PBEFlavor.Spicy, PBEFlavor.Dry) },
            { PBENature.Bold, (PBEFlavor.Sour, PBEFlavor.Spicy) },
            { PBENature.Brave, (PBEFlavor.Spicy, PBEFlavor.Sweet) },
            { PBENature.Calm, (PBEFlavor.Bitter, PBEFlavor.Spicy) },
            { PBENature.Careful, (PBEFlavor.Bitter, PBEFlavor.Dry) },
            { PBENature.Gentle, (PBEFlavor.Bitter, PBEFlavor.Sour) },
            { PBENature.Hasty, (PBEFlavor.Sweet, PBEFlavor.Sour) },
            { PBENature.Impish, (PBEFlavor.Sour, PBEFlavor.Dry) },
            { PBENature.Jolly, (PBEFlavor.Sweet, PBEFlavor.Dry) },
            { PBENature.Lax, (PBEFlavor.Sour, PBEFlavor.Bitter) },
            { PBENature.Lonely, (PBEFlavor.Spicy, PBEFlavor.Sour) },
            { PBENature.Mild, (PBEFlavor.Dry, PBEFlavor.Sour) },
            { PBENature.Modest, (PBEFlavor.Dry, PBEFlavor.Spicy) },
            { PBENature.Naive, (PBEFlavor.Sweet, PBEFlavor.Bitter) },
            { PBENature.Naughty, (PBEFlavor.Spicy, PBEFlavor.Bitter) },
            { PBENature.Quiet, (PBEFlavor.Dry, PBEFlavor.Sweet) },
            { PBENature.Rash, (PBEFlavor.Dry, PBEFlavor.Bitter) },
            { PBENature.Relaxed, (PBEFlavor.Sour, PBEFlavor.Sweet) },
            { PBENature.Sassy, (PBEFlavor.Bitter, PBEFlavor.Sweet) },
            { PBENature.Timid, (PBEFlavor.Sweet, PBEFlavor.Spicy) }
        };
        private static readonly PBEType[] _hiddenPowerTypes = new PBEType[]
        {
            PBEType.Fighting, // 7.8125 %
            PBEType.Flying,   // 6.2500 %
            PBEType.Poison,   // 6.2500 %
            PBEType.Ground,   // 6.2500 %
            PBEType.Rock,     // 6.2500 %
            PBEType.Bug,      // 7.8125 %
            PBEType.Ghost,    // 6.2500 %
            PBEType.Steel,    // 6.2500 %
            PBEType.Fire,     // 6.2500 %
            PBEType.Water,    // 6.2500 %
            PBEType.Grass,    // 7.8125 %
            PBEType.Electric, // 6.2500 %
            PBEType.Psychic,  // 6.2500 %
            PBEType.Ice,      // 6.2500 %
            PBEType.Dragon,   // 6.2500 %
            PBEType.Dark      // 1.5625 %
        };
        #region Genders
        private static readonly PBEAlphabeticalList<PBEGender> _genderless = new PBEAlphabeticalList<PBEGender>(new[] { PBEGender.Genderless });
        private static readonly PBEAlphabeticalList<PBEGender> _male = new PBEAlphabeticalList<PBEGender>(new[] { PBEGender.Male });
        private static readonly PBEAlphabeticalList<PBEGender> _female = new PBEAlphabeticalList<PBEGender>(new[] { PBEGender.Female });
        private static readonly PBEAlphabeticalList<PBEGender> _maleFemale = new PBEAlphabeticalList<PBEGender>(new[] { PBEGender.Male, PBEGender.Female });
        #endregion
        #endregion

        public static sbyte GetRelationshipToFlavor(this PBENature nature, PBEFlavor flavor)
        {
            if (nature >= PBENature.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(nature));
            }
            if (flavor >= PBEFlavor.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(flavor));
            }

            if (_natureBoosts.TryGetValue(nature, out (PBEFlavor Favored, PBEFlavor Disliked) t))
            {
                if (t.Favored == flavor)
                {
                    return 1;
                }
                else if (t.Disliked == flavor)
                {
                    return -1;
                }
            }
            return 0;
        }
        public static sbyte GetRelationshipToStat(this PBENature nature, PBEStat stat)
        {
            if (nature >= PBENature.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(nature));
            }
            if (stat < PBEStat.Attack || stat > PBEStat.Speed)
            {
                throw new ArgumentOutOfRangeException(nameof(stat));
            }

            return nature.GetRelationshipToFlavor((PBEFlavor)(stat - 1));
        }
        public static PBEFlavor? GetLikedFlavor(this PBENature nature)
        {
            if (nature >= PBENature.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(nature));
            }
            if (_natureBoosts.TryGetValue(nature, out (PBEFlavor Favored, PBEFlavor Disliked) t))
            {
                return t.Favored;
            }
            return null;
        }
        public static PBEFlavor? GetDislikedFlavor(this PBENature nature)
        {
            if (nature >= PBENature.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(nature));
            }
            if (_natureBoosts.TryGetValue(nature, out (PBEFlavor Favored, PBEFlavor Disliked) t))
            {
                return t.Disliked;
            }
            return null;
        }
        public static PBEStat? GetLikedStat(this PBENature nature)
        {
            if (nature >= PBENature.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(nature));
            }
            if (_natureBoosts.TryGetValue(nature, out (PBEFlavor Favored, PBEFlavor Disliked) t))
            {
                return (PBEStat)(t.Favored + 1);
            }
            return null;
        }
        public static PBEStat? GetDislikedStat(this PBENature nature)
        {
            if (nature >= PBENature.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(nature));
            }
            if (_natureBoosts.TryGetValue(nature, out (PBEFlavor Favored, PBEFlavor Disliked) t))
            {
                return (PBEStat)(t.Disliked + 1);
            }
            return null;
        }
        public static bool IsNeutralNature(this PBENature nature)
        {
            if (nature >= PBENature.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(nature));
            }
            return !_natureBoosts.ContainsKey(nature);
        }

        public static int CalcMaxPP(byte ppTier, byte ppUps, PBESettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            return Math.Max(1, (ppTier * settings.PPMultiplier) + (ppTier * ppUps));
        }
        public static int CalcMaxPP(PBEMove move, byte ppUps, PBESettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            if (move >= PBEMove.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            if (move != PBEMove.None)
            {
                return CalcMaxPP(PBEDataProvider.Instance.GetMoveData(move).PPTier, ppUps, settings);
            }
            else
            {
                return 0;
            }
        }

        private static ushort CalcHP(PBESpecies species, IPBEReadOnlyStatCollection baseStats, byte evs, byte ivs, byte level)
        {
            return (ushort)(species == PBESpecies.Shedinja ? 1 : ((((2 * baseStats.HP) + ivs + (evs / 4)) * level / 100) + level + 10));
        }
        private static ushort CalcOtherStat(IPBEReadOnlyStatCollection baseStats, PBEStat stat, sbyte statRelationship, byte evs, byte ivs, byte level, PBESettings settings)
        {
            double natureMultiplier = 1 + (statRelationship * settings.NatureStatBoost);
            return (ushort)(((((2 * baseStats.GetStat(stat)) + ivs + (evs / 4)) * level / 100) + 5) * natureMultiplier);
        }
        public static ushort CalculateStat(PBESpecies species, IPBEReadOnlyStatCollection baseStats, PBEStat stat, PBENature nature, byte evs, byte ivs, byte level, PBESettings settings)
        {
            if (baseStats == null)
            {
                throw new ArgumentNullException(nameof(baseStats));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            PBELegalityChecker.ValidateLevel(level, settings);
            if (ivs > settings.MaxIVs)
            {
                throw new ArgumentOutOfRangeException(nameof(ivs));
            }

            switch (stat)
            {
                case PBEStat.HP:
                {
                    return CalcHP(species, baseStats, evs, ivs, level);
                }
                case PBEStat.Attack:
                case PBEStat.Defense:
                case PBEStat.SpAttack:
                case PBEStat.SpDefense:
                case PBEStat.Speed:
                {
                    return CalcOtherStat(baseStats, stat, nature.GetRelationshipToStat(stat), evs, ivs, level, settings);
                }
                default: throw new ArgumentOutOfRangeException(nameof(stat));
            }
        }
        public static ushort CalculateStat(IPBEPokemonData pData, PBEStat stat, PBENature nature, byte evs, byte ivs, byte level, PBESettings settings)
        {
            if (pData == null)
            {
                throw new ArgumentNullException(nameof(pData));
            }
            return CalculateStat(pData.Species, pData.BaseStats, stat, nature, evs, ivs, level, settings);
        }
        public static ushort CalculateStat(IPBESpeciesForm pkmn, PBEStat stat, PBENature nature, byte evs, byte ivs, byte level, PBESettings settings)
        {
            return CalculateStat(pkmn.Species, pkmn.Form, stat, nature, evs, ivs, level, settings);
        }
        public static ushort CalculateStat(PBESpecies species, PBEForm form, PBEStat stat, PBENature nature, byte evs, byte ivs, byte level, PBESettings settings)
        {
            PBELegalityChecker.ValidateSpecies(species, form, false);
            return CalculateStat(PBEDataProvider.Instance.GetPokemonData(species, form), stat, nature, evs, ivs, level, settings);
        }
        public static void GetStatRange(PBESpecies species, IPBEReadOnlyStatCollection baseStats, PBEStat stat, byte level, PBESettings settings, out ushort low, out ushort high)
        {
            if (baseStats == null)
            {
                throw new ArgumentNullException(nameof(baseStats));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            PBELegalityChecker.ValidateLevel(level, settings);
            switch (stat)
            {
                case PBEStat.HP:
                {
                    low = CalcHP(species, baseStats, 0, 0, level);
                    high = CalcHP(species, baseStats, byte.MaxValue, settings.MaxIVs, level);
                    break;
                }
                case PBEStat.Attack:
                case PBEStat.Defense:
                case PBEStat.SpAttack:
                case PBEStat.SpDefense:
                case PBEStat.Speed:
                {
                    low = CalcOtherStat(baseStats, stat, -1, 0, 0, level, settings);
                    high = CalcOtherStat(baseStats, stat, +1, byte.MaxValue, settings.MaxIVs, level, settings);
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(stat));
            }
        }
        public static void GetStatRange(IPBEPokemonData pData, PBEStat stat, byte level, PBESettings settings, out ushort low, out ushort high)
        {
            if (pData == null)
            {
                throw new ArgumentNullException(nameof(pData));
            }
            GetStatRange(pData.Species, pData.BaseStats, stat, level, settings, out low, out high);
        }
        public static void GetStatRange(IPBESpeciesForm pkmn, PBEStat stat, byte level, PBESettings settings, out ushort low, out ushort high)
        {
            GetStatRange(pkmn.Species, pkmn.Form, stat, level, settings, out low, out high);
        }
        public static void GetStatRange(PBESpecies species, PBEForm form, PBEStat stat, byte level, PBESettings settings, out ushort low, out ushort high)
        {
            PBELegalityChecker.ValidateSpecies(species, form, false);
            GetStatRange(PBEDataProvider.Instance.GetPokemonData(species, form), stat, level, settings, out low, out high);
        }

        public static PBEType GetHiddenPowerType(byte hpIV, byte attackIV, byte defenseIV, byte spAttackIV, byte spDefenseIV, byte speedIV)
        {
            int a = hpIV & 1,
                b = attackIV & 1,
                c = defenseIV & 1,
                d = speedIV & 1,
                e = spAttackIV & 1,
                f = spDefenseIV & 1;
            return _hiddenPowerTypes[(((1 << 0) * a) + ((1 << 1) * b) + ((1 << 2) * c) + ((1 << 3) * d) + ((1 << 4) * e) + ((1 << 5) * f)) * (_hiddenPowerTypes.Length - 1) / ((1 << 6) - 1)];
        }
        public static byte GetHiddenPowerBasePower(byte hpIV, byte attackIV, byte defenseIV, byte spAttackIV, byte spDefenseIV, byte speedIV, PBESettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            int a = (hpIV & 2) == 2 ? 1 : 0,
                b = (attackIV & 2) == 2 ? 1 : 0,
                c = (defenseIV & 2) == 2 ? 1 : 0,
                d = (speedIV & 2) == 2 ? 1 : 0,
                e = (spAttackIV & 2) == 2 ? 1 : 0,
                f = (spDefenseIV & 2) == 2 ? 1 : 0;
            byte mininumBasePower = settings.HiddenPowerMin,
                maximumBasePower = settings.HiddenPowerMax;
            return (byte)(((((1 << 0) * a) + ((1 << 1) * b) + ((1 << 2) * c) + ((1 << 3) * d) + ((1 << 4) * e) + ((1 << 5) * f)) * (maximumBasePower - mininumBasePower) / ((1 << 6) - 1)) + mininumBasePower);
        }

        public static IReadOnlyList<PBEGender> GetValidGenders(PBEGenderRatio genderRatio)
        {
            switch (genderRatio)
            {
                case PBEGenderRatio.M0_F0: return _genderless;
                case PBEGenderRatio.M1_F0: return _male;
                case PBEGenderRatio.M0_F1: return _female;
                default: return _maleFemale;
            }
        }
    }
}
