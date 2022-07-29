using ConsoleApp.Bindings;
using ConsoleApp.UI;
using ConsoleApp.UI.Controls;
using Color = SadRogue.Primitives.Color;
using HorizontalAlignment = ConsoleApp.UI.HorizontalAlignment;

namespace ConsoleApp.Controls
{
    public class BottomToolBar : VisualGroup
    {
        public static readonly BindableProperty HintProperty;

        private readonly Label label;
        private readonly ProgressBar progressBar;
        
        public string Hint
        {
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        public BottomToolBar()
        {
            label = new Label
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Padding = new Thickness(1, 0),
                Height = 1
            };

            progressBar = new ProgressBar
            {
                Foreground = Color.DarkGray,
                HorizontalAlignment = HorizontalAlignment.Right,
                Height = 1,
                Width = 10,
                IsVisible = true,
                Value = 0.4d
            };

            Children.Add(label);
            Children.Add(progressBar);

            label.SetBinding(ForegroundProperty, new Binding
            {
                Source = this,
                PropertyPath = new PropertyPath(nameof(Foreground))
            });
            label.SetBinding(BackgroundProperty, new Binding
            {
                Source = this,
                PropertyPath = new PropertyPath(nameof(Background))
            });
            label.SetBinding(Label.TextProperty, new Binding
            {
                Source = this,
                PropertyPath = new PropertyPath(nameof(Hint))
            });

            /*progressBar.SetBinding(ForegroundProperty, new Binding
            {
                Source = this,
                PropertyPath = new PropertyPath(nameof(Foreground))
            });*/
            progressBar.SetBinding(BackgroundProperty, new Binding
            {
                Source = this,
                PropertyPath = new PropertyPath(nameof(Background))
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

        protected virtual void OnHintChanged()
        {
            ;
        }

        private static void OnHintPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((BottomToolBar)sender).OnHintChanged();
        }
    }
}