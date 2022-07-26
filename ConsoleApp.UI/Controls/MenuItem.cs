using ConsoleApp.Bindings;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ConsoleApp.UI.Controls
{
    public class MenuItem : MenuElement
    {
        public static readonly BindableProperty CommandProperty;
        public static readonly BindableProperty CommandParameterProperty;
        public static readonly BindableProperty ShortCutProperty;
        public static readonly BindableProperty TitleProperty;

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ShortCut ShortCut
        {
            get => (ShortCut)GetValue(ShortCutProperty);
            set => SetValue(ShortCutProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public IList<MenuElement> Items
        {
            get;
        }

        public override bool IsSelectable => IsVisible && Enabled();

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

        public int Width
        {
            get
            {
                var length = TitleLength;

                if (null != ShortCut)
                {
                    var hint = ShortCut.Hint;
                    length += (2 + hint.Length);
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
            CommandProperty = BindableProperty.Create(
                nameof(Command),
                typeof(ICommand),
                ownerType: typeof(MenuItem),
                defaultValue: null
            );
            CommandParameterProperty = BindableProperty.Create(
                nameof(CommandParameter),
                typeof(object),
                ownerType: typeof(MenuItem),
                defaultValue: null
            );
            ShortCutProperty = BindableProperty.Create(
                nameof(ShortCut),
                typeof(ShortCut),
                ownerType: typeof(MenuItem),
                defaultValue: null,
                propertyChanged: OnShortCutPropertyChanged
            );
            TitleProperty = BindableProperty.Create(
                nameof(Title),
                typeof(string),
                ownerType: typeof(MenuItem),
                defaultValue: null,
                propertyChanged: OnTitlePropertyChanged
            );
        }

        public override void Render(ICellSurface surface, Rectangle rectangle, bool isSelected, bool renderGlyph)
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

            if (renderGlyph && 0 < Items.Count)
            {
                left = rectangle.Width - 3;
                surface.SetGlyph(left, rectangle.Y, Glyphs.DirectionRight, foreground);
            }
            else if (null != ShortCut)
            {
                var shortcut = ShortCut.Hint;
                left = rectangle.Width - (shortcut.Length + 2);
                surface.Print(left, rectangle.Y, shortcut, foreground);
            }
        }

        public void Click()
        {
            if (0 < Items.Count)
            {
                return;
            }

            var command = Command;

            if (null != command)
            {
                var parameter = CommandParameter;

                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }

            RaiseOnClick();

            if (null != Menu)
            {
                Menu.MenuItemClick(this);
            }
        }

        protected override void OnMenuChanged()
        {
            for (var index = 0; index < Items.Count; index++)
            {
                Items[index].Menu = Menu;
            }
        }

        protected Color GetForegroundColor(bool isSelected)
        {
            if (Enabled())
            {
                return isSelected ? Menu.SelectionForeground : Menu.Foreground;
            }

            return Menu.DisabledColor;
        }

        private bool Enabled()
        {
            if (null == Command)
            {
                return IsEnabled;
            }

            var parameter = CommandParameter;

            return IsEnabled && Command.CanExecute(parameter);
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

        private void OnShortCutChanged()
        {
            if (null != Menu)
            {
                Menu.Invalidate();
            }
        }

        private void OnTitleChanged()
        {
            if (null != Menu)
            {
                Menu.Invalidate();
            }
        }

        private static void OnShortCutPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuItem)sender).OnShortCutChanged();
        }

        private static void OnTitlePropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuItem)sender).OnTitleChanged();
        }
    }
}