using Kermalis.PokemonBattleEngine.Battle;
using System;

namespace Kermalis.PokemonBattleEngine.Data
{
    /// <summary>
    /// Represents a specific Pokémon's gender.
    /// </summary>
    public enum PBEGender : byte
    {
        /// <summary>
        /// The Pokémon is female.
        /// </summary>
        Female,
        /// <summary>
        /// The Pokémon is genderless.
        /// </summary>
        Genderless,
        /// <summary>
        /// The Pokémon is male.
        /// </summary>
        Male,
        /// <summary>
        /// Invalid gender.
        /// </summary>
        MAX
    }
    /// <summary>
    /// Represents a Pokémon species' <see cref="PBEGender"/> ratio.
    /// </summary>
    public enum PBEGenderRatio : byte
    {
        /// <summary>
        /// The species is 87.5% male, 12.5% female.
        /// </summary>
        M7_F1,
        /// <summary>
        /// The species is 75% male, 25% female.
        /// </summary>
        M3_F1,
        /// <summary>
        /// The species is 50% male, 50% female.
        /// </summary>
        M1_F1,
        /// <summary>
        /// The species is 25% male, 75% female.
        /// </summary>
        M1_F3,
        /// <summary>
        /// The species is 12.5% male, 87.5% female.
        /// </summary>
        /// <remarks>
        /// No species introduced before generation 6 has this gender ratio.
        /// </remarks>
        M1_F7,
        /// <summary>
        /// The species is 0% male, 100% female.
        /// </summary>
        M0_F1,
        /// <summary>
        /// The species is genderless.
        /// </summary>
        M0_F0,
        /// <summary>
        /// The species is 100% male, 0% female.
        /// </summary>
        M1_F0
    }
    /// <summary>
    /// Represents a Pokémon stat.
    /// </summary>
    public enum PBEStat : byte
    {
        /// <summary>
        /// Hit points.
        /// </summary>
        HP,
        /// <summary>
        /// Attack.
        /// </summary>
        Attack,
        /// <summary>
        /// Defense.
        /// </summary>
        Defense,
        /// <summary>
        /// Special Attack.
        /// </summary>
        SpAttack,
        /// <summary>
        /// Special Defense.
        /// </summary>
        SpDefense,
        /// <summary>
        /// Speed.
        /// </summary>
        Speed,
        /// <summary>
        /// Accuracy.
        /// </summary>
        Accuracy,
        /// <summary>
        /// Evasion.
        /// </summary>
        Evasion
    }
    /// <summary>
    /// Represents the effectiveness of a move against a target.
    /// </summary>
    public enum PBEEffectiveness : byte
    {
        /// <summary>
        /// The move does not affect the target.
        /// </summary>
        Ineffective,
        /// <summary>
        /// The move does less damage to the target.
        /// </summary>
        NotVeryEffective,
        /// <summary>
        /// The move affects the target as usual.
        /// </summary>
        Normal,
        /// <summary>
        /// The move does more damage to the target.
        /// </summary>
        SuperEffective
    }
    /// <summary>
    /// Represents the format of a specific battle.
    /// </summary>
    public enum PBEBattleFormat : byte
    {
        /// <summary>
        /// A 1v1 battle. Each Pokémon is able to use moves or switch out for another Pokémon.
        /// </summary>
        Single,
        /// <summary>
        /// A 2v2 battle where all Pokémon are able to use moves or switch out for another Pokémon.
        /// </summary>
        Double,
        /// <summary>
        /// A 3v3 battle where all Pokémon are able to use moves, shift positions with a teammate, or switch out for another Pokémon.
        /// </summary>
        Triple,
        /// <summary>
        /// A 3v3 battle where only the front Pokémon are able to force a team rotation, use a move, or switch out for another Pokémon.
        /// </summary>
        /// <remarks>
        /// Team rotation does not take up a turn and can be done once per turn.
        /// </remarks>
        Rotation
    }
    /// <summary>
    /// Represents the current state of a specific battle.
    /// </summary>
    public enum PBEBattleState : byte
    {
        /// <summary>
        /// The battle is waiting for team shells.
        /// </summary>
        WaitingForPlayers,
        /// <summary>
        /// The battle is ready to begin.
        /// </summary>
        ReadyToBegin,
        /// <summary>
        /// The battle is waiting for players to send actions.
        /// </summary>
        WaitingForActions,
        /// <summary>
        /// The battle is ready to run a turn.
        /// </summary>
        ReadyToRunTurn,
        /// <summary>
        /// The battle is processing.
        /// </summary>
        Processing,
        /// <summary>
        /// The battle is waiting for players to send switch-ins.
        /// </summary>
        WaitingForSwitchIns,
        /// <summary>
        /// The battle ended.
        /// </summary>
        Ended
    }
    /// <summary>
    /// Represents the weather in a specific battle.
    /// </summary>
    public enum PBEWeather : byte
    {
        /// <summary>
        /// There is no weather.
        /// </summary>
        None,
        /// <summary>
        /// It is hailing.
        /// </summary>
        Hailstorm,
        /// <summary>
        /// The sunlight is harsh.
        /// </summary>
        HarshSunlight,
        /// <summary>
        /// It is raining.
        /// </summary>
        Rain,
        /// <summary>
        /// A sandstorm is brewing.
        /// </summary>
        Sandstorm
    }
    /// <summary>
    /// Represents a position on the battle field.
    /// </summary>
    public enum PBEFieldPosition : byte
    {
        /// <summary>
        /// A Pokémon is not on the field.
        /// </summary>
        None,
        /// <summary>
        /// The Pokémon to a player's left in a Double, Triple, or Rotation battle.
        /// </summary>
        Left,
        /// <summary>
        /// The Pokémon in the center of the field in a Single, Triple, or Rotation battle.
        /// </summary>
        Center,
        /// <summary>
        /// The Pokémon to a player's right in a Double, Triple, or Rotation battle.
        /// </summary>
        Right
    }
    /// <summary>
    /// Represents a <see cref="PBEMove"/>'s targets.
    /// </summary>
    [Flags]
    public enum PBETarget : byte
    {
        /// <summary>
        /// The Pokémon has not chosen any targets.
        /// </summary>
        None,
        /// <summary>
        /// The move is targetting the player's left Pokémon.
        /// </summary>
        AllyLeft = 1 << 0,
        /// <summary>
        /// The move is targetting the player's center Pokémon.
        /// </summary>
        AllyCenter = 1 << 1,
        /// <summary>
        /// The move is targetting the player's right Pokémon.
        /// </summary>
        AllyRight = 1 << 2,
        /// <summary>
        /// The move is targetting the opponent's left Pokémon.
        /// </summary>
        FoeLeft = 1 << 3,
        /// <summary>
        /// The move is targetting the opponent's center Pokémon.
        /// </summary>
        FoeCenter = 1 << 4,
        /// <summary>
        /// The move is targetting the opponent's right Pokémon.
        /// </summary>
        FoeRight = 1 << 5
    }
    /// <summary>
    /// Represents a Pokémon's decision for a turn.
    /// </summary>
    public enum PBEDecision : byte
    {
        /// <summary>
        /// The Pokémon has not made a decision.
        /// </summary>
        None,
        /// <summary>
        /// The Pokémon has chosen to use a move.
        /// </summary>
        Fight,
        /// <summary>
        /// The Pokémon has chosen to switch out for another Pokémon.
        /// </summary>
        SwitchOut
    }
    /// <summary>
    /// Represents a specific <see cref="PBEMove"/>'s category.
    /// </summary>
    public enum PBEMoveCategory : byte
    {
        /// <summary>
        /// The move deals no damage.
        /// </summary>
        Status,
        /// <summary>
        /// The move deals physical damage using the Attack and Defense stats.
        /// </summary>
        Physical,
        /// <summary>
        /// The move deals special damage using the Special Attack and Special Defense stats.
        /// </summary>
        Special,
        /// <summary>
        /// Invalid category.
        /// </summary>
        MAX
    }
    /// <summary>
    /// Represents the various methods in which a Pokémon can learn a <see cref="PBEMove"/>.
    /// </summary>
    [Flags]
    public enum PBEMoveObtainMethod : ulong
    {
        /// <summary>
        /// There is no way to learn this move.
        /// </summary>
        None,
        /// <summary>
        /// The move can be learned by levelling up a Pokémon in Pokémon Ruby Version and Pokémon Sapphire Version.
        /// </summary>
        LevelUp_RS = 1u << 0,
        /// <summary>
        /// The move can be learned by levelling up a Pokémon in Pokémon Fire Red Version.
        /// </summary>
        LevelUp_FR = 1u << 1,
        /// <summary>
        /// The move can be learned by levelling up a Pokémon in Pokémon Leaf Green Version.
        /// </summary>
        LevelUp_LG = 1u << 2,
        /// <summary>
        /// The move can be learned by levelling up a Pokémon in Pokémon Emerald Version.
        /// </summary>
        LevelUp_E = 1u << 3,
        /// <summary>
        /// The move can be learned by levelling up a Pokémon in Pokémon Colosseum.
        /// </summary>
        LevelUp_Colo = 1u << 4,
        /// <summary>
        /// The move can be learned by levelling up a Pokémon in Pokémon XD: Gale of Darkness.
        /// </summary>
        LevelUp_XD = 1u << 5,
        /// <summary>
        /// The move can be learned by levelling up a Pokémon in Pokémon Diamond Version and Pokémon Pearl Version.
        /// </summary>
        LevelUp_DP = 1u << 6,
        /// <summary>
        /// The move can be learned by levelling up a Pokémon in Pokémon Platinum Version.
        /// </summary>
        LevelUp_Pt = 1u << 7,
        /// <summary>
        /// The move can be learned by levelling up a Pokémon in Pokémon HeartGold Version and Pokémon SoulSilver Version.
        /// </summary>
        LevelUp_HGSS = 1u << 8,
        /// <summary>
        /// The move can be learned by levelling up a Pokémon in Pokémon Black Version and Pokémon White Version.
        /// </summary>
        LevelUp_BW = 1u << 9,
        /// <summary>
        /// The move can be learned by levelling up a Pokémon in Pokémon Black Version 2 and Pokémon White Version 2.
        /// </summary>
        LevelUp_B2W2 = 1u << 10,
        /// <summary>
        /// The move can be learned by using a technical machine on a Pokémon in Pokémon Ruby Version, Pokémon Sapphire Version, Pokémon Fire Red Version, Pokémon Leaf Green Version and Pokémon Emerald Version.
        /// </summary>
        TM_RSFRLGE = 1u << 11,
        /// <summary>
        /// The move can be learned by using a technical machine on a Pokémon in Pokémon Diamond Version, Pokémon Pearl Version, Pokémon Platinum Version, Pokémon HeartGold Version and Pokémon SoulSilver Version.
        /// </summary>
        TM_DPPtHGSS = 1u << 12,
        /// <summary>
        /// The move can be learned by using a technical machine on a Pokémon in Pokémon Black Version, Pokémon White Version, Pokémon Black Version 2 and Pokémon White Version 2.
        /// </summary>
        TM_BWB2W2 = 1u << 13,
        /// <summary>
        /// The move can be learned by using a hidden machine on a Pokémon in Pokémon Ruby Version, Pokémon Sapphire Version, Pokémon Fire Red Version, Pokémon Leaf Green Version and Pokémon Emerald Version.
        /// </summary>
        HM_RSFRLGE = 1u << 14,
        /// <summary>
        /// The move can be learned by using a hidden machine on a Pokémon in Pokémon Diamond Version, Pokémon Pearl Version and Pokémon Platinum Version.
        /// </summary>
        HM_DPPt = 1u << 15,
        /// <summary>
        /// The move can be learned by using a hidden machine on a Pokémon in Pokémon HeartGold Version and Pokémon SoulSilver Version.
        /// </summary>
        HM_HGSS = 1u << 16,
        /// <summary>
        /// The move can be learned by using a hidden machine on a Pokémon in Pokémon Black Version, Pokémon White Version, Pokémon Black Version 2 and Pokémon White Version 2.
        /// </summary>
        HM_BWB2W2 = 1u << 17,
        /// <summary>
        /// The move can be taught to a Pokémon by a move tutor in Pokémon Fire Red Version and Pokémon Leaf Green Version.
        /// </summary>
        MoveTutor_FRLG = 1u << 18,
        /// <summary>
        /// The move can be taught to a Pokémon by a move tutor in Pokémon Emerald Version.
        /// </summary>
        MoveTutor_E = 1u << 19,
        /// <summary>
        /// The move can be taught to a Pokémon by a move tutor in Pokémon XD: Gale of Darkness.
        /// </summary>
        MoveTutor_XD = 1u << 20,
        /// <summary>
        /// The move can be taught to a Pokémon by a move tutor in Pokémon Diamond Version and Pokémon Pearl Version.
        /// </summary>
        MoveTutor_DP = 1u << 21,
        /// <summary>
        /// The move can be taught to a Pokémon by a move tutor in Pokémon Platinum Version.
        /// </summary>
        MoveTutor_Pt = 1u << 22,
        /// <summary>
        /// The move can be taught to a Pokémon by a move tutor in Pokémon HeartGold Version and Pokémon SoulSilver Version.
        /// </summary>
        MoveTutor_HGSS = 1u << 23,
        /// <summary>
        /// The move can be taught to a Pokémon by a move tutor in Pokémon Black Version and Pokémon White Version.
        /// </summary>
        MoveTutor_BW = 1u << 24,
        /// <summary>
        /// The move can be taught to a Pokémon by a move tutor in Pokémon Black Version 2 and Pokémon White Version 2.
        /// </summary>
        MoveTutor_B2W2 = 1u << 25,
        /// <summary>
        /// The move can be learned by hatching a Pokémon egg in Pokémon Ruby Version, Pokémon Sapphire Version, Pokémon Fire Red Version and Pokémon Leaf Green Version.
        /// </summary>
        EggMove_RSFRLG = 1u << 26,
        /// <summary>
        /// The move can be learned by hatching a Pokémon egg in Pokémon Emerald Version.
        /// </summary>
        EggMove_E = 1u << 27,
        /// <summary>
        /// The move can be learned by hatching a Pokémon egg in Pokémon Diamond Version, Pokémon Pearl Version and Pokémon Platinum Version.
        /// </summary>
        EggMove_DPPt = 1u << 28,
        /// <summary>
        /// The move can be learned by hatching a Pokémon egg in Pokémon HeartGold Version and Pokémon SoulSilver Version.
        /// </summary>
        EggMove_HGSS = 1u << 29,
        /// <summary>
        /// The move can be learned by hatching a Pokémon egg in Pokémon Black Version, Pokémon White Version, Pokémon Black Version 2 and Pokémon White Version 2.
        /// </summary>
        EggMove_BWB2W2 = 1u << 30,
        /// <summary>
        /// The move is known by a Pokémon when found in the Dream World.
        /// </summary>
        DreamWorld = 1u << 31,
        /// <summary>
        /// The move is learned by a Pokémon when changing formes.
        /// </summary>
        Forme = 1u << 32
    }
    /// <summary>
    /// Represents a specific Pokémon's non-volatile status.
    /// </summary>
    public enum PBEStatus1 : byte
    {
        /// <summary>
        /// The Pokémon has no status.
        /// </summary>
        None,
        /// <summary>
        /// The Pokémon is asleep.
        /// </summary>
        Asleep,
        /// <summary>
        /// The Pokémon is badly poisoned.
        /// </summary>
        BadlyPoisoned,
        /// <summary>
        /// The Pokémon is burned.
        /// </summary>
        Burned,
        /// <summary>
        /// The Pokémon is frozen.
        /// </summary>
        Frozen,
        /// <summary>
        /// The Pokémon is paralyzed.
        /// </summary>
        Paralyzed,
        /// <summary>
        /// The Pokémon is poisoned.
        /// </summary>
        Poisoned
    }
    /// <summary>
    /// Represents a specific Pokémon's volatile status.
    /// </summary>
    [Flags]
    public enum PBEStatus2 : uint
    {
        /// <summary>
        /// The Pokémon has no status.
        /// </summary>
        None,
        /// <summary>
        /// The Pokémon is high up in the air.
        /// A move will miss against the Pokémon unless it has <see cref="PBEMoveFlag.HitsAirborne"/> or either Pokémon has <see cref="PBEAbility.NoGuard"/>.
        /// </summary>
        Airborne = 1 << 0,
        /// <summary>
        /// The Pokémon is confused and may hurt itself instead of execute its chosen move.
        /// </summary>
        Confused = 1 << 1,
        /// <summary>
        /// The Pokémon is cursed and will take damage at the end of each turn.
        /// </summary>
        Cursed = 1 << 2,
        /// <summary>
        /// The Pokémon is flinching and will be unable to move this turn.
        /// </summary>
        Flinching = 1 << 3,
        /// <summary>
        /// The Pokémon is infatuated with another Pokémon and may be unable to a move this turn.
        /// </summary>
        Infatuated = 1 << 4, // TODO
        /// <summary>
        /// The Pokémon is seeded and HP will be stolen at the end of each turn.
        /// </summary>
        LeechSeed = 1 << 5,
        /// <summary>
        /// The Pokémon is minimized and will take double damage from <see cref="PBEMove.Steamroller"/> and <see cref="PBEMove.Stomp"/>.
        /// </summary>
        Minimized = 1 << 6,
        /// <summary>
        /// The Pokémon is protected from moves this turn.
        /// </summary>
        Protected = 1 << 7,
        /// <summary>
        /// The Pokémon is under the effect of <see cref="PBEMove.FocusEnergy"/> or <see cref="PBEItem.LansatBerry"/> and has a higher chance of landing critical hits.
        /// </summary>
        Pumped = 1 << 8,
        /// <summary>
        /// The Pokémon is behind a substitute that will take damage on behalf of the Pokémon and prevent most moves from affecting the Pokémon.
        /// </summary>
        Substitute = 1 << 9,
        /// <summary>
        /// The Pokémon is unable to use the same move two times in a row.
        /// </summary>
        Tormented = 1 << 10, // TODO
        /// <summary>
        /// The Pokémon is transformed into another Pokémon.
        /// </summary>
        Transformed = 1 << 11, // TODO: Fail if target is disguised with illusion
        /// <summary>
        /// The Pokémon is underground.
        /// A move will miss against the Pokémon unless it has <see cref="PBEMoveFlag.HitsUnderground"/> or either Pokémon has <see cref="PBEAbility.NoGuard"/>.
        /// The Pokémon will take double damage from <see cref="PBEMove.Earthquake"/> and <see cref="PBEMove.Magnitude"/>.
        /// </summary>
        Underground = 1 << 12,
        /// <summary>
        /// The Pokémon is underwater.
        /// A move will miss against the Pokémon unless it has <see cref="PBEMoveFlag.HitsUnderwater"/> or either Pokémon has <see cref="PBEAbility.NoGuard"/>.
        /// The Pokémon will take double damage from <see cref="PBEMove.Surf"/> and <see cref="PBEMove.Whirlpool"/>.
        /// </summary>
        Underwater = 1 << 13
    }
    /// <summary>
    /// Represents a specific <see cref="PBETeam"/>'s status.
    /// </summary>
    [Flags]
    public enum PBETeamStatus : byte
    {
        /// <summary>
        /// The team has no status.
        /// </summary>
        None,
        /// <summary>
        /// The team will take less damage from <see cref="PBEMoveCategory.Special"/> moves.
        /// </summary>
        LightScreen = 1 << 0,
        /// <summary>
        /// The team is shielded from critical hits.
        /// </summary>
        LuckyChant = 1 << 1,
        /// <summary>
        /// The team will take less damage from <see cref="PBEMoveCategory.Physical"/> moves.
        /// </summary>
        Reflect = 1 << 2,
        /// <summary>
        /// Grounded Pokémon that switch in will take damage.
        /// The amount of damage is based on <see cref="PBETeam.SpikeCount"/>.
        /// </summary>
        Spikes = 1 << 3, // TODO: Gravity, magnet rise, magic guard, iron ball, baton pass with ingrain, air balloon
        /// <summary>
        /// Pokémon that switch in will take damage.
        /// The amount of damage is based on the effectiveness of <see cref="PBEType.Rock"/> on the Pokémon.
        /// </summary>
        StealthRock = 1 << 4, // TODO: magic guard, castform transforms after taking damage
        /// <summary>
        /// Grounded Pokémon that switch in will be <see cref="PBEStatus1.Poisoned"/> if <see cref="PBETeam.ToxicSpikeCount"/> is 1 or <see cref="PBEStatus1.BadlyPoisoned"/> if it is 2.
        /// Grounded <see cref="PBEType.Poison"/> Pokémon will remove toxic spikes.
        /// </summary>
        ToxicSpikes = 1 << 5 // TODO: Gravity, immunity, leaf guard, magic guard, iron ball, baton pass with ingrain, air balloon, synchronize with roar/whirlwind
    }
    /// <summary>
    /// Represents an action regarding a <see cref="PBEAbility"/>.
    /// </summary>
    public enum PBEAbilityAction : byte
    {
        /// <summary>
        /// The ability caused a Pokémon to change its appearance.
        /// </summary>
        ChangedAppearance,
        /// <summary>
        /// The ability cured a Pokémon from a <see cref="PBEStatus1"/> or <see cref="PBEStatus2"/>.
        /// </summary>
        CuredStatus,
        /// <summary>
        /// The ability was involved with damage.
        /// </summary>
        Damage,
        /// <summary>
        /// The ability prevented a Pokémon from being inflicted with a <see cref="PBEStatus1"/> or <see cref="PBEStatus2"/>.
        /// </summary>
        PreventedStatus,
        /// <summary>
        /// The ability restored a Pokémon's HP.
        /// </summary>
        RestoredHP,
        /// <summary>
        /// The ability was involved with weather.
        /// </summary>
        Weather
    }
    /// <summary>
    /// Represents an action regarding a <see cref="PBEItem"/>.
    /// </summary>
    public enum PBEItemAction : byte
    {
        /// <summary>
        /// The item caused a Pokémon to take damage.
        /// </summary>
        CausedDamage,
        /// <summary>
        /// The item caused a Pokémon's <see cref="PBEStatus1"/> or <see cref="PBEStatus2"/> to change.
        /// </summary>
        ChangedStatus,
        /// <summary>
        /// The item was consumed by a Pokémon.
        /// </summary>
        Consumed,
        /// <summary>
        /// The item restored HP to a Pokémon.
        /// </summary>
        RestoredHP
    }
    /// <summary>
    /// Represents an action regarding a <see cref="PBEStatus1"/> or <see cref="PBEStatus2"/>.
    /// </summary>
    public enum PBEStatusAction : byte
    {
        /// <summary>
        /// The status activated its main effect.
        /// </summary>
        /// <example>
        /// <see cref="PBEStatus2.Flinching"/> prevented movement.
        /// </example>
        Activated,
        /// <summary>
        /// The status was added to a Pokémon.
        /// </summary>
        /// <example>
        /// The Pokémon became <see cref="PBEStatus1.Paralyzed"/>.
        /// </example>
        Added,
        /// <summary>
        /// The status was cured from a Pokémon.
        /// </summary>
        /// <example>
        /// <see cref="PBEAbility.Limber"/> cured a Pokémon of <see cref="PBEStatus1.Paralyzed"/>.
        /// </example>
        Cured,
        /// <summary>
        /// The status was involved with damage.
        /// </summary>
        /// <example>
        /// A Pokémon's <see cref="PBEStatus2.Substitute"/> took damage.
        /// </example>
        Damage,
        /// <summary>
        /// The status has ended.
        /// </summary>
        /// <example>
        /// A Pokémon with <see cref="PBEStatus2.Confused"/> regained its senses.
        /// </example>
        Ended
    }
    /// <summary>
    /// Represents an action regarding a <see cref="PBETeamStatus"/>.
    /// </summary>
    public enum PBETeamStatusAction : byte
    {
        /// <summary>
        /// The status was added to a team.
        /// </summary>
        /// <example>
        /// The team set up <see cref="PBETeamStatus.LightScreen"/>.
        /// </example>
        Added,
        /// <summary>
        /// The status was removed from a team.
        /// </summary>
        /// <example>
        /// An opponent used <see cref="PBEMove.BrickBreak"/> and destroyed <see cref="PBETeamStatus.Reflect"/>.
        /// </example>
        Cleared,
        /// <summary>
        /// The status caused a Pokémon to take damage.
        /// </summary>
        /// <example>
        /// An Pokémon switched in and took damage from <see cref="PBETeamStatus.StealthRock"/>.
        /// </example>
        Damage,
        /// <summary>
        /// The status ended.
        /// </summary>
        /// <example>
        /// <see cref="PBETeamStatus.LightScreen"/> wore off.
        /// </example>
        Ended
    }
    /// <summary>
    /// Represents the reason for a move failing.
    /// </summary>
    public enum PBEFailReason : byte
    {
        /// <summary>
        /// The move failed because the target is already confused.
        /// </summary>
        AlreadyConfused,
        /// <summary>
        /// General failure.
        /// </summary>
        Default,
        /// <summary>
        /// The move tried to heal a Pokémon's HP when it was already full.
        /// </summary>
        HPFull,
        /// <summary>
        /// The move was used when there were no available targets to hit.
        /// </summary>
        NoTarget
    }
    /// <summary>
    /// Represents an action regarding a <see cref="PBEWeather"/>.
    /// </summary>
    public enum PBEWeatherAction : byte
    {
        /// <summary>
        /// The weather was added to the battle.
        /// </summary>
        Added,
        /// <summary>
        /// The weather caused a Pokémon to take damage.
        /// </summary>
        CausedDamage,
        /// <summary>
        /// The weather was removed from the battle.
        /// </summary>
        Ended
    }
    public enum PBESpecialMessage : byte
    {
        DraggedOut,
        Magnitude,
        PainSplit,
        Recoil,
        Struggle,
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
        Water
    }
    /// <summary>
    /// Represents a specific Pokémon's nature.
    /// </summary>
    public enum PBENature : byte
    {
        /// <summary>
        /// The Pokémon favors attack and hinders special attack.
        /// </summary>
        Adamant,
        /// <summary>
        /// The Pokémon doesn't favor any stat.
        /// </summary>
        Bashful,
        /// <summary>
        /// The Pokémon favors defense and hinders attack.
        /// </summary>
        Bold,
        /// <summary>
        /// The Pokémon favors attack and hinders speed.
        /// </summary>
        Brave,
        /// <summary>
        /// The Pokémon favors special defense and hinders attack.
        /// </summary>
        Calm,
        /// <summary>
        /// The Pokémon favors special defense and hinders special attack.
        /// </summary>
        Careful,
        /// <summary>
        /// The Pokémon doesn't favor any stat.
        /// </summary>
        Docile,
        /// <summary>
        /// The Pokémon favors special defense and hinders defense.
        /// </summary>
        Gentle,
        /// <summary>
        /// The Pokémon doesn't favor any stat.
        /// </summary>
        Hardy,
        /// <summary>
        /// The Pokémon favors speed and hinders defense.
        /// </summary>
        Hasty,
        /// <summary>
        /// The Pokémon favors defense and hinders special attack.
        /// </summary>
        Impish,
        /// <summary>
        /// The Pokémon favors speed and hinders special attack.
        /// </summary>
        Jolly,
        /// <summary>
        /// The Pokémon favors defense and hinders special defense.
        /// </summary>
        Lax,
        /// <summary>
        /// The Pokémon favors attack and hinders defense.
        /// </summary>
        Lonely,
        /// <summary>
        /// The Pokémon favors special attack and hinders defense.
        /// </summary>
        Mild,
        /// <summary>
        /// The Pokémon favors special attack and hinders attack.
        /// </summary>
        Modest,
        /// <summary>
        /// The Pokémon favors speed and hinders special defense.
        /// </summary>
        Naive,
        /// <summary>
        /// The Pokémon favors attack and hinders special defense.
        /// </summary>
        Naughty,
        /// <summary>
        /// The Pokémon favors special attack and hinders speed.
        /// </summary>
        Quiet,
        /// <summary>
        /// The Pokémon doesn't favor any stat.
        /// </summary>
        Quirky,
        /// <summary>
        /// The Pokémon favors special attack and hinders special defense.
        /// </summary>
        Rash,
        /// <summary>
        /// The Pokémon favors defense and hinders speed.
        /// </summary>
        Relaxed,
        /// <summary>
        /// The Pokémon favors special defense and hinders speed.
        /// </summary>
        Sassy,
        /// <summary>
        /// The Pokémon doesn't favor any stat.
        /// </summary>
        Serious,
        /// <summary>
        /// The Pokémon favors speed and hinders attack.
        /// </summary>
        Timid,
        /// <summary>
        /// Invalid nature.
        /// </summary>
        MAX
    }
    /// <summary>
    /// Represents a specific Pokémon's held item.
    /// </summary>
    public enum PBEItem : ushort
    {
        /// <summary>
        /// No item.
        /// </summary>
        None = 0,
        AbsorbBulb = 545, // TODO
        AdamantOrb = 135,
        AguavBerry = 162, // TODO
        AirBalloon = 541, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        AmuletCoin = 223,
        /// <summary>
        /// No effect.
        /// </summary>
        Antidote = 18,
        ApicotBerry = 205, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        ArmorFossil = 104,
        AspearBerry = 153, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        Awakening = 21,
        BabiriBerry = 199, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        BalmMushroom = 580,
        BelueBerry = 183, // TODO
        BerryJuice = 43, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        BigMushroom = 87,
        /// <summary>
        /// No effect.
        /// </summary>
        BigNugget = 581,
        /// <summary>
        /// No effect.
        /// </summary>
        BigPearl = 89,
        BigRoot = 296, // TODO
        BindingBand = 544, // TODO
        BlackBelt = 241,
        /// <summary>
        /// No effect.
        /// </summary>
        BlackFlute = 68,
        BlackGlasses = 240,
        BlackSludge = 281,
        /// <summary>
        /// No effect.
        /// </summary>
        BlkApricorn = 491,
        /// <summary>
        /// No effect.
        /// </summary>
        BluApricorn = 486,
        /// <summary>
        /// No effect.
        /// </summary>
        BlueFlute = 65,
        /// <summary>
        /// No effect.
        /// </summary>
        BlueScarf = 261,
        /// <summary>
        /// No effect.
        /// </summary>
        BlueShard = 73,
        BlukBerry = 165, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        BridgeMailD = 145,
        /// <summary>
        /// No effect.
        /// </summary>
        BridgeMailM = 148,
        /// <summary>
        /// No effect.
        /// </summary>
        BridgeMailS = 144,
        /// <summary>
        /// No effect.
        /// </summary>
        BridgeMailT = 146,
        /// <summary>
        /// No effect.
        /// </summary>
        BridgeMailV = 147,
        BrightPowder = 213,
        BugGem = 558,
        BurnDrive = 118,
        /// <summary>
        /// No effect.
        /// </summary>
        BurnHeal = 19,
        /// <summary>
        /// No effect.
        /// </summary>
        Calcium = 49,
        /// <summary>
        /// No effect.
        /// </summary>
        Carbos = 48,
        /// <summary>
        /// No effect.
        /// </summary>
        Casteliacone = 591,
        CellBattery = 546, // TODO
        Charcoal = 249,
        ChartiBerry = 195, // TODO
        CheriBerry = 149, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        CherishBall = 16,
        ChestoBerry = 150, // TODO
        ChilanBerry = 200, // TODO
        ChillDrive = 119,
        ChoiceBand = 220,
        ChoiceScarf = 287,
        ChoiceSpecs = 297,
        ChopleBerry = 189, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        ClawFossil = 100,
        /// <summary>
        /// No effect.
        /// </summary>
        CleanseTag = 224,
        /// <summary>
        /// No effect.
        /// </summary>
        CleverWing = 569,
        CobaBerry = 192, // TODO
        ColburBerry = 198, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        CometShard = 583,
        CornnBerry = 175, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        CoverFossil = 572,
        CustapBerry = 210, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        DampMulch = 96,
        DampRock = 285,
        DarkGem = 562,
        /// <summary>
        /// No effect.
        /// </summary>
        DawnStone = 109,
        DeepSeaScale = 227,
        DeepSeaTooth = 226,
        DestinyKnot = 280, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        DireHit = 56,
        /// <summary>
        /// No effect.
        /// </summary>
        DiveBall = 7,
        /// <summary>
        /// No effect.
        /// </summary>
        DomeFossil = 102,
        DouseDrive = 116,
        DracoPlate = 311,
        DragonFang = 250,
        DragonGem = 561,
        /// <summary>
        /// No effect.
        /// </summary>
        DragonScale = 235,
        DreadPlate = 312,
        /// <summary>
        /// No effect.
        /// </summary>
        DreamBall = 576,
        /// <summary>
        /// No effect.
        /// </summary>
        DubiousDisc = 324,
        DurinBerry = 182, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        DuskBall = 13,
        /// <summary>
        /// No effect.
        /// </summary>
        DuskStone = 108,
        EarthPlate = 305,
        EjectButton = 547, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        Electirizer = 322,
        ElectricGem = 550,
        /// <summary>
        /// No effect.
        /// </summary>
        Elixir = 40,
        /// <summary>
        /// No effect.
        /// </summary>
        EnergyPowder = 34,
        /// <summary>
        /// No effect.
        /// </summary>
        EnergyRoot = 35,
        EnigmaBerry = 208, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        EscapeRope = 78,
        /// <summary>
        /// No effect.
        /// </summary>
        Ether = 38,
        /// <summary>
        /// No effect.
        /// </summary>
        Everstone = 229,
        Eviolite = 538,
        ExpertBelt = 268,
        /// <summary>
        /// No effect.
        /// </summary>
        ExpShare = 216,
        /// <summary>
        /// No effect.
        /// </summary>
        FastBall = 492,
        /// <summary>
        /// No effect.
        /// </summary>
        FavoredMail = 138,
        FightingGem = 553,
        FigyBerry = 159, // TODO
        FireGem = 548,
        /// <summary>
        /// No effect.
        /// </summary>
        FireStone = 82,
        FistPlate = 303,
        /// <summary>
        /// The Pokémon contracts <see cref="PBEStatus1.Burned"/> at the end of each turn if it has no other <see cref="PBEStatus1"/> and it does not have <see cref="PBEType.Fire"/>.
        /// </summary>
        FlameOrb = 273,
        FlamePlate = 298,
        FloatStone = 539, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        FluffyTail = 64,
        FlyingGem = 556,
        FocusBand = 230, // TODO
        FocusSash = 275, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        FreshWater = 30,
        /// <summary>
        /// No effect.
        /// </summary>
        FriendBall = 497,
        /// <summary>
        /// No effect.
        /// </summary>
        FullHeal = 27,
        FullIncense = 316, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        FullRestore = 23,
        GanlonBerry = 202, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        GeniusWing = 568,
        GhostGem = 560,
        /// <summary>
        /// No effect.
        /// </summary>
        GooeyMulch = 98,
        GrassGem = 551,
        /// <summary>
        /// No effect.
        /// </summary>
        GreatBall = 3,
        /// <summary>
        /// No effect.
        /// </summary>
        GreenScarf = 263,
        /// <summary>
        /// No effect.
        /// </summary>
        GreenShard = 75,
        /// <summary>
        /// No effect.
        /// </summary>
        GreetMail = 137,
        GrepaBerry = 173, // TODO
        GripClaw = 286, // TODO
        GriseousOrb = 112,
        /// <summary>
        /// No effect.
        /// </summary>
        GrnApricorn = 488,
        GroundGem = 555,
        /// <summary>
        /// No effect.
        /// </summary>
        GrowthMulch = 95,
        /// <summary>
        /// No effect.
        /// </summary>
        GuardSpec = 55,
        HabanBerry = 197, // TODO
        HardStone = 238,
        /// <summary>
        /// No effect.
        /// </summary>
        HealBall = 14,
        /// <summary>
        /// No effect.
        /// </summary>
        HealPowder = 36,
        /// <summary>
        /// No effect.
        /// </summary>
        HealthWing = 565,
        /// <summary>
        /// No effect.
        /// </summary>
        HeartScale = 93,
        HeatRock = 284,
        /// <summary>
        /// No effect.
        /// </summary>
        HeavyBall = 495,
        /// <summary>
        /// No effect.
        /// </summary>
        HelixFossil = 101,
        HondewBerry = 172, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        Honey = 94,
        /// <summary>
        /// No effect.
        /// </summary>
        HPUp = 45,
        /// <summary>
        /// No effect.
        /// </summary>
        HyperPotion = 25,
        IapapaBerry = 163, // TODO
        IceGem = 552,
        /// <summary>
        /// No effect.
        /// </summary>
        IceHeal = 20,
        IciclePlate = 302,
        IcyRock = 282,
        /// <summary>
        /// No effect.
        /// </summary>
        InquiryMail = 141,
        InsectPlate = 308,
        /// <summary>
        /// No effect.
        /// </summary>
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
        /// <summary>
        /// No effect.
        /// </summary>
        LavaCookie = 42,
        LaxIncense = 255,
        /// <summary>
        /// No effect.
        /// </summary>
        LeafStone = 85,
        Leftovers = 234,
        /// <summary>
        /// No effect.
        /// </summary>
        Lemonade = 32,
        LeppaBerry = 154, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        LevelBall = 493,
        LiechiBerry = 201, // TODO
        LifeOrb = 270,
        LightBall = 236,
        LightClay = 269,
        /// <summary>
        /// No effect.
        /// </summary>
        LikeMail = 142,
        /// <summary>
        /// No effect.
        /// </summary>
        LoveBall = 496,
        /// <summary>
        /// No effect.
        /// </summary>
        LuckIncense = 319,
        /// <summary>
        /// No effect.
        /// </summary>
        LuckyEgg = 231,
        LuckyPunch = 256,
        LumBerry = 157, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        LureBall = 494,
        LustrousOrb = 136,
        /// <summary>
        /// No effect.
        /// </summary>
        LuxuryBall = 11,
        MachoBrace = 215,
        /// <summary>
        /// No effect.
        /// </summary>
        Magmarizer = 323,
        Magnet = 242,
        MagoBerry = 161, // TODO
        MagostBerry = 176, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        MasterBall = 1,
        /// <summary>
        /// No effect.
        /// </summary>
        MaxElixir = 41,
        /// <summary>
        /// No effect.
        /// </summary>
        MaxEther = 39,
        /// <summary>
        /// No effect.
        /// </summary>
        MaxPotion = 24,
        /// <summary>
        /// No effect.
        /// </summary>
        MaxRepel = 77,
        /// <summary>
        /// No effect.
        /// </summary>
        MaxRevive = 29,
        MeadowPlate = 301,
        MentalHerb = 219, // TODO
        MetalCoat = 233,
        MetalPowder = 257,
        Metronome = 277, // TODO
        MicleBerry = 209, // TODO
        MindPlate = 307,
        MiracleSeed = 239,
        /// <summary>
        /// No effect.
        /// </summary>
        MoomooMilk = 33,
        /// <summary>
        /// No effect.
        /// </summary>
        MoonBall = 498,
        /// <summary>
        /// No effect.
        /// </summary>
        MoonStone = 81,
        MuscleBand = 266,
        /// <summary>
        /// No effect.
        /// </summary>
        MuscleWing = 566,
        MysticWater = 243,
        NanabBerry = 166, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        NestBall = 8,
        /// <summary>
        /// No effect.
        /// </summary>
        NetBall = 6,
        NeverMeltIce = 246,
        NomelBerry = 178, // TODO
        NormalGem = 564,
        /// <summary>
        /// No effect.
        /// </summary>
        Nugget = 92,
        OccaBerry = 184, // TODO
        OddIncense = 314,
        /// <summary>
        /// No effect.
        /// </summary>
        OddKeystone = 111,
        /// <summary>
        /// No effect.
        /// </summary>
        OldAmber = 103,
        /// <summary>
        /// No effect.
        /// </summary>
        OldGateau = 54,
        OranBerry = 155, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        OvalStone = 110,
        PamtreBerry = 180, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        ParalyzeHeal = 22,
        /// <summary>
        /// No effect.
        /// </summary>
        ParkBall = 500,
        PasshoBerry = 185, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        PassOrb = 575,
        PayapaBerry = 193, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        Pearl = 88,
        /// <summary>
        /// No effect.
        /// </summary>
        PearlString = 582,
        PechaBerry = 151, // TODO
        PersimBerry = 156, // TODO
        PetayaBerry = 204, // TODO
        PinapBerry = 168, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        PinkScarf = 262,
        /// <summary>
        /// No effect.
        /// </summary>
        PlumeFossil = 573,
        /// <summary>
        /// No effect.
        /// </summary>
        PnkApricorn = 489,
        PoisonBarb = 245,
        PoisonGem = 554,
        /// <summary>
        /// No effect.
        /// </summary>
        PokeBall = 4,
        /// <summary>
        /// No effect.
        /// </summary>
        PokeDoll = 63,
        /// <summary>
        /// No effect.
        /// </summary>
        PokeToy = 577,
        PomegBerry = 169, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        Potion = 17,
        PowerAnklet = 293,
        PowerBand = 292,
        PowerBelt = 290,
        PowerBracer = 289,
        PowerHerb = 271,
        PowerLens = 291,
        PowerWeight = 294,
        /// <summary>
        /// No effect.
        /// </summary>
        PPMax = 53,
        /// <summary>
        /// No effect.
        /// </summary>
        PPUp = 51,
        /// <summary>
        /// No effect.
        /// </summary>
        PremierBall = 12,
        /// <summary>
        /// No effect.
        /// </summary>
        PrettyWing = 571,
        /// <summary>
        /// No effect.
        /// </summary>
        PrismScale = 537,
        /// <summary>
        /// No effect.
        /// </summary>
        Protector = 321,
        /// <summary>
        /// No effect.
        /// </summary>
        Protein = 46,
        PsychicGem = 557,
        /// <summary>
        /// No effect.
        /// </summary>
        PureIncense = 320,
        QualotBerry = 171, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        QuickBall = 15,
        QuickClaw = 217, // TODO
        QuickPowder = 274,
        RabutaBerry = 177, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        RageCandyBar = 504,
        /// <summary>
        /// No effect.
        /// </summary>
        RareBone = 106,
        /// <summary>
        /// No effect.
        /// </summary>
        RareCandy = 50,
        RawstBerry = 152, // TODO
        RazorClaw = 326,
        RazorFang = 327, // TODO
        RazzBerry = 164, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        ReaperCloth = 325,
        /// <summary>
        /// No effect.
        /// </summary>
        RedApricorn = 485,
        RedCard = 542, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        RedFlute = 67,
        /// <summary>
        /// No effect.
        /// </summary>
        RedScarf = 260,
        /// <summary>
        /// No effect.
        /// </summary>
        RedShard = 72,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicBand = 588,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicCopper = 584,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicCrown = 590,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicGold = 586,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicSilver = 585,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicStatue = 589,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicVase = 587,
        /// <summary>
        /// No effect.
        /// </summary>
        RepeatBall = 9,
        /// <summary>
        /// No effect.
        /// </summary>
        Repel = 79,
        /// <summary>
        /// No effect.
        /// </summary>
        ReplyMail = 143,
        /// <summary>
        /// No effect.
        /// </summary>
        ResistWing = 567,
        /// <summary>
        /// No effect.
        /// </summary>
        RevivalHerb = 37,
        /// <summary>
        /// No effect.
        /// </summary>
        Revive = 28,
        RindoBerry = 187, // TODO
        RingTarget = 543, // TODO
        RockGem = 559,
        RockIncense = 315,
        RockyHelmet = 540, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        RootFossil = 99,
        RoseIncense = 318,
        RowapBerry = 212, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        RSVPMail = 139,
        /// <summary>
        /// No effect.
        /// </summary>
        SacredAsh = 44,
        /// <summary>
        /// No effect.
        /// </summary>
        SafariBall = 5,
        SalacBerry = 203, // TODO
        ScopeLens = 232,
        SeaIncense = 254,
        SharpBeak = 244,
        ShedShell = 295, // TODO
        ShellBell = 253, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        ShinyStone = 107,
        /// <summary>
        /// No effect.
        /// </summary>
        ShoalSalt = 70,
        /// <summary>
        /// No effect.
        /// </summary>
        ShoalShell = 71,
        ShockDrive = 117,
        ShucaBerry = 191, // TODO
        SilkScarf = 251,
        SilverPowder = 222,
        SitrusBerry = 158, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        SkullFossil = 105,
        SkyPlate = 306,
        /// <summary>
        /// No effect.
        /// </summary>
        SmokeBall = 228,
        SmoothRock = 283,
        /// <summary>
        /// No effect.
        /// </summary>
        SodaPop = 31,
        SoftSand = 237,
        /// <summary>
        /// No effect.
        /// </summary>
        SootheBell = 218,
        SoulDew = 225,
        SpellTag = 247,
        SpelonBerry = 179, // TODO
        SplashPlate = 299,
        SpookyPlate = 310,
        /// <summary>
        /// No effect.
        /// </summary>
        SportBall = 499,
        /// <summary>
        /// No effect.
        /// </summary>
        StableMulch = 97,
        /// <summary>
        /// No effect.
        /// </summary>
        Stardust = 90,
        StarfBerry = 207, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        StarPiece = 91,
        SteelGem = 563,
        Stick = 259,
        StickyBarb = 288, // TODO
        StonePlate = 309,
        /// <summary>
        /// No effect.
        /// </summary>
        SunStone = 80,
        /// <summary>
        /// No effect.
        /// </summary>
        SuperPotion = 26,
        /// <summary>
        /// No effect.
        /// </summary>
        SuperRepel = 76,
        /// <summary>
        /// No effect.
        /// </summary>
        SweetHeart = 134,
        /// <summary>
        /// No effect.
        /// </summary>
        SwiftWing = 570,
        TamatoBerry = 174, // TODO
        TangaBerry = 194, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        ThanksMail = 140,
        ThickClub = 258,
        /// <summary>
        /// No effect.
        /// </summary>
        Thunderstone = 83,
        /// <summary>
        /// No effect.
        /// </summary>
        TimerBall = 10,
        /// <summary>
        /// No effect.
        /// </summary>
        TinyMushroom = 86,
        /// <summary>
        /// The Pokémon contracts <see cref="PBEStatus1.BadlyPoisoned"/> at the end of each turn if it has no other <see cref="PBEStatus1"/> and it does not have <see cref="PBEType.Poison"/> or <see cref="PBEType.Steel"/>.
        /// </summary>
        ToxicOrb = 272,
        ToxicPlate = 304,
        TwistedSpoon = 248,
        /// <summary>
        /// No effect.
        /// </summary>
        UltraBall = 2,
        UpGrade = 252,
        WacanBerry = 186, // TODO
        WaterGem = 549,
        /// <summary>
        /// No effect.
        /// </summary>
        WaterStone = 84,
        WatmelBerry = 181, // TODO
        WaveIncense = 317,
        WepearBerry = 167, // TODO
        WhiteHerb = 214, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        WhiteFlute = 69,
        WideLens = 265,
        WikiBerry = 160, // TODO
        WiseGlasses = 267,
        /// <summary>
        /// No effect.
        /// </summary>
        WhtApricorn = 490,
        /// <summary>
        /// No effect.
        /// </summary>
        XAccuracy = 60,
        /// <summary>
        /// No effect.
        /// </summary>
        XAttack = 57,
        /// <summary>
        /// No effect.
        /// </summary>
        XDefend = 58,
        /// <summary>
        /// No effect.
        /// </summary>
        XSpecial = 61,
        /// <summary>
        /// No effect.
        /// </summary>
        XSpDef = 62,
        /// <summary>
        /// No effect.
        /// </summary>
        XSpeed = 59,
        YacheBerry = 188, // TODO
        /// <summary>
        /// No effect.
        /// </summary>
        YellowFlute = 66,
        /// <summary>
        /// No effect.
        /// </summary>
        YellowScarf = 264,
        /// <summary>
        /// No effect.
        /// </summary>
        YellowShard = 74,
        /// <summary>
        /// No effect.
        /// </summary>
        YlwApricorn = 487,
        ZapPlate = 300,
        /// <summary>
        /// No effect.
        /// </summary>
        Zinc = 52,
        ZoomLens = 276 // TODO
    }
    /// <summary>
    /// Represents a specific Pokémon's special ability.
    /// </summary>
    public enum PBEAbility : byte
    {
        /// <summary>
        /// The Pokémon's ability was removed.
        /// </summary>
        None = 0,
        /// <summary>
        /// The Pokémon has a stronger same-type-attack-bonus.
        /// </summary>
        Adaptability = 91,
        Aftermath = 106, // TODO
        AirLock = 76, // TODO
        Analytic = 148, // TODO
        AngerPoint = 83, // TODO
        Anticipation = 107, // TODO
        ArenaTrap = 71, // TODO
        BadDreams = 123, // TODO
        /// <summary>
        /// The Pokémon takes no critical hits.
        /// </summary>
        BattleArmor = 4,
        BigPecks = 145, // TODO
        /// <summary>
        /// When the Pokémon has low HP, its Fire-type moves get a power boost.
        /// </summary>
        Blaze = 66,
        /// <summary>
        /// The Pokémon gets a speed boost in harsh sunlight.
        /// </summary>
        Chlorophyll = 34,
        ClearBody = 29, // TODO
        CloudNine = 13, // TODO
        ColorChange = 16, // TODO
        /// <summary>
        /// The Pokémon accuracy is boosted.
        /// </summary>
        Compoundeyes = 14,
        Contrary = 126, // TODO
        CursedBody = 130, // TODO
        CuteCharm = 56, // TODO
        Damp = 6, // TODO
        Defeatist = 129, // TODO
        Defiant = 128, // TODO
        Download = 88, // TODO
        /// <summary>
        /// The Pokémon changes the weather to infinite rain.
        /// </summary>
        Drizzle = 2,
        /// <summary>
        /// The Pokémon changes the weather to infinite harsh sunlight.
        /// </summary>
        Drought = 70,
        DrySkin = 87, // TODO
        EarlyBird = 48, // TODO
        EffectSpore = 27, // TODO
        /// <summary>
        /// The Pokémon takes less damage from incoming super-effective moves.
        /// </summary>
        Filter = 111,
        FlameBody = 49, // TODO
        FlareBoost = 138, // TODO
        FlashFire = 18, // TODO
        FlowerGift = 122, // TODO
        Forecast = 59, // TODO
        Forewarn = 108, // TODO
        FriendGuard = 132, // TODO
        Frisk = 119, // TODO
        Gluttony = 82, // TODO
        /// <summary>
        /// The Pokémon's attack is boosted when it is afflicted with a <see cref="PBEStatus1"/>, and the damage reduction from <see cref="PBEStatus1.Burned"/> is not applied.
        /// </summary>
        Guts = 62,
        Harvest = 139, // TODO
        Healer = 131, // TODO
        /// <summary>
        /// The Pokémon takes less damage from Fire-type moves and from a burn.
        /// </summary>
        Heatproof = 85,
        HeavyMetal = 134, // TODO
        /// <summary>
        /// No effect in battle.
        /// </summary>
        HoneyGather = 118,
        /// <summary>
        /// The Pokémon's attack is boosted.
        /// </summary>
        HugePower = 37,
        /// <summary>
        /// The Pokémon's attack is boosted, but its accuracy is lower for physical moves.
        /// </summary>
        Hustle = 55,
        Hydration = 93, // TODO
        HyperCutter = 52, // TODO
        /// <summary>
        /// In a hailstorm, the Pokémon takes no damage from the hailstorm and restores HP at the end of each turn.
        /// </summary>
        IceBody = 115,
        /// <summary>
        /// No effect in battle.
        /// </summary>
        Illuminate = 35,
        Illusion = 149, // TODO
        Immunity = 17, // TODO
        /// <summary>
        /// The Pokémon transforms into the foe across from it when switching in.
        /// </summary>
        Imposter = 150,
        Infiltrator = 151, // TODO
        InnerFocus = 39, // TODO
        Insomnia = 15, // TODO
        Intimidate = 22, // TODO
        IronBarbs = 160, // TODO
        /// <summary>
        /// The power of moves with <see cref="PBEMoveFlag.AffectedByIronFist"/> is increased.
        /// </summary>
        IronFist = 89,
        Justified = 154, // TODO
        KeenEye = 51, // TODO
        Klutz = 103, // TODO
        LeafGuard = 102, // TODO
        /// <summary>
        /// The Pokémon is immune to Ground-type moves and most entry hazards.
        /// </summary>
        Levitate = 26,
        LightMetal = 135, // TODO
        Lightningrod = 31, // TODO
        /// <summary>
        /// The Pokémon cannot be paralyzed.
        /// </summary>
        Limber = 7,
        LiquidOoze = 64, // TODO
        MagicBounce = 156, // TODO
        MagicGuard = 98, // TODO
        MagmaArmor = 40, // TODO
        MagnetPull = 42, // TODO
        /// <summary>
        /// The Pokémon's defense is boosted when it is afflicted with a <see cref="PBEStatus1"/>.
        /// </summary>
        MarvelScale = 63,
        Minus = 58, // TODO
        MoldBreaker = 104, // TODO
        Moody = 141, // TODO
        MotorDrive = 78, // TODO
        Moxie = 153, // TODO
        Multiscale = 136, // TODO
        /// <summary>
        /// No effect in battle.
        /// </summary>
        Multitype = 121,
        Mummy = 152, // TODO
        NaturalCure = 30, // TODO
        /// <summary>
        /// The Pokémon will always hit and always get hit unless protection is used.
        /// </summary>
        NoGuard = 99,
        Normalize = 96, // TODO: Main effect, ignore [weather ball, hidden power, natural gift, judgment, techno blast], separate thunder wave from other status force effects
        Oblivious = 12, // TODO
        /// <summary>
        /// The Pokémon takes no damage from a hailstorm or sandstorm.
        /// </summary>
        Overcoat = 142,
        /// <summary>
        /// When the Pokémon has low HP, its Grass-type moves get a power boost.
        /// </summary>
        Overgrow = 65,
        OwnTempo = 20, // TODO
        Pickpocket = 124, // TODO
        Pickup = 53, // TODO
        Plus = 57, // TODO
        PoisonHeal = 90, // TODO
        PoisonPoint = 38, // TODO
        PoisonTouch = 143, // TODO
        Prankster = 158, // TODO
        Pressure = 46, // TODO
        /// <summary>
        /// The Pokémon's attack is boosted.
        /// </summary>
        PurePower = 74,
        QuickFeet = 95, // TODO
        /// <summary>
        /// In rain, the Pokémon restores HP at the end of each turn.
        /// </summary>
        RainDish = 44,
        Rattled = 155, // TODO
        Reckless = 120, // TODO
        Regenerator = 144, // TODO
        Rivalry = 79, // TODO
        RockHead = 69, // TODO
        RoughSkin = 24, // TODO
        /// <summary>
        /// No effect in battle.
        /// </summary>
        RunAway = 50,
        /// <summary>
        /// In a sandstorm, the Pokémon takes no damage from the sandstorm and its Rock-, Ground-, and Steel-type moves get a power boost.
        /// </summary>
        SandForce = 159,
        /// <summary>
        /// The Pokémon gets a speed boost in a sandstorm.
        /// </summary>
        SandRush = 146,
        /// <summary>
        /// The Pokémon changes the weather to an infinite sandstorm.
        /// </summary>
        SandStream = 45,
        /// <summary>
        /// In a sandstorm, the Pokémon takes no damage from the sandstorm and gets a 20% evasion boost.
        /// </summary>
        SandVeil = 8,
        SapSipper = 157, // TODO
        Scrappy = 113, // TODO
        SereneGrace = 32, // TODO
        ShadowTag = 23, // TODO
        ShedSkin = 61, // TODO
        SheerForce = 125, // TODO
        /// <summary>
        /// The Pokémon takes no critical hits.
        /// </summary>
        ShellArmor = 75,
        ShieldDust = 19, // TODO
        /// <summary>
        /// The Pokémon's stat changes are doubled.
        /// </summary>
        Simple = 86,
        SkillLink = 92, // TODO
        SlowStart = 112, // TODO
        /// <summary>
        /// The Pokémon deals more damage when landing critical hits.
        /// </summary>
        Sniper = 97,
        /// <summary>
        /// In a hailstorm, the Pokémon takes no damage from the hailstorm and gets a 20% evasion boost.
        /// </summary>
        SnowCloak = 81,
        /// <summary>
        /// The Pokémon changes the weather to an infinite hailstorm.
        /// </summary>
        SnowWarning = 117,
        /// <summary>
        /// In harsh sunlight, the Pokémon gets a special attack boost and takes damage at the end of each turn.
        /// </summary>
        SolarPower = 94,
        /// <summary>
        /// The Pokémon takes less damage from incoming super-effective moves.
        /// </summary>
        SolidRock = 116,
        Soundproof = 43, // TODO
        SpeedBoost = 3, // TODO
        Stall = 100, // TODO
        Static = 9, // TODO
        Steadfast = 80, // TODO
        Stench = 1, // TODO
        StickyHold = 60, // TODO
        StormDrain = 114, // TODO
        Sturdy = 5, // TODO
        SuctionCups = 21, // TODO
        /// <summary>
        /// The Pokémon is more likely to land critical hits.
        /// </summary>
        SuperLuck = 105,
        /// <summary>
        /// When the Pokémon has low HP, its Bug-type moves get a power boost.
        /// </summary>
        Swarm = 68,
        /// <summary>
        /// The Pokémon gets a speed boost in rain.
        /// </summary>
        SwiftSwim = 33,
        Synchronize = 28, // TODO
        TangledFeet = 77, // TODO
        Technician = 101, // TODO
        Telepathy = 140, // TODO
        Teravolt = 164, // TODO
        /// <summary>
        /// The Pokémon takes less damage from Ice- and Fire-type moves.
        /// </summary>
        ThickFat = 47,
        /// <summary>
        /// The Pokémon deals double damage with outgoing not-very-effective moves.
        /// </summary>
        TintedLens = 110,
        /// <summary>
        /// When the Pokémon has low HP, its Water-type moves get a power boost.
        /// </summary>
        Torrent = 67,
        ToxicBoost = 137, // TODO
        Trace = 36, // TODO
        Truant = 54, // TODO
        Turboblaze = 163, // TODO
        Unaware = 109, // TODO
        Unburden = 84, // TODO
        Unnerve = 127, // TODO
        VictoryStar = 162, // TODO
        VitalSpirit = 72, // TODO
        VoltAbsorb = 10, // TODO
        WaterAbsorb = 11, // TODO
        WaterVeil = 41, // TODO
        WeakArmor = 133, // TODO
        WhiteSmoke = 73, // TODO
        /// <summary>
        /// The Pokémon is immune to all damaging moves except for moves that would deal super-effective damage.
        /// </summary>
        WonderGuard = 25, // TODO: Mold Breaker, Teravolt, Turboblaze, Role Play, Skill Swap
        WonderSkin = 147, // TODO
        ZenMode = 161, // TODO
        /// <summary>
        /// Invalid ability.
        /// </summary>
        MAX = 165,
    }
    /// <summary>
    /// Represents a specific Pokémon species.
    /// </summary>
    public enum PBESpecies : uint
    {
        Absol = 359,
        Alomomola = 594,
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
        Azelf = 482,
        Azumarill = 184,
        Azurill = 298,
        Beedrill = 15,
        Blastoise = 9,
        Blaziken = 257,
        Blissey = 242,
        Bouffalant = 626,
        Bulbasaur = 1,
        Butterfree = 12,
        Carracosta = 565,
        Caterpie = 10,
        Chandelure = 609,
        Chansey = 113,
        Charizard = 6,
        Charmander = 4,
        Charmeleon = 5,
        Clamperl = 366,
        Cofagrigus = 563,
        Combusken = 256,
        Cradily = 346,
        Cresselia = 488,
        Crobat = 169,
        Cryogonal = 615,
        Cubone = 104,
        Darkrai = 491,
        Deino = 633,
        Delcatty = 301,
        Dialga = 483,
        Ditto = 132,
        Druddigon = 621,
        Eevee = 133,
        Emolga = 587,
        Espeon = 196,
        Farfetchd = 83,
        Flareon = 136,
        Gabite = 444,
        Garchomp = 445,
        Genesect = 649 | (0 << 0x10),
        Genesect_Burn = 649 | (1 << 0x10),
        Genesect_Chill = 649 | (2 << 0x10),
        Genesect_Douse = 649 | (3 << 0x10),
        Genesect_Shock = 649 | (4 << 0x10),
        Gible = 443,
        Giratina = 487 | (0 << 0x10),
        Giratina_Origin = 487 | (1 << 0x10),
        Glaceon = 471,
        Golbat = 42,
        Golduck = 55,
        Gorebyss = 368,
        Gothita = 574,
        Gothitelle = 576,
        Gothorita = 575,
        Gyarados = 130,
        Happiny = 440,
        Huntail = 367,
        Hydreigon = 635,
        Ivysaur = 2,
        Jirachi = 385,
        Jolteon = 135,
        Kakuna = 14,
        Lampent = 608,
        Larvesta = 636,
        Latias = 380,
        Latios = 381,
        Leafeon = 470,
        Lileep = 345,
        Litwick = 607,
        Lucario = 448,
        Luxio = 404,
        Luxray = 405,
        Magikarp = 129,
        Marill = 183,
        Marowak = 105,
        Mesprit = 481,
        Metagross = 376,
        Metapod = 11,
        Minun = 312,
        Misdreavus = 200,
        Mismagius = 429,
        Nincada = 290,
        Ninetales = 38,
        Ninjask = 291,
        Pachirisu = 417,
        Palkia = 484,
        Pichu = 172,
        Pikachu = 25,
        Plusle = 311,
        Politoed = 186,
        Poliwag = 60
        Poliwhirl = 61,
        Poliwrath = 62,
        Psyduck = 54,
        Raichu = 26,
        Regirock = 377,
        Riolu = 447,
        Rotom = 479 | (0 << 0x10),
        Rotom_Fan = 479 | (1 << 0x10),
        Rotom_Frost = 479 | (2 << 0x10),
        Rotom_Heat = 479 | (3 << 0x10),
        Rotom_Mow = 479 | (4 << 0x10),
        Rotom_Wash = 479 | (5 << 0x10),
        Shedinja = 292,
        Shinx = 403,
        Skitty = 300,
        Smeargle = 235,
        Squirtle = 7,
        Tirtouga = 564,
        Torchic = 255,
        Tropius = 357,
        Umbreon = 197,
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
        Uxie = 480,
        Vaporeon = 134,
        Venusaur = 3,
        Victini = 494,
        Volcarona = 637,
        Vulpix = 37,
        Wartortle = 8,
        Weedle = 13,
        Yamask = 562,
        Zubat = 41,
        Zweilous = 634
    }
    public enum PBEMoveTarget : byte // Used in MoveData
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
    /// <summary>
    /// Represents a specific <see cref="PBEMove"/>'s flags.
    /// </summary>
    [Flags]
    public enum PBEMoveFlag : ushort
    {
        /// <summary>
        /// The move has no flags.
        /// </summary>
        None,
        /// <summary>
        /// The move's power is boosted by <see cref="PBEAbility.IronFist"/>.
        /// </summary>
        AffectedByIronFist = 1 << 0,
        /// <summary>
        /// The move is blocked by <see cref="PBEMove.MagicCoat"/> and <see cref="PBEAbility.MagicBounce"/>.
        /// </summary>
        AffectedByMagicCoat = 1 << 1,
        /// <summary>
        /// The move can be copied by <see cref="PBEMove.MirrorMove"/>.
        /// </summary>
        AffectedByMirrorMove = 1 << 2,
        /// <summary>
        /// The move is blocked by <see cref="PBEMove.Detect"/>, <see cref="PBEMove.Protect"/>, and <see cref="PBEMove.WideGuard"/>.
        /// </summary>
        AffectedByProtect = 1 << 3,
        /// <summary>
        /// The move can be stolen by <see cref="PBEMove.Snatch"/>.
        /// </summary>
        AffectedBySnatch = 1 << 4,
        /// <summary>
        /// The move always lands a critical hit.
        /// </summary>
        AlwaysCrit = 1 << 5,
        /// <summary>
        /// The move removes <see cref="PBEStatus1.Frozen"/> from the user.
        /// </summary>
        DefrostsUser = 1 << 6,
        /// <summary>
        /// The move has a higher chance of landing a critical hit.
        /// </summary>
        HighCritChance = 1 << 7,
        /// <summary>
        /// The move can hit <see cref="PBEStatus2.Airborne"/> targets.
        /// </summary>
        HitsAirborne = 1 << 8,
        /// <summary>
        /// The move can hit <see cref="PBEStatus2.Underground"/> targets.
        /// </summary>
        HitsUnderground = 1 << 9,
        /// <summary>
        /// The move can hit <see cref="PBEStatus2.Underwater"/> targets.
        /// </summary>
        HitsUnderwater = 1 << 10,
        /// <summary>
        /// The user makes contact with the target, causing it to take damage from the target's <see cref="PBEAbility.IronBarbs"/>, <see cref="PBEAbility.RoughSkin"/>, and <see cref="PBEItem.RockyHelmet"/>.
        /// </summary>
        MakesContact = 1 << 11,
        /// <summary>
        /// The move is blocked by <see cref="PBEAbility.Soundproof"/>.
        /// </summary>
        SoundBased = 1 << 12,
        /// <summary>
        /// The move does not consume a held gem.
        /// </summary>
        UnaffectedByGems = 1 << 13
    }
    public enum PBEMoveEffect : byte
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
        Flatter,
        Fly,
        FocusEnergy,
        Growth,
        Hail,
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
        LuckyChant,
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
        Sandstorm,
        Sleep,
        Spikes,
        StealthRock,
        Struggle,
        Substitute,
        SunnyDay,
        Swagger,
        Toxic,
        ToxicSpikes,
        Transform,
        Whirlwind
    }
    public enum PBEMove : ushort
    {
        None = 0,
        Acid = 51,
        AcidArmor = 151,
        AcidSpray = 491,
        AerialAce = 332,
        Aeroblast = 177,
        Agility = 97,
        AirCutter = 314,
        AirSlash = 403,
        Amnesia = 133,
        AncientPower = 246,
        AquaJet = 453,
        AquaTail = 401,
        Astonish = 310,
        AttackOrder = 454,
        AuraSphere = 396,
        AuroraBeam = 62,
        Barrier = 112,
        Bite = 44,
        BlazeKick = 299,
        BlueFlare = 551,
        BrickBreak = 280,
        Brine = 362,
        BodySlam = 34,
        BoltStrike = 550,
        BoneClub = 125,
        Bubble = 145,
        BubbleBeam = 61,
        BugBuzz = 405,
        BulkUp = 339,
        Bulldoze = 523,
        BulletPunch = 418,
        CalmMind = 347,
        ChargeBeam = 451,
        Charm = 204,
        Chatter = 448,
        CloseCombat = 370,
        Coil = 489,
        //CometPunch = 4, // TODO: Iron Fist
        ConfuseRay = 109,
        Confusion = 93,
        CosmicPower = 322,
        CottonGuard = 538,
        Crabhammer = 152,
        CrossChop = 238,
        CrossPoison = 440,
        Crunch = 242,
        CrushClaw = 306,
        Curse = 174,
        Cut = 15,
        DarkPulse = 399,
        DarkVoid = 464,
        DefendOrder = 455,
        DefenseCurl = 111,
        Detect = 197,
        Dig = 91,
        Discharge = 435,
        Dive = 291,
        DizzyPunch = 146,
        DoubleTeam = 104,
        DracoMeteor = 434,
        DragonBreath = 225,
        DragonClaw = 337,
        DragonDance = 349,
        DragonPulse = 406,
        DragonRush = 407,
        //DrainPunch = 409, // TODO: Iron Fist
        DrillPeck = 65,
        DrillRun = 529,
        DynamicPunch = 223,
        EarthPower = 414,
        Earthquake = 89,
        EggBomb = 121,
        Electroweb = 527,
        Ember = 52,
        Endeavor = 283,
        EnergyBall = 412,
        Eruption = 284,
        Extrasensory = 326,
        ExtremeSpeed = 245,
        Facade = 263,
        FaintAttack = 185,
        FakeTears = 313,
        FeatherDance = 297,
        FieryDance = 552,
        FireBlast = 126,
        FirePunch = 7,
        FlameCharge = 488,
        Flamethrower = 53,
        FlameWheel = 172,
        Flash = 148,
        FlashCannon = 430,
        Flatter = 260,
        Fly = 19,
        FocusBlast = 411,
        FocusEnergy = 116,
        //FocusPunch = 264, // TODO: Iron Fist
        ForcePalm = 395,
        FrostBreath = 524,
        Frustration = 218,
        Glaciate = 549,
        Glare = 137,
        GrassKnot = 447,
        GrassWhistle = 320,
        Growl = 45,
        Growth = 74,
        GunkShot = 441,
        Hail = 258,
        HammerArm = 359,
        Harden = 106,
        Headbutt = 29,
        HealOrder = 456,
        HeartStamp = 531,
        HeatCrash = 535,
        HeatWave = 257,
        HeavySlam = 484,
        Hex = 506,
        HiddenPower = 237,
        HoneClaws = 468,
        HornAttack = 30,
        Howl = 336,
        HydroPump = 56,
        HyperFang = 158,
        HyperVoice = 304,
        Hypnosis = 95,
        IceBeam = 58,
        IcePunch = 8,
        IceShard = 420,
        IcicleCrash = 556,
        IcyWind = 196,
        Inferno = 517,
        IronDefense = 334,
        IronHead = 442,
        IronTail = 231,
        KarateChop = 2,
        Kinesis = 134,
        LavaPlume = 346,
        LeafBlade = 348,
        LeafStorm = 437,
        LeafTornado = 536,
        LeechSeed = 73,
        Leer = 43,
        Lick = 122,
        LightScreen = 113,
        LovelyKiss = 142,
        LowKick = 67,
        LowSweep = 490,
        LuckyChant = 381,
        LusterPurge = 295,
        MachPunch = 183,
        MagicalLeaf = 345,
        MagnetBomb = 443,
        Magnitude = 222,
        Meditate = 96,
        Megahorn = 224,
        MegaKick = 25,
        MegaPunch = 5,
        MetalClaw = 232,
        MetalSound = 319,
        MeteorMash = 309,
        MilkDrink = 208,
        Minimize = 107,
        MirrorShot = 429,
        MistBall = 296,
        Moonlight = 236,
        MorningSun = 234,
        MudBomb = 426,
        MuddyWater = 330,
        MudSlap = 189,
        MudShot = 341,
        NastyPlot = 417,
        NeedleArm = 302,
        NightDaze = 539,
        NightSlash = 400,
        Octazooka = 190,
        OminousWind = 466,
        Overheat = 315,
        PainSplit = 220,
        Peck = 64,
        PoisonFang = 305,
        PoisonGas = 139,
        PoisonJab = 398,
        PoisonPowder = 77,
        PoisonSting = 40,
        PoisonTail = 342,
        Pound = 1,
        PowderSnow = 181,
        PowerGem = 408,
        PowerWhip = 438,
        Protect = 182,
        Psybeam = 60,
        Psychic = 94,
        PsychoBoost = 354,
        PsychoCut = 427,
        PsychUp = 244,
        Psyshock = 473,
        Psystrike = 540,
        QuickAttack = 98,
        QuiverDance = 483,
        RainDance = 240,
        RazorLeaf = 75,
        RazorShell = 534,
        Recover = 105,
        Reflect = 115,
        Retaliate = 514, // TODO
        Return = 216,
        Roar = 46, // TODO: Suction Cups, Soundproof, Ingrain
        RockClimb = 431,
        RockPolish = 397,
        RockSlide = 157,
        RockSmash = 249,
        RockThrow = 88,
        RockTomb = 317,
        SacredFire = 221,
        SandAttack = 28,
        Sandstorm = 201,
        Scald = 503,
        ScaryFace = 184,
        Scratch = 10,
        Screech = 103,
        SearingShot = 545,
        SecretSword = 548,
        SeedBomb = 402,
        SeedFlare = 465,
        ShadowBall = 247,
        ShadowClaw = 421,
        ShadowPunch = 325,
        ShadowSneak = 425,
        Sharpen = 159,
        ShellSmash = 504,
        ShiftGear = 508,
        SignalBeam = 324,
        SilverWind = 318,
        Sing = 47,
        ShockWave = 351,
        SkyUppercut = 327,
        SlackOff = 303,
        Slam = 21,
        Slash = 163,
        SleepPowder = 79,
        Sludge = 124,
        SludgeBomb = 188,
        SludgeWave = 482,
        Smog = 123,
        SmokeScreen = 108,
        Snarl = 555,
        Softboiled = 135,
        SpacialRend = 460,
        Spark = 209,
        Spikes = 191,
        Spore = 147,
        StealthRock = 446,
        Steamroller = 537,
        SteelWing = 211,
        Stomp = 23,
        StoneEdge = 444,
        StormThrow = 480,
        Strength = 70,
        StringShot = 81,
        Struggle = 165,
        StruggleBug = 522,
        StunSpore = 78,
        Substitute = 164,
        SunnyDay = 241,
        Superpower = 276,
        Supersonic = 48,
        Surf = 57,
        Swagger = 207,
        SweetKiss = 186,
        SweetScent = 230,
        Swift = 129,
        SwordsDance = 14,
        Synthesis = 235,
        Tackle = 33,
        TailGlow = 294,
        TailWhip = 39,
        TechnoBlast = 546,
        TeeterDance = 298,
        Teleport = 100,
        Thunder = 87,
        Thunderbolt = 85,
        ThunderPunch = 9,
        ThunderShock = 84,
        ThunderWave = 86,
        Tickle = 321,
        Toxic = 92,
        ToxicSpikes = 390,
        Transform = 144,
        VacuumWave = 410,
        VCreate = 557,
        Venoshock = 474,
        ViceGrip = 11,
        VineWhip = 22,
        VitalThrow = 233,
        Waterfall = 127,
        WaterGun = 55,
        WaterPulse = 352,
        WaterSpout = 323,
        WeatherBall = 311,
        Whirlwind = 18, // TODO: Suction Cups, Ingrain
        WillOWisp = 261,
        WingAttack = 17,
        Withdraw = 110,
        WorkUp = 526,
        XScissor = 404,
        ZapCannon = 192,
        ZenHeadbutt = 428,
        /// <summary>
        /// Invalid move.
        /// </summary>
        MAX = 560
    }
}
