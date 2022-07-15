using System.Collections.Generic;
using System.Drawing;

namespace ConsoleApp.UI
{
    public interface ILayoutManager
    {
        Size Measure(IList<VisualElement> children, int widthConstraint, int heightConstraint);

        void Arrange(IList<VisualElement> children, Rectangle bounds);
    }
}