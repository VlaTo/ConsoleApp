using SadConsole;
using SadRogue.Primitives;
using System;

namespace ConsoleApp.UI.Controls
{
    public enum WindowFrameType
    {
        None,
        Thick,
        Thin
    }

    public class Window : VisualGroup
    {
        public static readonly BindableProperty FrameTypeProperty;
        public static readonly BindableProperty TitleProperty;

        public WindowFrameType FrameType
        {
            get => (WindowFrameType)GetValue(FrameTypeProperty);
            set => SetValue(FrameTypeProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public WindowFrame Frame
        {
            get;
        }

        public Window()
        {
            Frame = new WindowFrame(this);
        }

        static Window()
        {
            FrameTypeProperty = BindableProperty.Create(
                nameof(FrameType),
                typeof(WindowFrameType),
                typeof(Window),
                WindowFrameType.Thick,
                OnFrameTypePropertyChanged
            );
            TitleProperty = BindableProperty.Create(
                nameof(Title),
                typeof(string),
                typeof(Window),
                propertyChanged: OnTitlePropertyChanged
            );
        }

        public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            if (IsDirty)
            {
                var rectangle = new Rectangle(0, 0, Bounds.Width, Bounds.Height);

                RenderSurface.Fill(rectangle, Foreground, Background, Glyphs.Whitespace);
                Frame.Render(RenderSurface);
            }

            base.Render(surface, elapsed);
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Leave()
        {
            base.Leave();
        }

        protected override void OnChildAdded(VisualElement view)
        {
        }

        protected virtual void OnFrameTypeChanged()
        {
            ;
        }

        protected virtual void OnTitleChanged()
        {
            ;
        }

        private static void OnFrameTypePropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Window)sender).OnFrameTypeChanged();
        }

        private static void OnTitlePropertyChanged(object sender, object newValue, object oldValue)
        {
            ((Window)sender).OnTitleChanged();
        }
    }
}
