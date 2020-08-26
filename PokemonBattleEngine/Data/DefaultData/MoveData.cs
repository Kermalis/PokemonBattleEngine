using System;
using System.Text;

namespace Kermalis.PokemonBattleEngine.Data
{
    public sealed partial class PBEMoveData : IPBEMoveData
    {
        public PBEType Type { get; }
        public PBEMoveCategory Category { get; }
        public sbyte Priority { get; }
        public byte PPTier { get; }
        public byte Power { get; }
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

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Type: {Type}");
            sb.AppendLine($"Category: {Category}");
            sb.AppendLine($"Priority: {Priority}");
            sb.AppendLine($"PP: {Math.Max(1, PPTier * PBESettings.DefaultPPMultiplier)}");
            sb.AppendLine($"Power: {(Power == 0 ? "―" : Power.ToString())}");
            sb.AppendLine($"Accuracy: {(Accuracy == 0 ? "―" : Accuracy.ToString())}");
            sb.AppendLine($"Effect: {Effect}");
            sb.AppendLine($"Effect Parameter: {EffectParam}");
            sb.AppendLine($"Targets: {Targets}");
            sb.Append($"Flags: {Flags}");

            return sb.ToString();
        }
    }
}
