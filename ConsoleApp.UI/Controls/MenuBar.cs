using SadConsole;
using SadConsole.Input;
using System;
using Keys = SadConsole.Input.Keys;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace ConsoleApp.UI.Controls
{
    public class MenuBar : Menu
    {
        public MenuBar()
            : base(MenuOrientation.Horizontal)
        {
        }

        /*public System.Drawing.Rectangle GetItemBounds(MenuElement menuElement)
        {
            var origin = new Point(Padding.Left + 2, Padding.Top);
            var height = Bounds.Height - Padding.VerticalThickness;

            foreach (var item in Items)
            {
                if (item is MenuItem menuItem)
                {
                    var length = MenuItem.GetLength(menuItem.Title) + 4;

                    if (ReferenceEquals(item, menuItem))
                    {
                        return new System.Drawing.Rectangle(origin, new Size(length, height));
                    }

                    origin.X += length;
                }
                else if (item is MenuDelimiter)
                {
                    origin.X += 2;
                }
            }

            return System.Drawing.Rectangle.Empty;
        }*/


        protected override void RenderMenuItems(ICellSurface surface, System.Drawing.Rectangle bounds, TimeSpan elapsed)
        {
            var left = bounds.X + 2;
            var top = bounds.Y;

            foreach (var item in Items)
            {
                var rect = Rectangle.Empty;

                if (false == item.IsVisible)
                {
                    continue;
                }

                var isSelected = false;

                if (item is MenuItem menuItem)
                {
                    var length = menuItem.TitleLength;

                    isSelected = (IsFocused || IsDropDownOpened) && ReferenceEquals(SelectedItem, item);
                    rect = new Rectangle(left, top, length + 4, bounds.Height);
                    left += (length + 4);
                }
                else if (item is MenuDelimiter)
                {
                    rect = new Rectangle(left, top, 2, bounds.Height);
                    left += 2;
                }

                item.Render(surface, rect, isSelected);
            }
        }

        public override bool HandleKeyDown(AsciiKey key, ModificatorKeys modificators)
        {
            if (Keys.F9 == key && modificators.IsEmpty && false == IsFocused)
            {
                Focus();

                return true;
            }

            return false;
        }

        protected override void OnItemsListChanged(ItemsList<MenuElement>.ItemsListChangedEventArgs args)
        {
            base.OnItemsListChanged(args);

            switch (args.Action)
            {
                case ItemsChangeAction.Add:
                case ItemsChangeAction.Insert:
                {
                    args.NewItem.Menu = this;

                    break;
                }

                case ItemsChangeAction.Replace:
                {
                    args.NewItem.Menu = this;

                    break;
                }
            }
        }
    }
}