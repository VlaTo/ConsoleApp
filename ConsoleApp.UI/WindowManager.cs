using ConsoleApp.UI.Controls;

namespace ConsoleApp.UI
{
    public enum MoveDirection
    {
        Previous = -1,
        Next
    }

    public class WindowManager
    {
        private readonly VisualGroup owner;

        public WindowManager(VisualGroup owner)
        {
            this.owner = owner;
        }

        public void AddWindow(Window window)
        {
            owner.Children.Add(window);
        }

        public bool FocusWindow(MoveDirection direction)
        {
            var focusedElement = owner.FocusedElement;

            if (focusedElement is Window current)
            {
                var window = FindWindow(current, direction);

                if (null == window)
                {
                    return false;
                }

                if (owner.Children.Remove(window))
                {
                    owner.Children.Add(window);
                    window.Focus();

                    return true;
                }
            }

            return false;
        }

        private Window FindWindow(Window current, MoveDirection direction)
        {
            var index = owner.Children.IndexOf(current);

            switch (direction)
            {
                case MoveDirection.Previous:
                {
                    return FindPreviousWindow(index);
                }

                case MoveDirection.Next:
                {
                    return FindNextWindow(index);
                }
            }

            return null;
        }

        private Window FindPreviousWindow(int index)
        {
            var current = index - 1;

            while (true)
            {
                if (0 > current)
                {
                    current = owner.Children.Count - 1;
                }

                if (current == index)
                {
                    return null;
                }

                if (owner.Children[current] is Window window)
                {
                    if (window.IsVisible)
                    {
                        return window;
                    }
                }

                current--;
            }
        }

        private Window FindNextWindow(int index)
        {
            var current = index + 1;

            while (true)
            {
                if (owner.Children.Count <= current)
                {
                    current = 0;
                }

                if (current == index)
                {
                    return null;
                }

                if (owner.Children[current] is Window window)
                {
                    if (window.IsVisible)
                    {
                        return window;
                    }
                }

                current--;
            }
        }
    }
}