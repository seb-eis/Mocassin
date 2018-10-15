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
    public class MultisetList<T1> : BinarySearchableList<T1>
    {
        /// <inheritdoc />
        public MultisetList(IComparer<T1> comparer) :  base(comparer)
        {
        }

        /// <inheritdoc />
        public MultisetList(IComparer<T1> comparer, int capacity) :  base(comparer, capacity)
        {
        }

        /// <inheritdoc />
        public override void Insert(int index, T1 item)
        {
            Add(item);
        }

        /// <inheritdoc />
        public override void Add(T1 item)
        {
            List.Insert(GetCppLowerBound(item), item);
        }

        /// <inheritdoc />
        public override bool Remove(T1 item)
        {
            var target = GetCppLowerBound(item);
            if (Comparer.Compare(List[target], item) != 0)
                return false;

            List.RemoveAt(target);
            return true;
        }

        /// <inheritdoc />
        public override int IndexOf(T1 item)
        {
            var index = GetCppLowerBound(item);
            return Comparer.Compare(List[index], item) == 0 ? index : -1;
        }
    }
}
