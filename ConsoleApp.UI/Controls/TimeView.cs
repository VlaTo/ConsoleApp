using System;
using SadConsole;
using SadRogue.Primitives;

namespace ConsoleApp.UI.Controls
{
    public class TimeView : VisualElement
    {
        public static readonly BindableProperty ClockProperty;
        private static readonly TimeSpan Interval;

        private TimeSpan duration;

        public IClock Clock
        {
            get => (IClock)GetValue(ClockProperty);
            set => SetValue(ClockProperty, value);
        }

        static TimeView()
        {
            Interval = TimeSpan.FromSeconds(0.5d);
            ClockProperty = BindableProperty.Create(
                nameof(Clock),
                typeof(IClock),
                typeof(TimeView),
                propertyChanged: OnClockPropertyChanged
            );
        }

        public override void Update(TimeSpan elapsed)
        {
            if (null != Clock)
            {
                duration += elapsed;

                if (Interval < duration)
                {
                    duration = TimeSpan.Zero;
                    Invalidate();
                }
            }

            base.Update(elapsed);
        }

        protected override void RenderMain(ICellSurface surface, TimeSpan elapsed)
        {
            var rectangle = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
            
            surface.Fill(rectangle, Foreground, Background, Glyphs.Whitespace);
            RenderTime(surface, rectangle, Clock);

            base.RenderMain(surface, elapsed);
        }

        protected virtual void OnClockChanged()
        {
            Invalidate();
        }

        private void RenderTime(ICellSurface surface, Rectangle bounds, IClock clock)
        {
            if (null == clock)
            {
                return;
            }

            var now = clock.UtcNow.ToLocalTime();

            surface.Print(2, 0, now.Hour.ToString("D2"), Foreground);
            surface[4, 0].Glyph = (0 == now.Second % 2) ? Glyphs.Colon : Glyphs.Whitespace;
            surface.Print(5, 0, now.Minute.ToString("D2"), Foreground);
        }

        private static void OnClockPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((TimeView)sender).OnClockChanged();
        }
    }
}