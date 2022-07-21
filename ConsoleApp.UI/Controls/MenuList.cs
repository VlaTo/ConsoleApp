using System;
using System.Drawing;
using ConsoleApp.UI.Extensions;
using SadConsole;

namespace ConsoleApp.UI.Controls
{
    public class MenuList : Menu
    {
        public MenuList()
            : base(MenuOrientation.Vertical)
        {
        }

        public override Size Measure(int widthConstraint, int heightConstraint)
        {
            if (false == IsMeasureValid)
            {
                var width = 0;
                var count = null == Items ? 0 : Items.Count;

                for (var index = 0; index < count; index++)
                {
                    if (Items[index] is MenuItem menuItem)
                    {
                        var length = menuItem.TitleLength;
                        width = Math.Max(width, length + 6);
                    }
                }

                var height = Math.Min(count, heightConstraint);

                DesiredSize = new Size(Math.Min(width, widthConstraint), height);
                IsMeasureValid = true;
            }

            return DesiredSize;

            //return base.Measure(widthConstraint, heightConstraint);
        }

        protected override void RenderMenuItems(ICellSurface surface, Rectangle bounds, TimeSpan elapsed)
        {
            var rectangle = bounds.ToRectangle();
            var y = rectangle.Y;

            for (var index = 0; index < Items.Count; index++)
            {
                if (Items[index] is MenuItem menuItem)
                {
                    var isSelected = (IsFocused || IsPopupOpened) && ReferenceEquals(SelectedItem, menuItem);
                    var rect = new SadRogue.Primitives.Rectangle(rectangle.X, y, rectangle.Width, 1);

                    Items[index].Render(surface, rect, isSelected);
                }

                y += 1;
            }
        }
    }
}