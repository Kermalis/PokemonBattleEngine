using System;
using System.Collections.Generic;
using System.Linq;

namespace Kermalis.PokemonBattleEngine.Data.Utils
{
    public static partial class PBEDataUtils
    {
        #region Static Collections

        public static PBEAlphabeticalList<PBEMove> AllMoves { get; } = new(Enum.GetValues<PBEMove>().Except(new[] { PBEMove.None, PBEMove.MAX }));
        public static PBEAlphabeticalList<PBEMove> MetronomeMoves { get; } = new(GetMovesWithoutFlag(PBEMoveFlag.BlockedFromMetronome));
        public static PBEAlphabeticalList<PBEMove> SketchLegalMoves { get; } = new(GetMovesWithoutFlag(PBEMoveFlag.BlockedFromSketch, exception: PBEMoveEffect.Sketch));

        #endregion

        private static List<PBEMove> GetMovesWithoutFlag(PBEMoveFlag flag, PBEMoveEffect? exception = null)
        {
            return AllMoves.FindAll(m =>
            {
                IPBEMoveData mData = PBEDataProvider.Instance.GetMoveData(m, cache: false);
                if (!mData.IsMoveUsable())
                {
                    return false;
                }
                if (exception is not null && mData.Effect == exception.Value)
                {
                    return true;
                }
                return !mData.Flags.HasFlag(flag);
            });
        }

        public static bool HasSecondaryEffects(PBEMoveEffect effect, PBESettings settings)
        {
            settings.ShouldBeReadOnly(nameof(settings));
            switch (effect)
            {
                case PBEMoveEffect.Hit__MaybeBurn:
                case PBEMoveEffect.Hit__MaybeBurn__10PercentFlinch:
                case PBEMoveEffect.Hit__MaybeConfuse:
                case PBEMoveEffect.Hit__MaybeFlinch:
                case PBEMoveEffect.Hit__MaybeFreeze:
                case PBEMoveEffect.Hit__MaybeFreeze__10PercentFlinch:
                case PBEMoveEffect.Hit__MaybeLowerTarget_ACC_By1:
                case PBEMoveEffect.Hit__MaybeLowerTarget_ATK_By1:
                case PBEMoveEffect.Hit__MaybeLowerTarget_DEF_By1:
                case PBEMoveEffect.Hit__MaybeLowerTarget_SPATK_By1:
                case PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By1:
                case PBEMoveEffect.Hit__MaybeLowerTarget_SPDEF_By2:
                case PBEMoveEffect.Hit__MaybeLowerTarget_SPE_By1:
                case PBEMoveEffect.Hit__MaybeLowerUser_ATK_DEF_By1:
                case PBEMoveEffect.Hit__MaybeLowerUser_DEF_SPDEF_By1:
                case PBEMoveEffect.Hit__MaybeLowerUser_SPATK_By2:
                case PBEMoveEffect.Hit__MaybeLowerUser_SPE_By1:
                case PBEMoveEffect.Hit__MaybeLowerUser_SPE_DEF_SPDEF_By1:
                case PBEMoveEffect.Hit__MaybeParalyze:
                case PBEMoveEffect.Hit__MaybeParalyze__10PercentFlinch:
                case PBEMoveEffect.Hit__MaybePoison:
                case PBEMoveEffect.Hit__MaybeRaiseUser_ATK_By1:
                case PBEMoveEffect.Hit__MaybeRaiseUser_ATK_DEF_SPATK_SPDEF_SPE_By1:
                case PBEMoveEffect.Hit__MaybeRaiseUser_DEF_By1:
                case PBEMoveEffect.Hit__MaybeRaiseUser_SPATK_By1:
                case PBEMoveEffect.Hit__MaybeRaiseUser_SPE_By1:
                case PBEMoveEffect.Hit__MaybeToxic:
                case PBEMoveEffect.Snore: return true;
                // BUG: SecretPower is unaffected by SereneGrace and the Rainbow
                case PBEMoveEffect.SecretPower: return settings.BugFix;
                default: return false;
            }
        }
        public static bool IsHPDrainMove(PBEMoveEffect effect)
        {
            switch (effect)
            {
                case PBEMoveEffect.HPDrain:
                case PBEMoveEffect.HPDrain__RequireSleep: return true;
                default: return false;
            }
        }
        public static bool IsHPRestoreMove(PBEMoveEffect effect)
        {
            switch (effect)
            {
                case PBEMoveEffect.Rest:
                case PBEMoveEffect.RestoreTargetHP: return true;
                default: return false;
            }
        }
        public static bool IsMultiHitMove(PBEMoveEffect effect) // TODO: TripleKick
        {
            switch (effect)
            {
                case PBEMoveEffect.Hit__2Times:
                case PBEMoveEffect.Hit__2Times__MaybePoison:
                case PBEMoveEffect.Hit__2To5Times: return true;
                default: return false;
            }
        }
        public static bool IsRecoilMove(PBEMoveEffect effect) // TODO: JumpKick/HiJumpKick
        {
            switch (effect)
            {
                case PBEMoveEffect.Recoil:
                case PBEMoveEffect.Recoil__10PercentBurn:
                case PBEMoveEffect.Recoil__10PercentParalyze: return true;
                default: return false;
            }
        }
        public static bool IsSetDamageMove(PBEMoveEffect effect)
        {
            switch (effect)
            {
                case PBEMoveEffect.Endeavor:
                case PBEMoveEffect.FinalGambit:
                case PBEMoveEffect.OneHitKnockout:
                case PBEMoveEffect.Psywave:
                case PBEMoveEffect.SeismicToss:
                case PBEMoveEffect.SetDamage:
                case PBEMoveEffect.SuperFang: return true;
                default: return false;
            }
        }
        public static bool IsSpreadMove(PBEMoveTarget targets)
        {
            switch (targets)
            {
                case PBEMoveTarget.All:
                case PBEMoveTarget.AllFoes:
                case PBEMoveTarget.AllFoesSurrounding:
                case PBEMoveTarget.AllSurrounding:
                case PBEMoveTarget.AllTeam: return true;
                default: return false;
            }
        }
        public static bool IsWeatherMove(PBEMoveEffect effect)
        {
            switch (effect)
            {
                case PBEMoveEffect.Hail:
                case PBEMoveEffect.RainDance:
                case PBEMoveEffect.Sandstorm:
                case PBEMoveEffect.SunnyDay: return true;
                default: return false;
            }
        }

        /// <summary>Temporary check to see if a move is usable, can be removed once all moves are added</summary>
        public static bool IsMoveUsable(PBEMove move)
        {
            return PBEDataProvider.Instance.GetMoveData(move, cache: false).IsMoveUsable();
        }
        /// <summary>Temporary check to see if a move is usable, can be removed once all moves are added</summary>
        public static bool IsMoveUsable(PBEMoveEffect effect)
        {
            return effect != PBEMoveEffect.TODOMOVE && effect != PBEMoveEffect.Sketch;
        }
    }
}
