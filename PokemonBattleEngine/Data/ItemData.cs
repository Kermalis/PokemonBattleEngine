using System.Collections.Generic;

namespace PokemonBattleEngine.Data
{
    class ItemData
    {
        public Effect Effect { get; private set; }
        public int EffectParam { get; private set; }
        public int FlingPower { get; private set; }

        public static Dictionary<Item, ItemData> Data = new Dictionary<Item, ItemData>()
        {
            {
                Item.ChoiceBand,
                new ItemData
                {
                    Effect = Effect.None, EffectParam = 0,
                    FlingPower = 10
                }
            },
            {
                Item.LightBall,
                new ItemData
                {
                    Effect = Effect.None, EffectParam = 0,
                    FlingPower = 30
                }
            },
            {
                Item.SoulDew,
                new ItemData
                {
                    Effect = Effect.None, EffectParam = 0,
                    FlingPower = 30
                }
            },
        };
    }
}
