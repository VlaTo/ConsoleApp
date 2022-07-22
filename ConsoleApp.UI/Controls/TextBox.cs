using ConsoleApp.Bindings;
using ConsoleApp.UI.Extensions;
using SadConsole;
using System;

namespace ConsoleApp.UI.Controls
{
    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }

    public class TextBox : VisualElement
    {
        public static readonly BindableProperty TextAlignmentProperty;
        public static readonly BindableProperty TextProperty;

        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        static TextBox()
        {
            TextAlignmentProperty = BindableProperty.Create(
                nameof(TextAlignment),
                typeof(TextAlignment),
                typeof(TextBox),
                TextAlignment.Left,
                OnTextAlignmentPropertyChanged
            );
            TextProperty = BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(TextBox),
                propertyChanged: OnTextPropertyChanged
            );
        }

        public override void Update(TimeSpan elapsed)
        {
            ;
        }

        protected override void RenderMain(ICellSurface surface, TimeSpan elapsed)
        {
            //surface.Fill(new Rectangle(0, 0, Width, Height), Foreground, Background, '\x20');
            if (String.IsNullOrEmpty(Text))
            {
                return;
            }

            var rectangle = Bounds.ToRectangle();
            surface.Fill(rectangle, Foreground, Background, Glyphs.Whitespace);

            var width = Bounds.Width - Padding.HorizontalThickness;
            var text = width < Text.Length ? Text.Substring(0, width) : Text;

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            surface.Print(Padding.Left, Padding.Top, text, Foreground, Background);

            base.RenderMain(surface, elapsed);
        }

        protected virtual void OnTextChanged()
        {
            Invalidate();
        }

        private static void OnTextAlignmentPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((TextBox)sender).Invalidate();
        }

        private static void OnTextPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((TextBox)sender).OnTextChanged();
        }
    }
}