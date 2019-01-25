using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Infrastructure
{
    public class Canvas : Layout<View>
    {
        const double autoSize = -1.0;

        public static readonly BindableProperty IntendedWidthProperty = BindableProperty.Create("IntendedWidth", typeof(double), typeof(Canvas), autoSize, propertyChanged: OnIntendedWidthPropertyChanged);
        public static readonly BindableProperty IntendedHeightProperty = BindableProperty.Create("IntendedHeight", typeof(double), typeof(Canvas), autoSize, propertyChanged: OnIntendedHeightPropertyChanged);

        public static readonly BindableProperty LocationProperty = BindableProperty.CreateAttached("Location", typeof(Point), typeof(Canvas), new Point(0.0, 0.0));
        public static readonly BindableProperty SizeProperty = BindableProperty.CreateAttached("Size", typeof(Size), typeof(Canvas), new Size(autoSize, autoSize));

        public double IntendedWidth
        {
            get { return (double)GetValue(IntendedWidthProperty); }
            set { SetValue(IntendedWidthProperty, value); }
        }
        public double IntendedHeight
        {
            get { return (double)GetValue(IntendedHeightProperty); }
            set { SetValue(IntendedHeightProperty, value); }
        }

        static void OnIntendedWidthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((Canvas)bindable)?.InvalidateMeasure();
        }
        static void OnIntendedHeightPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((Canvas)bindable)?.InvalidateMeasure();
        }

        [TypeConverter(typeof(PointTypeConverter))]
        public static Point GetLocation(BindableObject bindable)
        {
            return (Point)bindable.GetValue(LocationProperty);
        }
        public static void SetLocation(BindableObject bindable, Point location)
        {
            bindable.SetValue(LocationProperty, location);
        }
        [TypeConverter(typeof(SizeTypeConverter))]
        public static Size GetSize(BindableObject bindable)
        {
            return (Size)bindable.GetValue(SizeProperty);
        }
        public static void SetSize(BindableObject bindable, Size size)
        {
            bindable.SetValue(SizeProperty, size);
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            Children.Add((View)child);
            child.PropertyChanged += ChildOnPropertyChanged;
        }
        protected override void OnChildRemoved(Element child)
        {
            child.PropertyChanged -= ChildOnPropertyChanged;
            base.OnChildRemoved(child);
            Children.Remove((View)child);
        }
        void ChildOnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == LocationProperty.PropertyName || e.PropertyName == SizeProperty.PropertyName)
            {
                InvalidateMeasure();
                UpdateChildrenLayout();
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            InvalidateMeasure();
        }
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            double w = IntendedWidth == autoSize ? widthConstraint : IntendedWidth,
                h = IntendedHeight == autoSize ? heightConstraint : IntendedHeight;
            double aspectRatio = w / h;

            Rectangle parentBounds = Utils.GetFirstParentVisualElement(this).Bounds;
            double parentAspectRatio = parentBounds.Width / parentBounds.Height;

            if (aspectRatio < parentAspectRatio)
            {
                return new SizeRequest(new Size(parentBounds.Height / h * w, parentBounds.Height));
            }
            else
            {
                return new SizeRequest(new Size(parentBounds.Width, parentBounds.Width / w * h));
            }
        }

        Rectangle ComputeLayoutForRegion(View view)
        {
            double xScale = Width / (IntendedWidth == autoSize ? Width : IntendedWidth),
                yScale = Height / (IntendedHeight == autoSize ? Height : IntendedHeight);

            Point location = GetLocation(view);
            Size size = GetSize(view);

            var result = new Rectangle
            {
                X = location.X * xScale,
                Y = location.Y * yScale
            };
            if (size.Width == autoSize && size.Height == autoSize)
            {
                SizeRequest sizeRequest = view.Measure(Width, Height, MeasureFlags.IncludeMargins);
                result.Width = sizeRequest.Request.Width * xScale;
                result.Height = sizeRequest.Request.Height * yScale;
            }
            else if (size.Width == autoSize)
            {
                SizeRequest sizeRequest = view.Measure(Width, Height, MeasureFlags.IncludeMargins);
                result.Width = sizeRequest.Request.Width * xScale;
                result.Height = size.Height * yScale;
            }
            else if (size.Height == autoSize)
            {
                SizeRequest sizeRequest = view.Measure(Width, Height, MeasureFlags.IncludeMargins);
                result.Width = size.Width * xScale;
                result.Height = sizeRequest.Request.Height * yScale;
            }
            else
            {
                result.Width = size.Width * xScale;
                result.Height = size.Height * yScale;
            }

            return result;
        }
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            foreach (View child in Children)
            {
                Rectangle rect = ComputeLayoutForRegion(child);
                rect.X += x;
                rect.Y += y;

                LayoutChildIntoBoundingRegion(child, rect);
            }
        }
    }
}
