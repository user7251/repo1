using System;
using System.Collections.Generic;
using System.Linq;

namespace GitHub.User7251
{
    public class CollectionTools
    {
        /// * Try to use the overload that takes an IEqualityComparer.  This overload is for collections of types
        ///   where "==" uses value semantics for equality, not reference semantics.
        /// * comment_1_CollectionTools:
        ///   I named the method Equals2() because when I used the name "Equals<T> ( IEnumerable<T> x, IEnumerable<T> y ),"
        ///   and I hover the mouse over "CollectionTools.Equals ( x, y )," the tooltip shows
        ///   "Object.Equals(...)."
        public static bool Equals2<T>(IEnumerable<T> x, IEnumerable<T> y)
        {
            if (x == null && y == null
                || (x == null && y.Count() == 0)
                || (y == null && x.Count() == 0)) return true;
            int intersectionCount = x.Intersect(y).Count();
            return x.Count() == intersectionCount && y.Count() == intersectionCount;
        }

        /// see comment_1_CollectionTools
        public static bool Equals2<T>(IEnumerable<T> x, IEnumerable<T> y, IEqualityComparer<T> comparer)
        {
            if (x == null && y == null
                || (x == null && y.Count() == 0)
                || (y == null && x.Count() == 0)) return true;
            int intersectionCount = x.Intersect(y, comparer).Count();
            return x.Count() == intersectionCount && y.Count() == intersectionCount;
        }

        /// <summary>
        /// From http://stackoverflow.com/questions/5489987/linq-full-outer-join
        /// </summary>
        public static IList<TR> FullOuterJoin<TA, TB, TK, TR>(
            IEnumerable<TA> a,
            IEnumerable<TB> b,
            Func<TA, TK> selectKeyA,
            Func<TB, TK> selectKeyB,
            Func<TA, TB, TK, TR> projection,
            TA defaultA = default(TA),
            TB defaultB = default(TB))
        {
            var alookup = a.ToLookup(selectKeyA);
            var blookup = b.ToLookup(selectKeyB);
            var keys = new HashSet<TK>(alookup.Select(p => p.Key));
            keys.UnionWith(blookup.Select(p => p.Key));
            var join = from key in keys
                from xa in alookup[key].DefaultIfEmpty(defaultA)
                from xb in blookup[key].DefaultIfEmpty(defaultB)
                select projection(xa, xb, key);
            return join.ToList();
        }

        /// <summary>
        /// IEnumerable.ToDictionary() will throw an exception if two values in the input have the same key.
        /// This ToDictionary() will discard one of the values.
        /// </summary>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(IEnumerable<TValue> enumerable, Func<TValue, TKey> keySelector)
        {
            Dictionary<TKey, TValue> d = new Dictionary<TKey, TValue>(enumerable.Count());
            foreach (var e in enumerable)
            {
                TKey key = keySelector(e);
#if DEBUG
                if (d.Keys.Contains(key)) DebugTools.WriteLine(string.Concat("CollectionTools.ToDictionary() input contains a duplicate key ", 
                    key.ToString()));
#endif
                d[key] = e;
            }
            return d;
        }
    }
}
