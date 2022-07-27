using ConsoleApp.Bindings;
using ConsoleApp.UI.Controls;
using System;
using System.Collections.Generic;

namespace ConsoleApp.UI
{
    public enum DialogDismissReason
    {
        Unknown = -1,
        UserCancel,
        Ok
    }

    public class DialogManager : BindableObject
    {
        public static readonly BindableProperty ForegroundShadeFactorProperty;
        public static readonly BindableProperty BackgroundShadeFactorProperty;

        private readonly Dictionary<Dialog, Handler> dialogs;
        private VisualElement focusedElement;
        private Overlay overlay;
        
        public float ForegroundShadeFactor
        {
            get => (float)GetValue(ForegroundShadeFactorProperty);
            set => SetValue(ForegroundShadeFactorProperty, value);
        }
        
        public float BackgroundShadeFactor
        {
            get => (float)GetValue(BackgroundShadeFactorProperty);
            set => SetValue(BackgroundShadeFactorProperty, value);
        }

        public Screen Screen
        {
            get;
        }

        public IReadOnlyList<Dialog> Dialogs
        {
            get
            {
                var keys = dialogs.Keys;
                var array = new Dialog[keys.Count];
                var index = 0;

                foreach (var dialog in keys)
                {
                    array[index++] = dialog;
                }

                return array;
            }
        }

        public DialogManager(Screen screen)
        {
            Screen = screen;
            dialogs = new Dictionary<Dialog, Handler>();
        }

        static DialogManager()
        {
            ForegroundShadeFactorProperty = BindableProperty.Create(
                nameof(ForegroundShadeFactor),
                typeof(float),
                typeof(DialogManager),
                defaultValue: Single.NaN,
                propertyChanged: OnForegroundShadeFactorPropertyChanged
            );
            BackgroundShadeFactorProperty = BindableProperty.Create(
                nameof(BackgroundShadeFactor),
                typeof(float),
                typeof(DialogManager),
                defaultValue: Single.NaN,
                propertyChanged: OnBackgroundShadeFactorPropertyChanged
            );
        }

        public void ShowModal(Dialog dialog, Action<DialogDismissReason> callback = null)
        {
            if (null == dialog)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            if (dialogs.ContainsKey(dialog))
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

                overlay.SetBinding(Overlay.ForegroundShadeFactorProperty, new Binding
                {
                    Source = this,
                    PropertyPath = new PropertyPath(nameof(ForegroundShadeFactor))
                });
                overlay.SetBinding(Overlay.BackgroundShadeFactorProperty, new Binding
                {
                    Source = this,
                    PropertyPath = new PropertyPath(nameof(BackgroundShadeFactor))
                });

                focusedElement = Screen.FocusedElement;

                /*while (null != element)
                {
                    if (null == element.Parent)
                    {
                        if (element.FocusedElement is Layout layout)
                        {
                            backgroundElement = layout.FocusedElement;
                        }

                        break;
                    }

                    element = element.Parent;
                }*/


                Screen.Children.Add(overlay);
            }

            dialogs.Add(dialog, new Handler(callback));
            overlay.Children.Add(dialog);
        }

        public void Dismiss(Dialog dialog, DialogDismissReason reason)
        {
            if (null == dialog)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            if (dialogs.Remove(dialog, out var handler))
            {
                overlay.Children.Remove(dialog);

                if (0 == dialogs.Count)
                {
                    Screen.Children.Remove(overlay);
                    overlay = null;

                    if (null != focusedElement)
                    {
                        focusedElement.Focus();
                    }
                }

                handler.RaiseCallback(reason);
            }
        }

        protected virtual void OnForegroundShadeFactorChanged()
        {
            ;
        }

        protected virtual void OnBackgroundShadeFactorChanged()
        {
            ;
        }

        private static void OnForegroundShadeFactorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((DialogManager)sender).OnForegroundShadeFactorChanged();
        }

        private static void OnBackgroundShadeFactorPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((DialogManager)sender).OnBackgroundShadeFactorChanged();
        }

        #region Dialog callback handler

        private sealed class Handler
        {
            private readonly Action<DialogDismissReason> callback;

            public Handler(Action<DialogDismissReason> callback)
            {
                this.callback = callback;
            }

            public void RaiseCallback(DialogDismissReason reason)
            {
                if (null != callback)
                {
                    callback.Invoke(reason);
                }
            }
        }

        #endregion
    }
}