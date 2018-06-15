using System;
using System.Collections;
using System.Collections.Generic;

namespace ICon.Framework.Collections
{
    /// <summary>
    /// Adapter that emulates a generic list interface for a non generic list interface (Avoids casting issues of list interfaces to list interfaces of derived types)
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class ListInterfaceAdapter<T1> : IList<T1>
    {
        /// <summary>
        /// The encapsulated non generic list interface
        /// </summary>
        protected IList List { get; set; }

        /// <summary>
        /// Get the number of elements in the list
        /// </summary>
        public int Count => List.Count;

        /// <summary>
        /// Get the info if the list is read only
        /// </summary>
        public bool IsReadOnly => List.IsReadOnly;

        /// <summary>
        /// Access list by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T1 this[int index]
        {
            get { return (T1)List[index]; }
            set { List[index] = value; }
        }

        /// <summary>
        /// Creates new generic list interface adapter for a non generic list
        /// </summary>
        /// <param name="list"></param>
        public ListInterfaceAdapter(IList list)
        {
            List = list ?? throw new ArgumentNullException(nameof(list));
        }

        /// <summary>
        /// Find an item in the list
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T1 item)
        {
            return List.IndexOf(item);
        }

        /// <summary>
        /// Insert an item at the speciffied index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T1 item)
        {
            List.Insert(index, item);
        }

        /// <summary>
        /// Remove item at specififed item
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        /// <summary>
        /// Add item to the list
        /// </summary>
        /// <param name="item"></param>
        public void Add(T1 item)
        {
            List.Add(item);
        }

        /// <summary>
        /// Clear the list
        /// </summary>
        public void Clear()
        {
            List.Clear();
        }

        /// <summary>
        /// Check if the list constains a specific item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T1 item)
        {
            return List.Contains(item);
        }

        /// <summary>
        /// Copies list to an array starting at the provided array index
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T1[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes item from the list, returns true if an item was removed
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T1 item)
        {
            int oldCount = List.Count;
            List.Remove(item);
            return oldCount != List.Count;
        }

        /// <summary>
        /// Get enumerator for the list
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T1> GetEnumerator()
        {
            foreach (var item in List)
            {
                yield return (T1)item;
            }
        }

        /// <summary>
        /// Get non generic enumerator for the list
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }
    }
}
