using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace GitHub_user7251 {
    /// <summary>
    /// A list with a ReaderWriterLockSlim for client use.
    /// </summary>
    public class RwLockList<T> : List<T> {
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        public ReaderWriterLockSlim RwLock { get { return _rwLock; } }
    }
}
