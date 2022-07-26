using System;
using System.Collections.Generic;
using System.Drawing;

namespace ConsoleApp.UI
{
    public class AbsoluteLayoutManager : LayoutManager, ILayoutManager
    {
        public Size Measure(IList<VisualElement> children, int widthConstraint, int heightConstraint)
        {
            var width = 0;
            var height = 0;

            for (var index = 0; index < children.Count; index++)
            {
                var child = children[index];

                if (false == child.IsVisible)
                {
                    continue;
                }

                var size = child.Measure(widthConstraint, heightConstraint);

                width = Math.Max(width, size.Width);
                height = Math.Max(height, size.Height);
            }

            return new Size(width, height);
        }

        public void Arrange(IList<VisualElement> children, Rectangle bounds)
        {
            for (var index = 0; index < children.Count; index++)
            {
                var child = children[index];

                if (child.IsVisible)
                {
                    child.Arrange(bounds);
                }
            }
        }
    }
}