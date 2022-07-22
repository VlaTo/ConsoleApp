using ConsoleApp.Bindings;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace ConsoleApp.UI.Controls
{
    public class MenuItem : MenuElement
    {
        public static readonly BindableProperty TitleProperty;
        //public static readonly BindableProperty MenuProperty;
        //public static readonly BindableProperty ParentProperty;
        //public static readonly BindableProperty IsEnabledProperty;
        public static readonly BindableProperty IsVisibleProperty;

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /*public MenuBar Menu
        {
            get => (MenuBar)GetValue(MenuProperty);
            set => SetValue(MenuProperty, value);
        }*/

        /*public MenuItem Parent
        {
            get => (MenuItem)GetValue(ParentProperty);
            set => SetValue(ParentProperty, value);
        }*/

        /*public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }*/

        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        public IList<MenuElement> Items
        {
            get;
        }

        /*public bool IsPopupOpen
        {
            get;
            private set;
        }*/

        /*public System.Drawing.Rectangle Bounds
        {
            get
            {

            }
        }*/

        public override bool IsSelectable => IsVisible && IsEnabled;

        public int TitleLength
        {
            get
            {
                var title = Title;

                if (String.IsNullOrEmpty(title))
                {
                    return 0;
                }

                var length = title.Length;

                if (title.Contains('~'))
                {
                    length--;
                }

                return length;
            }
        }

        public event EventHandler OnClick;

        public MenuItem()
        {
            Items = new ItemsList<MenuElement>(OnItemsListChanged);
        }

        static MenuItem()
        {
            TitleProperty = BindableProperty.Create(
                nameof(Title),
                typeof(string),
                typeof(MenuItem),
                string.Empty,
                OnTitlePropertyChanged
            );
            /*MenuProperty = BindableProperty.Create(
                nameof(Menu),
                typeof(MenuBar),
                typeof(MenuItem),
                defaultValue: null,
                propertyChanged: OnMenuPropertyChanged
            );*/
            /*ParentProperty = BindableProperty.Create(
                nameof(Parent),
                typeof(MenuItem),
                typeof(MenuItem),
                defaultValue: null,
                propertyChanged: OnParentPropertyChanged
            );*/
            /*IsEnabledProperty = BindableProperty.Create(
                nameof(IsEnabled),
                typeof(bool),
                typeof(MenuItem),
                defaultValue: true,
                propertyChanged: OnIsEnabledPropertyChanged
            );*/
            IsVisibleProperty = BindableProperty.Create(
                nameof(IsVisible),
                typeof(bool),
                typeof(MenuItem),
                defaultValue: true,
                propertyChanged: OnIsVisiblePropertyChanged
            );
        }

        public override void Render(ICellSurface surface, Rectangle rectangle, bool isSelected /*, Color background, Color foreground, Color hint*/)
        {
            var left = rectangle.X + 2;
            var title = Title;
            var hintIndex = title.IndexOf('~');
            var hasHint = 0 <= hintIndex;

            if (hasHint)
            {
                title = title.Remove(hintIndex, 1);
            }

            var foreground = GetForegroundColor(isSelected);
            var background = isSelected ? Menu.SelectionBackground : Menu.Background;

            surface.Fill(rectangle, Color.Transparent, background);
            surface.Print(left, rectangle.Y, title, foreground);

            if (hasHint)
            {
                surface.SetForeground(left + hintIndex, rectangle.Y, Menu.HintColor);
                //[left + hintIndex, rectangle.Y].Foreground = Menu.HintColor;
            }
        }

        public void Click()
        {
            if (0 == Items.Count)
            {
                RaiseOnClick();

                if (null != Menu)
                {
                    Menu.NotifyOnClick(this);
                }

                return;
            }

            //IsPopupOpen = true;
        }

        private void RaiseOnClick()
        {
            var handler = OnClick;

            if (null != handler)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        public void ShowItems()
        {
            if (null == Menu)
            {
                return;
            }

            /*var origin= Menu.GetAbsolutePosition();
            var rect = Menu.GetItemBounds(this);

            rect.X += origin.X;
            rect.Y += origin.Y;*/
            var anchor = new System.Drawing.Rectangle(2, 0, TitleLength, 1);
            MenuDropDown.Show(anchor, Items);
        }

        /*public override System.Drawing.Rectangle GetBounds()
        {
            if (null != Menu && null == Parent)
            {
                return Menu.GetItemBounds(this);
            }

            return Parent.GetItemBounds(this);
        }*/

        /*public static int GetLength(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return 0;
            }

            var length = title.Length;

            if (title.Contains('~'))
            {
                length--;
            }

            return length;
        }*/

        protected override void OnMenuChanged()
        {
            for (var index = 0; index < Items.Count; index++)
            {
                Items[index].Menu = Menu;
            }
        }

        protected virtual void OnIsVisibleChanged()
        {
            ;
        }

        private System.Drawing.Rectangle GetItemBounds(MenuItem menuItem)
        {
            for (var index = 0; index < Items.Count; index++)
            {
                
            }

            return System.Drawing.Rectangle.Empty;
        }

        private void OnItemsListChanged(ItemsList<MenuElement>.ItemsListChangedEventArgs args)
        {
            switch (args.Action)
            {
                case ItemsChangeAction.Add:
                case ItemsChangeAction.Insert:
                {
                    args.NewItem.Menu = Menu;
                    args.NewItem.Parent = this;

                    break;
                }

                case ItemsChangeAction.Remove:
                {
                    args.OldItem.Menu = null;
                    args.OldItem.Parent = null;

                    break;
                }

                case ItemsChangeAction.Replace:
                {
                    args.NewItem.Menu = Menu;
                    args.NewItem.Parent = this;
                    args.OldItem.Menu = null;
                    args.OldItem.Parent = null;

                    break;
                }
            }
        }

        private static void OnTitlePropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ;
        }

        /*private static void OnIsEnabledPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuItem)sender).OnIsEnabledChanged();
        }*/

        private static void OnIsVisiblePropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuItem)sender).OnIsVisibleChanged();
        }
    }
}