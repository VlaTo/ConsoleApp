using System;
using System.Collections.Generic;

namespace ConsoleApp.UI
{
    public delegate void PropertyChangedHandler(BindableObject sender, object newValue, object oldValue);

    public sealed class BindableProperty
    {
        private static readonly Dictionary<Type, List<BindableProperty>> bindableProperties;

        private readonly Type propertyType;
        private readonly Type ownerType;
        private readonly PropertyChangedHandler propertyChanged;

        public string Name
        {
            get;
        }

        public object DefaultValue
        {
            get;
        }

        private BindableProperty(string propertyName,
            Type propertyType,
            Type ownerType,
            object defaultValue,
            PropertyChangedHandler propertyChanged)
        {
            this.propertyType = propertyType;
            this.ownerType = ownerType;
            this.propertyChanged = propertyChanged;
            Name = propertyName;
            DefaultValue = defaultValue;
        }

        static BindableProperty()
        {
            bindableProperties = new Dictionary<Type, List<BindableProperty>>();
        }

        public static BindableProperty Create(
            string propertyName,
            Type propertyType,
            Type ownerType,
            object defaultValue = default,
            PropertyChangedHandler propertyChanged = default)
        {
            if (null == propertyName)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (String.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("", nameof(propertyName));
            }

            if (false == bindableProperties.TryGetValue(ownerType, out var collection))
            {
                collection = new List<BindableProperty>();
                bindableProperties.Add(ownerType, collection);
            }

            var index = FindPropertyIndex(collection, propertyName);

            if (-1 < index)
            {
                throw new ArgumentException("", nameof(propertyName));
            }

            var instance = new BindableProperty(
                propertyName,
                propertyType,
                ownerType,
                defaultValue,
                propertyChanged
            );

            collection.Add(instance);

            return instance;
        }

        public void RaisePropertyChanged(BindableObject sender, object newValue, object oldValue)
        {
            if (null != propertyChanged)
            {
                propertyChanged.Invoke(sender, newValue, oldValue);
            }
        }

        private static int FindPropertyIndex(IList<BindableProperty> collection, string propertyName)
        {
            for (var index = 0; index < collection.Count; index++)
            {
                var existingProperty = collection[index];

                if (String.Equals(existingProperty.Name, propertyName))
                {
                    return index;
                }
            }

            return -1;
        }
    }
}
