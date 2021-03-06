#if _
This file's primary location is under: https://github.com/user7251
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GitHub.User7251;
namespace GitHub.User7251.Test {
    class MainClass {
        static void Main(string[] args) {
            SynchReadOnlyList_Test d = new SynchReadOnlyList_Test();
            d.Run();
        }
    }
    public class SynchReadOnlyList_Test {
        Order _order;
        const int INITIAL_PRODUCT_COUNT = 9;
        const string PRODUCT_NAME_PREFIX = "Product ";
        ManualResetEvent _mre = new ManualResetEvent ( false );
        public void Run() {
            Console.Out.WriteLine ( "SynchReadOnlyList_Test.Run() {" );
            _order = BuildOrder();
            var addAndDeleteProductsTask = Task.Factory.StartNew ( AddAndDeleteProducts );
            ContainsProduct();
            addAndDeleteProductsTask.Wait();
            Console.Out.WriteLine ( "} SynchReadOnlyList_Test.Run()" );
        }
        Order BuildOrder() {
            Order order = new Order();
            Product p;
            int i = 0;
            while ( ++i <= INITIAL_PRODUCT_COUNT ) {
                p = new Product();
                p.Name = string.Concat ( PRODUCT_NAME_PREFIX, i );
                order.AddProduct ( p ); }
            return order;
        }
        void AddAndDeleteProducts() {
            Console.Out.WriteLine ( "AddAndDeleteProducts() {" );
            _order.AddAndDeleteProducts ( count: INITIAL_PRODUCT_COUNT, startingProductNumber: INITIAL_PRODUCT_COUNT + 1, mre: _mre );
            Console.Out.WriteLine ( "} AddAndDeleteProducts()" );
        }
        void ContainsProduct() {
            _mre.WaitOne();
            Product p = new Product();
            p.Name = string.Concat ( PRODUCT_NAME_PREFIX, INITIAL_PRODUCT_COUNT + 3 );
            Console.Out.WriteLine ( string.Concat ( "calling _order.Products.Contains(", p.Name, ")" ) );
            bool r = _order.Products.Contains ( p, Product.s_ProductByNameEqualityComparer );
            Console.Out.WriteLine ( string.Concat ( "_order.Products.Contains(", p.Name, ") returned {", r, "}" ) );
        }
    }
    public class Order {
	    //
        // these fields and properties are never null
        private readonly List<Product> _products;
        private readonly ReaderWriterLockSlim _productsRwLock = new ReaderWriterLockSlim ( LockRecursionPolicy.SupportsRecursion );
	    private readonly SynchReadOnlyList<Product> _productsSrol; 
	    public SynchReadOnlyList<Product> Products { get { return _productsSrol; } }
        //
        public Order() {
            _products = new List<Product>();
            _productsSrol = new SynchReadOnlyList<Product> ( _products, _productsRwLock ); }
        public Order AddProduct ( Product p ) { 
            _productsRwLock.EnterWriteLock();
            try { 
                _products.Add ( p );
                return this; }
            finally { _productsRwLock.ExitWriteLock(); } }
        public void AddAndDeleteProducts ( int count, int startingProductNumber, ManualResetEvent mre ) {
            Product p;
            int i = -1;
            _productsRwLock.EnterWriteLock();
            Console.Out.WriteLine ( "after EnterWriteLock()" ); 
            try {
                while ( ++i < count ) {
                    if ( i == count / 2 ) mre.Set();
                    p = new Product();
                    p.Name = string.Concat ( "Product ", startingProductNumber ++ );
                    Console.Out.WriteLine ( string.Concat ( "add    {", p.Name, "}" ) );
                    _products.Add ( p );
                    p = _products[0];
                    Console.Out.WriteLine ( string.Concat ( "remove {", p.Name, "}" ) );
                    _products.RemoveAt ( 0 ); } }
            finally { _productsRwLock.ExitWriteLock();
                Console.Out.WriteLine ( "after ExitWriteLock()" );  } }
    }
    public class ProductByNameEqualityComparer : IEqualityComparer<Product> {
        public bool Equals(Product x, Product y) {
            Console.Out.WriteLine ( string.Concat ( "ProductByNameEqualityComparer.Equals(){", x.Name, "}{", y.Name, "}" ) );
            if (object.ReferenceEquals(x, y)) return true;
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return false;
            return x.Name.Equals ( y.Name );
        }
        public int GetHashCode ( Product x ) {
            Console.Out.WriteLine ( string.Concat ( "ProductByNameEqualityComparer.GetHashCode(){", x.Name, "}" ) );
            if ( object.ReferenceEquals ( x, null ) ) return 0;
            return x.Name.GetHashCode();
        }
    }
    public class Product {
        public string Name { get; set; }
        public static ProductByNameEqualityComparer s_ProductByNameEqualityComparer = new ProductByNameEqualityComparer();
    }
}
