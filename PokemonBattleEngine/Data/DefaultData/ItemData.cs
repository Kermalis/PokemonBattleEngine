namespace Kermalis.PokemonBattleEngine.Data.DefaultData
{
    public sealed partial class PBEItemData : IPBEItemData
    {
        public byte FlingPower { get; }

        private PBEItemData(byte flingPower = 0)
        {
            FlingPower = flingPower;
        }
    }
}
