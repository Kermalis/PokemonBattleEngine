using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.AI
{
    public static partial class PBEAIManager
    {
        // TODO: Move these to battle targets and make them public, this file doesn't have to exist

        static PBETarget GetSpreadMoveTargets(PBEPokemon pkmn, PBEMoveTarget targets)
        {
            switch (pkmn.Team.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                    {
                        switch (targets)
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
                            default: throw new ArgumentOutOfRangeException(nameof(targets));
                        }
                    }
                case PBEBattleFormat.Double:
                    {
                        switch (targets)
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
                            default: throw new ArgumentOutOfRangeException(nameof(targets));
                        }
                    }
                case PBEBattleFormat.Triple:
                    {
                        switch (targets)
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
                            default: throw new ArgumentOutOfRangeException(nameof(targets));
                        }
                    }
                case PBEBattleFormat.Rotation:
                    {
                        switch (targets)
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
                            default: throw new ArgumentOutOfRangeException(nameof(targets));
                        }
                    }
                default: throw new ArgumentOutOfRangeException(nameof(pkmn.Team.Battle.BattleFormat));
            }
        }

        static PBETarget[] GetPossibleTargets(PBEPokemon pkmn, PBEMoveTarget targets)
        {
            switch (pkmn.Team.Battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                    {
                        switch (targets)
                        {
                            case PBEMoveTarget.SingleFoeSurrounding:
                            case PBEMoveTarget.SingleNotSelf:
                            case PBEMoveTarget.SingleSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return new PBETarget[] { PBETarget.FoeCenter };
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.RandomFoeSurrounding:
                            case PBEMoveTarget.Self:
                            case PBEMoveTarget.SelfOrAllySurrounding:
                            case PBEMoveTarget.SingleAllySurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return new PBETarget[] { PBETarget.AllyCenter };
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(targets));
                        }
                    }
                case PBEBattleFormat.Double:
                    {
                        switch (targets)
                        {
                            case PBEMoveTarget.RandomFoeSurrounding:
                            case PBEMoveTarget.Self:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return new PBETarget[] { PBETarget.AllyLeft };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return new PBETarget[] { PBETarget.AllyRight };
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
                                        return new PBETarget[] { PBETarget.AllyLeft, PBETarget.AllyRight };
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
                                        return new PBETarget[] { PBETarget.AllyRight };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return new PBETarget[] { PBETarget.AllyLeft };
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
                                        return new PBETarget[] { PBETarget.FoeLeft, PBETarget.FoeRight };
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
                                        return new PBETarget[] { PBETarget.AllyRight, PBETarget.FoeLeft, PBETarget.FoeRight };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return new PBETarget[] { PBETarget.AllyLeft, PBETarget.FoeLeft, PBETarget.FoeRight };
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(targets));
                        }
                    }
                case PBEBattleFormat.Triple:
                    {
                        switch (targets)
                        {
                            case PBEMoveTarget.RandomFoeSurrounding:
                            case PBEMoveTarget.Self:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left)
                                    {
                                        return new PBETarget[] { PBETarget.AllyLeft };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return new PBETarget[] { PBETarget.AllyCenter };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return new PBETarget[] { PBETarget.AllyRight };
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
                                        return new PBETarget[] { PBETarget.AllyLeft, PBETarget.AllyCenter };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return new PBETarget[] { PBETarget.AllyLeft, PBETarget.AllyCenter, PBETarget.AllyRight };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return new PBETarget[] { PBETarget.AllyCenter, PBETarget.AllyRight };
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
                                        return new PBETarget[] { PBETarget.AllyCenter };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return new PBETarget[] { PBETarget.AllyLeft, PBETarget.AllyRight };
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
                                        return new PBETarget[] { PBETarget.FoeCenter, PBETarget.FoeRight };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return new PBETarget[] { PBETarget.FoeLeft, PBETarget.FoeCenter, PBETarget.FoeRight };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return new PBETarget[] { PBETarget.FoeLeft, PBETarget.FoeCenter };
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
                                        return new PBETarget[] { PBETarget.AllyCenter, PBETarget.AllyRight, PBETarget.FoeLeft, PBETarget.FoeCenter, PBETarget.FoeRight };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return new PBETarget[] { PBETarget.AllyLeft, PBETarget.AllyRight, PBETarget.FoeLeft, PBETarget.FoeCenter, PBETarget.FoeRight };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return new PBETarget[] { PBETarget.AllyLeft, PBETarget.AllyCenter, PBETarget.FoeLeft, PBETarget.FoeCenter, PBETarget.FoeRight };
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
                                        return new PBETarget[] { PBETarget.AllyCenter, PBETarget.FoeCenter, PBETarget.FoeRight };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                                    {
                                        return new PBETarget[] { PBETarget.AllyLeft, PBETarget.AllyRight, PBETarget.FoeLeft, PBETarget.FoeCenter, PBETarget.FoeRight };
                                    }
                                    else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return new PBETarget[] { PBETarget.AllyCenter, PBETarget.FoeLeft, PBETarget.FoeCenter };
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(targets));
                        }
                    }
                case PBEBattleFormat.Rotation:
                    {
                        switch (targets)
                        {
                            case PBEMoveTarget.SingleFoeSurrounding:
                            case PBEMoveTarget.SingleNotSelf:
                            case PBEMoveTarget.SingleSurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return new PBETarget[] { PBETarget.FoeCenter };
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            case PBEMoveTarget.RandomFoeSurrounding:
                            case PBEMoveTarget.Self:
                            case PBEMoveTarget.SelfOrAllySurrounding:
                            case PBEMoveTarget.SingleAllySurrounding:
                                {
                                    if (pkmn.FieldPosition == PBEFieldPosition.Left || pkmn.FieldPosition == PBEFieldPosition.Center || pkmn.FieldPosition == PBEFieldPosition.Right)
                                    {
                                        return new PBETarget[] { PBETarget.AllyCenter };
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException(nameof(pkmn.FieldPosition));
                                    }
                                }
                            default: throw new ArgumentOutOfRangeException(nameof(targets));
                        }
                    }
                default: throw new ArgumentOutOfRangeException(nameof(pkmn.Team.Battle.BattleFormat));
            }
        }
    }
}
