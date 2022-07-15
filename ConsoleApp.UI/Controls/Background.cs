using SadConsole;
using SadConsole.Input;
using System;
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

        internal WindowsManager WindowsManager
        {
            get;
        }

        public Background()
        {
            WindowsManager = new WindowsManager(this);
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

        public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            if (IsDirty)
            {
                var rectangle = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
                RenderSurface.Fill(rectangle, Foreground, Background, Glyph);

                /*for (var line = 0; line < RenderSurface.Height; line++)
                {
                    for (var column = 0; column < RenderSurface.Width; column++)
                    {
                        var position = RenderSurface.Width * line + column;
                        var cell = RenderSurface[position];

                        cell.Glyph = (position % 10) + '0';
                        cell.Background = Background;
                        cell.Foreground = Foreground;
                    }
                }*/
            }

            base.Render(surface, elapsed);
        }

        public override bool HandleKeyPressed(AsciiKey key, ModificatorKeys modificators)
        {
            if (Keys.F6 == key)
            {
                return WindowsManager.FocusWindow(modificators.IsShiftPressed ? MoveDirection.Previous : MoveDirection.Next);
            }

            return base.HandleKeyPressed(key, modificators);
        }

        private static void OnGlyphPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Background)sender).Invalidate();
        }
    }
}
