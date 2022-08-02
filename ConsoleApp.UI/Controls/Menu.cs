using ConsoleApp.Bindings;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using ConsoleApp.UI.Extensions;
using Point = System.Drawing.Point;

namespace ConsoleApp.UI.Controls
{
    public enum MenuOrientation
    {
        Horizontal,
        Vertical
    }

    public sealed class MenuItemClickEventArgs : EventArgs
    {
        public MenuItem MenuItem
        {
            get;
        }

        public MenuItemClickEventArgs(MenuItem menuItem)
        {
            MenuItem = menuItem;
        }
    }

    internal enum MenuDismissReason
    {
        ItemClick,
        UserCancel,
        MoveNext,
        MovePrevious
    }
    
    public abstract class Menu : VisualElement
    {
        public static readonly BindableProperty SelectionBackgroundProperty;
        public static readonly BindableProperty SelectionForegroundProperty;
        public static readonly BindableProperty HintColorProperty;
        public static readonly BindableProperty DisabledColorProperty;
        
        private int selectedIndex;
        private MenuFlyout menuFlyout;

        public Color SelectionBackground
        {
            get => (Color)GetValue(SelectionBackgroundProperty);
            set => SetValue(SelectionBackgroundProperty, value);
        }

        public Color SelectionForeground
        {
            get => (Color)GetValue(SelectionForegroundProperty);
            set => SetValue(SelectionForegroundProperty, value);
        }

        public Color HintColor
        {
            get => (Color)GetValue(HintColorProperty);
            set => SetValue(HintColorProperty, value);
        }

        public Color DisabledColor
        {
            get => (Color)GetValue(DisabledColorProperty);
            set => SetValue(DisabledColorProperty, value);
        }

        public IList<MenuElement> Items
        {
            get;
        }

        public MenuElement SelectedItem
        {
            get
            {
                if (0 > selectedIndex)
                {
                    return null;
                }

                return Items[selectedIndex];
            }
        }
        
        public int SelectedIndex
        {
            get => selectedIndex;
            protected set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    Invalidate();
                }
            }
        }
        
        public bool IsDropDownOpened => null != menuFlyout;

        protected bool OpenDropDown
        {
            get;
            set;
        }

        protected MenuOrientation Orientation
        {
            get;
        }

        protected Menu ParentMenu
        {
            get;
            set;
        }

        protected virtual Point MenuDropDownOffset => Point.Empty;

        public event EventHandler<MenuItemClickEventArgs> MenuItemClicked;

        protected Menu(MenuOrientation orientation)
        {
            selectedIndex = -1;
            OpenDropDown = false;
            Orientation = orientation;
            ParentMenu = null;
            Items = new ItemsList<MenuElement>(OnItemsListChanged);
        }

        static Menu()
        {
            SelectionBackgroundProperty = BindableProperty.Create(
                nameof(SelectionBackground),
                typeof(Color),
                typeof(Menu),
                Color.Black,
                OnSelectionBackgroundPropertyChanged
            );
            SelectionForegroundProperty = BindableProperty.Create(
                nameof(SelectionForeground),
                typeof(Color),
                typeof(Menu),
                Color.White,
                OnSelectionForegroundPropertyChanged
            );
            HintColorProperty = BindableProperty.Create(
                nameof(HintColor),
                typeof(Color),
                typeof(Menu),
                Color.Yellow,
                OnHintColorPropertyChanged
            );
            DisabledColorProperty = BindableProperty.Create(
                nameof(DisabledColor),
                typeof(Color),
                typeof(Menu),
                defaultValue: Color.DarkGray,
                propertyChanged: OnDisabledColorPropertyChanged
            );
        }

        public override bool HandleKeyPressed(Keys key, ShiftKeys shiftKeys)
        {
            if (Keys.Escape == key)
            {
                OpenDropDown = false;
                
                if (null != ParentMenu)
                {
                    ParentMenu.DismissMenuDropDown(MenuDismissReason.UserCancel);
                }
                else
                {
                    CancelMenu();
                }

                return true;
            }

            if (IsPreviousKey(key))
            {
                SelectPreviousItem();

                return true;
            }

            if (IsNextKey(key))
            {
                SelectNextItem();

                return true;
            }

            if (IsDropDownKey(key))
            {
                if (SelectedItem is MenuItem menuItem && 0 < menuItem.Items.Count && false == IsDropDownOpened)
                {
                    OpenDropDown = true;
                    ShowMenuDropDown(menuItem);

                    return true;
                }

                return false;
            }

            if (Keys.Enter == key)
            {
                if (SelectedItem is MenuItem menuItem)
                {
                    if (0 < menuItem.Items.Count && false == IsDropDownOpened)
                    {
                        OpenDropDown = true;
                        ShowMenuDropDown(menuItem);

                        return true;
                    }

                    menuItem.Click();
                }

                return true;
            }
            
            var asciiKey = AsciiKey.Get(key, shiftKeys.HasShift(), KeyboardState.Empty);

            if (Char.IsLetterOrDigit(asciiKey.Character) && 0 == shiftKeys)
            {
                for (var index = 0; index < Items.Count; index++)
                {
                    var menuElement = Items[index];

                    if (false == (menuElement.IsVisible && menuElement.IsEnabled))
                    {
                        continue;
                    }

                    if (menuElement is MenuItem menuItem && menuItem.IsHotKey(asciiKey.Character))
                    {
                        SelectedIndex = index;
                        Invalidate();

                        break;
                    }
                }

                return true;
            }

            return false;
        }

        internal void MenuItemClick(MenuItem menuItem)
        {
            if (null == menuItem || false == ReferenceEquals(menuItem.Menu, this))
            {
                return;
            }

            DismissMenuDropDown(MenuDismissReason.ItemClick);
            RaiseOnMenuItemClick(new MenuItemClickEventArgs(menuItem));
            CancelMenu();
        }

        protected override void RenderMain(ICellSurface surface, TimeSpan elapsed)
        {
            var rectangle = new Rectangle(0, 0, Bounds.Width, Bounds.Height);

            surface.Fill(rectangle, Color.Transparent, Background, Glyphs.Whitespace);

            if (0 < Items.Count)
            {
                var bounds = new System.Drawing.Rectangle(
                    Padding.Left,
                    Padding.Top,
                    Bounds.Width - Padding.HorizontalThickness,
                    Bounds.Height - Padding.VerticalThickness
                );
                RenderMenuItems(surface, bounds, elapsed);
            }
        }

        protected void SelectPreviousItem()
        {
            var index = GetPreviousIndex();

            SelectedIndex = index;
            Invalidate();

            if (OpenDropDown && SelectedItem is MenuItem menuItem && 0 < menuItem.Items.Count)
            {
                ShowMenuDropDown(menuItem);
            }
        }

        protected void SelectNextItem()
        {
            var index = GetNextIndex();

            SelectedIndex = index;
            Invalidate();

            if (OpenDropDown && SelectedItem is MenuItem menuItem && 0 < menuItem.Items.Count)
            {
                ShowMenuDropDown(menuItem);
            }
        }

        protected virtual void CancelMenu()
        {
            ;
        }

        protected abstract void RenderMenuItems(ICellSurface surface, System.Drawing.Rectangle bounds, TimeSpan elapsed);

        protected virtual void OnItemsListChanged(ItemsList<MenuElement>.ItemsListChangedEventArgs args)
        {
            switch (args.Action)
            {
                case ItemsChangeAction.Add:
                case ItemsChangeAction.Insert:
                {
                    //args.NewItem.Menu = this;
                    args.NewItem.Parent = null;

                    if (0 > SelectedIndex && 0 < Items.Count)
                    {
                        SelectedIndex = 0;
                    }

                    break;
                }

                case ItemsChangeAction.Clear:
                {
                    SelectedIndex = -1;

                    break;
                }

                case ItemsChangeAction.Remove:
                {
                    if (0 > SelectedIndex)
                    {
                        break;
                    }

                    args.OldItem.Menu = null;

                    if (Items.Count == SelectedIndex)
                    {
                        SelectedIndex = Items.Count - 1;
                    }
                    else if (0 == Items.Count)
                    {
                        SelectedIndex = -1;
                    }

                    break;
                }

                case ItemsChangeAction.Replace:
                {
                    //args.NewItem.Menu = this;
                    args.NewItem.Parent = null;
                    args.OldItem.Menu = null;

                    break;
                }
            }
        }

        protected int GetPreviousIndex()
        {
            if (0 == Items.Count)
            {
                return -1;
            }

            var position = selectedIndex;

            while (true)
            {
                position--;

                if (0 > position)
                {
                    position = Items.Count - 1;
                }

                if (position == selectedIndex || Items[position].IsSelectable)
                {
                    return position;
                }
            }
        }
        
        protected int GetNextIndex()
        {
            if (0 == Items.Count)
            {
                return -1;
            }

            var position = selectedIndex;

            while (true)
            {
                position++;

                if (Items.Count <= position)
                {
                    position = 0;
                }

                if (position == selectedIndex || Items[position].IsSelectable)
                {
                    return position;
                }
            }
        }

        protected void RaiseOnMenuItemClick(MenuItemClickEventArgs e)
        {
            var handler = MenuItemClicked;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }

        internal void DismissMenuDropDown(MenuDismissReason reason)
        {
            if (IsDropDownOpened)
            {
                menuFlyout.Dismiss();
                menuFlyout = null;
            }

            if (reason != MenuDismissReason.MoveNext && reason != MenuDismissReason.MovePrevious)
            {
                OpenDropDown = false;
            }
            
            switch (reason)
            {
                case MenuDismissReason.UserCancel:
                {
                    Focus();
                    break;
                }

                case MenuDismissReason.MovePrevious:
                {
                    SelectPreviousItem();
                    break;
                }

                case MenuDismissReason.MoveNext:
                {
                    SelectNextItem();
                    break;
                }

                case MenuDismissReason.ItemClick:
                {
                    /*var menu = ParentMenu;

                    while (null != menu)
                    {
                        menu.DismissMenuDropDown(reason);
                        menu = menu.ParentMenu;
                    }*/

                    if (null != ParentMenu)
                    {
                        ParentMenu.DismissMenuDropDown(reason);
                    }
                    else
                    {
                        CancelMenu();
                    }

                    break;
                }
            }
        }

        protected void ShowMenuDropDown(MenuItem menuItem)
        {
            var anchor = MakeAbsolute(GetMenuItemBounds(menuItem));

            anchor.Offset(MenuDropDownOffset);

            menuFlyout = MenuFlyout.Create(anchor);
            menuFlyout.MenuList.ParentMenu = this;
            for (var index = 0; index < menuItem.Items.Count; index++)
            {
                menuFlyout.MenuList.Items.Add(menuItem.Items[index]);
            }

            menuFlyout.Show();
        }

        protected abstract System.Drawing.Rectangle GetMenuItemBounds(MenuItem menuItem);

        protected virtual void OnDisabledColorChanged()
        {
            ;
        }

        protected virtual void OnHintColorChanged()
        {
            ;
        }

        protected virtual void OnSelectionForegroundChanged()
        {
            ;
        }

        protected virtual void OnSelectionBackgroundChanged()
        {
            ;
        }

        private bool IsNextKey(Keys key)
        {
            switch (Orientation)
            {
                case MenuOrientation.Horizontal:
                {
                    return Keys.Right == key;
                }

                case MenuOrientation.Vertical:
                {
                    return Keys.Down == key;
                }
            }

            return false;
        }

        private bool IsPreviousKey(Keys key)
        {
            switch (Orientation)
            {
                case MenuOrientation.Horizontal:
                {
                    return Keys.Left == key;
                }

                case MenuOrientation.Vertical:
                {
                    return Keys.Up == key;
                }
            }

            return false;
        }

        private bool IsDropDownKey(Keys key)
        {
            return MenuOrientation.Horizontal == Orientation && Keys.Down == key;
        }

        private static void OnHintColorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Menu)sender).OnHintColorChanged();
        }

        private static void OnSelectionForegroundPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Menu)sender).OnSelectionForegroundChanged();
        }

        private static void OnSelectionBackgroundPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Menu)sender).OnSelectionBackgroundChanged();
        }

        private static void OnDisabledColorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((Menu)sender).OnDisabledColorChanged();
        }
    }
}