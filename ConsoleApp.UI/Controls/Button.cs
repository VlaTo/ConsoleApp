using SadConsole;
using System;

namespace ConsoleApp.UI.Controls
{
    internal class Button : VisualElement
    {
        public static readonly BindableProperty TextProperty;

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        static Button()
        {
            TextProperty = BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(Button),
                propertyChanged: OnTextPropertyChanged
            );
        }

        public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            ;
        }

        public override void Invalidate()
        {
            ;
        }

        protected virtual void OnTextChanged()
        {
            ;
        }

        private static void OnTextPropertyChanged(BindableObject sender, object newValue, object oldValue)
        {
            ((Button)sender).OnTextChanged();
        }
    }
}
