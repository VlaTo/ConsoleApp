using ConsoleApp.Bindings;
using NVorbis.Ogg;
using SadConsole.Input;

namespace ConsoleApp.UI.Controls
{
    public class Control : VisualElement
    {
        public static readonly BindableProperty TabStopProperty;
        public static readonly BindableProperty TabIndexProperty;
        public static readonly BindableProperty TabManagerProperty;

        public bool TabStop
        {
            get => (bool)GetValue(TabStopProperty);
            set => SetValue(TabStopProperty, value);
        }

        public int TabIndex
        {
            get => (int)GetValue(TabIndexProperty);
            set => SetValue(TabIndexProperty, value);
        }

        public TabManager TabManager
        {
            get => (TabManager)GetValue(TabManagerProperty);
            set => SetValue(TabManagerProperty, value);
        }

        public Control()
        {
        }

        static Control()
        {
            TabStopProperty = BindableProperty.Create(
                nameof(TabStop),
                typeof(bool),
                ownerType: typeof(Control),
                defaultValue: true
            );
            TabIndexProperty = BindableProperty.Create(
                nameof(TabIndex),
                typeof(int),
                ownerType: typeof(Control),
                defaultValue: -1
            );
            TabManagerProperty = BindableProperty.Create(
                nameof(TabManager),
                typeof(TabManager),
                ownerType: typeof(Control),
                defaultValue: new TabManager()
            );
        }

        public override bool HandleKeyPressed(AsciiKey key, ModificatorKeys modificators)
        {
            if (Keys.Tab == key)
            {
                var manager = TabManager;

                if (null == manager)
                {
                    return false;
                }

                if (modificators.IsEmpty)
                {
                    var control = manager.GetNextControl(this);

                    if (null != control && false == ReferenceEquals(this, control))
                    {
                        control.Focus();
                    }
                }

                if (modificators.IsShiftPressed)
                {
                    var control = manager.GetPreviousControl(this);

                    if (null != control && false == ReferenceEquals(this, control))
                    {
                        control.Focus();
                    }
                }

                return true;
            }

            return false;
        }
    }
}