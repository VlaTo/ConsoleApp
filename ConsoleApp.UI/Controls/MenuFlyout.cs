using System.Drawing;
using ConsoleApp.Bindings;
using Color = SadRogue.Primitives.Color;

namespace ConsoleApp.UI.Controls
{
    public sealed class MenuFlyout : Popup
    {
        public MenuList MenuList
        {
            get;
            private set;
        }

        public MenuFlyout()
        {
            Frame = new MenuDropDownFrame(this);
        }

        public static MenuFlyout Create(Rectangle anchor)
        {
            var flyout = new MenuFlyout
            {
                FrameType = WindowFrameType.Thick,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Left = anchor.Left,
                Top = anchor.Top + 1
            };

            var menuList = new MenuList
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            menuList.SetBinding(BackgroundProperty, new Binding
            {
                Source = flyout,
                PropertyPath = new PropertyPath(nameof(Background))
            });
            menuList.SetBinding(ForegroundProperty, new Binding
            {
                Source = flyout,
                PropertyPath = new PropertyPath(nameof(Foreground))
            });

            flyout.Children.Add(menuList);
            flyout.MenuList = menuList;
            flyout.Background = Color.DarkCyan;
            flyout.Foreground = Color.White;

            return flyout;
        }

        public override Size Measure(int widthConstraint, int heightConstraint)
        {
            if (false == IsMeasureValid)
            {
                var thickness = Frame.Thickness;

                widthConstraint -= Margin.HorizontalThickness;
                heightConstraint -= Margin.VerticalThickness;

                widthConstraint = ResolveWidth(widthConstraint);
                heightConstraint = ResolveHeight(heightConstraint);

                var width = widthConstraint - thickness.HorizontalThickness;
                var height = heightConstraint - thickness.VerticalThickness;
                var childrenSize = LayoutManager.Measure(Children, width, height);

                DesiredSize = new Size(
                    childrenSize.Width + thickness.HorizontalThickness + Shadow.Width,
                    childrenSize.Height + thickness.VerticalThickness + Shadow.Height
                );

                IsMeasureValid = true;
            }

            return DesiredSize;

        }
    }
}