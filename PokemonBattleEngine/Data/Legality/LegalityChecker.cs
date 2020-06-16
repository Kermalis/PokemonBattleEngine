using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data.Legality
{
    public static class PBELegalityChecker
    {
        private static List<(PBESpecies, PBEForm)> GetSpecies(PBESpecies species)
        {
            var list = new List<(PBESpecies, PBEForm)>();
            void Add(PBESpecies s)
            {
                IReadOnlyList<PBEForm> allForms = PBEDataUtils.GetForms(s, true);
                if (allForms.Count > 0)
                {
                    foreach (PBEForm form in allForms)
                    {
                        Add2(s, form);
                    }
                }
                else
                {
                    Add2(s, 0);
                }
            }
            void Add2(PBESpecies s, PBEForm form)
            {
                foreach (PBESpecies spe in PBEPokemonData.GetData(s, form).PreEvolutions)
                {
                    Add(spe);
                }
                list.Add((s, form));
            }
            Add(species);
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
            List<(PBESpecies, PBEForm)> speciesToStealFrom = GetSpecies(species);

            var moves = new List<PBEMove>();
            foreach ((PBESpecies spe, PBEForm fo) in speciesToStealFrom)
            {
                var pData = PBEPokemonData.GetData(spe, fo);
                moves.AddRange(pData.LevelUpMoves.Where(t => t.Level <= level).Select(t => t.Move));
                moves.AddRange(pData.OtherMoves.Select(t => t.Move));
                if (PBEEventPokemon.Events.TryGetValue(spe, out ReadOnlyCollection<PBEEventPokemon> events))
                {
                    moves.AddRange(events.SelectMany(e => e.Moves));
                }
            }
            if (moves.Contains(PBEMove.Sketch))
            {
                return PBEDataUtils.SketchLegalMoves;
            }
            // None is here because of events
            return moves.Distinct().Where(m => m != PBEMove.None && PBEMoveData.IsMoveUsable(m)).ToArray();
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
    }
}
