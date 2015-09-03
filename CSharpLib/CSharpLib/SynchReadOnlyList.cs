#if _
This file's primary location is under: https://github.com/user7251

SynchReadOnlyList<T> is a synchronized read-only list developed for exposing an internal IList<T>.  
It holds a reference to the original IList<T>, so it reflects changes in that list.  It uses a 
ReaderWriterLockSlim to synchronize access to the IList<T> with other clients of that list.

- Microsoft's ReadOnlyCollection<T> references an IList<T>, but it does not synchronize with other
  clients of the IList<T>.

- Microsoft's SynchronizedReadOnlyCollection<T> makes a copy of the original IList<T>.  
  SynchReadOnlyList<T> hold a reference to the original IList<T>.
- SynchronizedReadOnlyCollection<T> uses lock() for synchronization.  SynchReadOnlyList<T> uses a ReaderWriterLockSlim.
- SynchronizedReadOnlyCollection<T> does not make sense in any situation.  It copies the original list, and it's read-only, 
  so it does not need synchronization.  But it has synchronization build-in.

- ConcurrentBag<T> is not read-only.
- ConcurrentBag<T> copies the original list.  SynchReadOnlyList<T> references the original list, 
  so it reflects changes in the original list, and it uses less memory.

- To develop SynchReadOnlyList<T>, I started with the code of SynchronizedReadOnlyCollection<T>.

comment_SynchReadOnlyList_GetEnumerator_1: 
    GetEnumerator() does not enter a read lock because the client should do it.
    Usage:
        SynchReadOnlyList<int> l = x.List;
        l.RwLock.EnterReadLock();
        try { foreach ( int i in l ) useI ( i ); }
        finally { l.RwLock.ExitReadLock(); }
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.ServiceModel;
using System.Linq;
namespace GitHub.User7251 {   
    [System.Runtime.InteropServices.ComVisible(false)]
    public class SynchReadOnlyList<T> : IList<T>, IList {
        private readonly IList<T> _iList; // is never null
        private ReaderWriterLockSlim _rwLock; // is never null
        //
        public SynchReadOnlyList ( IList<T> iList, ReaderWriterLockSlim rwLock ) { 
            if ( iList == null ) throw new ArgumentException("IList<T> iList == null");
            if ( rwLock == null ) throw new ArgumentException("ReaderWriterLockSlim rwLock == null");
            _iList = iList;
            _rwLock = rwLock; 
        }
        //
        public ReaderWriterLockSlim RwLock { get { return _rwLock; } }
        public int Count { 
            get { 
                _rwLock.EnterReadLock();
                try { return _iList.Count; }
                finally { _rwLock.ExitReadLock(); } } }
        // protected IList<T> Items { get {  return _iList; } }
        public T this[int index] { 
            get { 
                _rwLock.EnterReadLock();
                try { return _iList[index]; }
                finally { _rwLock.ExitReadLock(); } } }
        public bool Contains ( T value ) {
            _rwLock.EnterReadLock();
            try { return _iList.Contains ( value ); }
            finally { _rwLock.ExitReadLock(); }
        }
        public bool Contains ( T value, IEqualityComparer<T> iec ) {
            //Console.Out.WriteLine ( "SynchReadOnlyList.Contains() waiting on EnterReadLock()" );
            _rwLock.EnterReadLock();
            try { 
                //Console.Out.WriteLine ( "SynchReadOnlyList.Contains() after EnterReadLock()" );
                var ie = _iList as IEnumerable<T>;
                return ie.Contains ( value, iec ); }
            finally { _rwLock.ExitReadLock(); 
                //Console.Out.WriteLine ( "SynchReadOnlyList.Contains() after ExitReadLock()" );
                }
        }
        public void CopyTo(T[] array, int index) {
            _rwLock.EnterReadLock();
            try { 
                _iList.CopyTo(array, index); }
            finally { _rwLock.ExitReadLock(); }
        } 
        /// <summary>
        /// see comment_SynchReadOnlyList_GetEnumerator_1
        /// </summary>
        public IEnumerator<T> GetEnumerator() { 
            if ( _rwLock.RecursiveReadCount < 1 ) throw new NotSupportedException ( 
                "SynchReadOnlyList<T>.GetEnumerator() expects the client to call SynchReadOnlyList.RwLock.EnterReadLock() and ExitReadLock()." );
            return _iList.GetEnumerator();
        }
        public int IndexOf(T value) { 
            _rwLock.EnterReadLock();
            try { return _iList.IndexOf(value); }
            finally { _rwLock.ExitReadLock(); }
        }
        void ThrowReadOnly() {
            throw new NotSupportedException("read only");
        }
        static void VerifyValueType(object value) {
            if ((value is T) || (value == null && !typeof(T).IsValueType)) return;  
            Type type = (value == null) ? typeof(Object) : value.GetType();
            throw new ArgumentException ( string.Concat ( "The collection of type {", typeof(T).ToString(), 
                "} does not support values of type {", type.ToString(),"}." ) );
        }
        /// <summary>
        /// See comment_SynchReadOnlyList_GetEnumerator_1
        /// </summary>
        sealed class EnumeratorAdapter: IEnumerator, IDisposable { 
            IList<T> _iList;
            IEnumerator<T> _iEnum;  
            public EnumeratorAdapter(IList<T> list) {
                _iList = list; 
                _iEnum = list.GetEnumerator();
            } 
            public object Current { 
                get { return _iEnum.Current; }
            }   
            public bool MoveNext() {
                return _iEnum.MoveNext(); 
            } 
            public void Dispose() {
                _iEnum.Dispose(); 
            }
            public void Reset() { 
                _iEnum = _iList.GetEnumerator();
            } 
        }
        bool ICollection<T>.IsReadOnly {
            get { return true; }
        }
        void ICollection<T>.Add(T value) { 
            ThrowReadOnly();
        } 
        void ICollection<T>.Clear() {
            ThrowReadOnly(); 
        }  
        bool ICollection<T>.Contains(T t) {
            return Contains(t);
        }  
        bool ICollection<T>.Remove(T value) {
            ThrowReadOnly();
            return false;
        }  
        bool ICollection.IsSynchronized {
            get { return true; } 
        } 
        object ICollection.SyncRoot { 
            get { throw new NotSupportedException("Use the RwLock instead of SyncRoot."); }
        }
        void ICollection.CopyTo(Array array, int index) {
            ICollection asCollection = _iList as ICollection;
            if (asCollection == null) throw new NotSupportedException("_iList as ICollection == null");
            _rwLock.EnterReadLock();
            try { asCollection.CopyTo(array, index); }
            finally { _rwLock.ExitReadLock(); }            
        }
        /// <summary>
        /// See comment_SynchReadOnlyList_GetEnumerator_1
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() {
            IEnumerable asEnumerable = _iList as IEnumerable; 
            if (asEnumerable != null) return asEnumerable.GetEnumerator();
            else return new EnumeratorAdapter(_iList);
        }
        T IList<T>.this[int index] { 
            get { return this[index]; } // cal the non-explicit interface implementation
            set { ThrowReadOnly(); }
        } 
        void IList<T>.Insert(int index, T value) { 
            ThrowReadOnly();
        }  
        void IList<T>.RemoveAt(int index) {
            ThrowReadOnly(); 
        }  
        bool IList.IsFixedSize { get { return true; } }
        bool IList.IsReadOnly { get { return true; } } 
        object IList.this[int index] { 
            get { return this[index]; }
            set { ThrowReadOnly(); } 
        }  
        int IList.Add(object value) {
            ThrowReadOnly();
            return 0;
        } 
        void IList.Clear() { 
            ThrowReadOnly();
        } 
        bool IList.Contains(object value) {
            VerifyValueType(value);
            return Contains((T)value);
        }   
        int IList.IndexOf(object value) { 
            VerifyValueType(value);
            return IndexOf((T)value);
        }  
        void IList.Insert(int index, object value) { 
            ThrowReadOnly(); 
        }  
        void IList.Remove(object value) {
            ThrowReadOnly();
        }  
        void IList.RemoveAt(int index) { 
            ThrowReadOnly();
        }
    }
}
