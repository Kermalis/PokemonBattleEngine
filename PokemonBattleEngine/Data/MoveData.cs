using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PBEMoveData
    {
        public PBEType Type { get; }
        public PBEMoveCategory Category { get; }
        public sbyte Priority { get; }
        public byte PPTier { get; } // 0 PPTier will become 1 PP (unaffected by pp ups)
        public byte Power { get; } // 0 power or accuracy will show up as --
        public byte Accuracy { get; }
        public PBEMoveEffect Effect { get; }
        public int EffectParam { get; }
        public PBEMoveTarget Targets { get; }
        public PBEMoveFlag Flags { get; }

        private PBEMoveData(PBEType type, PBEMoveCategory category, sbyte priority, byte ppTier, byte power, byte accuracy,
            PBEMoveEffect effect, int effectParam, PBEMoveTarget targets,
            PBEMoveFlag flags)
        {
            Type = type; Category = category; Priority = priority; PPTier = ppTier; Power = power; Accuracy = accuracy;
            Effect = effect; EffectParam = effectParam; Targets = targets;
            Flags = flags;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Type: {Type}");
            sb.AppendLine($"Category: {Category}");
            sb.AppendLine($"Priority: {Priority}");
            sb.AppendLine($"PP: {Math.Max(1, PPTier * PBESettings.DefaultPPMultiplier)}");
            sb.AppendLine($"Power: {(Power == 0 ? "--" : Power.ToString())}");
            sb.AppendLine($"Accuracy: {(Accuracy == 0 ? "--" : Accuracy.ToString())}");
            sb.AppendLine($"Effect: {Effect}");
            sb.AppendLine($"Effect Parameter: {EffectParam}");
            sb.AppendLine($"Targets: {Targets}");
            sb.Append($"Flags: {Flags}");

            return sb.ToString();
        }

        public static bool IsSpreadMove(PBEMoveTarget targets)
        {
            return targets == PBEMoveTarget.All || targets == PBEMoveTarget.AllFoes || targets == PBEMoveTarget.AllFoesSurrounding || targets == PBEMoveTarget.AllSurrounding || targets == PBEMoveTarget.AllTeam;
        }

        public static ReadOnlyDictionary<PBEMove, PBEMoveData> Data { get; } = new ReadOnlyDictionary<PBEMove, PBEMoveData>(new Dictionary<PBEMove, PBEMoveData>
        {
            {
                PBEMove.Absorb,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 5, 20, 100,
                    PBEMoveEffect.HPDrain, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Acid,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Special, 0, 6, 40, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.AcidArmor,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Status, 0, 8, 0, 0,
                    PBEMoveEffect.ChangeTarget_DEF, +2, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.AcidSpray,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Special, 0, 4, 40, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Acrobatics,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Physical, 0, 3, 55, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.AerialAce,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Physical, 0, 4, 60, 0,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Aeroblast,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Special, 0, 1, 100, 95,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
                )
            },
            {
                PBEMove.Agility,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.ChangeTarget_SPE, +2, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.AirCutter,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Special, 0, 5, 55, 95,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
                )
            },
            {
                PBEMove.AirSlash,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Special, 0, 4, 75, 95,
                    PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Amnesia,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.ChangeTarget_SPDEF, +2, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.AncientPower,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Special, 0, 1, 60, 100,
                    PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.AquaJet,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Physical, +1, 4, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.AquaTail,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Physical, 0, 2, 90, 90,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Astonish,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Physical, 0, 3, 30, 100,
                    PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.AttackOrder,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Physical, 0, 3, 90, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
                )
            },
            {
                PBEMove.Attract,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 100,
                    PBEMoveEffect.Attract, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.AuraSphere,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Special, 0, 4, 90, 0,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.AuroraBeam,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Special, 0, 4, 65, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_ATK_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Barrier,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.ChangeTarget_DEF, +2, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Bite,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 5, 60, 100,
                    PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.BlazeKick,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Physical, 0, 2, 85, 90,
                    PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Blizzard,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Special, 0, 1, 120, 70,
                    PBEMoveEffect.Hit__MaybeFreeze, 10, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.BlueFlare,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 1, 130, 85,
                    PBEMoveEffect.Hit__MaybeBurn, 20, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.BrickBreak,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 3, 75, 100,
                    PBEMoveEffect.BrickBreak, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Brine,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 2, 65, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.BodySlam,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 85, 100,
                    PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.BoltStrike,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Physical, 0, 1, 130, 85,
                    PBEMoveEffect.Hit__MaybeParalyze, 20, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.BoneClub,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Physical, 0, 4, 65, 85,
                    PBEMoveEffect.Hit__MaybeFlinch, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.BraveBird,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Physical, 0, 3, 120, 100,
                    PBEMoveEffect.Recoil, 3, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Bubble,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 6, 20, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 10, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.BubbleBeam,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 4, 65, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.BugBuzz,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Special, 0, 2, 90, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.SoundBased
                )
            },
            {
                PBEMove.BulkUp,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.RaiseTarget_ATK_DEF_By1, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Bulldoze,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Physical, 0, 4, 60, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 100, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.BulletPunch,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Physical, +1, 6, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.CalmMind,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.RaiseTarget_SPATK_SPDEF_By1, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Captivate,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 100,
                    PBEMoveEffect.ChangeTarget_SPATK__IfAttractionPossible, -2, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.ChargeBeam,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Special, 0, 2, 50, 90,
                    PBEMoveEffect.Hit__MaybeRaiseUser_SPATK_By1, 70, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Charm,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.ChangeTarget_ATK, -2, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Chatter,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Special, 0, 4, 60, 100,
                    PBEMoveEffect.Hit__MaybeConfuse, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedByMetronome | PBEMoveFlag.SoundBased
                )
            },
            {
                PBEMove.CloseCombat,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 1, 120, 100,
                    PBEMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Coil,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.RaiseTarget_ATK_DEF_ACC_By1, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.ConfuseRay,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Status, 0, 2, 0, 100,
                    PBEMoveEffect.Confuse, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Confusion,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 5, 50, 100,
                    PBEMoveEffect.Hit__MaybeConfuse, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Constrict,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 7, 10, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Conversion,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.Conversion, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.CosmicPower,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.RaiseTarget_DEF_SPDEF_By1, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.CottonGuard,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.ChangeTarget_DEF, +3, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.CottonSpore,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 8, 0, 100,
                    PBEMoveEffect.ChangeTarget_SPE, -2, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Crabhammer,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Physical, 0, 2, 90, 90,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.CrossChop,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 1, 100, 80,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.CrossPoison,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Physical, 0, 4, 70, 100,
                    PBEMoveEffect.Hit__MaybePoison, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Crunch,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 3, 80, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1, 20, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.CrushClaw,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 75, 95,
                    PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.CrushGrip,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Curse,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.Curse, 0, PBEMoveTarget.Varies,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.Cut,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 6, 50, 95,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.DarkPulse,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Special, 0, 3, 80, 100,
                    PBEMoveEffect.Hit__MaybeFlinch, 20, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.DarkVoid,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Status, 0, 2, 0, 80,
                    PBEMoveEffect.Sleep, 0, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.DefendOrder,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.RaiseTarget_DEF_SPDEF_By1, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.DefenseCurl,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 0,
                    PBEMoveEffect.ChangeTarget_DEF, +1, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Detect,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Status, +4, 1, 0, 0,
                    PBEMoveEffect.Protect, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedByMetronome
                )
            },
            {
                PBEMove.Dig,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Physical, 0, 2, 80, 100,
                    PBEMoveEffect.Dig, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Discharge,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Special, 0, 3, 80, 100,
                    PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Dive,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Physical, 0, 2, 80, 100,
                    PBEMoveEffect.Dive, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.DizzyPunch,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 70, 100,
                    PBEMoveEffect.Hit__MaybeConfuse, 20, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.DoubleEdge,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 120, 100,
                    PBEMoveEffect.Recoil, 3, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.DoubleTeam,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 0,
                    PBEMoveEffect.ChangeTarget_EVA, +1, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.DracoMeteor,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Special, 0, 1, 140, 90,
                    PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.DragonBreath,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Special, 0, 4, 60, 100,
                    PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.DragonClaw,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Physical, 0, 3, 80, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.DragonDance,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.RaiseTarget_ATK_SPE_By1, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.DragonPulse,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Special, 0, 2, 90, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.DragonRage,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Special, 0, 2, 0, 100,
                    PBEMoveEffect.SetDamage, 40, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.DragonRush,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Physical, 0, 2, 100, 75,
                    PBEMoveEffect.Hit__MaybeFlinch, 20, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.DrainPunch,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 75, 100,
                    PBEMoveEffect.HPDrain, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.DreamEater,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 3, 100, 100,
                    PBEMoveEffect.HPDrain__RequireSleep, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.DrillPeck,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Physical, 0, 4, 80, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.DrillRun,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Physical, 0, 2, 80, 95,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.DynamicPunch,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 1, 100, 50,
                    PBEMoveEffect.Hit__MaybeConfuse, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.EarthPower,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Special, 0, 2, 90, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Earthquake,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Physical, 0, 2, 100, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsUnderground
                )
            },
            {
                PBEMove.EggBomb,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 100, 75,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Electroweb,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Special, 0, 3, 55, 95,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 100, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Ember,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 5, 40, 100,
                    PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Endeavor,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 0, 100,
                    PBEMoveEffect.Endeavor, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.EnergyBall,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 2, 80, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Eruption,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 1, 150, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Explosion,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 250, 100,
                    PBEMoveEffect.Selfdestruct, 0, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Extrasensory,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 6, 80, 100,
                    PBEMoveEffect.Hit__MaybeFlinch, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.ExtremeSpeed,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, +2, 1, 80, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Facade,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 70, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FaintAttack,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 4, 60, 0,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FakeTears,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Status, 0, 4, 0, 100,
                    PBEMoveEffect.ChangeTarget_SPDEF, -2, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.FeatherDance,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Status, 0, 3, 0, 100,
                    PBEMoveEffect.ChangeTarget_ATK, -2, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.FieryDance,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 2, 80, 100,
                    PBEMoveEffect.Hit__MaybeRaiseUser_SPATK_By1, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.FinalGambit,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Special, 0, 1, 0, 100,
                    PBEMoveEffect.FinalGambit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.FireBlast,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 1, 120, 85,
                    PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.FireFang,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Physical, 0, 3, 65, 95,
                    PBEMoveEffect.Hit__MaybeBurn__10PercentFlinch, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FirePunch,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Physical, 0, 3, 75, 100,
                    PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Fissure,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Physical, 0, 1, 0, 0,
                    PBEMoveEffect.OneHitKnockout, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsUnderground
                )
            },
            {
                PBEMove.Flail,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FlameCharge,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Physical, 0, 4, 50, 100,
                    PBEMoveEffect.Hit__MaybeRaiseUser_SPE_By1, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Flamethrower,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 3, 95, 100,
                    PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.FlameWheel,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Physical, 0, 5, 60, 100,
                    PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DefrostsUser | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FlareBlitz,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Physical, 0, 3, 120, 100,
                    PBEMoveEffect.Recoil__10PercentBurn, 3, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.DefrostsUser | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Flash,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 100,
                    PBEMoveEffect.ChangeTarget_ACC, -1, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.FlashCannon,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Special, 0, 2, 80, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Flatter,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Status, 0, 3, 0, 100,
                    PBEMoveEffect.Flatter, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Fly,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Physical, 0, 3, 90, 95,
                    PBEMoveEffect.Fly, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FocusBlast,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Special, 0, 1, 120, 70,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.FocusEnergy,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.FocusEnergy, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.ForcePalm,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 60, 100,
                    PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FoulPlay,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 3, 95, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FrostBreath,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Special, 0, 2, 40, 90,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AlwaysCrit
                )
            },
            {
                PBEMove.Frustration,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.GastroAcid,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Status, 0, 2, 0, 100,
                    PBEMoveEffect.GastroAcid, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.GigaDrain,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 2, 75, 100,
                    PBEMoveEffect.HPDrain, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Glaciate,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Special, 0, 2, 65, 95,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 100, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Glare,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 90,
                    PBEMoveEffect.Paralyze, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.GrassKnot,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 4, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.GrassWhistle,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 3, 0, 55,
                    PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.SoundBased
                )
            },
            {
                PBEMove.Growl,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 100,
                    PBEMoveEffect.ChangeTarget_ATK, -1, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.SoundBased
                )
            },
            {
                PBEMove.Growth,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 8, 0, 0,
                    PBEMoveEffect.Growth, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Guillotine,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 0, 0,
                    PBEMoveEffect.OneHitKnockout, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.GunkShot,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Physical, 0, 1, 120, 70,
                    PBEMoveEffect.Hit__MaybePoison, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Gust,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Special, 0, 7, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsAirborne
                )
            },
            {
                PBEMove.Hail,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.Hail, 0, PBEMoveTarget.All,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.HammerArm,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 100, 90,
                    PBEMoveEffect.Hit__MaybeLowerUser_SPE_By1, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Harden,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.ChangeTarget_DEF, +1, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Haze,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.Haze, 0, PBEMoveTarget.All,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.Headbutt,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 70, 100,
                    PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.HeadCharge,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 120, 100,
                    PBEMoveEffect.Recoil, 4, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.HeadSmash,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Physical, 0, 1, 150, 80,
                    PBEMoveEffect.Recoil, 2, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.HealOrder,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.RestoreTargetHP, 50, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.HealPulse,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.RestoreTargetHP, 50, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.HeartStamp,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Physical, 0, 5, 60, 100,
                    PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.HeatCrash,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Physical, 0, 2, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.HeatWave,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 2, 100, 90,
                    PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.HeavySlam,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Physical, 0, 2, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.HelpingHand,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, +5, 4, 0, 0,
                    PBEMoveEffect.HelpingHand, 0, PBEMoveTarget.SingleAllySurrounding,
                    PBEMoveFlag.BlockedByMetronome
                )
            },
            {
                PBEMove.Hex,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Special, 0, 2, 50, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.HiddenPower,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 3, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.HoneClaws,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Status, 0, 3, 0, 0,
                    PBEMoveEffect.RaiseTarget_ATK_ACC_By1, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.HornAttack,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 5, 65, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.HornDrill,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 0, 0,
                    PBEMoveEffect.OneHitKnockout, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.HornLeech,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Physical, 0, 2, 75, 100,
                    PBEMoveEffect.HPDrain, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Howl,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 0,
                    PBEMoveEffect.ChangeTarget_ATK, +1, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Hurricane,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Special, 0, 2, 120, 70,
                    PBEMoveEffect.Hit__MaybeConfuse, 30, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsAirborne
                )
            },
            {
                PBEMove.HydroPump,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 1, 120, 80,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.HyperFang,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 80, 90,
                    PBEMoveEffect.Hit__MaybeFlinch, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.HyperVoice,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 2, 90, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.SoundBased
                )
            },
            {
                PBEMove.Hypnosis,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 4, 0, 60,
                    PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.IceBeam,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Special, 0, 2, 95, 100,
                    PBEMoveEffect.Hit__MaybeFreeze, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.IceFang,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Physical, 0, 3, 65, 95,
                    PBEMoveEffect.Hit__MaybeFreeze__10PercentFlinch, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.IcePunch,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Physical, 0, 3, 75, 100,
                    PBEMoveEffect.Hit__MaybeFreeze, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.IceShard,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Physical, +1, 6, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.IcicleCrash,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Physical, 0, 2, 85, 90,
                    PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.IcyWind,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Special, 0, 3, 55, 95,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 100, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Inferno,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 1, 100, 50,
                    PBEMoveEffect.Hit__MaybeBurn, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.IronDefense,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Status, 0, 3, 0, 0,
                    PBEMoveEffect.ChangeTarget_DEF, +2, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.IronHead,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Physical, 0, 3, 80, 100,
                    PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.IronTail,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Physical, 0, 3, 100, 75,
                    PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Judgment,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 2, 100, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.KarateChop,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 5, 50, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Kinesis,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 3, 0, 80,
                    PBEMoveEffect.ChangeTarget_ACC, -1, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.LavaPlume,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 3, 80, 100,
                    PBEMoveEffect.Hit__MaybeBurn, 30, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.LeafBlade,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Physical, 0, 3, 90, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.LeafStorm,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 1, 140, 90,
                    PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.LeafTornado,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 2, 65, 90,
                    PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.LeechLife,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Physical, 0, 3, 20, 100,
                    PBEMoveEffect.HPDrain, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.LeechSeed,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 2, 0, 90,
                    PBEMoveEffect.LeechSeed, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Leer,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 100,
                    PBEMoveEffect.ChangeTarget_DEF, -1, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Lick,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Physical, 0, 6, 20, 100,
                    PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.LightScreen,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.LightScreen, 0, PBEMoveTarget.AllTeam,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.LovelyKiss,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 75,
                    PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.LowKick,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 4, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.LowSweep,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 4, 60, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.LuckyChant,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.LuckyChant, 0, PBEMoveTarget.AllTeam,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.LusterPurge,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 1, 70, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.MachPunch,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, +1, 6, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.MagicalLeaf,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 4, 60, 0,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.MagnetBomb,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Physical, 0, 4, 60, 0,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.MagnetRise,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.MagnetRise, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Magnitude,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Physical, 0, 6, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsUnderground
                )
            },
            {
                PBEMove.Meditate,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 8, 0, 0,
                    PBEMoveEffect.ChangeTarget_ATK, +1, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.MegaDrain,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 3, 40, 100,
                    PBEMoveEffect.HPDrain, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Megahorn,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Physical, 0, 2, 120, 85,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.MegaKick,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 120, 75,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.MegaPunch,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 80, 85,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.MetalClaw,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Physical, 0, 7, 50, 95,
                    PBEMoveEffect.Hit__MaybeRaiseUser_ATK_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.MetalSound,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Status, 0, 8, 0, 85,
                    PBEMoveEffect.ChangeTarget_SPDEF, -2, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.SoundBased
                )
            },
            {
                PBEMove.MeteorMash,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Special, 0, 2, 100, 85,
                    PBEMoveEffect.Hit__MaybeRaiseUser_ATK_By1, 20, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Metronome,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.Metronome, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedByMetronome
                )
            },
            {
                PBEMove.MilkDrink,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.RestoreTargetHP, 50, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Minimize,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.ChangeTarget_EVA, +2, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.MiracleEye,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 8, 0, 0,
                    PBEMoveEffect.MiracleEye, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.MirrorShot,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Special, 0, 2, 65, 85,
                    PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.MistBall,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 1, 70, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Moonlight,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.Moonlight, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.MorningSun,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.Moonlight, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.MudBomb,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Special, 0, 2, 65, 85,
                    PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.MuddyWater,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 2, 95, 85,
                    PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 30, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.MudSlap,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Special, 0, 2, 20, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.MudShot,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Special, 0, 3, 55, 95,
                    PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.NastyPlot,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.ChangeTarget_SPATK, +2, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.NeedleArm,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Physical, 0, 3, 60, 100,
                    PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.NightDaze,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Special, 0, 2, 85, 95,
                    PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 40, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.NightShade,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Special, 0, 3, 0, 100,
                    PBEMoveEffect.SeismicToss, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.NightSlash,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 3, 70, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Octazooka,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 2, 65, 85,
                    PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.OminousWind,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Special, 0, 1, 60, 100,
                    PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Overheat,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 1, 140, 90,
                    PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.PainSplit,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.PainSplit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Peck,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Physical, 0, 7, 35, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.PoisonFang,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Physical, 0, 3, 50, 100,
                    PBEMoveEffect.Hit__MaybeToxic, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.PoisonGas,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Status, 0, 8, 0, 80,
                    PBEMoveEffect.Poison, 0, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.PoisonJab,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Physical, 0, 4, 80, 100,
                    PBEMoveEffect.Hit__MaybePoison, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.PoisonPowder,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Status, 0, 7, 0, 75,
                    PBEMoveEffect.Poison, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.PoisonSting,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Physical, 0, 7, 15, 100,
                    PBEMoveEffect.Hit__MaybePoison, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.PoisonTail,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Physical, 0, 5, 50, 100,
                    PBEMoveEffect.Hit__MaybePoison, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Pound,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 7, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.PowderSnow,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Special, 0, 5, 40, 100,
                    PBEMoveEffect.Hit__MaybeFreeze, 10, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.PowerGem,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Special, 0, 4, 70, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.PowerTrick,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.PowerTrick, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.PowerWhip,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Physical, 0, 2, 120, 85,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Protect,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, +4, 2, 0, 0,
                    PBEMoveEffect.Protect, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedByMetronome
                )
            },
            {
                PBEMove.Psybeam,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 4, 65, 100,
                    PBEMoveEffect.Hit__MaybeConfuse, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Psychic,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 2, 90, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.PsychoBoost,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 1, 140, 90,
                    PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.PsychoCut,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Physical, 0, 4, 70, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
                )
            },
            {
                PBEMove.PsychUp,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.PsychUp, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.Psyshock,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 2, 80, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Psystrike,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 2, 100, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Psywave,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 3, 0, 80,
                    PBEMoveEffect.Psywave, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Punishment,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 1, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.QuickAttack,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, +1, 6, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.QuiverDance,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.RaiseTarget_SPATK_SPDEF_SPE_By1, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.RainDance,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.RainDance, 0, PBEMoveTarget.All,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.RazorLeaf,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Physical, 0, 5, 55, 95,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
                )
            },
            {
                PBEMove.RazorShell,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Physical, 0, 2, 75, 95,
                    PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Recover,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.RestoreTargetHP, 50, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Reflect,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.Reflect, 0, PBEMoveTarget.AllTeam,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Rest,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.Rest, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Retaliate,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 70, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Return,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Reversal,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 3, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Roar,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, -6, 4, 0, 100,
                    PBEMoveEffect.Whirlwind, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.SoundBased
                )
            },
            {
                PBEMove.RockClimb,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 90, 85,
                    PBEMoveEffect.Hit__MaybeConfuse, 20, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.RockPolish,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.ChangeTarget_SPE, +2, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.RockSlide,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Physical, 0, 2, 75, 90,
                    PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.RockSmash,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 3, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.RockThrow,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Physical, 0, 3, 50, 90,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.RockTomb,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Physical, 0, 2, 50, 80,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.RollingKick,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 3, 60, 85,
                    PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.SacredFire,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Physical, 0, 1, 100, 95,
                    PBEMoveEffect.Hit__MaybeBurn, 50, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DefrostsUser
                )
            },
            {
                PBEMove.Safeguard,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 5, 0, 0,
                    PBEMoveEffect.Safeguard, 0, PBEMoveTarget.AllTeam,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.SandAttack,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Status, 0, 3, 0, 100,
                    PBEMoveEffect.ChangeTarget_ACC, -1, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Sandstorm,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.Sandstorm, 0, PBEMoveTarget.All,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.Scald,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 3, 80, 100,
                    PBEMoveEffect.Hit__MaybeBurn, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DefrostsUser
                )
            },
            {
                PBEMove.ScaryFace,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 100,
                    PBEMoveEffect.ChangeTarget_SPE, -2, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Scratch,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 8, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Screech,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 85,
                    PBEMoveEffect.ChangeTarget_DEF, -2, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.SoundBased
                )
            },
            {
                PBEMove.SearingShot,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 1, 100, 100,
                    PBEMoveEffect.Hit__MaybeBurn, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SecretSword,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Special, 0, 2, 85, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedByMetronome
                )
            },
            {
                PBEMove.SeedBomb,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Physical, 0, 3, 80, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SeedFlare,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 1, 120, 85,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2, 40, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SeismicToss,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 4, 0, 100,
                    PBEMoveEffect.SeismicToss, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Selfdestruct,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 200, 100,
                    PBEMoveEffect.Selfdestruct, 0, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.ShadowBall,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Special, 0, 3, 80, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 20, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.ShadowClaw,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Physical, 0, 3, 70, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.ShadowPunch,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Physical, 0, 4, 60, 0,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.ShadowSneak,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Physical, +1, 6, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Sharpen,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.ChangeTarget_ATK, +1, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.SheerCold,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Special, 0, 1, 0, 0,
                    PBEMoveEffect.OneHitKnockout, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.ShellSmash,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 0,
                    PBEMoveEffect.LowerTarget_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.ShiftGear,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.RaiseTarget_SPE_By2_ATK_By1, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.SignalBeam,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Special, 0, 3, 75, 100,
                    PBEMoveEffect.Hit__MaybeConfuse, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SilverWind,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Special, 0, 1, 60, 100,
                    PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SimpleBeam,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 100,
                    PBEMoveEffect.SimpleBeam, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Sing,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 55,
                    PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.SoundBased
                )
            },
            {
                PBEMove.ShockWave,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Special, 0, 4, 60, 0,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SkyUppercut,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 3, 85, 90,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsAirborne | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.SlackOff,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.RestoreTargetHP, 50, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Slam,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 80, 75,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Slash,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 70, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.SleepPowder,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 3, 0, 75,
                    PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Sludge,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Special, 0, 4, 65, 100,
                    PBEMoveEffect.Hit__MaybePoison, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SludgeBomb,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Special, 0, 2, 90, 100,
                    PBEMoveEffect.Hit__MaybePoison, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SludgeWave,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Special, 0, 2, 95, 100,
                    PBEMoveEffect.Hit__MaybePoison, 10, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Smog,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Special, 0, 4, 20, 70,
                    PBEMoveEffect.Hit__MaybePoison, 40, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SmokeScreen,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 100,
                    PBEMoveEffect.ChangeTarget_ACC, -1, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Snarl,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Special, 0, 3, 55, 95,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1, 100, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedByMetronome | PBEMoveFlag.SoundBased
                )
            },
            {
                PBEMove.Snore,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 3, 40, 100,
                    PBEMoveEffect.Snore, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedByMetronome | PBEMoveFlag.SoundBased
                )
            },
            {
                PBEMove.Soak,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Status, 0, 4, 0, 100,
                    PBEMoveEffect.Soak, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Softboiled,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.RestoreTargetHP, 50, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.SonicBoom,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 4, 0, 90,
                    PBEMoveEffect.SetDamage, 20, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SpacialRend,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Special, 0, 1, 100, 95,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
                )
            },
            {
                PBEMove.Spark,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Physical, 0, 4, 65, 100,
                    PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Spikes,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.Spikes, 0, PBEMoveTarget.AllFoes,
                    PBEMoveFlag.AffectedByMagicCoat
                )
            },
            {
                PBEMove.Splash,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 0,
                    PBEMoveEffect.Nothing, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.Spore,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 3, 0, 100,
                    PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.StealthRock,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.StealthRock, 0, PBEMoveTarget.AllFoes,
                    PBEMoveFlag.AffectedByMagicCoat
                )
            },
            {
                PBEMove.Steamroller,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Physical, 0, 4, 65, 100,
                    PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.SteelWing,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Physical, 0, 5, 70, 90,
                    PBEMoveEffect.Hit__MaybeRaiseUser_DEF_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Stomp,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 65, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.StoneEdge,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Physical, 0, 1, 100, 80,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
                )
            },
            {
                PBEMove.StoredPower,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 2, 20, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.StormThrow,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AlwaysCrit | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Strength,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 80, 90,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.StringShot,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Status, 0, 8, 0, 95,
                    PBEMoveEffect.ChangeTarget_SPE, -1, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Struggle,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 0, 50, 0,
                    PBEMoveEffect.Struggle, 0, PBEMoveTarget.RandomFoeSurrounding,
                    PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedByMetronome | PBEMoveFlag.MakesContact | PBEMoveFlag.UnaffectedByGems
                )
            },
            {
                PBEMove.StruggleBug,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Special, 0, 4, 30, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1, 100, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.StunSpore,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 6, 0, 75,
                    PBEMoveEffect.Paralyze, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Submission,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 5, 80, 80,
                    PBEMoveEffect.Recoil, 4, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Substitute,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.Substitute, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.SuckerPunch,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, +1, 1, 80, 100,
                    PBEMoveEffect.SuckerPunch, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.SunnyDay,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.SunnyDay, 0, PBEMoveTarget.All,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.SuperFang,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 0, 90,
                    PBEMoveEffect.SuperFang, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Superpower,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 1, 120, 100,
                    PBEMoveEffect.Hit__MaybeLowerUser_ATK_DEF_By1, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Supersonic,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 55,
                    PBEMoveEffect.Confuse, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.SoundBased
                )
            },
            {
                PBEMove.Surf,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 3, 95, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsUnderwater
                )
            },
            {
                PBEMove.Swagger,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 90,
                    PBEMoveEffect.Swagger, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SweetKiss,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 75,
                    PBEMoveEffect.Confuse, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SweetScent,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 5, 0, 100,
                    PBEMoveEffect.ChangeTarget_EVA, -1, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Swift,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 4, 60, 0,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SwordsDance,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.ChangeTarget_ATK, +2, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Synthesis,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.Moonlight, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Tackle,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 7, 50, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.TailGlow,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.ChangeTarget_SPATK, +3, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.TailWhip,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 100,
                    PBEMoveEffect.ChangeTarget_DEF, -1, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Tailwind,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.Tailwind, 0, PBEMoveTarget.AllTeam,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.TakeDown,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 90, 85,
                    PBEMoveEffect.Recoil, 4, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.TechnoBlast,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 1, 85, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedByMetronome
                )
            },
            {
                PBEMove.TeeterDance,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 100,
                    PBEMoveEffect.Confuse, 0, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Teleport,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.Teleport, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.Thunder,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Special, 0, 2, 120, 70,
                    PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsAirborne
                )
            },
            {
                PBEMove.Thunderbolt,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Special, 0, 3, 95, 100,
                    PBEMoveEffect.Hit__MaybeParalyze, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.ThunderFang,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Physical, 0, 3, 65, 95,
                    PBEMoveEffect.Hit__MaybeParalyze__10PercentFlinch, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.ThunderPunch,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Physical, 0, 3, 75, 100,
                    PBEMoveEffect.Hit__MaybeParalyze, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.ThunderShock,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Special, 0, 6, 40, 100,
                    PBEMoveEffect.Hit__MaybeParalyze, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.ThunderWave,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Status, 0, 4, 0, 100,
                    PBEMoveEffect.Paralyze, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Tickle,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 100,
                    PBEMoveEffect.LowerTarget_ATK_DEF_By1, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Toxic,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Status, 0, 2, 0, 90,
                    PBEMoveEffect.Toxic, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.ToxicSpikes,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.ToxicSpikes, 0, PBEMoveTarget.AllFoes,
                    PBEMoveFlag.AffectedByMagicCoat
                )
            },
            {
                PBEMove.Transform,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.Transform, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.BlockedByMetronome
                )
            },
            {
                PBEMove.TrickRoom,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, -7, 1, 0, 0,
                    PBEMoveEffect.TrickRoom, 0, PBEMoveTarget.All,
                    PBEMoveFlag.AffectedByMirrorMove
                )
            },
            {
                PBEMove.Twister,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Special, 0, 4, 40, 100,
                    PBEMoveEffect.Hit__MaybeFlinch, 20, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsAirborne
                )
            },
            {
                PBEMove.VacuumWave,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Special, +1, 6, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.VCreate,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Physical, 0, 1, 180, 95,
                    PBEMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedByMetronome | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Venoshock,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Special, 0, 2, 65, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.ViceGrip,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 6, 55, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.VineWhip,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Physical, 0, 3, 35, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.VitalThrow,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, -1, 2, 70, 0,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.VoltTackle,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Physical, 0, 3, 120, 100,
                    PBEMoveEffect.Recoil__10PercentParalyze, 3, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Waterfall,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Physical, 0, 3, 80, 100,
                    PBEMoveEffect.Hit__MaybeFlinch, 20, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.WaterGun,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 5, 40, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.WaterPulse,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 4, 60, 100,
                    PBEMoveEffect.Hit__MaybeConfuse, 20, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.WaterSpout,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 1, 150, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.WeatherBall,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 2, 50, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Whirlwind,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, -6, 4, 0, 100,
                    PBEMoveEffect.Whirlwind, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.WideGuard,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Status, +3, 2, 0, 0,
                    PBEMoveEffect.WideGuard, 0, PBEMoveTarget.AllTeam,
                    PBEMoveFlag.AffectedBySnatch | PBEMoveFlag.BlockedByMetronome
                )
            },
            {
                PBEMove.WildCharge,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Physical, 0, 3, 90, 100,
                    PBEMoveEffect.Recoil, 4, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.WillOWisp,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Status, 0, 3, 0, 75,
                    PBEMoveEffect.Burn, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.WingAttack,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Physical, 0, 7, 60, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Withdraw,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Status, 0, 8, 0, 0,
                    PBEMoveEffect.ChangeTarget_DEF, +1, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.WoodHammer,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Physical, 0, 3, 120, 100,
                    PBEMoveEffect.Recoil, 3, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.WorkUp,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.RaiseTarget_ATK_SPATK_By1, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.WorrySeed,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 2, 0, 100,
                    PBEMoveEffect.WorrySeed, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.WringOut,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 1, 0, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.XScissor,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Physical, 0, 3, 80, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.ZapCannon,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Special, 0, 1, 120, 50,
                    PBEMoveEffect.Hit__MaybeParalyze, 100, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.ZenHeadbutt,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Physical, 0, 3, 80, 90,
                    PBEMoveEffect.Hit__MaybeFlinch, 20, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            }
        });
    }
}
