using System.Collections;
using System.Collections.Generic;

namespace ConsoleApp.Bindings
{
    public sealed class PropertyPath : IEnumerable<string>
    {
        private readonly string path;

        public PropertyPath(string path)
        {
            this.path = path;
        }


        public IEnumerator<string> GetEnumerator()
        {
            var temp = path.Split('.');

            for (int index = 0; index < temp.Length; index++)
            {
                var x = temp[index];

                yield return x;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}