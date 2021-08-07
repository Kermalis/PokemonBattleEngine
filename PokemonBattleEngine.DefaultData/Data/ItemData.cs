using Kermalis.PokemonBattleEngine.Data;

namespace Kermalis.PokemonBattleEngine.DefaultData.Data
{
    public sealed partial class PBEDDItemData : IPBEItemData
    {
        public byte FlingPower { get; }

        private PBEDDItemData(byte flingPower = 0)
        {
            FlingPower = flingPower;
        }
    }
}
