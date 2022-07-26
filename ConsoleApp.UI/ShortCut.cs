using System;
using ConsoleApp.UI.Extensions;
using SadConsole.Input;

namespace ConsoleApp.UI
{
    [Flags]
    public enum KeyModificator : short
    {
        LeftShift = 1,
        RightShift = 2,
        LeftCtrl = 4,
        RightCtrl = 8,
        LeftAlt = 16,
        RightAlt = 32
    }

    public class ShortCut
    {
        private readonly Keys key;
        private readonly KeyModificator keyModificator;
        private string hint;

        public string Hint
        {
            get
            {
                if (null == hint)
                {
                    var modificators = KeyModificatorExtensions.ToString(keyModificator);
                    hint = String.IsNullOrEmpty(modificators)
                        ? key.ToString()
                        : String.Join('+', modificators, key.ToString());
                }

                return hint;
            }
        }

        public ShortCut(Keys key, KeyModificator keyModificator = 0)
        {
            this.key = key;
            this.keyModificator = keyModificator;
        }

        public override string ToString() => Hint;
    }
}