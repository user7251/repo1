using System.Threading;

namespace GitHub.User7251
{
    /// <summary>
    /// WaitableInt holds an int _value.  It allows multiple threads to add positive or negative 
    /// values and wait for _value to reach a target value.
    /// </summary>
    public class WaitableInt
    {
        private int _targetValue;
        private int _value;
        private object _lock = new object();
        private ManualResetEvent _event;
        // private string _name; // for debugging

        public WaitableInt(string name, int startingValue = 0, int targetValue = 0)
        {
            _value = startingValue;
            _targetValue = targetValue;
            _event = new ManualResetEvent(_value == _targetValue);
            // _name = name;
        }

        public void Add(int i)
        {
            lock (_lock)
            {
                _value += i;
                if (_value == _targetValue) _event.Set();
                else _event.Reset();
            }
        }

        public void WaitOne()
        {
            _event.WaitOne();
        }

        public override string ToString()
        {
            return string.Concat(
                // "name{", _name, "} ", 
                "hash{", GetHashCode().ToString(),
                "} val{", _value.ToString(), "}");
        }
    }
}
