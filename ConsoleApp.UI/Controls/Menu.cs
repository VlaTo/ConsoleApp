using ConsoleApp.Bindings;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

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
    
    public abstract class Menu : VisualElement
    {
        public static readonly BindableProperty SelectionBackgroundProperty;
        public static readonly BindableProperty SelectionForegroundProperty;
        public static readonly BindableProperty HintColorProperty;
        public static readonly BindableProperty DisabledColorProperty;
        
        private int selectedIndex;
        private MenuDropDown menuDropDown;

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
        
        public bool IsDropDownOpened
        {
            get => null != menuDropDown;
        }

        protected MenuOrientation Orientation
        {
            get;
        }
        
        public event EventHandler OnMenuCancel;

        public event EventHandler<MenuItemClickEventArgs> OnMenuItemClick;

        protected Menu(MenuOrientation orientation)
        {
            selectedIndex = -1;
            Orientation = orientation;
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

        public override bool HandleKeyPressed(AsciiKey key, ModificatorKeys modificators)
        {
            if (Keys.Escape == key)
            {
                RaiseMenuCancel(EventArgs.Empty);

                return true;
            }

            if (IsPreviousKey(key))
            {
                var index = GetPreviousIndex();

                SelectedIndex = index;
                Invalidate();

                return true;
            }

            if (IsNextKey(key))
            {
                var index = GetNextIndex();

                SelectedIndex = index;
                Invalidate();

                if (IsDropDownOpened && SelectedItem is MenuItem menuItem)
                {
                    ShowMenuDropDown(menuItem);
                }

                return true;
            }

            if (IsDropDownKey(key))
            {
                if (SelectedItem is MenuItem menuItem && 0 < menuItem.Items.Count && false == IsDropDownOpened)
                {
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
                        ShowMenuDropDown(menuItem);

                        return true;
                    }

                    menuItem.Click();
                }

                return true;
            }

            return false;
        }

        internal void NotifyOnClick(MenuItem menuItem)
        {
            if (null == menuItem || false == ReferenceEquals(menuItem.Menu, this))
            {
                return;
            }

            if (IsDropDownOpened)
            {
                DismissMenuDropDown();
            }
            
            RaiseMenuCancel(EventArgs.Empty);
            RaiseOnMenuItemClick(new MenuItemClickEventArgs(menuItem));
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
        
        protected void RaiseMenuCancel(EventArgs e)
        {
            var handler = OnMenuCancel;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }

        protected void RaiseOnMenuItemClick(MenuItemClickEventArgs e)
        {
            var handler = OnMenuItemClick;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }

        protected void DismissMenuDropDown()
        {
            var application = ConsoleApplication.Instance;
            var dialogManager = application.DialogManager;

            menuDropDown.MenuList.OnMenuCancel -= OnMenuDismiss;

            dialogManager.Dismiss(menuDropDown);

            menuDropDown = null;

            Focus();
        }

        protected void ShowMenuDropDown(MenuItem menuItem)
        {
            var application = ConsoleApplication.Instance;
            var dialogManager = application.DialogManager;

            var anchor = new System.Drawing.Rectangle(2, 0, menuItem.TitleLength, 1);

            menuDropDown = MenuDropDown.Create(anchor, menuItem.Items);
            menuDropDown.MenuList.OnMenuCancel += OnMenuDismiss;

            dialogManager.ShowModal(menuDropDown);
        }

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

        private bool IsNextKey(AsciiKey key)
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

        private bool IsPreviousKey(AsciiKey key)
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

        private bool IsDropDownKey(AsciiKey key)
        {
            return MenuOrientation.Horizontal == Orientation && Keys.Down == key;
        }

        private void OnMenuDismiss(object sender, EventArgs e)
        {
            DismissMenuDropDown();
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