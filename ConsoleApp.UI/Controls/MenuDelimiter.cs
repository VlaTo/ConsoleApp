using SadConsole;
using SadRogue.Primitives;

namespace ConsoleApp.UI.Controls
{
    public class MenuDelimiter : MenuElement
    {
        public override bool IsSelectable => false;

        public override void Render(ICellSurface surface, Rectangle rect, bool isSelected, bool renderGlyph)
        {
            ;
        }
    }
}