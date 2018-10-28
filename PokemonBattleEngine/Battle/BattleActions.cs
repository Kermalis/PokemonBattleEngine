using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public sealed class PAction
    {
        public readonly Guid PokemonId;
        // TODO: Action (switch, forfeit, move)
        public readonly byte Param1, Param2;

        public PAction(Guid pkmnId, byte param1, byte param2)
        {
            PokemonId = pkmnId;
            Param1 = param1;
            Param2 = param2;
        }

        internal byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(PokemonId.ToByteArray());
            bytes.Add(Param1);
            bytes.Add(Param2);
            return bytes.ToArray();
        }
        internal static PAction FromBytes(BinaryReader r)
        {
            return new PAction(new Guid(r.ReadBytes(16)), r.ReadByte(), r.ReadByte());
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
            if (action.Param1 >= PConstants.NumMoves || pkmn.Mon.Shell.Moves[action.Param1] == PMove.None)
                return false;

            // TODO: Struggle

            // Out of PP
            if (pkmn.Mon.PP[action.Param1] < 1)
                return false;

            PMove move = pkmn.Mon.Shell.Moves[action.Param1];
            PMoveData mData = PMoveData.Data[move];
            PTarget targets = (PTarget)action.Param2;
            Console.WriteLine(pkmn.Mon.FieldPosition);
            Console.WriteLine(move);
            Console.WriteLine(targets);
            Console.ReadKey();
            switch (BattleStyle)
            {
                case PBattleStyle.Single:
                case PBattleStyle.Rotation:
                    // Only center in single battles & rotation battles
                    switch (mData.Targets)
                    {
                        case PMoveTarget.All:
                            if (targets != (PTarget.AllyCenter | PTarget.FoeCenter))
                                return false;
                            break;
                        case PMoveTarget.AllFoes:
                        case PMoveTarget.AllFoesSurrounding:
                        case PMoveTarget.AllSurrounding:
                        case PMoveTarget.SingleFoeSurrounding:
                        case PMoveTarget.SingleNotSelf:
                        case PMoveTarget.SingleSurrounding:
                            if (targets != PTarget.FoeCenter)
                                return false;
                            break;
                        case PMoveTarget.AllTeam:
                        case PMoveTarget.Self:
                        case PMoveTarget.SelfOrAllySurrounding:
                            if (targets != PTarget.AllyCenter)
                                return false;
                            break;
                        case PMoveTarget.RandomFoeSurrounding:
                        case PMoveTarget.SingleAllySurrounding:
                            // TODO: set the target
                            break;
                    }
                    break;
                case PBattleStyle.Double:
                    // No center in double battles
                    switch (mData.Targets)
                    {
                        case PMoveTarget.All:
                            if (targets != (PTarget.AllyLeft | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllFoes:
                        case PMoveTarget.AllFoesSurrounding:
                            if (targets != (PTarget.FoeLeft | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != (PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeRight))
                                    return false;
                            }
                            else
                            {
                                if (targets != (PTarget.AllyLeft | PTarget.FoeLeft | PTarget.FoeRight))
                                    return false;
                            }
                            break;
                        case PMoveTarget.AllTeam:
                            if (targets != (PTarget.AllyLeft | PTarget.AllyRight))
                                return false;
                            break;
                        case PMoveTarget.RandomFoeSurrounding:
                            // TODO: Set the target
                            break;
                        case PMoveTarget.Self:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyLeft)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SelfOrAllySurrounding:
                            if (targets != PTarget.AllyLeft && targets != PTarget.AllyRight)
                                return false;
                            break;
                        case PMoveTarget.SingleAllySurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyLeft)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleFoeSurrounding:
                            if (targets != PTarget.FoeLeft && targets != PTarget.FoeRight)
                                return false;
                            break;
                        case PMoveTarget.SingleNotSelf:
                        case PMoveTarget.SingleSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyRight && targets != PTarget.FoeLeft && targets != PTarget.FoeRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.FoeLeft && targets != PTarget.FoeRight)
                                    return false;
                            }
                            break;
                    }
                    break;
                case PBattleStyle.Triple:
                    switch (mData.Targets)
                    {
                        case PMoveTarget.All:
                            if (targets != (PTarget.AllyLeft | PTarget.AllyCenter | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllFoes:
                            if (targets != (PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                return false;
                            break;
                        case PMoveTarget.AllFoesSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != (PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != (PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            else
                            {
                                if (targets != (PTarget.FoeLeft | PTarget.FoeCenter))
                                    return false;
                            }
                            break;
                        case PMoveTarget.AllSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != (PTarget.AllyCenter | PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != (PTarget.AllyLeft | PTarget.AllyRight | PTarget.FoeLeft | PTarget.FoeCenter | PTarget.FoeRight))
                                    return false;
                            }
                            else
                            {
                                if (targets != (PTarget.AllyCenter | PTarget.FoeLeft | PTarget.FoeCenter))
                                    return false;
                            }
                            break;
                        case PMoveTarget.AllTeam:
                            if (targets != (PTarget.AllyLeft | PTarget.AllyCenter | PTarget.AllyRight))
                                return false;
                            break;
                        case PMoveTarget.RandomFoeSurrounding:
                            // TODO: set the target
                            break;
                        case PMoveTarget.Self:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyLeft)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != PTarget.AllyCenter)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SelfOrAllySurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.AllyCenter)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.AllyCenter && targets != PTarget.AllyRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyCenter && targets != PTarget.AllyRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleAllySurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyCenter)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.AllyRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyCenter)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleFoeSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != PTarget.FoeLeft && targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.FoeLeft && targets != PTarget.FoeCenter)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleNotSelf:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyCenter && targets != PTarget.AllyRight && targets != PTarget.FoeLeft && targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.AllyRight && targets != PTarget.FoeLeft && targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.AllyCenter && targets != PTarget.FoeLeft && targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            break;
                        case PMoveTarget.SingleSurrounding:
                            if (pkmn.Mon.FieldPosition == PFieldPosition.Left)
                            {
                                if (targets != PTarget.AllyCenter && targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            else if (pkmn.Mon.FieldPosition == PFieldPosition.Center)
                            {
                                if (targets != PTarget.AllyLeft && targets != PTarget.AllyRight && targets != PTarget.FoeLeft && targets != PTarget.FoeCenter && targets != PTarget.FoeRight)
                                    return false;
                            }
                            else
                            {
                                if (targets != PTarget.AllyCenter && targets != PTarget.FoeLeft && targets != PTarget.FoeCenter)
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
            // TODO
            // if (action == PAction.Fight)
            pkmn.SelectedAction = action;
        }
    }
}
