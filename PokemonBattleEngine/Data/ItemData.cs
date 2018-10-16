using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Data
{
    class ItemData
    {
        public int FlingPower;

        public static Dictionary<Item, ItemData> Data = new Dictionary<Item, ItemData>()
        {
            { Item.ChoiceBand, new ItemData { FlingPower = 10 } },
            { Item.DeepSeaScale, new ItemData { FlingPower = 30 } },
            { Item.DeepSeaTooth, new ItemData { FlingPower = 90 } },
            { Item.Leftovers, new ItemData { FlingPower = 10 } },
            { Item.LightBall, new ItemData { FlingPower = 30 } },
            { Item.MetalPowder, new ItemData { FlingPower = 10 } },
            { Item.SoulDew, new ItemData { FlingPower = 30 } },
            { Item.ThickClub, new ItemData { FlingPower = 90 } },
        };
    }
}
