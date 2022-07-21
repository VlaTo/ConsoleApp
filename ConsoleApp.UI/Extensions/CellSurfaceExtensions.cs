using System;
using SadConsole;
using SadRogue.Primitives;

namespace ConsoleApp.UI.Extensions
{
    public static class CellSurfaceExtensions
    {
        public static void Shade(this ICellSurface surface, Rectangle area, float backgroundFactor = Single.NaN, float foregroundFactor = Single.NaN)
        {
            var shadeBackground = false == Single.IsNaN(backgroundFactor);
            var shadeForeground = false == Single.IsNaN(foregroundFactor);

            if (false == shadeBackground && false == shadeForeground)
            {
                return;
            }

            var rectangle = Rectangle.GetIntersection(surface.Area, area);
            // var shadeBackground = false == Single.IsNaN(backgroundFactor);
            // var shadeForeground = false == Single.IsNaN(foregroundFactor);

            for (var y = rectangle.MinExtentY; y <= rectangle.MaxExtentY; y++)
            {
                for (var x = rectangle.MinExtentX; x <= rectangle.MaxExtentX; x++)
                {
                    var cell = surface[x, y];

                    if (shadeBackground)
                    {
                        cell.Background = Shade(cell.Background, backgroundFactor);
                    }

                    if (shadeForeground)
                    {
                        cell.Foreground = Shade(cell.Foreground, foregroundFactor);
                    }
                }
            }
        }

        private static Color Shade(Color color, float factor)
        {
            var r = (byte)(color.R * (1 - factor));
            var g = (byte)(color.G * (1 - factor));
            var b = (byte)(color.B * (1 - factor));

            return new Color(r, g, b, color.A);
        }
    }
}