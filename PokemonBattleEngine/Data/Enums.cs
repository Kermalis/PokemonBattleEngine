using Kermalis.PokemonBattleEngine.Battle;
using System;

namespace Kermalis.PokemonBattleEngine.Data
{
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
    /// <summary>Represents the battle's terrain.</summary>
    public enum PBEBattleTerrain : byte
    {
        Cave, // Rock, RockSlide, RockThrow
        Grass, // Grass, SeedBomb, NeedleArm
        /// <summary>Used for bridges, buildings, and link battles.</summary>
        Plain, // Normal, TriAttack, BodySlam
        Puddle, // Ground, MudBomb, MudShot
        Sand, // Ground, Earthquake, MudSlap
        Snow, // Ice, Blizzard, Avalanche
        Water, // Water, HydroPump, WaterPulse
        MAX
    }
    /// <summary>Represents the format of a specific battle.</summary>
    public enum PBEBattleFormat : byte
    {
        /// <summary>A 1v1 battle. Each Pokémon is able to use moves or switch out for another Pokémon.</summary>
        Single,
        /// <summary>A 2v2 battle where all Pokémon are able to use moves or switch out for another Pokémon.</summary>
        Double,
        /// <summary>A 3v3 battle where all Pokémon are able to use moves, shift positions with a teammate, or switch out for another Pokémon.</summary>
        Triple,
        /// <summary>A 3v3 battle where only the front Pokémon are able to force a team rotation, use a move, or switch out for another Pokémon.</summary>
        /// <remarks>Team rotation does not take up a turn and can be done once per turn.</remarks>
        Rotation,
        /// <summary>Invalid battle format.</summary>
        MAX
    }
    /// <summary>Represents the current state of a specific battle.</summary>
    public enum PBEBattleState : byte
    {
        /// <summary>The battle is waiting for team shells.</summary>
        WaitingForPlayers,
        /// <summary>The battle is ready to begin.</summary>
        ReadyToBegin,
        /// <summary>The battle is waiting for players to send actions.</summary>
        WaitingForActions,
        /// <summary>The battle is ready to run a turn.</summary>
        ReadyToRunTurn,
        /// <summary>The battle is processing.</summary>
        Processing,
        /// <summary>The battle is waiting for players to send switch-ins.</summary>
        WaitingForSwitchIns,
        /// <summary>The battle ended.</summary>
        Ended
    }
    /// <summary>Represents the weather in a specific battle.</summary>
    public enum PBEWeather : byte
    {
        /// <summary>There is no weather.</summary>
        None,
        /// <summary>It is hailing.</summary>
        Hailstorm,
        /// <summary>The sunlight is harsh.</summary>
        HarshSunlight,
        /// <summary>It is raining.</summary>
        Rain,
        /// <summary>A sandstorm is brewing.</summary>
        Sandstorm
    }
    /// <summary>Represents a position on the battle field.</summary>
    public enum PBEFieldPosition : byte
    {
        /// <summary>A Pokémon is not on the field.</summary>
        None,
        /// <summary>The Pokémon to a player's left in a Double, Triple, or Rotation battle.</summary>
        Left,
        /// <summary>The Pokémon in the center of the field in a Single, Triple, or Rotation battle.</summary>
        Center,
        /// <summary>The Pokémon to a player's right in a Double, Triple, or Rotation battle.</summary>
        Right
    }
    /// <summary>Represents a <see cref="PBEMove"/>'s targets.</summary>
    [Flags]
    public enum PBETurnTarget : byte
    {
        /// <summary>The Pokémon has not chosen any targets.</summary>
        None,
        /// <summary>The move is targetting the player's left Pokémon.</summary>
        AllyLeft = 1 << 0,
        /// <summary>The move is targetting the player's center Pokémon.</summary>
        AllyCenter = 1 << 1,
        /// <summary>The move is targetting the player's right Pokémon.</summary>
        AllyRight = 1 << 2,
        /// <summary>The move is targetting the opponent's left Pokémon.</summary>
        FoeLeft = 1 << 3,
        /// <summary>The move is targetting the opponent's center Pokémon.</summary>
        FoeCenter = 1 << 4,
        /// <summary>The move is targetting the opponent's right Pokémon.</summary>
        FoeRight = 1 << 5
    }
    /// <summary>Represents a Pokémon's decision for a turn.</summary>
    public enum PBETurnDecision : byte
    {
        /// <summary>The Pokémon has not made a decision.</summary>
        None,
        /// <summary>The Pokémon has chosen to use a move.</summary>
        Fight,
        /// <summary>The Pokémon has chosen to switch out for another Pokémon.</summary>
        SwitchOut
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
    [Flags]
    public enum PBEMoveObtainMethod : ulong
    {
        /// <summary>There is no way to learn this move.</summary>
        None,
        /// <summary>The move can be learned by levelling up a Pokémon in Pokémon Ruby Version and Pokémon Sapphire Version.</summary>
        LevelUp_RSColoXD = 1uL << 0,
        /// <summary>The move can be learned by levelling up a Pokémon in Pokémon Fire Red Version.</summary>
        LevelUp_FR = 1uL << 1,
        /// <summary>The move can be learned by levelling up a Pokémon in Pokémon Leaf Green Version.</summary>
        LevelUp_LG = 1uL << 2,
        /// <summary>The move can be learned by levelling up a Pokémon in Pokémon Emerald Version.</summary>
        LevelUp_E = 1uL << 3,
        /// <summary>The move can be learned by levelling up a Pokémon in Pokémon Diamond Version and Pokémon Pearl Version.</summary>
        LevelUp_DP = 1uL << 4,
        /// <summary>The move can be learned by levelling up a Pokémon in Pokémon Platinum Version.</summary>
        LevelUp_Pt = 1uL << 5,
        /// <summary>The move can be learned by levelling up a Pokémon in Pokémon HeartGold Version and Pokémon SoulSilver Version.</summary>
        LevelUp_HGSS = 1uL << 6,
        /// <summary>The move can be learned by levelling up a Pokémon in Pokémon Black Version and Pokémon White Version.</summary>
        LevelUp_BW = 1uL << 7,
        /// <summary>The move can be learned by levelling up a Pokémon in Pokémon Black Version 2 and Pokémon White Version 2.</summary>
        LevelUp_B2W2 = 1uL << 8,
        /// <summary>The move can be learned by using a technical machine on a Pokémon in Pokémon Ruby Version, Pokémon Sapphire Version, Pokémon Fire Red Version, Pokémon Leaf Green Version, Pokémon Emerald Version, Pokémon Colosseum, and Pokémon XD: Gale of Darkness.</summary>
        TM_RSFRLGEColoXD = 1uL << 9,
        /// <summary>The move can be learned by using a technical machine on a Pokémon in Pokémon Diamond Version, Pokémon Pearl Version, and Pokémon Platinum Version.</summary>
        TM_DPPt = 1uL << 10,
        /// <summary>The move can be learned by using a technical machine on a Pokémon in Pokémon HeartGold Version and Pokémon SoulSilver Version.</summary>
        TM_HGSS = 1uL << 11,
        /// <summary>The move can be learned by using a technical machine on a Pokémon in Pokémon Black Version and Pokémon White Version.</summary>
        TM_BW = 1uL << 12,
        /// <summary>The move can be learned by using a technical machine on a Pokémon in Pokémon Black Version 2 and Pokémon White Version 2.</summary>
        TM_B2W2 = 1uL << 13,
        /// <summary>The move can be learned by using a hidden machine on a Pokémon in Pokémon Ruby Version, Pokémon Sapphire Version, Pokémon Fire Red Version, Pokémon Leaf Green Version, Pokémon Emerald Version, Pokémon Colosseum, and Pokémon XD: Gale of Darkness.</summary>
        HM_RSFRLGEColoXD = 1uL << 14,
        /// <summary>The move can be learned by using a hidden machine on a Pokémon in Pokémon Diamond Version, Pokémon Pearl Version, and Pokémon Platinum Version.</summary>
        HM_DPPt = 1uL << 15,
        /// <summary>The move can be learned by using a hidden machine on a Pokémon in Pokémon HeartGold Version and Pokémon SoulSilver Version.</summary>
        HM_HGSS = 1uL << 16,
        /// <summary>The move can be learned by using a hidden machine on a Pokémon in Pokémon Black Version, Pokémon White Version, Pokémon Black Version 2, and Pokémon White Version 2.</summary>
        HM_BWB2W2 = 1uL << 17,
        /// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon Fire Red Version and Pokémon Leaf Green Version.</summary>
        MoveTutor_FRLG = 1uL << 18,
        /// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon Emerald Version.</summary>
        MoveTutor_E = 1uL << 19,
        /// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon XD: Gale of Darkness.</summary>
        MoveTutor_XD = 1uL << 20,
        /// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon Diamond Version and Pokémon Pearl Version.</summary>
        MoveTutor_DP = 1uL << 21,
        /// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon Platinum Version.</summary>
        MoveTutor_Pt = 1uL << 22,
        /// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon HeartGold Version and Pokémon SoulSilver Version.</summary>
        MoveTutor_HGSS = 1uL << 23,
        /// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon Black Version and Pokémon White Version.</summary>
        MoveTutor_BW = 1uL << 24,
        /// <summary>The move can be taught to a Pokémon by a move tutor in Pokémon Black Version 2 and Pokémon White Version 2.</summary>
        MoveTutor_B2W2 = 1uL << 25,
        /// <summary>The move can be learned by hatching a Pokémon egg in Pokémon Ruby Version, Pokémon Sapphire Version, Pokémon Fire Red Version, Pokémon Leaf Green Version, and Pokémon Emerald Version.</summary>
        EggMove_RSFRLGE = 1uL << 26,
        /// <summary>The move can be learned by hatching a Pokémon egg in Pokémon Diamond Version, Pokémon Pearl Version, and Pokémon Platinum Version.</summary>
        EggMove_DPPt = 1uL << 27,
        /// <summary>The move can be learned by hatching a Pokémon egg in Pokémon HeartGold Version and Pokémon SoulSilver Version.</summary>
        EggMove_HGSS = 1uL << 28,
        /// <summary>The move can be learned by hatching a Pokémon egg in Pokémon Black Version, Pokémon White Version, Pokémon Black Version 2, and Pokémon White Version 2.</summary>
        EggMove_BWB2W2 = 1uL << 29,
        /// <summary>The move is known by a Pokémon when found in the Dream World.</summary>
        DreamWorld = 1uL << 30,
        /// <summary>The move can be learned by hatching a Pokémon egg under special conditions.</summary>
        EggMove_Special = 1uL << 31,
        /// <summary>The move is learned by a Pokémon when changing forms.</summary>
        Form = 1uL << 32
    }
    /// <summary>Represents a specific Pokémon's non-volatile status.</summary>
    public enum PBEStatus1 : byte
    {
        /// <summary>The Pokémon has no status.</summary>
        None,
        /// <summary>The Pokémon is asleep.</summary>
        Asleep,
        /// <summary>The Pokémon is badly poisoned.</summary>
        BadlyPoisoned,
        /// <summary>The Pokémon is burned.</summary>
        Burned,
        /// <summary>The Pokémon is frozen.</summary>
        Frozen,
        /// <summary>The Pokémon is paralyzed.</summary>
        Paralyzed,
        /// <summary>The Pokémon is poisoned.</summary>
        Poisoned
    }
    /// <summary>Represents a specific Pokémon's volatile status.</summary>
    [Flags]
    public enum PBEStatus2 : uint
    {
        /// <summary>The Pokémon has no status.</summary>
        None,
        /// <summary>The Pokémon is high up in the air. A move will miss against the Pokémon unless it has <see cref="PBEMoveFlag.HitsAirborne"/> or either Pokémon has <see cref="PBEAbility.NoGuard"/>.</summary>
        Airborne = 1 << 0,
        /// <summary>The Pokémon is confused and may hurt itself instead of execute its chosen move.</summary>
        Confused = 1 << 1,
        /// <summary>The Pokémon is cursed and will take damage at the end of each turn.</summary>
        Cursed = 1 << 2,
        /// <summary>The Pokémon is disguised as <see cref="PBEPokemon.DisguisedAsPokemon"/> with <see cref="PBEAbility.Illusion"/>.</summary>
        Disguised = 1 << 3,
        /// <summary>The Pokémon is flinching and will be unable to move this turn.</summary>
        Flinching = 1 << 4,
        /// <summary>The Pokémon will gain a power boost due to <see cref="PBEMove.HelpingHand"/>.</summary>
        HelpingHand = 1 << 5,
        /// <summary>The Pokémon is infatuated with <see cref="PBEPokemon.InfatuatedWithPokemon"/> and may be unable to move this turn.</summary>
        Infatuated = 1 << 6,
        /// <summary>The Pokémon is seeded and HP will be stolen at the end of each turn.</summary>
        LeechSeed = 1 << 7,
        LockOn = 1 << 8,
        MagnetRise = 1 << 9,
        MiracleEye = 1 << 10,
        /// <summary>The Pokémon's <see cref="PBEStat.Attack"/> and <see cref="PBEStat.Defense"/> are switched.</summary>
        PowerTrick = 1 << 11,
        /// <summary>The Pokémon is protected from moves this turn.</summary>
        Protected = 1 << 12,
        /// <summary>The Pokémon is under the effect of <see cref="PBEMove.FocusEnergy"/> or <see cref="PBEItem.LansatBerry"/> and has a higher chance of landing critical hits.</summary>
        Pumped = 1 << 13,
        /// <summary>The Pokémon is behind a substitute that will take damage on behalf of the Pokémon and prevent most moves from affecting the Pokémon.</summary>
        Substitute = 1 << 14,
        /// <summary>The Pokémon is transformed into another Pokémon.</summary>
        Transformed = 1 << 15,
        /// <summary>The Pokémon is underground. A move will miss against the Pokémon unless it has <see cref="PBEMoveFlag.HitsUnderground"/> or either Pokémon has <see cref="PBEAbility.NoGuard"/>.
        /// The Pokémon will take double damage from <see cref="PBEMove.Earthquake"/> and <see cref="PBEMove.Magnitude"/>.</summary>
        Underground = 1 << 16,
        /// <summary>The Pokémon is underwater. A move will miss against the Pokémon unless it has <see cref="PBEMoveFlag.HitsUnderwater"/> or either Pokémon has <see cref="PBEAbility.NoGuard"/>.
        /// The Pokémon will take double damage from <see cref="PBEMove.Surf"/> and <see cref="PBEMove.Whirlpool"/>.</summary>
        Underwater = 1 << 17
    }
    /// <summary>Represents a specific <see cref="PBEBattle"/>'s status.</summary>
    [Flags]
    public enum PBEBattleStatus : byte
    {
        /// <summary>The battle has no status.</summary>
        None,
        /// <summary>The acting order of Pokémon in this battle is reversed.</summary>
        TrickRoom = 1 << 0 // TODO: Full Incense, Lagging Tail, Stall, Quick Claw
    }
    /// <summary>Represents a specific <see cref="PBETeam"/>'s status.</summary>
    [Flags]
    public enum PBETeamStatus : ushort
    {
        /// <summary>The team has no status.</summary>
        None,
        /// <summary>The team will take less damage from <see cref="PBEMoveCategory.Special"/> moves.</summary>
        LightScreen = 1 << 0,
        /// <summary>The team is shielded from critical hits.</summary>
        LuckyChant = 1 << 1,
        /// <summary>The team will take less damage from <see cref="PBEMoveCategory.Physical"/> moves.</summary>
        Reflect = 1 << 2,
        Safeguard = 1 << 3,
        /// <summary>Grounded Pokémon that switch in will take damage. The amount of damage is based on <see cref="PBETeam.SpikeCount"/>. </summary>
        Spikes = 1 << 4, // TODO: Gravity, magic guard, iron ball, baton pass with ingrain, air balloon
        /// <summary>Pokémon that switch in will take damage. The amount of damage is based on the effectiveness of <see cref="PBEType.Rock"/> on the Pokémon. </summary>
        StealthRock = 1 << 5, // TODO: magic guard
        Tailwind = 1 << 6,
        /// <summary>Grounded Pokémon that switch in will be <see cref="PBEStatus1.Poisoned"/> if <see cref="PBETeam.ToxicSpikeCount"/> is 1 or <see cref="PBEStatus1.BadlyPoisoned"/> if it is 2.
        /// Grounded <see cref="PBEType.Poison"/> Pokémon will remove toxic spikes.</summary>
        ToxicSpikes = 1 << 7, // TODO: Gravity, magic guard, iron ball, baton pass with ingrain, air balloon, synchronize with roar/whirlwind
        /// <summary>The team is protected from spread moves for a turn.</summary>
        WideGuard = 1 << 8
    }
    /// <summary>Represents an action regarding a <see cref="PBEAbility"/>.</summary>
    public enum PBEAbilityAction : byte
    {
        /// <summary>The ability is first announced.</summary>
        Announced = 0,
        /// <summary>The ability caused a Pokémon to change its appearance.</summary>
        ChangedAppearance = 1,
        /// <summary>The ability changed a Pokémon's <see cref="PBEStatus1"/> or <see cref="PBEStatus2"/>.</summary>
        ChangedStatus = 2,
        /// <summary>The ability was involved with damage.</summary>
        Damage = 3,
        /// <summary>The ability prevented a Pokémon from being inflicted with a <see cref="PBEStatus1"/> or <see cref="PBEStatus2"/>.</summary>
        PreventedStatus = 4,
        /// <summary>The ability restored a Pokémon's HP.</summary>
        RestoredHP = 5,
        /// <summary><see cref="PBEAbility.SlowStart"/> ended.</summary>
        SlowStart_Ended = 6,
        /// <summary>The ability was involved with stats.</summary>
        Stats = 7,
        /// <summary>The ability was involved with weather.</summary>
        Weather = 8
    }
    /// <summary>Represents an action regarding a <see cref="PBEItem"/>.</summary>
    public enum PBEItemAction : byte
    {
        /// <summary>The item caused a Pokémon's <see cref="PBEStatus1"/> or <see cref="PBEStatus2"/> to change.</summary>
        ChangedStatus = 0,
        /// <summary>The item was consumed by a Pokémon.</summary>
        Consumed = 1,
        /// <summary>The item was involved with damage.</summary>
        Damage = 2,
        /// <summary>The item restored HP to a Pokémon.</summary>
        RestoredHP = 3
    }
    /// <summary>Represents an action regarding a <see cref="PBEStatus1"/> or <see cref="PBEStatus2"/>.</summary>
    public enum PBEStatusAction : byte
    {
        /// <summary>The status activated its main effect.</summary>
        /// <example>
        /// <see cref="PBEStatus2.Flinching"/> prevented movement.
        /// </example>
        Activated = 0,
        /// <summary>The status was added to a Pokémon.</summary>
        /// <example>
        /// The Pokémon became <see cref="PBEStatus1.Paralyzed"/>.
        /// </example>
        Added = 1,
        /// <summary>The status caused a Pokémon to be immobile.</summary>
        CausedImmobility = 2,
        /// <summary>The status was cured from a Pokémon.</summary>
        /// <example>
        /// <see cref="PBEAbility.Limber"/> cured a Pokémon of <see cref="PBEStatus1.Paralyzed"/>.
        /// </example>
        Cured = 3,
        /// <summary>The status was involved with damage.</summary>
        /// <example>
        /// A Pokémon's <see cref="PBEStatus2.Substitute"/> took damage.
        /// </example>
        Damage = 4,
        /// <summary>The status has ended.</summary>
        /// <example>
        /// A Pokémon with <see cref="PBEStatus2.Confused"/> regained its senses.
        /// </example>
        Ended = 5
    }
    public enum PBEBattleStatusAction : byte
    {
        Added = 0,
        Cleared = 1,
        Ended = 2
    }
    /// <summary>Represents an action regarding a <see cref="PBETeamStatus"/>.</summary>
    public enum PBETeamStatusAction : byte
    {
        /// <summary>The status was added to a team.</summary>
        /// <example>
        /// The team set up <see cref="PBETeamStatus.LightScreen"/>.
        /// </example>
        Added = 0,
        /// <summary>The status was removed from a team.</summary>
        /// <example>
        /// An opponent used <see cref="PBEMove.BrickBreak"/> and destroyed <see cref="PBETeamStatus.Reflect"/>.
        /// </example>
        Cleared = 1,
        /// <summary>The status caused a Pokémon to take damage.</summary>
        /// <example>
        /// An Pokémon switched in and took damage from <see cref="PBETeamStatus.StealthRock"/>.
        /// </example>
        Damage = 2,
        /// <summary>The status ended.</summary>
        /// <example>
        /// <see cref="PBETeamStatus.LightScreen"/> wore off.
        /// </example>
        Ended = 3
    }
    /// <summary>Represents the result of an intention.</summary>
    public enum PBEResult : byte
    {
        /// <summary>No failure.</summary>
        Success = 0,
        /// <summary>Failure due to a <see cref="PBEAbility"/>.</summary>
        Ineffective_Ability = 1,
        /// <summary>Failure due to a <see cref="PBEGender"/>.</summary>
        Ineffective_Gender = 2,
        /// <summary>Failure due to a Pokémon's level.</summary>
        Ineffective_Level = 3,
        /// <summary>Failure due to <see cref="PBEStatus2.MagnetRise"/>.</summary>
        Ineffective_MagnetRise = 4,
        /// <summary>Failure due to <see cref="PBETeamStatus.Safeguard"/>.</summary>
        Ineffective_Safeguard = 5,
        /// <summary>Failure due to a <see cref="PBEStat"/>.</summary>
        Ineffective_Stat = 6,
        /// <summary>Failure due to a <see cref="PBEStatus1"/>, <see cref="PBEStatus2"/>, <see cref="PBETeamStatus"/>, <see cref="PBEBattleStatus"/>, or <see cref="PBEWeather"/>.</summary>
        Ineffective_Status = 7,
        /// <summary>Failure due to <see cref="PBEStatus2.Substitute"/>.</summary>
        Ineffective_Substitute = 8,
        /// <summary>Failure due to a <see cref="PBEType"/>.</summary>
        Ineffective_Type = 9,
        /// <summary>Failure due to the intention's unmet special conditions.</summary>
        InvalidConditions = 10,
        /// <summary>Failure due to having no available targets.</summary>
        NoTarget = 11,
        /// <summary>Limited success due to a Pokémon's <see cref="PBEType"/>.</summary>
        NotVeryEffective_Type = 12,
        /// <summary>Great success due to a Pokémon's <see cref="PBEType"/>.</summary>
        SuperEffective_Type = 13
    }
    /// <summary>Represents an action regarding a <see cref="PBEWeather"/>.</summary>
    public enum PBEWeatherAction : byte
    {
        /// <summary>The weather was added to the battle.</summary>
        Added = 0,
        /// <summary>The weather caused a Pokémon to take damage.</summary>
        CausedDamage = 1,
        /// <summary>The weather was removed from the battle.</summary>
        Ended = 2
    }
    public enum PBESpecialMessage : byte
    {
        DraggedOut = 0,
        Endure = 1,
        HPDrained = 2,
        Magnitude = 3,
        MultiHit = 4,
        NothingHappened = 5,
        OneHitKnockout = 6,
        PainSplit = 7,
        Recoil = 8,
        Struggle = 9
    }
    public enum PBEMoveLockType : byte
    {
        ChoiceItem = 0,
        Temporary = 1
    }
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
        /// <summary>No effect.</summary>
        Antidote = 18,
        ApicotBerry = 205,
        /// <summary>No effect.</summary>
        ArmorFossil = 104,
        AspearBerry = 153, // TODO
        /// <summary>No effect.</summary>
        Awakening = 21,
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
        /// <summary>No effect.</summary>
        BurnHeal = 19,
        /// <summary>No effect.</summary>
        Calcium = 49,
        /// <summary>No effect.</summary>
        Carbos = 48,
        /// <summary>No effect.</summary>
        Casteliacone = 591,
        CellBattery = 546, // TODO
        Charcoal = 249,
        ChartiBerry = 195, // TODO
        CheriBerry = 149, // TODO
        /// <summary>No effect.</summary>
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
        /// <summary>No effect.</summary>
        DireHit = 56,
        /// <summary>No effect.</summary>
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
        /// <summary>No effect.</summary>
        DreamBall = 576,
        /// <summary>No effect.</summary>
        DubiousDisc = 324,
        DurinBerry = 182, // TODO
        /// <summary>No effect.</summary>
        DuskBall = 13,
        /// <summary>No effect.</summary>
        DuskStone = 108,
        EarthPlate = 305,
        EjectButton = 547, // TODO
        /// <summary>No effect.</summary>
        Electirizer = 322,
        ElectricGem = 550,
        /// <summary>No effect.</summary>
        Elixir = 40,
        /// <summary>No effect.</summary>
        EnergyPowder = 34,
        /// <summary>No effect.</summary>
        EnergyRoot = 35,
        EnigmaBerry = 208, // TODO
        /// <summary>No effect.</summary>
        EscapeRope = 78,
        /// <summary>No effect.</summary>
        Ether = 38,
        /// <summary>No effect.</summary>
        Everstone = 229,
        Eviolite = 538,
        ExpertBelt = 268,
        /// <summary>No effect.</summary>
        ExpShare = 216,
        /// <summary>No effect.</summary>
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
        /// <summary>No effect.</summary>
        FluffyTail = 64,
        FlyingGem = 556,
        FocusBand = 230,
        FocusSash = 275,
        /// <summary>No effect.</summary>
        FreshWater = 30,
        /// <summary>No effect.</summary>
        FriendBall = 497,
        /// <summary>No effect.</summary>
        FullHeal = 27,
        FullIncense = 316, // TODO
        /// <summary>No effect.</summary>
        FullRestore = 23,
        GanlonBerry = 202,
        /// <summary>No effect.</summary>
        GeniusWing = 568,
        GhostGem = 560,
        /// <summary>No effect.</summary>
        GooeyMulch = 98,
        GrassGem = 551,
        /// <summary>No effect.</summary>
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
        /// <summary>No effect.</summary>
        GuardSpec = 55,
        HabanBerry = 197, // TODO
        HardStone = 238,
        /// <summary>No effect.</summary>
        HealBall = 14,
        /// <summary>No effect.</summary>
        HealPowder = 36,
        /// <summary>No effect.</summary>
        HealthWing = 565,
        /// <summary>No effect.</summary>
        HeartScale = 93,
        HeatRock = 284,
        /// <summary>No effect.</summary>
        HeavyBall = 495,
        /// <summary>No effect.</summary>
        HelixFossil = 101,
        HondewBerry = 172, // TODO
        /// <summary>No effect.</summary>
        Honey = 94,
        /// <summary>No effect.</summary>
        HPUp = 45,
        /// <summary>No effect.</summary>
        HyperPotion = 25,
        IapapaBerry = 163,
        IceGem = 552,
        /// <summary>No effect.</summary>
        IceHeal = 20,
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
        /// <summary>No effect.</summary>
        LavaCookie = 42,
        LaxIncense = 255,
        /// <summary>No effect.</summary>
        LeafStone = 85,
        Leftovers = 234,
        /// <summary>No effect.</summary>
        Lemonade = 32,
        LeppaBerry = 154, // TODO
        /// <summary>No effect.</summary>
        LevelBall = 493,
        LiechiBerry = 201,
        LifeOrb = 270,
        LightBall = 236,
        LightClay = 269,
        /// <summary>No effect.</summary>
        LikeMail = 142,
        /// <summary>No effect.</summary>
        LoveBall = 496,
        /// <summary>No effect.</summary>
        LuckIncense = 319,
        /// <summary>No effect.</summary>
        LuckyEgg = 231,
        LuckyPunch = 256,
        LumBerry = 157, // TODO
        /// <summary>No effect.</summary>
        LureBall = 494,
        LustrousOrb = 136,
        /// <summary>No effect.</summary>
        LuxuryBall = 11,
        MachoBrace = 215,
        /// <summary>No effect.</summary>
        Magmarizer = 323,
        Magnet = 242,
        MagoBerry = 161,
        MagostBerry = 176, // TODO
        /// <summary>No effect.</summary>
        MasterBall = 1,
        /// <summary>No effect.</summary>
        MaxElixir = 41,
        /// <summary>No effect.</summary>
        MaxEther = 39,
        /// <summary>No effect.</summary>
        MaxPotion = 24,
        /// <summary>No effect.</summary>
        MaxRepel = 77,
        /// <summary>No effect.</summary>
        MaxRevive = 29,
        MeadowPlate = 301,
        MentalHerb = 219, // TODO
        MetalCoat = 233,
        MetalPowder = 257,
        Metronome = 277, // TODO
        MicleBerry = 209, // TODO
        MindPlate = 307,
        MiracleSeed = 239,
        /// <summary>No effect.</summary>
        MoomooMilk = 33,
        /// <summary>No effect.</summary>
        MoonBall = 498,
        /// <summary>No effect.</summary>
        MoonStone = 81,
        MuscleBand = 266,
        /// <summary>No effect.</summary>
        MuscleWing = 566,
        MysticWater = 243,
        NanabBerry = 166, // TODO
        /// <summary>No effect.</summary>
        NestBall = 8,
        /// <summary>No effect.</summary>
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
        /// <summary>No effect.</summary>
        OldGateau = 54,
        OranBerry = 155,
        /// <summary>No effect.</summary>
        OvalStone = 110,
        PamtreBerry = 180, // TODO
        /// <summary>No effect.</summary>
        ParalyzHeal = 22,
        /// <summary>No effect.</summary>
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
        /// <summary>No effect.</summary>
        PokeBall = 4,
        /// <summary>No effect.</summary>
        PokeDoll = 63,
        /// <summary>No effect.</summary>
        PokeToy = 577,
        PomegBerry = 169, // TODO
        /// <summary>No effect.</summary>
        Potion = 17,
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
        /// <summary>No effect.</summary>
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
        /// <summary>No effect.</summary>
        QuickBall = 15,
        QuickClaw = 217, // TODO
        QuickPowder = 274,
        RabutaBerry = 177, // TODO
        /// <summary>No effect.</summary>
        RageCandyBar = 504,
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
        /// <summary>No effect.</summary>
        RepeatBall = 9,
        /// <summary>No effect.</summary>
        Repel = 79,
        /// <summary>No effect.</summary>
        ReplyMail = 143,
        /// <summary>No effect.</summary>
        ResistWing = 567,
        /// <summary>No effect.</summary>
        RevivalHerb = 37,
        /// <summary>No effect.</summary>
        Revive = 28,
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
        /// <summary>No effect.</summary>
        SacredAsh = 44,
        /// <summary>No effect.</summary>
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
        /// <summary>No effect.</summary>
        SmokeBall = 228,
        SmoothRock = 283,
        /// <summary>No effect.</summary>
        SodaPop = 31,
        SoftSand = 237,
        /// <summary>No effect.</summary>
        SootheBell = 218,
        SoulDew = 225,
        SpellTag = 247,
        SpelonBerry = 179, // TODO
        SplashPlate = 299,
        SpookyPlate = 310,
        /// <summary>No effect.</summary>
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
        /// <summary>No effect.</summary>
        SuperPotion = 26,
        /// <summary>No effect.</summary>
        SuperRepel = 76,
        /// <summary>No effect.</summary>
        SweetHeart = 134,
        /// <summary>No effect.</summary>
        SwiftWing = 570,
        TamatoBerry = 174, // TODO
        TangaBerry = 194, // TODO
        /// <summary>No effect.</summary>
        ThanksMail = 140,
        ThickClub = 258,
        /// <summary>No effect.</summary>
        Thunderstone = 83,
        /// <summary>No effect.</summary>
        TimerBall = 10,
        /// <summary>No effect.</summary>
        TinyMushroom = 86,
        /// <summary>The Pokémon contracts <see cref="PBEStatus1.BadlyPoisoned"/> at the end of each turn if it has no other <see cref="PBEStatus1"/> and it does not have <see cref="PBEType.Poison"/> or <see cref="PBEType.Steel"/>.</summary>
        ToxicOrb = 272,
        ToxicPlate = 304,
        TwistedSpoon = 248,
        /// <summary>No effect.</summary>
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
        /// <summary>No effect.</summary>
        XAccuracy = 60,
        /// <summary>No effect.</summary>
        XAttack = 57,
        /// <summary>No effect.</summary>
        XDefend = 58,
        /// <summary>No effect.</summary>
        XSpecial = 61,
        /// <summary>No effect.</summary>
        XSpDef = 62,
        /// <summary>No effect.</summary>
        XSpeed = 59,
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
        /// <summary>The Pokémon's ability was suppressed with <see cref="PBEMove.GastroAcid"/>.</summary>
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
        BigPecks = 145, // TODO
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
        FlareBoost = 138, // TODO
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
        HyperCutter = 52, // TODO
        /// <summary>In a hailstorm, the Pokémon takes no damage from the hailstorm and restores HP at the end of each turn.</summary>
        IceBody = 115,
        /// <summary>No effect in battle.</summary>
        Illuminate = 35,
        Illusion = 149,
        Immunity = 17,
        /// <summary>The Pokémon transforms into the foe across from it when switching in.</summary>
        Imposter = 150,
        Infiltrator = 151, // TODO
        InnerFocus = 39,
        Insomnia = 15,
        Intimidate = 22, // TODO (Also, does this activate when given/taken?)
        IronBarbs = 160,
        /// <summary>The power of moves with <see cref="PBEMoveFlag.AffectedByIronFist"/> is increased.</summary>
        IronFist = 89,
        Justified = 154,
        KeenEye = 51, // TODO
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
        PoisonHeal = 90, // TODO
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
        /// <summary>No effect in battle.</summary>
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
        SereneGrace = 32, // TODO
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
        Technician = 101, // TODO
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
    /// <summary>Represents a specific Pokémon species.</summary>
    public enum PBESpecies : uint
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
        Arceus = 493 | (0 << 0x10),
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
        Basculin_Blue = 550 | (0 << 0x10),
        Basculin_Red = 550 | (1 << 0x10),
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
        Burmy_Plant = 412 | (0 << 0x10),
        Burmy_Sandy = 412 | (1 << 0x10),
        Burmy_Trash = 412 | (2 << 0x10),
        Butterfree = 12,
        Cacnea = 331,
        Cacturne = 332,
        Camerupt = 323,
        Carnivine = 455,
        Carracosta = 565,
        Carvanha = 318,
        Cascoon = 268,
        Castform = 351 | (0 << 0x10),
        Castform_Rainy = 351 | (1 << 0x10),
        Castform_Snowy = 351 | (2 << 0x10),
        Castform_Sunny = 351 | (3 << 0x10),
        Caterpie = 10,
        Celebi = 251,
        Chandelure = 609,
        Chansey = 113,
        Charizard = 6,
        Charmander = 4,
        Charmeleon = 5,
        Chatot = 441,
        Cherrim = 421 | (0 << 0x10),
        Cherrim_Sunshine = 421 | (1 << 0x10),
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
        Darmanitan = 555 | (0 << 0x10),
        Darmanitan_Zen = 555 | (1 << 0x10),
        Darumaka = 554,
        Deerling_Autumn = 585 | (0 << 0x10),
        Deerling_Spring = 585 | (1 << 0x10),
        Deerling_Summer = 585 | (2 << 0x10),
        Deerling_Winter = 585 | (3 << 0x10),
        Deino = 633,
        Delcatty = 301,
        Delibird = 225,
        Deoxys = 386 | (0 << 0x10),
        Deoxys_Attack = 386 | (1 << 0x10),
        Deoxys_Defense = 386 | (2 << 0x10),
        Deoxys_Speed = 386 | (3 << 0x10),
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
        Gastrodon_East = 423 | (0 << 0x10),
        Gastrodon_West = 423 | (1 << 0x10),
        Genesect = 649 | (0 << 0x10),
        Genesect_Burn = 649 | (1 << 0x10),
        Genesect_Chill = 649 | (2 << 0x10),
        Genesect_Douse = 649 | (3 << 0x10),
        Genesect_Shock = 649 | (4 << 0x10),
        Gengar = 94,
        Geodude = 74,
        Gible = 443,
        Gigalith = 526,
        Girafarig = 203,
        Giratina = 487 | (0 << 0x10),
        Giratina_Origin = 487 | (1 << 0x10),
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
        Keldeo = 647 | (0 << 0x10),
        Keldeo_Resolute = 647 | (1 << 0x10),
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
        Kyurem = 646 | (0 << 0x10),
        Kyurem_Black = 646 | (1 << 0x10),
        Kyurem_White = 646 | (2 << 0x10),
        Lairon = 305,
        Lampent = 608,
        Landorus = 645 | (0 << 0x10),
        Landorus_Therian = 645 | (1 << 0x10),
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
        Meloetta = 648 | (0 << 0x10),
        Meloetta_Pirouette = 648 | (1 << 0x10),
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
        Rotom = 479 | (0 << 0x10),
        Rotom_Fan = 479 | (1 << 0x10),
        Rotom_Frost = 479 | (2 << 0x10),
        Rotom_Heat = 479 | (3 << 0x10),
        Rotom_Mow = 479 | (4 << 0x10),
        Rotom_Wash = 479 | (5 << 0x10),
        Rufflet = 627,
        Sableye = 302,
        Salamence = 373,
        Samurott = 503,
        Sandile = 551,
        Sandshrew = 27,
        Sandslash = 28,
        Sawk = 539,
        Sawsbuck_Autumn = 586 | (0 << 0x10),
        Sawsbuck_Spring = 586 | (1 << 0x10),
        Sawsbuck_Summer = 586 | (2 << 0x10),
        Sawsbuck_Winter = 586 | (3 << 0x10),
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
        Shaymin = 492 | (0 << 0x10),
        Shaymin_Sky = 492 | (1 << 0x10),
        Shedinja = 292,
        Shelgon = 372,
        Shellder = 90,
        Shellos_East = 422 | (0 << 0x10),
        Shellos_West = 422 | (1 << 0x10),
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
        Thundurus = 642 | (0 << 0x10),
        Thundurus_Therian = 642 | (1 << 0x10),
        Timburr = 532,
        Tirtouga = 564,
        Togekiss = 468,
        Togepi = 175,
        Togetic = 176,
        Torchic = 255,
        Torkoal = 324,
        Tornadus = 641 | (0 << 0x10),
        Tornadus_Therian = 641 | (1 << 0x10),
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
        Unown_A = 201 | (0 << 0x10),
        Unown_B = 201 | (1 << 0x10),
        Unown_C = 201 | (2 << 0x10),
        Unown_D = 201 | (3 << 0x10),
        Unown_E = 201 | (4 << 0x10),
        Unown_Exclamation = 201 | (5 << 0x10),
        Unown_F = 201 | (6 << 0x10),
        Unown_G = 201 | (7 << 0x10),
        Unown_H = 201 | (8 << 0x10),
        Unown_I = 201 | (9 << 0x10),
        Unown_J = 201 | (10 << 0x10),
        Unown_K = 201 | (11 << 0x10),
        Unown_L = 201 | (12 << 0x10),
        Unown_M = 201 | (13 << 0x10),
        Unown_N = 201 | (14 << 0x10),
        Unown_O = 201 | (15 << 0x10),
        Unown_P = 201 | (16 << 0x10),
        Unown_Q = 201 | (17 << 0x10),
        Unown_Question = 201 | (18 << 0x10),
        Unown_R = 201 | (19 << 0x10),
        Unown_S = 201 | (20 << 0x10),
        Unown_T = 201 | (21 << 0x10),
        Unown_U = 201 | (22 << 0x10),
        Unown_V = 201 | (23 << 0x10),
        Unown_W = 201 | (24 << 0x10),
        Unown_X = 201 | (25 << 0x10),
        Unown_Y = 201 | (26 << 0x10),
        Unown_Z = 201 | (27 << 0x10),
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
        Wormadam_Plant = 413 | (0 << 0x10),
        Wormadam_Sandy = 413 | (1 << 0x10),
        Wormadam_Trash = 413 | (2 << 0x10),
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
        Zweilous = 634
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
        SingleNotSelf,         // Single battler except itself (Ex. Dark Pulse)
        SingleFoeSurrounding,  // Single foe surrounding (Ex. Me First)
        SingleSurrounding,     // Single battler surrounding (Ex. Tackle)
        Varies                 // Possible targets vary (Ex. Curse)
    }
    /// <summary>Represents a specific <see cref="PBEMove"/>'s flags.</summary>
    [Flags]
    public enum PBEMoveFlag : ushort
    {
        /// <summary>The move has no flags.</summary>
        None,
        /// <summary>The move's power is boosted by <see cref="PBEAbility.IronFist"/>.</summary>
        AffectedByIronFist = 1 << 0,
        /// <summary>The move is blocked by <see cref="PBEMove.MagicCoat"/> and <see cref="PBEAbility.MagicBounce"/>.</summary>
        AffectedByMagicCoat = 1 << 1,
        /// <summary>The move can be copied by <see cref="PBEMove.MirrorMove"/>.</summary>
        AffectedByMirrorMove = 1 << 2,
        /// <summary>The move is blocked by <see cref="PBEMove.Detect"/>, <see cref="PBEMove.Protect"/>, and <see cref="PBEMove.WideGuard"/>.</summary>
        AffectedByProtect = 1 << 3,
        /// <summary>The move's power is boosted by <see cref="PBEAbility.Reckless"/>.</summary>
        AffectedByReckless = 1 << 4,
        /// <summary>The move can be stolen by <see cref="PBEMove.Snatch"/>.</summary>
        AffectedBySnatch = 1 << 5,
        /// <summary>The move always lands a critical hit.</summary>
        AlwaysCrit = 1 << 6,
        BlockedByMetronome = 1 << 7,
        /// <summary>The move removes <see cref="PBEStatus1.Frozen"/> from the user.</summary>
        DefrostsUser = 1 << 8,
        /// <summary>The move has a higher chance of landing a critical hit.</summary>
        HighCritChance = 1 << 9,
        /// <summary>The move can hit <see cref="PBEStatus2.Airborne"/> targets.</summary>
        HitsAirborne = 1 << 10,
        /// <summary>The move can hit <see cref="PBEStatus2.Underground"/> targets.</summary>
        HitsUnderground = 1 << 11,
        /// <summary>The move can hit <see cref="PBEStatus2.Underwater"/> targets.</summary>
        HitsUnderwater = 1 << 12,
        /// <summary>The user makes contact with the target, causing it to take damage from the target's <see cref="PBEAbility.IronBarbs"/>, <see cref="PBEAbility.RoughSkin"/>, and <see cref="PBEItem.RockyHelmet"/>.</summary>
        MakesContact = 1 << 13,
        /// <summary>The move is blocked by <see cref="PBEAbility.Soundproof"/>.</summary>
        SoundBased = 1 << 14
    }
    public enum PBEMoveEffect : byte
    {
        Attract,
        BellyDrum,
        Bounce,
        BrickBreak,
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
        Confuse,
        Conversion,
        Curse,
        Dig,
        Dive,
        Endeavor,
        Entrainment,
        FinalGambit,
        Flatter,
        Fly,
        FocusEnergy,
        GastroAcid,
        Growth,
        Hail,
        Haze,
        HelpingHand,
        Hit,
        Hit__2Times,
        Hit__2Times__MaybePoison,
        Hit__2To5Times,
        Hit__MaybeBurn,
        Hit__MaybeBurn__10PercentFlinch,
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
        LeechSeed,
        LightScreen,
        LockOn,
        LowerTarget_ATK_DEF_By1,
        LowerTarget_DEF_SPDEF_By1_Raise_ATK_SPATK_SPE_By2,
        LuckyChant,
        MagnetRise,
        Metronome,
        MiracleEye,
        Moonlight,
        Nothing,
        OneHitKnockout,
        PainSplit,
        Paralyze,
        Poison,
        PowerTrick,
        Protect, // TODO: If the user goes last, fail
        PsychUp,
        Psywave,
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
        Rest,
        RestoreTargetHP,
        RolePlay,
        Safeguard,
        Sandstorm,
        SecretPower,
        SeismicToss,
        Selfdestruct,
        SetDamage,
        SimpleBeam,
        Sleep,
        Snore,
        Soak,
        Spikes,
        StealthRock,
        Struggle,
        Substitute,
        SuckerPunch,
        SunnyDay,
        SuperFang,
        Swagger,
        Tailwind,
        Teleport,
        Toxic,
        ToxicSpikes,
        Transform,
        TrickRoom,
        Whirlwind,
        WideGuard,
        WorrySeed
    }
    public enum PBEMove : ushort
    {
        None = 0,
        Absorb = 71,
        Acid = 51,
        AcidArmor = 151,
        AcidSpray = 491,
        Acrobatics = 512,
        //Acupressure = 367,
        AerialAce = 332,
        Aeroblast = 177,
        //AfterYou = 495, // TODO: Blocked by Metronome
        Agility = 97,
        AirCutter = 314,
        AirSlash = 403,
        //AllySwitch = 502,
        Amnesia = 133,
        AncientPower = 246,
        AquaJet = 453,
        //AquaRing = 392,
        AquaTail = 401,
        ArmThrust = 292,
        //Aromatherapy = 312,
        //Assist = 274, // TODO: Blocked by Metronome
        //Assurance = 372,
        Astonish = 310,
        AttackOrder = 454,
        Attract = 213,
        AuraSphere = 396,
        AuroraBeam = 62,
        //Autotomize = 475,
        //Avalanche = 419,
        Barrage = 140,
        Barrier = 112,
        //BatonPass = 226,
        //BeatUp = 251,
        BellyDrum = 187,
        //Bestow = 516, // TODO: Blocked by Metronome
        //Bide = 117,
        //Bind = 20,
        Bite = 44,
        //BlastBurn = 307,
        BlazeKick = 299,
        Blizzard = 59,
        //Block = 335,
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
        //BugBite = 450,
        BugBuzz = 405,
        BulkUp = 339,
        Bulldoze = 523,
        BulletPunch = 418,
        BulletSeed = 331,
        CalmMind = 347,
        Camouflage = 293,
        Captivate = 445,
        //Charge = 268,
        ChargeBeam = 451,
        Charm = 204,
        Chatter = 448,
        //ChipAway = 498,
        //CircleThrow = 509,
        //Clamp = 128,
        //ClearSmog = 499,
        CloseCombat = 370,
        Coil = 489,
        CometPunch = 4,
        ConfuseRay = 109,
        Confusion = 93,
        Constrict = 132,
        Conversion = 160,
        //Conversion2 = 176,
        //Copycat = 383, // TODO: Blocked by Metronome
        CosmicPower = 322,
        CottonGuard = 538,
        CottonSpore = 178,
        //Counter = 68, // TODO: Blocked by Metronome
        //Covet = 343, // TODO: Blocked by Metronome
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
        //Defog = 432,
        //DestinyBond = 194, // TODO: Blocked by Metronome
        Detect = 197,
        Dig = 91,
        //Disable = 50,
        Discharge = 435,
        Dive = 291,
        DizzyPunch = 146,
        //DoomDesire = 353,
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
        //DragonTail = 525,
        DrainPunch = 409,
        DreamEater = 138,
        DrillPeck = 65,
        DrillRun = 529,
        DualChop = 530,
        DynamicPunch = 223,
        EarthPower = 414,
        Earthquake = 89,
        //EchoedVoice = 497,
        EggBomb = 121,
        //ElectroBall = 486,
        Electroweb = 527,
        //Embargo = 373,
        Ember = 52,
        //Encore = 227,
        Endeavor = 283,
        //Endure = 203, // TODO: Blocked by Metronome
        EnergyBall = 412,
        Entrainment = 494, // TODO: Does abilities activate? (Slow Start, Intimidate, etc)
        Eruption = 284,
        Explosion = 153,
        Extrasensory = 326,
        ExtremeSpeed = 245,
        Facade = 263,
        FaintAttack = 185,
        //FakeOut = 252,
        FakeTears = 313,
        //FalseSwipe = 206,
        FeatherDance = 297,
        //Feint = 364, // TODO: Blocked by Metronome
        FieryDance = 552,
        FinalGambit = 515,
        FireBlast = 126,
        FireFang = 424,
        //FirePledge = 519,
        FirePunch = 7,
        //FireSpin = 83,
        Fissure = 90,
        Flail = 175,
        //FlameBurst = 481,
        FlameCharge = 488,
        Flamethrower = 53,
        FlameWheel = 172,
        FlareBlitz = 394,
        Flash = 148,
        FlashCannon = 430,
        Flatter = 260,
        //Fling = 374,
        Fly = 19,
        FocusBlast = 411,
        FocusEnergy = 116,
        //FocusPunch = 264, // TODO: Iron Fist, Blocked by Metronome
        //FollowMe = 266, // TODO: Blocked by Metronome
        ForcePalm = 395,
        //Foresight = 193,
        FoulPlay = 492,
        //FreezeShock = 553, // TODO: Blocked by Metronome
        //FrenzyPlant = 338,
        FrostBreath = 524,
        Frustration = 218,
        FuryAttack = 31,
        //FuryCutter = 210,
        FurySwipes = 154,
        //FusionBolt = 559,
        //FusionFlare = 558,
        //FutureSight = 248,
        GastroAcid = 380, // TODO: Magic Bounce, Magic Coat
        GearGrind = 544,
        GigaDrain = 202,
        //GigaImpact = 416,
        Glaciate = 549,
        Glare = 137,
        GrassKnot = 447,
        //GrassPledge = 520,
        GrassWhistle = 320,
        //Gravity = 356,
        Growl = 45,
        Growth = 74,
        //Grudge = 288,
        //GuardSplit = 470,
        //GuardSwap = 385,
        Guillotine = 12,
        GunkShot = 441,
        Gust = 16,
        //GyroBall = 360,
        Hail = 258,
        HammerArm = 359,
        Harden = 106,
        Haze = 114,
        Headbutt = 29,
        HeadCharge = 543,
        HeadSmash = 457,
        //HealBell = 215,
        //HealBlock = 377,
        //HealingWish = 361,
        HealOrder = 456,
        HealPulse = 505,
        HeartStamp = 531,
        //HeartSwap = 391,
        HeatCrash = 535,
        HeatWave = 257,
        HeavySlam = 484,
        HelpingHand = 270,
        Hex = 506,
        HiddenPower = 237,
        //HiJumpKick = 136, // TODO: Reckless
        HoneClaws = 468,
        HornAttack = 30,
        HornDrill = 32,
        HornLeech = 532,
        Howl = 336,
        Hurricane = 542,
        //HydroCannon = 308,
        HydroPump = 56,
        //HyperBeam = 63,
        HyperFang = 158,
        HyperVoice = 304,
        Hypnosis = 95,
        //IceBall = 301,
        IceBeam = 58,
        //IceBurn = 554, // TODO: Blocked by Metronome
        IceFang = 423,
        IcePunch = 8,
        IceShard = 420,
        IcicleCrash = 556,
        IcicleSpear = 333,
        IcyWind = 196,
        //Imprison = 286,
        //Incinerate = 510,
        Inferno = 517,
        //Ingrain = 275,
        IronDefense = 334,
        IronHead = 442,
        IronTail = 231,
        Judgment = 449,
        //JumpKick = 26, // TODO: Reckless
        KarateChop = 2,
        Kinesis = 134,
        //KnockOff = 282,
        //LastResort = 387,
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
        //LunarDance = 461,
        LusterPurge = 295,
        MachPunch = 183,
        MagicalLeaf = 345,
        //MagicCoat = 277,
        //MagicRoom = 478,
        //MagmaStorm = 463,
        MagnetBomb = 443,
        MagnetRise = 393,
        Magnitude = 222,
        //MeanLook = 212,
        Meditate = 96,
        //MeFirst = 382, // TODO: Blocked by Metronome, Sucker Punch
        MegaDrain = 72,
        Megahorn = 224,
        MegaKick = 25,
        MegaPunch = 5,
        //Memento = 262,
        //MetalBurst = 368,
        MetalClaw = 232,
        MetalSound = 319,
        MeteorMash = 309,
        Metronome = 118,
        MilkDrink = 208,
        //Mimic = 102, // TODO: Blocked by Metronome
        MindReader = 170,
        Minimize = 107,
        MiracleEye = 357,
        //MirrorCoat = 243, // TODO: Blocked by Metronome
        //MirrorMove = 119, // TODO: Blocked by Metronome
        MirrorShot = 429,
        //Mist = 54,
        MistBall = 296,
        Moonlight = 236,
        MorningSun = 234,
        MudBomb = 426,
        MuddyWater = 330,
        MudShot = 341,
        MudSlap = 189,
        //MudSport = 300,
        NastyPlot = 417,
        //NaturalGift = 363,
        //NaturePower = 267, // TODO: Blocked by Metronome
        NeedleArm = 302,
        NightDaze = 539,
        //Nightmare = 171,
        NightShade = 101,
        NightSlash = 400,
        Octazooka = 190,
        //OdorSleuth = 316,
        OminousWind = 466,
        //Outrage = 200,
        Overheat = 315,
        PainSplit = 220,
        //Payback = 371,
        //PayDay = 6,
        Peck = 64,
        //PerishSong = 195,
        //PetalDance = 80,
        PinMissile = 42,
        //Pluck = 365,
        PoisonFang = 305,
        PoisonGas = 139,
        PoisonJab = 398,
        PoisonPowder = 77,
        PoisonSting = 40,
        PoisonTail = 342,
        Pound = 1,
        PowderSnow = 181,
        PowerGem = 408,
        //PowerSplit = 471,
        //PowerSwap = 384,
        PowerTrick = 379,
        PowerWhip = 438,
        //Present = 217,
        Protect = 182,
        Psybeam = 60,
        Psychic = 94,
        PsychoBoost = 354,
        PsychoCut = 427,
        //PsychoShift = 375,
        PsychUp = 244,
        Psyshock = 473,
        Psystrike = 540,
        Psywave = 149,
        Punishment = 386,
        //Pursuit = 228,
        //Quash = 511, // TODO: Blocked by Metronome
        QuickAttack = 98,
        //QuickGuard = 501, // TODO: Blocked by Metronome
        QuiverDance = 483,
        //Rage = 99,
        //RagePowder = 476, // TODO: Blocked by Metronome
        RainDance = 240,
        //RapidSpin = 229,
        RazorLeaf = 75,
        RazorShell = 534,
        //RazorWind = 13,
        Recover = 105,
        //Recycle = 278,
        Reflect = 115,
        //ReflectType = 513,
        //Refresh = 287,
        //RelicSong = 547, // TODO: Blocked by Metronome
        Rest = 156, // TODO: Uproar, Leaf Guard
        Retaliate = 514,
        Return = 216,
        //Revenge = 279,
        Reversal = 179,
        Roar = 46, // TODO: Suction Cups, Soundproof, Ingrain
        //RoarOfTime = 459,
        RockBlast = 350,
        RockClimb = 431,
        RockPolish = 397,
        RockSlide = 157,
        RockSmash = 249,
        RockThrow = 88,
        RockTomb = 317,
        //RockWrecker = 439,
        RolePlay = 272,
        RollingKick = 27,
        //Rollout = 205,
        //Roost = 355,
        //Round = 496,
        SacredFire = 221,
        //SacredSword = 533,
        Safeguard = 219,
        SandAttack = 28,
        Sandstorm = 201,
        //SandTomb = 328,
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
        //ShadowForce = 467,
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
        //Sketch = 166, // TODO: Blocked by Metronome
        //SkillSwap = 285,
        //SkullBash = 130,
        //SkyAttack = 143,
        //SkyDrop = 507,
        SkyUppercut = 327,
        SlackOff = 303,
        Slam = 21,
        Slash = 163,
        SleepPowder = 79,
        //SleepTalk = 214, // TODO: Blocked by Metronome
        Sludge = 124,
        SludgeBomb = 188,
        SludgeWave = 482,
        //SmackDown = 479,
        //SmellingSalt = 265,
        Smog = 123,
        SmokeScreen = 108,
        Snarl = 555,
        //Snatch = 289, // TODO: Blocked by Metronome
        Snore = 173,
        Soak = 487,
        Softboiled = 135,
        //SolarBeam = 49,
        SonicBoom = 49,
        SpacialRend = 460,
        Spark = 209,
        //SpiderWeb = 169,
        SpikeCannon = 131,
        Spikes = 191,
        //Spite = 180,
        //SpitUp = 255,
        Splash = 150,
        Spore = 147,
        StealthRock = 446,
        Steamroller = 537,
        SteelWing = 211,
        //Stockpile = 254,
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
        //Swallow = 256,
        SweetKiss = 186,
        SweetScent = 230,
        Swift = 129,
        //Switcheroo = 415, // TODO: Blocked by Metronome
        SwordsDance = 14,
        //Synchronoise = 485,
        Synthesis = 235,
        Tackle = 33,
        TailGlow = 294,
        TailSlap = 541,
        TailWhip = 39,
        Tailwind = 366,
        TakeDown = 36,
        //Taunt = 269,
        TechnoBlast = 546,
        TeeterDance = 298,
        //Telekinesis = 477,
        Teleport = 100,
        //Thief = 168, // TODO: Blocked by Metronome
        //Thrash = 37,
        Thunder = 87,
        Thunderbolt = 85,
        ThunderFang = 422,
        ThunderPunch = 9,
        ThunderShock = 84,
        ThunderWave = 86,
        Tickle = 321,
        //Torment = 259,
        Toxic = 92,
        ToxicSpikes = 390,
        Transform = 144,
        //TriAttack = 161,
        //Trick = 271, // TODO: Blocked by Metronome
        TrickRoom = 433,
        //TripleKick = 167,
        //TrumpCard = 376,
        Twineedle = 41,
        Twister = 239,
        //Uproar = 253,
        //Uturn = 369,
        VacuumWave = 410,
        VCreate = 557,
        Venoshock = 474,
        ViceGrip = 11,
        VineWhip = 22,
        VitalThrow = 233,
        //VoltSwitch = 521,
        VoltTackle = 344,
        //WakeUpSlap = 358,
        Waterfall = 127,
        WaterGun = 55,
        //WaterPledge = 518,
        WaterPulse = 352,
        //WaterSport = 346,
        WaterSpout = 323,
        WeatherBall = 311,
        //Whirlpool = 250,
        Whirlwind = 18, // TODO: Suction Cups, Ingrain
        WideGuard = 469,
        WildCharge = 528,
        WillOWisp = 261,
        WingAttack = 17,
        //Wish = 273,
        Withdraw = 110,
        //WonderRoom = 472,
        WoodHammer = 452,
        WorkUp = 526,
        WorrySeed = 388,
        //Wrap = 35,
        WringOut = 378,
        XScissor = 404,
        //Yawn = 281,
        ZapCannon = 192,
        ZenHeadbutt = 428,
        /// <summary>Invalid move.</summary>
        MAX = 560
    }
}
