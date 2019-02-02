using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Models
{
    public class MoveInfo
    {
        static Dictionary<PBEType, Tuple<Color, Color>> typeToColor;
        public static void CreateBrushes()
        {
            if (typeToColor == null)
            {
                typeToColor = new Dictionary<PBEType, Tuple<Color, Color>>
                {
                    { PBEType.Bug, Tuple.Create(Color.FromRgb(173, 189, 31), Color.FromRgb(66, 107, 57)) },
                    { PBEType.Dark, Tuple.Create(Color.FromRgb(115, 90, 74), Color.FromRgb(74, 57, 49)) },
                    { PBEType.Dragon, Tuple.Create(Color.FromRgb(123, 99, 231), Color.FromRgb(74, 57, 148)) },
                    { PBEType.Electric, Tuple.Create(Color.FromRgb(255, 198, 49), Color.FromRgb(115, 82, 24)) },
                    { PBEType.Fighting, Tuple.Create(Color.FromRgb(165, 82, 57), Color.FromRgb(74, 57, 49)) },
                    { PBEType.Fire, Tuple.Create(Color.FromRgb(247, 82, 49), Color.FromRgb(115, 33, 8)) },
                    { PBEType.Flying, Tuple.Create(Color.FromRgb(156, 173, 247), Color.FromRgb(66, 82, 148)) },
                    { PBEType.Ghost, Tuple.Create(Color.FromRgb(99, 99, 181), Color.FromRgb(74, 57, 82)) },
                    { PBEType.Grass, Tuple.Create(Color.FromRgb(123, 206, 82), Color.FromRgb(66, 107, 57)) },
                    { PBEType.Ground, Tuple.Create(Color.FromRgb(214, 181, 90), Color.FromRgb(115, 82, 24)) },
                    { PBEType.Ice, Tuple.Create(Color.FromRgb(90, 206, 231), Color.FromRgb(66, 82, 148)) },
                    { PBEType.Normal, Tuple.Create(Color.FromRgb(173, 165, 148), Color.FromRgb(82, 82, 82)) },
                    { PBEType.Poison, Tuple.Create(Color.FromRgb(181, 90, 165), Color.FromRgb(74, 57, 82)) },
                    { PBEType.Psychic, Tuple.Create(Color.FromRgb(255, 115, 165), Color.FromRgb(107, 57, 57)) },
                    { PBEType.Rock, Tuple.Create(Color.FromRgb(189, 165, 90), Color.FromRgb(115, 82, 24)) },
                    { PBEType.Steel, Tuple.Create(Color.FromRgb(173, 173, 198), Color.FromRgb(82, 82, 82)) },
                    { PBEType.Water, Tuple.Create(Color.FromRgb(57, 156, 255), Color.FromRgb(66, 82, 148)) }
                };
            }
        }

        public Command SelectMoveCommand { get; }

        public PBEMove Move { get; }
        public Color Color { get; }
        public Color BorderColor { get; }

        public MoveInfo(int i, PBEPokemon pkmn, Action<MoveInfo> clickAction)
        {
            bool forcedToStruggle = pkmn.IsForcedToStruggle();
            PBEMove move = forcedToStruggle ? PBEMove.Struggle : pkmn.Moves[i];
            var ttc = move == PBEMove.None ? typeToColor[PBEType.Normal] : typeToColor[pkmn.GetMoveType(move)];

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
            Color = ttc.Item1;
            BorderColor = ttc.Item2;

            SelectMoveCommand = new Command(() => clickAction(this), () => enabled);
        }
    }
}
