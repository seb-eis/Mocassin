using System;
using System.Collections;
using System.Collections.Generic;

namespace Mocassin.Framework.Collections
{
    /// <summary>
    ///     Generic abstract base class for sorted lists that support binary search operations
    /// </summary>
    public abstract class BinarySearchableList<T1> : IList<T1>
    {
        /// <summary>
        ///     The internally wrapped list of type T1
        /// </summary>
        public List<T1> List { get; protected set; }

        /// <summary>
        ///     The used comparer of type T1 to sort the entries
        /// </summary>
        public IComparer<T1> Comparer { get; protected set; }

        /// <inheritdoc />
        public int Count => List.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public T1 this[int index]
        {
            get => List[index];
            set => List[index] = value;
        }

        /// <summary>
        ///     Creates a new binary searchable list with the specified comparer
        /// </summary>
        /// <param name="comparer"></param>
        protected BinarySearchableList(IComparer<T1> comparer)
        {
            List = new List<T1>();
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        /// <summary>
        ///     Creates a new binary searchable list with the specified comparer and capacity
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="capacity"></param>
        protected BinarySearchableList(IComparer<T1> comparer, int capacity)
        {
            List = new List<T1>(capacity);
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        /// <inheritdoc />
        public void Clear()
        {
            List.Clear();
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        /// <inheritdoc />
        public bool Contains(T1 item)
        {
            return List.BinarySearch(item, Comparer) >= 0;
        }

        /// <inheritdoc />
        public void CopyTo(T1[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<T1> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        /// <summary>
        ///     Calls excess trim on the internal list object
        /// </summary>
        public void TrimExcess()
        {
            List.TrimExcess();
        }

        /// <inheritdoc />
        public abstract int IndexOf(T1 item);

        /// <inheritdoc />
        public abstract void Insert(int index, T1 item);

        /// <inheritdoc />
        public abstract void Add(T1 item);

        /// <inheritdoc />
        public abstract bool Remove(T1 item);

        /// <summary>
        ///     C++ Style lower bound binary search for sorted (provided comparer) collections: O log(n) complexity search of first
        ///     entry that does not compare less than value
        /// </summary>
        public int GetCppLowerBound(T1 item)
        {
            var (currentIndex, firstIndex, lastIndex, step, counter) = (0, 0, List.Count, 0, List.Count);
            while (counter > 0)
            {
                step = counter / 2;
                currentIndex = firstIndex + step;
                if (Comparer.Compare(List[currentIndex], item) == -1)
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
        ///     C++ Style upper bound binary search for sorted (provided comparer) collections: O log(n) complexity search of first
        ///     entry that does not compare less than value
        /// </summary>
        public int GetCppUpperBound(T1 item)
        {
            var (currentIndex, firstIndex, lastIndex, step, counter) = (0, 0, List.Count, 0, List.Count);
            while (counter > 0)
            {
                step = counter / 2;
                currentIndex = firstIndex + step;
                if (Comparer.Compare(item, List[currentIndex]) > -1)
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
        ///     C++ style lower and upper bound binary range search for sorted (provided comparer) collections: 2* O log(n) search
        ///     for first entry not lesser and first entry greater than value
        /// </summary>
        public (int Start, int End) GetCppEnclosingBounds(T1 item)
        {
            return (GetCppLowerBound(item), GetCppUpperBound(item));
        }

        /// <summary>
        ///     Adds multiple new entries from a sequence
        /// </summary>
        /// <param name="source"></param>
        public void Add(IEnumerable<T1> source)
        {
            foreach (var item in source) Add(item);
        }

        /// <summary>
        ///     Add multiple entries from a params set
        /// </summary>
        /// <param name="source"></param>
        public void Add(params T1[] source)
        {
            Add((IEnumerable<T1>) source);
        }
    }
}