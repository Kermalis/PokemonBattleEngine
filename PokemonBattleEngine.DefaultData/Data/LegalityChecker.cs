using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.DefaultData.Data;

public static class PBEDDLegalityChecker
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
			foreach ((PBESpecies cs, PBEForm cf) in PBEDefaultDataProvider.Instance.GetPokemonDataExtended(s, f).PreEvolutions)
			{
				Add1(cs, cf);
			}
			list.Add((s, f));
		}
		Add1(species, form);
		return list;
	}

	public static IReadOnlyCollection<PBEMove> GetLegalMoves(PBESpecies species, PBEForm form, byte level)
	{
		List<(PBESpecies, PBEForm)> speciesToStealFrom = GetSpecies(species, form);

		var moves = new List<PBEMove>();
		foreach ((PBESpecies spe, PBEForm fo) in speciesToStealFrom)
		{
			IPBEDDPokemonDataExtended pData = PBEDefaultDataProvider.Instance.GetPokemonDataExtended(spe, fo);
			// Disallow moves learned after the current level
			moves.AddRange(pData.LevelUpMoves.Where(t => t.Level <= level).Select(t => t.Move));
			// Disallow form-specific moves from other forms (Rotom)
			moves.AddRange(pData.OtherMoves.Where(t => (spe == species && fo == form) || t.ObtainMethod != PBEDDMoveObtainMethod.Form).Select(t => t.Move));
			// Event Pokémon checking is extremely basic and wrong, but the goal is not to be super restricting or accurate
			if (PBEDDEventPokemon.Events.TryGetValue(spe, out ReadOnlyCollection<PBEDDEventPokemon>? events))
			{
				// Disallow moves learned after the current level
				moves.AddRange(events.Where(e => e.Level <= level).SelectMany(e => e.Moves).Where(m => m != PBEMove.None));
			}
			if (moves.FindIndex(m => PBEDataProvider.Instance.GetMoveData(m, cache: false).Effect == PBEMoveEffect.Sketch) != -1)
			{
				return PBEDataUtils.SketchLegalMoves;
			}
		}
		return moves.Distinct().Where(m => PBEDataUtils.IsMoveUsable(m)).ToArray();
	}
}
