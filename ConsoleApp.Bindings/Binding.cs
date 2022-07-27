using System;
using System.Linq;
using System.Reflection;
using ConsoleApp.Bindings.Extensions;

namespace ConsoleApp.Bindings
{
    public sealed class BindingTarget
    {
        public BindableObject Target
        {
            get;
        }

        public BindableProperty TargetProperty
        {
            get;
        }

        public BindingTarget(BindableProperty property, BindableObject target)
        {
            TargetProperty = property;
            Target = target;
        }
    }

    public class Binding
    {
        private object source;
        private PropertyPath propertyPath;
        private BindablePropertyChangedCallback registeredCallback;

        public object Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }

        public PropertyPath PropertyPath
        {
            get
            {
                return propertyPath;
            }
            set
            {
                propertyPath = value;
            }
        }

        internal BindingTarget BindingTarget
        {
            get;
            set;
        }

        public Binding()
        {
        }

        public void NotifyTargetChanged()
        {

        }

        public void TriggerSourceChanged()
        {
            var obj = GetBindingSource(out var propertyName);

            if (null == obj)
            {
                throw new Exception();
            }

            var property = BindableProperty.FindProperty(obj.GetType(), propertyName);

            if (null == property)
            {
                throw new Exception();
            }

            if (obj is BindableObject bindable)
            {
                OnSourceChanged(bindable, property);
            }
        }

        internal void BindSource()
        {
            var obj = GetBindingSource(out var propertyName);

            if (null == obj)
            {
                throw new Exception();
            }

            var property = BindableProperty.FindProperty(obj.GetType(), propertyName);

            if (null == property)
            {
                throw new Exception();
            }

            if (obj is BindableObject bindable)
            {
                // NOTE: save local OnSourceChanged since WeakReference will drop it immediately
                registeredCallback = OnSourceChanged;
                bindable.RegisterCallback(property, registeredCallback);
            }
        }

        private object GetBindingSource(out string propertyName)
        {
            var obj = source;
            var propertyNames = PropertyPath.ToArray();

            for (var index = 0; index < propertyNames.Length; index++)
            {
                var last = 1 == (propertyNames.Length - index);

                if (ReferenceEquals(obj, null))
                {
                    throw new Exception();
                }

                if (last)
                {
                    propertyName = propertyNames[index];
                    return obj;
                }

                var property = obj.GetProperty(propertyNames[index]);

                if (null == property)
                {
                    throw new Exception();
                }

                obj = property.GetValue(obj);
            }

            propertyName = null;

            return null;
        }

        private void OnSourceChanged(BindableObject sender, BindableProperty property)
        {
            var value = sender.GetValue(property);
            var target = BindingTarget.Target;
            target.SetValue(BindingTarget.TargetProperty, value);
        }
    }
}