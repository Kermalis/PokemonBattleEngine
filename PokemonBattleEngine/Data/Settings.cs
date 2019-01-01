namespace Kermalis.PokemonBattleEngine.Data
{
    /// <summary>
    /// The various engine settings.
    /// </summary>
    public class PBESettings
    {
        /// <summary>
        /// The default settings used in official games.
        /// </summary>
        public static PBESettings DefaultSettings = new PBESettings();

        /// <summary>
        /// The maximum level a Pokémon can be. Used in stat calculation.
        /// </summary>
        public byte MaxLevel = 100;
        /// <summary>
        /// The maximum amount of Pokémon each team can bring into a battle.
        /// </summary>
        public sbyte MaxPartySize = 6;
        /// <summary>
        /// The maximum amount of characters a Pokémon name can have.
        /// </summary>
        public byte MaxPokemonNameLength = 10;
        /// <summary>
        /// The maximum amount of characters a trainer's name can have.
        /// </summary>
        [System.Obsolete]
        public byte MaxTrainerNameLength = 7;
        /// <summary>
        /// The maximum sum of a Pokémon's EVs.
        /// </summary>
        public ushort MaxTotalEVs = 510;
        /// <summary>
        /// The maximum IVs per stat a Pokémon can have.
        /// </summary>
        /// <remarks>
        /// Raising MaxIVs will not affect <see cref="PBEMove.HiddenPower"/>.
        /// </remarks>
        public byte MaxIVs = 31;
        /// <summary>
        /// The amount of influence a Pokémon's nature has in stat calculation.
        /// </summary>
        public double NatureStatBoost = 0.1;
        /// <summary>
        /// The maximum change a stat can have in the negative and positive direction.
        /// </summary>
        public sbyte MaxStatChange = 6;
        /// <summary>
        /// The maximum amount of moves a specific Pokémon can have.
        /// </summary>
        public byte NumMoves = 4;
        /// <summary>
        /// PPMultiplier affects the base PP of each move and the boost PP-Ups give.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Growl is a tier 8 move, so the maximum PP will be 64 ((tier * PPMultiplier) + (tier * PPUps)).
        /// </para>
        /// <para>
        /// <see cref="PBEMove.Transform"/> will change the max PP of each copied move to PPMultiplier.
        /// </para>
        /// </remarks>
        public byte PPMultiplier = 5;
        /// <summary>
        /// The maximum amount of PP-Ups that can be used on each of a Pokémon's moves.
        /// </summary>
        public byte MaxPPUps = 3;
        /// <summary>
        /// The damage boost awarded by critical hits.
        /// </summary>
        public double CritMultiplier = 2.0;
        /// <summary>
        /// The minimum amount of turns a Pokémon can be <see cref="PBEStatus2.Confused"/>.
        /// </summary>
        public byte ConfusionMinTurns = 1;
        /// <summary>
        /// The maximum amount of turns a Pokémon can be <see cref="PBEStatus2.Confused"/>.
        /// </summary>
        public byte ConfusionMaxTurns = 4;
        /// <summary>
        /// The minimum amount of turns a Pokémon can be <see cref="PBEStatus1.Asleep"/>.
        /// </summary>
        public byte SleepMinTurns = 1;
        /// <summary>
        /// The maximum amount of turns a Pokémon can be <see cref="PBEStatus1.Asleep"/>.
        /// </summary>
        public byte SleepMaxTurns = 3;
        /// <summary>
        /// A Pokémon with <see cref="PBEStatus1.Burned"/> loses (1/BurnDamageDenominator) HP at the end of every turn.
        /// </summary>
        public byte BurnDamageDenominator = 8;
        /// <summary>
        /// A Pokémon with <see cref="PBEStatus1.Poisoned"/> loses (1/PoisonDamageDenominator) HP at the end of every turn.
        /// </summary>
        public byte PoisonDamageDenominator = 8;
        /// <summary>
        /// A Pokémon with <see cref="PBEStatus1.BadlyPoisoned"/> loses (<see cref="PBEPokemon.Status1Counter"/>/ToxicDamageDenominator) HP at the end of every turn.
        /// </summary>
        public byte ToxicDamageDenominator = 16;
        /// <summary>
        /// (1/LeechSeedDenominator) HP is stolen from a Pokémon with <see cref="PBEStatus2.LeechSeed"/> at the end of every turn and the stolen HP goes to the foe at <see cref="PBEPokemon.SeededPosition"/>.
        /// No HP is lost if there is no foe at the position.
        /// </summary>
        public byte LeechSeedDenominator = 8;
        /// <summary>
        /// A Pokémon with <see cref="PBEStatus2.Cursed"/> loses (1/CurseDenominator) HP at the end of every turn.
        /// </summary>
        public byte CurseDenominator = 4;
        /// <summary>
        /// A Pokémon holding a <see cref="PBEItem.Leftovers"/> gains (1/LeftoversHealDenominator) HP at the end of every turn.
        /// </summary>
        public byte LeftoversHealDenominator = 16;
        /// <summary>
        /// A Pokémon holding a <see cref="PBEItem.BlackSludge"/> without <see cref="PBEType.Poison"/> loses (1/BlackSludgeDamageDenominator) HP at the end of every turn.
        /// </summary>
        public byte BlackSludgeDamageDenominator = 8;
        /// <summary>
        /// A Pokémon holding a <see cref="PBEItem.BlackSludge"/> with <see cref="PBEType.Poison"/> gains (1/BlackSludgeHealDenominator) HP at the end of every turn.
        /// </summary>
        public byte BlackSludgeHealDenominator = 16;
        /// <summary>
        /// The amount of turns <see cref="PBEMove.Reflect"/> lasts.
        /// </summary>
        public byte ReflectTurns = 5;
        /// <summary>
        /// The amount of turns <see cref="PBEMove.LightScreen"/> lasts.
        /// </summary>
        public byte LightScreenTurns = 5;
        /// <summary>
        /// The amount of turns added to <see cref="ReflectTurns"/> and <see cref="LightScreenTurns"/> when the user is holding a <see cref="PBEItem.LightClay"/>.
        /// </summary>
        public byte LightClayTurnExtension = 3;
        /// <summary>
        /// The amount of turns <see cref="PBEWeather.Hailstorm"/> lasts.
        /// </summary>
        public byte HailTurns = 5;
        /// <summary>
        /// A Pokémon in <see cref="PBEWeather.Hailstorm"/> loses (1/HailDamageDenominator) HP at the end of every turn.
        /// </summary>
        public byte HailDamageDenominator = 16;
        /// <summary>
        /// The amount of turns added to <see cref="HailTurns"/> when the user is holding a <see cref="PBEItem.IcyRock"/>.
        /// </summary>
        public byte IcyRockTurnExtension = 3;
        /// <summary>
        /// A Pokémon with <see cref="PBEAbility.IceBody"/> in <see cref="PBEWeather.Hailstorm"/> gains (1/IceBodyHealDenominator) HP at the end of every turn.
        /// </summary>
        public byte IceBodyHealDenominator = 16;
        /// <summary>
        /// The amount of turns <see cref="PBEWeather.Rain"/> lasts.
        /// </summary>
        public byte RainTurns = 5;
        /// <summary>
        /// The amount of turns added to <see cref="RainTurns"/> when the user is holding a <see cref="PBEItem.DampRock"/>.
        /// </summary>
        public byte DampRockTurnExtension = 3;
        /// <summary>
        /// The amount of turns <see cref="PBEWeather.Sandstorm"/> lasts.
        /// </summary>
        public byte SandstormTurns = 5;
        /// <summary>
        /// A Pokémon in <see cref="PBEWeather.Sandstorm"/> loses (1/SandstormDamageDenominator) HP at the end of every turn.
        /// </summary>
        public byte SandstormDamageDenominator = 16;
        /// <summary>
        /// The amount of turns added to <see cref="SandstormTurns"/> when the user is holding a <see cref="PBEItem.SmoothRock"/>.
        /// </summary>
        public byte SmoothRockTurnExtension = 3;
        /// <summary>
        /// The amount of turns <see cref="PBEWeather.HarshSunlight"/> lasts.
        /// </summary>
        public byte SunTurns = 5;
        /// <summary>
        /// The amount of turns added to <see cref="SunTurns"/> when the user is holding a <see cref="PBEItem.HeatRock"/>.
        /// </summary>
        public byte HeatRockTurnExtension = 3;
    }
}
