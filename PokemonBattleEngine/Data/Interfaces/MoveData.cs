namespace Kermalis.PokemonBattleEngine.Data
{
    public interface IPBEMoveData
    {
        PBEType Type { get; }
        PBEMoveCategory Category { get; }
        sbyte Priority { get; }
        /// <summary>0 PPTier will become 1 PP (unaffected by pp ups)</summary>
        byte PPTier { get; }
        /// <summary>0 power or accuracy will show up as --</summary>
        byte Power { get; }
        byte Accuracy { get; }
        PBEMoveEffect Effect { get; }
        int EffectParam { get; }
        PBEMoveTarget Targets { get; }
        PBEMoveFlag Flags { get; }
    }

    public static class PBEMoveDataExtensions
    {
        public static bool HasSecondaryEffects(this IPBEMoveData mData, PBESettings settings)
        {
            return PBEDataUtils.HasSecondaryEffects(mData.Effect, settings);
        }
        public static bool IsHPDrainMove(this IPBEMoveData mData)
        {
            return PBEDataUtils.IsHPDrainMove(mData.Effect);
        }
        public static bool IsHPRestoreMove(this IPBEMoveData mData)
        {
            return PBEDataUtils.IsHPRestoreMove(mData.Effect);
        }
        public static bool IsMultiHitMove(this IPBEMoveData mData)
        {
            return PBEDataUtils.IsMultiHitMove(mData.Effect);
        }
        public static bool IsRecoilMove(this IPBEMoveData mData)
        {
            return PBEDataUtils.IsRecoilMove(mData.Effect);
        }
        public static bool IsSetDamageMove(this IPBEMoveData mData)
        {
            return PBEDataUtils.IsSetDamageMove(mData.Effect);
        }
        public static bool IsSpreadMove(this IPBEMoveData mData)
        {
            return PBEDataUtils.IsSpreadMove(mData.Targets);
        }
        public static bool IsWeatherMove(this IPBEMoveData mData)
        {
            return PBEDataUtils.IsWeatherMove(mData.Effect);
        }

        public static bool IsMoveUsable(this IPBEMoveData mData)
        {
            return PBEDataUtils.IsMoveUsable(mData.Effect);
        }
    }
}
