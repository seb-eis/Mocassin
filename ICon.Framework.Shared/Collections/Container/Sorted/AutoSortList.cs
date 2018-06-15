using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.Collections
{
    /// <summary>
    /// Generic abstract base class for all containers that ensure that the contents are always ordered and searchable by binary methods
    /// </summary>
    public abstract class AutoSortList<T1> : IList<T1>
    {
        /// <summary>
        /// The internally wrapped list of type T1
        /// </summary>
        public List<T1> List { get; protected set; }

        /// <summary>
        /// The used comparer of type T1 to sort the entries
        /// </summary>
        public IComparer<T1> Comparer { get; protected set; }

        /// <summary>
        /// Get the number of entries in the stored list
        /// </summary>
        public int Count => List.Count;

        /// <summary>
        /// Returns if the list is readonly
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Access the stored list by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T1 this[int index] { get { return List[index]; } set { List[index] = value; } }

        /// <summary>
        /// Creates a new multiset list with the specified comparer
        /// </summary>
        /// <param name="comparer"></param>
        protected AutoSortList(IComparer<T1> comparer)
        {
            List = new List<T1>();
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        /// <summary>
        /// Creates a new multiset list with the specified comparer and specified capacity
        /// </summary>
        /// <param name="comparer"></param>
        protected AutoSortList(IComparer<T1> comparer, int capacity)
        {
            List = new List<T1>(capacity);
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        /// <summary>
        /// Clears the internal list
        /// </summary>
        public void Clear()
        {
            List.Clear();
        }

        /// <summary>
        /// Removes the element at the provided index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        /// <summary>
        /// Checks if an item can be found in the list
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T1 item)
        {
            return List.BinarySearch(item, Comparer) >= 0;
        }

        /// <summary>
        /// Copies list elements to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T1[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns generic enumerator of the list
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T1> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        /// <summary>
        /// Returns non-generic enumerator of the list
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        /// <summary>
        /// Calls excess trim on the internal list object
        /// </summary>
        public void TrimExcess()
        {
            List.TrimExcess();
        }

        /// <summary>
        /// Fins the index of the provided element
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public abstract int IndexOf(T1 item);

        /// <summary>
        /// Inserts an element, index is usually ignored in sorted lists
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public abstract void Insert(int index, T1 item);

        /// <summary>
        /// Adds an element under preservation of sorting order
        /// </summary>
        /// <param name="item"></param>
        public abstract void Add(T1 item);

        /// <summary>
        /// Removes an item or all items from the list depending on implementing class
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public abstract bool Remove(T1 item);

        /// <summary>
        /// C++ Style lower bound binary search for sorted (provided comparer) collections: O log(n) complexity search of first entry that does not compare less than value
        /// </summary>
        public int CppFindLowerBound(T1 item) 
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
                {
                    counter = step;
                }
            }
            return firstIndex;
        }

        /// <summary>
        /// C++ Style upper bound binary search for sorted (provided comparer) collections: O log(n) complexity search of first entry that does not compare less than value
        /// </summary>
        public int CppFindUpperBound(T1 item)
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
                {
                    counter = step;
                }
            }
            return firstIndex;
        }

        /// <summary>
        /// C++ style lower and upper bound binary range search for sorted (provided comparer) collections: 2* O log(n) search for first entry not lesser and first entry greater than value
        /// </summary>
        public (int Start, int End) CppFindEnclosingBounds(T1 item)
        {
            return (CppFindLowerBound(item), CppFindUpperBound(item));
        }

        /// <summary>
        /// Adds multiple new entries from a sequence
        /// </summary>
        /// <param name="source"></param>
        public void Add(IEnumerable<T1> source)
        {
            foreach (var item in source)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Add multiple entries from a params set
        /// </summary>
        /// <param name="source"></param>
        public void Add(params T1[] source)
        {
            Add((IEnumerable<T1>)source);
        }
    }
}
