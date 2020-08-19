using Kermalis.PokemonBattleEngine.Data;
using System;

namespace Kermalis.PokemonBattleEngine.Battle
{
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
    public enum PBEBattleType : byte
    {
        Trainer,
        Wild
    }
    /// <summary>Represents the current state of a specific battle.</summary>
    public enum PBEBattleState : byte
    {
        /// <summary>The battle is waiting for teams.</summary>
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
        /// <summary>The battle is ready to run switches.</summary>
        ReadyToRunSwitches,
        /// <summary>The battle ended.</summary>
        Ended
    }
    /// <summary>Represents the result of an ended battle.</summary>
    public enum PBEBattleResult : byte
    {
        /// <summary>Team 0 forfeited.</summary>
        Team0Forfeit,
        /// <summary>Team 0 defeated Team 1.</summary>
        Team0Win,
        /// <summary>Team 1 forfeited.</summary>
        Team1Forfeit,
        /// <summary>Team 1 defeated Team 0.</summary>
        Team1Win,
        /// <summary>A wild Pokémon was captured.</summary>
        WildCapture,
        /// <summary>The trainer(s) escaped from the wild Pokémon.</summary>
        WildEscape,
        /// <summary>A wild Pokémon escaped from the trainer(s).</summary>
        WildFlee
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
        Sandstorm,
        MAX
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
        Right,
        MAX
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
        Poisoned,
        MAX
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
        /// <summary>The Pokémon is disguised as <see cref="PBEBattlePokemon.DisguisedAsPokemon"/> with <see cref="PBEAbility.Illusion"/>.</summary>
        Disguised = 1 << 3,
        /// <summary>The Pokémon is flinching and will be unable to move this turn.</summary>
        Flinching = 1 << 4,
        /// <summary>The Pokémon will gain a power boost due to <see cref="PBEMoveEffect.HelpingHand"/>.</summary>
        HelpingHand = 1 << 5,
        Identified = 1 << 6,
        /// <summary>The Pokémon is infatuated with <see cref="PBEBattlePokemon.InfatuatedWithPokemon"/> and may be unable to move this turn.</summary>
        Infatuated = 1 << 7,
        /// <summary>The Pokémon is seeded and HP will be stolen at the end of each turn.</summary>
        LeechSeed = 1 << 8,
        LockOn = 1 << 9,
        MagnetRise = 1 << 10,
        MiracleEye = 1 << 11,
        Nightmare = 1 << 12,
        /// <summary>The Pokémon's <see cref="PBEStat.Attack"/> and <see cref="PBEStat.Defense"/> are switched.</summary>
        PowerTrick = 1 << 13,
        /// <summary>The Pokémon is protected from moves this turn.</summary>
        Protected = 1 << 14,
        /// <summary>The Pokémon is under the effect of <see cref="PBEMoveEffect.FocusEnergy"/> or <see cref="PBEItem.LansatBerry"/> and has a higher chance of landing critical hits.</summary>
        Pumped = 1 << 15,
        Roost = 1 << 16,
        ShadowForce = 1 << 17,
        /// <summary>The Pokémon is behind a substitute that will take damage on behalf of the Pokémon and prevent most moves from affecting the Pokémon.</summary>
        Substitute = 1 << 18,
        /// <summary>The Pokémon is transformed into another Pokémon.</summary>
        Transformed = 1 << 19,
        /// <summary>The Pokémon is underground. A move will miss against the Pokémon unless it has <see cref="PBEMoveFlag.HitsUnderground"/> or either Pokémon has <see cref="PBEAbility.NoGuard"/>.</summary>
        Underground = 1 << 20,
        /// <summary>The Pokémon is underwater. A move will miss against the Pokémon unless it has <see cref="PBEMoveFlag.HitsUnderwater"/> or either Pokémon has <see cref="PBEAbility.NoGuard"/>.</summary>
        Underwater = 1 << 21
    }
    /// <summary>Keeps track of which types <see cref="PBEStatus2.Roost"/> changes to/from <see cref="PBEType.Flying"/>.</summary>
    [Flags]
    public enum PBERoostTypes : byte
    {
        None,
        KnownType1 = 1 << 0,
        KnownType2 = 1 << 1,
        Type1 = 1 << 2,
        Type2 = 1 << 3
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
        QuickGuard = 1 << 2,
        /// <summary>The team will take less damage from <see cref="PBEMoveCategory.Physical"/> moves.</summary>
        Reflect = 1 << 3,
        Safeguard = 1 << 4,
        /// <summary>Grounded Pokémon that switch in will take damage. The amount of damage is based on <see cref="PBETeam.SpikeCount"/>. </summary>
        Spikes = 1 << 5, // TODO: Gravity, magic guard, iron ball, baton pass with ingrain, air balloon
        /// <summary>Pokémon that switch in will take damage. The amount of damage is based on the effectiveness of <see cref="PBEType.Rock"/> on the Pokémon. </summary>
        StealthRock = 1 << 6, // TODO: magic guard
        Tailwind = 1 << 7,
        /// <summary>Grounded Pokémon that switch in will be <see cref="PBEStatus1.Poisoned"/> if <see cref="PBETeam.ToxicSpikeCount"/> is 1 or <see cref="PBEStatus1.BadlyPoisoned"/> if it is 2.
        /// Grounded <see cref="PBEType.Poison"/> Pokémon will remove toxic spikes.</summary>
        ToxicSpikes = 1 << 8, // TODO: Gravity, magic guard, iron ball, baton pass with ingrain, air balloon, synchronize with roar/whirlwind
        /// <summary>The team is protected from spread moves for a turn.</summary>
        WideGuard = 1 << 9
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
        /// <summary>The status was added to a Pokémon.</summary>
        /// <example>The Pokémon became <see cref="PBEStatus1.Paralyzed"/>.</example>
        Added = 0,
        /// <summary>The status activated its main effect.</summary>
        /// <example><see cref="PBEStatus2.Infatuated"/> states the Pokémon is in love.</example>
        Announced = 1,
        /// <summary>The status caused a Pokémon to be immobile.</summary>
        /// <example><see cref="PBEStatus2.Flinching"/> prevented movement.</example>
        CausedImmobility = 2,
        /// <summary>The status was cured from a Pokémon.</summary>
        /// <example><see cref="PBEAbility.Limber"/> cured a Pokémon of <see cref="PBEStatus1.Paralyzed"/>.</example>
        Cleared = 3,
        /// <summary>The status was involved with damage.</summary>
        /// <example>A Pokémon's <see cref="PBEStatus2.Substitute"/> took damage.</example>
        Damage = 4,
        /// <summary>The status has ended naturally.</summary>
        /// <example>A Pokémon with <see cref="PBEStatus2.Confused"/> regained its senses.</example>
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
        /// <example>A team set up <see cref="PBETeamStatus.LightScreen"/>.</example>
        Added = 0,
        /// <summary>The status was forcefully removed from a team.</summary>
        /// <example>A Pokémon used <see cref="PBEMoveEffect.BrickBreak"/> and destroyed <see cref="PBETeamStatus.Reflect"/>.</example>
        Cleared = 1,
        /// <summary>The status caused a Pokémon to take damage.</summary>
        /// <example>A Pokémon switched in and took damage from <see cref="PBETeamStatus.StealthRock"/>.</example>
        Damage = 2,
        /// <summary>The status ended naturally.</summary>
        /// <example><see cref="PBETeamStatus.LightScreen"/> wore off.</example>
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
        /// <summary>Failure due to accuracy and/or evasion.</summary>
        Missed = 11,
        /// <summary>Failure due to having no available targets.</summary>
        NoTarget = 12,
        /// <summary>Limited success due to a Pokémon's <see cref="PBEType"/>.</summary>
        NotVeryEffective_Type = 13,
        /// <summary>Great success due to a Pokémon's <see cref="PBEType"/>.</summary>
        SuperEffective_Type = 14
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
        PayDay = 8,
        Recoil = 9,
        Struggle = 10
    }
    public enum PBEMoveLockType : byte
    {
        ChoiceItem = 0,
        Temporary = 1
    }
}
