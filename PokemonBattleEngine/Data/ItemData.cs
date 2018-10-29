using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PItemData
    {
        public int FlingPower;

        public static Dictionary<PItem, PItemData> Data = new Dictionary<PItem, PItemData>()
        {
            { PItem.ChoiceBand, new PItemData { FlingPower = 10 } },
            { PItem.DeepSeaScale, new PItemData { FlingPower = 30 } },
            { PItem.DeepSeaTooth, new PItemData { FlingPower = 90 } },
            { PItem.Leftovers, new PItemData { FlingPower = 10 } },
            { PItem.LightBall, new PItemData { FlingPower = 30 } },
            { PItem.MetalPowder, new PItemData { FlingPower = 10 } },
            { PItem.SoulDew, new PItemData { FlingPower = 30 } },
            { PItem.ThickClub, new PItemData { FlingPower = 90 } },
        };
    }
}
