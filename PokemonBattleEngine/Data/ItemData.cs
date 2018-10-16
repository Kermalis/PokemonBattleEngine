using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Data
{
    class ItemData
    {
        public int FlingPower;

        public static Dictionary<PItem, ItemData> Data = new Dictionary<PItem, ItemData>()
        {
            { PItem.ChoiceBand, new ItemData { FlingPower = 10 } },
            { PItem.DeepSeaScale, new ItemData { FlingPower = 30 } },
            { PItem.DeepSeaTooth, new ItemData { FlingPower = 90 } },
            { PItem.Leftovers, new ItemData { FlingPower = 10 } },
            { PItem.LightBall, new ItemData { FlingPower = 30 } },
            { PItem.MetalPowder, new ItemData { FlingPower = 10 } },
            { PItem.SoulDew, new ItemData { FlingPower = 30 } },
            { PItem.ThickClub, new ItemData { FlingPower = 90 } },
        };
    }
}
