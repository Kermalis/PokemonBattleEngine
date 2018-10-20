namespace Kermalis.PokemonBattleEngine.Data
{
    public static class PConstants
    {
        public const byte MaxLevel = 100;
        public const byte NumMoves = 4;
        public const byte MaxPartySize = 6;
        public const sbyte MaxStatChange = 6;

        public const byte BurnDamageDenominator = 8; // burnDamage = (MaxHP / BurnDamageDenominator)
        public const byte PoisonDamageDenominator = 8;
        public const byte ToxicDamageDenominator = 16;
    }
}
