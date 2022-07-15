using System;
using ConsoleApp.UI.Controls;
using SadRogue.Primitives;

namespace ConsoleApp.UI
{
    public class ConsoleApplication
    {
        public static ConsoleApplication Instance;

        public Screen Screen
        {
            get;
        }

        public MenuBar MenuBar
        {
            get;
        }

        public Background Background
        {
            get;
        }

        public Layout Container
        {
            get;
        }

        public DialogManager DialogManager
        {
            get;
        }

        protected ConsoleApplication(Screen screen)
        {
            Instance = this;

            Screen = screen;
            MenuBar = new MenuBar
            {
                Background = Color.DarkCyan,
                Foreground = Color.Black,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            Container = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            Background = new Background
            {
                Background = Color.DarkBlue,
                Foreground = Color.DarkGray,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            var time = new TimeView
            {
                Background = Color.DarkCyan,
                Foreground = Color.Black,
                Clock = new DateTimeClock(),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 8
            };

            var topBar = new StackLayout
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Orientation = StackOrientation.Horizontal,
                Height = 1
            };

            topBar.Children.Add(MenuBar);
            topBar.Children.Add(time);
            
            Container.Children.Add(topBar);
            Container.Children.Add(Background);

            Screen.Children.Add(Container);

            MenuBar.OnMenuCancel += DoMenuCancel;
            
            DialogManager = new DialogManager(screen);
        }

        public void Run()
        {
            Screen.Start();
            Background.Focus();
            SadConsole.Game.Instance.Run();
        }

        protected virtual void DoMenuCancel(object sender, EventArgs e)
        {
            Background.Focus();
        }
    }
}