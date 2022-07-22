using ConsoleApp.Bindings;
using ConsoleApp.UI.Extensions;
using SadConsole;
using System;
using System.Drawing;
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
        public static readonly BindableProperty TitleProperty;

        public WindowFrameType FrameType
        {
            get => (WindowFrameType)GetValue(FrameTypeProperty);
            set => SetValue(FrameTypeProperty, value);
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
                var thickness = Frame.Thickness;

                widthConstraint -= Margin.HorizontalThickness;
                heightConstraint -= Margin.VerticalThickness;

                widthConstraint = ResolveConstraint(widthConstraint, Width, Shadow.Width);
                heightConstraint = ResolveConstraint(heightConstraint, Height, Shadow.Height);

                var width = widthConstraint - thickness.HorizontalThickness;
                var height = heightConstraint - thickness.VerticalThickness;
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

        protected override void OnChildAdded(VisualElement view)
        {
        }

        protected virtual void OnFrameTypeChanged()
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

        private static void OnTitlePropertyChanged(object sender, object newValue, object oldValue)
        {
            ((Window)sender).OnTitleChanged();
        }
    }
}
