using System;
using System.Drawing;

namespace ConsoleApp.UI
{
    public sealed class Thickness : IEquatable<Thickness>
    {
        public static readonly Thickness Empty;

        public int Left
        {
            get;
        }

        public int Top
        {
            get;
        }

        public int Right
        {
            get;
        }

        public int Bottom
        {
            get;
        }

        public bool IsEmpty => 0 == Left && 0 == Top && 0 == Right && 0 == Bottom;

        public int HorizontalThickness => Left + Right;

        public int VerticalThickness => Top + Bottom;

        public Point Origin => new Point(Left, Top);

        public Size Size => new Size(Right, Bottom);

        public Thickness(int all)
            : this(all, all, all, all)
        {
        }

        public Thickness(int leftAndRight, int topAndBottom)
            : this(leftAndRight, topAndBottom, leftAndRight, topAndBottom)
        {
        }

        public Thickness(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        static Thickness()
        {
            Empty = new Thickness(0, 0, 0, 0);
        }

        public bool Equals(Thickness other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is Thickness other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Left, Top, Right, Bottom);
        }
    }
}