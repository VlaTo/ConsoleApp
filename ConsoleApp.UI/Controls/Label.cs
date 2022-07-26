using ConsoleApp.Bindings;
using ConsoleApp.UI.Extensions;
using SadConsole;
using System;
using System.Drawing;

namespace ConsoleApp.UI.Controls
{
    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }

    public enum TextTrimming
    {
        None,
        CharacterEllipsis,
        WordEllipsis
    }

    public class Label : VisualElement
    {
        private const string ellipsis = "...";

        public static readonly BindableProperty TextAlignmentProperty;
        public static readonly BindableProperty TextTrimmingProperty;
        public static readonly BindableProperty TextProperty;

        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        public TextTrimming TextTrimming
        {
            get => (TextTrimming)GetValue(TextTrimmingProperty);
            set => SetValue(TextTrimmingProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        static Label()
        {
            TextAlignmentProperty = BindableProperty.Create(
                nameof(TextAlignment),
                typeof(TextAlignment),
                ownerType: typeof(Label),
                defaultValue: TextAlignment.Left,
                propertyChanged: OnTextAlignmentPropertyChanged
            );
            TextTrimmingProperty = BindableProperty.Create(
                nameof(TextTrimming),
                typeof(TextTrimming),
                ownerType: typeof(Label),
                defaultValue: TextTrimming.None,
                propertyChanged: OnTextTrimmingPropertyChanged
            );
            TextProperty = BindableProperty.Create(
                nameof(Text),
                typeof(string),
                ownerType: typeof(Label),
                propertyChanged: OnTextPropertyChanged
            );
        }

        protected override void RenderMain(ICellSurface surface, TimeSpan elapsed)
        {
            var rectangle = Bounds.ToRectangle();
            surface.Fill(rectangle, Foreground, Background, Glyphs.Whitespace);

            if (String.IsNullOrEmpty(Text))
            {
                return;
            }

            var area = new Rectangle(
                Padding.Left,
                Padding.Top,
                Bounds.Width - Padding.HorizontalThickness,
                Bounds.Height - Padding.VerticalThickness
            );

            DrawText(surface, area);

            base.RenderMain(surface, elapsed);
        }

        public override Size Measure(int widthConstraint, int heightConstraint)
        {
            return base.Measure(widthConstraint, heightConstraint);
        }

        protected void DrawText(ICellSurface surface, Rectangle rectangle)
        {
            switch (TextAlignment)
            {
                case TextAlignment.Left:
                {
                    DrawLeftAligned(surface, rectangle);
                    break;
                }

                case TextAlignment.Center:
                {
                    DrawCentered(surface, rectangle);
                    break;
                }

                case TextAlignment.Right:
                {
                    DrawRightAligned(surface, rectangle);
                    break;
                }
            }
        }

        protected virtual void OnTextChanged()
        {
            Invalidate();
        }

        protected virtual void OnTextTrimmingChanged()
        {
            Invalidate();
        }

        private void DrawLeftAligned(ICellSurface surface, Rectangle rectangle)
        {
            var text = TrimText(rectangle.Width);
            surface.Print(rectangle.Left, rectangle.Top, text, Foreground, Background);
        }

        private void DrawCentered(ICellSurface surface, Rectangle rectangle)
        {
            throw new NotImplementedException();
        }

        private void DrawRightAligned(ICellSurface surface, Rectangle rectangle)
        {
            throw new NotImplementedException();
        }

        private string TrimText(int width)
        {
            if (width >= Text.Length)
            {
                return Text;
            }

            switch (TextTrimming)
            {
                case TextTrimming.None:
                {
                    return Text.Substring(0, width);
                }

                case TextTrimming.CharacterEllipsis:
                {
                    var position = width - ellipsis.Length;

                    while (Char.IsWhiteSpace(Text[position]) && 0 < position)
                    {
                        position--;
                    }

                    var line = Text.Substring(0, position) + ellipsis;

                    return line;
                }

                case TextTrimming.WordEllipsis:
                {
                    throw new NotImplementedException();
                }

                default:
                {
                    throw new Exception();
                }
            }
        }

        private static void OnTextAlignmentPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Label)sender).Invalidate();
        }

        private static void OnTextTrimmingPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Label)sender).OnTextTrimmingChanged();
        }

        private static void OnTextPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Label)sender).OnTextChanged();
        }
    }
}