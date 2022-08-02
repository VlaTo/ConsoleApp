using SadConsole.Input;
using System;
using ConsoleApp.UI.Extensions;

namespace ConsoleApp.UI
{
    [Flags]
    public enum ShiftKeys : short
    {
        LeftShift = 1,
        RightShift = 2,
        LeftCtrl = 4,
        RightCtrl = 8,
        LeftAlt = 16,
        RightAlt = 32
    }

    public class ShortCut : IEquatable<ShortCut>
    {
        private readonly Keys key;
        private readonly ShiftKeys shiftKeys;
        private string hint;

        public string Hint
        {
            get
            {
                if (null == hint)
                {
                    if (shiftKeys != 0)
                    {
                        var shift = GetShiftKeysString();
                        hint = String.Join('+', shift, key.ToString());
                    }
                    else
                    {
                        hint = key.ToString();
                    }
                }

                return hint;
            }
        }

        public ShortCut(Keys key, ShiftKeys shiftKeys = 0)
        {
            this.key = key;
            this.shiftKeys = shiftKeys;
        }

        public ShortCut(Keys key, Keyboard keyboard)
        {
            this.key = key;
            shiftKeys = keyboard.ToShiftKeys();
        }

        public bool Equals(ShortCut other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return key == other.key && shiftKeys == other.shiftKeys;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is ShortCut shortCut && Equals(shortCut);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetType(), key, shiftKeys);
        }

        public override string ToString() => Hint;

        private string GetShiftKeysString()
        {
            var str = String.Empty;
            var modificators = new[] { GetAlt(shiftKeys), GetCtrl(shiftKeys), GetShift(shiftKeys) };

            for (var index = 0; index < modificators.Length; index++)
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

        private static string GetAlt(ShiftKeys modificator)
        {
            const ShiftKeys altMask = ShiftKeys.LeftAlt | ShiftKeys.RightAlt;
            var altKeys = modificator & altMask;

            if (0 != altKeys)
            {
                if (altMask == altKeys)
                {
                    return "Alt";
                }

                if (ShiftKeys.LeftAlt == altKeys)
                {
                    return "LeftAlt";
                }

                if (ShiftKeys.RightAlt == altKeys)
                {
                    return "RightAlt";
                }
            }

            return null;
        }

        private static string GetCtrl(ShiftKeys modificator)
        {
            const ShiftKeys ctrlMask = ShiftKeys.LeftCtrl | ShiftKeys.RightCtrl;
            var ctrlKeys = modificator & ctrlMask;

            if (0 != ctrlKeys)
            {
                if (ctrlMask == ctrlKeys)
                {
                    return "Ctrl";
                }

                if (ShiftKeys.LeftCtrl == ctrlKeys)
                {
                    return "LeftCtrl";
                }

                if (ShiftKeys.RightCtrl == ctrlKeys)
                {
                    return "RightCtrl";
                }
            }

            return null;
        }

        private static string GetShift(ShiftKeys modificator)
        {
            const ShiftKeys shiftMask = ShiftKeys.LeftShift | ShiftKeys.RightShift;
            var shiftKeys = modificator & shiftMask;

            if (0 != shiftKeys)
            {
                if (shiftMask == shiftKeys)
                {
                    return "Shift";
                }

                if (ShiftKeys.LeftShift == shiftKeys)
                {
                    return "LeftShift";
                }

                if (ShiftKeys.RightShift == shiftKeys)
                {
                    return "RightShift";
                }
            }

            return null;
        }

        public static bool operator ==(ShortCut a, ShortCut b)
        {
            return a?.Equals(b) ?? false;
        }

        public static bool operator !=(ShortCut a, ShortCut b)
        {
            if (ReferenceEquals(null, a))
            {
                return false == ReferenceEquals(null, b);
            }

            return ReferenceEquals(null, b) || false == a.Equals(b);
        }
    }
}