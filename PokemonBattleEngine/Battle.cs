using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine
{
    public sealed class TeamData
    {
        public Pokemon[] Pokemon;
        public string PlayerName;
    }

    public sealed partial class Battle
    {
        private class BattlePokemon
        {
            public readonly Pokemon Mon;
            public readonly Team Team;

            public Move PreviousMove, SelectedMove;
            public Target SelectedTarget;

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

            public Team(TeamData data)
            {
                PlayerName = data.PlayerName;
                int min = Math.Min(data.Pokemon.Length, Constants.MaxPokemon);
                Pokemon = new BattlePokemon[min];
                for (int i = 0; i < min; i++)
                    Pokemon[i] = new BattlePokemon(data.Pokemon[i], this);
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

        public Battle(TeamData td0, TeamData td1)
        {
            battlers = new BattlePokemon[td0.Pokemon.Length + td1.Pokemon.Length];
            turnOrder = new byte[battlers.Length];
            teams[0] = new Team(td0);
            for (int i = 0; i < teams[0].Pokemon.Length; i++)
                battlers[i] = teams[0].Pokemon[i];
            teams[1] = new Team(td1);
            for (int i = teams[0].Pokemon.Length; i < battlers.Length; i++)
                battlers[i] = teams[1].Pokemon[i - teams[0].Pokemon.Length];
            status = BattleStatus.WaitingForMoves;
        }

        bool AllMonSelectedMoves()
        {
            for (int i = 0; i < battlers.Length; i++)
                if (battlers[i].SelectedMove == Move.None)
                    return false;
            return true;
        }
        public void SelectMove(int team, int pkmn, int move, Target target)
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
                int damage = CalculateDamage(attacker, defender, attacker.SelectedMove);
                DealDamage(defender, damage);
            }
        }
        void DealDamage(BattlePokemon victim, int damage)
        {
            victim.Mon.HP = Math.Max(0, victim.Mon.HP - damage);
        }
    }
}
