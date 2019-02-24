using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.AI
{
    /// <summary>
    /// Creates valid decisions for a team in a battle.
    /// </summary>
    public static partial class PBEAIManager
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
                throw new InvalidOperationException($"{nameof(team)} must have at least one active battler.");
            }

            var actions = new PBEAction[team.ActionsRequired.Count];
            var standBy = new List<PBEPokemon>();
            for (int i = 0; i < actions.Length; i++)
            {
                PBEPokemon pkmn = team.ActionsRequired[i];
                if (pkmn.Moves.All(m => m == PBEMove.None))
                {
                    throw new ArgumentOutOfRangeException(nameof(pkmn.Moves), $"{pkmn.Shell.Nickname} has no moves.");
                }

                // If a Pokémon is forced to struggle, it is best that it just stays in until it faints
                if (pkmn.IsForcedToStruggle())
                {
                    actions[i].Decision = PBEDecision.Fight;
                    actions[i].FightMove = PBEMove.Struggle;
                    actions[i].FightTargets = DecideTargets(pkmn, PBEMove.Struggle);
                }
                // If a Pokémon has a temp locked move (Dig, Dive, Shadow Force) it must be used
                else if (pkmn.TempLockedMove != PBEMove.None)
                {
                    actions[i].Decision = PBEDecision.Fight;
                    actions[i].FightMove = pkmn.TempLockedMove;
                    actions[i].FightTargets = pkmn.TempLockedTargets;
                }
                // The Pokémon is free to switch or fight (unless it cannot switch due to Magnet Pull etc)
                else
                {
                    // Gather all options of switching and moves
                    PBEPokemon[] availableForSwitch = team.Party.Except(standBy).Except(active).Where(p => p.HP > 0).ToArray();
                    var movesToChooseFrom = new List<PBEMove>();
                    if (pkmn.ChoiceLockedMove != PBEMove.None)
                    {
                        movesToChooseFrom.Add(pkmn.ChoiceLockedMove);
                    }
                    else
                    {
                        for (int m = 0; m < pkmn.Moves.Length; m++)
                        {
                            if (pkmn.PP[m] > 0)
                            {
                                movesToChooseFrom.Add(pkmn.Moves[m]);
                            }
                        }
                    }

                    var possibleActions = new List<Tuple<PBEAction, int>>(); // Associate specific actions with a score
                    for (int m = 0; m < movesToChooseFrom.Count; m++) // Score moves
                    {
                        PBEMove move = movesToChooseFrom[m];
                        var mAction = new PBEAction
                        {
                            Decision = PBEDecision.Fight,
                            FightMove = move,
                            FightTargets = DecideTargets(pkmn, move)
                        };
                        possibleActions.Add(Tuple.Create(mAction, PBEUtils.RNG.Next()));
                    }
                    for (int s = 0; s < availableForSwitch.Length; s++) // Score switches
                    {
                        PBEPokemon switchPkmn = availableForSwitch[s];
                        var sAction = new PBEAction
                        {
                            Decision = PBEDecision.SwitchOut,
                            SwitchPokemonId = switchPkmn.Id
                        };
                        possibleActions.Add(Tuple.Create(sAction, PBEUtils.RNG.Next()));
                    }

                    string ToDebugString(Tuple<PBEAction, int> a)
                    {
                        string str;
                        if (a.Item1.Decision == PBEDecision.Fight)
                        {
                            str = string.Format("Fight {0} {1}", a.Item1.FightMove, a.Item1.FightTargets);
                        }
                        else
                        {
                            str = string.Format("Switch {0}", team.TryGetPokemon(a.Item1.SwitchPokemonId).Shell.Nickname);
                        }
                        str += " [" + a.Item2 + "]";
                        return str;
                    }
                    Debug.WriteLine("{0}'s possible actions: {1}", pkmn.Shell.Nickname, possibleActions.Select(a => ToDebugString(a)).Print());
                    actions[i] = possibleActions.OrderByDescending(a => a.Item2).First().Item1;
                }

                // Action was chosen, finish up for this Pokémon
                actions[i].PokemonId = pkmn.Id;
                if (actions[i].Decision == PBEDecision.SwitchOut)
                {
                    standBy.Add(team.TryGetPokemon(actions[i].SwitchPokemonId));
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
