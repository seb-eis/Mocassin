using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Extensions;

namespace ICon.Framework.Collections
{
    /// <summary>
    /// Wrapped list that is always sorted utilizing the provided comparer and does not allow multiple entries that compare equal to each other
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class SetList<T1> : AutoSortList<T1>
    {
        /// <summary>
        /// Creates new set list that uses the default comparer
        /// </summary>
        public SetList() : base(Comparer<T1>.Default)
        {

        }

        /// <summary>
        /// Creates a new setlist with the specified comparer
        /// </summary>
        /// <param name="comparer"></param>
        public SetList(IComparer<T1> comparer) : base(comparer)
        {
        }

        /// <summary>
        /// Creates a new setlist with the specified comparer and specified capacity
        /// </summary>
        /// <param name="comparer"></param>
        public SetList(IComparer<T1> comparer, Int32 capacity) : base(comparer, capacity)
        {
        }

        /// <summary>
        /// Finds the index of the provided item with O(log n) time
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override Int32 IndexOf(T1 item)
        {
            return List.BinarySearch(item, Comparer);
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
        /// Adds an entry to the list, always inserts at the first value that does not compare smaller than item and only if this value is not equal to item
        /// </summary>
        /// <param name="item"></param>
        public override void Add(T1 item)
        {
            Int32 target = CppFindLowerBound(item);
            if (target == Count)
            {
                List.Add(item);
            }
            if (Comparer.Compare(List[target], item) != 0)
            {
                List.Insert(target, item);
            }
        }

        /// <summary>
        /// Removes an item if it can be found in the list, returns false if non is found
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override Boolean Remove(T1 item)
        {
            Int32 target = List.BinarySearch(item);
            if (target >= 0)
            {
                List.RemoveAt(target);
                return true;
            }
            return false;
        }
    }
}

