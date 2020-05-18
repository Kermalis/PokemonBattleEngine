using Kermalis.EndianBinaryIO;
using Kermalis.PokemonBattleEngine.Battle;
using System;
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

        private bool _isReadOnly;
        /// <summary>Gets a value that indicates whether this <see cref="PBESettings"/> object is read-only.</summary>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            private set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
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
        private byte _maxLevel = DefaultMaxLevel;
        /// <summary>The maximum level a Pokémon can be. Not used in stat/damage calculation.</summary>
        public byte MaxLevel
        {
            get => _maxLevel;
            set
            {
                ReadOnlyCheck();
                if (_maxLevel != value)
                {
                    if (value < _minLevel)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxLevel)} must be at least {nameof(MinLevel)} ({_minLevel}).");
                    }
                    _maxLevel = value;
                    OnPropertyChanged(nameof(MaxLevel));
                }
            }
        }
        /// <summary>The default value of <see cref="MinLevel"/>.</summary>
        public const byte DefaultMinLevel = 1;
        private byte _minLevel = DefaultMinLevel;
        /// <summary>The minimum level a Pokémon can be.</summary>
        public byte MinLevel
        {
            get => _minLevel;
            set
            {
                ReadOnlyCheck();
                if (_minLevel != value)
                {
                    if (value < 1 || value > _maxLevel)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MinLevel)} must be at least 1 and cannot exceed {nameof(MaxLevel)} ({_maxLevel}).");
                    }
                    _minLevel = value;
                    OnPropertyChanged(nameof(MinLevel));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxPartySize"/>.</summary>
        public const sbyte DefaultMaxPartySize = 6;
        private sbyte _maxPartySize = DefaultMaxPartySize;
        /// <summary>The maximum amount of Pokémon each team can bring into a battle.</summary>
        public sbyte MaxPartySize
        {
            get => _maxPartySize;
            set
            {
                ReadOnlyCheck();
                if (_maxPartySize != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxPartySize)} must be at least 1.");
                    }
                    _maxPartySize = value;
                    OnPropertyChanged(nameof(MaxPartySize));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxPokemonNameLength"/>.</summary>
        public const byte DefaultMaxPokemonNameLength = 10;
        private byte _maxPokemonNameLength = DefaultMaxPokemonNameLength;
        /// <summary>The maximum amount of characters a Pokémon nickname can have.</summary>
        public byte MaxPokemonNameLength
        {
            get => _maxPokemonNameLength;
            set
            {
                ReadOnlyCheck();
                if (_maxPokemonNameLength != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxPokemonNameLength)} must be at least 1.");
                    }
                    _maxPokemonNameLength = value;
                    OnPropertyChanged(nameof(MaxPokemonNameLength));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxTrainerNameLength"/>. This value is different in non-English games.</summary>
        public const byte DefaultMaxTrainerNameLength = 7;
        private byte _maxTrainerNameLength = DefaultMaxTrainerNameLength;
        /// <summary>The maximum amount of characters a trainer's name can have.</summary>
        [Obsolete("Currently not used anywhere.")]
        public byte MaxTrainerNameLength
        {
            get => _maxTrainerNameLength;
            set
            {
                ReadOnlyCheck();
                if (_maxTrainerNameLength != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxTrainerNameLength)} must be at least 1.");
                    }
                    _maxTrainerNameLength = value;
                    OnPropertyChanged(nameof(MaxTrainerNameLength));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxTotalEVs"/>.</summary>
        public const ushort DefaultMaxTotalEVs = 510;
        private ushort _maxTotalEVs = DefaultMaxTotalEVs;
        /// <summary>The maximum sum of a Pokémon's EVs.</summary>
        public ushort MaxTotalEVs
        {
            get => _maxTotalEVs;
            set
            {
                const int max = byte.MaxValue * 6;
                ReadOnlyCheck();
                if (_maxTotalEVs != value)
                {
                    if (value > max)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxTotalEVs)} must not exceed {max}.");
                    }
                    _maxTotalEVs = value;
                    OnPropertyChanged(nameof(MaxTotalEVs));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxIVs"/>.</summary>
        public const byte DefaultMaxIVs = 31;
        private byte _maxIVs = DefaultMaxIVs;
        /// <summary>The maximum amount of IVs Pokémon can have in each stat. Raising this will not affect <see cref="PBEMove.HiddenPower"/>.</summary>
        public byte MaxIVs
        {
            get => _maxIVs;
            set
            {
                ReadOnlyCheck();
                if (_maxIVs != value)
                {
                    _maxIVs = value;
                    OnPropertyChanged(nameof(MaxIVs));
                }
            }
        }
        /// <summary>The default value of <see cref="NatureStatBoost"/>.</summary>
        public const double DefaultNatureStatBoost = 0.1;
        private double _natureStatBoost = DefaultNatureStatBoost;
        /// <summary>The amount of influence a Pokémon's <see cref="PBENature"/> has on its stats.</summary>
        public double NatureStatBoost
        {
            get => _natureStatBoost;
            set
            {
                ReadOnlyCheck();
                if (_natureStatBoost != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(NatureStatBoost)} must be at least 0.");
                    }
                    _natureStatBoost = value;
                    OnPropertyChanged(nameof(NatureStatBoost));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxStatChange"/>.</summary>
        public const sbyte DefaultMaxStatChange = 6;
        private sbyte _maxStatChange = DefaultMaxStatChange;
        /// <summary>The maximum change a stat can have in the negative and positive direction.</summary>
        public sbyte MaxStatChange
        {
            get => _maxStatChange;
            set
            {
                ReadOnlyCheck();
                if (_maxStatChange != value)
                {
                    _maxStatChange = value;
                    OnPropertyChanged(nameof(MaxStatChange));
                }
            }
        }
        /// <summary>The default value of <see cref="NumMoves"/>.</summary>
        public const byte DefaultNumMoves = 4;
        private byte _numMoves = DefaultNumMoves;
        /// <summary>The maximum amount of moves a specific Pokémon can remember at once.</summary>
        public byte NumMoves
        {
            get => _numMoves;
            set
            {
                ReadOnlyCheck();
                if (_numMoves != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(NumMoves)} must be at least 1.");
                    }
                    _numMoves = value;
                    OnPropertyChanged(nameof(NumMoves));
                }
            }
        }
        /// <summary>The default value of <see cref="PPMultiplier"/>.</summary>
        public const byte DefaultPPMultiplier = 5;
        private byte _ppMultiplier = DefaultPPMultiplier;
        /// <summary>This affects the base PP of each move and the boost PP-Ups give. The formulas that determine PP are at <see cref="PBEBattleMoveset.GetNonTransformPP(PBESettings, PBEMove, byte)"/> and <see cref="PBEBattleMoveset.GetTransformPP(PBESettings, PBEMove)"/>.</summary>
        public byte PPMultiplier
        {
            get => _ppMultiplier;
            set
            {
                ReadOnlyCheck();
                if (_ppMultiplier != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(PPMultiplier)} must be at least 1.");
                    }
                    _ppMultiplier = value;
                    OnPropertyChanged(nameof(PPMultiplier));
                }
            }
        }
        /// <summary>The default value of <see cref="MaxPPUps"/>.</summary>
        public const byte DefaultMaxPPUps = 3;
        private byte _maxPPUps = DefaultMaxPPUps;
        /// <summary>The maximum amount of PP-Ups that can be used on each of a Pokémon's moves.</summary>
        public byte MaxPPUps
        {
            get => _maxPPUps;
            set
            {
                ReadOnlyCheck();
                if (_maxPPUps != value)
                {
                    _maxPPUps = value;
                    OnPropertyChanged(nameof(MaxPPUps));
                }
            }
        }
        /// <summary>The default value of <see cref="CritMultiplier"/>.</summary>
        public const double DefaultCritMultiplier = 2.0;
        private double _critMultiplier = DefaultCritMultiplier;
        /// <summary>The damage boost awarded by critical hits.</summary>
        public double CritMultiplier
        {
            get => _critMultiplier;
            set
            {
                ReadOnlyCheck();
                if (_critMultiplier != value)
                {
                    _critMultiplier = value;
                    OnPropertyChanged(nameof(CritMultiplier));
                }
            }
        }
        /// <summary>The default value of <see cref="ConfusionMaxTurns"/>.</summary>
        public const byte DefaultConfusionMaxTurns = 4;
        private byte _confusionMaxTurns = DefaultConfusionMaxTurns;
        /// <summary>The maximum amount of turns a Pokémon can be <see cref="PBEStatus2.Confused"/>.</summary>
        public byte ConfusionMaxTurns
        {
            get => _confusionMaxTurns;
            set
            {
                ReadOnlyCheck();
                if (_confusionMaxTurns != value)
                {
                    if (value < _confusionMinTurns)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ConfusionMaxTurns)} must be at least {nameof(ConfusionMinTurns)} ({_confusionMinTurns}).");
                    }
                    _confusionMaxTurns = value;
                    OnPropertyChanged(nameof(ConfusionMaxTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="ConfusionMinTurns"/>.</summary>
        public const byte DefaultConfusionMinTurns = 1;
        private byte _confusionMinTurns = DefaultConfusionMinTurns;
        /// <summary>The minimum amount of turns a Pokémon can be <see cref="PBEStatus2.Confused"/>.</summary>
        public byte ConfusionMinTurns
        {
            get => _confusionMinTurns;
            set
            {
                ReadOnlyCheck();
                if (_confusionMinTurns != value)
                {
                    if (value > _confusionMaxTurns)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ConfusionMinTurns)} cannot exceed {nameof(ConfusionMaxTurns)} ({_confusionMaxTurns}).");
                    }
                    _confusionMinTurns = value;
                    OnPropertyChanged(nameof(ConfusionMinTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="SleepMaxTurns"/>.</summary>
        public const byte DefaultSleepMaxTurns = 3;
        private byte _sleepMaxTurns = DefaultSleepMaxTurns;
        /// <summary>The maximum amount of turns a Pokémon can be <see cref="PBEStatus1.Asleep"/>. <see cref="PBEMove.Rest"/> will always sleep for <see cref="SleepMaxTurns"/> turns.</summary>
        public byte SleepMaxTurns
        {
            get => _sleepMaxTurns;
            set
            {
                ReadOnlyCheck();
                if (_sleepMaxTurns != value)
                {
                    if (value < _sleepMinTurns)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(SleepMaxTurns)} must be at least {nameof(SleepMinTurns)} ({_sleepMinTurns}).");
                    }
                    _sleepMaxTurns = value;
                    OnPropertyChanged(nameof(SleepMaxTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="SleepMinTurns"/>.</summary>
        public const byte DefaultSleepMinTurns = 1;
        private byte _sleepMinTurns = DefaultSleepMinTurns;
        /// <summary>The minimum amount of turns a Pokémon can be <see cref="PBEStatus1.Asleep"/>. <see cref="PBEMove.Rest"/> will ignore this value and always sleep for <see cref="SleepMaxTurns"/> turns.</summary>
        public byte SleepMinTurns
        {
            get => _sleepMinTurns;
            set
            {
                ReadOnlyCheck();
                if (_sleepMinTurns != value)
                {
                    if (value > _sleepMaxTurns)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(SleepMinTurns)} cannot exceed {nameof(SleepMaxTurns)} ({_sleepMaxTurns}).");
                    }
                    _sleepMinTurns = value;
                    OnPropertyChanged(nameof(SleepMinTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="BurnDamageDenominator"/>.</summary>
        public const byte DefaultBurnDamageDenominator = 8;
        private byte _burnDamageDenominator = DefaultBurnDamageDenominator;
        /// <summary>A Pokémon with <see cref="PBEStatus1.Burned"/> loses (1/this) of its HP at the end of every turn.</summary>
        public byte BurnDamageDenominator
        {
            get => _burnDamageDenominator;
            set
            {
                ReadOnlyCheck();
                if (_burnDamageDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(BurnDamageDenominator)} must be at least 1.");
                    }
                    _burnDamageDenominator = value;
                    OnPropertyChanged(nameof(BurnDamageDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="PoisonDamageDenominator"/>.</summary>
        public const byte DefaultPoisonDamageDenominator = 8;
        private byte _poisonDamageDenominator = DefaultPoisonDamageDenominator;
        /// <summary>A Pokémon with <see cref="PBEStatus1.Poisoned"/> loses (1/this) of its HP at the end of every turn.</summary>
        public byte PoisonDamageDenominator
        {
            get => _poisonDamageDenominator;
            set
            {
                ReadOnlyCheck();
                if (_poisonDamageDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(PoisonDamageDenominator)} must be at least 1.");
                    }
                    _poisonDamageDenominator = value;
                    OnPropertyChanged(nameof(PoisonDamageDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="ToxicDamageDenominator"/>.</summary>
        public const byte DefaultToxicDamageDenominator = 16;
        private byte _toxicDamageDenominator = DefaultToxicDamageDenominator;
        /// <summary>A Pokémon with <see cref="PBEStatus1.BadlyPoisoned"/> loses (<see cref="PBEPokemon.Status1Counter"/>/this) of its HP at the end of every turn.</summary>
        public byte ToxicDamageDenominator
        {
            get => _toxicDamageDenominator;
            set
            {
                ReadOnlyCheck();
                if (_toxicDamageDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ToxicDamageDenominator)} must be at least 1.");
                    }
                    _toxicDamageDenominator = value;
                    OnPropertyChanged(nameof(ToxicDamageDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="LeechSeedDenominator"/>.</summary>
        public const byte DefaultLeechSeedDenominator = 8;
        private byte _leechSeedDenominator = DefaultLeechSeedDenominator;
        /// <summary>A Pokémon with <see cref="PBEStatus2.LeechSeed"/> loses (1/this) of its HP at the end of every turn and the Pokémon at <see cref="PBEPokemon.SeededPosition"/> on <see cref="PBEPokemon.SeededTeam"/> restores the lost HP.</summary>
        public byte LeechSeedDenominator
        {
            get => _leechSeedDenominator;
            set
            {
                ReadOnlyCheck();
                if (_leechSeedDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(LeechSeedDenominator)} must be at least 1.");
                    }
                    _leechSeedDenominator = value;
                    OnPropertyChanged(nameof(LeechSeedDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="CurseDenominator"/>.</summary>
        public const byte DefaultCurseDenominator = 4;
        private byte _curseDenominator = DefaultCurseDenominator;
        /// <summary>A Pokémon with <see cref="PBEStatus2.Cursed"/> loses (1/this) of its HP at the end of every turn.</summary>
        public byte CurseDenominator
        {
            get => _curseDenominator;
            set
            {
                ReadOnlyCheck();
                if (_curseDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(CurseDenominator)} must be at least 1.");
                    }
                    _curseDenominator = value;
                    OnPropertyChanged(nameof(CurseDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="LeftoversHealDenominator"/>.</summary>
        public const byte DefaultLeftoversHealDenominator = 16;
        private byte _leftoversHealDenominator = DefaultLeftoversHealDenominator;
        /// <summary>A Pokémon holding a <see cref="PBEItem.Leftovers"/> restores (1/this) of its HP at the end of every turn.</summary>
        public byte LeftoversHealDenominator
        {
            get => _leftoversHealDenominator;
            set
            {
                ReadOnlyCheck();
                if (_leftoversHealDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(LeftoversHealDenominator)} must be at least 1.");
                    }
                    _leftoversHealDenominator = value;
                    OnPropertyChanged(nameof(LeftoversHealDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="BlackSludgeDamageDenominator"/>.</summary>
        public const byte DefaultBlackSludgeDamageDenominator = 8;
        private byte _blackSludgeDamageDenominator = DefaultBlackSludgeDamageDenominator;
        /// <summary>A Pokémon holding a <see cref="PBEItem.BlackSludge"/> without <see cref="PBEType.Poison"/> loses (1/this) of its HP at the end of every turn.</summary>
        public byte BlackSludgeDamageDenominator
        {
            get => _blackSludgeDamageDenominator;
            set
            {
                ReadOnlyCheck();
                if (_blackSludgeDamageDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(BlackSludgeDamageDenominator)} must be at least 1.");
                    }
                    _blackSludgeDamageDenominator = value;
                    OnPropertyChanged(nameof(BlackSludgeDamageDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="BlackSludgeHealDenominator"/>.</summary>
        public const byte DefaultBlackSludgeHealDenominator = 16;
        private byte _blackSludgeHealDenominator = DefaultBlackSludgeHealDenominator;
        /// <summary>A Pokémon holding a <see cref="PBEItem.BlackSludge"/> with <see cref="PBEType.Poison"/> restores (1/this) of its HP at the end of every turn.</summary>
        public byte BlackSludgeHealDenominator
        {
            get => _blackSludgeHealDenominator;
            set
            {
                ReadOnlyCheck();
                if (_blackSludgeHealDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(BlackSludgeHealDenominator)} must be at least 1.");
                    }
                    _blackSludgeHealDenominator = value;
                    OnPropertyChanged(nameof(BlackSludgeHealDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="ReflectTurns"/>.</summary>
        public const byte DefaultReflectTurns = 5;
        private byte _reflectTurns = DefaultReflectTurns;
        /// <summary>The amount of turns <see cref="PBEMove.Reflect"/> lasts.</summary>
        public byte ReflectTurns
        {
            get => _reflectTurns;
            set
            {
                ReadOnlyCheck();
                if (_reflectTurns != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(ReflectTurns)} must be at least 1.");
                    }
                    _reflectTurns = value;
                    OnPropertyChanged(nameof(ReflectTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="LightScreenTurns"/>.</summary>
        public const byte DefaultLightScreenTurns = 5;
        private byte _lightScreenTurns = DefaultLightScreenTurns;
        /// <summary>The amount of turns <see cref="PBEMove.LightScreen"/> lasts.</summary>
        public byte LightScreenTurns
        {
            get => _lightScreenTurns;
            set
            {
                ReadOnlyCheck();
                if (_lightScreenTurns != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(LightScreenTurns)} must be at least 1.");
                    }
                    _lightScreenTurns = value;
                    OnPropertyChanged(nameof(LightScreenTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="LightClayTurnExtension"/>.</summary>
        public const byte DefaultLightClayTurnExtension = 3;
        private byte _lightClayTurnExtension = DefaultLightClayTurnExtension;
        /// <summary>The amount of turns added to <see cref="ReflectTurns"/> and <see cref="LightScreenTurns"/> when the user is holding a <see cref="PBEItem.LightClay"/>.</summary>
        public byte LightClayTurnExtension
        {
            get => _lightClayTurnExtension;
            set
            {
                ReadOnlyCheck();
                if (_lightClayTurnExtension != value)
                {
                    _lightClayTurnExtension = value;
                    OnPropertyChanged(nameof(LightClayTurnExtension));
                }
            }
        }
        /// <summary>The default value of <see cref="HailTurns"/>.</summary>
        public const byte DefaultHailTurns = 5;
        private byte _hailTurns = DefaultHailTurns;
        /// <summary>The amount of turns <see cref="PBEWeather.Hailstorm"/> lasts. For infinite turns, set <see cref="IcyRockTurnExtension"/> to 0 first, then this to 0.</summary>
        public byte HailTurns
        {
            get => _hailTurns;
            set
            {
                ReadOnlyCheck();
                if (_hailTurns != value)
                {
                    if (value == 0 && _icyRockTurnExtension != 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"For infinite turns, set {nameof(IcyRockTurnExtension)} to 0 first, then {nameof(HailTurns)} to 0.");
                    }
                    _hailTurns = value;
                    OnPropertyChanged(nameof(HailTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="HailDamageDenominator"/>.</summary>
        public const byte DefaultHailDamageDenominator = 16;
        private byte _hailDamageDenominator = DefaultHailDamageDenominator;
        /// <summary>A Pokémon in <see cref="PBEWeather.Hailstorm"/> loses (1/this) of its HP at the end of every turn.</summary>
        public byte HailDamageDenominator
        {
            get => _hailDamageDenominator;
            set
            {
                ReadOnlyCheck();
                if (_hailDamageDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(HailDamageDenominator)} must be at least 1.");
                    }
                    _hailDamageDenominator = value;
                    OnPropertyChanged(nameof(HailDamageDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="IcyRockTurnExtension"/>.</summary>
        public const byte DefaultIcyRockTurnExtension = 3;
        private byte _icyRockTurnExtension = DefaultIcyRockTurnExtension;
        /// <summary>The amount of turns added to <see cref="HailTurns"/> when the user is holding a <see cref="PBEItem.IcyRock"/>. If <see cref="HailTurns"/> is 0 (infinite turns), this must also be 0.</summary>
        public byte IcyRockTurnExtension
        {
            get => _icyRockTurnExtension;
            set
            {
                ReadOnlyCheck();
                if (_icyRockTurnExtension != value)
                {
                    if (value != 0 && _hailTurns == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"If {nameof(HailTurns)} is 0 (infinite turns), {nameof(IcyRockTurnExtension)} must also be 0.");
                    }
                    _icyRockTurnExtension = value;
                    OnPropertyChanged(nameof(IcyRockTurnExtension));
                }
            }
        }
        /// <summary>The default value of <see cref="IceBodyHealDenominator"/>.</summary>
        public const byte DefaultIceBodyHealDenominator = 16;
        private byte _iceBodyHealDenominator = DefaultIceBodyHealDenominator;
        /// <summary>A Pokémon with <see cref="PBEAbility.IceBody"/> in <see cref="PBEWeather.Hailstorm"/> restores (1/this) of its HP at the end of every turn.</summary>
        public byte IceBodyHealDenominator
        {
            get => _iceBodyHealDenominator;
            set
            {
                ReadOnlyCheck();
                if (_iceBodyHealDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(IceBodyHealDenominator)} must be at least 1.");
                    }
                    _iceBodyHealDenominator = value;
                    OnPropertyChanged(nameof(IceBodyHealDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="RainTurns"/>.</summary>
        public const byte DefaultRainTurns = 5;
        private byte _rainTurns = DefaultRainTurns;
        /// <summary>The amount of turns <see cref="PBEWeather.Rain"/> lasts. For infinite turns, set <see cref="DampRockTurnExtension"/> to 0 first, then this to 0.</summary>
        public byte RainTurns
        {
            get => _rainTurns;
            set
            {
                ReadOnlyCheck();
                if (_rainTurns != value)
                {
                    if (value == 0 && _dampRockTurnExtension != 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"For infinite turns, set {nameof(DampRockTurnExtension)} to 0 first, then {nameof(RainTurns)} to 0.");
                    }
                    _rainTurns = value;
                    OnPropertyChanged(nameof(RainTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="DampRockTurnExtension"/>.</summary>
        public const byte DefaultDampRockTurnExtension = 3;
        private byte _dampRockTurnExtension = DefaultDampRockTurnExtension;
        /// <summary>The amount of turns added to <see cref="RainTurns"/> when the user is holding a <see cref="PBEItem.DampRock"/>. If <see cref="RainTurns"/> is 0 (infinite turns), this must also be 0.</summary>
        public byte DampRockTurnExtension
        {
            get => _dampRockTurnExtension;
            set
            {
                ReadOnlyCheck();
                if (_dampRockTurnExtension != value)
                {
                    if (value != 0 && _rainTurns == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"If {nameof(RainTurns)} is 0 (infinite turns), {nameof(DampRockTurnExtension)} must also be 0.");
                    }
                    _dampRockTurnExtension = value;
                    OnPropertyChanged(nameof(DampRockTurnExtension));
                }
            }
        }
        /// <summary>The default value of <see cref="SandstormTurns"/>.</summary>
        public const byte DefaultSandstormTurns = 5;
        private byte _sandstormTurns = DefaultSandstormTurns;
        /// <summary>The amount of turns <see cref="PBEWeather.Sandstorm"/> lasts. For infinite turns, set <see cref="SmoothRockTurnExtension"/> to 0 first, then this to 0.</summary>
        public byte SandstormTurns
        {
            get => _sandstormTurns;
            set
            {
                ReadOnlyCheck();
                if (_sandstormTurns != value)
                {
                    if (value == 0 && _smoothRockTurnExtension != 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"For infinite turns, set {nameof(SmoothRockTurnExtension)} to 0 first, then {nameof(SandstormTurns)} to 0.");
                    }
                    _sandstormTurns = value;
                    OnPropertyChanged(nameof(SandstormTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="SandstormDamageDenominator"/>.</summary>
        public const byte DefaultSandstormDamageDenominator = 16;
        private byte _sandstormDamageDenominator = DefaultSandstormDamageDenominator;
        /// <summary>A Pokémon in <see cref="PBEWeather.Sandstorm"/> loses (1/this) of its HP at the end of every turn.</summary>
        public byte SandstormDamageDenominator
        {
            get => _sandstormDamageDenominator;
            set
            {
                ReadOnlyCheck();
                if (_sandstormDamageDenominator != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(SandstormDamageDenominator)} must be at least 1.");
                    }
                    _sandstormDamageDenominator = value;
                    OnPropertyChanged(nameof(SandstormDamageDenominator));
                }
            }
        }
        /// <summary>The default value of <see cref="SmoothRockTurnExtension"/>.</summary>
        public const byte DefaultSmoothRockTurnExtension = 3;
        private byte _smoothRockTurnExtension = DefaultSmoothRockTurnExtension;
        /// <summary>The amount of turns added to <see cref="SandstormTurns"/> when the user is holding a <see cref="PBEItem.SmoothRock"/>. If <see cref="SandstormTurns"/> is 0 (infinite turns), this must also be 0.</summary>
        public byte SmoothRockTurnExtension
        {
            get => _smoothRockTurnExtension;
            set
            {
                ReadOnlyCheck();
                if (_smoothRockTurnExtension != value)
                {
                    if (value != 0 && _sandstormTurns == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"If {nameof(SandstormTurns)} is 0 (infinite turns), {nameof(SmoothRockTurnExtension)} must also be 0.");
                    }
                    _smoothRockTurnExtension = value;
                    OnPropertyChanged(nameof(SmoothRockTurnExtension));
                }
            }
        }
        /// <summary>The default value of <see cref="SunTurns"/>.</summary>
        public const byte DefaultSunTurns = 5;
        private byte _sunTurns = DefaultSunTurns;
        /// <summary>The amount of turns <see cref="PBEWeather.HarshSunlight"/> lasts. For infinite turns, set <see cref="HeatRockTurnExtension"/> to 0 first, then this to 0.</summary>
        public byte SunTurns
        {
            get => _sunTurns;
            set
            {
                ReadOnlyCheck();
                if (_sunTurns != value)
                {
                    if (value == 0 && _heatRockTurnExtension != 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"For infinite turns, set {nameof(HeatRockTurnExtension)} to 0 first, then {nameof(SunTurns)} to 0.");
                    }
                    _sunTurns = value;
                    OnPropertyChanged(nameof(SunTurns));
                }
            }
        }
        /// <summary>The default value of <see cref="HeatRockTurnExtension"/>.</summary>
        public const byte DefaultHeatRockTurnExtension = 3;
        private byte _heatRockTurnExtension = DefaultHeatRockTurnExtension;
        /// <summary>The amount of turns added to <see cref="SunTurns"/> when the user is holding a <see cref="PBEItem.HeatRock"/>. If <see cref="SunTurns"/> is 0 (infinite turns), this must also be 0.</summary>
        public byte HeatRockTurnExtension
        {
            get => _heatRockTurnExtension;
            set
            {
                ReadOnlyCheck();
                if (_heatRockTurnExtension != value)
                {
                    if (value != 0 && _sunTurns == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"If {nameof(SunTurns)} is 0 (infinite turns), {nameof(HeatRockTurnExtension)} must also be 0.");
                    }
                    _heatRockTurnExtension = value;
                    OnPropertyChanged(nameof(HeatRockTurnExtension));
                }
            }
        }
        public const byte DefaultHiddenPowerMax = 70;
        private byte _hiddenPowerMax = DefaultHiddenPowerMax;
        /// <summary>The maximum base power of <see cref="PBEMove.HiddenPower"/>.</summary>
        public byte HiddenPowerMax
        {
            get => _hiddenPowerMax;
            set
            {
                ReadOnlyCheck();
                if (_hiddenPowerMax != value)
                {
                    if (value < _hiddenPowerMin)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(HiddenPowerMax)} must be at least {nameof(HiddenPowerMin)} ({_hiddenPowerMin}).");
                    }
                    _hiddenPowerMax = value;
                    OnPropertyChanged(nameof(HiddenPowerMax));
                }
            }
        }
        public const byte DefaultHiddenPowerMin = 30;
        private byte _hiddenPowerMin = DefaultHiddenPowerMin;
        /// <summary>The minimum base power of <see cref="PBEMove.HiddenPower"/>.</summary>
        public byte HiddenPowerMin
        {
            get => _hiddenPowerMin;
            set
            {
                ReadOnlyCheck();
                if (_hiddenPowerMin != value)
                {
                    if (value == 0 || value > _hiddenPowerMax)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(HiddenPowerMin)} must be at least 1 and cannot exceed {nameof(HiddenPowerMax)} ({_hiddenPowerMax}).");
                    }
                    _hiddenPowerMin = value;
                    OnPropertyChanged(nameof(HiddenPowerMin));
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
            using (var r = new EndianBinaryReader(new MemoryStream(Convert.FromBase64String(code)), encoding: EncodingType.UTF16))
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
            MaxLevel = other._maxLevel;
            MinLevel = other._minLevel;
            MaxPartySize = other._maxPartySize;
            MaxPokemonNameLength = other._maxPokemonNameLength;
            MaxTrainerNameLength = other._maxTrainerNameLength;
            MaxTotalEVs = other._maxTotalEVs;
            MaxIVs = other._maxIVs;
            NatureStatBoost = other._natureStatBoost;
            MaxStatChange = other._maxStatChange;
            NumMoves = other._numMoves;
            PPMultiplier = other._ppMultiplier;
            MaxPPUps = other._maxPPUps;
            CritMultiplier = other._critMultiplier;
            ConfusionMaxTurns = other._confusionMaxTurns;
            ConfusionMinTurns = other._confusionMinTurns;
            SleepMaxTurns = other._sleepMaxTurns;
            SleepMinTurns = other._sleepMinTurns;
            BurnDamageDenominator = other._burnDamageDenominator;
            PoisonDamageDenominator = other._poisonDamageDenominator;
            ToxicDamageDenominator = other._toxicDamageDenominator;
            LeechSeedDenominator = other._leechSeedDenominator;
            CurseDenominator = other._curseDenominator;
            LeftoversHealDenominator = other._leftoversHealDenominator;
            BlackSludgeDamageDenominator = other._blackSludgeDamageDenominator;
            BlackSludgeHealDenominator = other._blackSludgeHealDenominator;
            ReflectTurns = other._reflectTurns;
            LightScreenTurns = other._lightScreenTurns;
            LightClayTurnExtension = other._lightClayTurnExtension;
            HailTurns = other._hailTurns;
            HailDamageDenominator = other._hailDamageDenominator;
            IcyRockTurnExtension = other._icyRockTurnExtension;
            IceBodyHealDenominator = other._iceBodyHealDenominator;
            RainTurns = other._rainTurns;
            DampRockTurnExtension = other._dampRockTurnExtension;
            SandstormTurns = other._sandstormTurns;
            SandstormDamageDenominator = other._sandstormDamageDenominator;
            SmoothRockTurnExtension = other._smoothRockTurnExtension;
            SunTurns = other._sunTurns;
            HeatRockTurnExtension = other._heatRockTurnExtension;
            HiddenPowerMax = other._hiddenPowerMax;
            HiddenPowerMin = other._hiddenPowerMin;
        }
        internal PBESettings(EndianBinaryReader r)
        {
            FromBytes(r);
        }

        private void ReadOnlyCheck()
        {
            if (_isReadOnly)
            {
                throw new InvalidOperationException($"This {nameof(PBESettings)} is marked as read-only.");
            }
        }
        /// <summary>Marks this <see cref="PBESettings"/> object as read-only.</summary>
        public void MakeReadOnly()
        {
            if (!_isReadOnly)
            {
                IsReadOnly = true;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 31) + _maxLevel.GetHashCode();
                hash = (hash * 31) + _minLevel.GetHashCode();
                hash = (hash * 31) + _maxPartySize.GetHashCode();
                hash = (hash * 31) + _maxPokemonNameLength.GetHashCode();
                hash = (hash * 31) + _maxTrainerNameLength.GetHashCode();
                hash = (hash * 31) + _maxTotalEVs.GetHashCode();
                hash = (hash * 31) + _maxIVs.GetHashCode();
                hash = (hash * 31) + _natureStatBoost.GetHashCode();
                hash = (hash * 31) + _maxStatChange.GetHashCode();
                hash = (hash * 31) + _numMoves.GetHashCode();
                hash = (hash * 31) + _ppMultiplier.GetHashCode();
                hash = (hash * 31) + _maxPPUps.GetHashCode();
                hash = (hash * 31) + _critMultiplier.GetHashCode();
                hash = (hash * 31) + _confusionMaxTurns.GetHashCode();
                hash = (hash * 31) + _confusionMinTurns.GetHashCode();
                hash = (hash * 31) + _sleepMaxTurns.GetHashCode();
                hash = (hash * 31) + _sleepMinTurns.GetHashCode();
                hash = (hash * 31) + _burnDamageDenominator.GetHashCode();
                hash = (hash * 31) + _poisonDamageDenominator.GetHashCode();
                hash = (hash * 31) + _toxicDamageDenominator.GetHashCode();
                hash = (hash * 31) + _leechSeedDenominator.GetHashCode();
                hash = (hash * 31) + _curseDenominator.GetHashCode();
                hash = (hash * 31) + _leftoversHealDenominator.GetHashCode();
                hash = (hash * 31) + _blackSludgeDamageDenominator.GetHashCode();
                hash = (hash * 31) + _blackSludgeHealDenominator.GetHashCode();
                hash = (hash * 31) + _reflectTurns.GetHashCode();
                hash = (hash * 31) + _lightScreenTurns.GetHashCode();
                hash = (hash * 31) + _lightClayTurnExtension.GetHashCode();
                hash = (hash * 31) + _hailTurns.GetHashCode();
                hash = (hash * 31) + _hailDamageDenominator.GetHashCode();
                hash = (hash * 31) + _icyRockTurnExtension.GetHashCode();
                hash = (hash * 31) + _iceBodyHealDenominator.GetHashCode();
                hash = (hash * 31) + _rainTurns.GetHashCode();
                hash = (hash * 31) + _dampRockTurnExtension.GetHashCode();
                hash = (hash * 31) + _sandstormTurns.GetHashCode();
                hash = (hash * 31) + _sandstormDamageDenominator.GetHashCode();
                hash = (hash * 31) + _smoothRockTurnExtension.GetHashCode();
                hash = (hash * 31) + _sunTurns.GetHashCode();
                hash = (hash * 31) + _heatRockTurnExtension.GetHashCode();
                return hash;
            }
        }
        /// <summary>Returns a value indicating whether a code <see cref="string"/> or another <see cref="PBESettings"/> object represents the same settings as this <see cref="PBESettings"/> object.</summary>
        /// <param name="obj">The code <see cref="string"/> or the <see cref="PBESettings"/> object to check for equality.</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            else if (ReferenceEquals(obj, this))
            {
                return true;
            }
            else if (obj is string str)
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
                return other._maxLevel.Equals(_maxLevel)
                    && other._minLevel.Equals(_minLevel)
                    && other._maxPartySize.Equals(_maxPartySize)
                    && other._maxPokemonNameLength.Equals(_maxPokemonNameLength)
                    && other._maxTrainerNameLength.Equals(_maxTrainerNameLength)
                    && other._maxTotalEVs.Equals(_maxTotalEVs)
                    && other._maxIVs.Equals(_maxIVs)
                    && other._natureStatBoost.Equals(_natureStatBoost)
                    && other._maxStatChange.Equals(_maxStatChange)
                    && other._numMoves.Equals(_numMoves)
                    && other._ppMultiplier.Equals(_ppMultiplier)
                    && other._maxPPUps.Equals(_maxPPUps)
                    && other._critMultiplier.Equals(_critMultiplier)
                    && other._confusionMaxTurns.Equals(_confusionMaxTurns)
                    && other._confusionMinTurns.Equals(_confusionMinTurns)
                    && other._sleepMaxTurns.Equals(_sleepMaxTurns)
                    && other._sleepMinTurns.Equals(_sleepMinTurns)
                    && other._burnDamageDenominator.Equals(_burnDamageDenominator)
                    && other._poisonDamageDenominator.Equals(_poisonDamageDenominator)
                    && other._toxicDamageDenominator.Equals(_toxicDamageDenominator)
                    && other._leechSeedDenominator.Equals(_leechSeedDenominator)
                    && other._curseDenominator.Equals(_curseDenominator)
                    && other._leftoversHealDenominator.Equals(_leftoversHealDenominator)
                    && other._blackSludgeDamageDenominator.Equals(_blackSludgeDamageDenominator)
                    && other._blackSludgeHealDenominator.Equals(_blackSludgeHealDenominator)
                    && other._reflectTurns.Equals(_reflectTurns)
                    && other._lightScreenTurns.Equals(_lightScreenTurns)
                    && other._lightClayTurnExtension.Equals(_lightClayTurnExtension)
                    && other._hailTurns.Equals(_hailTurns)
                    && other._hailDamageDenominator.Equals(_hailDamageDenominator)
                    && other._icyRockTurnExtension.Equals(_icyRockTurnExtension)
                    && other._iceBodyHealDenominator.Equals(_iceBodyHealDenominator)
                    && other._rainTurns.Equals(_rainTurns)
                    && other._dampRockTurnExtension.Equals(_dampRockTurnExtension)
                    && other._sandstormTurns.Equals(_sandstormTurns)
                    && other._sandstormDamageDenominator.Equals(_sandstormDamageDenominator)
                    && other._smoothRockTurnExtension.Equals(_smoothRockTurnExtension)
                    && other._sunTurns.Equals(_sunTurns)
                    && other._heatRockTurnExtension.Equals(_heatRockTurnExtension)
                    && other._hiddenPowerMax.Equals(_hiddenPowerMax)
                    && other._hiddenPowerMin.Equals(_hiddenPowerMin);
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
            HeatRockTurnExtension,
            HiddenPowerMax,
            HiddenPowerMin
        }

        /// <summary>Converts this <see cref="PBESettings"/> object into a unique code <see cref="string"/>.</summary>
        public override string ToString()
        {
            return Convert.ToBase64String(ToBytes());
        }

        internal byte[] ToBytes()
        {
            byte[] data;
            ushort numChanged = 0;
            using (var ms = new MemoryStream())
            using (var w = new EndianBinaryWriter(ms, encoding: EncodingType.UTF16))
            {
                if (_maxLevel != DefaultMaxLevel)
                {
                    w.Write(PBESettingID.MaxLevel);
                    w.Write(_maxLevel);
                    numChanged++;
                }
                if (_minLevel != DefaultMinLevel)
                {
                    w.Write(PBESettingID.MinLevel);
                    w.Write(_minLevel);
                    numChanged++;
                }
                if (_maxPartySize != DefaultMaxPartySize)
                {
                    w.Write(PBESettingID.MaxPartySize);
                    w.Write(_maxPartySize);
                    numChanged++;
                }
                if (_maxPokemonNameLength != DefaultMaxPokemonNameLength)
                {
                    w.Write(PBESettingID.MaxPokemonNameLength);
                    w.Write(_maxPokemonNameLength);
                    numChanged++;
                }
                if (_maxTrainerNameLength != DefaultMaxTrainerNameLength)
                {
                    w.Write(PBESettingID.MaxTrainerNameLength);
                    w.Write(_maxTrainerNameLength);
                    numChanged++;
                }
                if (_maxTotalEVs != DefaultMaxTotalEVs)
                {
                    w.Write(PBESettingID.MaxTotalEVs);
                    w.Write(_maxTotalEVs);
                    numChanged++;
                }
                if (_maxIVs != DefaultMaxIVs)
                {
                    w.Write(PBESettingID.MaxIVs);
                    w.Write(_maxIVs);
                    numChanged++;
                }
                if (_natureStatBoost != DefaultNatureStatBoost)
                {
                    w.Write(PBESettingID.NatureStatBoost);
                    w.Write(_natureStatBoost);
                    numChanged++;
                }
                if (_maxStatChange != DefaultMaxStatChange)
                {
                    w.Write(PBESettingID.MaxStatChange);
                    w.Write(_maxStatChange);
                    numChanged++;
                }
                if (_numMoves != DefaultNumMoves)
                {
                    w.Write(PBESettingID.NumMoves);
                    w.Write(_numMoves);
                    numChanged++;
                }
                if (_ppMultiplier != DefaultPPMultiplier)
                {
                    w.Write(PBESettingID.PPMultiplier);
                    w.Write(_ppMultiplier);
                    numChanged++;
                }
                if (_maxPPUps != DefaultMaxPPUps)
                {
                    w.Write(PBESettingID.MaxPPUps);
                    w.Write(_maxPPUps);
                    numChanged++;
                }
                if (_critMultiplier != DefaultCritMultiplier)
                {
                    w.Write(PBESettingID.CritMultiplier);
                    w.Write(_critMultiplier);
                    numChanged++;
                }
                if (_confusionMaxTurns != DefaultConfusionMaxTurns)
                {
                    w.Write(PBESettingID.ConfusionMaxTurns);
                    w.Write(_confusionMaxTurns);
                    numChanged++;
                }
                if (_confusionMinTurns != DefaultConfusionMinTurns)
                {
                    w.Write(PBESettingID.ConfusionMinTurns);
                    w.Write(_confusionMinTurns);
                    numChanged++;
                }
                if (_sleepMaxTurns != DefaultSleepMaxTurns)
                {
                    w.Write(PBESettingID.SleepMaxTurns);
                    w.Write(_sleepMaxTurns);
                    numChanged++;
                }
                if (_sleepMinTurns != DefaultSleepMinTurns)
                {
                    w.Write(PBESettingID.SleepMinTurns);
                    w.Write(_sleepMinTurns);
                    numChanged++;
                }
                if (_burnDamageDenominator != DefaultBurnDamageDenominator)
                {
                    w.Write(PBESettingID.BurnDamageDenominator);
                    w.Write(_burnDamageDenominator);
                    numChanged++;
                }
                if (_poisonDamageDenominator != DefaultPoisonDamageDenominator)
                {
                    w.Write(PBESettingID.PoisonDamageDenominator);
                    w.Write(_poisonDamageDenominator);
                    numChanged++;
                }
                if (_toxicDamageDenominator != DefaultToxicDamageDenominator)
                {
                    w.Write(PBESettingID.ToxicDamageDenominator);
                    w.Write(_toxicDamageDenominator);
                    numChanged++;
                }
                if (_leechSeedDenominator != DefaultLeechSeedDenominator)
                {
                    w.Write(PBESettingID.LeechSeedDenominator);
                    w.Write(_leechSeedDenominator);
                    numChanged++;
                }
                if (_curseDenominator != DefaultCurseDenominator)
                {
                    w.Write(PBESettingID.CurseDenominator);
                    w.Write(_curseDenominator);
                    numChanged++;
                }
                if (_leftoversHealDenominator != DefaultLeftoversHealDenominator)
                {
                    w.Write(PBESettingID.LeftoversHealDenominator);
                    w.Write(_leftoversHealDenominator);
                    numChanged++;
                }
                if (_blackSludgeDamageDenominator != DefaultBlackSludgeDamageDenominator)
                {
                    w.Write(PBESettingID.BlackSludgeDamageDenominator);
                    w.Write(_blackSludgeDamageDenominator);
                    numChanged++;
                }
                if (_blackSludgeHealDenominator != DefaultBlackSludgeHealDenominator)
                {
                    w.Write(PBESettingID.BlackSludgeHealDenominator);
                    w.Write(_blackSludgeHealDenominator);
                    numChanged++;
                }
                if (_reflectTurns != DefaultReflectTurns)
                {
                    w.Write(PBESettingID.ReflectTurns);
                    w.Write(_reflectTurns);
                    numChanged++;
                }
                if (_lightScreenTurns != DefaultLightScreenTurns)
                {
                    w.Write(PBESettingID.LightScreenTurns);
                    w.Write(_lightScreenTurns);
                    numChanged++;
                }
                if (_lightClayTurnExtension != DefaultLightClayTurnExtension)
                {
                    w.Write(PBESettingID.LightClayTurnExtension);
                    w.Write(_lightClayTurnExtension);
                    numChanged++;
                }
                if (_hailTurns != DefaultHailTurns)
                {
                    w.Write(PBESettingID.HailTurns);
                    w.Write(_hailTurns);
                    numChanged++;
                }
                if (_hailDamageDenominator != DefaultHailDamageDenominator)
                {
                    w.Write(PBESettingID.HailDamageDenominator);
                    w.Write(_hailDamageDenominator);
                    numChanged++;
                }
                if (_icyRockTurnExtension != DefaultIcyRockTurnExtension)
                {
                    w.Write(PBESettingID.IcyRockTurnExtension);
                    w.Write(_icyRockTurnExtension);
                    numChanged++;
                }
                if (_iceBodyHealDenominator != DefaultIceBodyHealDenominator)
                {
                    w.Write(PBESettingID.IceBodyHealDenominator);
                    w.Write(_iceBodyHealDenominator);
                    numChanged++;
                }
                if (_rainTurns != DefaultRainTurns)
                {
                    w.Write(PBESettingID.RainTurns);
                    w.Write(_rainTurns);
                    numChanged++;
                }
                if (_dampRockTurnExtension != DefaultDampRockTurnExtension)
                {
                    w.Write(PBESettingID.DampRockTurnExtension);
                    w.Write(_dampRockTurnExtension);
                    numChanged++;
                }
                if (_sandstormTurns != DefaultSandstormTurns)
                {
                    w.Write(PBESettingID.SandstormTurns);
                    w.Write(_sandstormTurns);
                    numChanged++;
                }
                if (_sandstormDamageDenominator != DefaultSandstormDamageDenominator)
                {
                    w.Write(PBESettingID.SandstormDamageDenominator);
                    w.Write(_sandstormDamageDenominator);
                    numChanged++;
                }
                if (_smoothRockTurnExtension != DefaultSmoothRockTurnExtension)
                {
                    w.Write(PBESettingID.SmoothRockTurnExtension);
                    w.Write(_smoothRockTurnExtension);
                    numChanged++;
                }
                if (_sunTurns != DefaultSunTurns)
                {
                    w.Write(PBESettingID.SunTurns);
                    w.Write(_sunTurns);
                    numChanged++;
                }
                if (_heatRockTurnExtension != DefaultHeatRockTurnExtension)
                {
                    w.Write(PBESettingID.HeatRockTurnExtension);
                    w.Write(_heatRockTurnExtension);
                    numChanged++;
                }
                if (_hiddenPowerMax != DefaultHiddenPowerMax)
                {
                    w.Write(PBESettingID.HiddenPowerMax);
                    w.Write(_hiddenPowerMax);
                    numChanged++;
                }
                if (_hiddenPowerMin != DefaultHiddenPowerMin)
                {
                    w.Write(PBESettingID.HiddenPowerMin);
                    w.Write(_hiddenPowerMin);
                    numChanged++;
                }
                data = ms.ToArray();
            }
            byte[] ret = new byte[data.Length + 2];
            ret[0] = (byte)(numChanged & 0xFF); // Convert numChanged to little endian each time regardless of system endianness
            ret[1] = (byte)(numChanged >> 8);
            Buffer.BlockCopy(data, 0, ret, 2, data.Length);
            return ret;
        }
        private void FromBytes(EndianBinaryReader r)
        {
            ushort numChanged = r.ReadUInt16();
            for (ushort i = 0; i < numChanged; i++)
            {
                switch (r.ReadEnum<PBESettingID>())
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
                    case PBESettingID.HiddenPowerMax: HiddenPowerMax = r.ReadByte(); break;
                    case PBESettingID.HiddenPowerMin: HiddenPowerMin = r.ReadByte(); break;
                    default: throw new InvalidDataException();
                }
            }
        }
    }
}
