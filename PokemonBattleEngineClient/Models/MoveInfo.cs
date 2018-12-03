using Avalonia.Media;
using Avalonia.Media.Imaging;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Kermalis.PokemonBattleEngineClient.Models
{
    class MoveInfo
    {
        static Dictionary<PType, Tuple<SolidColorBrush, SolidColorBrush>> typeToBrush;
        public static void CreateBrushes()
        {
            if (typeToBrush == null)
            {
                typeToBrush = new Dictionary<PType, Tuple<SolidColorBrush, SolidColorBrush>>
                {
                    { PType.Bug, Tuple.Create(new SolidColorBrush(Color.FromRgb(173, 189, 31)), new SolidColorBrush(Color.FromRgb(66, 107, 57))) },
                    { PType.Dark, Tuple.Create(new SolidColorBrush(Color.FromRgb(115, 90, 74)), new SolidColorBrush(Color.FromRgb(74, 57, 49))) },
                    { PType.Dragon, Tuple.Create(new SolidColorBrush(Color.FromRgb(123, 99, 231)), new SolidColorBrush(Color.FromRgb(74, 57, 148))) },
                    { PType.Electric, Tuple.Create(new SolidColorBrush(Color.FromRgb(255, 198, 49)), new SolidColorBrush(Color.FromRgb(115, 82, 24))) },
                    { PType.Fighting, Tuple.Create(new SolidColorBrush(Color.FromRgb(165, 82, 57)), new SolidColorBrush(Color.FromRgb(74, 57, 49))) },
                    { PType.Fire, Tuple.Create(new SolidColorBrush(Color.FromRgb(247, 82, 49)), new SolidColorBrush(Color.FromRgb(115, 33, 8))) },
                    { PType.Flying, Tuple.Create(new SolidColorBrush(Color.FromRgb(156, 173, 247)), new SolidColorBrush(Color.FromRgb(66, 82, 148))) },
                    { PType.Ghost, Tuple.Create(new SolidColorBrush(Color.FromRgb(99, 99, 181)), new SolidColorBrush(Color.FromRgb(74, 57, 82))) },
                    { PType.Grass, Tuple.Create(new SolidColorBrush(Color.FromRgb(123, 206, 82)), new SolidColorBrush(Color.FromRgb(66, 107, 57))) },
                    { PType.Ground, Tuple.Create(new SolidColorBrush(Color.FromRgb(214, 181, 90)), new SolidColorBrush(Color.FromRgb(115, 82, 24))) },
                    { PType.Ice, Tuple.Create(new SolidColorBrush(Color.FromRgb(90, 206, 231)), new SolidColorBrush(Color.FromRgb(66, 82, 148))) },
                    { PType.Normal, Tuple.Create(new SolidColorBrush(Color.FromRgb(173, 165, 148)), new SolidColorBrush(Color.FromRgb(82, 82, 82))) },
                    { PType.Poison, Tuple.Create(new SolidColorBrush(Color.FromRgb(181, 90, 165)), new SolidColorBrush(Color.FromRgb(74, 57, 82))) },
                    { PType.Psychic, Tuple.Create(new SolidColorBrush(Color.FromRgb(255, 115, 165)), new SolidColorBrush(Color.FromRgb(107, 57, 57))) },
                    { PType.Rock, Tuple.Create(new SolidColorBrush(Color.FromRgb(189, 165, 90)), new SolidColorBrush(Color.FromRgb(115, 82, 24))) },
                    { PType.Steel, Tuple.Create(new SolidColorBrush(Color.FromRgb(173, 173, 198)), new SolidColorBrush(Color.FromRgb(82, 82, 82))) },
                    { PType.Water, Tuple.Create(new SolidColorBrush(Color.FromRgb(57, 156, 255)), new SolidColorBrush(Color.FromRgb(66, 82, 148))) }
                };
            }
        }

        ReactiveCommand SelectMoveCommand { get; }

        public PMove Move { get; }
        IBitmap Label { get; }
        IBrush Brush { get; }
        IBrush BorderBrush { get; }
        string Description { get; }

        public MoveInfo(int i, PPokemon pkmn, ActionsView parent)
        {
            PMove move = pkmn.Moves[i];
            var ttb = typeToBrush[PMoveData.GetMoveTypeForPokemon(pkmn, move)];

            bool enabled;
            if (pkmn.LockedAction.Decision == PDecision.Fight && pkmn.LockedAction.FightMove != PMove.None)
            {
                enabled = pkmn.LockedAction.FightMove == move;
            }
            else
            {
                enabled = move != PMove.None && pkmn.PP[i] > 0;
            }
            Move = move;
            Label = Utils.RenderString(move.ToString());
            Brush = ttb.Item1;
            BorderBrush = ttb.Item2;
            Description = move == PMove.None ? string.Empty : PMoveData.Data[move].ToString();

            var sub = new Subject<bool>();
            SelectMoveCommand = ReactiveCommand.Create(() => parent.SelectMove(this), sub);
            sub.OnNext(enabled);
        }
    }
}
