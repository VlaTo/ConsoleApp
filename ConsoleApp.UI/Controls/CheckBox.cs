using System;
using System.Drawing;
using ConsoleApp.Bindings;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using Point = System.Drawing.Point;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace ConsoleApp.UI.Controls
{
    public enum CheckBoxState
    {
        Indeterminate,
        Unchecked,
        Checked
    }

    public class CheckBox : Control
    {
        public static readonly BindableProperty StateProperty;
        public static readonly BindableProperty IsTriStateProperty;
        public static readonly BindableProperty TextProperty;

        public bool IsTriState
        {
            get => (bool)GetValue(IsTriStateProperty);
            set => SetValue(IsTriStateProperty, value);
        }

        public CheckBoxState State
        {
            get => (CheckBoxState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsChecked
        {
            get => CheckBoxState.Checked == State;
        }
        public bool IsUnchecked
        {
            get => CheckBoxState.Unchecked == State;
        }

        public bool IsIndeterminate
        {
            get => CheckBoxState.Indeterminate == State;
        }

        static CheckBox()
        {
            IsTriStateProperty = BindableProperty.Create(
                nameof(IsTriState),
                typeof(bool),
                ownerType: typeof(CheckBox),
                defaultValue: false,
                propertyChanged: OnIsTriStatePropertyChanged
            );
            StateProperty = BindableProperty.Create(
                nameof(State),
                typeof(CheckBoxState),
                ownerType: typeof(CheckBox),
                defaultValue: CheckBoxState.Unchecked,
                propertyChanged: OnStatePropertyChanged
            );
            TextProperty = BindableProperty.Create(
                nameof(Text),
                typeof(string),
                ownerType: typeof(CheckBox),
                defaultValue: null,
                propertyChanged: OnTextPropertyChanged
            );
        }

        public override void Enter()
        {
            var origin = MakeAbsolute(new Point(1, 0));
            var application = ConsoleApplication.Instance;
            var cursor = application.Screen.Cursor;

            cursor.Position = new SadRogue.Primitives.Point(origin.X, origin.Y);
            cursor.IsVisible = true;

            base.Enter();
        }

        public override void Leave()
        {
            var application = ConsoleApplication.Instance;
            var cursor = application.Screen.Cursor;

            cursor.IsVisible = false;

            base.Leave();
        }

        protected override void RenderMain(ICellSurface surface, TimeSpan elapsed)
        {
            var rectangle = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
            
            surface.Fill(rectangle, background: Background);
            
            surface.SetGlyph(rectangle.X, rectangle.Y, Glyphs.SquareBracketLeft, foreground: Foreground);
            surface.SetGlyph(rectangle.X + 2, rectangle.Y, Glyphs.SquareBracketRight, foreground: Foreground);

            if (false == IsUnchecked)
            {
                surface.SetGlyph(rectangle.X + 1, rectangle.Y, IsChecked ? Glyphs.CheckMark : Glyphs.Indeterminate, foreground: Foreground);
            }

            surface.Print(rectangle.X + 4, rectangle.Y, Text, foreground: Foreground);
        }

        public override bool HandleKeyPressed(AsciiKey key, ModificatorKeys modificators)
        {
            if (Keys.Space == key && modificators.IsEmpty)
            {
                State = NextState(State);
                return true;
            }

            return base.HandleKeyPressed(key, modificators);
        }

        protected virtual void OnIsTriStateChanged()
        {
            if (false == IsTriState && CheckBoxState.Indeterminate == State)
            {
                State = CheckBoxState.Unchecked;
            }
            else
            {
                Invalidate();
            }
        }

        protected virtual void OnStateChanged()
        {
            Invalidate();
        }

        protected virtual void OnTextChanged()
        {
            Invalidate();
        }

        private CheckBoxState NextState(CheckBoxState state)
        {
            switch (state)
            {
                case CheckBoxState.Indeterminate:
                {
                    return CheckBoxState.Checked;
                }

                case CheckBoxState.Checked:
                {
                    return CheckBoxState.Unchecked;
                }

                case CheckBoxState.Unchecked:
                {
                    return IsTriState ? CheckBoxState.Indeterminate : CheckBoxState.Checked;
                }
            }

            return CheckBoxState.Indeterminate;
        }

        private static void OnIsTriStatePropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((CheckBox)sender).OnIsTriStateChanged();
        }

        private static void OnStatePropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((CheckBox)sender).OnStateChanged();
        }

        private static void OnTextPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((CheckBox)sender).OnTextChanged();
        }
    }
}