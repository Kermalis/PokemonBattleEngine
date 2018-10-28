using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        // TODO: Get rid of this
        private class PBattlePokemon
        {
            public readonly PPokemon Mon;

            public byte Status1Counter;

            public PMove PreviousMove, SelectedMove;
            public PTarget SelectedTarget;
            public int TurnOrder;

            public PBattlePokemon(PPokemon pkmn)
            {
                Mon = pkmn;
            }
        }
        private class PTeam
        {
            public readonly PBattlePokemon[] Party;

            public bool MonFaintedLastTurn; // Retaliate

            public PTeam(PTeamShell data)
            {
                int min = Math.Min(data.Party.Count, PConstants.MaxPartySize);
                Party = new PBattlePokemon[min];
                for (int i = 0; i < min; i++)
                {
                    var pkmn = new PPokemon(Guid.Empty, data.Party[i]);
                    Party[i] = new PBattlePokemon(pkmn);
                }
            }

            // Returns null if there is no pokemon at that position
            public PBattlePokemon BattlerAtPosition(PFieldPosition pos) => Party.SingleOrDefault(p => p.Mon.FieldPosition == pos);
        }

        public readonly PBattleStyle BattleStyle;
        PTeam[] teams = new PTeam[2];
        List<PBattlePokemon> activeBattlers = new List<PBattlePokemon>();
        PBattlePokemon Battler(Guid pkmnId) => activeBattlers.SingleOrDefault(b => b.Mon.Id == pkmnId);

        public PBattle(PBattleStyle style, PTeamShell td0, PTeamShell td1)
        {
            PKnownInfo.Instance.Clear();
            BattleStyle = style;

            teams[0] = new PTeam(td0);
            PKnownInfo.Instance.LocalDisplayName = td0.DisplayName;
            // Team 0 pokemon get (LocallyOwned = true) here:
            PKnownInfo.Instance.SetPartyPokemon(teams[0].Party.Select(p => p.Mon), true);

            teams[1] = new PTeam(td1);
            PKnownInfo.Instance.RemoteDisplayName = td1.DisplayName;
            // Team 1 pokemon get (LocallyOwned = false) here:
            PKnownInfo.Instance.SetPartyPokemon(teams[1].Party.Select(p => p.Mon), false);

            // Set pokemon field positions
            switch (BattleStyle)
            {
                case PBattleStyle.Single:
                    teams[0].Party[0].Mon.FieldPosition = PFieldPosition.Center;
                    activeBattlers.Add(teams[0].Party[0]);
                    teams[1].Party[0].Mon.FieldPosition = PFieldPosition.Center;
                    activeBattlers.Add(teams[1].Party[0]);
                    break;
                case PBattleStyle.Double:
                    teams[0].Party[0].Mon.FieldPosition = PFieldPosition.Left;
                    activeBattlers.Add(teams[0].Party[0]);
                    if (teams[0].Party.Length > 1)
                    {
                        teams[0].Party[1].Mon.FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(teams[0].Party[1]);
                    }
                    teams[1].Party[0].Mon.FieldPosition = PFieldPosition.Left;
                    activeBattlers.Add(teams[1].Party[0]);
                    if (teams[1].Party.Length > 1)
                    {
                        teams[1].Party[1].Mon.FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(teams[1].Party[1]);
                    }
                    break;
                case PBattleStyle.Triple:
                    teams[0].Party[0].Mon.FieldPosition = PFieldPosition.Left;
                    activeBattlers.Add(teams[0].Party[0]);
                    if (teams[0].Party.Length > 1)
                    {
                        teams[0].Party[1].Mon.FieldPosition = PFieldPosition.Center;
                        activeBattlers.Add(teams[0].Party[1]);
                    }
                    if (teams[0].Party.Length > 2)
                    {
                        teams[0].Party[2].Mon.FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(teams[0].Party[2]);
                    }
                    teams[1].Party[0].Mon.FieldPosition = PFieldPosition.Left;
                    activeBattlers.Add(teams[1].Party[0]);
                    if (teams[1].Party.Length > 1)
                    {
                        teams[1].Party[1].Mon.FieldPosition = PFieldPosition.Center;
                        activeBattlers.Add(teams[1].Party[1]);
                    }
                    if (teams[1].Party.Length > 2)
                    {
                        teams[1].Party[2].Mon.FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(teams[1].Party[2]);
                    }
                    break;
                case PBattleStyle.Rotation:
                    teams[0].Party[0].Mon.FieldPosition = PFieldPosition.Center;
                    activeBattlers.Add(teams[0].Party[0]);
                    if (teams[0].Party.Length > 1)
                    {
                        teams[0].Party[1].Mon.FieldPosition = PFieldPosition.Left;
                        activeBattlers.Add(teams[0].Party[1]);
                    }
                    if (teams[0].Party.Length > 2)
                    {
                        teams[0].Party[2].Mon.FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(teams[0].Party[2]);
                    }
                    teams[1].Party[0].Mon.FieldPosition = PFieldPosition.Center;
                    activeBattlers.Add(teams[1].Party[0]);
                    if (teams[1].Party.Length > 1)
                    {
                        teams[1].Party[1].Mon.FieldPosition = PFieldPosition.Left;
                        activeBattlers.Add(teams[1].Party[1]);
                    }
                    if (teams[1].Party.Length > 2)
                    {
                        teams[1].Party[2].Mon.FieldPosition = PFieldPosition.Right;
                        activeBattlers.Add(teams[1].Party[2]);
                    }
                    break;
            }
        }
        public void Start()
        {
            foreach (PBattlePokemon battler in activeBattlers)
                BroadcastSwitchIn(battler.Mon);
        }

        public bool IsActionValid(Guid pkmnId, byte param1, byte param2)
        {
            // TODO: Non-fighting actions (make a class)

            PBattlePokemon pkmn = Battler(pkmnId);

            // Not on the field
            if (pkmn == null)
                return false;

            // Invalid move
            if (param1 >= PConstants.NumMoves || pkmn.Mon.Shell.Moves[param1] == PMove.None)
                return false;

            // TODO: Struggle

            // Out of PP
            if (pkmn.Mon.PP[param1] < 1)
                return false;
            
            // TODO: Targets (param2)
            
            return true;
        }
        // Returns true if the action was valid (and was selected)
        public bool SelectActionIfValid(Guid pkmnId, byte param1, byte param2)
        {
            if (IsActionValid(pkmnId, param1, param2))
            {
                SelectAction(pkmnId, param1, param2);
                return true;
            }
            return false;
        }
        void SelectAction(Guid pkmnId, byte param1, byte param2)
        {
            PBattlePokemon pkmn = Battler(pkmnId);
            // TODO
            // if (action == PAction.Fight)
            pkmn.SelectedMove = pkmn.Mon.Shell.Moves[param1];
            pkmn.SelectedTarget = (PTarget)param2;
        }

        public bool IsReadyToRunTurn()
        {
            return activeBattlers.All(b => b.SelectedMove != PMove.None);
        }

        public bool RunTurn()
        {
            if (!IsReadyToRunTurn())
                return false;

            DetermineTurnOrder();
            ClearTemporaryStuff();
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
        void ClearTemporaryStuff()
        {
            foreach (PBattlePokemon battler in activeBattlers)
            {
                battler.Mon.Status2 &= ~PStatus2.Flinching;
            }
        }
        void RunMovesInOrder()
        {
            PBattlePokemon[] turnOrder = activeBattlers.OrderBy(b => b.TurnOrder).ToArray();
            for (int i = 0; i < turnOrder.Length; i++)
            {
                PBattlePokemon pkmn = turnOrder[i];
                if (pkmn.Mon.HP < 1)
                    continue;
                UseMove(pkmn); // BattleEffects.cs
                pkmn.PreviousMove = bMove;
            }
        }
        void TurnEnded()
        {
            foreach (PBattlePokemon battler in activeBattlers)
            {
                battler.SelectedMove = PMove.None;
                if (battler.Mon.HP > 0)
                    DoTurnEndedEffects(battler); // BattleEffects.cs
            }
        }
    }
}
