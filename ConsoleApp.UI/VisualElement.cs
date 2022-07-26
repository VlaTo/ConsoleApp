using System;
using System.Drawing;
using ConsoleApp.Bindings;
using SadConsole;
using SadConsole.Input;
using Color = SadRogue.Primitives.Color;

namespace ConsoleApp.UI
{
    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right,
        Stretch
    }

    public enum VerticalAlignment
    {
        Top,
        Center,
        Bottom,
        Stretch
    }

    public enum InvalidateTrigger
    {
        MeasureChanged,
        LayoutChanged
    }

    public enum HandleKeyboardStage
    {
        PreProcess,
        Process,
        PostProcess
    }

    public enum HandleKeyStage
    {
        Preview,
        Focused
    }

    public sealed class InvalidationEventArgs : EventArgs
    {
        public InvalidateTrigger Trigger
        {
            get;
        }

        public InvalidationEventArgs(InvalidateTrigger trigger)
        {
            Trigger = trigger;
        }
    }

    public abstract class VisualElement : BindableObject
    {
        private VisualGroup parent;
        //private int actualLeft;
        //private int actualTop;
        //private int actualWidth;
        //private int actualHeight;
        //private bool stopTheRecursion;
        //private readonly Dictionary<Size, SizeRequest> measureCache;
        
        public static readonly BindableProperty ForegroundProperty;
        public static readonly BindableProperty BackgroundProperty;
        public static readonly BindableProperty LeftProperty;
        public static readonly BindableProperty TopProperty;
        public static readonly BindableProperty WidthProperty;
        public static readonly BindableProperty HeightProperty;
        public static readonly BindableProperty PaddingProperty;
        public static readonly BindableProperty MarginProperty;
        public static readonly BindableProperty HorizontalAlignmentProperty;
        public static readonly BindableProperty VerticalAlignmentProperty;
        public static readonly BindableProperty IsVisibleProperty;
        public static readonly BindableProperty IsDirtyProperty;
        
        public Color Foreground
        {
            get => (Color) GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public Color Background
        {
            get => (Color) GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get => (HorizontalAlignment) GetValue(HorizontalAlignmentProperty);
            set => SetValue(HorizontalAlignmentProperty, value);
        }

        public VerticalAlignment VerticalAlignment
        {
            get => (VerticalAlignment) GetValue(VerticalAlignmentProperty);
            set => SetValue(VerticalAlignmentProperty, value);
        }

        public int Left
        {
            get => (int) GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }

        public int Top
        {
            get => (int) GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }

        public int Width
        {
            get => (int) GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public int Height
        {
            get => (int) GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public Thickness Padding
        {
            get => (Thickness) GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        public Thickness Margin
        {
            get => (Thickness) GetValue(MarginProperty);
            set => SetValue(MarginProperty, value);
        }

        public bool IsVisible
        {
            get => (bool) GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        public VisualGroup Parent
        {
            get => parent;
            set
            {
                if (ReferenceEquals(parent, value))
                {
                    return;
                }

                if (null != parent)
                {
                    ;
                }

                parent = value;

                if (null != parent)
                {
                    ;
                }

                DoParentChanged();
            }
        }

        public virtual Rectangle Bounds
        {
            get;
            //{
            //    return new Rectangle(Left, Top, Width, Height);
            //}
            protected set;
            //{
            //    ;
            //}
        }

        public bool IsDirty
        {
            get => (bool)GetValue(IsDirtyProperty);
            protected set => SetValue(IsDirtyProperty, value);
        }

        public bool IsFocused => null != Parent && ReferenceEquals(Parent.FocusedElement, this);

        protected bool IsMeasureValid
        {
            get;
            set;
        }

        internal bool IsArrangeValid
        {
            get;
            set;
        }

        public Size DesiredSize
        {
            get;
            protected set;
        }

        /*protected int ActualLeft
        {
            get => actualLeft;
            set
            {
                if (actualLeft == value)
                {

                }

                actualLeft = value;
            }
        }

        protected int ActualTop
        {
            get => actualTop;
            set
            {
                if (actualTop == value)
                {

                }

                actualTop = value;
            }
        }

        protected int ActualWidth
        {
            get => actualWidth;
            set
            {
                if (actualWidth == value)
                {

                }

                actualWidth = value;
            }
        }

        protected int ActualHeight
        {
            get => actualHeight;
            set
            {
                if (actualHeight == value)
                {

                }

                actualHeight = value;
            }
        }*/

        public event EventHandler<InvalidationEventArgs> MeasureInvalidated;

        protected VisualElement()
        {
            IsDirty = true;
            IsMeasureValid = false;
            IsArrangeValid = false;
        }

        static VisualElement()
        {
            BackgroundProperty = BindableProperty.Create(
                nameof(Background),
                typeof(Color),
                typeof(VisualElement),
                Color.Transparent,
                OnBackgroundColorPropertyChanged
            );
            ForegroundProperty = BindableProperty.Create(
                nameof(Foreground),
                typeof(Color),
                typeof(VisualElement),
                Color.Transparent,
                OnForegroundColorPropertyChanged
            );
            HorizontalAlignmentProperty = BindableProperty.Create(
                nameof(HorizontalAlignment),
                typeof(HorizontalAlignment),
                typeof(VisualElement),
                HorizontalAlignment.Left,
                OnHorizontalAlignmentPropertyChanged
            );
            VerticalAlignmentProperty = BindableProperty.Create(
                nameof(VerticalAlignment),
                typeof(VerticalAlignment),
                typeof(VisualElement),
                UI.VerticalAlignment.Top,
                OnVerticalAlignmentPropertyChanged
            );
            LeftProperty = BindableProperty.Create(
                nameof(Left),
                typeof(int),
                typeof(VisualElement),
                -1,
                OnLeftPropertyChanged
            );
            TopProperty = BindableProperty.Create(
                nameof(Top),
                typeof(int),
                typeof(VisualElement),
                -1,
                OnTopPropertyChanged
            );
            WidthProperty = BindableProperty.Create(
                nameof(Width),
                typeof(int),
                typeof(VisualElement),
                -1,
                OnWidthPropertyChanged
            );
            HeightProperty = BindableProperty.Create(
                nameof(Height),
                typeof(int),
                typeof(VisualElement),
                -1,
                OnHeightPropertyChanged
            );
            PaddingProperty = BindableProperty.Create(
                nameof(Padding),
                typeof(Thickness),
                typeof(VisualElement),
                Thickness.Empty,
                OnPaddingPropertyChanged
            );
            MarginProperty = BindableProperty.Create(
                nameof(Margin),
                typeof(Thickness),
                typeof(VisualElement),
                Thickness.Empty,
                OnMarginPropertyChanged
            );
            IsVisibleProperty = BindableProperty.Create(
                nameof(IsVisible),
                typeof(bool),
                typeof(VisualElement),
                true
            );
            IsDirtyProperty = BindableProperty.Create(
                nameof(IsDirty),
                typeof(bool),
                typeof(VisualElement),
                defaultValue: false,
                propertyChanged: OnIsDirtyPropertyChanged
            );
        }

        public virtual void Update(TimeSpan elapsed)
        {
        }

        public virtual void Arrange(Rectangle bounds)
        {
            if (IsArrangeValid)
            {
                return;
            }

            IsArrangeValid = true;

            Bounds = GetAvailableBounds(bounds);
        }

        public virtual Size Measure(int widthConstraint, int heightConstraint)
        {
            if (false == IsMeasureValid)
            {
                DesiredSize = GetDesiredSize(widthConstraint, heightConstraint);
            }

            IsMeasureValid = true;

            return DesiredSize;
        }

        public virtual void Render(ICellSurface surface, TimeSpan elapsed)
        {
            PreRender(surface);
            RenderMain(surface, elapsed);
            PostRender(surface);

            IsDirty = false;
        }

        public virtual void Invalidate()
        {
            IsDirty = true;

            var element = Parent;

            while (null != element)
            {
                element.Invalidate();
                element = element.Parent;
            }
        }

        public virtual void Enter()
        {
            Invalidate();
        }

        public virtual void Leave()
        {
            Invalidate();
        }

        public virtual bool HandleKeyDown(AsciiKey key, ModificatorKeys modificators) => false;

        public virtual bool HandleKeyUp(AsciiKey key, ModificatorKeys modificators) => false;

        public virtual bool HandleKeyPressed(AsciiKey key, ModificatorKeys modificators) => false;

        public void Focus()
        {
            if (null != Parent)
            {
                Parent.FocusElement(this);
            }
        }

        public Point GetAbsolutePosition()
        {
            var position = Bounds.Location;
            var element = Parent;

            while (null != element)
            {
                var location = element.Bounds.Location;

                position.X += location.X;
                position.Y += location.Y;
                //position = new Point(position.X + location.X, position.Y + location.Y);
                element = element.Parent;
            }

            return position;
        }

        protected Rectangle GetAvailableBounds(Rectangle bounds)
        {
            var left = CalculateLeft(bounds.X, bounds.Width);
            var top = CalculateTop(bounds.Top, bounds.Height);


            var width = Math.Max(0, bounds.Width - Padding.HorizontalThickness);
            var height = Math.Max(0, bounds.Height - Padding.VerticalThickness);
            
            width = Math.Min(DesiredSize.Width, width);
            height = Math.Min(DesiredSize.Height, height);

            return new Rectangle(left, top, width, height);
        }

        protected int CalculateLeft(int left, int width)
        {
            var value = -1;

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                {
                    value = 0 > Left ? left : Left;
                    break;
                }

                case HorizontalAlignment.Center:
                {
                    value = (width - (DesiredSize.Width + Margin.HorizontalThickness)) >> 1;
                    break;
                }

                case HorizontalAlignment.Stretch:
                {
                    value = left;
                    break;
                }

                case HorizontalAlignment.Right:
                {
                    value = width - (DesiredSize.Width + Margin.HorizontalThickness);
                    break;
                }
            }

            value = Math.Max(value, left);

            return value;
        }

        protected int CalculateTop(int top, int height)
        {
            var value = -1;

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                {
                    value = 0 > Top ? top : Top;
                    break;
                }

                case VerticalAlignment.Center:
                {
                    value = (height - (DesiredSize.Height + Margin.VerticalThickness)) >> 1;
                    break;
                }

                case VerticalAlignment.Stretch:
                {
                    value = top;
                    break;
                }

                case VerticalAlignment.Bottom:
                {
                    value = height - (DesiredSize.Height + Margin.VerticalThickness);
                    break;
                }
            }

            value = Math.Max(value, top);

            return value;
        }

        protected Size GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            /*_ = frameworkElement ?? throw new ArgumentNullException(nameof(frameworkElement));

            if (frameworkElement.Handler == null)
            {
                return Size.Zero;
            }*/

            // Adjust the constraints to account for the margins
            widthConstraint -= Margin.HorizontalThickness;
            heightConstraint -= Margin.VerticalThickness;

            // Determine whether the external constraints or the requested size values will determine the measurements
            widthConstraint = ResolveConstraint(widthConstraint, Width, 0);
            heightConstraint = ResolveConstraint(heightConstraint, Height, 0);
            
            // Ask the handler to do the actual measuring								
            //var measureWithMargins = frameworkElement.Handler.GetDesiredSize(widthConstraint, heightConstraint);

            // Account for the margins when reporting the desired size value
            //return new Size(
            //    measureWithMargins.Width + margin.HorizontalThickness,
            //    measureWithMargins.Height + margin.VerticalThickness
            //);
            return new Size(
                widthConstraint/* + Margin.HorizontalThickness*/,
                heightConstraint/* + Margin.VerticalThickness*/
            );

            //return new Size(widthConstraint, heightConstraint);
        }

        protected virtual void InvalidateMeasureInternal(InvalidateTrigger trigger)
        {
            IsArrangeValid = false;
            IsMeasureValid = false;
        }

        protected Rectangle MakeAbsolute(Rectangle rectangle)
        {
            var element = this;

            while (null != element)
            {
                rectangle.Offset(element.Bounds.Location);
                element = element.Parent;
            }

            return rectangle;
        }

        /*protected virtual SizeRequest OnMeasure(in int widthConstraint, in int heightConstraint)
        {
            if (IsVisible)
            {
                var width = Math.Min(actualWidth, widthConstraint);
                var height = Math.Min(actualHeight, heightConstraint);

                return new SizeRequest(new Size(width, height));
            }

            return new SizeRequest(new Size(-1, -1));
        }*/

        //protected Point GetOrigin()
        //{
        //    var element = Parent;
        //    var origin = new Point(Left, Top);

        //    while (null != element)
        //    {
        //        var point = element.GetOrigin();

        //        origin.X += point.X;
        //        origin.Y += point.Y;

        //        element = element.Parent;
        //    }

        //    return origin;
        //}

        //protected Rectangle GetClientRect()
        //{
        //    var x = 0;
        //    var y = 0;
        //    var width = Bounds.Width;
        //    var height = Bounds.Height;

        //    if (0 < Padding.Left)
        //    {
        //        x = Padding.Left;
        //        width -= x;
        //    }

        //    if (0 < Padding.Top)
        //    {
        //        y = Padding.Top;
        //        height -= y;
        //    }

        //    if (0 < Padding.Right)
        //    {
        //        width -= Padding.Right;
        //    }

        //    if (0 < Padding.Bottom)
        //    {
        //        height -= Padding.Bottom;
        //    }

        //    return new Rectangle(x, y, width, height);
        //}

        /*public SizeRequest Measure(int widthConstraint, int heightConstraint, MeasureFlags flags = MeasureFlags.None)
        {
            var includeMargins = MeasureFlags.IncludeMargins == (flags & MeasureFlags.IncludeMargins);

            if (includeMargins)
            {
                widthConstraint = Math.Max(0, widthConstraint - Margin.HorizontalThickness);
                heightConstraint = Math.Max(0, heightConstraint - Margin.VerticalThickness);
            }

            var request = GetSizeRequest(widthConstraint, heightConstraint);

            if (includeMargins && false == Margin.IsEmpty)
            {
                request.Minimum = new Size(
                    request.Minimum.Width + Margin.HorizontalThickness,
                    request.Minimum.Height + Margin.VerticalThickness
                );
                request.Request = new Size(
                    request.Request.Width + Margin.HorizontalThickness,
                    request.Request.Height + Margin.VerticalThickness
                );
            }

            return request;
        }*/

        /*public sealed override SizeRequest GetSizeRequest(double widthConstraint, double heightConstraint)
        {
            SizeRequest size = base.GetSizeRequest(widthConstraint - Padding.HorizontalThickness, heightConstraint - Padding.VerticalThickness);
            return new SizeRequest(new Size(size.Request.Width + Padding.HorizontalThickness, size.Request.Height + Padding.VerticalThickness),
                new Size(size.Minimum.Width + Padding.HorizontalThickness, size.Minimum.Height + Padding.VerticalThickness));
        }*/

        /*public virtual SizeRequest GetSizeRequest(int widthConstraint, int heightConstraint)
        {
            var constraintSize = new Size(widthConstraint, heightConstraint);

            if (measureCache.TryGetValue(constraintSize, out SizeRequest cachedResult))
            {
                return cachedResult;
            }

            var widthRequest = Width;
            var heightRequest = Height;

            if (0 <= widthRequest)
            {
                widthConstraint = Math.Min(widthConstraint, widthRequest);
            }

            if (0 <= heightRequest)
            {
                heightConstraint = Math.Min(heightConstraint, heightRequest);
            }

            var result = OnMeasure(widthConstraint, heightConstraint);

            var hasMinimum = false == result.Minimum.Equals(result.Request);
            var request = result.Request;
            var minimum = result.Minimum;

            if (-1 < heightRequest)
            {
                request.Height = heightRequest;

                if (false == hasMinimum)
                {
                    minimum.Height = heightRequest;
                }
            }

            if (-1 < widthRequest)
            {
                request.Width = widthRequest;

                if (false == hasMinimum)
                {
                    minimum.Width = widthRequest;
                }
            }

            /--double minimumHeightRequest = MinimumHeightRequest;
            double minimumWidthRequest = MinimumWidthRequest;

            if (minimumHeightRequest != -1)
                minimum.Height = minimumHeightRequest;
            if (minimumWidthRequest != -1)
                minimum.Width = minimumWidthRequest;--/

            minimum.Height = Math.Min(request.Height, minimum.Height);
            minimum.Width = Math.Min(request.Width, minimum.Width);

            var req = new SizeRequest(request, minimum);

            if (0 < req.Request.Width && 0 < req.Request.Height)
            {
                measureCache[constraintSize] = req;
            }

            return req;
        }*/

        protected int ResolveConstraint(int value, int constraint, int extent)
        {
            if (0 > constraint)
            {
                return value;
            }

            return Math.Min(value, constraint + extent);
        }

        protected virtual void PreRender(ICellSurface surface)
        {

        }
        
        protected virtual void RenderMain(ICellSurface surface, TimeSpan elapsed)
        {

        }

        protected virtual void PostRender(ICellSurface surface)
        {

        }

        protected virtual void DoParentChanged()
        {
        }

        protected virtual void OnLeftChanged(int newValue, int oldValue)
        {
            ;
        }

        protected virtual void OnTopChanged(int newValue, int oldValue)
        {
            ;
        }

        protected virtual void OnBackgroundColorChanged()
        {
            Invalidate();
        }

        protected virtual void OnForegroundColorChanged()
        {
            Invalidate();
        }

        protected virtual void OnWidthChanged()
        {
            ;
        }

        protected virtual void OnHeightChanged()
        {
            ;
        }

        protected virtual void OnHorizontalAlignmentChanged()
        {
            ;
        }

        protected virtual void OnVerticalAlignmentChanged()
        {
            ;
        }

        protected virtual void OnPaddingChanged()
        {
            ;
        }

        protected virtual void OnMarginChanged()
        {
            ;
        }

        protected virtual void OnIsDirtyChanged()
        {
            ;
        }

        private void RaiseMeasureInvalidated(InvalidateTrigger trigger)
        {
            var handler = MeasureInvalidated;

            if (null != handler)
            {
                handler.Invoke(this, new InvalidationEventArgs(trigger));
            }
        }

        private static void OnBackgroundColorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((VisualElement)sender).OnBackgroundColorChanged();
        }

        private static void OnForegroundColorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((VisualElement) sender).OnForegroundColorChanged();
        }

        private static void OnHorizontalAlignmentPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((VisualElement) sender).OnHorizontalAlignmentChanged();
        }

        private static void OnVerticalAlignmentPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((VisualElement)sender).OnVerticalAlignmentChanged();
        }

        private static void OnLeftPropertyChanged(BindableObject sender, object newValue, object oldValue)
        {
            ((VisualElement) sender).OnLeftChanged((int) newValue, (int) oldValue);
        }

        private static void OnTopPropertyChanged(BindableObject sender, object newValue, object oldValue)
        {
            ((VisualElement) sender).OnTopChanged((int) newValue, (int) oldValue);
        }

        private static void OnWidthPropertyChanged(BindableObject sender, object newValue, object oldValue)
        {
            ((VisualElement) sender).OnWidthChanged();
        }

        private static void OnHeightPropertyChanged(BindableObject sender, object newValue, object oldValue)
        {
            ((VisualElement) sender).OnHeightChanged();
        }

        private static void OnPaddingPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((VisualElement) sender).OnPaddingChanged();
        }

        private static void OnMarginPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((VisualElement) sender).OnMarginChanged();
        }

        private static void OnIsDirtyPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((VisualElement)sender).OnIsDirtyChanged();
        }
    }
}
