using SadConsole;
using System;
using SadRogue.Primitives;

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

        public override void Invalidate()
        {
            ;
        }

        protected override void PreRender(ICellSurface surface)
        {
            var rectangle = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
            surface.Fill(rectangle, background: Background);
        }

        protected override void RenderMain(ICellSurface surface, TimeSpan elapsed)
        {
            base.RenderMain(surface, elapsed);
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
