using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed class PTeamShell
    {
        public string PlayerName;
        public List<PPokemonShell> Pokemon = new List<PPokemonShell>(PConstants.MaxPokemon);

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();

            byte[] playerNameBytes = Encoding.ASCII.GetBytes(PlayerName);
            bytes.Add((byte)playerNameBytes.Length);
            bytes.AddRange(playerNameBytes);

            var numPkmn = Math.Min(PConstants.MaxPokemon, Pokemon.Count);
            bytes.Add((byte)numPkmn);
            for (int i = 0; i < numPkmn; i++)
                bytes.AddRange(Pokemon[i].ToBytes());

            return bytes.ToArray();
        }
        public static PTeamShell FromBytes(BinaryReader r)
        {
            var team = new PTeamShell { PlayerName = Encoding.ASCII.GetString(r.ReadBytes(r.ReadByte())) };
            var numPkmn = Math.Min(PConstants.MaxPokemon, r.ReadByte());
            for (int i = 0; i < numPkmn; i++)
                team.Pokemon.Add(PPokemonShell.FromBytes(r));
            return team;
        }
    }

    public sealed partial class PBattle
    {
        // TODO: get rid of this
        private class PBattlePokemon
        {
            public readonly PPokemon Pokemon;
            public readonly PTeam Team;

            public PMove PreviousMove, SelectedMove;
            public PTarget SelectedTarget;

            public PBattlePokemon(PPokemon pokemon, PTeam team)
            {
                Pokemon = pokemon;
                Team = team;
            }
        }
        private class PTeam
        {
            public readonly PBattlePokemon[] Pokemon;
            public readonly string PlayerName;

            public PBattlePokemon CurrentMon;
            public bool MonFaintedLastTurn; // Retaliate

            public PTeam(PTeamShell data)
            {
                PlayerName = data.PlayerName;
                int min = Math.Min(data.Pokemon.Count, PConstants.MaxPokemon);
                Pokemon = new PBattlePokemon[min];
                for (int i = 0; i < min; i++)
                    Pokemon[i] = new PBattlePokemon(new PPokemon(data.Pokemon[i]), this);
                CurrentMon = Pokemon[0];
            }
        }

        PBattlePokemon[] battlers;
        byte[] turnOrder;
        readonly PTeam[] teams = new PTeam[2];

        public PBattle(PTeamShell td0, PTeamShell td1)
        {
            teams[0] = new PTeam(td0);
            teams[1] = new PTeam(td1);
            // Needs work because two teams with 2 pokemon each in a single battle will inflate this
            battlers = teams[0].Pokemon.Concat(teams[1].Pokemon).ToArray();
            turnOrder = new byte[battlers.Length];
        }

        // Debugging
        internal PPokemon GetBattler(int index) => battlers[index].Pokemon;

        bool AllMonSelectedMoves()
        {
            for (int i = 0; i < battlers.Length; i++)
                if (battlers[i].SelectedMove == PMove.None)
                    return false;
            return true;
        }
        public void SelectMove(int team, int pkmn, int move, PTarget target)
        {
            teams[team].Pokemon[pkmn].SelectedMove = teams[team].Pokemon[pkmn].Pokemon.Shell.Moves[move];
            teams[team].Pokemon[pkmn].SelectedTarget = target;

            if (AllMonSelectedMoves())
            {
                DetermineTurnOrder();
                DoDamageInOrder();
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
                PBattlePokemon attacker = battlers[turnOrder[i]];
                // Temporarily get the opponent
                PBattlePokemon defender = turnOrder[i] == 0 ? battlers[1] : battlers[0];
                ushort damage = CalculateDamage(attacker, defender, attacker.SelectedMove);
                DealDamage(defender, damage);
            }
        }
        void DealDamage(PBattlePokemon victim, ushort damage)
        {
            victim.Pokemon.HP = (ushort)Math.Max(0, victim.Pokemon.HP - damage);
        }
    }
}
