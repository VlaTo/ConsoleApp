using System;
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleApp.UI
{
    public enum StackOrientation
    {
        Horizontal,
        Vertical
    }

    public class StackLayoutManager : LayoutManager, ILayoutManager
    {
        public StackOrientation Orientation
        {
            get;
            set;
        }

        public StackLayoutManager()
        {
            Orientation = StackOrientation.Horizontal;
        }

        public Size Measure(IList<VisualElement> children, int widthConstraint, int heightConstraint)
        {
            switch (Orientation)
            {
                case StackOrientation.Horizontal:
                {
                    return MeasureHorizontal(children, widthConstraint, heightConstraint);
                }

                case StackOrientation.Vertical:
                {
                    return MeasureVertical(children, widthConstraint, heightConstraint);
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        public void Arrange(IList<VisualElement> children, Rectangle bounds)
        {
            switch (Orientation)
            {
                case StackOrientation.Horizontal:
                {
                    ArrangeHorizontal(children, bounds);
                    break;
                }

                case StackOrientation.Vertical:
                {
                    ArrangeVertical(children, bounds);
                    break;
                }

                default:
                {
                    throw new NotSupportedException();
                }
            }
        }

        private static Size MeasureHorizontal(IList<VisualElement> children, in int widthConstraint, in int heightConstraint)
        {
            var width = widthConstraint;
            var arrange = 0;

            for (var index = 0; index < children.Count; index++)
            {
                var child = children[index];

                if (false == child.IsVisible)
                {
                    continue;
                }

                if (HorizontalAlignment.Stretch == child.HorizontalAlignment)
                {
                    arrange++;
                    continue;
                }

                var size = child.Measure(width, heightConstraint);

                width -= size.Width;
            }

            if (0 < arrange)
            {
                var slot = width / arrange;

                for (var index = 0; index < children.Count; index++)
                {
                    var child = children[index];

                    if (HorizontalAlignment.Stretch != child.HorizontalAlignment)
                    {
                        continue;
                    }

                    child.Measure(width, heightConstraint);

                    width -= slot;
                }
            }

            return new Size(widthConstraint, heightConstraint);
        }

        private static Size MeasureVertical(IList<VisualElement> children, in int widthConstraint, in int heightConstraint)
        {
            var height = heightConstraint;
            var arrange = 0;

            for (var index = 0; index < children.Count; index++)
            {
                var child = children[index];

                if (false == child.IsVisible)
                {
                    continue;
                }

                if (VerticalAlignment.Stretch == child.VerticalAlignment)
                {
                    arrange++;
                    continue;
                }

                var size = child.Measure(widthConstraint, height);

                height -= size.Height;
            }

            if (0 < arrange)
            {
                var slot = height / arrange;

                for (var index = 0; index < children.Count; index++)
                {
                    var child = children[index];

                    if (child.VerticalAlignment != VerticalAlignment.Stretch)
                    {
                        continue;
                    }

                    child.Measure(widthConstraint, height);
                    
                    height -= slot;
                }
            }

            return new Size(widthConstraint, heightConstraint);
        }

        private static void ArrangeVertical(IList<VisualElement> children, Rectangle bounds)
        {
            var y = bounds.Y;
            var height = bounds.Height;

            for (var index = 0; index < children.Count; index++)
            {
                var child = children[index];

                if (false == child.IsVisible)
                {
                    continue;
                }

                child.Arrange(new Rectangle(bounds.X, y, bounds.Width, height));

                y += child.Bounds.Height;
                height -= child.Bounds.Height;
            }
        }

        private static void ArrangeHorizontal(IList<VisualElement> children, Rectangle bounds)
        {
            var x = bounds.X;
            var width = bounds.Width;

            for (var index = 0; index < children.Count; index++)
            {
                var child = children[index];

                if (false == child.IsVisible)
                {
                    continue;
                }

                child.Arrange(new Rectangle(x, bounds.Y, width, bounds.Height));

                x += child.Bounds.Width;
                width -= child.Bounds.Width;
            }
        }
    }
}