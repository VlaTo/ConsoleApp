using SadConsole;
using SadRogue.Primitives;

namespace ConsoleApp.UI.Controls
{
    public class MenuDelimiter : MenuElement
    {
        public override bool IsSelectable => false;

        /*public override System.Drawing.Rectangle GetBounds()
        {
            return System.Drawing.Rectangle.Empty;
        }*/

        public override void Render(ICellSurface surface, Rectangle rect, bool isSelected)
        {
            /*surface.Fill(rect, foreground, background, glyph: '\xC4');
            surface[rect.Y * surface.Width + rect.X].Glyph = '\xC7';
            surface[rect.Y * surface.Width + rect.MaxExtentX].Glyph = '\xB6';*/
        }
    }
}