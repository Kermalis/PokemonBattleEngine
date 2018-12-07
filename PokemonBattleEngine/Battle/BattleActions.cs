using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Kermalis.PokemonBattleEngine.Battle
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PBEAction
    {
        [FieldOffset(0)]
        public byte PokemonId;
        [FieldOffset(1)]
        public PBEDecision Decision;
        [FieldOffset(2)]
        public PBEMove FightMove;
        [FieldOffset(4)]
        public PBETarget FightTargets;
        [FieldOffset(4)]
        public byte SwitchPokemonId;

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.Add(PokemonId);
            bytes.Add((byte)Decision);
            bytes.AddRange(BitConverter.GetBytes((ushort)FightMove));
            bytes.Add(SwitchPokemonId);
            return bytes.ToArray();
        }
        internal static PBEAction FromBytes(BinaryReader r)
        {
            return new PBEAction
            {
                PokemonId = r.ReadByte(),
                Decision = (PBEDecision)r.ReadByte(),
                FightMove = (PBEMove)r.ReadUInt16(),
                SwitchPokemonId = r.ReadByte()
            };
        }
    }
    public sealed partial class PBEBattle
    {
        public bool AreActionsValid(bool local, IEnumerable<PBEAction> actions)
        {
            if (BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBEBattleState.WaitingForActions)} to validate actions.");
            }

            PBETeam team = Teams[local ? 0 : 1];
            if (actions.Count() == 0 || actions.Count() != team.ActionsRequired.Count)
            {
                return false;
            }

            var standBy = new List<PBEPokemon>();
            foreach (PBEAction action in actions)
            {
                PBEPokemon pkmn = GetPokemon(action.PokemonId);
                // Validate Pokémon
                if (!team.ActionsRequired.Contains(pkmn))
                {
                    return false;
                }
                switch (action.Decision)
                {
                    case PBEDecision.Fight:
                        // Invalid move
                        if (!pkmn.Moves.Contains(action.FightMove) || action.FightMove == PBEMove.None)
                        {
                            return false;
                        }
                        // TODO: Struggle
                        // Out of PP
                        if (pkmn.PP[Array.IndexOf(pkmn.Moves, action.FightMove)] < 1)
                        {
                            return false;
                        }
                        // If the mon has a locked move, it must be used
                        if (pkmn.LockedAction.Decision == PBEDecision.Fight)
                        {
                            if ((pkmn.LockedAction.FightMove != PBEMove.None && pkmn.LockedAction.FightMove != action.FightMove)
                                || (pkmn.LockedAction.FightTargets != PBETarget.None && pkmn.LockedAction.FightTargets != action.FightTargets))
                            {
                                return false;
                            }
                        }

                        // Verify targets
                        PBEMoveTarget possibleTargets = PBEMoveData.GetMoveTargetsForPokemon(pkmn, action.FightMove);
                        switch (BattleStyle)
                        {
                            case PBEBattleStyle.Single:
                            case PBEBattleStyle.Rotation:
                                switch (possibleTargets)
                                {
                                    case PBEMoveTarget.All:
                                        if (action.FightTargets != (PBETarget.AllyCenter | PBETarget.FoeCenter))
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
                                        if (action.FightTargets != PBETarget.FoeCenter)
                                        {
                                            return false;
                                        }
                                        break;
                                    case PBEMoveTarget.AllTeam:
                                    case PBEMoveTarget.Self:
                                    case PBEMoveTarget.SelfOrAllySurrounding:
                                        if (action.FightTargets != PBETarget.AllyCenter)
                                        {
                                            return false;
                                        }
                                        break;
                                }
                                break;
                            case PBEBattleStyle.Double:
                                switch (possibleTargets)
                                {
                                    case PBEMoveTarget.All:
                                        if (action.FightTargets != (PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight))
                                        {
                                            return false;
                                        }
                                        break;
                                    case PBEMoveTarget.AllFoes:
                                    case PBEMoveTarget.AllFoesSurrounding:
                                        if (action.FightTargets != (PBETarget.FoeLeft | PBETarget.FoeRight))
                                        {
                                            return false;
                                        }
                                        break;
                                    case PBEMoveTarget.AllTeam:
                                        if (action.FightTargets != (PBETarget.AllyLeft | PBETarget.AllyRight))
                                        {
                                            return false;
                                        }
                                        break;
                                    case PBEMoveTarget.AllSurrounding:
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            if (action.FightTargets != (PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight))
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (action.FightTargets != (PBETarget.AllyLeft | PBETarget.FoeLeft | PBETarget.FoeRight))
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    case PBEMoveTarget.Self:
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PBETarget.AllyLeft)
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PBETarget.AllyRight)
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    case PBEMoveTarget.SelfOrAllySurrounding:
                                        if (action.FightTargets != PBETarget.AllyLeft && action.FightTargets != PBETarget.AllyRight)
                                        {
                                            return false;
                                        }
                                        break;
                                    case PBEMoveTarget.SingleAllySurrounding:
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PBETarget.AllyRight)
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PBETarget.AllyLeft)
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    case PBEMoveTarget.SingleFoeSurrounding:
                                        if (action.FightTargets != PBETarget.FoeLeft && action.FightTargets != PBETarget.FoeRight)
                                        {
                                            return false;
                                        }
                                        break;
                                    case PBEMoveTarget.SingleNotSelf:
                                    case PBEMoveTarget.SingleSurrounding:
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PBETarget.AllyRight && action.FightTargets != PBETarget.FoeLeft && action.FightTargets != PBETarget.FoeRight)
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PBETarget.AllyLeft && action.FightTargets != PBETarget.FoeLeft && action.FightTargets != PBETarget.FoeRight)
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                }
                                break;
                            case PBEBattleStyle.Triple:
                                switch (possibleTargets)
                                {
                                    case PBEMoveTarget.All:
                                        if (action.FightTargets != (PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight))
                                        {
                                            return false;
                                        }
                                        break;
                                    case PBEMoveTarget.AllFoes:
                                        if (action.FightTargets != (PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight))
                                        {
                                            return false;
                                        }
                                        break;
                                    case PBEMoveTarget.AllFoesSurrounding:
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            if (action.FightTargets != (PBETarget.FoeCenter | PBETarget.FoeRight))
                                            {
                                                return false;
                                            }
                                        }
                                        else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                        {
                                            if (action.FightTargets != (PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight))
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (action.FightTargets != (PBETarget.FoeLeft | PBETarget.FoeCenter))
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    case PBEMoveTarget.AllSurrounding:
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            if (action.FightTargets != (PBETarget.AllyCenter | PBETarget.FoeCenter | PBETarget.FoeRight))
                                            {
                                                return false;
                                            }
                                        }
                                        else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                        {
                                            if (action.FightTargets != (PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight))
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (action.FightTargets != (PBETarget.AllyCenter | PBETarget.FoeLeft | PBETarget.FoeCenter))
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    case PBEMoveTarget.AllTeam:
                                        if (action.FightTargets != (PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight))
                                        {
                                            return false;
                                        }
                                        break;
                                    case PBEMoveTarget.Self:
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PBETarget.AllyLeft)
                                            {
                                                return false;
                                            }
                                        }
                                        else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                        {
                                            if (action.FightTargets != PBETarget.AllyCenter)
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PBETarget.AllyRight)
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    case PBEMoveTarget.SelfOrAllySurrounding:
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PBETarget.AllyLeft && action.FightTargets != PBETarget.AllyCenter)
                                            {
                                                return false;
                                            }
                                        }
                                        else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                        {
                                            if (action.FightTargets != PBETarget.AllyLeft && action.FightTargets != PBETarget.AllyCenter && action.FightTargets != PBETarget.AllyRight)
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PBETarget.AllyCenter && action.FightTargets != PBETarget.AllyRight)
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    case PBEMoveTarget.SingleAllySurrounding:
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PBETarget.AllyCenter)
                                            {
                                                return false;
                                            }
                                        }
                                        else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                        {
                                            if (action.FightTargets != PBETarget.AllyLeft && action.FightTargets != PBETarget.AllyRight)
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PBETarget.AllyCenter)
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    case PBEMoveTarget.SingleFoeSurrounding:
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PBETarget.FoeCenter && action.FightTargets != PBETarget.FoeRight)
                                            {
                                                return false;
                                            }
                                        }
                                        else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                        {
                                            if (action.FightTargets != PBETarget.FoeLeft && action.FightTargets != PBETarget.FoeCenter && action.FightTargets != PBETarget.FoeRight)
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PBETarget.FoeLeft && action.FightTargets != PBETarget.FoeCenter)
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    case PBEMoveTarget.SingleNotSelf:
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PBETarget.AllyCenter && action.FightTargets != PBETarget.AllyRight && action.FightTargets != PBETarget.FoeLeft && action.FightTargets != PBETarget.FoeCenter && action.FightTargets != PBETarget.FoeRight)
                                            {
                                                return false;
                                            }
                                        }
                                        else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                        {
                                            if (action.FightTargets != PBETarget.AllyLeft && action.FightTargets != PBETarget.AllyRight && action.FightTargets != PBETarget.FoeLeft && action.FightTargets != PBETarget.FoeCenter && action.FightTargets != PBETarget.FoeRight)
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PBETarget.AllyLeft && action.FightTargets != PBETarget.AllyCenter && action.FightTargets != PBETarget.FoeLeft && action.FightTargets != PBETarget.FoeCenter && action.FightTargets != PBETarget.FoeRight)
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                    case PBEMoveTarget.SingleSurrounding:
                                        if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                        {
                                            if (action.FightTargets != PBETarget.AllyCenter && action.FightTargets != PBETarget.FoeCenter && action.FightTargets != PBETarget.FoeRight)
                                            {
                                                return false;
                                            }
                                        }
                                        else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                        {
                                            if (action.FightTargets != PBETarget.AllyLeft && action.FightTargets != PBETarget.AllyRight && action.FightTargets != PBETarget.FoeLeft && action.FightTargets != PBETarget.FoeCenter && action.FightTargets != PBETarget.FoeRight)
                                            {
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            if (action.FightTargets != PBETarget.AllyCenter && action.FightTargets != PBETarget.FoeLeft && action.FightTargets != PBETarget.FoeCenter)
                                            {
                                                return false;
                                            }
                                        }
                                        break;
                                }
                                break;
                        }
                        break;
                    case PBEDecision.Switch:
                        // Cannot switch while airborne, underground or underwater
                        if (pkmn.Status2.HasFlag(PBEStatus2.Airborne) || pkmn.Status2.HasFlag(PBEStatus2.Underground) || pkmn.Status2.HasFlag(PBEStatus2.Underwater))
                        {
                            return false;
                        }
                        PBEPokemon switchPkmn = GetPokemon(action.SwitchPokemonId);
                        // Validate the new battler's ID
                        if (switchPkmn == null || switchPkmn.LocalTeam != local || switchPkmn.Id == pkmn.Id)
                        {
                            return false;
                        }
                        // Cannot switch into a fainted Pokémon
                        if (switchPkmn.HP < 1)
                        {
                            return false;
                        }
                        // Cannot switch into a Pokémon already on the field
                        if (switchPkmn.FieldPosition != PBEFieldPosition.None)
                        {
                            return false;
                        }
                        // Cannot switch into a Pokémon already selected to switch in this turn
                        if (standBy.Contains(switchPkmn))
                        {
                            return false;
                        }
                        standBy.Add(switchPkmn);
                        break;
                }
            }
            return true;
        }
        // Returns true if the actions were valid (and selected)
        public bool SelectActionsIfValid(bool local, IEnumerable<PBEAction> actions)
        {
            if (BattleState != PBEBattleState.WaitingForActions)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBEBattleState.WaitingForActions)} to select actions.");
            }
            if (AreActionsValid(local, actions))
            {
                Teams[local ? 0 : 1].ActionsRequired.Clear();
                foreach (PBEAction action in actions)
                {
                    PBEPokemon pkmn = GetPokemon(action.PokemonId);
                    pkmn.SelectedAction = action;
                    switch (pkmn.SelectedAction.Decision)
                    {
                        case PBEDecision.Fight:
                            switch (PBEMoveData.GetMoveTargetsForPokemon(pkmn, pkmn.SelectedAction.FightMove))
                            {
                                case PBEMoveTarget.RandomFoeSurrounding:
                                    switch (BattleStyle)
                                    {
                                        case PBEBattleStyle.Single:
                                        case PBEBattleStyle.Rotation:
                                            pkmn.SelectedAction.FightTargets = PBETarget.FoeCenter;
                                            break;
                                        case PBEBattleStyle.Double:
                                            pkmn.SelectedAction.FightTargets = PBEUtils.RNG.Next(2) == 0 ? PBETarget.FoeLeft : PBETarget.FoeRight;
                                            break;
                                        case PBEBattleStyle.Triple:
                                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                            {
                                                pkmn.SelectedAction.FightTargets = PBEUtils.RNG.Next(2) == 0 ? PBETarget.FoeCenter : PBETarget.FoeRight;
                                            }
                                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                            {
                                                PBETeam opposingTeam = Teams[pkmn.LocalTeam ? 1 : 0]; // Other team                                                                                                  
                                                int r; // Keep randomly picking until a non-fainted foe is selected
                                            roll:
                                                r = PBEUtils.RNG.Next(3);
                                                if (r == 0)
                                                {
                                                    if (opposingTeam.PokemonAtPosition(PBEFieldPosition.Left) != null)
                                                    {
                                                        pkmn.SelectedAction.FightTargets = PBETarget.FoeLeft;
                                                    }
                                                    else
                                                    {
                                                        goto roll;
                                                    }
                                                }
                                                else if (r == 1)
                                                {
                                                    if (opposingTeam.PokemonAtPosition(PBEFieldPosition.Center) != null)
                                                    {
                                                        pkmn.SelectedAction.FightTargets = PBETarget.FoeCenter;
                                                    }
                                                    else
                                                    {
                                                        goto roll;
                                                    }
                                                }
                                                else
                                                {
                                                    if (opposingTeam.PokemonAtPosition(PBEFieldPosition.Right) != null)
                                                    {
                                                        pkmn.SelectedAction.FightTargets = PBETarget.FoeRight;
                                                    }
                                                    else
                                                    {
                                                        goto roll;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                pkmn.SelectedAction.FightTargets = PBEUtils.RNG.Next(2) == 0 ? PBETarget.FoeLeft : PBETarget.FoeCenter;
                                            }
                                            break;
                                    }
                                    break;
                                case PBEMoveTarget.SingleAllySurrounding:
                                    if (BattleStyle == PBEBattleStyle.Single || BattleStyle == PBEBattleStyle.Rotation)
                                    {
                                        pkmn.SelectedAction.FightTargets = PBETarget.AllyCenter;
                                    }
                                    break;
                            }
                            break;
                    }
                }
                if (Teams[0].ActionsRequired.Count == 0 && Teams[1].ActionsRequired.Count == 0)
                {
                    BattleState = PBEBattleState.ReadyToRunTurn;
                    OnStateChanged?.Invoke(this);
                }
                return true;
            }
            return false;
        }

        public bool AreSwitchesValid(bool local, IEnumerable<Tuple<byte, PBEFieldPosition>> switches)
        {
            if (BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBEBattleState.WaitingForSwitchIns)} to validate switches.");
            }
            if (switches.Count() == 0 || switches.Count() != Teams[local ? 0 : 1].SwitchInsRequired)
            {
                return false;
            }
            foreach (Tuple<byte, PBEFieldPosition> s in switches)
            {
                PBEPokemon pkmn = GetPokemon(s.Item1);
                if (pkmn == null || pkmn.LocalTeam != local || pkmn.HP < 1 || pkmn.FieldPosition != PBEFieldPosition.None)
                {
                    return false;
                }
            }
            return true;
        }
        // Returns true if the switches were valid (and selected)
        public bool SelectSwitchesIfValid(bool local, IEnumerable<Tuple<byte, PBEFieldPosition>> switches)
        {
            if (BattleState != PBEBattleState.WaitingForSwitchIns)
            {
                throw new InvalidOperationException($"{nameof(BattleState)} must be {nameof(PBEBattleState.WaitingForSwitchIns)} to select switches.");
            }
            if (AreSwitchesValid(local, switches))
            {
                PBETeam team = Teams[local ? 0 : 1];
                team.SwitchInsRequired = 0;
                foreach (Tuple<byte, PBEFieldPosition> s in switches)
                {
                    PBEPokemon pkmn = GetPokemon(s.Item1);
                    pkmn.FieldPosition = s.Item2;
                    team.SwitchInQueue.Add(pkmn);
                }
                if (Teams[0].SwitchInsRequired == 0 && Teams[1].SwitchInsRequired == 0)
                {
                    SwitchInQueuedPokemon();
                    RequestActions();
                }
                return true;
            }
            return false;
        }

        public static PBEFieldPosition GetPositionAcross(PBEBattleStyle style, PBEFieldPosition pos)
        {
            switch (style)
            {
                case PBEBattleStyle.Single:
                case PBEBattleStyle.Rotation:
                    if (pos == PBEFieldPosition.Center)
                    {
                        return PBEFieldPosition.Center;
                    }
                    break;
                case PBEBattleStyle.Double:
                    if (pos == PBEFieldPosition.Left)
                    {
                        return PBEFieldPosition.Right;
                    }
                    else if (pos == PBEFieldPosition.Right)
                    {
                        return PBEFieldPosition.Left;
                    }
                    break;
                case PBEBattleStyle.Triple:
                    if (pos == PBEFieldPosition.Left)
                    {
                        return PBEFieldPosition.Right;
                    }
                    else if (pos == PBEFieldPosition.Center)
                    {
                        return PBEFieldPosition.Center;
                    }
                    else if (pos == PBEFieldPosition.Right)
                    {
                        return PBEFieldPosition.Left;
                    }
                    break;
            }
            return PBEFieldPosition.None;
        }
        // Gets Pokémon that can be hit at the moment
        // For example, if "FoeCenter" is passed in, logic will be used to fall-back to another opponent that can be hit if FoeCenter fainted
        // For each flag passed in, zero or one opponent will be returned for it
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
                    if (BattleStyle == PBEBattleStyle.Double)
                    {
                        b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Right);
                    }
                    else if (BattleStyle == PBEBattleStyle.Triple)
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
                    if (BattleStyle == PBEBattleStyle.Triple)
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
                    if (BattleStyle == PBEBattleStyle.Double)
                    {
                        b = opposingTeam.PokemonAtPosition(PBEFieldPosition.Left);
                    }
                    else if (BattleStyle == PBEBattleStyle.Triple)
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
    }
}
