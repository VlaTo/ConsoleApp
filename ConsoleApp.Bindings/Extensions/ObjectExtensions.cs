using System.Reflection;

namespace ConsoleApp.Bindings.Extensions
{
    internal static class ObjectExtensions
    {
        public static PropertyInfo GetProperty(this object source, string propertyName)
        {
            return source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        }
    }
}
