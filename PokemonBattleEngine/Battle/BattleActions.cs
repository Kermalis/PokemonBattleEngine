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
    public struct PAction
    {
        [FieldOffset(4)]
        public Guid PokemonId;
        [FieldOffset(3)]
        public PDecision Decision; // TODO (Switch, forfeit)
        [FieldOffset(0)]
        public PMove Move;
        [FieldOffset(2)]
        public PTarget Targets;

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(PokemonId.ToByteArray());
            bytes.Add((byte)Decision);
            bytes.AddRange(BitConverter.GetBytes((ushort)Move));
            bytes.Add((byte)Targets);
            return bytes.ToArray();
        }
        internal static PAction FromBytes(BinaryReader r)
        {
            return new PAction
            {
                PokemonId = new Guid(r.ReadBytes(16)),
                Decision = (PDecision)r.ReadByte(),
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

            PPokemon pkmn = PKnownInfo.Instance.Pokemon(action.PokemonId);

            // Not on the field
            if (pkmn.FieldPosition == PFieldPosition.None)
                return false;

            // Invalid move
            if (!pkmn.Shell.Moves.Contains(action.Move) || action.Move == PMove.None)
                return false;

            // TODO: Struggle

            // Out of PP
            if (pkmn.PP[Array.IndexOf(pkmn.Shell.Moves, action.Move)] < 1)
                return false;

            // Verify targets
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
                        case PMoveTarget.AllFoesSurrounding:
                        case PMoveTarget.AllSurrounding:
                        case PMoveTarget.SingleFoeSurrounding:
                        case PMoveTarget.SingleNotSelf:
                        case PMoveTarget.SingleSurrounding:
                            if (action.Targets != PTarget.FoeCenter)
                                return false;
                            break;
                        case PMoveTarget.Self:
                        case PMoveTarget.SelfOrAllySurrounding:
                            if (action.Targets != PTarget.AllyCenter)
                                return false;
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
                        case PMoveTarget.AllFoesSurrounding:
                            if (action.Targets != (PTarget.FoeLeft | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllSurrounding:
                            if (pkmn.FieldPosition == PFieldPosition.Left)
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
                        case PMoveTarget.Self:
                            if (pkmn.FieldPosition == PFieldPosition.Left)
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
                            if (pkmn.FieldPosition == PFieldPosition.Left)
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
                            if (pkmn.FieldPosition == PFieldPosition.Left)
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
                        case PMoveTarget.AllFoesSurrounding:
                            if (pkmn.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != (PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            else if (pkmn.FieldPosition == PFieldPosition.Center)
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
                            if (pkmn.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != (PTarget.AllyCenter | PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            else if (pkmn.FieldPosition == PFieldPosition.Center)
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
                        case PMoveTarget.Self:
                            if (pkmn.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyLeft)
                                    return false;
                            }
                            else if (pkmn.FieldPosition == PFieldPosition.Center)
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
                            if (pkmn.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyLeft && action.Targets != PTarget.AllyCenter)
                                    return false;
                            }
                            else if (pkmn.FieldPosition == PFieldPosition.Center)
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
                            if (pkmn.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyCenter)
                                    return false;
                            }
                            else if (pkmn.FieldPosition == PFieldPosition.Center)
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
                            if (pkmn.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.FoeCenter && action.Targets != PTarget.FoeRight)
                                    return false;
                            }
                            else if (pkmn.FieldPosition == PFieldPosition.Center)
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
                            if (pkmn.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyCenter && action.Targets != PTarget.AllyRight && action.Targets != PTarget.FoeLeft && action.Targets != PTarget.FoeCenter && action.Targets != PTarget.FoeRight)
                                    return false;
                            }
                            else if (pkmn.FieldPosition == PFieldPosition.Center)
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
                            if (pkmn.FieldPosition == PFieldPosition.Left)
                            {
                                if (action.Targets != PTarget.AllyCenter && action.Targets != PTarget.FoeCenter && action.Targets != PTarget.FoeRight)
                                    return false;
                            }
                            else if (pkmn.FieldPosition == PFieldPosition.Center)
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
            PPokemon pkmn = PKnownInfo.Instance.Pokemon(action.PokemonId);
            // TODO:
            // if (action == PDecision.Fight)

            switch (PMoveData.Data[action.Move].Targets)
            {
                // These two are not supposed to activate 2/3 times for double/triple battles, they affect the team itself
                case PMoveTarget.AllFoes:
                case PMoveTarget.AllTeam:
                    if (pkmn.FieldPosition == PFieldPosition.Left)
                        action.Targets = PTarget.AllyLeft;
                    else if (pkmn.FieldPosition == PFieldPosition.Center)
                        action.Targets = PTarget.AllyCenter;
                    else
                        action.Targets = PTarget.AllyRight;
                    break;
                case PMoveTarget.RandomFoeSurrounding:
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
                            if (pkmn.FieldPosition == PFieldPosition.Left)
                            {
                                // If one is fainted, BattleEffects.cs->UseMove() will change the target
                                action.Targets = PUtils.RNG.Next(2) == 0 ? PTarget.FoeCenter : PTarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PFieldPosition.Center)
                            {
                                PTeam opposingTeam = teams[pkmn.Local ? 1 : 0]; // Other team
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
                    break;
                case PMoveTarget.SingleAllySurrounding:
                    if (BattleStyle == PBattleStyle.Single || BattleStyle == PBattleStyle.Rotation)
                        action.Targets = PTarget.AllyCenter;
                    break;
            }
            pkmn.Action = action;
        }
    }
}
