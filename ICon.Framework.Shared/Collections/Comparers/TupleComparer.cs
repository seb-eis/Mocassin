using System;
using System.Collections.Generic;

namespace Mocassin.Framework.Collections
{
    /// <summary>
    ///     Generic value tuple comparer for tuples with 2 comparable components that compares the actual content in tuple
    ///     definition order
    /// </summary>
    public class TupleComparer<T1, T2> : Comparer<(T1, T2)>, IEqualityComparer<(T1, T2)>
        where T1 : IComparable<T1>
        where T2 : IComparable<T2>
    {
        /// <inheritdoc />
        public bool Equals((T1, T2) x, (T1, T2) y)
        {
            return Compare(x, y) == 0;
        }

        /// <inheritdoc />
        public int GetHashCode((T1, T2) obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        ///     Compares the content of two tuples in tuple definition order
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare((T1, T2) x, (T1, T2) y)
        {
            var compItem1 = x.Item1.CompareTo(y.Item1);
            return compItem1 == 0
                ? x.Item2.CompareTo(y.Item2)
                : compItem1;
        }
    }
}