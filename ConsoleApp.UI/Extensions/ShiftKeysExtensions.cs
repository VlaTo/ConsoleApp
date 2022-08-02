namespace ConsoleApp.UI.Extensions
{
    internal static class ShiftKeysExtensions
    {
        public static bool HasShift(this ShiftKeys keys)
        {
            return 0 != (keys & (ShiftKeys.LeftShift | ShiftKeys.RightShift));
        }
    }
}