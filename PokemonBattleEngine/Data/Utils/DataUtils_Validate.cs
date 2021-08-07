using System;

namespace Kermalis.PokemonBattleEngine.Data.Utils
{
    public static partial class PBEDataUtils
    {
        public static void ValidateSpecies(PBESpecies species, PBEForm form, bool requireUsableOutsideOfBattle)
        {
            if (!IsValidForm(species, form, requireUsableOutsideOfBattle))
            {
                throw new ArgumentOutOfRangeException(nameof(form));
            }
        }
        public static void ValidateNickname(string value, PBESettings settings)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (value.Length > settings.MaxPokemonNameLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} cannot have more than {nameof(settings.MaxPokemonNameLength)} ({settings.MaxPokemonNameLength}) characters.");
            }
        }
        public static void ValidateLevel(byte value, PBESettings settings)
        {
            if (value < settings.MinLevel || value > settings.MaxLevel)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} must be at least {nameof(settings.MinLevel)} ({settings.MinLevel}) and cannot exceed {nameof(settings.MaxLevel)} ({settings.MaxLevel}).");
            }
        }
        public static void ValidateEXP(PBEGrowthRate type, uint value, byte level)
        {
            uint requiredForLevel = PBEDataProvider.Instance.GetEXPRequired(type, level);
            if (value < requiredForLevel)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (level < 100)
            {
                uint requiredForNextLevel = PBEDataProvider.Instance.GetEXPRequired(type, (byte)(level + 1));
                if (value >= requiredForNextLevel)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }
        public static void ValidateAbility(PBEAlphabeticalList<PBEAbility> valid, PBEAbility value)
        {
            if (!valid.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        public static void ValidateNature(PBENature value)
        {
            if (!AllNatures.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        public static void ValidateGender(PBEAlphabeticalList<PBEGender> valid, PBEGender value)
        {
            if (!valid.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        public static void ValidateItem(PBEAlphabeticalList<PBEItem> valid, PBEItem value)
        {
            if (!valid.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        public static void ValidateCaughtBall(PBEItem value)
        {
            if (!AllBalls.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}
