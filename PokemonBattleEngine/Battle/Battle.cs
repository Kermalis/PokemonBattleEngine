using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed class PTeam
    {
        public readonly PBattle Battle;
        public string TrainerName;
        public readonly bool Local;
        public List<PPokemon> Party { get; private set; } // TODO: Do not allow outsiders to add

        public PPokemon[] ActiveBattlers => Battle.ActiveBattlers.Where(p => p.Local == Local).ToArray();
        public int NumPkmnAlive => Party.Count(p => p.HP > 0);
        public int NumPkmnOnField => Party.Count(p => p.FieldPosition != PFieldPosition.None);

        public List<PPokemon> ActionsRequired { get; } = new List<PPokemon>(); // PBattleState.WaitingForActions // TODO: Do not allow outsiders to add
        public byte SwitchInsRequired { get; internal set; } // PBattleState.WaitingForSwitchIns
        public List<PPokemon> SwitchInQueue { get; } = new List<PPokemon>(); // PBattleState.WaitingForSwitchIns // TODO: Do not allow outsiders to add

        public PTeamStatus Status;
        public byte ReflectCount, LightScreenCount; // Reflect & Light Screen
        public byte SpikeCount, ToxicSpikeCount; // Spikes & Toxic Spikes
        public bool MonFaintedLastTurn; // Retaliate

        // Host constructor
        internal PTeam(PBattle battle, PTeamShell shell, bool local, ref byte idCount)
        {
            Battle = battle;
            TrainerName = shell.PlayerName;
            Local = local;
            Party = new List<PPokemon>(PSettings.MaxPartySize);
            for (int i = 0; i < shell.Party.Count; i++)
                Party.Add(new PPokemon(idCount++, shell.Party[i]) { Local = local });
        }
        // Client constructor
        internal PTeam(PBattle battle, bool local)
        {
            Battle = battle;
            Local = local;
            Party = new List<PPokemon>(PSettings.MaxPartySize);
        }

        // Returns null if there is no Pokémon at that position
        public PPokemon PokemonAtPosition(PFieldPosition pos) => Party.SingleOrDefault(p => p.FieldPosition == pos);

        internal void SetParty(IEnumerable<PPokemon> party)
        {
            Party = new List<PPokemon>(party);
            foreach (PPokemon p in Party)
                p.Local = Local;
        }
    }
    public sealed partial class PBattle
    {
        public delegate void BattleStateChangedEvent(PBattle battle);
        public event BattleStateChangedEvent OnStateChanged;
        public PBattleState BattleState { get; private set; }

        public readonly PBattleStyle BattleStyle;
        public readonly PTeam[] Teams = new PTeam[2];
        public readonly List<PPokemon> ActiveBattlers = new List<PPokemon>(); // TODO: Do not allow outsiders to add
        List<PPokemon> turnOrder = new List<PPokemon>();

        public PWeather Weather;
        public byte WeatherCounter; // Not updated for client

        // Returns null if it doesn't exist
        public PPokemon GetPokemon(byte pkmnId) => Teams[0].Party.Concat(Teams[1].Party).SingleOrDefault(p => p.Id == pkmnId);

        // Host constructor
        public PBattle(PBattleStyle style, PTeamShell t0, PTeamShell t1)
        {
            BattleStyle = style;

            byte idCount = 0;
            Teams[0] = new PTeam(this, t0, true, ref idCount);
            Teams[1] = new PTeam(this, t1, false, ref idCount);

            // Set pokemon field positions
            switch (BattleStyle)
            {
                case PBattleStyle.Single:
                    Teams[0].Party[0].FieldPosition = PFieldPosition.Center;
                    Teams[0].SwitchInQueue.Add(Teams[0].Party[0]);
                    Teams[1].Party[0].FieldPosition = PFieldPosition.Center;
                    Teams[1].SwitchInQueue.Add(Teams[1].Party[0]);
                    break;
                case PBattleStyle.Double:
                    Teams[0].Party[0].FieldPosition = PFieldPosition.Left;
                    Teams[0].SwitchInQueue.Add(Teams[0].Party[0]);
                    if (Teams[0].Party.Count > 1)
                    {
                        Teams[0].Party[1].FieldPosition = PFieldPosition.Right;
                        Teams[0].SwitchInQueue.Add(Teams[0].Party[1]);
                    }
                    Teams[1].Party[0].FieldPosition = PFieldPosition.Left;
                    Teams[1].SwitchInQueue.Add(Teams[1].Party[0]);
                    if (Teams[1].Party.Count > 1)
                    {
                        Teams[1].Party[1].FieldPosition = PFieldPosition.Right;
                        Teams[1].SwitchInQueue.Add(Teams[1].Party[1]);
                    }
                    break;
                case PBattleStyle.Triple:
                    Teams[0].Party[0].FieldPosition = PFieldPosition.Left;
                    Teams[0].SwitchInQueue.Add(Teams[0].Party[0]);
                    if (Teams[0].Party.Count > 1)
                    {
                        Teams[0].Party[1].FieldPosition = PFieldPosition.Center;
                        Teams[0].SwitchInQueue.Add(Teams[0].Party[1]);
                    }
                    if (Teams[0].Party.Count > 2)
                    {
                        Teams[0].Party[2].FieldPosition = PFieldPosition.Right;
                        Teams[0].SwitchInQueue.Add(Teams[0].Party[2]);
                    }
                    Teams[1].Party[0].FieldPosition = PFieldPosition.Left;
                    Teams[1].SwitchInQueue.Add(Teams[1].Party[0]);
                    if (Teams[1].Party.Count > 1)
                    {
                        Teams[1].Party[1].FieldPosition = PFieldPosition.Center;
                        Teams[1].SwitchInQueue.Add(Teams[1].Party[1]);
                    }
                    if (Teams[1].Party.Count > 2)
                    {
                        Teams[1].Party[2].FieldPosition = PFieldPosition.Right;
                        Teams[1].SwitchInQueue.Add(Teams[1].Party[2]);
                    }
                    break;
                case PBattleStyle.Rotation:
                    Teams[0].Party[0].FieldPosition = PFieldPosition.Center;
                    Teams[0].SwitchInQueue.Add(Teams[0].Party[0]);
                    if (Teams[0].Party.Count > 1)
                    {
                        Teams[0].Party[1].FieldPosition = PFieldPosition.Left;
                        Teams[0].SwitchInQueue.Add(Teams[0].Party[1]);
                    }
                    if (Teams[0].Party.Count > 2)
                    {
                        Teams[0].Party[2].FieldPosition = PFieldPosition.Right;
                        Teams[0].SwitchInQueue.Add(Teams[0].Party[2]);
                    }
                    Teams[1].Party[0].FieldPosition = PFieldPosition.Center;
                    Teams[1].SwitchInQueue.Add(Teams[1].Party[0]);
                    if (Teams[1].Party.Count > 1)
                    {
                        Teams[1].Party[1].FieldPosition = PFieldPosition.Left;
                        Teams[1].SwitchInQueue.Add(Teams[1].Party[1]);
                    }
                    if (Teams[1].Party.Count > 2)
                    {
                        Teams[1].Party[2].FieldPosition = PFieldPosition.Right;
                        Teams[1].SwitchInQueue.Add(Teams[1].Party[2]);
                    }
                    break;
            }

            BattleState = PBattleState.ReadyToBegin;
            OnStateChanged?.Invoke(this);
        }
        // Client constructor
        public PBattle(PBattleStyle style)
        {
            BattleStyle = style;

            Teams[0] = new PTeam(this, true);
            Teams[1] = new PTeam(this, false);

            BattleState = PBattleState.WaitingForPlayers;
            OnStateChanged?.Invoke(this);
        }
        public void SetTeamParty(bool local, IEnumerable<PPokemon> party)
        {
            if (BattleState != PBattleState.WaitingForPlayers)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBattleState.WaitingForPlayers)} to set a team's party.");
            }

            Teams[local ? 0 : 1].SetParty(party);
            if (Teams[0].NumPkmnAlive > 1 && Teams[1].NumPkmnAlive > 1)
            {
                BattleState = PBattleState.ReadyToBegin;
                OnStateChanged?.Invoke(this);
            }
        }
        // For clients
        public void RemotePokemonSwitchedIn(PPkmnSwitchInPacket psip)
        {
            if (psip.Local)
                return;
            PPokemon pkmn = GetPokemon(psip.PokemonId);

            if (pkmn == null)
            {
                // Use remote Pokémon constructor which sets Local to false and moves to PMove.MAX
                pkmn = new PPokemon(psip);
                Teams[1].Party.Add(pkmn);
            }

            // If this Pokémon was already added, the client already knows info other than hp (could have regenerator or could have been healed by an ally)
            pkmn.HP = psip.HP;
            pkmn.MaxHP = psip.MaxHP;
        }
        // Starts the battle
        // Sets BattleState to PBattleState.Processing, then PBattleState.WaitingForActions
        public void Begin()
        {
            if (BattleState != PBattleState.ReadyToBegin)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBattleState.ReadyToBegin)} to begin the battle.");
            }

            SwitchInQueuedPokemon();
            RequestActions();
        }
        // Runs a turn
        // Sets BattleState to PBattleState.Processing, then PBattleState.WaitingForActions/PBattleState.WaitingForSwitches/PBattleState.Ended
        public void RunTurn()
        {
            if (BattleState != PBattleState.ReadyToRunTurn)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBattleState.ReadyToRunTurn)} to run a turn.");
            }

            BattleState = PBattleState.Processing;
            OnStateChanged?.Invoke(this);

            DetermineTurnOrder();
            RunActionsInOrder();
            TurnEnded();
        }
        // Switches in all Pokémon in PTeam.SwitchInQueue
        // Sets BattleState to PBattleState.Processing
        void SwitchInQueuedPokemon()
        {
            BattleState = PBattleState.Processing;
            OnStateChanged?.Invoke(this);

            IEnumerable<PPokemon> all = Teams[0].SwitchInQueue.Concat(Teams[1].SwitchInQueue);
            foreach (PPokemon pkmn in all)
            {
                ActiveBattlers.Add(pkmn);
                BroadcastSwitchIn(pkmn);
            }
            foreach (PPokemon pkmn in all)
            {
                DoSwitchInEffects(pkmn); // BattleEffects.cs
            }
        }
        // Sets BattleState to PBattleState.WaitingForActions
        void RequestActions()
        {
            foreach (PTeam team in Teams)
            {
                team.ActionsRequired.Clear();
                team.ActionsRequired.AddRange(team.ActiveBattlers);
                BroadcastActionsRequest(team.Local, team.ActionsRequired);
            }
            BattleState = PBattleState.WaitingForActions;
            OnStateChanged?.Invoke(this);
        }
        void DetermineTurnOrder()
        {
            turnOrder.Clear();
            IEnumerable<PPokemon> pkmnSwitchingOut = ActiveBattlers.Where(p => p.SelectedAction.Decision == PDecision.Switch);
            IEnumerable<PPokemon> pkmnFighting = ActiveBattlers.Where(p => p.SelectedAction.Decision == PDecision.Fight);
            // Switching happens first:
            turnOrder.AddRange(pkmnSwitchingOut);
            // Moves:
            // Highest priority is +5, lowest is -7
            for (int i = +5; i >= -7; i--)
            {
                IEnumerable<PPokemon> pkmnWithThisPriority = pkmnFighting.Where(p => PMoveData.Data[p.SelectedAction.FightMove].Priority == i);
                if (pkmnWithThisPriority.Count() == 0)
                    continue;

                Debug.WriteLine("Priority {0} bracket...", i);
                var evaluated = new List<Tuple<PPokemon, double>>(); // TODO: two bools for wanting to go first or last
                foreach (PPokemon pkmn in pkmnWithThisPriority)
                {
                    double speed = pkmn.Speed * GetStatMultiplier(pkmn.SpeedChange);

                    switch (pkmn.Item)
                    {
                        case PItem.ChoiceScarf:
                            if (pkmn.Item == PItem.ChoiceScarf)
                                speed *= 1.5;
                            break;
                        case PItem.MachoBrace:
                            if (pkmn.Item == PItem.MachoBrace)
                                speed *= 0.5;
                            break;
                        case PItem.QuickPowder:
                            if (pkmn.Species == PSpecies.Ditto)
                                speed *= 2.0;
                            break;
                    }
                    // Paralyzed Pokémon get a 75% speed decrease
                    if (pkmn.Status1 == PStatus1.Paralyzed)
                        speed *= 0.25;

                    Debug.WriteLine("{0} {1}'s evaluated speed: {2}", pkmn.Local ? "Local" : "Remote", pkmn.Shell.Nickname, speed);
                    var tup = Tuple.Create(pkmn, speed);
                    if (evaluated.Count == 0)
                    {
                        evaluated.Add(tup);
                    }
                    else
                    {
                        int pkmnTiedWith = evaluated.FindIndex(t => t.Item2 == speed);
                        if (pkmnTiedWith != -1)
                        {
                            if (PUtils.RNG.NextBoolean()) // Randomly go before or after the Pokémon it tied with
                            {
                                if (pkmnTiedWith == evaluated.Count - 1)
                                    evaluated.Add(tup);
                                else
                                    evaluated.Insert(pkmnTiedWith + 1, tup);
                            }
                            else
                            {
                                evaluated.Insert(pkmnTiedWith, tup);
                            }
                        }
                        else
                        {
                            int pkmnToGoBefore = evaluated.FindIndex(t => t.Item2 < speed);
                            if (pkmnToGoBefore == -1)
                                evaluated.Add(tup); // All evaluated Pokémon are faster than this one
                            else
                                evaluated.Insert(pkmnToGoBefore, tup);
                        }
                    }
                    Debug.WriteLine(evaluated.Select(t => $"{(t.Item1.Local ? "Local" : "Remote")} {t.Item1.Shell.Nickname} {t.Item2}").Print());
                }
                turnOrder.AddRange(evaluated.Select(t => t.Item1));
            }
        }
        void RunActionsInOrder()
        {
            foreach (PPokemon pkmn in turnOrder)
            {
                if (pkmn.HP < 1)
                    continue;
                switch (pkmn.SelectedAction.Decision)
                {
                    case PDecision.Fight:
                        DoPreMoveEffects(pkmn); // BattleEffects.cs
                        UseMove(pkmn); // BattleEffects.cs
                        break;
                    case PDecision.Switch:
                        PFieldPosition pos = pkmn.FieldPosition;
                        pkmn.ClearForSwitch();
                        ActiveBattlers.Remove(pkmn);
                        BroadcastSwitchOut(pkmn);
                        PPokemon switchPkmn = GetPokemon(pkmn.SelectedAction.SwitchPokemonId);
                        switchPkmn.FieldPosition = pos;
                        ActiveBattlers.Add(switchPkmn);
                        BroadcastSwitchIn(switchPkmn);
                        DoSwitchInEffects(switchPkmn);
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(pkmn.SelectedAction.Decision), $"Invalid decision: {pkmn.SelectedAction.Decision}");
                }
                pkmn.PreviousAction = pkmn.SelectedAction;
            }
        }
        // Sets BattleState to PBattleState.WaitingForActions/PBattleState.WaitingForSwitches/PBattleState.Ended
        void TurnEnded()
        {
            // Pokémon
            foreach (PPokemon pkmn in ActiveBattlers.ToArray()) // Copy the list so a faint does not cause a collection modified exception
            {
                pkmn.SelectedAction.Decision = PDecision.None;
                pkmn.Status2 &= ~PStatus2.Flinching;
                pkmn.Status2 &= ~PStatus2.Protected;
                if (pkmn.PreviousAction.Decision == PDecision.Fight && pkmn.PreviousAction.FightMove != PMove.Protect && pkmn.PreviousAction.FightMove != PMove.Detect)
                    pkmn.ProtectCounter = 0;
                if (pkmn.HP > 0)
                    DoTurnEndedEffects(pkmn); // BattleEffects.cs
            }

            // Teams
            foreach (PTeam team in Teams)
            {
                if (team.NumPkmnAlive == 0) // TODO: Figure out how wins are determined (tie exists?)
                {
                    BattleState = PBattleState.Ended;
                    OnStateChanged?.Invoke(this);
                    return;
                }
                if (team.Status.HasFlag(PTeamStatus.Reflect))
                {
                    team.ReflectCount--;
                    if (team.ReflectCount == 0)
                    {
                        team.Status &= ~PTeamStatus.Reflect;
                        BroadcastTeamStatus(team.Local, PTeamStatus.Reflect, PTeamStatusAction.Ended);
                    }
                }
                if (team.Status.HasFlag(PTeamStatus.LightScreen))
                {
                    team.LightScreenCount--;
                    if (team.LightScreenCount == 0)
                    {
                        team.Status &= ~PTeamStatus.LightScreen;
                        BroadcastTeamStatus(team.Local, PTeamStatus.LightScreen, PTeamStatusAction.Ended);
                    }
                }
            }

            // Weather
            if (WeatherCounter > 0)
            {
                WeatherCounter--;
                if (WeatherCounter == 0)
                {
                    PWeather w = Weather;
                    Weather = PWeather.None;
                    BroadcastWeather(w, PWeatherAction.Ended);
                }
            }

            PBattleState nextState = PBattleState.WaitingForActions;
            // Requesting a replacement
            foreach (PTeam team in Teams)
            {
                int available = team.NumPkmnAlive - team.NumPkmnOnField;
                team.SwitchInsRequired = 0;
                team.SwitchInQueue.Clear();
                switch (BattleStyle)
                {
                    case PBattleStyle.Single:
                        if (available > 0 && team.PokemonAtPosition(PFieldPosition.Center) == null)
                        {
                            team.SwitchInsRequired = 1;
                            nextState = PBattleState.WaitingForSwitchIns;
                            BroadcastSwitchInRequest(team.Local, team.SwitchInsRequired);
                        }
                        break;
                    case PBattleStyle.Double:
                        if (available > 0 && team.PokemonAtPosition(PFieldPosition.Left) == null)
                        {
                            available--;
                            team.SwitchInsRequired++;
                        }
                        if (available > 0 && team.PokemonAtPosition(PFieldPosition.Right) == null)
                        {
                            team.SwitchInsRequired++;
                        }
                        if (team.SwitchInsRequired > 0)
                        {
                            nextState = PBattleState.WaitingForSwitchIns;
                            BroadcastSwitchInRequest(team.Local, team.SwitchInsRequired);
                        }
                        break;
                    case PBattleStyle.Rotation:
                    case PBattleStyle.Triple:
                        if (available > 0 && team.PokemonAtPosition(PFieldPosition.Left) == null)
                        {
                            available--;
                            team.SwitchInsRequired++;
                        }
                        if (available > 0 && team.PokemonAtPosition(PFieldPosition.Center) == null)
                        {
                            available--;
                            team.SwitchInsRequired++;
                        }
                        if (available > 0 && team.PokemonAtPosition(PFieldPosition.Right) == null)
                        {
                            team.SwitchInsRequired++;
                        }
                        if (team.SwitchInsRequired > 0)
                        {
                            nextState = PBattleState.WaitingForSwitchIns;
                            BroadcastSwitchInRequest(team.Local, team.SwitchInsRequired);
                        }
                        break;
                }
            }

            if (nextState == PBattleState.WaitingForActions)
            {
                RequestActions();
            }
            else // PBattleState.WaitingForSwitchIns
            {
                BattleState = nextState;
                OnStateChanged?.Invoke(this);
            }
        }
    }
}
