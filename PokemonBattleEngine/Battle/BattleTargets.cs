using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        /// <summary>
        /// Gets a move's possible targets for a specific Pokémon.
        /// </summary>
        /// <param name="pkmn">The Pokémon using the move.</param>
        /// <param name="move">The move the Pokémon will use.</param>
        public static PBEMoveTarget GetMoveTargetsForPokemon(PBEPokemon pkmn, PBEMove move)
        {
            switch (move)
            {
                case PBEMove.Curse:
                    if (pkmn.HasType(PBEType.Ghost))
                    {
                        return PBEMoveTarget.SingleSurrounding;
                    }
                    else
                    {
                        return PBEMoveTarget.Self;
                    }
                case PBEMove.None: throw new ArgumentOutOfRangeException(nameof(move), $"Invalid move: {move}");
                default: return PBEMoveData.Data[move].Targets;
            }
        }

        /// <summary>
        /// Gets the position across from the inputted position for a specific battle format.
        /// </summary>
        /// <param name="battleFormat">The battle format.</param>
        /// <param name="position">The position.</param>
        public static PBEFieldPosition GetPositionAcross(PBEBattleFormat battleFormat, PBEFieldPosition position)
        {
            switch (battleFormat)
            {
                case PBEBattleFormat.Single:
                case PBEBattleFormat.Rotation:
                    if (position == PBEFieldPosition.Center)
                    {
                        return PBEFieldPosition.Center;
                    }
                    break;
                case PBEBattleFormat.Double:
                    if (position == PBEFieldPosition.Left)
                    {
                        return PBEFieldPosition.Right;
                    }
                    else if (position == PBEFieldPosition.Right)
                    {
                        return PBEFieldPosition.Left;
                    }
                    break;
                case PBEBattleFormat.Triple:
                    if (position == PBEFieldPosition.Left)
                    {
                        return PBEFieldPosition.Right;
                    }
                    else if (position == PBEFieldPosition.Center)
                    {
                        return PBEFieldPosition.Center;
                    }
                    else if (position == PBEFieldPosition.Right)
                    {
                        return PBEFieldPosition.Left;
                    }
                    break;
            }
            return PBEFieldPosition.None;
        }

        /// <summary>
        /// Gets all Pokémon that will be hit.
        /// </summary>
        /// <param name="user">The Pokémon that will act.</param>
        /// <param name="requestedTargets">The targets the Pokémon wishes to hit.</param>
        /// <param name="canHitFarCorners">Whether the move can hit far Pokémon in a triple battle.</param>
        PBEPokemon[] GetRuntimeTargets(PBEPokemon user, PBETarget requestedTargets, bool canHitFarCorners)
        {
            PBETeam attackerTeam = Teams[user.LocalTeam ? 0 : 1]; // Attacker's team
            PBETeam opposingTeam = Teams[user.LocalTeam ? 1 : 0]; // Other team

            var targets = new List<PBEPokemon>();
            if (requestedTargets.HasFlag(PBETarget.AllyLeft))
            {
                PBEPokemon b = attackerTeam.PokemonAtPosition(PBEFieldPosition.Left);
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PBETarget.AllyCenter))
            {
                PBEPokemon b = attackerTeam.PokemonAtPosition(PBEFieldPosition.Center);
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PBETarget.AllyRight))
            {
                PBEPokemon b = attackerTeam.PokemonAtPosition(PBEFieldPosition.Right);
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PBETarget.FoeLeft))
            {
                PBEPokemon b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Left);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleFormat == PBEBattleFormat.Double)
                    {
                        b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Right);
                    }
                    else if (BattleFormat == PBEBattleFormat.Triple)
                    {
                        b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Center);
                        // Center fainted as well and user can reach far right
                        if (b == null && (user.FieldPosition != PBEFieldPosition.Right || canHitFarCorners))
                        {
                            b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Right);
                        }
                    }
                }
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PBETarget.FoeCenter))
            {
                PBEPokemon b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Center);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleFormat == PBEBattleFormat.Triple)
                    {
                        if (user.FieldPosition == PBEFieldPosition.Left)
                        {
                            b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Right);
                            // Right fainted as well and user can reach far left
                            if (b == null && (user.FieldPosition != PBEFieldPosition.Left || canHitFarCorners))
                            {
                                b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Left);
                            }
                        }
                        else if (user.FieldPosition == PBEFieldPosition.Right)
                        {
                            b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Left);
                            // Left fainted as well and user can reach far right
                            if (b == null && (user.FieldPosition != PBEFieldPosition.Right || canHitFarCorners))
                            {
                                b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Right);
                            }
                        }
                        else // Center
                        {
                            PBEPokemon oppLeft = opposingTeam.PokemonAtPosition(PBEFieldPosition.Left),
                                oppRight = opposingTeam.PokemonAtPosition(PBEFieldPosition.Right);
                            // Left is dead but not right
                            if (oppLeft == null && oppRight != null)
                            {
                                b = oppRight;
                            }
                            // Right is dead but not left
                            else if (oppLeft != null && oppRight == null)
                            {
                                b = oppLeft;
                            }
                            // Randomly select left or right
                            else
                            {
                                b = PBEUtils.RNG.NextBoolean() ? oppLeft : oppRight;
                            }
                        }
                    }
                }
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PBETarget.FoeRight))
            {
                PBEPokemon b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Right);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleFormat == PBEBattleFormat.Double)
                    {
                        b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Left);
                    }
                    else if (BattleFormat == PBEBattleFormat.Triple)
                    {
                        b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Center);
                        // Center fainted as well and user can reach far left
                        if (b == null && (user.FieldPosition != PBEFieldPosition.Left || canHitFarCorners))
                        {
                            b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Left);
                        }
                    }
                }
                targets.Add(b);
            }
            return targets.Where(p => p != null).Distinct().ToArray(); // Remove duplicate targets
        }

        /// <summary>
        /// Determines whether chosen targets are valid for a given move.
        /// </summary>
        /// <param name="pkmn">The Pokémon that will act.</param>
        /// <param name="move">The move the Pokémon wishes to use.</param>
        /// <param name="targets">The targets bitfield to validate.</param>
        public bool AreTargetsValid(PBEPokemon pkmn, PBEMove move, PBETarget targets)
        {
            PBEMoveTarget possibleTargets = GetMoveTargetsForPokemon(pkmn, move);
            switch (BattleFormat)
            {
                case PBEBattleFormat.Single:
                case PBEBattleFormat.Rotation:
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                            if (targets != (PBETarget.AllyCenter | PBETarget.FoeCenter))
                            {
                                return false;
                            }
                            break;
                        case PBEMoveTarget.AllFoes:
                        case PBEMoveTarget.AllFoesSurrounding:
                        case PBEMoveTarget.AllSurrounding:
                        case PBEMoveTarget.SingleFoeSurrounding:
                        case PBEMoveTarget.SingleNotSelf:
                        case PBEMoveTarget.SingleSurrounding:
                            if (targets != PBETarget.FoeCenter)
                            {
                                return false;
                            }
                            break;
                        case PBEMoveTarget.AllTeam:
                        case PBEMoveTarget.Self:
                        case PBEMoveTarget.SelfOrAllySurrounding:
                            if (targets != PBETarget.AllyCenter)
                            {
                                return false;
                            }
                            break;
                    }
                    break;
                case PBEBattleFormat.Double:
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                            if (targets != (PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight))
                            {
                                return false;
                            }
                            break;
                        case PBEMoveTarget.AllFoes:
                        case PBEMoveTarget.AllFoesSurrounding:
                            if (targets != (PBETarget.FoeLeft | PBETarget.FoeRight))
                            {
                                return false;
                            }
                            break;
                        case PBEMoveTarget.AllTeam:
                            if (targets != (PBETarget.AllyLeft | PBETarget.AllyRight))
                            {
                                return false;
                            }
                            break;
                        case PBEMoveTarget.AllSurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (targets != (PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (targets != (PBETarget.AllyLeft | PBETarget.FoeLeft | PBETarget.FoeRight))
                                {
                                    return false;
                                }
                            }
                            break;
                        case PBEMoveTarget.Self:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (targets != PBETarget.AllyLeft)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (targets != PBETarget.AllyRight)
                                {
                                    return false;
                                }
                            }
                            break;
                        case PBEMoveTarget.SelfOrAllySurrounding:
                            if (targets != PBETarget.AllyLeft && targets != PBETarget.AllyRight)
                            {
                                return false;
                            }
                            break;
                        case PBEMoveTarget.SingleAllySurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (targets != PBETarget.AllyRight)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (targets != PBETarget.AllyLeft)
                                {
                                    return false;
                                }
                            }
                            break;
                        case PBEMoveTarget.SingleFoeSurrounding:
                            if (targets != PBETarget.FoeLeft && targets != PBETarget.FoeRight)
                            {
                                return false;
                            }
                            break;
                        case PBEMoveTarget.SingleNotSelf:
                        case PBEMoveTarget.SingleSurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (targets != PBETarget.AllyRight && targets != PBETarget.FoeLeft && targets != PBETarget.FoeRight)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (targets != PBETarget.AllyLeft && targets != PBETarget.FoeLeft && targets != PBETarget.FoeRight)
                                {
                                    return false;
                                }
                            }
                            break;
                    }
                    break;
                case PBEBattleFormat.Triple:
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                            if (targets != (PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight))
                            {
                                return false;
                            }
                            break;
                        case PBEMoveTarget.AllFoes:
                            if (targets != (PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight))
                            {
                                return false;
                            }
                            break;
                        case PBEMoveTarget.AllFoesSurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (targets != (PBETarget.FoeCenter | PBETarget.FoeRight))
                                {
                                    return false;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                if (targets != (PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (targets != (PBETarget.FoeLeft | PBETarget.FoeCenter))
                                {
                                    return false;
                                }
                            }
                            break;
                        case PBEMoveTarget.AllSurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (targets != (PBETarget.AllyCenter | PBETarget.FoeCenter | PBETarget.FoeRight))
                                {
                                    return false;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                if (targets != (PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (targets != (PBETarget.AllyCenter | PBETarget.FoeLeft | PBETarget.FoeCenter))
                                {
                                    return false;
                                }
                            }
                            break;
                        case PBEMoveTarget.AllTeam:
                            if (targets != (PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight))
                            {
                                return false;
                            }
                            break;
                        case PBEMoveTarget.Self:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (targets != PBETarget.AllyLeft)
                                {
                                    return false;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                if (targets != PBETarget.AllyCenter)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (targets != PBETarget.AllyRight)
                                {
                                    return false;
                                }
                            }
                            break;
                        case PBEMoveTarget.SelfOrAllySurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (targets != PBETarget.AllyLeft && targets != PBETarget.AllyCenter)
                                {
                                    return false;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                if (targets != PBETarget.AllyLeft && targets != PBETarget.AllyCenter && targets != PBETarget.AllyRight)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (targets != PBETarget.AllyCenter && targets != PBETarget.AllyRight)
                                {
                                    return false;
                                }
                            }
                            break;
                        case PBEMoveTarget.SingleAllySurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (targets != PBETarget.AllyCenter)
                                {
                                    return false;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                if (targets != PBETarget.AllyLeft && targets != PBETarget.AllyRight)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (targets != PBETarget.AllyCenter)
                                {
                                    return false;
                                }
                            }
                            break;
                        case PBEMoveTarget.SingleFoeSurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (targets != PBETarget.FoeCenter && targets != PBETarget.FoeRight)
                                {
                                    return false;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                if (targets != PBETarget.FoeLeft && targets != PBETarget.FoeCenter && targets != PBETarget.FoeRight)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (targets != PBETarget.FoeLeft && targets != PBETarget.FoeCenter)
                                {
                                    return false;
                                }
                            }
                            break;
                        case PBEMoveTarget.SingleNotSelf:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (targets != PBETarget.AllyCenter && targets != PBETarget.AllyRight && targets != PBETarget.FoeLeft && targets != PBETarget.FoeCenter && targets != PBETarget.FoeRight)
                                {
                                    return false;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                if (targets != PBETarget.AllyLeft && targets != PBETarget.AllyRight && targets != PBETarget.FoeLeft && targets != PBETarget.FoeCenter && targets != PBETarget.FoeRight)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (targets != PBETarget.AllyLeft && targets != PBETarget.AllyCenter && targets != PBETarget.FoeLeft && targets != PBETarget.FoeCenter && targets != PBETarget.FoeRight)
                                {
                                    return false;
                                }
                            }
                            break;
                        case PBEMoveTarget.SingleSurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (targets != PBETarget.AllyCenter && targets != PBETarget.FoeCenter && targets != PBETarget.FoeRight)
                                {
                                    return false;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                if (targets != PBETarget.AllyLeft && targets != PBETarget.AllyRight && targets != PBETarget.FoeLeft && targets != PBETarget.FoeCenter && targets != PBETarget.FoeRight)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (targets != PBETarget.AllyCenter && targets != PBETarget.FoeLeft && targets != PBETarget.FoeCenter)
                                {
                                    return false;
                                }
                            }
                            break;
                    }
                    break;
            }
            return true;
        }
    }
}
