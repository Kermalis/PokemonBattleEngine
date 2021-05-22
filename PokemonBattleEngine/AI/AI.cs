using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.AI
{
    /// <summary>Creates valid decisions for a team in a battle. Decisions may not be valid for custom settings and/or move changes.</summary>
    public static partial class PBEAI
    {
        // TODO: Control multiple trainers of a multi battle team

        /// <summary>Creates valid actions for a battle turn for a specific team.</summary>
        /// <param name="trainer">The trainer to create actions for.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="trainer"/> has no active battlers or <paramref name="trainer"/>'s <see cref="PBETrainer.Battle"/>'s <see cref="PBEBattle.BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a Pokémon has no moves, the AI tries to use a move with invalid targets, or <paramref name="trainer"/>'s <see cref="PBETrainer.Battle"/>'s <see cref="PBEBattle.BattleFormat"/> is invalid.</exception>
        public static void CreateAIActions(this PBETrainer trainer)
        {
            if (trainer is null)
            {
                throw new ArgumentNullException(nameof(trainer));
            }
            if (trainer.Battle.BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(trainer.Battle.BattleState)} must be {PBEBattleState.WaitingForActions} to create actions.");
            }
            if (trainer.IsWild)
            {
                trainer.CreateWildAIActions(false);
                return;
            }
            var actions = new PBETurnAction[trainer.ActionsRequired.Count];
            var standBy = new List<PBEBattlePokemon>();
            for (int i = 0; i < actions.Length; i++)
            {
                PBEBattlePokemon user = trainer.ActionsRequired[i];
                // If a Pokémon is forced to struggle, it is best that it just stays in until it faints
                if (user.IsForcedToStruggle())
                {
                    actions[i] = new PBETurnAction(user, PBEMove.Struggle, PBEBattleUtils.GetPossibleTargets(user, user.GetMoveTargets(PBEMove.Struggle))[0]);
                    continue;
                }
                // If a Pokémon has a temp locked move (Dig, Dive, ShadowForce) it must be used
                else if (user.TempLockedMove != PBEMove.None)
                {
                    actions[i] = new PBETurnAction(user, user.TempLockedMove, user.TempLockedTargets);
                    continue;
                }
                // The Pokémon is free to switch or fight (unless it cannot switch due to Magnet Pull etc)
                actions[i] = DecideAction(trainer, user, actions, standBy);
                // Action was chosen, finish up for this Pokémon
                if (actions[i].Decision == PBETurnDecision.SwitchOut)
                {
                    standBy.Add(trainer.TryGetPokemon(actions[i].SwitchPokemonId));
                }
            }
            string valid = trainer.SelectActionsIfValid(actions);
            if (valid != null)
            {
                throw new Exception("AI created bad actions. - " + valid);
            }
        }

        // Wild Pokémon always select a random usable move (unless they are forced to use a move)
        // They will flee randomly based on their PBEPokemonData.FleeRate only if it's a single battle and they are allowed to flee
        public static void CreateWildAIActions(this PBETrainer trainer, bool allowFlee)
        {
            if (trainer is null)
            {
                throw new ArgumentNullException(nameof(trainer));
            }
            if (trainer.Battle.BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(trainer.Battle.BattleState)} must be {PBEBattleState.WaitingForActions} to create actions.");
            }
            // Try to flee if it's a single wild battle and the Pokémon is a runner
            if (allowFlee && trainer.IsWild && trainer.Battle.BattleFormat == PBEBattleFormat.Single && trainer.IsFleeValid() is null)
            {
                PBEBattlePokemon user = trainer.ActionsRequired[0];
                IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(user);
                if (PBEDataProvider.GlobalRandom.RandomBool(pData.FleeRate, 255))
                {
                    string valid = trainer.SelectFleeIfValid();
                    if (valid != null)
                    {
                        throw new Exception("Wild AI tried to flee but couldn't. - " + valid);
                    }
                    return;
                }
            }
            var actions = new PBETurnAction[trainer.ActionsRequired.Count];
            for (int i = 0; i < actions.Length; i++)
            {
                PBEBattlePokemon user = trainer.ActionsRequired[i];
                // If a Pokémon is forced to struggle, it must use Struggle
                if (user.IsForcedToStruggle())
                {
                    actions[i] = new PBETurnAction(user, PBEMove.Struggle, PBEBattleUtils.GetPossibleTargets(user, user.GetMoveTargets(PBEMove.Struggle))[0]);
                    continue;
                }
                // If a Pokémon has a temp locked move (Dig, Dive, ShadowForce) it must be used
                if (user.TempLockedMove != PBEMove.None)
                {
                    actions[i] = new PBETurnAction(user, user.TempLockedMove, user.TempLockedTargets);
                    continue;
                }
                // The Pokémon is free to fight, so pick a random move
                PBEMove[] usableMoves = user.GetUsableMoves();
                PBEMove move = PBEDataProvider.GlobalRandom.RandomElement(usableMoves);
                actions[i] = new PBETurnAction(user, move, PBEBattle.GetRandomTargetForMetronome(user, move, PBEDataProvider.GlobalRandom));
            }
            string valid2 = trainer.SelectActionsIfValid(actions);
            if (valid2 != null)
            {
                throw new Exception("Wild AI created bad actions. - " + valid2);
            }
        }

        /// <summary>Creates valid switches for a battle for a specific team.</summary>
        /// <param name="trainer">The trainer to create switches for.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="trainer"/> does not require switch-ins or <paramref name="trainer"/>'s <see cref="PBETrainer.Battle"/>'s <see cref="PBEBattle.BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="trainer"/>'s <see cref="PBETrainer.Battle"/>'s <see cref="PBEBattle.BattleFormat"/> is invalid.</exception>
        public static void CreateAISwitches(this PBETrainer trainer)
        {
            if (trainer == null)
            {
                throw new ArgumentNullException(nameof(trainer));
            }
            if (trainer.Battle.BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(trainer.Battle.BattleState)} must be {PBEBattleState.WaitingForSwitchIns} to create switch-ins.");
            }
            if (trainer.SwitchInsRequired == 0)
            {
                throw new InvalidOperationException($"{nameof(trainer)} must require switch-ins.");
            }
            PBEBattlePokemon[] available = trainer.Party.Where(p => p.FieldPosition == PBEFieldPosition.None && p.CanBattle).ToArray();
            PBEDataProvider.GlobalRandom.Shuffle(available);
            var availablePositions = new List<PBEFieldPosition>();
            switch (trainer.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                {
                    availablePositions.Add(PBEFieldPosition.Center);
                    break;
                }
                case PBEBattleFormat.Double:
                {
                    if (trainer.OwnsSpot(PBEFieldPosition.Left) && trainer.TryGetPokemon(PBEFieldPosition.Left) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Left);
                    }
                    if (trainer.OwnsSpot(PBEFieldPosition.Right) && trainer.TryGetPokemon(PBEFieldPosition.Right) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Right);
                    }
                    break;
                }
                case PBEBattleFormat.Triple:
                case PBEBattleFormat.Rotation:
                {
                    if (trainer.OwnsSpot(PBEFieldPosition.Left) && trainer.TryGetPokemon(PBEFieldPosition.Left) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Left);
                    }
                    if (trainer.OwnsSpot(PBEFieldPosition.Center) && trainer.TryGetPokemon(PBEFieldPosition.Center) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Center);
                    }
                    if (trainer.OwnsSpot(PBEFieldPosition.Right) && trainer.TryGetPokemon(PBEFieldPosition.Right) == null)
                    {
                        availablePositions.Add(PBEFieldPosition.Right);
                    }
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(trainer.Battle.BattleFormat));
            }
            var switches = new PBESwitchIn[trainer.SwitchInsRequired];
            for (int i = 0; i < trainer.SwitchInsRequired; i++)
            {
                switches[i] = new PBESwitchIn(available[i], availablePositions[i]);
            }
            string valid = trainer.SelectSwitchesIfValid(switches);
            if (valid != null)
            {
                throw new Exception("AI created bad switches. - " + valid);
            }
        }
    }
}
