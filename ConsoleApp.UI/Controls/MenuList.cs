using System;
using System.Drawing;
using ConsoleApp.UI.Extensions;
using SadConsole;
using SadConsole.Input;

namespace ConsoleApp.UI.Controls
{
    public class MenuList : Menu
    {
        protected override Point MenuDropDownOffset => new Point(2, 0);

        public MenuList()
            : base(MenuOrientation.Vertical)
        {
        }

        public override Size Measure(int widthConstraint, int heightConstraint)
        {
            if (false == IsMeasureValid)
            {
                var width = 0;
                var count = 0;

                for (var index = 0; index < Items.Count; index++)
                {
                    var item = Items[index];

                    if (false == item.IsVisible)
                    {
                        continue;
                    }

                    if (item is MenuItem menuItem)
                    {
                        var length = menuItem.Width + 6;
                        width = Math.Max(width, length);
                    }

                    count++;
                }

                var height = Math.Min(count, heightConstraint);

                DesiredSize = new Size(Math.Min(width, widthConstraint), height);
                IsMeasureValid = true;
            }

            return DesiredSize;
        }

        public override bool HandleKeyPressed(AsciiKey key, ModificatorKeys modificators)
        {
            var handled = base.HandleKeyPressed(key, modificators);

            if (Keys.Right == key && false == handled)
            {
                if (null != ParentMenu)
                {
                    ParentMenu.DismissMenuDropDown(MenuDismissReason.MoveNext);
                }

                return true;
            }

            if (Keys.Left == key && false == handled)
            {
                if (null != ParentMenu)
                {
                    ParentMenu.DismissMenuDropDown(MenuDismissReason.MovePrevious);
                }

                return true;
            }

            return handled;
        }

        protected override void RenderMenuItems(ICellSurface surface, Rectangle bounds, TimeSpan elapsed)
        {
            var rectangle = bounds.ToRectangle();
            var y = rectangle.Y;

            for (var index = 0; index < Items.Count; index++)
            {
                var menuElement = Items[index];

                if (false == menuElement.IsVisible)
                {
                    continue;
                }

                if (menuElement is MenuItem menuItem)
                {
                    var isSelected = (IsFocused || IsDropDownOpened) && ReferenceEquals(SelectedItem, menuItem);
                    var rect = new SadRogue.Primitives.Rectangle(rectangle.X, y, rectangle.Width, 1);

                    Items[index].Render(surface, rect, isSelected, true);
                }

                y += 1;
            }
        }

        protected override Rectangle GetMenuItemBounds(MenuItem menuItem)
        {
            var origin = new Point(Padding.Left, Padding.Top);
            var top = Padding.Top;
            var width = 0;

            for (var index = 0; index < Items.Count; index++)
            {
                var menuElement = Items[index];

                if (false == menuElement.IsVisible)
                {
                    continue;
                }

                if (menuElement is MenuItem mi)
                {
                    width = Math.Max(mi.Width + 6, width);

                    if (ReferenceEquals(menuElement, menuItem))
                    {
                        top = origin.Y;
                    }
                }

                origin.Y += 1;
            }

            return new Rectangle(new Point(Padding.Left, top), new Size(width, 1));
        }
    }
}