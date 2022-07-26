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
        public static readonly BindableProperty IsVisibleProperty;

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
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
            IsVisibleProperty = BindableProperty.Create(
                nameof(IsVisible),
                typeof(bool),
                typeof(MenuElement),
                defaultValue: true,
                propertyChanged: OnIsVisiblePropertyChanged
            );
        }

        public abstract void Render(ICellSurface surface, Rectangle rectangle, bool isSelected, bool renderGlyph);

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

        protected virtual void OnIsVisibleChanged()
        {
            ;
        }

        private static void OnIsEnabledPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuElement)sender).OnIsEnabledChanged();
        }

        private static void OnIsVisiblePropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((MenuElement)sender).OnIsVisibleChanged();
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