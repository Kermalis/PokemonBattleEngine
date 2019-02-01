using Kermalis.PokemonBattleEngine.Data;
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
        public PBEPokemon TryGetPokemon(PBEFieldPosition pos) => ActiveBattlers.SingleOrDefault(p => p.FieldPosition == pos);

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
        /// <summary>
        /// The winner of the battle. Null if the battle is ongoing or the battle resulted in a draw.
        /// </summary>
        public PBETeam Winner { get; set; }

        public PBEBattleFormat BattleFormat { get; }
        public PBESettings Settings { get; }
        public PBETeam[] Teams { get; } = new PBETeam[2];
        public List<PBEPokemon> ActiveBattlers { get; } = new List<PBEPokemon>(6);
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
            if (battleFormat >= PBEBattleFormat.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(battleFormat));
            }
            if (team0Party == null)
            {
                throw new ArgumentNullException(nameof(team0Party));
            }
            if (team1Party == null)
            {
                throw new ArgumentNullException(nameof(team1Party));
            }
            if (team0Party.Count() == 0 || team0Party.Count() > settings.MaxPartySize)
            {
                throw new ArgumentOutOfRangeException(nameof(team0Party));
            }
            if (team1Party.Count() == 0 || team1Party.Count() > settings.MaxPartySize)
            {
                throw new ArgumentOutOfRangeException(nameof(team1Party));
            }
            BattleFormat = battleFormat;
            Settings = settings;
            Teams[0] = new PBETeam(this, 0, team0Party, ref pkmnIdCounter);
            Teams[1] = new PBETeam(this, 1, team1Party, ref pkmnIdCounter);
            CheckForReadiness();
        }
        public PBEBattle(PBEBattleFormat battleFormat, PBESettings settings)
        {
            if (battleFormat >= PBEBattleFormat.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(battleFormat));
            }
            BattleFormat = battleFormat;
            Settings = settings;

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
                switch (BattleFormat)
                {
                    case PBEBattleFormat.Single:
                        {
                            foreach (PBETeam team in Teams)
                            {
                                team.Party[0].FieldPosition = PBEFieldPosition.Center;
                                team.SwitchInQueue.Add(team.Party[0]);
                            }
                            break;
                        }
                    case PBEBattleFormat.Double:
                        {
                            foreach (PBETeam team in Teams)
                            {
                                team.Party[0].FieldPosition = PBEFieldPosition.Left;
                                team.SwitchInQueue.Add(team.Party[0]);
                                if (team.Party.Count > 1)
                                {
                                    team.Party[1].FieldPosition = PBEFieldPosition.Right;
                                    team.SwitchInQueue.Add(team.Party[1]);
                                }
                            }
                            break;
                        }
                    case PBEBattleFormat.Triple:
                        {
                            foreach (PBETeam team in Teams)
                            {
                                team.Party[0].FieldPosition = PBEFieldPosition.Left;
                                team.SwitchInQueue.Add(team.Party[0]);
                                if (team.Party.Count > 1)
                                {
                                    team.Party[1].FieldPosition = PBEFieldPosition.Center;
                                    team.SwitchInQueue.Add(team.Party[1]);
                                }
                                if (team.Party.Count > 2)
                                {
                                    team.Party[2].FieldPosition = PBEFieldPosition.Right;
                                    team.SwitchInQueue.Add(team.Party[2]);
                                }
                            }
                            break;
                        }
                    case PBEBattleFormat.Rotation:
                        {
                            foreach (PBETeam team in Teams)
                            {
                                team.Party[0].FieldPosition = PBEFieldPosition.Center;
                                team.SwitchInQueue.Add(team.Party[0]);
                                if (team.Party.Count > 1)
                                {
                                    team.Party[1].FieldPosition = PBEFieldPosition.Left;
                                    team.SwitchInQueue.Add(team.Party[1]);
                                }
                                if (team.Party.Count > 2)
                                {
                                    team.Party[2].FieldPosition = PBEFieldPosition.Right;
                                    team.SwitchInQueue.Add(team.Party[2]);
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
        /// <summary>
        /// Sets a specific team's party. <see cref="BattleState"/> will change to <see cref="PBEBattleState.ReadyToBegin"/> if all teams have parties.
        /// </summary>
        /// <param name="team">The team which will have its party set.</param>
        /// <param name="party">The Pokémon party <paramref name="team"/> will use.</param>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForPlayers"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="party"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="party"/>'s size is invalid.</exception>
        public static void CreateTeamParty(PBETeam team, IEnumerable<PBEPokemonShell> party)
        {
            if (team.Battle.BattleState != PBEBattleState.WaitingForPlayers)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForPlayers} to set a team's party.");
            }
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }
            if (party.Count() == 0 || party.Count() > team.Battle.Settings.MaxPartySize)
            {
                throw new ArgumentOutOfRangeException(nameof(party));
            }
            team.CreateParty(party, ref team.Battle.pkmnIdCounter);
            team.Battle.CheckForReadiness();
        }
        // Starts the battle
        // Sets BattleState to PBEBattleState.Processing, then PBEBattleState.WaitingForActions
        public void Begin()
        {
            if (BattleState != PBEBattleState.ReadyToBegin)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.ReadyToBegin} to begin the battle.");
            }
            SwitchesOrActions();
        }
        // Runs a turn
        // Sets BattleState to PBEBattleState.Processing, then PBEBattleState.WaitingForActions/PBEBattleState.WaitingForSwitches/PBEBattleState.Ended
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
        // Sets BattleState to PBEBattleState.Processing/PBEBattleState.WaitingForActions/PBEBattleState.WaitingForSwitches
        void SwitchesOrActions()
        {
            BattleState = PBEBattleState.Processing;
            OnStateChanged?.Invoke(this);

            IEnumerable<PBETeam> teamsWithSwitchIns = Teams.Where(t => t.SwitchInQueue.Count > 0);
            if (teamsWithSwitchIns.Count() > 0)
            {
                foreach (PBETeam team in teamsWithSwitchIns)
                {
                    ActiveBattlers.AddRange(team.SwitchInQueue);
                    BroadcastPkmnSwitchIn(team, team.SwitchInQueue.Select(p => CreateSwitchInInfo(p)), false);
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
            teamsWithSwitchIns = Teams.Where(t => t.SwitchInsRequired > 0);
            if (teamsWithSwitchIns.Count() > 0)
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
                // TODO: Turn began packet
                // TODO: This should go for all Pokémon, and flinching and protected should be cleared on switch just in case (Set Status2 to None if possible)
                foreach (PBEPokemon pkmn in ActiveBattlers)
                {
                    pkmn.SelectedAction.Decision = PBEDecision.None; // No longer necessary
                    pkmn.Status2 &= ~PBEStatus2.Flinching;
                    pkmn.Status2 &= ~PBEStatus2.Protected;

                    // TODO: https://github.com/Kermalis/PokemonBattleEngine/issues/79
                    if (pkmn.PreviousAction.Decision == PBEDecision.Fight && pkmn.PreviousAction.FightMove != PBEMove.Protect && pkmn.PreviousAction.FightMove != PBEMove.Detect)
                    {
                        pkmn.ProtectCounter = 0;
                    }
                }

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
        }
        IEnumerable<PBEPokemon> GetActingOrder(IEnumerable<PBEPokemon> pokemon, bool ignoreItemsThatActivate)
        {
            var evaluated = new List<Tuple<PBEPokemon, double>>(); // TODO: Full Incense, Lagging Tail, Stall, Quick Claw
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
                            if (pkmn.Shell.Species == PBESpecies.Ditto && !pkmn.Status2.HasFlag(PBEStatus2.Transformed))
                            {
                                speed *= 2.0;
                            }
                            break;
                        }
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
                    if (pkmnTiedWith != -1) // Speed tie - randomly go before or after the Pokémon it tied with
                    {
                        if (PBEUtils.RNG.NextBoolean())
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
                            evaluated.Add(tup);
                        }
                        else
                        {
                            evaluated.Insert(pkmnToGoBefore, tup);
                        }
                    }
                }
                Debug.WriteLine(evaluated.Select(t => $"{t.Item1.Team.Id} {t.Item1.Shell.Nickname} {t.Item2}").Print());
            }
            return evaluated.Select(t => t.Item1);
        }
        void DetermineTurnOrder()
        {
            turnOrder.Clear();
            IEnumerable<PBEPokemon> pkmnSwitchingOut = ActiveBattlers.Where(p => p.SelectedAction.Decision == PBEDecision.SwitchOut);
            IEnumerable<PBEPokemon> pkmnFighting = ActiveBattlers.Where(p => p.SelectedAction.Decision == PBEDecision.Fight);
            // Switching happens first:
            turnOrder.AddRange(GetActingOrder(pkmnSwitchingOut, true));
            // Moves:
            foreach (sbyte priority in pkmnFighting.Select(p => PBEMoveData.Data[p.SelectedAction.FightMove].Priority).Distinct().OrderByDescending(p => p))
            {
                IEnumerable<PBEPokemon> pkmnWithThisPriority = pkmnFighting.Where(p => PBEMoveData.Data[p.SelectedAction.FightMove].Priority == priority);
                if (pkmnWithThisPriority.Count() > 0)
                {
                    Debug.WriteLine("Priority {0} bracket...", priority);
                    turnOrder.AddRange(GetActingOrder(pkmnWithThisPriority, false));
                }
            }
        }
        void RunActionsInOrder()
        {
            foreach (PBEPokemon pkmn in turnOrder.ToArray()) // Copy the list so a faint or ejection does not cause a collection modified exception
            {
                if (ActiveBattlers.Contains(pkmn))
                {
                    switch (pkmn.SelectedAction.Decision)
                    {
                        case PBEDecision.Fight:
                            {
                                UseMove(pkmn);
                                break;
                            }
                        case PBEDecision.SwitchOut:
                            {
                                SwitchTwoPokemon(pkmn, TryGetPokemon(pkmn.SelectedAction.SwitchPokemonId), false);
                                break;
                            }
                        default: throw new ArgumentOutOfRangeException(nameof(pkmn.SelectedAction.Decision));
                    }
                    pkmn.PreviousAction = pkmn.SelectedAction;
                }
            }
        }
        // Sets BattleState to PBEBattleState.WaitingForActions/PBEBattleState.WaitingForSwitches/PBEBattleState.Ended
        void TurnEnded()
        {
            if (Winner != null)
            {
                BroadcastWinner(Winner);
                BattleState = PBEBattleState.Ended;
                OnStateChanged?.Invoke(this);
                return;
            }

            // Verified: Weather stops before doing damage
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
            // Verified: Effects before Reflect/LightScreen/LuckyChant
            DoTurnEndedEffects();

            // Verified: Reflect then Light Screen then Lucky Chant then Trick Room
            foreach (PBETeam team in Teams)
            {
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
    }
}
