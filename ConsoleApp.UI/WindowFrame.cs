using System;
using ConsoleApp.UI.Controls;
using ConsoleApp.UI.Extensions;
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

        public Thickness Thickness =>
            WindowFrameType.None == Window.FrameType
                ? Thickness.Empty
                : new Thickness(1);

        public System.Drawing.Rectangle Bounds
        {
            get
            {
                if (WindowFrameType.None == Window.FrameType)
                {
                    return System.Drawing.Rectangle.Empty;
                }

                return new System.Drawing.Rectangle(
                    System.Drawing.Point.Empty,
                    Window.Bounds.Size - Window.Shadow
                );
            }
        }

        public WindowFrame(Window window)
        {
            Window = window;
        }

        public virtual void Render(ICellSurface surface)
        {
            var shadow = Window.Shadow;

            if (shadow.IsEmpty)
            {
                return;
            }

            if (WindowFrameType.None == Window.FrameType)
            {
                return;
            }

            var rectangle = Bounds.ToRectangle();
            var foreground = Window.IsFocused ? Color.White : Color.DarkGray;
            var border = new ColoredGlyph(foreground, Window.Background);
            var background = new ColoredGlyph(Color.Transparent, Window.Background);
            var connectedLineStyle = GetConnectedLineStyle(Window.FrameType);

            var options = new ShapeParameters(
                true,
                border,
                false,
                false,
                false,
                true,
                Window.IsOpaque,
                background,
                false,
                false,
                false,
                true,
                connectedLineStyle,
                null
            );

            surface.DrawBox(rectangle, options);

            //surface.ConnectLines(connectedLineStyle, rectangle);

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

        protected static int[] GetConnectedLineStyle(WindowFrameType frameType)
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
