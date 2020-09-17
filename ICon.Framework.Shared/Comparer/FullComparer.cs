using System;
using System.Collections.Generic;

namespace Mocassin.Framework.Comparer
{
    /// <summary>
    ///     Generic compare adapter for compare delegates that implements both generic IComparer and IEqualityComparer
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class FullComparer<T1> : Comparer<T1>, IEqualityComparer<T1>
    {
        /// <summary>
        ///     The wrapper comparison delegate
        /// </summary>
        public Func<T1, T1, int> CompareFunction { get; protected set; }

        /// <summary>
        ///     Creates new wrapped comparer from the passed delegate
        /// </summary>
        /// <param name="compareFunction"></param>
        public FullComparer(Func<T1, T1, int> compareFunction)
        {
            CompareFunction = compareFunction;
        }

        /// <inheritdoc />
        public bool Equals(T1 x, T1 y) => CompareFunction(x, y) == 0;

        /// <inheritdoc />
        public int GetHashCode(T1 obj) => obj.GetHashCode();

        /// <summary>
        ///     Compares two values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(T1 x, T1 y) => CompareFunction(x, y);
    }
}