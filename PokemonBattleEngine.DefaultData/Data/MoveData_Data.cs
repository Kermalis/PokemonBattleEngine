using Kermalis.PokemonBattleEngine.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kermalis.PokemonBattleEngine.DefaultData.Data;

public sealed partial class PBEDDMoveData
{
	public static ReadOnlyDictionary<PBEMove, PBEDDMoveData> Data { get; } = new(new Dictionary<PBEMove, PBEDDMoveData>
	{
		{
			PBEMove.Absorb,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 5, 20, 100,
				PBEMoveEffect.HPDrain, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Acid,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Special, 0, 6, 40, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.AcidArmor,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Status, 0, 8, 0, 0,
				PBEMoveEffect.ChangeTarget_DEF, +2, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.AcidSpray,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Special, 0, 4, 40, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Acrobatics,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Physical, 0, 3, 55, 100,
				PBEMoveEffect.Acrobatics, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Acupressure,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleAllySurrounding,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.AerialAce,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Physical, 0, 4, 60, 0,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Aeroblast,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Special, 0, 1, 100, 95,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
			)
		},
		{
			PBEMove.AfterYou,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.Agility,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.ChangeTarget_SPE, +2, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.AirCutter,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Special, 0, 5, 55, 95,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
			)
		},
		{
			PBEMove.AirSlash,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Special, 0, 4, 75, 95,
				PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.AllySwitch,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, +1, 3, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.Amnesia,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.ChangeTarget_SPDEF, +2, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.AncientPower,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Special, 0, 1, 60, 100,
				PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.AquaJet,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Physical, +1, 4, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.AquaRing,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.AquaTail,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Physical, 0, 2, 90, 90,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.ArmThrust,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 4, 15, 100,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Aromatherapy,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllTeam,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Assist,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSleepTalk
			)
		},
		{
			PBEMove.Assurance,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 2, 50, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Astonish,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Physical, 0, 3, 30, 100,
				PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.AttackOrder,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Physical, 0, 3, 90, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
			)
		},
		{
			PBEMove.Attract,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 100,
				PBEMoveEffect.Attract, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.AuraSphere,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Special, 0, 4, 90, 0,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.AuroraBeam,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Special, 0, 4, 65, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_ATK_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Autotomize,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Status, 0, 3, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Avalanche,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Physical, -4, 2, 60, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Barrage,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 15, 85,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Barrier,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.ChangeTarget_DEF, +2, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.BatonPass,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.BeatUp,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 2, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.BellyDrum,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.BellyDrum, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Bestow,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.Bide,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, +1, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Bind,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 15, 85,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Bite,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 5, 60, 100,
				PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.BlastBurn,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 1, 150, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.BlazeKick,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Physical, 0, 2, 85, 90,
				PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Blizzard,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Special, 0, 1, 120, 70,
				PBEMoveEffect.Hit__MaybeFreeze, 10, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.NeverMissHail
			)
		},
		{
			PBEMove.Block,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove
			)
		},
		{
			PBEMove.BlueFlare,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 1, 130, 85,
				PBEMoveEffect.Hit__MaybeBurn, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.BrickBreak,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 3, 75, 100,
				PBEMoveEffect.BrickBreak, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Brine,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 2, 65, 100,
				PBEMoveEffect.Brine, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.BodySlam,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 85, 100,
				PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.BoltStrike,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Physical, 0, 1, 130, 85,
				PBEMoveEffect.Hit__MaybeParalyze, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.BoneClub,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Physical, 0, 4, 65, 85,
				PBEMoveEffect.Hit__MaybeFlinch, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Bonemerang,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Physical, 0, 2, 50, 90,
				PBEMoveEffect.Hit__2Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.BoneRush,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Physical, 0, 2, 25, 90,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Bounce,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Physical, 0, 1, 85, 85,
				PBEMoveEffect.Bounce, 30, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedByGravity | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.BraveBird,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Physical, 0, 3, 120, 100,
				PBEMoveEffect.Recoil, 3, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Bubble,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 6, 20, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 10, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.BubbleBeam,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 4, 65, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.BugBite,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Physical, 0, 4, 60, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.BugBuzz,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Special, 0, 2, 90, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
			)
		},
		{
			PBEMove.BulkUp,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.RaiseTarget_ATK_DEF_By1, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Bulldoze,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Physical, 0, 4, 60, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 100, PBEMoveTarget.AllSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.BulletPunch,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Physical, +1, 6, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.BulletSeed,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Physical, 0, 6, 25, 100,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.CalmMind,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.RaiseTarget_SPATK_SPDEF_By1, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Camouflage,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.Camouflage, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Captivate,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 100,
				PBEMoveEffect.ChangeTarget_SPATK__IfAttractionPossible, -2, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Charge,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.ChargeBeam,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Special, 0, 2, 50, 90,
				PBEMoveEffect.Hit__MaybeRaiseUser_SPATK_By1, 70, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Charm,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.ChangeTarget_ATK, -2, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Chatter,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Special, 0, 4, 60, 100,
				PBEMoveEffect.Hit__MaybeConfuse, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSketch | PBEMoveFlag.BlockedFromMimic
			)
		},
		{
			PBEMove.ChipAway,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 70, 100,
				PBEMoveEffect.ChipAway, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.CircleThrow,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, -6, 2, 60, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Clamp,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Physical, 0, 3, 35, 85,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.ClearSmog,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Special, 0, 3, 50, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.CloseCombat,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 1, 120, 100,
				PBEMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Coil,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.RaiseTarget_ATK_DEF_ACC_By1, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.CometPunch,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 18, 85,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.ConfuseRay,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Status, 0, 2, 0, 100,
				PBEMoveEffect.Confuse, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Confusion,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 5, 50, 100,
				PBEMoveEffect.Hit__MaybeConfuse, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Constrict,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 7, 10, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Conversion,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.Conversion, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Conversion2,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.Copycat,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSleepTalk
			)
		},
		{
			PBEMove.CosmicPower,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.RaiseTarget_DEF_SPDEF_By1, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.CottonGuard,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.ChangeTarget_DEF, +3, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.CottonSpore,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Status, 0, 8, 0, 100,
				PBEMoveEffect.ChangeTarget_SPE, -2, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Counter,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, -5, 4, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Covet,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 8, 60, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Crabhammer,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Physical, 0, 2, 90, 90,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.CrossChop,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 1, 100, 80,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.CrossPoison,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Physical, 0, 4, 70, 100,
				PBEMoveEffect.Hit__MaybePoison, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Crunch,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 3, 80, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.CrushClaw,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 75, 95,
				PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.CrushGrip,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 120, 100,
				PBEMoveEffect.CrushGrip, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Curse,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.Curse, 0, PBEMoveTarget.Varies,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.Cut,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 6, 50, 95,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DarkPulse,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Special, 0, 3, 80, 100,
				PBEMoveEffect.Hit__MaybeFlinch, 20, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.DarkVoid,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Status, 0, 2, 0, 80,
				PBEMoveEffect.Sleep, 0, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.DefendOrder,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.RaiseTarget_DEF_SPDEF_By1, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.DefenseCurl,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 0,
				PBEMoveEffect.ChangeTarget_DEF, +1, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Defog,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Status, 0, 3, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.DestinyBond,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.Detect,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Status, +4, 1, 0, 0,
				PBEMoveEffect.Protect, 0, PBEMoveTarget.Self,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.Dig,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Physical, 0, 2, 80, 100,
				PBEMoveEffect.Dig, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Disable,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Discharge,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Special, 0, 3, 80, 100,
				PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.AllSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Dive,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Physical, 0, 2, 80, 100,
				PBEMoveEffect.Dive, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DizzyPunch,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 70, 100,
				PBEMoveEffect.Hit__MaybeConfuse, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DoomDesire,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Special, 0, 1, 140, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DoubleEdge,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 120, 100,
				PBEMoveEffect.Recoil, 3, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DoubleHit,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 35, 90,
				PBEMoveEffect.Hit__2Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DoubleKick,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 6, 30, 100,
				PBEMoveEffect.Hit__2Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DoubleSlap,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 15, 85,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DoubleTeam,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 0,
				PBEMoveEffect.ChangeTarget_EVA, +1, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.DracoMeteor,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Special, 0, 1, 140, 90,
				PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.DragonBreath,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Special, 0, 4, 60, 100,
				PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.DragonClaw,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Physical, 0, 3, 80, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DragonDance,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.RaiseTarget_ATK_SPE_By1, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.DragonPulse,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Special, 0, 2, 90, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.DragonRage,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Special, 0, 2, 0, 100,
				PBEMoveEffect.SetDamage, 40, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.DragonRush,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Physical, 0, 2, 100, 75,
				PBEMoveEffect.Hit__MaybeFlinch, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DragonTail,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Physical, -6, 2, 60, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DrainPunch,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 75, 100,
				PBEMoveEffect.HPDrain, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DreamEater,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 3, 100, 100,
				PBEMoveEffect.HPDrain__RequireSleep, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.DrillPeck,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Physical, 0, 4, 80, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DrillRun,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Physical, 0, 2, 80, 95,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DualChop,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Physical, 0, 3, 40, 90,
				PBEMoveEffect.Hit__2Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.DynamicPunch,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 1, 100, 50,
				PBEMoveEffect.Hit__MaybeConfuse, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.EarthPower,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Special, 0, 2, 90, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Earthquake,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Physical, 0, 2, 100, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.AllSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageUnderground
			)
		},
		{
			PBEMove.EchoedVoice,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 3, 40, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
			)
		},
		{
			PBEMove.EggBomb,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 100, 75,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.ElectroBall,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Special, 0, 2, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Electroweb,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Special, 0, 3, 55, 95,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 100, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Embargo,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Status, 0, 3, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Ember,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 5, 40, 100,
				PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Encore,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Endeavor,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 0, 100,
				PBEMoveEffect.Endeavor, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Endure,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, +4, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.EnergyBall,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 2, 80, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Entrainment,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 100,
				PBEMoveEffect.Entrainment, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Eruption,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 1, 150, 100,
				PBEMoveEffect.Eruption, 0, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Explosion,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 250, 100,
				PBEMoveEffect.Selfdestruct, 0, PBEMoveTarget.AllSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Extrasensory,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 6, 80, 100,
				PBEMoveEffect.Hit__MaybeFlinch, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.ExtremeSpeed,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, +2, 1, 80, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Facade,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 70, 100,
				PBEMoveEffect.Facade, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FaintAttack,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 4, 60, 0,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FakeOut,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, +3, 2, 40, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FakeTears,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Status, 0, 4, 0, 100,
				PBEMoveEffect.ChangeTarget_SPDEF, -2, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.FalseSwipe,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 8, 40, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FeatherDance,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Status, 0, 3, 0, 100,
				PBEMoveEffect.ChangeTarget_ATK, -2, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Feint,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, +2, 2, 30, 100,
				PBEMoveEffect.Feint, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.FieryDance,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 2, 80, 100,
				PBEMoveEffect.Hit__MaybeRaiseUser_SPATK_By1, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.FinalGambit,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Special, 0, 1, 0, 100,
				PBEMoveEffect.FinalGambit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.FireBlast,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 1, 120, 85,
				PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.FireFang,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Physical, 0, 3, 65, 95,
				PBEMoveEffect.Hit__MaybeBurn__10PercentFlinch, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FirePledge,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 2, 50, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.UnaffectedByGems
			)
		},
		{
			PBEMove.FirePunch,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Physical, 0, 3, 75, 100,
				PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FireSpin,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 3, 35, 85,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Fissure,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Physical, 0, 1, 0, 30,
				PBEMoveEffect.OneHitKnockout, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsUnderground
			)
		},
		{
			PBEMove.Flail,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 0, 100,
				PBEMoveEffect.Flail, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FlameBurst,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 3, 70, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.FlameCharge,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Physical, 0, 4, 50, 100,
				PBEMoveEffect.Hit__MaybeRaiseUser_SPE_By1, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Flamethrower,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 3, 95, 100,
				PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.FlameWheel,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Physical, 0, 5, 60, 100,
				PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DefrostsUser | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FlareBlitz,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Physical, 0, 3, 120, 100,
				PBEMoveEffect.Recoil__10PercentBurn, 3, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.DefrostsUser | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Flash,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 100,
				PBEMoveEffect.ChangeTarget_ACC, -1, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.FlashCannon,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Special, 0, 2, 80, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Flatter,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Status, 0, 3, 0, 100,
				PBEMoveEffect.Flatter, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Fling,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 2, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Fly,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Physical, 0, 3, 90, 95,
				PBEMoveEffect.Fly, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedByGravity | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FocusBlast,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Special, 0, 1, 120, 70,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.FocusEnergy,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.FocusEnergy, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.FocusPunch,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, -3, 4, 150, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FollowMe,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, +3, 4, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.ForcePalm,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 60, 100,
				PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Foresight,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 0,
				PBEMoveEffect.Foresight, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.FoulPlay,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 3, 95, 100,
				PBEMoveEffect.FoulPlay, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FreezeShock,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Physical, 0, 1, 140, 90,
				PBEMoveEffect.TODOMOVE, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.FrenzyPlant,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 1, 150, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.FrostBreath,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Special, 0, 2, 40, 90,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AlwaysCrit
			)
		},
		{
			PBEMove.Frustration,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 0, 100,
				PBEMoveEffect.Frustration, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FuryAttack,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 15, 85,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FuryCutter,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Physical, 0, 4, 20, 95,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FurySwipes,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 18, 85,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.FusionBolt,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Physical, 0, 1, 100, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.FusionFlare,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 1, 100, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.FutureSight,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 2, 100, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.GastroAcid,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Status, 0, 2, 0, 100,
				PBEMoveEffect.GastroAcid, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.GearGrind,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Physical, 0, 3, 50, 85,
				PBEMoveEffect.Hit__2Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.GigaDrain,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 2, 75, 100,
				PBEMoveEffect.HPDrain, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.GigaImpact,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 150, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Glaciate,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Special, 0, 2, 65, 95,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 100, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Glare,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 90,
				PBEMoveEffect.Paralyze, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.GrassKnot,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 4, 0, 100,
				PBEMoveEffect.GrassKnot, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.GrassPledge,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 2, 50, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.UnaffectedByGems
			)
		},
		{
			PBEMove.GrassWhistle,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Status, 0, 3, 0, 55,
				PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
			)
		},
		{
			PBEMove.Gravity,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.All,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.Growl,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 100,
				PBEMoveEffect.ChangeTarget_ATK, -1, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
			)
		},
		{
			PBEMove.Growth,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Status, 0, 8, 0, 0,
				PBEMoveEffect.Growth, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Grudge,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.GuardSplit,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.GuardSwap,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Guillotine,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 0, 30,
				PBEMoveEffect.OneHitKnockout, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.GunkShot,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Physical, 0, 1, 120, 70,
				PBEMoveEffect.Hit__MaybePoison, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Gust,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Special, 0, 7, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageAirborne
			)
		},
		{
			PBEMove.GyroBall,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Physical, 0, 1, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Hail,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.Hail, 0, PBEMoveTarget.All,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.HammerArm,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 100, 90,
				PBEMoveEffect.Hit__MaybeLowerUser_SPE_By1, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Harden,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.ChangeTarget_DEF, +1, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Haze,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.Haze, 0, PBEMoveTarget.All,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.Headbutt,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 70, 100,
				PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.HeadCharge,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 120, 100,
				PBEMoveEffect.Recoil, 4, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.HeadSmash,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Physical, 0, 1, 150, 80,
				PBEMoveEffect.Recoil, 2, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.HealBell,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllTeam,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.HealBlock,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 3, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.HealingWish,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.HealOrder,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.RestoreTargetHP, 50, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.HealPulse,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.RestoreTargetHP, 50, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.HeartStamp,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Physical, 0, 5, 60, 100,
				PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.HeartSwap,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.HeatCrash,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Physical, 0, 2, 0, 100,
				PBEMoveEffect.HeatCrash, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.HeatWave,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 2, 100, 90,
				PBEMoveEffect.Hit__MaybeBurn, 10, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.HeavySlam,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Physical, 0, 2, 0, 100,
				PBEMoveEffect.HeatCrash, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.HelpingHand,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, +5, 4, 0, 0,
				PBEMoveEffect.HelpingHand, 0, PBEMoveTarget.SingleAllySurrounding,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.Hex,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Special, 0, 2, 50, 100,
				PBEMoveEffect.Hex, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.HiddenPower,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 3, 0, 100,
				PBEMoveEffect.HiddenPower, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.HiJumpKick,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 130, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.BlockedByGravity | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.HoneClaws,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Status, 0, 3, 0, 0,
				PBEMoveEffect.RaiseTarget_ATK_ACC_By1, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.HornAttack,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 5, 65, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.HornDrill,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 0, 30,
				PBEMoveEffect.OneHitKnockout, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.HornLeech,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Physical, 0, 2, 75, 100,
				PBEMoveEffect.HPDrain, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Howl,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 0,
				PBEMoveEffect.ChangeTarget_ATK, +1, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Hurricane,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Special, 0, 2, 120, 70,
				PBEMoveEffect.Hit__MaybeConfuse, 30, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsAirborne | PBEMoveFlag.NeverMissRain
			)
		},
		{
			PBEMove.HydroCannon,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 1, 150, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.HydroPump,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 1, 120, 80,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.HyperBeam,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 1, 150, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.HyperFang,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 80, 90,
				PBEMoveEffect.Hit__MaybeFlinch, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.HyperVoice,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 2, 90, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
			)
		},
		{
			PBEMove.Hypnosis,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 4, 0, 60,
				PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.IceBall,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Physical, 0, 4, 30, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageUserDefenseCurl | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.IceBeam,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Special, 0, 2, 95, 100,
				PBEMoveEffect.Hit__MaybeFreeze, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.IceBurn,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Special, 0, 1, 140, 90,
				PBEMoveEffect.TODOMOVE, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.IceFang,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Physical, 0, 3, 65, 95,
				PBEMoveEffect.Hit__MaybeFreeze__10PercentFlinch, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.IcePunch,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Physical, 0, 3, 75, 100,
				PBEMoveEffect.Hit__MaybeFreeze, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.IceShard,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Physical, +1, 6, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.IcicleCrash,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Physical, 0, 2, 85, 90,
				PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.IcicleSpear,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Physical, 0, 6, 25, 100,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.IcyWind,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Special, 0, 3, 55, 95,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 100, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Imprison,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Incinerate,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 3, 30, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Inferno,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 1, 100, 50,
				PBEMoveEffect.Hit__MaybeBurn, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Ingrain,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.IronDefense,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Status, 0, 3, 0, 0,
				PBEMoveEffect.ChangeTarget_DEF, +2, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.IronHead,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Physical, 0, 3, 80, 100,
				PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.IronTail,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Physical, 0, 3, 100, 75,
				PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Judgment,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 2, 100, 100,
				PBEMoveEffect.Judgment, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.JumpKick,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 100, 95,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.BlockedByGravity | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.KarateChop,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 5, 50, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Kinesis,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 3, 0, 80,
				PBEMoveEffect.ChangeTarget_ACC, -1, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.KnockOff,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 4, 20, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.LastResort,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 140, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.LavaPlume,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 3, 80, 100,
				PBEMoveEffect.Hit__MaybeBurn, 30, PBEMoveTarget.AllSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.LeafBlade,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Physical, 0, 3, 90, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.LeafStorm,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 1, 140, 90,
				PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.LeafTornado,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 2, 65, 90,
				PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.LeechLife,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Physical, 0, 3, 20, 100,
				PBEMoveEffect.HPDrain, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.LeechSeed,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Status, 0, 2, 0, 90,
				PBEMoveEffect.LeechSeed, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Leer,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 100,
				PBEMoveEffect.ChangeTarget_DEF, -1, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Lick,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Physical, 0, 6, 20, 100,
				PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.LightScreen,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.LightScreen, 0, PBEMoveTarget.AllTeam,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.LockOn,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.LockOn, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.LovelyKiss,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 75,
				PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.LowKick,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 4, 0, 100,
				PBEMoveEffect.GrassKnot, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.LowSweep,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 4, 60, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.LuckyChant,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.LuckyChant, 0, PBEMoveTarget.AllTeam,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.LunarDance,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.LusterPurge,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 1, 70, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.MachPunch,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, +1, 6, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.MagicalLeaf,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 4, 60, 0,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.MagicCoat,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, +4, 3, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.MagicRoom,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, -7, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.All,
				PBEMoveFlag.AffectedByMirrorMove
			)
		},
		{
			PBEMove.MagmaStorm,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 1, 120, 75,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.MagnetBomb,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Physical, 0, 4, 60, 0,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.MagnetRise,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.MagnetRise, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch | PBEMoveFlag.BlockedByGravity
			)
		},
		{
			PBEMove.Magnitude,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Physical, 0, 6, 0, 100,
				PBEMoveEffect.Magnitude, 0, PBEMoveTarget.AllSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageUnderground
			)
		},
		{
			PBEMove.MeanLook,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove
			)
		},
		{
			PBEMove.Meditate,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 8, 0, 0,
				PBEMoveEffect.ChangeTarget_ATK, +1, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.MeFirst,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleFoeSurrounding,
				PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSleepTalk
			)
		},
		{
			PBEMove.MegaDrain,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 3, 40, 100,
				PBEMoveEffect.HPDrain, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Megahorn,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Physical, 0, 2, 120, 85,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.MegaKick,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 120, 75,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.MegaPunch,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 80, 85,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Memento,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Status, 0, 2, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.MetalBurst,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Physical, 0, 2, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromMeFirst
			)
		},
		{
			PBEMove.MetalClaw,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Physical, 0, 7, 50, 95,
				PBEMoveEffect.Hit__MaybeRaiseUser_ATK_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.MetalSound,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Status, 0, 8, 0, 85,
				PBEMoveEffect.ChangeTarget_SPDEF, -2, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
			)
		},
		{
			PBEMove.MeteorMash,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Special, 0, 2, 100, 85,
				PBEMoveEffect.Hit__MaybeRaiseUser_ATK_By1, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Metronome,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.Metronome, 0, PBEMoveTarget.Self,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromMimic | PBEMoveFlag.BlockedFromSketchWhenSuccessful | PBEMoveFlag.BlockedFromSleepTalk
			)
		},
		{
			PBEMove.MilkDrink,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.RestoreTargetHP, 50, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Mimic,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSketchWhenSuccessful | PBEMoveFlag.BlockedFromSleepTalk
			)
		},
		{
			PBEMove.MindReader,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.LockOn, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Minimize,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.Minimize, +2, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.MiracleEye,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 8, 0, 0,
				PBEMoveEffect.MiracleEye, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.MirrorCoat,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, -5, 4, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.MirrorMove,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSleepTalk
			)
		},
		{
			PBEMove.MirrorShot,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Special, 0, 2, 65, 85,
				PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Mist,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllTeam,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.MistBall,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 1, 70, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Moonlight,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.Moonlight, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.MorningSun,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.Moonlight, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.MudBomb,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Special, 0, 2, 65, 85,
				PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.MuddyWater,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 2, 95, 85,
				PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 30, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.MudSlap,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Special, 0, 2, 20, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.MudSport,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Status, 0, 3, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.All,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.MudShot,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Special, 0, 3, 55, 95,
				PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.NastyPlot,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.ChangeTarget_SPATK, +2, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.NaturalGift,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.NaturePower,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.NeedleArm,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Physical, 0, 3, 60, 100,
				PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.NightDaze,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Special, 0, 2, 85, 95,
				PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 40, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Nightmare,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Status, 0, 3, 0, 100,
				PBEMoveEffect.Nightmare, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.NightShade,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Special, 0, 3, 0, 100,
				PBEMoveEffect.SeismicToss, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.NightSlash,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 3, 70, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Octazooka,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 2, 65, 85,
				PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.OdorSleuth,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 0,
				PBEMoveEffect.Foresight, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.OminousWind,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Special, 0, 1, 60, 100,
				PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Outrage,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Physical, 0, 2, 120, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.RandomFoeSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Overheat,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 1, 140, 90,
				PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.PainSplit,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.PainSplit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Payback,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 2, 50, 100,
				PBEMoveEffect.Payback, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.PayDay,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 40, 100,
				PBEMoveEffect.PayDay, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Peck,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Physical, 0, 7, 35, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.PerishSong,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.All,
				PBEMoveFlag.AffectedBySoundproof
			)
		},
		{
			PBEMove.PetalDance,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 2, 120, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.RandomFoeSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.PinMissile,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Physical, 0, 4, 14, 85,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Pluck,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Physical, 0, 4, 60, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.PoisonFang,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Physical, 0, 3, 50, 100,
				PBEMoveEffect.Hit__MaybeToxic, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.PoisonGas,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Status, 0, 8, 0, 80,
				PBEMoveEffect.Poison, 0, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.PoisonJab,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Physical, 0, 4, 80, 100,
				PBEMoveEffect.Hit__MaybePoison, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.PoisonPowder,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Status, 0, 7, 0, 75,
				PBEMoveEffect.Poison, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.PoisonSting,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Physical, 0, 7, 15, 100,
				PBEMoveEffect.Hit__MaybePoison, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.PoisonTail,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Physical, 0, 5, 50, 100,
				PBEMoveEffect.Hit__MaybePoison, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Pound,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 7, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.PowderSnow,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Special, 0, 5, 40, 100,
				PBEMoveEffect.Hit__MaybeFreeze, 10, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.PowerGem,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Special, 0, 4, 70, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.PowerSplit,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.PowerSwap,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.PowerTrick,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.PowerTrick, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.PowerWhip,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Physical, 0, 2, 120, 85,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Present,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 0, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Protect,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, +4, 2, 0, 0,
				PBEMoveEffect.Protect, 0, PBEMoveTarget.Self,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.Psybeam,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 4, 65, 100,
				PBEMoveEffect.Hit__MaybeConfuse, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Psychic,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 2, 90, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.PsychoBoost,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 1, 140, 90,
				PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.PsychoCut,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Physical, 0, 4, 70, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
			)
		},
		{
			PBEMove.PsychoShift,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.PsychUp,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.PsychUp, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.Psyshock,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 2, 80, 100,
				PBEMoveEffect.Psyshock, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Psystrike,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 2, 100, 100,
				PBEMoveEffect.Psyshock, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Psywave,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 3, 0, 80,
				PBEMoveEffect.Psywave, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Punishment,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 1, 60, 100,
				PBEMoveEffect.Punishment, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Pursuit,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 4, 40, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Quash,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Status, 0, 3, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.QuickAttack,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, +1, 6, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.QuickGuard,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Status, +3, 3, 0, 0,
				PBEMoveEffect.QuickGuard, 0, PBEMoveTarget.AllTeam,
				PBEMoveFlag.AffectedBySnatch | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.QuiverDance,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.RaiseTarget_SPATK_SPDEF_SPE_By1, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Rage,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 20, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.RagePowder,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Status, +3, 4, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.RainDance,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.RainDance, 0, PBEMoveTarget.All,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.RapidSpin,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 8, 20, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.RazorLeaf,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Physical, 0, 5, 55, 95,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
			)
		},
		{
			PBEMove.RazorShell,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Physical, 0, 2, 75, 95,
				PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.RazorWind,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 2, 80, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.HighCritChance
			)
		},
		{
			PBEMove.Recover,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.RestoreTargetHP, 50, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Recycle,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Reflect,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.Reflect, 0, PBEMoveTarget.AllTeam,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.ReflectType,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 0,
				PBEMoveEffect.ReflectType, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Refresh,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.Refresh, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.RelicSong,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 2, 75, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.Rest,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.Rest, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Retaliate,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 70, 100,
				PBEMoveEffect.Retaliate, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Return,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 0, 100,
				PBEMoveEffect.Return, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Revenge,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, -4, 2, 60, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Reversal,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 3, 0, 100,
				PBEMoveEffect.Flail, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Roar,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, -6, 4, 0, 100,
				PBEMoveEffect.Whirlwind, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
			)
		},
		{
			PBEMove.RoarOfTime,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Special, 0, 1, 150, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.RockBlast,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Physical, 0, 2, 25, 90,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.RockClimb,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 90, 85,
				PBEMoveEffect.Hit__MaybeConfuse, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.RockPolish,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.ChangeTarget_SPE, +2, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.RockSlide,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Physical, 0, 2, 75, 90,
				PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.RockSmash,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 3, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.RockThrow,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Physical, 0, 3, 50, 90,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.RockTomb,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Physical, 0, 2, 50, 80,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.RockWrecker,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Physical, 0, 1, 150, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.RolePlay,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.RolePlay, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.RollingKick,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 3, 60, 85,
				PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Rollout,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Physical, 0, 4, 30, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageUserDefenseCurl | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Roost,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.Roost, 50, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Round,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 3, 60, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
			)
		},
		{
			PBEMove.SacredFire,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Physical, 0, 1, 100, 95,
				PBEMoveEffect.Hit__MaybeBurn, 50, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DefrostsUser
			)
		},
		{
			PBEMove.SacredSword,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 4, 90, 100,
				PBEMoveEffect.ChipAway, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Safeguard,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 5, 0, 0,
				PBEMoveEffect.Safeguard, 0, PBEMoveTarget.AllTeam,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.SandAttack,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Status, 0, 3, 0, 100,
				PBEMoveEffect.ChangeTarget_ACC, -1, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Sandstorm,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.Sandstorm, 0, PBEMoveTarget.All,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.SandTomb,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Physical, 0, 3, 35, 85,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Scald,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 3, 80, 100,
				PBEMoveEffect.Hit__MaybeBurn, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DefrostsUser
			)
		},
		{
			PBEMove.ScaryFace,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 100,
				PBEMoveEffect.ChangeTarget_SPE, -2, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Scratch,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 8, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Screech,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 85,
				PBEMoveEffect.ChangeTarget_DEF, -2, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
			)
		},
		{
			PBEMove.SearingShot,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Special, 0, 1, 100, 100,
				PBEMoveEffect.Hit__MaybeBurn, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SecretPower,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 70, 100,
				PBEMoveEffect.SecretPower, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SecretSword,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Special, 0, 2, 85, 100,
				PBEMoveEffect.Psyshock, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.SeedBomb,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Physical, 0, 3, 80, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SeedFlare,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 1, 120, 85,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2, 40, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SeismicToss,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 4, 0, 100,
				PBEMoveEffect.SeismicToss, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Selfdestruct,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 1, 200, 100,
				PBEMoveEffect.Selfdestruct, 0, PBEMoveTarget.AllSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.ShadowBall,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Special, 0, 3, 80, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.ShadowClaw,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Physical, 0, 3, 70, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.ShadowForce,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Physical, 0, 1, 120, 100,
				PBEMoveEffect.ShadowForce, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.ShadowPunch,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Physical, 0, 4, 60, 0,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.ShadowSneak,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Physical, +1, 6, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Sharpen,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.ChangeTarget_ATK, +1, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.SheerCold,
			new PBEDDMoveData
			(
				PBEType.Ice, PBEMoveCategory.Special, 0, 1, 0, 30,
				PBEMoveEffect.OneHitKnockout, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.ShellSmash,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 0,
				PBEMoveEffect.LowerTarget_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.ShiftGear,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.RaiseTarget_SPE_By2_ATK_By1, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.SignalBeam,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Special, 0, 3, 75, 100,
				PBEMoveEffect.Hit__MaybeConfuse, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SilverWind,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Special, 0, 1, 60, 100,
				PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SimpleBeam,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 100,
				PBEMoveEffect.SimpleBeam, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Sing,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 55,
				PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
			)
		},
		{
			PBEMove.Sketch,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 0, 0, 0,
				PBEMoveEffect.Sketch, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromMimic | PBEMoveFlag.BlockedFromSketch | PBEMoveFlag.BlockedFromSleepTalk
			)
		},
		{
			PBEMove.SkillSwap,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SkullBash,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 100, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.SkyAttack,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Physical, 0, 1, 140, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk
			)
		},
		{
			PBEMove.SkyDrop,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Physical, 0, 2, 60, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedByGravity | PBEMoveFlag.BlockedFromSleepTalk | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.ShockWave,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Special, 0, 4, 60, 0,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SkyUppercut,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 3, 85, 90,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsAirborne | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.SlackOff,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.RestoreTargetHP, 50, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Slam,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 80, 75,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Slash,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 70, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.SleepPowder,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Status, 0, 3, 0, 75,
				PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SleepTalk,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromSleepTalk
			)
		},
		{
			PBEMove.Sludge,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Special, 0, 4, 65, 100,
				PBEMoveEffect.Hit__MaybePoison, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SludgeBomb,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Special, 0, 2, 90, 100,
				PBEMoveEffect.Hit__MaybePoison, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SmackDown,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Physical, 0, 3, 50, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsAirborne
			)
		},
		{
			PBEMove.SmellingSalt,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 60, 100,
				PBEMoveEffect.SmellingSalt, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.SludgeWave,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Special, 0, 2, 95, 100,
				PBEMoveEffect.Hit__MaybePoison, 10, PBEMoveTarget.AllSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Smog,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Special, 0, 4, 20, 70,
				PBEMoveEffect.Hit__MaybePoison, 40, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SmokeScreen,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 100,
				PBEMoveEffect.ChangeTarget_ACC, -1, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Snarl,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Special, 0, 3, 55, 95,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1, 100, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.Snatch,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Status, +4, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.Snore,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 3, 40, 100,
				PBEMoveEffect.Snore, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.Soak,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Status, 0, 4, 0, 100,
				PBEMoveEffect.Soak, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Softboiled,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.RestoreTargetHP, 50, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.SolarBeam,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Special, 0, 2, 120, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromSleepTalk
			)
		},
		{
			PBEMove.SonicBoom,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 4, 0, 90,
				PBEMoveEffect.SetDamage, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SpacialRend,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Special, 0, 1, 100, 95,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
			)
		},
		{
			PBEMove.Spark,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Physical, 0, 4, 65, 100,
				PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.SpiderWeb,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove
			)
		},
		{
			PBEMove.SpikeCannon,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 20, 100,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Spikes,
			new PBEDDMoveData
			(
				PBEType.Ground, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.Spikes, 0, PBEMoveTarget.AllFoes,
				PBEMoveFlag.AffectedByMagicCoat
			)
		},
		{
			PBEMove.Spite,
			new PBEDDMoveData
			(
				PBEType.Ghost, PBEMoveCategory.Status, 0, 2, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SpitUp,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 2, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Splash,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 8, 0, 0,
				PBEMoveEffect.Nothing, 0, PBEMoveTarget.Self,
				PBEMoveFlag.BlockedByGravity
			)
		},
		{
			PBEMove.Spore,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Status, 0, 3, 0, 100,
				PBEMoveEffect.Sleep, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.StealthRock,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.StealthRock, 0, PBEMoveTarget.AllFoes,
				PBEMoveFlag.AffectedByMagicCoat
			)
		},
		{
			PBEMove.Steamroller,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Physical, 0, 4, 65, 100,
				PBEMoveEffect.Hit__MaybeFlinch, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageMinimized | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.SteelWing,
			new PBEDDMoveData
			(
				PBEType.Steel, PBEMoveCategory.Physical, 0, 5, 70, 90,
				PBEMoveEffect.Hit__MaybeRaiseUser_DEF_By1, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Stockpile,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Stomp,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 65, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageMinimized | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.StoneEdge,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Physical, 0, 1, 100, 80,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HighCritChance
			)
		},
		{
			PBEMove.StoredPower,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 2, 20, 100,
				PBEMoveEffect.StoredPower, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.StormThrow,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AlwaysCrit | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Strength,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 3, 80, 90,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.StringShot,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Status, 0, 8, 0, 95,
				PBEMoveEffect.ChangeTarget_SPE, -1, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Struggle,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 0, 50, 0,
				PBEMoveEffect.Struggle, 4, PBEMoveTarget.RandomFoeSurrounding,
				PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromMimic | PBEMoveFlag.BlockedFromSketch | PBEMoveFlag.MakesContact | PBEMoveFlag.UnaffectedByGems
			)
		},
		{
			PBEMove.StruggleBug,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Special, 0, 4, 30, 100,
				PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1, 100, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.StunSpore,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Status, 0, 6, 0, 75,
				PBEMoveEffect.Paralyze, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Submission,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 5, 80, 80,
				PBEMoveEffect.Recoil, 4, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Substitute,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.Substitute, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.SuckerPunch,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, +1, 1, 80, 100,
				PBEMoveEffect.SuckerPunch, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.SunnyDay,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.SunnyDay, 0, PBEMoveTarget.All,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.SuperFang,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 0, 90,
				PBEMoveEffect.SuperFang, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Superpower,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 1, 120, 100,
				PBEMoveEffect.Hit__MaybeLowerUser_ATK_DEF_By1, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Supersonic,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 55,
				PBEMoveEffect.Confuse, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof
			)
		},
		{
			PBEMove.Surf,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 3, 95, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.AllSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageUnderwater
			)
		},
		{
			PBEMove.Swagger,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 3, 0, 90,
				PBEMoveEffect.Swagger, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Swallow,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.SweetKiss,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 75,
				PBEMoveEffect.Confuse, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.SweetScent,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 5, 0, 100,
				PBEMoveEffect.ChangeTarget_EVA, -1, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Swift,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 4, 60, 0,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Switcheroo,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Status, 0, 2, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.SwordsDance,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.ChangeTarget_ATK, +2, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Synchronoise,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Special, 0, 3, 70, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.AllSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Synthesis,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Status, 0, 1, 0, 0,
				PBEMoveEffect.Moonlight, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Tackle,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 7, 50, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.TailGlow,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.ChangeTarget_SPATK, +3, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.TailSlap,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 25, 85,
				PBEMoveEffect.Hit__2To5Times, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.TailWhip,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 100,
				PBEMoveEffect.ChangeTarget_DEF, -1, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Tailwind,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.Tailwind, 0, PBEMoveTarget.AllTeam,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.TakeDown,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 90, 85,
				PBEMoveEffect.Recoil, 4, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Taunt,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Status, 0, 4, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.TechnoBlast,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 1, 85, 100,
				PBEMoveEffect.TechnoBlast, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.TeeterDance,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 100,
				PBEMoveEffect.Confuse, 0, PBEMoveTarget.AllSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Telekinesis,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 3, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedByGravity
			)
		},
		{
			PBEMove.Teleport,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.Teleport, 0, PBEMoveTarget.Self,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.Thief,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Physical, 0, 2, 40, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMeFirst | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Thrash,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 2, 120, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.RandomFoeSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Thunder,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Special, 0, 2, 120, 70,
				PBEMoveEffect.Hit__MaybeParalyze, 30, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.HitsAirborne | PBEMoveFlag.NeverMissRain
			)
		},
		{
			PBEMove.Thunderbolt,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Special, 0, 3, 95, 100,
				PBEMoveEffect.Hit__MaybeParalyze, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.ThunderFang,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Physical, 0, 3, 65, 95,
				PBEMoveEffect.Hit__MaybeParalyze__10PercentFlinch, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.ThunderPunch,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Physical, 0, 3, 75, 100,
				PBEMoveEffect.Hit__MaybeParalyze, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByIronFist | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.ThunderShock,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Special, 0, 6, 40, 100,
				PBEMoveEffect.Hit__MaybeParalyze, 10, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.ThunderWave,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Status, 0, 4, 0, 100,
				PBEMoveEffect.ThunderWave, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Tickle,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 4, 0, 100,
				PBEMoveEffect.LowerTarget_ATK_DEF_By1, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Torment,
			new PBEDDMoveData
			(
				PBEType.Dark, PBEMoveCategory.Status, 0, 3, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Toxic,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Status, 0, 2, 0, 90,
				PBEMoveEffect.Toxic, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.ToxicSpikes,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Status, 0, 4, 0, 0,
				PBEMoveEffect.ToxicSpikes, 0, PBEMoveTarget.AllFoes,
				PBEMoveFlag.AffectedByMagicCoat
			)
		},
		{
			PBEMove.Transform,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.Transform, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.BlockedFromMimic | PBEMoveFlag.BlockedFromSketchWhenSuccessful
			)
		},
		{
			PBEMove.TriAttack,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 2, 80, 100,
				PBEMoveEffect.Hit__MaybeBurnFreezeParalyze, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Trick,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, 0, 2, 0, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromAssist | PBEMoveFlag.BlockedFromCopycat | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.TrickRoom,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, -7, 1, 0, 0,
				PBEMoveEffect.TrickRoom, 0, PBEMoveTarget.All,
				PBEMoveFlag.AffectedByMirrorMove
			)
		},
		{
			PBEMove.TripleKick,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 10, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.TrumpCard,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 1, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Twineedle,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Physical, 0, 4, 25, 100,
				PBEMoveEffect.Hit__2Times__MaybePoison, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Twister,
			new PBEDDMoveData
			(
				PBEType.Dragon, PBEMoveCategory.Special, 0, 4, 40, 100,
				PBEMoveEffect.Hit__MaybeFlinch, 20, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageAirborne
			)
		},
		{
			PBEMove.Uproar,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 2, 90, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.RandomFoeSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedBySoundproof | PBEMoveFlag.BlockedFromSleepTalk
			)
		},
		{
			PBEMove.Uturn,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Physical, 0, 4, 70, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.VacuumWave,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Special, +1, 6, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.VCreate,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Physical, 0, 1, 180, 95,
				PBEMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.BlockedFromMetronome | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Venoshock,
			new PBEDDMoveData
			(
				PBEType.Poison, PBEMoveCategory.Special, 0, 2, 65, 100,
				PBEMoveEffect.Venoshock, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.ViceGrip,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 6, 55, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.VineWhip,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Physical, 0, 3, 35, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.VitalThrow,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, -1, 2, 70, 0,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.VoltSwitch,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Special, 0, 4, 70, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.VoltTackle,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Physical, 0, 3, 120, 100,
				PBEMoveEffect.Recoil__10PercentParalyze, 3, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.WakeUpSlap,
			new PBEDDMoveData
			(
				PBEType.Fighting, PBEMoveCategory.Physical, 0, 2, 60, 100,
				PBEMoveEffect.WakeUpSlap, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Waterfall,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Physical, 0, 3, 80, 100,
				PBEMoveEffect.Hit__MaybeFlinch, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.WaterGun,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 5, 40, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.WaterPledge,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 2, 50, 100,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.UnaffectedByGems
			)
		},
		{
			PBEMove.WaterPulse,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 4, 60, 100,
				PBEMoveEffect.Hit__MaybeConfuse, 20, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.WaterSport,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Status, 0, 3, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.All,
				PBEMoveFlag.None
			)
		},
		{
			PBEMove.WaterSpout,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 1, 150, 100,
				PBEMoveEffect.Eruption, 0, PBEMoveTarget.AllFoesSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.WeatherBall,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 2, 50, 100,
				PBEMoveEffect.WeatherBall, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Whirlpool,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Special, 0, 3, 35, 85,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.DoubleDamageUnderwater
			)
		},
		{
			PBEMove.Whirlwind,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, -6, 4, 0, 100,
				PBEMoveEffect.Whirlwind, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.WideGuard,
			new PBEDDMoveData
			(
				PBEType.Rock, PBEMoveCategory.Status, +3, 2, 0, 0,
				PBEMoveEffect.WideGuard, 0, PBEMoveTarget.AllTeam,
				PBEMoveFlag.AffectedBySnatch | PBEMoveFlag.BlockedFromMetronome
			)
		},
		{
			PBEMove.WildCharge,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Physical, 0, 3, 90, 100,
				PBEMoveEffect.Recoil, 4, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.WillOWisp,
			new PBEDDMoveData
			(
				PBEType.Fire, PBEMoveCategory.Status, 0, 3, 0, 75,
				PBEMoveEffect.Burn, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.WingAttack,
			new PBEDDMoveData
			(
				PBEType.Flying, PBEMoveCategory.Physical, 0, 7, 60, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleNotSelf,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Wish,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.Withdraw,
			new PBEDDMoveData
			(
				PBEType.Water, PBEMoveCategory.Status, 0, 8, 0, 0,
				PBEMoveEffect.ChangeTarget_DEF, +1, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.WonderRoom,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Status, -7, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.All,
				PBEMoveFlag.AffectedByMirrorMove
			)
		},
		{
			PBEMove.WoodHammer,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Physical, 0, 3, 120, 100,
				PBEMoveEffect.Recoil, 3, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.AffectedByReckless | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.WorkUp,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 6, 0, 0,
				PBEMoveEffect.RaiseTarget_ATK_SPATK_By1, 0, PBEMoveTarget.Self,
				PBEMoveFlag.AffectedBySnatch
			)
		},
		{
			PBEMove.WorrySeed,
			new PBEDDMoveData
			(
				PBEType.Grass, PBEMoveCategory.Status, 0, 2, 0, 100,
				PBEMoveEffect.WorrySeed, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.Wrap,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Physical, 0, 4, 15, 90,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.WringOut,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Special, 0, 1, 120, 100,
				PBEMoveEffect.CrushGrip, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.XScissor,
			new PBEDDMoveData
			(
				PBEType.Bug, PBEMoveCategory.Physical, 0, 3, 80, 100,
				PBEMoveEffect.Hit, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		},
		{
			PBEMove.Yawn,
			new PBEDDMoveData
			(
				PBEType.Normal, PBEMoveCategory.Status, 0, 2, 0, 0,
				PBEMoveEffect.TODOMOVE, 0, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMagicCoat | PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.ZapCannon,
			new PBEDDMoveData
			(
				PBEType.Electric, PBEMoveCategory.Special, 0, 1, 120, 50,
				PBEMoveEffect.Hit__MaybeParalyze, 100, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect
			)
		},
		{
			PBEMove.ZenHeadbutt,
			new PBEDDMoveData
			(
				PBEType.Psychic, PBEMoveCategory.Physical, 0, 3, 80, 90,
				PBEMoveEffect.Hit__MaybeFlinch, 20, PBEMoveTarget.SingleSurrounding,
				PBEMoveFlag.AffectedByMirrorMove | PBEMoveFlag.AffectedByProtect | PBEMoveFlag.MakesContact
			)
		}
	});
}
