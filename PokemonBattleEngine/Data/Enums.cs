using System;

namespace Kermalis.PokemonBattleEngine.Data
{
    public enum PGender : byte
    {
        Female,
        Genderless,
        Male,
        MAX
    }
    public enum PGenderRatio : byte
    {
        M7_F1, // Male 7:1 Female
        M3_F1, // Male 3:1 Female
        M1_F1, // Male 1:1 Female
        M1_F3, // Male 1:3 Female
        M1_F7, // Male 1:7 Female (Does not occur before generation 6)
        M0_F1, // Female species
        M0_F0, // Genderless species
        M1_F0  // Male species
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
        Sunny
    }
    public enum PFieldPosition : byte
    {
        // Not on the field.
        None,
        // In a double, triple, or rotation battle, the Pokémon to __your__ left.
        Left,
        // In a single battle, Pokémon are in the center.
        // In a double battle, no Pokémon are in the center.
        // In a triple or rotation battle, it is obvious.
        Center,
        // In a double, triple, or rotation battle, the Pokémon to __your__ right.
        Right
    }
    [Flags]
    public enum PTarget : byte
    {
        None,
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
        Switch
    }
    public enum PMoveCategory : byte
    {
        Status,
        Physical,
        Special,
        MAX
    }
    [Flags]
    public enum PMoveObtainMethod : ulong
    {
        None,
        LevelUp_RS = 1u << 0,
        LevelUp_FR = 1u << 1,
        LevelUp_LG = 1u << 2,
        LevelUp_E = 1u << 3,
        LevelUp_Colo = 1u << 4,
        LevelUp_XD = 1u << 5,
        LevelUp_DP = 1u << 6,
        LevelUp_Pt = 1u << 7,
        LevelUp_HGSS = 1u << 8,
        LevelUp_BW = 1u << 9,
        LevelUp_B2W2 = 1u << 10,
        TM_RSFRLGE = 1u << 11,
        TM_DPPtHGSS = 1u << 12,
        TM_BWB2W2 = 1u << 13,
        HM_RSFRLGE = 1u << 14,
        HM_DPPt = 1u << 15,
        HM_HGSS = 1u << 16,
        HM_BWB2W2 = 1u << 17,
        MoveTutor_FRLG = 1u << 18,
        MoveTutor_E = 1u << 19,
        MoveTutor_XD = 1u << 20,
        MoveTutor_DP = 1u << 21,
        MoveTutor_Pt = 1u << 22,
        MoveTutor_HGSS = 1u << 23,
        MoveTutor_BW = 1u << 24,
        MoveTutor_B2W2 = 1u << 25,
        EggMove_RSFRLG = 1u << 26,
        EggMove_E = 1u << 27,
        EggMove_DPPt = 1u << 28,
        EggMove_HGSS = 1u << 29,
        EggMove_BWB2W2 = 1u << 30,
        DreamWorld = 1u << 31,
        Forme = 1u << 32 // Learned when changing formes
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
        Airborne = 1 << 0,
        Confused = 1 << 1,
        Cursed = 1 << 2,
        Flinching = 1 << 3,
        Infatuated = 1 << 4, // TODO
        LeechSeed = 1 << 5,
        Minimized = 1 << 6,
        Protected = 1 << 7,
        Pumped = 1 << 8, // Focus Energy / Lansat Berry
        Substitute = 1 << 9,
        Tormented = 1 << 10, // TODO
        Transformed = 1 << 11,
        Underground = 1 << 12,
        Underwater = 1 << 13
    }
    [Flags]
    public enum PTeamStatus : byte
    {
        None,
        LightScreen = 1 << 0,
        Reflect = 1 << 1,
        Spikes = 1 << 2, // TODO: Gravity, magnet rise, magic guard, iron ball, baton pass with ingrain, air balloon
        StealthRock = 1 << 3, // TODO: magic guard, castform transforms after taking damage
        ToxicSpikes = 1 << 4 // TODO: Gravity, immunity, leaf guard, magic guard, iron ball, baton pass with ingrain, air balloon, synchronize with roar/whirlwind
    }
    public enum PStatusAction : byte
    {
        Activated, // flinch prevented movement, protect activated, etc
        Added, // protected itself, became paralyzed, etc
        Cured, // limber curing paralysis
        Damage, // hurt from confusion, substitute took damage, etc
        Ended // woke up, no longer confused, etc
    }
    public enum PTeamStatusAction : byte
    {
        Added, // set up
        Cleared, // brick break destroying reflect, defog clearing spikes, etc
        Damage, // hazard causing damage
        Ended // reflect & light screen wearing off
    }
    public enum PFailReason : byte
    {
        Default, // "But it failed!"
        HPFull, // Trying to use a healing move with max HP
        NoTarget // All opponents fainted already
    }
    public enum PWeatherAction : byte
    {
        Added,
        Ended
    }
    public enum PType : byte
    {
        None,
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
        AdamantOrb,
        BlackBelt,
        BlackGlasses,
        BlackSludge, // TODO
        BrightPowder,
        BurnDrive,
        Charcoal,
        ChillDrive,
        ChoiceBand, // TODO: Lock move
        ChoiceScarf, // TODO: Lock move
        ChoiceSpecs, // TODO: Lock move
        DampRock,
        DeepSeaScale,
        DeepSeaTooth,
        DouseDrive,
        DracoPlate,
        DragonFang,
        DreadPlate,
        EarthPlate,
        FistPlate,
        FlamePlate,
        GriseousOrb,
        HardStone,
        HeatRock,
        IciclePlate,
        InsectPlate,
        IronPlate,
        LaxIncense,
        Leftovers,
        LifeOrb, // TODO: Damage user
        LightBall,
        LightClay,
        LustrousOrb,
        MachoBrace,
        Magnet,
        MeadowPlate,
        MetalCoat,
        MetalPowder,
        MindPlate,
        MiracleSeed,
        MuscleBand,
        MysticWater,
        NeverMeltIce,
        OddIncense,
        PoisonBarb,
        PowerHerb,
        QuickPowder,
        RazorClaw,
        RockIncense,
        RoseIncense,
        ScopeLens,
        SeaIncense,
        SharpBeak,
        ShockDrive,
        SilkScarf,
        SilverPowder,
        SkyPlate,
        SoftSand,
        SoulDew,
        SpellTag,
        SplashPlate,
        SpookyPlate,
        StonePlate,
        ThickClub,
        ToxicPlate,
        TwistedSpoon,
        UpGrade,
        WaveIncense,
        WideLens,
        WiseGlasses,
        ZapPlate,
        MAX
    }
    public enum PAbility : byte
    {
        None, // Only used when an ability is removed
        Adaptability,
        BadDreams, // TODO
        BattleArmor,
        Blaze,
        Chlorophyll, // TODO
        Compoundeyes,
        CuteCharm, // TODO
        Download, // TODO
        Guts,
        Heatproof,
        HoneyGather,
        HugePower,
        Hustle,
        Illuminate,
        Infiltrator, // TODO
        InnerFocus, // TODO
        Imposter, // TODO
        Justified, // TODO
        Levitate, // TODO: Immunity to ground-type moves
        LightningRod, // TODO
        Limber,
        MarvelScale,
        Minus, // TODO
        Multitype,
        Mummy, // TODO
        NoGuard,
        Normalize, // TODO
        Overgrow,
        Plus, // TODO
        Pressure, // TODO
        PurePower,
        RainDish, // TODO
        Rattled, // TODO
        RockHead, // TODO
        RunAway,
        SapSipper, // TODO
        SereneGrace, // TODO
        ShedSkin, // TODO
        ShellArmor,
        ShieldDust, // TODO
        Simple,
        Sniper,
        SolarPower, // TODO
        Static, // TODO
        SuperLuck,
        Swarm,
        Telepathy, // TODO
        ThickFat,
        TintedLens, // TODO
        Torrent,
        VictoryStar, // TODO
        WonderSkin, // TODO
        MAX,
    }
    public enum PSpecies : uint
    {
        None,
        Bulbasaur = 1,
        Ivysaur = 2,
        Venusaur = 3,
        Charmander = 4,
        Charmeleon = 5,
        Charizard = 6,
        Squirtle = 7,
        Wartortle = 8,
        Blastoise = 9,
        Caterpie = 10,
        Metapod = 11,
        Butterfree = 12,
        Weedle = 13,
        Kakuna = 14,
        Beedrill = 15,
        Pikachu = 25,
        Cubone = 104,
        Marowak = 105,
        Ditto = 132,
        Crobat = 169,
        Pichu = 172,
        Azumarill = 184,
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
        Skitty = 300,
        Absol = 359,
        Clamperl = 366,
        Latias = 380,
        Latios = 381,
        Jirachi = 385,
        Rotom = 479 | (0 << 0x10),
        Rotom_Fan = 479 | (1 << 0x10),
        Rotom_Frost = 479 | (2 << 0x10),
        Rotom_Heat = 479 | (3 << 0x10),
        Rotom_Mow = 479 | (4 << 0x10),
        Rotom_Wash = 479 | (5 << 0x10),
        Dialga = 483,
        Palkia = 484,
        Giratina_Altered = 487 | (0 << 0x10),
        Giratina_Origin = 487 | (1 << 0x10),
        Cresselia = 488,
        Darkrai = 491,
        Arceus_Normal = 493 | (0 << 0x10),
        Arceus_Bug = 493 | (1 << 0x10),
        Arceus_Dark = 493 | (2 << 0x10),
        Arceus_Dragon = 493 | (3 << 0x10),
        Arceus_Electric = 493 | (4 << 0x10),
        Arceus_Fighting = 493 | (5 << 0x10),
        Arceus_Fire = 493 | (6 << 0x10),
        Arceus_Flying = 493 | (7 << 0x10),
        Arceus_Ghost = 493 | (8 << 0x10),
        Arceus_Grass = 493 | (9 << 0x10),
        Arceus_Ground = 493 | (10 << 0x10),
        Arceus_Ice = 493 | (11 << 0x10),
        Arceus_Poison = 493 | (12 << 0x10),
        Arceus_Psychic = 493 | (13 << 0x10),
        Arceus_Rock = 493 | (14 << 0x10),
        Arceus_Steel = 493 | (15 << 0x10),
        Arceus_Water = 493 | (16 << 0x10),
        Victini = 494,
        Cofagrigus = 563,
        Genesect = 649 | (0 << 0x10),
        Genesect_Burn = 649 | (1 << 0x10),
        Genesect_Chill = 649 | (2 << 0x10),
        Genesect_Douse = 649 | (3 << 0x10),
        Genesect_Shock = 649 | (4 << 0x10)
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
        Varies                 // Possible targets vary (Ex. Curse)
    }
    [Flags]
    public enum PMoveFlag : ushort
    {
        None,
        AlwaysCrit = 1 << 0,           // Always lands a critical hit
        AffectedByMagicCoat = 1 << 1,  // Magic Coat blocks the move
        AffectedByMirrorMove = 1 << 2, // Mirror Move can copy the move
        AffectedByProtect = 1 << 3,    // Protect blocks the move
        AffectedBySnatch = 1 << 4,     // Snatch can steal the move
        DefrostsUser = 1 << 5,         // User unfreezes when using this move
        HighCritChance = 1 << 6,       // +1 critical hit stage
        HitsAirborne = 1 << 7,         // Can hit airborne targets
        HitsUnderground = 1 << 8,      // Can hit underground targets
        HitsUnderwater = 1 << 9,       // Can hit underwater targets
        MakesContact = 1 << 10,        // Rough Skin, Iron Barbs and Rocky Helmet hurt the user
        SoundBased = 1 << 11           // Soundproof blocks the move
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
        Curse,
        Dig,
        Dive,
        Endeavor,
        Fail,
        Fly,
        FocusEnergy,
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
        Hit__MaybeLowerUser_ATK_DEF_By1,
        Hit__MaybeLowerUser_DEF_SPDEF_By1,
        Hit__MaybeLowerUser_SPATK_By2,
        Hit__MaybeLowerUser_SPE_By1,
        Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1,
        Hit__MaybeParalyze,
        Hit__MaybePoison,
        Hit__MaybeRaiseUser_ATK_By1,
        Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1,
        Hit__MaybeRaiseUser_DEF_By1,
        Hit__MaybeRaiseUser_SPATK_By1,
        Hit__MaybeRaiseUser_SPE_By1,
        Hit__MaybeToxic,
        LeechSeed,
        LightScreen,
        LowerTarget_ATK_DEF_By1,
        LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2,
        Magnitude,
        Minimize,
        Moonlight,
        PainSplit,
        Paralyze,
        Poison,
        Protect, // TODO: If the user goes last, fail
        PsychUp,
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
        RestoreUserHealth,
        Sleep,
        Spikes,
        StealthRock,
        Substitute,
        SunnyDay,
        Toxic,
        ToxicSpikes,
        Transform
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
        AncientPower,
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
        Brine,
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
        Curse,
        Cut,
        DarkPulse,
        DarkVoid,
        DefendOrder,
        Detect,
        Dig,
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
        Earthquake,
        EggBomb,
        Electroweb,
        Ember,
        Endeavor,
        EnergyBall,
        Eruption,
        Extrasensory,
        ExtremeSpeed,
        Facade,
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
        Fly,
        FocusBlast,
        FocusEnergy,
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
        HealOrder,
        HeartStamp,
        HeatCrash,
        HeatWave,
        HeavySlam,
        Hex,
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
        LeafStorm,
        LeafTornado,
        LeechSeed,
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
        Magnitude,
        Meditate,
        Megahorn,
        MegaKick,
        MegaPunch,
        MetalClaw,
        MetalSound,
        MeteorMash,
        MilkDrink,
        Minimize,
        MirrorShot,
        MistBall,
        Moonlight,
        MorningSun,
        MudBomb,
        MuddyWater,
        MudSlap,
        MudShot,
        NastyPlot,
        NeedleArm,
        NightDaze,
        NightSlash,
        Octazooka,
        OminousWind,
        Overheat,
        PainSplit,
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
        PsychUp,
        Psyshock,
        Psystrike,
        QuickAttack,
        QuiverDance,
        RainDance,
        RazorLeaf,
        RazorShell,
        Recover,
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
        SecretSword,
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
        SilverWind,
        Sing,
        ShockWave,
        SkyUppercut,
        SlackOff,
        Slam,
        Slash,
        SleepPowder,
        Sludge,
        SludgeBomb,
        SludgeWave,
        Smog,
        SmokeScreen,
        Snarl,
        Softboiled,
        SpacialRend,
        Spark,
        Spikes,
        Spore,
        StealthRock,
        Steamroller,
        SteelWing,
        Stomp,
        StoneEdge,
        StormThrow,
        Strength,
        StringShot,
        StruggleBug,
        StunSpore,
        Substitute,
        SunnyDay,
        Superpower,
        Supersonic,
        Surf,
        SweetKiss,
        SweetScent,
        Swift,
        SwordsDance,
        Synthesis,
        Tackle,
        TailGlow,
        TailWhip,
        TechnoBlast,
        TeeterDance,
        Teleport,
        Tickle,
        Thunder, // TODO: Bypass accuracy checks in rain, reduce accuracy in harsh sunlight
        Thunderbolt,
        ThunderPunch,
        ThunderShock,
        ThunderWave,
        Toxic,
        ToxicSpikes,
        Transform,
        VacuumWave,
        VCreate,
        Venoshock,
        ViceGrip,
        VineWhip,
        VitalThrow,
        Waterfall,
        WaterGun,
        WaterPulse,
        WaterSpout,
        WillOWisp,
        WingAttack,
        Withdraw,
        WorkUp,
        XScissor,
        ZapCannon,
        ZenHeadbutt,
        MAX
    }
}
