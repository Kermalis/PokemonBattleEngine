using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;

namespace Kermalis.PokemonBattleEngine.AI
{
    public static partial class AIManager
    {
        static PBETarget DecideTargets(PBEBattle battle, PBEPokemon pkmn, PBEMove move)
        {
            PBEMoveTarget possibleTargets = PBEBattle.GetMoveTargetsForPokemon(pkmn, move);
            switch (battle.BattleFormat)
            {
                case PBEBattleFormat.Single:
                case PBEBattleFormat.Rotation:
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                            return PBETarget.AllyCenter | PBETarget.FoeCenter;
                        case PBEMoveTarget.AllFoes:
                        case PBEMoveTarget.AllFoesSurrounding:
                        case PBEMoveTarget.AllSurrounding:
                        case PBEMoveTarget.SingleFoeSurrounding:
                        case PBEMoveTarget.SingleNotSelf:
                        case PBEMoveTarget.SingleSurrounding:
                            return PBETarget.FoeCenter;
                        case PBEMoveTarget.AllTeam:
                        case PBEMoveTarget.Self:
                        case PBEMoveTarget.SelfOrAllySurrounding:
                            return PBETarget.AllyCenter;
                    }
                    break;
                case PBEBattleFormat.Double:
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                            return PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight;
                        case PBEMoveTarget.AllFoes:
                        case PBEMoveTarget.AllFoesSurrounding:
                            return PBETarget.FoeLeft | PBETarget.FoeRight;
                        case PBEMoveTarget.AllTeam:
                            return PBETarget.AllyLeft | PBETarget.AllyRight;
                        case PBEMoveTarget.AllSurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                return PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeRight;
                            }
                            else
                            {
                                return PBETarget.AllyLeft | PBETarget.FoeLeft | PBETarget.FoeRight;
                            }
                        case PBEMoveTarget.Self:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                return PBETarget.AllyLeft;
                            }
                            else
                            {
                                return PBETarget.AllyRight;
                            }
                        case PBEMoveTarget.SelfOrAllySurrounding: // TODO
                            if (PBEUtils.RNG.NextBoolean())
                            {
                                return PBETarget.AllyLeft;
                            }
                            else
                            {
                                return PBETarget.AllyRight;
                            }
                        case PBEMoveTarget.SingleAllySurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                return PBETarget.AllyRight;
                            }
                            else
                            {
                                return PBETarget.AllyLeft;
                            }
                        case PBEMoveTarget.SingleFoeSurrounding: // TODO
                            if (PBEUtils.RNG.NextBoolean())
                            {
                                return PBETarget.FoeLeft;
                            }
                            else
                            {
                                return PBETarget.FoeRight;
                            }
                        case PBEMoveTarget.SingleNotSelf:
                        case PBEMoveTarget.SingleSurrounding: // TODO
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
                            else
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
                    }
                    break;
                case PBEBattleFormat.Triple:
                    switch (possibleTargets)
                    {
                        case PBEMoveTarget.All:
                            return PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
                        case PBEMoveTarget.AllFoes:
                            return PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
                        case PBEMoveTarget.AllFoesSurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                return PBETarget.FoeCenter | PBETarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
                            }
                            else
                            {
                                return PBETarget.FoeLeft | PBETarget.FoeCenter;
                            }
                        case PBEMoveTarget.AllSurrounding:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                return PBETarget.AllyCenter | PBETarget.FoeCenter | PBETarget.FoeRight;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return PBETarget.AllyLeft | PBETarget.AllyRight | PBETarget.FoeLeft | PBETarget.FoeCenter | PBETarget.FoeRight;
                            }
                            else
                            {
                                return PBETarget.AllyCenter | PBETarget.FoeLeft | PBETarget.FoeCenter;
                            }
                        case PBEMoveTarget.AllTeam:
                            return PBETarget.AllyLeft | PBETarget.AllyCenter | PBETarget.AllyRight;
                        case PBEMoveTarget.Self:
                            if (pkmn.FieldPosition == PBEFieldPosition.Left)
                            {
                                return PBETarget.AllyLeft;
                            }
                            else if (pkmn.FieldPosition == PBEFieldPosition.Center)
                            {
                                return PBETarget.AllyCenter;
                            }
                            else
                            {
                                return PBETarget.AllyRight;
                            }
                        case PBEMoveTarget.SelfOrAllySurrounding: // TODO
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
                            else
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
                        case PBEMoveTarget.SingleAllySurrounding: // TODO
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
                            else
                            {
                                return PBETarget.AllyCenter;
                            }
                        case PBEMoveTarget.SingleFoeSurrounding: // TODO
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
                            else
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
                        case PBEMoveTarget.SingleNotSelf: // TODO
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
                            else
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
                        case PBEMoveTarget.SingleSurrounding: // TODO
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
                            else
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
                    }
                    break;
            }
            return PBETarget.None; // Moves like Outrage
        }
    }
}
