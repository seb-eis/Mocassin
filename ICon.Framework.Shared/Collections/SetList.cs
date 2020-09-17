using System.Collections.Generic;

namespace Mocassin.Framework.Collections
{
    /// <summary>
    ///     Wrapped list that is always sorted utilizing the provided comparer and does not allow multiple entries that compare
    ///     equal to each other
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class SetList<T1> : BinarySearchableList<T1>
    {
        /// <inheritdoc />
        public SetList()
            : base(Comparer<T1>.Default)
        {
        }

        /// <inheritdoc />
        public SetList(IComparer<T1> comparer)
            : base(comparer)
        {
        }

        /// <inheritdoc />
        public SetList(IComparer<T1> comparer, int capacity)
            : base(comparer, capacity)
        {
        }

        /// <inheritdoc />
        public override int IndexOf(T1 item) => List.BinarySearch(item, Comparer);

        /// <inheritdoc />
        public override void Insert(int index, T1 item)
        {
            Add(item);
        }

        /// <inheritdoc />
        public override void Add(T1 item)
        {
            var target = GetCppLowerBound(item);
            if (target == Count)
                List.Add(item);

            if (Comparer.Compare(List[target], item) != 0)
                List.Insert(target, item);
        }

        /// <inheritdoc />
        public override bool Remove(T1 item)
        {
            var target = List.BinarySearch(item);
            if (target < 0)
                return false;

            List.RemoveAt(target);
            return true;
        }
    }
}