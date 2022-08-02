using SadConsole.Input;

namespace ConsoleApp.UI
{
    internal class KeyboardState : IKeyboardState
    {
        public static readonly IKeyboardState Empty;

        static KeyboardState()
        {
            Empty = new KeyboardState();
        }

        public bool IsKeyDown(Keys key)
        {
            throw new System.NotImplementedException();
        }

        public bool IsKeyUp(Keys key)
        {
            throw new System.NotImplementedException();
        }

        public Keys[] GetPressedKeys()
        {
            throw new System.NotImplementedException();
        }

        public bool CapsLock => false;

        public bool NumLock => false;
    }
}