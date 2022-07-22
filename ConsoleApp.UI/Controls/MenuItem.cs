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

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }



        public IList<MenuElement> Items
        {
            get;
        }

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
        }

        public override void Render(ICellSurface surface, Rectangle rectangle, bool isSelected)
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
            }
        }

        public void Click()
        {
            if (0 < Items.Count)
            {
                return;
            }

            if (null != Menu)
            {
                Menu.NotifyOnClick(this);
            }

            RaiseOnClick();
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

        private System.Drawing.Rectangle GetItemBounds(MenuItem menuItem)
        {
            for (var index = 0; index < Items.Count; index++)
            {
                
            }

            return System.Drawing.Rectangle.Empty;
        }

        private void RaiseOnClick()
        {
            var handler = OnClick;

            if (null != handler)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
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
    }
}