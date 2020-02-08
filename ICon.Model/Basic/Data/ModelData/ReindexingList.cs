using System.Collections;
using System.Collections.Generic;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Carries reindexing information for the contents of a model data container after deprecated data was removed as
    ///     pairs of old and new index (-1 means removed)
    /// </summary>
    public class ReindexingList : IList<(int Old, int New)>
    {
        /// <summary>
        ///     Contains the actual reindexing info as pairs of new and old index
        /// </summary>
        public List<(int Old, int New)> Data;

        /// <inheritdoc />
        public (int Old, int New) this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        /// <inheritdoc />
        public int Count => Data.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <summary>
        ///     Creates new reindexing list with an initial capacity of 0
        /// </summary>
        public ReindexingList()
            : this(0)
        {
        }

        /// <summary>
        ///     Creates new list with specified start capacity
        /// </summary>
        /// <param name="capacity"></param>
        public ReindexingList(int capacity)
        {
            Data = new List<(int Old, int New)>(capacity);
        }

        /// <inheritdoc />
        public void Add((int Old, int New) item)
        {
            Data.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            Data.Clear();
        }

        /// <inheritdoc />
        public bool Contains((int Old, int New) item)
        {
            return Data.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo((int Old, int New)[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<(int Old, int New)> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        /// <inheritdoc />
        public int IndexOf((int Old, int New) item)
        {
            return Data.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, (int Old, int New) item)
        {
            Data.Insert(index, item);
        }

        /// <inheritdoc />
        public bool Remove((int Old, int New) item)
        {
            return Data.Remove(item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            Data.RemoveAt(index);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        /// <summary>
        ///     Set the internal list capacity to the actual number of entries
        /// </summary>
        public void TrimExcess()
        {
            Data.TrimExcess();
        }
    }
}