using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using com.GitHub.user7251;
#if SYNCH_READ_ONLY_LIST
// Ideas not demo'd in Program1:
//   - A new class, SynchReadOnlyList2<T>, that synchronizes on an internal ReaderWriterLockSlim.
//     It's a minor improvement over SynchReadOnlyList<T>.
//
namespace com.GitHub.user7251.SynchReadOnlyList_Demo {
    //class MainClass {
    //    static void Main(string[] args) {
    //        Demo d = new Demo();
    //        d.Run();
    //    }
    //}
    public class Demo {
        Order _order;
        public void Run() {
            _order = BuildOrder();
            var addAndDeleteProductsTask = Task.Factory.StartNew ( AddAndDeleteProducts );
            Enumerate();
            addAndDeleteProductsTask.Wait();
            // ...
        }
        Order BuildOrder() {
            Order order = new Order();
            Product p = new Product();
            p.Name = "Product 1";
            order.AddProduct ( p );
            p = new Product();
            p.Name = "Product 2";
            order.AddProduct ( p );
            return order;
        }
        void AddAndDeleteProducts() {
        
        
        }
        void Enumerate() {
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
        public Order AddProduct ( Product p ) { _products.Add ( p ); return this; }
    }
    public class Product {
        public string Name { get; set; }
    }
}
#endif