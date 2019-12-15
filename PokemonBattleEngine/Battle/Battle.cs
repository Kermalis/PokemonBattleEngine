using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    // TODO: Fully implement INPC
    /// <summary>Represents a specific Pokémon battle.</summary>
    public sealed partial class PBEBattle : IDisposable, INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void BattleStateChangedEvent(PBEBattle battle);
        public event BattleStateChangedEvent OnStateChanged;
        public PBEBattleState BattleState { get; private set; }
        public ushort TurnNumber { get; set; }
        /// <summary>The winner of the battle. null if <see cref="BattleState"/> is not <see cref="PBEBattleState.Ended"/> or the battle resulted in a draw.</summary>
        public PBETeam Winner { get; set; }

        public PBEBattleFormat BattleFormat { get; }
        public PBESettings Settings { get; }
        public PBETeams Teams { get; }
        public List<PBEPokemon> ActiveBattlers { get; } = new List<PBEPokemon>(6);
        private readonly List<PBEPokemon> _turnOrder = new List<PBEPokemon>();

        public PBEWeather Weather { get; set; }
        public byte WeatherCounter { get; set; }
        public PBEBattleStatus BattleStatus { get; set; }
        public byte TrickRoomCount { get; set; }

        public List<IPBEPacket> Events { get; } = new List<IPBEPacket>();

        /// <summary>Gets a specific <see cref="PBEPokemon"/> participating in this battle by its ID.</summary>
        /// <param name="pkmnId">The ID of the <see cref="PBEPokemon"/>.</param>
        public PBEPokemon TryGetPokemon(byte pkmnId)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            return Teams.SelectMany(t => t.Party).SingleOrDefault(p => p.Id == pkmnId);
        }

        /// <summary>Creates a new <see cref="PBEBattle"/> object with the specified <see cref="PBEBattleFormat"/> and teams. Each team must have equal settings. The battle's settings are set to a copy of the teams' settings. <see cref="BattleState"/> will be <see cref="PBEBattleState.ReadyToBegin"/>.</summary>
        /// <param name="battleFormat">The <see cref="PBEBattleFormat"/> of the battle.</param>
        /// <param name="team1Shell">The <see cref="PBETeamShell"/> object to use to create <see cref="Teams"/>[0].</param>
        /// <param name="team1TrainerName">The name of the trainer(s) on <see cref="Teams"/>[0].</param>
        /// <param name="team2Shell">The <see cref="PBETeamShell"/> object to use to create <see cref="Teams"/>[1].</param>
        /// <param name="team2TrainerName">The name of the trainer(s) on <see cref="Teams"/>[1].</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="team1Shell"/> or <paramref name="team2Shell"/> are null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="team1Shell"/> and <paramref name="team2Shell"/> have unequal <see cref="PBETeamShell.Settings"/> or when <paramref name="team1TrainerName"/> or <paramref name="team2TrainerName"/> are invalid.</exception>
        public PBEBattle(PBEBattleFormat battleFormat, PBETeamShell team1Shell, string team1TrainerName, PBETeamShell team2Shell, string team2TrainerName)
        {
            if (battleFormat >= PBEBattleFormat.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(battleFormat));
            }
            if (team1Shell == null)
            {
                throw new ArgumentNullException(nameof(team1Shell));
            }
            if (team2Shell == null)
            {
                throw new ArgumentNullException(nameof(team2Shell));
            }
            if (string.IsNullOrWhiteSpace(team1TrainerName))
            {
                throw new ArgumentOutOfRangeException(nameof(team1TrainerName));
            }
            if (string.IsNullOrWhiteSpace(team2TrainerName))
            {
                throw new ArgumentOutOfRangeException(nameof(team2TrainerName));
            }
            if (team1Shell.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(team1Shell));
            }
            if (team2Shell.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(team2Shell));
            }
            if (!team1Shell.Settings.Equals(team2Shell.Settings))
            {
                throw new ArgumentOutOfRangeException(nameof(team1Shell.Settings), "Team settings must be equal to each other.");
            }
            BattleFormat = battleFormat;
            Settings = new PBESettings(team1Shell.Settings);
            Settings.MakeReadOnly();
            Teams = new PBETeams(this, team1Shell, team1TrainerName, team2Shell, team2TrainerName);
            CheckForReadiness();
        }
        /// <summary>Creates a new <see cref="PBEBattle"/> object with the specified <see cref="PBEBattleFormat"/> and a copy of the specified <see cref="PBESettings"/>. <see cref="BattleState"/> will be <see cref="PBEBattleState.WaitingForPlayers"/>.</summary>
        /// <param name="battleFormat">The <see cref="PBEBattleFormat"/> of the battle.</param>
        /// <param name="settings">The <see cref="PBESettings"/> to copy for the battle to use.</param>
        public PBEBattle(PBEBattleFormat battleFormat, PBESettings settings)
        {
            if (battleFormat >= PBEBattleFormat.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(battleFormat));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            BattleFormat = battleFormat;
            Settings = new PBESettings(settings);
            Settings.MakeReadOnly();
            Teams = new PBETeams(this);
            BattleState = PBEBattleState.WaitingForPlayers;
            OnStateChanged?.Invoke(this);
        }
        private void CheckForReadiness()
        {
            if (Teams.All(t => t.NumPkmnAlive > 0))
            {
                switch (BattleFormat)
                {
                    case PBEBattleFormat.Single:
                    {
                        foreach (PBETeam team in Teams)
                        {
                            PBEPokemon pkmn = team.Party[0];
                            pkmn.FieldPosition = PBEFieldPosition.Center;
                            team.SwitchInQueue.Add(pkmn);
                        }
                        break;
                    }
                    case PBEBattleFormat.Double:
                    {
                        foreach (PBETeam team in Teams)
                        {
                            PBEPokemon pkmn = team.Party[0];
                            pkmn.FieldPosition = PBEFieldPosition.Left;
                            team.SwitchInQueue.Add(pkmn);
                            if (team.Party.Count > 1)
                            {
                                pkmn = team.Party[1];
                                pkmn.FieldPosition = PBEFieldPosition.Right;
                                team.SwitchInQueue.Add(pkmn);
                            }
                        }
                        break;
                    }
                    case PBEBattleFormat.Triple:
                    {
                        foreach (PBETeam team in Teams)
                        {
                            PBEPokemon pkmn = team.Party[0];
                            pkmn.FieldPosition = PBEFieldPosition.Left;
                            team.SwitchInQueue.Add(pkmn);
                            if (team.Party.Count > 1)
                            {
                                pkmn = team.Party[1];
                                pkmn.FieldPosition = PBEFieldPosition.Center;
                                team.SwitchInQueue.Add(pkmn);
                            }
                            if (team.Party.Count > 2)
                            {
                                pkmn = team.Party[2];
                                pkmn.FieldPosition = PBEFieldPosition.Right;
                                team.SwitchInQueue.Add(pkmn);
                            }
                        }
                        break;
                    }
                    case PBEBattleFormat.Rotation:
                    {
                        foreach (PBETeam team in Teams)
                        {
                            PBEPokemon pkmn = team.Party[0];
                            pkmn.FieldPosition = PBEFieldPosition.Center;
                            team.SwitchInQueue.Add(pkmn);
                            if (team.Party.Count > 1)
                            {
                                pkmn = team.Party[1];
                                pkmn.FieldPosition = PBEFieldPosition.Left;
                                team.SwitchInQueue.Add(pkmn);
                            }
                            if (team.Party.Count > 2)
                            {
                                pkmn = team.Party[2];
                                pkmn.FieldPosition = PBEFieldPosition.Right;
                                team.SwitchInQueue.Add(pkmn);
                            }
                        }
                        break;
                    }
                    default: throw new ArgumentOutOfRangeException(nameof(BattleFormat));
                }

                BattleState = PBEBattleState.ReadyToBegin;
                OnStateChanged?.Invoke(this);
            }
        }
        /// <summary>Sets a specific team's party. <see cref="BattleState"/> will change to <see cref="PBEBattleState.ReadyToBegin"/> if all teams have parties.</summary>
        /// <param name="team">The team which will have its party set.</param>
        /// <param name="teamShell">The information <paramref name="team"/> will use to create its party.</param>
        /// <param name="teamTrainerName">The name of the trainer(s) on <paramref name="team"/>.</param>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForPlayers"/> or <paramref name="team"/> already has its party set.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="team"/> or <paramref name="teamShell"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="teamShell"/>'s settings are unequal to <paramref name="team"/>'s battle's settings or when <paramref name="teamTrainerName"/> is invalid.</exception>
        public static void CreateTeamParty(PBETeam team, PBETeamShell teamShell, string teamTrainerName)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }
            if (teamShell == null)
            {
                throw new ArgumentNullException(nameof(teamShell));
            }
            if (string.IsNullOrEmpty(teamTrainerName))
            {
                throw new ArgumentOutOfRangeException(nameof(teamTrainerName));
            }
            if (team.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(team));
            }
            if (teamShell.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(teamShell));
            }
            if (!teamShell.Settings.Equals(team.Battle.Settings))
            {
                throw new ArgumentOutOfRangeException(nameof(teamShell), $"\"{nameof(teamShell)}\"'s settings must be equal to the battle's settings.");
            }
            if (team.Battle.BattleState != PBEBattleState.WaitingForPlayers)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForPlayers} to set a team's party.");
            }
            if (team.Party.Count > 0)
            {
                throw new InvalidOperationException("This team already has its party set.");
            }
            team.CreateParty(teamShell, teamTrainerName);
            team.Battle.CheckForReadiness();
        }
        /// <summary>Begins the battle.</summary>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.ReadyToBegin"/>.</exception>
        public void Begin()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            if (BattleState != PBEBattleState.ReadyToBegin)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.ReadyToBegin} to begin the battle.");
            }
            lock (_disposeLockObj)
            {
                if (!IsDisposed)
                {
                    foreach (PBETeam team in Teams)
                    {
                        BroadcastTeam(team);
                    }
                    SwitchesOrActions();
                }
            }
        }
        /// <summary>Runs a turn.</summary>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.ReadyToRunTurn"/>.</exception>
        public void RunTurn()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }
            if (BattleState != PBEBattleState.ReadyToRunTurn)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.ReadyToRunTurn} to run a turn.");
            }
            lock (_disposeLockObj)
            {
                if (!IsDisposed)
                {
                    BattleState = PBEBattleState.Processing;
                    OnStateChanged?.Invoke(this);
                    DetermineTurnOrder();
                    RunActionsInOrder();
                    TurnEnded();
                }
            }
        }

        private bool WinCheck()
        {
            if (Winner != null)
            {
                BroadcastWinner(Winner);
                BattleState = PBEBattleState.Ended;
                OnStateChanged?.Invoke(this);
                return true;
            }
            return false;
        }
        private void SwitchesOrActions()
        {
            BattleState = PBEBattleState.Processing;
            OnStateChanged?.Invoke(this);

            PBETeam[] teamsWithSwitchIns = Teams.Where(t => t.SwitchInQueue.Count > 0).ToArray();
            if (teamsWithSwitchIns.Length > 0)
            {
                foreach (PBETeam team in teamsWithSwitchIns)
                {
                    ActiveBattlers.AddRange(team.SwitchInQueue);
                    BroadcastPkmnSwitchIn(team, team.SwitchInQueue.Select(p => CreateSwitchInInfo(p)).ToArray());
                }
                DoSwitchInEffects(teamsWithSwitchIns.SelectMany(t => t.SwitchInQueue));
            }

            foreach (PBETeam team in Teams)
            {
                int available = team.NumPkmnAlive - team.NumPkmnOnField;
                team.SwitchInsRequired = 0;
                team.SwitchInQueue.Clear();
                switch (BattleFormat)
                {
                    case PBEBattleFormat.Single:
                    {
                        if (available > 0 && team.TryGetPokemon(PBEFieldPosition.Center) == null)
                        {
                            team.SwitchInsRequired = 1;
                        }
                        break;
                    }
                    case PBEBattleFormat.Double:
                    {
                        if (available > 0 && team.TryGetPokemon(PBEFieldPosition.Left) == null)
                        {
                            available--;
                            team.SwitchInsRequired++;
                        }
                        if (available > 0 && team.TryGetPokemon(PBEFieldPosition.Right) == null)
                        {
                            team.SwitchInsRequired++;
                        }
                        break;
                    }
                    case PBEBattleFormat.Rotation:
                    case PBEBattleFormat.Triple:
                    {
                        if (available > 0 && team.TryGetPokemon(PBEFieldPosition.Left) == null)
                        {
                            available--;
                            team.SwitchInsRequired++;
                        }
                        if (available > 0 && team.TryGetPokemon(PBEFieldPosition.Center) == null)
                        {
                            available--;
                            team.SwitchInsRequired++;
                        }
                        if (available > 0 && team.TryGetPokemon(PBEFieldPosition.Right) == null)
                        {
                            team.SwitchInsRequired++;
                        }
                        break;
                    }
                    default: throw new ArgumentOutOfRangeException(nameof(BattleFormat));
                }
            }
            teamsWithSwitchIns = Teams.Where(t => t.SwitchInsRequired > 0).ToArray();
            if (teamsWithSwitchIns.Length > 0)
            {
                BattleState = PBEBattleState.WaitingForSwitchIns;
                OnStateChanged?.Invoke(this);
                foreach (PBETeam team in teamsWithSwitchIns)
                {
                    BroadcastSwitchInRequest(team);
                }
            }
            else
            {
                if (WinCheck())
                {
                    return;
                }

                foreach (PBEPokemon pkmn in ActiveBattlers)
                {
                    pkmn.HasUsedMoveThisTurn = false;
                    pkmn.TurnAction = null;
                    pkmn.SpeedBoost_AbleToSpeedBoostThisTurn = pkmn.Ability == PBEAbility.SpeedBoost;
                    if (pkmn.Status2.HasFlag(PBEStatus2.Flinching))
                    {
                        pkmn.Status2 &= ~PBEStatus2.Flinching;
                        BroadcastStatus2(pkmn, pkmn, PBEStatus2.Flinching, PBEStatusAction.Ended);
                    }
                    if (pkmn.Status2.HasFlag(PBEStatus2.HelpingHand))
                    {
                        pkmn.Status2 &= ~PBEStatus2.HelpingHand;
                        BroadcastStatus2(pkmn, pkmn, PBEStatus2.HelpingHand, PBEStatusAction.Ended);
                    }
                    if (pkmn.Status2.HasFlag(PBEStatus2.Protected))
                    {
                        pkmn.Status2 &= ~PBEStatus2.Protected;
                        BroadcastStatus2(pkmn, pkmn, PBEStatus2.Protected, PBEStatusAction.Ended);
                    }
                }
                foreach (PBETeam team in Teams)
                {
                    if (team.TeamStatus.HasFlag(PBETeamStatus.WideGuard))
                    {
                        team.TeamStatus &= ~PBETeamStatus.WideGuard;
                        BroadcastTeamStatus(team, PBETeamStatus.WideGuard, PBETeamStatusAction.Ended);
                    }
                }
                foreach (PBETeam team in Teams)
                {
                    team.ActionsRequired.Clear();
                    team.ActionsRequired.AddRange(team.ActiveBattlers);
                }

                if (BattleFormat == PBEBattleFormat.Triple && Teams.All(t => t.NumPkmnAlive == 1))
                {
                    PBEPokemon pkmn1 = ActiveBattlers[0],
                        pkmn2 = ActiveBattlers[1];
                    if ((pkmn1.FieldPosition == PBEFieldPosition.Left && pkmn2.FieldPosition == PBEFieldPosition.Right) || (pkmn1.FieldPosition == PBEFieldPosition.Right && pkmn2.FieldPosition == PBEFieldPosition.Left))
                    {
                        PBEFieldPosition pkmn1OldPos = pkmn1.FieldPosition,
                            pkmn2OldPos = pkmn2.FieldPosition;
                        pkmn2.FieldPosition = pkmn1.FieldPosition = PBEFieldPosition.Center;
                        BroadcastAutoCenter(pkmn1, pkmn1OldPos, pkmn2, pkmn2OldPos);
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
                foreach (PBETeam team in Teams)
                {
                    BroadcastActionsRequest(team);
                }
            }
        }
        private PBEPokemon[] GetActingOrder(IEnumerable<PBEPokemon> pokemon, bool ignoreItemsThatActivate)
        {
            var evaluated = new List<(PBEPokemon Pokemon, double Speed)>(); // TODO: Full Incense, Lagging Tail, Stall, Quick Claw
            foreach (PBEPokemon pkmn in pokemon)
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
                (PBEPokemon Pokemon, double Speed) tup = (pkmn, speed);
                if (evaluated.Count == 0)
                {
                    evaluated.Add(tup);
                }
                else
                {
                    int pkmnTiedWith = evaluated.FindIndex(t => t.Speed == speed);
                    if (pkmnTiedWith != -1) // Speed tie - randomly go before or after the Pokémon it tied with
                    {
                        if (PBEUtils.RandomBool())
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
            return evaluated.Select(t => t.Pokemon).ToArray();
        }
        private void DetermineTurnOrder()
        {
            _turnOrder.Clear();
            IEnumerable<PBEPokemon> pkmnSwitchingOut = ActiveBattlers.Where(p => p.TurnAction.Decision == PBETurnDecision.SwitchOut);
            IEnumerable<PBEPokemon> pkmnFighting = ActiveBattlers.Where(p => p.TurnAction.Decision == PBETurnDecision.Fight);
            // Switching happens first:
            _turnOrder.AddRange(GetActingOrder(pkmnSwitchingOut, true));
            // Moves:
            sbyte GetPrio(PBEPokemon p)
            {
                PBEMoveData mData = PBEMoveData.Data[p.TurnAction.FightMove];
                return (sbyte)PBEUtils.Clamp(mData.Priority + (p.Ability == PBEAbility.Prankster && mData.Category == PBEMoveCategory.Status ? 1 : 0), sbyte.MinValue, sbyte.MaxValue);
            }
            foreach (sbyte priority in pkmnFighting.Select(p => GetPrio(p)).Distinct().OrderByDescending(p => p))
            {
                PBEPokemon[] pkmnWithThisPriority = pkmnFighting.Where(p => GetPrio(p) == priority).ToArray();
                if (pkmnWithThisPriority.Length > 0)
                {
                    Debug.WriteLine("Priority {0} bracket...", priority);
                    _turnOrder.AddRange(GetActingOrder(pkmnWithThisPriority, false));
                }
            }
        }
        private void RunActionsInOrder()
        {
            foreach (PBEPokemon pkmn in _turnOrder.ToArray()) // Copy the list so a faint or ejection does not cause a collection modified exception
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
                            SwitchTwoPokemon(pkmn, TryGetPokemon(pkmn.TurnAction.SwitchPokemonId));
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
                        team.TeamStatus &= ~PBETeamStatus.LightScreen;
                        BroadcastTeamStatus(team, PBETeamStatus.LightScreen, PBETeamStatusAction.Ended);
                    }
                }
                if (team.TeamStatus.HasFlag(PBETeamStatus.LuckyChant))
                {
                    team.LuckyChantCount--;
                    if (team.LuckyChantCount == 0)
                    {
                        team.TeamStatus &= ~PBETeamStatus.LuckyChant;
                        BroadcastTeamStatus(team, PBETeamStatus.LuckyChant, PBETeamStatusAction.Ended);
                    }
                }
                if (team.TeamStatus.HasFlag(PBETeamStatus.Reflect))
                {
                    team.ReflectCount--;
                    if (team.ReflectCount == 0)
                    {
                        team.TeamStatus &= ~PBETeamStatus.Reflect;
                        BroadcastTeamStatus(team, PBETeamStatus.Reflect, PBETeamStatusAction.Ended);
                    }
                }
                if (team.TeamStatus.HasFlag(PBETeamStatus.Safeguard))
                {
                    team.SafeguardCount--;
                    if (team.SafeguardCount == 0)
                    {
                        team.TeamStatus &= ~PBETeamStatus.Safeguard;
                        BroadcastTeamStatus(team, PBETeamStatus.Safeguard, PBETeamStatusAction.Ended);
                    }
                }
                if (team.TeamStatus.HasFlag(PBETeamStatus.Tailwind))
                {
                    team.TailwindCount--;
                    if (team.TailwindCount == 0)
                    {
                        team.TeamStatus &= ~PBETeamStatus.Tailwind;
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
                    BattleStatus &= ~PBEBattleStatus.TrickRoom;
                    BroadcastBattleStatus(PBEBattleStatus.TrickRoom, PBEBattleStatusAction.Ended);
                }
            }

            SwitchesOrActions();
        }

        private readonly object _disposeLockObj = new object();
        public bool IsDisposed { get; private set; }
        public void Dispose()
        {
            lock (_disposeLockObj)
            {
                if (!IsDisposed)
                {
                    IsDisposed = true;
                    OnPropertyChanged(nameof(IsDisposed));
                    foreach (PBETeam team in Teams)
                    {
                        team.Dispose();
                    }
                }
            }
        }
    }
}
