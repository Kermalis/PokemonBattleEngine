using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngine.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Kermalis.PokemonBattleEngine.Battle
{
    [StructLayout(LayoutKind.Explicit)]
    public sealed class PAction
    {
        [FieldOffset(4)]
        public Guid PokemonId;
        // TODO: Action (switch, forfeit, move)
        [FieldOffset(0)]
        public PMove Move;
        [FieldOffset(2)]
        public PTarget Targets;

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(PokemonId.ToByteArray());
            bytes.AddRange(BitConverter.GetBytes((ushort)Move));
            bytes.Add((byte)Targets);
            return bytes.ToArray();
        }
        internal static PAction FromBytes(BinaryReader r)
        {
            return new PAction
            {
                PokemonId = new Guid(r.ReadBytes(16)),
                Move = (PMove)r.ReadUInt16(),
                Targets = (PTarget)r.ReadByte()
            };
        }
    }

    public sealed partial class PBattle
    {
        public bool IsActionValid(PAction action)
        {
            // TODO: Non-fighting actions

            PBattlePokemon pkmn = Battler(action.PokemonId);

            // Not on the field
            if (pkmn == null)
                return false;

            // Invalid move
            if (!pkmn.Mon.Shell.Moves.Contains(action.Move) || action.Move == PMove.None)
                return false;

            // TODO: Struggle

            // Out of PP
            if (pkmn.Mon.PP[Array.IndexOf(pkmn.Mon.Shell.Moves, action.Move)] < 1)
                return false;

            PMoveData mData = PMoveData.Data[action.Move];
            switch (BattleStyle)
            {
                case PBattleStyle.Single:
                case PBattleStyle.Rotation:
                    // Only center in single battles & rotation battles
                    switch (mData.Targets)
                    {
                        case PMoveTarget.All:
                            if (action.Targets != (PTarget.AllyCenter | PTarget.FoeCenter))
                                return false;
                            break;
                        case PMoveTarget.AllFoes:
                        case PMoveTarget.AllFoesSurrounding:
                        case PMoveTarget.AllSurrounding:
                        case PMoveTarget.SingleFoeSurrounding:
                        case PMoveTarget.SingleNotSelf:
                        case PMoveTarget.SingleSurrounding:
                            if (action.Targets != PTarget.FoeCenter)
                                return false;
                            break;
                        case PMoveTarget.AllTeam:
                        case PMoveTarget.Self:
                        case PMoveTarget.SelfOrAllySurrounding:
                        case PMoveTarget.SingleAllySurrounding: // Client should send "AllyCenter" even though the move will fail
                            if (action.Targets != PTarget.AllyCenter)
                                return false;
                            break;
                        case PMoveTarget.RandomFoeSurrounding:
                            break;
                    }
                    break;
                case PBattleStyle.Double:
                    // No center in double battles
                    switch (mData.Targets)
                    {
                        case PMoveTarget.All:
                            if (action.Targets != (PTarget.AllyLeft | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllFoes:
                        case PMoveTarget.AllFoesSurrounding:
                            if (action.Targets != (PTarget.FoeLeft | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != (PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeRight))
                                    return false;
                            }
                            else
                            {
                                if (action.Targets != (PTarget.AllyLeft | PTarget.FoeLeft | PTarget.FoeRight))
                                    return false;
                            }
                            break;
                        case PMoveTarget.AllTeam:
                            if (action.Targets != (PTarget.AllyLeft | PTarget.AllyRight))
                                return false;
                            break;
                        case PMoveTarget.RandomFoeSurrounding:
                            break;
                        case PMoveTarget.Self:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyLeft)
                                    return false;
                            }
                            else
                            {
                                if (action.Targets != PTarget.AllyRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SelfOrAllySurrounding:
                            if (action.Targets != PTarget.AllyLeft && action.Targets != PTarget.AllyRight)
                                return false;
                            break;
                        case PMoveTarget.SingleAllySurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyRight)
                                    return false;
                            }
                            else
                            {
                                if (action.Targets != PTarget.AllyLeft)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleFoeSurrounding:
                            if (action.Targets != PTarget.FoeLeft && action.Targets != PTarget.FoeRight)
                                return false;
                            break;
                        case PMoveTarget.SingleNotSelf:
                        case PMoveTarget.SingleSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyRight && action.Targets != PTarget.FoeLeft && action.Targets != PTarget.FoeRight)
                                    return false;
                            }
                            else
                            {
                                if (action.Targets != PTarget.AllyLeft && action.Targets != PTarget.FoeLeft && action.Targets != PTarget.FoeRight)
                                    return false;
                            }
                            break;
                    }
                    break;
                case PBattleStyle.Triple:
                    switch (mData.Targets)
                    {
                        case PMoveTarget.All:
                            if (action.Targets != (PTarget.AllyLeft | PTarget.AllyCenter | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllFoes:
                            if (action.Targets != (PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllFoesSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != (PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (action.Targets != (PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            else
                            {
                                if (action.Targets != (PTarget.FoeLeft | PTarget.FoeCenter))
                                    return false;
                            }
                            break;
                        case PMoveTarget.AllSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != (PTarget.AllyCenter | PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (action.Targets != (PTarget.AllyLeft | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            else
                            {
                                if (action.Targets != (PTarget.AllyCenter | PTarget.FoeLeft | PTarget.FoeCenter))
                                    return false;
                            }
                            break;
                        case PMoveTarget.AllTeam:
                            if (action.Targets != (PTarget.AllyLeft | PTarget.AllyCenter | PTarget.AllyRight))
                                return false;
                            break;
                        case PMoveTarget.RandomFoeSurrounding:
                            break;
                        case PMoveTarget.Self:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyLeft)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (action.Targets != PTarget.AllyCenter)
                                    return false;
                            }
                            else
                            {
                                if (action.Targets != PTarget.AllyRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SelfOrAllySurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyLeft && action.Targets != PTarget.AllyCenter)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (action.Targets != PTarget.AllyLeft && action.Targets != PTarget.AllyCenter && action.Targets != PTarget.AllyRight)
                                    return false;
                            }
                            else
                            {
                                if (action.Targets != PTarget.AllyCenter && action.Targets != PTarget.AllyRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleAllySurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyCenter)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (action.Targets != PTarget.AllyLeft && action.Targets != PTarget.AllyRight)
                                    return false;
                            }
                            else
                            {
                                if (action.Targets != PTarget.AllyCenter)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleFoeSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.FoeCenter && action.Targets != PTarget.FoeRight)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (action.Targets != PTarget.FoeLeft && action.Targets != PTarget.FoeCenter && action.Targets != PTarget.FoeRight)
                                    return false;
                            }
                            else
                            {
                                if (action.Targets != PTarget.FoeLeft && action.Targets != PTarget.FoeCenter)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleNotSelf:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyCenter && action.Targets != PTarget.AllyRight && action.Targets != PTarget.FoeLeft && action.Targets != PTarget.FoeCenter && action.Targets != PTarget.FoeRight)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (action.Targets != PTarget.AllyLeft && action.Targets != PTarget.AllyRight && action.Targets != PTarget.FoeLeft && action.Targets != PTarget.FoeCenter && action.Targets != PTarget.FoeRight)
                                    return false;
                            }
                            else
                            {
                                if (action.Targets != PTarget.AllyLeft && action.Targets != PTarget.AllyCenter && action.Targets != PTarget.FoeLeft && action.Targets != PTarget.FoeCenter && action.Targets != PTarget.FoeRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyCenter && action.Targets != PTarget.FoeCenter && action.Targets != PTarget.FoeRight)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (action.Targets != PTarget.AllyLeft && action.Targets != PTarget.AllyRight && action.Targets != PTarget.FoeLeft && action.Targets != PTarget.FoeCenter && action.Targets != PTarget.FoeRight)
                                    return false;
                            }
                            else
                            {
                                if (action.Targets != PTarget.AllyCenter && action.Targets != PTarget.FoeLeft && action.Targets != PTarget.FoeCenter)
                                    return false;
                            }
                            break;
                    }
                    break;
            }

            return true;
        }
        // Returns true if the action was valid (and was selected)
        public bool SelectActionIfValid(PAction action)
        {
            if (IsActionValid(action))
            {
                SelectAction(action);
                return true;
            }
            return false;
        }
        void SelectAction(PAction action)
        {
            PBattlePokemon pkmn = Battler(action.PokemonId);
            PTeam attackerTeam = teams[pkmn.Mon.LocallyOwned ? 0 : 1]; // Attacker's team
            PTeam opposingTeam = teams[pkmn.Mon.LocallyOwned ? 1 : 0]; // Other team
            // TODO:
            // if (action == PAction.Fight)
            pkmn.SelectedAction = action;

            // Choose target if move selects a random target naturally
            if (PMoveData.Data[pkmn.SelectedAction.Move].Targets == PMoveTarget.RandomFoeSurrounding)
            {
                switch (BattleStyle)
                {
                    case PBattleStyle.Single:
                    case PBattleStyle.Rotation:
                        action.Targets = PTarget.FoeCenter;
                        break;
                    case PBattleStyle.Double:
                        action.Targets = PUtils.RNG.Next(2) == 0 ? PTarget.FoeLeft : PTarget.FoeRight;
                        break;
                    case PBattleStyle.Triple:
                        if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                        {
                            // If one is fainted, BattleEffects.cs->UseMove() will change the target
                            action.Targets = PUtils.RNG.Next(2) == 0 ? PTarget.FoeCenter : PTarget.FoeRight;
                        }
                        else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                        {
                            // Keep randomly picking until a non-fainted foe is selected
                            int r;
                            roll:
                            r = PUtils.RNG.Next(3);
                            // Prioritize left
                            if (r == 0)
                            {
                                if (opposingTeam.BattlerAtPosition(PFieldPosition.Left) != null)
                                    action.Targets = PTarget.FoeLeft;
                                else
                                    goto roll;
                            }
                            // Prioritize center
                            else if (r == 1)
                            {
                                if (opposingTeam.BattlerAtPosition(PFieldPosition.Center) != null)
                                    action.Targets = PTarget.FoeCenter;
                                else
                                    goto roll;
                            }
                            // Prioritize right
                            else
                            {
                                if (opposingTeam.BattlerAtPosition(PFieldPosition.Right) != null)
                                    action.Targets = PTarget.FoeRight;
                                else
                                    goto roll;
                            }
                        }
                        else
                        {
                            // If one is fainted, BattleEffects.cs->UseMove() will change the target
                            action.Targets = PUtils.RNG.Next(2) == 0 ? PTarget.FoeLeft : PTarget.FoeCenter;
                        }
                        break;
                }
            }
        }
    }
}
