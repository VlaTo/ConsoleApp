using ConsoleApp.Bindings;
using ConsoleApp.UI.Extensions;
using SadConsole;
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

        private static void OnBackgroundShadeFactorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Overlay)sender).OnBackgroundShadeFactorChanged();
        }
    }
}