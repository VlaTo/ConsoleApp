using ConsoleApp.Bindings;
using SadConsole;
using SadRogue.Primitives;

namespace ConsoleApp.UI.Controls
{
    public abstract class MenuElement : BindableObject
    {
        public static readonly BindableProperty IsEnabledProperty;
        public static readonly BindableProperty MenuProperty;
        public static readonly BindableProperty ParentProperty;
        
        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
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

        public abstract bool IsSelectable
        {
            get;
        }

        static MenuElement()
        {
            IsEnabledProperty = BindableProperty.Create(
                nameof(IsEnabled),
                typeof(bool),
                typeof(MenuElement),
                defaultValue: true,
                propertyChanged: OnIsEnabledPropertyChanged
            );
            MenuProperty = BindableProperty.Create(
                nameof(Menu),
                typeof(MenuBar),
                typeof(MenuElement),
                defaultValue: null,
                propertyChanged: OnMenuPropertyChanged
            );
            ParentProperty = BindableProperty.Create(
                nameof(Parent),
                typeof(MenuItem),
                typeof(MenuElement),
                defaultValue: null,
                propertyChanged: OnParentPropertyChanged
            );
        }

        //public abstract System.Drawing.Rectangle GetBounds();

        public abstract void Render(ICellSurface surface, Rectangle rectangle, bool isSelected /*, Color background, Color foreground, Color hint*/);

        protected Color GetForegroundColor(bool isSelected)
        {
            if (IsEnabled)
            {
                return isSelected ? Menu.SelectionForeground : Menu.Foreground;
            }

            return Menu.DisabledColor;
        }

        protected virtual void OnIsEnabledChanged()
        {
            ;
        }

        protected virtual void OnMenuChanged()
        {
            ;
        }

        protected virtual void OnParentChanged()
        {
            ;
        }
        
        private static void OnIsEnabledPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuElement)sender).OnIsEnabledChanged();
        }

        private static void OnMenuPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuElement)sender).OnMenuChanged();
        }

        private static void OnParentPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuElement)sender).OnParentChanged();
        }
    }
}