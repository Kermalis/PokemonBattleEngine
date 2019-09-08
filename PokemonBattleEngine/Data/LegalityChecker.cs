using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data
{
    public static class PBELegalityChecker
    {
        // TODO: Include generation?
        // TODO: Sketch
        // TODO: Same goals as MoveLegalityCheck
        public static PBEMove[] GetLegalMoves(PBESpecies species, byte level)
        {
            var evolutionChain = new List<PBESpecies>();
            void AddPreEvolutions(PBESpecies sp)
            {
                foreach (PBESpecies pkmn in PBEPokemonData.GetData(sp).PreEvolutions)
                {
                    AddPreEvolutions(pkmn);
                }
                evolutionChain.Add(sp);
            }
            AddPreEvolutions(species);

            var moves = new List<PBEMove>();
            foreach (PBESpecies pkmn in evolutionChain)
            {
                var pData = PBEPokemonData.GetData(pkmn);
                moves.AddRange(pData.LevelUpMoves.Where(t => t.Level <= level).Select(t => t.Move));
                moves.AddRange(pData.OtherMoves.Select(t => t.Move));
                if (PBEEventPokemon.Events.TryGetValue(pkmn, out ReadOnlyCollection<PBEEventPokemon> events))
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
        public static void MoveLegalityCheck(PBESpecies species, byte level, IEnumerable<PBEMove> moves, PBESettings settings)
        {
            // Validate basic move rules first
            if (moves == null || moves.Count() != settings.NumMoves)
            {
                throw new ArgumentOutOfRangeException(nameof(moves), $"A Pokémon must have exactly {settings.NumMoves} moves.");
            }
            if (moves.Any(m => moves.Count(m2 => m != PBEMove.None && m == m2) > 1))
            {
                throw new ArgumentOutOfRangeException(nameof(moves), $"A Pokémon cannot have duplicate moves other than {PBEMove.None}.");
            }
            if (moves.All(m => m == PBEMove.None))
            {
                throw new ArgumentOutOfRangeException(nameof(moves), $"A Pokémon must have at least one move other than {PBEMove.None}.");
            }
            if (species == PBESpecies.Keldeo_Resolute && !moves.Contains(PBEMove.SecretSword))
            {
                throw new ArgumentOutOfRangeException(nameof(moves), $"{species} must have {PBEMove.SecretSword}.");
            }

            // Combine all moves from pre-evolutions
            IEnumerable<PBESpecies> evolutionChain = PBEPokemonData.GetData(species).PreEvolutions.Concat(new[] { species });

            var levelUp = new List<(PBEMove Move, byte Level, PBEMoveObtainMethod ObtainMethod)>();
            var other = new List<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)>();
            foreach (PBESpecies pkmn in evolutionChain)
            {
                var pData = PBEPokemonData.GetData(pkmn);
                levelUp.AddRange(pData.LevelUpMoves.Where(t => t.Level <= level));
                other.AddRange(pData.OtherMoves);
            }
            // TODO:
            PBEMove[] allAsMoves = GetLegalMoves(species, level);

            // Check if there's a move it cannot possibly learn
            foreach (PBEMove m in moves)
            {
                if (m != PBEMove.None && !allAsMoves.Contains(m))
                {
                    throw new ArgumentOutOfRangeException(nameof(moves), $"{species} cannot learn {m}.");
                }
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

            IEnumerable<(PBEMove Move, PBEMoveObtainMethod ObtainMethod)> movesAsObtainMethods = levelUp.Where(t => moves.Contains(t.Move)).Select(t => (t.Move, t.ObtainMethod)).Union(other.Where(t => moves.Contains(t.Move)));

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
