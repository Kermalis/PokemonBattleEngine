using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.AI
{
    /// <summary>
    /// Creates valid decisions for a team in a battle.
    /// </summary>
    public static partial class AIManager
    {
        /// <summary>
        /// Creates valid actions for a battle turn for a specific team.
        /// </summary>
        /// <param name="battle">The battle to look at.</param>
        /// <param name="localTeam">The team to create actions for.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the battle format is invalid.</exception>
        public static IEnumerable<PBEAction> CreateActions(PBEBattle battle, bool localTeam)
        {
            PBETeam team = battle.Teams[localTeam ? 0 : 1];
            PBEPokemon[] active = team.ActiveBattlers;
            var actions = new PBEAction[active.Length];
            PBEPokemon pkmn;
            int move;
            for (int i = 0; i < active.Length; i++)
            {
                pkmn = active[i];
                do
                {
                    move = PBEUtils.RNG.Next(0, pkmn.Moves.Length);
                } while (pkmn.Moves[move] == PBEMove.None);
                actions[i].PokemonId = pkmn.Id;
                actions[i].Decision = PBEDecision.Fight;
                actions[i].FightMove = pkmn.Moves[move];
                actions[i].FightTargets = DecideTargets(battle, pkmn, pkmn.Moves[move]);
            }
            return actions;
        }

        /// <summary>
        /// Creates valid switches for a battle for a specific team.
        /// </summary>
        /// <param name="battle">The battle to look at.</param>
        /// <param name="localTeam">The team to create switches for.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the battle format is invalid.</exception>
        public static IEnumerable<Tuple<byte, PBEFieldPosition>> CreateSwitches(PBEBattle battle, bool localTeam)
        {
            PBETeam team = battle.Teams[localTeam ? 0 : 1];
            var switches = new List<Tuple<byte, PBEFieldPosition>>(team.SwitchInsRequired);
            PBEPokemon[] available = team.Party.Where(p => p.FieldPosition == PBEFieldPosition.None && p.HP > 0).ToArray();
            available.Shuffle();
            var availablePositions = new List<PBEFieldPosition>();
            switch (battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                    availablePositions.Add(PBEFieldPosition.Center);
                    break;
                case PBEBattleFormat.Double:
                    if (team.PokemonAtPosition(PBEFieldPosition.Left) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Left);
                    }
                    if (team.PokemonAtPosition(PBEFieldPosition.Right) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Right);
                    }
                    break;
                case PBEBattleFormat.Triple:
                case PBEBattleFormat.Rotation:
                    if (team.PokemonAtPosition(PBEFieldPosition.Left) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Left);
                    }
                    if (team.PokemonAtPosition(PBEFieldPosition.Center) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Center);
                    }
                    if (team.PokemonAtPosition(PBEFieldPosition.Right) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Right);
                    }
                    break;
            }
            for (int i = 0; i < team.SwitchInsRequired; i++)
            {
                switches.Add(Tuple.Create(available[i].Id, availablePositions[i]));
            }
            return switches;
        }
    }
}
