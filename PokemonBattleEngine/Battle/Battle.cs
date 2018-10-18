using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBattle
    {
        private class PBattlePokemon
        {
            public readonly PPokemon Mon;
            public readonly PTeam Team;

            public PMove PreviousMove, SelectedMove;
            public PTarget SelectedTarget;

            public PBattlePokemon(PPokemon pkmn, PTeam team)
            {
                Mon = pkmn;
                Team = team;
            }
        }
        private class PTeam
        {
            public readonly PBattlePokemon[] Party;

            public PBattlePokemon CurPokemon;
            public bool MonFaintedLastTurn; // Retaliate

            public PTeam(PTeamShell data)
            {
                int min = Math.Min(data.Party.Count, PConstants.MaxPokemon);
                Party = new PBattlePokemon[min];
                for (int i = 0; i < min; i++)
                {
                    var pkmn = new PPokemon(Guid.Empty, data.Party[i]);
                    Party[i] = new PBattlePokemon(pkmn, this);
                }
                CurPokemon = Party[0];
            }
        }

        // TODO: Set this length to be 2 * pkmnPerTeam
        PBattlePokemon[] battlers;
        byte[] turnOrder;
        PTeam[] teams = new PTeam[2];

        public PBattle(PTeamShell td0, PTeamShell td1)
        {
            PKnownInfo.Instance.Clear();

            teams[0] = new PTeam(td0);
            PKnownInfo.Instance.LocalDisplayName = td0.DisplayName;
            // Team 0 pokemon get (LocallyOwned = true) here
            PKnownInfo.Instance.SetPartyPokemon(teams[0].Party.Select(p => p.Mon), true);

            teams[1] = new PTeam(td1);
            PKnownInfo.Instance.RemoteDisplayName = td1.DisplayName;
            // Team 1 pokemon get (LocallyOwned = false) here
            PKnownInfo.Instance.SetPartyPokemon(teams[1].Party.Select(p => p.Mon), false);

            // TODO: Needs work because two teams with 2 pokemon each in a single battle will inflate this
            battlers = teams[0].Party.Concat(teams[1].Party).ToArray();
            turnOrder = new byte[battlers.Length];
        }
        public void Start()
        {
            // TODO:
            // Properly switch in (take into account double battles for example)
            // Set CurPokemon here
            // Temporary:
            foreach (PBattlePokemon battler in battlers)
                PrintSwitchIn(battler.Mon);
        }

        public void SelectMove(int team, int pkmn, int move, PTarget target)
        {
            teams[team].Party[pkmn].SelectedMove = teams[team].Party[pkmn].Mon.Shell.Moves[move];
            teams[team].Party[pkmn].SelectedTarget = target; // TODO: Validate

            if (AllMonSelectedMoves())
            {
                DetermineTurnOrder();
                ClearTemporaryStuff();
                RunMovesInOrder();
                TurnEnded();
            }
        }

        bool AllMonSelectedMoves()
        {
            for (int i = 0; i < battlers.Length; i++)
                if (battlers[i].SelectedMove == PMove.None)
                    return false;
            return true;
        }
        void DetermineTurnOrder()
        {
            // TODO: Turn order
            turnOrder[0] = 0;
            turnOrder[1] = 1;
        }
        void ClearTemporaryStuff()
        {
            foreach (PBattlePokemon battler in battlers)
            {
                battler.Mon.Status2 &= PStatus2.Flinching;
            }
        }
        void RunMovesInOrder()
        {
            for (int i = 0; i < turnOrder.Length; i++)
            {
                PBattlePokemon pkmn = battlers[turnOrder[i]];
                if (pkmn.Mon.HP < 1)
                    continue;
                UseMove(pkmn);
                pkmn.PreviousMove = efCurMove;
            }
        }
        void TurnEnded()
        {
            foreach (PBattlePokemon battler in battlers)
            {
                battler.SelectedMove = PMove.None;
                DoTurnEndedEffects(battler);
            }
        }
    }
}
