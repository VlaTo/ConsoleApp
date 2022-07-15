using SadConsole;
using SadRogue.Primitives;
using System;

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

        public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            surface.Copy(RenderSurface);

            var factor = BackgroundShadeFactor;

            if (false == Single.IsNaN(factor))
            {
                for (var line = 0; line < RenderSurface.Height; line++)
                {
                    for (var column = 0; column < RenderSurface.Width; column++)
                    {
                        var index = RenderSurface.Width * line + column;
                        var cell = RenderSurface[index];

                        cell.Background = Shade(cell.Background, factor);
                        //cell.Foreground = Shade(cell.Foreground, 025f);
                    }
                }
            }

            if (0 < Children.Count)
            {
                RenderChildren(elapsed);
            }

            RenderSurface.Copy(surface);
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