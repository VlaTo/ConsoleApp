using ConsoleApp.Bindings;
using ConsoleApp.UI.Extensions;
using SadConsole;
using SadConsole.Input;
using Keys = SadConsole.Input.Keys;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace ConsoleApp.UI.Controls
{
    public sealed class Background : VisualGroup
    {
        public static readonly BindableProperty GlyphProperty;

        public char Glyph
        {
            get => (char)GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }

        internal WindowManager WindowManager
        {
            get;
        }

        public Background()
        {
            WindowManager = new WindowManager(this);
            LayoutManager = new AbsoluteLayoutManager();
        }

        static Background()
        {
            GlyphProperty = BindableProperty.Create(
                nameof(Glyph),
                typeof(char),
                typeof(Background),
                Glyphs.Filler0,
                OnGlyphPropertyChanged
            );
        }

        public override bool HandleKeyPressed(Keys key, ShiftKeys shiftKeys)
        {
            if (Keys.F6 == key)
            {
                return WindowManager.FocusWindow(shiftKeys.HasShift() ? MoveDirection.Previous : MoveDirection.Next);
            }

            return base.HandleKeyPressed(key, shiftKeys);
        }

        protected override void PreRender(ICellSurface surface)
        {
            if (false == IsDirty)
            {
                return;
            }

            var rectangle = new Rectangle(0, 0, Bounds.Width, Bounds.Height);

            base.PreRender(surface);

            RenderSurface.Fill(rectangle, Foreground, Background, Glyph);
        }

        private static void OnGlyphPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Background)sender).Invalidate();
        }
    }
}
