using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Threading;

namespace Kermalis.PokemonBattleEngineClient.Clients
{
    internal abstract class NonLocalClient : BattleClient
    {
        public override void Dispose()
        {
            _stopPacketThread = true;
        }

        #region Automatic packet processing
        private int _currentPacket = -1;
        private Thread _packetThread;
        private readonly object _packetThreadLockObj = new object();
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
                if (_packetThread == null)
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
        protected override bool ProcessPacket(IPBEPacket packet)
        {
            switch (packet)
            {
                case PBEAbilityPacket ap:
                {
                    PBEBattlePokemon abilityOwner = ap.AbilityOwner;
                    abilityOwner.Ability = ap.Ability;
                    abilityOwner.KnownAbility = ap.Ability;
                    break;
                }
                case PBEAbilityReplacedPacket arp:
                {
                    PBEBattlePokemon abilityOwner = arp.AbilityOwner;
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
                        default: throw new ArgumentOutOfRangeException(nameof(bsp.BattleStatusAction));
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
                    PBEBattlePokemon pokemon = ilp.Pokemon;
                    pokemon.DisguisedAsPokemon = null;
                    pokemon.Ability = pokemon.KnownAbility = PBEAbility.Illusion;
                    pokemon.Gender = pokemon.KnownGender = ilp.ActualGender;
                    pokemon.Nickname = pokemon.KnownNickname = ilp.ActualNickname;
                    pokemon.Shiny = pokemon.KnownShiny = ilp.ActualShiny;
                    pokemon.OriginalSpecies = pokemon.Species = pokemon.KnownSpecies = ilp.ActualSpecies;
                    pokemon.Form = pokemon.KnownForm = ilp.ActualForm;
                    pokemon.Type1 = pokemon.KnownType1 = ilp.ActualType1;
                    pokemon.Type2 = pokemon.KnownType2 = ilp.ActualType2;
                    pokemon.Weight = pokemon.KnownWeight = ilp.ActualWeight;
                    return true;
                }
                case PBEItemPacket ip:
                {
                    PBEBattlePokemon itemHolder = ip.ItemHolder;
                    switch (ip.ItemAction)
                    {
                        case PBEItemAction.ChangedStatus:
                        case PBEItemAction.Damage:
                        case PBEItemAction.RestoredHP: itemHolder.Item = itemHolder.KnownItem = ip.Item; break;
                        case PBEItemAction.Consumed: itemHolder.Item = itemHolder.KnownItem = PBEItem.None; break;
                        default: throw new ArgumentOutOfRangeException(nameof(ip.ItemAction));
                    }
                    break;
                }
                case PBEMoveLockPacket mlp:
                {
                    PBEBattlePokemon moveUser = mlp.MoveUser;
                    switch (mlp.MoveLockType)
                    {
                        case PBEMoveLockType.ChoiceItem: moveUser.ChoiceLockedMove = mlp.LockedMove; break;
                        case PBEMoveLockType.Temporary: moveUser.TempLockedMove = mlp.LockedMove; break;
                        default: throw new ArgumentOutOfRangeException(nameof(mlp.MoveLockType));
                    }
                    if (mlp.LockedTargets.HasValue)
                    {
                        moveUser.TempLockedTargets = mlp.LockedTargets.Value;
                    }
                    return true;
                }
                case PBEMovePPChangedPacket mpcp:
                {
                    mpcp.MoveUser.UpdateKnownPP(mpcp.Move, mpcp.AmountReduced);
                    return true;
                }
                case PBEMoveUsedPacket mup:
                {
                    if (mup.Reveal)
                    {
                        mup.MoveUser.KnownMoves[PBEMove.MAX].Move = mup.Move;
                    }
                    break;
                }
                case PBEPkmnFaintedPacket pfp:
                {
                    PBEBattlePokemon pokemon = pfp.Pokemon;
                    Battle.ActiveBattlers.Remove(pokemon);
                    pokemon.FieldPosition = PBEFieldPosition.None;
                    break;
                }
                case PBEPkmnFaintedPacket_Hidden pfph:
                {
                    PBEBattlePokemon pokemon = pfph.Pokemon;
                    Battle.ActiveBattlers.Remove(pokemon);
                    pokemon.FieldPosition = PBEFieldPosition.None;
                    PBETrainer.Remove(pokemon);
                    break;
                }
                case PBEPkmnFormChangedPacket pfcp:
                {
                    PBEBattlePokemon pokemon = pfcp.Pokemon;
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
                case PBEPkmnFormChangedPacket_Hidden pfcph:
                {
                    PBEBattlePokemon pokemon = pfcph.Pokemon;
                    pokemon.KnownAbility = pfcph.NewKnownAbility;
                    pokemon.KnownForm = pfcph.NewForm;
                    pokemon.KnownType1 = pfcph.NewType1;
                    pokemon.KnownType2 = pfcph.NewType2;
                    pokemon.KnownWeight = pfcph.NewWeight;
                    break;
                }
                case PBEPkmnHPChangedPacket phcp:
                {
                    PBEBattlePokemon pokemon = phcp.Pokemon;
                    pokemon.HP = phcp.NewHP;
                    pokemon.HPPercentage = phcp.NewHPPercentage;
                    break;
                }
                case PBEPkmnHPChangedPacket_Hidden phcph:
                {
                    phcph.Pokemon.HPPercentage = phcph.NewHPPercentage;
                    break;
                }
                case PBEPkmnStatChangedPacket pscp:
                {
                    pscp.Pokemon.SetStatChange(pscp.Stat, pscp.NewValue);
                    break;
                }
                case PBEPkmnSwitchInPacket psip:
                {
                    foreach (PBEPkmnSwitchInPacket.PBESwitchInInfo info in psip.SwitchIns)
                    {
                        PBEBattlePokemon pokemon = info.Pokemon;
                        pokemon.FieldPosition = info.FieldPosition;
                        PBETrainer.SwitchTwoPokemon(pokemon, info.FieldPosition);
                        if (info.DisguisedAsPokemon != pokemon)
                        {
                            pokemon.Status2 |= PBEStatus2.Disguised;
                            pokemon.DisguisedAsPokemon = info.DisguisedAsPokemon;
                            pokemon.KnownGender = pokemon.DisguisedAsPokemon.Gender;
                            pokemon.KnownNickname = pokemon.DisguisedAsPokemon.Nickname;
                            pokemon.KnownShiny = pokemon.DisguisedAsPokemon.Shiny;
                            pokemon.KnownSpecies = pokemon.DisguisedAsPokemon.OriginalSpecies;
                            pokemon.KnownForm = pokemon.DisguisedAsPokemon.Form;
                            var pData = PBEPokemonData.GetData(pokemon.KnownSpecies, pokemon.KnownForm);
                            pokemon.KnownType1 = pData.Type1;
                            pokemon.KnownType2 = pData.Type2;
                        }
                        Battle.ActiveBattlers.Add(pokemon);
                    }
                    break;
                }
                case PBEPkmnSwitchInPacket_Hidden psiph:
                {
                    foreach (PBEPkmnSwitchInPacket_Hidden.PBESwitchInInfo info in psiph.SwitchIns)
                    {
                        new PBEBattlePokemon(psiph.Trainer, info);
                    }
                    break;
                }
                case PBEPkmnSwitchOutPacket psop:
                {
                    PBEBattlePokemon pokemon = psop.Pokemon;
                    Battle.ActiveBattlers.Remove(pokemon);
                    pokemon.ClearForSwitch();
                    break;
                }
                case PBEPkmnSwitchOutPacket_Hidden psoph:
                {
                    PBEBattlePokemon pokemon = psoph.Pokemon;
                    Battle.ActiveBattlers.Remove(pokemon);
                    PBETrainer.Remove(pokemon);
                    break;
                }
                case PBEPsychUpPacket pup:
                {
                    PBEBattlePokemon user = pup.User;
                    PBEBattlePokemon target = pup.Target;
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
                    PBEBattlePokemon user = rtp.User;
                    PBEBattlePokemon target = rtp.Target;
                    PBEType type1 = rtp.Type1;
                    PBEType type2 = rtp.Type2;
                    user.Type1 = user.KnownType1 = target.KnownType1 = target.Type1 = type1;
                    user.Type2 = user.KnownType2 = target.KnownType2 = target.Type2 = type2;
                    break;
                }
                case PBEReflectTypePacket_Hidden rtph:
                {
                    PBEBattlePokemon user = rtph.User;
                    PBEBattlePokemon target = rtph.Target;
                    user.KnownType1 = target.KnownType1;
                    user.KnownType2 = target.KnownType2;
                    break;
                }
                case PBEStatus1Packet s1p:
                {
                    PBEBattlePokemon status1Receiver = s1p.Status1Receiver;
                    switch (s1p.StatusAction)
                    {
                        case PBEStatusAction.Added:
                        case PBEStatusAction.Announced:
                        case PBEStatusAction.CausedImmobility:
                        case PBEStatusAction.Damage: status1Receiver.Status1 = s1p.Status1; break;
                        case PBEStatusAction.Cleared:
                        case PBEStatusAction.Ended: status1Receiver.Status1 = PBEStatus1.None; break;
                        default: throw new ArgumentOutOfRangeException(nameof(s1p.StatusAction));
                    }
                    break;
                }
                case PBEStatus2Packet s2p:
                {
                    PBEBattlePokemon status2Receiver = s2p.Status2Receiver;
                    PBEBattlePokemon pokemon2 = s2p.Pokemon2;
                    switch (s2p.StatusAction)
                    {
                        case PBEStatusAction.Added:
                        case PBEStatusAction.Announced:
                        case PBEStatusAction.CausedImmobility:
                        case PBEStatusAction.Damage: status2Receiver.Status2 |= s2p.Status2; status2Receiver.KnownStatus2 |= s2p.Status2; break;
                        case PBEStatusAction.Cleared:
                        case PBEStatusAction.Ended: status2Receiver.Status2 &= ~s2p.Status2; status2Receiver.KnownStatus2 &= ~s2p.Status2; break;
                        default: throw new ArgumentOutOfRangeException(nameof(s2p.StatusAction));
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
                            switch (s2p.StatusAction)
                            {
                                case PBEStatusAction.Added: status2Receiver.Transform(pokemon2); break;
                            }
                            break;
                    }
                    break;
                }
                case PBETeamStatusPacket tsp:
                {
                    PBETeam team = tsp.Team;
                    switch (tsp.TeamStatusAction)
                    {
                        case PBETeamStatusAction.Added:
                        case PBETeamStatusAction.Damage: team.TeamStatus |= tsp.TeamStatus; break;
                        case PBETeamStatusAction.Cleared:
                        case PBETeamStatusAction.Ended: team.TeamStatus &= ~tsp.TeamStatus; break;
                        default: throw new ArgumentOutOfRangeException(nameof(tsp.TeamStatusAction));
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
                case PBETransformPacket tp:
                {
                    PBEBattlePokemon target = tp.Target;
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
                    target.Type1 = target.KnownType1 = tp.TargetType1;
                    target.Type2 = target.KnownType2 = tp.TargetType2;
                    target.Weight = target.KnownWeight = tp.TargetWeight;
                    return true;
                }
                case PBETypeChangedPacket tcp:
                {
                    PBEBattlePokemon pokemon = tcp.Pokemon;
                    PBEType type1 = tcp.Type1;
                    PBEType type2 = tcp.Type2;
                    pokemon.Type1 = type1;
                    pokemon.KnownType1 = type1;
                    pokemon.Type2 = type2;
                    pokemon.KnownType2 = type2;
                    break;
                }
                case PBEWeatherPacket wp:
                {
                    switch (wp.WeatherAction)
                    {
                        case PBEWeatherAction.Added:
                        case PBEWeatherAction.CausedDamage: Battle.Weather = wp.Weather; break;
                        case PBEWeatherAction.Ended: Battle.Weather = PBEWeather.None; break;
                        default: throw new ArgumentOutOfRangeException(nameof(wp.WeatherAction));
                    }
                    break;
                }
                case IPBEAutoCenterPacket acp:
                {
                    acp.Pokemon0.FieldPosition = PBEFieldPosition.Center;
                    acp.Pokemon1.FieldPosition = PBEFieldPosition.Center;
                    break;
                }
                case PBETurnBeganPacket tbp:
                {
                    Battle.TurnNumber = tbp.TurnNumber;
                    break;
                }
                case PBEWinnerPacket win:
                {
                    Battle.Winner = win.WinningTeam;
                    break;
                }
            }
            return base.ProcessPacket(packet);
        }
        #endregion
    }
}
