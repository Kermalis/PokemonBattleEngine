using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed partial class PBEMoveData
    {
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
                    PBEMoveEffect.Acrobatics, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Acupressure,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleAllySurrounding,
                    PBEMoveFlag.None
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
                PBEMove.AfterYou,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.AllySwitch,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, +1, 3, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.None
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
                PBEMove.AquaRing,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
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
                PBEMove.ArmThrust,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 4, 15, 100,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Aromatherapy,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllTeam,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Assist,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSleepTalk
                )
            },
            {
                PBEMove.Assurance,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 2, 50, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.Autotomize,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Status, 0, 3, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Avalanche,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Physical, -4, 2, 60, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Barrage,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 15, 85,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.BatonPass,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.BeatUp,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 2, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.BellyDrum,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.BellyDrum, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Bestow,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
                )
            },
            {
                PBEMove.Bide,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, +1, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Bind,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 15, 85,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.BlastBurn,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 1, 150, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.NeverMissHail
                )
            },
            {
                PBEMove.Block,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove
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
                    PBEMoveEffect.Brine, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.Bonemerang,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Physical, 0, 2, 50, 90,
                    PBEMoveEffect.Hit__2Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.BoneRush,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Physical, 0, 2, 25, 90,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Bounce,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Physical, 0, 1, 85, 85,
                    PBEMoveEffect.Bounce, 30, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
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
                PBEMove.BugBite,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Physical, 0, 4, 60, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.BugBuzz,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Special, 0, 2, 90, 100,
                    PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
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
                PBEMove.BulletSeed,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Physical, 0, 6, 25, 100,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                PBEMove.Camouflage,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.Camouflage, 0, PBEMoveTarget.Self,
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
                PBEMove.Charge,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
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
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSketch | PBEMoveFlag.BlockedFromMimic
                )
            },
            {
                PBEMove.ChipAway,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 70, 100,
                    PBEMoveEffect.ChipAway, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.CircleThrow,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, -6, 2, 60, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Clamp,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Physical, 0, 3, 35, 85,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.ClearSmog,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Special, 0, 3, 50, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                PBEMove.CometPunch,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 18, 85,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.Conversion2,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.Copycat,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSleepTalk
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
                PBEMove.Counter,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, -5, 4, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Covet,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 8, 60, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.MakesContact
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
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 120, 100,
                    PBEMoveEffect.CrushGrip, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.Defog,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Status, 0, 3, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.DestinyBond,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
                )
            },
            {
                PBEMove.Detect,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Status, +4, 1, 0, 0,
                    PBEMoveEffect.Protect, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
                )
            },
            {
                PBEMove.Dig,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Physical, 0, 2, 80, 100,
                    PBEMoveEffect.Dig, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Disable,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
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
                PBEMove.DoomDesire,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Special, 0, 1, 140, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.DoubleHit,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 35, 90,
                    PBEMoveEffect.Hit__2Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.DoubleKick,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 6, 30, 100,
                    PBEMoveEffect.Hit__2Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.DoubleSlap,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 15, 85,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.DragonTail,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Physical, -6, 2, 60, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.MakesContact
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
                PBEMove.DualChop,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Physical, 0, 3, 40, 90,
                    PBEMoveEffect.Hit__2Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageUnderground
                )
            },
            {
                PBEMove.EchoedVoice,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 3, 40, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
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
                PBEMove.ElectroBall,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Special, 0, 2, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.Embargo,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Status, 0, 3, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                PBEMove.Encore,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                PBEMove.Endure,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, +4, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.Entrainment,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 100,
                    PBEMoveEffect.Entrainment, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Eruption,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 1, 150, 100,
                    PBEMoveEffect.Eruption, 0, PBEMoveTarget.AllFoesSurrounding,
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
                    PBEMoveEffect.Facade, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.FakeOut,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, +3, 2, 40, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.FalseSwipe,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 8, 40, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.Feint,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, +2, 2, 30, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.FirePledge,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 2, 50, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.UnaffectedByGems
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
                PBEMove.FireSpin,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 3, 35, 85,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Fissure,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Physical, 0, 1, 0, 30,
                    PBEMoveEffect.OneHitKnockout, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsUnderground
                )
            },
            {
                PBEMove.Flail,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 0, 100,
                    PBEMoveEffect.Flail, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FlameBurst,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 3, 70, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                PBEMove.Fling,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 2, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Fly,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Physical, 0, 3, 90, 95,
                    PBEMoveEffect.Fly, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
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
                PBEMove.FocusPunch,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, -3, 4, 150, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FollowMe,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, +3, 4, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.Foresight,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 0,
                    PBEMoveEffect.Foresight, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.FoulPlay,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 3, 95, 100,
                    PBEMoveEffect.FoulPlay, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FreezeShock,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Physical, 0, 1, 140, 90,
                    PBEMoveEffect.TODOMOVE, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.BlockedFromMetronome
                )
            },
            {
                PBEMove.FrenzyPlant,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 1, 150, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                    PBEMoveEffect.Frustration, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FuryAttack,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 15, 85,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FuryCutter,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Physical, 0, 4, 20, 95,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FurySwipes,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 18, 85,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.FusionBolt,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Physical, 0, 1, 100, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.FusionFlare,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 1, 100, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.FutureSight,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 2, 100, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.None
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
                PBEMove.GearGrind,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Physical, 0, 3, 50, 85,
                    PBEMoveEffect.Hit__2Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.GigaImpact,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 150, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                    PBEMoveEffect.GrassKnot, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.GrassPledge,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 2, 50, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.UnaffectedByGems
                )
            },
            {
                PBEMove.GrassWhistle,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 3, 0, 55,
                    PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
                )
            },
            {
                PBEMove.Gravity,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.All,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.Growl,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 100,
                    PBEMoveEffect.ChangeTarget_ATK, -1, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
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
                PBEMove.Grudge,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.GuardSplit,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.GuardSwap,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Guillotine,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 0, 30,
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
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageAirborne
                )
            },
            {
                PBEMove.GyroBall,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Physical, 0, 1, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.HealBell,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllTeam,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.HealBlock,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 3, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.HealingWish,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
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
                PBEMove.HeartSwap,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.HeatCrash,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Physical, 0, 2, 0, 100,
                    PBEMoveEffect.HeatCrash, 0, PBEMoveTarget.SingleSurrounding,
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
                    PBEMoveEffect.HeatCrash, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.HelpingHand,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, +5, 4, 0, 0,
                    PBEMoveEffect.HelpingHand, 0, PBEMoveTarget.SingleAllySurrounding,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
                )
            },
            {
                PBEMove.Hex,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Special, 0, 2, 50, 100,
                    PBEMoveEffect.Hex, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.HiddenPower,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 3, 0, 100,
                    PBEMoveEffect.HiddenPower, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.HiJumpKick,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 130, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
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
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 0, 30,
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
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsAirborne | PBEMoveFlag.NeverMissRain
                )
            },
            {
                PBEMove.HydroCannon,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 1, 150, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                PBEMove.HyperBeam,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 1, 150, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
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
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
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
                PBEMove.IceBall,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Physical, 0, 4, 30, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageUserDefenseCurl | PBEMoveFlag.MakesContact
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
                PBEMove.IceBurn,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Special, 0, 1, 140, 90,
                    PBEMoveEffect.TODOMOVE, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.IcicleSpear,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Physical, 0, 6, 25, 100,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.Imprison,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Incinerate,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 3, 30, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllFoesSurrounding,
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
                PBEMove.Ingrain,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
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
                    PBEMoveEffect.Judgment, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.JumpKick,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 100, 95,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
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
                PBEMove.KnockOff,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 4, 20, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.LastResort,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 140, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.LockOn,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.LockOn, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                    PBEMoveEffect.GrassKnot, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.LunarDance,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
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
                PBEMove.MagicCoat,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, +4, 3, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.MagicRoom,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, -7, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.All,
                    PBEMoveFlag.AffectedByMirrorMove
                )
            },
            {
                PBEMove.MagmaStorm,
                new PBEMoveData
                (
                    PBEType.Fire, PBEMoveCategory.Special, 0, 1, 120, 75,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
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
                    PBEMoveEffect.Magnitude, 0, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageUnderground
                )
            },
            {
                PBEMove.MeanLook,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove
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
                PBEMove.MeFirst,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleFoeSurrounding,
                    PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSleepTalk
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
                PBEMove.Memento,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Status, 0, 2, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.MetalBurst,
                new PBEMoveData
                (
                    PBEType.Steel, PBEMoveCategory.Physical, 0, 2, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromMeFirst
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
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
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
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromMimic | PBEMoveFlag.BlockedFromSketchWhenSuccessful | PBEMoveFlag.BlockedFromSleepTalk
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
                PBEMove.Mimic,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSketchWhenSuccessful | PBEMoveFlag.BlockedFromSleepTalk
                )
            },
            {
                PBEMove.MindReader,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.LockOn, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Minimize,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.Minimize, +2, PBEMoveTarget.Self,
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
                PBEMove.MirrorCoat,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, -5, 4, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome
                )
            },
            {
                PBEMove.MirrorMove,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSleepTalk
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
                PBEMove.Mist,
                new PBEMoveData
                (
                    PBEType.Ice, PBEMoveCategory.Status, 0, 6, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllTeam,
                    PBEMoveFlag.AffectedBySnatch
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
                PBEMove.MudSport,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Status, 0, 3, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.All,
                    PBEMoveFlag.None
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
                PBEMove.NaturalGift,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.NaturePower,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.Nightmare,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Status, 0, 3, 0, 100,
                    PBEMoveEffect.Nightmare, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.OdorSleuth,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 0,
                    PBEMoveEffect.Foresight, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                PBEMove.Outrage,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Physical, 0, 2, 120, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.RandomFoeSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.Payback,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 2, 50, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.PayDay,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 40, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.PerishSong,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.All,
                    PBEMoveFlag.AffectedBySoundproof
                )
            },
            {
                PBEMove.PetalDance,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 2, 120, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.RandomFoeSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.PinMissile,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Physical, 0, 4, 14, 85,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Pluck,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Physical, 0, 4, 60, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleNotSelf,
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
                PBEMove.PowerSplit,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.PowerSwap,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.Present,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 0, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Protect,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, +4, 2, 0, 0,
                    PBEMoveEffect.Protect, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.PsychoShift,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                    PBEMoveEffect.Psyshock, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Psystrike,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 2, 100, 100,
                    PBEMoveEffect.Psyshock, 0, PBEMoveTarget.SingleSurrounding,
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
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 1, 60, 100,
                    PBEMoveEffect.Punishment, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Pursuit,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 4, 40, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Quash,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Status, 0, 3, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.QuickGuard,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Status, +3, 3, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllTeam,
                    PBEMoveFlag.AffectedBySnatch | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.Rage,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 20, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.RagePowder,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Status, +3, 4, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.RapidSpin,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 8, 20, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.RazorWind,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 2, 80, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.HighCritChance
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
                PBEMove.Recycle,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
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
                PBEMove.ReflectType,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 0,
                    PBEMoveEffect.ReflectType, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Refresh,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.RelicSong,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 2, 75, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof | PBEMoveFlag.BlockedFromMetronome
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
                    PBEMoveEffect.Retaliate, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Return,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 0, 100,
                    PBEMoveEffect.Return, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Revenge,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, -4, 2, 60, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Reversal,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 3, 0, 100,
                    PBEMoveEffect.Flail, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Roar,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, -6, 4, 0, 100,
                    PBEMoveEffect.Whirlwind, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
                )
            },
            {
                PBEMove.RoarOfTime,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Special, 0, 1, 150, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.RockBlast,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Physical, 0, 2, 25, 90,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                PBEMove.RockWrecker,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Physical, 0, 1, 150, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.RolePlay,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.RolePlay, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.None
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
                PBEMove.Rollout,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Physical, 0, 4, 30, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageUserDefenseCurl | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Roost,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Round,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 3, 60, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
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
                PBEMove.SacredSword,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 4, 90, 100,
                    PBEMoveEffect.ChipAway, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.SandTomb,
                new PBEMoveData
                (
                    PBEType.Ground, PBEMoveCategory.Physical, 0, 3, 35, 85,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
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
                PBEMove.SecretPower,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 70, 100,
                    PBEMoveEffect.SecretPower, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SecretSword,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Special, 0, 2, 85, 100,
                    PBEMoveEffect.Psyshock, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.ShadowForce,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Physical, 0, 1, 120, 100,
                    PBEMoveEffect.ShadowForce, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
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
                    PBEType.Ice, PBEMoveCategory.Special, 0, 1, 0, 30,
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
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
                )
            },
            {
                PBEMove.Sketch,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 0, 0, 0,
                    PBEMoveEffect.Sketch, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromMimic | PBEMoveFlag.BlockedFromSketch | PBEMoveFlag.BlockedFromSleepTalk
                )
            },
            {
                PBEMove.SkillSwap,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SkullBash,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 100, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.SkyAttack,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Physical, 0, 1, 140, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk
                )
            },
            {
                PBEMove.SkyDrop,
                new PBEMoveData
                (
                    PBEType.Flying, PBEMoveCategory.Physical, 0, 2, 60, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleNotSelf,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
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
                PBEMove.SleepTalk,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSleepTalk
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
                PBEMove.SmackDown,
                new PBEMoveData
                (
                    PBEType.Rock, PBEMoveCategory.Physical, 0, 3, 50, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsAirborne
                )
            },
            {
                PBEMove.SmellingSalt,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 60, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof | PBEMoveFlag.BlockedFromMetronome
                )
            },
            {
                PBEMove.Snatch,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Status, +4, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
                )
            },
            {
                PBEMove.Snore,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 3, 40, 100,
                    PBEMoveEffect.Snore, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.SolarBeam,
                new PBEMoveData
                (
                    PBEType.Grass, PBEMoveCategory.Special, 0, 2, 120, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk
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
                PBEMove.SpiderWeb,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove
                )
            },
            {
                PBEMove.SpikeCannon,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 20, 100,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                PBEMove.Spite,
                new PBEMoveData
                (
                    PBEType.Ghost, PBEMoveCategory.Status, 0, 2, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.SpitUp,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 2, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageMinimized | PBEMoveFlag.MakesContact
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
                PBEMove.Stockpile,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
                )
            },
            {
                PBEMove.Stomp,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 65, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageMinimized | PBEMoveFlag.MakesContact
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
                    PBEMoveEffect.StoredPower, 0, PBEMoveTarget.SingleSurrounding,
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
                    PBEMoveEffect.Struggle, 4, PBEMoveTarget.RandomFoeSurrounding,
                    PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromMimic | PBEMoveFlag.BlockedFromSketch | PBEMoveFlag.MakesContact | PBEMoveFlag.UnaffectedByGems
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
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
                )
            },
            {
                PBEMove.Surf,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 3, 95, 100,
                    PBEMoveEffect.Hit, 0, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageUnderwater
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
                PBEMove.Swallow,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
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
                PBEMove.Switcheroo,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Status, 0, 2, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.Synchronoise,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Special, 0, 3, 70, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                PBEMove.TailSlap,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 25, 85,
                    PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.Taunt,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Status, 0, 4, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.TechnoBlast,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 1, 85, 100,
                    PBEMoveEffect.TechnoBlast, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.Telekinesis,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 3, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByProtect
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
                PBEMove.Thief,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Physical, 0, 2, 40, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Thrash,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 120, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.RandomFoeSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Thunder,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Special, 0, 2, 120, 70,
                    PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsAirborne | PBEMoveFlag.NeverMissRain
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
                    PBEMoveEffect.ThunderWave, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.Torment,
                new PBEMoveData
                (
                    PBEType.Dark, PBEMoveCategory.Status, 0, 3, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
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
                    PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromMimic | PBEMoveFlag.BlockedFromSketchWhenSuccessful
                )
            },
            {
                PBEMove.TriAttack,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 2, 80, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Trick,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.TripleKick,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 10, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.TrumpCard,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 1, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Twineedle,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Physical, 0, 4, 25, 100,
                    PBEMoveEffect.Hit__2Times__MaybePoison, 20, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Twister,
                new PBEMoveData
                (
                    PBEType.Dragon, PBEMoveCategory.Special, 0, 4, 40, 100,
                    PBEMoveEffect.Hit__MaybeFlinch, 20, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageAirborne
                )
            },
            {
                PBEMove.Uproar,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 2, 90, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.RandomFoeSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof | PBEMoveFlag.BlockedFromSleepTalk
                )
            },
            {
                PBEMove.Uturn,
                new PBEMoveData
                (
                    PBEType.Bug, PBEMoveCategory.Physical, 0, 4, 70, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.Venoshock,
                new PBEMoveData
                (
                    PBEType.Poison, PBEMoveCategory.Special, 0, 2, 65, 100,
                    PBEMoveEffect.Venoshock, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.VoltSwitch,
                new PBEMoveData
                (
                    PBEType.Electric, PBEMoveCategory.Special, 0, 4, 70, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
                PBEMove.WakeUpSlap,
                new PBEMoveData
                (
                    PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 60, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
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
                PBEMove.WaterPledge,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 2, 50, 100,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.UnaffectedByGems
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
                PBEMove.WaterSport,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Status, 0, 3, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.All,
                    PBEMoveFlag.None
                )
            },
            {
                PBEMove.WaterSpout,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 1, 150, 100,
                    PBEMoveEffect.Eruption, 0, PBEMoveTarget.AllFoesSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.WeatherBall,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 2, 50, 100,
                    PBEMoveEffect.WeatherBall, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
                )
            },
            {
                PBEMove.Whirlpool,
                new PBEMoveData
                (
                    PBEType.Water, PBEMoveCategory.Special, 0, 3, 35, 85,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageUnderwater
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
                    PBEMoveFlag.AffectedBySnatch | PBEMoveFlag.BlockedFromMetronome
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
                PBEMove.Wish,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
                    PBEMoveFlag.AffectedBySnatch
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
                PBEMove.WonderRoom,
                new PBEMoveData
                (
                    PBEType.Psychic, PBEMoveCategory.Status, -7, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.All,
                    PBEMoveFlag.AffectedByMirrorMove
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
                PBEMove.Wrap,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 15, 90,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
                )
            },
            {
                PBEMove.WringOut,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Special, 0, 1, 120, 100,
                    PBEMoveEffect.CrushGrip, 0, PBEMoveTarget.SingleSurrounding,
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
                PBEMove.Yawn,
                new PBEMoveData
                (
                    PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
                    PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
                    PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
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
