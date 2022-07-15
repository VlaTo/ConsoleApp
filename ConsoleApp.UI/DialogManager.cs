using ConsoleApp.UI.Controls;

namespace ConsoleApp.UI
{
    public class DialogManager : BindableObject
    {
        public static readonly BindableProperty BackgroundShadeFactorProperty;

        private Overlay overlay;
        
        public float BackgroundShadeFactor
        {
            get => (float)GetValue(BackgroundShadeFactorProperty);
            set => SetValue(BackgroundShadeFactorProperty, value);
        }

        public Screen Screen
        {
            get;
        }

        public DialogManager(Screen screen)
        {
            Screen = screen;
        }

        public void ShowModal(Popover popover)
        {
            if (null == overlay)
            {
                overlay = new Overlay
                {
                    //BackgroundShadeFactor = 0.3f,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                Screen.Children.Add(overlay);
            }

            overlay.Children.Add(popover);
        }
    }
}