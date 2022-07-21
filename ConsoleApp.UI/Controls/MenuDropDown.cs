using System.Collections.Generic;
using System.Drawing;
using Color = SadRogue.Primitives.Color;

namespace ConsoleApp.UI.Controls
{
    public sealed class MenuDropDown : Window
    {
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
                Background = Color.DarkCyan,
                Foreground = Color.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 24,
                Height = 10
            };

            // NOTE: use Binding here
            for (var index = 0; index < menuElements.Count; index++)
            {
                menuList.Items.Add(menuElements[index]);
            }

            dropDown.Children.Add(menuList);

            dialogManager.ShowModal(dropDown);
        }

        public override Size Measure(int widthConstraint, int heightConstraint)
        {
            if (false == IsMeasureValid)
            {
                var thickness = Frame.Thickness;

                widthConstraint -= Margin.HorizontalThickness;
                heightConstraint -= Margin.VerticalThickness;

                widthConstraint = ResolveConstraint(widthConstraint, Width, Shadow.Width);
                heightConstraint = ResolveConstraint(heightConstraint, Height, Shadow.Height);

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