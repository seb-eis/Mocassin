using System;
using System.Collections.Generic;

namespace Mocassin.Framework.Collections
{
    /// <summary>
    ///     Equality compare adapter that encapsulates an equality comparer from compare and hash delegate
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class EqualityCompareAdapter<T1> : EqualityComparer<T1>
    {
        /// <summary>
        ///     The equality compare delegate
        /// </summary>
        public Func<T1, T1, bool> CompareFunction { get; protected set; }

        /// <summary>
        ///     The hash function used by the comparer
        /// </summary>
        public Func<T1, int> HashFunction { get; protected set; }

        /// <summary>
        ///     Create new compare adapter from delegate and hash function delegate
        /// </summary>
        /// <param name="compareDelegate"></param>
        /// <param name="hashFunction"></param>
        public EqualityCompareAdapter(Func<T1, T1, bool> compareDelegate, Func<T1, int> hashFunction)
        {
            CompareFunction = compareDelegate ?? throw new ArgumentNullException(nameof(compareDelegate));
            HashFunction = hashFunction ?? throw new ArgumentNullException(nameof(hashFunction));
        }

        /// <summary>
        ///     Creates new equality compare adapter with the passed compare delegate and the default hash function
        /// </summary>
        /// <param name="compareDelegate"></param>
        public EqualityCompareAdapter(Func<T1, T1, bool> compareDelegate)
        {
            CompareFunction = compareDelegate ?? throw new ArgumentNullException(nameof(compareDelegate));
            HashFunction = a => a.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(T1 x, T1 y)
        {
            return CompareFunction(x, y);
        }

        /// <inheritdoc />
        public override int GetHashCode(T1 obj)
        {
            return HashFunction(obj);
        }
    }
}