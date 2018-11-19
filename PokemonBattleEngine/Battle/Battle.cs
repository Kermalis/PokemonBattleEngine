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
        public string TrainerName;
        public readonly bool Local;
        public List<PPokemon> Party { get; private set; }

        public int NumPkmnAlive => Party.Count(p => p.HP > 0);
        public int NumPkmnOnField => Party.Count(p => p.FieldPosition != PFieldPosition.None);

        public PTeamStatus Status;
        public byte ReflectCount, LightScreenCount; // Reflect & Light Screen
        public byte SpikeCount; // Spikes
        public bool MonFaintedLastTurn; // Retaliate

        // Host constructor
        internal PTeam(PTeamShell shell, bool local, ref byte idCount)
        {
            TrainerName = shell.PlayerName;
            Local = local;
            Party = new List<PPokemon>(PSettings.MaxPartySize);
            for (int i = 0; i < shell.Party.Count; i++)
                Party.Add(new PPokemon(idCount++, shell.Party[i]) { Local = local });
        }
        // Client constructor
        internal PTeam(bool local)
        {
            Local = local;
            Party = new List<PPokemon>(PSettings.MaxPartySize);
        }

        // Returns null if there is no Pokémon at that position
        public PPokemon PokemonAtPosition(PFieldPosition pos) => Party.SingleOrDefault(p => p.FieldPosition == pos);

        public void SetParty(IEnumerable<PPokemon> party)
        {
            Party = new List<PPokemon>(party);
            foreach (PPokemon p in Party)
                p.Local = Local;
        }
    }
    public sealed partial class PBattle
    {
        public readonly PBattleStyle BattleStyle;
        public readonly PTeam[] Teams = new PTeam[2];
        List<PPokemon> activeBattlers = new List<PPokemon>();
        List<PPokemon> turnOrder = new List<PPokemon>();

        public PWeather Weather;
        public byte WeatherCounter;

        // Returns null if it doesn't exist
        public PPokemon GetPokemon(byte pkmnId) => Teams[0].Party.Concat(Teams[1].Party).SingleOrDefault(p => p.Id == pkmnId);
        public bool TemporaryKeepBattlingBool => Teams[0].NumPkmnOnField > 0 && Teams[1].NumPkmnOnField > 0; // Temporary

        // Host constructor
        public PBattle(PBattleStyle style, PTeamShell t0, PTeamShell t1)
        {
            BattleStyle = style;

            byte idCount = 0;
            Teams[0] = new PTeam(t0, true, ref idCount);
            Teams[1] = new PTeam(t1, false, ref idCount);

            // Set pokemon field positions
            switch (BattleStyle)
            {
                case PBattleStyle.Single:
                    Teams[0].Party[0].FieldPosition = PFieldPosition.Center;
                    activeBattlers.Add(Teams[0].Party[0]);
                    Teams[1].Party[0].FieldPosition = PFieldPosition.Center;
                    activeBattlers.Add(Teams[1].Party[0]);
                    break;
                case PBattleStyle.Double:
                    Teams[0].Party[0].FieldPosition = PFieldPosition.Left;
                    activeBattlers.Add(Teams[0].Party[0]);
                    if (Teams[0].Party.Count > 1)
                    {
                        Teams[0].Party[1].FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(Teams[0].Party[1]);
                    }
                    Teams[1].Party[0].FieldPosition = PFieldPosition.Left;
                    activeBattlers.Add(Teams[1].Party[0]);
                    if (Teams[1].Party.Count > 1)
                    {
                        Teams[1].Party[1].FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(Teams[1].Party[1]);
                    }
                    break;
                case PBattleStyle.Triple:
                    Teams[0].Party[0].FieldPosition = PFieldPosition.Left;
                    activeBattlers.Add(Teams[0].Party[0]);
                    if (Teams[0].Party.Count > 1)
                    {
                        Teams[0].Party[1].FieldPosition = PFieldPosition.Center;
                        activeBattlers.Add(Teams[0].Party[1]);
                    }
                    if (Teams[0].Party.Count > 2)
                    {
                        Teams[0].Party[2].FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(Teams[0].Party[2]);
                    }
                    Teams[1].Party[0].FieldPosition = PFieldPosition.Left;
                    activeBattlers.Add(Teams[1].Party[0]);
                    if (Teams[1].Party.Count > 1)
                    {
                        Teams[1].Party[1].FieldPosition = PFieldPosition.Center;
                        activeBattlers.Add(Teams[1].Party[1]);
                    }
                    if (Teams[1].Party.Count > 2)
                    {
                        Teams[1].Party[2].FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(Teams[1].Party[2]);
                    }
                    break;
                case PBattleStyle.Rotation:
                    Teams[0].Party[0].FieldPosition = PFieldPosition.Center;
                    activeBattlers.Add(Teams[0].Party[0]);
                    if (Teams[0].Party.Count > 1)
                    {
                        Teams[0].Party[1].FieldPosition = PFieldPosition.Left;
                        activeBattlers.Add(Teams[0].Party[1]);
                    }
                    if (Teams[0].Party.Count > 2)
                    {
                        Teams[0].Party[2].FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(Teams[0].Party[2]);
                    }
                    Teams[1].Party[0].FieldPosition = PFieldPosition.Center;
                    activeBattlers.Add(Teams[1].Party[0]);
                    if (Teams[1].Party.Count > 1)
                    {
                        Teams[1].Party[1].FieldPosition = PFieldPosition.Left;
                        activeBattlers.Add(Teams[1].Party[1]);
                    }
                    if (Teams[1].Party.Count > 2)
                    {
                        Teams[1].Party[2].FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(Teams[1].Party[2]);
                    }
                    break;
            }
        }
        // Client constructor
        public PBattle(PBattleStyle style)
        {
            BattleStyle = style;

            Teams[0] = new PTeam(true);
            Teams[1] = new PTeam(false);
        }
        public void Start()
        {
            foreach (PPokemon pkmn in activeBattlers)
                BroadcastSwitchIn(pkmn);
            foreach (PPokemon pkmn in activeBattlers)
                DoSwitchInEffects(pkmn); // BattleEffects.cs
        }

        public bool IsReadyToRunTurn() => activeBattlers.All(b => b.SelectedAction.Decision != PDecision.None);

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

        public bool RunTurn()
        {
            if (!IsReadyToRunTurn())
                return false;

            DetermineTurnOrder();
            RunActionsInOrder();
            TurnEnded();

            return true;
        }
        void DetermineTurnOrder()
        {
            turnOrder.Clear();
            IEnumerable<PPokemon> pkmnSwitchingOut = activeBattlers.Where(p => p.SelectedAction.Decision == PDecision.Switch);
            IEnumerable<PPokemon> pkmnFighting = activeBattlers.Where(p => p.SelectedAction.Decision == PDecision.Fight);
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

                    // Pokémon holding a Choice Scarf get a 50% speed increase
                    if (pkmn.Item == PItem.ChoiceScarf)
                        speed *= 1.5;
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
                        activeBattlers.Remove(pkmn);
                        BroadcastSwitchOut(pkmn);
                        PPokemon switchPkmn = GetPokemon(pkmn.SelectedAction.SwitchPokemonId);
                        switchPkmn.FieldPosition = pos;
                        activeBattlers.Add(switchPkmn);
                        BroadcastSwitchIn(switchPkmn);
                        DoSwitchInEffects(switchPkmn);
                        break;
                }
                pkmn.PreviousAction = pkmn.SelectedAction;
            }
        }
        void TurnEnded()
        {
            foreach (PPokemon pkmn in activeBattlers.ToArray()) // Copy the list so a faint does not cause a collection modified exception
            {
                pkmn.SelectedAction.Decision = PDecision.None;
                pkmn.Status2 &= ~PStatus2.Flinching;
                pkmn.Status2 &= ~PStatus2.Protected;
                if (pkmn.PreviousAction.Decision == PDecision.Fight && pkmn.PreviousAction.FightMove != PMove.Protect && pkmn.PreviousAction.FightMove != PMove.Detect)
                    pkmn.ProtectCounter = 0;
                if (pkmn.HP > 0)
                    DoTurnEndedEffects(pkmn); // BattleEffects.cs
            }
            foreach (PTeam team in Teams)
            {
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
        }
    }
}
