using ConsoleApp.Bindings;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Windows.Input;
using SadConsole.Input;

namespace ConsoleApp.UI.Controls
{
    internal class Button : Control
    {
        public static readonly BindableProperty ActiveColorProperty;
        public static readonly BindableProperty BackgroundColorProperty;
        public static readonly BindableProperty CommandProperty;
        public static readonly BindableProperty CommandParameterProperty;
        public static readonly BindableProperty DisabledColorProperty;
        public static readonly BindableProperty InactiveColorProperty;
        public static readonly BindableProperty IsCancelProperty;
        public static readonly BindableProperty IsDefaultProperty;
        public static readonly BindableProperty TextProperty;

        public Color ActiveColor
        {
            get => (Color)GetValue(ActiveColorProperty);
            set => SetValue(ActiveColorProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public Color InactiveColor
        {
            get => (Color)GetValue(InactiveColorProperty);
            set => SetValue(InactiveColorProperty, value);
        }

        public Color DisabledColor
        {
            get => (Color)GetValue(DisabledColorProperty);
            set => SetValue(DisabledColorProperty, value);
        }

        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public bool IsCancel
        {
            get => (bool)GetValue(IsCancelProperty);
            set => SetValue(IsCancelProperty, value);
        }

        public bool IsDefault
        {
            get => (bool)GetValue(IsDefaultProperty);
            set => SetValue(IsDefaultProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public event EventHandler Clicked;

        public Button()
        {
            Foreground = Color.White;
            Height = 2;
        }

        static Button()
        {
            ActiveColorProperty = BindableProperty.Create(
                nameof(ActiveColor),
                typeof(Color),
                ownerType: typeof(Button),
                defaultValue: Color.White,
                propertyChanged: OnBackgroundColorPropertyChanged
            );
            BackgroundColorProperty = BindableProperty.Create(
                nameof(Background),
                typeof(Color),
                ownerType: typeof(Button),
                defaultValue: Color.Green,
                propertyChanged: OnBackgroundColorPropertyChanged
            );
            CommandProperty = BindableProperty.Create(
                nameof(Command),
                typeof(ICommand),
                ownerType: typeof(Button),
                defaultValue: null,
                propertyChanged: OnCommandPropertyChanged
            );
            CommandParameterProperty = BindableProperty.Create(
                nameof(CommandParameter),
                typeof(object),
                ownerType: typeof(Button),
                defaultValue: null,
                propertyChanged: OnCommandParameterPropertyChanged
            );
            DisabledColorProperty = BindableProperty.Create(
                nameof(DisabledColor),
                typeof(Color),
                ownerType: typeof(Button),
                defaultValue: Color.Gray,
                propertyChanged: OnBackgroundColorPropertyChanged
            );
            InactiveColorProperty = BindableProperty.Create(
                nameof(InactiveColor),
                typeof(Color),
                ownerType: typeof(Button),
                defaultValue: Color.Black,
                propertyChanged: OnBackgroundColorPropertyChanged
            );
            IsCancelProperty = BindableProperty.Create(
                nameof(IsCancel),
                typeof(bool),
                ownerType: typeof(Button),
                defaultValue: false,
                propertyChanged: OnIsCancelPropertyChanged
            );
            IsDefaultProperty = BindableProperty.Create(
                nameof(IsDefault),
                typeof(bool),
                ownerType: typeof(Button),
                defaultValue: false,
                propertyChanged: OnIsDefaultPropertyChanged
            );
            TextProperty = BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(Button),
                propertyChanged: OnTextPropertyChanged
            );
        }

        public void Click()
        {
            if (false == Enabled())
            {
                return;
            }

            if (null != Command)
            {
                var parameter = CommandParameter;
                Command.Execute(parameter);
            }

            RaiseOnClick(EventArgs.Empty);
        }

        public override bool HandleKeyPressed(Keys key, ShiftKeys shiftKeys)
        {
            var handled = base.HandleKeyPressed(key, shiftKeys);

            if (handled)
            {
                return true;
            }

            if (Keys.Space == key && 0 == shiftKeys)
            {
                Click();

                return true;
            }

            return false;
        }

        protected override void RenderMain(ICellSurface surface, TimeSpan elapsed)
        {
            var rectangle = new Rectangle(0, 0, Bounds.Width - 1, Bounds.Height - 1);
            surface.Fill(rectangle, background: BackgroundColor);
            surface.SetGlyph(Bounds.Width - 1, 0, Glyphs.Box0, foreground: Color.Black);
            for (var position = 1; position < Bounds.Width; position++)
            {
                surface.SetGlyph(position, Bounds.Height - 1, Glyphs.Box1, foreground: Color.Black);
            }

            var caption = Text;
            var offset = (rectangle.Width - caption.Length) >> 1;
            var foreground = GetTextForegroundColor();

            surface.Print(offset, 0, caption, foreground: foreground);
        }

        protected virtual void OnBackgroundChanged()
        {
            Invalidate();
        }

        protected virtual void OnCommandChanged()
        {
            Invalidate();
        }

        protected virtual void OnCommandParameterChanged()
        {
            ;
        }

        protected virtual void OnTextChanged()
        {
            ;
        }

        protected virtual void OnIsCancelChanged()
        {
            ;
        }

        protected virtual void OnIsDefaultChanged()
        {
            ;
        }

        private bool Enabled()
        {
            var enabled = IsVisible && IsEnabled;

            if (false == enabled)
            {
                return false;
            }

            if (null != Command)
            {
                var parameter = CommandParameter;
                return Command.CanExecute(parameter);
            }

            return true;
        }

        private Color GetTextForegroundColor()
        {
            if (false == Enabled())
            {
                return DisabledColor;
            }

            if (IsFocused)
            {
                return ActiveColor;
            }

            return IsDefault ? Color.Cyan : InactiveColor;
        }

        private void RaiseOnClick(EventArgs e)
        {
            var handler = Clicked;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }

        private static void OnIsCancelPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Button)sender).OnIsCancelChanged();
        }

        private static void OnIsDefaultPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Button)sender).OnIsDefaultChanged();
        }

        private static void OnBackgroundColorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Button)sender).OnBackgroundChanged();
        }

        private static void OnCommandPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Button)sender).OnCommandChanged();
        }

        private static void OnCommandParameterPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Button)sender).OnCommandParameterChanged();
        }

        private static void OnTextPropertyChanged(BindableObject sender, object newValue, object oldValue)
        {
            ((Button)sender).OnTextChanged();
        }
    }
}
