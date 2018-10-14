using System.Collections.Generic;

namespace PokemonBattleEngine.Data
{
    class ItemData
    {
        public Effect Effect;
        public int EffectParam;
        public int FlingPower;

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
                Item.DeepSeaScale,
                new ItemData
                {
                    Effect = Effect.None, EffectParam = 0,
                    FlingPower = 30
                }
            },
            {
                Item.DeepSeaTooth,
                new ItemData
                {
                    Effect = Effect.None, EffectParam = 0,
                    FlingPower = 90
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
                Item.MetalPowder,
                new ItemData
                {
                    Effect = Effect.None, EffectParam = 0,
                    FlingPower = 10
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
            {
                Item.ThickClub,
                new ItemData
                {
                    Effect = Effect.None, EffectParam = 0,
                    FlingPower = 90
                }
            },
        };
    }
}
