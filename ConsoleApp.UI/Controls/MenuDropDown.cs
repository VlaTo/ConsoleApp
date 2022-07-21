using System.Collections.Generic;
using System.Drawing;
using Color = SadRogue.Primitives.Color;

namespace ConsoleApp.UI.Controls
{
    public sealed class MenuDropDown : Window
    {
        // public static readonly BindableProperty SelectionBackgroundProperty;
        // public static readonly BindableProperty SelectionForegroundProperty;
        // public static readonly BindableProperty HintColorProperty;
        // public static readonly BindableProperty DisabledColorProperty;
        // public static readonly BindableProperty ItemsProperty;
        // public static readonly BindableProperty SelectedItemProperty;
        // public static readonly BindableProperty SelectedIndexProperty;
        /*public Color SelectionBackground
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
            get => (IList<MenuElement>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public MenuItem SelectedItem
        {
            get => (MenuItem)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }*/

        /*static MenuDropDown()
        {
            SelectionBackgroundProperty = BindableProperty.Create(
                nameof(SelectionBackground),
                typeof(Color),
                typeof(MenuDropDown),
                Color.Black,
                OnSelectionBackgroundPropertyChanged
            );
            SelectionForegroundProperty = BindableProperty.Create(
                nameof(SelectionForeground),
                typeof(Color),
                typeof(MenuDropDown),
                Color.White,
                OnSelectionForegroundPropertyChanged
            );
            HintColorProperty = BindableProperty.Create(
                nameof(HintColor),
                typeof(Color),
                typeof(MenuDropDown),
                Color.Yellow,
                OnHintColorPropertyChanged
            );
            DisabledColorProperty = BindableProperty.Create(
                nameof(DisabledColor),
                typeof(Color),
                typeof(MenuDropDown),
                defaultValue: Color.DarkGray,
                propertyChanged: OnDisabledColorPropertyChanged
            );
            ItemsProperty = BindableProperty.Create(
                nameof(Items),
                typeof(IList<MenuElement>),
                typeof(MenuDropDown),
                defaultValue: Array.Empty<MenuElement>(),
                propertyChanged: OnItemsPropertyChanged
            );
            SelectedItemProperty = BindableProperty.Create(
                nameof(SelectedItem),
                typeof(MenuItem),
                typeof(MenuDropDown),
                defaultValue: null,
                propertyChanged: OnSelectedItemPropertyChanged
            );
        }*/

        public static void Show(Rectangle anchor, IList<MenuElement> menuElements)
        {
            var application = ConsoleApplication.Instance;
            var dialogManager = application.DialogManager;
            var dropDown = new MenuDropDown
            {
                Background = Color.DarkCyan,
                Foreground = Color.White,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Left = anchor.Left,
                Top = anchor.Top + 1
            };

            var menuList = new MenuList
            {
                Background = Color.Cyan,
                Foreground = Color.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 24,
                Height = 10
            };

            for (int index = 0; index < menuElements.Count; index++)
            {
                menuList.Items.Add(menuElements[index]);
            }

            dropDown.Children.Add(menuList);

            dialogManager.ShowModal(dropDown);
        }

        public override Size Measure(int widthConstraint, int heightConstraint)
        {
            return base.Measure(widthConstraint, heightConstraint);
        }

        /*public override Size Measure(int widthConstraint, int heightConstraint)
        {
            if (false == IsMeasureValid)
            {
                var width = 0;
                var count = null == Items || 0 == Items.Count ? 0 : Items.Count;

                for (var index = 0; index < count; index++)
                {
                    if (Items[index] is MenuItem menuItem)
                    {
                        var length = menuItem.TitleLength;
                        width = Math.Max(width, length + 6);
                    }
                }

                var height = Math.Min(count, heightConstraint);

                DesiredSize = new Size(Math.Min(width + 2, widthConstraint), height + 2);
                IsMeasureValid = true;
            }

            return DesiredSize;
        }*/

        /*public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            if (IsDirty)
            {
                var rectangle = new SadRogue.Primitives.Rectangle(0, 0, Bounds.Width, Bounds.Height);

                RenderSurface.Fill(rectangle, Foreground, Background, Glyphs.Whitespace);
                Frame.Render(RenderSurface);

                if (null != Items && 0 < Items.Count)
                {
                    var left = Padding.Left + 2;
                    var top = Padding.Top + 1;

                    for (var index = 0; index < Items.Count; index++)
                    {
                        var rect = SadRogue.Primitives.Rectangle.Empty;
                        var background = Background;
                        var foreground = Foreground;
                        var hint = HintColor;

                        //var menuItem = Items[index];
                        if (Items[index] is MenuItem menuItem)
                        {
                            //var length = MenuItem.GetLength(menuItem.Title);
                            var isSelected = IsFocused && ReferenceEquals(SelectedItem, menuItem);
                            
                            background = isSelected ? SelectionBackground : Background;
                            foreground = GetItemForegroundColor(menuItem, isSelected);
                            rect = new SadRogue.Primitives.Rectangle(left, top, Bounds.Width - (left + Padding.Right + 2), 1);

                            //menuItem.Render(RenderSurface, rect, background, foreground, HintColor);
                        }
                        else if (Items[index] is MenuDelimiter menuDelimiter)
                        {
                            //menuDelimiter.Render(RenderSurface, rect, Background, Foreground, Color.Transparent);
                            rect = new SadRogue.Primitives.Rectangle(rectangle.X, top, rectangle.Width, 1);
                            hint = Color.Transparent;
                        }

                        Items[index].Render(RenderSurface, rect, false);

                        top += 1;
                    }
                }

                IsDirty = false;
            }

            RenderSurface.Copy(surface);
        }*/

        /*private Color GetItemForegroundColor(MenuItem menuItem, bool isSelected)
        {
            if (menuItem.IsEnabled)
            {
                return isSelected ? SelectionForeground : Foreground;
            }

            return DisabledColor;
        }

        private void OnItemsChanged()
        {
            InvalidateMeasureInternal(InvalidateTrigger.MeasureChanged);
            Invalidate();
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

        private static void OnDisabledColorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuBar)sender).Invalidate();
        }

        private static void OnItemsPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuDropDown)sender).OnItemsChanged();
        }

        private static void OnSelectedItemPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuDropDown)sender).Invalidate();
        }*/
    }
}