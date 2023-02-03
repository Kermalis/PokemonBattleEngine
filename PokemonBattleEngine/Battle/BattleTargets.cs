using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Data.Utils;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle;

public sealed partial class PBEBattle
{
	/// <summary>Gets the position across from the inputted position for a specific battle format.</summary>
	/// <param name="battleFormat">The battle format.</param>
	/// <param name="position">The position.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="battleFormat"/> is invalid or <paramref name="position"/> is invalid for <paramref name="battleFormat"/>.</exception>
	public static PBEFieldPosition GetPositionAcross(PBEBattleFormat battleFormat, PBEFieldPosition position)
	{
		switch (battleFormat)
		{
			case PBEBattleFormat.Single:
			case PBEBattleFormat.Rotation:
			{
				if (position == PBEFieldPosition.Center)
				{
					return PBEFieldPosition.Center;
				}
				else
				{
					throw new ArgumentOutOfRangeException(nameof(position));
				}
			}
			case PBEBattleFormat.Double:
			{
				if (position == PBEFieldPosition.Left)
				{
					return PBEFieldPosition.Right;
				}
				else if (position == PBEFieldPosition.Right)
				{
					return PBEFieldPosition.Left;
				}
				else
				{
					throw new ArgumentOutOfRangeException(nameof(position));
				}
			}
			case PBEBattleFormat.Triple:
			{
				if (position == PBEFieldPosition.Left)
				{
					return PBEFieldPosition.Right;
				}
				else if (position == PBEFieldPosition.Center)
				{
					return PBEFieldPosition.Center;
				}
				else if (position == PBEFieldPosition.Right)
				{
					return PBEFieldPosition.Left;
				}
				else
				{
					throw new ArgumentOutOfRangeException(nameof(position));
				}
			}
			default: throw new ArgumentOutOfRangeException(nameof(battleFormat));
		}
	}

	/// <summary>Gets the Pokémon surrounding <paramref name="pkmn"/>.</summary>
	/// <param name="pkmn">The Pokémon to check.</param>
	/// <param name="includeAllies">True if allies should be included, False otherwise.</param>
	/// <param name="includeFoes">True if foes should be included, False otherwise.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="pkmn"/>'s <see cref="PBEBattle"/>'s <see cref="BattleFormat"/> is invalid or <paramref name="pkmn"/>'s <see cref="PBEBattlePokemon.FieldPosition"/> is invalid for <paramref name="pkmn"/>'s <see cref="PBEBattle"/>'s <see cref="BattleFormat"/>.</exception>
	public static IReadOnlyList<PBEBattlePokemon> GetRuntimeSurrounding(PBEBattlePokemon pkmn, bool includeAllies, bool includeFoes)
	{
		if (!includeAllies && !includeFoes)
		{
			throw new ArgumentException($"\"{nameof(includeAllies)}\" and \"{nameof(includeFoes)}\" were false.");
		}
		List<PBEBattlePokemon> allies = pkmn.Team.ActiveBattlers.FindAll(p => p != pkmn);
		List<PBEBattlePokemon> foes = pkmn.Team.OpposingTeam.ActiveBattlers;
		switch (pkmn.Battle.BattleFormat)
		{
			case PBEBattleFormat.Single:
			{
				if (pkmn.FieldPosition == PBEFieldPosition.Center)
				{
					if (includeFoes)
					{
						return foes.FindAll(p => p.FieldPosition == PBEFieldPosition.Center);
					}
					return Array.Empty<PBEBattlePokemon>();
				}
				else
				{
					throw new InvalidDataException(nameof(pkmn.FieldPosition));
				}
			}
			case PBEBattleFormat.Double:
			{
				if (pkmn.FieldPosition == PBEFieldPosition.Left)
				{
					List<PBEBattlePokemon> ret = null!;
					if (includeAllies)
					{
						ret = allies.FindAll(p => p.FieldPosition == PBEFieldPosition.Right);
						if (!includeFoes)
						{
							return ret;
						}
					}
					if (includeFoes)
					{
						List<PBEBattlePokemon> f = foes.FindAll(p => p.FieldPosition == PBEFieldPosition.Left || p.FieldPosition == PBEFieldPosition.Right);
						if (!includeAllies)
						{
							return f;
						}
						ret.AddRange(f);
						return ret;
					}
					return Array.Empty<PBEBattlePokemon>();
				}
				else if (pkmn.FieldPosition == PBEFieldPosition.Right)
				{
					List<PBEBattlePokemon> ret = null!;
					if (includeAllies)
					{
						ret = allies.FindAll(p => p.FieldPosition == PBEFieldPosition.Left);
						if (!includeFoes)
						{
							return ret;
						}
					}
					if (includeFoes)
					{
						List<PBEBattlePokemon> f = foes.FindAll(p => p.FieldPosition == PBEFieldPosition.Left || p.FieldPosition == PBEFieldPosition.Right);
						if (!includeAllies)
						{
							return f;
						}
						ret.AddRange(f);
						return ret;
					}
					return Array.Empty<PBEBattlePokemon>();
				}
				else
				{
					throw new InvalidDataException(nameof(pkmn.FieldPosition));
				}
			}
			case PBEBattleFormat.Triple:
			case PBEBattleFormat.Rotation:
			{
				if (pkmn.FieldPosition == PBEFieldPosition.Left)
				{
					List<PBEBattlePokemon> ret = null!;
					if (includeAllies)
					{
						ret = allies.FindAll(p => p.FieldPosition == PBEFieldPosition.Center);
						if (!includeFoes)
						{
							return ret;
						}
					}
					if (includeFoes)
					{
						List<PBEBattlePokemon> f = foes.FindAll(p => p.FieldPosition == PBEFieldPosition.Center || p.FieldPosition == PBEFieldPosition.Right);
						if (!includeAllies)
						{
							return f;
						}
						ret.AddRange(f);
						return ret;
					}
					return Array.Empty<PBEBattlePokemon>();
				}
				else if (pkmn.FieldPosition == PBEFieldPosition.Center)
				{
					List<PBEBattlePokemon> ret = null!;
					if (includeAllies)
					{
						ret = allies.FindAll(p => p.FieldPosition == PBEFieldPosition.Left || p.FieldPosition == PBEFieldPosition.Right);
						if (!includeFoes)
						{
							return ret;
						}
					}
					if (includeFoes)
					{
						List<PBEBattlePokemon> f = foes.FindAll(p => p.FieldPosition == PBEFieldPosition.Left || p.FieldPosition == PBEFieldPosition.Center || p.FieldPosition == PBEFieldPosition.Right);
						if (!includeAllies)
						{
							return f;
						}
						ret.AddRange(f);
						return ret;
					}
					return Array.Empty<PBEBattlePokemon>();
				}
				else if (pkmn.FieldPosition == PBEFieldPosition.Right)
				{
					List<PBEBattlePokemon> ret = null!;
					if (includeAllies)
					{
						ret = allies.FindAll(p => p.FieldPosition == PBEFieldPosition.Center);
						if (!includeFoes)
						{
							return ret;
						}
					}
					if (includeFoes)
					{
						List<PBEBattlePokemon> f = foes.FindAll(p => p.FieldPosition == PBEFieldPosition.Center || p.FieldPosition == PBEFieldPosition.Left);
						if (!includeAllies)
						{
							return f;
						}
						ret.AddRange(f);
						return ret;
					}
					return Array.Empty<PBEBattlePokemon>();
				}
				else
				{
					throw new InvalidDataException(nameof(pkmn.FieldPosition));
				}
			}
			default: throw new InvalidDataException(nameof(pkmn.Battle.BattleFormat));
		}
	}

	private static void FindFoeLeftTarget(PBEBattlePokemon user, bool canHitFarCorners, List<PBEBattlePokemon> targets)
	{
		PBETeam ot = user.Team.OpposingTeam;
		if (!ot.TryGetPokemon(PBEFieldPosition.Left, out PBEBattlePokemon? pkmn))
		{
			// Left not found; fallback to its teammate
			switch (user.Battle.BattleFormat)
			{
				case PBEBattleFormat.Double:
				{
					if (!ot.TryGetPokemon(PBEFieldPosition.Right, out pkmn))
					{
						return; // Nobody left and nobody right; fail
					}
					break;
				}
				case PBEBattleFormat.Triple:
				{
					if (!ot.TryGetPokemon(PBEFieldPosition.Center, out pkmn))
					{
						if (user.FieldPosition != PBEFieldPosition.Right || canHitFarCorners)
						{
							// Center fainted as well but the user can reach far right
							if (!ot.TryGetPokemon(PBEFieldPosition.Right, out pkmn))
							{
								return; // Nobody left, center, or right; fail
							}
						}
						else
						{
							return; // Nobody left and nobody center; fail since we can't reach the right
						}
					}
					break;
				}
				default: throw new InvalidOperationException();
			}
		}
		targets.Add(pkmn);
	}
	private static void FindFoeCenterTarget(PBEBattlePokemon user, bool canHitFarCorners, PBERandom rand, List<PBEBattlePokemon> targets)
	{
		PBETeam ot = user.Team.OpposingTeam;
		if (!ot.TryGetPokemon(PBEFieldPosition.Center, out PBEBattlePokemon? pkmn))
		{
			switch (user.Battle.BattleFormat)
			{
				case PBEBattleFormat.Single:
				case PBEBattleFormat.Rotation: return;
				default: throw new InvalidOperationException();
				case PBEBattleFormat.Triple:
				{
					// Center not found; fallback to its teammate
					switch (user.FieldPosition)
					{
						case PBEFieldPosition.Left:
						{
							if (!ot.TryGetPokemon(PBEFieldPosition.Right, out pkmn))
							{
								if (canHitFarCorners)
								{
									if (!ot.TryGetPokemon(PBEFieldPosition.Left, out pkmn))
									{
										return; // Nobody center, right, or left; fail
									}
								}
								else
								{
									return; // Nobody center and nobody right; fail since we can't reach the left
								}
							}
							break;
						}
						case PBEFieldPosition.Center:
						{
							if (!ot.TryGetPokemon(PBEFieldPosition.Left, out PBEBattlePokemon? left))
							{
								if (!ot.TryGetPokemon(PBEFieldPosition.Right, out PBEBattlePokemon? right))
								{
									return; // Nobody left or right; fail
								}
								pkmn = right; // Nobody left; pick right
							}
							else
							{
								if (!ot.TryGetPokemon(PBEFieldPosition.Right, out PBEBattlePokemon? right))
								{
									pkmn = left; // Nobody right; pick left
								}
								else
								{
									pkmn = rand.RandomBool() ? left : right; // Left and right present; randomly select left or right
								}
							}
							break;
						}
						case PBEFieldPosition.Right:
						{
							if (!ot.TryGetPokemon(PBEFieldPosition.Left, out pkmn))
							{
								if (canHitFarCorners)
								{
									if (!ot.TryGetPokemon(PBEFieldPosition.Right, out pkmn))
									{
										return; // Nobody center, left, or right; fail
									}
								}
								else
								{
									return; // Nobody center and nobody left; fail since we can't reach the right
								}
							}
							break;
						}
						default: throw new InvalidDataException();
					}
					break;
				}
			}
		}
		targets.Add(pkmn);
	}
	private static void FindFoeRightTarget(PBEBattlePokemon user, bool canHitFarCorners, List<PBEBattlePokemon> targets)
	{
		PBETeam ot = user.Team.OpposingTeam;
		if (!ot.TryGetPokemon(PBEFieldPosition.Right, out PBEBattlePokemon? pkmn))
		{
			// Right not found; fallback to its teammate
			switch (user.Battle.BattleFormat)
			{
				case PBEBattleFormat.Double:
				{
					if (!ot.TryGetPokemon(PBEFieldPosition.Left, out pkmn))
					{
						return; // Nobody right and nobody left; fail
					}
					break;
				}
				case PBEBattleFormat.Triple:
				{
					if (!ot.TryGetPokemon(PBEFieldPosition.Center, out pkmn))
					{
						if (user.FieldPosition != PBEFieldPosition.Left || canHitFarCorners)
						{
							// Center fainted as well but the user can reach far left
							if (!ot.TryGetPokemon(PBEFieldPosition.Left, out pkmn))
							{
								return; // Nobody right, center, or left; fail
							}
						}
						else
						{
							return; // Nobody right and nobody center; fail since we can't reach the left
						}
					}
					break;
				}
				default: throw new InvalidOperationException();
			}
		}
		targets.Add(pkmn);
	}
	/// <summary>Gets all Pokémon that will be hit.</summary>
	/// <param name="user">The Pokémon that will act.</param>
	/// <param name="requestedTargets">The targets the Pokémon wishes to hit.</param>
	/// <param name="canHitFarCorners">Whether the move can hit far Pokémon in a triple battle.</param>
	/// <param name="rand">The random to use.</param>
	private static PBEBattlePokemon[] GetRuntimeTargets(PBEBattlePokemon user, PBETurnTarget requestedTargets, bool canHitFarCorners, PBERandom rand)
	{
		var targets = new List<PBEBattlePokemon>();
		// Foes first, then allies (since initial attack effects run that way)
		if (requestedTargets.HasFlag(PBETurnTarget.FoeLeft))
		{
			FindFoeLeftTarget(user, canHitFarCorners, targets);
		}
		if (requestedTargets.HasFlag(PBETurnTarget.FoeCenter))
		{
			FindFoeCenterTarget(user, canHitFarCorners, rand, targets);
		}
		if (requestedTargets.HasFlag(PBETurnTarget.FoeRight))
		{
			FindFoeRightTarget(user, canHitFarCorners, targets);
		}
		PBETeam t = user.Team;
		if (requestedTargets.HasFlag(PBETurnTarget.AllyLeft))
		{
			t.TryAddPokemonToCollection(PBEFieldPosition.Left, targets);
		}
		if (requestedTargets.HasFlag(PBETurnTarget.AllyCenter))
		{
			t.TryAddPokemonToCollection(PBEFieldPosition.Center, targets);
		}
		if (requestedTargets.HasFlag(PBETurnTarget.AllyRight))
		{
			t.TryAddPokemonToCollection(PBEFieldPosition.Right, targets);
		}
		return targets.Distinct().ToArray(); // Remove duplicate targets
	}

	/// <summary>Determines whether chosen targets are valid for a given move.</summary>
	/// <param name="pkmn">The Pokémon that will act.</param>
	/// <param name="move">The move the Pokémon wishes to use.</param>
	/// <param name="targets">The targets bitfield to validate.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targets"/>, <paramref name="move"/>, <paramref name="pkmn"/>'s <see cref="PBEBattlePokemon.FieldPosition"/>, or <paramref name="pkmn"/>'s <see cref="PBEBattle"/>'s <see cref="BattleFormat"/> is invalid.</exception>
	public static bool AreTargetsValid(PBEBattlePokemon pkmn, PBEMove move, PBETurnTarget targets)
	{
		if (move == PBEMove.None || move >= PBEMove.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(move));
		}
		IPBEMoveData mData = PBEDataProvider.Instance.GetMoveData(move);
		if (!mData.IsMoveUsable())
		{
			throw new ArgumentOutOfRangeException(nameof(move));
		}
		return AreTargetsValid(pkmn, mData, targets);
	}
	public static bool AreTargetsValid(PBEBattlePokemon pkmn, IPBEMoveData mData, PBETurnTarget targets)
	{
		PBEMoveTarget possibleTargets = pkmn.GetMoveTargets(mData);
		switch (pkmn.Battle.BattleFormat)
		{
			case PBEBattleFormat.Single:
			{
				switch (possibleTargets)
				{
					case PBEMoveTarget.All:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return targets == (PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter);
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllFoes:
					case PBEMoveTarget.AllFoesSurrounding:
					case PBEMoveTarget.AllSurrounding:
					case PBEMoveTarget.SingleFoeSurrounding:
					case PBEMoveTarget.SingleNotSelf:
					case PBEMoveTarget.SingleSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return targets == PBETurnTarget.FoeCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllTeam:
					case PBEMoveTarget.RandomFoeSurrounding:
					case PBEMoveTarget.Self:
					case PBEMoveTarget.SelfOrAllySurrounding:
					case PBEMoveTarget.SingleAllySurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return targets == PBETurnTarget.AllyCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					default: throw new InvalidDataException(nameof(possibleTargets));
				}
			}
			case PBEBattleFormat.Double:
			{
				switch (possibleTargets)
				{
					case PBEMoveTarget.All:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == (PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight);
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllFoes:
					case PBEMoveTarget.AllFoesSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == (PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight);
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllTeam:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == (PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight);
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return targets == (PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight);
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == (PBETurnTarget.AllyLeft | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight);
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.RandomFoeSurrounding:
					case PBEMoveTarget.Self:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return targets == PBETurnTarget.AllyLeft;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.AllyRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SelfOrAllySurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SingleAllySurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return targets == PBETurnTarget.AllyRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.AllyLeft;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SingleFoeSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SingleNotSelf:
					case PBEMoveTarget.SingleSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return targets == PBETurnTarget.AllyRight || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					default: throw new InvalidDataException(nameof(possibleTargets));
				}
			}
			case PBEBattleFormat.Triple:
			{
				switch (possibleTargets)
				{
					case PBEMoveTarget.All:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == (PBETurnTarget.AllyLeft | PBETurnTarget.AllyCenter | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight);
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllFoes:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == (PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight);
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllFoesSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return targets == (PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight);
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return targets == (PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight);
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == (PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter);
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return targets == (PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight);
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return targets == (PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight);
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == (PBETurnTarget.AllyCenter | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter);
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllTeam:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == (PBETurnTarget.AllyLeft | PBETurnTarget.AllyCenter | PBETurnTarget.AllyRight);
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.RandomFoeSurrounding:
					case PBEMoveTarget.Self:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return targets == PBETurnTarget.AllyLeft;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return targets == PBETurnTarget.AllyCenter;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.AllyRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SelfOrAllySurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyCenter;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyCenter || targets == PBETurnTarget.AllyRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.AllyCenter || targets == PBETurnTarget.AllyRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SingleAllySurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.AllyCenter;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SingleFoeSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SingleNotSelf:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return targets == PBETurnTarget.AllyCenter || targets == PBETurnTarget.AllyRight || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyRight || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyCenter || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SingleSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return targets == PBETurnTarget.AllyCenter || targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyRight || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.AllyCenter || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					default: throw new InvalidDataException(nameof(possibleTargets));
				}
			}
			case PBEBattleFormat.Rotation:
			{
				switch (possibleTargets)
				{
					case PBEMoveTarget.All:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == (PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter);
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllFoes:
					case PBEMoveTarget.AllFoesSurrounding:
					case PBEMoveTarget.AllSurrounding:
					case PBEMoveTarget.SingleFoeSurrounding:
					case PBEMoveTarget.SingleNotSelf:
					case PBEMoveTarget.SingleSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.FoeCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllTeam:
					case PBEMoveTarget.RandomFoeSurrounding:
					case PBEMoveTarget.Self:
					case PBEMoveTarget.SelfOrAllySurrounding:
					case PBEMoveTarget.SingleAllySurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return targets == PBETurnTarget.AllyCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					default: throw new InvalidDataException(nameof(possibleTargets));
				}
			}
			default: throw new InvalidDataException(nameof(pkmn.Battle.BattleFormat));
		}
	}

	/// <summary>Gets a random target a move can hit when called by <see cref="PBEMoveEffect.Metronome"/>.</summary>
	/// <param name="pkmn">The Pokémon using <paramref name="calledMove"/>.</param>
	/// <param name="calledMove">The move being called.</param>
	/// <param name="rand">The random to use.</param>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="calledMove"/>, <paramref name="pkmn"/>'s <see cref="PBEBattlePokemon.FieldPosition"/>, or <paramref name="pkmn"/>'s <see cref="PBEBattle"/>'s <see cref="BattleFormat"/> is invalid.</exception>
	public static PBETurnTarget GetRandomTargetForMetronome(PBEBattlePokemon pkmn, PBEMove calledMove, PBERandom rand)
	{
		if (calledMove == PBEMove.None || calledMove >= PBEMove.MAX || !PBEDataUtils.IsMoveUsable(calledMove))
		{
			throw new ArgumentOutOfRangeException(nameof(calledMove));
		}
		IPBEMoveData mData = PBEDataProvider.Instance.GetMoveData(calledMove);
		if (!mData.IsMoveUsable())
		{
			throw new ArgumentOutOfRangeException(nameof(calledMove));
		}
		PBEMoveTarget possibleTargets = pkmn.GetMoveTargets(mData);
		switch (pkmn.Battle.BattleFormat)
		{
			case PBEBattleFormat.Single:
			{
				switch (possibleTargets)
				{
					case PBEMoveTarget.All:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllFoes:
					case PBEMoveTarget.AllFoesSurrounding:
					case PBEMoveTarget.AllSurrounding:
					case PBEMoveTarget.RandomFoeSurrounding:
					case PBEMoveTarget.SingleFoeSurrounding:
					case PBEMoveTarget.SingleNotSelf:
					case PBEMoveTarget.SingleSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return PBETurnTarget.FoeCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllTeam:
					case PBEMoveTarget.Self:
					case PBEMoveTarget.SelfOrAllySurrounding:
					case PBEMoveTarget.SingleAllySurrounding: // Helping Hand cannot be called by Metronome anyway
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return PBETurnTarget.AllyCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					default: throw new InvalidDataException(nameof(possibleTargets));
				}
			}
			case PBEBattleFormat.Double:
			{
				switch (possibleTargets)
				{
					case PBEMoveTarget.All:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllFoes:
					case PBEMoveTarget.AllFoesSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllTeam:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.AllyLeft | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.Self:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return PBETurnTarget.AllyLeft;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.AllyRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SelfOrAllySurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							if (rand.RandomBool())
							{
								return PBETurnTarget.AllyLeft;
							}
							else
							{
								return PBETurnTarget.AllyRight;
							}
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SingleAllySurrounding: // Helping Hand cannot be called by Metronome anyway
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return PBETurnTarget.AllyRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.AllyLeft;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.RandomFoeSurrounding:
					case PBEMoveTarget.SingleFoeSurrounding:
					case PBEMoveTarget.SingleNotSelf:
					case PBEMoveTarget.SingleSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							if (rand.RandomBool())
							{
								return PBETurnTarget.FoeLeft;
							}
							else
							{
								return PBETurnTarget.FoeRight;
							}
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					default: throw new InvalidDataException(nameof(possibleTargets));
				}
			}
			case PBEBattleFormat.Triple:
			{
				switch (possibleTargets)
				{
					case PBEMoveTarget.All:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.AllyLeft | PBETurnTarget.AllyCenter | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllFoes:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllFoesSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.AllyCenter | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllTeam:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.AllyLeft | PBETurnTarget.AllyCenter | PBETurnTarget.AllyRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.Self:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return PBETurnTarget.AllyLeft;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							return PBETurnTarget.AllyCenter;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.AllyRight;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SelfOrAllySurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							if (rand.RandomBool())
							{
								return PBETurnTarget.AllyLeft;
							}
							else
							{
								return PBETurnTarget.AllyCenter;
							}
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							int val = rand.RandomInt(0, 2);
							if (val == 0)
							{
								return PBETurnTarget.AllyLeft;
							}
							else if (val == 1)
							{
								return PBETurnTarget.AllyCenter;
							}
							else
							{
								return PBETurnTarget.AllyRight;
							}
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							if (rand.RandomBool())
							{
								return PBETurnTarget.AllyCenter;
							}
							else
							{
								return PBETurnTarget.AllyRight;
							}
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SingleAllySurrounding: // Helping Hand cannot be called by Metronome anyway
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							return PBETurnTarget.AllyCenter;
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							if (rand.RandomBool())
							{
								return PBETurnTarget.AllyLeft;
							}
							else
							{
								return PBETurnTarget.AllyRight;
							}
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.AllyCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.RandomFoeSurrounding:
					case PBEMoveTarget.SingleFoeSurrounding:
					case PBEMoveTarget.SingleSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left)
						{
							if (rand.RandomBool())
							{
								return PBETurnTarget.FoeCenter;
							}
							else
							{
								return PBETurnTarget.FoeRight;
							}
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Center)
						{
							int val = rand.RandomInt(0, 2);
							if (val == 0)
							{
								return PBETurnTarget.FoeLeft;
							}
							else if (val == 1)
							{
								return PBETurnTarget.FoeCenter;
							}
							else
							{
								return PBETurnTarget.FoeRight;
							}
						}
						else if (pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							if (rand.RandomBool())
							{
								return PBETurnTarget.FoeLeft;
							}
							else
							{
								return PBETurnTarget.FoeCenter;
							}
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.SingleNotSelf:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							int val = rand.RandomInt(0, 2);
							if (val == 0)
							{
								return PBETurnTarget.FoeLeft;
							}
							else if (val == 1)
							{
								return PBETurnTarget.FoeCenter;
							}
							else
							{
								return PBETurnTarget.FoeRight;
							}
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					default: throw new InvalidDataException(nameof(possibleTargets));
				}
			}
			case PBEBattleFormat.Rotation:
			{
				switch (possibleTargets)
				{
					case PBEMoveTarget.All:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllFoes:
					case PBEMoveTarget.AllFoesSurrounding:
					case PBEMoveTarget.AllSurrounding:
					case PBEMoveTarget.RandomFoeSurrounding:
					case PBEMoveTarget.SingleFoeSurrounding:
					case PBEMoveTarget.SingleNotSelf:
					case PBEMoveTarget.SingleSurrounding:
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.FoeCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					case PBEMoveTarget.AllTeam:
					case PBEMoveTarget.Self:
					case PBEMoveTarget.SelfOrAllySurrounding:
					case PBEMoveTarget.SingleAllySurrounding: // Helping Hand cannot be called by Metronome anyway
					{
						if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
						{
							return PBETurnTarget.AllyCenter;
						}
						else
						{
							throw new InvalidDataException(nameof(pkmn.FieldPosition));
						}
					}
					default: throw new InvalidDataException(nameof(possibleTargets));
				}
			}
			default: throw new InvalidDataException(nameof(pkmn.Battle.BattleFormat));
		}
	}
}
