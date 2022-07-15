using ConsoleApp.UI.Controls;
using SadRogue.Primitives;

namespace ConsoleApp.Controls
{
    public class EntityTreeViewWindow : Window
    {
        //private readonly UI.TextBox textBox;
        private readonly LoadingIndicator indicator;

        public EntityTreeViewWindow()
        {
            /*textBox = new UI.TextBox
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

            indicator = new LoadingIndicator
            {
                Foreground = Color.White,
                Background = Color.Transparent,
                Left = 2,
                Top = 5,
                Width = 36,
                Height = 1
            };

            Children.Add(indicator);
            //Children.Add(textBox);
        }
    }
}