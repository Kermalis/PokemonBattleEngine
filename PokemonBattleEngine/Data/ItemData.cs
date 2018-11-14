using System.Collections.Generic;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed class PItemData
    {
        public byte FlingPower;

        public static Dictionary<PItem, PItemData> Data = new Dictionary<PItem, PItemData>()
        {
            { PItem.BrightPowder, new PItemData { FlingPower = 10 } },
            { PItem.BurnDrive, new PItemData { FlingPower = 70 } },
            { PItem.ChillDrive, new PItemData { FlingPower = 70 } },
            { PItem.ChoiceBand, new PItemData { FlingPower = 10 } },
            { PItem.DampRock, new PItemData { FlingPower = 60 } },
            { PItem.DeepSeaScale, new PItemData { FlingPower = 30 } },
            { PItem.DeepSeaTooth, new PItemData { FlingPower = 90 } },
            { PItem.DouseDrive, new PItemData { FlingPower = 70 } },
            { PItem.LaxIncense, new PItemData { FlingPower = 10 } },
            { PItem.Leftovers, new PItemData { FlingPower = 10 } },
            { PItem.LightBall, new PItemData { FlingPower = 30 } },
            { PItem.LightClay, new PItemData { FlingPower = 30 } },
            { PItem.MetalPowder, new PItemData { FlingPower = 10 } },
            { PItem.PowerHerb, new PItemData { FlingPower = 10 } },
            { PItem.RazorClaw, new PItemData { FlingPower = 80 } },
            { PItem.ScopeLens, new PItemData { FlingPower = 30 } },
            { PItem.ShockDrive, new PItemData { FlingPower = 70 } },
            { PItem.SoulDew, new PItemData { FlingPower = 30 } },
            { PItem.ThickClub, new PItemData { FlingPower = 90 } },
            { PItem.WideLens, new PItemData { FlingPower = 10 } },
        };
    }
}
