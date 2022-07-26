﻿using System;
using System.Collections.Generic;
using System.Drawing;
using ConsoleApp.Bindings;
using Color = SadRogue.Primitives.Color;

namespace ConsoleApp.UI.Controls
{
    public sealed class MenuDropDown : Popup
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

        public static MenuDropDown Create(Rectangle anchor)
        {
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

            dropDown.Children.Add(menuList);
            dropDown.MenuList = menuList;
            dropDown.Background = Color.DarkCyan;
            dropDown.Foreground = Color.White;

            return dropDown;
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