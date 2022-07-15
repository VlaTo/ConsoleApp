using System;
using ConsoleApp.Controls;
using ConsoleApp.UI;
using ConsoleApp.UI.Controls;
using SadRogue.Primitives;

namespace ConsoleApp
{
    internal sealed class EntityTreeViewerApplication:ConsoleApplication
    {
        private readonly EntityTreeViewWindow window;
        private readonly MenuItem connectMenuItem;
        private readonly MenuItem exitMenuItem;
        private readonly BottomToolBar toolBar;

        public EntityTreeViewerApplication(Screen screen)
            : base(screen)
        {
            screen.WindowTitle = "Entity Tree Viewer";

            connectMenuItem = new MenuItem
            {
                Title = "~Connect..."
            };
            exitMenuItem = new MenuItem
            {
                Title = "E~xit",
            };

            connectMenuItem.OnClick += DoConnect;
            exitMenuItem.OnClick += DoExit;

            MenuBar.Items.Add(new MenuItem
            {
                Title = "~Commands",
                Items =
                {
                    connectMenuItem,
                    exitMenuItem
                }
            });
            MenuBar.Items.Add(new MenuItem
            {
                Title = "~Refresh"
            });
            MenuBar.Items.Add(new MenuItem
            {
                Title = "~Timer"
            });
            MenuBar.Items.Add(new MenuItem
            {
                Title = "~Macro"
            });
            MenuBar.Items.Add(new MenuItem
            {
                Title = "T~ests"
            });

            toolBar = new BottomToolBar
            {
                Foreground = Color.Black,
                Background = Color.DarkCyan,
                Hint = "EntityTreeViewer, version: 0.1"
            };

            Container.Children.Add(toolBar);
            
            var temp = new Window
            {
                Title = "Window #1",
                Background = Color.DarkBlue,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Left = 90,
                Top = 0,
                Width = 20,
                Height = 20
            };

            window = new EntityTreeViewWindow
            {
                Title = "Tree View",
                Background = Color.DarkBlue,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(8)
            };

            Background.Children.Add(window);
            Background.Children.Add(temp);
        }

        private void DoConnect(object sender, EventArgs e)
        {
            ;
        }

        private void DoExit(object sender, EventArgs e)
        {
            ;
        }
    }
}
