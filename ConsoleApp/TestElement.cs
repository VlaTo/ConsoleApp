using System;
using ConsoleApp.UI;
using SadConsole;
using SadRogue.Primitives;

namespace ConsoleApp
{
    public class TestElement : VisualElement
    {
        public TestElement()
        {
        }

        public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            var bounds = new Rectangle(0, 0, Width, Height);
            
            surface.Fill(bounds, Foreground, Background, '\x20');
            
            for (var x = 0; x < Width; x++)
            {
                var reminder = x % 10;
                surface[x, 0].Glyph = '0' + reminder;
            }

            for (var y = 0; y < Height; y++)
            {
                var reminder = y % 10;
                surface[0, y].Glyph = '0' + reminder;
            }
        }
    }

    public class TestGroup : VisualGroup
    {
        public TestGroup()
        {
        }

        public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            var bounds = new Rectangle(0, 0, Width, Height);

            RenderSurface.Fill(bounds, Foreground, Background, '\x20');

            for (var x = 0; x < Width; x++)
            {
                var reminder = x % 10;
                RenderSurface[x, 0].Glyph = '0' + reminder;
            }

            for (var y = 0; y < Height; y++)
            {
                var reminder = y % 10;
                RenderSurface[0, y].Glyph = '0' + reminder;
            }
            
            base.Render(surface, elapsed);
        }
    }
}