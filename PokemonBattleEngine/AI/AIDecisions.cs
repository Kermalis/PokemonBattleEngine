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
        /// <param name="team">The team to create actions for.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="team"/> has no active battlers or <paramref name="team"/>'s <see cref="PBETeam.Battle"/>'s <see cref="PBEBattle.BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a Pokémon has no moves, the AI tries to use a move with invalid targets, or <paramref name="team"/>'s <see cref="PBETeam.Battle"/>'s <see cref="PBEBattle.BattleFormat"/> is invalid.</exception>
        public static IEnumerable<PBEAction> CreateActions(PBETeam team)
        {
            if (team.Battle.BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(team.Battle.BattleState)} must be {PBEBattleState.WaitingForActions} to create actions.");
            }
            PBEPokemon[] active = team.ActiveBattlers.ToArray();
            if (active.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(team)} must have active battlers.");
            }
            var actions = new PBEAction[active.Length];
            for (int i = 0; i < active.Length; i++)
            {
                PBEPokemon pkmn = active[i];
                if (pkmn.Moves.All(m => m == PBEMove.None))
                {
                    throw new ArgumentOutOfRangeException(nameof(pkmn.Moves), $"{pkmn.Shell.Nickname} has no moves.");
                }
                actions[i].PokemonId = pkmn.Id;
                actions[i].Decision = PBEDecision.Fight;
                if (pkmn.IsForcedToStruggle())
                {
                    actions[i].FightMove = PBEMove.Struggle;
                }
                else if (pkmn.ChoiceLockedMove != PBEMove.None)
                {
                    actions[i].FightMove = pkmn.ChoiceLockedMove;
                }
                else if (pkmn.TempLockedMove != PBEMove.None)
                {
                    actions[i].FightMove = pkmn.TempLockedMove;
                }
                else
                {
                    int index;
                    do
                    {
                        index = PBEUtils.RNG.Next(0, pkmn.Moves.Length);
                        actions[i].FightMove = pkmn.Moves[index];
                    } while (pkmn.PP[index] == 0); // PP of PBEMove.None is always 0 as well
                }
                if (pkmn.TempLockedTargets != PBETarget.None)
                {
                    actions[i].FightTargets = pkmn.TempLockedTargets;
                }
                else
                {
                    actions[i].FightTargets = DecideTargets(pkmn, actions[i].FightMove);
                }
            }
            return actions;
        }

        /// <summary>
        /// Creates valid switches for a battle for a specific team.
        /// </summary>
        /// <param name="team">The team to create switches for.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="team"/> does not require switch-ins or <paramref name="team"/>'s <see cref="PBETeam.Battle"/>'s <see cref="PBEBattle.BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="team"/>'s <see cref="PBETeam.Battle"/>'s <see cref="PBEBattle.BattleFormat"/> is invalid.</exception>
        public static IEnumerable<Tuple<byte, PBEFieldPosition>> CreateSwitches(PBETeam team)
        {
            if (team.Battle.BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(team.Battle.BattleState)} must be {PBEBattleState.WaitingForSwitchIns} to create switch-ins.");
            }
            if (team.SwitchInsRequired == 0)
            {
                throw new InvalidOperationException($"{nameof(team)} must require switch-ins.");
            }
            var switches = new List<Tuple<byte, PBEFieldPosition>>(team.SwitchInsRequired);
            PBEPokemon[] available = team.Party.Where(p => p.FieldPosition == PBEFieldPosition.None && p.HP > 0).ToArray();
            available.Shuffle();
            var availablePositions = new List<PBEFieldPosition>();
            switch (team.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                    {
                        availablePositions.Add(PBEFieldPosition.Center);
                        break;
                    }
                case PBEBattleFormat.Double:
                    {
                        if (team.TryGetPokemon(PBEFieldPosition.Left) == null)
                        {
                            availablePositions.Add(PBEFieldPosition.Left);
                        }
                        if (team.TryGetPokemon(PBEFieldPosition.Right) == null)
                        {
                            availablePositions.Add(PBEFieldPosition.Right);
                        }
                        break;
                    }
                case PBEBattleFormat.Triple:
                case PBEBattleFormat.Rotation:
                    {
                        if (team.TryGetPokemon(PBEFieldPosition.Left) == null)
                        {
                            availablePositions.Add(PBEFieldPosition.Left);
                        }
                        if (team.TryGetPokemon(PBEFieldPosition.Center) == null)
                        {
                            availablePositions.Add(PBEFieldPosition.Center);
                        }
                        if (team.TryGetPokemon(PBEFieldPosition.Right) == null)
                        {
                            availablePositions.Add(PBEFieldPosition.Right);
                        }
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(team.Battle.BattleFormat));
            }
            for (int i = 0; i < team.SwitchInsRequired; i++)
            {
                switches.Add(Tuple.Create(available[i].Id, availablePositions[i]));
            }
            return switches;
        }
    }
}
