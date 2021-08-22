using Kermalis.PokemonBattleEngine.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data.Utils
{
    public static partial class PBEDataUtils
    {
        #region Static Collections

        public static PBEAlphabeticalList<PBEItem> AllItems { get; } = new(Enum.GetValues<PBEItem>());
        public static PBEAlphabeticalList<PBEItem> AllBalls { get; } = new(new[] { PBEItem.MasterBall, PBEItem.UltraBall, PBEItem.GreatBall, PBEItem.PokeBall,
            PBEItem.SafariBall, PBEItem.NetBall, PBEItem.DiveBall, PBEItem.NestBall, PBEItem.RepeatBall, PBEItem.TimerBall, PBEItem.LuxuryBall, PBEItem.PremierBall, PBEItem.DuskBall, PBEItem.HealBall,
            PBEItem.QuickBall, PBEItem.CherishBall, PBEItem.FastBall, PBEItem.LevelBall, PBEItem.LureBall, PBEItem.HeavyBall, PBEItem.LoveBall, PBEItem.FriendBall, PBEItem.MoonBall, PBEItem.SportBall,
            PBEItem.ParkBall, PBEItem.DreamBall });
        public static ReadOnlyDictionary<PBEType, PBEItem> TypeToGem { get; } = new(new Dictionary<PBEType, PBEItem>()
        {
            { PBEType.Bug, PBEItem.BugGem },
            { PBEType.Dark, PBEItem.DarkGem },
            { PBEType.Dragon, PBEItem.DragonGem },
            { PBEType.Electric, PBEItem.ElectricGem },
            { PBEType.Fighting, PBEItem.FightingGem },
            { PBEType.Fire, PBEItem.FireGem },
            { PBEType.Flying, PBEItem.FlyingGem },
            { PBEType.Ghost, PBEItem.GhostGem },
            { PBEType.Grass, PBEItem.GrassGem },
            { PBEType.Ground, PBEItem.GroundGem },
            { PBEType.Ice, PBEItem.IceGem },
            { PBEType.Normal, PBEItem.NormalGem },
            { PBEType.Poison, PBEItem.PoisonGem },
            { PBEType.Psychic, PBEItem.PsychicGem },
            { PBEType.Rock, PBEItem.RockGem },
            { PBEType.Steel, PBEItem.SteelGem },
            { PBEType.Water, PBEItem.WaterGem }
        });

        #region Form Items

        private static readonly PBEAlphabeticalList<PBEItem> _arceusItems = new(AllItems.Except(new[] { PBEItem.DracoPlate, PBEItem.DreadPlate, PBEItem.EarthPlate,
            PBEItem.FistPlate, PBEItem.FlamePlate, PBEItem.IciclePlate, PBEItem.InsectPlate, PBEItem.IronPlate, PBEItem.MeadowPlate, PBEItem.MindPlate, PBEItem.SkyPlate, PBEItem.SplashPlate,
            PBEItem.SpookyPlate, PBEItem.StonePlate, PBEItem.ToxicPlate, PBEItem.ZapPlate }));
        private static readonly PBEAlphabeticalList<PBEItem> _arceusBugItems = new(new[] { PBEItem.InsectPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusDarkItems = new(new[] { PBEItem.DreadPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusDragonItems = new(new[] { PBEItem.DracoPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusElectricItems = new(new[] { PBEItem.ZapPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusFightingItems = new(new[] { PBEItem.FistPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusFireItems = new(new[] { PBEItem.FlamePlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusFlyingItems = new(new[] { PBEItem.SkyPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusGhostItems = new(new[] { PBEItem.SpookyPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusGrassItems = new(new[] { PBEItem.MeadowPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusGroundItems = new(new[] { PBEItem.EarthPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusIceItems = new(new[] { PBEItem.IciclePlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusPoisonItems = new(new[] { PBEItem.ToxicPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusPsychicItems = new(new[] { PBEItem.MindPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusRockItems = new(new[] { PBEItem.StonePlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusSteelItems = new(new[] { PBEItem.IronPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _arceusWaterItems = new(new[] { PBEItem.SplashPlate });
        private static readonly PBEAlphabeticalList<PBEItem> _genesectItems = new(AllItems.Except(new[] { PBEItem.BurnDrive, PBEItem.ChillDrive, PBEItem.DouseDrive,
            PBEItem.ShockDrive }));
        private static readonly PBEAlphabeticalList<PBEItem> _genesectBurnItems = new(new[] { PBEItem.BurnDrive });
        private static readonly PBEAlphabeticalList<PBEItem> _genesectChillItems = new(new[] { PBEItem.ChillDrive });
        private static readonly PBEAlphabeticalList<PBEItem> _genesectDouseItems = new(new[] { PBEItem.DouseDrive });
        private static readonly PBEAlphabeticalList<PBEItem> _genesectShockItems = new(new[] { PBEItem.ShockDrive });
        private static readonly PBEAlphabeticalList<PBEItem> _giratinaItems = new(AllItems.ExceptOne(PBEItem.GriseousOrb));
        private static readonly PBEAlphabeticalList<PBEItem> _giratinaOriginItems = new(new[] { PBEItem.GriseousOrb });

        #endregion

        #endregion
    }
}
