namespace PokemonBattleEngine.Data
{
    enum Gender : byte
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
    enum PossibleTarget // Used in MoveData
    {
        Self,
        AnySurrounding,
        Any,
    }
    enum Target
    {
        Self,
        FoeLeft,
        FoeMiddle,
        FoeRight,
        AllyLeft,
        AllyMiddle,
        AllyRight,
        All
    }
    enum Status
    {
        None,
        Poisoned,
        BadlyPoisoned,
        Paralyzed,
        Asleep,
        Frozen,
        Burned
    }
    enum Type
    {
        Fire,
        Grass,
        Water,
        Dark,
        Psychic,
        Ghost,
        Rock,
        Ground,
        Steel,
        Electric,
        Ice,
        Bug,
        Flying,
        Dragon,
        Fighting,
        Normal,
        Poison,
        MAX
    }
    enum Nature
    {
        Hardy,
        Loney,
        Brave,
        Adamant,
        Naughty,
        Bold,
        Docile,
        Relaxed,
        Impish,
        Lax,
        Timid,
        Hasty,
        Serious,
        Jolly,
        Naive,
        Modest,
        Mild,
        Quiet,
        Bashful,
        Rash,
        Calm,
        Gentle,
        Sassy,
        Careful,
        Quirky
    }
    enum ItemEffect
    {
        None,
    }
    enum Item
    {
        None,
        ChoiceBand,
        SoulDew,
    }
    enum Ability
    {
        None,
        Overgrow,
        Blaze,
        Torrent,
        Swarm,
        Levitate,
        Hustle,
        Guts,
        ThickFat,
        HugePower,
        PurePower,
        MarvelScale,
        BadDreams,
        SapSipper,
    }
    enum Species
    {
        Azumarill = 184,
        Latias = 380,
        Latios,
        Cresselia = 488,
        Darkrai = 491,
    }
    enum MoveCategory
    {
        Status,
        Physical,
        Special
    }
    enum MoveEffect
    {
        None,
        LowerSPDEFBy1,
        Flinch,
    }
    enum Move
    {
        None,
        Tackle,
        HydroPump,
        DragonPulse,
        Psychic,
        DarkPulse,
    }
}
