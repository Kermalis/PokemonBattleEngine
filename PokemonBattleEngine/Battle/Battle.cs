using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        internal class PTeam
        {
            public readonly PTeamShell Shell;
            public readonly bool Local;
            public readonly PPokemon[] Party;

            public int NumPkmnAlive => Party.Count(p => p.HP > 0);
            public int NumPkmnOnField => Party.Count(p => p.FieldPosition != PFieldPosition.None);

            public byte ReflectCount, LightScreenCount; // Reflect & Light Screen
            public bool MonFaintedLastTurn; // Retaliate

            public PTeam(PTeamShell shell, bool local)
            {
                Shell = shell;
                Local = local;
                int min = Math.Min(shell.Party.Count, PSettings.MaxPartySize);
                Party = new PPokemon[min];
                for (int i = 0; i < min; i++)
                    Party[i] = new PPokemon(Guid.NewGuid(), Shell.Party[i]);

                if (Local)
                    PKnownInfo.Instance.LocalDisplayName = Shell.DisplayName;
                else
                    PKnownInfo.Instance.RemoteDisplayName = Shell.DisplayName;
                PKnownInfo.Instance.SetPartyPokemon(Party, Local);
            }

            // Returns null if there is no pokemon at that position
            public PPokemon BattlerAtPosition(PFieldPosition pos) => Party.SingleOrDefault(p => p.FieldPosition == pos);
        }

        public readonly PBattleStyle BattleStyle;
        internal PTeam[] teams = new PTeam[2];
        List<PPokemon> activeBattlers = new List<PPokemon>();
        List<PPokemon> turnOrder = new List<PPokemon>();

        public PWeather Weather { get; internal set; }
        public byte WeatherCounter { get; internal set; }

        public bool TemporaryKeepBattlingBool => teams[0].NumPkmnOnField > 0 && teams[1].NumPkmnOnField > 0; // Temporary

        public PBattle(PBattleStyle style, PTeamShell t0, PTeamShell t1)
        {
            PKnownInfo.Instance.Clear();
            BattleStyle = style;

            teams[0] = new PTeam(t0, true);
            teams[1] = new PTeam(t1, false);

            // Set pokemon field positions
            switch (BattleStyle)
            {
                case PBattleStyle.Single:
                    teams[0].Party[0].FieldPosition = PFieldPosition.Center;
                    activeBattlers.Add(teams[0].Party[0]);
                    teams[1].Party[0].FieldPosition = PFieldPosition.Center;
                    activeBattlers.Add(teams[1].Party[0]);
                    break;
                case PBattleStyle.Double:
                    teams[0].Party[0].FieldPosition = PFieldPosition.Left;
                    activeBattlers.Add(teams[0].Party[0]);
                    if (teams[0].Party.Length > 1)
                    {
                        teams[0].Party[1].FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(teams[0].Party[1]);
                    }
                    teams[1].Party[0].FieldPosition = PFieldPosition.Left;
                    activeBattlers.Add(teams[1].Party[0]);
                    if (teams[1].Party.Length > 1)
                    {
                        teams[1].Party[1].FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(teams[1].Party[1]);
                    }
                    break;
                case PBattleStyle.Triple:
                    teams[0].Party[0].FieldPosition = PFieldPosition.Left;
                    activeBattlers.Add(teams[0].Party[0]);
                    if (teams[0].Party.Length > 1)
                    {
                        teams[0].Party[1].FieldPosition = PFieldPosition.Center;
                        activeBattlers.Add(teams[0].Party[1]);
                    }
                    if (teams[0].Party.Length > 2)
                    {
                        teams[0].Party[2].FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(teams[0].Party[2]);
                    }
                    teams[1].Party[0].FieldPosition = PFieldPosition.Left;
                    activeBattlers.Add(teams[1].Party[0]);
                    if (teams[1].Party.Length > 1)
                    {
                        teams[1].Party[1].FieldPosition = PFieldPosition.Center;
                        activeBattlers.Add(teams[1].Party[1]);
                    }
                    if (teams[1].Party.Length > 2)
                    {
                        teams[1].Party[2].FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(teams[1].Party[2]);
                    }
                    break;
                case PBattleStyle.Rotation:
                    teams[0].Party[0].FieldPosition = PFieldPosition.Center;
                    activeBattlers.Add(teams[0].Party[0]);
                    if (teams[0].Party.Length > 1)
                    {
                        teams[0].Party[1].FieldPosition = PFieldPosition.Left;
                        activeBattlers.Add(teams[0].Party[1]);
                    }
                    if (teams[0].Party.Length > 2)
                    {
                        teams[0].Party[2].FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(teams[0].Party[2]);
                    }
                    teams[1].Party[0].FieldPosition = PFieldPosition.Center;
                    activeBattlers.Add(teams[1].Party[0]);
                    if (teams[1].Party.Length > 1)
                    {
                        teams[1].Party[1].FieldPosition = PFieldPosition.Left;
                        activeBattlers.Add(teams[1].Party[1]);
                    }
                    if (teams[1].Party.Length > 2)
                    {
                        teams[1].Party[2].FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(teams[1].Party[2]);
                    }
                    break;
            }
        }
        public void Start()
        {
            foreach (PPokemon pkmn in activeBattlers)
                BroadcastSwitchIn(pkmn);
            foreach (PPokemon pkmn in activeBattlers)
                DoSwitchInEffects(pkmn); // BattleEffects.cs
        }

        public bool IsReadyToRunTurn() => activeBattlers.All(b => b.SelectedAction.Decision != PDecision.None);

        public bool RunTurn()
        {
            if (!IsReadyToRunTurn())
                return false;

            DetermineTurnOrder();
            RunMovesInOrder();
            TurnEnded();

            return true;
        }
        void DetermineTurnOrder()
        {
            turnOrder.Clear();
            // Highest priority is +5, lowest is -7
            for (int i = +5; i >= -7; i--)
            {
                IEnumerable<PPokemon> pkmnWithThisPriority = activeBattlers.Where(p => p.SelectedAction.Decision == PDecision.Fight && PMoveData.Data[p.SelectedAction.Move].Priority == i);
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
        void RunMovesInOrder()
        {
            foreach (PPokemon pkmn in turnOrder)
            {
                if (pkmn.HP < 1)
                    continue;
                DoPreMoveEffects(pkmn); // BattleEffects.cs
                UseMove(pkmn); // BattleEffects.cs
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
                if (pkmn.PreviousAction.Decision == PDecision.Fight && pkmn.PreviousAction.Move != PMove.Protect && pkmn.PreviousAction.Move != PMove.Detect)
                    pkmn.ProtectCounter = 0;
                if (pkmn.HP > 0)
                    DoTurnEndedEffects(pkmn); // BattleEffects.cs
            }
            foreach (PTeam team in teams)
            {
                if (team.ReflectCount > 0)
                {
                    team.ReflectCount--;
                    if (team.ReflectCount == 0)
                        BroadcastReflectLightScreen(team.Local, true, PReflectLightScreenAction.Ended);
                }
                if (team.LightScreenCount > 0)
                {
                    team.LightScreenCount--;
                    if (team.LightScreenCount == 0)
                        BroadcastReflectLightScreen(team.Local, false, PReflectLightScreenAction.Ended);
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
