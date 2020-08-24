using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data.Legality
{
    public static class PBELegalityChecker
    {
        private static List<(PBESpecies, PBEForm)> GetSpecies(PBESpecies species, PBEForm form)
        {
            // Recursion BAYBEE
            // IDK what to name these functions so enjoy Add1 and Add2
            var list = new List<(PBESpecies, PBEForm)>();
            void Add1(PBESpecies s, PBEForm f)
            {
                // Do not take forms if unable to change into them (Wormadam)
                if (PBEDataUtils.CanChangeForm(s, true))
                {
                    foreach (PBEForm cf in PBEDataUtils.GetForms(s, true))
                    {
                        Add2(s, cf);
                    }
                }
                else
                {
                    Add2(s, f);
                }
            }
            void Add2(PBESpecies s, PBEForm f)
            {
                foreach ((PBESpecies cs, PBEForm cf) in PBEDataProvider.Instance.GetPokemonData(s, f).PreEvolutions)
                {
                    Add1(cs, cf);
                }
                list.Add((s, f));
            }
            Add1(species, form);
            return list;
        }

        public static IReadOnlyCollection<PBEMove> GetLegalMoves(PBESpecies species, PBEForm form, byte level, PBESettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            ValidateSpecies(species, form, true);
            ValidateLevel(level, settings);
            List<(PBESpecies, PBEForm)> speciesToStealFrom = GetSpecies(species, form);

            var moves = new List<PBEMove>();
            foreach ((PBESpecies spe, PBEForm fo) in speciesToStealFrom)
            {
                IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(spe, fo);
                // Disallow moves learned after the current level
                moves.AddRange(pData.LevelUpMoves.Where(t => t.Level <= level).Select(t => t.Move));
                // Disallow form-specific moves from other forms (Rotom)
                moves.AddRange(pData.OtherMoves.Where(t => (spe == species && fo == form) || t.ObtainMethod != PBEMoveObtainMethod.Form).Select(t => t.Move));
                // Event Pokémon checking is extremely basic and wrong, but the goal is not to be super restricting or accurate
                if (PBEEventPokemon.Events.TryGetValue(spe, out ReadOnlyCollection<PBEEventPokemon> events))
                {
                    // Disallow moves learned after the current level
                    moves.AddRange(events.Where(e => e.Level <= level).SelectMany(e => e.Moves).Where(m => m != PBEMove.None));
                }
                if (moves.Any(m => PBEDataProvider.Instance.GetMoveData(m, cache: false).Effect == PBEMoveEffect.Sketch))
                {
                    return PBEDataUtils.SketchLegalMoves;
                }
            }
            return moves.Distinct().Where(m => PBEDataUtils.IsMoveUsable(m)).ToArray();
        }

        internal static void ValidateSpecies(PBESpecies species, PBEForm form, bool requireUsableOutsideOfBattle)
        {
            if (!PBEDataUtils.IsValidForm(species, form, requireUsableOutsideOfBattle))
            {
                throw new ArgumentOutOfRangeException(nameof(form));
            }
        }
        internal static void ValidateNickname(string value, PBESettings settings)
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
        internal static void ValidateLevel(byte value, PBESettings settings)
        {
            if (value < settings.MinLevel || value > settings.MaxLevel)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} must be at least {nameof(settings.MinLevel)} ({settings.MinLevel}) and cannot exceed {nameof(settings.MaxLevel)} ({settings.MaxLevel}).");
            }
        }
        internal static void ValidateAbility(PBEAlphabeticalList<PBEAbility> valid, PBEAbility value)
        {
            if (!valid.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        internal static void ValidateNature(PBENature value)
        {
            if (!PBEDataUtils.AllNatures.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        internal static void ValidateGender(PBEAlphabeticalList<PBEGender> valid, PBEGender value)
        {
            if (!valid.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        internal static void ValidateItem(PBEAlphabeticalList<PBEItem> valid, PBEItem value)
        {
            if (!valid.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        internal static void ValidateCaughtBall(PBEItem value)
        {
            if (!PBEDataUtils.AllBalls.Contains(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}
