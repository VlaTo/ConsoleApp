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
                Title = "~Connect to GameServer..."
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
                    new MenuItem
                    {
                        Title = "Open ~File...",
                    },
                    new MenuItem
                    {
                        Title = "Open ~Dump...",
                    },
                    new MenuDelimiter(),
                    connectMenuItem,
                    new MenuItem
                    {
                        Title = "Di~sconnect",
                        IsEnabled = false
                    },
                    new MenuDelimiter(),
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
                Title = "~Macro",
                IsEnabled = false
            });
            MenuBar.Items.Add(new MenuItem
            {
                Title = "T~ests"
            });

            toolBar = new BottomToolBar
            {
                Foreground = Color.Black,
                Background = Color.DarkCyan
            };

            Container.Children.Add(toolBar);
            
            var temp = new Window
            {
                Title = "Window #1",
                Background = Color.DarkBlue,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Left = 7,
                Top = 0,
                Width = 20,
                Height = 20
            };

            window = new EntityTreeViewWindow
            {
                Title = "Tree View",
                Background = Color.DarkBlue,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                //Margin = new Thickness(8)
                Left = 2,
                Top = 2,
                Width = 80,
                Height = 40
            };

            Background.Children.Add(window);
            Background.Children.Add(temp);
        }

        private void DoConnect(object sender, EventArgs e)
        {
            toolBar.Hint = "EntityTreeViewer, version: 0.1";
        }

        private void DoExit(object sender, EventArgs e)
        {
            ;
        }
    }
}
