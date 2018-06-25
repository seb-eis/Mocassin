using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Collections
{
    /// <summary>
    /// Generic compare object that wraps a generic comparison delegate to support both equality and comparable interface
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class CompareAdapter<T1> : Comparer<T1>, IEqualityComparer<T1>
    {
        /// <summary>
        /// The wrapper comparison delegate
        /// </summary>
        public Func<T1, T1, Int32> CompDelegate { get; protected set; }

        /// <summary>
        /// Creates new wrapped comparer from the passed delegate
        /// </summary>
        /// <param name="compDelegate"></param>
        public CompareAdapter(Func<T1, T1, Int32> compDelegate)
        {
            CompDelegate = compDelegate;
        }

        /// <summary>
        /// Compares two values with the wrapped delegate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override Int32 Compare(T1 x, T1 y)
        {
            return CompDelegate(x, y);
        }

        /// <summary>
        /// Cehcks if two values compare equal with the wrapped delegate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Boolean Equals(T1 x, T1 y)
        {
            return CompDelegate(x, y) == 0;
        }

        /// <summary>
        /// Returns the hash code of the object type this wrapper is used for
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Int32 GetHashCode(T1 obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// Equality compare adapter to encapsulate an equality compare delegate
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class EqualityCompareAdapter<T1> : EqualityComparer<T1>
    {
        /// <summary>
        /// The equality compare delegate
        /// </summary>
        public Func<T1, T1, bool> CompareDelegate { get; protected set; }

        /// <summary>
        /// The hash function used by the comparer
        /// </summary>
        public Func<T1, int> HashFunction { get; protected set; }

        /// <summary>
        /// Create new compare adapter from delegate and hash function delegate
        /// </summary>
        /// <param name="compareDelegate"></param>
        /// <param name="hashFunction"></param>
        public EqualityCompareAdapter(Func<T1, T1, bool> compareDelegate, Func<T1, int> hashFunction)
        {
            CompareDelegate = compareDelegate ?? throw new ArgumentNullException(nameof(compareDelegate));
            HashFunction = hashFunction;
        }

        /// <summary>
        /// Compares for equality
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals(T1 x, T1 y)
        {
            return CompareDelegate(x, y);
        }

        /// <summary>
        /// Get the hach code of the generic object type
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int GetHashCode(T1 obj)
        {
            if (HashFunction == null)
            {
                return obj.GetHashCode();
            }
            return HashFunction(obj);
        }
    }
}
