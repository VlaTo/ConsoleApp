using ConsoleApp.Bindings;
using ConsoleApp.UI.Extensions;
using SadConsole;
using System;
using System.Drawing;
using Color = SadRogue.Primitives.Color;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace ConsoleApp.UI.Controls
{
    public enum WindowFrameType
    {
        None,
        Thick,
        Thin
    }

    public class Window : VisualGroup
    {
        public const float ShadowBackgroundShadeFactor = 0.45f;
        public const float ShadowForegroundShadeFactor = 0.15f;

        public static readonly BindableProperty FrameTypeProperty;
        public static readonly BindableProperty FrameActiveColorProperty;
        public static readonly BindableProperty FrameInactiveColorProperty;
        public static readonly BindableProperty CanMoveProperty;
        public static readonly BindableProperty MinimalSizeProperty;
        public static readonly BindableProperty TitleProperty;

        public WindowFrameType FrameType
        {
            get => (WindowFrameType)GetValue(FrameTypeProperty);
            set => SetValue(FrameTypeProperty, value);
        }

        public Color FrameActiveColor
        {
            get => (Color)GetValue(FrameActiveColorProperty);
            set => SetValue(FrameActiveColorProperty, value);
        }

        public Color FrameInactiveColor
        {
            get => (Color)GetValue(FrameInactiveColorProperty);
            set => SetValue(FrameInactiveColorProperty, value);
        }

        public bool CanMove
        {
            get => (bool)GetValue(CanMoveProperty);
            set => SetValue(CanMoveProperty, value);
        }

        public Size MinimalSize
        {
            get => (Size)GetValue(MinimalSizeProperty);
            set => SetValue(MinimalSizeProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        protected WindowFrame Frame
        {
            get;
            set;
        }

        internal Size Shadow
        {
            get;
        }

        public Window()
        {
            IsOpaque = false;
            Frame = new WindowFrame(this);
            Shadow = new Size(2, 1);
        }

        static Window()
        {
            FrameTypeProperty = BindableProperty.Create(
                nameof(FrameType),
                propertyType: typeof(WindowFrameType),
                ownerType: typeof(Window),
                defaultValue: WindowFrameType.Thick,
                propertyChanged: OnFrameTypePropertyChanged
            );
            FrameActiveColorProperty = BindableProperty.Create(
                nameof(FrameActiveColor),
                typeof(Color),
                ownerType: typeof(Window),
                defaultValue: Color.White,
                propertyChanged: OnFrameActiveColorPropertyChanged
            );
            FrameInactiveColorProperty = BindableProperty.Create(
                nameof(FrameInactiveColor),
                typeof(Color),
                ownerType: typeof(Window),
                defaultValue: Color.DarkGray,
                propertyChanged: OnFrameInactiveColorPropertyChanged
            );
            CanMoveProperty = BindableProperty.Create(
                nameof(CanMove),
                typeof(bool),
                ownerType: typeof(Window),
                defaultValue: true,
                propertyChanged: OnCanMovePropertyChanged
            );
            MinimalSizeProperty = BindableProperty.Create(
                nameof(MinimalSize),
                propertyType: typeof(Size),
                ownerType: typeof(Window),
                defaultValue: new Size(8, 4),
                propertyChanged: OnMinimalSizePropertyChanged
            );
            TitleProperty = BindableProperty.Create(
                nameof(Title),
                propertyType: typeof(string),
                ownerType: typeof(Window),
                propertyChanged: OnTitlePropertyChanged
            );
        }

        public override Size Measure(int widthConstraint, int heightConstraint)
        {
            if (false == IsMeasureValid)
            {
                var frame = Frame.Thickness;

                widthConstraint -= Margin.HorizontalThickness;
                heightConstraint -= Margin.VerticalThickness;

                widthConstraint = ResolveWidth(widthConstraint);
                heightConstraint = ResolveHeight(heightConstraint);

                var width = widthConstraint - frame.HorizontalThickness;
                var height = heightConstraint - frame.VerticalThickness;
                var childrenSize = LayoutManager.Measure(Children, width, height);

                DesiredSize = new Size(
                    Math.Max(childrenSize.Width, widthConstraint),
                    Math.Max(childrenSize.Height, heightConstraint)
                );

                IsMeasureValid = true;
            }

            return DesiredSize;
        }

        public override void Arrange(System.Drawing.Rectangle bounds)
        {
            if (!IsMeasureValid)
            {
                return;
            }

            if (IsArrangeValid)
            {
                return;
            }

            Bounds = GetAvailableBounds(bounds);

            var thickness = Frame.Thickness;
            var rectangle = new System.Drawing.Rectangle(thickness.Origin, Bounds.Size - (thickness.Size + Shadow));

            LayoutManager.Arrange(Children, rectangle);

            var width = Bounds.Width + Shadow.Width;
            var height = Bounds.Height + Shadow.Height;

            RenderSurface.Resize(width, height, true);
            IsArrangeValid = true;
        }

        protected override void PreRender(ICellSurface surface)
        {
            base.PreRender(surface);

            if (false == Shadow.IsEmpty)
            {
                var right = new Rectangle(
                    Bounds.Width - Shadow.Width,
                    Shadow.Height,
                    Bounds.Width,
                    Bounds.Height
                );

                var bottom = new Rectangle(
                    Shadow.Width,
                    Bounds.Height - Shadow.Height,
                    Bounds.Width - (Shadow.Width * 2),
                    Bounds.Height
                );

                RenderSurface.Shade(right, ShadowBackgroundShadeFactor, ShadowForegroundShadeFactor);
                RenderSurface.Shade(bottom, ShadowBackgroundShadeFactor, ShadowForegroundShadeFactor);
            }
        }

        protected override void RenderMain(ICellSurface surface, TimeSpan elapsed)
        {
            if (false == IsDirty && IsOpaque)
            {
                return;
            }

            var frameThickness = Frame.Thickness;
            var rectangle = new Rectangle(
                frameThickness.Left,
                frameThickness.Top,
                Bounds.Width - frameThickness.HorizontalThickness - Shadow.Width,
                Bounds.Height - frameThickness.VerticalThickness - Shadow.Height
            );

            //Frame.Render(RenderSurface);
            RenderSurface.Fill(rectangle, Foreground, Background, Glyphs.Whitespace);

            RenderChildren(elapsed);
            
            Frame.Render(RenderSurface);

            IsDirty = false;
        }
        
        protected virtual int ResolveHeight(int constraint)
        {
            if (0 > Height)
            {
                return Math.Max(constraint, MinimalSize.Height);
            }

            return Math.Max(Height, MinimalSize.Height) + Shadow.Height;
        }
        
        protected virtual int ResolveWidth(int constraint)
        {
            if (0 > Width)
            {
                return Math.Max(constraint, MinimalSize.Width);
            }

            return Math.Max(Width, MinimalSize.Width) + Shadow.Width;
        }

        protected override void OnChildAdded(VisualElement view)
        {
        }

        protected virtual void OnFrameTypeChanged()
        {
            ;
        }

        protected virtual void OnFrameInactiveColorChanged()
        {
            ;
        }

        protected virtual void OnFrameActiveColorChanged()
        {
            ;
        }

        protected virtual void OnCanMoveChanged()
        {
            ;
        }

        protected virtual void OnMinimalSizeChanged()
        {
            ;
        }

        protected virtual void OnTitleChanged()
        {
            ;
        }

        private static void OnFrameTypePropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Window)sender).OnFrameTypeChanged();
        }

        private static void OnFrameInactiveColorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Window)sender).OnFrameInactiveColorChanged();
        }

        private static void OnFrameActiveColorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Window)sender).OnFrameActiveColorChanged();
        }

        private static void OnCanMovePropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Window)sender).OnCanMoveChanged();
        }

        private static void OnMinimalSizePropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Window)sender).OnMinimalSizeChanged();
        }

        private static void OnTitlePropertyChanged(object sender, object newValue, object oldValue)
        {
            ((Window)sender).OnTitleChanged();
        }
    }
}
