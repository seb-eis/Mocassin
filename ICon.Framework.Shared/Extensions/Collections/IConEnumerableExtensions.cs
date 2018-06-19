using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace ICon.Framework.Extensions
{
    /// <summary>
    /// Contains extension methods for the IEnumerable interface
    /// </summary>
    public static class IConEnumerableExtensions
    {
        /// <summary>
        /// Get a reverse iterator for a generic enumerable sequence (Uses the list or generic interface if available, else linq reverse)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static IEnumerator<T1> GetReverseIterator<T1>(this IEnumerable<T1> sequence)
        {
            if (sequence is IList<T1> genericList)
            {
                for (int i = genericList.Count - 1; i >= 0; i--)
                {
                    yield return genericList[i];
                }
            }
            if (sequence is IList list)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    yield return (T1)list[i];
                }
            }
            else
            {
                foreach (var item in sequence.Reverse())
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Perform an action on each elemnt of an enumerable sequence and returns the modofied value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="values"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T1> Change<T1>(this IEnumerable<T1> values, Action<T1> action)
        {
            foreach (var item in values)
            {
                action(item);
                yield return item;
            }
        }

        /// <summary>
        /// Generic lexicographical compare for two sequences of values where the items implement generic IComparable
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static int LexicographicCompare<T1>(this IEnumerable<T1> lhs, IEnumerable<T1> rhs) where T1 : IComparable<T1>
        {
            return LexicographicCompare(lhs, rhs, Comparer<T1>.Default);
        }

        /// <summary>
        /// Perform a sequence comparison of two sequnces in order using the provided comparer interface. If the sequnces are identical till point of completion of the first, the lengtsh are compared
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int LexicographicCompare<T1>(this IEnumerable<T1> lhs, IEnumerable<T1> rhs, IComparer<T1> comparer)
        {
            var lhsIterator = lhs.GetEnumerator();
            var rhsIterator = rhs.GetEnumerator();
            while (lhsIterator.MoveNext() && rhsIterator.MoveNext())
            {
                var compValue = comparer.Compare(lhsIterator.Current, rhsIterator.Current);
                if (compValue != 0)
                {
                    return compValue;
                }
            }
            return lhs.Count().CompareTo(rhs.Count());
        }

        /// <summary>
        /// Performs a select operation on consecutive pairs within the enumerable yielding N-1 results. First argument of function is last value, second is current value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="values"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static IEnumerable<T2> ConsecutivePairSelect<T1, T2>(this IEnumerable<T1> values, Func<T1, T1, T2> function)
        {
            var iterator = values.GetEnumerator();
            T1 last;

            if (!iterator.MoveNext())
            {
                yield break;
            }
            last = iterator.Current;
            while (iterator.MoveNext())
            {
                yield return function(last, iterator.Current);
                last = iterator.Current;
            }
        }

        /// <summary>
        /// Enables passing of a single item as en enumerable seqeunce containing only that item
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T1> AsSingleton<T1>(this T1 item)
        {
            yield return item;
        }
    }
}
