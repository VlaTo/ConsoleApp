using System;
using System.Collections.Generic;

namespace ConsoleApp.UI
{
    public class BindableObject
    {
        private readonly List<BindablePropertyHolder> bindableProperties;

        protected BindableObject()
        {
            bindableProperties = new List<BindablePropertyHolder>(4);
        }

        public void SetValue(BindableProperty property, object value)
        {
            if (null == property)
            {
                throw new Exception();
            }

            var wasAdded = false;
            var index = FindPropertyIndex(property);

            if (0 > index)
            {
                index = bindableProperties.Count;
                wasAdded = true;
                bindableProperties.Add(new BindablePropertyHolder(property));
            }

            var holder = bindableProperties[index];

            if (Convert.Equals(holder.Value, value))
            {
                return;
            }

            var existing = wasAdded ? property.DefaultValue : holder.Value;

            holder.Value = value;

            property.RaisePropertyChanged(this, value, existing);
        }

        public object GetValue(BindableProperty property)
        {
            if (null == property)
            {
                throw new Exception();
            }

            var index = FindPropertyIndex(property);

            if (0 > index)
            {
                return property.DefaultValue;
            }

            var holder = bindableProperties[index];

            return holder.Value;
        }

        public bool HasValue(BindableProperty property)
        {
            if (null == property)
            {
                throw new Exception();
            }

            return -1 < FindPropertyIndex(property);
        }

        private int FindPropertyIndex(BindableProperty property)
        {
            for (var index = 0; index < bindableProperties.Count; index++)
            {
                var holder = bindableProperties[index];

                if (false == ReferenceEquals(holder.Property, property))
                {
                    continue;
                }

                return index;
            }

            return -1;
        }

        private sealed class BindablePropertyHolder
        {
            public BindableProperty Property { get; }

            public object Value { get; set; }

            public BindablePropertyHolder(BindableProperty property)
            {
                Property = property;
            }
        }
    }
}
