using System;
using ConsoleApp.UI.Controls;
using SadConsole;
using SadRogue.Primitives;

namespace ConsoleApp.UI
{
    public static class Glyphs
    {
        public const char Whitespace = '\x20';

        public const char Colon = ':';

        public const char Filler0 = '\xB1';
    }

    public class WindowFrame
    {
        public Window Window
        {
            get;
        }

        public WindowFrame(Window window)
        {
            Window = window;
        }

        public void Render(ICellSurface surface)
        {
            //SadConsole.Shapes.Box box = SadConsole.Shapes.Box.GetDefaultBox();
            //var rect = new Rectangle(0, 0, window.Width, window.Height);
            var rect = new Rectangle(0, 0, Window.Bounds.Width, Window.Bounds.Height);
            var foreground = Window.IsFocused ? Color.White : Color.DarkGray;
            var border = new ColoredGlyph(foreground, Window.Background);
            var background = new ColoredGlyph(Color.Transparent, Window.Background);

            if (WindowFrameType.None == Window.FrameType)
            {
                surface.Fill(rect, Color.Transparent, Window.Background, Glyphs.Whitespace);
                return;
            }

            var connectedLineStyle = GetConnectedLineStyle(Window.FrameType);

            var temp = new ShapeParameters(
                true,
                border,
                false,
                false,
                false,
                true,
                true,
                background,
                false,
                false,
                false,
                true,
                connectedLineStyle,
                ColoredGlyph.CreateArray(8)
            );

            surface.DrawBox(rect, temp);

            if (String.IsNullOrEmpty(Window.Title))
            {
                return;
            }

            var length = Window.Title.Length;
            var space = Window.Bounds.Width - 4;
            var width = Math.Min(length, space);
            var offset = (space - width) >> 1;

            surface[offset, 0].Glyph = Glyphs.Whitespace;
            surface[offset + width + 1, 0].Glyph = Glyphs.Whitespace;
            surface.Print(offset + 1, 0, Window.Title, foreground);
        }

        private static int[] GetConnectedLineStyle(WindowFrameType frameType)
        {
            switch (frameType)
            {
                case WindowFrameType.Thick:
                {
                    return ICellSurface.ConnectedLineThick;
                }

                case WindowFrameType.Thin:
                {
                    return ICellSurface.ConnectedLineThin;
                }

                default:
                {
                    return ICellSurface.ConnectedLineEmpty;
                }
            }
        }
    }
}
