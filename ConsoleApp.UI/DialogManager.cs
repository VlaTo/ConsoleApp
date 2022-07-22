using ConsoleApp.Bindings;
using ConsoleApp.UI.Controls;
using System;
using System.Collections.Generic;

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

        public IList<Window> Windows
        {
            get;
        }

        public DialogManager(Screen screen)
        {
            Screen = screen;
            Windows = new ItemsList<Window>(OnWindowsListChanged);
        }

        static DialogManager()
        {
            BackgroundShadeFactorProperty = BindableProperty.Create(
                nameof(BackgroundShadeFactor),
                typeof(float),
                typeof(DialogManager),
                defaultValue: Single.NaN,
                propertyChanged: OnBackgroundShadeFactorPropertyChanged
            );
        }

        public void ShowModal(Window window)
        {
            if (null == overlay)
            {
                overlay = new Overlay
                {
                    BackgroundShadeFactor = BackgroundShadeFactor,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                Screen.Children.Add(overlay);
            }

            Windows.Add(window);
            overlay.Children.Add(window);
        }

        protected virtual void OnBackgroundShadeFactorChanged()
        {
            ;
        }

        private void OnWindowsListChanged(ItemsList<Window>.ItemsListChangedEventArgs e)
        {
            ;
        }

        private static void OnBackgroundShadeFactorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((DialogManager)sender).OnBackgroundShadeFactorChanged();
        }
    }
}