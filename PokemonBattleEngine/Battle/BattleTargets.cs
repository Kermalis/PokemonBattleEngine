using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
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
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(position));
                    }
                case PBEBattleFormat.Double:
                    if (position == PBEFieldPosition.Left)
                    {
                        return PBEFieldPosition.Right;
                    }
                    else if (position == PBEFieldPosition.Right)
                    {
                        return PBEFieldPosition.Left;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(position));
                    }
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
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(position));
                    }
                default: throw new ArgumentOutOfRangeException(nameof(BattleFormat));
            }
        }

        public static IEnumerable<PBEPokemon> GetRuntimeSurrounding(PBEPokemon pkmn, bool includeAllies, bool includeFoes)
        {
            IEnumerable<PBEPokemon> allies = pkmn.Team.ActiveBattlers.Where(p => p != pkmn);
            IEnumerable<PBEPokemon> foes = (pkmn.Team == pkmn.Team.Battle.Teams[0] ? pkmn.Team.Battle.Teams[1] : pkmn.Team.Battle.Teams[0]).ActiveBattlers;
            switch (pkmn.Team.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                    if (pkmn.FieldPosition == PBEFieldPosition.Center)
                    {
                        var ret = new List<PBEPokemon>();
                        if (includeFoes)
                        {
                            ret.AddRange(foes.Where(p => p.FieldPosition == PBEFieldPosition.Center));
                        }
                        return ret;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                    }
                case PBEBattleFormat.Double:
                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                    {
                        var ret = new List<PBEPokemon>();
                        if (includeAllies)
                        {
                            ret.AddRange(allies.Where(p => p.FieldPosition == PBEFieldPosition.Right));
                        }
                        if (includeFoes)
                        {
                            ret.AddRange(foes.Where(p => p.FieldPosition == PBEFieldPosition.Left || p.FieldPosition == PBEFieldPosition.Right));
                        }
                        return ret;
                    }
                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                    {
                        var ret = new List<PBEPokemon>();
                        if (includeAllies)
                        {
                            ret.AddRange(allies.Where(p => p.FieldPosition == PBEFieldPosition.Left));
                        }
                        if (includeFoes)
                        {
                            ret.AddRange(foes.Where(p => p.FieldPosition == PBEFieldPosition.Left || p.FieldPosition == PBEFieldPosition.Right));
                        }
                        return ret;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                    }
                case PBEBattleFormat.Triple:
                case PBEBattleFormat.Rotation:
                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                    {
                        var ret = new List<PBEPokemon>();
                        if (includeAllies)
                        {
                            ret.AddRange(allies.Where(p => p.FieldPosition == PBEFieldPosition.Center));
                        }
                        if (includeFoes)
                        {
                            ret.AddRange(foes.Where(p => p.FieldPosition == PBEFieldPosition.Center || p.FieldPosition == PBEFieldPosition.Right));
                        }
                        return ret;
                    }
                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                    {
                        var ret = new List<PBEPokemon>();
                        if (includeAllies)
                        {
                            ret.AddRange(allies.Where(p => p.FieldPosition == PBEFieldPosition.Left || p.FieldPosition == PBEFieldPosition.Right));
                        }
                        if (includeFoes)
                        {
                            ret.AddRange(foes.Where(p => p.FieldPosition == PBEFieldPosition.Left || p.FieldPosition == PBEFieldPosition.Center || p.FieldPosition == PBEFieldPosition.Right));
                        }
                        return ret;
                    }
                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                    {
                        var ret = new List<PBEPokemon>();
                        if (includeAllies)
                        {
                            ret.AddRange(allies.Where(p => p.FieldPosition == PBEFieldPosition.Center));
                        }
                        if (includeFoes)
                        {
                            ret.AddRange(foes.Where(p => p.FieldPosition == PBEFieldPosition.Center || p.FieldPosition == PBEFieldPosition.Left));
                        }
                        return ret;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                    }
                default: throw new ArgumentOutOfRangeException(nameof(pkmn.Team.Battle.BattleFormat));
            }
        }

        /// <summary>
        /// Gets all Pokémon that will be hit.
        /// </summary>
        /// <param name="user">The Pokémon that will act.</param>
        /// <param name="requestedTargets">The targets the Pokémon wishes to hit.</param>
        /// <param name="canHitFarCorners">Whether the move can hit far Pokémon in a triple battle.</param>
        PBEPokemon[] GetRuntimeTargets(PBEPokemon user, PBETarget requestedTargets, bool canHitFarCorners)
        {
            PBETeam opposingTeam = user.Team == Teams[0] ? Teams[1] : Teams[0];

            var targets = new List<PBEPokemon>();
            if (requestedTargets.HasFlag(PBETarget.AllyLeft))
            {
                PBEPokemon b = user.Team.TryGetPokemonAtPosition(PBEFieldPosition.Left);
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PBETarget.AllyCenter))
            {
                PBEPokemon b = user.Team.TryGetPokemonAtPosition(PBEFieldPosition.Center);
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PBETarget.AllyRight))
            {
                PBEPokemon b = user.Team.TryGetPokemonAtPosition(PBEFieldPosition.Right);
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PBETarget.FoeLeft))
            {
                PBEPokemon b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Left);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleFormat == PBEBattleFormat.Double)
                    {
                        b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Right);
                    }
                    else if (BattleFormat == PBEBattleFormat.Triple)
                    {
                        b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Center);
                        // Center fainted as well and user can reach far right
                        if (b == null && (user.FieldPosition != PBEFieldPosition.Right || canHitFarCorners))
                        {
                            b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Right);
                        }
                    }
                }
                targets.Add(b);
            }
            if (requestedTargets.HasFlag(PBETarget.FoeCenter))
            {
                PBEPokemon b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Center);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleFormat == PBEBattleFormat.Triple)
                    {
                        if (user.FieldPosition == PBEFieldPosition.Left)
                        {
                            b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Right);
                            // Right fainted as well and user can reach far left
                            if (b == null && (user.FieldPosition != PBEFieldPosition.Left || canHitFarCorners))
                            {
                                b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Left);
                            }
                        }
                        else if (user.FieldPosition == PBEFieldPosition.Right)
                        {
                            b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Left);
                            // Left fainted as well and user can reach far right
                            if (b == null && (user.FieldPosition != PBEFieldPosition.Right || canHitFarCorners))
                            {
                                b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Right);
                            }
                        }
                        else // Center
                        {
                            PBEPokemon oppLeft = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Left),
                                oppRight = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Right);
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
                PBEPokemon b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Right);
                // Target fainted, fallback to its teammate
                if (b == null)
                {
                    if (BattleFormat == PBEBattleFormat.Double)
                    {
                        b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Left);
                    }
                    else if (BattleFormat == PBEBattleFormat.Triple)
                    {
                        b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Center);
                        // Center fainted as well and user can reach far left
                        if (b == null && (user.FieldPosition != PBEFieldPosition.Left || canHitFarCorners))
                        {
                            b = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Left);
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
            PBEMoveTarget possibleTargets = pkmn.GetMoveTargets(move);
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
                default: throw new ArgumentOutOfRangeException(nameof(BattleFormat));
            }
            return true;
        }
    }
}
