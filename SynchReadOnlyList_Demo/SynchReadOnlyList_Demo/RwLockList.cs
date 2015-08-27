using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
#if SYNCH_READ_ONLY_LIST
namespace com.GitHub.user7251 {
    /// <summary>
    /// A list with a ReaderWriterLockSlim for client use.
    /// </summary>
    public class RwLockList<T> : List<T> {
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        public ReaderWriterLockSlim RwLock { get { return _rwLock; } }
        public RwLockList() { }
        public RwLockList ( IEnumerable<T> list ) : base ( list ) { }
        public RwLockList ( int count ) : base ( count ) { }
    }
}
#endif