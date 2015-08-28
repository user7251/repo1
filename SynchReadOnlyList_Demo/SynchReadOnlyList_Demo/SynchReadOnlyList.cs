using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
#if SYNCH_READ_ONLY_LIST
namespace com.GitHub.user7251 {   
    #if _
    - SynchReadOnlyList is a synchronized read-only list.
    - Microsoft's ReadOnlyCollection (ROC) does not synchronize the ROC with other
      clients of the IList that the ROC references. ROC has a syncRoot that it gets from 
      ICollection.SyncRoot, but sometimes ROC references a List, which does not have a ctor 
      that takes a syncRoot, so List's ICollection.SyncRoot property will return null.
    - Microsoft's SynchronizedReadOnlyCollection (SROC) makes a copy of the primary List.  
      SynchReadOnlyList hold a reference to the original list.
    - SROC uses lock() for synchronization.  SynchReadOnlyList uses a ReaderWriterLockSlim.
    - ConcurrentBag is not read-only.
    - ConcurrentBag copies the original list.  SynchReadOnlyList references the original list, 
      so it reflects changes in the original list, and it uses less memory.
    - To develop SynchReadOnlyList, I started with the code for SynchronizedReadOnlyCollection.
    #endif
    [System.Runtime.InteropServices.ComVisible(false)]
    public class SynchReadOnlyList<T> : IList<T>, IList {
        private readonly RwLockList<T> _items; // is never null
        //
        public SynchReadOnlyList ( RwLockList<T> pitems ) { 
            if ( _items == null ) throw new ArgumentException("RwLockList<T> items == null");
            _items = pitems; } 
        public SynchReadOnlyList ( IEnumerable<T> list )
        {
            if (list == null) throw new ArgumentNullException("list");  
            this._items = new RwLockList<T>(list);
        }  
        public SynchReadOnlyList ( params T[] list ) 
        { 
            if (list == null) throw new ArgumentNullException("list"); 
            this._items = new RwLockList<T>(list.Length); 
            for (int i=0; i<list.Length; i++) this._items.Add(list[i]);
        }        
        internal SynchReadOnlyList ( RwLockList<T> list, bool makeCopy )
        {
            if (list == null) throw new ArgumentNullException("list");  
            if (makeCopy) this._items = new RwLockList<T>(list);
            else this._items = list;
        } 
        //
        public ReaderWriterLockSlim RwLock { get { return _items.RwLock; } }  
        public int Count
        { 
            get { 
                try { _items.RwLock.EnterReadLock();
                    return this._items.Count; }
                finally { _items.RwLock.ExitReadLock(); } }
        }
 #if _
        protected IList<T> Items 
        {
            get
            { 
                return this._items;
            } 
        }
 
        public T this[int index]
        { 
            get { lock (this.sync) { return this._items[index]; } }
        } 
  
        public bool Contains(T value)
        { 
            lock (this.sync)
            {
                return this._items.Contains(value);
            } 
        }
  
        public void CopyTo(T[] array, int index) 
        {
            lock (this.sync) 
            {
                this._items.CopyTo(array, index);
            }
        } 
 
        public IEnumerator<T> GetEnumerator() 
        { 
            lock (this.sync)
            { 
                return this._items.GetEnumerator();
            }
        }
  
        public int IndexOf(T value)
        { 
            lock (this.sync) 
            {
                return this._items.IndexOf(value); 
            }
        }
 
        void ThrowReadOnly() 
        {
            throw new NotSupportedException("read only");
        } 
 
        bool ICollection<T>.IsReadOnly 
        {
            get { return true; }
        }
  
        T IList<T>.this[int index]
        { 
            get
            {
                return this[index]; 
            }
            set
            {
                this.ThrowReadOnly(); 
            }
        } 
  
        void ICollection<T>.Add(T value)
        { 
            this.ThrowReadOnly();
        }
 
        void ICollection<T>.Clear() 
        {
            this.ThrowReadOnly(); 
        } 
 
        bool ICollection<T>.Remove(T value) 
        {
            this.ThrowReadOnly();
            return false;
        } 
 
        void IList<T>.Insert(int index, T value) 
        { 
            this.ThrowReadOnly();
        } 
 
        void IList<T>.RemoveAt(int index)
        {
            this.ThrowReadOnly(); 
        }
  
        bool ICollection.IsSynchronized 
        {
            get { return true; } 
        }
 
        object ICollection.SyncRoot
        { 
            get { return this.sync; }
        } 
  
        void ICollection.CopyTo(Array array, int index)
        { 
            ICollection asCollection = this._items as ICollection;
            if (asCollection == null)
                throw new NotSupportedException("asCollection == null");
  
            lock (this.sync)
            { 
                asCollection.CopyTo(array, index); 
            }
        } 
 
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this.sync) 
            {
                IEnumerable asEnumerable = this._items as IEnumerable; 
                if (asEnumerable != null) 
                    return asEnumerable.GetEnumerator();
                else
                    return new EnumeratorAdapter(this._items);
            }
        }
  
        bool IList.IsFixedSize
        { 
            get { return true; } 
        }
  
        bool IList.IsReadOnly
        {
            get { return true; }
        } 
 
        object IList.this[int index] 
        { 
            get
            { 
                return this[index];
            }
            set
            { 
                this.ThrowReadOnly();
            } 
        } 
 
        int IList.Add(object value) 
        {
            this.ThrowReadOnly();
            return 0;
        } 
 
        void IList.Clear() 
        { 
            this.ThrowReadOnly();
        } 
 
        bool IList.Contains(object value)
        {
            VerifyValueType(value); 
            return this.Contains((T)value);
        } 
  
        int IList.IndexOf(object value)
        { 
            VerifyValueType(value);
            return this.IndexOf((T)value);
        }
  
        void IList.Insert(int index, object value)
        { 
            this.ThrowReadOnly(); 
        }
  
        void IList.Remove(object value)
        {
            this.ThrowReadOnly();
        } 
 
        void IList.RemoveAt(int index) 
        { 
            this.ThrowReadOnly();
        } 
 
        static void VerifyValueType(object value)
        {
            if ((value is T) || (value == null && !typeof(T).IsValueType)) 
                return;
  
            Type type = (value == null) ? typeof(Object) : value.GetType(); 
            throw new ArgumentException("VerifyValueType()"); 
        }
 
        sealed class EnumeratorAdapter: IEnumerator, IDisposable
        { 
            IList<T> list;
            IEnumerator<T> e; 
  
            public EnumeratorAdapter(IList<T> list) {
                this.list = list; 
                this.e = list.GetEnumerator();
            }
 
            public object Current { 
                get { return e.Current; }
            } 
  
            public bool MoveNext() {
                return e.MoveNext(); 
            }
 
            public void Dispose() {
                e.Dispose(); 
            }
  
            public void Reset() { 
                e = list.GetEnumerator();
            } 
        }
#endif
    }
}
#endif