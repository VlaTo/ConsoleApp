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
        {
            Action = action;
            NewItems = Array.Empty<object>();
            OldItems = Array.Empty<object>();
            NewStartingIndex = -1;
            OldStartingIndex = -1;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems)
        {
            Action = action;
            NewItems = newItems;
            OldItems = Array.Empty<object>();
            NewStartingIndex = -1;
            OldStartingIndex = -1;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
        {
            Action = action;
            NewItems = newItems;
            OldItems = oldItems;
            NewStartingIndex = -1;
            OldStartingIndex = -1;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
        {
            Action = action;
            NewItems = newItems;
            OldItems = oldItems;
            NewStartingIndex = startingIndex;
            OldStartingIndex = startingIndex;
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, int startingIndex)
        {
            Action = action;
            NewItems = newItems;
            OldItems = Array.Empty<object>();
            NewStartingIndex = startingIndex;
            OldStartingIndex = startingIndex;
        }
    }

    public delegate void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e);

    public interface INotifyCollectionChanged
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}