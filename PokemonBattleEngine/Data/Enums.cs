using System;

namespace Kermalis.PokemonBattleEngine.Data
{
    public enum PGender : byte
    {
        Male = 0x00,
        M7F1 = 0x1F, // Male 7:1 Female
        M3F1 = 0x3F, // Male 3:1 Female
        M1F1 = 0x7F, // Male 1:1 Female
        M1F3 = 0xBF, // Male 1:3 Female
        M1F7 = 0xE1, // Male 1:7 Female // Does not exist before gen 6
        Female = 0xFE,
        Genderless = 0xFF
    }
    public enum PStat : byte
    {
        HP,
        Attack,
        Defense,
        SpAttack,
        SpDefense,
        Speed, // TODO
        Accuracy, // TODO
        Evasion // TODO
    }
    public enum PEffectiveness : byte
    {
        Ineffective,
        NotVeryEffective,
        Normal,
        SuperEffective
    }
    public enum PBattleStyle : byte
    {
        Single,
        Double,
        Triple,
        Rotation
    }
    public enum PWeather : byte
    {
        None,
        Raining,
        Sunny,
    }
    public enum PFieldPosition : byte
    {
        // Not on the field.
        None,
        // In a double, triple, or rotation battle, the pokemon to __your__ left.
        Left,
        // In a single battle, pokemon are in the center.
        // In a double battle, no pokemon are in the center.
        // In a triple or rotation battle, it is obvious.
        Center,
        // In a double, triple, or rotation battle, the pokemon to __your__ right.
        Right
    }
    [Flags]
    public enum PTarget : byte
    {
        AllyLeft = 1 << 0,
        AllyCenter = 1 << 1,
        AllyRight = 1 << 2,
        FoeLeft = 1 << 3,
        FoeCenter = 1 << 4,
        FoeRight = 1 << 5
    }
    public enum PDecision : byte
    {
        None,
        Fight,
    }
    public enum PMoveCategory : byte
    {
        Status,
        Physical,
        Special
    }
    public enum PStatus1 : byte
    {
        None,
        Asleep,
        BadlyPoisoned,
        Burned,
        Frozen,
        Paralyzed,
        Poisoned
    }
    [Flags]
    public enum PStatus2 : uint
    {
        None,
        Confused = 1 << 0,
        Cursed = 1 << 1, // TODO
        Flinching = 1 << 2,
        Pumped = 1 << 3, // Focus Energy / Lansat Berry
        Infatuated = 1 << 4, // TODO
        Protected = 1 << 5,
        Substitute = 1 << 6,
        Tormented = 1 << 7, // TODO
        Transformed = 1 << 8,
        Underground = 1 << 9, // TODO
        Underwater = 1 << 10,
    }
    public enum PStatusAction : byte
    {
        Activated, // flinch prevented movement, protect activated, etc
        Added, // protected itself, became paralyzed, etc
        Cured, // limber curing paralysis
        Damage, // hurt from confusion, substitute took damage, etc
        Ended // woke up, no longer confused, etc
    }
    public enum PReflectLightScreenAction : byte
    {
        Added,
        Broke,
        Ended
    }
    public enum PFailReason : byte
    {
        Default, // "But it failed!" and nothing else
        HPFull, // Trying to use a healing move with max HP
    }
    public enum PWeatherAction : byte
    {
        Added,
        Ended
    }
    public enum PType : byte
    {
        Bug,
        Dark,
        Dragon,
        Electric,
        Fighting,
        Fire,
        Flying,
        Ghost,
        Grass,
        Ground,
        Ice,
        Normal,
        Poison,
        Psychic,
        Rock,
        Steel,
        Water
    }
    public enum PNature : byte
    {
        Adamant,
        Bashful,
        Bold,
        Brave,
        Calm,
        Careful,
        Docile,
        Gentle,
        Hardy,
        Hasty,
        Impish,
        Jolly,
        Lax,
        Loney,
        Mild,
        Modest,
        Naive,
        Naughty,
        Quiet,
        Quirky,
        Rash,
        Relaxed,
        Sassy,
        Serious,
        Timid,
        MAX
    }
    public enum PItem : ushort
    {
        None,
        BrightPowder,
        ChoiceBand, // TODO: Lock move
        DampRock,
        DeepSeaScale,
        DeepSeaTooth,
        HeatRock,
        LaxIncense,
        Leftovers,
        LightBall,
        LightClay,
        MetalPowder,
        PowerHerb,
        RazorClaw,
        ScopeLens,
        SoulDew,
        ThickClub,
        WideLens,
        MAX
    }
    public enum PAbility : byte
    {
        None, // Only used when an ability is removed
        BadDreams, // TODO
        BattleArmor,
        Blaze,
        Compoundeyes,
        Guts,
        HugePower,
        Hustle,
        Imposter, // TODO
        Levitate, // TODO
        LightningRod, // TODO
        Limber,
        MarvelScale,
        Minus, // TODO
        NoGuard,
        Overgrow,
        Plus, // TODO
        PurePower,
        Rattled, // TODO
        RockHead, // TODO
        SapSipper, // TODO
        ShellArmor,
        Simple,
        Sniper,
        Static, // TODO
        SuperLuck,
        Swarm,
        ThickFat,
        Torrent,
        MAX,
    }
    public enum PSpecies : uint
    {
        None,
        Pikachu = 25, // TODO
        Cubone = 104, // TODO
        Marowak, // TODO
        Ditto = 132,
        Pichu = 172, // TODO
        Azumarill = 184, // TODO
        Unown_A = 201 | (0 << 0x10),
        Unown_B = 201 | (1 << 0x10),
        Unown_C = 201 | (2 << 0x10),
        Unown_D = 201 | (3 << 0x10),
        Unown_E = 201 | (4 << 0x10),
        Unown_F = 201 | (5 << 0x10),
        Unown_G = 201 | (6 << 0x10),
        Unown_H = 201 | (7 << 0x10),
        Unown_I = 201 | (8 << 0x10),
        Unown_J = 201 | (9 << 0x10),
        Unown_K = 201 | (10 << 0x10),
        Unown_L = 201 | (11 << 0x10),
        Unown_M = 201 | (12 << 0x10),
        Unown_N = 201 | (13 << 0x10),
        Unown_O = 201 | (14 << 0x10),
        Unown_P = 201 | (15 << 0x10),
        Unown_Q = 201 | (16 << 0x10),
        Unown_R = 201 | (17 << 0x10),
        Unown_S = 201 | (18 << 0x10),
        Unown_T = 201 | (19 << 0x10),
        Unown_U = 201 | (20 << 0x10),
        Unown_V = 201 | (21 << 0x10),
        Unown_W = 201 | (22 << 0x10),
        Unown_X = 201 | (23 << 0x10),
        Unown_Y = 201 | (24 << 0x10),
        Unown_Z = 201 | (25 << 0x10),
        Unown_Exclamation = 201 | (26 << 0x10),
        Unown_Question = 201 | (27 << 0x10),
        Clamperl = 366, // TODO
        Latias = 380, // TODO
        Latios, // TODO
        Cresselia = 488, // TODO
        Darkrai = 491, // TODO
    }
    public enum PMoveTarget : byte // Used in MoveData
    {
        All,                   // Every battler (Ex. Perish Song)
        AllFoes,               // Every foe (Ex. Stealth Rock)
        AllFoesSurrounding,    // All foes surrounding (Ex. Growl)
        AllSurrounding,        // All battlers surrounding (Ex. Earthquake)
        AllTeam,               // User's entire team (Ex. Light Screen)
        RandomFoeSurrounding,  // Randomly picks a surrounding foe (Ex. Outrage)
        Self,                  // Self (Ex. Growth)
        SelfOrAllySurrounding, // Self or adjacent ally (Ex. Acupressure)
        SingleAllySurrounding, // Adjacent ally (Ex. Helping Hand)
        SingleNotSelf,         // Single battler except itself (Ex. Dark Pulse)
        SingleFoeSurrounding,  // Single foe surrounding (Ex. Me First)
        SingleSurrounding,     // Single battler surrounding (Ex. Tackle)
    }
    [Flags]
    public enum PMoveFlag : ushort
    {
        None = 0,
        MakesContact = 1 << 0,
        AffectedByProtect = 1 << 1,
        AffectedByMagicCoat = 1 << 2,
        AffectedBySnatch = 1 << 3,
        AffectedByMirrorMove = 1 << 4,
        SoundBased = 1 << 5,
        DefrostsUser = 1 << 6,
        HitsAirborne = 1 << 7,
        HitsUnderground = 1 << 8,
        HitsUnderwater = 1 << 9,
        AlwaysCrit = 1 << 10,
        HighCritChance = 1 << 11,
    }
    public enum PMoveEffect : byte
    {
        BrickBreak,
        Burn,
        ChangeTarget_ACC,
        ChangeTarget_ATK,
        ChangeTarget_DEF,
        ChangeTarget_EVA,
        ChangeTarget_SPDEF,
        ChangeTarget_SPE,
        ChangeUser_ATK,
        ChangeUser_DEF,
        ChangeUser_EVA,
        ChangeUser_SPATK,
        ChangeUser_SPDEF,
        ChangeUser_SPE,
        Confuse,
        Dive,
        Fail,
        Growth,
        Hit,
        Hit__MaybeBurn,
        Hit__MaybeConfuse,
        Hit__MaybeFlinch,
        Hit__MaybeFreeze,
        Hit__MaybeLowerTarget_ACC_By1,
        Hit__MaybeLowerTarget_ATK_By1,
        Hit__MaybeLowerTarget_DEF_By1,
        Hit__MaybeLowerTarget_SPATK_By1,
        Hit__MaybeLowerTarget_SPDEF_By1,
        Hit__MaybeLowerTarget_SPDEF_By2,
        Hit__MaybeLowerTarget_SPE_By1,
        Hit__MaybeLowerUser_DEF_SPDEF_By1,
        Hit__MaybeLowerUser_SPATK_By2,
        Hit__MaybeLowerUser_SPE_By1,
        Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1,
        Hit__MaybeParalyze,
        Hit__MaybePoison,
        Hit__MaybeRaiseUser_ATK_By1,
        Hit__MaybeRaiseUser_DEF_By1,
        Hit__MaybeRaiseUser_SPATK_By1,
        Hit__MaybeRaiseUser_SPE_By1,
        Hit__MaybeToxic,
        LightScreen,
        LowerTarget_ATK_DEF_By1,
        LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2,
        Moonlight,
        Paralyze,
        Poison,
        Protect, // TODO: If the user goes last, fail
        RainDance,
        RaiseUser_ATK_ACC_By1,
        RaiseUser_ATK_DEF_By1,
        RaiseUser_ATK_DEF_ACC_By1,
        RaiseUser_ATK_SPATK_By1,
        RaiseUser_ATK_SPE_By1,
        RaiseUser_DEF_SPDEF_By1,
        RaiseUser_SPATK_SPDEF_By1,
        RaiseUser_SPATK_SPDEF_SPE_By1,
        RaiseUser_SPE_By2_ATK_By1,
        Reflect,
        Sleep,
        Substitute,
        SunnyDay,
        Toxic,
        Transform,
    }
    public enum PMove : ushort
    {
        None,
        Acid,
        AcidArmor,
        AcidSpray,
        AerialAce,
        Aeroblast,
        Agility,
        AirCutter,
        AirSlash,
        Amnesia,
        AquaJet,
        AquaTail,
        Astonish,
        AttackOrder,
        AuraSphere,
        AuroraBeam,
        Barrier,
        Bite,
        BlazeKick,
        BlueFlare,
        BrickBreak,
        BodySlam,
        BoltStrike,
        BoneClub,
        Bubble,
        BubbleBeam,
        BugBuzz,
        BulkUp,
        Bulldoze,
        BulletPunch,
        CalmMind,
        ChargeBeam,
        Charm,
        Chatter,
        CloseCombat,
        Coil,
        ConfuseRay,
        Confusion,
        CosmicPower,
        CottonGuard,
        Crabhammer,
        CrossChop,
        CrossPoison,
        Crunch,
        CrushClaw,
        Cut,
        DarkPulse,
        DarkVoid,
        Detect,
        Discharge,
        Dive,
        DizzyPunch,
        DracoMeteor,
        DragonBreath,
        DragonClaw,
        DragonDance,
        DragonPulse,
        DragonRush,
        DoubleTeam,
        DrillPeck,
        DrillRun,
        EarthPower,
        EggBomb,
        Electroweb,
        Ember,
        EnergyBall,
        Extrasensory,
        ExtremeSpeed,
        FaintAttack,
        FakeTears,
        FeatherDance,
        FieryDance,
        FireBlast,
        FirePunch,
        FlameCharge,
        Flamethrower,
        FlameWheel,
        Flash,
        FlashCannon,
        FocusBlast,
        ForcePalm,
        FrostBreath,
        Frustration,
        Glaciate,
        Glare,
        GrassKnot,
        GrassWhistle,
        Growl,
        Growth,
        GunkShot,
        HammerArm,
        Harden,
        Headbutt,
        HeartStamp,
        HeatWave,
        HiddenPower,
        HoneClaws,
        HornAttack,
        Howl,
        HydroPump,
        HyperFang,
        HyperVoice,
        Hypnosis,
        IceBeam,
        IcePunch,
        IceShard,
        IcicleCrash,
        IcyWind,
        Inferno,
        IronDefense,
        IronHead,
        IronTail,
        KarateChop,
        Kinesis,
        LavaPlume,
        LeafBlade,
        LeafTornado,
        Leer,
        Lick,
        LightScreen,
        LovelyKiss,
        LowKick,
        LowSweep,
        LusterPurge,
        MachPunch,
        MagicalLeaf,
        MagnetBomb,
        Meditate,
        Megahorn,
        MegaKick,
        MegaPunch,
        MetalClaw,
        MetalSound,
        MeteorMash,
        MirrorShot,
        MistBall,
        Moonlight,
        MudBomb,
        MuddyWater,
        MudSlap,
        MudShot,
        NastyPlot,
        NightDaze,
        NightSlash,
        Octazooka,
        Overheat,
        Peck,
        PoisonFang,
        PoisonGas,
        PoisonJab,
        PoisonPowder,
        PoisonSting,
        PoisonTail,
        Pound,
        PowderSnow,
        PowerGem,
        PowerWhip,
        Protect,
        Psybeam,
        Psychic,
        PsychoBoost,
        PsychoCut,
        QuickAttack,
        QuiverDance,
        RainDance,
        RazorLeaf,
        RazorShell,
        Reflect,
        Retaliate, // TODO
        Return,
        RockClimb,
        RockPolish,
        RockSlide,
        RockSmash,
        RockThrow,
        RockTomb,
        SacredFire,
        SandAttack,
        Scald,
        ScaryFace,
        Scratch,
        Screech,
        SearingShot,
        SeedBomb,
        SeedFlare,
        ShadowBall,
        ShadowClaw,
        ShadowPunch,
        ShadowSneak,
        Sharpen,
        ShellSmash,
        ShiftGear,
        SignalBeam,
        Sing,
        ShockWave,
        Slam,
        Slash,
        SleepPowder,
        Sludge,
        SludgeBomb,
        SludgeWave,
        Smog,
        SmokeScreen,
        Snarl,
        SpacialRend,
        Spark,
        Spore,
        SteelWing,
        StoneEdge,
        StormThrow,
        Strength,
        StringShot,
        StruggleBug,
        StunSpore,
        Substitute,
        SunnyDay,
        Supersonic,
        Surf,
        SweetKiss,
        SweetScent,
        Swift,
        SwordsDance,
        Tackle,
        TailGlow,
        TailWhip,
        TeeterDance,
        Teleport,
        Tickle,
        Thunder, // TODO: Bypass accuracy checks in rain, reduce accuracy in harsh sunlight, hit flying/bouncing/skydropping
        Thunderbolt,
        ThunderPunch,
        ThunderShock,
        ThunderWave,
        Toxic,
        Transform,
        VacuumWave,
        VCreate,
        ViceGrip,
        VineWhip,
        VitalThrow,
        Waterfall,
        WaterGun,
        WaterPulse,
        WillOWisp,
        WingAttack,
        Withdraw,
        Workup,
        XScissor,
        ZapCannon,
        ZenHeadbutt,
        MAX
    }
}
