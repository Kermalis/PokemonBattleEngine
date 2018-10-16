using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine
{
    public sealed class TeamShell
    {
        public List<PokemonShell> Pokemon = new List<PokemonShell>();
        public string PlayerName;
    }

    public sealed partial class Battle
    {
        private class BattlePokemon
        {
            public readonly Pokemon Mon;
            public readonly Team Team;

            public PMove PreviousMove, SelectedMove;
            public PTarget SelectedTarget;

            public BattlePokemon(Pokemon mon, Team team)
            {
                Mon = mon;
                Team = team;
            }
        }
        private class Team
        {
            public readonly BattlePokemon[] Pokemon;
            public readonly string PlayerName;

            public BattlePokemon CurrentMon;
            public bool MonFaintedLastTurn; // Retaliate

            public Team(TeamShell data)
            {
                PlayerName = data.PlayerName;
                int min = Math.Min(data.Pokemon.Count, Constants.MaxPokemon);
                Pokemon = new BattlePokemon[min];
                for (int i = 0; i < min; i++)
                    Pokemon[i] = new BattlePokemon(new Pokemon(data.Pokemon[i]), this);
                CurrentMon = Pokemon[0];
            }
        }
        private enum BattleStatus
        {
            WaitingForMoves,
            Processing,
            Ended
        }

        BattlePokemon[] battlers;
        byte[] turnOrder;
        BattleStatus status;
        readonly Team[] teams = new Team[2];

        public Battle(TeamShell td0, TeamShell td1)
        {
            teams[0] = new Team(td0);
            teams[1] = new Team(td1);
            // Needs work because two teams with 2 pokemon each in a single battle will inflate this
            battlers = teams[0].Pokemon.Concat(teams[1].Pokemon).ToArray();
            turnOrder = new byte[battlers.Length];
            status = BattleStatus.WaitingForMoves;
        }

        // Debugging
        internal Pokemon GetBattler(int index) => battlers[index].Mon;

        bool AllMonSelectedMoves()
        {
            for (int i = 0; i < battlers.Length; i++)
                if (battlers[i].SelectedMove == PMove.None)
                    return false;
            return true;
        }
        public void SelectMove(int team, int pkmn, int move, PTarget target)
        {
            if (status == BattleStatus.WaitingForMoves)
            {
                teams[team].Pokemon[pkmn].SelectedMove = teams[team].Pokemon[pkmn].Mon.Moves[move];
                teams[team].Pokemon[pkmn].SelectedTarget = target;
                
                if (AllMonSelectedMoves())
                {
                    status = BattleStatus.Processing;

                    DetermineTurnOrder();
                    DoDamageInOrder();

                    status = BattleStatus.WaitingForMoves;
                }
            }
        }

        void DetermineTurnOrder()
        {
            // TODO: Turn order
            turnOrder[0] = 0;
            turnOrder[1] = 1;
        }
        void DoDamageInOrder()
        {
            for (int i = 0; i < turnOrder.Length; i++)
            {
                // TODO: Targets
                BattlePokemon attacker = battlers[turnOrder[i]];
                // Temporarily get the opponent
                BattlePokemon defender = turnOrder[i] == 0 ? battlers[1] : battlers[0];
                ushort damage = CalculateDamage(attacker, defender, attacker.SelectedMove);
                DealDamage(defender, damage);
            }
        }
        void DealDamage(BattlePokemon victim, ushort damage)
        {
            victim.Mon.HP = (ushort)Math.Max(0, victim.Mon.HP - damage);
        }
    }
}
