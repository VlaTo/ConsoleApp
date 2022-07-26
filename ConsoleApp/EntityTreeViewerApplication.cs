using ConsoleApp.Controls;
using ConsoleApp.UI;
using ConsoleApp.UI.Controls;
using SadRogue.Primitives;
using System;
using ConsoleApp.UI.Commands;

namespace ConsoleApp
{
    internal sealed class EntityTreeViewerApplication : ConsoleApplication
    {
        private readonly EntityTreeViewWindow window;
        // private readonly MenuItem connectMenuItem;
        // private readonly MenuItem exitMenuItem;
        private readonly BottomToolBar toolBar;
        // private MenuItem windowsMenuItem;

        public EntityTreeViewerApplication(Screen screen)
            : base(screen)
        {
            screen.WindowTitle = "Entity Tree Viewer";

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
                        ShortCut = new ShortCut(SadConsole.Input.Keys.F3)
                    },
                    new MenuItem
                    {
                        Title = "Open Recent",
                        Items =
                        {
                            new MenuItem
                            {
                                Title = "~1. 10.171.11.23:23460",
                            },
                            new MenuItem
                            {
                                Title = "~2. 10.171.11.23:28460",
                            },
                            new MenuItem
                            {
                                Title = "~3. 10.171.11.23:53460",
                            },
                            new MenuItem
                            {
                                Title = "~4. 10.171.11.23:38460",
                            }
                        }
                    },
                    new MenuDelimiter(),
                    new MenuItem
                    {
                        Title = "~Connect to GameServer...",
                        ShortCut = new ShortCut(SadConsole.Input.Keys.F4, KeyModificator.LeftCtrl | KeyModificator.RightCtrl),
                        Command = new DelegateCommand(DoConnect)
                    },
                    new MenuItem
                    {
                        Title = "Di~sconnect",
                        IsEnabled = false
                    },
                    new MenuDelimiter(),
                    new MenuItem
                    {
                        Title = "E~xit",
                        ShortCut = new ShortCut(SadConsole.Input.Keys.F10),
                        Command = new DelegateCommand(DoExit, () => false)
                    }
                }
            });
            MenuBar.Items.Add(new MenuItem
            {
                Title = "~Macro",
                Items =
                {
                    new MenuItem
                    {
                        Title = "~Execute macros...",
                    },
                    new MenuDelimiter(),
                    new MenuItem
                    {
                        Title = "~1. Add All heroes",
                    },
                    new MenuItem
                    {
                        Title = "~2. Build City",
                    },
                    new MenuItem
                    {
                        Title = "~3. Collect rewards",
                    },
                    new MenuItem
                    {
                        Title = "~4. Power-up Catgirl",
                    },
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
                Title = "~Windows",
            });
            MenuBar.Items.Add(new MenuItem
            {
                Title = "T~ests",
                IsEnabled = false
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

            WindowManager.AddWindow(window);
            WindowManager.AddWindow(temp);
        }

        private void DoConnect()
        {
            toolBar.Hint = "EntityTreeViewer, version: 0.1";
        }

        private void DoExit()
        {
            Exit();
        }
    }
}
