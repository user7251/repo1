using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
// Ideas not demo'd in Program1:
//   - A new class, SynchRoCollection3<T>, that synchronizes on an internal ReaderWriterLockSlim.
//     It's a minor improvement over SynchRoCollection<T>.
//
namespace GitHub_user7251 {
    class SynchRoCollection_Demo {
        static void Main(string[] args) {
        }
    }
    public class List2<T> : List<T> {
        private readonly ReaderWriterLockSlim _rwls = new ReaderWriterLockSlim();
        public ReaderWriterLockSlim Lock { get { return _rwls; } }
    }
    /// Synchronizes on List2.Lock.
    public class Order {
        // _list is never null
	    private readonly List2<string> _list = new List2<string>();
	    private readonly SynchRoCollection<string> _sroc;
        // List is never null
	    public SynchRoCollection<string> List { get { return _sroc; } }
        public Order() {
            _sroc = new SynchRoCollection<string> ( _list ); }
    }
}
