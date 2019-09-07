using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Kermalis.PokemonBattleEngine.Data
{
    /// <summary>The various engine settings.</summary>
    public sealed class PBESettings : INotifyPropertyChanged
    {
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        /// <summary>Fires whenever a property changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isReadOnly;
        /// <summary>Gets a value that indicates whether this <see cref="PBESettings"/> object is read-only.</summary>
        public bool IsReadOnly
        {
            get => isReadOnly;
            private set
            {
                if (isReadOnly != value)
                {
                    isReadOnly = value;
                    OnPropertyChanged(nameof(IsReadOnly));
                }
            }
        }

        /// <summary>The default settings used in official games.</summary>
        public static PBESettings DefaultSettings { get; }

        static PBESettings()
        {
            DefaultSettings = new PBESettings();
            DefaultSettings.MakeReadOnly();
        }

        /// <summary>The default value of <see cref="MaxLevel"/>.</summary>
        public const byte DefaultMaxLevel = 100;
        private byte maxLevel = DefaultMaxLevel;
        /// <summary>The maximum level a Pokémon can be. Not used in stat/damage calculation.</summary>
        public byte MaxLevel
        {
            get => maxLevel;
            set
            {
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
        [Obsolete("Currently not used anywhere.")]
        public byte MaxTrainerNameLength
        {
            get => maxTrainerNameLength;
            set
            {
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
                ReadOnlyCheck();
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
        /// <summary>Creates a new <see cref="PBESettings"/> object with the specified code <see cref="string"/>.</summary>
        /// <param name="code">The code <see cref="string"/> to use.</param>
        public PBESettings(string code)
        {
            if (code == null)
            {
                throw new ArgumentNullException(code);
            }
            using (var r = new BinaryReader(new MemoryStream(Convert.FromBase64String(code))))
            {
                FromBytes(r);
            }
        }
        /// <summary>Creates a new <see cref="PBESettings"/> object which copies the settings from the specified <see cref="PBESettings"/> object. <see cref="IsReadOnly"/> and <see cref="PropertyChanged"/> are not copied.</summary>
        /// <param name="other">The <see cref="PBESettings"/> object to copy settings from.</param>
        public PBESettings(PBESettings other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            MaxLevel = other.maxLevel;
            MinLevel = other.minLevel;
            MaxPartySize = other.maxPartySize;
            MaxPokemonNameLength = other.maxPokemonNameLength;
            MaxTrainerNameLength = other.maxTrainerNameLength;
            MaxTotalEVs = other.maxTotalEVs;
            MaxIVs = other.maxIVs;
            NatureStatBoost = other.natureStatBoost;
            MaxStatChange = other.maxStatChange;
            NumMoves = other.numMoves;
            PPMultiplier = other.ppMultiplier;
            MaxPPUps = other.maxPPUps;
            CritMultiplier = other.critMultiplier;
            ConfusionMaxTurns = other.confusionMaxTurns;
            ConfusionMinTurns = other.confusionMinTurns;
            SleepMaxTurns = other.sleepMaxTurns;
            SleepMinTurns = other.sleepMinTurns;
            BurnDamageDenominator = other.burnDamageDenominator;
            PoisonDamageDenominator = other.poisonDamageDenominator;
            ToxicDamageDenominator = other.toxicDamageDenominator;
            LeechSeedDenominator = other.leechSeedDenominator;
            CurseDenominator = other.curseDenominator;
            LeftoversHealDenominator = other.leftoversHealDenominator;
            BlackSludgeDamageDenominator = other.blackSludgeDamageDenominator;
            BlackSludgeHealDenominator = other.blackSludgeHealDenominator;
            ReflectTurns = other.reflectTurns;
            LightScreenTurns = other.lightScreenTurns;
            LightClayTurnExtension = other.lightClayTurnExtension;
            HailTurns = other.hailTurns;
            HailDamageDenominator = other.hailDamageDenominator;
            IcyRockTurnExtension = other.icyRockTurnExtension;
            IceBodyHealDenominator = other.iceBodyHealDenominator;
            RainTurns = other.rainTurns;
            DampRockTurnExtension = other.dampRockTurnExtension;
            SandstormTurns = other.sandstormTurns;
            SandstormDamageDenominator = other.sandstormDamageDenominator;
            SmoothRockTurnExtension = other.smoothRockTurnExtension;
            SunTurns = other.sunTurns;
            HeatRockTurnExtension = other.heatRockTurnExtension;
        }
        internal PBESettings(BinaryReader r)
        {
            FromBytes(r);
        }

        private void ReadOnlyCheck()
        {
            if (isReadOnly)
            {
                throw new InvalidOperationException($"This {nameof(PBESettings)} is marked as read-only.");
            }
        }
        /// <summary>Marks this <see cref="PBESettings"/> object as read-only.</summary>
        public void MakeReadOnly()
        {
            if (!isReadOnly)
            {
                IsReadOnly = true;
            }
        }

        /// <summary>Returns a value indicating whether a code <see cref="string"/> or another <see cref="PBESettings"/> object represent the same settings as this <see cref="PBESettings"/> object.</summary>
        /// <param name="obj">The code <see cref="string"/> or the <see cref="PBESettings"/> object to check for equality.</param>
        public override bool Equals(object obj)
        {
            if (obj is string str)
            {
                PBESettings ps;
                try
                {
                    ps = new PBESettings(str);
                }
                catch
                {
                    return false;
                }
                return ps.Equals(this);
            }
            else if (obj is PBESettings other)
            {
                return other.maxLevel.Equals(maxLevel)
                    && other.minLevel.Equals(minLevel)
                    && other.maxPartySize.Equals(maxPartySize)
                    && other.maxPokemonNameLength.Equals(maxPokemonNameLength)
                    && other.maxTrainerNameLength.Equals(maxTrainerNameLength)
                    && other.maxTotalEVs.Equals(maxTotalEVs)
                    && other.maxIVs.Equals(maxIVs)
                    && other.natureStatBoost.Equals(natureStatBoost)
                    && other.maxStatChange.Equals(maxStatChange)
                    && other.numMoves.Equals(numMoves)
                    && other.ppMultiplier.Equals(ppMultiplier)
                    && other.maxPPUps.Equals(maxPPUps)
                    && other.critMultiplier.Equals(critMultiplier)
                    && other.confusionMaxTurns.Equals(confusionMaxTurns)
                    && other.confusionMinTurns.Equals(confusionMinTurns)
                    && other.sleepMaxTurns.Equals(sleepMaxTurns)
                    && other.sleepMinTurns.Equals(sleepMinTurns)
                    && other.burnDamageDenominator.Equals(burnDamageDenominator)
                    && other.poisonDamageDenominator.Equals(poisonDamageDenominator)
                    && other.toxicDamageDenominator.Equals(toxicDamageDenominator)
                    && other.leechSeedDenominator.Equals(leechSeedDenominator)
                    && other.curseDenominator.Equals(curseDenominator)
                    && other.leftoversHealDenominator.Equals(leftoversHealDenominator)
                    && other.blackSludgeDamageDenominator.Equals(blackSludgeDamageDenominator)
                    && other.blackSludgeHealDenominator.Equals(blackSludgeHealDenominator)
                    && other.reflectTurns.Equals(reflectTurns)
                    && other.lightScreenTurns.Equals(lightScreenTurns)
                    && other.lightClayTurnExtension.Equals(lightClayTurnExtension)
                    && other.hailTurns.Equals(hailTurns)
                    && other.hailDamageDenominator.Equals(hailDamageDenominator)
                    && other.icyRockTurnExtension.Equals(icyRockTurnExtension)
                    && other.iceBodyHealDenominator.Equals(iceBodyHealDenominator)
                    && other.rainTurns.Equals(rainTurns)
                    && other.dampRockTurnExtension.Equals(dampRockTurnExtension)
                    && other.sandstormTurns.Equals(sandstormTurns)
                    && other.sandstormDamageDenominator.Equals(sandstormDamageDenominator)
                    && other.smoothRockTurnExtension.Equals(smoothRockTurnExtension)
                    && other.sunTurns.Equals(sunTurns)
                    && other.heatRockTurnExtension.Equals(heatRockTurnExtension);
            }
            else
            {
                return false;
            }
        }

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

        /// <summary>Converts this <see cref="PBESettings"/> object into a unique code <see cref="string"/>.</summary>
        public override string ToString()
        {
            return Convert.ToBase64String(ToBytes().ToArray());
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
        private void FromBytes(BinaryReader r)
        {
            ushort numChanged = r.ReadUInt16();
            for (ushort i = 0; i < numChanged; i++)
            {
                switch ((PBESettingID)r.ReadUInt16())
                {
                    case PBESettingID.MaxLevel: MaxLevel = r.ReadByte(); break;
                    case PBESettingID.MinLevel: MinLevel = r.ReadByte(); break;
                    case PBESettingID.MaxPartySize: MaxPartySize = r.ReadSByte(); break;
                    case PBESettingID.MaxPokemonNameLength: MaxPokemonNameLength = r.ReadByte(); break;
                    case PBESettingID.MaxTrainerNameLength: MaxTrainerNameLength = r.ReadByte(); break;
                    case PBESettingID.MaxTotalEVs: MaxTotalEVs = r.ReadUInt16(); break;
                    case PBESettingID.MaxIVs: MaxIVs = r.ReadByte(); break;
                    case PBESettingID.NatureStatBoost: NatureStatBoost = r.ReadDouble(); break;
                    case PBESettingID.MaxStatChange: MaxStatChange = r.ReadSByte(); break;
                    case PBESettingID.NumMoves: NumMoves = r.ReadByte(); break;
                    case PBESettingID.PPMultiplier: PPMultiplier = r.ReadByte(); break;
                    case PBESettingID.MaxPPUps: MaxPPUps = r.ReadByte(); break;
                    case PBESettingID.CritMultiplier: CritMultiplier = r.ReadDouble(); break;
                    case PBESettingID.ConfusionMaxTurns: ConfusionMaxTurns = r.ReadByte(); break;
                    case PBESettingID.ConfusionMinTurns: ConfusionMinTurns = r.ReadByte(); break;
                    case PBESettingID.SleepMaxTurns: SleepMaxTurns = r.ReadByte(); break;
                    case PBESettingID.SleepMinTurns: SleepMinTurns = r.ReadByte(); break;
                    case PBESettingID.BurnDamageDenominator: BurnDamageDenominator = r.ReadByte(); break;
                    case PBESettingID.PoisonDamageDenominator: PoisonDamageDenominator = r.ReadByte(); break;
                    case PBESettingID.ToxicDamageDenominator: ToxicDamageDenominator = r.ReadByte(); break;
                    case PBESettingID.LeechSeedDenominator: LeechSeedDenominator = r.ReadByte(); break;
                    case PBESettingID.CurseDenominator: CurseDenominator = r.ReadByte(); break;
                    case PBESettingID.LeftoversHealDenominator: LeftoversHealDenominator = r.ReadByte(); break;
                    case PBESettingID.BlackSludgeDamageDenominator: BlackSludgeDamageDenominator = r.ReadByte(); break;
                    case PBESettingID.BlackSludgeHealDenominator: BlackSludgeHealDenominator = r.ReadByte(); break;
                    case PBESettingID.ReflectTurns: ReflectTurns = r.ReadByte(); break;
                    case PBESettingID.LightScreenTurns: LightScreenTurns = r.ReadByte(); break;
                    case PBESettingID.LightClayTurnExtension: LightClayTurnExtension = r.ReadByte(); break;
                    case PBESettingID.HailTurns: HailTurns = r.ReadByte(); break;
                    case PBESettingID.HailDamageDenominator: HailDamageDenominator = r.ReadByte(); break;
                    case PBESettingID.IcyRockTurnExtension: IcyRockTurnExtension = r.ReadByte(); break;
                    case PBESettingID.IceBodyHealDenominator: IceBodyHealDenominator = r.ReadByte(); break;
                    case PBESettingID.RainTurns: RainTurns = r.ReadByte(); break;
                    case PBESettingID.DampRockTurnExtension: DampRockTurnExtension = r.ReadByte(); break;
                    case PBESettingID.SandstormTurns: SandstormTurns = r.ReadByte(); break;
                    case PBESettingID.SandstormDamageDenominator: SandstormDamageDenominator = r.ReadByte(); break;
                    case PBESettingID.SmoothRockTurnExtension: SmoothRockTurnExtension = r.ReadByte(); break;
                    case PBESettingID.SunTurns: SunTurns = r.ReadByte(); break;
                    case PBESettingID.HeatRockTurnExtension: HeatRockTurnExtension = r.ReadByte(); break;
                    default: throw new InvalidDataException();
                }
            }
        }
    }
}
