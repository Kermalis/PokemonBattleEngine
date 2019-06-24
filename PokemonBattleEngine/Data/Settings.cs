using Kermalis.PokemonBattleEngine.Battle;
using System;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngine.Data
{
    // TODO: .Equals, struct?
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
        public static PBESettings DefaultSettings { get; } = new PBESettings(); // TODO: I wish I could make this constant somehow (it can be edited) (would a struct work?)

        private byte maxLevel;
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
        private byte minLevel;
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
        private sbyte maxPartySize;
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
        private byte maxPokemonNameLength;
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
        private byte maxTrainerNameLength;
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
        private ushort maxTotalEVs;
        /// <summary>The maximum sum of a Pokémon's EVs.</summary>
        public ushort MaxTotalEVs
        {
            get => maxTotalEVs;
            set
            {
                if (maxTotalEVs != value)
                {
                    maxTotalEVs = value;
                    OnPropertyChanged(nameof(MaxTotalEVs));
                }
            }
        }
        private byte maxIVs;
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
        private double natureStatBoost;
        /// <summary>The amount of influence a Pokémon's <see cref="PBENature"/> has on its stats.</summary>
        public double NatureStatBoost
        {
            get => natureStatBoost;
            set
            {
                if (natureStatBoost != value)
                {
                    natureStatBoost = value;
                    OnPropertyChanged(nameof(NatureStatBoost));
                }
            }
        }
        private sbyte maxStatChange;
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
        private byte numMoves;
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
        private byte ppMultiplier;
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
        private byte maxPPUps;
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
        private double critMultiplier;
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
        private byte confusionMinTurns;
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
        private byte confusionMaxTurns;
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
        private byte sleepMinTurns;
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
        private byte sleepMaxTurns;
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
        private byte burnDamageDenominator;
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
        private byte poisonDamageDenominator;
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
        private byte toxicDamageDenominator;
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
        private byte leechSeedDenominator;
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
        private byte curseDenominator;
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
        private byte leftoversHealDenominator;
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
        private byte blackSludgeDamageDenominator;
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
        private byte blackSludgeHealDenominator;
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
        private byte reflectTurns;
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
        private byte lightScreenTurns;
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
        private byte lightClayTurnExtension;
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
        private byte hailTurns;
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
        private byte hailDamageDenominator;
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
        private byte icyRockTurnExtension;
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
        private byte iceBodyHealDenominator;
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
        private byte rainTurns;
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
        private byte dampRockTurnExtension;
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
        private byte sandstormTurns;
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
        private byte sandstormDamageDenominator;
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
        private byte smoothRockTurnExtension;
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
        private byte sunTurns;
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
        private byte heatRockTurnExtension;
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
        public PBESettings()
        {
            maxLevel = 100;
            minLevel = 1;
            maxPartySize = 6;
            maxPokemonNameLength = 10;
            maxTrainerNameLength = 7; // English is 7, other languages are different
            maxTotalEVs = 510;
            maxIVs = 31;
            natureStatBoost = 0.1;
            maxStatChange = 6;
            numMoves = 4;
            ppMultiplier = 5;
            maxPPUps = 3;
            critMultiplier = 2.0;
            confusionMinTurns = 1;
            confusionMaxTurns = 4;
            sleepMinTurns = 1;
            sleepMaxTurns = 3;
            burnDamageDenominator = 8;
            poisonDamageDenominator = 8;
            toxicDamageDenominator = 16;
            leechSeedDenominator = 8;
            curseDenominator = 4;
            leftoversHealDenominator = 16;
            blackSludgeDamageDenominator = 8;
            blackSludgeHealDenominator = 16;
            reflectTurns = 5;
            lightScreenTurns = 5;
            lightClayTurnExtension = 3;
            hailTurns = 5;
            hailDamageDenominator = 16;
            icyRockTurnExtension = 3;
            iceBodyHealDenominator = 16;
            rainTurns = 5;
            dampRockTurnExtension = 3;
            sandstormTurns = 5;
            sandstormDamageDenominator = 16;
            smoothRockTurnExtension = 3;
            sunTurns = 5;
            heatRockTurnExtension = 3;
        }
    }
}
