using System;

namespace ConsoleApp.UI.Extensions
{
    internal static class KeyModificatorExtensions
    {
        public static string ToString(this KeyModificator modificator)
        {
            var str = String.Empty;
            var modificators = new[] { GetAlt(modificator), GetCtrl(modificator), GetShift(modificator) };
            for (int index = 0; index < modificators.Length; index++)
            {
                if (String.IsNullOrEmpty(modificators[index]))
                {
                    continue;
                }

                if (false == String.IsNullOrEmpty(str))
                {
                    str += '+';
                }

                str += modificators[index];
            }

            return str;
        }

        private static string GetAlt(KeyModificator modificator)
        {
            const KeyModificator altMask = KeyModificator.LeftAlt | KeyModificator.RightAlt;
            var altKeys = modificator & altMask;

            if (0 != altKeys)
            {
                if (altMask == altKeys)
                {
                    return "Alt";
                }

                if (KeyModificator.LeftAlt == altKeys)
                {
                    return "LeftAlt";
                }

                if (KeyModificator.RightAlt == altKeys)
                {
                    return "RightAlt";
                }
            }

            return null;
        }

        private static string GetCtrl(KeyModificator modificator)
        {
            const KeyModificator ctrlMask = KeyModificator.LeftCtrl | KeyModificator.RightCtrl;
            var ctrlKeys = modificator & ctrlMask;

            if (0 != ctrlKeys)
            {
                if (ctrlMask == ctrlKeys)
                {
                    return "Ctrl";
                }

                if (KeyModificator.LeftCtrl == ctrlKeys)
                {
                    return "LeftCtrl";
                }

                if (KeyModificator.RightCtrl == ctrlKeys)
                {
                    return "RightCtrl";
                }
            }

            return null;
        }

        private static string GetShift(KeyModificator modificator)
        {
            const KeyModificator shiftMask = KeyModificator.LeftShift | KeyModificator.RightShift;
            var shiftKeys = modificator & shiftMask;

            if (0 != shiftKeys)
            {
                if (shiftMask == shiftKeys)
                {
                    return "Shift";
                }

                if (KeyModificator.LeftShift == shiftKeys)
                {
                    return "LeftShift";
                }

                if (KeyModificator.RightShift == shiftKeys)
                {
                    return "RightShift";
                }
            }

            return null;
        }
    }
}