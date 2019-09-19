// This source file is adapted from the Avalonia project. 
// (https://github.com/AvaloniaUI/Avalonia)

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Utilities;
using static System.Math;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    public sealed class BetterWrapPanel : Panel, INavigableContainer
    {
        public static readonly StyledProperty<Orientation> OrientationProperty = AvaloniaProperty.Register<BetterWrapPanel, Orientation>(nameof(Orientation), defaultValue: Orientation.Horizontal);

        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty = AvaloniaProperty.Register<BetterWrapPanel, HorizontalAlignment>(nameof(HorizontalContentAlignment), defaultValue: HorizontalAlignment.Left);

        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty = AvaloniaProperty.Register<BetterWrapPanel, VerticalAlignment>(nameof(VerticalContentAlignment), defaultValue: VerticalAlignment.Top);

        static BetterWrapPanel()
        {
            AffectsMeasure<BetterWrapPanel>(OrientationProperty);
            AffectsArrange<BetterWrapPanel>(HorizontalContentAlignmentProperty,
                VerticalContentAlignmentProperty);
        }

        public Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public HorizontalAlignment HorizontalContentAlignment
        {
            get => GetValue(HorizontalContentAlignmentProperty);
            set => SetValue(HorizontalContentAlignmentProperty, value);
        }

        public VerticalAlignment VerticalContentAlignment
        {
            get => GetValue(VerticalContentAlignmentProperty);
            set => SetValue(VerticalContentAlignmentProperty, value);
        }

        IInputElement INavigableContainer.GetControl(NavigationDirection direction, IInputElement from, bool wrap)
        {
            bool horiz = Orientation == Orientation.Horizontal;
            int index = Children.IndexOf((IControl)from);

            switch (direction)
            {
                case NavigationDirection.First:
                    index = 0;
                    break;
                case NavigationDirection.Last:
                    index = Children.Count - 1;
                    break;
                case NavigationDirection.Next:
                    ++index;
                    break;
                case NavigationDirection.Previous:
                    --index;
                    break;
                case NavigationDirection.Left:
                    index = horiz ? index - 1 : -1;
                    break;
                case NavigationDirection.Right:
                    index = horiz ? index + 1 : -1;
                    break;
                case NavigationDirection.Up:
                    index = horiz ? -1 : index - 1;
                    break;
                case NavigationDirection.Down:
                    index = horiz ? -1 : index + 1;
                    break;
            }

            if (index >= 0 && index < Children.Count)
            {
                return Children[index];
            }
            else
            {
                return null;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var curLineSize = new UVSize(Orientation);
            var panelSize = new UVSize(Orientation);
            var uvConstraint = new UVSize(Orientation, constraint.Width, constraint.Height);

            var childConstraint = new Size(constraint.Width, constraint.Height);

            for (int i = 0, count = Children.Count; i < count; i++)
            {
                IControl child = Children[i];
                if (child == null)
                {
                    continue;
                }

                //Flow passes its own constrint to children
                child.Measure(childConstraint);

                //this is the size of the child in UV space
                var sz = new UVSize(Orientation, child.DesiredSize.Width, child.DesiredSize.Height);

                if (MathUtilities.GreaterThan(curLineSize.U + sz.U, uvConstraint.U)) //need to switch to another line
                {
                    panelSize.U = Max(curLineSize.U, panelSize.U);
                    panelSize.V += curLineSize.V;
                    curLineSize = sz;

                    if (MathUtilities.GreaterThan(sz.U, uvConstraint.U)) //the element is wider then the constrint - give it a separate line                    
                    {
                        panelSize.U = Max(sz.U, panelSize.U);
                        panelSize.V += sz.V;
                        curLineSize = new UVSize(Orientation);
                    }
                }
                else //continue to accumulate a line
                {
                    curLineSize.U += sz.U;
                    curLineSize.V = Max(sz.V, curLineSize.V);
                }
            }

            //the last line size, if any should be added
            panelSize.U = Max(curLineSize.U, panelSize.U);
            panelSize.V += curLineSize.V;

            //go from UV space to W/H space
            return new Size(panelSize.Width, panelSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int firstInLine = 0;
            double accumulatedV = 0;
            var curLineSize = new UVSize(Orientation);
            var uvFinalSize = new UVSize(Orientation, finalSize.Width, finalSize.Height);

            for (int i = 0; i < Children.Count; i++)
            {
                IControl child = Children[i];
                if (child == null)
                {
                    continue;
                }

                var sz = new UVSize(Orientation, child.DesiredSize.Width, child.DesiredSize.Height);

                if (MathUtilities.GreaterThan(curLineSize.U + sz.U, uvFinalSize.U)) //need to switch to another line
                {
                    ArrangeLine(finalSize, accumulatedV, curLineSize, firstInLine, i);

                    accumulatedV += curLineSize.V;
                    curLineSize = sz;

                    if (MathUtilities.GreaterThan(sz.U, uvFinalSize.U)) //the element is wider then the constraint - give it a separate line                    
                    {
                        //switch to next line which only contain one element
                        ArrangeLine(finalSize, accumulatedV, sz, i, ++i);

                        accumulatedV += sz.V;
                        curLineSize = new UVSize(Orientation);
                    }
                    firstInLine = i;
                }
                else //continue to accumulate a line
                {
                    curLineSize.U += sz.U;
                    curLineSize.V = Max(sz.V, curLineSize.V);
                }
            }

            //arrange the last line, if any
            if (firstInLine < Children.Count)
            {
                ArrangeLine(finalSize, accumulatedV, curLineSize, firstInLine, Children.Count);
            }

            return finalSize;
        }

        private void ArrangeLine(Size finalSize, double v, UVSize line, int start, int end)
        {
            double u;
            bool isHorizontal = Orientation == Orientation.Horizontal;

            if (isHorizontal)
            {
                switch (HorizontalContentAlignment)
                {
                    case HorizontalAlignment.Center:
                        u = (finalSize.Width - line.U) / 2;
                        break;
                    case HorizontalAlignment.Right:
                        u = finalSize.Width - line.U;
                        break;
                    default:
                        u = 0;
                        break;
                }
            }
            else
            {
                switch (VerticalContentAlignment)
                {
                    case VerticalAlignment.Center:
                        u = (finalSize.Height - line.U) / 2;
                        break;
                    case VerticalAlignment.Bottom:
                        u = finalSize.Height - line.U;
                        break;
                    default:
                        u = 0;
                        break;
                }
            }

            for (int i = start; i < end; i++)
            {
                IControl child = Children[i];
                if (child != null)
                {
                    var childSize = new UVSize(Orientation, child.DesiredSize.Width, child.DesiredSize.Height);
                    double layoutSlotU = childSize.U;
                    child.Arrange(new Rect(
                        isHorizontal ? u : v,
                        isHorizontal ? v : u,
                        isHorizontal ? layoutSlotU : line.V,
                        isHorizontal ? line.V : layoutSlotU));
                    u += layoutSlotU;
                }
            }
        }

        private struct UVSize
        {
            internal UVSize(Orientation orientation, double width, double height)
            {
                U = V = 0d;
                _orientation = orientation;
                Width = width;
                Height = height;
            }

            internal UVSize(Orientation orientation)
            {
                U = V = 0d;
                _orientation = orientation;
            }

            internal double U;
            internal double V;
            private readonly Orientation _orientation;

            internal double Width
            {
                get => _orientation == Orientation.Horizontal ? U : V;
                set
                {
                    if (_orientation == Orientation.Horizontal)
                    {
                        U = value;
                    }
                    else
                    {
                        V = value;
                    }
                }
            }
            internal double Height
            {
                get => _orientation == Orientation.Horizontal ? V : U;
                set
                {
                    if (_orientation == Orientation.Horizontal)
                    {
                        V = value;
                    }
                    else
                    {
                        U = value;
                    }
                }
            }
        }
    }
}
