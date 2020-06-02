using System;
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

        // TODO: Include generation?
        // TODO: Sketch
        // TODO: Same goals as MoveLegalityCheck
        public static PBEMove[] GetLegalMoves(PBESpecies species, PBEForm form, byte level, PBESettings settings)
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

            return moves.Distinct().Where(m => m != PBEMove.None).ToArray();
        }

        // TODO: Check where the species was born
        // TODO: Check if moves make sense (example: learns a move in gen4 but was born in gen5/caught in dreamworld/is gen5 event)
        // TODO: Check if HMs were transferred
        // TODO: Check events for moves
        // TODO: EggMove_Special
        public static void MoveLegalityCheck(PBEMoveset moveset)
        {
            // Validate basic move rules first
            if (moveset == null)
            {
                throw new ArgumentNullException(nameof(moveset));
            }

            List<(PBESpecies, PBEForm)> speciesToStealFrom = GetSpecies(moveset.Species);

            var levelUp = new List<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)>();
            var other = new List<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)>();
            foreach ((PBESpecies spe, PBEForm fo) in speciesToStealFrom)
            {
                var pData = PBEPokemonData.GetData(spe, fo);
                levelUp.AddRange(pData.LevelUpMoves.Where(t => t.Level <= moveset.Level));
                other.AddRange(pData.OtherMoves);
            }

            // Check generational rules
            bool HasGen3Method(PBEMoveObtainMethod method)
            {
                return method.HasFlag(PBEMoveObtainMethod.LevelUp_RSColoXD)
                    || method.HasFlag(PBEMoveObtainMethod.LevelUp_FR)
                    || method.HasFlag(PBEMoveObtainMethod.LevelUp_LG)
                    || method.HasFlag(PBEMoveObtainMethod.LevelUp_E)
                    || method.HasFlag(PBEMoveObtainMethod.TM_RSFRLGEColoXD)
                    || method.HasFlag(PBEMoveObtainMethod.HM_RSFRLGEColoXD)
                    || method.HasFlag(PBEMoveObtainMethod.MoveTutor_FRLG)
                    || method.HasFlag(PBEMoveObtainMethod.MoveTutor_E)
                    || method.HasFlag(PBEMoveObtainMethod.MoveTutor_XD)
                    || method.HasFlag(PBEMoveObtainMethod.EggMove_RSFRLGE);
            }
            bool HasGen4Method(PBEMoveObtainMethod method)
            {
                return method.HasFlag(PBEMoveObtainMethod.LevelUp_DP)
                    || method.HasFlag(PBEMoveObtainMethod.LevelUp_Pt)
                    || method.HasFlag(PBEMoveObtainMethod.LevelUp_HGSS)
                    || method.HasFlag(PBEMoveObtainMethod.TM_DPPt)
                    || method.HasFlag(PBEMoveObtainMethod.TM_HGSS)
                    || method.HasFlag(PBEMoveObtainMethod.HM_DPPt)
                    || method.HasFlag(PBEMoveObtainMethod.HM_HGSS)
                    || method.HasFlag(PBEMoveObtainMethod.MoveTutor_DP)
                    || method.HasFlag(PBEMoveObtainMethod.MoveTutor_Pt)
                    || method.HasFlag(PBEMoveObtainMethod.MoveTutor_HGSS)
                    || method.HasFlag(PBEMoveObtainMethod.EggMove_DPPt)
                    || method.HasFlag(PBEMoveObtainMethod.EggMove_HGSS);
            }
            bool HasGen5Method(PBEMoveObtainMethod method)
            {
                return method.HasFlag(PBEMoveObtainMethod.LevelUp_BW)
                    || method.HasFlag(PBEMoveObtainMethod.LevelUp_B2W2)
                    || method.HasFlag(PBEMoveObtainMethod.TM_BW)
                    || method.HasFlag(PBEMoveObtainMethod.TM_B2W2)
                    || method.HasFlag(PBEMoveObtainMethod.HM_BWB2W2)
                    || method.HasFlag(PBEMoveObtainMethod.MoveTutor_BW)
                    || method.HasFlag(PBEMoveObtainMethod.MoveTutor_B2W2)
                    || method.HasFlag(PBEMoveObtainMethod.EggMove_BWB2W2);
            }

            IEnumerable<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)> movesAsObtainMethods = levelUp.Where(t => moveset.Contains(t.Move)).Select(t => (t.Move, t.ObtainMethod)).Union(other.Where(t => moveset.Contains(t.Move)));

            // Check to see where the Pokémon DEFINITELY has been
            bool definitelyBeenInGeneration3 = false,
                definitelyBeenInGeneration4 = false,
                definitelyBeenInGeneration5 = false;
            foreach ((PBEMove Move, PBEMoveObtainMethod ObtainMethod) in movesAsObtainMethods)
            {
                bool gen3 = HasGen3Method(ObtainMethod),
                    gen4 = HasGen4Method(ObtainMethod),
                    gen5 = HasGen5Method(ObtainMethod);
                if (gen3 && !gen4 && !gen5)
                {
                    definitelyBeenInGeneration3 = true;
                }
                else if (!gen3 && gen4 && !gen5)
                {
                    definitelyBeenInGeneration4 = true;
                }
                else if (!gen3 && !gen4 && gen5)
                {
                    definitelyBeenInGeneration5 = true;
                }
            }
        }
    }
}
