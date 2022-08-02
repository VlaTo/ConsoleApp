using System;
using System.Diagnostics;
using ConsoleApp.Bindings;
using ConsoleApp.UI.Extensions;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;

namespace ConsoleApp.UI.Controls
{
    public class Entry : Control
    {
        public static readonly BindableProperty SelectionStartProperty;
        public static readonly BindableProperty SelectionLengthProperty;
        public static readonly BindableProperty SelectionTextProperty;
        public static readonly BindableProperty TextProperty;
        public static readonly BindableProperty MaxLengthProperty;

        private const int DefaultBufferSize = 128;

        private char[] buffer;
        private int length;
        private int gapStart;
        private int gapLength;
        private int scrollOffset;
        private int cursorPosition;

        public int SelectionStart
        {
            get => (int)GetValue(SelectionStartProperty);
            set => SetValue(SelectionStartProperty, value);
        }

        public int SelectionLength
        {
            get => (int)GetValue(SelectionLengthProperty);
            set => SetValue(SelectionLengthProperty, value);
        }

        public string SelectionText
        {
            get => (string)GetValue(SelectionTextProperty);
            set => SetValue(SelectionTextProperty, value);
        }

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

        public event EventHandler TextChanged;

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
            SelectionStartProperty = BindableProperty.Create(
                nameof(SelectionStart),
                typeof(int),
                ownerType: typeof(Entry),
                defaultValue: -1,
                propertyChanged: OnSelectionStartPropertyChanged
            );
            SelectionLengthProperty = BindableProperty.Create(
                nameof(SelectionLength),
                typeof(int),
                ownerType: typeof(Entry),
                defaultValue: 0,
                propertyChanged: OnSelectionLengthPropertyChanged
            );
            SelectionTextProperty = BindableProperty.Create(
                nameof(SelectionText),
                typeof(string),
                ownerType: typeof(Entry),
                defaultValue: String.Empty,
                propertyChanged: OnSelectionTextPropertyChanged
            );
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

        public override bool HandleKeyPressed(Keys key, ShiftKeys shiftKeys)
        {
            var handled= base.HandleKeyPressed(key, shiftKeys);

            if (handled)
            {
                return true;
            }

            if (Keys.Left == key)
            {
                if (0 == shiftKeys)
                {
                    MoveCursorLeft();
                }
                else if (shiftKeys.HasShift())
                {
                    ;
                }

                return true;
            }

            if (Keys.Right == key)
            {
                if (0 == shiftKeys)
                {
                    MoveCursorRight();
                }
                else if (shiftKeys.HasShift())
                {
                    ;
                }

                return true;
            }

            if (Keys.Home == key)
            {
                if (0 == shiftKeys)
                {
                    MoveCursorHome();
                }
                else if (shiftKeys.HasShift())
                {
                    ;
                }

                return true;
            }

            if (Keys.End == key)
            {
                if (0 == shiftKeys)
                {
                    MoveCursorEnd();
                }
                else if (shiftKeys.HasShift())
                {
                    ;
                }

                return true;
            }

            if (Keys.Back == key)
            {
                if (0 == shiftKeys)
                {
                    Backspace();
                    
                    Text = GetText();
                }
                else if (shiftKeys.HasShift())
                {
                    ;
                }

                return true;
            }

            var asciiKey = AsciiKey.Get(key, shiftKeys.HasShift(), KeyboardState.Empty);

            if (CanPrint(asciiKey.Character))
            {
                PutCharacter(asciiKey.Character);
                MoveCursorRight();

                Text = GetText();

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

        protected bool CanPrint(char character)
        {
            return Char.IsLetterOrDigit(character) || Char.IsPunctuation(character) || Char.IsWhiteSpace(character);
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

            scrollOffset = 0;

            Invalidate();

            RaiseTextChanged(EventArgs.Empty);
        }

        protected virtual void OnSelectionStartChanged()
        {
            ;
        }

        protected virtual void OnSelectionLengthChanged()
        {
            ;
        }

        protected virtual void OnSelectionTextChanged()
        {
            ;
        }

        private void PutCharacter(char character)
        {
            MoveGapTo(scrollOffset + cursorPosition);

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

        private void MoveCursorHome()
        {
            if (0 < cursorPosition)
            {
                cursorPosition = 0;
                SetCursor();
            }

            if (0 < scrollOffset)
            {
                scrollOffset = 0;
                Invalidate();
            }
        }

        private void MoveCursorEnd()
        {
            scrollOffset = Math.Max(length - Bounds.Width + 1, 0);
            cursorPosition = length - scrollOffset;

            SetCursor();
            Invalidate();
        }

        private void SetCursor()
        {
            var origin = MakeAbsolute(new System.Drawing.Point(cursorPosition, 0));
            Cursor.Position = new Point(origin.X, origin.Y);
        }

        private void Backspace()
        {
            var position = scrollOffset + cursorPosition;

            if (0 == position)
            {
                return;
            }

            MoveGapTo(position);

            gapStart--;
            gapLength++;
            length--;

            if (0 == cursorPosition && 0 < scrollOffset)
            {
                var offset = Bounds.Width >> 1;

                scrollOffset = Math.Max(scrollOffset - offset, 0);
                cursorPosition = Math.Min(offset - 1, length);

                SetCursor();
            }
            else if (0 < cursorPosition)
            {
                cursorPosition--;
                SetCursor();
            }

            Invalidate();
        }

        private void MoveGapTo(int position)
        {
            if (position < gapStart)
            {
                for (int destination = gapStart + gapLength - 1, index = gapStart - 1;
                     index >= position;
                     index--, destination--)
                {
                    buffer[destination] = buffer[index];
                }
            }
            else if (position > gapStart)
            {
                for (int source = gapStart + gapLength, destination = gapStart, index = gapStart;
                     index < position;
                     index++)
                {
                    buffer[destination++] = buffer[source++];
                }
            }
            else
            {
                return;
            }
            
            gapStart = position;
        }

        private void DrawText(ICellSurface surface, Rectangle rectangle)
        {
            if (0 == length)
            {
                return;
            }

            var x = rectangle.X;
            var count = Math.Min(length - scrollOffset, rectangle.Width);

            for (var index = 0; index < count; index++)
            {
                var position = scrollOffset + index;

                if (position >= gapStart)
                {
                    position += gapLength;
                }

                var character = buffer[position];

                surface.SetGlyph(x++, rectangle.Y, character, foreground: Foreground);
            }
        }

        private string GetText()
        {
            if (0 == length)
            {
                return String.Empty;
            }

            var symbols = new char[length];

            for (int index = 0, position = 0; index < length; index++)
            {
                if (position == gapStart)
                {
                    position += gapLength;
                }

                symbols[index] = buffer[position++];
            }

            return new string(symbols);
        }

        private void RaiseTextChanged(EventArgs e)
        {
            var handler = TextChanged;

            if (null != handler)
            {
                handler.Invoke(this, e);
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

        private static void OnSelectionStartPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Entry)sender).OnSelectionStartChanged();
        }

        private static void OnSelectionLengthPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Entry)sender).OnSelectionLengthChanged();
        }

        private static void OnSelectionTextPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Entry)sender).OnSelectionTextChanged();
        }
    }
}