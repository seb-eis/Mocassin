using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ICon.Framework.Collections
{
    /// <summary>
    /// Read only wrapper for a list interface to restrict access to it but still be compatible with functions expecting a list interface
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class ReadOnlyList<T1> : IList<T1>, IReadOnlyList<T1>
    {
        /// <summary>
        /// The encapsulated list interface
        /// </summary>
        private IList<T1> Data { get; set; }

        /// <summary>
        /// Get entry count
        /// </summary>
        public int Count => Data.Count;

        /// <summary>
        /// Get read only flag (always true)
        /// </summary>
        public bool IsReadOnly =>true;

        /// <summary>
        /// Acccess by index, set always throws exception
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T1 this[int index]
        {
            get { return Data[index]; }
            set { throw new NotSupportedException("List is read only"); }
        }

        /// <summary>
        /// Create new read only list
        /// </summary>
        public ReadOnlyList()
        {
            Data = new List<T1>();
        }

        /// <summary>
        /// Find the index of an item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T1 item)
        {
            return Data.IndexOf(item);
        }

        /// <summary>
        /// Insert is not supported (Read only)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T1 item)
        {
            throw new NotSupportedException("List is read only");
        }

        /// <summary>
        /// Remove is not supported (Read only)
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException("List is read only");
        }

        /// <summary>
        /// Add is not supported (Read only)
        /// </summary>
        /// <param name="item"></param>
        public void Add(T1 item)
        {
            throw new NotSupportedException("List is read only");
        }

        /// <summary>
        /// Clear is not supported (Read only)
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException("List is read only");
        }

        /// <summary>
        /// Returns true if the entry is contained
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T1 item)
        {
            return Data.Contains(item);
        }

        /// <summary>
        /// Copies data to array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T1[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Remove is not supported (Read only)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T1 item)
        {
            throw new NotSupportedException("List is read only");
        }

        /// <summary>
        /// Get the enumarator of the wrapped list
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T1> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        /// <summary>
        /// Get the enumerator of the wrapped list
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        /// <summary>
        /// Creates read only list from an enumerable of potentially derived type
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ReadOnlyList<T1> FromEnumerable<T2>(IEnumerable<T2> list) where T2 : T1
        {
            return new ReadOnlyList<T1>() { Data = list.Cast<T1>().ToList() };
        }
    }
}
