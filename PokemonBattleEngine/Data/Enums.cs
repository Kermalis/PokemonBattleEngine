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
    public enum PBattleStyle : byte
    {
        Single,
        Double,
        Triple,
        Rotation
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
    public enum PStatus1 : byte
    {
        None,
        Asleep, // TODO
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
        Confused = 1 << 0, // TODO
        Cursed = 1 << 1, // TODO
        Flinching = 1 << 2,
        FocusEnergy = 1 << 3, // TODO
        Infatuated = 1 << 4, // TODO
        Tormented = 1 << 5, // TODO
        Underground = 1 << 6, // TODO
        Underwater = 1 << 7, // TODO
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
        ChoiceBand, // TODO: Lock move
        DeepSeaScale,
        DeepSeaTooth,
        Leftovers,
        LightBall,
        MetalPowder, // TODO
        SoulDew,
        ThickClub,
        MAX
    }
    public enum PAbility : byte
    {
        None, // Only used when an ability is removed
        BadDreams, // TODO
        BattleArmor, // TODO
        Blaze,
        Guts,
        HugePower,
        Hustle, // TODO: Accuracy
        Imposter, // TODO
        Levitate, // TODO
        LightningRod, // TODO
        Limber, // TODO
        MarvelScale,
        Minus, // TODO
        Overgrow,
        Plus, // TODO
        PurePower,
        Rattled, // TODO
        RockHead, // TODO
        SapSipper, // TODO
        ShellArmor, // TODO
        Static, // TODO
        Swarm,
        ThickFat,
        Torrent,
        MAX,
    }
    public enum PSpecies : ushort
    {
        None,
        Pikachu = 25, // TODO
        Cubone = 104, // TODO
        Marowak, // TODO
        Ditto = 132, // TODO
        Pichu = 172, // TODO
        Azumarill = 184, // TODO
        Clamperl = 366, // TODO
        Latias = 380, // TODO
        Latios, // TODO
        Cresselia = 488, // TODO
        Darkrai = 491 // TODO
    }
    public enum PMoveCategory : byte
    {
        Status,
        Physical,
        Special
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
    public enum PMoveFlag : byte
    {
        None = 0,
        MakesContact = 1 << 0,
        AffectedByProtect = 1 << 1,
        AffectedByMagicCoat = 1 << 2,
        AffectedBySnatch = 1 << 3,
        AffectedByMirrorMove = 1 << 4,
        SoundBased = 1 << 5
    }
    public enum PMoveEffect : byte
    {
        ChangeTarget_ATK,
        ChangeTarget_DEF,
        ChangeTarget_EVA,
        ChangeTarget_SPE,
        ChangeUser_DEF,
        ChangeUser_SPATK,
        Hit,
        Hit__MaybeBurn,
        Hit__MaybeFlinch,
        Hit__MaybeFreeze,
        Hit__MaybeLowerTarget_SPDEF_By1,
        Hit__MaybeParalyze,
        Hit__MaybeRaiseUser_DEF_By1,
        LowerTarget_ATK_DEF_By1,
        LowerUser_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2,
        Moonlight, // TODO
        RaiseUser_ATK_SPE_By1,
        RaiseUser_SPATK_SPDEF_By1,
        Toxic,
        Transform, // TODO
    }
    public enum PMove : ushort
    {
        None,
        AerialAce,
        AquaJet,
        CalmMind,
        Charm,
        DarkPulse,
        DragonBreath,
        DragonClaw,
        DragonDance,
        DragonPulse,
        Frustration,
        Growl,
        Harden,
        HiddenPower,
        HydroPump,
        IceBeam,
        IcePunch,
        Inferno,
        Leer,
        Lick,
        Megahorn,
        Moonlight, // TODO
        NastyPlot,
        Psychic,
        Retaliate, // TODO
        Return,
        ScaryFace,
        Scratch,
        ShellSmash,
        Spark,
        SteelWing,
        SweetScent,
        Tackle,
        Tickle,
        Thunder,
        Thunderbolt,
        Toxic,
        Transform, // TODO
        Waterfall,
        XScissor,
        MAX
    }
}
