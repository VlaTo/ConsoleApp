using System;
using System.Drawing;
using ConsoleApp.UI;
using ConsoleApp.UI.Controls;
using ConsoleApp.UI.Extensions;
using SadConsole;
//using SadRogue.Primitives;
using Color = SadRogue.Primitives.Color;
using HorizontalAlignment = ConsoleApp.UI.HorizontalAlignment;
using VerticalAlignment = ConsoleApp.UI.VerticalAlignment;

namespace ConsoleApp.Controls
{
    public class BottomToolBar : VisualGroup
    {
        public static readonly BindableProperty HintProperty;

        private readonly TextBox textBox;
        private readonly LoadingIndicator loadingIndicator;
        
        public string Hint
        {
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        public BottomToolBar()
        {
            Height = 1;
            VerticalAlignment = VerticalAlignment.Bottom;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            textBox = new TextBox
            {
                Background = Color.Transparent,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Padding = new Thickness(1, 0),
                Height = 1,
                //Width = 100
            };

            loadingIndicator = new LoadingIndicator
            {
                Background = Color.Transparent,
                HorizontalAlignment = HorizontalAlignment.Right,
                Height = 1,
                Width = 24
            };

            Children.Add(textBox);
            //Children.Add(loadingIndicator);
        }

        static BottomToolBar()
        {
            HintProperty = BindableProperty.Create(
                nameof(Hint),
                typeof(string),
                typeof(BottomToolBar),
                propertyChanged: OnHintPropertyChanged
            );
        }

        public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            //var rectangle = new Rectangle(new Point(), Bounds.Size);
            var rectangle = Bounds.ToRectangle();

            RenderSurface.Fill(rectangle, Foreground, Background, '\x20');

            base.Render(surface, elapsed);
        }

        public override void Arrange(Rectangle bounds)
        {
            base.Arrange(bounds);
        }

        protected override void OnBackgroundColorChanged()
        {
            textBox.Background = Background;
            loadingIndicator.Background = Background;

            base.OnBackgroundColorChanged();
        }

        protected override void OnForegroundColorChanged()
        {
            textBox.Foreground = Foreground;
            //loadingIndicator.Background = Background;

            base.OnForegroundColorChanged();
        }

        private void OnHintChanged()
        {
            textBox.Text = Hint;
        }

        private static void OnHintPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((BottomToolBar)sender).OnHintChanged();
        }
    }
}