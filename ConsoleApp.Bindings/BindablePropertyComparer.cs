using System.Collections.Generic;

namespace ConsoleApp.Bindings
{
    internal sealed class BindablePropertyComparer : IEqualityComparer<BindableProperty>
    {
        public static readonly IEqualityComparer<BindableProperty> Default;

        private BindablePropertyComparer()
        {
        }

        static BindablePropertyComparer()
        {
            Default = new BindablePropertyComparer();
        }

        public bool Equals(BindableProperty x, BindableProperty y)
        {
            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            return x.Equals(y);
        }

        public int GetHashCode(BindableProperty obj)
        {
            return obj.GetHashCode();
        }
    }
}