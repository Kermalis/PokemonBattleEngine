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
            PMove move = pkmn.Mon.Shell.Moves[param1];
            if (param1 >= PConstants.NumMoves || move == PMove.None)
                return false;

            // TODO: Struggle

            // Out of PP
            if (pkmn.Mon.PP[param1] < 1)
                return false;

            PMoveData mData = PMoveData.Data[move];
            PTarget targets = (PTarget)param2;
            switch (BattleStyle)
            {
                case PBattleStyle.Single:
                case PBattleStyle.Rotation:
                    // Only center in single battles & rotation battles
                    switch (mData.Targets)
                    {
                        case PMoveTarget.All:
                            if (targets != (PTarget.AllyCenter | PTarget.FoeCenter))
                                return false;
                            break;
                        case PMoveTarget.AllFoes:
                        case PMoveTarget.AllFoesSurrounding:
                        case PMoveTarget.AllSurrounding:
                        case PMoveTarget.SingleFoeSurrounding:
                        case PMoveTarget.SingleNotSelf:
                        case PMoveTarget.SingleSurrounding:
                            if (targets != PTarget.FoeCenter)
                                return false;
                            break;
                        case PMoveTarget.AllTeam:
                        case PMoveTarget.Self:
                        case PMoveTarget.SelfOrAllySurrounding:
                            if (targets != PTarget.AllyCenter)
                                return false;
                            break;
                        case PMoveTarget.RandomFoeSurrounding:
                        case PMoveTarget.SingleAllySurrounding:
                            // TODO: set the target
                            break;
                    }
                    break;
                case PBattleStyle.Double:
                    // No center in double battles
                    switch (mData.Targets)
                    {
                        case PMoveTarget.All:
                            if (targets != (PTarget.AllyLeft | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllFoes:
                        case PMoveTarget.AllFoesSurrounding:
                            if (targets != (PTarget.FoeLeft | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != (PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeRight))
                                    return false;
                            }
                            else
                            {
                                if (targets != (PTarget.AllyLeft | PTarget.FoeLeft | PTarget.FoeRight))
                                    return false;
                            }
                            break;
                        case PMoveTarget.AllTeam:
                            if (targets != (PTarget.AllyLeft | PTarget.AllyRight))
                                return false;
                            break;
                        case PMoveTarget.RandomFoeSurrounding:
                            // TODO: Set the target
                            break;
                        case PMoveTarget.Self:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyLeft)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SelfOrAllySurrounding:
                            if (targets != PTarget.AllyLeft && targets != PTarget.AllyRight)
                                return false;
                            break;
                        case PMoveTarget.SingleAllySurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyLeft)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleFoeSurrounding:
                            if (targets != PTarget.FoeLeft && targets != PTarget.FoeRight)
                                return false;
                            break;
                        case PMoveTarget.SingleNotSelf:
                        case PMoveTarget.SingleSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyRight && targets != PTarget.FoeLeft && targets != PTarget.FoeRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.FoeLeft && targets != PTarget.FoeRight)
                                    return false;
                            }
                            break;
                    }
                    break;
                case PBattleStyle.Triple:
                    switch (mData.Targets)
                    {
                        case PMoveTarget.All:
                            if (targets != (PTarget.AllyLeft | PTarget.AllyCenter | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllFoes:
                            if (targets != (PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllFoesSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != (PTarget.FoeLeft | PTarget.FoeCenter))
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != (PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            else
                            {
                                if (targets != (PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            break;
                        case PMoveTarget.AllSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != (PTarget.AllyCenter | PTarget.FoeLeft | PTarget.FoeCenter))
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != (PTarget.AllyLeft | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            else
                            {
                                if (targets != (PTarget.AllyCenter | PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            break;
                        case PMoveTarget.AllTeam:
                            if (targets != (PTarget.AllyLeft | PTarget.AllyCenter | PTarget.AllyRight))
                                return false;
                            break;
                        case PMoveTarget.RandomFoeSurrounding:
                            // TODO: set the target
                            break;
                        case PMoveTarget.Self:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyLeft)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != PTarget.AllyCenter)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SelfOrAllySurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.AllyCenter)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.AllyCenter && targets != PTarget.AllyRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyCenter && targets != PTarget.AllyRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleAllySurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyCenter)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.AllyRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyCenter)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleFoeSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.FoeLeft && targets != PTarget.FoeCenter)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != PTarget.FoeLeft && targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleNotSelf:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyCenter && targets != PTarget.AllyRight && targets != PTarget.FoeLeft && targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.AllyRight && targets != PTarget.FoeLeft && targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.AllyCenter && targets != PTarget.FoeLeft && targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyCenter && targets != PTarget.FoeLeft && targets != PTarget.FoeCenter)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.AllyRight && targets != PTarget.FoeLeft && targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyCenter && targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            break;
                    }
                    break;
            }

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
