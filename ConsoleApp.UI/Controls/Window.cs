using SadConsole;
using System;
using System.Drawing;
using ConsoleApp.UI.Extensions;
using SadConsole.UI;
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
                typeof(WindowFrameType),
                typeof(Window),
                WindowFrameType.Thick,
                OnFrameTypePropertyChanged
            );
            TitleProperty = BindableProperty.Create(
                nameof(Title),
                typeof(string),
                typeof(Window),
                propertyChanged: OnTitlePropertyChanged
            );
        }

        /*public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            var transparent = false == IsOpaque;
            
            if (transparent)
            {
                surface.Copy(RenderSurface);
            }

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

            if (IsDirty || transparent)
            {
                var frameThickness = Frame.Thickness;
                var rectangle = new Rectangle(
                    frameThickness.Left,
                    frameThickness.Top,
                    Bounds.Width - frameThickness.HorizontalThickness - Shadow.Width,
                    Bounds.Height - frameThickness.VerticalThickness - Shadow.Height
                );

                Frame.Render(RenderSurface);
                RenderSurface.Fill(rectangle, Foreground, Background, Glyphs.Whitespace);

                RenderChildren(elapsed);

                IsDirty = false;
            }

            RenderSurface.Copy(surface);
        }*/

        /*public override Size Measure(int widthConstraint, int heightConstraint)
        {
            if (false == IsMeasureValid)
            {
                widthConstraint -= Margin.HorizontalThickness;
                heightConstraint -= Margin.VerticalThickness;

                widthConstraint = ResolveConstraint(widthConstraint, Width, Shadow.Width);
                heightConstraint = ResolveConstraint(heightConstraint, Height, Shadow.Height);
                
                var sizeWithoutMargins = LayoutManager.Measure(Children, widthConstraint, heightConstraint);

                DesiredSize = new Size(
                    sizeWithoutMargins.Width,
                    sizeWithoutMargins.Height
                );

                IsMeasureValid = true;
            }

            return DesiredSize;
        }*/

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

            Frame.Render(RenderSurface);
            RenderSurface.Fill(rectangle, Foreground, Background, Glyphs.Whitespace);

            RenderChildren(elapsed);

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
