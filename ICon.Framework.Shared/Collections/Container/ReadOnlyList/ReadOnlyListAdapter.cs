using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mocassin.Framework.Collections
{
    /// <summary>
    /// Emulates a read only behavior of a list while providing the generic IList interface
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class ReadOnlyListAdapter<T1> : IList<T1>, IReadOnlyList<T1>
    {
        /// <summary>
        /// The encapsulated list interface
        /// </summary>
        private IList<T1> Data { get; set; }

        /// <inheritdoc cref="IReadOnlyCollection{T}.Count"/>
        public int Count => Data.Count;

        /// <inheritdoc />
        public bool IsReadOnly =>true;

        /// <inheritdoc cref="IReadOnlyList{T}.this"/>
        public T1 this[int index]
        {
            get => Data[index];
            set => throw new NotSupportedException("List is read only");
        }

        /// <summary>
        /// Create new read only list
        /// </summary>
        public ReadOnlyListAdapter()
        {
            Data = new List<T1>();
        }

        /// <inheritdoc />
        public int IndexOf(T1 item)
        {
            return Data.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, T1 item)
        {
            throw new NotSupportedException("List is read only");
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            throw new NotSupportedException("List is read only");
        }

        /// <inheritdoc />
        public void Add(T1 item)
        {
            throw new NotSupportedException("List is read only");
        }

        /// <inheritdoc />
        public void Clear()
        {
            throw new NotSupportedException("List is read only");
        }

        /// <inheritdoc />
        public bool Contains(T1 item)
        {
            return Data.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T1[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(T1 item)
        {
            throw new NotSupportedException("List is read only");
        }

        /// <inheritdoc />
        public IEnumerator<T1> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        /// <inheritdoc />
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
        public static ReadOnlyListAdapter<T1> FromEnumerable<T2>(IEnumerable<T2> list) where T2 : T1
        {
            return new ReadOnlyListAdapter<T1>() { Data = list.Cast<T1>().ToList() };
        }
    }
}
