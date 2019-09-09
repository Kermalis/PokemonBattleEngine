using Avalonia.Media;
using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    public class MoveInfo
    {
        private static Dictionary<PBEType, (SolidColorBrush Brush, SolidColorBrush BorderBrush)> typeToBrush;
        public static void CreateBrushes()
        {
            if (typeToBrush == null)
            {
                typeToBrush = new Dictionary<PBEType, (SolidColorBrush Brush, SolidColorBrush BorderBrush)>
                {
                    { PBEType.Bug, (new SolidColorBrush(Color.FromRgb(173, 189, 31)), new SolidColorBrush(Color.FromRgb(66, 107, 57))) },
                    { PBEType.Dark, (new SolidColorBrush(Color.FromRgb(115, 90, 74)), new SolidColorBrush(Color.FromRgb(74, 57, 49))) },
                    { PBEType.Dragon, (new SolidColorBrush(Color.FromRgb(123, 99, 231)), new SolidColorBrush(Color.FromRgb(74, 57, 148))) },
                    { PBEType.Electric, (new SolidColorBrush(Color.FromRgb(255, 198, 49)), new SolidColorBrush(Color.FromRgb(115, 82, 24))) },
                    { PBEType.Fighting, (new SolidColorBrush(Color.FromRgb(165, 82, 57)), new SolidColorBrush(Color.FromRgb(74, 57, 49))) },
                    { PBEType.Fire, (new SolidColorBrush(Color.FromRgb(247, 82, 49)), new SolidColorBrush(Color.FromRgb(115, 33, 8))) },
                    { PBEType.Flying, (new SolidColorBrush(Color.FromRgb(156, 173, 247)), new SolidColorBrush(Color.FromRgb(66, 82, 148))) },
                    { PBEType.Ghost, (new SolidColorBrush(Color.FromRgb(99, 99, 181)), new SolidColorBrush(Color.FromRgb(74, 57, 82))) },
                    { PBEType.Grass, (new SolidColorBrush(Color.FromRgb(123, 206, 82)), new SolidColorBrush(Color.FromRgb(66, 107, 57))) },
                    { PBEType.Ground, (new SolidColorBrush(Color.FromRgb(214, 181, 90)), new SolidColorBrush(Color.FromRgb(115, 82, 24))) },
                    { PBEType.Ice, (new SolidColorBrush(Color.FromRgb(90, 206, 231)), new SolidColorBrush(Color.FromRgb(66, 82, 148))) },
                    { PBEType.Normal, (new SolidColorBrush(Color.FromRgb(173, 165, 148)), new SolidColorBrush(Color.FromRgb(82, 82, 82))) },
                    { PBEType.Poison, (new SolidColorBrush(Color.FromRgb(181, 90, 165)), new SolidColorBrush(Color.FromRgb(74, 57, 82))) },
                    { PBEType.Psychic, (new SolidColorBrush(Color.FromRgb(255, 115, 165)), new SolidColorBrush(Color.FromRgb(107, 57, 57))) },
                    { PBEType.Rock, (new SolidColorBrush(Color.FromRgb(189, 165, 90)), new SolidColorBrush(Color.FromRgb(115, 82, 24))) },
                    { PBEType.Steel, (new SolidColorBrush(Color.FromRgb(173, 173, 198)), new SolidColorBrush(Color.FromRgb(82, 82, 82))) },
                    { PBEType.Water, (new SolidColorBrush(Color.FromRgb(57, 156, 255)), new SolidColorBrush(Color.FromRgb(66, 82, 148))) }
                };
            }
        }

        public PBEMove Move { get; }
        public IBrush Brush { get; }
        public IBrush BorderBrush { get; }
        public string Description { get; }
        public ReactiveCommand SelectMoveCommand { get; }

        public MoveInfo(PBEPokemon pkmn, PBEMove move, Action<PBEMove> clickAction)
        {
            Move = move;
            (SolidColorBrush Brush, SolidColorBrush BorderBrush) ttb = typeToBrush[pkmn.GetMoveType(move)];
            Brush = ttb.Brush;
            BorderBrush = ttb.BorderBrush;

            var sb = new StringBuilder();
            if (move != PBEMove.None)
            {
                PBEMoveData mData = PBEMoveData.Data[move];
                sb.AppendLine($"Type: {PBELocalizedString.GetTypeName(mData.Type).ToString()}");
                sb.AppendLine($"Category: {mData.Category}");
                int moveIndex = Array.IndexOf(pkmn.Moves, move);
                if (moveIndex != -1)
                {
                    sb.AppendLine($"PP: {pkmn.PP[moveIndex]}/{pkmn.MaxPP[moveIndex]}");
                }
                sb.AppendLine($"Priority: {mData.Priority}");
                sb.AppendLine($"Power: {(mData.Power == 0 ? "--" : mData.Power.ToString())}");
                sb.AppendLine($"Accuracy: {(mData.Accuracy == 0 ? "--" : mData.Accuracy.ToString())}");
                switch (mData.Effect)
                {
                    case PBEMoveEffect.FlareBlitz: sb.AppendLine("Recoil: 1/3 damage dealt"); break; // TODO: Burn chance
                    case PBEMoveEffect.Recoil: sb.AppendLine($"Recoil: 1/{mData.EffectParam} damage dealt"); break;
                    case PBEMoveEffect.Struggle: sb.AppendLine("Recoil: 1/4 user's max HP"); break;
                    case PBEMoveEffect.VoltTackle: sb.AppendLine("Recoil: 1/3 damage dealt"); break; // TODO: Paralyze chance
                }
                sb.AppendLine($"Targets: {mData.Targets}");
                sb.AppendLine($"Flags: {mData.Flags}");
                sb.AppendLine();
                sb.Append(PBELocalizedString.GetMoveDescription(move).ToString().Replace('\n', ' '));
            }
            Description = sb.ToString();

            SelectMoveCommand = ReactiveCommand.Create(() => clickAction(move));
        }
    }
}
