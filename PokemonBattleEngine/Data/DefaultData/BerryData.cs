namespace Kermalis.PokemonBattleEngine.Data.DefaultData
{
    public sealed partial class PBEBerryData : IPBEBerryData
    {
        public byte Bitterness { get; }
        public byte Dryness { get; }
        public byte Sourness { get; }
        public byte Spicyness { get; }
        public byte Sweetness { get; }
        public byte NaturalGiftPower { get; }
        public PBEType NaturalGiftType { get; }

        private PBEBerryData(byte naturalGiftPower, PBEType naturalGiftType,
            byte bitterness = 0, byte dryness = 0, byte sourness = 0, byte spicyness = 0, byte sweetness = 0)
        {
            Bitterness = bitterness;
            Dryness = dryness;
            Sourness = sourness;
            Spicyness = spicyness;
            Sweetness = sweetness;
            NaturalGiftPower = naturalGiftPower;
            NaturalGiftType = naturalGiftType;
        }
    }
}
