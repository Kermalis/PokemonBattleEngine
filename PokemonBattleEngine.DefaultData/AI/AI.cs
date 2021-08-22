using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.DefaultData.AI
{
    /// <summary>Creates valid decisions for a team in a battle. Decisions may not be valid for custom settings and/or move changes.</summary>
    public partial class PBEDDAI
    {
        // TODO: Control multiple trainers of a multi battle team
        public PBETrainer Trainer { get; }

        public PBEDDAI(PBETrainer trainer)
        {
            if (trainer.IsWild)
            {
                throw new ArgumentOutOfRangeException(nameof(trainer), $"Cannot use this AI type with a wild trainer. Use {nameof(PBEDDWildAI)} or another type of AI.");
            }
            Trainer = trainer;
        }

        /// <summary>Creates valid actions for a battle turn for a specific team.</summary>
        /// <exception cref="InvalidOperationException">Thrown when <see name="Trainer"/> has no active battlers or <see name="Trainer"/>'s <see cref="PBETrainer.Battle"/>'s <see cref="PBEBattle.BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when a Pokémon has no moves, the AI tries to use a move with invalid targets, or <see name="Trainer"/>'s <see cref="PBETrainer.Battle"/>'s <see cref="PBEBattle.BattleFormat"/> is invalid.</exception>
        public void CreateActions()
        {
            if (Trainer.Battle.BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(Trainer.Battle.BattleState)} must be {PBEBattleState.WaitingForActions} to create actions.");
            }
            int count = Trainer.ActionsRequired.Count;
            var actions = new List<PBETurnAction>(count);
            var standBy = new List<PBEBattlePokemon>();
            for (int i = 0; i < count; i++)
            {
                PBEBattlePokemon user = Trainer.ActionsRequired[i];
                // If a Pokémon is forced to struggle, it is best that it just stays in until it faints
                if (user.IsForcedToStruggle())
                {
                    actions.Add(new PBETurnAction(user, PBEMove.Struggle, PBEBattleUtils.GetPossibleTargets(user, user.GetMoveTargets(PBEMove.Struggle))[0]));
                    continue;
                }
                // If a Pokémon has a temp locked move (Dig, Dive, ShadowForce) it must be used
                else if (user.TempLockedMove != PBEMove.None)
                {
                    actions.Add(new PBETurnAction(user, user.TempLockedMove, user.TempLockedTargets));
                    continue;
                }
                // The Pokémon is free to switch or fight (unless it cannot switch due to Magnet Pull etc)
                PBETurnAction a = DecideAction(user, actions, standBy);
                // Action was chosen, finish up for this Pokémon
                if (a.Decision == PBETurnDecision.SwitchOut)
                {
                    standBy.Add(Trainer.GetPokemon(a.SwitchPokemonId));
                }
                actions.Add(a);
            }
            if (!Trainer.SelectActionsIfValid(actions, out string? valid))
            {
                throw new Exception("AI created bad actions. - " + valid);
            }
        }

        /// <summary>Creates valid switches for a battle for a specific team.</summary>
        /// <exception cref="InvalidOperationException">Thrown when <see name="Trainer"/> does not require switch-ins or <see name="Trainer"/>'s <see cref="PBETrainer.Battle"/>'s <see cref="PBEBattle.BattleState"/> is not <see cref="PBEBattleState.WaitingForActions"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <see name="Trainer"/>'s <see cref="PBETrainer.Battle"/>'s <see cref="PBEBattle.BattleFormat"/> is invalid.</exception>
        public void CreateAISwitches()
        {
            if (Trainer.Battle.BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(Trainer.Battle.BattleState)} must be {PBEBattleState.WaitingForSwitchIns} to create switch-ins.");
            }
            if (Trainer.SwitchInsRequired == 0)
            {
                throw new InvalidOperationException($"{nameof(Trainer)} must require switch-ins.");
            }
            List<PBEBattlePokemon> available = Trainer.Party.FindAll(p => p.FieldPosition == PBEFieldPosition.None && p.CanBattle);
            PBEDataProvider.GlobalRandom.Shuffle(available);
            var availablePositions = new List<PBEFieldPosition>();
            switch (Trainer.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                {
                    availablePositions.Add(PBEFieldPosition.Center);
                    break;
                }
                case PBEBattleFormat.Double:
                {
                    if (Trainer.OwnsSpot(PBEFieldPosition.Left) && !Trainer.IsSpotOccupied(PBEFieldPosition.Left))
                    {
                        availablePositions.Add(PBEFieldPosition.Left);
                    }
                    if (Trainer.OwnsSpot(PBEFieldPosition.Right) && !Trainer.IsSpotOccupied(PBEFieldPosition.Right))
                    {
                        availablePositions.Add(PBEFieldPosition.Right);
                    }
                    break;
                }
                case PBEBattleFormat.Triple:
                case PBEBattleFormat.Rotation:
                {
                    if (Trainer.OwnsSpot(PBEFieldPosition.Left) && !Trainer.IsSpotOccupied(PBEFieldPosition.Left))
                    {
                        availablePositions.Add(PBEFieldPosition.Left);
                    }
                    if (Trainer.OwnsSpot(PBEFieldPosition.Center) && !Trainer.IsSpotOccupied(PBEFieldPosition.Center))
                    {
                        availablePositions.Add(PBEFieldPosition.Center);
                    }
                    if (Trainer.OwnsSpot(PBEFieldPosition.Right) && !Trainer.IsSpotOccupied(PBEFieldPosition.Right))
                    {
                        availablePositions.Add(PBEFieldPosition.Right);
                    }
                    break;
                }
                default: throw new InvalidOperationException(nameof(Trainer.Battle.BattleFormat));
            }
            var switches = new PBESwitchIn[Trainer.SwitchInsRequired];
            for (int i = 0; i < Trainer.SwitchInsRequired; i++)
            {
                switches[i] = new PBESwitchIn(available[i], availablePositions[i]);
            }
            if (!Trainer.SelectSwitchesIfValid(out string? valid, switches))
            {
                throw new Exception("AI created bad switches. - " + valid);
            }
        }
    }
}
