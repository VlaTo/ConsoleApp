using System;
using System.Diagnostics;
using ConsoleApp.Bindings;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;

namespace ConsoleApp.UI.Controls
{
    public class Entry : Control
    {
        public static readonly BindableProperty TextProperty;
        public static readonly BindableProperty MaxLengthProperty;

        private const int DefaultBufferSize = 128;

        private char[] buffer;
        private int length;
        private int gapStart;
        private int gapLength;
        private int scrollOffset;
        private int cursorPosition;

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        internal Cursor Cursor
        {
            get;
        }

        public Entry()
        {
            buffer = null;
            length = 0;
            gapStart = -1;
            gapLength = 0;
            scrollOffset = 0;
            cursorPosition = 0;

            var application = ConsoleApplication.Instance;

            Cursor = application.Screen.Cursor;
        }

        static Entry()
        {
            MaxLengthProperty = BindableProperty.Create(
                nameof(MaxLength),
                typeof(int),
                ownerType: typeof(Entry),
                defaultValue: -1,
                propertyChanged: OnMaxLengthPropertyChanged
            );
            TextProperty = BindableProperty.Create(
                nameof(Text),
                typeof(string),
                ownerType: typeof(Entry),
                defaultValue: null,
                propertyChanged: OnTextPropertyChanged
            );
        }

        public override void Enter()
        {
            SetCursor();

            Cursor.IsVisible = true;

            base.Enter();
        }

        public override void Leave()
        {
            Cursor.IsVisible = false;

            base.Leave();
        }

        public override bool HandleKeyPressed(AsciiKey key, ModificatorKeys modificators)
        {
            var handled= base.HandleKeyPressed(key, modificators);

            if (handled)
            {
                return true;
            }

            if (Keys.Left == key)
            {
                if (modificators.IsEmpty)
                {
                    MoveCursorLeft();
                }
                else if (modificators.IsShiftPressed)
                {
                    ;
                }

                return true;
            }

            if (Keys.Right == key)
            {
                if (modificators.IsEmpty)
                {
                    MoveCursorRight();
                }
                else if (modificators.IsShiftPressed)
                {
                    ;
                }

                return true;
            }

            if (Char.IsLetterOrDigit(key.Character))
            {
                PutCharacter(key.Character);
                MoveCursorRight();

                return true;
            }

            return false;
        }

        protected override void PreRender(ICellSurface surface)
        {
            var rectangle = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
            surface.Fill(rectangle, foreground: Foreground, background: Background);
        }

        protected override void RenderMain(ICellSurface surface, TimeSpan elapsed)
        {
            var rectangle = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
            DrawText(surface, rectangle);
        }

        protected virtual void OnMaxLengthChanged()
        {
            ;
        }

        protected virtual void OnTextChanged()
        {
            var value = Text;


            if (null == value)
            {
                buffer = null;
                length = 0;
                gapStart = -1;
                gapLength = 0;
            }
            else if (0 == value.Length)
            {
                buffer = new char[Math.Max(MaxLength, DefaultBufferSize)];
                length = 0;
                gapStart = -1;
                gapLength = 0;
            }
            else
            {
                var size = Math.Max(MaxLength, DefaultBufferSize);

                length = Math.Min(value.Length, size);
                buffer = new char[Math.Max(length, DefaultBufferSize)];

                if (0 < value.Length)
                {
                    value.CopyTo(0, buffer, 0, length);
                    gapStart = length;
                    gapLength = buffer.Length - length;
                }
            }

            //gapStart = -1;
            //gapLength = 0;
            scrollOffset = 0;

            Invalidate();
        }

        private void PutCharacter(char character)
        {
            var position = scrollOffset + cursorPosition;

            if (position < gapStart)
            {
                for (var destination = gapStart + gapLength - 1; gapStart >= position; gapStart--)
                {
                    buffer[destination--] = buffer[gapStart];
                }

                gapStart = position;
            }
            else if (position > gapStart)
            {
                var source = gapStart + gapLength;

                while (gapStart < position)
                {
                    buffer[gapStart++] = buffer[source++];
                }
            }

            buffer[gapStart++] = character;
            gapLength--;
            length++;

            Invalidate();
        }

        private void MoveCursorLeft()
        {
            if (0 < cursorPosition)
            {
                cursorPosition--;
                SetCursor();

                return;
            }

            if (0 < scrollOffset)
            {
                scrollOffset--;
                Invalidate();
            }
        }

        private void MoveCursorRight()
        {
            var position = scrollOffset + cursorPosition;

            if (position > length)
            {
                return;
            }

            var width = (Bounds.Width - 1);

            if (width > cursorPosition)
            {
                cursorPosition++;
                SetCursor();

                return;
            }

            var maxOffset = Math.Max(length - Bounds.Width + 1, 0);

            if (scrollOffset < maxOffset)
            {
                scrollOffset++;
                Invalidate();
            }
        }

        private void SetCursor()
        {
            var origin = MakeAbsolute(new System.Drawing.Point(cursorPosition, 0));
            Cursor.Position = new Point(origin.X, origin.Y);
        }

        private void DrawText(ICellSurface surface, Rectangle rectangle)
        {
            if (0 == length)
            {
                return;
            }

            var x = rectangle.X;
            var start = scrollOffset;
            var count = length - start;

            for (var index = start; index < gapStart && 0 < count; index++, count--)
            {
                var character = buffer[index];
                surface.SetGlyph(x++, rectangle.Y, character, foreground: Foreground);
            }

            var offset = gapStart + gapLength - 1;

            for (var index = 0; index < count; index++)
            {
                var character = buffer[offset + index];
                surface.SetGlyph(x++, rectangle.Y, character, foreground: Foreground);
            }
        }

        private static void OnMaxLengthPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Entry)sender).OnMaxLengthChanged();
        }

        private static void OnTextPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Entry)sender).OnTextChanged();
        }
    }
}