using System;
using System.Collections.Generic;

namespace ConsoleApp.Bindings
{
    public delegate void BindablePropertyChangedCallback(BindableObject sender, BindableProperty property);

    public class BindableObject
    {
        private readonly List<BindablePropertyHolder> bindableProperties;
        private readonly List<BindingHolder> bindings;
        private readonly Dictionary<BindableProperty, List<WeakReference<BindablePropertyChangedCallback>>> callbacks;

        protected BindableObject()
        {
            bindableProperties = new List<BindablePropertyHolder>(4);
            bindings = new List<BindingHolder>(4);
            callbacks = new Dictionary<BindableProperty, List<WeakReference<BindablePropertyChangedCallback>>>(2, BindablePropertyComparer.Default);
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

            RaiseTargetChanged(property);
            NotifyCallback(property);
        }

        public void SetBinding(BindableProperty property, Binding binding)
        {
            if (null == property)
            {
                throw new Exception();
            }

            var index = FindBindingIndex(property);

            if (0 > index)
            {
                index = bindings.Count;
                bindings.Add(new BindingHolder(property));
            }

            var holder = bindings[index];

            binding.BindingTarget = new BindingTarget(property, this);
            binding.BindSource();

            holder.Bindings.Add(binding);
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

        public void RegisterCallback(BindableProperty property, BindablePropertyChangedCallback callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            var collection = GetOrCreateCallbacks(property);

            collection.Add(new WeakReference<BindablePropertyChangedCallback>(callback));
        }

        private void RaiseTargetChanged(BindableProperty property)
        {
            var index = FindBindingIndex(property);

            if (0 > index)
            {
                return ;
            }

            var holder = bindings[index];

            holder.NotifyTargetChanged();
        }

        private void NotifyCallback(BindableProperty property)
        {
            if (false == callbacks.TryGetValue(property, out var collection))
            {
                return;
            }

            for (var index = 0; index < collection.Count;)
            {
                if (collection[index].TryGetTarget(out var target))
                {
                    target.Invoke(this, property);
                    index++;

                    continue;
                }

                collection.RemoveAt(index);
            }
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

        private int FindBindingIndex(BindableProperty property)
        {
            for (var index = 0; index < bindings.Count; index++)
            {
                var holder = bindings[index];

                if (false == ReferenceEquals(holder.Property, property))
                {
                    continue;
                }

                return index;
            }

            return -1;
        }

        private List<WeakReference<BindablePropertyChangedCallback>> GetOrCreateCallbacks(BindableProperty property)
        {
            if (false == callbacks.TryGetValue(property, out var collection))
            {
                collection = new List<WeakReference<BindablePropertyChangedCallback>>(4);
                callbacks.Add(property, collection);
            }

            return collection;

            /*for (var index = 0; index < callbacks.Count;)
            {
                if (false == callbacks[index].TryGetTarget(out var target))
                {
                    callbacks.RemoveAt(index);
                    continue;
                }

                if (ReferenceEquals(target, callback))
                {
                    index++;
                    continue;
                }

                return index;
            }

            return -1;*/
        }

        private sealed class BindablePropertyHolder
        {
            public BindableProperty Property
            {
                get;
            }

            public object Value
            {
                get;
                set;
            }

            public BindablePropertyHolder(BindableProperty property)
            {
                Property = property;
            }
        }

        private sealed class BindingHolder
        {
            public BindableProperty Property
            {
                get;
            }

            public List<Binding> Bindings
            {
                get;
            }

            public BindingHolder(BindableProperty property)
            {
                Property = property;
                Bindings = new List<Binding>();
            }

            public void NotifyTargetChanged()
            {
                for (var index = 0; index < Bindings.Count; index++)
                {
                    Bindings[index].NotifyTargetChanged();
                }
            }
        }
    }
}
