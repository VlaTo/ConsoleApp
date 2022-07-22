using System;
using System.Threading;
using ConsoleApp.Bindings;
using ConsoleApp.UI;
using SadConsole;
using SadRogue.Primitives;

namespace ConsoleApp.Controls
{
    public class LoadingIndicator : VisualElement
    {
        private const int IndicatorWidth = 3;
        private const int Forward = 1;
        private const int Backward = -1;

        private readonly Timer timer;
        private int offset;
        private int direction;
        
        public LoadingIndicator()
        {
            var timeout = TimeSpan.FromSeconds(0.1d);

            offset = 0;
            direction = Forward;
            timer = new Timer(OnTimerTick, null, timeout, timeout);
        }

        /*static LoadingIndicator()
        {
            BackgroundProperty = BindableProperty.Create(
                nameof(Background),
                typeof(Color),
                typeof(LoadingIndicator),
                Color.Black,
                OnBackgroundColorPropertyChanged
            );
        }*/

        public override void Update(TimeSpan elapsed)
        {
            ;
        }

        public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            var bounds = new Rectangle(0, 0, Width, Height);
            
            surface.Fill(bounds, Foreground, Background, '\xB0');
            surface.Print(bounds.X + offset, bounds.Y, new string('\xDB', IndicatorWidth), Foreground);
        }

        private void OnTimerTick(object _)
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
        }

        private static void OnBackgroundColorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((LoadingIndicator)sender).Invalidate();
        }
    }
}