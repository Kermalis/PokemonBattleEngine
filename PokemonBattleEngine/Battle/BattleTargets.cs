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
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="battleFormat"/> is invalid or <paramref name="position"/> is invalid for <paramref name="battleFormat"/>.</exception>
        public static PBEFieldPosition GetPositionAcross(PBEBattleFormat battleFormat, PBEFieldPosition position)
        {
            switch (battleFormat)
            {
                case PBEBattleFormat.Single:
                case PBEBattleFormat.Rotation:
                    {
                        if (position == PBEFieldPosition.Center)
                        {
                            return PBEFieldPosition.Center;
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(nameof(position));
                        }
                    }
                case PBEBattleFormat.Double:
                    {
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
                    }
                case PBEBattleFormat.Triple:
                    {
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
                    }
                default: throw new ArgumentOutOfRangeException(nameof(BattleFormat));
            }
        }

        /// <summary>
        /// Gets the Pokémon surrounding <paramref name="pkmn"/>.
        /// </summary>
        /// <param name="pkmn">The Pokémon to check.</param>
        /// <param name="includeAllies">True if allies should be included, False otherwise.</param>
        /// <param name="includeFoes">True if foes should be included, False otherwise.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="pkmn"/>'s <see cref="PBEBattle"/>'s <see cref="BattleFormat"/> is invalid or <paramref name="pkmn"/>'s <see cref="PBEPokemon.FieldPosition"/> is invalid for <paramref name="pkmn"/>'s <see cref="PBEBattle"/>'s <see cref="BattleFormat"/>.</exception>
        public static IEnumerable<PBEPokemon> GetRuntimeSurrounding(PBEPokemon pkmn, bool includeAllies, bool includeFoes)
        {
            IEnumerable<PBEPokemon> allies = pkmn.Team.ActiveBattlers.Where(p => p != pkmn);
            IEnumerable<PBEPokemon> foes = (pkmn.Team == pkmn.Team.Battle.Teams[0] ? pkmn.Team.Battle.Teams[1] : pkmn.Team.Battle.Teams[0]).ActiveBattlers;
            switch (pkmn.Team.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                    {
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
                    }
                case PBEBattleFormat.Double:
                    {
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
                    }
                case PBEBattleFormat.Triple:
                case PBEBattleFormat.Rotation:
                    {
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
        static PBEPokemon[] GetRuntimeTargets(PBEPokemon user, PBETarget requestedTargets, bool canHitFarCorners)
        {
            PBETeam opposingTeam = user.Team == user.Team.Battle.Teams[0] ? user.Team.Battle.Teams[1] : user.Team.Battle.Teams[0];

            var targets = new List<PBEPokemon>();
            if (requestedTargets.HasFlag(PBETarget.AllyLeft))
            {
                targets.Add(user.Team.TryGetPokemonAtPosition(PBEFieldPosition.Left));
            }
            if (requestedTargets.HasFlag(PBETarget.AllyCenter))
            {
                targets.Add(user.Team.TryGetPokemonAtPosition(PBEFieldPosition.Center));
            }
            if (requestedTargets.HasFlag(PBETarget.AllyRight))
            {
                targets.Add(user.Team.TryGetPokemonAtPosition(PBEFieldPosition.Right));
            }
            if (requestedTargets.HasFlag(PBETarget.FoeLeft))
            {
                PBEPokemon pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Left);
                if (pkmn == null)
                {
                    if (user.Team.Battle.BattleFormat == PBEBattleFormat.Double)
                    {
                        pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Right);
                    }
                    else if (user.Team.Battle.BattleFormat == PBEBattleFormat.Triple)
                    {
                        pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Center);
                        // Center fainted as well and user can reach far right
                        if (pkmn == null && (user.FieldPosition != PBEFieldPosition.Right || canHitFarCorners))
                        {
                            pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Right);
                        }
                    }
                }
                targets.Add(pkmn);
            }
            if (requestedTargets.HasFlag(PBETarget.FoeCenter))
            {
                PBEPokemon pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Center);
                // Target fainted, fallback to its teammate
                if (pkmn == null)
                {
                    if (user.Team.Battle.BattleFormat == PBEBattleFormat.Triple)
                    {
                        if (user.FieldPosition == PBEFieldPosition.Left)
                        {
                            pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Right);
                            // Right fainted as well and user can reach far left
                            if (pkmn == null && (user.FieldPosition != PBEFieldPosition.Left || canHitFarCorners))
                            {
                                pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Left);
                            }
                        }
                        else if (user.FieldPosition == PBEFieldPosition.Right)
                        {
                            pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Left);
                            // Left fainted as well and user can reach far right
                            if (pkmn == null && (user.FieldPosition != PBEFieldPosition.Right || canHitFarCorners))
                            {
                                pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Right);
                            }
                        }
                        else // Center
                        {
                            PBEPokemon oppLeft = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Left),
                                oppRight = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Right);
                            // Left is dead but not right
                            if (oppLeft == null && oppRight != null)
                            {
                                pkmn = oppRight;
                            }
                            // Right is dead but not left
                            else if (oppLeft != null && oppRight == null)
                            {
                                pkmn = oppLeft;
                            }
                            // Randomly select left or right
                            else
                            {
                                pkmn = PBEUtils.RNG.NextBoolean() ? oppLeft : oppRight;
                            }
                        }
                    }
                }
                targets.Add(pkmn);
            }
            if (requestedTargets.HasFlag(PBETarget.FoeRight))
            {
                PBEPokemon pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Right);
                // Target fainted, fallback to its teammate
                if (pkmn == null)
                {
                    if (user.Team.Battle.BattleFormat == PBEBattleFormat.Double)
                    {
                        pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Left);
                    }
                    else if (user.Team.Battle.BattleFormat == PBEBattleFormat.Triple)
                    {
                        pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Center);
                        // Center fainted as well and user can reach far left
                        if (pkmn == null && (user.FieldPosition != PBEFieldPosition.Left || canHitFarCorners))
                        {
                            pkmn = opposingTeam.TryGetPokemonAtPosition(PBEFieldPosition.Left);
                        }
                    }
                }
                targets.Add(pkmn);
            }
            return targets.Where(p => p != null).Distinct().ToArray(); // Remove duplicate targets
        }

        /// <summary>
        /// Determines whether chosen targets are valid for a given move.
        /// </summary>
        /// <param name="pkmn">The Pokémon that will act.</param>
        /// <param name="move">The move the Pokémon wishes to use.</param>
        /// <param name="targets">The targets bitfield to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targets"/>, <paramref name="move"/>, <paramref name="pkmn"/>'s <see cref="PBEPokemon.FieldPosition"/>, or <paramref name="pkmn"/>'s <see cref="PBEBattle"/>'s <see cref="BattleFormat"/> is invalid.</exception>
        public static bool AreTargetsValid(PBEPokemon pkmn, PBEMove move, PBETarget targets)
        {
            if (move == PBEMove.None || move >= PBEMove.MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            PBEMoveTarget possibleTargets = pkmn.GetMoveTargets(move);
            switch (pkmn.Team.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                    {
                        switch (possibleTargets)
                        {
                            case PBEMoveTarget.All:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return targets == (PBETarget.AllyCenter | PBETarget.FoeCenter);
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.AllFoes:
                            case PBEMoveTarget.AllFoesSurrounding:
                            case PBEMoveTarget.AllSurrounding:
                            case PBEMoveTarget.SingleFoeSurrounding:
                            case PBEMoveTarget.SingleNotSelf:
                            case PBEMoveTarget.SingleSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return targets == PBETarget.FoeCenter;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.AllTeam:
                            case PBEMoveTarget.Self:
                            case PBEMoveTarget.SelfOrAllySurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return targets == PBETarget.AllyCenter;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.RandomFoeSurrounding:
                            case PBEMoveTarget.SingleAllySurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(possibleTargets));
                        }
                    }
                case PBEBattleFormat.Double:
                    {
                        switch (possibleTargets)
                        {
                            case PBEMoveTarget.All:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == (PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight);
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.AllFoes:
                            case PBEMoveTarget.AllFoesSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == (PBETarget.FoeLeft | PBETarget.FoeRight);
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.AllTeam:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == (PBETarget.AllyLeft | PBETarget.AllyRight);
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.AllSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return targets == (PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight);
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == (PBETarget.AllyLeft | PBETarget.FoeLeft | PBETarget.FoeRight);
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.Self:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return targets == PBETarget.AllyLeft;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.AllyRight;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SelfOrAllySurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.AllyLeft || targets == PBETarget.AllyRight;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleAllySurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return targets == PBETarget.AllyRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.AllyLeft;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleFoeSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.FoeLeft || targets == PBETarget.FoeRight;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleNotSelf:
                            case PBEMoveTarget.SingleSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return targets == PBETarget.AllyRight || targets == PBETarget.FoeLeft || targets == PBETarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.AllyLeft || targets == PBETarget.FoeLeft || targets == PBETarget.FoeRight;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.RandomFoeSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(possibleTargets));
                        }
                    }
                case PBEBattleFormat.Triple:
                    {
                        switch (possibleTargets)
                        {
                            case PBEMoveTarget.All:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == (PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight);
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.AllFoes:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == (PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight);
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.AllFoesSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return targets == (PBETarget.FoeCenter | PBETarget.FoeRight);
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return targets == (PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight);
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == (PBETarget.FoeLeft | PBETarget.FoeCenter);
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.AllSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return targets == (PBETarget.AllyCenter | PBETarget.FoeCenter | PBETarget.FoeRight);
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return targets == (PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight);
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == (PBETarget.AllyCenter | PBETarget.FoeLeft | PBETarget.FoeCenter);
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.AllTeam:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == (PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight);
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.Self:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return targets == PBETarget.AllyLeft;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return targets == PBETarget.AllyCenter;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.AllyRight;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SelfOrAllySurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return targets == PBETarget.AllyLeft || targets == PBETarget.AllyCenter;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return targets == PBETarget.AllyLeft || targets == PBETarget.AllyCenter || targets == PBETarget.AllyRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.AllyCenter || targets == PBETarget.AllyRight;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleAllySurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return targets == PBETarget.AllyCenter;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return targets == PBETarget.AllyLeft || targets == PBETarget.AllyRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.AllyCenter;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleFoeSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return targets == PBETarget.FoeCenter || targets == PBETarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return targets == PBETarget.FoeLeft || targets == PBETarget.FoeCenter || targets == PBETarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.FoeLeft || targets == PBETarget.FoeCenter;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleNotSelf:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return targets == PBETarget.AllyCenter || targets == PBETarget.AllyRight || targets == PBETarget.FoeLeft || targets == PBETarget.FoeCenter || targets == PBETarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return targets == PBETarget.AllyLeft || targets == PBETarget.AllyRight || targets == PBETarget.FoeLeft || targets == PBETarget.FoeCenter || targets == PBETarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.AllyLeft || targets == PBETarget.AllyCenter || targets == PBETarget.FoeLeft || targets == PBETarget.FoeCenter || targets == PBETarget.FoeRight;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return targets == PBETarget.AllyCenter || targets == PBETarget.FoeCenter || targets == PBETarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return targets == PBETarget.AllyLeft || targets == PBETarget.AllyRight || targets == PBETarget.FoeLeft || targets == PBETarget.FoeCenter || targets == PBETarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.AllyCenter || targets == PBETarget.FoeLeft || targets == PBETarget.FoeCenter;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.RandomFoeSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(possibleTargets));
                        }
                    }
                case PBEBattleFormat.Rotation:
                    {
                        switch (possibleTargets)
                        {
                            case PBEMoveTarget.All:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == (PBETarget.AllyCenter | PBETarget.FoeCenter);
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.AllFoes:
                            case PBEMoveTarget.AllFoesSurrounding:
                            case PBEMoveTarget.AllSurrounding:
                            case PBEMoveTarget.SingleFoeSurrounding:
                            case PBEMoveTarget.SingleNotSelf:
                            case PBEMoveTarget.SingleSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.FoeCenter;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.AllTeam:
                            case PBEMoveTarget.Self:
                            case PBEMoveTarget.SelfOrAllySurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return targets == PBETarget.AllyCenter;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.RandomFoeSurrounding:
                            case PBEMoveTarget.SingleAllySurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return true;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(possibleTargets));
                        }
                    }
                default: throw new ArgumentOutOfRangeException(nameof(pkmn.Team.Battle.BattleFormat));
            }
        }
    }
}
