using ConsoleApp.Bindings;

namespace ConsoleApp.UI.Controls
{
    public class StackLayout : Layout
    {
        public static readonly BindableProperty OrientationProperty;

        public StackOrientation Orientation
        {
            get => (StackOrientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public StackLayout()
        {
            LayoutManager = new StackLayoutManager
            {
                Orientation = Orientation
            };
        }

        static StackLayout()
        {
            OrientationProperty = BindableProperty.Create(
                nameof(Orientation),
                typeof(StackOrientation),
                typeof(StackLayout),
                defaultValue: StackOrientation.Horizontal,
                propertyChanged: OnOrientationPropertyChanged
            );
        }

        protected virtual void OnOrientationChanged()
        {
            if (LayoutManager is StackLayoutManager manager)
            {
                manager.Orientation = Orientation;
            }
            
            Invalidate();
        }

        private static void OnOrientationPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((StackLayout)sender).OnOrientationChanged();
        }
    }
}