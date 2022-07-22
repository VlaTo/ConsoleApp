using System;
using ConsoleApp.UI.Controls;
using SadRogue.Primitives;

namespace ConsoleApp.UI
{
    public class ConsoleApplication
    {
        public static ConsoleApplication Instance;

        private readonly Background background;

        public Screen Screen
        {
            get;
        }

        public MenuBar MenuBar
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

        public WindowManager WindowManager
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

            background = new Background
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
            Container.Children.Add(background);

            Screen.Children.Add(Container);

            MenuBar.OnMenuCancel += DoMenuCancel;
            
            DialogManager = new DialogManager(screen);
            WindowManager = background.WindowManager;
        }

        public void Run()
        {
            Screen.Start();
            background.Focus();
            SadConsole.Game.Instance.Run();
        }

        public void Exit()
        {
            var instance = SadConsole.Game.Instance.MonoGameInstance;
            instance.Exit();
        }

        protected virtual void DoMenuCancel(object sender, EventArgs e)
        {
            background.Focus();
        }
    }
}