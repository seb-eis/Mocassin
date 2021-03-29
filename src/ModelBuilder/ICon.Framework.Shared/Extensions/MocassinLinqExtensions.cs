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
            switch (sequence)
            {
                case IReadOnlyList<T1> list:
                {
                    for (var i = list.Count - 1; i >= 0; i--)
                        yield return (T1) list[i];
                    break;
                }
                case IList<T1> list:
                {
                    for (var i = list.Count - 1; i >= 0; i--)
                        yield return list[i];
                    yield break;
                }
                case IList list:
                {
                    for (var i = list.Count - 1; i >= 0; i--)
                        yield return (T1) list[i];
                    break;
                }
                default:
                {
                    foreach (var item in sequence.Reverse())
                        yield return item;
                    break;
                }
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
        public static int LexicographicCompare<T1>(this IEnumerable<T1> lhs, IEnumerable<T1> rhs) where T1 : IComparable<T1> =>
            LexicographicCompare(lhs, rhs, Comparer<T1>.Default);

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
        ///     Performs a select operation where data is returned as (n, n+1), (n+1, n+2), ... pairs. This yields N - 1 results
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="values"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<T2> SelectAllConsecutivePairs<T1, T2>(this IEnumerable<T1> values, Func<T1, T1, T2> selector)
        {
            // ReSharper disable once GenericEnumeratorNotDisposed
            using var iterator = values.GetEnumerator();
            if (!iterator.MoveNext()) yield break;

            var lhs = iterator.Current;
            while (iterator.MoveNext())
            {
                yield return selector.Invoke(lhs, iterator.Current);
                lhs = iterator.Current;
            }
        }

        /// <summary>
        ///     Perform select operation where the data is returned as (n, n+1), (n+2, n+3), ... pairs. This yields N / 2 results
        ///     and fails if the data count is not even
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="values"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<T2> SelectInPairs<T1, T2>(this IEnumerable<T1> values, Func<T1, T1, T2> selector)
        {
            // ReSharper disable once GenericEnumeratorNotDisposed
            using var iterator = values.GetEnumerator();

            while (iterator.MoveNext())
            {
                var lhs = iterator.Current;
                if (!iterator.MoveNext()) throw new InvalidOperationException("The Enumerable does not contains an uneven number of entries");
                var rhs = iterator.Current;
                yield return selector.Invoke(lhs, rhs);
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
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source) => new ObservableCollection<T>(source);

        /// <summary>
        ///     Get an <see cref="ICollection{T}" /> from the provided <see cref="IEnumerable{T}" />. If the source does not
        ///     implement the interface a new collection is created by invoking the sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ICollection<T> AsCollection<T>(this IEnumerable<T> source) => source as ICollection<T> ?? source.ToList();

        /// <summary>
        ///     Get an <see cref="IList{T}" /> from the provided <see cref="IEnumerable{T}" />. If the source does not implement
        ///     the interface a new list is created by invoking the sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<T> AsList<T>(this IEnumerable<T> source) => source as IList<T> ?? source.ToList();

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
        
        /// <summary>
        ///     Splits an <see cref="IEnumerable{T}"/> into multiple sub enumerations of specified size where the last entry may me smaller
        /// </summary>
        /// <param name="self"></param>
        /// <param name="size"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> ToChunks<T>(this IEnumerable<T> self, int size)
        {
            IEnumerable<T> EnumerateChunk(IEnumerator<T> value)
            {
                for (var i = 0; i < size; i++)
                {
                    yield return value.Current;
                    if (!value.MoveNext()) yield break;
                }
            }
            using var enumerator = self.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return EnumerateChunk(enumerator);
            }
        }
    }
}