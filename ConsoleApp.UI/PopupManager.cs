using ConsoleApp.Bindings;
using ConsoleApp.UI.Controls;
using System;
using System.Collections.Generic;

namespace ConsoleApp.UI
{
    public class PopupManager : BindableObject
    {
        private readonly Dictionary<Popup, Handler> popups;
        private Overlay overlay;

        public Screen Screen
        {
            get;
        }

        public IReadOnlyList<Popup> Popups
        {
            get
            {
                var keys = popups.Keys;
                var array = new Popup[keys.Count];
                var index = 0;

                foreach (var popup in keys)
                {
                    array[index++] = popup;
                }

                return array;
            }
        }

        public PopupManager(Screen screen)
        {
            Screen = screen;
            popups = new Dictionary<Popup, Handler>();
        }

        public void Show(Popup popup, Action callback = null)
        {
            if (null == popup)
            {
                throw new ArgumentNullException(nameof(popup));
            }

            if (popups.ContainsKey(popup))
            {
                throw new InvalidOperationException();
            }

            if (null == overlay)
            {
                overlay = new Overlay
                {
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                var application = ConsoleApplication.Instance;
                var index = Screen.Children.IndexOf(application.Container);

                if (0 > index)
                {
                    Screen.Children.Add(overlay);
                }
                else
                {
                    var position = index + 1;
                    Screen.Children.Insert(position, overlay);
                }
            }

            popups.Add(popup, new Handler(callback));
            overlay.Children.Add(popup);
        }

        public void Dismiss(Popup popup)
        {
            if (null == popup)
            {
                throw new ArgumentNullException(nameof(popup));
            }

            if (popups.Remove(popup, out var handler))
            {
                overlay.Children.Remove(popup);

                if (0 == popups.Count)
                {
                    Screen.Children.Remove(overlay);
                    overlay = null;
                }

                handler.RaiseCallback();
            }
        }

        private sealed class Handler
        {
            private readonly Action callback;

            public Handler(Action callback)
            {
                this.callback = callback;
            }

            public void RaiseCallback()
            {
                if (null != callback)
                {
                    callback.Invoke();
                }
            }
        }
    }
}