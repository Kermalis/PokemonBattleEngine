using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed class PBETeam
    {
        public readonly PBEBattle Battle;
        public string TrainerName;
        public readonly bool LocalTeam;
        public List<PBEPokemon> Party { get; private set; } // TODO: Do not allow outsiders to add

        public PBEPokemon[] ActiveBattlers => Battle.ActiveBattlers.Where(p => p.LocalTeam == LocalTeam).ToArray();
        public int NumPkmnAlive => Party.Count(p => p.HP > 0);
        public int NumPkmnOnField => Party.Count(p => p.FieldPosition != PBEFieldPosition.None);

        public List<PBEPokemon> ActionsRequired { get; } = new List<PBEPokemon>(); // PBEBattleState.WaitingForActions // TODO: Do not allow outsiders to add
        public byte SwitchInsRequired { get; internal set; } // PBEBattleState.WaitingForSwitchIns
        public List<PBEPokemon> SwitchInQueue { get; } = new List<PBEPokemon>(); // PBEBattleState.WaitingForSwitchIns // TODO: Do not allow outsiders to add

        public PBETeamStatus Status;
        public byte ReflectCount, LightScreenCount; // Reflect & Light Screen
        public byte SpikeCount, ToxicSpikeCount; // Spikes & Toxic Spikes
        public bool MonFaintedLastTurn; // Retaliate

        // Host constructor
        internal PBETeam(PBEBattle battle, PBETeamShell shell, bool localTeam, ref byte idCount)
        {
            Battle = battle;
            TrainerName = shell.PlayerName;
            LocalTeam = localTeam;
            Party = new List<PBEPokemon>(PBESettings.MaxPartySize);
            for (int i = 0; i < shell.Party.Count; i++)
            {
                Party.Add(new PBEPokemon(idCount++, shell.Party[i]) { LocalTeam = localTeam });
            }
        }
        // Client constructor
        internal PBETeam(PBEBattle battle, bool localTeam)
        {
            Battle = battle;
            LocalTeam = localTeam;
            Party = new List<PBEPokemon>(PBESettings.MaxPartySize);
        }

        // Returns null if there is no Pokémon at that position
        public PBEPokemon PokemonAtPosition(PBEFieldPosition pos) => Party.SingleOrDefault(p => p.FieldPosition == pos);

        internal void SetParty(IEnumerable<PBEPokemon> party)
        {
            Party = new List<PBEPokemon>(party);
            foreach (PBEPokemon p in Party)
            {
                p.LocalTeam = LocalTeam;
            }
        }
    }
    public sealed partial class PBEBattle
    {
        public delegate void BattleStateChangedEvent(PBEBattle battle);
        public event BattleStateChangedEvent OnStateChanged;
        public PBEBattleState BattleState { get; private set; }

        public readonly PBEBattleFormat BattleFormat;
        public readonly PBETeam[] Teams = new PBETeam[2];
        public readonly List<PBEPokemon> ActiveBattlers = new List<PBEPokemon>(); // TODO: Do not allow outsiders to add
        List<PBEPokemon> turnOrder = new List<PBEPokemon>();

        public PBEWeather Weather;
        public byte WeatherCounter;

        // Returns null if it doesn't exist
        public PBEPokemon GetPokemon(byte pkmnId) => Teams[0].Party.Concat(Teams[1].Party).SingleOrDefault(p => p.Id == pkmnId);

        // Host constructor
        public PBEBattle(PBEBattleFormat battleFormat, PBETeamShell localTeamShell, PBETeamShell remoteTeamShell)
        {
            BattleFormat = battleFormat;

            byte idCount = 0;
            Teams[0] = new PBETeam(this, localTeamShell, true, ref idCount);
            Teams[1] = new PBETeam(this, remoteTeamShell, false, ref idCount);

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
            }

            BattleState = PBEBattleState.ReadyToBegin;
            OnStateChanged?.Invoke(this);
        }
        // Client constructor
        public PBEBattle(PBEBattleFormat battleFormat)
        {
            BattleFormat = battleFormat;

            Teams[0] = new PBETeam(this, true);
            Teams[1] = new PBETeam(this, false);

            BattleState = PBEBattleState.WaitingForPlayers;
            OnStateChanged?.Invoke(this);
        }
        public void SetTeamParty(bool localTeam, IEnumerable<PBEPokemon> party)
        {
            if (BattleState != PBEBattleState.WaitingForPlayers)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBEBattleState.WaitingForPlayers)} to set a team's party.");
            }
            Teams[localTeam ? 0 : 1].SetParty(party);
            if (Teams[0].NumPkmnAlive > 1 && Teams[1].NumPkmnAlive > 1)
            {
                BattleState = PBEBattleState.ReadyToBegin;
                OnStateChanged?.Invoke(this);
            }
        }
        // For clients
        public void RemotePokemonSwitchedIn(PBEPkmnSwitchInPacket psip)
        {
            if (psip.LocalTeam)
            {
                return;
            }
            PBEPokemon pkmn = GetPokemon(psip.PokemonId);
            if (pkmn == null)
            {
                // Use remote Pokémon constructor which sets LocalTeam to false and moves to PBEMove.MAX
                pkmn = new PBEPokemon(psip);
                Teams[1].Party.Add(pkmn);
            }
            // If this Pokémon was already added, the client already knows info other than hp (could have Regenerator or could have been healed by an ally)
            pkmn.HP = psip.HP;
            pkmn.MaxHP = psip.MaxHP;
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
        // Switches in all Pokémon in PBETeam.SwitchInQueue
        // Sets BattleState to PBEBattleState.Processing
        void SwitchInQueuedPokemon()
        {
            BattleState = PBEBattleState.Processing;
            OnStateChanged?.Invoke(this);
            IEnumerable<PBEPokemon> all = Teams[0].SwitchInQueue.Concat(Teams[1].SwitchInQueue);
            foreach (PBEPokemon pkmn in all)
            {
                ActiveBattlers.Add(pkmn);
                BroadcastPkmnSwitchIn(pkmn);
            }
            foreach (PBEPokemon pkmn in all)
            {
                DoSwitchInEffects(pkmn); // BattleEffects.cs
            }
        }
        // Sets BattleState to PBEBattleState.WaitingForActions
        void RequestActions()
        {
            foreach (PBETeam team in Teams)
            {
                team.ActionsRequired.Clear();
                team.ActionsRequired.AddRange(team.ActiveBattlers);
                BroadcastActionsRequest(team.LocalTeam, team.ActionsRequired);
            }
            BattleState = PBEBattleState.WaitingForActions;
            OnStateChanged?.Invoke(this);
        }
        void DetermineTurnOrder()
        {
            turnOrder.Clear();
            IEnumerable<PBEPokemon> pkmnSwitchingOut = ActiveBattlers.Where(p => p.SelectedAction.Decision == PBEDecision.Switch);
            IEnumerable<PBEPokemon> pkmnFighting = ActiveBattlers.Where(p => p.SelectedAction.Decision == PBEDecision.Fight);
            // Switching happens first:
            turnOrder.AddRange(pkmnSwitchingOut);
            // Moves:
            // Highest priority is +5, lowest is -7
            for (int i = +5; i >= -7; i--)
            {
                IEnumerable<PBEPokemon> pkmnWithThisPriority = pkmnFighting.Where(p => PBEMoveData.Data[p.SelectedAction.FightMove].Priority == i);
                if (pkmnWithThisPriority.Count() == 0)
                {
                    continue;
                }
                Debug.WriteLine("Priority {0} bracket...", i);
                var evaluated = new List<Tuple<PBEPokemon, double>>(); // TODO: two bools for wanting to go first or last
                foreach (PBEPokemon pkmn in pkmnWithThisPriority)
                {
                    double speed = pkmn.Speed * GetStatMultiplier(pkmn.SpeedChange);

                    switch (pkmn.Item)
                    {
                        case PBEItem.ChoiceScarf:
                            if (pkmn.Item == PBEItem.ChoiceScarf)
                            {
                                speed *= 1.5;
                            }
                            break;
                        case PBEItem.MachoBrace:
                            if (pkmn.Item == PBEItem.MachoBrace)
                            {
                                speed *= 0.5;
                            }
                            break;
                        case PBEItem.QuickPowder:
                            if (pkmn.Species == PBESpecies.Ditto)
                            {
                                speed *= 2.0;
                            }
                            break;
                    }
                    // Paralyzed Pokémon get a 75% speed decrease
                    if (pkmn.Status1 == PBEStatus1.Paralyzed)
                    {
                        speed *= 0.25;
                    }
                    Debug.WriteLine("{0} {1}'s evaluated speed: {2}", pkmn.LocalTeam ? "LocalTeam" : "Remote", pkmn.Shell.Nickname, speed);
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
                            int pkmnToGoBefore = evaluated.FindIndex(t => t.Item2 < speed);
                            if (pkmnToGoBefore == -1)
                            {
                                evaluated.Add(tup); // All evaluated Pokémon are faster than this one
                            }
                            else
                            {
                                evaluated.Insert(pkmnToGoBefore, tup);
                            }
                        }
                    }
                    Debug.WriteLine(evaluated.Select(t => $"{(t.Item1.LocalTeam ? "LocalTeam" : "Remote")} {t.Item1.Shell.Nickname} {t.Item2}").Print());
                }
                turnOrder.AddRange(evaluated.Select(t => t.Item1));
            }
        }
        void RunActionsInOrder()
        {
            foreach (PBEPokemon pkmn in turnOrder)
            {
                if (pkmn.HP < 1)
                {
                    continue;
                }
                switch (pkmn.SelectedAction.Decision)
                {
                    case PBEDecision.Fight:
                        DoPreMoveEffects(pkmn); // BattleEffects.cs
                        UseMove(pkmn); // BattleEffects.cs
                        break;
                    case PBEDecision.Switch:
                        PBEFieldPosition pos = pkmn.FieldPosition;
                        pkmn.ClearForSwitch();
                        ActiveBattlers.Remove(pkmn);
                        BroadcastPkmnSwitchOut(pkmn);
                        PBEPokemon switchPkmn = GetPokemon(pkmn.SelectedAction.SwitchPokemonId);
                        switchPkmn.FieldPosition = pos;
                        ActiveBattlers.Add(switchPkmn);
                        BroadcastPkmnSwitchIn(switchPkmn);
                        DoSwitchInEffects(switchPkmn);
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(pkmn.SelectedAction.Decision), $"Invalid decision: {pkmn.SelectedAction.Decision}");
                }
                pkmn.PreviousAction = pkmn.SelectedAction;
            }
        }
        // Sets BattleState to PBEBattleState.WaitingForActions/PBEBattleState.WaitingForSwitches/PBEBattleState.Ended
        void TurnEnded()
        {
            // Pokémon
            foreach (PBEPokemon pkmn in ActiveBattlers.ToArray()) // Copy the list so a faint does not cause a collection modified exception
            {
                pkmn.SelectedAction.Decision = PBEDecision.None;
                pkmn.Status2 &= ~PBEStatus2.Flinching;
                pkmn.Status2 &= ~PBEStatus2.Protected;
                if (pkmn.PreviousAction.Decision == PBEDecision.Fight && pkmn.PreviousAction.FightMove != PBEMove.Protect && pkmn.PreviousAction.FightMove != PBEMove.Detect)
                {
                    pkmn.ProtectCounter = 0;
                }
                if (pkmn.HP > 0)
                {
                    DoTurnEndedEffects(pkmn); // BattleEffects.cs
                }
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
                if (team.Status.HasFlag(PBETeamStatus.Reflect))
                {
                    team.ReflectCount--;
                    if (team.ReflectCount == 0)
                    {
                        team.Status &= ~PBETeamStatus.Reflect;
                        BroadcastTeamStatus(team.LocalTeam, PBETeamStatus.Reflect, PBETeamStatusAction.Ended);
                    }
                }
                if (team.Status.HasFlag(PBETeamStatus.LightScreen))
                {
                    team.LightScreenCount--;
                    if (team.LightScreenCount == 0)
                    {
                        team.Status &= ~PBETeamStatus.LightScreen;
                        BroadcastTeamStatus(team.LocalTeam, PBETeamStatus.LightScreen, PBETeamStatusAction.Ended);
                    }
                }
            }

            // Weather
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
                        if (available > 0 && team.PokemonAtPosition(PBEFieldPosition.Center) == null)
                        {
                            team.SwitchInsRequired = 1;
                            nextState = PBEBattleState.WaitingForSwitchIns;
                            BroadcastSwitchInRequest(team.LocalTeam, team.SwitchInsRequired);
                        }
                        break;
                    case PBEBattleFormat.Double:
                        if (available > 0 && team.PokemonAtPosition(PBEFieldPosition.Left) == null)
                        {
                            available--;
                            team.SwitchInsRequired++;
                        }
                        if (available > 0 && team.PokemonAtPosition(PBEFieldPosition.Right) == null)
                        {
                            team.SwitchInsRequired++;
                        }
                        if (team.SwitchInsRequired > 0)
                        {
                            nextState = PBEBattleState.WaitingForSwitchIns;
                            BroadcastSwitchInRequest(team.LocalTeam, team.SwitchInsRequired);
                        }
                        break;
                    case PBEBattleFormat.Rotation:
                    case PBEBattleFormat.Triple:
                        if (available > 0 && team.PokemonAtPosition(PBEFieldPosition.Left) == null)
                        {
                            available--;
                            team.SwitchInsRequired++;
                        }
                        if (available > 0 && team.PokemonAtPosition(PBEFieldPosition.Center) == null)
                        {
                            available--;
                            team.SwitchInsRequired++;
                        }
                        if (available > 0 && team.PokemonAtPosition(PBEFieldPosition.Right) == null)
                        {
                            team.SwitchInsRequired++;
                        }
                        if (team.SwitchInsRequired > 0)
                        {
                            nextState = PBEBattleState.WaitingForSwitchIns;
                            BroadcastSwitchInRequest(team.LocalTeam, team.SwitchInsRequired);
                        }
                        break;
                }
            }

            if (nextState == PBEBattleState.WaitingForActions)
            {
                RequestActions();
            }
            else // PBEBattleState.WaitingForSwitchIns
            {
                BattleState = nextState;
                OnStateChanged?.Invoke(this);
            }
        }
    }
}
