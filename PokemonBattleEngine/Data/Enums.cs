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
        HP = 0,
        /// <summary>
        /// Attack.
        /// </summary>
        Attack = 1,
        /// <summary>
        /// Defense.
        /// </summary>
        Defense = 2,
        /// <summary>
        /// Special Attack.
        /// </summary>
        SpAttack = 3,
        /// <summary>
        /// Special Defense.
        /// </summary>
        SpDefense = 4,
        /// <summary>
        /// Speed.
        /// </summary>
        Speed = 5,
        /// <summary>
        /// Accuracy.
        /// </summary>
        Accuracy = 6,
        /// <summary>
        /// Evasion.
        /// </summary>
        Evasion = 7
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
        /// The team will take less damage from <see cref="PBEMoveCategory.Physical"/> moves.
        /// </summary>
        Reflect = 1 << 1,
        /// <summary>
        /// Grounded Pokémon that switch in will take damage.
        /// The amount of damage is based on <see cref="PBETeam.SpikeCount"/>.
        /// </summary>
        Spikes = 1 << 2, // TODO: Gravity, magnet rise, magic guard, iron ball, baton pass with ingrain, air balloon
        /// <summary>
        /// Pokémon that switch in will take damage.
        /// The amount of damage is based on the effectiveness of <see cref="PBEType.Rock"/> on the Pokémon.
        /// </summary>
        StealthRock = 1 << 3, // TODO: magic guard, castform transforms after taking damage
        /// <summary>
        /// Grounded Pokémon that switch in will be <see cref="PBEStatus1.Poisoned"/> if <see cref="PBETeam.ToxicSpikeCount"/> is 1 or <see cref="PBEStatus1.BadlyPoisoned"/> if it is 2.
        /// Grounded <see cref="PBEType.Poison"/> Pokémon will remove toxic spikes.
        /// </summary>
        ToxicSpikes = 1 << 4 // TODO: Gravity, immunity, leaf guard, magic guard, iron ball, baton pass with ingrain, air balloon, synchronize with roar/whirlwind
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
        None,
        AdamantOrb,
        /// <summary>
        /// No effect.
        /// </summary>
        AmuletCoin,
        /// <summary>
        /// No effect.
        /// </summary>
        Antidote,
        /// <summary>
        /// No effect.
        /// </summary>
        ArmorFossil,
        /// <summary>
        /// No effect.
        /// </summary>
        Awakening,
        /// <summary>
        /// No effect.
        /// </summary>
        BalmMushroom,
        /// <summary>
        /// No effect.
        /// </summary>
        BigMushroom,
        /// <summary>
        /// No effect.
        /// </summary>
        BigNugget,
        /// <summary>
        /// No effect.
        /// </summary>
        BigPearl,
        /// <summary>
        /// No effect.
        /// </summary>
        BlackApricorn,
        BlackBelt,
        /// <summary>
        /// No effect.
        /// </summary>
        BlackFlute,
        BlackGlasses,
        BlackSludge,
        /// <summary>
        /// No effect.
        /// </summary>
        BlueApricorn,
        /// <summary>
        /// No effect.
        /// </summary>
        BlueFlute,
        /// <summary>
        /// No effect.
        /// </summary>
        BlueScarf,
        /// <summary>
        /// No effect.
        /// </summary>
        BlueShard,
        /// <summary>
        /// No effect.
        /// </summary>
        BridgeMailD,
        /// <summary>
        /// No effect.
        /// </summary>
        BridgeMailM,
        /// <summary>
        /// No effect.
        /// </summary>
        BridgeMailS,
        /// <summary>
        /// No effect.
        /// </summary>
        BridgeMailT,
        /// <summary>
        /// No effect.
        /// </summary>
        BridgeMailV,
        BrightPowder,
        BurnDrive,
        /// <summary>
        /// No effect.
        /// </summary>
        BurnHeal,
        /// <summary>
        /// No effect.
        /// </summary>
        Calcium,
        /// <summary>
        /// No effect.
        /// </summary>
        Carbos,
        /// <summary>
        /// No effect.
        /// </summary>
        Casteliacone,
        Charcoal,
        /// <summary>
        /// No effect.
        /// </summary>
        CherishBall,
        ChillDrive,
        ChoiceBand,
        ChoiceScarf,
        ChoiceSpecs,
        /// <summary>
        /// No effect.
        /// </summary>
        ClawFossil,
        /// <summary>
        /// No effect.
        /// </summary>
        CleanseTag,
        /// <summary>
        /// No effect.
        /// </summary>
        CleverWing,
        /// <summary>
        /// No effect.
        /// </summary>
        CometShard,
        /// <summary>
        /// No effect.
        /// </summary>
        CoverFossil,
        /// <summary>
        /// No effect.
        /// </summary>
        DampMulch,
        DampRock,
        /// <summary>
        /// No effect.
        /// </summary>
        DawnStone,
        DeepSeaScale,
        DeepSeaTooth,
        /// <summary>
        /// No effect.
        /// </summary>
        DireHit,
        /// <summary>
        /// No effect.
        /// </summary>
        DiveBall,
        /// <summary>
        /// No effect.
        /// </summary>
        DomeFossil,
        DouseDrive,
        DracoPlate,
        DragonFang,
        /// <summary>
        /// No effect.
        /// </summary>
        DragonScale,
        DreadPlate,
        /// <summary>
        /// No effect.
        /// </summary>
        DreamBall,
        /// <summary>
        /// No effect.
        /// </summary>
        DubiousDisc,
        /// <summary>
        /// No effect.
        /// </summary>
        DuskBall,
        /// <summary>
        /// No effect.
        /// </summary>
        DuskStone,
        EarthPlate,
        /// <summary>
        /// No effect.
        /// </summary>
        Electirizer,
        /// <summary>
        /// No effect.
        /// </summary>
        Elixir,
        /// <summary>
        /// No effect.
        /// </summary>
        EnergyPowder,
        /// <summary>
        /// No effect.
        /// </summary>
        EnergyRoot,
        /// <summary>
        /// No effect.
        /// </summary>
        EscapeRope,
        /// <summary>
        /// No effect.
        /// </summary>
        Ether,
        /// <summary>
        /// No effect.
        /// </summary>
        Everstone,
        /// <summary>
        /// No effect.
        /// </summary>
        ExpShare,
        /// <summary>
        /// No effect.
        /// </summary>
        FastBall,
        /// <summary>
        /// No effect.
        /// </summary>
        FavoredMail,
        /// <summary>
        /// No effect.
        /// </summary>
        FireStone,
        FistPlate,
        /// <summary>
        /// The Pokémon contracts <see cref="PBEStatus1.Burned"/> at the end of each turn if it has no other <see cref="PBEStatus1"/> and it does not have <see cref="PBEType.Fire"/>.
        /// </summary>
        FlameOrb,
        FlamePlate,
        /// <summary>
        /// No effect.
        /// </summary>
        FluffyTail,
        /// <summary>
        /// No effect.
        /// </summary>
        FreshWater,
        /// <summary>
        /// No effect.
        /// </summary>
        FriendBall,
        /// <summary>
        /// No effect.
        /// </summary>
        FullHeal,
        /// <summary>
        /// No effect.
        /// </summary>
        FullRestore,
        /// <summary>
        /// No effect.
        /// </summary>
        GeniusWing,
        /// <summary>
        /// No effect.
        /// </summary>
        GooeyMulch,
        /// <summary>
        /// No effect.
        /// </summary>
        GreatBall,
        /// <summary>
        /// No effect.
        /// </summary>
        GreenApricorn,
        /// <summary>
        /// No effect.
        /// </summary>
        GreenScarf,
        /// <summary>
        /// No effect.
        /// </summary>
        GreenShard,
        /// <summary>
        /// No effect.
        /// </summary>
        GreetMail,
        GriseousOrb,
        /// <summary>
        /// No effect.
        /// </summary>
        GrowthMulch,
        /// <summary>
        /// No effect.
        /// </summary>
        GuardSpec,
        HardStone,
        /// <summary>
        /// No effect.
        /// </summary>
        HealBall,
        /// <summary>
        /// No effect.
        /// </summary>
        HealPowder,
        /// <summary>
        /// No effect.
        /// </summary>
        HealthWing,
        /// <summary>
        /// No effect.
        /// </summary>
        HeartScale,
        HeatRock,
        /// <summary>
        /// No effect.
        /// </summary>
        HeavyBall,
        /// <summary>
        /// No effect.
        /// </summary>
        HelixFossil,
        /// <summary>
        /// No effect.
        /// </summary>
        Honey,
        /// <summary>
        /// No effect.
        /// </summary>
        HPUp,
        /// <summary>
        /// No effect.
        /// </summary>
        HyperPotion,
        /// <summary>
        /// No effect.
        /// </summary>
        IceHeal,
        IciclePlate,
        IcyRock,
        /// <summary>
        /// No effect.
        /// </summary>
        InquiryMail,
        InsectPlate,
        /// <summary>
        /// No effect.
        /// </summary>
        Iron,
        IronPlate,
        /// <summary>
        /// No effect.
        /// </summary>
        LavaCookie,
        LaxIncense,
        /// <summary>
        /// No effect.
        /// </summary>
        LeafStone,
        Leftovers,
        /// <summary>
        /// No effect.
        /// </summary>
        Lemonade,
        /// <summary>
        /// No effect.
        /// </summary>
        LevelBall,
        LifeOrb, // TODO: Damage user
        LightBall,
        LightClay,
        /// <summary>
        /// No effect.
        /// </summary>
        LikeMail,
        /// <summary>
        /// No effect.
        /// </summary>
        LoveBall,
        /// <summary>
        /// No effect.
        /// </summary>
        LuckIncense,
        /// <summary>
        /// No effect.
        /// </summary>
        LuckyEgg,
        /// <summary>
        /// No effect.
        /// </summary>
        LureBall,
        LustrousOrb,
        /// <summary>
        /// No effect.
        /// </summary>
        LuxuryBall,
        MachoBrace,
        /// <summary>
        /// No effect.
        /// </summary>
        Magmarizer,
        Magnet,
        /// <summary>
        /// No effect.
        /// </summary>
        MasterBall,
        /// <summary>
        /// No effect.
        /// </summary>
        MaxElixir,
        /// <summary>
        /// No effect.
        /// </summary>
        MaxEther,
        /// <summary>
        /// No effect.
        /// </summary>
        MaxPotion,
        /// <summary>
        /// No effect.
        /// </summary>
        MaxRepel,
        /// <summary>
        /// No effect.
        /// </summary>
        MaxRevive,
        MeadowPlate,
        MetalCoat,
        MetalPowder,
        MindPlate,
        MiracleSeed,
        /// <summary>
        /// No effect.
        /// </summary>
        MoomooMilk,
        /// <summary>
        /// No effect.
        /// </summary>
        MoonBall,
        /// <summary>
        /// No effect.
        /// </summary>
        MoonStone,
        MuscleBand,
        /// <summary>
        /// No effect.
        /// </summary>
        MuscleWing,
        MysticWater,
        /// <summary>
        /// No effect.
        /// </summary>
        NestBall,
        /// <summary>
        /// No effect.
        /// </summary>
        NetBall,
        NeverMeltIce,
        /// <summary>
        /// No effect.
        /// </summary>
        Nugget,
        OddIncense,
        /// <summary>
        /// No effect.
        /// </summary>
        OddKeystone,
        /// <summary>
        /// No effect.
        /// </summary>
        OldAmber,
        /// <summary>
        /// No effect.
        /// </summary>
        OldGateau,
        /// <summary>
        /// No effect.
        /// </summary>
        OvalStone,
        /// <summary>
        /// No effect.
        /// </summary>
        ParalyzeHeal,
        /// <summary>
        /// No effect.
        /// </summary>
        ParkBall,
        /// <summary>
        /// No effect.
        /// </summary>
        PassOrb,
        /// <summary>
        /// No effect.
        /// </summary>
        Pearl,
        /// <summary>
        /// No effect.
        /// </summary>
        PearlString,
        /// <summary>
        /// No effect.
        /// </summary>
        PinkApricorn,
        /// <summary>
        /// No effect.
        /// </summary>
        PinkScarf,
        /// <summary>
        /// No effect.
        /// </summary>
        PlumeFossil,
        PoisonBarb,
        /// <summary>
        /// No effect.
        /// </summary>
        PokeBall,
        /// <summary>
        /// No effect.
        /// </summary>
        PokeDoll,
        /// <summary>
        /// No effect.
        /// </summary>
        PokeToy,
        /// <summary>
        /// No effect.
        /// </summary>
        Potion,
        PowerAnklet,
        PowerBand,
        PowerBelt,
        PowerBracer,
        PowerHerb,
        PowerLens,
        PowerWeight,
        /// <summary>
        /// No effect.
        /// </summary>
        PPMax,
        /// <summary>
        /// No effect.
        /// </summary>
        PPUp,
        /// <summary>
        /// No effect.
        /// </summary>
        PremierBall,
        /// <summary>
        /// No effect.
        /// </summary>
        PrettyWing,
        /// <summary>
        /// No effect.
        /// </summary>
        PrismScale,
        /// <summary>
        /// No effect.
        /// </summary>
        Protector,
        /// <summary>
        /// No effect.
        /// </summary>
        Protein,
        /// <summary>
        /// No effect.
        /// </summary>
        PureIncense,
        /// <summary>
        /// No effect.
        /// </summary>
        QuickBall,
        QuickPowder,
        /// <summary>
        /// No effect.
        /// </summary>
        RageCandyBar,
        /// <summary>
        /// No effect.
        /// </summary>
        RareBone,
        /// <summary>
        /// No effect.
        /// </summary>
        RareCandy,
        RazorClaw,
        /// <summary>
        /// No effect.
        /// </summary>
        ReaperCloth,
        /// <summary>
        /// No effect.
        /// </summary>
        RedApricorn,
        /// <summary>
        /// No effect.
        /// </summary>
        RedFlute,
        /// <summary>
        /// No effect.
        /// </summary>
        RedScarf,
        /// <summary>
        /// No effect.
        /// </summary>
        RedShard,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicBand,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicCopper,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicCrown,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicGold,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicSilver,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicStatue,
        /// <summary>
        /// No effect.
        /// </summary>
        RelicVase,
        /// <summary>
        /// No effect.
        /// </summary>
        RepeatBall,
        /// <summary>
        /// No effect.
        /// </summary>
        Repel,
        /// <summary>
        /// No effect.
        /// </summary>
        ReplyMail,
        /// <summary>
        /// No effect.
        /// </summary>
        ResistWing,
        /// <summary>
        /// No effect.
        /// </summary>
        RevivalHerb,
        /// <summary>
        /// No effect.
        /// </summary>
        Revive,
        RockIncense,
        /// <summary>
        /// No effect.
        /// </summary>
        RootFossil,
        RoseIncense,
        /// <summary>
        /// No effect.
        /// </summary>
        RSVPMail,
        /// <summary>
        /// No effect.
        /// </summary>
        SacredAsh,
        /// <summary>
        /// No effect.
        /// </summary>
        SafariBall,
        ScopeLens,
        SeaIncense,
        SharpBeak,
        /// <summary>
        /// No effect.
        /// </summary>
        ShinyStone,
        /// <summary>
        /// No effect.
        /// </summary>
        ShoalSalt,
        /// <summary>
        /// No effect.
        /// </summary>
        ShoalShell,
        ShockDrive,
        SilkScarf,
        SilverPowder,
        /// <summary>
        /// No effect.
        /// </summary>
        SkullFossil,
        SkyPlate,
        /// <summary>
        /// No effect.
        /// </summary>
        SmokeBall,
        SmoothRock,
        /// <summary>
        /// No effect.
        /// </summary>
        SodaPop,
        SoftSand,
        /// <summary>
        /// No effect.
        /// </summary>
        SootheBell,
        SoulDew,
        SpellTag,
        SplashPlate,
        SpookyPlate,
        /// <summary>
        /// No effect.
        /// </summary>
        SportBall,
        /// <summary>
        /// No effect.
        /// </summary>
        StableMulch,
        /// <summary>
        /// No effect.
        /// </summary>
        Stardust,
        /// <summary>
        /// No effect.
        /// </summary>
        StarPiece,
        Stick,
        StonePlate,
        /// <summary>
        /// No effect.
        /// </summary>
        SunStone,
        /// <summary>
        /// No effect.
        /// </summary>
        SuperPotion,
        /// <summary>
        /// No effect.
        /// </summary>
        SuperRepel,
        /// <summary>
        /// No effect.
        /// </summary>
        SweetHeart,
        /// <summary>
        /// No effect.
        /// </summary>
        SwiftWing,
        /// <summary>
        /// No effect.
        /// </summary>
        ThanksMail,
        ThickClub,
        /// <summary>
        /// No effect.
        /// </summary>
        ThunderStone,
        /// <summary>
        /// No effect.
        /// </summary>
        TimerBall,
        /// <summary>
        /// No effect.
        /// </summary>
        TinyMushroom,
        /// <summary>
        /// The Pokémon contracts <see cref="PBEStatus1.BadlyPoisoned"/> at the end of each turn if it has no other <see cref="PBEStatus1"/> and it does not have <see cref="PBEType.Poison"/> or <see cref="PBEType.Steel"/>.
        /// </summary>
        ToxicOrb,
        ToxicPlate,
        TwistedSpoon,
        /// <summary>
        /// No effect.
        /// </summary>
        UltraBall,
        UpGrade,
        /// <summary>
        /// No effect.
        /// </summary>
        WaterStone,
        WaveIncense,
        /// <summary>
        /// No effect.
        /// </summary>
        WhiteApricorn,
        /// <summary>
        /// No effect.
        /// </summary>
        WhiteFlute,
        WideLens,
        WiseGlasses,
        /// <summary>
        /// No effect.
        /// </summary>
        XAccuracy,
        /// <summary>
        /// No effect.
        /// </summary>
        XAttack,
        /// <summary>
        /// No effect.
        /// </summary>
        XDefense,
        /// <summary>
        /// No effect.
        /// </summary>
        XSpAtk,
        /// <summary>
        /// No effect.
        /// </summary>
        XSpDef,
        /// <summary>
        /// No effect.
        /// </summary>
        XSpeed,
        /// <summary>
        /// No effect.
        /// </summary>
        YellowApricorn,
        /// <summary>
        /// No effect.
        /// </summary>
        YellowFlute,
        /// <summary>
        /// No effect.
        /// </summary>
        YellowScarf,
        /// <summary>
        /// No effect.
        /// </summary>
        YellowShard,
        ZapPlate,
        /// <summary>
        /// No effect.
        /// </summary>
        Zinc,
        /// <summary>
        /// Invalid item.
        /// </summary>
        MAX
    }
    /// <summary>
    /// Represents a specific Pokémon's special ability.
    /// </summary>
    public enum PBEAbility : byte
    {
        /// <summary>
        /// The Pokémon's ability was removed.
        /// </summary>
        None,
        /// <summary>
        /// The Pokémon has a stronger same-type-attack-bonus.
        /// </summary>
        Adaptability,
        Aftermath, // TODO
        AirLock, // TODO
        Analytic, // TODO
        AngerPoint, // TODO
        Anticipation, // TODO
        ArenaTrap, // TODO
        BadDreams, // TODO
        /// <summary>
        /// The Pokémon takes no critical hits.
        /// </summary>
        BattleArmor,
        BigPecks, // TODO
        /// <summary>
        /// When the Pokémon has low HP, its Fire-type moves get a power boost.
        /// </summary>
        Blaze,
        /// <summary>
        /// The Pokémon gets a speed boost in harsh sunlight.
        /// </summary>
        Chlorophyll,
        ClearBody, // TODO
        CloudNine, // TODO
        ColorChange, // TODO
        /// <summary>
        /// The Pokémon accuracy is boosted.
        /// </summary>
        Compoundeyes,
        Contrary, // TODO
        CursedBody, // TODO
        CuteCharm, // TODO
        Damp, // TODO
        Defeatist, // TODO
        Defiant, // TODO
        Download, // TODO
        /// <summary>
        /// The Pokémon changes the weather to infinite rain.
        /// </summary>
        Drizzle,
        /// <summary>
        /// The Pokémon changes the weather to infinite harsh sunlight.
        /// </summary>
        Drought,
        DrySkin, // TODO
        EarlyBird, // TODO
        EffectSpore, // TODO
        /// <summary>
        /// The Pokémon takes less damage from incoming super-effective moves.
        /// </summary>
        Filter,
        FlameBody, // TODO
        FlareBoost, // TODO
        FlashFire, // TODO
        FlowerGift, // TODO
        Forecast, // TODO
        Forewarn, // TODO
        FriendGuard, // TODO
        Frisk, // TODO
        Gluttony, // TODO
        /// <summary>
        /// The Pokémon's attack is boosted when it is afflicted with a <see cref="PBEStatus1"/>, and the damage reduction from <see cref="PBEStatus1.Burned"/> is not applied.
        /// </summary>
        Guts,
        Harvest, // TODO
        Healer, // TODO
        /// <summary>
        /// The Pokémon takes less damage from Fire-type moves and from a burn.
        /// </summary>
        Heatproof,
        HeavyMetal, // TODO
        /// <summary>
        /// No effect in battle.
        /// </summary>
        HoneyGather,
        /// <summary>
        /// The Pokémon's attack is boosted.
        /// </summary>
        HugePower,
        /// <summary>
        /// The Pokémon's attack is boosted, but its accuracy is lower for physical moves.
        /// </summary>
        Hustle,
        Hydration, // TODO
        HyperCutter, // TODO
        /// <summary>
        /// In a hailstorm, the Pokémon takes no damage from the hailstorm and restores HP at the end of each turn.
        /// </summary>
        IceBody,
        /// <summary>
        /// No effect in battle.
        /// </summary>
        Illuminate,
        Illusion, // TODO
        Immunity, // TODO
        /// <summary>
        /// The Pokémon transforms into the foe across from it when switching in.
        /// </summary>
        Imposter,
        Infiltrator, // TODO
        InnerFocus, // TODO
        Insomnia, // TODO
        Intimidate, // TODO
        IronBarbs, // TODO
        IronFist, // TODO
        Justified, // TODO
        KeenEye, // TODO
        Klutz, // TODO
        LeafGuard, // TODO
        /// <summary>
        /// The Pokémon is immune to Ground-type moves and most entry hazards.
        /// </summary>
        Levitate,
        LightMetal, // TODO
        Lightningrod, // TODO
        /// <summary>
        /// The Pokémon cannot be paralyzed.
        /// </summary>
        Limber,
        LiquidOoze, // TODO
        MagicBounce, // TODO
        MagicGuard, // TODO
        MagmaArmor, // TODO
        MagnetPull, // TODO
        /// <summary>
        /// The Pokémon's defense is boosted when it is afflicted with a <see cref="PBEStatus1"/>.
        /// </summary>
        MarvelScale,
        Minus, // TODO
        MoldBreaker, // TODO
        Moody, // TODO
        MotorDrive, // TODO
        Moxie, // TODO
        Multiscale, // TODO
        /// <summary>
        /// No effect in battle.
        /// </summary>
        Multitype,
        Mummy, // TODO
        NaturalCure, // TODO
        /// <summary>
        /// The Pokémon will always hit and always get hit unless protection is used.
        /// </summary>
        NoGuard,
        Normalize, // TODO
        Oblivious, // TODO
        /// <summary>
        /// The Pokémon takes no damage from a hailstorm or sandstorm.
        /// </summary>
        Overcoat,
        /// <summary>
        /// When the Pokémon has low HP, its Grass-type moves get a power boost.
        /// </summary>
        Overgrow,
        OwnTempo, // TODO
        Pickpocket, // TODO
        Pickup, // TODO
        Plus, // TODO
        PoisonHeal, // TODO
        PoisonPoint, // TODO
        PoisonTouch, // TODO
        Prankster, // TODO
        Pressure, // TODO
        /// <summary>
        /// The Pokémon's attack is boosted.
        /// </summary>
        PurePower,
        QuickFeet, // TODO
        /// <summary>
        /// In rain, the Pokémon restores HP at the end of each turn.
        /// </summary>
        RainDish,
        Rattled, // TODO
        Reckless, // TODO
        Regenerator, // TODO
        Rivalry, // TODO
        RockHead, // TODO
        RoughSkin, // TODO
        /// <summary>
        /// No effect in battle.
        /// </summary>
        RunAway,
        /// <summary>
        /// In a sandstorm, the Pokémon takes no damage from the sandstorm and its Rock-, Ground-, and Steel-type moves get a power boost.
        /// </summary>
        SandForce,
        /// <summary>
        /// The Pokémon gets a speed boost in a sandstorm.
        /// </summary>
        SandRush,
        /// <summary>
        /// The Pokémon changes the weather to an infinite sandstorm.
        /// </summary>
        SandStream,
        /// <summary>
        /// In a sandstorm, the Pokémon takes no damage from the sandstorm and gets a 20% evasion boost.
        /// </summary>
        SandVeil,
        SapSipper, // TODO
        Scrappy, // TODO
        SereneGrace, // TODO
        ShadowTag, // TODO
        ShedSkin, // TODO
        SheerForce, // TODO
        /// <summary>
        /// The Pokémon takes no critical hits.
        /// </summary>
        ShellArmor,
        ShieldDust, // TODO
        /// <summary>
        /// The Pokémon's stat changes are doubled.
        /// </summary>
        Simple,
        SkillLink, // TODO
        SlowStart, // TODO
        /// <summary>
        /// The Pokémon deals more damage when landing critical hits.
        /// </summary>
        Sniper,
        /// <summary>
        /// In a hailstorm, the Pokémon takes no damage from the hailstorm and gets a 20% evasion boost.
        /// </summary>
        SnowCloak,
        /// <summary>
        /// The Pokémon changes the weather to an infinite hailstorm.
        /// </summary>
        SnowWarning,
        /// <summary>
        /// In harsh sunlight, the Pokémon gets a special attack boost and takes damage at the end of each turn.
        /// </summary>
        SolarPower,
        /// <summary>
        /// The Pokémon takes less damage from incoming super-effective moves.
        /// </summary>
        SolidRock,
        Soundproof, // TODO
        SpeedBoost, // TODO
        Stall, // TODO
        Static, // TODO
        Steadfast, // TODO
        Stench, // TODO
        StickyHold, // TODO
        StormDrain, // TODO
        Sturdy, // TODO
        SuctionCups, // TODO
        /// <summary>
        /// The Pokémon is more likely to land critical hits.
        /// </summary>
        SuperLuck,
        /// <summary>
        /// When the Pokémon has low HP, its Bug-type moves get a power boost.
        /// </summary>
        Swarm,
        /// <summary>
        /// The Pokémon gets a speed boost in rain.
        /// </summary>
        SwiftSwim,
        Synchronize, // TODO
        TangledFeet, // TODO
        Technician, // TODO
        Telepathy, // TODO
        Teravolt, // TODO
        /// <summary>
        /// The Pokémon takes less damage from Ice- and Fire-type moves.
        /// </summary>
        ThickFat,
        /// <summary>
        /// The Pokémon deals double damage with outgoing not-very-effective moves.
        /// </summary>
        TintedLens,
        /// <summary>
        /// When the Pokémon has low HP, its Water-type moves get a power boost.
        /// </summary>
        Torrent,
        ToxicBoost, // TODO
        Trace, // TODO
        Truant, // TODO
        Turboblaze, // TODO
        Unaware, // TODO
        Unburden, // TODO
        Unnerve, // TODO
        VictoryStar, // TODO
        VitalSpirit, // TODO
        VoltAbsorb, // TODO
        WaterAbsorb, // TODO
        WaterVeil, // TODO
        WeakArmor, // TODO
        WhiteSmoke, // TODO
        /// <summary>
        /// The Pokémon is immune to all damaging moves except for moves that would deal super-effective damage.
        /// </summary>
        WonderGuard, // TODO: Mold Breaker, Teravolt, Turboblaze, Role Play, Skill Swap
        WonderSkin, // TODO
        ZenMode, // TODO
        /// <summary>
        /// Invalid ability.
        /// </summary>
        MAX,
    }
    /// <summary>
    /// Represents a specific Pokémon species.
    /// </summary>
    public enum PBESpecies : uint
    {
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
        Vulpix = 37,
        Ninetales = 38,
        Farfetchd = 83,
        Cubone = 104,
        Marowak = 105,
        Ditto = 132,
        Eevee = 133,
        Vaporeon = 134,
        Jolteon = 135,
        Flareon = 136,
        Crobat = 169,
        Pichu = 172,
        Azumarill = 184,
        Espeon = 196,
        Umbreon = 197,
        Misdreavus = 200,
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
        Smeargle = 235,
        Nincada = 290,
        Ninjask = 291,
        Shedinja = 292,
        Skitty = 300,
        Delcatty = 301,
        Lileep = 345,
        Cradily = 346,
        Absol = 359,
        Clamperl = 366,
        Latias = 380,
        Latios = 381,
        Jirachi = 385,
        Shinx = 403,
        Luxio = 404,
        Luxray = 405,
        Mismagius = 429,
        Gible = 443,
        Gabite = 444,
        Garchomp = 445,
        Riolu = 447,
        Lucario = 448,
        Leafeon = 470,
        Glaceon = 471,
        Rotom = 479 | (0 << 0x10),
        Rotom_Fan = 479 | (1 << 0x10),
        Rotom_Frost = 479 | (2 << 0x10),
        Rotom_Heat = 479 | (3 << 0x10),
        Rotom_Mow = 479 | (4 << 0x10),
        Rotom_Wash = 479 | (5 << 0x10),
        Uxie = 480,
        Mesprit = 481,
        Azelf = 482,
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
        Tirtouga = 564,
        Carracosta = 565,
        Gothita = 574,
        Gothorita = 575,
        Gothitelle = 576,
        Litwick = 607,
        Lampent = 608,
        Chandelure = 609,
        Cryogonal = 615,
        Druddigon = 621,
        Deino = 633,
        Zweilous = 634,
        Hydreigon = 635,
        Larvesta = 636,
        Volcarona = 637,
        Genesect = 649 | (0 << 0x10),
        Genesect_Burn = 649 | (1 << 0x10),
        Genesect_Chill = 649 | (2 << 0x10),
        Genesect_Douse = 649 | (3 << 0x10),
        Genesect_Shock = 649 | (4 << 0x10)
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
    [Flags]
    public enum PBEMoveFlag : ushort
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
        Substitute,
        SunnyDay,
        Swagger,
        Toxic,
        ToxicSpikes,
        Transform
    }
    public enum PBEMove : ushort
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
        Flatter,
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
        Hail,
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
        Sandstorm,
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
        Swagger,
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
        /// <summary>
        /// Invalid move.
        /// </summary>
        MAX
    }
}
