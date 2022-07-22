using System.Collections.Generic;
using System.Drawing;
using ConsoleApp.Bindings;
using Color = SadRogue.Primitives.Color;

namespace ConsoleApp.UI.Controls
{
    public sealed class MenuDropDown : Window
    {
        public MenuList MenuList
        {
            get;
            private set;
        }

        public MenuDropDown()
        {
            Frame = new MenuDropDownFrame(this);
        }

        public static void Show(Rectangle anchor, IList<MenuElement> menuElements)
        {
            var application = ConsoleApplication.Instance;
            var dialogManager = application.DialogManager;
            var dropDown = new MenuDropDown
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
                Source = dropDown,
                PropertyPath = new PropertyPath(nameof(Background))
            });
            menuList.SetBinding(ForegroundProperty, new Binding
            {
                Source = dropDown,
                PropertyPath = new PropertyPath(nameof(Foreground))
            });

            for (var index = 0; index < menuElements.Count; index++)
            {
                menuList.Items.Add(menuElements[index]);
            }

            dropDown.Children.Add(menuList);
            dropDown.MenuList = menuList;
            dropDown.Background = Color.DarkCyan;
            dropDown.Foreground = Color.White;

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