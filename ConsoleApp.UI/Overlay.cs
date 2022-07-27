using ConsoleApp.Bindings;
using ConsoleApp.UI.Extensions;
using SadConsole;
using System;

namespace ConsoleApp.UI
{
    public class Overlay : VisualGroup
    {
        public static readonly BindableProperty ForegroundShadeFactorProperty;
        public static readonly BindableProperty BackgroundShadeFactorProperty;

        public float ForegroundShadeFactor
        {
            get => (float)GetValue(ForegroundShadeFactorProperty);
            set => SetValue(ForegroundShadeFactorProperty, value);
        }

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
            ForegroundShadeFactorProperty = BindableProperty.Create(
                nameof(ForegroundShadeFactor),
                typeof(float),
                typeof(Overlay),
                defaultValue: Single.NaN,
                propertyChanged: OnForegroundShadeFactorPropertyChanged
            );
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
                RenderSurface.Shade(
                    RenderSurface.Area,
                    foregroundFactor: ForegroundShadeFactor,
                    backgroundFactor: BackgroundShadeFactor
                );
            }

            base.RenderMain(surface, elapsed);
        }

        protected virtual void OnForegroundShadeFactorChanged()
        {
            Invalidate();
        }

        protected virtual void OnBackgroundShadeFactorChanged()
        {
            Invalidate();
        }

        private static void OnForegroundShadeFactorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Overlay)sender).OnForegroundShadeFactorChanged();
        }

        private static void OnBackgroundShadeFactorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Overlay)sender).OnBackgroundShadeFactorChanged();
        }
    }
}