using SadConsole.Input;

namespace ConsoleApp.UI.Extensions
{
    internal static class KeyboardExtensions
    {
        public static ShiftKeys ToShiftKeys(this Keyboard keyboard)
        {
            ShiftKeys shiftKeys = 0;

            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                shiftKeys |= ShiftKeys.LeftShift;
            }

            if (keyboard.IsKeyDown(Keys.RightShift))
            {
                shiftKeys |= ShiftKeys.RightShift;
            }

            if (keyboard.IsKeyDown(Keys.LeftControl))
            {
                shiftKeys |= ShiftKeys.LeftCtrl;
            }

            if (keyboard.IsKeyDown(Keys.RightControl))
            {
                shiftKeys |= ShiftKeys.RightCtrl;
            }

            if (keyboard.IsKeyDown(Keys.LeftAlt))
            {
                shiftKeys |= ShiftKeys.LeftAlt;
            }

            if (keyboard.IsKeyDown(Keys.RightAlt))
            {
                shiftKeys |= ShiftKeys.RightAlt;
            }

            return shiftKeys;
        }
    }
}