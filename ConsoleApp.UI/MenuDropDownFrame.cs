using ConsoleApp.UI.Controls;
using ConsoleApp.UI.Extensions;
using SadConsole;
using SadRogue.Primitives;

namespace ConsoleApp.UI
{
    public class MenuDropDownFrame : WindowFrame
    {
        public static readonly int[] LineThin;
        public static readonly int[] LineThick;
        public static readonly int[] LineNone;

        public MenuFlyout Flyout
        {
            get;
        }

        public MenuDropDownFrame(MenuFlyout flyout)
            : base(flyout)
        {
            Flyout = flyout;
        }

        static MenuDropDownFrame()
        {
            LineThin = new int[3] { '\xC3', ICellSurface.ConnectedLineThin[1], '\xB4' };
            LineThick = new int[3] { '\xC7', ICellSurface.ConnectedLineThin[1], '\xB6' };
            LineNone = new int[3] { '\x20', ICellSurface.ConnectedLineEmpty[3], '\x20' };
        }

        public override void Render(ICellSurface surface)
        {
            base.Render(surface);

            var rectangle = Bounds;
            var items = Flyout.MenuList.Items;
            var top = rectangle.Y + Flyout.MenuList.Bounds.Top;
            var line = GetLine();

            for (var index = 0; index < items.Count; index++)
            {
                if (items[index] is MenuDelimiter)
                {
                    var left = rectangle.X;
                    var right = rectangle.Right - 2;

                    surface.SetGlyph(left++, top, line[0]);
                    surface.DrawLine(
                        new Point(left, top),
                        new Point(right, top),
                        line[1],
                        foreground: Window.Foreground,
                        background: Window.Background
                    );
                    surface.SetGlyph(right + 1, top, line[2]);
                }

                top++;
            }
        }

        private int[] GetLine()
        {
            switch (Window.FrameType)
            {
                case WindowFrameType.Thick:
                {
                    return LineThick;
                }

                case WindowFrameType.Thin:
                {
                    return LineThin;
                }
            }

            return LineNone;
        }
    }
}