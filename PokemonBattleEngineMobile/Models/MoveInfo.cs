using Kermalis.PokemonBattleEngine.Battle;
using Kermalis.PokemonBattleEngine.Data;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Models
{
    public class MoveInfo
    {
        private static Dictionary<PBEType, (Color Color, Color BorderColor)> typeToColor;
        public static void CreateBrushes()
        {
            if (typeToColor == null)
            {
                typeToColor = new Dictionary<PBEType, (Color Color, Color BorderColor)>
                {
                    { PBEType.Bug, (Color.FromRgb(173, 189, 31), Color.FromRgb(66, 107, 57)) },
                    { PBEType.Dark, (Color.FromRgb(115, 90, 74), Color.FromRgb(74, 57, 49)) },
                    { PBEType.Dragon, (Color.FromRgb(123, 99, 231), Color.FromRgb(74, 57, 148)) },
                    { PBEType.Electric, (Color.FromRgb(255, 198, 49), Color.FromRgb(115, 82, 24)) },
                    { PBEType.Fighting, (Color.FromRgb(165, 82, 57), Color.FromRgb(74, 57, 49)) },
                    { PBEType.Fire, (Color.FromRgb(247, 82, 49), Color.FromRgb(115, 33, 8)) },
                    { PBEType.Flying, (Color.FromRgb(156, 173, 247), Color.FromRgb(66, 82, 148)) },
                    { PBEType.Ghost, (Color.FromRgb(99, 99, 181), Color.FromRgb(74, 57, 82)) },
                    { PBEType.Grass, (Color.FromRgb(123, 206, 82), Color.FromRgb(66, 107, 57)) },
                    { PBEType.Ground, (Color.FromRgb(214, 181, 90), Color.FromRgb(115, 82, 24)) },
                    { PBEType.Ice, (Color.FromRgb(90, 206, 231), Color.FromRgb(66, 82, 148)) },
                    { PBEType.Normal, (Color.FromRgb(173, 165, 148), Color.FromRgb(82, 82, 82)) },
                    { PBEType.Poison, (Color.FromRgb(181, 90, 165), Color.FromRgb(74, 57, 82)) },
                    { PBEType.Psychic, (Color.FromRgb(255, 115, 165), Color.FromRgb(107, 57, 57)) },
                    { PBEType.Rock, (Color.FromRgb(189, 165, 90), Color.FromRgb(115, 82, 24)) },
                    { PBEType.Steel, (Color.FromRgb(173, 173, 198), Color.FromRgb(82, 82, 82)) },
                    { PBEType.Water, (Color.FromRgb(57, 156, 255), Color.FromRgb(66, 82, 148)) }
                };
            }
        }

        public PBEMove Move { get; }
        public Color Color { get; }
        public Color BorderColor { get; }
        public Command SelectMoveCommand { get; }

        public MoveInfo(PBEPokemon pkmn, PBEMove move, Action<PBEMove> clickAction)
        {
            Move = move;
            (Color Color, Color BorderColor) ttc = typeToColor[pkmn.GetMoveType(move)];
            Color = ttc.Color;
            BorderColor = ttc.BorderColor;
            SelectMoveCommand = new Command(() => clickAction(move));
        }
    }
}
