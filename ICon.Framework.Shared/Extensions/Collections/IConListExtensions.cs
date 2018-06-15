using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Collections;

namespace ICon.Framework.Extensions
{
    /// <summary>
    /// ICon extension class for objects that implement the generic IList interface, provides e.g. extended C++ style search functionality
    /// </summary>
    public static class IConListExtensions
    {
        /// <summary>
        /// C++ Style lower bound binary search for sorted (provided comparer) collections: O log(n) complexity search of first entry that does not compare less than value (Returns Count if non found)
        /// </summary>
        /// <typeparam name="List"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="Comp"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int CppFindLowerBound<List, T1, Comp>(this List list, T1 value, Comp comparer) where List : IList<T1> where Comp : IComparer<T1>
        {
            var (currentIndex, firstIndex, lastIndex, step, counter) = (0, 0, list.Count, 0, list.Count);
            while (counter > 0)
            {
                step = counter / 2;
                currentIndex = firstIndex + step;
                if (comparer.Compare(list[currentIndex], value) == -1)
                {
                    firstIndex = ++currentIndex;
                    counter -= step + 1;
                }
                else
                {
                    counter = step;
                }
            }
            return firstIndex;
        }

        /// <summary>
        /// C++ Style lower bound binary search for sorted (default comparer) collections: O log(n) complexity search of first entry that does not compare less than value (Returns Count if non found)
        /// </summary>
        /// <typeparam name="List"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32 CppFindLowerBound<List, T1>(this List list, T1 value) where List : IList<T1>
        {
            return list.CppFindLowerBound(value, Comparer<T1>.Default);
        }

        /// <summary>
        /// C++ Style upper bound binary search for sorted (provided comparer) collections: O log(n) complexity search of first entry that does not compare less than value (Returns Count if non found)
        /// </summary>
        /// <typeparam name="List"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="Comp"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static int CppFindUpperBound<List, T1, Comp>(this List list, T1 value, Comp comparer) where List : IList<T1> where Comp : IComparer<T1>
        {
            var (currentIndex, firstIndex, lastIndex, step, counter) = (0, 0, list.Count, 0, list.Count);
            while (counter > 0)
            {
                step = counter / 2;
                currentIndex = firstIndex + step;
                if (comparer.Compare(value, list[currentIndex]) > -1)
                {
                    firstIndex = ++currentIndex;
                    counter -= step + 1;
                }
                else
                {
                    counter = step;
                }
            }
            return firstIndex;
        }

        /// <summary>
        /// C++ Style upper bound binary search for sorted (default comparer) collections: O log(n) complexity search of first entry that does compare greater than value (Returns Count if non found)
        /// </summary>
        /// <typeparam name="List"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CppFindUpperBound<List, T1>(this List list, T1 value) where List : IList<T1>
        {
            return list.CppFindUpperBound(value, Comparer<T1>.Default);
        }

        /// <summary>
        /// C++ style lower and upper bound binary range search for sorted (provided comparer) collections: 2* O log(n) search for first entry not lesser and first entry greater than value (Returns Count for non existing indexes)
        /// </summary>
        /// <typeparam name="List"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="Comp"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static (int Start, int End) CppFindEnclosingBounds<List, T1, Comp>(this List list, T1 value, Comp comparer) where List : IList<T1> where Comp : IComparer<T1>
        {
            return (list.CppFindLowerBound(value, comparer), list.CppFindUpperBound(value, comparer));
        }

        /// <summary>
        /// C++ style lower and upper bound binary range search for sorted (default comparer) collections: 2* O log(n) search for first entry not lesser and first entry greater than value (Returns Count if non are found)
        /// </summary>
        /// <typeparam name="List"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static (int Start, int End) CppFindEnclosingBounds<List, T1>(this List list, T1 value) where List : IList<T1>
        {
            return list.CppFindEnclosingBounds(value, Comparer<T1>.Default);
        }

        /// <summary>
        /// Creates a new sorted multiset list from an IEnumerable<T1> interface with the default comparer of that type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static MultisetList<T1> ToMultisetList<T1>(this IEnumerable<T1> enumerable)
        {
            return enumerable.ToMultisetList(Comparer<T1>.Default);
        }

        /// <summary>
        /// Creates a new sorted multiset list from an IEnumerable<T1> interface with the provided comparer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="Comp"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static MultisetList<T1> ToMultisetList<T1, Comp>(this IEnumerable<T1> enumerable, Comp comparer) where Comp : IComparer<T1>
        {
            var multisetList = new MultisetList<T1>(comparer);
            foreach (T1 item in enumerable)
            {
                multisetList.List.Add(item);
            }
            multisetList.List.Sort(comparer);
            return multisetList;
        }

        /// <summary>
        /// Creates a new sorted dublicate free set list from IEnumerable<T1> interface and default comparer of type T1
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static SetList<T1> ToSetList<T1>(this IEnumerable<T1> enumerable)
        {
            return enumerable.ToSetList(Comparer<T1>.Default);
        }

        /// <summary>
        /// Creates a new sorted dublicate free set list from IEnumerable<T1> interface and the provided comparer of that type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="Comp"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static SetList<T1> ToSetList<Comp, T1>(this IEnumerable<T1> enumerable, Comp comparer) where Comp : IComparer<T1>
        {
            var setList = new SetList<T1>(comparer);
            foreach (T1 item in enumerable)
            {
                setList.Add(item);
            }
            return setList;
        }

        /// <summary>
        /// Replaces the first element matching the predicate and returns the index, if non is found list.Add() is called and the new last index is returned
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int ReplaceFirstOrAdd<T1>(this IList<T1> list, Predicate<T1> predicate, T1 item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    list[i] = item;
                    return i;
                }
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
            int removed = 0;
            for (int i = list.Count - 1; i > 0; --i)
            {
                if (predicate(list[i]))
                {
                    list.RemoveAt(i);
                    ++removed;
                }
            }
            return removed;
        }
    }
}
