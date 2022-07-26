using ConsoleApp.Bindings;
using ConsoleApp.UI.Controls;

namespace ConsoleApp.UI
{
    public class TabManager : BindableObject
    {
        public TabManager()
        {
        }

        public Control GetNextControl(Control current)
        {
            return null;
        }

        public Control GetPreviousControl(Control current)
        {
            return null;
        }
    }
}