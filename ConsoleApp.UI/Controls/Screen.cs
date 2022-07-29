using ConsoleApp.Bindings;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using System;
using SadRogue.Primitives;
using Settings = SadConsole.Settings;

namespace ConsoleApp.UI.Controls
{
    public class Screen : VisualGroup
    {
        private SadConsole.Console console;
        private RootConsole container;

        public static readonly BindableProperty WindowTitleProperty;

        public string WindowTitle
        {
            get => (string)GetValue(WindowTitleProperty);
            set => SetValue(WindowTitleProperty, value);
        }

        public bool IsRunning
        {
            get;
            private set;
        }

        public Cursor Cursor
        {
            get;
            private set;
        }

        public Screen()
        {
            Left = 0;
            Top = 0;
            LayoutManager = new AbsoluteLayoutManager();
            IsRunning = false;
        }

        static Screen()
        {
            WindowTitleProperty = BindableProperty.Create(
                nameof(WindowTitle),
                typeof(string),
                typeof(Screen),
                null,
                OnWindowTitlePropertyChanged
            );
        }

        public void Start()
        {
            Game.Create(Width, Height);
            Game.Instance.OnStart = OnStart;
        }

        public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            var isDirty = IsDirty;

            base.Render(surface, elapsed);
            
            surface.IsDirty = isDirty;
        }

        protected virtual void OnWindowTitleChanged()
        {
            if (IsRunning)
            {
                UpdateWindowTitle();
            }
        }

        protected override void OnWidthChanged()
        {
            ;
        }

        protected override void OnHeightChanged()
        {
            ;
        }

        private void OnStart()
        {
            console = new SadConsole.Console(Width, Height);
            container = new RootConsole(console, Render, Update);

            Cursor = console.Cursor;

            var keyboardComponent = new KeyboardHandlerComponent(this);
            var mouseComponent = new MouseHandlerComponent(this);

            container.SadComponents.Add(keyboardComponent);
            container.SadComponents.Add(mouseComponent);
            container.Children.Add(console);

            Settings.ResizeMode = Settings.WindowResizeOptions.None;

            Game.Instance.Screen = container;
            //Game.Instance.Screen = console;
            Game.Instance.DestroyDefaultStartingConsole();
            Game.Instance.MonoGameInstance.WindowResized += OnResizeConsole;

            UpdateWindowTitle();

            var size = Measure(Width, Height);
            Arrange(new System.Drawing.Rectangle(0, 0, size.Width, size.Height));

            UpdateChildrenLayout();

            IsRunning = true;
        }

        private void HandleKeyboard(Keyboard keyboard, out bool handled)
        {
            if (0 < keyboard.KeysDown.Count)
            {
                var modificators = new ModificatorKeys(keyboard);

                for (var index = 0; index < keyboard.KeysDown.Count; index++)
                {
                    if (HandleKeyDown(keyboard.KeysDown[index], modificators))
                    {
                        handled = true;
                        return;
                    }
                }
            }

            if (0 < keyboard.KeysReleased.Count)
            {
                var modificators = new ModificatorKeys(keyboard);

                for (var index = 0; index < keyboard.KeysReleased.Count; index++)
                {
                    if (HandleKeyUp(keyboard.KeysReleased[index], modificators))
                    {
                        handled = true;
                        return;
                    }
                }
            }

            handled = false;

            if (0 < keyboard.KeysPressed.Count)
            {
                var modificators = new ModificatorKeys(keyboard);

                for (var index = 0; index < keyboard.KeysPressed.Count; index++)
                {
                    if (HandleKeyPressed(keyboard.KeysPressed[index], modificators))
                    {
                        handled = true;
                        break;
                    }
                }
            }
        }

        private void HandleMouse(MouseScreenObjectState mouseState, out bool handled)
        {
            var position = mouseState.WorldCellPosition;

            handled = false;
        }

        private void Resize()
        {
            if (false == IsRunning)
            {
                return;
            }

            InvalidateMeasureInternal(InvalidateTrigger.MeasureChanged);

            var size = Measure(Width, Height);
            var bounds = new System.Drawing.Rectangle(System.Drawing.Point.Empty, size);

            Arrange(bounds);
            UpdateChildrenLayout();
        }

        private void UpdateWindowTitle()
        {
            if (HasValue(WindowTitleProperty))
            {
                Game.Instance.MonoGameInstance.Window.Title = WindowTitle;
            }
        }

        private void OnResizeConsole(object sender, EventArgs e)
        {
            var width = Game.Instance.MonoGameInstance.WindowWidth / console.FontSize.X;
            var height = Game.Instance.MonoGameInstance.WindowHeight / console.FontSize.Y;

            console.Resize(width, height, width, height, true);

            Width = width;
            Height = height;

            Resize();
        }

        private static void OnWindowTitlePropertyChanged(BindableObject sender, object newValue, object oldValue)
        {
            ((Screen)sender).OnWindowTitleChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class RootConsole : ScreenObject
        {
            private readonly IScreenSurface screen;
            private readonly Action<ICellSurface, TimeSpan> draw;
            private readonly Action<TimeSpan> update;

            public RootConsole(
                IScreenSurface screen,
                Action<ICellSurface, TimeSpan> draw,
                Action<TimeSpan> update)
            {
                this.screen = screen;
                this.draw = draw;
                this.update = update;

                UseKeyboard = true;
                IsFocused = true;
            }

            public override void Render(TimeSpan elapsed)
            {
                draw.Invoke(screen.Surface, elapsed);
                base.Render(elapsed);
            }

            public override void Update(TimeSpan elapsed)
            {
                update.Invoke(elapsed);
                //screen.Update(elapsed);
                base.Update(elapsed);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class KeyboardHandlerComponent : KeyboardConsoleComponent
        {
            private readonly Screen screen;

            public KeyboardHandlerComponent(Screen screen)
            {
                this.screen = screen;
            }

            public override void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
            {
                screen.HandleKeyboard(keyboard, out handled);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class MouseHandlerComponent : MouseConsoleComponent
        {
            private readonly Screen screen;

            public MouseHandlerComponent(Screen screen)
            {
                this.screen = screen;
            }

            public override void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled)
            {
                screen.HandleMouse(state, out handled);
            }
        }
    }
}
