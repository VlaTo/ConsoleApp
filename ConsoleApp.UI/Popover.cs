using System.Drawing;
using Color = SadRogue.Primitives.Color;
using Rectangle = System.Drawing.Rectangle;
using Window = ConsoleApp.UI.Controls.Window;

namespace ConsoleApp.UI
{
    public class Popover : Window
    {
        public Popover()
        {
        }

        public static void Show(Rectangle rectangle)
        {
            var application = ConsoleApplication.Instance;
            var dialogManager = application.DialogManager;
            var popover = new Popover
            {
                Background = Color.DarkCyan,
                Foreground = Color.White,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Left = rectangle.Left,
                Top = rectangle.Top + 1,
                Width = 30,
                Height = 10
            };

            dialogManager.ShowModal(popover);
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