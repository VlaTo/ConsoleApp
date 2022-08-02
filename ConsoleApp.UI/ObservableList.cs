using System;
using System.Collections;
using System.Collections.Generic;

namespace ConsoleApp.UI
{
    public class ObservableList<T> : IList<T>, INotifyCollectionChanged
    {
        private readonly ArrayList list;
        private int version;

        public int Count => list.Count;

        public bool IsReadOnly => list.IsReadOnly;

        public T this[int index]
        {
            get
            {
                var item = list[index];
                return (T)item;
            }
            set
            {
                var oldItem = list[index];

                list[index] = value;
                version++;

                var args = new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace,
                    value,
                    oldItem,
                    index
                );

                RaiseCollectionChanged(args);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableList(int capacity)
        {
            list = new ArrayList(capacity);
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            var index = list.Add(item);

            version++;

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void Clear()
        {
            list.Clear();
            
            version++;
            
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var index = list.IndexOf(item);

            if (0 > index)
            {
                return false;
            }

            list.RemoveAt(index);
            
            version++;

            var args = new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove,
                Array.Empty<object>(),
                new object[] { item },
                -1,
                index
            );

            RaiseCollectionChanged(args);

            return true;
        }

        public int IndexOf(T item)
        {
            var index = list.IndexOf(item);
            return index;
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);

            version++;
            
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);

            RaiseCollectionChanged(args);
        }

        public void RemoveAt(int index)
        {
            var item = list[index];

            list.RemoveAt(index);

            version++;

            var args = new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove,
                Array.Empty<object>(),
                new[] { item },
                -1,
                index
            );

            RaiseCollectionChanged(args);
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }

        private sealed class Enumerator : IEnumerator<T>
        {
            private readonly int version;
            private ObservableList<T> owner;
            private int index;
            private bool disposed;

            public T Current
            {
                get
                {
                    EnsureNotDisposed();
                    EnsureNotModified();

                    if (0 > index || owner.list.Count <= index)
                    {
                        throw new IndexOutOfRangeException();
                    }

                    return (T)owner.list[index];
                }
            }

            object IEnumerator.Current => Current;

            public Enumerator(ObservableList<T> owner)
            {
                this.owner = owner;
                version = owner.version;
                index = -1;
            }

            public bool MoveNext()
            {
                EnsureNotDisposed();
                EnsureNotModified();

                if (0 > index)
                {
                    if (0 < owner.list.Count)
                    {
                        index = 0;
                        return true;
                    }

                    return false;
                }

                var newIndex = index + 1;

                if (newIndex < owner.list.Count)
                {
                    index = newIndex;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                index = -1;
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                Dispose(true);
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
                        owner = null;
                        index = -1;
                    }
                }
                finally
                {
                    disposed = true;
                }
            }

            private void EnsureNotDisposed()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
            }

            private void EnsureNotModified()
            {
                if (version != owner.version)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}