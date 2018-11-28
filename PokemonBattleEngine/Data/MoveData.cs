using System.Collections.Generic;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PMoveData
    {
        public PType Type;
        public PMoveCategory Category;
        public PMoveEffect Effect;
        public int EffectParam;
        public byte PPTier, Power, Accuracy; // 0 power or accuracy will show up as --
        public sbyte Priority;
        public PMoveFlag Flags;
        public PMoveTarget Targets;

        public static PType GetMoveTypeForPokemon(PPokemon pkmn, PMove move)
        {
            switch (move)
            {
                case PMove.None: return PType.Normal;
                case PMove.HiddenPower: return pkmn.GetHiddenPowerType();
                case PMove.TechnoBlast:
                    switch (pkmn.Item)
                    {
                        case PItem.BurnDrive: return PType.Fire;
                        case PItem.ChillDrive: return PType.Ice;
                        case PItem.DouseDrive: return PType.Water;
                        case PItem.ShockDrive: return PType.Electric;
                        default: return Data[PMove.TechnoBlast].Type;
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
            sb.AppendLine($"PP: {PPTier * PSettings.PPMultiplier}");
            sb.AppendLine($"Power: {(Power == 0 ? "--" : Power.ToString())}");
            sb.AppendLine($"Accuracy: {(Accuracy == 0 ? "--" : Accuracy.ToString())}");
            sb.AppendLine($"Priority: {Priority}");
            sb.AppendLine($"Flags: {Flags}");
            sb.Append($"Targets: {Targets}");

            return sb.ToString();
        }

        public static Dictionary<PMove, PMoveData> Data = new Dictionary<PMove, PMoveData>()
        {
            {
                PMove.Acid,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.AcidArmor,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_DEF, EffectParam = +2,
                    PPTier = 8, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.AcidSpray,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2, EffectParam = 100,
                    PPTier = 4, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.AerialAce,
                new PMoveData
                {
                    Type = PType.Flying, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleNotSelf
                }
            },
            {
                PMove.Aeroblast,
                new PMoveData
                {
                    Type = PType.Flying, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 100, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleNotSelf
                }
            },
            {
                PMove.Agility,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_SPE, EffectParam = +2,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.AirCutter,
                new PMoveData
                {
                    Type = PType.Flying, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 5, Power = 55, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.AirSlash,
                new PMoveData
                {
                    Type = PType.Flying, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 4, Power = 75, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleNotSelf
                }
            },
            {
                PMove.Amnesia,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_SPDEF, EffectParam = +2,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.AncientPower,
                new PMoveData
                {
                    Type = PType.Rock, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1, EffectParam = 10,
                    PPTier = 1, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.AquaJet,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.AquaTail,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 90, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Astonish,
                new PMoveData
                {
                    Type = PType.Ghost, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 3, Power = 30, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.AttackOrder,
                new PMoveData
                {
                    Type = PType.Bug, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.AuraSphere,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 90, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleNotSelf
                }
            },
            {
                PMove.AuroraBeam,
                new PMoveData
                {
                    Type = PType.Ice, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_ATK_By1, EffectParam = 10,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Barrier,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_DEF, EffectParam = +2,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.Bite,
                new PMoveData
                {
                    Type = PType.Dark, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 5, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.BlazeKick,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 2, Power = 85, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.BlueFlare,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 20,
                    PPTier = 1, Power = 130, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.BrickBreak,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.BrickBreak, EffectParam = 0,
                    PPTier = 3, Power = 75, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.BodySlam,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 3, Power = 85, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.BoltStrike,
                new PMoveData
                {
                    Type = PType.Electric, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeParalyze, EffectParam = 20,
                    PPTier = 1, Power = 130, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.BoneClub,
                new PMoveData
                {
                    Type = PType.Ground, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 10,
                    PPTier = 4, Power = 65, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Bubble,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 10,
                    PPTier = 6, Power = 20, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.BubbleBeam,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 10,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.BugBuzz,
                new PMoveData
                {
                    Type = PType.Bug, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 2, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.SoundBased,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.BulkUp,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.RaiseUser_ATK_DEF_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.Bulldoze,
                new PMoveData
                {
                    Type = PType.Ground, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 100,
                    PPTier = 4, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllSurrounding
                }
            },
            {
                PMove.BulletPunch,
                new PMoveData
                {
                    Type = PType.Steel, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.CalmMind,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.RaiseUser_SPATK_SPDEF_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.ChargeBeam,
                new PMoveData
                {
                    Type = PType.Electric, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeRaiseUser_SPATK_By1, EffectParam = 70,
                    PPTier = 2, Power = 50, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Charm,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_ATK, EffectParam = -2,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Chatter,
                new PMoveData
                {
                    Type = PType.Flying, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeConfuse, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.SoundBased,
                    Targets = PMoveTarget.SingleNotSelf
                }
            },
            {
                PMove.CloseCombat,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1, EffectParam = 100,
                    PPTier = 1, Power = 120, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Coil,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.RaiseUser_ATK_DEF_ACC_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.ConfuseRay,
                new PMoveData
                {
                    Type = PType.Ghost, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Confuse, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Confusion,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeConfuse, EffectParam = 10,
                    PPTier = 5, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.CosmicPower,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.RaiseUser_DEF_SPDEF_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.CottonGuard,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_DEF, EffectParam = +3,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.Crabhammer,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 90, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.CrossChop,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 100, Accuracy = 80, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.CrossPoison,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybePoison, EffectParam = 10,
                    PPTier = 4, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Crunch,
                new PMoveData
                {
                    Type = PType.Dark, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_DEF_By1, EffectParam = 20,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.CrushClaw,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_DEF_By1, EffectParam = 50,
                    PPTier = 2, Power = 75, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Curse,
                new PMoveData
                {
                    Type = PType.Ghost, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Curse, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.None,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.Cut,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 50, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.DarkPulse,
                new PMoveData
                {
                    Type = PType.Dark, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 20,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleNotSelf
                }
            },
            {
                PMove.DarkVoid,
                new PMoveData
                {
                    Type = PType.Dark, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 80, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.Detect,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Protect, EffectParam = 0,
                    PPTier = 1, Power = 0, Accuracy = 0, Priority = +4,
                    Flags = PMoveFlag.None,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.Dig,
                new PMoveData
                {
                    Type = PType.Ground, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Dig, EffectParam = 0,
                    PPTier = 2, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Discharge,
                new PMoveData
                {
                    Type = PType.Electric, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllSurrounding
                }
            },
            {
                PMove.Dive,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Dive, EffectParam = 0,
                    PPTier = 2, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.DizzyPunch,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeConfuse, EffectParam = 20,
                    PPTier = 2, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.DracoMeteor,
                new PMoveData
                {
                    Type = PType.Dragon, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerUser_SPATK_By2, EffectParam = 100,
                    PPTier = 1, Power = 140, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.DragonBreath,
                new PMoveData
                {
                    Type = PType.Dragon, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 4, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.DragonClaw,
                new PMoveData
                {
                    Type = PType.Dragon, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.DragonDance,
                new PMoveData
                {
                    Type = PType.Dragon, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.RaiseUser_ATK_SPE_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.DragonPulse,
                new PMoveData
                {
                    Type = PType.Dragon, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleNotSelf
                }
            },
            {
                PMove.DragonRush,
                new PMoveData
                {
                    Type = PType.Dragon, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 20,
                    PPTier = 2, Power = 100, Accuracy = 75, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.DoubleTeam,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_EVA, EffectParam = +1,
                    PPTier = 3, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.DrillPeck,
                new PMoveData
                {
                    Type = PType.Flying, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleNotSelf
                }
            },
            {
                PMove.DrillRun,
                new PMoveData
                {
                    Type = PType.Ground, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 80, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.EarthPower,
                new PMoveData
                {
                    Type = PType.Ground, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 2, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Earthquake,
                new PMoveData
                {
                    Type = PType.Ground, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 100, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HitsUnderground,
                    Targets = PMoveTarget.AllSurrounding
                }
            },
            {
                PMove.EggBomb,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 100, Accuracy = 75, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Electroweb,
                new PMoveData
                {
                    Type = PType.Electric, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 100,
                    PPTier = 3, Power = 55, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.Ember,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 5, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.EnergyBall,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 2, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Extrasensory,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 10,
                    PPTier = 6, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ExtremeSpeed,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 80, Accuracy = 100, Priority = +2,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.FaintAttack,
                new PMoveData
                {
                    Type = PType.Dark, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.FakeTears,
                new PMoveData
                {
                    Type = PType.Dark, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_SPDEF, EffectParam = -2,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.FeatherDance,
                new PMoveData
                {
                    Type = PType.Flying, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_ATK, EffectParam = -2,
                    PPTier = 3, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.FieryDance,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeRaiseUser_SPATK_By1, EffectParam = 50,
                    PPTier = 2, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.FireBlast,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 1, Power = 120, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.FirePunch,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 3, Power = 75, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.FlameCharge,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeRaiseUser_SPE_By1, EffectParam = 100,
                    PPTier = 4, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Flamethrower,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 3, Power = 95, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.FlameWheel,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 5, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.DefrostsUser,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Flash,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_ACC, EffectParam = -1,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.FlashCannon,
                new PMoveData
                {
                    Type = PType.Steel, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 2, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Fly,
                new PMoveData
                {
                    Type = PType.Flying, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Fly, EffectParam = 0,
                    PPTier = 3, Power = 90, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleNotSelf
                }
            },
            {
                PMove.FocusBlast,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 1, Power = 120, Accuracy = 70, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.FocusEnergy,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.FocusEnergy, EffectParam = 0,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.ForcePalm,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 2, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.FrostBreath,
                new PMoveData
                {
                    Type = PType.Ice, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 40, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.AlwaysCrit,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Frustration,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Glaciate,
                new PMoveData
                {
                    Type = PType.Ice, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 100,
                    PPTier = 2, Power = 65, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.Glare,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Paralyze, EffectParam = 0,
                    PPTier = 6, Power = 0, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.GrassKnot,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.GrassWhistle,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 55, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove | PMoveFlag.SoundBased,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Growl,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_ATK, EffectParam = -1,
                    PPTier = 8, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove | PMoveFlag.SoundBased,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.Growth,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Growth, EffectParam = 0,
                    PPTier = 8, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.GunkShot,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybePoison, EffectParam = 30,
                    PPTier = 1, Power = 120, Accuracy = 70, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.HammerArm,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeLowerUser_SPE_By1, EffectParam = 100,
                    PPTier = 2, Power = 100, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Harden,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_DEF, EffectParam = +1,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.Headbutt,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 3, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.HeartStamp,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 5, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.HeatWave,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 10,
                    PPTier = 2, Power = 100, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.HiddenPower,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.HoneClaws,
                new PMoveData
                {
                    Type = PType.Dark, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.RaiseUser_ATK_ACC_By1, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.HornAttack,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 5, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Howl,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_ATK, EffectParam = +1,
                    PPTier = 8, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.HydroPump,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 120, Accuracy = 80, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.HyperFang,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 10,
                    PPTier = 3, Power = 80, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.HyperVoice,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.SoundBased,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.Hypnosis,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 60, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.IceBeam,
                new PMoveData
                {
                    Type = PType.Ice, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeFreeze, EffectParam = 10,
                    PPTier = 2, Power = 95, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.IcePunch,
                new PMoveData
                {
                    Type = PType.Ice, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFreeze, EffectParam = 10,
                    PPTier = 3, Power = 75, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.IceShard,
                new PMoveData
                {
                    Type = PType.Ice, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.IcicleCrash,
                new PMoveData
                {
                    Type = PType.Ice, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 2, Power = 85, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.IcyWind,
                new PMoveData
                {
                    Type = PType.Ice, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 100,
                    PPTier = 3, Power = 55, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.Inferno,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 100,
                    PPTier = 1, Power = 10, Accuracy = 50, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.IronDefense,
                new PMoveData
                {
                    Type = PType.Steel, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_DEF, EffectParam = +2,
                    PPTier = 3, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.IronHead,
                new PMoveData
                {
                    Type = PType.Steel, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.IronTail,
                new PMoveData
                {
                    Type = PType.Steel, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_DEF_By1, EffectParam = 30,
                    PPTier = 3, Power = 100, Accuracy = 75, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.KarateChop,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 5, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Kinesis,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_ACC, EffectParam = -1,
                    PPTier = 3, Power = 0, Accuracy = 80, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.LavaPlume,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 30,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllSurrounding
                }
            },
            {
                PMove.LeafBlade,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.LeafTornado,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 50,
                    PPTier = 2, Power = 65, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.LeechSeed,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.LeechSeed, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Leer,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_DEF, EffectParam = -1,
                    PPTier = 6, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.Lick,
                new PMoveData
                {
                    Type = PType.Ghost, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 6, Power = 20, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.LightScreen,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.LightScreen, EffectParam = 0,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.AllTeam
                }
            },
            {
                PMove.LovelyKiss,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 75, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.LowKick,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.LowSweep,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 100,
                    PPTier = 4, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.LusterPurge,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 50,
                    PPTier = 1, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.MachPunch,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.MagicalLeaf,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.MagnetBomb,
                new PMoveData
                {
                    Type = PType.Steel, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Magnitude,
                new PMoveData
                {
                    Type = PType.Ground, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Magnitude, EffectParam = 0,
                    PPTier = 6, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HitsUnderground,
                    Targets = PMoveTarget.AllSurrounding
                }
            },
            {
                PMove.Meditate,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_ATK, EffectParam = +1,
                    PPTier = 8, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.Megahorn,
                new PMoveData
                {
                    Type = PType.Bug, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 120, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.MegaKick,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 120, Accuracy = 75, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.MegaPunch,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 80, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.MetalClaw,
                new PMoveData
                {
                    Type = PType.Steel, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeRaiseUser_ATK_By1, EffectParam = 10,
                    PPTier = 7, Power = 50, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.MetalSound,
                new PMoveData
                {
                    Type = PType.Steel, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_SPDEF, EffectParam = -2,
                    PPTier = 8, Power = 0, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove | PMoveFlag.SoundBased,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.MeteorMash,
                new PMoveData
                {
                    Type = PType.Steel, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeRaiseUser_ATK_By1, EffectParam = 20,
                    PPTier = 2, Power = 100, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Minimize,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Minimize, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.MirrorShot,
                new PMoveData
                {
                    Type = PType.Steel, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 30,
                    PPTier = 2, Power = 65, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.MistBall,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPATK_By1, EffectParam = 50,
                    PPTier = 1, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Moonlight,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Moonlight, EffectParam = 0,
                    PPTier = 1, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.MudBomb,
                new PMoveData
                {
                    Type = PType.Ground, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 30,
                    PPTier = 2, Power = 65, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.MuddyWater,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 30,
                    PPTier = 2, Power = 95, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.MudSlap,
                new PMoveData
                {
                    Type = PType.Ground, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 100,
                    PPTier = 2, Power = 20, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.MudShot,
                new PMoveData
                {
                    Type = PType.Ground, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 100,
                    PPTier = 3, Power = 55, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.NastyPlot,
                new PMoveData
                {
                    Type = PType.Dark, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_SPATK, EffectParam = +2,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.NightDaze,
                new PMoveData
                {
                    Type = PType.Dark, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 40,
                    PPTier = 2, Power = 85, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.NightSlash,
                new PMoveData
                {
                    Type = PType.Dark, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Octazooka,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_ACC_By1, EffectParam = 50,
                    PPTier = 2, Power = 65, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.OminousWind,
                new PMoveData
                {
                    Type = PType.Ghost, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1, EffectParam = 10,
                    PPTier = 1, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Overheat,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerUser_SPATK_By2, EffectParam = 100,
                    PPTier = 1, Power = 140, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.PainSplit,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.PainSplit, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Peck,
                new PMoveData
                {
                    Type = PType.Flying, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 7, Power = 35, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleNotSelf
                }
            },
            {
                PMove.PoisonFang,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeToxic, EffectParam = 30,
                    PPTier = 3, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.PoisonGas,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Poison, EffectParam = 0,
                    PPTier = 8, Power = 0, Accuracy = 80, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.PoisonJab,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybePoison, EffectParam = 30,
                    PPTier = 4, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.PoisonPowder,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Poison, EffectParam = 0,
                    PPTier = 7, Power = 0, Accuracy = 75, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.PoisonSting,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybePoison, EffectParam = 30,
                    PPTier = 7, Power = 15, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.PoisonTail,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybePoison, EffectParam = 10,
                    PPTier = 5, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Pound,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 7, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.PowderSnow,
                new PMoveData
                {
                    Type = PType.Ice, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeFreeze, EffectParam = 10,
                    PPTier = 5, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.PowerGem,
                new PMoveData
                {
                    Type = PType.Rock, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.PowerWhip,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 120, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Protect,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Protect, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = +4,
                    Flags = PMoveFlag.None,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.Psybeam,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeConfuse, EffectParam = 10,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Psychic,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 10,
                    PPTier = 2, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.PsychoBoost,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerUser_SPATK_By2, EffectParam = 0,
                    PPTier = 1, Power = 140, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.PsychoCut,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.QuickAttack,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.QuiverDance,
                new PMoveData
                {
                    Type = PType.Bug, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.RaiseUser_SPATK_SPDEF_SPE_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.RainDance,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.RainDance, EffectParam = 0,
                    PPTier = 1, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.None,
                    Targets = PMoveTarget.All
                }
            },
            {
                PMove.RazorLeaf,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 5, Power = 55, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.RazorShell,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_DEF_By1, EffectParam = 50,
                    PPTier = 2, Power = 75, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Reflect,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Reflect, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.AllTeam
                }
            },
            {
                PMove.Retaliate,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Return,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.RockClimb,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeConfuse, EffectParam = 20,
                    PPTier = 4, Power = 90, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.RockPolish,
                new PMoveData
                {
                    Type = PType.Rock, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_SPE, EffectParam = +2,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.RockSlide,
                new PMoveData
                {
                    Type = PType.Rock, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 2, Power = 75, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.RockSmash,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.RockThrow,
                new PMoveData
                {
                    Type = PType.Rock, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 50, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.RockTomb,
                new PMoveData
                {
                    Type = PType.Rock, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPE_By1, EffectParam = 100,
                    PPTier = 2, Power = 50, Accuracy = 80, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SacredFire,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 50,
                    PPTier = 1, Power = 100, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.DefrostsUser,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SandAttack,
                new PMoveData
                {
                    Type = PType.Ground, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_ACC, EffectParam = -1,
                    PPTier = 3, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Scald,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 30,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.DefrostsUser,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ScaryFace,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_SPE, EffectParam = -2,
                    PPTier = 2, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Scratch,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 8, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Screech,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_DEF, EffectParam = -2,
                    PPTier = 8, Power = 0, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove | PMoveFlag.SoundBased,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SearingShot,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeBurn, EffectParam = 30,
                    PPTier = 1, Power = 100, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SeedBomb,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SeedFlare,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2, EffectParam = 40,
                    PPTier = 1, Power = 120, Accuracy = 85, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ShadowBall,
                new PMoveData
                {
                    Type = PType.Ghost, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, EffectParam = 20,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ShadowClaw,
                new PMoveData
                {
                    Type = PType.Ghost, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ShadowPunch,
                new PMoveData
                {
                    Type = PType.Ghost, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ShadowSneak,
                new PMoveData
                {
                    Type = PType.Ghost, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Sharpen,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_ATK, EffectParam = +1,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.ShellSmash,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.ShiftGear,
                new PMoveData
                {
                    Type = PType.Steel, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.RaiseUser_SPE_By2_ATK_By1, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.SignalBeam,
                new PMoveData
                {
                    Type = PType.Bug, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeConfuse, EffectParam = 10,
                    PPTier = 3, Power = 75, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SilverWind,
                new PMoveData
                {
                    Type = PType.Bug, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1, EffectParam = 10,
                    PPTier = 1, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Sing,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 55, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove | PMoveFlag.SoundBased,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ShockWave,
                new PMoveData
                {
                    Type = PType.Electric, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SkyUppercut,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 85, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HitsAirborne,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Slam,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 80, Accuracy = 75, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Slash,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 70, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SleepPowder,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 75, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Sludge,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybePoison, EffectParam = 30,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SludgeBomb,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybePoison, EffectParam = 30,
                    PPTier = 2, Power = 90, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SludgeWave,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybePoison, EffectParam = 10,
                    PPTier = 2, Power = 95, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllSurrounding
                }
            },
            {
                PMove.Smog,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybePoison, EffectParam = 40,
                    PPTier = 4, Power = 20, Accuracy = 70, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SmokeScreen,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_ACC, EffectParam = -1,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Snarl,
                new PMoveData
                {
                    Type = PType.Dark, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPATK_By1, EffectParam = 100,
                    PPTier = 3, Power = 55, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.SoundBased,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.SpacialRend,
                new PMoveData
                {
                    Type = PType.Dragon, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 100, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Spark,
                new PMoveData
                {
                    Type = PType.Electric, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Spikes,
                new PMoveData
                {
                    Type = PType.Ground, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Spikes, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedByMagicCoat,
                    Targets = PMoveTarget.AllFoes
                }
            },
            {
                PMove.Spore,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Sleep, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.StealthRock,
                new PMoveData
                {
                    Type = PType.Rock, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.StealthRock, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedByMagicCoat,
                    Targets = PMoveTarget.AllFoes
                }
            },
            {
                PMove.Steamroller,
                new PMoveData
                {
                    Type = PType.Bug, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 30,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SteelWing,
                new PMoveData
                {
                    Type = PType.Steel, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeRaiseUser_DEF_By1, EffectParam = 10,
                    PPTier = 5, Power = 70, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Stomp,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 65, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.StoneEdge,
                new PMoveData
                {
                    Type = PType.Rock, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 100, Accuracy = 80, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HighCritChance,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.StormThrow,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.AlwaysCrit,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Strength,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 80, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.StringShot,
                new PMoveData
                {
                    Type = PType.Bug, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_SPE, EffectParam = -1,
                    PPTier = 8, Power = 0, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.StruggleBug,
                new PMoveData
                {
                    Type = PType.Bug, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeLowerTarget_SPATK_By1, EffectParam = 100,
                    PPTier = 4, Power = 30, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.StunSpore,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Paralyze, EffectParam = 0,
                    PPTier = 6, Power = 0, Accuracy = 75, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Substitute,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Substitute, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.SunnyDay,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.SunnyDay, EffectParam = 0,
                    PPTier = 1, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.None,
                    Targets = PMoveTarget.All
                }
            },
            {
                PMove.Supersonic,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Confuse, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 55, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove | PMoveFlag.SoundBased,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Surf,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 95, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HitsUnderwater,
                    Targets = PMoveTarget.AllSurrounding
                }
            },
            {
                PMove.SweetKiss,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Confuse, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 75, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.SweetScent,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_EVA, EffectParam = -1,
                    PPTier = 5, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.Swift,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 4, Power = 60, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.SwordsDance,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_ATK, EffectParam = +2,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.Tackle,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 7, Power = 50, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.TailGlow,
                new PMoveData
                {
                    Type = PType.Bug, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_SPATK, EffectParam = +3,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.TailWhip,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeTarget_DEF, EffectParam = -1,
                    PPTier = 6, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllFoesSurrounding
                }
            },
            {
                PMove.TechnoBlast,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 1, Power = 85, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.TeeterDance,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Confuse, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.AllSurrounding
                }
            },
            {
                PMove.Teleport,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Fail, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.None,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.Tickle,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.LowerTarget_ATK_DEF_By1, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Thunder,
                new PMoveData
                {
                    Type = PType.Electric, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeParalyze, EffectParam = 30,
                    PPTier = 2, Power = 120, Accuracy = 70, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove | PMoveFlag.HitsAirborne,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Thunderbolt,
                new PMoveData
                {
                    Type = PType.Electric, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeParalyze, EffectParam = 10,
                    PPTier = 3, Power = 95, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ThunderPunch,
                new PMoveData
                {
                    Type = PType.Electric, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeParalyze, EffectParam = 10,
                    PPTier = 3, Power = 75, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ThunderShock,
                new PMoveData
                {
                    Type = PType.Electric, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeParalyze, EffectParam = 10,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ThunderWave,
                new PMoveData
                {
                    Type = PType.Electric, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Paralyze, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Toxic,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Toxic, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ToxicSpikes,
                new PMoveData
                {
                    Type = PType.Poison, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ToxicSpikes, EffectParam = 0,
                    PPTier = 4, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedByMagicCoat,
                    Targets = PMoveTarget.AllFoes
                }
            },
            {
                PMove.Transform,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Transform, EffectParam = 0,
                    PPTier = 2, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.None,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.VacuumWave,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 40, Accuracy = 100, Priority = +1,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.VCreate,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1, EffectParam = 100,
                    PPTier = 1, Power = 180, Accuracy = 95, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ViceGrip,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 6, Power = 55, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.VineWhip,
                new PMoveData
                {
                    Type = PType.Grass, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 35, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.VitalThrow,
                new PMoveData
                {
                    Type = PType.Fighting, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 2, Power = 70, Accuracy = 0, Priority = -1,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.Waterfall,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 20,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.WaterGun,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 5, Power = 40, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.WaterPulse,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeConfuse, EffectParam = 20,
                    PPTier = 4, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleNotSelf
                }
            },
            {
                PMove.WillOWisp,
                new PMoveData
                {
                    Type = PType.Fire, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.Burn, EffectParam = 0,
                    PPTier = 3, Power = 0, Accuracy = 75, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMagicCoat | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.WingAttack,
                new PMoveData
                {
                    Type = PType.Flying, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 7, Power = 60, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleNotSelf
                }
            },
            {
                PMove.Withdraw,
                new PMoveData
                {
                    Type = PType.Water, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.ChangeUser_DEF, EffectParam = +1,
                    PPTier = 8, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.Workup,
                new PMoveData
                {
                    Type = PType.Normal, Category = PMoveCategory.Status,
                    Effect = PMoveEffect.RaiseUser_ATK_SPATK_By1, EffectParam = 0,
                    PPTier = 6, Power = 0, Accuracy = 0, Priority = 0,
                    Flags = PMoveFlag.AffectedBySnatch,
                    Targets = PMoveTarget.Self
                }
            },
            {
                PMove.XScissor,
                new PMoveData
                {
                    Type = PType.Bug, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit, EffectParam = 0,
                    PPTier = 3, Power = 80, Accuracy = 100, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ZapCannon,
                new PMoveData
                {
                    Type = PType.Electric, Category = PMoveCategory.Special,
                    Effect = PMoveEffect.Hit__MaybeParalyze, EffectParam = 100,
                    PPTier = 1, Power = 120, Accuracy = 50, Priority = 0,
                    Flags = PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
            {
                PMove.ZenHeadbutt,
                new PMoveData
                {
                    Type = PType.Psychic, Category = PMoveCategory.Physical,
                    Effect = PMoveEffect.Hit__MaybeFlinch, EffectParam = 20,
                    PPTier = 3, Power = 80, Accuracy = 90, Priority = 0,
                    Flags = PMoveFlag.MakesContact | PMoveFlag.AffectedByProtect | PMoveFlag.AffectedByMirrorMove,
                    Targets = PMoveTarget.SingleSurrounding
                }
            },
        };
    }
}
