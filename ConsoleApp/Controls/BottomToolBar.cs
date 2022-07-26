using ConsoleApp.Bindings;
using ConsoleApp.UI;
using ConsoleApp.UI.Controls;
using System.Drawing;
using SadConsole;
using Color = SadRogue.Primitives.Color;
using HorizontalAlignment = ConsoleApp.UI.HorizontalAlignment;
using VerticalAlignment = ConsoleApp.UI.VerticalAlignment;

namespace ConsoleApp.Controls
{
    public class BottomToolBar : VisualGroup
    {
        public static readonly BindableProperty HintProperty;

        private readonly Label label;
        private readonly LoadingIndicator loadingIndicator;
        
        public string Hint
        {
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        public BottomToolBar()
        {
            label = new Label
            {
                Background = Color.Transparent,
                Foreground = Foreground,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Padding = new Thickness(1, 0),
                Height = 1
            };

            loadingIndicator = new LoadingIndicator
            {
                Background = Color.Transparent,
                HorizontalAlignment = HorizontalAlignment.Right,
                Height = 1,
                Width = 24
            };

            Children.Add(label);
            //Children.Add(loadingIndicator);

            label.SetBinding(Label.TextProperty, new Binding
            {
                Source = this,
                PropertyPath = new PropertyPath(nameof(Hint))
            });
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

        /*public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            //var rectangle = new Rectangle(new Point(), Bounds.Size);
            var rectangle = Bounds.ToRectangle();

            RenderSurface.Fill(rectangle, Foreground, Background, '\x20');

            base.Render(surface, elapsed);
        }*/

        public override void Arrange(Rectangle bounds)
        {
            base.Arrange(bounds);
        }

        protected override void PreRender(ICellSurface surface)
        {
            base.PreRender(surface);

            // var rectangle=
            // RenderSurface.Fill()
        }

        protected override void OnBackgroundColorChanged()
        {
            label.Background = Background;
            loadingIndicator.Background = Background;

            base.OnBackgroundColorChanged();
        }

        protected override void OnForegroundColorChanged()
        {
            label.Foreground = Foreground;
            //loadingIndicator.Background = Background;

            base.OnForegroundColorChanged();
        }

        private void OnHintChanged()
        {
            //label.Text = Hint;
        }

        private static void OnHintPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((BottomToolBar)sender).OnHintChanged();
        }
    }
}