using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle;

public sealed class PBETurnAction
{
	public byte PokemonId { get; }
	public PBETurnDecision Decision { get; }
	public PBEMove FightMove { get; }
	public PBETurnTarget FightTargets { get; internal set; } // Internal set because of PBEMoveTarget.RandomFoeSurrounding (TODO: Shouldn't this happen at runtime?)
	public PBEItem UseItem { get; }
	public byte SwitchPokemonId { get; }

	internal PBETurnAction(EndianBinaryReader r)
	{
		PokemonId = r.ReadByte();
		Decision = r.ReadEnum<PBETurnDecision>();
		switch (Decision)
		{
			case PBETurnDecision.Fight:
			{
				FightMove = r.ReadEnum<PBEMove>();
				FightTargets = r.ReadEnum<PBETurnTarget>();
				break;
			}
			case PBETurnDecision.Item:
			{
				UseItem = r.ReadEnum<PBEItem>();
				break;
			}
			case PBETurnDecision.SwitchOut:
			{
				SwitchPokemonId = r.ReadByte();
				break;
			}
			case PBETurnDecision.WildFlee: break;
			default: throw new InvalidDataException(nameof(Decision));
		}
	}
	// Fight
	public PBETurnAction(PBEBattlePokemon pokemon, PBEMove fightMove, PBETurnTarget fightTargets)
		: this(pokemon.Id, fightMove, fightTargets) { }
	public PBETurnAction(byte pokemonId, PBEMove fightMove, PBETurnTarget fightTargets)
	{
		PokemonId = pokemonId;
		Decision = PBETurnDecision.Fight;
		FightMove = fightMove;
		FightTargets = fightTargets;
	}
	// Item
	public PBETurnAction(PBEBattlePokemon pokemon, PBEItem item)
		: this(pokemon.Id, item) { }
	public PBETurnAction(byte pokemonId, PBEItem item)
	{
		PokemonId = pokemonId;
		Decision = PBETurnDecision.Item;
		UseItem = item;
	}
	// Switch
	public PBETurnAction(PBEBattlePokemon pokemon, PBEBattlePokemon switchPokemon)
		: this(pokemon.Id, switchPokemon.Id) { }
	public PBETurnAction(byte pokemonId, byte switchPokemonId)
	{
		PokemonId = pokemonId;
		Decision = PBETurnDecision.SwitchOut;
		SwitchPokemonId = switchPokemonId;
	}
	// Internal wild flee
	internal PBETurnAction(PBEBattlePokemon pokemon)
		: this(pokemon.Id) { }
	internal PBETurnAction(byte pokemonId)
	{
		PokemonId = pokemonId;
		Decision = PBETurnDecision.WildFlee;
	}

	internal void ToBytes(EndianBinaryWriter w)
	{
		w.WriteByte(PokemonId);
		w.WriteEnum(Decision);
		switch (Decision)
		{
			case PBETurnDecision.Fight:
			{
				w.WriteEnum(FightMove);
				w.WriteEnum(FightTargets);
				break;
			}
			case PBETurnDecision.Item:
			{
				w.WriteEnum(UseItem);
				break;
			}
			case PBETurnDecision.SwitchOut:
			{
				w.WriteByte(SwitchPokemonId);
				break;
			}
			case PBETurnDecision.WildFlee: break;
			default: throw new InvalidDataException(nameof(Decision));
		}
	}
}
public sealed class PBESwitchIn
{
	public byte PokemonId { get; }
	public PBEFieldPosition Position { get; }

	internal PBESwitchIn(EndianBinaryReader r)
	{
		PokemonId = r.ReadByte();
		Position = r.ReadEnum<PBEFieldPosition>();
	}
	public PBESwitchIn(PBEBattlePokemon pokemon, PBEFieldPosition position)
		: this(pokemon.Id, position) { }
	public PBESwitchIn(byte pokemonId, PBEFieldPosition position)
	{
		PokemonId = pokemonId;
		Position = position;
	}

	internal void ToBytes(EndianBinaryWriter w)
	{
		w.WriteByte(PokemonId);
		w.WriteEnum(Position);
	}
}
public sealed partial class PBEBattle
{
	internal static bool AreActionsValid(PBETrainer trainer, IReadOnlyCollection<PBETurnAction> actions, [NotNullWhen(false)] out string? invalidReason)
	{
		if (trainer.Battle._battleState != PBEBattleState.WaitingForActions)
		{
			throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForActions} to validate actions.");
		}
		if (trainer.ActionsRequired.Count == 0)
		{
			invalidReason = "Actions were already submitted";
			return false;
		}
		if (actions.Count != trainer.ActionsRequired.Count)
		{
			invalidReason = $"Invalid amount of actions submitted; required amount is {trainer.ActionsRequired.Count}";
			return false;
		}

		var verified = new List<PBEBattlePokemon>(trainer.ActionsRequired.Count);
		var standBy = new List<PBEBattlePokemon>(trainer.ActionsRequired.Count);
		var items = new Dictionary<PBEItem, int>(trainer.ActionsRequired.Count);
		foreach (PBETurnAction action in actions)
		{
			if (!trainer.TryGetPokemon(action.PokemonId, out PBEBattlePokemon? pkmn))
			{
				invalidReason = $"Invalid Pokémon ID ({action.PokemonId})";
				return false;
			}
			if (!trainer.ActionsRequired.Contains(pkmn))
			{
				invalidReason = $"Pokémon {action.PokemonId} not looking for actions";
				return false;
			}
			if (verified.Contains(pkmn))
			{
				invalidReason = $"Pokémon {action.PokemonId} was multiple actions";
				return false;
			}
			switch (action.Decision)
			{
				case PBETurnDecision.Fight:
				{
					if (Array.IndexOf(pkmn.GetUsableMoves(), action.FightMove) == -1)
					{
						invalidReason = $"{action.FightMove} is not usable by Pokémon {action.PokemonId}";
						return false;
					}
					if (action.FightMove == pkmn.TempLockedMove && action.FightTargets != pkmn.TempLockedTargets)
					{
						invalidReason = $"Pokémon {action.PokemonId} must target {pkmn.TempLockedTargets}";
						return false;
					}
					if (!AreTargetsValid(pkmn, action.FightMove, action.FightTargets))
					{
						invalidReason = $"Invalid move targets for Pokémon {action.PokemonId}'s {action.FightMove}";
						return false;
					}
					break;
				}
				case PBETurnDecision.Item:
				{
					if (pkmn.TempLockedMove != PBEMove.None)
					{
						invalidReason = $"Pokémon {action.PokemonId} must use {pkmn.TempLockedMove}";
						return false;
					}
					if (!trainer.Inventory.TryGetValue(action.UseItem, out PBEBattleInventory.PBEBattleInventorySlot? slot))
					{
						invalidReason = $"Trainer \"{trainer.Name}\" does not have any {action.UseItem}"; // Handles wild Pokémon
						return false;
					}
					bool used = items.TryGetValue(action.UseItem, out int amtUsed);
					if (!used)
					{
						amtUsed = 0;
					}
					long newAmt = slot.Quantity - amtUsed;
					if (newAmt <= 0)
					{
						invalidReason = $"Tried to use too many {action.UseItem}";
						return false;
					}
					if (trainer.Battle.BattleType == PBEBattleType.Wild && trainer.Team.OpposingTeam.ActiveBattlers.Count > 1
						&& PBEDataUtils.AllBalls.Contains(action.UseItem))
					{
						invalidReason = $"Cannot throw a ball at multiple wild Pokémon";
						return false;
					}
					amtUsed++;
					if (used)
					{
						items[action.UseItem] = amtUsed;
					}
					else
					{
						items.Add(action.UseItem, amtUsed);
					}
					break;
				}
				case PBETurnDecision.SwitchOut:
				{
					if (!pkmn.CanSwitchOut())
					{
						invalidReason = $"Pokémon {action.PokemonId} cannot switch out";
						return false;
					}
					if (!trainer.TryGetPokemon(action.SwitchPokemonId, out PBEBattlePokemon? switchPkmn))
					{
						invalidReason = $"Invalid switch Pokémon ID ({action.PokemonId})";
						return false;
					}
					if (switchPkmn.HP == 0)
					{
						invalidReason = $"Switch Pokémon {action.PokemonId} is fainted";
						return false;
					}
					if (switchPkmn.PBEIgnore)
					{
						invalidReason = $"Switch Pokémon {action.PokemonId} cannot battle";
						return false;
					}
					if (switchPkmn.FieldPosition != PBEFieldPosition.None)
					{
						invalidReason = $"Switch Pokémon {action.PokemonId} is already on the field";
						return false;
					}
					if (standBy.Contains(switchPkmn))
					{
						invalidReason = $"Switch Pokémon {action.PokemonId} was asked to be switched in multiple times";
						return false;
					}
					standBy.Add(switchPkmn);
					break;
				}
				default:
				{
					invalidReason = $"Invalid turn decision ({action.Decision})";
					return false;
				}
			}
			verified.Add(pkmn);
		}
		invalidReason = null;
		return true;
	}
	internal static bool SelectActionsIfValid(PBETrainer trainer, IReadOnlyCollection<PBETurnAction> actions, [NotNullWhen(false)] out string? invalidReason)
	{
		if (!AreActionsValid(trainer, actions, out invalidReason))
		{
			return false;
		}

		trainer.ActionsRequired.Clear();
		foreach (PBETurnAction action in actions)
		{
			PBEBattlePokemon pkmn = trainer.GetPokemon(action.PokemonId);
			if (action.Decision == PBETurnDecision.Fight && pkmn.GetMoveTargets(action.FightMove) == PBEMoveTarget.RandomFoeSurrounding)
			{
				switch (trainer.Battle.BattleFormat)
				{
					case PBEBattleFormat.Single:
					case PBEBattleFormat.Rotation:
					{
						action.FightTargets = PBETurnTarget.FoeCenter;
						break;
					}
					case PBEBattleFormat.Double:
					{
						action.FightTargets = trainer.Battle._rand.RandomBool() ? PBETurnTarget.FoeLeft : PBETurnTarget.FoeRight;
						break;
					}
					case PBEBattleFormat.Triple:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							action.FightTargets = trainer.Battle._rand.RandomBool() ? PBETurnTarget.FoeCenter : PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							PBETeam oppTeam = trainer.Team.OpposingTeam;
							int r; // Keep randomly picking until a non-fainted foe is selected
						roll:
							r = trainer.Battle._rand.RandomInt(0, 2);
							if (r == 0)
							{
								if (oppTeam.IsSpotOccupied(PBEFieldPosition.Left))
								{
									action.FightTargets = PBETurnTarget.FoeLeft;
								}
								else
								{
									goto roll;
								}
							}
							else if (r == 1)
							{
								if (oppTeam.IsSpotOccupied(PBEFieldPosition.Center))
								{
									action.FightTargets = PBETurnTarget.FoeCenter;
								}
								else
								{
									goto roll;
								}
							}
							else
							{
								if (oppTeam.IsSpotOccupied(PBEFieldPosition.Right))
								{
									action.FightTargets = PBETurnTarget.FoeRight;
								}
								else
								{
									goto roll;
								}
							}
						}
						else
						{
							action.FightTargets = trainer.Battle._rand.RandomBool() ? PBETurnTarget.FoeLeft : PBETurnTarget.FoeCenter;
						}
						break;
					}
					default: throw new InvalidDataException(nameof(trainer.Battle.BattleFormat));
				}
			}
			pkmn.TurnAction = action;
		}
		if (trainer.Battle.Trainers.All(t => t.ActionsRequired.Count == 0))
		{
			trainer.Battle.BattleState = PBEBattleState.ReadyToRunTurn;
		}
		return true;
	}

	internal static bool AreSwitchesValid(PBETrainer trainer, IReadOnlyCollection<PBESwitchIn> switches, [NotNullWhen(false)] out string? invalidReason)
	{
		if (trainer.Battle._battleState != PBEBattleState.WaitingForSwitchIns)
		{
			throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForSwitchIns} to validate switches.");
		}
		if (trainer.SwitchInsRequired == 0)
		{
			invalidReason = "Switches were already submitted";
			return false;
		}
		if (switches.Count != trainer.SwitchInsRequired)
		{
			invalidReason = $"Invalid amount of switches submitted; required amount is {trainer.SwitchInsRequired}";
			return false;
		}
		var verified = new List<PBEBattlePokemon>(trainer.SwitchInsRequired);
		foreach (PBESwitchIn s in switches)
		{
			if (s.Position == PBEFieldPosition.None || s.Position >= PBEFieldPosition.MAX || !trainer.OwnsSpot(s.Position))
			{
				invalidReason = $"Invalid position ({s.PokemonId})";
				return false;
			}
			if (!trainer.TryGetPokemon(s.PokemonId, out PBEBattlePokemon? pkmn))
			{
				invalidReason = $"Invalid Pokémon ID ({s.PokemonId})";
				return false;
			}
			if (pkmn.HP == 0)
			{
				invalidReason = $"Pokémon {s.PokemonId} is fainted";
				return false;
			}
			if (pkmn.PBEIgnore)
			{
				invalidReason = $"Pokémon {s.PokemonId} cannot battle";
				return false;
			}
			if (pkmn.FieldPosition != PBEFieldPosition.None)
			{
				invalidReason = $"Pokémon {s.PokemonId} is already on the field";
				return false;
			}
			if (verified.Contains(pkmn))
			{
				invalidReason = $"Pokémon {s.PokemonId} was asked to be switched in multiple times";
				return false;
			}
			verified.Add(pkmn);
		}
		invalidReason = null;
		return true;
	}
	internal static bool SelectSwitchesIfValid(PBETrainer trainer, IReadOnlyCollection<PBESwitchIn> switches, [NotNullWhen(false)] out string? invalidReason)
	{
		if (!AreSwitchesValid(trainer, switches, out invalidReason))
		{
			return false;
		}
		trainer.SwitchInsRequired = 0;
		foreach (PBESwitchIn s in switches)
		{
			trainer.SwitchInQueue.Add((trainer.GetPokemon(s.PokemonId), s.Position));
		}
		if (trainer.Battle.Trainers.All(t => t.SwitchInsRequired == 0))
		{
			trainer.Battle.BattleState = PBEBattleState.ReadyToRunSwitches;
		}
		return true;
	}

	internal static bool IsFleeValid(PBETrainer trainer, [NotNullWhen(false)] out string? invalidReason)
	{
		if (trainer.Battle.BattleType != PBEBattleType.Wild)
		{
			throw new InvalidOperationException($"{nameof(BattleType)} must be {PBEBattleType.Wild} to flee.");
		}
		switch (trainer.Battle._battleState)
		{
			case PBEBattleState.WaitingForActions:
			{
				if (trainer.ActionsRequired.Count == 0)
				{
					invalidReason = "Actions were already submitted";
					return false;
				}
				PBEBattlePokemon pkmn = trainer.ActiveBattlersOrdered.First();
				if (pkmn.TempLockedMove != PBEMove.None)
				{
					invalidReason = $"Pokémon {pkmn.Id} must use {pkmn.TempLockedMove}";
					return false;
				}
				break;
			}
			case PBEBattleState.WaitingForSwitchIns:
			{
				if (trainer.SwitchInsRequired == 0)
				{
					invalidReason = "Switches were already submitted";
					return false;
				}
				break;
			}
			default: throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForActions} or {PBEBattleState.WaitingForSwitchIns} to flee.");
		}
		invalidReason = null;
		return true;
	}
	internal static bool SelectFleeIfValid(PBETrainer trainer, [NotNullWhen(false)] out string? invalidReason)
	{
		if (!IsFleeValid(trainer, out invalidReason))
		{
			return false;
		}
		trainer.RequestedFlee = true;
		if (trainer.Battle._battleState == PBEBattleState.WaitingForActions)
		{
			trainer.ActionsRequired.Clear();
			if (trainer.Battle.Trainers.All(t => t.ActionsRequired.Count == 0))
			{
				trainer.Battle.BattleState = PBEBattleState.ReadyToRunTurn;
			}
		}
		else // WaitingForSwitches
		{
			trainer.SwitchInsRequired = 0;
			if (trainer.Battle.Trainers.All(t => t.SwitchInsRequired == 0))
			{
				trainer.Battle.BattleState = PBEBattleState.ReadyToRunSwitches;
			}
		}
		return true;
	}
}
public sealed partial class PBETrainer
{
	public bool AreActionsValid([NotNullWhen(false)] out string? invalidReason, params PBETurnAction[] actions)
	{
		return PBEBattle.AreActionsValid(this, actions, out invalidReason);
	}
	public bool AreActionsValid(IReadOnlyCollection<PBETurnAction> actions, [NotNullWhen(false)] out string? invalidReason)
	{
		return PBEBattle.AreActionsValid(this, actions, out invalidReason);
	}
	public bool SelectActionsIfValid([NotNullWhen(false)] out string? invalidReason, params PBETurnAction[] actions)
	{
		return PBEBattle.SelectActionsIfValid(this, actions, out invalidReason);
	}
	public bool SelectActionsIfValid(IReadOnlyCollection<PBETurnAction> actions, [NotNullWhen(false)] out string? invalidReason)
	{
		return PBEBattle.SelectActionsIfValid(this, actions, out invalidReason);
	}

	public bool AreSwitchesValid([NotNullWhen(false)] out string? invalidReason, params PBESwitchIn[] switches)
	{
		return PBEBattle.AreSwitchesValid(this, switches, out invalidReason);
	}
	public bool AreSwitchesValid(IReadOnlyCollection<PBESwitchIn> switches, [NotNullWhen(false)] out string? invalidReason)
	{
		return PBEBattle.AreSwitchesValid(this, switches, out invalidReason);
	}
	public bool SelectSwitchesIfValid([NotNullWhen(false)] out string? invalidReason, params PBESwitchIn[] switches)
	{
		return PBEBattle.SelectSwitchesIfValid(this, switches, out invalidReason);
	}
	public bool SelectSwitchesIfValid(IReadOnlyCollection<PBESwitchIn> switches, [NotNullWhen(false)] out string? invalidReason)
	{
		return PBEBattle.SelectSwitchesIfValid(this, switches, out invalidReason);
	}

	public bool IsFleeValid([NotNullWhen(false)] out string? invalidReason)
	{
		return PBEBattle.IsFleeValid(this, out invalidReason);
	}
	public bool SelectFleeIfValid([NotNullWhen(false)] out string? invalidReason)
	{
		return PBEBattle.SelectFleeIfValid(this, out invalidReason);
	}
}
