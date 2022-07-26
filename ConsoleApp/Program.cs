using ConsoleApp.Controls;
using ConsoleApp.UI;
using ConsoleApp.UI.Controls;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using SadRogue.Primitives;
using Game = SadConsole.Game;
using HorizontalAlignment = SadConsole.HorizontalAlignment;
using VerticalAlignment = SadConsole.VerticalAlignment;
using Window = SadConsole.UI.Window;

namespace ConsoleApp
{
    public static class Program
    {
        internal const int ConsoleWidth = 120;
        internal const int ConsoleHeight = 50;

        public static void Main(string[] args)
        {
            var screen = new Screen
            {
                Width = 120,
                Height = 50
            };

            var application = new EntityTreeViewerApplication(screen);

            application.Run();

            /*
            // Setup the engine and create the main window.
            SadConsole.Game.Create(ConsoleWidth, ConsoleHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.Instance.OnStart = OnConsoleStart;

            // Start the game.
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
            */
        }

        /*private static void Init1()
        {
            var container = new BackgroundConsole("container");

            SadConsole.Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.None;
            ((SadConsole.Game)SadConsole.Game.Instance).WindowResized += OnResizeConsole;

            SadConsole.Global.CurrentScreen = container;

            var background = new SadConsole.Console(80, 25);

            //background.FillWithRandomGarbage();
            background.Fill(Color.DarkGray, Color.Blue, 177);

            container.Children.Add(background);

            var childConsole = new SadConsole.Console(22, 4)
            {
                Position = new Point(32, 2)
            };

            childConsole.Fill(null, Color.DarkGray, null);
            childConsole.Print(1, 1, "Child console#1");
            childConsole.Components.Add(new MouseMoveConsoleComponent());
            container.Children.Add(childConsole);

            var controlsConsole = new SadConsole.ControlsConsole(22, 8)
            {
                Position = new Point(32, 10)
            };
            var label1 = new SadConsole.Controls.Label(20)
            {
                Alignment = SadConsole.HorizontalAlignment.Left,
                DisplayText = "Label#1",
                Name = "Label1",
                Position = new Point(2, 1)
            };
            var timer1 = new SadConsole.Timer(TimeSpan.FromSeconds(0.5d));
            var count = 0;
            timer1.TimerElapsed += (sender, e) =>
            {
                label1.DisplayText = $"Timer: {++count}";
                //controlsConsole.IsDirty = true;
                label1.IsDirty = true;
            };
            var button1 = new SadConsole.Controls.Button(8, 1)
            {
                Position = new Point(4, 4),
                Name = "Button1",
                Text = "OK",
                TextAlignment = SadConsole.HorizontalAlignment.Center
            };
            var textBox = new SadConsole.Controls.Label(10)
            {
                Text = "Sample",
                Position = new Point(4, 6),
                Name = "textBox1"
            };

            controlsConsole.Add(label1);
            controlsConsole.Add(button1);
            controlsConsole.Add(textBox);
            controlsConsole.Components.Add(timer1);

            container.Children.Add(controlsConsole);
        }*/

        private static void OnConsoleStart()
        {
            // Any startup code for your game. We will use an example console for now
            //var startingConsole = (SadConsole.Console) GameHost.Instance.Screen;
            var console = new ControlsConsole(ConsoleWidth, ConsoleHeight);

            //startingConsole.FillWithRandomGarbage(255);
            //startingConsole.Fill(new SadRogue.Primitives.Rectangle(3, 3, 27, 5), null, SadRogue.Primitives.Color.Black, 0, Mirror.None);
            //startingConsole.Print(6, 5, "Hello from SadConsole", SadRogue.Primitives.Color.AnsiCyanBright);


            console.FillWithRandomGarbage(255);

            /*
            // WINDOW 1
            var window1 = new Window(57, 23)
            {
                Position = new SadRogue.Primitives.Point(2, 1),
                Title = "Window #1",
                IsVisible = true,
                DefaultBackground = SadRogue.Primitives.Color.Blue,
            };
            var button = new Button(14)
            {
                Position = new SadRogue.Primitives.Point(10, 20),
                FocusOnClick = true,
                Text = "Message",
                IsVisible = true,
            };

            button.Click += (o, args) =>
            {
                Window.Message("Sample Text Message Window", "Close", () => { }, Colors.CreateSadConsoleBlue(), new Button3dTheme());
            };
            window1.Controls.Add(button);

            // WINDOW 2
            var window2 = new Window(57, 23)
            {
                Position = new SadRogue.Primitives.Point(61, 1),
                Title = "Window #2",
                IsVisible = true,
                DefaultBackground = SadRogue.Primitives.Color.Blue,
            };

            // WINDOW 3
            var window3 = new Window(116, 24)
            {
                Position = new SadRogue.Primitives.Point(2, 25),
                Title = "Window #3",
                IsVisible = true,
                DefaultBackground = SadRogue.Primitives.Color.Blue,
            };

            console.Children.Add(window1);
            console.Children.Add(window2);
            console.Children.Add(window3);
            */
            
            var surface = console.Surface;
            var rectangle = new Rectangle(2, 2, 10, 5);
            var subSurface = surface.GetSubSurface(rectangle);
            var area = new Rectangle(0, 0, 10, 5);

            subSurface.Fill(background: Color.AnsiBlack, glyph: ' ');
            subSurface.Fill(area: area, background: Color.AnsiCyan, glyph: ' ');

            /*SadConsole.Game.OnInitialize = () =>
            {
            };
            
            SadConsole.Game.OnDraw = (time) =>
            {
            };

            SadConsole.Game.OnUpdate = (time) =>
            {
            };

            var container = new SadConsole.Console(80, 25);

            SadConsole.Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.None;
            ((SadConsole.Game)SadConsole.Game.Instance).WindowResized += OnResizeConsole;

            SadConsole.Global.CurrentScreen = container;

            var background = new BackgroundConsole("background");
            container.Children.Add(background);

            var console = new BackgroundConsole("inner");
            background.Children.Add(console);*/

            Game.Instance.Screen = console;
            Game.Instance.DestroyDefaultStartingConsole();
        }

        /*private static void OnResizeConsole(object sender, EventArgs e)
        {
            var container = (SadConsole.ContainerConsole) SadConsole.Global.CurrentScreen;

            var width = SadConsole.Global.WindowWidth / container.Font.Size.X;
            var height = SadConsole.Global.WindowHeight / container.Font.Size.Y;

            var background = container.Children[0];

            background.Resize(width, height, false);
            background.Fill(Color.DarkGray, Color.Blue, 177);
        }*/
    }

    /*internal sealed class BackgroundConsole : SadConsole.ContainerConsole
    {
        private readonly string name;

        public BackgroundConsole(string name)
        {
            this.name = name;
        }

        public override void Draw(TimeSpan timeElapsed)
        {
            Debug.WriteLine($"[BackgroundConsole] [Draw] name: \"{name}\"");
            base.Draw(timeElapsed);
        }

        public override void Update(TimeSpan timeElapsed)
        {
            Debug.WriteLine($"[BackgroundConsole] [Update] name: \"{name}\"");
            base.Update(timeElapsed);
        }
    }

    internal sealed class MouseMoveConsoleComponent : MouseConsoleComponent
    {
        public override void ProcessMouse(SadConsole.Console console, MouseConsoleState state, out bool handled)
        {
            if (state.Mouse.IsOnScreen && state.Mouse.LeftButtonDown)
            {
                console.Position = state.WorldCellPosition;
                handled = true;

                return;
            }

            handled = false;
        }
    }*/
}
