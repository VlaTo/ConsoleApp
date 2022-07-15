using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp.UI
{
    public enum ItemsChangeAction
    {
        Add,
        Insert,
        Replace,
        Remove,
        Clear
    }


    public class ItemsList<T> : IList<T>
    {
        #region ItemsChangedEventArgs

        public sealed class ItemsListChangedEventArgs : EventArgs
        {
            public ItemsChangeAction Action
            {
                get;
            }

            public int Index
            {
                get;
            }

            public T NewItem
            {
                get;
            }

            public T OldItem
            {
                get;
            }

            private ItemsListChangedEventArgs(ItemsChangeAction action, int index, T newItem, T oldItem = default)
            {
                Action = action;
                Index = index;
                NewItem = newItem;
                OldItem = oldItem;
            }

            public static ItemsListChangedEventArgs Clear() => new ItemsListChangedEventArgs(ItemsChangeAction.Clear, -1, newItem: default);

            public static ItemsListChangedEventArgs Insert(int index, T item) => new ItemsListChangedEventArgs(ItemsChangeAction.Insert, index, newItem: item);

            public static ItemsListChangedEventArgs Added(int index, T item) => new ItemsListChangedEventArgs(ItemsChangeAction.Add, index, newItem: item);

            public static ItemsListChangedEventArgs Remove(int index, T item) => new ItemsListChangedEventArgs(ItemsChangeAction.Remove, index, newItem: default, oldItem: item);

            public static ItemsListChangedEventArgs Replace(int index, T newItem, T oldItem) => new ItemsListChangedEventArgs(ItemsChangeAction.Replace, index, newItem, oldItem);
        }
        
        #endregion
        
        private const int NoIndex = -1;

        private readonly ArrayList items;
        private readonly Action<ItemsListChangedEventArgs> notifier;
        private int updatesCount;

        public int Count => items.Count;

        public bool IsReadOnly => items.IsReadOnly;

        public T this[int index]
        {
            get
            {
                if (0 > index || items.Count <= index)
                {
                    throw new IndexOutOfRangeException();
                }

                return (T) items[index];
            }

            set
            {
                if (0 > index || items.Count < index)
                {
                    throw new IndexOutOfRangeException();
                }

                var oldItem = items.Count > index ? items[index] : null;

                items[index] = value;
                
                updatesCount++;

                notifier?.Invoke(ItemsListChangedEventArgs.Replace(index, value, (T)oldItem));
            }
        }

        public ItemsList(Action<ItemsListChangedEventArgs> notifier)
        {
            items = new ArrayList();
            this.notifier = notifier;
        }

        public IEnumerator<T> GetEnumerator() => new ItemsEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var index = items.Add(item);
            updatesCount++;

            notifier?.Invoke(ItemsListChangedEventArgs.Added(index, item));
        }

        public void Clear()
        {
            items.Clear();
            updatesCount++;

            notifier?.Invoke(ItemsListChangedEventArgs.Clear());
        }

        public bool Contains(T item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var index = items.IndexOf(item);

            if (NoIndex == index)
            {
                return false;
            }

            items.RemoveAt(index);

            updatesCount++;

            notifier?.Invoke(ItemsListChangedEventArgs.Remove(index, item));

            return true;
        }

        public int IndexOf(T item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (null == item)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (0 > index || items.Count < index)
            {
                throw new IndexOutOfRangeException();
            }

            updatesCount++;

            items.Insert(index, item);

            notifier?.Invoke(ItemsListChangedEventArgs.Insert(index, item));
        }

        public void RemoveAt(int index)
        {
            if (0 > index || items.Count <= index)
            {
                throw new IndexOutOfRangeException();
            }

            updatesCount++;

            var item = this[index];

            items.RemoveAt(index);

            notifier?.Invoke(ItemsListChangedEventArgs.Remove(index, item));
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class ItemsEnumerator : IEnumerator<T>
        {
            private readonly int updatesCount;
            private ItemsList<T> owner;
            private T[] items;
            private bool disposed;
            private int index;
            private T current;

            public T Current
            {
                get
                {
                    EnsureNotDisposed();
                    EnsureNotModified();

                    return current;
                }
            }

            object IEnumerator.Current => Current;

            public ItemsEnumerator(ItemsList<T> owner)
            {
                this.owner = owner;

                updatesCount = owner.updatesCount;
                index = NoIndex;
                items = owner.items.Cast<T>().ToArray();
                current = default;
            }

            public bool MoveNext()
            {
                EnsureNotDisposed();

                if (NoIndex == index)
                {
                    EnsureNotModified();

                    if (0 == items.Length)
                    {
                        return false;
                    }

                    index = 0;
                    current = items[index];

                    return true;
                }

                if (items.Length <= index)
                {
                    return false;
                }

                if (items.Length == ++index)
                {
                    return false;
                }

                EnsureNotModified();

                current = items[index];

                return true;
            }

            public void Reset()
            {
                EnsureNotDisposed();
                EnsureNotModified();
                
                index = NoIndex;
                current = default;
            }

            public void Dispose()
            {
                if (false == disposed)
                {
                    Dispose(true);
                }
            }

            private void EnsureNotDisposed()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(null);
                }
            }

            private void EnsureNotModified()
            {
                if (updatesCount != owner.updatesCount)
                {
                    throw new InvalidOperationException();
                }
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
                        items = null;
                        owner = null;
                        current = default;
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