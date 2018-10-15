using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ICon.Framework.Collections
{
    /// <summary>
    /// Adapter that emulates a generic list interface for a non generic list
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <remarks> Circumvents the C# invalid cast problem of list interfaces to list interfaces of derived type </remarks>
    public class GenericListAdapter<T1> : IList<T1>
    {
        /// <summary>
        /// The encapsulated non generic list interface
        /// </summary>
        protected IList List { get; set; }

        /// <inheritdoc />
        public int Count => List.Count;

        /// <inheritdoc />
        public bool IsReadOnly => List.IsReadOnly;

        /// <inheritdoc />
        public T1 this[int index]
        {
            get => (T1)List[index];
            set => List[index] = value;
        }

        /// <inheritdoc />
        public GenericListAdapter(IList list)
        {
            List = list ?? throw new ArgumentNullException(nameof(list));
        }

        /// <inheritdoc />
        public int IndexOf(T1 item)
        {
            return List.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, T1 item)
        {
            List.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        /// <inheritdoc />
        public void Add(T1 item)
        {
            List.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            List.Clear();
        }

        /// <inheritdoc />
        public bool Contains(T1 item)
        {
            return List.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T1[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(T1 item)
        {
            var oldCount = List.Count;
            List.Remove(item);
            return oldCount != List.Count;
        }

        /// <inheritdoc />
        public IEnumerator<T1> GetEnumerator()
        {
            return List.Cast<T1>().GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }
    }
}
