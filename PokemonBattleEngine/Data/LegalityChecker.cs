using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
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
            PBEPokemonShell.ValidateSpecies(species, form, true);
            PBEPokemonShell.ValidateLevel(level, settings);
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
    }
}
