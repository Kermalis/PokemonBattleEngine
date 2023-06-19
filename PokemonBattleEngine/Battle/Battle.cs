﻿using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kermalis.PokemonBattleEngine.Battle;

/// <summary>Represents a specific Pokémon battle.</summary>
public sealed partial class PBEBattle
{
	public delegate void BattleStateChangedEvent(PBEBattle battle);
	public event BattleStateChangedEvent? OnStateChanged;
	public PBEBattleState BattleState { get; private set; }
	public ushort TurnNumber { get; set; }
	public PBEBattleResult? BattleResult { get; set; }

	private readonly PBERandom _rand;
	public bool IsLocallyHosted { get; }
	public PBEBattleType BattleType { get; }
	public PBEBattleTerrain BattleTerrain { get; }
	public PBEBattleFormat BattleFormat { get; }
	public PBESettings Settings { get; }
	public PBETeams Teams { get; }
	public PBETrainers Trainers { get; }
	public List<PBEBattlePokemon> ActiveBattlers { get; } = new(6);
	private readonly List<PBEBattlePokemon> _turnOrder;

	public PBEWeather Weather { get; set; }
	public byte WeatherCounter { get; set; }
	public PBEBattleStatus BattleStatus { get; set; }
	public byte TrickRoomCount { get; set; }

	public List<IPBEPacket> Events { get; } = new();

	// Trainer battle
	private PBEBattle(PBEBattleFormat battleFormat, PBESettings settings, IReadOnlyList<PBETrainerInfo> ti0, IReadOnlyList<PBETrainerInfo> ti1,
		PBEBattleTerrain battleTerrain, PBEWeather weather, int? randomSeed)
	{
		if (battleFormat >= PBEBattleFormat.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(battleFormat));
		}
		if (battleTerrain >= PBEBattleTerrain.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(battleTerrain));
		}
		if (weather >= PBEWeather.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(weather));
		}

		settings.ShouldBeReadOnly(nameof(settings));
		_rand = new PBERandom(randomSeed ?? PBEDataProvider.GlobalRandom.RandomInt());
		IsLocallyHosted = true;
		BattleType = PBEBattleType.Trainer;
		BattleTerrain = battleTerrain;
		BattleFormat = battleFormat;
		Settings = settings;
		Weather = weather;
		Teams = new PBETeams(this, ti0, ti1, out PBETrainers trainers);
		Trainers = trainers;
		_turnOrder = new List<PBEBattlePokemon>(6);
		QueueUpPokemon();
	}
	// Wild battle
	private PBEBattle(PBEBattleFormat battleFormat, PBESettings settings, IReadOnlyList<PBETrainerInfo> ti, PBEWildInfo wi,
		PBEBattleTerrain battleTerrain, PBEWeather weather, int? randomSeed)
	{
		if (battleFormat >= PBEBattleFormat.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(battleFormat));
		}
		if (battleTerrain >= PBEBattleTerrain.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(battleTerrain));
		}
		if (weather >= PBEWeather.MAX)
		{
			throw new ArgumentOutOfRangeException(nameof(weather));
		}
		settings.ShouldBeReadOnly(nameof(settings));
		_rand = new PBERandom(randomSeed ?? PBEDataProvider.GlobalRandom.RandomInt());
		IsLocallyHosted = true;
		BattleType = PBEBattleType.Wild;
		BattleTerrain = battleTerrain;
		BattleFormat = battleFormat;
		Settings = settings;
		Weather = weather;
		Teams = new PBETeams(this, ti, wi, out PBETrainers trainers);
		Trainers = trainers;
		_turnOrder = new List<PBEBattlePokemon>(6);
		QueueUpPokemon();
	}
	// Remote battle
	private PBEBattle(PBEBattlePacket packet)
	{
		IsLocallyHosted = false;
		BattleType = packet.BattleType;
		BattleFormat = packet.BattleFormat;
		BattleTerrain = packet.BattleTerrain;
		Weather = packet.Weather;
		Settings = packet.Settings;
		Teams = new PBETeams(this, packet, out PBETrainers trainers);
		Trainers = trainers;
		// These two will never be used in a non-local battle
		_rand = null!;
		_turnOrder = null!;
	}

	public static PBEBattle CreateTrainerBattle(PBEBattleFormat battleFormat, PBESettings settings, PBETrainerInfo ti0, PBETrainerInfo ti1,
		PBEBattleTerrain battleTerrain = PBEBattleTerrain.Plain, PBEWeather weather = PBEWeather.None, int? randomSeed = null)
	{
		return new PBEBattle(battleFormat, settings, new[] { ti0 }, new[] { ti1 }, battleTerrain, weather, randomSeed);
	}
	public static PBEBattle CreateTrainerBattle(PBEBattleFormat battleFormat, PBESettings settings, IReadOnlyList<PBETrainerInfo> ti0, IReadOnlyList<PBETrainerInfo> ti1,
		PBEBattleTerrain battleTerrain = PBEBattleTerrain.Plain, PBEWeather weather = PBEWeather.None, int? randomSeed = null)
	{
		return new PBEBattle(battleFormat, settings, ti0, ti1, battleTerrain, weather, randomSeed);
	}
	public static PBEBattle CreateWildBattle(PBEBattleFormat battleFormat, PBESettings settings, PBETrainerInfo ti0, PBEWildInfo wi,
		PBEBattleTerrain battleTerrain = PBEBattleTerrain.Plain, PBEWeather weather = PBEWeather.None, int? randomSeed = null)
	{
		return new PBEBattle(battleFormat, settings, new[] { ti0 }, wi, battleTerrain, weather, randomSeed);
	}
	public static PBEBattle CreateWildBattle(PBEBattleFormat battleFormat, PBESettings settings, IReadOnlyList<PBETrainerInfo> ti0, PBEWildInfo wi,
		PBEBattleTerrain battleTerrain = PBEBattleTerrain.Plain, PBEWeather weather = PBEWeather.None, int? randomSeed = null)
	{
		return new PBEBattle(battleFormat, settings, ti0, wi, battleTerrain, weather, randomSeed);
	}
	public static PBEBattle CreateRemoteBattle(PBEBattlePacket packet)
	{
		return new PBEBattle(packet);
	}

	private void QueueUp(PBETeam team, PBEFieldPosition pos, ref PBETrainer? tr, ref int i)
	{
		// See which trainer owns this spot
		PBETrainer t = team.GetTrainer(pos);
		// If it's not the previous trainer, we start at their first PKMN
		if (tr != t)
		{
			i = 0;
			tr = t;
		}
		PBEList<PBEBattlePokemon> party = t.Party;
	// If the check index is valid, try to send out a non-fainted non-ignore PKMN
	tryget:
		if (i < party.Count)
		{
			PBEBattlePokemon p = party[i];
			// If we should ignore this PKMN, try to get the one in the next index
			if (!p.CanBattle)
			{
				i++;
				goto tryget;
			}
			// Valid PKMN, send it out
			p.Trainer.SwitchInQueue.Add((p, pos));
			// Wild PKMN should be out already
			if (team.IsWild)
			{
				p.FieldPosition = pos;
				ActiveBattlers.Add(p);
			}
			// Next slot to check
			i++;
		}
	}
	internal void QueueUpPokemon()
	{
		switch (BattleFormat)
		{
			case PBEBattleFormat.Single:
			{
				foreach (PBETeam team in Teams)
				{
					PBETrainer? t = null;
					int i = 0;
					QueueUp(team, PBEFieldPosition.Center, ref t, ref i);
				}
				break;
			}
			case PBEBattleFormat.Double:
			{
				foreach (PBETeam team in Teams)
				{
					PBETrainer? t = null;
					int i = 0;
					QueueUp(team, PBEFieldPosition.Left, ref t, ref i);
					QueueUp(team, PBEFieldPosition.Right, ref t, ref i);
				}
				break;
			}
			case PBEBattleFormat.Triple:
			{
				foreach (PBETeam team in Teams)
				{
					PBETrainer? t = null;
					int i = 0;
					QueueUp(team, PBEFieldPosition.Left, ref t, ref i);
					QueueUp(team, PBEFieldPosition.Center, ref t, ref i);
					QueueUp(team, PBEFieldPosition.Right, ref t, ref i);
				}
				break;
			}
			case PBEBattleFormat.Rotation:
			{
				foreach (PBETeam team in Teams)
				{
					PBETrainer? t = null;
					int i = 0;
					QueueUp(team, PBEFieldPosition.Center, ref t, ref i);
					QueueUp(team, PBEFieldPosition.Left, ref t, ref i);
					QueueUp(team, PBEFieldPosition.Right, ref t, ref i);
				}
				break;
			}
			default: throw new ArgumentOutOfRangeException(nameof(BattleFormat));
		}

		SetBattleState(PBEBattleState.ReadyToBegin);
	}
	private void CheckLocal()
	{
		if (!IsLocallyHosted)
		{
			throw new InvalidOperationException("This battle is not locally hosted");
		}
	}
	/// <summary>Begins the battle.</summary>
	/// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.ReadyToBegin"/>.</exception>
	public async Task Begin()
	{
		CheckLocal();
		if (BattleState != PBEBattleState.ReadyToBegin)
		{
			throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.ReadyToBegin} to begin the battle.");
		}

		SetBattleState(PBEBattleState.Processing);
		await BroadcastBattle(); // The first packet sent is PBEBattlePacket which replays rely on

		// Wild Pokémon appearing
		if (BattleType == PBEBattleType.Wild)
		{
			PBETeam team = Teams[1];
			PBETrainer trainer = team.Trainers[0];
			int count = trainer.SwitchInQueue.Count;
			var appearances = new PBEPkmnAppearedInfo[count];
			for (int i = 0; i < count; i++)
			{
				appearances[i] = new PBEPkmnAppearedInfo(trainer.SwitchInQueue[i].Pkmn);
			}
			trainer.SwitchInQueue.Clear();
			await BroadcastWildPkmnAppeared(appearances);
		}
		await SwitchesOrActions();
	}
	/// <summary>Runs a turn.</summary>
	/// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.ReadyToRunTurn"/>.</exception>
	public async Task RunTurn()
	{
		CheckLocal();
		if (BattleState != PBEBattleState.ReadyToRunTurn)
		{
			throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.ReadyToRunTurn} to run a turn.");
		}

		SetBattleState(PBEBattleState.Processing);
		await FleeCheck();
		if (await EndCheck())
		{
			return;
		}
		DetermineTurnOrder();
		await RunActionsInOrder();
		await TurnEnded();
	}
	public async Task RunSwitches()
	{
		CheckLocal();
		if (BattleState != PBEBattleState.ReadyToRunSwitches)
		{
			throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.ReadyToRunSwitches} to run switches.");
		}

		SetBattleState(PBEBattleState.Processing);
		await FleeCheck();
		if (await EndCheck())
		{
			return;
		}
		await SwitchesOrActions();
	}

	private void SetBattleState(PBEBattleState state)
	{
		if (state != BattleState)
		{
			BattleState = state;
			OnStateChanged?.Invoke(this);
		}
	}
	/// <summary>Sets <see cref="BattleState"/> to <see cref="PBEBattleState.Ended"/> and clears <see cref="OnNewEvent"/> and <see cref="OnStateChanged"/>. Does not touch <see cref="BattleResult"/>.</summary>
	public void SetEnded()
	{
		SetBattleState(PBEBattleState.Ended);
		OnNewEvent = null;
		OnStateChanged = null;
	}
	private async Task<bool> EndCheck()
	{
		if (BattleResult is null)
		{
			return false;
		}

		await BroadcastBattleResult(BattleResult.Value);
		foreach (PBEBattlePokemon pkmn in ActiveBattlers)
		{
			pkmn.ApplyNaturalCure(); // Natural Cure happens at the end of the battle. Pokémon should be copied when BattleState is set to "Ended", not upon battle result.
		}
		SetEnded();
		return true;
	}
	private async Task SwitchesOrActions()
	{
		// Checking SwitchInQueue count since SwitchInsRequired is set to 0 after submitting switches
		PBETrainer[] trainersWithSwitchIns = Trainers.Where(t => t.SwitchInQueue.Count > 0).ToArray();
		if (trainersWithSwitchIns.Length > 0)
		{
			var list = new List<PBEBattlePokemon>(6);
			foreach (PBETrainer trainer in trainersWithSwitchIns)
			{
				int count = trainer.SwitchInQueue.Count;
				var switches = new PBEPkmnAppearedInfo[count];
				for (int i = 0; i < count; i++)
				{
					(PBEBattlePokemon pkmn, PBEFieldPosition pos) = trainer.SwitchInQueue[i];
					pkmn.FieldPosition = pos;
					switches[i] = CreateSwitchInInfo(pkmn);
					PBETrainer.SwitchTwoPokemon(pkmn, pos); // Swap after Illusion
					ActiveBattlers.Add(pkmn); // Add before broadcast
					list.Add(pkmn);
				}
				await BroadcastPkmnSwitchIn(trainer, switches);
			}
			await DoSwitchInEffects(list);
		}

		foreach (PBETrainer trainer in Trainers)
		{
			int available = trainer.NumConsciousPkmn - trainer.NumPkmnOnField;
			trainer.SwitchInsRequired = 0;
			trainer.SwitchInQueue.Clear();
			if (available > 0)
			{
				switch (BattleFormat)
				{
					case PBEBattleFormat.Single:
					{
						if (!trainer.IsSpotOccupied(PBEFieldPosition.Center))
						{
							trainer.SwitchInsRequired = 1;
						}
						break;
					}
					case PBEBattleFormat.Double:
					{
						if (trainer.OwnsSpot(PBEFieldPosition.Left) && !trainer.IsSpotOccupied(PBEFieldPosition.Left))
						{
							available--;
							trainer.SwitchInsRequired++;
						}
						if (available > 0 && trainer.OwnsSpot(PBEFieldPosition.Right) && !trainer.IsSpotOccupied(PBEFieldPosition.Right))
						{
							trainer.SwitchInsRequired++;
						}
						break;
					}
					case PBEBattleFormat.Rotation:
					case PBEBattleFormat.Triple:
					{
						if (trainer.OwnsSpot(PBEFieldPosition.Left) && !trainer.IsSpotOccupied(PBEFieldPosition.Left))
						{
							available--;
							trainer.SwitchInsRequired++;
						}
						if (available > 0 && trainer.OwnsSpot(PBEFieldPosition.Center) && !trainer.IsSpotOccupied(PBEFieldPosition.Center))
						{
							available--;
							trainer.SwitchInsRequired++;
						}
						if (available > 0 && trainer.OwnsSpot(PBEFieldPosition.Right) && !trainer.IsSpotOccupied(PBEFieldPosition.Right))
						{
							trainer.SwitchInsRequired++;
						}
						break;
					}
					default: throw new ArgumentOutOfRangeException(nameof(BattleFormat));
				}
			}
		}
		trainersWithSwitchIns = Trainers.Where(t => t.SwitchInsRequired > 0).ToArray();
		if (trainersWithSwitchIns.Length > 0)
		{
			SetBattleState(PBEBattleState.WaitingForSwitchIns);
			foreach (PBETrainer trainer in trainersWithSwitchIns)
			{
				await BroadcastSwitchInRequest(trainer);
			}
		}
		else
		{
			if (await EndCheck())
			{
				return;
			}

			foreach (PBEBattlePokemon pkmn in ActiveBattlers)
			{
				pkmn.HasUsedMoveThisTurn = false;
				pkmn.TurnAction = null;
				pkmn.SpeedBoost_AbleToSpeedBoostThisTurn = pkmn.Ability == PBEAbility.SpeedBoost;

				if (pkmn.Status2.HasFlag(PBEStatus2.Flinching))
				{
					await BroadcastStatus2(pkmn, pkmn, PBEStatus2.Flinching, PBEStatusAction.Ended);
				}
				if (pkmn.Status2.HasFlag(PBEStatus2.HelpingHand))
				{
					await BroadcastStatus2(pkmn, pkmn, PBEStatus2.HelpingHand, PBEStatusAction.Ended);
				}
				if (pkmn.Status2.HasFlag(PBEStatus2.LockOn))
				{
					if (--pkmn.LockOnTurns == 0)
					{
						pkmn.LockOnPokemon = null;
						await BroadcastStatus2(pkmn, pkmn, PBEStatus2.LockOn, PBEStatusAction.Ended);
					}
				}
				if (pkmn.Protection_Used)
				{
					pkmn.Protection_Counter++;
					pkmn.Protection_Used = false;
					if (pkmn.Status2.HasFlag(PBEStatus2.Protected))
					{
						await BroadcastStatus2(pkmn, pkmn, PBEStatus2.Protected, PBEStatusAction.Ended);
					}
				}
				else
				{
					pkmn.Protection_Counter = 0;
				}
				if (pkmn.Status2.HasFlag(PBEStatus2.Roost))
				{
					pkmn.EndRoost();
					await BroadcastStatus2(pkmn, pkmn, PBEStatus2.Roost, PBEStatusAction.Ended);
				}
			}
			foreach (PBETeam team in Teams)
			{
				if (team.TeamStatus.HasFlag(PBETeamStatus.QuickGuard))
				{
					await BroadcastTeamStatus(team, PBETeamStatus.QuickGuard, PBETeamStatusAction.Ended);
				}
				if (team.TeamStatus.HasFlag(PBETeamStatus.WideGuard))
				{
					await BroadcastTeamStatus(team, PBETeamStatus.WideGuard, PBETeamStatusAction.Ended);
				}
			}
			foreach (PBETrainer trainer in Trainers)
			{
				trainer.ActionsRequired.Clear();
				trainer.ActionsRequired.AddRange(trainer.ActiveBattlersOrdered);
			}

			// #318 - We check pkmn on the field instead of conscious pkmn because of multi-battles
			// It still works if there's only one trainer on the team since we check for available switch-ins above
			if (BattleFormat == PBEBattleFormat.Triple && Teams.All(t => t.NumPkmnOnField == 1))
			{
				PBEBattlePokemon pkmn0 = ActiveBattlers[0],
					pkmn1 = ActiveBattlers[1];
				if ((pkmn0.FieldPosition == PBEFieldPosition.Left && pkmn1.FieldPosition == PBEFieldPosition.Left) || (pkmn0.FieldPosition == PBEFieldPosition.Right && pkmn1.FieldPosition == PBEFieldPosition.Right))
				{
					PBEFieldPosition pkmn0OldPos = pkmn0.FieldPosition,
						pkmn1OldPos = pkmn1.FieldPosition;
					pkmn0.FieldPosition = PBEFieldPosition.Center;
					pkmn1.FieldPosition = PBEFieldPosition.Center;
					await BroadcastAutoCenter(pkmn0, pkmn0OldPos, pkmn1, pkmn1OldPos);
				}
			}

			TurnNumber++;
			await BroadcastTurnBegan();
			foreach (PBETeam team in Teams)
			{
				bool old = team.MonFaintedThisTurn; // Fire events in a specific order
				team.MonFaintedThisTurn = false;
				team.MonFaintedLastTurn = old;
			}

			SetBattleState(PBEBattleState.WaitingForActions);
			foreach (PBETrainer trainer in Trainers.Where(t => t.NumConsciousPkmn > 0))
			{
				await BroadcastActionsRequest(trainer);
			}
		}
	}
	private IEnumerable<PBEBattlePokemon> GetActingOrder(IEnumerable<PBEBattlePokemon> pokemon, bool ignoreItemsThatActivate)
	{
		var evaluated = new List<(PBEBattlePokemon Pokemon, float Speed)>(); // TODO: Full Incense, Lagging Tail, Stall, Quick Claw
		foreach (PBEBattlePokemon pkmn in pokemon)
		{
			float speed = pkmn.Speed * GetStatChangeModifier(pkmn.SpeedChange, false);

			switch (pkmn.Item)
			{
				case PBEItem.ChoiceScarf:
				{
					speed *= 1.5f;
					break;
				}
				case PBEItem.MachoBrace:
				case PBEItem.PowerAnklet:
				case PBEItem.PowerBand:
				case PBEItem.PowerBelt:
				case PBEItem.PowerBracer:
				case PBEItem.PowerLens:
				case PBEItem.PowerWeight:
				{
					speed *= 0.5f;
					break;
				}
				case PBEItem.QuickPowder:
				{
					if (pkmn.OriginalSpecies == PBESpecies.Ditto && !pkmn.Status2.HasFlag(PBEStatus2.Transformed))
					{
						speed *= 2.0f;
					}
					break;
				}
			}
			if (ShouldDoWeatherEffects())
			{
				if (Weather == PBEWeather.HarshSunlight && pkmn.Ability == PBEAbility.Chlorophyll)
				{
					speed *= 2.0f;
				}
				else if (Weather == PBEWeather.Rain && pkmn.Ability == PBEAbility.SwiftSwim)
				{
					speed *= 2.0f;
				}
				else if (Weather == PBEWeather.Sandstorm && pkmn.Ability == PBEAbility.SandRush)
				{
					speed *= 2.0f;
				}
			}
			switch (pkmn.Ability)
			{
				case PBEAbility.QuickFeet:
				{
					if (pkmn.Status1 != PBEStatus1.None)
					{
						speed *= 1.5f;
					}
					break;
				}
				case PBEAbility.SlowStart:
				{
					if (pkmn.SlowStart_HinderTurnsLeft > 0)
					{
						speed *= 0.5f;
					}
					break;
				}
			}
			if (pkmn.Ability != PBEAbility.QuickFeet && pkmn.Status1 == PBEStatus1.Paralyzed)
			{
				speed *= 0.25f;
			}
			if (pkmn.Team.TeamStatus.HasFlag(PBETeamStatus.Tailwind))
			{
				speed *= 2.0f;
			}

			(PBEBattlePokemon Pokemon, float Speed) tup = (pkmn, speed);
			if (evaluated.Count == 0)
			{
				evaluated.Add(tup);
			}
			else
			{
				int pkmnTiedWith = evaluated.FindIndex(t => t.Speed == speed);
				if (pkmnTiedWith != -1) // Speed tie - randomly go before or after the Pokémon it tied with
				{
					if (_rand.RandomBool())
					{
						if (pkmnTiedWith == evaluated.Count - 1)
						{
							evaluated.Add(tup);
						}
						else
						{
							evaluated.Insert(pkmnTiedWith + 1, tup);
						}
					}
					else
					{
						evaluated.Insert(pkmnTiedWith, tup);
					}
				}
				else
				{
					int pkmnToGoBefore = evaluated.FindIndex(t => BattleStatus.HasFlag(PBEBattleStatus.TrickRoom) ? t.Speed > speed : t.Speed < speed);
					if (pkmnToGoBefore == -1)
					{
						evaluated.Add(tup);
					}
					else
					{
						evaluated.Insert(pkmnToGoBefore, tup);
					}
				}
			}
		}
		return evaluated.Select(t => t.Pokemon);
	}
	private void DetermineTurnOrder()
	{
		static int GetMovePrio(PBEBattlePokemon p)
		{
			IPBEMoveData mData = PBEDataProvider.Instance.GetMoveData(p.TurnAction!.FightMove);
			int priority = mData.Priority;
			if (p.Ability == PBEAbility.Prankster && mData.Category == PBEMoveCategory.Status)
			{
				priority++;
			}
			return priority;
		}

		_turnOrder.Clear();
		//const int PursuitPriority = +7;
		const int SwitchRotatePriority = +6;
		const int WildFleePriority = -7;
		List<PBEBattlePokemon> pkmnUsingItem = ActiveBattlers.FindAll(p => p.TurnAction?.Decision == PBETurnDecision.Item);
		List<PBEBattlePokemon> pkmnSwitchingOut = ActiveBattlers.FindAll(p => p.TurnAction?.Decision == PBETurnDecision.SwitchOut);
		List<PBEBattlePokemon> pkmnFighting = ActiveBattlers.FindAll(p => p.TurnAction?.Decision == PBETurnDecision.Fight);
		List<PBEBattlePokemon> wildFleeing = ActiveBattlers.FindAll(p => p.TurnAction?.Decision == PBETurnDecision.WildFlee);
		// Item use happens first:
		_turnOrder.AddRange(GetActingOrder(pkmnUsingItem, true));
		// Get move/switch/rotate/wildflee priority sorted
		IOrderedEnumerable<IGrouping<int, PBEBattlePokemon>> prios =
				pkmnSwitchingOut.Select(p => (p, SwitchRotatePriority))
				.Concat(pkmnFighting.Select(p => (p, GetMovePrio(p)))) // Get move priority
                .Concat(wildFleeing.Select(p => (p, WildFleePriority)))
				.GroupBy(t => t.Item2, t => t.p)
				.OrderByDescending(t => t.Key);
		foreach (IGrouping<int, PBEBattlePokemon> bracket in prios)
		{
			bool ignoreItemsThatActivate = bracket.Key == SwitchRotatePriority || bracket.Key == WildFleePriority;
			_turnOrder.AddRange(GetActingOrder(bracket, ignoreItemsThatActivate));
		}
	}
	private async Task RunActionsInOrder()
	{
		foreach (PBEBattlePokemon pkmn in _turnOrder.ToArray()) // Copy the list so a faint or ejection does not cause a collection modified exception
		{
			if (BattleResult is not null) // Do not broadcast battle result by calling EndCheck() in here; do it in TurnEnded()
			{
				return;
			}

			else if (ActiveBattlers.Contains(pkmn))
			{
				switch (pkmn.TurnAction!.Decision)
				{
					case PBETurnDecision.Fight:
					{
						await UseMove(pkmn, pkmn.TurnAction.FightMove, pkmn.TurnAction.FightTargets);
						break;
					}
					case PBETurnDecision.Item:
					{
						await UseItem(pkmn, pkmn.TurnAction.UseItem);
						break;
					}
					case PBETurnDecision.SwitchOut:
					{
						await SwitchTwoPokemon(pkmn, pkmn.Trainer.GetPokemon(pkmn.TurnAction.SwitchPokemonId));
						break;
					}
					case PBETurnDecision.WildFlee:
					{
						WildFleeCheck(pkmn);
						break;
					}
					default: throw new ArgumentOutOfRangeException(nameof(pkmn.TurnAction.Decision));
				}
			}
		}
	}
	private async Task TurnEnded()
	{
		if (await EndCheck())
		{
			return;
		}

		// Verified: Effects before LightScreen/LuckyChant/Reflect/Safeguard/TrickRoom
		await DoTurnEndedEffects();

		if (await EndCheck())
		{
			return;
		}

		// Verified: LightScreen/LuckyChant/Reflect/Safeguard/TrickRoom are removed in the order they were added
		foreach (PBETeam team in Teams)
		{
			if (team.TeamStatus.HasFlag(PBETeamStatus.LightScreen))
			{
				team.LightScreenCount--;
				if (team.LightScreenCount == 0)
				{
					await BroadcastTeamStatus(team, PBETeamStatus.LightScreen, PBETeamStatusAction.Ended);
				}
			}
			if (team.TeamStatus.HasFlag(PBETeamStatus.LuckyChant))
			{
				team.LuckyChantCount--;
				if (team.LuckyChantCount == 0)
				{
					await BroadcastTeamStatus(team, PBETeamStatus.LuckyChant, PBETeamStatusAction.Ended);
				}
			}
			if (team.TeamStatus.HasFlag(PBETeamStatus.Reflect))
			{
				team.ReflectCount--;
				if (team.ReflectCount == 0)
				{
					await BroadcastTeamStatus(team, PBETeamStatus.Reflect, PBETeamStatusAction.Ended);
				}
			}
			if (team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
			{
				team.SafeguardCount--;
				if (team.SafeguardCount == 0)
				{
					await BroadcastTeamStatus(team, PBETeamStatus.Safeguard, PBETeamStatusAction.Ended);
				}
			}
			if (team.TeamStatus.HasFlag(PBETeamStatus.Tailwind))
			{
				team.TailwindCount--;
				if (team.TailwindCount == 0)
				{
					await BroadcastTeamStatus(team, PBETeamStatus.Tailwind, PBETeamStatusAction.Ended);
				}
			}
		}
		// Trick Room
		if (BattleStatus.HasFlag(PBEBattleStatus.TrickRoom))
		{
			TrickRoomCount--;
			if (TrickRoomCount == 0)
			{
				await BroadcastBattleStatus(PBEBattleStatus.TrickRoom, PBEBattleStatusAction.Ended);
			}
		}

		await SwitchesOrActions();
	}
}
