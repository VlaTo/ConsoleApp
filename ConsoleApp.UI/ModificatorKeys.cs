using System;
using SadConsole.Input;

namespace ConsoleApp.UI
{
    public sealed class ModificatorKeys
    {
        private static readonly Keys[] allModificatorKeys;

        private readonly Keyboard keyboard;

        public bool IsLeftShiftPressed => keyboard.IsKeyDown(Keys.LeftShift);
        
        public bool IsRightShiftPressed => keyboard.IsKeyDown(Keys.RightShift);
        
        public bool IsShiftPressed => IsLeftShiftPressed || IsRightShiftPressed;

        public bool IsEmpty => Array.TrueForAll(allModificatorKeys, key => false == keyboard.IsKeyDown(key));

        public bool HasAny => Array.Exists(allModificatorKeys, keyboard.IsKeyDown);

        public ModificatorKeys(Keyboard keyboard)
        {
            this.keyboard = keyboard;
        }

        static ModificatorKeys()
        {
            allModificatorKeys = new[]
            {
                Keys.LeftShift,
                Keys.RightShift,
                Keys.LeftControl,
                Keys.RightControl,
                Keys.LeftAlt,
                Keys.RightAlt,
                Keys.LeftWindows,
                Keys.RightWindows
            };
        }
    }
}