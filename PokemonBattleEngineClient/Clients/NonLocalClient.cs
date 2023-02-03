using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System.IO;
using System.Threading;

namespace Kermalis.PokemonBattleEngineClient.Clients;

internal abstract class NonLocalClient : BattleClient
{
	protected NonLocalClient(string name) : base(name) { }

	public override void Dispose()
	{
		_stopPacketThread = true;
	}

	#region Automatic packet processing
	private int _currentPacket = -1;
	private Thread? _packetThread;
	private readonly object _packetThreadLockObj = new();
	private bool _plsStartPacketThreadForMe = false;
	private bool _stopPacketThread = false;

	private void CreateThread__Unsafe()
	{
		_packetThread = new Thread(PacketThread) { Name = "Packet Thread" };
		_packetThread.Start();
	}
	protected void StartPacketThread()
	{
		lock (_packetThreadLockObj)
		{
			_stopPacketThread = false;
			if (_packetThread is null)
			{
				CreateThread__Unsafe();
			}
			else
			{
				_plsStartPacketThreadForMe = true;
			}
		}
	}
	private void PacketThread()
	{
		while (!_stopPacketThread && _currentPacket < Battle.Events.Count - 1)
		{
			_plsStartPacketThreadForMe = false;
			_currentPacket++;
			if (!ProcessPacket(Battle.Events[_currentPacket]))
			{
				Thread.Sleep(WaitMilliseconds);
			}
		}
		lock (_packetThreadLockObj)
		{
			if (_plsStartPacketThreadForMe)
			{
				CreateThread__Unsafe();
			}
			else
			{
				_packetThread = null;
			}
		}
	}

	private static void DoDisguisedAppearance(PBEBattlePokemon pkmn, PBEPkmnAppearedInfo info)
	{
		if (info.IsDisguised)
		{
			pkmn.Status2 |= PBEStatus2.Disguised;
			pkmn.KnownCaughtBall = info.CaughtBall;
			pkmn.KnownGender = info.Gender;
			pkmn.KnownNickname = info.Nickname;
			pkmn.KnownShiny = info.Shiny;
			pkmn.KnownSpecies = info.Species;
			pkmn.KnownForm = info.Form;
			IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(info);
			pkmn.KnownType1 = pData.Type1;
			pkmn.KnownType2 = pData.Type2;
		}
	}
	protected override bool ProcessPacket(IPBEPacket packet)
	{
		switch (packet)
		{
			case PBEAbilityPacket ap:
			{
				PBEBattlePokemon abilityOwner = ap.AbilityOwnerTrainer.GetPokemon(ap.AbilityOwner);
				abilityOwner.Ability = ap.Ability;
				abilityOwner.KnownAbility = ap.Ability;
				break;
			}
			case PBEAbilityReplacedPacket arp:
			{
				PBEBattlePokemon abilityOwner = arp.AbilityOwnerTrainer.GetPokemon(arp.AbilityOwner);
				abilityOwner.Ability = arp.NewAbility;
				abilityOwner.KnownAbility = arp.NewAbility;
				break;
			}
			case PBEBattleStatusPacket bsp:
			{
				switch (bsp.BattleStatusAction)
				{
					case PBEBattleStatusAction.Added: Battle.BattleStatus |= bsp.BattleStatus; break;
					case PBEBattleStatusAction.Cleared:
					case PBEBattleStatusAction.Ended: Battle.BattleStatus &= ~bsp.BattleStatus; break;
					default: throw new InvalidDataException(nameof(bsp.BattleStatusAction));
				}
				break;
			}
			case PBECapturePacket cp:
			{
				if (cp.Success)
				{
					PBEBattlePokemon pokemon = cp.PokemonTrainer.GetPokemon(cp.Pokemon);
					pokemon.CaughtBall = cp.Ball;
					pokemon.KnownCaughtBall = cp.Ball;
				}
				break;
			}
			case PBEHazePacket _:
			{
				foreach (PBEBattlePokemon pkmn in Battle.ActiveBattlers)
				{
					pkmn.ClearStatChanges();
				}
				break;
			}
			case PBEIllusionPacket ilp:
			{
				PBEBattlePokemon pokemon = ilp.PokemonTrainer.GetPokemon(ilp.Pokemon);
				pokemon.Ability = pokemon.KnownAbility = PBEAbility.Illusion;
				pokemon.Gender = pokemon.KnownGender = ilp.ActualGender;
				pokemon.CaughtBall = pokemon.KnownCaughtBall = ilp.ActualCaughtBall;
				pokemon.Nickname = pokemon.KnownNickname = ilp.ActualNickname;
				pokemon.Shiny = pokemon.KnownShiny = ilp.ActualShiny;
				pokemon.Species = pokemon.KnownSpecies = ilp.ActualSpecies;
				pokemon.Form = pokemon.KnownForm = ilp.ActualForm;
				pokemon.Type1 = pokemon.KnownType1 = ilp.ActualType1;
				pokemon.Type2 = pokemon.KnownType2 = ilp.ActualType2;
				pokemon.Weight = pokemon.KnownWeight = ilp.ActualWeight;
				return true;
			}
			case PBEItemPacket ip:
			{
				PBEBattlePokemon itemHolder = ip.ItemHolderTrainer.GetPokemon(ip.ItemHolder);
				switch (ip.ItemAction)
				{
					case PBEItemAction.Announced:
					case PBEItemAction.Damage:
					case PBEItemAction.RestoredHP: itemHolder.Item = itemHolder.KnownItem = ip.Item; break;
					case PBEItemAction.Consumed: itemHolder.Item = itemHolder.KnownItem = PBEItem.None; break;
					default: throw new InvalidDataException(nameof(ip.ItemAction));
				}
				break;
			}
			case PBEMoveLockPacket mlp:
			{
				PBEBattlePokemon moveUser = mlp.MoveUserTrainer.GetPokemon(mlp.MoveUser);
				switch (mlp.MoveLockType)
				{
					case PBEMoveLockType.ChoiceItem: moveUser.ChoiceLockedMove = mlp.LockedMove; break;
					case PBEMoveLockType.Temporary: moveUser.TempLockedMove = mlp.LockedMove; break;
					default: throw new InvalidDataException(nameof(mlp.MoveLockType));
				}
				if (mlp.LockedTargets is not null)
				{
					moveUser.TempLockedTargets = mlp.LockedTargets.Value;
				}
				return true;
			}
			case PBEMovePPChangedPacket mpcp:
			{
				PBEBattlePokemon moveUser = mpcp.MoveUserTrainer.GetPokemon(mpcp.MoveUser);
				moveUser.UpdateKnownPP(mpcp.Move, mpcp.AmountReduced);
				return true;
			}
			case PBEMoveUsedPacket mup:
			{
				PBEBattlePokemon moveUser = mup.MoveUserTrainer.GetPokemon(mup.MoveUser);
				if (mup.Owned && !moveUser.KnownMoves.Contains(mup.Move))
				{
					moveUser.KnownMoves[PBEMove.MAX]!.Move = mup.Move;
					PBEBattleMoveset.PBEBattleMovesetSlot? slot = moveUser.Moves[PBEMove.MAX];
					if (slot is not null)
					{
						slot.Move = mup.Move; // Copy to Moves as well so Transform doesn't break for spectators/allies
					}
				}
				break;
			}
			case PBEPkmnEXPChangedPacket pecp:
			{
				PBEBattlePokemon pokemon = pecp.PokemonTrainer.GetPokemon(pecp.Pokemon);
				pokemon.EXP = pecp.NewEXP;
				break;
			}
			case PBEPkmnFaintedPacket pfp:
			{
				bool ret = base.ProcessPacket(packet); // Process before removal
				PBEBattlePokemon pokemon = pfp.PokemonTrainer.GetPokemon(pfp.Pokemon);
				Battle.ActiveBattlers.Remove(pokemon);
				pokemon.ClearForFaint();
				return ret;
			}
			case PBEPkmnFormChangedPacket pfcp:
			{
				PBEBattlePokemon pokemon = pfcp.PokemonTrainer.GetPokemon(pfcp.Pokemon);
				pokemon.HP = pfcp.NewHP;
				pokemon.MaxHP = pfcp.NewMaxHP;
				pokemon.HPPercentage = pfcp.NewHPPercentage;
				pokemon.Attack = pfcp.NewAttack;
				pokemon.Defense = pfcp.NewDefense;
				pokemon.SpAttack = pfcp.NewSpAttack;
				pokemon.SpDefense = pfcp.NewSpDefense;
				pokemon.Speed = pfcp.NewSpeed;
				pokemon.Ability = pfcp.NewAbility;
				pokemon.KnownAbility = pfcp.NewKnownAbility;
				pokemon.Form = pokemon.KnownForm = pfcp.NewForm;
				pokemon.Type1 = pokemon.KnownType1 = pfcp.NewType1;
				pokemon.Type2 = pokemon.KnownType2 = pfcp.NewType2;
				pokemon.Weight = pokemon.KnownWeight = pfcp.NewWeight;
				if (pfcp.IsRevertForm)
				{
					pokemon.RevertForm = pfcp.NewForm;
					pokemon.RevertAbility = pfcp.NewAbility;
				}
				break;
			}
			case PBEPkmnHPChangedPacket phcp:
			{
				PBEBattlePokemon pokemon = phcp.PokemonTrainer.GetPokemon(phcp.Pokemon);
				pokemon.HP = phcp.NewHP;
				pokemon.HPPercentage = phcp.NewHPPercentage;
				break;
			}
			case PBEPkmnLevelChangedPacket plcp:
			{
				PBEBattlePokemon pokemon = plcp.PokemonTrainer.GetPokemon(plcp.Pokemon);
				pokemon.Level = plcp.NewLevel;
				break;
			}
			case PBEPkmnStatChangedPacket pscp:
			{
				PBEBattlePokemon pokemon = pscp.PokemonTrainer.GetPokemon(pscp.Pokemon);
				pokemon.SetStatChange(pscp.Stat, pscp.NewValue);
				break;
			}
			case PBEPkmnSwitchInPacket psip:
			{
				foreach (PBEPkmnAppearedInfo info in psip.SwitchIns)
				{
					PBEBattlePokemon pokemon = psip.Trainer.GetPokemon(info.Pokemon);
					pokemon.FieldPosition = info.FieldPosition;
					PBETrainer.SwitchTwoPokemon(pokemon, info.FieldPosition);
					DoDisguisedAppearance(pokemon, info);
					Battle.ActiveBattlers.Add(pokemon);
				}
				break;
			}
			case PBEPkmnSwitchOutPacket psop:
			{
				bool ret = base.ProcessPacket(packet); // Process before removal
				PBEBattlePokemon pokemon = psop.PokemonTrainer.GetPokemon(psop.Pokemon);
				Battle.ActiveBattlers.Remove(pokemon);
				pokemon.ClearForSwitch();
				return ret;
			}
			case PBEPsychUpPacket pup:
			{
				PBEBattlePokemon user = pup.UserTrainer.GetPokemon(pup.User);
				PBEBattlePokemon target = pup.TargetTrainer.GetPokemon(pup.Target);
				user.AttackChange = target.AttackChange = pup.AttackChange;
				user.DefenseChange = target.DefenseChange = pup.DefenseChange;
				user.SpAttackChange = target.SpAttackChange = pup.SpAttackChange;
				user.SpDefenseChange = target.SpDefenseChange = pup.SpDefenseChange;
				user.SpeedChange = target.SpeedChange = pup.SpeedChange;
				user.AccuracyChange = target.AccuracyChange = pup.AccuracyChange;
				user.EvasionChange = target.EvasionChange = pup.EvasionChange;
				break;
			}
			case PBEReflectTypePacket rtp:
			{
				PBEBattlePokemon user = rtp.UserTrainer.GetPokemon(rtp.User);
				PBEBattlePokemon target = rtp.TargetTrainer.GetPokemon(rtp.Target);
				user.Type1 = user.KnownType1 = target.KnownType1 = target.Type1 = rtp.Type1;
				user.Type2 = user.KnownType2 = target.KnownType2 = target.Type2 = rtp.Type2;
				break;
			}
			case PBEStatus1Packet s1p:
			{
				PBEBattlePokemon status1Receiver = s1p.Status1ReceiverTrainer.GetPokemon(s1p.Status1Receiver);
				switch (s1p.StatusAction)
				{
					case PBEStatusAction.Added:
					case PBEStatusAction.Announced:
					case PBEStatusAction.CausedImmobility:
					case PBEStatusAction.Damage: status1Receiver.Status1 = s1p.Status1; break;
					case PBEStatusAction.Cleared:
					case PBEStatusAction.Ended: status1Receiver.Status1 = PBEStatus1.None; break;
					default: throw new InvalidDataException(nameof(s1p.StatusAction));
				}
				break;
			}
			case PBEStatus2Packet s2p:
			{
				PBEBattlePokemon status2Receiver = s2p.Status2ReceiverTrainer.GetPokemon(s2p.Status2Receiver);
				PBEBattlePokemon pokemon2 = s2p.Pokemon2Trainer.GetPokemon(s2p.Pokemon2);
				switch (s2p.StatusAction)
				{
					case PBEStatusAction.Added:
					case PBEStatusAction.Announced:
					case PBEStatusAction.CausedImmobility:
					case PBEStatusAction.Damage: status2Receiver.Status2 |= s2p.Status2; status2Receiver.KnownStatus2 |= s2p.Status2; break;
					case PBEStatusAction.Cleared:
					case PBEStatusAction.Ended: status2Receiver.Status2 &= ~s2p.Status2; status2Receiver.KnownStatus2 &= ~s2p.Status2; break;
					default: throw new InvalidDataException(nameof(s2p.StatusAction));
				}
				switch (s2p.Status2)
				{
					case PBEStatus2.Infatuated:
					{
						switch (s2p.StatusAction)
						{
							case PBEStatusAction.Added: status2Receiver.InfatuatedWithPokemon = pokemon2; break;
							case PBEStatusAction.Cleared:
							case PBEStatusAction.Ended: status2Receiver.InfatuatedWithPokemon = null; break;
						}
						break;
					}
					case PBEStatus2.LeechSeed:
					{
						switch (s2p.StatusAction)
						{
							case PBEStatusAction.Added: status2Receiver.SeededPosition = pokemon2.FieldPosition; status2Receiver.SeededTeam = pokemon2.Team; break;
						}
						break;
					}
					case PBEStatus2.LockOn:
					{
						switch (s2p.StatusAction)
						{
							case PBEStatusAction.Added: status2Receiver.LockOnPokemon = pokemon2; break;
							case PBEStatusAction.Ended: status2Receiver.LockOnPokemon = null; break;
						}
						break;
					}
					case PBEStatus2.PowerTrick:
					{
						switch (s2p.StatusAction)
						{
							case PBEStatusAction.Added: status2Receiver.ApplyPowerTrickChange(); break;
						}
						break;
					}
					case PBEStatus2.Roost:
					{
						switch (s2p.StatusAction)
						{
							case PBEStatusAction.Added: status2Receiver.StartRoost(); break;
							case PBEStatusAction.Ended: status2Receiver.EndRoost(); break;
						}
						break;
					}
					case PBEStatus2.Transformed:
					{
						switch (s2p.StatusAction)
						{
							case PBEStatusAction.Added: status2Receiver.Transform(pokemon2); break;
						}
						break;
					}
				}
				break;
			}
			case PBETeamStatusPacket tsp:
			{
				PBETeam team = tsp.Team;
				switch (tsp.TeamStatusAction)
				{
					case PBETeamStatusAction.Added:
					case PBETeamStatusAction.Cleared:
					case PBETeamStatusAction.Ended: team.TeamStatus &= ~tsp.TeamStatus; break;
					default: throw new InvalidDataException(nameof(tsp.TeamStatusAction));
				}
				switch (tsp.TeamStatus)
				{
					case PBETeamStatus.Spikes:
					{
						switch (tsp.TeamStatusAction)
						{
							case PBETeamStatusAction.Added: team.SpikeCount++; break;
								//case PBETeamStatusAction.Cleared: team.SpikeCount = 0; break;
						}
						break;
					}
					case PBETeamStatus.ToxicSpikes:
					{
						switch (tsp.TeamStatusAction)
						{
							case PBETeamStatusAction.Added: team.ToxicSpikeCount++; break;
							case PBETeamStatusAction.Cleared: team.ToxicSpikeCount = 0; break;
						}
						break;
					}
				}
				break;
			}
			case PBETeamStatusDamagePacket tsdp:
			{
				tsdp.Team.TeamStatus |= tsdp.TeamStatus;
				break;
			}
			case PBETransformPacket tp:
			{
				PBEBattlePokemon target = tp.TargetTrainer.GetPokemon(tp.Target);
				target.Attack = tp.TargetAttack;
				target.Defense = tp.TargetDefense;
				target.SpAttack = tp.TargetSpAttack;
				target.SpDefense = tp.TargetSpDefense;
				target.Speed = tp.TargetSpeed;
				target.AttackChange = tp.TargetAttackChange;
				target.DefenseChange = tp.TargetDefenseChange;
				target.SpAttackChange = tp.TargetSpAttackChange;
				target.SpDefenseChange = tp.TargetSpDefenseChange;
				target.SpeedChange = tp.TargetSpeedChange;
				target.AccuracyChange = tp.TargetAccuracyChange;
				target.EvasionChange = tp.TargetEvasionChange;
				target.Ability = target.KnownAbility = tp.TargetAbility;
				for (int i = 0; i < Battle.Settings.NumMoves; i++)
				{
					target.Moves[i].Move = tp.TargetMoves[i];
				}
				target.Species = target.KnownSpecies = tp.TargetSpecies;
				target.Form = target.KnownForm = tp.TargetForm;
				target.Type1 = target.KnownType1 = tp.TargetType1;
				target.Type2 = target.KnownType2 = tp.TargetType2;
				target.Weight = target.KnownWeight = tp.TargetWeight;
				return true;
			}
			case PBETypeChangedPacket tcp:
			{
				PBEBattlePokemon pokemon = tcp.PokemonTrainer.GetPokemon(tcp.Pokemon);
				pokemon.Type1 = pokemon.KnownType1 = tcp.Type1;
				pokemon.Type2 = pokemon.KnownType2 = tcp.Type2;
				break;
			}
			case PBEWeatherPacket wp:
			{
				switch (wp.WeatherAction)
				{
					case PBEWeatherAction.Added:
					case PBEWeatherAction.Ended: Battle.Weather = PBEWeather.None; break;
					default: throw new InvalidDataException(nameof(wp.WeatherAction));
				}
				break;
			}
			case PBEWeatherDamagePacket wdp:
			{
				Battle.Weather = wdp.Weather;
				break;
			}
			case PBEWildPkmnAppearedPacket wpap:
			{
				PBETrainer wildTrainer = Battle.Teams[1].Trainers[0];
				foreach (PBEPkmnAppearedInfo info in wpap.Pokemon)
				{
					PBEBattlePokemon pokemon = wildTrainer.GetPokemon(info.Pokemon);
					DoDisguisedAppearance(pokemon, info);
				}
				break;
			}
			case IPBEAutoCenterPacket acp:
			{
				PBEBattlePokemon pokemon0 = acp.Pokemon0Trainer.GetPokemon(acp.Pokemon0OldPosition);
				PBEBattlePokemon pokemon1 = acp.Pokemon1Trainer.GetPokemon(acp.Pokemon1OldPosition);
				pokemon0.FieldPosition = PBEFieldPosition.Center;
				pokemon1.FieldPosition = PBEFieldPosition.Center;
				break;
			}
			case PBEBattleResultPacket brp:
			{
				Battle.BattleResult = brp.BattleResult;
				break;
			}
			case PBETurnBeganPacket tbp:
			{
				Battle.TurnNumber = tbp.TurnNumber;
				break;
			}
		}
		return base.ProcessPacket(packet);
	}
	#endregion
}
