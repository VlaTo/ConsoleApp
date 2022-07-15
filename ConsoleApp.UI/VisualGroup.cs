﻿using SadConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ConsoleApp.UI.Extensions;
using SadConsole.Input;

namespace ConsoleApp.UI
{

    public class VisualGroup : VisualElement
    {
        private VisualElement lastFocusedElement;

        public static readonly BindableProperty LayoutManagerProperty;
        
        public IList<VisualElement> Children
        {
            get;
        }

        public VisualElement FocusedElement
        {
            get;
            private set;
        }

        public ILayoutManager LayoutManager
        {
            get => (ILayoutManager)GetValue(LayoutManagerProperty);
            set => SetValue(LayoutManagerProperty, value);
        }

        protected CellSurface RenderSurface
        {
            get;
        }

        public event EventHandler OnLayoutChanged;

        public VisualGroup()
        {
            lastFocusedElement = null;

            RenderSurface = new CellSurface(1, 1);
            Children = new ChildrenCollection(this);
            FocusedElement = null;
        }

        static VisualGroup()
        {
            LayoutManagerProperty = BindableProperty.Create(
                nameof(LayoutManager),
                typeof(ILayoutManager),
                typeof(VisualGroup),
                defaultValue: new AbsoluteLayoutManager(),
                propertyChanged: OnLayoutManagerPropertyChanged
            );
        }

        public override void Update(TimeSpan elapsed)
        {
            foreach (var child in Children)
            {
                child.Update(elapsed);
            }
        }

        public override void Arrange(Rectangle bounds)
        {
            if (!IsMeasureValid)
            {
                return;
            }

            if (IsArrangeValid)
            {
                return;
            }

            base.Arrange(bounds);

            var rectangle = new Rectangle(Point.Empty, Bounds.Size);

            LayoutManager.Arrange(Children, rectangle);

            var width = rectangle.Width;
            var height = rectangle.Height;
            
            RenderSurface.Resize(width, height, true);
            IsArrangeValid = true;
        }

        public override Size Measure(int widthConstraint, int heightConstraint)
        {
            if (false == IsMeasureValid)
            {
                //var size = base.Measure(widthConstraint, heightConstraint);

                widthConstraint -= Margin.HorizontalThickness;
                heightConstraint -= Margin.VerticalThickness;
                
                widthConstraint = ResolveConstraints(widthConstraint, Width);
                heightConstraint = ResolveConstraints(heightConstraint, Height);
                
                var sizeWithoutMargins = LayoutManager.Measure(Children, widthConstraint, heightConstraint);

                DesiredSize = new Size(
                    sizeWithoutMargins.Width, // + Margin.HorizontalThickness,
                    sizeWithoutMargins.Height // + Margin.VerticalThickness
                );

                IsMeasureValid = true;
            }

            return DesiredSize;
        }

        public override void Render(ICellSurface surface, TimeSpan elapsed)
        {
            if (IsDirty && 0 < Children.Count)
            {
                RenderChildren(elapsed);
                IsDirty = false;
            }

            RenderSurface.Copy(surface);
        }

        public override void Invalidate()
        {
            for (var index = 0; index < Children.Count; index++)
            {
                var child = Children[index];

                if (child.IsDirty)
                {
                    IsDirty = true;
                    break;
                }
            }
        }

        public virtual void InvalidateLayout()
        {
            InvalidateMeasureInternal(InvalidateTrigger.LayoutChanged);
        }

        public void FocusElement(VisualElement element)
        {
            if (ReferenceEquals(FocusedElement, element))
            {
                return;
            }

            Focus();

            if (null != FocusedElement)
            {
                FocusedElement.Leave();
            }

            FocusedElement = element;

            if (null != FocusedElement)
            {
                FocusedElement.Enter();
            }
        }

        public override bool HandleKeyDown(AsciiKey key, ModificatorKeys modificators)
        {
            for (var index = 0; index < Children.Count; index++)
            {
                if (Children[index].HandleKeyDown(key, modificators))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool HandleKeyUp(AsciiKey key, ModificatorKeys modificators)
        {
            for (var index = 0; index < Children.Count; index++)
            {
                if (Children[index].HandleKeyUp(key, modificators))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool HandleKeyPressed(AsciiKey key, ModificatorKeys modificators)
        {
            return null != FocusedElement && FocusedElement.HandleKeyPressed(key, modificators);
        }

        public override void Enter()
        {
            var element = lastFocusedElement;

            if (null == element && 0 < Children.Count)
            {
                element = Children.FirstOrDefault(child => child.IsVisible);
            }

            FocusElement(element);

            base.Enter();
        }

        public override void Leave()
        {
            lastFocusedElement = FocusedElement;

            FocusElement(null);

            base.Leave();
        }

        protected override void InvalidateMeasureInternal(InvalidateTrigger trigger)
        {
            base.InvalidateMeasureInternal(trigger);
            UpdateChildrenLayout();
        }

        protected virtual bool ShouldLayoutChildren() => false == Bounds.IsEmpty && IsVisible && Children.Any();

        protected void RenderChildren(TimeSpan elapsed)
        {
            foreach (var child in Children)
            {
                var childBounds = child.Bounds.ToRectangle();
                var rectangle = SadRogue.Primitives.Rectangle.GetIntersection(RenderSurface.Area, childBounds);

                if (rectangle.IsEmpty)
                {
                    continue;
                }

                child.Render(RenderSurface.GetSubSurface(rectangle), elapsed);
            }
        }

        protected void UpdateChildrenLayout()
        {
            if (false == ShouldLayoutChildren())
            {
                return;
            }

            var oldBounds = new Rectangle[Children.Count];

            for (var index = 0; index < oldBounds.Length; index++)
            {
                var c = Children[index];
                oldBounds[index] = c.Bounds;
            }

            //var width = Width;
            var width = Bounds.Width;
            // var height = Height;
            var height = Bounds.Height;

            var x = Padding.Left;
            var y = Padding.Top;
            var w = Math.Max(0, width - Padding.HorizontalThickness);
            var h = Math.Max(0, height - Padding.VerticalThickness);

            /*var isHeadless = CompressedLayout.GetIsHeadless(this);
            var headlessOffset = CompressedLayout.GetHeadlessOffset(this);
            for (var i = 0; i < LogicalChildrenInternal.Count; i++)
                CompressedLayout.SetHeadlessOffset((VisualElement)LogicalChildrenInternal[i],
                    isHeadless ? new Point(headlessOffset.X + Bounds.X, headlessOffset.Y + Bounds.Y) : new Point());
            */

            //lastLayoutSize = new Size(width, height);

            LayoutChildren(x, y, w, h);

            for (var index = 0; index < oldBounds.Length; index++)
            {
                var oldBound = oldBounds[index];
                var newBound = Children[index].Bounds;

                if (oldBound == newBound)
                {
                    continue;
                }

                OnLayoutChanged?.Invoke(this, EventArgs.Empty);

                return;
            }
        }

        protected void LayoutChildren(int x, int y, int width, int height)
        {
            var size = LayoutManager.Measure(Children, width, height);
            var bounds = new Rectangle(Padding.Left, Padding.Top, size.Width, size.Height);
            LayoutManager.Arrange(Children, bounds);
        }

        protected virtual void OnChildAdded(VisualElement child)
        {
            if (ShouldInvalidateOnChildAdded(child))
            {
                InvalidateLayout();
                //UpdateChildrenLayout();
            }

            child.MeasureInvalidated += OnChildMeasureInvalidated;

            child.Focus();
        }

        protected virtual void OnChildRemoved(VisualElement child)
        {
            child.MeasureInvalidated -= OnChildMeasureInvalidated;

            if (ShouldInvalidateOnChildRemoved(child))
            {
                InvalidateLayout();
                //UpdateChildrenLayout();
            }
        }

        protected virtual bool ShouldInvalidateOnChildAdded(VisualElement child) => true;

        protected virtual bool ShouldInvalidateOnChildRemoved(VisualElement child) => true;

        protected virtual void OnChildMeasureInvalidated(object sender, EventArgs _)
        {
            UpdateChildrenLayout();
        }

        protected override void OnWidthChanged() => ResizeSurface();

        protected override void OnHeightChanged() => ResizeSurface();

        private void DoChildAdded(int index, VisualElement child)
        {
            child.Parent = this;
            OnChildAdded(child);
        }

        private void DoChildRemoved(int index, VisualElement child)
        {
            OnChildRemoved(child);
            child.Parent = null;
        }

        private void ResizeSurface()
        {
            var bounds = Bounds;

            if (false == bounds.IsEmpty)
            {
                RenderSurface.Resize(bounds.Width, bounds.Height, bounds.Width, bounds.Height, true);
            }
        }

        private void OnLayoutManagerChanged()
        {
            Invalidate();
        }

        private static void OnLayoutManagerPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((VisualGroup)sender).OnLayoutManagerChanged();
        }

        /// <summary>
        /// Child controls collection
        /// </summary>
        internal sealed class ChildrenCollection : IList<VisualElement>, IReadOnlyList<VisualElement>
        {
            private readonly ArrayList collection;
            private readonly VisualGroup owner;
            private int updates;

            public int Count => collection.Count;

            public bool IsReadOnly => collection.IsReadOnly;

            public VisualElement this[int index]
            {
                get
                {
                    var element = collection[index];
                    return (VisualElement)element;
                }
                set
                {
                    var element = (VisualElement)collection[index];

                    collection[index] = value;

                    owner.DoChildRemoved(index, element);
                    owner.DoChildRemoved(index, value);
                }
            }

            public ChildrenCollection(VisualGroup owner)
            {
                collection = new ArrayList();
                updates = 0;
                this.owner = owner;
            }

            public void Add(VisualElement item)
            {
                if (null == item)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                var index = collection.Count;

                collection.Insert(index, item);
                updates++;

                owner.DoChildAdded(index, item);
            }

            public void Clear()
            {
                updates++;

                for (var index = collection.Count - 1; index >= 0; index--)
                {
                    var item = (VisualElement)collection[index];

                    collection.RemoveAt(index);

                    owner.DoChildRemoved(index, item);
                }
            }

            public bool Contains(VisualElement item) => collection.Contains(item);

            public void CopyTo(VisualElement[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);

            public IEnumerator<VisualElement> GetEnumerator() => new ChildEnumerator(this);

            public bool Remove(VisualElement item)
            {
                if (null == item)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                var index = collection.IndexOf(item);

                if (0 > index)
                {
                    return false;
                }

                collection.RemoveAt(index);
                updates++;

                owner.DoChildRemoved(index, item);

                return true;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public int IndexOf(VisualElement item)
            {
                if (null == item)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                return collection.IndexOf(item);
            }

            public void Insert(int index, VisualElement item)
            {
                if (null == item)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                if (0 > index || collection.Count < index)
                {
                    throw new IndexOutOfRangeException();
                }

                collection.Insert(index, item);
                updates++;

                owner.DoChildAdded(index, item);
            }

            public void RemoveAt(int index)
            {
                if (0 > index || collection.Count <= index)
                {
                    throw new IndexOutOfRangeException();
                }

                var child = (VisualElement)collection[index];

                collection.RemoveAt(index);
                updates++;

                owner.DoChildRemoved(index, child);
            }

            /// <summary>
            /// 
            /// </summary>
            private sealed class ChildEnumerator : IEnumerator<VisualElement>
            {
                private readonly ChildrenCollection owner;
                private int position;
                private int updates;
                private bool disposed;

                public VisualElement Current
                {
                    get;
                    private set;
                }

                object IEnumerator.Current => Current;

                public ChildEnumerator(ChildrenCollection owner)
                {
                    this.owner = owner;
                    updates = owner.updates;
                    position = -1;
                }

                public void Dispose()
                {
                    Dispose(true);
                }

                public bool MoveNext()
                {
                    if (0 > position)
                    {
                        if (0 < owner.collection.Count)
                        {
                            position = 0;
                            Current = (VisualElement)owner.collection[position];

                            return true;
                        }

                        Current = null;

                        return false;
                    }

                    if (updates != owner.updates)
                    {
                        throw new Exception();
                    }

                    if ((owner.collection.Count - 1) <= position)
                    {
                        Current = null;

                        return false;
                    }

                    Current = (VisualElement)owner.collection[++position];

                    return true;
                }

                public void Reset()
                {
                    updates = owner.updates;
                    position = -1;
                }

                private void Dispose(bool dispose)
                {
                    if (disposed)
                    {
                        return;
                    }

                    try
                    {
                        if (dispose)
                        {

                        }
                    }
                    finally
                    {
                        disposed = true;
                    }
                }
            }
        }
    }
}