using System.Drawing;

namespace ConsoleApp.UI.Controls
{
    public class AbsoluteLayout : Layout
    {
        public AbsoluteLayout()
        {
            LayoutManager = new AbsoluteLayoutManager();
        }

        public override void Arrange(Rectangle bounds)
        {
            base.Arrange(bounds);
        }

        public override Size Measure(int widthConstraint, int heightConstraint)
        {
            return base.Measure(widthConstraint, heightConstraint);
        }
    }
}