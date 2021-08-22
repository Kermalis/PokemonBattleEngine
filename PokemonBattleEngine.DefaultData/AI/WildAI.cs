using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.DefaultData.AI
{
    // Wild Pokémon always select a random usable move (unless they are forced to use a move)
    // They will flee randomly based on their IPBEPokemonData.FleeRate only if it's a single battle and they are allowed to flee
    public sealed class PBEDDWildAI
    {
        public PBETrainer Trainer { get; }

        public PBEDDWildAI(PBETrainer trainer)
        {
            Trainer = trainer;
        }

        public void CreateActions(bool allowFlee)
        {
            if (Trainer.Battle.BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(Trainer.Battle.BattleState)} must be {PBEBattleState.WaitingForActions} to create actions.");
            }
            // Try to flee if it's a single wild battle and the Pokémon is a runner
            if (allowFlee && Trainer.IsWild && Trainer.Battle.BattleFormat == PBEBattleFormat.Single && Trainer.IsFleeValid(out _))
            {
                PBEBattlePokemon user = Trainer.ActionsRequired[0];
                IPBEPokemonData pData = PBEDataProvider.Instance.GetPokemonData(user);
                if (PBEDataProvider.GlobalRandom.RandomBool(pData.FleeRate, byte.MaxValue))
                {
                    if (!Trainer.SelectFleeIfValid(out string? valid))
                    {
                        throw new Exception("Wild AI tried to flee but couldn't. - " + valid);
                    }
                    return;
                }
            }
            // Select a move
            var actions = new PBETurnAction[Trainer.ActionsRequired.Count];
            for (int i = 0; i < actions.Length; i++)
            {
                PBEBattlePokemon user = Trainer.ActionsRequired[i];
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
            if (!Trainer.SelectActionsIfValid(out string? valid2, actions))
            {
                throw new Exception("Wild AI created bad actions. - " + valid2);
            }
        }
    }
}
