using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.Battle
{
    public static class PBEBattleUtils
    {
        public static PBETurnTarget GetSpreadMoveTargets(PBEPokemon pkmn, PBEMoveTarget targets)
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
                        default: throw new ArgumentOutOfRangeException(nameof(targets));
                    }
                }
                default: throw new ArgumentOutOfRangeException(nameof(pkmn.Team.Battle.BattleFormat));
            }
        }
        public static PBETurnTarget[] GetPossibleTargets(PBEPokemon pkmn, PBEMoveTarget targets)
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
                                return new PBETurnTarget[] { PBETurnTarget.FoeCenter };
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
                                return new PBETurnTarget[] { PBETurnTarget.AllyCenter };
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
                                return new PBETurnTarget[] { PBETurnTarget.AllyLeft };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.AllyRight };
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
                                return new PBETurnTarget[] { PBETurnTarget.AllyLeft, PBETurnTarget.AllyRight };
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
                                return new PBETurnTarget[] { PBETurnTarget.AllyRight };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.AllyLeft };
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
                                return new PBETurnTarget[] { PBETurnTarget.FoeLeft, PBETurnTarget.FoeRight };
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
                                return new PBETurnTarget[] { PBETurnTarget.AllyRight, PBETurnTarget.FoeLeft, PBETurnTarget.FoeRight };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.AllyLeft, PBETurnTarget.FoeLeft, PBETurnTarget.FoeRight };
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
                                return new PBETurnTarget[] { PBETurnTarget.AllyLeft };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.AllyCenter };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.AllyRight };
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
                                return new PBETurnTarget[] { PBETurnTarget.AllyLeft, PBETurnTarget.AllyCenter };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.AllyLeft, PBETurnTarget.AllyCenter, PBETurnTarget.AllyRight };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.AllyCenter, PBETurnTarget.AllyRight };
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
                                return new PBETurnTarget[] { PBETurnTarget.AllyCenter };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.AllyLeft, PBETurnTarget.AllyRight };
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
                                return new PBETurnTarget[] { PBETurnTarget.FoeCenter, PBETurnTarget.FoeRight };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.FoeLeft, PBETurnTarget.FoeCenter, PBETurnTarget.FoeRight };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.FoeLeft, PBETurnTarget.FoeCenter };
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
                                return new PBETurnTarget[] { PBETurnTarget.AllyCenter, PBETurnTarget.AllyRight, PBETurnTarget.FoeLeft, PBETurnTarget.FoeCenter, PBETurnTarget.FoeRight };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.AllyLeft, PBETurnTarget.AllyRight, PBETurnTarget.FoeLeft, PBETurnTarget.FoeCenter, PBETurnTarget.FoeRight };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.AllyLeft, PBETurnTarget.AllyCenter, PBETurnTarget.FoeLeft, PBETurnTarget.FoeCenter, PBETurnTarget.FoeRight };
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
                                return new PBETurnTarget[] { PBETurnTarget.AllyCenter, PBETurnTarget.FoeCenter, PBETurnTarget.FoeRight };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.AllyLeft, PBETurnTarget.AllyRight, PBETurnTarget.FoeLeft, PBETurnTarget.FoeCenter, PBETurnTarget.FoeRight };
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Right)
                            {
                                return new PBETurnTarget[] { PBETurnTarget.AllyCenter, PBETurnTarget.FoeLeft, PBETurnTarget.FoeCenter };
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
                                return new PBETurnTarget[] { PBETurnTarget.FoeCenter };
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
                                return new PBETurnTarget[] { PBETurnTarget.AllyCenter };
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
