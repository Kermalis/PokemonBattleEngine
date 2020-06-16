using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
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
        public List<PBEBattlePokemon> ActiveBattlers { get; } = new List<PBEBattlePokemon>(6);
        private readonly List<PBEBattlePokemon> _turnOrder = new List<PBEBattlePokemon>(6);

        public PBEWeather Weather { get; set; }
        public byte WeatherCounter { get; set; }
        public PBEBattleStatus BattleStatus { get; set; }
        public byte TrickRoomCount { get; set; }

        public List<IPBEPacket> Events { get; } = new List<IPBEPacket>();

        /// <summary>Gets a specific <see cref="PBEBattlePokemon"/> participating in this battle by its ID.</summary>
        /// <param name="pkmnId">The ID of the <see cref="PBEBattlePokemon"/>.</param>
        public PBEBattlePokemon TryGetPokemon(byte pkmnId)
        {
            return Teams.SelectMany(t => t.Party).SingleOrDefault(p => p.Id == pkmnId);
        }

        // TODO: Constructor with weather
        public PBEBattle(PBEBattleTerrain battleTerrain, PBEBattleFormat battleFormat, PBETeamInfo ti0, PBETeamInfo ti1, PBESettings settings)
        {
            if (battleTerrain >= PBEBattleTerrain.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(battleTerrain));
            }
            if (battleFormat >= PBEBattleFormat.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(battleFormat));
            }
            if (ti0 == null)
            {
                throw new ArgumentNullException(nameof(ti0));
            }
            if (ti1 == null)
            {
                throw new ArgumentNullException(nameof(ti1));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            BattleTerrain = battleTerrain;
            BattleFormat = battleFormat;
            Settings = settings;
            Teams = new PBETeams(this, ti0, ti1);
            CheckForReadiness();
        }
        /// <summary>Creates a new <see cref="PBEBattle"/> object with the specified <see cref="PBEBattleFormat"/> and a copy of the specified <see cref="PBESettings"/>. <see cref="BattleState"/> will be <see cref="PBEBattleState.WaitingForPlayers"/>.</summary>
        /// <param name="battleTerrain">The <see cref="PBEBattleTerrain"/> of the battle.</param>
        /// <param name="battleFormat">The <see cref="PBEBattleFormat"/> of the battle.</param>
        /// <param name="settings">The <see cref="PBESettings"/> to copy for the battle to use.</param>
        public PBEBattle(PBEBattleTerrain battleTerrain, PBEBattleFormat battleFormat, PBESettings settings)
        {
            if (battleTerrain >= PBEBattleTerrain.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(battleTerrain));
            }
            if (battleFormat >= PBEBattleFormat.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(battleFormat));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            BattleTerrain = battleTerrain;
            BattleFormat = battleFormat;
            Settings = settings;
            Teams = new PBETeams(this);
            BattleState = PBEBattleState.WaitingForPlayers;
            OnStateChanged?.Invoke(this);
        }
        private void CheckForReadiness()
        {
            if (Teams.All(t => t.NumConsciousPkmn > 0))
            {
                switch (BattleFormat)
                {
                    case PBEBattleFormat.Single:
                    {
                        foreach (PBETeam team in Teams)
                        {
                            team.SwitchInQueue.Add((team.Party[0], PBEFieldPosition.Center));
                        }
                        break;
                    }
                    case PBEBattleFormat.Double:
                    {
                        foreach (PBETeam team in Teams)
                        {
                            team.SwitchInQueue.Add((team.Party[0], PBEFieldPosition.Left));
                            if (team.Party.Count > 1)
                            {
                                team.SwitchInQueue.Add((team.Party[1], PBEFieldPosition.Right));
                            }
                        }
                        break;
                    }
                    case PBEBattleFormat.Triple:
                    {
                        foreach (PBETeam team in Teams)
                        {
                            team.SwitchInQueue.Add((team.Party[0], PBEFieldPosition.Left));
                            if (team.Party.Count > 1)
                            {
                                team.SwitchInQueue.Add((team.Party[1], PBEFieldPosition.Center));
                            }
                            if (team.Party.Count > 2)
                            {
                                team.SwitchInQueue.Add((team.Party[2], PBEFieldPosition.Right));
                            }
                        }
                        break;
                    }
                    case PBEBattleFormat.Rotation:
                    {
                        foreach (PBETeam team in Teams)
                        {
                            team.SwitchInQueue.Add((team.Party[0], PBEFieldPosition.Center));
                            if (team.Party.Count > 1)
                            {
                                team.SwitchInQueue.Add((team.Party[1], PBEFieldPosition.Left));
                            }
                            if (team.Party.Count > 2)
                            {
                                team.SwitchInQueue.Add((team.Party[2], PBEFieldPosition.Right));
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
        /// <param name="ti">The information <paramref name="team"/> will use to create its party.</param>
        /// <param name="teamName">The name of the trainer(s) on <paramref name="team"/>.</param>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.WaitingForPlayers"/> or <paramref name="team"/> already has its party set.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="team"/> or <paramref name="ti"/> == null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="ti"/>'s settings are unequal to <paramref name="team"/>'s battle's settings or when <paramref name="teamName"/> is invalid.</exception>
        public static void CreateTeamParty(PBETeam team, PBETeamInfo ti)
        {
            if (team == null)
            {
                throw new ArgumentNullException(nameof(team));
            }
            if (ti == null)
            {
                throw new ArgumentNullException(nameof(ti));
            }
            if (team.Battle.BattleState != PBEBattleState.WaitingForPlayers)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.WaitingForPlayers} to set a team's party.");
            }
            if (team.Party.Count > 0)
            {
                throw new InvalidOperationException("This team already has its party set.");
            }
            team.CreateParty(ti);
            team.Battle.CheckForReadiness();
        }
        /// <summary>Begins the battle.</summary>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="BattleState"/> is not <see cref="PBEBattleState.ReadyToBegin"/>.</exception>
        public void Begin()
        {
            if (BattleState != PBEBattleState.ReadyToBegin)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {PBEBattleState.ReadyToBegin} to begin the battle.");
            }
            foreach (PBETeam team in Teams)
            {
                BroadcastTeam(team);
            }
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
            PBETeam[] teamsWithSwitchIns = Teams.Where(t => t.SwitchInQueue.Count > 0).ToArray();
            if (teamsWithSwitchIns.Length > 0)
            {
                var list = new List<PBEBattlePokemon>(6);
                foreach (PBETeam team in teamsWithSwitchIns)
                {
                    int count = team.SwitchInQueue.Count;
                    var switches = new PBEPkmnSwitchInPacket.PBESwitchInInfo[count];
                    for (int i = 0; i < count; i++)
                    {
                        (PBEBattlePokemon pkmn, PBEFieldPosition pos) = team.SwitchInQueue[i];
                        pkmn.FieldPosition = pos;
                        switches[i] = CreateSwitchInInfo(pkmn);
                        PBETeam.SwitchTwoPokemon(pkmn, pos);
                        list.Add(pkmn);
                    }
                    BroadcastPkmnSwitchIn(team, switches);
                }
                ActiveBattlers.AddRange(list);
                DoSwitchInEffects(list);
            }

            foreach (PBETeam team in Teams)
            {
                int available = team.NumConsciousPkmn - team.NumPkmnOnField;
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
                }
                foreach (PBETeam team in Teams)
                {
                    if (team.TeamStatus.HasFlag(PBETeamStatus.WideGuard))
                    {
                        BroadcastTeamStatus(team, PBETeamStatus.WideGuard, PBETeamStatusAction.Ended);
                    }
                }
                foreach (PBETeam team in Teams)
                {
                    team.ActionsRequired.Clear();
                    team.ActionsRequired.AddRange(team.ActiveBattlers);
                }

                if (BattleFormat == PBEBattleFormat.Triple && Teams.All(t => t.NumConsciousPkmn == 1))
                {
                    PBEBattlePokemon pkmn1 = ActiveBattlers[0],
                        pkmn2 = ActiveBattlers[1];
                    if ((pkmn1.FieldPosition == PBEFieldPosition.Left && pkmn2.FieldPosition == PBEFieldPosition.Left) || (pkmn1.FieldPosition == PBEFieldPosition.Right && pkmn2.FieldPosition == PBEFieldPosition.Right))
                    {
                        PBEFieldPosition pkmn1OldPos = pkmn1.FieldPosition,
                            pkmn2OldPos = pkmn2.FieldPosition;
                        pkmn1.FieldPosition = PBEFieldPosition.Center;
                        pkmn2.FieldPosition = PBEFieldPosition.Center;
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
        private PBEBattlePokemon[] GetActingOrder(IEnumerable<PBEBattlePokemon> pokemon, bool ignoreItemsThatActivate)
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
            return evaluated.Select(t => t.Pokemon).ToArray();
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
