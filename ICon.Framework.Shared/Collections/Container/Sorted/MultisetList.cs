using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Extensions;

namespace ICon.Framework.Collections
{
    /// <summary>
    /// Wrapped list that is always sorted utilizing the provided comparer and allows multiple entries that compare equal to each other
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class MultisetList<T1> : AutoSortList<T1>
    {
        /// <summary>
        /// Creates a new multiset list with the specified comparer
        /// </summary>
        /// <param name="comparer"></param>
        public MultisetList(IComparer<T1> comparer) :  base(comparer)
        {
        }

        /// <summary>
        /// Creates a new multiset list with the specified comparer and specified capacity
        /// </summary>
        /// <param name="comparer"></param>
        public MultisetList(IComparer<T1> comparer, Int32 capacity) :  base(comparer, capacity)
        {
        }

        /// <summary>
        /// Adds and item to the list (The index is ignored as it does not make sense for a sorted list)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public override void Insert(Int32 index, T1 item)
        {
            Add(item);
        }

        /// <summary>
        /// Adds an entry to the list, always inserts at the first value that does not compare smaller than value
        /// </summary>
        /// <param name="item"></param>
        public override void Add(T1 item)
        {
            List.Insert(CppFindLowerBound(item), item);
        }

        /// <summary>
        /// Removes the first occurence of the item from the list, returns false if non is found
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override Boolean Remove(T1 item)
        {
            Int32 target = CppFindLowerBound(item);
            if (Comparer.Compare(List[target], item) == 0)
            {
                List.RemoveAt(target);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Finds the first occurence of the item, returns -1 if non is found
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override Int32 IndexOf(T1 item)
        {
            Int32 index = CppFindLowerBound(item);
            return Comparer.Compare(List[index], item) == 0 ? index : -1;
        }
    }
}
