using ConsoleApp.UI;
using ConsoleApp.UI.Controls;
using SadRogue.Primitives;

namespace ConsoleApp.Controls
{
    public class EntityTreeViewWindow : Window
    {
        //private readonly UI.Label textBox;
        private readonly ProgressBar indicator;

        public EntityTreeViewWindow()
        {
            /*textBox = new UI.Label
            {
                Foreground = Color.White,
                Background = Color.Brown,
                Left = 10,
                Top = 3,
                Width = 17,
                Height = 5,
                HorizontalAlignment = UI.HorizontalAlignment.Stretch,
                VerticalAlignment = UI.VerticalAlignment.Top,
                Text = "Text"
            };*/

            indicator = new ProgressBar
            {
                Foreground = Color.White,
                Background = Color.Transparent,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                IsIndeterminate = true,
                Left = 3,
                Top = 2,
                Width = 72,
                Height = 1
            };

            Children.Add(indicator);
            //Children.Add(textBox);
        }
    }
}