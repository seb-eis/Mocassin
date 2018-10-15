using System;
using System.Collections.Generic;
using System.Linq;

using ICon.Framework.Collections;

namespace ICon.Framework.Extensions
{
    /// <summary>
    /// Extension class for objects that implement the generic IList interface, provides e.g. extended C++ style search functionality
    /// </summary>
    public static class MocassinListExtensions
    {
        /// <summary>
        /// C++ Style lower bound binary search for sorted (provided comparer) collections:
        /// O log(n) complexity search of first entry that does not compare less than value (Returns Count if non found)
        /// </summary>
        /// <typeparam name="TList"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TComp"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int CppLowerBound<TList, TValue, TComp>(this TList list, TValue value, TComp comparer)
            where TList : IList<TValue>
            where TComp : IComparer<TValue>
        {
            var (firstIndex, counter) = (0, list.Count);
            while (counter > 0)
            {
                var step = counter / 2;
                var currentIndex = firstIndex + step;
                if (comparer.Compare(list[currentIndex], value) == -1)
                {
                    firstIndex = ++currentIndex;
                    counter -= step + 1;
                }
                else
                    counter = step;
            }
            return firstIndex;
        }

        /// <summary>
        /// C++ Style lower bound binary search for sorted (default comparer) collections:
        /// O log(n) complexity search of first entry that does not compare less than value (Returns Count if non found)
        /// </summary>
        /// <typeparam name="TList"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CppLowerBound<TList, TValue>(this TList list, TValue value)
            where TList : IList<TValue>
        {
            return list.CppLowerBound(value, Comparer<TValue>.Default);
        }

        /// <summary>
        /// C++ Style upper bound binary search for sorted (provided comparer) collections:
        /// O log(n) complexity search of first entry that does not compare less than value (Returns Count if non found)
        /// </summary>
        /// <typeparam name="TList"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TComp"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int CppUpperBound<TList, TValue, TComp>(this TList list, TValue value, TComp comparer)
            where TList : IList<TValue> 
            where TComp : IComparer<TValue>
        {
            var (firstIndex, counter) = (0, list.Count);
            while (counter > 0)
            {
                var step = counter / 2;
                var currentIndex = firstIndex + step;
                if (comparer.Compare(value, list[currentIndex]) > -1)
                {
                    firstIndex = ++currentIndex;
                    counter -= step + 1;
                }
                else
                    counter = step;
            }
            return firstIndex;
        }

        /// <summary>
        /// C++ Style upper bound binary search for sorted (default comparer) collections:
        /// O log(n) complexity search of first entry that does compare greater than value (Returns Count if non found)
        /// </summary>
        /// <typeparam name="TList"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CppUpperBound<TList, TValue>(this TList list, TValue value) where TList : IList<TValue>
        {
            return list.CppUpperBound(value, Comparer<TValue>.Default);
        }

        /// <summary>
        /// C++ style lower and upper bound binary range search for sorted (provided comparer) collections:
        /// 2* O log(n) search for first entry not lesser and first entry greater than value (Returns Count for non existing indexes)
        /// </summary>
        /// <typeparam name="TList"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TComp"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static (int Start, int End) CppEnclosingBounds<TList, TValue, TComp>(this TList list, TValue value, TComp comparer)
            where TList : IList<TValue>
            where TComp : IComparer<TValue>
        {
            return (list.CppLowerBound(value, comparer), list.CppUpperBound(value, comparer));
        }

        /// <summary>
        /// C++ style lower and upper bound binary range search for sorted (default comparer) collections:
        /// 2* O log(n) search for first entry not lesser and first entry greater than value (Returns Count if non are found)
        /// </summary>
        /// <typeparam name="TList"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static (int Start, int End) CppEnclosingBounds<TList, TValue>(this TList list, TValue value)
            where TList : IList<TValue>
        {
            return list.CppEnclosingBounds(value, Comparer<TValue>.Default);
        }

        /// <summary>
        /// Creates a new sorted multiset list from an IEnumerable interface with the default comparer of that type
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static MultisetList<TValue> ToMultisetList<TValue>(this IEnumerable<TValue> enumerable)
        {
            return enumerable.ToMultisetList(Comparer<TValue>.Default);
        }

        /// <summary>
        /// Creates a new sorted multiset list from an IEnumerable interface with the provided comparer
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TComp"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static MultisetList<TValue> ToMultisetList<TValue, TComp>(this IEnumerable<TValue> enumerable, TComp comparer)
            where TComp : IComparer<TValue>
        {
            var multisetList = new MultisetList<TValue>(comparer);
            foreach (var item in enumerable) multisetList.List.Add(item);
            multisetList.List.Sort(comparer);
            return multisetList;
        }

        /// <summary>
        /// Creates a new sorted duplicate free set list from IEnumerable interface and default comparer of type T1
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static SetList<T1> ToSetList<T1>(this IEnumerable<T1> enumerable)
        {
            return enumerable.ToSetList(Comparer<T1>.Default);
        }

        /// <summary>
        /// Creates a new sorted duplicate free set list from IEnumerable interface and the provided comparer of that type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TComp"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static SetList<T1> ToSetList<TComp, T1>(this IEnumerable<T1> enumerable, TComp comparer)
            where TComp : IComparer<T1>
        {
            var setList = new SetList<T1>(comparer);
            foreach (var item in enumerable)
                setList.Add(item);

            return setList;
        }

        /// <summary>
        /// Replaces the first element matching the predicate and returns the index, if non is found list.Add()
        /// is called and the new last index is returned
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int ReplaceFirstOrAdd<T1>(this IList<T1> list, Predicate<T1> predicate, T1 item)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (!predicate(list[i]))
                    continue;

                list[i] = item;
                return i;
            }
            list.Add(item);
            return list.Count - 1;
        }

        /// <summary>
        /// RemoveAll implementation of the default list for the list interface that removes all items that compare to true in the defined predicate
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int RemoveAll<T1>(this IList<T1> list, Predicate<T1> predicate)
        {
            var removed = 0;
            for (var i = list.Count - 1; i > 0; --i)
            {
                if (!predicate(list[i]))
                    continue;

                list.RemoveAt(i);
                ++removed;
            }
            return removed;
        }

        /// <summary>
        /// Gets the sequence equality direction for list one to the second. Returns 1 for positive or -1 for inverted direction. Retruns 0 if sequences are not equal in
        /// either direction
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int GetSequenceEqualityDirectionTo<T1>(this IList<T1> lhs, IList<T1> rhs, IEqualityComparer<T1> comparer)
        {
            if (lhs.Count != rhs.Count)
                return 0;

            if (ReferenceEquals(lhs, rhs)) 
                return 1;

            if (lhs.SequenceEqual(rhs, comparer))
                return 1;

            var rhsIndex = rhs.Count;
            foreach (var value in lhs)
            {
                if (!comparer.Equals(value, rhs[--rhsIndex]))
                    return 0;
            }
            return -1;
        }

        /// <summary>
        /// Removes all duplicates from a list based upon the provided equality comparer and returns removed indices
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <param name="comparer"></param>
        public static IEnumerable<int> RemoveDuplicates<T1>(this IList<T1> list, IEqualityComparer<T1> comparer)
        {
            for (var i = 0; i < list.Count - 1; i++)
            {
                for (var j = list.Count - 1; j > i; j--)
                {
                    if (!comparer.Equals(list[i], list[j]))
                        continue;

                    list.RemoveAt(j);
                    yield return j;
                }
            }
        }

        /// <summary>
        /// Populates a generic list with a default value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TList"></typeparam>
        /// <param name="values"></param>
        /// <param name="value"></param>
        /// <param name="counts"></param>
        /// <returns></returns>
        public static TList Populate<TList,T1>(this TList values, T1 value, int counts) where TList : IList<T1>
        {
            values.Clear();
            for (var i = 0; i < counts; i++)
                values.Add(value);

            return values;
        }

        /// <summary>
        /// Populates a generic list from a provider function to the specified size
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TList"></typeparam>
        /// <param name="list"></param>
        /// <param name="provider"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static TList Populate<TList,T1>(this TList list, Func<T1> provider, int count) where TList : IList<T1>
        {
            list.Clear();
            for (var i = 0; i < count; i++)
                list.Add(provider());

            return list;
        }

        /// <summary>
        /// Shuffles the entries of a list multiple times using the provided random number generator
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <param name="random"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IList<T1> Shuffle<T1>(this IList<T1> list, System.Random random, int count)
        {
            for (var i = 0; i < count; i++)
            {
                for (var j = list.Count; j > 1;) list.Swap(random.Next(j), --j);
            }
            return list;
        }

        /// <summary>
        /// Swap the values at the provided indices within a list
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static void Swap<T1>(this IList<T1> list, int lhs, int rhs)
        {
            var tmp = list[lhs];
            list[lhs] = list[rhs];
            list[rhs] = tmp;
        }

        /// <summary>
        /// Selects entries from an index-able list interface in the order of a passed indexing
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        public static IEnumerable<T1> SelectByIndexing<T1>(this IList<T1> list, IEnumerable<int> indices)
        {
            return indices.Select(index => list[index]);
        }

        /// <summary>
        /// Selects entries from an index-able list interface in the order of a passed indexing
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        public static IEnumerable<T1> SelectByIndexing<T1>(this IReadOnlyList<T1> list, IEnumerable<int> indices)
        {
            return indices.Select(index => list[index]);
        }
    }
}
