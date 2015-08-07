using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

// Ideas not demo'd in Program1:
//   - A new class, SynchronizedReadOnlyCollection3<T>, that synchronizes on an internal ReaderWriterLockSlim.
//     It's a minor improvement over SynchronizedReadOnlyCollection<T>.
//
namespace GitHub_user7251 {
    class Program1 {
        static void Main(string[] args) {
        }
    }
    public class List2<T> : List<T> {
        private readonly ReaderWriterLockSlim _rwls = new ReaderWriterLockSlim();
        public ReaderWriterLockSlim Lock { get { return _rwls; } }
    }
    // Synchronizes on List2<T>.Lock.
    public class SynchronizedReadOnlyCollection2<T> {
        // _list is never null
        private readonly List2<T> _list;
        public SynchronizedReadOnlyCollection2 ( List2<T> list ) { _list = list; }
    }
    // Synchronizes on List2<T>.Lock.
    public class Demo1 {
        // _list is never null
	    private readonly List2<string> _list = new List2<string>();
	    private readonly SynchronizedReadOnlyCollection2<string> _sroc;
        // List is never null
	    public SynchronizedReadOnlyCollection2<string> List { get { return _sroc; } }
        public Demo1() {
            _sroc = new SynchronizedReadOnlyCollection2<string> ( _list ); }
    }
}
