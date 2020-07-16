using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed partial class PBEBattle
    {
        /// <summary>Gets the position across from the inputted position for a specific battle format.</summary>
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
                default: throw new ArgumentOutOfRangeException(nameof(battleFormat));
            }
        }

        /// <summary>Gets the Pokémon surrounding <paramref name="pkmn"/>.</summary>
        /// <param name="pkmn">The Pokémon to check.</param>
        /// <param name="includeAllies">True if allies should be included, False otherwise.</param>
        /// <param name="includeFoes">True if foes should be included, False otherwise.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="pkmn"/>'s <see cref="PBEBattle"/>'s <see cref="BattleFormat"/> is invalid or <paramref name="pkmn"/>'s <see cref="PBEBattlePokemon.FieldPosition"/> is invalid for <paramref name="pkmn"/>'s <see cref="PBEBattle"/>'s <see cref="BattleFormat"/>.</exception>
        public static List<PBEBattlePokemon> GetRuntimeSurrounding(PBEBattlePokemon pkmn, bool includeAllies, bool includeFoes)
        {
            if (pkmn == null)
            {
                throw new ArgumentNullException(nameof(pkmn));
            }
            if (!includeAllies && !includeFoes)
            {
                throw new ArgumentException($"\"{nameof(includeAllies)}\" and \"{nameof(includeFoes)}\" were false.");
            }
            IEnumerable<PBEBattlePokemon> allies = pkmn.Team.ActiveBattlers.Where(p => p != pkmn);
            IEnumerable<PBEBattlePokemon> foes = pkmn.Team.OpposingTeam.ActiveBattlers;
            switch (pkmn.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                {
                    if (pkmn.FieldPosition == PBEFieldPosition.Center)
                    {
                        var ret = new List<PBEBattlePokemon>();
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
                        var ret = new List<PBEBattlePokemon>();
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
                        var ret = new List<PBEBattlePokemon>();
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
                        var ret = new List<PBEBattlePokemon>();
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
                        var ret = new List<PBEBattlePokemon>();
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
                        var ret = new List<PBEBattlePokemon>();
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
                default: throw new ArgumentOutOfRangeException(nameof(pkmn.Battle.BattleFormat));
            }
        }

        /// <summary>Gets all Pokémon that will be hit.</summary>
        /// <param name="user">The Pokémon that will act.</param>
        /// <param name="requestedTargets">The targets the Pokémon wishes to hit.</param>
        /// <param name="canHitFarCorners">Whether the move can hit far Pokémon in a triple battle.</param>
        private static PBEBattlePokemon[] GetRuntimeTargets(PBEBattlePokemon user, PBETurnTarget requestedTargets, bool canHitFarCorners, PBERandom rand)
        {
            var targets = new List<PBEBattlePokemon>();
            // Foes first, then allies (since initial attack effects run that way)
            if (requestedTargets.HasFlag(PBETurnTarget.FoeLeft))
            {
                PBEBattlePokemon pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Left);
                if (pkmn == null)
                {
                    if (user.Battle.BattleFormat == PBEBattleFormat.Double)
                    {
                        pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Right);
                    }
                    else if (user.Battle.BattleFormat == PBEBattleFormat.Triple)
                    {
                        pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Center);
                        // Center fainted as well and user can reach far right
                        if (pkmn == null && (user.FieldPosition != PBEFieldPosition.Right || canHitFarCorners))
                        {
                            pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Right);
                        }
                    }
                }
                targets.Add(pkmn);
            }
            if (requestedTargets.HasFlag(PBETurnTarget.FoeCenter))
            {
                PBEBattlePokemon pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Center);
                // Target fainted, fallback to its teammate
                if (pkmn == null)
                {
                    if (user.Battle.BattleFormat == PBEBattleFormat.Triple)
                    {
                        if (user.FieldPosition == PBEFieldPosition.Left)
                        {
                            pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Right);
                            // Right fainted as well and user can reach far left
                            if (pkmn == null && (user.FieldPosition != PBEFieldPosition.Left || canHitFarCorners))
                            {
                                pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Left);
                            }
                        }
                        else if (user.FieldPosition == PBEFieldPosition.Right)
                        {
                            pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Left);
                            // Left fainted as well and user can reach far right
                            if (pkmn == null && (user.FieldPosition != PBEFieldPosition.Right || canHitFarCorners))
                            {
                                pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Right);
                            }
                        }
                        else // Center
                        {
                            PBEBattlePokemon oppLeft = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Left),
                                oppRight = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Right);
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
                                pkmn = rand.RandomBool() ? oppLeft : oppRight;
                            }
                        }
                    }
                }
                targets.Add(pkmn);
            }
            if (requestedTargets.HasFlag(PBETurnTarget.FoeRight))
            {
                PBEBattlePokemon pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Right);
                // Target fainted, fallback to its teammate
                if (pkmn == null)
                {
                    if (user.Battle.BattleFormat == PBEBattleFormat.Double)
                    {
                        pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Left);
                    }
                    else if (user.Battle.BattleFormat == PBEBattleFormat.Triple)
                    {
                        pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Center);
                        // Center fainted as well and user can reach far left
                        if (pkmn == null && (user.FieldPosition != PBEFieldPosition.Left || canHitFarCorners))
                        {
                            pkmn = user.Team.OpposingTeam.TryGetPokemon(PBEFieldPosition.Left);
                        }
                    }
                }
                targets.Add(pkmn);
            }
            if (requestedTargets.HasFlag(PBETurnTarget.AllyLeft))
            {
                targets.Add(user.Team.TryGetPokemon(PBEFieldPosition.Left));
            }
            if (requestedTargets.HasFlag(PBETurnTarget.AllyCenter))
            {
                targets.Add(user.Team.TryGetPokemon(PBEFieldPosition.Center));
            }
            if (requestedTargets.HasFlag(PBETurnTarget.AllyRight))
            {
                targets.Add(user.Team.TryGetPokemon(PBEFieldPosition.Right));
            }
            return targets.Where(p => p != null).Distinct().ToArray(); // Remove duplicate targets
        }

        /// <summary>Determines whether chosen targets are valid for a given move.</summary>
        /// <param name="pkmn">The Pokémon that will act.</param>
        /// <param name="move">The move the Pokémon wishes to use.</param>
        /// <param name="targets">The targets bitfield to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targets"/>, <paramref name="move"/>, <paramref name="pkmn"/>'s <see cref="PBEBattlePokemon.FieldPosition"/>, or <paramref name="pkmn"/>'s <see cref="PBEBattle"/>'s <see cref="BattleFormat"/> is invalid.</exception>
        public static bool AreTargetsValid(PBEBattlePokemon pkmn, PBEMove move, PBETurnTarget targets)
        {
            if (pkmn == null)
            {
                throw new ArgumentNullException(nameof(pkmn));
            }
            if (move == PBEMove.None || move >= PBEMove.MAX || !PBEMoveData.IsMoveUsable(move))
            {
                throw new ArgumentOutOfRangeException(nameof(move));
            }
            PBEMoveTarget possibleTargets = pkmn.GetMoveTargets(move);
            switch (pkmn.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                {
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return targets == (PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter);
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
                                return targets == PBETurnTarget.FoeCenter;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.AllTeam:
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.Self:
                        case PBEMoveTarget.SelfOrAllySurrounding:
                        case PBEMoveTarget.SingleAllySurrounding:
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return targets == PBETurnTarget.AllyCenter;
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
                                return targets == (PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight);
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
                                return targets == (PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight);
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
                                return targets == (PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight);
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
                                return targets == (PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight);
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == (PBETurnTarget.AllyLeft | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight);
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.Self:
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                return targets == PBETurnTarget.AllyLeft;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == PBETurnTarget.AllyRight;
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
                                return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyRight;
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
                                return targets == PBETurnTarget.AllyRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == PBETurnTarget.AllyLeft;
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
                                return targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeRight;
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
                                return targets == PBETurnTarget.AllyRight || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeRight;
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
                                return targets == (PBETurnTarget.AllyLeft | PBETurnTarget.AllyCenter | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight);
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
                                return targets == (PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight);
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
                                return targets == (PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight);
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return targets == (PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight);
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == (PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter);
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
                                return targets == (PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight);
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return targets == (PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight);
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == (PBETurnTarget.AllyCenter | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter);
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
                                return targets == (PBETurnTarget.AllyLeft | PBETurnTarget.AllyCenter | PBETurnTarget.AllyRight);
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.Self:
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                return targets == PBETurnTarget.AllyLeft;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return targets == PBETurnTarget.AllyCenter;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == PBETurnTarget.AllyRight;
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
                                return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyCenter;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyCenter || targets == PBETurnTarget.AllyRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == PBETurnTarget.AllyCenter || targets == PBETurnTarget.AllyRight;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.SingleAllySurrounding:
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == PBETurnTarget.AllyCenter;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyRight;
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
                                return targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter;
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
                                return targets == PBETurnTarget.AllyCenter || targets == PBETurnTarget.AllyRight || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyRight || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyCenter || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
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
                                return targets == PBETurnTarget.AllyCenter || targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return targets == PBETurnTarget.AllyLeft || targets == PBETurnTarget.AllyRight || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter || targets == PBETurnTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == PBETurnTarget.AllyCenter || targets == PBETurnTarget.FoeLeft || targets == PBETurnTarget.FoeCenter;
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
                                return targets == (PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter);
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
                                return targets == PBETurnTarget.FoeCenter;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.AllTeam:
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.Self:
                        case PBEMoveTarget.SelfOrAllySurrounding:
                        case PBEMoveTarget.SingleAllySurrounding:
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return targets == PBETurnTarget.AllyCenter;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(possibleTargets));
                    }
                }
                default: throw new ArgumentOutOfRangeException(nameof(pkmn.Battle.BattleFormat));
            }
        }

        /// <summary>Gets a random target a move can hit when called by <see cref="PBEMoveEffect.Metronome"/>.</summary>
        /// <param name="pkmn">The Pokémon using <paramref name="calledMove"/>.</param>
        /// <param name="calledMove">The move being called.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="calledMove"/>, <paramref name="pkmn"/>'s <see cref="PBEBattlePokemon.FieldPosition"/>, or <paramref name="pkmn"/>'s <see cref="PBEBattle"/>'s <see cref="BattleFormat"/> is invalid.</exception>
        public static PBETurnTarget GetRandomTargetForMetronome(PBEBattlePokemon pkmn, PBEMove calledMove, PBERandom rand)
        {
            if (pkmn == null)
            {
                throw new ArgumentNullException(nameof(pkmn));
            }
            if (calledMove == PBEMove.None || calledMove >= PBEMove.MAX || !PBEMoveData.IsMoveUsable(calledMove))
            {
                throw new ArgumentOutOfRangeException(nameof(calledMove));
            }
            PBEMoveTarget possibleTargets = pkmn.GetMoveTargets(calledMove);
            switch (pkmn.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                {
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.AllFoes:
                        case PBEMoveTarget.AllFoesSurrounding:
                        case PBEMoveTarget.AllSurrounding:
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.SingleFoeSurrounding:
                        case PBEMoveTarget.SingleNotSelf:
                        case PBEMoveTarget.SingleSurrounding:
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return PBETurnTarget.FoeCenter;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.AllTeam:
                        case PBEMoveTarget.Self:
                        case PBEMoveTarget.SelfOrAllySurrounding:
                        case PBEMoveTarget.SingleAllySurrounding: // Helping Hand cannot be called by Metronome anyway
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return PBETurnTarget.AllyCenter;
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
                                return PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
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
                                return PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
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
                                return PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight;
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
                                return PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return PBETurnTarget.AllyLeft | PBETurnTarget.FoeLeft | PBETurnTarget.FoeRight;
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
                                return PBETurnTarget.AllyLeft;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return PBETurnTarget.AllyRight;
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
                                if (rand.RandomBool())
                                {
                                    return PBETurnTarget.AllyLeft;
                                }
                                else
                                {
                                    return PBETurnTarget.AllyRight;
                                }
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.SingleAllySurrounding: // Helping Hand cannot be called by Metronome anyway
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                return PBETurnTarget.AllyRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return PBETurnTarget.AllyLeft;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.SingleFoeSurrounding:
                        case PBEMoveTarget.SingleNotSelf:
                        case PBEMoveTarget.SingleSurrounding:
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                if (rand.RandomBool())
                                {
                                    return PBETurnTarget.FoeLeft;
                                }
                                else
                                {
                                    return PBETurnTarget.FoeRight;
                                }
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
                                return PBETurnTarget.AllyLeft | PBETurnTarget.AllyCenter | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
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
                                return PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
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
                                return PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter;
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
                                return PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return PBETurnTarget.AllyLeft | PBETurnTarget.AllyRight | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter | PBETurnTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return PBETurnTarget.AllyCenter | PBETurnTarget.FoeLeft | PBETurnTarget.FoeCenter;
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
                                return PBETurnTarget.AllyLeft | PBETurnTarget.AllyCenter | PBETurnTarget.AllyRight;
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
                                return PBETurnTarget.AllyLeft;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return PBETurnTarget.AllyCenter;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return PBETurnTarget.AllyRight;
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
                                if (rand.RandomBool())
                                {
                                    return PBETurnTarget.AllyLeft;
                                }
                                else
                                {
                                    return PBETurnTarget.AllyCenter;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                int val = rand.RandomInt(0, 2);
                                if (val == 0)
                                {
                                    return PBETurnTarget.AllyLeft;
                                }
                                else if (val == 1)
                                {
                                    return PBETurnTarget.AllyCenter;
                                }
                                else
                                {
                                    return PBETurnTarget.AllyRight;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                if (rand.RandomBool())
                                {
                                    return PBETurnTarget.AllyCenter;
                                }
                                else
                                {
                                    return PBETurnTarget.AllyRight;
                                }
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.SingleAllySurrounding: // Helping Hand cannot be called by Metronome anyway
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                return PBETurnTarget.AllyCenter;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                if (rand.RandomBool())
                                {
                                    return PBETurnTarget.AllyLeft;
                                }
                                else
                                {
                                    return PBETurnTarget.AllyRight;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return PBETurnTarget.AllyCenter;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.SingleFoeSurrounding:
                        case PBEMoveTarget.SingleSurrounding:
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                if (rand.RandomBool())
                                {
                                    return PBETurnTarget.FoeCenter;
                                }
                                else
                                {
                                    return PBETurnTarget.FoeRight;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                int val = rand.RandomInt(0, 2);
                                if (val == 0)
                                {
                                    return PBETurnTarget.FoeLeft;
                                }
                                else if (val == 1)
                                {
                                    return PBETurnTarget.FoeCenter;
                                }
                                else
                                {
                                    return PBETurnTarget.FoeRight;
                                }
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                if (rand.RandomBool())
                                {
                                    return PBETurnTarget.FoeLeft;
                                }
                                else
                                {
                                    return PBETurnTarget.FoeCenter;
                                }
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.SingleNotSelf:
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                int val = rand.RandomInt(0, 2);
                                if (val == 0)
                                {
                                    return PBETurnTarget.FoeLeft;
                                }
                                else if (val == 1)
                                {
                                    return PBETurnTarget.FoeCenter;
                                }
                                else
                                {
                                    return PBETurnTarget.FoeRight;
                                }
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
                                return PBETurnTarget.AllyCenter | PBETurnTarget.FoeCenter;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.AllFoes:
                        case PBEMoveTarget.AllFoesSurrounding:
                        case PBEMoveTarget.AllSurrounding:
                        case PBEMoveTarget.RandomFoeSurrounding:
                        case PBEMoveTarget.SingleFoeSurrounding:
                        case PBEMoveTarget.SingleNotSelf:
                        case PBEMoveTarget.SingleSurrounding:
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return PBETurnTarget.FoeCenter;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        case PBEMoveTarget.AllTeam:
                        case PBEMoveTarget.Self:
                        case PBEMoveTarget.SelfOrAllySurrounding:
                        case PBEMoveTarget.SingleAllySurrounding: // Helping Hand cannot be called by Metronome anyway
                        {
                            if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return PBETurnTarget.AllyCenter;
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                            }
                        }
                        default: throw new ArgumentOutOfRangeException(nameof(possibleTargets));
                    }
                }
                default: throw new ArgumentOutOfRangeException(nameof(pkmn.Battle.BattleFormat));
            }
        }
    }
}
