using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using System;

namespace Kermalis.PokemonBattleEngine.AI
{
    public static partial class PBEAI
    {
        // Wild Pokémon always select a random usable move (unless they are forced to use a move)
        // They will flee randomly based on their PBEPokemonData.FleeRate only if it's a single battle and they are allowed to flee
        public static void CreateWildAIActions(this PBETrainer trainer)
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
            if (trainer.IsWild && trainer.Battle.BattleFormat == PBEBattleFormat.Single && trainer.IsFleeValid() is null)
            {
                PBEBattlePokemon user = trainer.ActionsRequired[0];
                var pData = PBEPokemonData.GetData(user.Species, user.Form);
                if (PBEUtils.GlobalRandom.RandomBool(pData.FleeRate, 255))
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
                // The Pokémon is free to fight
                PBEMove[] usableMoves = user.GetUsableMoves();
                PBEMove move = PBEUtils.GlobalRandom.RandomElement(usableMoves);
                actions[i] = new PBETurnAction(user, move, PBEBattle.GetRandomTargetForMetronome(user, move, PBEUtils.GlobalRandom));
            }
            string valid2 = trainer.SelectActionsIfValid(actions);
            if (valid2 != null)
            {
                throw new Exception("Wild AI created bad actions. - " + valid2);
            }
        }
    }
}
