using System;
using System.Collections.Generic;
using ConsoleApp.Bindings;
using ConsoleApp.UI.Controls;

namespace ConsoleApp.UI
{
    public class TabManager : BindableObject
    {
        public TabManager()
        {
        }

        public Control GetNextControl(Control current)
        {
            if (null == current)
            {
                throw new ArgumentNullException(nameof(current));
            }

            var parent = current.Parent;

            if (null == parent)
            {
                return null;
            }

            var map = BuildMap(parent);
            var index = NextIndex(map, current);

            return 0 > index ? null : map[index];
        }

        public Control GetPreviousControl(Control current)
        {
            if (null == current)
            {
                throw new ArgumentNullException(nameof(current));
            }

            var parent = current.Parent;

            if (null == parent)
            {
                return null;
            }

            var map = BuildMap(parent);
            var index = NextIndex(map, current);

            return 0 > index ? null : map[index];
        }

        private static IReadOnlyList<Control> BuildMap(VisualGroup parent)
        {
            var map = new List<Control>();

            for (var index = 0; index < parent.Children.Count; index++)
            {
                var child = parent.Children[index];

                if (false == child.IsVisible)
                {
                    continue;
                }

                if (child is Control control)
                {
                    if (false == control.IsEnabled || false == control.TabStop)
                    {
                        continue;
                    }

                    AddToMap(map, control);
                }
            }

            return map;
        }

        private static int FindIndex(IReadOnlyList<Control> controls, Control control)
        {
            for (var index = 0; index < controls.Count; index++)
            {
                if (ReferenceEquals(controls[index], control))
                {
                    return index;
                }
            }

            return -1;
        }

        private static int PreviousIndex(IReadOnlyList<Control> controls, Control control)
        {
            var index = FindIndex(controls, control);

            if (0 > index)
            {
                return -1;
            }

            var position = index - 1;

            if (0 <= position)
            {
                return position;
            }

            return 0 < controls.Count ? 0 : -1;
        }

        private static int NextIndex(IReadOnlyList<Control> controls, Control control)
        {
            var index = FindIndex(controls, control);

            if (0 > index)
            {
                return -1;
            }

            var position = index + 1;

            if (position < controls.Count)
            {
                return position;
            }

            return 0 < controls.Count ? 0 : -1;
        }

        private static void AddToMap(List<Control> controls, Control control)
        {
            if (0 < controls.Count)
            {
                var tabIndex = control.TabIndex;

                for (var index = 0; index < controls.Count; index++)
                {
                    var current = controls[index];

                    if (0 > tabIndex)
                    {
                        if (0 <= current.TabIndex)
                        {
                            controls.Insert(index, control);
                            return;
                        }

                        continue;
                    }

                    if (tabIndex > current.TabIndex)
                    {
                        continue;
                    }

                    controls.Insert(index, control);

                    return;
                }
            }

            controls.Add(control);
        }
    }
}