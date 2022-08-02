using SadConsole;
using System;
using System.Collections.Generic;
using System.Drawing;
using Keys = SadConsole.Input.Keys;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace ConsoleApp.UI.Controls
{
    public class MenuBar : Menu
    {
        private VisualElement backgroundElement;

        public MenuBar()
            : base(MenuOrientation.Horizontal)
        {
            backgroundElement = null;
        }

        protected override void CancelMenu()
        {
            if (null != backgroundElement)
            {
                backgroundElement.Focus();
                backgroundElement = null;
            }

            base.CancelMenu();
        }

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
                    var length = menuItem.Width;

                    isSelected = (IsFocused || OpenDropDown) && ReferenceEquals(SelectedItem, item);
                    rect = new Rectangle(left, top, length + 4, bounds.Height);
                    left += rect.Width;
                }
                else if (item is MenuDelimiter)
                {
                    rect = new Rectangle(left, top, 2, bounds.Height);
                    left += 2;
                }

                item.Render(surface, rect, isSelected, false);
            }
        }

        protected override System.Drawing.Rectangle GetMenuItemBounds(MenuItem menuItem)
        {
            var origin = new Point(Padding.Left + 2, Padding.Top);
            var height = Bounds.Height - Padding.VerticalThickness;
            
            foreach (var item in Items)
            {
                if (false == item.IsVisible)
                {
                    continue;
                }

                if (item is MenuItem mi)
                {
                    var width = mi.Width + 4;

                    if (ReferenceEquals(item, menuItem))
                    {
                        return new System.Drawing.Rectangle(origin, new Size(width, height));
                    }

                    origin.X += width;
                }
                else if (item is MenuDelimiter)
                {
                    origin.X += 2;
                }
            }

            return System.Drawing.Rectangle.Empty;
        }

        public override bool HandleKeyDown(Keys key, ShiftKeys shiftKeys)
        {
            if (Keys.F9 == key && 0 == shiftKeys && false == IsFocused)
            {
                var element = Parent;

                while (null != element)
                {
                    if (null == element.Parent)
                    {
                        if (element.FocusedElement is Layout layout)
                        {
                            backgroundElement = layout.FocusedElement;
                        }

                        break;
                    }

                    element = element.Parent;
                }

                Focus();

                return true;
            }

            var shortCut = new ShortCut(key, shiftKeys);

            return HandleShortCut(Items, shortCut);
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

        private static bool HandleShortCut(IList<MenuElement> menuElements, ShortCut shortCut)
        {
            for (var index = 0; index < menuElements.Count; index++)
            {
                var menuElement = menuElements[index];

                if (false == (menuElement.IsEnabled && menuElement.IsVisible))
                {
                    continue;
                }

                if (menuElement is MenuItem menuItem)
                {
                    if (null != menuItem.ShortCut && menuItem.ShortCut.Equals(shortCut))
                    {
                        menuItem.Click();
                        return true;
                    }

                    if (HandleShortCut(menuItem.Items, shortCut))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}