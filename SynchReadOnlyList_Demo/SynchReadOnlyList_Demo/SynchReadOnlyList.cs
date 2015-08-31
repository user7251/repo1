using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.ServiceModel;
using System.Linq;
#if SYNCH_READ_ONLY_LIST
namespace com.GitHub.user7251 {   
    #if _
    - SynchReadOnlyList is a synchronized read-only list.
    - Microsoft's ReadOnlyCollection (ROC) does not synchronize the ROC with other
      clients of the IList that the ROC references.
    - Microsoft's SynchronizedReadOnlyCollection (SROC) makes a copy of the primary List.  
      SynchReadOnlyList hold a reference to the original list.
    - SROC uses lock() for synchronization.  SynchReadOnlyList uses a ReaderWriterLockSlim.
    - ConcurrentBag is not read-only.
    - ConcurrentBag copies the original list.  SynchReadOnlyList references the original list, 
      so it reflects changes in the original list, and it uses less memory.
    - To develop SynchReadOnlyList, I started with the code for SynchronizedReadOnlyCollection.
    - ~~ I removed the inheritance from IList<T> and IList because when I called 
      SynchReadOnlyList.Contains() ...
    #endif
    [System.Runtime.InteropServices.ComVisible(false)]
    public class SynchReadOnlyList<T> {
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
        public bool Contains ( T value, IEqualityComparer<T> iec ) {
            Console.Out.WriteLine ( "SynchReadOnlyList.Contains() waiting on EnterReadLock()" );
            _rwLock.EnterReadLock();
            try { 
                Console.Out.WriteLine ( "SynchReadOnlyList.Contains() after EnterReadLock()" );
                var ie = _iList as IEnumerable<T>;
                return ie.Contains ( value, iec ); }
            finally { _rwLock.ExitReadLock(); 
                Console.Out.WriteLine ( "SynchReadOnlyList.Contains() after ExitReadLock()" );}
        }
        public void CopyTo(T[] array, int index) {
            _rwLock.EnterReadLock();
            try { 
                _iList.CopyTo(array, index); }
            finally { _rwLock.ExitReadLock(); }
        } 
        #if _
        GetEnumerator() does not enter a read lock because the client should do it.
        Usage:
            SynchReadOnlyList<int> l = x.List();
            l.RwLock.EnterReadLock();
            try { foreach ( int i in l ) useI ( i ); }
            finally { l.RwLock.ExitReadLock(); }
        #endif
        public IEnumerator<T> GetEnumerator() { 
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
    }
}
#endif