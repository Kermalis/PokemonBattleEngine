using Kermalis.PokemonBattleEngine.Battle;
using System;

namespace Kermalis.PokemonBattleEngine.Data
{
    /// <summary>Represents a language the engine supports.</summary>
    public enum PBELanguage : byte
    {
        English,
        French,
        German,
        Italian,
        Japanese_Kana,
        Japanese_Kanji,
        Korean,
        Spanish,
        MAX
    }
    /// <summary>Represents a specific Pokémon's gender.</summary>
    public enum PBEGender : byte
    {
        /// <summary>The Pokémon is female.</summary>
        Female,
        /// <summary>The Pokémon is genderless.</summary>
        Genderless,
        /// <summary>The Pokémon is male.</summary>
        Male,
        /// <summary>Invalid gender.</summary>
        MAX
    }
    /// <summary>Represents a Pokémon species' <see cref="PBEGender"/> ratio.</summary>
    public enum PBEGenderRatio : byte
    {
        /// <summary>The species is 87.5% male, 12.5% female.</summary>
        M7_F1 = 0x1F,
        /// <summary>The species is 75% male, 25% female.</summary>
        M3_F1 = 0x3F,
        /// <summary>The species is 50% male, 50% female.</summary>
        M1_F1 = 0x7F,
        /// <summary>The species is 25% male, 75% female.</summary>
        M1_F3 = 0xBF,
        /// <summary>The species is 0% male, 100% female.</summary>
        M0_F1 = 0xFE,
        /// <summary>The species is genderless.</summary>
        M0_F0 = 0xFF,
        /// <summary>The species is 100% male, 0% female.</summary>
        M1_F0 = 0x00
    }
    public enum PBEGrowthRate : byte
    {
        Erratic = 1,
        Fast = 4,
        Fluctuating = 2,
        MediumFast = 0,
        MediumSlow = 3,
        Slow = 5,
        MAX = 6 // 6 & 7 in-game are clones of MediumFast, but no Pokémon uses 6 or 7
    }
    /// <summary>Represents a Pokémon stat.</summary>
    public enum PBEStat : byte
    {
        /// <summary>Hit points.</summary>
        HP,
        /// <summary>Attack.</summary>
        Attack,
        /// <summary>Defense.</summary>
        Defense,
        /// <summary>Special Attack.</summary>
        SpAttack,
        /// <summary>Special Defense.</summary>
        SpDefense,
        /// <summary>Speed.</summary>
        Speed,
        /// <summary>Accuracy.</summary>
        Accuracy,
        /// <summary>Evasion.</summary>
        Evasion
    }
    /// <summary>Represents a specific <see cref="PBEMove"/>'s category.</summary>
    public enum PBEMoveCategory : byte
    {
        /// <summary>The move deals no damage.</summary>
        Status,
        /// <summary>The move deals physical damage using the Attack and Defense stats.</summary>
        Physical,
        /// <summary>The move deals special damage using the Special Attack and Special Defense stats.</summary>
        Special,
        /// <summary>Invalid category.</summary>
        MAX
    }
    /// <summary>Represents the various methods in which a Pokémon can learn a <see cref="PBEMove"/>.</summary>
    public enum PBEType : byte
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
        Water,
        MAX
    }
    /// <summary>Represents a specific Pokémon's nature.</summary>
    public enum PBENature : byte
    {
        /// <summary>The Pokémon favors attack and hinders special attack.</summary>
        Adamant = 3,
        /// <summary>The Pokémon doesn't favor any stat.</summary>
        Bashful = 18,
        /// <summary>The Pokémon favors defense and hinders attack.</summary>
        Bold = 5,
        /// <summary>The Pokémon favors attack and hinders speed.</summary>
        Brave = 2,
        /// <summary>The Pokémon favors special defense and hinders attack.</summary>
        Calm = 20,
        /// <summary>The Pokémon favors special defense and hinders special attack.</summary>
        Careful = 23,
        /// <summary>The Pokémon doesn't favor any stat.</summary>
        Docile = 6,
        /// <summary>The Pokémon favors special defense and hinders defense.</summary>
        Gentle = 21,
        /// <summary>The Pokémon doesn't favor any stat.</summary>
        Hardy = 0,
        /// <summary>The Pokémon favors speed and hinders defense.</summary>
        Hasty = 11,
        /// <summary>The Pokémon favors defense and hinders special attack.</summary>
        Impish = 8,
        /// <summary>The Pokémon favors speed and hinders special attack.</summary>
        Jolly = 13,
        /// <summary>The Pokémon favors defense and hinders special defense.</summary>
        Lax = 9,
        /// <summary>The Pokémon favors attack and hinders defense.</summary>
        Lonely = 1,
        /// <summary>The Pokémon favors special attack and hinders defense.</summary>
        Mild = 16,
        /// <summary>The Pokémon favors special attack and hinders attack.</summary>
        Modest = 15,
        /// <summary>The Pokémon favors speed and hinders special defense.</summary>
        Naive = 14,
        /// <summary>The Pokémon favors attack and hinders special defense.</summary>
        Naughty = 4,
        /// <summary>The Pokémon favors special attack and hinders speed.</summary>
        Quiet = 17,
        /// <summary>The Pokémon doesn't favor any stat.</summary>
        Quirky = 24,
        /// <summary>The Pokémon favors special attack and hinders special defense.</summary>
        Rash = 19,
        /// <summary>The Pokémon favors defense and hinders speed.</summary>
        Relaxed = 7,
        /// <summary>The Pokémon favors special defense and hinders speed.</summary>
        Sassy = 22,
        /// <summary>The Pokémon doesn't favor any stat.</summary>
        Serious = 12,
        /// <summary>The Pokémon favors speed and hinders attack.</summary>
        Timid = 10,
        /// <summary>Invalid nature.</summary>
        MAX = 25
    }
    public enum PBEFlavor : byte
    {
        Bitter = 3,
        Dry = 2,
        Sour = 1,
        Spicy = 0,
        Sweet = 4,
        MAX = 5
    }
    /// <summary>Represents a specific Pokémon's held item.</summary>
    public enum PBEItem : ushort
    {
        /// <summary>No item.</summary>
        None = 0,
        AbsorbBulb = 545, // TODO
        AdamantOrb = 135,
        AguavBerry = 162,
        AirBalloon = 541, // TODO
        /// <summary>No effect.</summary>
        AmuletCoin = 223,
        Antidote = 18, // TODO
        ApicotBerry = 205,
        /// <summary>No effect.</summary>
        ArmorFossil = 104,
        AspearBerry = 153, // TODO
        Awakening = 21, // TODO
        BabiriBerry = 199, // TODO
        /// <summary>No effect.</summary>
        BalmMushroom = 580,
        BelueBerry = 183, // TODO
        BerryJuice = 43,
        /// <summary>No effect.</summary>
        BigMushroom = 87,
        /// <summary>No effect.</summary>
        BigNugget = 581,
        /// <summary>No effect.</summary>
        BigPearl = 89,
        BigRoot = 296,
        BindingBand = 544, // TODO
        BlackBelt = 241,
        /// <summary>No effect.</summary>
        BlackFlute = 68,
        BlackGlasses = 240,
        BlackSludge = 281,
        /// <summary>No effect.</summary>
        BlkApricorn = 491,
        /// <summary>No effect.</summary>
        BluApricorn = 486,
        /// <summary>No effect.</summary>
        BlueFlute = 65,
        /// <summary>No effect.</summary>
        BlueScarf = 261,
        /// <summary>No effect.</summary>
        BlueShard = 73,
        BlukBerry = 165, // TODO
        /// <summary>No effect.</summary>
        BridgeMailD = 145,
        /// <summary>No effect.</summary>
        BridgeMailM = 148,
        /// <summary>No effect.</summary>
        BridgeMailS = 144,
        /// <summary>No effect.</summary>
        BridgeMailT = 146,
        /// <summary>No effect.</summary>
        BridgeMailV = 147,
        BrightPowder = 213,
        BugGem = 558,
        BurnDrive = 118,
        BurnHeal = 19, // TODO
        /// <summary>No effect.</summary>
        Calcium = 49,
        /// <summary>No effect.</summary>
        Carbos = 48,
        Casteliacone = 591, // TODO
        CellBattery = 546, // TODO
        Charcoal = 249,
        ChartiBerry = 195, // TODO
        CheriBerry = 149, // TODO
        CherishBall = 16,
        ChestoBerry = 150, // TODO
        ChilanBerry = 200, // TODO
        ChillDrive = 119,
        ChoiceBand = 220,
        ChoiceScarf = 287,
        ChoiceSpecs = 297,
        ChopleBerry = 189, // TODO
        /// <summary>No effect.</summary>
        ClawFossil = 100,
        /// <summary>No effect.</summary>
        CleanseTag = 224,
        /// <summary>No effect.</summary>
        CleverWing = 569,
        CobaBerry = 192, // TODO
        ColburBerry = 198, // TODO
        /// <summary>No effect.</summary>
        CometShard = 583,
        CornnBerry = 175, // TODO
        /// <summary>No effect.</summary>
        CoverFossil = 572,
        CustapBerry = 210, // TODO
        /// <summary>No effect.</summary>
        DampMulch = 96,
        DampRock = 285,
        DarkGem = 562,
        /// <summary>No effect.</summary>
        DawnStone = 109,
        DeepSeaScale = 227,
        DeepSeaTooth = 226,
        DestinyKnot = 280,
        DireHit = 56, // TODO
        DiveBall = 7,
        /// <summary>No effect.</summary>
        DomeFossil = 102,
        DouseDrive = 116,
        DracoPlate = 311,
        DragonFang = 250,
        DragonGem = 561,
        /// <summary>No effect.</summary>
        DragonScale = 235,
        DreadPlate = 312,
        DreamBall = 576,
        /// <summary>No effect.</summary>
        DubiousDisc = 324,
        DurinBerry = 182, // TODO
        DuskBall = 13,
        /// <summary>No effect.</summary>
        DuskStone = 108,
        EarthPlate = 305,
        EjectButton = 547, // TODO
        /// <summary>No effect.</summary>
        Electirizer = 322,
        ElectricGem = 550,
        Elixir = 40, // TODO
        EnergyPowder = 34, // TODO
        EnergyRoot = 35, // TODO
        EnigmaBerry = 208, // TODO
        /// <summary>No effect.</summary>
        EscapeRope = 78,
        Ether = 38, // TODO
        /// <summary>No effect.</summary>
        Everstone = 229,
        Eviolite = 538,
        ExpertBelt = 268,
        ExpShare = 216,
        FastBall = 492,
        /// <summary>No effect.</summary>
        FavoredMail = 138,
        FightingGem = 553,
        FigyBerry = 159,
        FireGem = 548,
        /// <summary>No effect.</summary>
        FireStone = 82,
        FistPlate = 303,
        /// <summary>The Pokémon contracts <see cref="PBEStatus1.Burned"/> at the end of each turn if it has no other <see cref="PBEStatus1"/> and it does not have <see cref="PBEType.Fire"/>.</summary>
        FlameOrb = 273,
        FlamePlate = 298,
        FloatStone = 539, // TODO
        FluffyTail = 64,
        FlyingGem = 556,
        FocusBand = 230,
        FocusSash = 275,
        FreshWater = 30, // TODO
        FriendBall = 497,
        FullHeal = 27, // TODO
        FullIncense = 316, // TODO
        FullRestore = 23, // TODO
        GanlonBerry = 202,
        /// <summary>No effect.</summary>
        GeniusWing = 568,
        GhostGem = 560,
        /// <summary>No effect.</summary>
        GooeyMulch = 98,
        GrassGem = 551,
        GreatBall = 3,
        /// <summary>No effect.</summary>
        GreenScarf = 263,
        /// <summary>No effect.</summary>
        GreenShard = 75,
        /// <summary>No effect.</summary>
        GreetMail = 137,
        GrepaBerry = 173, // TODO
        GripClaw = 286, // TODO
        GriseousOrb = 112,
        /// <summary>No effect.</summary>
        GrnApricorn = 488,
        GroundGem = 555,
        /// <summary>No effect.</summary>
        GrowthMulch = 95,
        GuardSpec = 55, // TODO
        HabanBerry = 197, // TODO
        HardStone = 238,
        HealBall = 14,
        HealPowder = 36, // TODO
        /// <summary>No effect.</summary>
        HealthWing = 565,
        /// <summary>No effect.</summary>
        HeartScale = 93,
        HeatRock = 284,
        HeavyBall = 495,
        /// <summary>No effect.</summary>
        HelixFossil = 101,
        HondewBerry = 172, // TODO
        /// <summary>No effect.</summary>
        Honey = 94,
        /// <summary>No effect.</summary>
        HPUp = 45,
        HyperPotion = 25, // TODO
        IapapaBerry = 163,
        IceGem = 552,
        IceHeal = 20, // TODO
        IciclePlate = 302,
        IcyRock = 282,
        /// <summary>No effect.</summary>
        InquiryMail = 141,
        InsectPlate = 308,
        /// <summary>No effect.</summary>
        Iron = 47,
        IronBall = 278, // TODO
        IronPlate = 313,
        JabocaBerry = 211, // TODO
        KasibBerry = 196, // TODO
        KebiaBerry = 190, // TODO
        KelpsyBerry = 170, // TODO
        KingsRock = 221, // TODO
        LaggingTail = 279, // TODO
        LansatBerry = 206, // TODO
        LavaCookie = 42, // TODO
        LaxIncense = 255,
        /// <summary>No effect.</summary>
        LeafStone = 85,
        Leftovers = 234,
        Lemonade = 32, // TODO
        LeppaBerry = 154, // TODO
        LevelBall = 493,
        LiechiBerry = 201,
        LifeOrb = 270,
        LightBall = 236,
        LightClay = 269,
        /// <summary>No effect.</summary>
        LikeMail = 142,
        LoveBall = 496,
        /// <summary>No effect.</summary>
        LuckIncense = 319,
        LuckyEgg = 231,
        LuckyPunch = 256,
        LumBerry = 157, // TODO
        LureBall = 494,
        LustrousOrb = 136,
        LuxuryBall = 11,
        MachoBrace = 215,
        /// <summary>No effect.</summary>
        Magmarizer = 323,
        Magnet = 242,
        MagoBerry = 161,
        MagostBerry = 176, // TODO
        MasterBall = 1,
        MaxElixir = 41, // TODO
        MaxEther = 39, // TODO
        MaxPotion = 24, // TODO
        /// <summary>No effect.</summary>
        MaxRepel = 77,
        MaxRevive = 29, // TODO
        MeadowPlate = 301,
        MentalHerb = 219, // TODO
        MetalCoat = 233,
        MetalPowder = 257,
        Metronome = 277, // TODO
        MicleBerry = 209, // TODO
        MindPlate = 307,
        MiracleSeed = 239,
        MoomooMilk = 33, // TODO
        MoonBall = 498,
        /// <summary>No effect.</summary>
        MoonStone = 81,
        MuscleBand = 266,
        /// <summary>No effect.</summary>
        MuscleWing = 566,
        MysticWater = 243,
        NanabBerry = 166, // TODO
        NestBall = 8,
        NetBall = 6,
        NeverMeltIce = 246,
        NomelBerry = 178, // TODO
        NormalGem = 564,
        /// <summary>No effect.</summary>
        Nugget = 92,
        OccaBerry = 184, // TODO
        OddIncense = 314,
        /// <summary>No effect.</summary>
        OddKeystone = 111,
        /// <summary>No effect.</summary>
        OldAmber = 103,
        OldGateau = 54, // TODO
        OranBerry = 155,
        /// <summary>No effect.</summary>
        OvalStone = 110,
        PamtreBerry = 180, // TODO
        ParalyzHeal = 22, // TODO
        ParkBall = 500,
        PasshoBerry = 185, // TODO
        /// <summary>No effect.</summary>
        PassOrb = 575,
        PayapaBerry = 193, // TODO
        /// <summary>No effect.</summary>
        Pearl = 88,
        /// <summary>No effect.</summary>
        PearlString = 582,
        PechaBerry = 151, // TODO
        PersimBerry = 156, // TODO
        PetayaBerry = 204,
        PinapBerry = 168, // TODO
        /// <summary>No effect.</summary>
        PinkScarf = 262,
        /// <summary>No effect.</summary>
        PlumeFossil = 573,
        /// <summary>No effect.</summary>
        PnkApricorn = 489,
        PoisonBarb = 245,
        PoisonGem = 554,
        PokeBall = 4,
        PokeDoll = 63,
        PokeToy = 577,
        PomegBerry = 169, // TODO
        Potion = 17, // TODO
        PowerAnklet = 293,
        PowerBand = 292,
        PowerBelt = 290,
        PowerBracer = 289,
        PowerHerb = 271,
        PowerLens = 291,
        PowerWeight = 294,
        /// <summary>No effect.</summary>
        PPMax = 53,
        /// <summary>No effect.</summary>
        PPUp = 51,
        PremierBall = 12,
        /// <summary>No effect.</summary>
        PrettyWing = 571,
        /// <summary>No effect.</summary>
        PrismScale = 537,
        /// <summary>No effect.</summary>
        Protector = 321,
        /// <summary>No effect.</summary>
        Protein = 46,
        PsychicGem = 557,
        /// <summary>No effect.</summary>
        PureIncense = 320,
        QualotBerry = 171, // TODO
        QuickBall = 15,
        QuickClaw = 217, // TODO
        QuickPowder = 274,
        RabutaBerry = 177, // TODO
        RageCandyBar = 504, // TODO
        /// <summary>No effect.</summary>
        RareBone = 106,
        /// <summary>No effect.</summary>
        RareCandy = 50,
        RawstBerry = 152, // TODO
        RazorClaw = 326,
        RazorFang = 327, // TODO
        RazzBerry = 164, // TODO
        /// <summary>No effect.</summary>
        ReaperCloth = 325,
        /// <summary>No effect.</summary>
        RedApricorn = 485,
        RedCard = 542, // TODO
        /// <summary>No effect.</summary>
        RedFlute = 67,
        /// <summary>No effect.</summary>
        RedScarf = 260,
        /// <summary>No effect.</summary>
        RedShard = 72,
        /// <summary>No effect.</summary>
        RelicBand = 588,
        /// <summary>No effect.</summary>
        RelicCopper = 584,
        /// <summary>No effect.</summary>
        RelicCrown = 590,
        /// <summary>No effect.</summary>
        RelicGold = 586,
        /// <summary>No effect.</summary>
        RelicSilver = 585,
        /// <summary>No effect.</summary>
        RelicStatue = 589,
        /// <summary>No effect.</summary>
        RelicVase = 587,
        RepeatBall = 9,
        /// <summary>No effect.</summary>
        Repel = 79,
        /// <summary>No effect.</summary>
        ReplyMail = 143,
        /// <summary>No effect.</summary>
        ResistWing = 567,
        RevivalHerb = 37, // TODO
        Revive = 28, // TODO
        RindoBerry = 187, // TODO
        RingTarget = 543, // TODO
        RockGem = 559,
        RockIncense = 315,
        RockyHelmet = 540,
        /// <summary>No effect.</summary>
        RootFossil = 99,
        RoseIncense = 318,
        RowapBerry = 212, // TODO
        /// <summary>No effect.</summary>
        RSVPMail = 139,
        SacredAsh = 44, // TODO
        SafariBall = 5,
        SalacBerry = 203,
        ScopeLens = 232,
        SeaIncense = 254,
        SharpBeak = 244,
        ShedShell = 295, // TODO
        ShellBell = 253, // TODO
        /// <summary>No effect.</summary>
        ShinyStone = 107,
        /// <summary>No effect.</summary>
        ShoalSalt = 70,
        /// <summary>No effect.</summary>
        ShoalShell = 71,
        ShockDrive = 117,
        ShucaBerry = 191, // TODO
        SilkScarf = 251,
        SilverPowder = 222,
        SitrusBerry = 158,
        /// <summary>No effect.</summary>
        SkullFossil = 105,
        SkyPlate = 306,
        SmokeBall = 228,
        SmoothRock = 283,
        SodaPop = 31, // TODO
        SoftSand = 237,
        /// <summary>No effect.</summary>
        SootheBell = 218,
        SoulDew = 225,
        SpellTag = 247,
        SpelonBerry = 179, // TODO
        SplashPlate = 299,
        SpookyPlate = 310,
        SportBall = 499,
        /// <summary>No effect.</summary>
        StableMulch = 97,
        /// <summary>No effect.</summary>
        Stardust = 90,
        StarfBerry = 207,
        /// <summary>No effect.</summary>
        StarPiece = 91,
        SteelGem = 563,
        Stick = 259,
        StickyBarb = 288, // TODO
        StonePlate = 309,
        /// <summary>No effect.</summary>
        SunStone = 80,
        SuperPotion = 26, // TODO
        /// <summary>No effect.</summary>
        SuperRepel = 76,
        SweetHeart = 134, // TODO
        /// <summary>No effect.</summary>
        SwiftWing = 570,
        TamatoBerry = 174, // TODO
        TangaBerry = 194, // TODO
        /// <summary>No effect.</summary>
        ThanksMail = 140,
        ThickClub = 258,
        /// <summary>No effect.</summary>
        Thunderstone = 83,
        TimerBall = 10,
        /// <summary>No effect.</summary>
        TinyMushroom = 86,
        /// <summary>The Pokémon contracts <see cref="PBEStatus1.BadlyPoisoned"/> at the end of each turn if it has no other <see cref="PBEStatus1"/> and it does not have <see cref="PBEType.Poison"/> or <see cref="PBEType.Steel"/>.</summary>
        ToxicOrb = 272,
        ToxicPlate = 304,
        TwistedSpoon = 248,
        UltraBall = 2,
        UpGrade = 252,
        WacanBerry = 186, // TODO
        WaterGem = 549,
        /// <summary>No effect.</summary>
        WaterStone = 84,
        WatmelBerry = 181, // TODO
        WaveIncense = 317,
        WepearBerry = 167, // TODO
        WhiteHerb = 214, // TODO
        /// <summary>No effect.</summary>
        WhiteFlute = 69,
        WideLens = 265,
        WikiBerry = 160,
        WiseGlasses = 267,
        /// <summary>No effect.</summary>
        WhtApricorn = 490,
        XAccuracy = 60, // TODO
        XAttack = 57, // TODO
        XDefend = 58, // TODO
        XSpecial = 61, // TODO
        XSpDef = 62, // TODO
        XSpeed = 59, // TODO
        YacheBerry = 188, // TODO
        /// <summary>No effect.</summary>
        YellowFlute = 66,
        /// <summary>No effect.</summary>
        YellowScarf = 264,
        /// <summary>No effect.</summary>
        YellowShard = 74,
        /// <summary>No effect.</summary>
        YlwApricorn = 487,
        ZapPlate = 300,
        /// <summary>No effect.</summary>
        Zinc = 52,
        ZoomLens = 276 // TODO
    }
    /// <summary>Represents a specific Pokémon's special ability.</summary>
    public enum PBEAbility : byte
    {
        /// <summary>The Pokémon's ability was suppressed with <see cref="PBEMoveEffect.GastroAcid"/>.</summary>
        None = 0,
        /// <summary>The Pokémon has a stronger same-type-attack-bonus.</summary>
        Adaptability = 91,
        Aftermath = 106, // TODO
        AirLock = 76,
        Analytic = 148, // TODO
        AngerPoint = 83, // TODO
        Anticipation = 107,
        ArenaTrap = 71, // TODO
        BadDreams = 123,
        /// <summary>The Pokémon takes no critical hits.</summary>
        BattleArmor = 4,
        BigPecks = 145,
        /// <summary>When the Pokémon has low HP, its Fire-type moves get a power boost.</summary>
        Blaze = 66,
        /// <summary>The Pokémon gets a speed boost in harsh sunlight.</summary>
        Chlorophyll = 34,
        ClearBody = 29,
        CloudNine = 13,
        ColorChange = 16,
        /// <summary>The Pokémon accuracy is boosted.</summary>
        Compoundeyes = 14,
        Contrary = 126,
        CursedBody = 130, // TODO
        CuteCharm = 56,
        Damp = 6, // TODO
        Defeatist = 129,
        Defiant = 128, // TODO
        Download = 88,
        /// <summary>The Pokémon changes the weather to infinite rain.</summary>
        Drizzle = 2,
        /// <summary>The Pokémon changes the weather to infinite harsh sunlight.</summary>
        Drought = 70,
        DrySkin = 87, // TODO
        EarlyBird = 48,
        EffectSpore = 27,
        /// <summary>The Pokémon takes less damage from incoming super-effective moves.</summary>
        Filter = 111,
        FlameBody = 49,
        FlareBoost = 138,
        FlashFire = 18, // TODO
        FlowerGift = 122,
        Forecast = 59,
        Forewarn = 108, // TODO (Also, does this activate when given/taken?)
        FriendGuard = 132, // TODO
        Frisk = 119, // TODO (Also, does this activate when given/taken?)
        Gluttony = 82, // TODO
        /// <summary>The Pokémon's attack is boosted when it is afflicted with a <see cref="PBEStatus1"/> and the damage reduction from <see cref="PBEStatus1.Burned"/> is not applied.</summary>
        Guts = 62,
        Harvest = 139, // TODO
        Healer = 131,
        /// <summary>The Pokémon takes less damage from Fire-type moves and from a burn.</summary>
        Heatproof = 85,
        HeavyMetal = 134, // TODO
        /// <summary>No effect in battle.</summary>
        HoneyGather = 118,
        /// <summary>The Pokémon's attack is boosted.</summary>
        HugePower = 37,
        /// <summary>The Pokémon's attack is boosted, but its accuracy is lower for physical moves.</summary>
        Hustle = 55,
        Hydration = 93, // TODO
        HyperCutter = 52,
        /// <summary>In a hailstorm, the Pokémon takes no damage from the hailstorm and restores HP at the end of each turn.</summary>
        IceBody = 115,
        /// <summary>No effect in battle.</summary>
        Illuminate = 35,
        Illusion = 149,
        Immunity = 17,
        /// <summary>The Pokémon transforms into the foe across from it when switching in.</summary>
        Imposter = 150,
        Infiltrator = 151, // TODO: Mist
        InnerFocus = 39,
        Insomnia = 15,
        Intimidate = 22,
        IronBarbs = 160,
        /// <summary>The power of moves with <see cref="PBEMoveFlag.AffectedByIronFist"/> is increased.</summary>
        IronFist = 89,
        Justified = 154,
        KeenEye = 51,
        Klutz = 103, // TODO
        LeafGuard = 102,
        /// <summary>The Pokémon is immune to Ground-type moves and most entry hazards.</summary>
        Levitate = 26,
        LightMetal = 135, // TODO
        Lightningrod = 31, // TODO
        /// <summary>The Pokémon cannot be paralyzed.</summary>
        Limber = 7,
        LiquidOoze = 64,
        MagicBounce = 156, // TODO
        MagicGuard = 98, // TODO
        MagmaArmor = 40,
        MagnetPull = 42, // TODO
        /// <summary>The Pokémon's defense is boosted when it is afflicted with a <see cref="PBEStatus1"/>.</summary>
        MarvelScale = 63,
        Minus = 58,
        MoldBreaker = 104,
        Moody = 141,
        MotorDrive = 78, // TODO
        Moxie = 153, // TODO
        Multiscale = 136, // TODO
        /// <summary>No effect in battle.</summary>
        Multitype = 121,
        Mummy = 152,
        NaturalCure = 30,
        /// <summary>The Pokémon will always hit and always get hit unless protection is used.</summary>
        NoGuard = 99,
        Normalize = 96,
        Oblivious = 12,
        /// <summary>The Pokémon takes no damage from a hailstorm or sandstorm.</summary>
        Overcoat = 142,
        /// <summary>When the Pokémon has low HP, its Grass-type moves get a power boost.</summary>
        Overgrow = 65,
        OwnTempo = 20,
        Pickpocket = 124, // TODO
        Pickup = 53, // TODO
        Plus = 57,
        PoisonHeal = 90,
        PoisonPoint = 38,
        PoisonTouch = 143, // TODO
        Prankster = 158,
        Pressure = 46, // TODO (Also, does this activate when given/taken?)
        /// <summary>The Pokémon's attack is boosted.</summary>
        PurePower = 74,
        QuickFeet = 95,
        /// <summary>In rain, the Pokémon restores HP at the end of each turn.</summary>
        RainDish = 44,
        Rattled = 155,
        Reckless = 120,
        Regenerator = 144,
        Rivalry = 79, // TODO
        RockHead = 69,
        RoughSkin = 24,
        RunAway = 50,
        /// <summary>In a sandstorm, the Pokémon takes no damage from the sandstorm and its Rock-, Ground-, and Steel-type moves get a power boost.</summary>
        SandForce = 159,
        /// <summary>The Pokémon gets a speed boost in a sandstorm.</summary>
        SandRush = 146,
        /// <summary>The Pokémon changes the weather to an infinite sandstorm.</summary>
        SandStream = 45,
        /// <summary>In a sandstorm, the Pokémon takes no damage from the sandstorm and gets a 20% evasion boost.</summary>
        SandVeil = 8,
        SapSipper = 157, // TODO
        Scrappy = 113,
        SereneGrace = 32,
        ShadowTag = 23, // TODO
        ShedSkin = 61,
        SheerForce = 125, // TODO
        /// <summary>The Pokémon takes no critical hits.</summary>
        ShellArmor = 75,
        ShieldDust = 19, // TODO
        /// <summary>The Pokémon's stat changes are doubled.</summary>
        Simple = 86,
        SkillLink = 92, // TODO: Effect on Triple Kick
        SlowStart = 112,
        /// <summary>The Pokémon deals more damage when landing critical hits.</summary>
        Sniper = 97,
        /// <summary>In a hailstorm, the Pokémon takes no damage from the hailstorm and gets a 20% evasion boost.</summary>
        SnowCloak = 81,
        /// <summary>The Pokémon changes the weather to an infinite hailstorm.</summary>
        SnowWarning = 117,
        /// <summary>In harsh sunlight, the Pokémon gets a special attack boost and takes damage at the end of each turn.</summary>
        SolarPower = 94,
        /// <summary>The Pokémon takes less damage from incoming super-effective moves.</summary>
        SolidRock = 116,
        Soundproof = 43, // TODO
        SpeedBoost = 3,
        Stall = 100, // TODO
        Static = 9,
        Steadfast = 80,
        Stench = 1, // TODO
        StickyHold = 60, // TODO
        StormDrain = 114, // TODO
        Sturdy = 5,
        SuctionCups = 21, // TODO
        /// <summary>The Pokémon is more likely to land critical hits.</summary>
        SuperLuck = 105,
        /// <summary>When the Pokémon has low HP, its Bug-type moves get a power boost.</summary>
        Swarm = 68,
        /// <summary>The Pokémon gets a speed boost in rain.</summary>
        SwiftSwim = 33,
        Synchronize = 28, // TODO
        TangledFeet = 77,
        Technician = 101,
        Telepathy = 140, // TODO
        Teravolt = 164,
        /// <summary>The Pokémon takes less damage from Ice- and Fire-type moves.</summary>
        ThickFat = 47,
        /// <summary>The Pokémon deals double damage with outgoing not-very-effective moves.</summary>
        TintedLens = 110,
        /// <summary>When the Pokémon has low HP, its Water-type moves get a power boost.</summary>
        Torrent = 67,
        ToxicBoost = 137,
        Trace = 36, // TODO (Also, does this activate when given/taken?)
        Truant = 54, // TODO
        Turboblaze = 163,
        Unaware = 109,
        Unburden = 84, // TODO
        Unnerve = 127, // TODO
        VictoryStar = 162,
        VitalSpirit = 72,
        VoltAbsorb = 10, // TODO
        WaterAbsorb = 11, // TODO
        WaterVeil = 41,
        WeakArmor = 133,
        WhiteSmoke = 73,
        /// <summary>The Pokémon is immune to all damaging moves except for moves that would deal super-effective damage.</summary>
        WonderGuard = 25,
        WonderSkin = 147,
        ZenMode = 161, // TODO (Also, does this activate when given/taken?)
        /// <summary>Invalid ability.</summary>
        MAX = 165,
    }
    // Official IDs for the forms
#pragma warning disable CA1069 // Enums values should not be duplicated
    public enum PBEForm : byte
    {
        Arceus = 0,
        Arceus_Bug = 6,
        Arceus_Dark = 16,
        Arceus_Dragon = 15,
        Arceus_Electric = 12,
        Arceus_Fighting = 1,
        Arceus_Fire = 9,
        Arceus_Flying = 2,
        Arceus_Ghost = 7,
        Arceus_Grass = 11,
        Arceus_Ground = 4,
        Arceus_Ice = 14,
        Arceus_Poison = 3,
        Arceus_Psychic = 13,
        Arceus_Rock = 5,
        Arceus_Steel = 8,
        Arceus_Water = 10,
        Basculin_Blue = 1,
        Basculin_Red = 0,
        Burmy_Plant = 0,
        Burmy_Sandy = 1,
        Burmy_Trash = 2,
        Castform = 0,
        Castform_Rainy = 2,
        Castform_Snowy = 3,
        Castform_Sunny = 1,
        Cherrim = 0,
        Cherrim_Sunshine = 1,
        Darmanitan = 0,
        Darmanitan_Zen = 1,
        Deerling_Autumn = 2,
        Deerling_Spring = 0,
        Deerling_Summer = 1,
        Deerling_Winter = 3,
        Deoxys = 0,
        Deoxys_Attack = 1,
        Deoxys_Defense = 2,
        Deoxys_Speed = 3,
        Gastrodon_East = 1,
        Gastrodon_West = 0,
        Genesect = 0,
        Genesect_Burn = 3,
        Genesect_Chill = 4,
        Genesect_Douse = 1,
        Genesect_Shock = 2,
        Giratina = 0,
        Giratina_Origin = 1,
        Keldeo = 0,
        Keldeo_Resolute = 1,
        Kyurem = 0,
        Kyurem_Black = 2,
        Kyurem_White = 1,
        Landorus = 0,
        Landorus_Therian = 1,
        Meloetta = 0,
        Meloetta_Pirouette = 1,
        Rotom = 0,
        Rotom_Fan = 4,
        Rotom_Frost = 3,
        Rotom_Heat = 1,
        Rotom_Mow = 5,
        Rotom_Wash = 2,
        Sawsbuck_Autumn = 2,
        Sawsbuck_Spring = 0,
        Sawsbuck_Summer = 1,
        Sawsbuck_Winter = 3,
        Shaymin = 0,
        Shaymin_Sky = 1,
        Shellos_East = 1,
        Shellos_West = 0,
        Thundurus = 0,
        Thundurus_Therian = 1,
        Tornadus = 0,
        Tornadus_Therian = 1,
        Unown_A = 0,
        Unown_B = 1,
        Unown_C = 2,
        Unown_D = 3,
        Unown_E = 4,
        Unown_Exclamation = 26,
        Unown_F = 5,
        Unown_G = 6,
        Unown_H = 7,
        Unown_I = 8,
        Unown_J = 9,
        Unown_K = 10,
        Unown_L = 11,
        Unown_M = 12,
        Unown_N = 13,
        Unown_O = 14,
        Unown_P = 15,
        Unown_Q = 16,
        Unown_Question = 27,
        Unown_R = 17,
        Unown_S = 18,
        Unown_T = 19,
        Unown_U = 20,
        Unown_V = 21,
        Unown_W = 22,
        Unown_X = 23,
        Unown_Y = 24,
        Unown_Z = 25,
        Wormadam_Plant = 0,
        Wormadam_Sandy = 1,
        Wormadam_Trash = 2
#pragma warning restore CA1069 // Enums values should not be duplicated
    }
    /// <summary>Represents a specific Pokémon species.</summary>
    public enum PBESpecies : ushort
    {
        Abomasnow = 460,
        Abra = 63,
        Absol = 359,
        Accelgor = 617,
        Aerodactyl = 142,
        Aggron = 306,
        Aipom = 190,
        Alakazam = 65,
        Alomomola = 594,
        Altaria = 334,
        Ambipom = 424,
        Amoonguss = 591,
        Ampharos = 181,
        Anorith = 347,
        Arbok = 24,
        Arcanine = 59,
        Arceus = 493,
        Archen = 566,
        Archeops = 567,
        Ariados = 168,
        Armaldo = 348,
        Aron = 304,
        Articuno = 144,
        Audino = 531,
        Axew = 610,
        Azelf = 482,
        Azumarill = 184,
        Azurill = 298,
        Bagon = 371,
        Baltoy = 343,
        Banette = 354,
        Barboach = 339,
        Basculin = 550,
        Bastiodon = 411,
        Bayleef = 153,
        Beartic = 614,
        Beautifly = 267,
        Beedrill = 15,
        Beheeyem = 606,
        Beldum = 374,
        Bellossom = 182,
        Bellsprout = 69,
        Bibarel = 400,
        Bidoof = 399,
        Bisharp = 625,
        Blastoise = 9,
        Blaziken = 257,
        Blissey = 242,
        Blitzle = 522,
        Boldore = 525,
        Bonsly = 438,
        Bouffalant = 626,
        Braviary = 628,
        Breloom = 286,
        Bronzong = 437,
        Bronzor = 436,
        Budew = 406,
        Buizel = 418,
        Bulbasaur = 1,
        Buneary = 427,
        Burmy = 412,
        Butterfree = 12,
        Cacnea = 331,
        Cacturne = 332,
        Camerupt = 323,
        Carnivine = 455,
        Carracosta = 565,
        Carvanha = 318,
        Cascoon = 268,
        Castform = 351,
        Caterpie = 10,
        Celebi = 251,
        Chandelure = 609,
        Chansey = 113,
        Charizard = 6,
        Charmander = 4,
        Charmeleon = 5,
        Chatot = 441,
        Cherrim = 421,
        Cherubi = 420,
        Chikorita = 152,
        Chimchar = 390,
        Chimecho = 358,
        Chinchou = 170,
        Chingling = 433,
        Cinccino = 573,
        Clamperl = 366,
        Claydol = 344,
        Clefable = 36,
        Clefairy = 35,
        Cleffa = 173,
        Cloyster = 91,
        Cobalion = 638,
        Cofagrigus = 563,
        Combee = 415,
        Combusken = 256,
        Conkeldurr = 534,
        Corphish = 341,
        Corsola = 222,
        Cottonee = 546,
        Cradily = 346,
        Cranidos = 408,
        Crawdaunt = 342,
        Cresselia = 488,
        Croagunk = 453,
        Crobat = 169,
        Croconaw = 159,
        Crustle = 558,
        Cryogonal = 615,
        Cubchoo = 613,
        Cubone = 104,
        Cyndaquil = 155,
        Darkrai = 491,
        Darmanitan = 555,
        Darumaka = 554,
        Deerling = 585,
        Deino = 633,
        Delcatty = 301,
        Delibird = 225,
        Deoxys = 386,
        Dewgong = 87,
        Dewott = 502,
        Dialga = 483,
        Diglett = 50,
        Ditto = 132,
        Dodrio = 85,
        Doduo = 84,
        Donphan = 232,
        Dragonair = 148,
        Dragonite = 149,
        Drapion = 452,
        Dratini = 147,
        Drifblim = 426,
        Drifloon = 425,
        Drilbur = 529,
        Drowzee = 96,
        Druddigon = 621,
        Ducklett = 580,
        Dugtrio = 51,
        Dunsparce = 206,
        Duosion = 578,
        Durant = 632,
        Dusclops = 356,
        Dusknoir = 477,
        Duskull = 355,
        Dustox = 269,
        Dwebble = 557,
        Eelektrik = 603,
        Eelektross = 604,
        Eevee = 133,
        Ekans = 23,
        Electabuzz = 125,
        Electivire = 466,
        Electrike = 309,
        Electrode = 101,
        Elekid = 239,
        Elgyem = 605,
        Emboar = 500,
        Emolga = 587,
        Empoleon = 395,
        Entei = 244,
        Escavalier = 589,
        Espeon = 196,
        Excadrill = 530,
        Exeggcute = 102,
        Exeggutor = 103,
        Exploud = 295,
        Farfetchd = 83,
        Fearow = 22,
        Feebas = 349,
        Feraligatr = 160,
        Ferroseed = 597,
        Ferrothorn = 598,
        Finneon = 456,
        Flaaffy = 180,
        Flareon = 136,
        Floatzel = 419,
        Flygon = 330,
        Foongus = 590,
        Forretress = 205,
        Fraxure = 611,
        Frillish = 592,
        Froslass = 478,
        Furret = 162,
        Gabite = 444,
        Gallade = 475,
        Galvantula = 596,
        Garbodor = 569,
        Garchomp = 445,
        Gardevoir = 282,
        Gastly = 92,
        Gastrodon = 423,
        Genesect = 649,
        Gengar = 94,
        Geodude = 74,
        Gible = 443,
        Gigalith = 526,
        Girafarig = 203,
        Giratina = 487,
        Glaceon = 471,
        Glalie = 362,
        Glameow = 431,
        Gligar = 207,
        Gliscor = 472,
        Gloom = 44,
        Golbat = 42,
        Goldeen = 118,
        Golduck = 55,
        Golem = 76,
        Golett = 622,
        Golurk = 623,
        Gorebyss = 368,
        Gothita = 574,
        Gothitelle = 576,
        Gothorita = 575,
        Granbull = 210,
        Graveler = 75,
        Grimer = 88,
        Grotle = 388,
        Groudon = 383,
        Grovyle = 253,
        Growlithe = 58,
        Grumpig = 326,
        Gulpin = 316,
        Gurdurr = 533,
        Gyarados = 130,
        Happiny = 440,
        Hariyama = 297,
        Haunter = 93,
        Haxorus = 612,
        Heatmor = 631,
        Heatran = 485,
        Heracross = 214,
        Herdier = 507,
        Hippopotas = 449,
        Hippowdon = 450,
        Hitmonchan = 107,
        Hitmonlee = 106,
        Hitmontop = 237,
        HoOh = 250,
        Honchkrow = 430,
        Hoothoot = 163,
        Hoppip = 187,
        Horsea = 116,
        Houndoom = 229,
        Houndour = 228,
        Huntail = 367,
        Hydreigon = 635,
        Hypno = 97,
        Igglybuff = 174,
        Illumise = 314,
        Infernape = 392,
        Ivysaur = 2,
        Jellicent = 593,
        Jigglypuff = 39,
        Jirachi = 385,
        Jolteon = 135,
        Joltik = 595,
        Jumpluff = 189,
        Jynx = 124,
        Kabuto = 140,
        Kabutops = 141,
        Kadabra = 64,
        Kakuna = 14,
        Kangaskhan = 115,
        Karrablast = 588,
        Kecleon = 352,
        Keldeo = 647,
        Kingdra = 230,
        Kingler = 99,
        Kirlia = 281,
        Klang = 600,
        Klink = 599,
        Klinklang = 601,
        Koffing = 109,
        Krabby = 98,
        Kricketot = 401,
        Kricketune = 402,
        Krokorok = 552,
        Krookodile = 553,
        Kyogre = 382,
        Kyurem = 646,
        Lairon = 305,
        Lampent = 608,
        Landorus = 645,
        Lanturn = 171,
        Lapras = 131,
        Larvesta = 636,
        Larvitar = 246,
        Latias = 380,
        Latios = 381,
        Leafeon = 470,
        Leavanny = 542,
        Ledian = 166,
        Ledyba = 165,
        Lickilicky = 463,
        Lickitung = 108,
        Liepard = 510,
        Lileep = 345,
        Lilligant = 549,
        Lillipup = 506,
        Linoone = 264,
        Litwick = 607,
        Lombre = 271,
        Lopunny = 428,
        Lotad = 270,
        Loudred = 294,
        Lucario = 448,
        Ludicolo = 272,
        Lugia = 249,
        Lumineon = 457,
        Lunatone = 337,
        Luvdisc = 370,
        Luxio = 404,
        Luxray = 405,
        Machamp = 68,
        Machoke = 67,
        Machop = 66,
        Magby = 240,
        Magcargo = 219,
        Magikarp = 129,
        Magmar = 126,
        Magmortar = 467,
        Magnemite = 81,
        Magneton = 82,
        Magnezone = 462,
        Makuhita = 296,
        Mamoswine = 473,
        Manaphy = 490,
        Mandibuzz = 630,
        Manectric = 310,
        Mankey = 56,
        Mantine = 226,
        Mantyke = 458,
        Maractus = 556,
        Mareep = 179,
        Marill = 183,
        Marowak = 105,
        Marshtomp = 259,
        Masquerain = 284,
        Mawile = 303,
        Medicham = 308,
        Meditite = 307,
        Meganium = 154,
        Meloetta = 648,
        Meowth = 52,
        Mesprit = 481,
        Metagross = 376,
        Metang = 375,
        Metapod = 11,
        Mew = 151,
        Mewtwo = 150,
        Mienfoo = 619,
        Mienshao = 620,
        Mightyena = 262,
        Milotic = 350,
        Miltank = 241,
        MimeJr = 439,
        Minccino = 572,
        Minun = 312,
        Misdreavus = 200,
        Mismagius = 429,
        Moltres = 146,
        Monferno = 391,
        Mothim = 414,
        MrMime = 122,
        Mudkip = 258,
        Muk = 89,
        Munchlax = 446,
        Munna = 517,
        Murkrow = 198,
        Musharna = 518,
        Natu = 177,
        Nidoking = 34,
        Nidoqueen = 31,
        Nidoran_F = 29,
        Nidoran_M = 32,
        Nidorina = 30,
        Nidorino = 33,
        Nincada = 290,
        Ninetales = 38,
        Ninjask = 291,
        Noctowl = 164,
        Nosepass = 299,
        Numel = 322,
        Nuzleaf = 274,
        Octillery = 224,
        Oddish = 43,
        Omanyte = 138,
        Omastar = 139,
        Onix = 95,
        Oshawott = 501,
        Pachirisu = 417,
        Palkia = 484,
        Palpitoad = 536,
        Panpour = 515,
        Pansage = 511,
        Pansear = 513,
        Paras = 46,
        Parasect = 47,
        Patrat = 504,
        Pawniard = 624,
        Pelipper = 279,
        Persian = 53,
        Petilil = 548,
        Phanpy = 231,
        Phione = 489,
        Pichu = 172,
        Pidgeot = 18,
        Pidgeotto = 17,
        Pidgey = 16,
        Pidove = 519,
        Pignite = 499,
        Pikachu = 25,
        Piloswine = 221,
        Pineco = 204,
        Pinsir = 127,
        Piplup = 393,
        Plusle = 311,
        Politoed = 186,
        Poliwag = 60,
        Poliwhirl = 61,
        Poliwrath = 62,
        Ponyta = 77,
        Poochyena = 261,
        Porygon = 137,
        Porygon2 = 233,
        PorygonZ = 474,
        Primeape = 57,
        Prinplup = 394,
        Probopass = 476,
        Psyduck = 54,
        Pupitar = 247,
        Purrloin = 509,
        Purugly = 432,
        Quagsire = 195,
        Quilava = 156,
        Qwilfish = 211,
        Raichu = 26,
        Raikou = 243,
        Ralts = 280,
        Rampardos = 409,
        Rapidash = 78,
        Raticate = 20,
        Rattata = 19,
        Rayquaza = 384,
        Regice = 378,
        Regigigas = 486,
        Regirock = 377,
        Registeel = 379,
        Relicanth = 369,
        Remoraid = 223,
        Reshiram = 643,
        Reuniclus = 579,
        Rhydon = 112,
        Rhyhorn = 111,
        Rhyperior = 464,
        Riolu = 447,
        Roggenrola = 524,
        Roselia = 315,
        Roserade = 407,
        Rotom = 479,
        Rufflet = 627,
        Sableye = 302,
        Salamence = 373,
        Samurott = 503,
        Sandile = 551,
        Sandshrew = 27,
        Sandslash = 28,
        Sawk = 539,
        Sawsbuck = 586,
        Sceptile = 254,
        Scizor = 212,
        Scolipede = 545,
        Scrafty = 560,
        Scraggy = 559,
        Scyther = 123,
        Seadra = 117,
        Seaking = 119,
        Sealeo = 364,
        Seedot = 273,
        Seel = 86,
        Seismitoad = 537,
        Sentret = 161,
        Serperior = 497,
        Servine = 496,
        Seviper = 336,
        Sewaddle = 540,
        Sharpedo = 319,
        Shaymin = 492,
        Shedinja = 292,
        Shelgon = 372,
        Shellder = 90,
        Shellos = 422,
        Shelmet = 616,
        Shieldon = 410,
        Shiftry = 275,
        Shinx = 403,
        Shroomish = 285,
        Shuckle = 213,
        Shuppet = 353,
        Sigilyph = 561,
        Silcoon = 266,
        Simipour = 516,
        Simisage = 512,
        Simisear = 514,
        Skarmory = 227,
        Skiploom = 188,
        Skitty = 300,
        Skorupi = 451,
        Skuntank = 435,
        Slaking = 289,
        Slakoth = 287,
        Slowbro = 80,
        Slowking = 199,
        Slowpoke = 79,
        Slugma = 218,
        Smeargle = 235,
        Smoochum = 238,
        Sneasel = 215,
        Snivy = 495,
        Snorlax = 143,
        Snorunt = 361,
        Snover = 459,
        Snubbull = 209,
        Solosis = 577,
        Solrock = 338,
        Spearow = 21,
        Spheal = 363,
        Spinarak = 167,
        Spinda = 327,
        Spiritomb = 442,
        Spoink = 325,
        Squirtle = 7,
        Stantler = 234,
        Staraptor = 398,
        Staravia = 397,
        Starly = 396,
        Starmie = 121,
        Staryu = 120,
        Steelix = 208,
        Stoutland = 508,
        Stunfisk = 618,
        Stunky = 434,
        Sudowoodo = 185,
        Suicune = 245,
        Sunflora = 192,
        Sunkern = 191,
        Surskit = 283,
        Swablu = 333,
        Swadloon = 541,
        Swalot = 317,
        Swampert = 260,
        Swanna = 581,
        Swellow = 277,
        Swinub = 220,
        Swoobat = 528,
        Taillow = 276,
        Tangela = 114,
        Tangrowth = 465,
        Tauros = 128,
        Teddiursa = 216,
        Tentacool = 72,
        Tentacruel = 73,
        Tepig = 498,
        Terrakion = 639,
        Throh = 538,
        Thundurus = 642,
        Timburr = 532,
        Tirtouga = 564,
        Togekiss = 468,
        Togepi = 175,
        Togetic = 176,
        Torchic = 255,
        Torkoal = 324,
        Tornadus = 641,
        Torterra = 389,
        Totodile = 158,
        Toxicroak = 454,
        Tranquill = 520,
        Trapinch = 328,
        Treecko = 252,
        Tropius = 357,
        Trubbish = 568,
        Turtwig = 387,
        Tympole = 535,
        Tynamo = 602,
        Typhlosion = 157,
        Tyranitar = 248,
        Tyrogue = 236,
        Umbreon = 197,
        Unfezant = 521,
        Unown = 201,
        Ursaring = 217,
        Uxie = 480,
        Vanillish = 583,
        Vanillite = 582,
        Vanilluxe = 584,
        Vaporeon = 134,
        Venipede = 543,
        Venomoth = 49,
        Venonat = 48,
        Venusaur = 3,
        Vespiquen = 416,
        Vibrava = 329,
        Victini = 494,
        Victreebel = 71,
        Vigoroth = 288,
        Vileplume = 45,
        Virizion = 640,
        Volbeat = 313,
        Volcarona = 637,
        Voltorb = 100,
        Vullaby = 629,
        Vulpix = 37,
        Wailmer = 320,
        Wailord = 321,
        Walrein = 365,
        Wartortle = 8,
        Watchog = 505,
        Weavile = 461,
        Weedle = 13,
        Weepinbell = 70,
        Weezing = 110,
        Whimsicott = 547,
        Whirlipede = 544,
        Whiscash = 340,
        Whismur = 293,
        Wigglytuff = 40,
        Wingull = 278,
        Wobbuffet = 202,
        Woobat = 527,
        Wooper = 194,
        Wormadam = 413,
        Wurmple = 265,
        Wynaut = 360,
        Xatu = 178,
        Yamask = 562,
        Yanma = 193,
        Yanmega = 469,
        Zangoose = 335,
        Zapdos = 145,
        Zebstrika = 523,
        Zekrom = 644,
        Zigzagoon = 263,
        Zoroark = 571,
        Zorua = 570,
        Zubat = 41,
        Zweilous = 634,
        MAX = 650
    }
    public enum PBEMoveTarget : byte
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
        SingleFoeSurrounding,  // Single foe surrounding (Ex. Me First)
        SingleNotSelf,         // Single battler except itself (Ex. Dark Pulse)
        SingleSurrounding,     // Single battler surrounding (Ex. Tackle)
        Varies                 // Possible targets vary (Ex. Curse)
    }
    /// <summary>Represents a specific <see cref="PBEMove"/>'s flags.</summary>
    [Flags]
    public enum PBEMoveFlag : ulong
    {
        /// <summary>The move has no flags.</summary>
        None,
        /// <summary>The move's power is boosted by <see cref="PBEAbility.IronFist"/>.</summary>
        AffectedByIronFist = 1 << 0,
        AffectedByMagicCoat = 1 << 1,
        AffectedByMirrorMove = 1 << 2,
        AffectedByProtect = 1 << 3,
        /// <summary>The move's power is boosted by <see cref="PBEAbility.Reckless"/>.</summary>
        AffectedByReckless = 1 << 4,
        AffectedBySnatch = 1 << 5,
        /// <summary>The move is blocked by <see cref="PBEAbility.Soundproof"/>.</summary>
        AffectedBySoundproof = 1 << 6,
        /// <summary>The move always lands a critical hit.</summary>
        AlwaysCrit = 1 << 7,
        BlockedByGravity = 1 << 8,
        BlockedFromAssist = 1 << 9,
        BlockedFromCopycat = 1 << 10,
        BlockedFromMeFirst = 1 << 12,
        BlockedFromMetronome = 1 << 13,
        BlockedFromMimic = 1 << 14,
        BlockedFromSketch = 1 << 15,
        BlockedFromSketchWhenSuccessful = 1 << 16,
        BlockedFromSleepTalk = 1 << 17,
        /// <summary>The move removes <see cref="PBEStatus1.Frozen"/> from the user.</summary>
        DefrostsUser = 1 << 18,
        DoubleDamageAirborne = 1 << 19,
        DoubleDamageMinimized = 1 << 20,
        DoubleDamageUnderground = 1 << 21,
        DoubleDamageUnderwater = 1 << 22,
        DoubleDamageUserDefenseCurl = 1 << 23,
        /// <summary>The move has a higher chance of landing a critical hit.</summary>
        HighCritChance = 1 << 24,
        /// <summary>The move can hit <see cref="PBEStatus2.Airborne"/> targets.</summary>
        HitsAirborne = 1 << 25,
        /// <summary>The move can hit <see cref="PBEStatus2.Underground"/> targets.</summary>
        HitsUnderground = 1 << 26,
        /// <summary>The move can hit <see cref="PBEStatus2.Underwater"/> targets.</summary>
        HitsUnderwater = 1 << 27,
        /// <summary>The user makes contact with the target, causing it to take damage from the target's <see cref="PBEAbility.IronBarbs"/>, <see cref="PBEAbility.RoughSkin"/>, and <see cref="PBEItem.RockyHelmet"/>.</summary>
        MakesContact = 1 << 28,
        NeverMissHail = 1 << 29,
        NeverMissRain = 1 << 30,
        UnaffectedByGems = 1uL << 31 // TODO
    }
    public enum PBEMoveEffect : byte
    {
        Acrobatics,
        Attract,
        BellyDrum,
        Bounce,
        BrickBreak,
        Brine,
        Burn,
        Camouflage,
        ChangeTarget_ACC,
        ChangeTarget_ATK,
        ChangeTarget_DEF,
        ChangeTarget_EVA,
        ChangeTarget_SPATK,
        ChangeTarget_SPATK__IfAttractionPossible,
        ChangeTarget_SPDEF,
        ChangeTarget_SPE,
        ChipAway,
        Confuse,
        Conversion,
        CrushGrip,
        Curse,
        Dig,
        Dive,
        Endeavor,
        Entrainment,
        Eruption,
        Facade,
        Feint,
        FinalGambit,
        Flatter,
        Flail,
        Fly,
        FocusEnergy,
        Foresight,
        FoulPlay,
        Frustration,
        GastroAcid,
        GrassKnot,
        Growth,
        Hail,
        Haze,
        HeatCrash,
        HelpingHand,
        Hex,
        HiddenPower,
        Hit,
        Hit__2Times,
        Hit__2Times__MaybePoison,
        Hit__2To5Times,
        Hit__MaybeBurn,
        Hit__MaybeBurn__10PercentFlinch,
        Hit__MaybeBurnFreezeParalyze,
        Hit__MaybeConfuse,
        Hit__MaybeFlinch,
        Hit__MaybeFreeze,
        Hit__MaybeFreeze__10PercentFlinch,
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
        Hit__MaybeParalyze__10PercentFlinch,
        Hit__MaybePoison,
        Hit__MaybeRaiseUser_ATK_By1,
        Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1,
        Hit__MaybeRaiseUser_DEF_By1,
        Hit__MaybeRaiseUser_SPATK_By1,
        Hit__MaybeRaiseUser_SPE_By1,
        Hit__MaybeToxic,
        HPDrain,
        HPDrain__RequireSleep,
        Judgment,
        LeechSeed,
        LightScreen,
        LockOn,
        LowerTarget_ATK_DEF_By1,
        LowerTarget_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2,
        LuckyChant,
        MagnetRise,
        Magnitude,
        Metronome,
        Minimize,
        MiracleEye,
        Moonlight,
        Nightmare,
        Nothing,
        OneHitKnockout,
        PainSplit,
        Paralyze,
        Payback,
        PayDay,
        Poison,
        PowerTrick,
        Protect, // TODO: If the user goes last, fail
        PsychUp,
        Psyshock,
        Psywave,
        Punishment,
        QuickGuard,
        RainDance,
        RaiseTarget_ATK_ACC_By1,
        RaiseTarget_ATK_DEF_By1,
        RaiseTarget_ATK_DEF_ACC_By1,
        RaiseTarget_ATK_SPATK_By1,
        RaiseTarget_ATK_SPE_By1,
        RaiseTarget_DEF_SPDEF_By1,
        RaiseTarget_SPATK_SPDEF_By1,
        RaiseTarget_SPATK_SPDEF_SPE_By1,
        RaiseTarget_SPE_By2_ATK_By1,
        Recoil,
        Recoil__10PercentBurn,
        Recoil__10PercentParalyze,
        Reflect,
        ReflectType,
        Refresh,
        Rest,
        RestoreTargetHP,
        Retaliate,
        Return,
        RolePlay,
        Roost,
        Safeguard,
        Sandstorm,
        SecretPower,
        SeismicToss,
        Selfdestruct,
        SetDamage,
        ShadowForce,
        SimpleBeam,
        Sketch, // TODO
        Sleep,
        SmellingSalt,
        Snore,
        Soak,
        Spikes,
        StealthRock,
        StoredPower,
        Struggle,
        Substitute,
        SuckerPunch,
        SunnyDay,
        SuperFang,
        Swagger,
        Tailwind,
        TechnoBlast,
        Teleport, // TODO: Trapping effects & SmokeBall
        ThunderWave,
        Toxic,
        ToxicSpikes,
        Transform,
        TrickRoom,
        Venoshock,
        WakeUpSlap,
        WeatherBall,
        Whirlwind, // TODO: Trapping effects
        WideGuard,
        WorrySeed,
        TODOMOVE // Moves that are not added yet
    }
    public enum PBEMove : ushort
    {
        None = 0,
        Absorb = 71,
        Acid = 51,
        AcidArmor = 151,
        AcidSpray = 491,
        Acrobatics = 512,
        Acupressure = 367, // TODO
        AerialAce = 332,
        Aeroblast = 177,
        AfterYou = 495, // TODO
        Agility = 97,
        AirCutter = 314,
        AirSlash = 403,
        AllySwitch = 502, // TODO
        Amnesia = 133,
        AncientPower = 246,
        AquaJet = 453,
        AquaRing = 392, // TODO
        AquaTail = 401,
        ArmThrust = 292,
        Aromatherapy = 312, // TODO
        Assist = 274, // TODO
        Assurance = 372, // TODO
        Astonish = 310,
        AttackOrder = 454,
        Attract = 213,
        AuraSphere = 396,
        AuroraBeam = 62,
        Autotomize = 475, // TODO
        Avalanche = 419, // TODO
        Barrage = 140,
        Barrier = 112,
        BatonPass = 226, // TODO
        BeatUp = 251, // TODO
        BellyDrum = 187,
        Bestow = 516, // TODO
        Bide = 117, // TODO
        Bind = 20, // TODO
        Bite = 44,
        BlastBurn = 307, // TODO
        BlazeKick = 299,
        Blizzard = 59,
        Block = 335, // TODO
        BlueFlare = 551,
        BodySlam = 34,
        BoltStrike = 550,
        BoneClub = 125,
        Bonemerang = 155,
        BoneRush = 198,
        Bounce = 340,
        BraveBird = 413,
        BrickBreak = 280,
        Brine = 362,
        Bubble = 145,
        BubbleBeam = 61,
        BugBite = 450, // TODO
        BugBuzz = 405,
        BulkUp = 339,
        Bulldoze = 523,
        BulletPunch = 418,
        BulletSeed = 331,
        CalmMind = 347,
        Camouflage = 293,
        Captivate = 445,
        Charge = 268, // TODO
        ChargeBeam = 451,
        Charm = 204,
        Chatter = 448,
        ChipAway = 498,
        CircleThrow = 509, // TODO
        Clamp = 128, // TODO
        ClearSmog = 499, // TODO
        CloseCombat = 370,
        Coil = 489,
        CometPunch = 4,
        ConfuseRay = 109,
        Confusion = 93,
        Constrict = 132,
        Conversion = 160,
        Conversion2 = 176, // TODO
        Copycat = 383, // TODO
        CosmicPower = 322,
        CottonGuard = 538,
        CottonSpore = 178,
        Counter = 68, // TODO
        Covet = 343, // TODO
        Crabhammer = 152,
        CrossChop = 238,
        CrossPoison = 440,
        Crunch = 242,
        CrushClaw = 306,
        CrushGrip = 462,
        Curse = 174,
        Cut = 15,
        DarkPulse = 399,
        DarkVoid = 464,
        DefendOrder = 455,
        DefenseCurl = 111,
        Defog = 432, // TODO
        DestinyBond = 194, // TODO
        Detect = 197,
        Dig = 91,
        Disable = 50, // TODO
        Discharge = 435,
        Dive = 291,
        DizzyPunch = 146,
        DoomDesire = 353, // TODO
        DoubleEdge = 38,
        DoubleHit = 458,
        DoubleKick = 24,
        DoubleSlap = 3,
        DoubleTeam = 104,
        DracoMeteor = 434,
        DragonBreath = 225,
        DragonClaw = 337,
        DragonDance = 349,
        DragonPulse = 406,
        DragonRage = 82,
        DragonRush = 407,
        DragonTail = 525, // TODO
        DrainPunch = 409,
        DreamEater = 138,
        DrillPeck = 65,
        DrillRun = 529,
        DualChop = 530,
        DynamicPunch = 223,
        EarthPower = 414,
        Earthquake = 89,
        EchoedVoice = 497, // TODO
        EggBomb = 121,
        ElectroBall = 486, // TODO
        Electroweb = 527,
        Embargo = 373, // TODO
        Ember = 52,
        Encore = 227, // TODO
        Endeavor = 283,
        Endure = 203, // TODO
        EnergyBall = 412,
        Entrainment = 494,
        Eruption = 284,
        Explosion = 153,
        Extrasensory = 326,
        ExtremeSpeed = 245,
        Facade = 263,
        FaintAttack = 185,
        FakeOut = 252, // TODO
        FakeTears = 313,
        FalseSwipe = 206, // TODO
        FeatherDance = 297,
        Feint = 364,
        FieryDance = 552,
        FinalGambit = 515,
        FireBlast = 126,
        FireFang = 424,
        FirePledge = 519, // TODO
        FirePunch = 7,
        FireSpin = 83, // TODO
        Fissure = 90,
        Flail = 175,
        FlameBurst = 481, // TODO
        FlameCharge = 488,
        Flamethrower = 53,
        FlameWheel = 172,
        FlareBlitz = 394,
        Flash = 148,
        FlashCannon = 430,
        Flatter = 260,
        Fling = 374, // TODO
        Fly = 19,
        FocusBlast = 411,
        FocusEnergy = 116,
        FocusPunch = 264, // TODO
        FollowMe = 266, // TODO
        ForcePalm = 395,
        Foresight = 193,
        FoulPlay = 492,
        FreezeShock = 553, // TODO
        FrenzyPlant = 338, // TODO
        FrostBreath = 524,
        Frustration = 218,
        FuryAttack = 31,
        FuryCutter = 210, // TODO
        FurySwipes = 154,
        FusionBolt = 559, // TODO
        FusionFlare = 558, // TODO
        FutureSight = 248, // TODO
        GastroAcid = 380, // TODO: Magic Bounce, Magic Coat
        GearGrind = 544,
        GigaDrain = 202,
        GigaImpact = 416, // TODO
        Glaciate = 549,
        Glare = 137,
        GrassKnot = 447,
        GrassPledge = 520, // TODO
        GrassWhistle = 320,
        Gravity = 356, // TODO
        Growl = 45,
        Growth = 74,
        Grudge = 288, // TODO
        GuardSplit = 470, // TODO
        GuardSwap = 385, // TODO
        Guillotine = 12,
        GunkShot = 441,
        Gust = 16,
        GyroBall = 360, // TODO
        Hail = 258,
        HammerArm = 359,
        Harden = 106,
        Haze = 114,
        Headbutt = 29,
        HeadCharge = 543,
        HeadSmash = 457,
        HealBell = 215, // TODO
        HealBlock = 377, // TODO
        HealingWish = 361, // TODO
        HealOrder = 456,
        HealPulse = 505,
        HeartStamp = 531,
        HeartSwap = 391, // TODO
        HeatCrash = 535,
        HeatWave = 257,
        HeavySlam = 484,
        HelpingHand = 270,
        Hex = 506,
        HiddenPower = 237,
        HiJumpKick = 136, // TODO
        HoneClaws = 468,
        HornAttack = 30,
        HornDrill = 32,
        HornLeech = 532,
        Howl = 336,
        Hurricane = 542,
        HydroCannon = 308, // TODO
        HydroPump = 56,
        HyperBeam = 63, // TODO
        HyperFang = 158,
        HyperVoice = 304,
        Hypnosis = 95,
        IceBall = 301, // TODO
        IceBeam = 58,
        IceBurn = 554, // TODO
        IceFang = 423,
        IcePunch = 8,
        IceShard = 420,
        IcicleCrash = 556,
        IcicleSpear = 333,
        IcyWind = 196,
        Imprison = 286, // TODO
        Incinerate = 510, // TODO
        Inferno = 517,
        Ingrain = 275, // TODO
        IronDefense = 334,
        IronHead = 442,
        IronTail = 231,
        Judgment = 449,
        JumpKick = 26, // TODO
        KarateChop = 2,
        Kinesis = 134,
        KnockOff = 282, // TODO
        LastResort = 387, // TODO
        LavaPlume = 436,
        LeafBlade = 348,
        LeafStorm = 437,
        LeafTornado = 536,
        LeechLife = 141,
        LeechSeed = 73,
        Leer = 43,
        Lick = 122,
        LightScreen = 113,
        LockOn = 199,
        LovelyKiss = 142,
        LowKick = 67,
        LowSweep = 490,
        LuckyChant = 381,
        LunarDance = 461, // TODO
        LusterPurge = 295,
        MachPunch = 183,
        MagicalLeaf = 345,
        MagicCoat = 277, // TODO
        MagicRoom = 478, // TODO
        MagmaStorm = 463, // TODO
        MagnetBomb = 443,
        MagnetRise = 393,
        Magnitude = 222,
        MeanLook = 212, // TODO
        Meditate = 96,
        MeFirst = 382, // TODO & TODO: Sucker Punch
        MegaDrain = 72,
        Megahorn = 224,
        MegaKick = 25,
        MegaPunch = 5,
        Memento = 262, // TODO
        MetalBurst = 368, // TODO
        MetalClaw = 232,
        MetalSound = 319,
        MeteorMash = 309,
        Metronome = 118,
        MilkDrink = 208,
        Mimic = 102, // TODO
        MindReader = 170,
        Minimize = 107,
        MiracleEye = 357,
        MirrorCoat = 243, // TODO
        MirrorMove = 119, // TODO
        MirrorShot = 429,
        Mist = 54, // TODO & TODO: Infiltrator
        MistBall = 296,
        Moonlight = 236,
        MorningSun = 234,
        MudBomb = 426,
        MuddyWater = 330,
        MudShot = 341,
        MudSlap = 189,
        MudSport = 300, // TODO
        NastyPlot = 417,
        NaturalGift = 363, // TODO
        NaturePower = 267, // TODO
        NeedleArm = 302,
        NightDaze = 539,
        Nightmare = 171,
        NightShade = 101,
        NightSlash = 400,
        Octazooka = 190,
        OdorSleuth = 316,
        OminousWind = 466,
        Outrage = 200, // TODO
        Overheat = 315,
        PainSplit = 220,
        Payback = 371, // TODO: If the target used an item instead of a move
        PayDay = 6,
        Peck = 64,
        PerishSong = 195, // TODO
        PetalDance = 80, // TODO
        PinMissile = 42,
        Pluck = 365, // TODO
        PoisonFang = 305,
        PoisonGas = 139,
        PoisonJab = 398,
        PoisonPowder = 77,
        PoisonSting = 40,
        PoisonTail = 342,
        Pound = 1,
        PowderSnow = 181,
        PowerGem = 408,
        PowerSplit = 471, // TODO
        PowerSwap = 384, // TODO
        PowerTrick = 379,
        PowerWhip = 438,
        Present = 217, // TODO
        Protect = 182,
        Psybeam = 60,
        Psychic = 94,
        PsychoBoost = 354,
        PsychoCut = 427,
        PsychoShift = 375, // TODO
        PsychUp = 244,
        Psyshock = 473,
        Psystrike = 540,
        Psywave = 149,
        Punishment = 386,
        Pursuit = 228, // TODO
        Quash = 511, // TODO
        QuickAttack = 98,
        QuickGuard = 501,
        QuiverDance = 483,
        Rage = 99, // TODO
        RagePowder = 476, // TODO
        RainDance = 240,
        RapidSpin = 229, // TODO
        RazorLeaf = 75,
        RazorShell = 534,
        RazorWind = 13, // TODO
        Recover = 105,
        Recycle = 278, // TODO
        Reflect = 115,
        ReflectType = 513,
        Refresh = 287,
        RelicSong = 547, // TODO
        Rest = 156, // TODO: Uproar, Leaf Guard
        Retaliate = 514,
        Return = 216,
        Revenge = 279, // TODO
        Reversal = 179,
        Roar = 46, // TODO: Suction Cups, Soundproof, Ingrain
        RoarOfTime = 459, // TODO
        RockBlast = 350,
        RockClimb = 431,
        RockPolish = 397,
        RockSlide = 157,
        RockSmash = 249,
        RockThrow = 88,
        RockTomb = 317,
        RockWrecker = 439, // TODO
        RolePlay = 272,
        RollingKick = 27,
        Rollout = 205, // TODO
        Roost = 355,
        Round = 496, // TODO
        SacredFire = 221,
        SacredSword = 533,
        Safeguard = 219,
        SandAttack = 28,
        Sandstorm = 201,
        SandTomb = 328, // TODO
        Scald = 503,
        ScaryFace = 184,
        Scratch = 10,
        Screech = 103,
        SearingShot = 545,
        SecretPower = 290,
        SecretSword = 548,
        SeedBomb = 402,
        SeedFlare = 465,
        SeismicToss = 69,
        Selfdestruct = 120,
        ShadowBall = 247,
        ShadowClaw = 421,
        ShadowForce = 467,
        ShadowPunch = 325,
        ShadowSneak = 425,
        Sharpen = 159,
        SheerCold = 329,
        ShellSmash = 504,
        ShiftGear = 508,
        ShockWave = 351,
        SignalBeam = 324,
        SilverWind = 318,
        SimpleBeam = 493,
        Sing = 47,
        Sketch = 166, // TODO
        SkillSwap = 285, // TODO
        SkullBash = 130, // TODO
        SkyAttack = 143, // TODO
        SkyDrop = 507, // TODO
        SkyUppercut = 327,
        SlackOff = 303,
        Slam = 21,
        Slash = 163,
        SleepPowder = 79,
        SleepTalk = 214, // TODO & TODO: Moves such as ice ball/outrage that last multiple turns are not locked, even upon waking up (is this the same for metronome etc?)
        Sludge = 124,
        SludgeBomb = 188,
        SludgeWave = 482,
        SmackDown = 479, // TODO
        SmellingSalt = 265,
        Smog = 123,
        SmokeScreen = 108,
        Snarl = 555,
        Snatch = 289, // TODO
        Snore = 173,
        Soak = 487,
        Softboiled = 135,
        SolarBeam = 76, // TODO
        SonicBoom = 49,
        SpacialRend = 460,
        Spark = 209,
        SpiderWeb = 169, // TODO
        SpikeCannon = 131,
        Spikes = 191,
        Spite = 180, // TODO
        SpitUp = 255, // TODO
        Splash = 150,
        Spore = 147,
        StealthRock = 446,
        Steamroller = 537,
        SteelWing = 211,
        Stockpile = 254, // TODO
        Stomp = 23,
        StoneEdge = 444,
        StoredPower = 500,
        StormThrow = 480,
        Strength = 70,
        StringShot = 81,
        Struggle = 165,
        StruggleBug = 522,
        StunSpore = 78,
        Submission = 66,
        Substitute = 164,
        SuckerPunch = 389,
        SunnyDay = 241,
        SuperFang = 162,
        Superpower = 276,
        Supersonic = 48,
        Surf = 57,
        Swagger = 207,
        Swallow = 256, // TODO
        SweetKiss = 186,
        SweetScent = 230,
        Swift = 129,
        Switcheroo = 415, // TODO
        SwordsDance = 14,
        Synchronoise = 485, // TODO
        Synthesis = 235,
        Tackle = 33,
        TailGlow = 294,
        TailSlap = 541,
        TailWhip = 39,
        Tailwind = 366,
        TakeDown = 36,
        Taunt = 269, // TODO
        TechnoBlast = 546,
        TeeterDance = 298,
        Telekinesis = 477, // TODO
        Teleport = 100,
        Thief = 168, // TODO
        Thrash = 37, // TODO
        Thunder = 87,
        Thunderbolt = 85,
        ThunderFang = 422,
        ThunderPunch = 9,
        ThunderShock = 84,
        ThunderWave = 86,
        Tickle = 321,
        Torment = 259, // TODO
        Toxic = 92,
        ToxicSpikes = 390,
        Transform = 144,
        TriAttack = 161,
        Trick = 271, // TODO
        TrickRoom = 433,
        TripleKick = 167, // TODO
        TrumpCard = 376, // TODO
        Twineedle = 41,
        Twister = 239,
        Uproar = 253, // TODO
        Uturn = 369, // TODO
        VacuumWave = 410,
        VCreate = 557,
        Venoshock = 474,
        ViceGrip = 11,
        VineWhip = 22,
        VitalThrow = 233,
        VoltSwitch = 521, // TODO
        VoltTackle = 344,
        WakeUpSlap = 358,
        Waterfall = 127,
        WaterGun = 55,
        WaterPledge = 518, // TODO
        WaterPulse = 352,
        WaterSport = 346, // TODO
        WaterSpout = 323,
        WeatherBall = 311,
        Whirlpool = 250, // TODO
        Whirlwind = 18, // TODO: Suction Cups, Ingrain
        WideGuard = 469,
        WildCharge = 528,
        WillOWisp = 261,
        WingAttack = 17,
        Wish = 273, // TODO
        Withdraw = 110,
        WonderRoom = 472, // TODO
        WoodHammer = 452,
        WorkUp = 526,
        WorrySeed = 388,
        Wrap = 35, // TODO
        WringOut = 378,
        XScissor = 404,
        Yawn = 281, // TODO
        ZapCannon = 192,
        ZenHeadbutt = 428,
        /// <summary>Invalid move.</summary>
        MAX = 560
    }
}
