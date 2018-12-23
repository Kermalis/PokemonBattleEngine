using System.Collections.Generic;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEMoveData
    {
        public PBEType Type;
        public PBEMoveCategory Category;
        public PBEMoveEffect Effect;
        public int EffectParam;
        public byte PPTier, Power, Accuracy; // 0 power or accuracy will show up as --
        public sbyte Priority;
        public PBEMoveFlag Flags;
        public PBEMoveTarget Targets;

        public static PBEType GetMoveTypeForPokemon(PBEPokemon pkmn, PBEMove move)
        {
            switch (move)
            {
                case PBEMove.None: return PBEType.Normal;
                case PBEMove.HiddenPower: return pkmn.GetHiddenPowerType();
                case PBEMove.TechnoBlast:
                    switch (pkmn.Item)
                    {
                        case PBEItem.BurnDrive: return PBEType.Fire;
                        case PBEItem.ChillDrive: return PBEType.Ice;
                        case PBEItem.DouseDrive: return PBEType.Water;
                        case PBEItem.ShockDrive: return PBEType.Electric;
                        default: return Data[PBEMove.TechnoBlast].Type;
                    }
                default: return Data[move].Type;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Type: {Type}");
            sb.AppendLine($"Category: {Category}");
            sb.AppendLine($"Effect: {Effect}");
            sb.AppendLine($"Effect Parameter: {EffectParam}");
            sb.AppendLine($"PP: {PPTier * PBESettings.DefaultSettings.PPMultiplier}");
            sb.AppendLine($"Power: {(Power == 0 ? "--" : Power.ToString())}");
            sb.AppendLine($"Accuracy: {(Accuracy == 0 ? "--" : Accuracy.ToString())}");
            sb.AppendLine($"Priority: {Priority}");
            sb.AppendLine($"Flags: {Flags}");
            sb.Append($"Targets: {Targets}");

            return sb.ToString();
        }

        public static IReadOnlyDictionary<PBEMove, PBEMoveData> Data { get; } = new Dictionary<PBEMove, PBEMoveData>()
        {
            {
                PBEMove.Acid,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.AcidArmor,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_DEF, EffectParam = +2,
                    PPTier = 8, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.AcidSpray,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2, EffectParam = 100,
                    PPTier = 4, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.AerialAce,
                new PBEMoveData
                {
                    Type = PBEType.Flying, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleNotSelf
                }
            },
            {
                PBEMove.Aeroblast,
                new PBEMoveData
                {
                    Type = PBEType.Flying, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 100, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleNotSelf
                }
            },
            {
                PBEMove.Agility,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_SPE, EffectParam = +2,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.AirCutter,
                new PBEMoveData
                {
                    Type = PBEType.Flying, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 5, Power = 55, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.AirSlash,
                new PBEMoveData
                {
                    Type = PBEType.Flying, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 4, Power = 75, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleNotSelf
                }
            },
            {
                PBEMove.Amnesia,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_SPDEF, EffectParam = +2,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.AncientPower,
                new PBEMoveData
                {
                    Type = PBEType.Rock, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1, EffectParam = 10,
                    PPTier = 1, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.AquaJet,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.AquaTail,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 90, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Astonish,
                new PBEMoveData
                {
                    Type = PBEType.Ghost, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 3, Power = 30, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.AttackOrder,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.AuraSphere,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 90, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleNotSelf
                }
            },
            {
                PBEMove.AuroraBeam,
                new PBEMoveData
                {
                    Type = PBEType.Ice, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_ATK_By1, EffectParam = 10,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Barrier,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_DEF, EffectParam = +2,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Bite,
                new PBEMoveData
                {
                    Type = PBEType.Dark, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 5, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.BlazeKick,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 2, Power = 85, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.BlueFlare,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 20,
                    PPTier = 1, Power = 130, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.BrickBreak,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.BrickBreak, EffectParam = 0,
                    PPTier = 3, Power = 75, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Brine,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.BodySlam,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 3, Power = 85, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.BoltStrike,
                new PBEMoveData
                {
                    Type = PBEType.Electric, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeParalyze, EffectParam = 20,
                    PPTier = 1, Power = 130, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.BoneClub,
                new PBEMoveData
                {
                    Type = PBEType.Ground, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 10,
                    PPTier = 4, Power = 65, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Bubble,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 10,
                    PPTier = 6, Power = 20, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.BubbleBeam,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 10,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.BugBuzz,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 2, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.SoundBased,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.BulkUp,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RaiseUser_ATK_DEF_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Bulldoze,
                new PBEMoveData
                {
                    Type = PBEType.Ground, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 100,
                    PPTier = 4, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllSurrounding
                }
            },
            {
                PBEMove.BulletPunch,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.CalmMind,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RaiseUser_SPATK_SPDEF_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.ChargeBeam,
                new PBEMoveData
                {
                    Type = PBEType.Electric, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeRaiseUser_SPATK_By1, EffectParam = 70,
                    PPTier = 2, Power = 50, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Charm,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_ATK, EffectParam = -2,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Chatter,
                new PBEMoveData
                {
                    Type = PBEType.Flying, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeConfuse, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.SoundBased,
                    Targets = PBEMoveTarget.SingleNotSelf
                }
            },
            {
                PBEMove.CloseCombat,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1, EffectParam = 100,
                    PPTier = 1, Power = 120, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Coil,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RaiseUser_ATK_DEF_ACC_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.ConfuseRay,
                new PBEMoveData
                {
                    Type = PBEType.Ghost, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Confuse, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Confusion,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeConfuse, EffectParam = 10,
                    PPTier = 5, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.CosmicPower,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RaiseUser_DEF_SPDEF_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.CottonGuard,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_DEF, EffectParam = +3,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Crabhammer,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 90, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.CrossChop,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 100, Accuracy = 80, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.CrossPoison,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybePoison, EffectParam = 10,
                    PPTier = 4, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Crunch,
                new PBEMoveData
                {
                    Type = PBEType.Dark, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1, EffectParam = 20,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.CrushClaw,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1, EffectParam = 50,
                    PPTier = 2, Power = 75, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Curse,
                new PBEMoveData
                {
                    Type = PBEType.Ghost, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Curse, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.None,
                    Targets = PBEMoveTarget.Varies
                }
            },
            {
                PBEMove.Cut,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 50, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.DarkPulse,
                new PBEMoveData
                {
                    Type = PBEType.Dark, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 20,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleNotSelf
                }
            },
            {
                PBEMove.DarkVoid,
                new PBEMoveData
                {
                    Type = PBEType.Dark, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 80, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.DefendOrder,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RaiseUser_DEF_SPDEF_By1, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Detect,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Protect, EffectParam = 0,
                    PPTier = 1, Power = 0, Accuracy = 0, Priority = +4,
                    Flags = PBEMoveFlag.None,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Dig,
                new PBEMoveData
                {
                    Type = PBEType.Ground, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Dig, EffectParam = 0,
                    PPTier = 2, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Discharge,
                new PBEMoveData
                {
                    Type = PBEType.Electric, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllSurrounding
                }
            },
            {
                PBEMove.Dive,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Dive, EffectParam = 0,
                    PPTier = 2, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.DizzyPunch,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeConfuse, EffectParam = 20,
                    PPTier = 2, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.DracoMeteor,
                new PBEMoveData
                {
                    Type = PBEType.Dragon, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2, EffectParam = 100,
                    PPTier = 1, Power = 140, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.DragonBreath,
                new PBEMoveData
                {
                    Type = PBEType.Dragon, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 4, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.DragonClaw,
                new PBEMoveData
                {
                    Type = PBEType.Dragon, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.DragonDance,
                new PBEMoveData
                {
                    Type = PBEType.Dragon, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RaiseUser_ATK_SPE_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.DragonPulse,
                new PBEMoveData
                {
                    Type = PBEType.Dragon, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleNotSelf
                }
            },
            {
                PBEMove.DragonRush,
                new PBEMoveData
                {
                    Type = PBEType.Dragon, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 20,
                    PPTier = 2, Power = 100, Accuracy = 75, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.DoubleTeam,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_EVA, EffectParam = +1,
                    PPTier = 3, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.DrillPeck,
                new PBEMoveData
                {
                    Type = PBEType.Flying, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleNotSelf
                }
            },
            {
                PBEMove.DrillRun,
                new PBEMoveData
                {
                    Type = PBEType.Ground, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 80, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.EarthPower,
                new PBEMoveData
                {
                    Type = PBEType.Ground, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 2, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Earthquake,
                new PBEMoveData
                {
                    Type = PBEType.Ground, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 100, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HitsUnderground,
                    Targets = PBEMoveTarget.AllSurrounding
                }
            },
            {
                PBEMove.EggBomb,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 100, Accuracy = 75, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Electroweb,
                new PBEMoveData
                {
                    Type = PBEType.Electric, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 100,
                    PPTier = 3, Power = 55, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.Ember,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 5, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Endeavor,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Endeavor, EffectParam = 0,
                    PPTier = 1, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.EnergyBall,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 2, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Eruption,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 150, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.Extrasensory,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 10,
                    PPTier = 6, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ExtremeSpeed,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 80, Accuracy = 100, Priority = +2,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Facade,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.FaintAttack,
                new PBEMoveData
                {
                    Type = PBEType.Dark, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.FakeTears,
                new PBEMoveData
                {
                    Type = PBEType.Dark, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_SPDEF, EffectParam = -2,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.FeatherDance,
                new PBEMoveData
                {
                    Type = PBEType.Flying, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_ATK, EffectParam = -2,
                    PPTier = 3, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.FieryDance,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeRaiseUser_SPATK_By1, EffectParam = 50,
                    PPTier = 2, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.FireBlast,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 1, Power = 120, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.FirePunch,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 3, Power = 75, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.FlameCharge,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeRaiseUser_SPE_By1, EffectParam = 100,
                    PPTier = 4, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Flamethrower,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 3, Power = 95, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.FlameWheel,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 5, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.DefrostsUser,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Flash,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_ACC, EffectParam = -1,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.FlashCannon,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 2, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Flatter,
                new PBEMoveData
                {
                    Type = PBEType.Dark, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Flatter, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Fly,
                new PBEMoveData
                {
                    Type = PBEType.Flying, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Fly, EffectParam = 0,
                    PPTier = 3, Power = 90, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleNotSelf
                }
            },
            {
                PBEMove.FocusBlast,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 1, Power = 120, Accuracy = 70, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.FocusEnergy,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.FocusEnergy, EffectParam = 0,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.ForcePalm,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 2, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.FrostBreath,
                new PBEMoveData
                {
                    Type = PBEType.Ice, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 40, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AlwaysCrit,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Frustration,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Glaciate,
                new PBEMoveData
                {
                    Type = PBEType.Ice, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 100,
                    PPTier = 2, Power = 65, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.Glare,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Paralyze, EffectParam = 0,
                    PPTier = 6, Power = 0, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.GrassKnot,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.GrassWhistle,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 55, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.SoundBased,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Growl,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_ATK, EffectParam = -1,
                    PPTier = 8, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.SoundBased,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.Growth,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Growth, EffectParam = 0,
                    PPTier = 8, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.GunkShot,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybePoison, EffectParam = 30,
                    PPTier = 1, Power = 120, Accuracy = 70, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Hail,
                new PBEMoveData
                {
                    Type = PBEType.Ice, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Hail, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.None,
                    Targets = PBEMoveTarget.All
                }
            },
            {
                PBEMove.HammerArm,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeLowerUser_SPE_By1, EffectParam = 100,
                    PPTier = 2, Power = 100, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Harden,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_DEF, EffectParam = +1,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Headbutt,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 3, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.HealOrder,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RestoreUserHealth, EffectParam = 50,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.HeartStamp,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 5, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.HeatCrash,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.HeatWave,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 2, Power = 100, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.HeavySlam,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Hex,
                new PBEMoveData
                {
                    Type = PBEType.Ghost, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.HiddenPower,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.HoneClaws,
                new PBEMoveData
                {
                    Type = PBEType.Dark, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RaiseUser_ATK_ACC_By1, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.HornAttack,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 5, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Howl,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_ATK, EffectParam = +1,
                    PPTier = 8, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.HydroPump,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 120, Accuracy = 80, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.HyperFang,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 10,
                    PPTier = 3, Power = 80, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.HyperVoice,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.SoundBased,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.Hypnosis,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 60, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.IceBeam,
                new PBEMoveData
                {
                    Type = PBEType.Ice, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeFreeze, EffectParam = 10,
                    PPTier = 2, Power = 95, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.IcePunch,
                new PBEMoveData
                {
                    Type = PBEType.Ice, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFreeze, EffectParam = 10,
                    PPTier = 3, Power = 75, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.IceShard,
                new PBEMoveData
                {
                    Type = PBEType.Ice, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.IcicleCrash,
                new PBEMoveData
                {
                    Type = PBEType.Ice, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 2, Power = 85, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.IcyWind,
                new PBEMoveData
                {
                    Type = PBEType.Ice, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 100,
                    PPTier = 3, Power = 55, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.Inferno,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 100,
                    PPTier = 1, Power = 10, Accuracy = 50, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.IronDefense,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_DEF, EffectParam = +2,
                    PPTier = 3, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.IronHead,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.IronTail,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1, EffectParam = 30,
                    PPTier = 3, Power = 100, Accuracy = 75, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.KarateChop,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 5, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Kinesis,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_ACC, EffectParam = -1,
                    PPTier = 3, Power = 0, Accuracy = 80, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.LavaPlume,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 30,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllSurrounding
                }
            },
            {
                PBEMove.LeafBlade,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.LeafStorm,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2, EffectParam = 100,
                    PPTier = 1, Power = 140, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.LeafTornado,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 50,
                    PPTier = 2, Power = 65, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.LeechSeed,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.LeechSeed, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Leer,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_DEF, EffectParam = -1,
                    PPTier = 6, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.Lick,
                new PBEMoveData
                {
                    Type = PBEType.Ghost, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 6, Power = 20, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.LightScreen,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.LightScreen, EffectParam = 0,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.AllTeam
                }
            },
            {
                PBEMove.LovelyKiss,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 75, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.LowKick,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.LowSweep,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 100,
                    PPTier = 4, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.LusterPurge,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 50,
                    PPTier = 1, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.MachPunch,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.MagicalLeaf,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.MagnetBomb,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Magnitude,
                new PBEMoveData
                {
                    Type = PBEType.Ground, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Magnitude, EffectParam = 0,
                    PPTier = 6, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HitsUnderground,
                    Targets = PBEMoveTarget.AllSurrounding
                }
            },
            {
                PBEMove.Meditate,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_ATK, EffectParam = +1,
                    PPTier = 8, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Megahorn,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 120, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.MegaKick,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 120, Accuracy = 75, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.MegaPunch,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 80, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.MetalClaw,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeRaiseUser_ATK_By1, EffectParam = 10,
                    PPTier = 7, Power = 50, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.MetalSound,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_SPDEF, EffectParam = -2,
                    PPTier = 8, Power = 0, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.SoundBased,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.MeteorMash,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeRaiseUser_ATK_By1, EffectParam = 20,
                    PPTier = 2, Power = 100, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.MilkDrink,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RestoreUserHealth, EffectParam = 50,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Minimize,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Minimize, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.MirrorShot,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 30,
                    PPTier = 2, Power = 65, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.MistBall,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1, EffectParam = 50,
                    PPTier = 1, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Moonlight,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Moonlight, EffectParam = 0,
                    PPTier = 1, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.MorningSun,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Moonlight, EffectParam = 0,
                    PPTier = 1, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.MudBomb,
                new PBEMoveData
                {
                    Type = PBEType.Ground, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 30,
                    PPTier = 2, Power = 65, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.MuddyWater,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 30,
                    PPTier = 2, Power = 95, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.MudSlap,
                new PBEMoveData
                {
                    Type = PBEType.Ground, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 100,
                    PPTier = 2, Power = 20, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.MudShot,
                new PBEMoveData
                {
                    Type = PBEType.Ground, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 100,
                    PPTier = 3, Power = 55, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.NastyPlot,
                new PBEMoveData
                {
                    Type = PBEType.Dark, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_SPATK, EffectParam = +2,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.NeedleArm,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 3, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.NightDaze,
                new PBEMoveData
                {
                    Type = PBEType.Dark, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 40,
                    PPTier = 2, Power = 85, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.NightSlash,
                new PBEMoveData
                {
                    Type = PBEType.Dark, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Octazooka,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 50,
                    PPTier = 2, Power = 65, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.OminousWind,
                new PBEMoveData
                {
                    Type = PBEType.Ghost, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1, EffectParam = 10,
                    PPTier = 1, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Overheat,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2, EffectParam = 100,
                    PPTier = 1, Power = 140, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.PainSplit,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.PainSplit, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Peck,
                new PBEMoveData
                {
                    Type = PBEType.Flying, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 7, Power = 35, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleNotSelf
                }
            },
            {
                PBEMove.PoisonFang,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeToxic, EffectParam = 30,
                    PPTier = 3, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.PoisonGas,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Poison, EffectParam = 0,
                    PPTier = 8, Power = 0, Accuracy = 80, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.PoisonJab,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybePoison, EffectParam = 30,
                    PPTier = 4, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.PoisonPowder,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Poison, EffectParam = 0,
                    PPTier = 7, Power = 0, Accuracy = 75, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.PoisonSting,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybePoison, EffectParam = 30,
                    PPTier = 7, Power = 15, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.PoisonTail,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybePoison, EffectParam = 10,
                    PPTier = 5, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Pound,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 7, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.PowderSnow,
                new PBEMoveData
                {
                    Type = PBEType.Ice, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeFreeze, EffectParam = 10,
                    PPTier = 5, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.PowerGem,
                new PBEMoveData
                {
                    Type = PBEType.Rock, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.PowerWhip,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 120, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Protect,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Protect, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = +4,
                    Flags = PBEMoveFlag.None,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Psybeam,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeConfuse, EffectParam = 10,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Psychic,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 2, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.PsychoBoost,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2, EffectParam = 0,
                    PPTier = 1, Power = 140, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.PsychoCut,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.PsychUp,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.PsychUp, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.None,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Psyshock,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Psystrike,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 100, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.QuickAttack,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.QuiverDance,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RaiseUser_SPATK_SPDEF_SPE_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.RainDance,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RainDance, EffectParam = 0,
                    PPTier = 1, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.None,
                    Targets = PBEMoveTarget.All
                }
            },
            {
                PBEMove.RazorLeaf,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 5, Power = 55, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.RazorShell,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1, EffectParam = 50,
                    PPTier = 2, Power = 75, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Recover,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RestoreUserHealth, EffectParam = 50,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Reflect,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Reflect, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.AllTeam
                }
            },
            {
                PBEMove.Retaliate,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Return,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.RockClimb,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeConfuse, EffectParam = 20,
                    PPTier = 4, Power = 90, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.RockPolish,
                new PBEMoveData
                {
                    Type = PBEType.Rock, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_SPE, EffectParam = +2,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.RockSlide,
                new PBEMoveData
                {
                    Type = PBEType.Rock, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 2, Power = 75, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.RockSmash,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.RockThrow,
                new PBEMoveData
                {
                    Type = PBEType.Rock, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 50, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.RockTomb,
                new PBEMoveData
                {
                    Type = PBEType.Rock, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 100,
                    PPTier = 2, Power = 50, Accuracy = 80, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SacredFire,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 50,
                    PPTier = 1, Power = 100, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.DefrostsUser,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SandAttack,
                new PBEMoveData
                {
                    Type = PBEType.Ground, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_ACC, EffectParam = -1,
                    PPTier = 3, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Sandstorm,
                new PBEMoveData
                {
                    Type = PBEType.Rock, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Sandstorm, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.None,
                    Targets = PBEMoveTarget.All
                }
            },
            {
                PBEMove.Scald,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 30,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.DefrostsUser,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ScaryFace,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_SPE, EffectParam = -2,
                    PPTier = 2, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Scratch,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 8, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Screech,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_DEF, EffectParam = -2,
                    PPTier = 8, Power = 0, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.SoundBased,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SearingShot,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeBurn, EffectParam = 30,
                    PPTier = 1, Power = 100, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SecretSword,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 85, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SeedBomb,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SeedFlare,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2, EffectParam = 40,
                    PPTier = 1, Power = 120, Accuracy = 85, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ShadowBall,
                new PBEMoveData
                {
                    Type = PBEType.Ghost, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 20,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ShadowClaw,
                new PBEMoveData
                {
                    Type = PBEType.Ghost, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ShadowPunch,
                new PBEMoveData
                {
                    Type = PBEType.Ghost, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ShadowSneak,
                new PBEMoveData
                {
                    Type = PBEType.Ghost, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Sharpen,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_ATK, EffectParam = +1,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.ShellSmash,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.ShiftGear,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RaiseUser_SPE_By2_ATK_By1, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.SignalBeam,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeConfuse, EffectParam = 10,
                    PPTier = 3, Power = 75, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SilverWind,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1, EffectParam = 10,
                    PPTier = 1, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Sing,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 55, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.SoundBased,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ShockWave,
                new PBEMoveData
                {
                    Type = PBEType.Electric, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SkyUppercut,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 85, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HitsAirborne,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SlackOff,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RestoreUserHealth, EffectParam = 50,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Slam,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 80, Accuracy = 75, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Slash,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SleepPowder,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 75, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Sludge,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybePoison, EffectParam = 30,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SludgeBomb,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybePoison, EffectParam = 30,
                    PPTier = 2, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SludgeWave,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybePoison, EffectParam = 10,
                    PPTier = 2, Power = 95, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllSurrounding
                }
            },
            {
                PBEMove.Smog,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybePoison, EffectParam = 40,
                    PPTier = 4, Power = 20, Accuracy = 70, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SmokeScreen,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_ACC, EffectParam = -1,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Snarl,
                new PBEMoveData
                {
                    Type = PBEType.Dark, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1, EffectParam = 100,
                    PPTier = 3, Power = 55, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.SoundBased,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.Softboiled,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RestoreUserHealth, EffectParam = 50,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.SpacialRend,
                new PBEMoveData
                {
                    Type = PBEType.Dragon, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 100, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Spark,
                new PBEMoveData
                {
                    Type = PBEType.Electric, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Spikes,
                new PBEMoveData
                {
                    Type = PBEType.Ground, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Spikes, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByMagicCoat,
                    Targets = PBEMoveTarget.AllFoes
                }
            },
            {
                PBEMove.Spore,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.StealthRock,
                new PBEMoveData
                {
                    Type = PBEType.Rock, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.StealthRock, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByMagicCoat,
                    Targets = PBEMoveTarget.AllFoes
                }
            },
            {
                PBEMove.Steamroller,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SteelWing,
                new PBEMoveData
                {
                    Type = PBEType.Steel, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeRaiseUser_DEF_By1, EffectParam = 10,
                    PPTier = 5, Power = 70, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Stomp,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.StoneEdge,
                new PBEMoveData
                {
                    Type = PBEType.Rock, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 100, Accuracy = 80, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HighCritChance,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.StormThrow,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AlwaysCrit,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Strength,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 80, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.StringShot,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_SPE, EffectParam = -1,
                    PPTier = 8, Power = 0, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.StruggleBug,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1, EffectParam = 100,
                    PPTier = 4, Power = 30, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.StunSpore,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Paralyze, EffectParam = 0,
                    PPTier = 6, Power = 0, Accuracy = 75, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Substitute,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Substitute, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.SunnyDay,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.SunnyDay, EffectParam = 0,
                    PPTier = 1, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.None,
                    Targets = PBEMoveTarget.All
                }
            },
            {
                PBEMove.Superpower,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeLowerUser_ATK_DEF_By1, EffectParam = 100,
                    PPTier = 1, Power = 120, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Supersonic,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Confuse, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 55, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.SoundBased,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Surf,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 95, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HitsUnderwater,
                    Targets = PBEMoveTarget.AllSurrounding
                }
            },
            {
                PBEMove.Swagger,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Swagger, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SweetKiss,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Confuse, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 75, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.SweetScent,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_EVA, EffectParam = -1,
                    PPTier = 5, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.Swift,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.SwordsDance,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_ATK, EffectParam = +2,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Synthesis,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Moonlight, EffectParam = 0,
                    PPTier = 1, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Tackle,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 7, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.TailGlow,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_SPATK, EffectParam = +3,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.TailWhip,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeTarget_DEF, EffectParam = -1,
                    PPTier = 6, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.TechnoBlast,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 85, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.TeeterDance,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Confuse, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllSurrounding
                }
            },
            {
                PBEMove.Teleport,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Fail, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.None,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.Tickle,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.LowerTarget_ATK_DEF_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Thunder,
                new PBEMoveData
                {
                    Type = PBEType.Electric, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 2, Power = 120, Accuracy = 70, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.HitsAirborne,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Thunderbolt,
                new PBEMoveData
                {
                    Type = PBEType.Electric, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeParalyze, EffectParam = 10,
                    PPTier = 3, Power = 95, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ThunderPunch,
                new PBEMoveData
                {
                    Type = PBEType.Electric, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeParalyze, EffectParam = 10,
                    PPTier = 3, Power = 75, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ThunderShock,
                new PBEMoveData
                {
                    Type = PBEType.Electric, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeParalyze, EffectParam = 10,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ThunderWave,
                new PBEMoveData
                {
                    Type = PBEType.Electric, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Paralyze, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Toxic,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Toxic, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ToxicSpikes,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ToxicSpikes, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByMagicCoat,
                    Targets = PBEMoveTarget.AllFoes
                }
            },
            {
                PBEMove.Transform,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Transform, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.None,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.VacuumWave,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.VCreate,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1, EffectParam = 100,
                    PPTier = 1, Power = 180, Accuracy = 95, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Venoshock,
                new PBEMoveData
                {
                    Type = PBEType.Poison, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ViceGrip,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 55, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.VineWhip,
                new PBEMoveData
                {
                    Type = PBEType.Grass, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 35, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.VitalThrow,
                new PBEMoveData
                {
                    Type = PBEType.Fighting, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 70, Accuracy = 0, Priority = -1,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.Waterfall,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 20,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.WaterGun,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 5, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.WaterPulse,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeConfuse, EffectParam = 20,
                    PPTier = 4, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleNotSelf
                }
            },
            {
                PBEMove.WaterSpout,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 150, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.AllFoesSurrounding
                }
            },
            {
                PBEMove.WillOWisp,
                new PBEMoveData
                {
                    Type = PBEType.Fire, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.Burn, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 75, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.WingAttack,
                new PBEMoveData
                {
                    Type = PBEType.Flying, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 7, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleNotSelf
                }
            },
            {
                PBEMove.Withdraw,
                new PBEMoveData
                {
                    Type = PBEType.Water, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.ChangeUser_DEF, EffectParam = +1,
                    PPTier = 8, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.WorkUp,
                new PBEMoveData
                {
                    Type = PBEType.Normal, Category = PBEMoveCategory.Status,
                    Effect = PBEMoveEffect.RaiseUser_ATK_SPATK_By1, EffectParam = 0,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PBEMoveFlag.AffectedBySnatch,
                    Targets = PBEMoveTarget.Self
                }
            },
            {
                PBEMove.XScissor,
                new PBEMoveData
                {
                    Type = PBEType.Bug, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ZapCannon,
                new PBEMoveData
                {
                    Type = PBEType.Electric, Category = PBEMoveCategory.Special,
                    Effect = PBEMoveEffect.Hit__MaybeParalyze, EffectParam = 100,
                    PPTier = 1, Power = 120, Accuracy = 50, Priority = 0,
                    Flags = PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
            {
                PBEMove.ZenHeadbutt,
                new PBEMoveData
                {
                    Type = PBEType.Psychic, Category = PBEMoveCategory.Physical,
                    Effect = PBEMoveEffect.Hit__MaybeFlinch, EffectParam = 20,
                    PPTier = 3, Power = 80, Accuracy = 90, Priority = 0,
                    Flags = PBEMoveFlag.MakesContact | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByMirrorMove,
                    Targets = PBEMoveTarget.SingleSurrounding
                }
            },
        };
    }
}
