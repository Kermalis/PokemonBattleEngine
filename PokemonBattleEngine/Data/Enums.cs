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
        Speed,
        Accuracy,
        Evasion
    }
    public enum PTarget : byte
    {
        All,
        AllyLeft,
        AllyMiddle,
        AllyRight,
        FoeLeft,
        FoeMiddle,
        FoeRight,
        Self
    }
    public enum PStatus : byte
    {
        NoStatus,
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
        Cursed = 1 << 1,
        Flinching = 1 << 2,
        FocusEnergy = 1 << 3,
        Infatuated = 1 << 4,
        Tormented = 1 << 5,
        Underground = 1 << 6,
        Underwater = 1 << 7,
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
        NoItem,
        ChoiceBand,
        DeepSeaScale,
        DeepSeaTooth,
        Leftovers,
        LightBall,
        MetalPowder,
        SoulDew,
        ThickClub,
        MAX
    }
    public enum PAbility : byte
    {
        None, // Only used when an ability is removed
        BadDreams,
        BattleArmor,
        Blaze,
        Guts,
        HugePower,
        Hustle,
        Imposter,
        Levitate,
        LightningRod,
        Limber,
        MarvelScale,
        Minus,
        Overgrow,
        Plus,
        PurePower,
        Rattled,
        RockHead,
        SapSipper,
        ShellArmor,
        Static,
        Swarm,
        ThickFat,
        Torrent,
    }
    public enum PSpecies : ushort
    {
        None,
        Pikachu = 25,
        Cubone = 104,
        Marowak,
        Ditto = 132,
        Azumarill = 184,
        Clamperl = 366,
        Latias = 380,
        Latios,
        Cresselia = 488,
        Darkrai = 491
    }
    public enum PMoveCategory : byte
    {
        Status,
        Physical,
        Special
    }
    public enum PMoveTarget : byte // Used in MoveData
    {
        Any,
        AnySurrounding,
        Self
    }
    public enum PMoveEffect : byte
    {
        Hit,
        Hit__MaybeFlinch,
        Hit__MaybeFreeze,
        Hit__MaybeLower_SPDEF_By1,
        Hit__MaybeParalyze,
        Lower_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2,
        Moonlight,
        Toxic,
        Transform,
    }
    [Flags]
    public enum PMoveFlag : byte
    {
        None = 0,
        MakesContact = 1 << 0,
        AffectedByProtect = 1 << 1,
        AffectedByMagicCoat = 1 << 2,
        AffectedBySnatch = 1 << 3,
        AffectedByMirrorMove = 1 << 4
    }
    public enum PMove : ushort
    {
        None,
        AquaJet,
        DarkPulse,
        DragonPulse,
        HydroPump,
        IceBeam,
        IcePunch,
        Moonlight,
        Psychic,
        Retaliate,
        Return,
        ShellSmash,
        Tackle,
        Thunder,
        Toxic,
        Transform,
        Waterfall,
    }
}
