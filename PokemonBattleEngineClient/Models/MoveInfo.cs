using Avalonia.Media;
using Kermalis.PokemonBattleEngine.Data;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    public class MoveInfo
    {
        static Dictionary<PBEType, Tuple<SolidColorBrush, SolidColorBrush>> typeToBrush;
        public static void CreateBrushes()
        {
            if (typeToBrush == null)
            {
                typeToBrush = new Dictionary<PBEType, Tuple<SolidColorBrush, SolidColorBrush>>
                {
                    { PBEType.Bug, Tuple.Create(new SolidColorBrush(Color.FromRgb(173, 189, 31)), new SolidColorBrush(Color.FromRgb(66, 107, 57))) },
                    { PBEType.Dark, Tuple.Create(new SolidColorBrush(Color.FromRgb(115, 90, 74)), new SolidColorBrush(Color.FromRgb(74, 57, 49))) },
                    { PBEType.Dragon, Tuple.Create(new SolidColorBrush(Color.FromRgb(123, 99, 231)), new SolidColorBrush(Color.FromRgb(74, 57, 148))) },
                    { PBEType.Electric, Tuple.Create(new SolidColorBrush(Color.FromRgb(255, 198, 49)), new SolidColorBrush(Color.FromRgb(115, 82, 24))) },
                    { PBEType.Fighting, Tuple.Create(new SolidColorBrush(Color.FromRgb(165, 82, 57)), new SolidColorBrush(Color.FromRgb(74, 57, 49))) },
                    { PBEType.Fire, Tuple.Create(new SolidColorBrush(Color.FromRgb(247, 82, 49)), new SolidColorBrush(Color.FromRgb(115, 33, 8))) },
                    { PBEType.Flying, Tuple.Create(new SolidColorBrush(Color.FromRgb(156, 173, 247)), new SolidColorBrush(Color.FromRgb(66, 82, 148))) },
                    { PBEType.Ghost, Tuple.Create(new SolidColorBrush(Color.FromRgb(99, 99, 181)), new SolidColorBrush(Color.FromRgb(74, 57, 82))) },
                    { PBEType.Grass, Tuple.Create(new SolidColorBrush(Color.FromRgb(123, 206, 82)), new SolidColorBrush(Color.FromRgb(66, 107, 57))) },
                    { PBEType.Ground, Tuple.Create(new SolidColorBrush(Color.FromRgb(214, 181, 90)), new SolidColorBrush(Color.FromRgb(115, 82, 24))) },
                    { PBEType.Ice, Tuple.Create(new SolidColorBrush(Color.FromRgb(90, 206, 231)), new SolidColorBrush(Color.FromRgb(66, 82, 148))) },
                    { PBEType.Normal, Tuple.Create(new SolidColorBrush(Color.FromRgb(173, 165, 148)), new SolidColorBrush(Color.FromRgb(82, 82, 82))) },
                    { PBEType.Poison, Tuple.Create(new SolidColorBrush(Color.FromRgb(181, 90, 165)), new SolidColorBrush(Color.FromRgb(74, 57, 82))) },
                    { PBEType.Psychic, Tuple.Create(new SolidColorBrush(Color.FromRgb(255, 115, 165)), new SolidColorBrush(Color.FromRgb(107, 57, 57))) },
                    { PBEType.Rock, Tuple.Create(new SolidColorBrush(Color.FromRgb(189, 165, 90)), new SolidColorBrush(Color.FromRgb(115, 82, 24))) },
                    { PBEType.Steel, Tuple.Create(new SolidColorBrush(Color.FromRgb(173, 173, 198)), new SolidColorBrush(Color.FromRgb(82, 82, 82))) },
                    { PBEType.Water, Tuple.Create(new SolidColorBrush(Color.FromRgb(57, 156, 255)), new SolidColorBrush(Color.FromRgb(66, 82, 148))) }
                };
            }
        }

        public ReactiveCommand SelectMoveCommand { get; }

        public PBEMove Move { get; }
        public IBrush Brush { get; }
        public IBrush BorderBrush { get; }
        public string Description { get; }

        public MoveInfo(int i, PBEPokemon pkmn, Action<MoveInfo> clickAction)
        {
            bool forcedToStruggle = pkmn.IsForcedToStruggle();
            PBEMove move = forcedToStruggle ? PBEMove.Struggle : pkmn.Moves[i];
            var ttb = move == PBEMove.None ? typeToBrush[PBEType.Normal] : typeToBrush[pkmn.GetMoveType(move)];

            bool enabled;
            if (forcedToStruggle)
            {
                enabled = true;
            }
            else if (pkmn.TempLockedMove != PBEMove.None)
            {
                enabled = pkmn.TempLockedMove == move;
            }
            else if (pkmn.ChoiceLockedMove != PBEMove.None)
            {
                enabled = pkmn.ChoiceLockedMove == move;
            }
            else
            {
                enabled = move != PBEMove.None && pkmn.PP[i] > 0;
            }
            Move = move;
            Brush = ttb.Item1;
            BorderBrush = ttb.Item2;
            Description = move == PBEMove.None ? string.Empty : PBEMoveData.Data[move].ToString();

            var sub = new Subject<bool>();
            SelectMoveCommand = ReactiveCommand.Create(() => clickAction(this), sub);
            sub.OnNext(enabled);
        }
    }
}
