using System;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed partial class PBEMoveData
    {
        public PBEType Type { get; }
        public PBEMoveCategory Category { get; }
        public sbyte Priority { get; }
        public byte PPTier { get; } // 0 PPTier will become 1 PP (unaffected by pp ups)
        public byte Power { get; } // 0 power or accuracy will show up as --
        public byte Accuracy { get; }
        public PBEMoveEffect Effect { get; }
        public int EffectParam { get; }
        public PBEMoveTarget Targets { get; }
        public PBEMoveFlag Flags { get; }

        private PBEMoveData(PBEType type, PBEMoveCategory category, sbyte priority, byte ppTier, byte power, byte accuracy,
            PBEMoveEffect effect, int effectParam, PBEMoveTarget targets,
            PBEMoveFlag flags)
        {
            Type = type; Category = category; Priority = priority; PPTier = ppTier; Power = power; Accuracy = accuracy;
            Effect = effect; EffectParam = effectParam; Targets = targets;
            Flags = flags;
        }

        public bool HasSecondaryEffects(PBESettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (!settings.IsReadOnly)
            {
                throw new ArgumentException("Settings must be read-only.", nameof(settings));
            }
            switch (Effect)
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
        public bool IsHPDrainMove()
        {
            switch (Effect)
            {
                case PBEMoveEffect.HPDrain:
                case PBEMoveEffect.HPDrain__RequireSleep: return true;
                default: return false;
            }
        }
        public bool IsHPRestoreMove()
        {
            switch (Effect)
            {
                case PBEMoveEffect.Rest:
                case PBEMoveEffect.RestoreTargetHP: return true;
                default: return false;
            }
        }
        public bool IsMultiHitMove() // TODO: TripleKick
        {
            switch (Effect)
            {
                case PBEMoveEffect.Hit__2Times:
                case PBEMoveEffect.Hit__2Times__MaybePoison:
                case PBEMoveEffect.Hit__2To5Times: return true;
                default: return false;
            }
        }
        public bool IsRecoilMove() // TODO: JumpKick/HiJumpKick
        {
            switch (Effect)
            {
                case PBEMoveEffect.Recoil:
                case PBEMoveEffect.Recoil__10PercentBurn:
                case PBEMoveEffect.Recoil__10PercentParalyze: return true;
                default: return false;
            }
        }
        public bool IsSetDamageMove()
        {
            switch (Effect)
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
        public bool IsSpreadMove()
        {
            return IsSpreadMove(Targets);
        }
        public bool IsWeatherMove()
        {
            switch (Effect)
            {
                case PBEMoveEffect.Hail:
                case PBEMoveEffect.RainDance:
                case PBEMoveEffect.Sandstorm:
                case PBEMoveEffect.SunnyDay: return true;
                default: return false;
            }
        }

        // Temporary check to see if a move is usable, can be removed once all moves are added
        public bool IsMoveUsable()
        {
            return Effect != PBEMoveEffect.TODOMOVE && Effect != PBEMoveEffect.Sketch;
        }
        public static bool IsMoveUsable(PBEMove move)
        {
            return Data[move].IsMoveUsable();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Type: {Type}");
            sb.AppendLine($"Category: {Category}");
            sb.AppendLine($"Priority: {Priority}");
            sb.AppendLine($"PP: {Math.Max(1, PPTier * PBESettings.DefaultPPMultiplier)}");
            sb.AppendLine($"Power: {(Power == 0 ? "--" : Power.ToString())}");
            sb.AppendLine($"Accuracy: {(Accuracy == 0 ? "--" : Accuracy.ToString())}");
            sb.AppendLine($"Effect: {Effect}");
            sb.AppendLine($"Effect Parameter: {EffectParam}");
            sb.AppendLine($"Targets: {Targets}");
            sb.Append($"Flags: {Flags}");

            return sb.ToString();
        }
    }
}
