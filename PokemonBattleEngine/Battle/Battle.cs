using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    // TODO: Fully implement INPC
    /// <summary>Represents a specific Pokémon battle.</summary>
    public sealed partial class PBEBattle : INotifyPropertyChanged
    {
        // Currently unused
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void BattleStateChangedEvent(PBEBattle battle);
        public event BattleStateChangedEvent OnStateChanged;
        public PBEBattleState BattleState { get; private set; }
        public ushort TurnNumber { get; set; }
        /// <summary>The winner of the battle. null if <see cref="BattleState"/> is not <see cref="PBEBattleState.Ended"/>.</summary>
        public PBETeam Winner { get; set; }

        public PBEBattleTerrain BattleTerrain { get; }
        public PBEBattleFormat BattleFormat { get; }
        public PBESettings Settings { get; }
        public PBETeams Teams { get; }
        public ReadOnlyCollection<PBETrainer> Trainers { get; }
        public List<PBEBattlePokemon> ActiveBattlers { get; } = new List<PBEBattlePokemon>(6);
        private readonly List<PBEBattlePokemon> _turnOrder = new List<PBEBattlePokemon>(6);

        public PBEWeather Weather { get; set; }
        public byte WeatherCounter { get; set; }
        public PBEBattleStatus BattleStatus { get; set; }
        public byte TrickRoomCount { get; set; }

        public List<IPBEPacket> Events { get; } = new List<IPBEPacket>();

        public PBEBattle(PBEBattleFormat battleFormat, PBESettings settings, PBETrainerInfo ti0, PBETrainerInfo ti1, PBEBattleTerrain battleTerrain = PBEBattleTerrain.Plain, PBEWeather weather = PBEWeather.None)
            : this(battleFormat, settings, new[] { ti0 }, new[] { ti1 }, battleTerrain: battleTerrain, weather: weather) { }
        public PBEBattle(PBEBattleFormat battleFormat, PBESettings settings, IReadOnlyList<PBETrainerInfo> ti0, IReadOnlyList<PBETrainerInfo> ti1, PBEBattleTerrain battleTerrain = PBEBattleTerrain.Plain, PBEWeather weather = PBEWeather.None)
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
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            if (ti0 == null || ti0.Any(t => t == null))
            {
                throw new ArgumentNullException(nameof(ti0));
            }
            if (ti1 == null || ti1.Any(t => t == null))
            {
                throw new ArgumentNullException(nameof(ti1));
            }
            BattleTerrain = battleTerrain;
            BattleFormat = battleFormat;
            Settings = settings;
            Weather = weather;
            Teams = new PBETeams(this, ti0, ti1, out ReadOnlyCollection<PBETrainer> trainers);
            Trainers = trainers;

            void QueueUp(PBETeam team, int i, PBEFieldPosition pos)
            {
                PBETrainer t;
                if (team.Trainers.Count == 1)
                {
                    t = team.Trainers[0];
                }
                else
                {
                    t = team.GetTrainer(pos);
                    i = 0;
                }
                PBEList<PBEBattlePokemon> party = t.Party;
                if (i < party.Count)
                {
                    PBEBattlePokemon p = party[i];
                    p.Trainer.SwitchInQueue.Add((p, pos));
                }
            }
            switch (BattleFormat)
            {
                case PBEBattleFormat.Single:
                {
                    foreach (PBETeam team in Teams)
                    {
                        QueueUp(team, 0, PBEFieldPosition.Center);
                    }
                    break;
                }
                case PBEBattleFormat.Double:
                {
                    foreach (PBETeam team in Teams)
                    {
                        QueueUp(team, 0, PBEFieldPosition.Left);
                        QueueUp(team, 1, PBEFieldPosition.Right);
                    }
                    break;
                }
                case PBEBattleFormat.Triple:
                {
                    foreach (PBETeam team in Teams)
                    {
                        QueueUp(team, 0, PBEFieldPosition.Left);
                        QueueUp(team, 1, PBEFieldPosition.Center);
                        QueueUp(team, 2, PBEFieldPosition.Right);
                    }
                    break;
                }
                case PBEBattleFormat.Rotation:
                {
                    foreach (PBETeam team in Teams)
                    {
                        QueueUp(team, 0, PBEFieldPosition.Center);
                        QueueUp(team, 1, PBEFieldPosition.Left);
                        QueueUp(team, 2, PBEFieldPosition.Right);
                    }
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(BattleFormat));
            }

            BattleState = PBEBattleState.ReadyToBegin;
            OnStateChanged?.Invoke(this);
        }
        public PBEBattle(PBEBattlePacket packet)
        {
            BattleFormat = packet.BattleFormat;
            BattleTerrain = packet.BattleTerrain;
            Weather = packet.Weather;
            Settings = packet.Settings;
            Teams = new PBETeams(this, packet, out ReadOnlyCollection<PBETrainer> trainers);
            Trainers = trainers;
        }
        /// <summary>Begins the battle.</summary>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.ReadyToBegin"/>.</exception>
        public void Begin()
        {
            if (BattleState != PBEBattleState.ReadyToBegin)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.ReadyToBegin} to begin the battle.");
            }
            BroadcastBattle(); // The first packet sent is PBEBattlePacket which replays rely on
            SwitchesOrActions();
        }
        /// <summary>Runs a turn.</summary>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.ReadyToRunTurn"/>.</exception>
        public void RunTurn()
        {
            if (BattleState != PBEBattleState.ReadyToRunTurn)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.ReadyToRunTurn} to run a turn.");
            }
            BattleState = PBEBattleState.Processing;
            OnStateChanged?.Invoke(this);
            DetermineTurnOrder();
            RunActionsInOrder();
            TurnEnded();
        }
        public void RunSwitches()
        {
            if (BattleState != PBEBattleState.ReadyToRunSwitches)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.ReadyToRunSwitches} to run switches.");
            }
            BattleState = PBEBattleState.Processing;
            OnStateChanged?.Invoke(this);
            SwitchesOrActions();
        }

        /// <summary>Sets <see cref="BattleState"/> to <see cref="PBEBattleState.Ended"/> and clears <see cref="OnNewEvent"/> and <see cref="OnStateChanged"/>. Does not touch <see cref="Winner"/>.</summary>
        public void SetEnded()
        {
            if (BattleState != PBEBattleState.Ended)
            {
                BattleState = PBEBattleState.Ended;
                OnStateChanged?.Invoke(this);
                OnNewEvent = null;
                OnStateChanged = null;
            }
        }
        private bool WinCheck()
        {
            if (Winner != null)
            {
                BroadcastWinner(Winner);
                SetEnded();
                return true;
            }
            return false;
        }
        private void SwitchesOrActions()
        {
            BattleState = PBEBattleState.Processing;
            OnStateChanged?.Invoke(this);

            // Checking SwitchInQueue count since SwitchInsRequired is set to 0 after submitting switches
            PBETrainer[] trainersWithSwitchIns = Trainers.Where(t => t.SwitchInQueue.Count > 0).ToArray();
            if (trainersWithSwitchIns.Length > 0)
            {
                var list = new List<PBEBattlePokemon>(6);
                foreach (PBETrainer trainer in trainersWithSwitchIns)
                {
                    int count = trainer.SwitchInQueue.Count;
                    var switches = new PBEPkmnSwitchInPacket.PBESwitchInInfo[count];
                    for (int i = 0; i < count; i++)
                    {
                        (PBEBattlePokemon pkmn, PBEFieldPosition pos) = trainer.SwitchInQueue[i];
                        pkmn.FieldPosition = pos;
                        switches[i] = CreateSwitchInInfo(pkmn);
                        PBETrainer.SwitchTwoPokemon(pkmn, pos);
                        ActiveBattlers.Add(pkmn); // Add before broadcast
                        list.Add(pkmn);
                    }
                    BroadcastPkmnSwitchIn(trainer, switches);
                }
                DoSwitchInEffects(list);
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
                            if (trainer.TryGetPokemon(PBEFieldPosition.Center) == null)
                            {
                                trainer.SwitchInsRequired = 1;
                            }
                            break;
                        }
                        case PBEBattleFormat.Double:
                        {
                            if (trainer.OwnsSpot(PBEFieldPosition.Left) && trainer.TryGetPokemon(PBEFieldPosition.Left) == null)
                            {
                                available--;
                                trainer.SwitchInsRequired++;
                            }
                            if (available > 0 && trainer.OwnsSpot(PBEFieldPosition.Right) && trainer.TryGetPokemon(PBEFieldPosition.Right) == null)
                            {
                                trainer.SwitchInsRequired++;
                            }
                            break;
                        }
                        case PBEBattleFormat.Rotation:
                        case PBEBattleFormat.Triple:
                        {
                            if (trainer.OwnsSpot(PBEFieldPosition.Left) && trainer.TryGetPokemon(PBEFieldPosition.Left) == null)
                            {
                                available--;
                                trainer.SwitchInsRequired++;
                            }
                            if (available > 0 && trainer.OwnsSpot(PBEFieldPosition.Center) && trainer.TryGetPokemon(PBEFieldPosition.Center) == null)
                            {
                                available--;
                                trainer.SwitchInsRequired++;
                            }
                            if (available > 0 && trainer.OwnsSpot(PBEFieldPosition.Right) && trainer.TryGetPokemon(PBEFieldPosition.Right) == null)
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
                BattleState = PBEBattleState.WaitingForSwitchIns;
                OnStateChanged?.Invoke(this);
                foreach (PBETrainer trainer in trainersWithSwitchIns)
                {
                    BroadcastSwitchInRequest(trainer);
                }
            }
            else
            {
                if (WinCheck())
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
                        BroadcastStatus2(pkmn, pkmn, PBEStatus2.Flinching, PBEStatusAction.Ended);
                    }
                    if (pkmn.Status2.HasFlag(PBEStatus2.HelpingHand))
                    {
                        BroadcastStatus2(pkmn, pkmn, PBEStatus2.HelpingHand, PBEStatusAction.Ended);
                    }
                    if (pkmn.Status2.HasFlag(PBEStatus2.LockOn))
                    {
                        if (--pkmn.LockOnTurns == 0)
                        {
                            pkmn.LockOnPokemon = null;
                            BroadcastStatus2(pkmn, pkmn, PBEStatus2.LockOn, PBEStatusAction.Ended);
                        }
                    }
                    if (pkmn.Protection_Used)
                    {
                        pkmn.Protection_Counter++;
                        pkmn.Protection_Used = false;
                        if (pkmn.Status2.HasFlag(PBEStatus2.Protected))
                        {
                            BroadcastStatus2(pkmn, pkmn, PBEStatus2.Protected, PBEStatusAction.Ended);
                        }
                    }
                    else
                    {
                        pkmn.Protection_Counter = 0;
                    }
                    if (pkmn.Status2.HasFlag(PBEStatus2.Roost))
                    {
                        pkmn.EndRoost();
                        BroadcastStatus2(pkmn, pkmn, PBEStatus2.Roost, PBEStatusAction.Ended);
                    }
                }
                foreach (PBETeam team in Teams)
                {
                    if (team.TeamStatus.HasFlag(PBETeamStatus.QuickGuard))
                    {
                        BroadcastTeamStatus(team, PBETeamStatus.QuickGuard, PBETeamStatusAction.Ended);
                    }
                    if (team.TeamStatus.HasFlag(PBETeamStatus.WideGuard))
                    {
                        BroadcastTeamStatus(team, PBETeamStatus.WideGuard, PBETeamStatusAction.Ended);
                    }
                }
                foreach (PBETrainer trainer in Trainers)
                {
                    trainer.ActionsRequired.Clear();
                    trainer.ActionsRequired.AddRange(trainer.ActiveBattlers);
                }

                if (BattleFormat == PBEBattleFormat.Triple && Teams.All(t => t.NumConsciousPkmn == 1))
                {
                    PBEBattlePokemon pkmn0 = ActiveBattlers[0],
                        pkmn1 = ActiveBattlers[1];
                    if ((pkmn0.FieldPosition == PBEFieldPosition.Left && pkmn1.FieldPosition == PBEFieldPosition.Left) || (pkmn0.FieldPosition == PBEFieldPosition.Right && pkmn1.FieldPosition == PBEFieldPosition.Right))
                    {
                        PBEFieldPosition pkmn0OldPos = pkmn0.FieldPosition,
                            pkmn1OldPos = pkmn1.FieldPosition;
                        pkmn0.FieldPosition = PBEFieldPosition.Center;
                        pkmn1.FieldPosition = PBEFieldPosition.Center;
                        BroadcastAutoCenter(pkmn0, pkmn0OldPos, pkmn1, pkmn1OldPos);
                    }
                }

                TurnNumber++;
                BroadcastTurnBegan();
                foreach (PBETeam team in Teams)
                {
                    bool old = team.MonFaintedThisTurn; // Fire events in a specific order
                    team.MonFaintedThisTurn = false;
                    team.MonFaintedLastTurn = old;
                }
                BattleState = PBEBattleState.WaitingForActions;
                OnStateChanged?.Invoke(this);
                foreach (PBETrainer trainer in Trainers.Where(t => t.NumConsciousPkmn > 0))
                {
                    BroadcastActionsRequest(trainer);
                }
            }
        }
        private IEnumerable<PBEBattlePokemon> GetActingOrder(IEnumerable<PBEBattlePokemon> pokemon, bool ignoreItemsThatActivate)
        {
            var evaluated = new List<(PBEBattlePokemon Pokemon, double Speed)>(); // TODO: Full Incense, Lagging Tail, Stall, Quick Claw
            foreach (PBEBattlePokemon pkmn in pokemon)
            {
                double speed = pkmn.Speed * GetStatChangeModifier(pkmn.SpeedChange, false);

                switch (pkmn.Item)
                {
                    case PBEItem.ChoiceScarf:
                    {
                        speed *= 1.5;
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
                        speed *= 0.5;
                        break;
                    }
                    case PBEItem.QuickPowder:
                    {
                        if (pkmn.OriginalSpecies == PBESpecies.Ditto && !pkmn.Status2.HasFlag(PBEStatus2.Transformed))
                        {
                            speed *= 2.0;
                        }
                        break;
                    }
                }
                if (ShouldDoWeatherEffects())
                {
                    if (Weather == PBEWeather.HarshSunlight && pkmn.Ability == PBEAbility.Chlorophyll)
                    {
                        speed *= 2.0;
                    }
                    else if (Weather == PBEWeather.Rain && pkmn.Ability == PBEAbility.SwiftSwim)
                    {
                        speed *= 2.0;
                    }
                    else if (Weather == PBEWeather.Sandstorm && pkmn.Ability == PBEAbility.SandRush)
                    {
                        speed *= 2.0;
                    }
                }
                switch (pkmn.Ability)
                {
                    case PBEAbility.QuickFeet:
                    {
                        if (pkmn.Status1 != PBEStatus1.None)
                        {
                            speed *= 1.5;
                        }
                        break;
                    }
                    case PBEAbility.SlowStart:
                    {
                        if (pkmn.SlowStart_HinderTurnsLeft > 0)
                        {
                            speed *= 0.5;
                        }
                        break;
                    }
                }
                if (pkmn.Ability != PBEAbility.QuickFeet && pkmn.Status1 == PBEStatus1.Paralyzed)
                {
                    speed *= 0.25;
                }
                if (pkmn.Team.TeamStatus.HasFlag(PBETeamStatus.Tailwind))
                {
                    speed *= 2.0;
                }

                Debug.WriteLine("Team {0}'s {1}'s evaluated speed: {2}", pkmn.Team.Id, pkmn.Nickname, speed);
                (PBEBattlePokemon Pokemon, double Speed) tup = (pkmn, speed);
                if (evaluated.Count == 0)
                {
                    evaluated.Add(tup);
                }
                else
                {
                    int pkmnTiedWith = evaluated.FindIndex(t => t.Speed == speed);
                    if (pkmnTiedWith != -1) // Speed tie - randomly go before or after the Pokémon it tied with
                    {
                        if (PBERandom.RandomBool())
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
                Debug.WriteLine(evaluated.Select(t => $"{t.Pokemon.Team.Id} {t.Pokemon.Nickname} {t.Speed}").Print());
            }
            return evaluated.Select(t => t.Pokemon);
        }
        private void DetermineTurnOrder()
        {
            _turnOrder.Clear();
            IEnumerable<PBEBattlePokemon> pkmnSwitchingOut = ActiveBattlers.Where(p => p.TurnAction.Decision == PBETurnDecision.SwitchOut);
            IEnumerable<PBEBattlePokemon> pkmnFighting = ActiveBattlers.Where(p => p.TurnAction.Decision == PBETurnDecision.Fight);
            // Switching happens first:
            _turnOrder.AddRange(GetActingOrder(pkmnSwitchingOut, true));
            // Moves:
            sbyte GetPrio(PBEBattlePokemon p)
            {
                PBEMoveData mData = PBEMoveData.Data[p.TurnAction.FightMove];
                return (sbyte)PBEUtils.Clamp(mData.Priority + (p.Ability == PBEAbility.Prankster && mData.Category == PBEMoveCategory.Status ? 1 : 0), sbyte.MinValue, sbyte.MaxValue);
            }
            foreach (sbyte priority in pkmnFighting.Select(p => GetPrio(p)).Distinct().OrderByDescending(p => p))
            {
                PBEBattlePokemon[] pkmnWithThisPriority = pkmnFighting.Where(p => GetPrio(p) == priority).ToArray();
                if (pkmnWithThisPriority.Length > 0)
                {
                    Debug.WriteLine("Priority {0} bracket...", priority);
                    _turnOrder.AddRange(GetActingOrder(pkmnWithThisPriority, false));
                }
            }
        }
        private void RunActionsInOrder()
        {
            foreach (PBEBattlePokemon pkmn in _turnOrder.ToArray()) // Copy the list so a faint or ejection does not cause a collection modified exception
            {
                if (Winner != null) // Do not broadcast winner by calling WinCheck() in here; do it in TurnEnded()
                {
                    return;
                }
                else if (ActiveBattlers.Contains(pkmn))
                {
                    switch (pkmn.TurnAction.Decision)
                    {
                        case PBETurnDecision.Fight:
                        {
                            UseMove(pkmn, pkmn.TurnAction.FightMove, pkmn.TurnAction.FightTargets);
                            break;
                        }
                        case PBETurnDecision.SwitchOut:
                        {
                            SwitchTwoPokemon(pkmn, pkmn.Trainer.TryGetPokemon(pkmn.TurnAction.SwitchPokemonId));
                            break;
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(pkmn.TurnAction.Decision));
                    }
                }
            }
        }
        private void TurnEnded()
        {
            if (WinCheck())
            {
                return;
            }

            // Verified: Effects before LightScreen/LuckyChant/Reflect/Safeguard/TrickRoom
            DoTurnEndedEffects();

            if (WinCheck())
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
                        BroadcastTeamStatus(team, PBETeamStatus.LightScreen, PBETeamStatusAction.Ended);
                    }
                }
                if (team.TeamStatus.HasFlag(PBETeamStatus.LuckyChant))
                {
                    team.LuckyChantCount--;
                    if (team.LuckyChantCount == 0)
                    {
                        BroadcastTeamStatus(team, PBETeamStatus.LuckyChant, PBETeamStatusAction.Ended);
                    }
                }
                if (team.TeamStatus.HasFlag(PBETeamStatus.Reflect))
                {
                    team.ReflectCount--;
                    if (team.ReflectCount == 0)
                    {
                        BroadcastTeamStatus(team, PBETeamStatus.Reflect, PBETeamStatusAction.Ended);
                    }
                }
                if (team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
                {
                    team.SafeguardCount--;
                    if (team.SafeguardCount == 0)
                    {
                        BroadcastTeamStatus(team, PBETeamStatus.Safeguard, PBETeamStatusAction.Ended);
                    }
                }
                if (team.TeamStatus.HasFlag(PBETeamStatus.Tailwind))
                {
                    team.TailwindCount--;
                    if (team.TailwindCount == 0)
                    {
                        BroadcastTeamStatus(team, PBETeamStatus.Tailwind, PBETeamStatusAction.Ended);
                    }
                }
            }
            // Trick Room
            if (BattleStatus.HasFlag(PBEBattleStatus.TrickRoom))
            {
                TrickRoomCount--;
                if (TrickRoomCount == 0)
                {
                    BroadcastBattleStatus(PBEBattleStatus.TrickRoom, PBEBattleStatusAction.Ended);
                }
            }

            SwitchesOrActions();
        }
    }
}
