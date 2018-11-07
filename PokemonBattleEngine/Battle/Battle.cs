using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        private class PTeam
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
                int min = Math.Min(shell.Party.Count, PConstants.MaxPartySize);
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
        PTeam[] teams = new PTeam[2];
        List<PPokemon> activeBattlers = new List<PPokemon>();

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

        public bool IsReadyToRunTurn() => activeBattlers.All(b => b.Action.Decision != PDecision.None);

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
            // TODO: Turn order
            // Temporary:
            for (int i = 0; i < activeBattlers.Count; i++)
                activeBattlers[i].TurnOrder = i;
        }
        void RunMovesInOrder()
        {
            PPokemon[] turnOrder = activeBattlers.OrderBy(b => b.TurnOrder).ToArray();
            for (int i = 0; i < turnOrder.Length; i++)
            {
                PPokemon pkmn = turnOrder[i];
                if (pkmn.HP < 1)
                    continue;
                DoPreMoveEffects(pkmn); // BattleEffects.cs
                UseMove(pkmn); // BattleEffects.cs
                pkmn.PreviousMove = bMove;
            }
        }
        void TurnEnded()
        {
            foreach (PPokemon pkmn in activeBattlers.ToArray()) // Copy the list so a faint does not cause a collection modified exception
            {
                pkmn.Action.Decision = PDecision.None;
                pkmn.Status2 &= ~PStatus2.Flinching;
                pkmn.Status2 &= ~PStatus2.Protected;
                if (pkmn.PreviousMove != PMove.Protect && pkmn.PreviousMove != PMove.Detect)
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
        }
    }
}
