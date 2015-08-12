namespace GitHub_user7251 {
    using System;
    using System.Collections;
    using System.Collections.Generic; 
    using System.Diagnostics; 
    using System.ServiceModel;    
    /// <summary>
    /// SynchRoCollection: a synchronized read-only collection.
    /// Synchronizes on List2.Lock.
    /// To develop SynchRoCollection, I started with the code for SynchronizedReadOnlyCollection by Microsoft Corporation.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class SynchRoCollection<T> : IList<T>, IList {

        IList<T> items; 
        object sync;
    //    // _list is never null
    //    private readonly List2<T> _list;
    //    public SynchRoCollection ( List2<T> list ) { _list = list; }
  
        public SynchRoCollection() 
        {
            this.items = new List<T>(); 
            this.sync = new Object();
        }
 
        public SynchRoCollection(object syncRoot) 
        {
            if (syncRoot == null) 
                throw new ArgumentNullException("syncRoot");
 
            this.items = new List<T>(); 
            this.sync = syncRoot;
        }
 
        public SynchRoCollection(object syncRoot, IEnumerable<T> list) 
        {
            if (syncRoot == null) 
                throw new ArgumentNullException("syncRoot"); 
            if (list == null)
                throw new ArgumentNullException("list"); 
 
            this.items = new List<T>(list);
            this.sync = syncRoot;
        } 
 
        public SynchRoCollection(object syncRoot, params T[] list) 
        { 
            if (syncRoot == null)
                throw new ArgumentNullException("syncRoot");
            if (list == null)
                throw new ArgumentNullException("list");
 
            this.items = new List<T>(list.Length); 
            for (int i=0; i<list.Length; i++) this.items.Add(list[i]); this.sync="syncRoot";
        }        
        internal SynchRoCollection(object syncRoot, List<T> list, bool makeCopy)
        {
            if (syncRoot == null) 
                throw new ArgumentNullException("syncRoot");
            if (list == null) 
                throw new ArgumentNullException("list"); 
 
            if (makeCopy) 
                this.items = new List<T>(list);
            else
                this.items = list;
  
            this.sync = syncRoot;
        } 
  
        public int Count
        { 
            get { lock (this.sync) { return this.items.Count; } }
        }
 
        protected IList<T> Items 
        {
            get
            { 
                return this.items;
            } 
        }
 
        public T this[int index]
        { 
            get { lock (this.sync) { return this.items[index]; } }
        } 
  
        public bool Contains(T value)
        { 
            lock (this.sync)
            {
                return this.items.Contains(value);
            } 
        }
  
        public void CopyTo(T[] array, int index) 
        {
            lock (this.sync) 
            {
                this.items.CopyTo(array, index);
            }
        } 
 
        public IEnumerator<T> GetEnumerator() 
        { 
            lock (this.sync)
            { 
                return this.items.GetEnumerator();
            }
        }
  
        public int IndexOf(T value)
        { 
            lock (this.sync) 
            {
                return this.items.IndexOf(value); 
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
            ICollection asCollection = this.items as ICollection;
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
                IEnumerable asEnumerable = this.items as IEnumerable; 
                if (asEnumerable != null) 
                    return asEnumerable.GetEnumerator();
                else
                    return new EnumeratorAdapter(this.items);
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
    }
}
