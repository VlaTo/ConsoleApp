using SadRogue.Primitives;

namespace ConsoleApp.UI.Extensions
{
    public static class RectangleExtensions
    {
        public static Rectangle ToRectangle(this System.Drawing.Rectangle rectangle)
        {
            //return new Rectangle(new Point(rectangle.X, rectangle.Y), new Point(rectangle.Right, rectangle.Bottom));
            return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}