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
            MoveData mData = MoveData.Data[move];
            int damage = 0;
            int movePower = mData.Power;

            int attack = attacker.Attack, spAttack = attacker.SpAttack;
            int defense = defender.Defense, spDefense = defender.SpDefense;
            Data.Type type = mData.Type;

            // 2x attack boost
            if ((attacker.Ability == Ability.HugePower || attacker.Ability == Ability.PurePower)
                //|| (attacker.Item == Item.ThickClub && (attacker.Species == Species.Cubone || attacker.Species == Species.Marowak))
                )
                attack *= 2;

            // 50% attack boost
            if (attacker.Item == Item.ChoiceBand
                || attacker.Ability == Ability.Hustle
                || (attacker.Ability == Ability.Guts && attacker.Status != Status.None)
                )
                attack = 150 * attack / 100;

            // 2x sp attack boost
            if (false//(attacker.Item == Item.DeepSeaTooth && attacker.Species == Species.Clamperl)
                )
                spAttack *= 2;

            // 50% sp attack boost
            if ((attacker.Item == Item.SoulDew && (attacker.Species == Species.Latios || attacker.Species == Species.Latias))
                //|| (attacker.Ability == Ability.Plus && PartnerHasAbility(Ability.Minus))
                //|| (attacker.Ability == Ability.Minus && PartnerHasAbility(Ability.Plus))
                )
                spAttack = 150 * spAttack / 100;

            // 2x defense boost
            if (false//(defender.Item == Item.MetalPowder && defender.Species == Species.Ditto)
                )
                defense *= 2;

            // 50% defense boost
            if ((defender.Ability == Ability.MarvelScale && defender.Status != Status.None)
                )
                defense = 150 * defense / 100;

            // 2x sp defense boost
            if (false//(defender.Item == Item.DeepSeaScale && defender.Species == Species.Clamperl)
                )
                spDefense *= 2;

            // 50% sp defense boost
            if ((defender.Item == Item.SoulDew && (defender.Species == Species.Latios || defender.Species == Species.Latias))
                )
                spDefense = 150 * spDefense / 100;

            // 2x attack & sp attack boost
            if (false//(attacker.Item == Item.LightBall && attacker.Species == Species.Pikachu)
                )
            {
                attack *= 2;
                spAttack *= 2;
            }

            // 2x attack & sp attack impairment
            if ((defender.Ability == Ability.ThickFat && (type == Data.Type.Fire || type == Data.Type.Ice))
                )
            {
                attack /= 2;
                spAttack /= 2;
            }

            // 50% power boost
            if (attacker.HP <= attacker.MaxHP / 3
                && ((type == Data.Type.Grass && attacker.Ability == Ability.Overgrow)
                || (type == Data.Type.Fire && attacker.Ability == Ability.Blaze)
                || (type == Data.Type.Water && attacker.Ability == Ability.Torrent)
                || (type == Data.Type.Bug && attacker.Ability == Ability.Swarm))
                )
                movePower = 150 * movePower / 100;

            // 2x power impairment
            if (false//(type == Data.Type.Electric && MudSportActive())
                //|| (type == Data.Type.Fire && WaterSportActive())
                )
                movePower /= 2;

            if (mData.Category == MoveCategory.Physical)
            {
                damage = attack;
            }
            else if (mData.Category == MoveCategory.Special)
            {
                damage = spAttack;
            }

            return damage + 2;
        }
    }
}
