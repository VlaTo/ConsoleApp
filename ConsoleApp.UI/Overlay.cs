using SadConsole;
using SadRogue.Primitives;
using System;
using ConsoleApp.UI.Extensions;

namespace ConsoleApp.UI
{
    public class Overlay : VisualGroup
    {
        public static readonly BindableProperty BackgroundShadeFactorProperty;

        public float BackgroundShadeFactor
        {
            get => (float)GetValue(BackgroundShadeFactorProperty);
            set => SetValue(BackgroundShadeFactorProperty, value);
        }

        public Overlay()
        {
            LayoutManager = new AbsoluteLayoutManager();
            IsOpaque = false;
        }

        static Overlay()
        {
            BackgroundShadeFactorProperty = BindableProperty.Create(
                nameof(BackgroundShadeFactor),
                typeof(float),
                typeof(Overlay),
                defaultValue: Single.NaN,
                propertyChanged: OnBackgroundShadeFactorPropertyChanged
            );
        }

        /*public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            surface.Copy(RenderSurface);


            if (0 < Children.Count)
            {
                RenderChildren(elapsed);
            }

            RenderSurface.Copy(surface);
        }*/

        protected override void RenderMain(ICellSurface surface, TimeSpan elapsed)
        {
            if (IsDirty || false == IsOpaque)
            {
                var factor = BackgroundShadeFactor;

                if (false == Single.IsNaN(factor))
                {
                    RenderSurface.Shade(RenderSurface.Area, backgroundFactor: factor);
                }
            }

            base.RenderMain(surface, elapsed);
        }

        protected virtual void OnBackgroundShadeFactorChanged()
        {
            Invalidate();
        }

        private static Color Shade(Color color, float factor)
        {
            var r = (byte)(color.R * (1 - factor));
            var g = (byte)(color.G * (1 - factor));
            var b = (byte)(color.B * (1 - factor));

            return new Color(r, g, b, color.A);
        }

        private static void OnBackgroundShadeFactorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Overlay)sender).OnBackgroundShadeFactorChanged();
        }
    }
}