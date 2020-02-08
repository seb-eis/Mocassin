using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mocassin.Framework.Extensions
{
    /// <summary>
    ///     Contains linq style extension methods for the IEnumerable interface
    /// </summary>
    public static class MocassinLinqExtensions
    {
        /// <summary>
        ///     Get a reverse iterator for a generic enumerable sequence (Uses the list or generic interface if available, else
        ///     linq reverse)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static IEnumerator<T1> GetReverseIterator<T1>(this IEnumerable<T1> sequence)
        {
            if (sequence is IList<T1> genericList)
            {
                for (var i = genericList.Count - 1; i >= 0; i--)
                    yield return genericList[i];
            }

            if (sequence is IList list)
            {
                for (var i = list.Count - 1; i >= 0; i--)
                    yield return (T1) list[i];
            }
            else
            {
                foreach (var item in sequence.Reverse())
                    yield return item;
            }
        }

        /// <summary>
        ///     Casts the enumerable to a generic collection if possible or creates a new collection from the enumerable
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static ICollection<T1> ToCollection<T1>(this IEnumerable<T1> enumerable)
        {
            if (!(enumerable is ICollection<T1> collection))
                collection = enumerable.ToList();

            return collection;
        }

        /// <summary>
        ///     Perform an action on each element of an enumerable sequence and returns the modified value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="values"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T1> Action<T1>(this IEnumerable<T1> values, Action<T1> action)
        {
            foreach (var item in values)
            {
                action(item);
                yield return item;
            }
        }

        /// <summary>
        ///     Generic lexicographical compare for two sequences of values where the items implement generic IComparable
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
        ///     Perform a sequence comparison of two sequences in order using the provided comparer interface.
        ///     If the sequences are identical till point of completion of the first, the lengths are compared
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int LexicographicCompare<T1>(this IEnumerable<T1> lhs, IEnumerable<T1> rhs, IComparer<T1> comparer)
        {
            if (!(lhs is ICollection<T1> lhsCollection))
                lhsCollection = lhs.ToList();

            if (!(rhs is ICollection<T1> rhsCollection))
                rhsCollection = rhs.ToList();

            using (var lhsIterator = lhsCollection.GetEnumerator())
            {
                // ReSharper disable once GenericEnumeratorNotDisposed
                using var rhsIterator = rhsCollection.GetEnumerator();
                while (lhsIterator.MoveNext() && rhsIterator.MoveNext())
                {
                    var compValue = comparer.Compare(lhsIterator.Current, rhsIterator.Current);
                    if (compValue != 0)
                        return compValue;
                }
            }

            return lhsCollection.Count.CompareTo(rhsCollection.Count);
        }

        /// <summary>
        ///     Performs a select operation on consecutive pairs within the enumerable yielding N-1 results. First argument of
        ///     function is last value, second is current value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="values"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static IEnumerable<T2> SelectConsecutivePairs<T1, T2>(this IEnumerable<T1> values, Func<T1, T1, T2> function)
        {
            // ReSharper disable once GenericEnumeratorNotDisposed
            using var iterator = values.GetEnumerator();
            if (!iterator.MoveNext()) yield break;

            var last = iterator.Current;
            while (iterator.MoveNext())
            {
                yield return function(last, iterator.Current);
                last = iterator.Current;
            }
        }

        /// <summary>
        ///     Enables passing of a single item as en enumerable sequence containing only that item
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T1> AsSingleton<T1>(this T1 item)
        {
            yield return item;
        }

        /// <summary>
        ///     Returns the enumeration index of all entries that match the search predicate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="searchSequence"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<int> IndexOfMany<T1>(this IEnumerable<T1> searchSequence, Predicate<T1> predicate)
        {
            var index = 0;
            foreach (var item in searchSequence)
            {
                if (predicate(item))
                    yield return index;

                index++;
            }
        }

        /// <summary>
        ///     Linq style <see cref="List{T}" /> conversion extension with initial capacity value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this IEnumerable<T> source, int capacity)
        {
            var result = new List<T>(capacity);
            result.AddRange(source);
            return result;
        }

        /// <summary>
        ///     Linq style <see cref="Array" /> conversion for cases where the size of the <see cref="IEnumerable{T}" /> is known
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this IEnumerable<T> source, int length)
        {
            if (length < 0) throw new ArgumentException("Length cannot be smaller than 0");
            var result = new T[length];
            var index = -1;
            foreach (var item in source) result[++index] = item;

            return result;
        }

        /// <summary>
        ///     Linq style <see cref="ObservableCollection{T}" /> conversion extension
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }

        /// <summary>
        ///     Get an <see cref="ICollection{T}" /> from the provided <see cref="IEnumerable{T}" />. If the source does not
        ///     implement the interface a new collection is created by invoking the sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ICollection<T> AsCollection<T>(this IEnumerable<T> source)
        {
            return source as ICollection<T> ?? source.ToList();
        }

        /// <summary>
        ///     Get an <see cref="IList{T}" /> from the provided <see cref="IEnumerable{T}" />. If the source does not implement
        ///     the interface a new list is created by invoking the sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<T> AsList<T>(this IEnumerable<T> source)
        {
            return source as IList<T> ?? source.ToList();
        }

        /// <summary>
        ///     Enumerates the provided <see cref="IEnumerable{T}" />. Equivalent to calling ToList() and throwing away the list
        ///     without the overhead of actually creating a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        public static void Load<T>(this IEnumerable<T> source)
        {
            using var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
            }

            enumerator.Dispose();
        }
    }
}