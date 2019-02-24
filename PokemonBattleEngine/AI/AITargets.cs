using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.AI
{
    public static partial class PBEAIManager
    {
        static PBETarget DecideTargets(PBEPokemon pkmn, PBEMove move)
        {
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
                                        return PBETarget.AllyCenter | PBETarget.FoeCenter;
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
                                        return PBETarget.FoeCenter;
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
                                        return PBETarget.AllyCenter;
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
                                        return PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight;
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
                                        return PBETarget.FoeLeft | PBETarget.FoeRight;
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
                                        return PBETarget.AllyLeft | PBETarget.AllyRight;
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
                                        return PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return PBETarget.AllyLeft | PBETarget.FoeLeft | PBETarget.FoeRight;
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
                                        return PBETarget.AllyLeft;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return PBETarget.AllyRight;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SelfOrAllySurrounding: // TODO
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        if (PBEUtils.RNG.NextBoolean())
                                        {
                                            return PBETarget.AllyLeft;
                                        }
                                        else
                                        {
                                            return PBETarget.AllyRight;
                                        }
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
                                        return PBETarget.AllyRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return PBETarget.AllyLeft;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleFoeSurrounding: // TODO
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        if (PBEUtils.RNG.NextBoolean())
                                        {
                                            return PBETarget.FoeLeft;
                                        }
                                        else
                                        {
                                            return PBETarget.FoeRight;
                                        }
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleNotSelf:
                            case PBEMoveTarget.SingleSurrounding: // TODO
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        int val = PBEUtils.RNG.Next(0, 3);
                                        if (val == 0)
                                        {
                                            return PBETarget.AllyRight;
                                        }
                                        else if (val == 1)
                                        {
                                            return PBETarget.FoeLeft;
                                        }
                                        else
                                        {
                                            return PBETarget.FoeRight;
                                        }
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        int val = PBEUtils.RNG.Next(0, 3);
                                        if (val == 0)
                                        {
                                            return PBETarget.AllyLeft;
                                        }
                                        else if (val == 1)
                                        {
                                            return PBETarget.FoeLeft;
                                        }
                                        else
                                        {
                                            return PBETarget.FoeRight;
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
                                        return PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
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
                                        return PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
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
                                        return PBETarget.FoeCenter | PBETarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return PBETarget.FoeLeft | PBETarget.FoeCenter;
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
                                        return PBETarget.AllyCenter | PBETarget.FoeCenter | PBETarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return PBETarget.AllyCenter | PBETarget.FoeLeft | PBETarget.FoeCenter;
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
                                        return PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight;
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
                                        return PBETarget.AllyLeft;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return PBETarget.AllyCenter;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return PBETarget.AllyRight;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SelfOrAllySurrounding: // TODO
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        if (PBEUtils.RNG.NextBoolean())
                                        {
                                            return PBETarget.AllyLeft;
                                        }
                                        else
                                        {
                                            return PBETarget.AllyCenter;
                                        }
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        int val = PBEUtils.RNG.Next(0, 3);
                                        if (val == 0)
                                        {
                                            return PBETarget.AllyLeft;
                                        }
                                        else if (val == 1)
                                        {
                                            return PBETarget.AllyCenter;
                                        }
                                        else
                                        {
                                            return PBETarget.AllyRight;
                                        }
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        if (PBEUtils.RNG.NextBoolean())
                                        {
                                            return PBETarget.AllyCenter;
                                        }
                                        else
                                        {
                                            return PBETarget.AllyRight;
                                        }
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleAllySurrounding: // TODO
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return PBETarget.AllyCenter;
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        if (PBEUtils.RNG.NextBoolean())
                                        {
                                            return PBETarget.AllyLeft;
                                        }
                                        else
                                        {
                                            return PBETarget.AllyRight;
                                        }
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return PBETarget.AllyCenter;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleFoeSurrounding: // TODO
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        if (PBEUtils.RNG.NextBoolean())
                                        {
                                            return PBETarget.FoeCenter;
                                        }
                                        else
                                        {
                                            return PBETarget.FoeRight;
                                        }
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        int val = PBEUtils.RNG.Next(0, 3);
                                        if (val == 0)
                                        {
                                            return PBETarget.FoeLeft;
                                        }
                                        else if (val == 1)
                                        {
                                            return PBETarget.FoeCenter;
                                        }
                                        else
                                        {
                                            return PBETarget.FoeRight;
                                        }
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        if (PBEUtils.RNG.NextBoolean())
                                        {
                                            return PBETarget.FoeLeft;
                                        }
                                        else
                                        {
                                            return PBETarget.FoeCenter;
                                        }
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleNotSelf: // TODO
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        int val = PBEUtils.RNG.Next(0, 5);
                                        if (val == 0)
                                        {
                                            return PBETarget.AllyCenter;
                                        }
                                        else if (val == 1)
                                        {
                                            return PBETarget.AllyRight;
                                        }
                                        else if (val == 2)
                                        {
                                            return PBETarget.FoeLeft;
                                        }
                                        else if (val == 3)
                                        {
                                            return PBETarget.FoeCenter;
                                        }
                                        else
                                        {
                                            return PBETarget.FoeRight;
                                        }
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        int val = PBEUtils.RNG.Next(0, 5);
                                        if (val == 0)
                                        {
                                            return PBETarget.AllyLeft;
                                        }
                                        else if (val == 1)
                                        {
                                            return PBETarget.AllyRight;
                                        }
                                        else if (val == 2)
                                        {
                                            return PBETarget.FoeLeft;
                                        }
                                        else if (val == 3)
                                        {
                                            return PBETarget.FoeCenter;
                                        }
                                        else
                                        {
                                            return PBETarget.FoeRight;
                                        }
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        int val = PBEUtils.RNG.Next(0, 5);
                                        if (val == 0)
                                        {
                                            return PBETarget.AllyLeft;
                                        }
                                        else if (val == 1)
                                        {
                                            return PBETarget.AllyCenter;
                                        }
                                        else if (val == 2)
                                        {
                                            return PBETarget.FoeLeft;
                                        }
                                        else if (val == 3)
                                        {
                                            return PBETarget.FoeCenter;
                                        }
                                        else
                                        {
                                            return PBETarget.FoeRight;
                                        }
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.SingleSurrounding: // TODO
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        int val = PBEUtils.RNG.Next(0, 3);
                                        if (val == 0)
                                        {
                                            return PBETarget.AllyCenter;
                                        }
                                        else if (val == 1)
                                        {
                                            return PBETarget.FoeCenter;
                                        }
                                        else
                                        {
                                            return PBETarget.FoeRight;
                                        }
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        int val = PBEUtils.RNG.Next(0, 5);
                                        if (val == 0)
                                        {
                                            return PBETarget.AllyLeft;
                                        }
                                        else if (val == 1)
                                        {
                                            return PBETarget.AllyRight;
                                        }
                                        else if (val == 2)
                                        {
                                            return PBETarget.FoeLeft;
                                        }
                                        else if (val == 3)
                                        {
                                            return PBETarget.FoeCenter;
                                        }
                                        else
                                        {
                                            return PBETarget.FoeRight;
                                        }
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        int val = PBEUtils.RNG.Next(0, 3);
                                        if (val == 0)
                                        {
                                            return PBETarget.AllyCenter;
                                        }
                                        else if (val == 1)
                                        {
                                            return PBETarget.FoeLeft;
                                        }
                                        else
                                        {
                                            return PBETarget.FoeCenter;
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
                                        return PBETarget.AllyCenter | PBETarget.FoeCenter;
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
                                        return PBETarget.FoeCenter;
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
                                        return PBETarget.AllyCenter;
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
