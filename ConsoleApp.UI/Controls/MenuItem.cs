using System;
using System.Collections.Generic;
using SadConsole;
using SadRogue.Primitives;

namespace ConsoleApp.UI.Controls
{
    public class MenuItem : BindableObject
    {
        public static readonly BindableProperty TitleProperty;
        public static readonly BindableProperty MenuProperty;
        public static readonly BindableProperty ParentProperty;

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public MenuBar Menu
        {
            get => (MenuBar)GetValue(MenuProperty);
            set => SetValue(MenuProperty, value);
        }

        public MenuItem Parent
        {
            get => (MenuItem)GetValue(ParentProperty);
            set => SetValue(ParentProperty, value);
        }

        public IList<MenuItem> Items
        {
            get;
        }

        public bool IsPopupOpen
        {
            get;
            private set;
        }

        public event EventHandler OnClick;

        public MenuItem()
        {
            Items = new ItemsList<MenuItem>(OnItemsListChanged);
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
            MenuProperty = BindableProperty.Create(
                nameof(Menu),
                typeof(MenuBar),
                typeof(MenuItem),
                defaultValue: null,
                propertyChanged: OnMenuPropertyChanged
            );
            ParentProperty = BindableProperty.Create(
                nameof(Parent),
                typeof(MenuItem),
                typeof(MenuItem),
                defaultValue: null,
                propertyChanged: OnParentPropertyChanged
            );
        }

        public void Render(ICellSurface surface, Rectangle rect, Color background, Color foreground, Color hint)
        {
            var top = rect.Height >> 1;
            var left = rect.X + 2;
            var title = Title;
            var hintIndex = title.IndexOf('~');
            var hasHint = 0 <= hintIndex;

            if (hasHint)
            {
                title = title.Remove(hintIndex, 1);
            }

            surface.Fill(rect, Color.Transparent, background);
            surface.Print(left, top, title, foreground);

            if (hasHint)
            {
                surface[left + hintIndex, top].Foreground = hint;
            }
        }

        public void Click()
        {
            if (0 == Items.Count)
            {
                var handler = OnClick;

                if (null != handler)
                {
                    handler.Invoke(this, EventArgs.Empty);
                }

                return;
            }

            IsPopupOpen = true;
        }

        public void ShowItems()
        {
            if (null == Menu)
            {
                return;
            }

            var bounds = Menu.Bounds;
            
            Popover.Show();
        }

        public static int GetLength(string title)
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
        }

        private string StripTitle()
        {
            var title = Title;
            var index = title.IndexOf('~');

            if (0 > index)
            {
                return title;
            }

            return title.Remove(index, 1);
        }

        protected virtual void OnMenuChanged()
        {
            for (var index = 0; index < Items.Count; index++)
            {
                Items[index].Menu = Menu;
            }
        }

        protected virtual void OnParentChanged()
        {
            ;
        }

        private void OnItemsListChanged(ItemsList<MenuItem>.ItemsListChangedEventArgs args)
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

        private static void OnMenuPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuItem)sender).OnMenuChanged();
        }

        private static void OnParentPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuItem)sender).OnParentChanged();
        }
    }
}