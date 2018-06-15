using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Collections
{
    /// <summary>
    /// Generic value tuple comparer for tuples with 2 comparable components that compares the actual content in tuple definition order
    /// </summary>
    public class TupleContentComparer<T1, T2> : Comparer<(T1, T2)>, IEqualityComparer<(T1, T2)>
        where T1 : IComparable<T1>
        where T2 : IComparable<T2>
    {
        /// <summary>
        /// Compares the content of two tuples in tuple definition order
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override Int32 Compare((T1, T2) x, (T1, T2) y)
        {
            Int32 compItem1 = x.Item1.CompareTo(y.Item1);
            if (compItem1 == 0)
            {
                return x.Item2.CompareTo(y.Item2);
            }
            return compItem1;
        }

        /// <summary>
        /// Compares two tuples for content equivalency
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Boolean Equals((T1, T2) x, (T1, T2) y)
        {
            return Compare(x, y) == 0;
        }

        /// <summary>
        /// Retursn the tuple hash code
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Int32 GetHashCode((T1, T2) obj)
        {
            return obj.GetHashCode();
        }
    }
}
