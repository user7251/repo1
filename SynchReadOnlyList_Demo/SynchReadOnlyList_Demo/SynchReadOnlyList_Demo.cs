using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
// Ideas not demo'd in Program1:
//   - A new class, SynchReadOnlyList2<T>, that synchronizes on an internal ReaderWriterLockSlim.
//     It's a minor improvement over SynchReadOnlyList<T>.
//
namespace GitHub_user7251 {
    class MainClass {
        static void Main(string[] args) {
            SynchReadOnlyList_Demo d = new SynchReadOnlyList_Demo();
            d.Run();
        }
    }
    public class SynchReadOnlyList_Demo {
        public void Run() {
            Order order = BuildOrder();
            // ...
        }
        Order BuildOrder() {
            Order order = new Order();
            // ...
            return order;
        }
    }
    // Synchronizes on _products.RwLock.
    public class Order {
        // _products is never null
	    private readonly RwLockList<Product> _products;
        // _productsSrol is never null
	    private readonly SynchReadOnlyList<Product> _productsSrol;
        // Products is never null
	    public SynchReadOnlyList<Product> Products { get { return _productsSrol; } }
        public Order() {
            _products = new RwLockList<Product>();
            _productsSrol = new SynchReadOnlyList<Product> ( _products ); }
    }
    public class Product {
        public string Name { get; set; }
    }
}
