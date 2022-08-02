using System;
using System.Collections;

namespace ConsoleApp.UI
{
    public enum NotifyCollectionChangedAction
    {
        Add,
        Remove,
        Replace,
        Move,
        Reset
    }

    public sealed class NotifyCollectionChangedEventArgs : EventArgs
    {
        public NotifyCollectionChangedAction Action
        {
            get;
        }

        public IList NewItems
        {
            get;
        }

        public int NewStartingIndex
        {
            get;
        }

        public IList OldItems
        {
            get;
        }

        public int OldStartingIndex
        {
            get;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
            : this(action, Array.Empty<object>(), Array.Empty<object>(), -1, -1)
        {
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems)
            : this(action, newItems, Array.Empty<object>(), -1, -1)
        {
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
            : this(action, newItems, oldItems, -1, -1)
        {
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
            : this(action, newItems, oldItems, startingIndex, -1)
        {
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, int startingIndex)
            : this(action, newItems, Array.Empty<object>(), startingIndex, startingIndex)
        {
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem)
            : this(action, new[] { newItem }, Array.Empty<object>(), -1, -1)
        {
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, int startingIndex)
            : this(action, new[] { newItem }, Array.Empty<object>(), startingIndex, -1)
        {
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
            : this(action, new[] { newItem }, new[] { oldItem }, -1, -1)
        {
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int startingIndex)
            : this(action, new[] { newItem }, new[] { oldItem }, startingIndex, startingIndex)
        {
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int startingIndex, int removeIndex)
            : this(action, new[] { newItem }, new[] { oldItem }, startingIndex, removeIndex)
        {
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex, int removeIndex)
        {
            Action = action;
            NewItems = newItems;
            OldItems = oldItems;
            NewStartingIndex = startingIndex;
            OldStartingIndex = removeIndex;
        }
    }

    public delegate void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e);

    public interface INotifyCollectionChanged
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}