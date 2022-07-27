using System;
using System.Drawing;
using SadConsole;
using SadConsole.Input;

namespace ConsoleApp.UI.Controls
{
    public sealed class DialogDismissEventArgs : EventArgs
    {
        public DialogDismissReason Reason
        {
            get;
        }

        public DialogDismissEventArgs(DialogDismissReason reason)
        {
            Reason = reason;
        }
    }

    public class Dialog : Window
    {
        public event EventHandler Dismissed;

        public DialogManager DialogManager
        {
            get
            {
                var application = ConsoleApplication.Instance;
                return application.DialogManager;
            }
        }

        public Dialog()
        {
        }

        public void ShowModal()
        {
            DialogManager.ShowModal(this, OnDismiss);
        }

        public void Dismiss(DialogDismissReason reason)
        {
            DialogManager.Dismiss(this, reason);
        }

        public override bool HandleKeyPressed(AsciiKey key, ModificatorKeys modificators)
        {
            return HandleInteractionKeys(key, modificators) || base.HandleKeyPressed(key, modificators);
        }

        public override Size Measure(int widthConstraint, int heightConstraint)
        {
            return base.Measure(widthConstraint, heightConstraint);
        }

        protected virtual void OnDismiss(DialogDismissReason reason)
        {
            RaiseDismissed(new DialogDismissEventArgs(reason));
        }

        private bool HandleInteractionKeys(AsciiKey key, ModificatorKeys modificators)
        {
            if (Keys.Escape == key && modificators.IsEmpty)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var index = 0; index < Children.Count; index++)
                {
                    if (Children[index] is Button button && button.IsCancel)
                    {
                        Dismiss(DialogDismissReason.UserCancel);
                    }
                }

                return true;
            }

            if (Keys.Enter == key && modificators.IsEmpty)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var index = 0; index < Children.Count; index++)
                {
                    if (Children[index] is Button button && button.IsDefault)
                    {
                        Dismiss(DialogDismissReason.Ok);
                    }
                }

                return true;
            }

            return false;
        }

        private void RaiseDismissed(EventArgs e)
        {
            var handler = Dismissed;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }
    }
}