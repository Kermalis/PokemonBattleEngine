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
    public enum PTarget : byte
    {
        AllButSelf,
        AllyLeft,
        AllyMiddle,
        AllyRight,
        FoeLeft,
        FoeMiddle,
        FoeRight,
        Self
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
        AllFoesSurrounding, // All foes surrounding
        Any, // Single battler except itself
        AnySurrounding, // Single battler surrounding
        Self // Self
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
        Change_Opponent_ATK,
        Change_User_SPATK,
        Hit,
        Hit__MaybeBurn,
        Hit__MaybeFlinch,
        Hit__MaybeFreeze,
        Hit__MaybeLower_SPDEF_By1,
        Hit__MaybeParalyze,
        Lower_DEF_SPDEF_By1_Raise_ATK_SPATK_SPD_By2,
        Moonlight, // TODO
        Toxic,
        Transform, // TODO
    }
    public enum PMove : ushort
    {
        None,
        AquaJet,
        DarkPulse,
        DragonPulse,
        Frustration,
        Growl,
        HiddenPower,
        HydroPump,
        IceBeam,
        IcePunch,
        Inferno,
        Moonlight, // TODO
        NastyPlot,
        Psychic,
        Retaliate, // TODO
        Return,
        ShellSmash,
        Tackle,
        Thunder,
        Thunderbolt,
        Toxic,
        Transform, // TODO
        Waterfall,
        MAX
    }
}
