using PokemonBattleEngine.Data;
using PokemonBattleEngine.Util;
using System;

namespace PokemonBattleEngine
{
    class TeamData
    {
        public readonly Pokemon[] Pokemon = new Pokemon[Constants.MaxPokemon];
        public string PlayerName;
    }

    class Battle
    {
        private class Team
        {
            public readonly TeamData Data;

            public Pokemon CurrentMon;
            public Move PreviousMove, SelectedMove;

            public Team(TeamData data)
            {
                Data = data;
                CurrentMon = Data.Pokemon[0];
            }
        }
        private enum BattleStatus
        {
            WaitingForMoves,
            Processing,
            Ended
        }

        BattleStatus status;
        readonly Team[] teams = new Team[2];

        public Battle(TeamData td1, TeamData td2)
        {
            teams[0] = new Team(td1);
            teams[1] = new Team(td2);
            status = BattleStatus.WaitingForMoves;
        }

        public void SelectMove(int team, int move)
        {
            if (status == BattleStatus.WaitingForMoves)
            {
                teams[team].SelectedMove = teams[team].CurrentMon.Moves[move];

                if (teams[0].SelectedMove != Move.None && teams[1].SelectedMove != Move.None)
                {
                    //DetermineTurnOrder();
                    int damageTo0 = CalculateDamage(teams[1].CurrentMon, teams[0].CurrentMon, teams[1].SelectedMove);
                    teams[0].CurrentMon.HP = Math.Max(0, teams[0].CurrentMon.HP - damageTo0);
                    int damageTo1 = CalculateDamage(teams[0].CurrentMon, teams[1].CurrentMon, teams[0].SelectedMove);
                    teams[1].CurrentMon.HP = Math.Max(0, teams[1].CurrentMon.HP - damageTo1);
                }
            }
        }

        void DetermineTurnOrder()
        {
            status = BattleStatus.Processing;

            int teamToGoFirst = -1;

            // Check priority moves
            if (teamToGoFirst == -1)
            {
                int t0MovePrio = MoveData.Data[teams[0].SelectedMove].Priority;
                int t1MovePrio = MoveData.Data[teams[1].SelectedMove].Priority;

                if (t0MovePrio > t1MovePrio)
                    teamToGoFirst = 0;
                else if (t1MovePrio > t0MovePrio)
                    teamToGoFirst = 1;
            }

            // Speed tie
            if (teamToGoFirst == -1)
            {
                teamToGoFirst = Utils.RNG.Next(0, 2); // MaxValue of .Next is exclusive, so it can only pick 0 and 1
            }
        }

        int CalculateDamage(Pokemon attacker, Pokemon defender, Move move)
        {
            int damage = 0;

            int attack = attacker.Attack, spAttack = attacker.SpAttack;
            int defense = defender.Defense, spDefense = defender.SpDefense;
            Data.Type type = MoveData.Data[move].Type;

            if (attacker.Ability == Ability.HugePower || attacker.Ability == Ability.PurePower)
                attack *= 2;

            return damage + 2;
        }
    }
}
