using SadConsole;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using Color = SadRogue.Primitives.Color;
using Keys = SadConsole.Input.Keys;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace ConsoleApp.UI.Controls
{
    public class MenuBar : Menu
    {
        //public static readonly BindableProperty SelectionBackgroundProperty;
        //public static readonly BindableProperty SelectionForegroundProperty;
        //public static readonly BindableProperty HintColorProperty;
        //public static readonly BindableProperty DisabledColorProperty;
        
        //private int selectedIndex;

        /*public Color SelectionBackground
        {
            get => (Color)GetValue(SelectionBackgroundProperty);
            set => SetValue(SelectionBackgroundProperty, value);
        }*/

        /*public Color SelectionForeground
        {
            get => (Color)GetValue(SelectionForegroundProperty);
            set => SetValue(SelectionForegroundProperty, value);
        }*/

        /*public Color HintColor
        {
            get => (Color)GetValue(HintColorProperty);
            set => SetValue(HintColorProperty, value);
        }*/

        /*public Color DisabledColor
        {
            get => (Color)GetValue(DisabledColorProperty);
            set => SetValue(DisabledColorProperty, value);
        }*/

        /*public MenuItem SelectedItem
        {
            get
            {
                if (0 > selectedIndex)
                {
                    return null;
                }

                return (MenuItem)Items[selectedIndex];
            }
        }*/

        /*public int SelectedIndex
        {
            get => selectedIndex;
            private set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    Invalidate();
                }
            }
        }*/

        /*public bool IsPopupOpened
        {
            get;
            private set;
        }*/

        /*public IList<MenuElement> Items
        {
            get;
        }*/

        //public event EventHandler OnMenuCancel;

        public MenuBar()
            : base(MenuOrientation.Horizontal)
        {
            //Items = new ItemsList<MenuElement>(OnItemsListChanged);
            //selectedIndex = -1;
        }

        /*static MenuBar()
        {
            SelectionBackgroundProperty = BindableProperty.Create(
                nameof(SelectionBackground),
                typeof(Color),
                typeof(MenuBar),
                Color.Black,
                OnSelectionBackgroundPropertyChanged
            );
            SelectionForegroundProperty = BindableProperty.Create(
                nameof(SelectionForeground),
                typeof(Color),
                typeof(MenuBar),
                Color.White,
                OnSelectionForegroundPropertyChanged
            );
            HintColorProperty = BindableProperty.Create(
                nameof(HintColor),
                typeof(Color),
                typeof(MenuBar),
                Color.Yellow,
                OnHintColorPropertyChanged
            );
            DisabledColorProperty = BindableProperty.Create(
                nameof(DisabledColor),
                typeof(Color),
                typeof(MenuBar),
                defaultValue: Color.DarkGray,
                propertyChanged: OnDisabledColorPropertyChanged
            );
        }*/

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

        /*
        public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            var rectangle = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
            surface.Fill(rectangle, Color.Transparent, Background, Glyphs.Whitespace);

            var left = Padding.Left + 2;
            var top = Bounds.Y + Padding.Top;
            var height = Bounds.Height - Padding.VerticalThickness;

            foreach (var item in Items)
            {
                var rect = Rectangle.Empty;
                var background = Background;
                var foreground = Foreground;

                if (item is MenuItem menuItem)
                {
                    var length = MenuItem.GetLength(menuItem.Title);
                    var isSelected = (IsFocused || IsPopupOpened) && ReferenceEquals(SelectedItem, item);
                    
                    background = isSelected ? SelectionBackground : Background;
                    foreground = GetItemForegroundColor(menuItem, isSelected);
                    rect = new Rectangle(left, top, length + 4, height);
                    
                    left += (length + 4);
                }
                else if (item is MenuDelimiter)
                {
                    background = Background;
                    foreground = Color.Transparent;
                    rect = new Rectangle(left, top, 2, height);

                    left += 2;
                }

                /*var length = MenuItem.GetLength(item.Title);
                var rect = new Rectangle(left, top, length + 4, height);
                var isSelected = (IsFocused || IsPopupOpened) && ReferenceEquals(SelectedItem, item);
                var background = isSelected ? SelectionBackground : Background;
                var foreground = GetItemForegroundColor(item, isSelected);#1#

                item.Render(surface, rect, background, foreground, HintColor);

                //left += (length + 4);
            }

            base.Render(surface, elapsed);
        }
        */

        protected override void RenderMenuItems(ICellSurface surface, System.Drawing.Rectangle bounds, TimeSpan elapsed)
        {
            var left = bounds.X + 2;
            var top = bounds.Y;

            foreach (var item in Items)
            {
                var rect = Rectangle.Empty;
                var isSelected = (IsFocused || IsPopupOpened) && ReferenceEquals(SelectedItem, item);

                if (item is MenuItem menuItem)
                {
                    var length = menuItem.TitleLength;

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

        /*
        public override bool HandleKeyPressed(AsciiKey key, ModificatorKeys modificators)
        {
            if (Keys.Escape == key)
            {
                RaiseMenuCancel(EventArgs.Empty);

                return true;
            }

            if (Keys.Left == key)
            {
                var index = GetPreviousIndex();

                SelectedIndex = index;
                Invalidate();

                return true;
            }

            if (Keys.Right == key)
            {
                var index = GetNextIndex();

                SelectedIndex = index;
                Invalidate();

                if (IsPopupOpened)
                {
                    SelectedItem.ShowItems();
                }

                return true;
            }

            if (Keys.Down == key)
            {
                var selectedItem = SelectedItem;

                if (null != selectedItem)
                {
                    selectedItem.ShowItems();
                    IsPopupOpened = true;

                    return true;
                }
            }

            if (Keys.Enter == key)
            {
                var selectedItem = SelectedItem;

                if (selectedItem is { IsEnabled: true, IsVisible: true })
                {
                    selectedItem.Click();
                    return true;
                }
            }

            return false;
        }
        */

        protected override void OnItemsListChanged(ItemsList<MenuElement>.ItemsListChangedEventArgs args)
        {
            base.OnItemsListChanged(args);

            switch (args.Action)
            {
                case ItemsChangeAction.Add:
                case ItemsChangeAction.Insert:
                {
                    args.NewItem.Menu = this;
                    /*args.NewItem.Parent = null;

                    if (0 > SelectedIndex && 0 < Items.Count)
                    {
                        SelectedIndex = 0;
                    }*/

                    break;
                }

                /*case ItemsChangeAction.Clear:
                {
                    SelectedIndex = -1;

                    break;
                }*/

                /*case ItemsChangeAction.Remove:
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
                }*/

                case ItemsChangeAction.Replace:
                {
                    args.NewItem.Menu = this;
                    //args.NewItem.Parent = null;
                    //args.OldItem.Menu = null;

                    break;
                }
            }
        }

        /*private Color GetItemForegroundColor(MenuItem menuItem, bool isSelected)
        {
            if (menuItem.IsEnabled)
            {
                return isSelected ? SelectionForeground : Foreground;
            }

            return DisabledColor;
        }*/

        /*private int GetPreviousIndex()
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
        }*/

        /*
        private int GetNextIndex()
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
        */

        /*private void RaiseMenuCancel(EventArgs e)
        {
            var handler = OnMenuCancel;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }*/
    }
}