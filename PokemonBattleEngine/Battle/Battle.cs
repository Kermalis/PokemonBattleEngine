using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Battle
{
    /// <summary>
    /// Represents a team in a specific <see cref="PBEBattle"/>.
    /// </summary>
    public sealed class PBETeam
    {
        /// <summary>
        /// The battle this team and its party belongs to.
        /// </summary>
        public PBEBattle Battle { get; }
        public byte Id { get; }
        public string TrainerName { get; set; }
        public List<PBEPokemon> Party { get; private set; }

        public IEnumerable<PBEPokemon> ActiveBattlers => Battle.ActiveBattlers.Where(p => p.Team == this).OrderBy(p => p.FieldPosition);
        public int NumPkmnAlive => Party.Count(p => p.HP > 0);
        public int NumPkmnOnField => Party.Count(p => p.FieldPosition != PBEFieldPosition.None);

        public List<PBEPokemon> ActionsRequired { get; } = new List<PBEPokemon>(3); // PBEBattleState.WaitingForActions
        public byte SwitchInsRequired { get; set; } // PBEBattleState.WaitingForSwitchIns
        public List<PBEPokemon> SwitchInQueue { get; } = new List<PBEPokemon>(3); // PBEBattleState.WaitingForSwitchIns

        public PBETeamStatus TeamStatus { get; set; }
        public byte LightScreenCount { get; set; }
        public byte LuckyChantCount { get; set; }
        public byte ReflectCount { get; set; }
        public byte SpikeCount { get; set; }
        public byte ToxicSpikeCount { get; set; }
        public bool MonFaintedLastTurn { get; set; }

        // Host constructor
        internal PBETeam(PBEBattle battle, byte id, IEnumerable<PBEPokemonShell> party, ref byte pkmnIdCounter)
        {
            Battle = battle;
            Id = id;
            CreateParty(party, ref pkmnIdCounter);
        }
        // Client constructor
        internal PBETeam(PBEBattle battle, byte id)
        {
            Battle = battle;
            Id = id;
            Party = new List<PBEPokemon>(Battle.Settings.MaxPartySize);
        }
        internal void CreateParty(IEnumerable<PBEPokemonShell> party, ref byte pkmnIdCounter)
        {
            Party = new List<PBEPokemon>(Battle.Settings.MaxPartySize);
            foreach (PBEPokemonShell pkmn in party)
            {
                new PBEPokemon(this, pkmnIdCounter++, pkmn);
            }
        }

        /// <summary>
        /// Gets a specific active Pokémon by its position.
        /// </summary>
        /// <param name="pos">The position of the Pokémon you want to get.</param>
        /// <returns>null if no Pokémon was found was found at <paramref name="pos"/>; otherwise the <see cref="PBEPokemon"/>.</returns>
        public PBEPokemon TryGetPokemonAtPosition(PBEFieldPosition pos) => ActiveBattlers.SingleOrDefault(p => p.FieldPosition == pos);

        public override bool Equals(object obj)
        {
            if (obj is PBETeam other)
            {
                return other.Id.Equals(Id) && other.Battle.Equals(Battle);
            }
            return base.Equals(obj);
        }
        public override int GetHashCode() => Id.GetHashCode() ^ Battle.GetHashCode();
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{TrainerName}'s team:");
            sb.AppendLine($"TeamStatus: {TeamStatus}");
            sb.AppendLine($"NumPkmn: {Party.Count}");
            sb.AppendLine($"NumPkmnAlive: {NumPkmnAlive}");
            sb.AppendLine($"NumPkmnOnField: {NumPkmnOnField}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents a specific Pokémon battle.
    /// </summary>
    public sealed partial class PBEBattle
    {
        public delegate void BattleStateChangedEvent(PBEBattle battle);
        public event BattleStateChangedEvent OnStateChanged;
        public PBEBattleState BattleState { get; private set; }

        public PBEBattleFormat BattleFormat { get; }
        public PBESettings Settings { get; }
        public PBETeam[] Teams { get; } = new PBETeam[2];
        public List<PBEPokemon> ActiveBattlers { get; }
        readonly List<PBEPokemon> turnOrder = new List<PBEPokemon>();

        public PBEWeather Weather { get; set; }
        public byte WeatherCounter { get; set; }
        public PBEBattleStatus BattleStatus { get; set; }
        public byte TrickRoomCount { get; set; }

        /// <summary>
        /// Gets a specific Pokémon participating in this battle by its ID.
        /// </summary>
        /// <param name="pkmnId">The ID of the Pokémon you want to get.</param>
        /// <returns>null if no Pokémon was found with <paramref name="pkmnId"/>; otherwise the <see cref="PBEPokemon"/>.</returns>
        public PBEPokemon TryGetPokemon(byte pkmnId) => Teams.SelectMany(t => t.Party).SingleOrDefault(p => p.Id == pkmnId);

        byte pkmnIdCounter;
        public PBEBattle(PBEBattleFormat battleFormat, PBESettings settings, IEnumerable<PBEPokemonShell> team0Party, IEnumerable<PBEPokemonShell> team1Party)
        {
            BattleFormat = battleFormat;
            Settings = settings;
            ActiveBattlers = new List<PBEPokemon>(Settings.MaxPartySize);
            Teams[0] = new PBETeam(this, 0, team0Party, ref pkmnIdCounter);
            Teams[1] = new PBETeam(this, 1, team1Party, ref pkmnIdCounter);
            CheckForReadiness();
        }
        public PBEBattle(PBEBattleFormat battleFormat, PBESettings settings)
        {
            BattleFormat = battleFormat;
            Settings = settings;
            ActiveBattlers = new List<PBEPokemon>(Settings.MaxPartySize);

            Teams[0] = new PBETeam(this, 0);
            Teams[1] = new PBETeam(this, 1);

            BattleState = PBEBattleState.WaitingForPlayers;
            OnStateChanged?.Invoke(this);
        }
        // Sets BattleState to PBEBattleState.ReadyToBegin
        void CheckForReadiness()
        {
            if (Teams.All(t => t.NumPkmnAlive > 0))
            {
                // Set pokemon field positions
                switch (BattleFormat)
                {
                    case PBEBattleFormat.Single:
                        Teams[0].Party[0].FieldPosition = PBEFieldPosition.Center;
                        Teams[0].SwitchInQueue.Add(Teams[0].Party[0]);
                        Teams[1].Party[0].FieldPosition = PBEFieldPosition.Center;
                        Teams[1].SwitchInQueue.Add(Teams[1].Party[0]);
                        break;
                    case PBEBattleFormat.Double:
                        Teams[0].Party[0].FieldPosition = PBEFieldPosition.Left;
                        Teams[0].SwitchInQueue.Add(Teams[0].Party[0]);
                        if (Teams[0].Party.Count > 1)
                        {
                            Teams[0].Party[1].FieldPosition = PBEFieldPosition.Right;
                            Teams[0].SwitchInQueue.Add(Teams[0].Party[1]);
                        }
                        Teams[1].Party[0].FieldPosition = PBEFieldPosition.Left;
                        Teams[1].SwitchInQueue.Add(Teams[1].Party[0]);
                        if (Teams[1].Party.Count > 1)
                        {
                            Teams[1].Party[1].FieldPosition = PBEFieldPosition.Right;
                            Teams[1].SwitchInQueue.Add(Teams[1].Party[1]);
                        }
                        break;
                    case PBEBattleFormat.Triple:
                        Teams[0].Party[0].FieldPosition = PBEFieldPosition.Left;
                        Teams[0].SwitchInQueue.Add(Teams[0].Party[0]);
                        if (Teams[0].Party.Count > 1)
                        {
                            Teams[0].Party[1].FieldPosition = PBEFieldPosition.Center;
                            Teams[0].SwitchInQueue.Add(Teams[0].Party[1]);
                        }
                        if (Teams[0].Party.Count > 2)
                        {
                            Teams[0].Party[2].FieldPosition = PBEFieldPosition.Right;
                            Teams[0].SwitchInQueue.Add(Teams[0].Party[2]);
                        }
                        Teams[1].Party[0].FieldPosition = PBEFieldPosition.Left;
                        Teams[1].SwitchInQueue.Add(Teams[1].Party[0]);
                        if (Teams[1].Party.Count > 1)
                        {
                            Teams[1].Party[1].FieldPosition = PBEFieldPosition.Center;
                            Teams[1].SwitchInQueue.Add(Teams[1].Party[1]);
                        }
                        if (Teams[1].Party.Count > 2)
                        {
                            Teams[1].Party[2].FieldPosition = PBEFieldPosition.Right;
                            Teams[1].SwitchInQueue.Add(Teams[1].Party[2]);
                        }
                        break;
                    case PBEBattleFormat.Rotation:
                        Teams[0].Party[0].FieldPosition = PBEFieldPosition.Center;
                        Teams[0].SwitchInQueue.Add(Teams[0].Party[0]);
                        if (Teams[0].Party.Count > 1)
                        {
                            Teams[0].Party[1].FieldPosition = PBEFieldPosition.Left;
                            Teams[0].SwitchInQueue.Add(Teams[0].Party[1]);
                        }
                        if (Teams[0].Party.Count > 2)
                        {
                            Teams[0].Party[2].FieldPosition = PBEFieldPosition.Right;
                            Teams[0].SwitchInQueue.Add(Teams[0].Party[2]);
                        }
                        Teams[1].Party[0].FieldPosition = PBEFieldPosition.Center;
                        Teams[1].SwitchInQueue.Add(Teams[1].Party[0]);
                        if (Teams[1].Party.Count > 1)
                        {
                            Teams[1].Party[1].FieldPosition = PBEFieldPosition.Left;
                            Teams[1].SwitchInQueue.Add(Teams[1].Party[1]);
                        }
                        if (Teams[1].Party.Count > 2)
                        {
                            Teams[1].Party[2].FieldPosition = PBEFieldPosition.Right;
                            Teams[1].SwitchInQueue.Add(Teams[1].Party[2]);
                        }
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(BattleFormat));
                }

                BattleState = PBEBattleState.ReadyToBegin;
                OnStateChanged?.Invoke(this);
            }
        }
        // Sets BattleState to PBEBattleState.ReadyToBegin
        public static void CreateTeamParty(PBETeam team, IEnumerable<PBEPokemonShell> party)
        {
            if (team.Battle.BattleState != PBEBattleState.WaitingForPlayers)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBEBattleState.WaitingForPlayers)} to set a team's party.");
            }
            team.CreateParty(party, ref team.Battle.pkmnIdCounter);
            team.Battle.CheckForReadiness();
        }
        // For clients
        // Does not update ActiveBattlers
        public void RemotePokemonSwitchedIn(PBEPkmnSwitchInPacket psip)
        {
            foreach (PBEPkmnSwitchInPacket.PBESwitchInInfo info in psip.SwitchIns)
            {
                PBEPokemon pkmn = TryGetPokemon(info.PokemonId);
                if (pkmn == null)
                {
                    pkmn = new PBEPokemon(psip.Team, info);
                }
                pkmn.HP = info.HP;
                pkmn.MaxHP = info.MaxHP;
                pkmn.FieldPosition = info.FieldPosition;
            }
        }
        // Starts the battle
        // Sets BattleState to PBEBattleState.Processing, then PBEBattleState.WaitingForActions
        public void Begin()
        {
            if (BattleState != PBEBattleState.ReadyToBegin)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBEBattleState.ReadyToBegin)} to begin the battle.");
            }
            SwitchInQueuedPokemon();
            RequestActions();
        }
        // Runs a turn
        // Sets BattleState to PBEBattleState.Processing, then PBEBattleState.WaitingForActions/PBEBattleState.WaitingForSwitches/PBEBattleState.Ended
        public void RunTurn()
        {
            if (BattleState != PBEBattleState.ReadyToRunTurn)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBEBattleState.ReadyToRunTurn)} to run a turn.");
            }
            BattleState = PBEBattleState.Processing;
            OnStateChanged?.Invoke(this);
            DetermineTurnOrder();
            RunActionsInOrder();
            TurnEnded();
        }
        // Sets BattleState to PBEBattleState.WaitingForActions
        void RequestActions()
        {
            foreach (PBETeam team in Teams)
            {
                team.ActionsRequired.Clear();
                team.ActionsRequired.AddRange(team.ActiveBattlers);
            }
            BattleState = PBEBattleState.WaitingForActions;
            OnStateChanged?.Invoke(this);
            foreach (PBETeam team in Teams)
            {
                BroadcastActionsRequest(team);
            }
        }
        void DetermineTurnOrder()
        {
            turnOrder.Clear();
            IEnumerable<PBEPokemon> pkmnSwitchingOut = ActiveBattlers.Where(p => p.SelectedAction.Decision == PBEDecision.SwitchOut);
            IEnumerable<PBEPokemon> pkmnFighting = ActiveBattlers.Where(p => p.SelectedAction.Decision == PBEDecision.Fight);
            // Switching happens first:
            turnOrder.AddRange(pkmnSwitchingOut);
            // Moves:
            IEnumerable<sbyte> uniquePriorities = pkmnFighting.Select(p => PBEMoveData.Data[p.SelectedAction.FightMove].Priority).Distinct().OrderByDescending(p => p);
            foreach (sbyte priority in uniquePriorities)
            {
                IEnumerable<PBEPokemon> pkmnWithThisPriority = pkmnFighting.Where(p => PBEMoveData.Data[p.SelectedAction.FightMove].Priority == priority);
                if (pkmnWithThisPriority.Count() == 0)
                {
                    continue;
                }
                Debug.WriteLine("Priority {0} bracket...", priority);
                var evaluated = new List<Tuple<PBEPokemon, double>>(); // TODO: two bools for wanting to go first or last
                foreach (PBEPokemon pkmn in pkmnWithThisPriority)
                {
                    double speed = pkmn.Speed * GetStatChangeModifier(pkmn.SpeedChange, false);

                    switch (pkmn.Item)
                    {
                        case PBEItem.ChoiceScarf:
                            speed *= 1.5;
                            break;
                        case PBEItem.MachoBrace:
                        case PBEItem.PowerAnklet:
                        case PBEItem.PowerBand:
                        case PBEItem.PowerBelt:
                        case PBEItem.PowerBracer:
                        case PBEItem.PowerLens:
                        case PBEItem.PowerWeight:
                            speed *= 0.5;
                            break;
                        case PBEItem.QuickPowder:
                            if (pkmn.Species == PBESpecies.Ditto)
                            {
                                speed *= 2.0;
                            }
                            break;
                    }
                    if (Weather == PBEWeather.HarshSunlight && pkmn.Ability == PBEAbility.Chlorophyll)
                    {
                        speed *= 2.0;
                    }
                    if (Weather == PBEWeather.Rain && pkmn.Ability == PBEAbility.SwiftSwim)
                    {
                        speed *= 2.0;
                    }
                    if (Weather == PBEWeather.Sandstorm && pkmn.Ability == PBEAbility.SandRush)
                    {
                        speed *= 2.0;
                    }
                    if (pkmn.Status1 == PBEStatus1.Paralyzed)
                    {
                        speed *= 0.25;
                    }

                    Debug.WriteLine("Team {0}'s {1}'s evaluated speed: {2}", pkmn.Team.Id, pkmn.Shell.Nickname, speed);
                    Tuple<PBEPokemon, double> tup = Tuple.Create(pkmn, speed);
                    if (evaluated.Count == 0)
                    {
                        evaluated.Add(tup);
                    }
                    else
                    {
                        int pkmnTiedWith = evaluated.FindIndex(t => t.Item2 == speed);
                        if (pkmnTiedWith != -1)
                        {
                            if (PBEUtils.RNG.NextBoolean()) // Randomly go before or after the Pokémon it tied with
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
                            int pkmnToGoBefore = evaluated.FindIndex(t => BattleStatus.HasFlag(PBEBattleStatus.TrickRoom) ? t.Item2 > speed : t.Item2 < speed);
                            if (pkmnToGoBefore == -1)
                            {
                                evaluated.Add(tup); // All evaluated Pokémon are faster (slower in trick room) than this one
                            }
                            else
                            {
                                evaluated.Insert(pkmnToGoBefore, tup);
                            }
                        }
                    }
                    Debug.WriteLine(evaluated.Select(t => $"{t.Item1.Team.Id} {t.Item1.Shell.Nickname} {t.Item2}").Print());
                }
                turnOrder.AddRange(evaluated.Select(t => t.Item1));
            }
        }
        void RunActionsInOrder()
        {
            foreach (PBEPokemon pkmn in turnOrder.ToArray()) // Copy the list so a faint or ejection does not cause a collection modified exception
            {
                if (!ActiveBattlers.Contains(pkmn))
                {
                    continue;
                }
                switch (pkmn.SelectedAction.Decision)
                {
                    case PBEDecision.Fight:
                        DoPreMoveEffects(pkmn);
                        UseMove(pkmn);
                        break;
                    case PBEDecision.SwitchOut:
                        SwitchTwoPokemon(pkmn, TryGetPokemon(pkmn.SelectedAction.SwitchPokemonId), false);
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(pkmn.SelectedAction.Decision), $"Invalid decision: {pkmn.SelectedAction.Decision}");
                }
                pkmn.PreviousAction = pkmn.SelectedAction;
            }
        }
        // Sets BattleState to PBEBattleState.WaitingForActions/PBEBattleState.WaitingForSwitches/PBEBattleState.Ended
        void TurnEnded()
        {
            // Weather stops before doing damage
            if (WeatherCounter > 0)
            {
                WeatherCounter--;
                if (WeatherCounter == 0)
                {
                    PBEWeather w = Weather;
                    Weather = PBEWeather.None;
                    BroadcastWeather(w, PBEWeatherAction.Ended);
                }
            }

            // Pokémon
            foreach (PBEPokemon pkmn in ActiveBattlers.ToArray()) // Copy the list so a faint or ejection does not cause a collection modified exception
            {
                if (!ActiveBattlers.Contains(pkmn))
                {
                    continue;
                }
                pkmn.SelectedAction.Decision = PBEDecision.None; // No longer necessary
                pkmn.Status2 &= ~PBEStatus2.Flinching;
                pkmn.Status2 &= ~PBEStatus2.Protected;
                if (pkmn.PreviousAction.Decision == PBEDecision.Fight && pkmn.PreviousAction.FightMove != PBEMove.Protect && pkmn.PreviousAction.FightMove != PBEMove.Detect)
                {
                    pkmn.ProtectCounter = 0;
                }
                DoTurnEndedEffects(pkmn);
            }

            // Teams
            foreach (PBETeam team in Teams)
            {
                if (team.NumPkmnAlive == 0) // TODO: Figure out how wins are determined (tie exists?)
                {
                    BattleState = PBEBattleState.Ended;
                    OnStateChanged?.Invoke(this);
                    return;
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
            }

            // Battle Statuses
            if (BattleStatus.HasFlag(PBEBattleStatus.TrickRoom))
            {
                TrickRoomCount--;
                if (TrickRoomCount == 0)
                {
                    BattleStatus &= ~PBEBattleStatus.TrickRoom;
                    BroadcastBattleStatus(PBEBattleStatus.TrickRoom, PBEBattleStatusAction.Ended);
                }
            }

            PBEBattleState nextState = PBEBattleState.WaitingForActions;
            // Requesting a replacement
            foreach (PBETeam team in Teams)
            {
                int available = team.NumPkmnAlive - team.NumPkmnOnField;
                team.SwitchInsRequired = 0;
                team.SwitchInQueue.Clear();
                switch (BattleFormat)
                {
                    case PBEBattleFormat.Single:
                        if (available > 0 && team.TryGetPokemonAtPosition(PBEFieldPosition.Center) == null)
                        {
                            team.SwitchInsRequired = 1;
                            nextState = PBEBattleState.WaitingForSwitchIns;
                        }
                        break;
                    case PBEBattleFormat.Double:
                        if (available > 0 && team.TryGetPokemonAtPosition(PBEFieldPosition.Left) == null)
                        {
                            available--;
                            team.SwitchInsRequired++;
                        }
                        if (available > 0 && team.TryGetPokemonAtPosition(PBEFieldPosition.Right) == null)
                        {
                            team.SwitchInsRequired++;
                        }
                        if (team.SwitchInsRequired > 0)
                        {
                            nextState = PBEBattleState.WaitingForSwitchIns;
                        }
                        break;
                    case PBEBattleFormat.Rotation:
                    case PBEBattleFormat.Triple:
                        if (available > 0 && team.TryGetPokemonAtPosition(PBEFieldPosition.Left) == null)
                        {
                            available--;
                            team.SwitchInsRequired++;
                        }
                        if (available > 0 && team.TryGetPokemonAtPosition(PBEFieldPosition.Center) == null)
                        {
                            available--;
                            team.SwitchInsRequired++;
                        }
                        if (available > 0 && team.TryGetPokemonAtPosition(PBEFieldPosition.Right) == null)
                        {
                            team.SwitchInsRequired++;
                        }
                        if (team.SwitchInsRequired > 0)
                        {
                            nextState = PBEBattleState.WaitingForSwitchIns;
                        }
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(BattleFormat));
                }
            }

            if (nextState == PBEBattleState.WaitingForSwitchIns)
            {
                BattleState = PBEBattleState.WaitingForSwitchIns;
                OnStateChanged?.Invoke(this);
                foreach (PBETeam team in Teams)
                {
                    if (team.SwitchInsRequired > 0)
                    {
                        BroadcastSwitchInRequest(team);
                    }
                }
            }
            else // PBEBattleState.WaitingForActions
            {
                RequestActions();
            }
        }
    }
}
