using System;
using System.Diagnostics;
using System.Threading;
using ConsoleApp.Bindings;
using ConsoleApp.UI;
using SadConsole;
using SadRogue.Primitives;

namespace ConsoleApp.Controls
{
    public class ProgressBar : VisualElement
    {
        public static readonly BindableProperty IsIndeterminateProperty;
        public static readonly BindableProperty ValueProperty;
        public static readonly BindableProperty MaximumProperty;
        public static readonly BindableProperty MinimumProperty;
        
        private static readonly TimeSpan IndeterminateTimeout = TimeSpan.FromSeconds(0.1d);

        private bool isIndeterminate;

        //private const int IndicatorWidth = 3;
        private const int Forward = 1;
        private const int Backward = -1;

        private TimeSpan lastElapsed;
        //private readonly Timer timer;
        private int gutterWidth;
        private int offset;
        private int direction;

        public bool IsIndeterminate
        {
            get => (bool)GetValue(IsIndeterminateProperty);
            set => SetValue(IsIndeterminateProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public ProgressBar()
        {
            //var timeout = TimeSpan.FromSeconds(0.1d);

            lastElapsed = TimeSpan.Zero;
            offset = 0;
            direction = Forward;
            // timer = new Timer(OnTimerTick, null, timeout, timeout);
        }

        static ProgressBar()
        {
            IsIndeterminateProperty = BindableProperty.Create(
                nameof(IsIndeterminate),
                typeof(bool),
                ownerType: typeof(ProgressBar),
                defaultValue: false,
                OnIsIndeterminatePropertyChanged
            );
            MaximumProperty = BindableProperty.Create(
                nameof(Maximum),
                typeof(double),
                ownerType: typeof(ProgressBar),
                defaultValue: 1.0d,
                OnMaximumPropertyChanged
            );
            MinimumProperty = BindableProperty.Create(
                nameof(Minimum),
                typeof(double),
                ownerType: typeof(ProgressBar),
                defaultValue: 0.0d,
                OnMinimumPropertyChanged
            );
            ValueProperty = BindableProperty.Create(
                nameof(Value),
                typeof(double),
                ownerType: typeof(ProgressBar),
                defaultValue: 0.0d,
                OnValuePropertyChanged
            );
        }

        public override void Update(TimeSpan elapsed)
        {
            if (false == isIndeterminate)
            {
                return;
            }

            lastElapsed += elapsed;

            if (IndeterminateTimeout < lastElapsed)
            {
                lastElapsed = TimeSpan.Zero;
                UpdateIndeterminate();
                Invalidate();
            }
        }

        protected override void RenderMain(ICellSurface surface, TimeSpan elapsed)
        {
            var rectangle = new Rectangle(0, 0, Width, Height);

            surface.Fill(rectangle, Foreground, Background, '\xB0');

            if (isIndeterminate)
            {
                DrawIndeterminate(surface, rectangle);
            }
            else
            {
                DrawDeterminate(surface, rectangle);
            }
        }

        public override void Arrange(System.Drawing.Rectangle bounds)
        {
            base.Arrange(bounds);

            if (isIndeterminate)
            {
                gutterWidth = Math.Max((int)(Bounds.Width / 4), 1);
            }
        }

        private void DrawIndeterminate(ICellSurface surface, Rectangle rectangle)
        {
            //surface.Print(bounds.X + offset, bounds.Y, new string('\xDB', IndicatorWidth), Foreground);
            for (var index = 0; index < gutterWidth; index++)
            {
                surface.SetGlyph(rectangle.X + offset + index, rectangle.Y, '\xDB');
            }
        }

        private void DrawDeterminate(ICellSurface surface, Rectangle rectangle)
        {
            //var range = Maximum - Minimum;
            var value = Math.Min(Math.Max(Minimum, Value), Maximum);
            var percentage = value / (Maximum - Minimum);

            var length = (int)(Width * percentage);

            for (var index = 0; index < length; index++)
            {
                surface.SetGlyph(rectangle.X + index, rectangle.Y, '\xDB');
            }
        }

        private void UpdateIndeterminate()
        {
            var width = Width - gutterWidth;

            if (Forward == direction)
            {
                if (width <= offset)
                {
                    direction = Backward;
                }
            }
            else if (Backward == direction)
            {
                if (0 >= offset)
                {
                    direction = Forward;
                }
            }

            offset += direction;
        }

        /*private void OnTimerTick(object _)
        {
            var width = Width - IndicatorWidth;

            if (Forward == direction)
            {
                if (width <= offset)
                {
                    direction = Backward;
                }
            }
            else if (Backward == direction)
            {
                if (0 >= offset)
                {
                    direction = Forward;
                }
            }

            offset += direction;

            Invalidate();
        }*/


        protected virtual void OnIsIndeterminateChanged()
        {
            isIndeterminate = IsIndeterminate;

            if (isIndeterminate)
            {
                gutterWidth = Math.Max((int)(Bounds.Width / 4), 1);
                lastElapsed = TimeSpan.Zero;
            }

            Invalidate();
        }

        protected virtual void OnMaximumChanged()
        {
            ;
        }

        protected virtual void OnMinimumChanged()
        {
            ;
        }

        protected virtual void OnValueChanged()
        {
            ;
        }

        private static void OnIsIndeterminatePropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((ProgressBar)sender).OnIsIndeterminateChanged();
        }

        private static void OnMinimumPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((ProgressBar)sender).OnMinimumChanged();
        }

        private static void OnMaximumPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((ProgressBar)sender).OnMaximumChanged();
        }

        private static void OnValuePropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((ProgressBar)sender).OnValueChanged();
        }
    }
}