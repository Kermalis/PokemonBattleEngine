using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Data
{
    // TODO: .Equals, ability to make read-only
    /// <summary>The various engine settings.</summary>
    public sealed class PBESettings : INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        /// <summary>Fires whenever a property changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>The default settings used in official games.</summary>
        public static PBESettings DefaultSettings { get; } = new PBESettings();

        /// <summary>The default value of <see cref="MaxLevel"/>.</summary>
        public const byte DefaultMaxLevel = 100;
        private byte maxLevel = DefaultMaxLevel;
        /// <summary>The maximum level a Pokémon can be. Used in stat calculation.</summary>
        public byte MaxLevel
        {
            get => maxLevel;
            set
            {
                if (maxLevel != value)
                {
                    if (value < minLevel)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxLevel)} must be at least {nameof(MinLevel)} ({minLevel}).");
                    }
                    maxLevel = value;
                    OnPropertyChanged(nameof(MaxLevel));
                }
            }
        }
        /// <summary>The default value of <see cref="MinLevel"/>.</summary>
        public const byte DefaultMinLevel = 1;
        private byte minLevel = DefaultMinLevel;
        /// <summary>The minimum level a Pokémon can be.</summary>
        public byte MinLevel
        {
            get => minLevel;
            set
            {
                if (minLevel != value)
                {
                    if (value < 1 || value > maxLevel)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MinLevel)} must be at least 1 and cannot exceed {nameof(MaxLevel)} ({maxLevel}).");
                    }
                    minLevel = value;
                    OnPropertyChanged(nameof(MinLevel));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxPartySize"/>.</summary>
        public const sbyte DefaultMaxPartySize = 6;
        private sbyte maxPartySize = DefaultMaxPartySize;
        /// <summary>The maximum amount of Pokémon each team can bring into a battle.</summary>
        public sbyte MaxPartySize
        {
            get => maxPartySize;
            set
            {
                if (maxPartySize != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxPartySize)} must be at least 1.");
                    }
                    maxPartySize = value;
                    OnPropertyChanged(nameof(MaxPartySize));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxPokemonNameLength"/>.</summary>
        public const byte DefaultMaxPokemonNameLength = 10;
        private byte maxPokemonNameLength = DefaultMaxPokemonNameLength;
        /// <summary>The maximum amount of characters a Pokémon nickname can have.</summary>
        public byte MaxPokemonNameLength
        {
            get => maxPokemonNameLength;
            set
            {
                if (maxPokemonNameLength != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxPokemonNameLength)} must be at least 1.");
                    }
                    maxPokemonNameLength = value;
                    OnPropertyChanged(nameof(MaxPokemonNameLength));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxTrainerNameLength"/>. This value is different in non-English games.</summary>
        public const byte DefaultMaxTrainerNameLength = 7;
        private byte maxTrainerNameLength = DefaultMaxTrainerNameLength;
        /// <summary>The maximum amount of characters a trainer's name can have.</summary>
        [Obsolete("Currently not used anywhere")]
        public byte MaxTrainerNameLength
        {
            get => maxTrainerNameLength;
            set
            {
                if (maxTrainerNameLength != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxTrainerNameLength)} must be at least 1.");
                    }
                    maxTrainerNameLength = value;
                    OnPropertyChanged(nameof(MaxTrainerNameLength));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxTotalEVs"/>.</summary>
        public const ushort DefaultMaxTotalEVs = 510;
        private ushort maxTotalEVs = DefaultMaxTotalEVs;
        /// <summary>The maximum sum of a Pokémon's EVs.</summary>
        public ushort MaxTotalEVs
        {
            get => maxTotalEVs;
            set
            {
                const int max = byte.MaxValue * 6;
                if (maxTotalEVs != value)
                {
                    if (value > max)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxTotalEVs)} must not exceed {max}.");
                    }
                    maxTotalEVs = value;
                    OnPropertyChanged(nameof(MaxTotalEVs));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxIVs"/>.</summary>
        public const byte DefaultMaxIVs = 31;
        private byte maxIVs = DefaultMaxIVs;
        /// <summary>The maximum amount of IVs Pokémon can have in each stat. Raising this will not affect <see cref="PBEMove.HiddenPower"/>.</summary>
        public byte MaxIVs
        {
            get => maxIVs;
            set
            {
                if (maxIVs != value)
                {
                    maxIVs = value;
                    OnPropertyChanged(nameof(MaxIVs));
                }
            }
        }
        /// <summary>The default value of <see cref="NatureStatBoost"/>.</summary>
        public const double DefaultNatureStatBoost = 0.1;
        private double natureStatBoost = DefaultNatureStatBoost;
        /// <summary>The amount of influence a Pokémon's <see cref="PBENature"/> has on its stats.</summary>
        public double NatureStatBoost
        {
            get => natureStatBoost;
            set
            {
                if (natureStatBoost != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(NatureStatBoost)} must be at least 0.");
                    }
                    natureStatBoost = value;
                    OnPropertyChanged(nameof(NatureStatBoost));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxStatChange"/>.</summary>
        public const sbyte DefaultMaxStatChange = 6;
        private sbyte maxStatChange = DefaultMaxStatChange;
        /// <summary>The maximum change a stat can have in the negative and positive direction.</summary>
        public sbyte MaxStatChange
        {
            get => maxStatChange;
            set
            {
                if (maxStatChange != value)
                {
                    maxStatChange = value;
                    OnPropertyChanged(nameof(MaxStatChange));
                }
            }
        }
        /// <summary>The default value of <see cref="NumMoves"/>.</summary>
        public const byte DefaultNumMoves = 4;
        private byte numMoves = DefaultNumMoves;
        /// <summary>The maximum amount of moves a specific Pokémon can remember at once.</summary>
        public byte NumMoves
        {
            get => numMoves;
            set
            {
                if (numMoves != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(NumMoves)} must be at least 1.");
                    }
                    numMoves = value;
                    OnPropertyChanged(nameof(NumMoves));
                }
            }
        }
        /// <summary>The default value of <see cref="PPMultiplier"/>.</summary>
        public const byte DefaultPPMultiplier = 5;
        private byte ppMultiplier = DefaultPPMultiplier;
        /// <summary>This affects the base PP of each move and the boost PP-Ups give.</summary>
        /// <remarks>
        /// <para>Growl is a tier 8 move, so the maximum PP will be 64. The formula: Max(1, ((tier * PPMultiplier) + (tier * PPUps))).</para>
        /// <para><see cref="PBEMove.Transform"/> will change the max PP of each copied move to PPMultiplier (1 if the move is tier 0).</para>
        /// </remarks>
        public byte PPMultiplier
        {
            get => ppMultiplier;
            set
            {
                if (ppMultiplier != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(PPMultiplier)} must be at least 1.");
                    }
                    ppMultiplier = value;
                    OnPropertyChanged(nameof(PPMultiplier));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxPPUps"/>.</summary>
        public const byte DefaultMaxPPUps = 3;
        private byte maxPPUps = DefaultMaxPPUps;
        /// <summary>The maximum amount of PP-Ups that can be used on each of a Pokémon's moves.</summary>
        public byte MaxPPUps
        {
            get => maxPPUps;
            set
            {
                if (maxPPUps != value)
                {
                    maxPPUps = value;
                    OnPropertyChanged(nameof(MaxPPUps));
                }
            }
        }
        /// <summary>The default value of <see cref="CritMultiplier"/>.</summary>
        public const double DefaultCritMultiplier = 2.0;
        private double critMultiplier = DefaultCritMultiplier;
        /// <summary>The damage boost awarded by critical hits.</summary>
        public double CritMultiplier
        {
            get => critMultiplier;
            set
            {
                if (critMultiplier != value)
                {
                    critMultiplier = value;
                    OnPropertyChanged(nameof(CritMultiplier));
                }
            }
        }
        /// <summary>The default value of <see cref="ConfusionMaxTurns"/>.</summary>
        public const byte DefaultConfusionMaxTurns = 4;
        private byte confusionMaxTurns = DefaultConfusionMaxTurns;
        /// <summary>The maximum amount of turns a Pokémon can be <see cref="PBEStatus2.Confused"/>.</summary>
        public byte ConfusionMaxTurns
        {
            get => confusionMaxTurns;
            set
            {
                if (confusionMaxTurns != value)
                {
                    if (value < confusionMinTurns)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ConfusionMaxTurns)} must be at least {nameof(ConfusionMinTurns)} ({confusionMinTurns}).");
                    }
                    confusionMaxTurns = value;
                    OnPropertyChanged(nameof(ConfusionMaxTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="ConfusionMinTurns"/>.</summary>
        public const byte DefaultConfusionMinTurns = 1;
        private byte confusionMinTurns = DefaultConfusionMinTurns;
        /// <summary>The minimum amount of turns a Pokémon can be <see cref="PBEStatus2.Confused"/>.</summary>
        public byte ConfusionMinTurns
        {
            get => confusionMinTurns;
            set
            {
                if (confusionMinTurns != value)
                {
                    if (value > confusionMaxTurns)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ConfusionMinTurns)} cannot exceed {nameof(ConfusionMaxTurns)} ({confusionMaxTurns}).");
                    }
                    confusionMinTurns = value;
                    OnPropertyChanged(nameof(ConfusionMinTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="SleepMaxTurns"/>.</summary>
        public const byte DefaultSleepMaxTurns = 3;
        private byte sleepMaxTurns = DefaultSleepMaxTurns;
        /// <summary>The maximum amount of turns a Pokémon can be <see cref="PBEStatus1.Asleep"/>.</summary>
        public byte SleepMaxTurns
        {
            get => sleepMaxTurns;
            set
            {
                if (sleepMaxTurns != value)
                {
                    if (value < sleepMinTurns)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(SleepMaxTurns)} must be at least {nameof(SleepMinTurns)} ({sleepMinTurns}).");
                    }
                    sleepMaxTurns = value;
                    OnPropertyChanged(nameof(SleepMaxTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="SleepMinTurns"/>.</summary>
        public const byte DefaultSleepMinTurns = 1;
        private byte sleepMinTurns = DefaultSleepMinTurns;
        /// <summary>The minimum amount of turns a Pokémon can be <see cref="PBEStatus1.Asleep"/>.</summary>
        public byte SleepMinTurns
        {
            get => sleepMinTurns;
            set
            {
                if (sleepMinTurns != value)
                {
                    if (value > sleepMaxTurns)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(SleepMinTurns)} cannot exceed {nameof(SleepMaxTurns)} ({sleepMaxTurns}).");
                    }
                    sleepMinTurns = value;
                    OnPropertyChanged(nameof(SleepMinTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="BurnDamageDenominator"/>.</summary>
        public const byte DefaultBurnDamageDenominator = 8;
        private byte burnDamageDenominator = DefaultBurnDamageDenominator;
        /// <summary>A Pokémon with <see cref="PBEStatus1.Burned"/> loses (1/this) of its HP at the end of every turn.</summary>
        public byte BurnDamageDenominator
        {
            get => burnDamageDenominator;
            set
            {
                if (burnDamageDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(BurnDamageDenominator)} must be at least 1.");
                    }
                    burnDamageDenominator = value;
                    OnPropertyChanged(nameof(BurnDamageDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="PoisonDamageDenominator"/>.</summary>
        public const byte DefaultPoisonDamageDenominator = 8;
        private byte poisonDamageDenominator = DefaultPoisonDamageDenominator;
        /// <summary>A Pokémon with <see cref="PBEStatus1.Poisoned"/> loses (1/this) of its HP at the end of every turn.</summary>
        public byte PoisonDamageDenominator
        {
            get => poisonDamageDenominator;
            set
            {
                if (poisonDamageDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(PoisonDamageDenominator)} must be at least 1.");
                    }
                    poisonDamageDenominator = value;
                    OnPropertyChanged(nameof(PoisonDamageDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="ToxicDamageDenominator"/>.</summary>
        public const byte DefaultToxicDamageDenominator = 16;
        private byte toxicDamageDenominator = DefaultToxicDamageDenominator;
        /// <summary>A Pokémon with <see cref="PBEStatus1.BadlyPoisoned"/> loses (<see cref="PBEPokemon.Status1Counter"/>/this) of its HP at the end of every turn.</summary>
        public byte ToxicDamageDenominator
        {
            get => toxicDamageDenominator;
            set
            {
                if (toxicDamageDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ToxicDamageDenominator)} must be at least 1.");
                    }
                    toxicDamageDenominator = value;
                    OnPropertyChanged(nameof(ToxicDamageDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="LeechSeedDenominator"/>.</summary>
        public const byte DefaultLeechSeedDenominator = 8;
        private byte leechSeedDenominator = DefaultLeechSeedDenominator;
        /// <summary>A Pokémon with <see cref="PBEStatus2.LeechSeed"/> loses (1/this) of its HP at the end of every turn and the Pokémon at <see cref="PBEPokemon.SeededPosition"/> on <see cref="PBEPokemon.SeededTeam"/> restores the lost HP.</summary>
        public byte LeechSeedDenominator
        {
            get => leechSeedDenominator;
            set
            {
                if (leechSeedDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(LeechSeedDenominator)} must be at least 1.");
                    }
                    leechSeedDenominator = value;
                    OnPropertyChanged(nameof(LeechSeedDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="CurseDenominator"/>.</summary>
        public const byte DefaultCurseDenominator = 4;
        private byte curseDenominator = DefaultCurseDenominator;
        /// <summary>A Pokémon with <see cref="PBEStatus2.Cursed"/> loses (1/this) of its HP at the end of every turn.</summary>
        public byte CurseDenominator
        {
            get => curseDenominator;
            set
            {
                if (curseDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(CurseDenominator)} must be at least 1.");
                    }
                    curseDenominator = value;
                    OnPropertyChanged(nameof(CurseDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="LeftoversHealDenominator"/>.</summary>
        public const byte DefaultLeftoversHealDenominator = 16;
        private byte leftoversHealDenominator = DefaultLeftoversHealDenominator;
        /// <summary>A Pokémon holding a <see cref="PBEItem.Leftovers"/> restores (1/this) of its HP at the end of every turn.</summary>
        public byte LeftoversHealDenominator
        {
            get => leftoversHealDenominator;
            set
            {
                if (leftoversHealDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(LeftoversHealDenominator)} must be at least 1.");
                    }
                    leftoversHealDenominator = value;
                    OnPropertyChanged(nameof(LeftoversHealDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="BlackSludgeDamageDenominator"/>.</summary>
        public const byte DefaultBlackSludgeDamageDenominator = 8;
        private byte blackSludgeDamageDenominator = DefaultBlackSludgeDamageDenominator;
        /// <summary>A Pokémon holding a <see cref="PBEItem.BlackSludge"/> without <see cref="PBEType.Poison"/> loses (1/this) of its HP at the end of every turn.</summary>
        public byte BlackSludgeDamageDenominator
        {
            get => blackSludgeDamageDenominator;
            set
            {
                if (blackSludgeDamageDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(BlackSludgeDamageDenominator)} must be at least 1.");
                    }
                    blackSludgeDamageDenominator = value;
                    OnPropertyChanged(nameof(BlackSludgeDamageDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="BlackSludgeHealDenominator"/>.</summary>
        public const byte DefaultBlackSludgeHealDenominator = 16;
        private byte blackSludgeHealDenominator = DefaultBlackSludgeHealDenominator;
        /// <summary>A Pokémon holding a <see cref="PBEItem.BlackSludge"/> with <see cref="PBEType.Poison"/> restores (1/this) of its HP at the end of every turn.</summary>
        public byte BlackSludgeHealDenominator
        {
            get => blackSludgeHealDenominator;
            set
            {
                if (blackSludgeHealDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(BlackSludgeHealDenominator)} must be at least 1.");
                    }
                    blackSludgeHealDenominator = value;
                    OnPropertyChanged(nameof(BlackSludgeHealDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="ReflectTurns"/>.</summary>
        public const byte DefaultReflectTurns = 5;
        private byte reflectTurns = DefaultReflectTurns;
        /// <summary>The amount of turns <see cref="PBEMove.Reflect"/> lasts.</summary>
        public byte ReflectTurns
        {
            get => reflectTurns;
            set
            {
                if (reflectTurns != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ReflectTurns)} must be at least 1.");
                    }
                    reflectTurns = value;
                    OnPropertyChanged(nameof(ReflectTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="LightScreenTurns"/>.</summary>
        public const byte DefaultLightScreenTurns = 5;
        private byte lightScreenTurns = DefaultLightScreenTurns;
        /// <summary>The amount of turns <see cref="PBEMove.LightScreen"/> lasts.</summary>
        public byte LightScreenTurns
        {
            get => lightScreenTurns;
            set
            {
                if (lightScreenTurns != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(LightScreenTurns)} must be at least 1.");
                    }
                    lightScreenTurns = value;
                    OnPropertyChanged(nameof(LightScreenTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="LightClayTurnExtension"/>.</summary>
        public const byte DefaultLightClayTurnExtension = 3;
        private byte lightClayTurnExtension = DefaultLightClayTurnExtension;
        /// <summary>The amount of turns added to <see cref="ReflectTurns"/> and <see cref="LightScreenTurns"/> when the user is holding a <see cref="PBEItem.LightClay"/>.</summary>
        public byte LightClayTurnExtension
        {
            get => lightClayTurnExtension;
            set
            {
                if (lightClayTurnExtension != value)
                {
                    lightClayTurnExtension = value;
                    OnPropertyChanged(nameof(LightClayTurnExtension));
                }
            }
        }
        /// <summary>The default value of <see cref="HailTurns"/>.</summary>
        public const byte DefaultHailTurns = 5;
        private byte hailTurns = DefaultHailTurns;
        /// <summary>The amount of turns <see cref="PBEWeather.Hailstorm"/> lasts. For infinite turns, set <see cref="IcyRockTurnExtension"/> to 0 first, then this to 0.</summary>
        public byte HailTurns
        {
            get => hailTurns;
            set
            {
                if (hailTurns != value)
                {
                    if (value == 0 && icyRockTurnExtension != 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"For infinite turns, set {nameof(IcyRockTurnExtension)} to 0 first, then {nameof(HailTurns)} to 0.");
                    }
                    hailTurns = value;
                    OnPropertyChanged(nameof(HailTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="HailDamageDenominator"/>.</summary>
        public const byte DefaultHailDamageDenominator = 16;
        private byte hailDamageDenominator = DefaultHailDamageDenominator;
        /// <summary>A Pokémon in <see cref="PBEWeather.Hailstorm"/> loses (1/this) of its HP at the end of every turn.</summary>
        public byte HailDamageDenominator
        {
            get => hailDamageDenominator;
            set
            {
                if (hailDamageDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(HailDamageDenominator)} must be at least 1.");
                    }
                    hailDamageDenominator = value;
                    OnPropertyChanged(nameof(HailDamageDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="IcyRockTurnExtension"/>.</summary>
        public const byte DefaultIcyRockTurnExtension = 3;
        private byte icyRockTurnExtension = DefaultIcyRockTurnExtension;
        /// <summary>The amount of turns added to <see cref="HailTurns"/> when the user is holding a <see cref="PBEItem.IcyRock"/>. If <see cref="HailTurns"/> is 0 (infinite turns), this must also be 0.</summary>
        public byte IcyRockTurnExtension
        {
            get => icyRockTurnExtension;
            set
            {
                if (icyRockTurnExtension != value)
                {
                    if (value != 0 && hailTurns == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"If {nameof(HailTurns)} is 0 (infinite turns), {nameof(IcyRockTurnExtension)} must also be 0.");
                    }
                    icyRockTurnExtension = value;
                    OnPropertyChanged(nameof(IcyRockTurnExtension));
                }
            }
        }
        /// <summary>The default value of <see cref="IceBodyHealDenominator"/>.</summary>
        public const byte DefaultIceBodyHealDenominator = 16;
        private byte iceBodyHealDenominator = DefaultIceBodyHealDenominator;
        /// <summary>A Pokémon with <see cref="PBEAbility.IceBody"/> in <see cref="PBEWeather.Hailstorm"/> restores (1/this) of its HP at the end of every turn.</summary>
        public byte IceBodyHealDenominator
        {
            get => iceBodyHealDenominator;
            set
            {
                if (iceBodyHealDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(IceBodyHealDenominator)} must be at least 1.");
                    }
                    iceBodyHealDenominator = value;
                    OnPropertyChanged(nameof(IceBodyHealDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="RainTurns"/>.</summary>
        public const byte DefaultRainTurns = 5;
        private byte rainTurns = DefaultRainTurns;
        /// <summary>The amount of turns <see cref="PBEWeather.Rain"/> lasts. For infinite turns, set <see cref="DampRockTurnExtension"/> to 0 first, then this to 0.</summary>
        public byte RainTurns
        {
            get => rainTurns;
            set
            {
                if (rainTurns != value)
                {
                    if (value == 0 && dampRockTurnExtension != 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"For infinite turns, set {nameof(DampRockTurnExtension)} to 0 first, then {nameof(RainTurns)} to 0.");
                    }
                    rainTurns = value;
                    OnPropertyChanged(nameof(RainTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="DampRockTurnExtension"/>.</summary>
        public const byte DefaultDampRockTurnExtension = 3;
        private byte dampRockTurnExtension = DefaultDampRockTurnExtension;
        /// <summary>The amount of turns added to <see cref="RainTurns"/> when the user is holding a <see cref="PBEItem.DampRock"/>. If <see cref="RainTurns"/> is 0 (infinite turns), this must also be 0.</summary>
        public byte DampRockTurnExtension
        {
            get => dampRockTurnExtension;
            set
            {
                if (dampRockTurnExtension != value)
                {
                    if (value != 0 && rainTurns == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"If {nameof(RainTurns)} is 0 (infinite turns), {nameof(DampRockTurnExtension)} must also be 0.");
                    }
                    dampRockTurnExtension = value;
                    OnPropertyChanged(nameof(DampRockTurnExtension));
                }
            }
        }
        /// <summary>The default value of <see cref="SandstormTurns"/>.</summary>
        public const byte DefaultSandstormTurns = 5;
        private byte sandstormTurns = DefaultSandstormTurns;
        /// <summary>The amount of turns <see cref="PBEWeather.Sandstorm"/> lasts. For infinite turns, set <see cref="SmoothRockTurnExtension"/> to 0 first, then this to 0.</summary>
        public byte SandstormTurns
        {
            get => sandstormTurns;
            set
            {
                if (sandstormTurns != value)
                {
                    if (value == 0 && smoothRockTurnExtension != 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"For infinite turns, set {nameof(SmoothRockTurnExtension)} to 0 first, then {nameof(SandstormTurns)} to 0.");
                    }
                    sandstormTurns = value;
                    OnPropertyChanged(nameof(SandstormTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="SandstormDamageDenominator"/>.</summary>
        public const byte DefaultSandstormDamageDenominator = 16;
        private byte sandstormDamageDenominator = DefaultSandstormDamageDenominator;
        /// <summary>A Pokémon in <see cref="PBEWeather.Sandstorm"/> loses (1/this) of its HP at the end of every turn.</summary>
        public byte SandstormDamageDenominator
        {
            get => sandstormDamageDenominator;
            set
            {
                if (sandstormDamageDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(SandstormDamageDenominator)} must be at least 1.");
                    }
                    sandstormDamageDenominator = value;
                    OnPropertyChanged(nameof(SandstormDamageDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="SmoothRockTurnExtension"/>.</summary>
        public const byte DefaultSmoothRockTurnExtension = 3;
        private byte smoothRockTurnExtension = DefaultSmoothRockTurnExtension;
        /// <summary>The amount of turns added to <see cref="SandstormTurns"/> when the user is holding a <see cref="PBEItem.SmoothRock"/>. If <see cref="SandstormTurns"/> is 0 (infinite turns), this must also be 0.</summary>
        public byte SmoothRockTurnExtension
        {
            get => smoothRockTurnExtension;
            set
            {
                if (smoothRockTurnExtension != value)
                {
                    if (value != 0 && sandstormTurns == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"If {nameof(SandstormTurns)} is 0 (infinite turns), {nameof(SmoothRockTurnExtension)} must also be 0.");
                    }
                    smoothRockTurnExtension = value;
                    OnPropertyChanged(nameof(SmoothRockTurnExtension));
                }
            }
        }
        /// <summary>The default value of <see cref="SunTurns"/>.</summary>
        public const byte DefaultSunTurns = 5;
        private byte sunTurns = DefaultSunTurns;
        /// <summary>The amount of turns <see cref="PBEWeather.HarshSunlight"/> lasts. For infinite turns, set <see cref="HeatRockTurnExtension"/> to 0 first, then this to 0.</summary>
        public byte SunTurns
        {
            get => sunTurns;
            set
            {
                if (sunTurns != value)
                {
                    if (value == 0 && heatRockTurnExtension != 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"For infinite turns, set {nameof(HeatRockTurnExtension)} to 0 first, then {nameof(SunTurns)} to 0.");
                    }
                    sunTurns = value;
                    OnPropertyChanged(nameof(SunTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="HeatRockTurnExtension"/>.</summary>
        public const byte DefaultHeatRockTurnExtension = 3;
        private byte heatRockTurnExtension = DefaultHeatRockTurnExtension;
        /// <summary>The amount of turns added to <see cref="SunTurns"/> when the user is holding a <see cref="PBEItem.HeatRock"/>. If <see cref="SunTurns"/> is 0 (infinite turns), this must also be 0.</summary>
        public byte HeatRockTurnExtension
        {
            get => heatRockTurnExtension;
            set
            {
                if (heatRockTurnExtension != value)
                {
                    if (value != 0 && sunTurns == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"If {nameof(SunTurns)} is 0 (infinite turns), {nameof(HeatRockTurnExtension)} must also be 0.");
                    }
                    heatRockTurnExtension = value;
                    OnPropertyChanged(nameof(HeatRockTurnExtension));
                }
            }
        }

        /// <summary>Creates a new <see cref="PBESettings"/> object where every setting is pre-set to the values used in official games.</summary>
        public PBESettings() { }

        private enum PBESettingID : ushort
        {
            MaxLevel,
            MinLevel,
            MaxPartySize,
            MaxPokemonNameLength,
            MaxTrainerNameLength,
            MaxTotalEVs,
            MaxIVs,
            NatureStatBoost,
            MaxStatChange,
            NumMoves,
            PPMultiplier,
            MaxPPUps,
            CritMultiplier,
            ConfusionMaxTurns,
            ConfusionMinTurns,
            SleepMaxTurns,
            SleepMinTurns,
            BurnDamageDenominator,
            PoisonDamageDenominator,
            ToxicDamageDenominator,
            LeechSeedDenominator,
            CurseDenominator,
            LeftoversHealDenominator,
            BlackSludgeDamageDenominator,
            BlackSludgeHealDenominator,
            ReflectTurns,
            LightScreenTurns,
            LightClayTurnExtension,
            HailTurns,
            HailDamageDenominator,
            IcyRockTurnExtension,
            IceBodyHealDenominator,
            RainTurns,
            DampRockTurnExtension,
            SandstormTurns,
            SandstormDamageDenominator,
            SmoothRockTurnExtension,
            SunTurns,
            HeatRockTurnExtension
        }

        public override string ToString()
        {
            return Convert.ToBase64String(ToBytes().ToArray());
        }
        public static PBESettings FromString(string code)
        {
            if (code == null)
            {
                throw new ArgumentNullException(code);
            }
            using (var r = new BinaryReader(new MemoryStream(Convert.FromBase64String(code))))
            {
                return FromBytes(r);
            }
        }

        internal List<byte> ToBytes()
        {
            var bytes = new List<byte>();
            ushort numChanged = 0;
            if (maxLevel != DefaultMaxLevel)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.MaxLevel));
                bytes.Add(maxLevel);
                numChanged++;
            }
            if (minLevel != DefaultMinLevel)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.MinLevel));
                bytes.Add(minLevel);
                numChanged++;
            }
            if (maxPartySize != DefaultMaxPartySize)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.MaxPartySize));
                bytes.Add((byte)maxPartySize);
                numChanged++;
            }
            if (maxPokemonNameLength != DefaultMaxPokemonNameLength)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.MaxPokemonNameLength));
                bytes.Add(maxPokemonNameLength);
                numChanged++;
            }
            if (maxTrainerNameLength != DefaultMaxTrainerNameLength)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.MaxTrainerNameLength));
                bytes.Add(maxTrainerNameLength);
                numChanged++;
            }
            if (maxTotalEVs != DefaultMaxTotalEVs)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.MaxTotalEVs));
                bytes.AddRange(BitConverter.GetBytes(maxTotalEVs));
                numChanged++;
            }
            if (maxIVs != DefaultMaxIVs)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.MaxIVs));
                bytes.Add(maxIVs);
                numChanged++;
            }
            if (natureStatBoost != DefaultNatureStatBoost)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.NatureStatBoost));
                bytes.AddRange(BitConverter.GetBytes(natureStatBoost));
                numChanged++;
            }
            if (maxStatChange != DefaultMaxStatChange)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.MaxStatChange));
                bytes.Add((byte)maxStatChange);
                numChanged++;
            }
            if (numMoves != DefaultNumMoves)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.NumMoves));
                bytes.Add(numMoves);
                numChanged++;
            }
            if (ppMultiplier != DefaultPPMultiplier)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.PPMultiplier));
                bytes.Add(ppMultiplier);
                numChanged++;
            }
            if (maxPPUps != DefaultMaxPPUps)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.MaxPPUps));
                bytes.Add(maxPPUps);
                numChanged++;
            }
            if (critMultiplier != DefaultCritMultiplier)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.CritMultiplier));
                bytes.AddRange(BitConverter.GetBytes(critMultiplier));
                numChanged++;
            }
            if (confusionMaxTurns != DefaultConfusionMaxTurns)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.ConfusionMaxTurns));
                bytes.Add(confusionMaxTurns);
                numChanged++;
            }
            if (confusionMinTurns != DefaultConfusionMinTurns)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.ConfusionMinTurns));
                bytes.Add(confusionMinTurns);
                numChanged++;
            }
            if (sleepMaxTurns != DefaultSleepMaxTurns)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.SleepMaxTurns));
                bytes.Add(sleepMaxTurns);
                numChanged++;
            }
            if (sleepMinTurns != DefaultSleepMinTurns)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.SleepMinTurns));
                bytes.Add(sleepMinTurns);
                numChanged++;
            }
            if (burnDamageDenominator != DefaultBurnDamageDenominator)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.BurnDamageDenominator));
                bytes.Add(burnDamageDenominator);
                numChanged++;
            }
            if (poisonDamageDenominator != DefaultPoisonDamageDenominator)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.PoisonDamageDenominator));
                bytes.Add(poisonDamageDenominator);
                numChanged++;
            }
            if (toxicDamageDenominator != DefaultToxicDamageDenominator)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.ToxicDamageDenominator));
                bytes.Add(toxicDamageDenominator);
                numChanged++;
            }
            if (leechSeedDenominator != DefaultLeechSeedDenominator)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.LeechSeedDenominator));
                bytes.Add(leechSeedDenominator);
                numChanged++;
            }
            if (curseDenominator != DefaultCurseDenominator)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.CurseDenominator));
                bytes.Add(curseDenominator);
                numChanged++;
            }
            if (leftoversHealDenominator != DefaultLeftoversHealDenominator)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.LeftoversHealDenominator));
                bytes.Add(leftoversHealDenominator);
                numChanged++;
            }
            if (blackSludgeDamageDenominator != DefaultBlackSludgeDamageDenominator)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.BlackSludgeDamageDenominator));
                bytes.Add(blackSludgeDamageDenominator);
                numChanged++;
            }
            if (blackSludgeHealDenominator != DefaultBlackSludgeHealDenominator)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.BlackSludgeHealDenominator));
                bytes.Add(blackSludgeHealDenominator);
                numChanged++;
            }
            if (reflectTurns != DefaultReflectTurns)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.ReflectTurns));
                bytes.Add(reflectTurns);
                numChanged++;
            }
            if (lightScreenTurns != DefaultLightScreenTurns)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.LightScreenTurns));
                bytes.Add(lightScreenTurns);
                numChanged++;
            }
            if (lightClayTurnExtension != DefaultLightClayTurnExtension)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.LightClayTurnExtension));
                bytes.Add(lightClayTurnExtension);
                numChanged++;
            }
            if (hailTurns != DefaultHailTurns)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.HailTurns));
                bytes.Add(hailTurns);
                numChanged++;
            }
            if (hailDamageDenominator != DefaultHailDamageDenominator)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.HailDamageDenominator));
                bytes.Add(hailDamageDenominator);
                numChanged++;
            }
            if (icyRockTurnExtension != DefaultIcyRockTurnExtension)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.IcyRockTurnExtension));
                bytes.Add(icyRockTurnExtension);
                numChanged++;
            }
            if (iceBodyHealDenominator != DefaultIceBodyHealDenominator)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.IceBodyHealDenominator));
                bytes.Add(iceBodyHealDenominator);
                numChanged++;
            }
            if (rainTurns != DefaultRainTurns)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.RainTurns));
                bytes.Add(rainTurns);
                numChanged++;
            }
            if (dampRockTurnExtension != DefaultDampRockTurnExtension)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.DampRockTurnExtension));
                bytes.Add(dampRockTurnExtension);
                numChanged++;
            }
            if (sandstormTurns != DefaultSandstormTurns)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.SandstormTurns));
                bytes.Add(sandstormTurns);
                numChanged++;
            }
            if (sandstormDamageDenominator != DefaultSandstormDamageDenominator)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.SandstormDamageDenominator));
                bytes.Add(sandstormDamageDenominator);
                numChanged++;
            }
            if (smoothRockTurnExtension != DefaultSmoothRockTurnExtension)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.SmoothRockTurnExtension));
                bytes.Add(smoothRockTurnExtension);
                numChanged++;
            }
            if (sunTurns != DefaultSunTurns)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.SunTurns));
                bytes.Add(sunTurns);
                numChanged++;
            }
            if (heatRockTurnExtension != DefaultHeatRockTurnExtension)
            {
                bytes.AddRange(BitConverter.GetBytes((ushort)PBESettingID.HeatRockTurnExtension));
                bytes.Add(heatRockTurnExtension);
                numChanged++;
            }
            bytes.InsertRange(0, BitConverter.GetBytes(numChanged));
            return bytes;
        }
        internal static PBESettings FromBytes(BinaryReader r)
        {
            var settings = new PBESettings();
            ushort numChanged = r.ReadUInt16();
            for (ushort i = 0; i < numChanged; i++)
            {
                switch ((PBESettingID)r.ReadUInt16())
                {
                    case PBESettingID.MaxLevel: settings.MaxLevel = r.ReadByte(); break;
                    case PBESettingID.MinLevel: settings.MinLevel = r.ReadByte(); break;
                    case PBESettingID.MaxPartySize: settings.MaxPartySize = r.ReadSByte(); break;
                    case PBESettingID.MaxPokemonNameLength: settings.MaxPokemonNameLength = r.ReadByte(); break;
                    case PBESettingID.MaxTrainerNameLength: settings.MaxTrainerNameLength = r.ReadByte(); break;
                    case PBESettingID.MaxTotalEVs: settings.MaxTotalEVs = r.ReadUInt16(); break;
                    case PBESettingID.MaxIVs: settings.MaxIVs = r.ReadByte(); break;
                    case PBESettingID.NatureStatBoost: settings.NatureStatBoost = r.ReadDouble(); break;
                    case PBESettingID.MaxStatChange: settings.MaxStatChange = r.ReadSByte(); break;
                    case PBESettingID.NumMoves: settings.NumMoves = r.ReadByte(); break;
                    case PBESettingID.PPMultiplier: settings.PPMultiplier = r.ReadByte(); break;
                    case PBESettingID.MaxPPUps: settings.MaxPPUps = r.ReadByte(); break;
                    case PBESettingID.CritMultiplier: settings.CritMultiplier = r.ReadDouble(); break;
                    case PBESettingID.ConfusionMaxTurns: settings.ConfusionMaxTurns = r.ReadByte(); break;
                    case PBESettingID.ConfusionMinTurns: settings.ConfusionMinTurns = r.ReadByte(); break;
                    case PBESettingID.SleepMaxTurns: settings.SleepMaxTurns = r.ReadByte(); break;
                    case PBESettingID.SleepMinTurns: settings.SleepMinTurns = r.ReadByte(); break;
                    case PBESettingID.BurnDamageDenominator: settings.BurnDamageDenominator = r.ReadByte(); break;
                    case PBESettingID.PoisonDamageDenominator: settings.PoisonDamageDenominator = r.ReadByte(); break;
                    case PBESettingID.ToxicDamageDenominator: settings.ToxicDamageDenominator = r.ReadByte(); break;
                    case PBESettingID.LeechSeedDenominator: settings.LeechSeedDenominator = r.ReadByte(); break;
                    case PBESettingID.CurseDenominator: settings.CurseDenominator = r.ReadByte(); break;
                    case PBESettingID.LeftoversHealDenominator: settings.LeftoversHealDenominator = r.ReadByte(); break;
                    case PBESettingID.BlackSludgeDamageDenominator: settings.BlackSludgeDamageDenominator = r.ReadByte(); break;
                    case PBESettingID.BlackSludgeHealDenominator: settings.BlackSludgeHealDenominator = r.ReadByte(); break;
                    case PBESettingID.ReflectTurns: settings.ReflectTurns = r.ReadByte(); break;
                    case PBESettingID.LightScreenTurns: settings.LightScreenTurns = r.ReadByte(); break;
                    case PBESettingID.LightClayTurnExtension: settings.LightClayTurnExtension = r.ReadByte(); break;
                    case PBESettingID.HailTurns: settings.HailTurns = r.ReadByte(); break;
                    case PBESettingID.HailDamageDenominator: settings.HailDamageDenominator = r.ReadByte(); break;
                    case PBESettingID.IcyRockTurnExtension: settings.IcyRockTurnExtension = r.ReadByte(); break;
                    case PBESettingID.IceBodyHealDenominator: settings.IceBodyHealDenominator = r.ReadByte(); break;
                    case PBESettingID.RainTurns: settings.RainTurns = r.ReadByte(); break;
                    case PBESettingID.DampRockTurnExtension: settings.DampRockTurnExtension = r.ReadByte(); break;
                    case PBESettingID.SandstormTurns: settings.SandstormTurns = r.ReadByte(); break;
                    case PBESettingID.SandstormDamageDenominator: settings.SandstormDamageDenominator = r.ReadByte(); break;
                    case PBESettingID.SmoothRockTurnExtension: settings.SmoothRockTurnExtension = r.ReadByte(); break;
                    case PBESettingID.SunTurns: settings.SunTurns = r.ReadByte(); break;
                    case PBESettingID.HeatRockTurnExtension: settings.HeatRockTurnExtension = r.ReadByte(); break;
                    default: throw new InvalidDataException();
                }
            }
            return settings;
        }
    }
}
