using SadConsole;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using Color = SadRogue.Primitives.Color;
using Keys = SadConsole.Input.Keys;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace ConsoleApp.UI.Controls
{
    public class MenuBar : VisualElement
    {
        public static readonly BindableProperty SelectionBackgroundProperty;
        public static readonly BindableProperty SelectionForegroundProperty;
        public static readonly BindableProperty HintColorProperty;
        
        private int selectedIndex;

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

        public MenuItem SelectedItem
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
            private set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    Invalidate();
                }
            }
        }

        public bool IsPopupOpened
        {
            get;
            private set;
        }

        public IList<MenuItem> Items
        {
            get;
        }

        public event EventHandler OnMenuCancel;

        public MenuBar()
        {
            Items = new ItemsList<MenuItem>(OnItemsListChanged);
            selectedIndex = -1;
        }

        static MenuBar()
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
        }

        public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            var rectangle = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
            surface.Fill(rectangle, Color.Transparent, Background, Glyphs.Whitespace);

            var left = Padding.Left + 2;

            foreach (var item in Items)
            {
                var length = MenuItem.GetLength(item.Title);
                var rect = new Rectangle(left, Bounds.Y, length + 4, Bounds.Height);
                var isSelected = IsFocused && ReferenceEquals(SelectedItem, item);
                var background = isSelected ? SelectionBackground : Background;
                var foreground = isSelected ? SelectionForeground : Foreground;

                item.Render(surface, rect, background, foreground, HintColor);

                left += (length + 4);
            }

            base.Render(surface, elapsed);
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

                if (null != selectedItem)
                {
                    selectedItem.Click();
                    return true;
                }
            }

            return false;
        }

        private int GetPreviousIndex()
        {
            if (0 == Items.Count)
            {
                return -1;
            }

            var index = selectedIndex - 1;

            if (0 > index)
            {
                index = Items.Count - 1;
            }

            return index;
        }

        private int GetNextIndex()
        {
            if (0 == Items.Count)
            {
                return -1;
            }

            var index = selectedIndex + 1;

            if (Items.Count <= index)
            {
                index = 0;
            }

            return index;
        }

        private void OnItemsListChanged(ItemsList<MenuItem>.ItemsListChangedEventArgs args)
        {
            switch (args.Action)
            {
                case ItemsChangeAction.Add:
                case ItemsChangeAction.Insert:
                {
                    args.NewItem.Menu = this;

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
                    args.NewItem.Menu = this;
                    args.OldItem.Menu = null;

                    break;
                }
            }
        }

        private void RaiseMenuCancel(EventArgs e)
        {
            var handler = OnMenuCancel;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }

        private static void OnHintColorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuBar)sender).Invalidate();
        }

        private static void OnSelectionForegroundPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuBar)sender).Invalidate();
        }

        private static void OnSelectionBackgroundPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuBar)sender).Invalidate();
        }
    }
}