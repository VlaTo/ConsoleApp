using System;
using System.Collections;
using System.Collections.Generic;

namespace ConsoleApp.UI.Controls
{
    public class CollectionView : VisualGroup
    {
        public static readonly BindableProperty ItemsProperty;

        private ItemsAdapter adapter;

        public IEnumerable Items
        {
            get => (IEnumerable)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public IReadOnlyList<object> List => adapter;

        public event EventHandler ItemsChanged;

        public CollectionView()
        {
            adapter = ItemsAdapter.Empty;
        }

        static CollectionView()
        {
            ItemsProperty = BindableProperty.Create(
                nameof(Items),
                typeof(IEnumerable),
                typeof(CollectionView),
                defaultValue: null,
                propertyChanged: OnItemsPropertyChanged
            );
        }

        protected virtual void OnItemsChanged()
        {
            if (null != adapter)
            {
                adapter.Dispose();
                adapter = null;
            }

            if (Items is IList list)
            {
                adapter = ItemsAdapter.FromList(list);
            }
            else if (Items is IEnumerable enumerable)
            {
                adapter = ItemsAdapter.FromEnumerable(enumerable);
            }
            else
            {
                adapter = ItemsAdapter.Empty;
            }

            Invalidate();
            RaiseItemsChanged(EventArgs.Empty);
        }

        private void RaiseItemsChanged(EventArgs e)
        {
            var handler = ItemsChanged;

            if (null != handler)
            {
                handler.Invoke(this, e);
            }
        }

        private static void OnItemsPropertyChanged(BindableObject sender, object newvalue, object oldvalue)
        {
            ((CollectionView)sender).OnItemsChanged();
        }

        private abstract class ItemsAdapter : IReadOnlyList<object>, IDisposable
        {
            public static readonly EmptyAdapter Empty;

            private bool disposed;

            public abstract int Count
            {
                get;
            }

            public abstract object this[int index]
            {
                get;
            }

            static ItemsAdapter()
            {
                Empty = new EmptyAdapter();
            }

            protected ItemsAdapter()
            {
                disposed = false;
            }

            public abstract IEnumerator<object> GetEnumerator();

            public void Dispose()
            {
                if (false == disposed)
                {
                    Dispose(true);
                }
            }

            public static ListAdapter FromList(IList list)
            {
                var adapter = new ListAdapter(list);

                if (list is INotifyCollectionChanged notify)
                {
                    adapter.Subscribe(notify);
                }

                return adapter;
            }

            public static EnumerableAdapter FromEnumerable(IEnumerable enumerable)
            {
                var adapter = new EnumerableAdapter(enumerable);
                return adapter;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            protected abstract void DoDispose();

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
                        DoDispose();
                    }
                }
                finally
                {
                    disposed = true;
                }
            }
        }

        private sealed class EmptyAdapter : ItemsAdapter
        {
            public override int Count => 0;

            public override object this[int index]
            {
                get
                {
                    throw new InvalidOperationException();
                }
            }

            public override IEnumerator<object> GetEnumerator() => new Enumerator();
            
            protected override void DoDispose()
            {
                
            }

            private sealed class Enumerator : IEnumerator<object>
            {
                public object Current
                {
                    get
                    {
                        throw new InvalidOperationException();
                    }
                }

                public bool MoveNext() => false;

                public void Reset()
                {
                    throw new InvalidOperationException();
                }

                public void Dispose()
                {
                    
                }
            }
        }

        private sealed class EnumerableAdapter : ItemsAdapter
        {
            private readonly ArrayList _source;

            public override int Count => _source.Count;

            public override object this[int index] => _source[index];

            public EnumerableAdapter(IEnumerable source)
            {
                _source = new ArrayList();

                foreach (var item in source)
                {
                    _source.Add(item);
                }
            }

            public override IEnumerator<object> GetEnumerator() => new Enumerator(_source);

            protected override void DoDispose()
            {
                throw new NotImplementedException();
            }

            private sealed class Enumerator : IEnumerator<object>
            {
                private ArrayList source;
                private bool disposed;
                private int index;
                private object current;

                public object Current
                {
                    get
                    {
                        EnsureNotDisposed();
                        return current;
                    }
                }

                object IEnumerator.Current => Current;

                public Enumerator(ArrayList source)
                {
                    this.source = source;
                    index = -1;
                    current = null;
                }

                public bool MoveNext()
                {
                    EnsureNotDisposed();

                    if (0 > index)
                    {
                        if (0 < source.Count)
                        {
                            index = 0;
                            current = source[index];

                            return true;
                        }

                        return false;
                    }

                    var nextIndex = index + 1;

                    if (source.Count > nextIndex)
                    {
                        index = nextIndex;
                        current = source[index];

                        return true;
                    }

                    return false;
                }

                public void Reset()
                {
                    EnsureNotDisposed();

                    index = -1;
                    current = null;
                }

                public void Dispose()
                {
                    if (false == disposed)
                    {
                        Dispose(true);
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
                            source = null;
                            current = null;
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
                        throw new ObjectDisposedException(null);
                    }
                }
            }
        }

        private sealed class ListAdapter : ItemsAdapter
        {
            private INotifyCollectionChanged _notificator;
            private ArrayList _source;
            private int version;

            public override int Count => _source.Count;

            public override object this[int index] => _source[index];

            public ListAdapter(IList source)
            {
                _source = new ArrayList(source);
                _notificator = null;
                version = 0;
            }

            public void Subscribe(INotifyCollectionChanged notificator)
            {
                if (null != _notificator)
                {
                    _notificator.CollectionChanged -= OnCollectionChanged;
                }

                _notificator = notificator;

                if (null != _notificator)
                {
                    notificator.CollectionChanged += OnCollectionChanged;
                }
            }

            public override IEnumerator<object> GetEnumerator() => new Enumerator(this);

            protected override void DoDispose()
            {
                if (null != _notificator)
                {
                    _notificator.CollectionChanged -= OnCollectionChanged;
                }

                _notificator = null;
                _source = null;
            }

            private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                version++;
            }

            private sealed class Enumerator : IEnumerator<object>
            {
                private ListAdapter adapter;
                private readonly int version;
                private bool disposed;
                private object current;
                private int index;

                public object Current
                {
                    get
                    {
                        EnsureNotDisposed();
                        EnsureNotModified();

                        return current;
                    }
                }

                object IEnumerator.Current => Current;

                public Enumerator(ListAdapter adapter)
                {
                    this.adapter = adapter;
                    index = -1;
                    current = null;
                    version = adapter.version;
                }

                public bool MoveNext()
                {
                    EnsureNotDisposed();
                    EnsureNotModified();

                    if (0 > index)
                    {
                        if (0 < adapter.Count)
                        {
                            index = 0;
                            current = adapter[index];

                            return true;
                        }

                        return false;
                    }

                    var nextIndex = index + 1;

                    if (adapter.Count > nextIndex)
                    {
                        index = nextIndex;
                        current = adapter[index];

                        return true;
                    }

                    return false;
                }

                public void Reset()
                {
                    EnsureNotDisposed();
                    EnsureNotModified();

                    index = -1;
                    current = null;
                }

                public void Dispose()
                {
                    if (false == disposed)
                    {
                        Dispose(true);
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
                            adapter = null;
                            current = null;
                        }
                    }
                    finally
                    {
                        disposed = true;
                    }
                }

                private void EnsureNotModified()
                {
                    if (version != adapter.version)
                    {
                        throw new InvalidOperationException();
                    }
                }

                private void EnsureNotDisposed()
                {
                    if (disposed)
                    {
                        throw new ObjectDisposedException(null);
                    }
                }
            }
        }
    }
}