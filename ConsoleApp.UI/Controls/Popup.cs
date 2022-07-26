using System;

namespace ConsoleApp.UI.Controls
{
    public class Popup : Window
    {
        public event EventHandler Dismissed;

        public PopupManager PopupManager
        {
            get
            {
                var application = ConsoleApplication.Instance;
                return application.PopupManager;
            }
        }

        public void Show()
        {
            PopupManager.Show(this, OnDismiss);
        }

        public void Dismiss()
        {
            PopupManager.Dismiss(this);

        }

        protected virtual void OnDismiss()
        {
            RaiseDismissed(EventArgs.Empty);
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